using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Pims.Core.Models;

namespace Pims.Core.Services
{
    public sealed class CredentialStore
    {
        private const string FileName = "credentials.json";
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public async Task<UserCredentials?> LoadAsync()
        {
            var path = GetPath();
            if (!File.Exists(path))
            {
                return null;
            }

            await using var stream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<UserCredentials>(stream, Options);
        }

        public async Task SaveAsync(UserCredentials credentials)
        {
            var path = GetPath();
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, credentials, Options);
        }

        private static string GetPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "Pims", FileName);
        }
    }
}
