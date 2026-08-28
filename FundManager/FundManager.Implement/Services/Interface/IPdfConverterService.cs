namespace DigitalDocumentPlatform.Implement.Services.Interface
{
    public interface IPdfConverterService
    {
        /// <summary>
        /// Convert a document file (Excel, Word, etc.) to PDF using LibreOffice
        /// </summary>
        /// <param name="sourceFilePath">Path to the source file to convert</param>
        /// <param name="outputDirectory">Directory where the PDF will be saved (optional, defaults to temp directory)</param>
        /// <returns>Path to the generated PDF file</returns>
        Task<string> ConvertToPdfAsync(string sourceFilePath, string? outputDirectory = null);

        /// <summary>
        /// Convert document content (byte array) to PDF
        /// </summary>
        /// <param name="sourceContent">The source file content as byte array</param>
        /// <param name="sourceFileName">Original filename with extension</param>
        /// <returns>PDF file content as byte array</returns>
        Task<byte[]> ConvertToPdfAsync(byte[] sourceContent, string sourceFileName);

        /// <summary>
        /// Check if LibreOffice is installed and accessible
        /// </summary>
        /// <returns>True if LibreOffice is available</returns>
        bool IsLibreOfficeAvailable();
    }
}