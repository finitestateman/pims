using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Pims.Core.Configuration;
using Pims.Core.Models;

namespace Pims.Core.Services
{
    public sealed class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;
        private readonly AppConfiguration _configuration;
        private readonly CredentialStore _credentialStore;

        private UserCredentials? _cachedCredentials;
        private bool _isAuthenticated;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public AuthenticationService(HttpClient httpClient, CookieContainer cookieContainer, AppConfiguration configuration, CredentialStore credentialStore)
        {
            _httpClient = httpClient;
            _cookieContainer = cookieContainer;
            _configuration = configuration;
            _credentialStore = credentialStore;
        }

        public async Task<UserCredentials> GetOrCreateCredentialsAsync(Func<Task<UserCredentials>> credentialFactory)
        {
            if (_cachedCredentials != null)
            {
                return _cachedCredentials;
            }

            var credentials = await _credentialStore.LoadAsync().ConfigureAwait(false);
            if (credentials == null)
            {
                credentials = await credentialFactory().ConfigureAwait(false);
                await _credentialStore.SaveAsync(credentials).ConfigureAwait(false);
            }

            _cachedCredentials = credentials;
            return credentials;
        }

        public void UpdateCachedCredentials(UserCredentials credentials)
        {
            _cachedCredentials = credentials;
            _isAuthenticated = false;
        }

        public async Task EnsureAuthenticatedAsync(CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_isAuthenticated)
                {
                    return;
                }

                var credentials = _cachedCredentials ?? await _credentialStore.LoadAsync().ConfigureAwait(false);
                if (credentials == null)
                {
                    throw new InvalidOperationException("Credentials are not configured.");
                }

                await LoginWithRotationAsync(credentials, cancellationToken).ConfigureAwait(false);
                _cachedCredentials = credentials;
                _isAuthenticated = true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task RefreshAsync(CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                ClearCookies();
                _isAuthenticated = false;
            }
            finally
            {
                _semaphore.Release();
            }

            await EnsureAuthenticatedAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task LoginWithRotationAsync(UserCredentials credentials, CancellationToken cancellationToken)
        {
            ClearCookies();
            if (await TryLoginAsync(credentials.UserId, credentials.PrimaryPassword, cancellationToken).ConfigureAwait(false))
            {
                return;
            }

            if (await TryLoginAsync(credentials.UserId, credentials.SecondaryPassword, cancellationToken).ConfigureAwait(false))
            {
                return;
            }

            throw new InvalidOperationException("Login failed with both primary and secondary passwords.");
        }

        private void ClearCookies()
        {
            var cookies = _cookieContainer.GetCookies(_configuration.BaseUri);
            foreach (Cookie cookie in cookies)
            {
                cookie.Expired = true;
            }
        }

        private async Task<bool> TryLoginAsync(string userId, string password, CancellationToken cancellationToken)
        {
            var payload = new Dictionary<string, string>
            {
                ["username"] = userId,
                ["password"] = password
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_configuration.BaseUri, _configuration.LoginEndpoint))
            {
                Content = new FormUrlEncodedContent(payload)
            };

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
    }
}
