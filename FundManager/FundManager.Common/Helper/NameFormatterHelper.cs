namespace DigitalDocumentPlatform.Common.Helper
{
    public static class NameFormatter
    {
        // iso2 == "vn"  => FirstName + LastName
        // iso2 != "vn"  => LastName + FirstName
        public static string FormatFullName(string? iso2, string? firstName, string? lastName)
        {
            var iso = (iso2 ?? string.Empty).Trim().ToLowerInvariant();
            var f = (firstName ?? string.Empty).Trim();
            var l = (lastName ?? string.Empty).Trim();

            return iso == "vn"
                ? string.Join(' ', new[] { l, f }.Where(s => !string.IsNullOrEmpty(s)))
                : string.Join(' ', new[] { f, l }.Where(s => !string.IsNullOrEmpty(s)));
        }

        public static string FormatFullNameOnline(string? iso2, string? firstName, string? lastName)
        {
            var iso = (iso2 ?? string.Empty).Trim().ToLowerInvariant();
            var f = (firstName ?? string.Empty).Trim();
            var l = (lastName ?? string.Empty).Trim();

            return iso == "vn"
                ? string.Join(' ', new[] { f, l }.Where(s => !string.IsNullOrEmpty(s)))
                : string.Join(' ', new[] { l, f }.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}