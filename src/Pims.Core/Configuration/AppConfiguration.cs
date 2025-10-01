using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pims.Core.Configuration
{
    public sealed class AppConfiguration
    {
        public Uri BaseUri { get; set; } = new("https://example.internal");

        public string LoginEndpoint { get; set; } = "/api/login";

        public string EmailEndpoint { get; set; } = "/api/emails";

        public string EmailLatestEndpoint { get; set; } = "/api/emails/latest";

        public string FtpHost { get; set; } = "ftp://files.internal";

        public string TrayTooltip { get; set; } = "Pims Utility";

        public string OtpSubjectPattern { get; set; } = "OTP";

        public AppConfiguration DeepCopy() => JsonSerializer.Deserialize<AppConfiguration>(JsonSerializer.Serialize(this))!;
    }

    public sealed class AppConfigurationProvider
    {
        private const string FileName = "appsettings.pims.json";
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public AppConfiguration Load()
        {
            var path = GetConfigPath();
            if (!File.Exists(path))
            {
                var configuration = new AppConfiguration();
                Save(configuration);
                return configuration;
            }

            using var stream = File.OpenRead(path);
            var config = JsonSerializer.Deserialize<AppConfiguration>(stream, Options);
            return config ?? new AppConfiguration();
        }

        public void Save(AppConfiguration configuration)
        {
            var path = GetConfigPath();
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var payload = JsonSerializer.Serialize(configuration, Options);
            File.WriteAllText(path, payload);
        }

        public string GetConfigPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "Pims", FileName);
        }
    }
}
