using DigitalDocumentPlatform.Implement.Services.Interface;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DigitalDocumentPlatform.Implement.Services
{
    public class LibreOfficePdfConverterService : IPdfConverterService
    {
        private readonly ILogger<LibreOfficePdfConverterService> _logger;
        private readonly string _libreOfficePath;
        private const int ConversionTimeoutSeconds = 300; // 5 minutes timeout

        // Semaphore to ensure only one LibreOffice conversion at a time (prevents conflicts)
        private static readonly SemaphoreSlim _conversionSemaphore = new SemaphoreSlim(1, 1);

        public LibreOfficePdfConverterService(ILogger<LibreOfficePdfConverterService> logger)
        {
            _logger = logger;
            _libreOfficePath = FindLibreOfficePath();

            // Kill any existing zombie LibreOffice processes on startup
            CleanupZombieProcesses();
        }

        public bool IsLibreOfficeAvailable()
        {
            return !string.IsNullOrEmpty(_libreOfficePath) && File.Exists(_libreOfficePath);
        }

        public async Task<string> ConvertToPdfAsync(string sourceFilePath, string? outputDirectory = null)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException($"Source file not found: {sourceFilePath}");
            }

            if (!IsLibreOfficeAvailable())
            {
                throw new InvalidOperationException("LibreOffice is not installed or not found. Please install LibreOffice from https://www.libreoffice.org/download/");
            }

            // Use temp folder in application root instead of system temp
            if (outputDirectory == null)
            {
                var appRootTemp = Path.Combine(AppContext.BaseDirectory, "temp");
                if (!Directory.Exists(appRootTemp))
                {
                    Directory.CreateDirectory(appRootTemp);
                }
                outputDirectory = appRootTemp;
            }

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Acquire semaphore to ensure only one conversion at a time
            _logger.LogInformation("Waiting for conversion semaphore...");
            await _conversionSemaphore.WaitAsync();

            try
            {
                _logger.LogInformation("Starting PDF conversion: {SourceFile}", sourceFilePath);

                // Create unique user profile in SYSTEM TEMP (not outputDirectory) to avoid
                // access-denied errors when LibreOffice creates its Documents sub-folder inside
                // a path that belongs to the application's working directory.
                var userProfilePath = Path.Combine(Path.GetTempPath(), $"lo_profile_{Guid.NewGuid():N}");
                Directory.CreateDirectory(userProfilePath);

                try
                {
                    // Determine correct export filter based on file extension
                    var fileExtension = Path.GetExtension(sourceFilePath).ToLowerInvariant();
                    var exportFilter = fileExtension switch
                    {
                        ".xlsx" or ".xls" or ".ods" or ".csv" => "calc_pdf_Export",
                        ".pptx" or ".ppt" or ".odp" => "impress_pdf_Export",
                        _ => "writer_pdf_Export" // .docx, .doc, .odt, etc.
                    };

                    var userProfileUri = "file:///" + userProfilePath.Replace("\\", "/").Replace(" ", "%20");

                    var arguments = $"--headless --norestore --nofirststartwizard --nolockcheck " +
                                  $"-env:UserInstallation=\"{userProfileUri}\" " +
                                  $"--convert-to pdf:{exportFilter} " +
                                  $"--outdir \"{outputDirectory}\" \"{sourceFilePath}\"";

                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = _libreOfficePath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        // Use the isolated profile path as WorkingDirectory so LibreOffice
                        // creates its 'Documents' sub-folder there, not inside outputDirectory.
                        WorkingDirectory = userProfilePath
                    };

                    // Set environment variables so LibreOffice can run under IIS/service accounts
                    processStartInfo.Environment["HOME"] = userProfilePath;
                    processStartInfo.Environment["USERPROFILE"] = userProfilePath;
                    processStartInfo.Environment["APPDATA"] = Path.Combine(userProfilePath, "AppData", "Roaming");
                    processStartInfo.Environment["LOCALAPPDATA"] = Path.Combine(userProfilePath, "AppData", "Local");
                    processStartInfo.Environment["TEMP"] = Path.Combine(userProfilePath, "Temp");
                    processStartInfo.Environment["TMP"] = Path.Combine(userProfilePath, "Temp");
                    processStartInfo.Environment["SAL_USE_VCLPLUGIN"] = "svp";

                    // Create necessary subdirectories for the profile
                    Directory.CreateDirectory(Path.Combine(userProfilePath, "AppData", "Roaming"));
                    Directory.CreateDirectory(Path.Combine(userProfilePath, "AppData", "Local"));
                    Directory.CreateDirectory(Path.Combine(userProfilePath, "Temp"));

                    using var process = new Process { StartInfo = processStartInfo };
                    var outputBuilder = new System.Text.StringBuilder();
                    var errorBuilder = new System.Text.StringBuilder();

                    process.OutputDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            outputBuilder.AppendLine(args.Data);
                            _logger.LogDebug("LibreOffice output: {Output}", args.Data);
                        }
                    };

                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            errorBuilder.AppendLine(args.Data);
                            _logger.LogWarning("LibreOffice error: {Error}", args.Data);
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    var completed = await Task.Run(() => process.WaitForExit(ConversionTimeoutSeconds * 1000));

                    if (!completed)
                    {
                        _logger.LogError("LibreOffice conversion timeout, attempting to kill process");
                        try
                        {
                            process.Kill(entireProcessTree: true);
                            await Task.Delay(1000); // Wait for cleanup
                        }
                        catch (Exception killEx)
                        {
                            _logger.LogError(killEx, "Failed to kill LibreOffice process");
                        }
                        throw new TimeoutException($"LibreOffice conversion timed out after {ConversionTimeoutSeconds} seconds");
                    }

                    // Wait for streams to finish
                    await process.WaitForExitAsync();

                    if (process.ExitCode != 0)
                    {
                        var errorMessage = errorBuilder.ToString();
                        _logger.LogError("LibreOffice conversion failed with exit code {ExitCode}. Error: {Error}",
                            process.ExitCode, errorMessage);
                        throw new InvalidOperationException($"LibreOffice conversion failed with exit code {process.ExitCode}. Error: {errorMessage}");
                    }

                    var sourceFileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath);
                    var pdfFilePath = Path.Combine(outputDirectory, $"{sourceFileNameWithoutExtension}.pdf");

                    // Wait for file to be written
                    int retries = 0;
                    while (!File.Exists(pdfFilePath) && retries < 10)
                    {
                        await Task.Delay(500);
                        retries++;
                    }

                    if (!File.Exists(pdfFilePath))
                    {
                        throw new FileNotFoundException($"PDF file was not created at expected location: {pdfFilePath}");
                    }

                    _logger.LogInformation("Successfully converted file to PDF: {PdfFile}", pdfFilePath);
                    return pdfFilePath;
                }
                finally
                {
                    // Cleanup user profile directory
                    try
                    {
                        if (Directory.Exists(userProfilePath))
                        {
                            Directory.Delete(userProfilePath, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to cleanup user profile directory: {ProfilePath}", userProfilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting file to PDF: {SourceFile}", sourceFilePath);
                throw;
            }
            finally
            {
                // Release semaphore
                _conversionSemaphore.Release();
                _logger.LogInformation("Released conversion semaphore");
            }
        }

        public async Task<byte[]> ConvertToPdfAsync(byte[] sourceContent, string sourceFileName)
        {
            if (sourceContent == null || sourceContent.Length == 0)
            {
                throw new ArgumentException("Source content cannot be null or empty", nameof(sourceContent));
            }

            if (string.IsNullOrWhiteSpace(sourceFileName))
            {
                throw new ArgumentException("Source file name cannot be null or empty", nameof(sourceFileName));
            }

            // Create temp directory in application root instead of system temp
#if DEBUG
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);

#elif RELEASE
            var appRootTemp = Path.Combine(AppContext.BaseDirectory, "temp");
            if (!Directory.Exists(appRootTemp))
            {
                Directory.CreateDirectory(appRootTemp);
            }

            var tempDirectory = Path.Combine(appRootTemp, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);

#endif

            try
            {
                var tempSourceFile = Path.Combine(tempDirectory, sourceFileName);
                await File.WriteAllBytesAsync(tempSourceFile, sourceContent);

                var pdfFilePath = await ConvertToPdfAsync(tempSourceFile, tempDirectory);

                var pdfContent = await File.ReadAllBytesAsync(pdfFilePath);

                return pdfContent;
            }
            finally
            {
                // Cleanup temp files
                try
                {
                    if (Directory.Exists(tempDirectory))
                    {
                        Directory.Delete(tempDirectory, true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to cleanup temporary directory: {TempDirectory}", tempDirectory);
                }
            }
        }

        private string FindLibreOfficePath()
        {
            var possiblePaths = new List<string>();

            if (OperatingSystem.IsWindows())
            {
                possiblePaths.AddRange(new[]
                {
                    @"C:\Program Files\LibreOffice\program\soffice.exe",
                    @"C:\Program Files (x86)\LibreOffice\program\soffice.exe",
                    @"C:\Program Files\LibreOffice 7\program\soffice.exe",
                    @"C:\Program Files (x86)\LibreOffice 7\program\soffice.exe",
                    @"C:\Program Files\LibreOffice 24\program\soffice.exe",
                    @"C:\Program Files (x86)\LibreOffice 24\program\soffice.exe"
                });
            }
            else if (OperatingSystem.IsLinux())
            {
                possiblePaths.AddRange(new[]
                {
                    "/usr/bin/soffice",
                    "/usr/bin/libreoffice",
                    "/usr/local/bin/soffice",
                    "/usr/local/bin/libreoffice"
                });
            }
            else if (OperatingSystem.IsMacOS())
            {
                possiblePaths.AddRange(new[]
                {
                    "/Applications/LibreOffice.app/Contents/MacOS/soffice"
                });
            }

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    _logger.LogInformation("Found LibreOffice at: {Path}", path);
                    return path;
                }
            }

            _logger.LogWarning("LibreOffice not found in common installation paths");
            return string.Empty;
        }

        private void CleanupZombieProcesses()
        {
            try
            {
                var libreOfficeProcesses = Process.GetProcessesByName("soffice");
                if (libreOfficeProcesses.Length > 0)
                {
                    _logger.LogWarning("Found {Count} existing LibreOffice processes, attempting to cleanup", libreOfficeProcesses.Length);

                    foreach (var proc in libreOfficeProcesses)
                    {
                        try
                        {
                            // Check if process is responsive
                            if (!proc.Responding)
                            {
                                _logger.LogWarning("Killing unresponsive LibreOffice process {ProcessId}", proc.Id);
                                proc.Kill(entireProcessTree: true);
                            }
                            else
                            {
                                _logger.LogInformation("LibreOffice process {ProcessId} is still responsive, not killing", proc.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to kill LibreOffice process {ProcessId}", proc.Id);
                        }
                        finally
                        {
                            proc.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during zombie process cleanup");
            }
        }
    }
}