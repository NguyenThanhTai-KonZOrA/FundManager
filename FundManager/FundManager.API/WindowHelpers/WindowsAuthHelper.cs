using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace DigitalDocumentPlatform.API.WindowHelpers
{
    public static class WindowsAuthHelper
    {
        public static string NormalizeReturnUrl(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                return "/";

            returnUrl = returnUrl.Replace("\\", "/");

            if (!returnUrl.StartsWith("/"))
                returnUrl = "/" + returnUrl;

            return returnUrl;
        }


        [SupportedOSPlatform("windows")]
        public static int WindowsAccount(string username, string password)
        {
            using (DirectoryEntry entry = new DirectoryEntry())
            {
                entry.Username = username;
                entry.Password = password;

                DirectorySearcher searcher = new DirectorySearcher(entry);

                searcher.Filter = "(objectclass=user)";
                try
                {
                    SearchResult sr = searcher.FindOne()!;
                    if (sr != null)
                    {

                        return 1;
                    }
                    else
                        return 0;

                }
                catch (COMException)
                {
                    return -2;
                }
            }
        }
    }
}