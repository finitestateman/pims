using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Pims.Core.Configuration;
using Pims.Core.Models;
using Pims.Core.Utilities;

namespace Pims.Core.Services
{
    public sealed class EmailService
    {
        private readonly SessionHttpClient _httpClient;
        private readonly AppConfiguration _configuration;
        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

        public EmailService(SessionHttpClient httpClient, AppConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IReadOnlyList<EmailMessage>> GetEmailsAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.SendAsync(() => new HttpRequestMessage(HttpMethod.Get, new Uri(_configuration.BaseUri, _configuration.EmailEndpoint)), cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var payload = await JsonSerializer.DeserializeAsync<List<EmailMessage>>(stream, SerializerOptions, cancellationToken).ConfigureAwait(false);
            return payload ?? Array.Empty<EmailMessage>();
        }

        public async Task<EmailMessage?> GetLatestEmailAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.SendAsync(() => new HttpRequestMessage(HttpMethod.Get, new Uri(_configuration.BaseUri, _configuration.EmailLatestEndpoint)), cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                return await JsonSerializer.DeserializeAsync<EmailMessage>(stream, SerializerOptions, cancellationToken).ConfigureAwait(false);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return null;
        }

        public async Task<string?> GetLatestOtpAsync(CancellationToken cancellationToken = default)
        {
            var email = await GetLatestEmailAsync(cancellationToken).ConfigureAwait(false);
            if (email == null)
            {
                return null;
            }

            return OtpParser.TryExtract(email.Subject, out var otp) ? otp : null;
        }
    }
}
