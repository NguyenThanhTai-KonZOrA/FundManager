namespace DigitalDocumentPlatform.Common.Helper
{
    public static class CommonHeplers
    {
        public static string GenerateClientSession()
        {
            var browser = "Chrome-140";
            var sdkVersion = "1.0.2.1";
            var uuid = Guid.NewGuid().ToString();
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return $"WEB-SDK_{browser}_{sdkVersion}_{uuid}_{timestamp}";
        }
    }
}