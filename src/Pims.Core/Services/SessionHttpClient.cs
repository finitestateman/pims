using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pims.Core.Services
{
    public sealed class SessionHttpClient
    {
        private readonly HttpClient _client;
        private readonly AuthenticationService _authenticationService;

        public SessionHttpClient(HttpClient client, AuthenticationService authenticationService)
        {
            _client = client;
            _authenticationService = authenticationService;
        }

        public async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> requestFactory, CancellationToken cancellationToken = default)
        {
            await _authenticationService.EnsureAuthenticatedAsync(cancellationToken).ConfigureAwait(false);
            using var request = requestFactory();
            var response = await _client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            response.Dispose();
            await _authenticationService.RefreshAsync(cancellationToken).ConfigureAwait(false);
            using var retry = requestFactory();
            return await _client.SendAsync(retry, cancellationToken).ConfigureAwait(false);
        }
    }
}
