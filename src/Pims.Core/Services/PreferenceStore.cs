using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Pims.Core.Models;

namespace Pims.Core.Services
{
    public sealed class PreferenceStore
    {
        private const string FileName = "preferences.json";
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public async Task<UserPreferences> LoadAsync()
        {
            var path = GetPath();
            if (!File.Exists(path))
            {
                return new UserPreferences();
            }

            await using var stream = File.OpenRead(path);
            var prefs = await JsonSerializer.DeserializeAsync<UserPreferences>(stream, Options);
            return prefs ?? new UserPreferences();
        }

        public async Task SaveAsync(UserPreferences preferences)
        {
            var path = GetPath();
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, preferences, Options);
        }

        private static string GetPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "Pims", FileName);
        }
    }
}
