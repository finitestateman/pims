using System.Text.RegularExpressions;

namespace Pims.Core.Utilities
{
    public static class OtpParser
    {
        private static readonly Regex OtpRegex = new("\\b\\d{4,8}\\b", RegexOptions.Compiled);

        public static bool TryExtract(string? subject, out string otp)
        {
            otp = string.Empty;
            if (string.IsNullOrWhiteSpace(subject))
            {
                return false;
            }

            var match = OtpRegex.Match(subject);
            if (!match.Success)
            {
                return false;
            }

            otp = match.Value;
            return true;
        }
    }
}
