namespace Pims.Core.Models
{
    public sealed class UserPreferences
    {
        public string? LastLocalJarPath { get; set; }
        public string? LastFtpDirectory { get; set; }
        public string OtpShortcutGesture { get; set; } = "Ctrl+Alt+O";
    }
}
