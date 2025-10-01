using System;
using System.Net;
using System.Net.Http;
using Pims.Core.Configuration;

namespace Pims.Core.Services
{
    public sealed class HttpClientFactory
    {
        public (HttpClient Client, CookieContainer CookieContainer) Create(AppConfiguration configuration)
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookies,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = configuration.BaseUri,
                Timeout = TimeSpan.FromSeconds(30)
            };

            return (client, cookies);
        }
    }
}
