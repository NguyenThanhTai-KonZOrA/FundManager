using System.Globalization;
using System.Text;

namespace DigitalDocumentPlatform.Common.Helper
{
    public static class StringHelper
    {
        // Accepts raw base64 or data URLs like "data:image/png;base64,AAAA..."
        public static bool TryGetBase64Bytes(string input, out byte[] bytes)
        {
            bytes = Array.Empty<byte>();
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var value = input.Trim();
            var commaIndex = value.IndexOf(',');
            if (value.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIndex >= 0)
                value = value[(commaIndex + 1)..];

            try
            {
                bytes = Convert.FromBase64String(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static string TryGetStringToRoundValue(string input)
        {
            var roundString = string.Empty;
            if (double.TryParse(input, out double result))
            {
                roundString = Math.Round(result, 3).ToString();
            }
            return roundString;
        }

        public static string SubStringPercent(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            const int max = 6;
            return input.Length <= max ? input : input.Substring(0, max);
            // .NET 8 alternative:
            // return input[..Math.Min(input.Length, max)];
        }

        /// <summary>
        /// Loại bỏ dấu tiếng Việt (diacritics) và trả về chuỗi ASCII cơ bản.
        /// - Giữ nguyên chữ cái không dấu.
        /// - Chuyển 'Đ'/'đ' thành 'D'/'d'.
        /// - Có thể chuyển toàn bộ sang In Hoa nếu toUpperInvariant = true.
        /// - Trả về chuỗi rỗng nếu input null/empty/whitespace.
        /// </summary>
        public static string RemoveVietnameseDiacritics(string? input, bool toUpperInvariant = false)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Chuẩn hóa sang FormD để tách dấu
            string normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);

            foreach (var c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc == UnicodeCategory.NonSpacingMark ||
                    uc == UnicodeCategory.SpacingCombiningMark ||
                    uc == UnicodeCategory.EnclosingMark)
                {
                    // Bỏ qua dấu
                    continue;
                }

                // Chuyển đặc biệt 'Đ', 'đ'
                switch (c)
                {
                    case 'Đ':
                        sb.Append('D');
                        continue;
                    case 'đ':
                        sb.Append('d');
                        continue;
                }

                sb.Append(c);
            }

            var result = sb.ToString().Normalize(NormalizationForm.FormC);
            return toUpperInvariant ? result.ToUpperInvariant() : result;
        }

        /// <summary>
        /// Tạo "slug" cơ bản từ chuỗi tiếng Việt:
        /// - Bỏ dấu
        /// - Lowercase
        /// - Thay khoảng trắng và ký tự không hợp lệ bằng '-'
        /// - Loại bỏ '-' thừa ở đầu/cuối
        /// </summary>
        public static string ToVietnameseSlug(string? input)
        {
            var core = RemoveVietnameseDiacritics(input, toUpperInvariant: false);
            if (string.IsNullOrEmpty(core))
                return string.Empty;

            var sb = new StringBuilder(core.Length);
            bool lastDash = false;
            foreach (var ch in core)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    sb.Append(char.ToLowerInvariant(ch));
                    lastDash = false;
                }
                else if (char.IsWhiteSpace(ch) || ch == '-' || ch == '_' || char.IsPunctuation(ch) || char.IsSeparator(ch))
                {
                    if (!lastDash)
                    {
                        sb.Append('-');
                        lastDash = true;
                    }
                }
                // ignore other symbols
            }

            // Trim leading / trailing '-'
            var slug = sb.ToString().Trim('-');
            return slug;
        }

        public static string GetEmailPrefix(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;

            int atIndex = email.IndexOf('@');
            if (atIndex <= 0)
                return string.Empty;

            return email.Substring(0, atIndex);
        }
    }
}