using Flurl.Http.Configuration;
using System.Net.Http;

namespace Sdk.AspNetCore.Serilog.Test
{
    public class TestHttpClientFactory : DefaultHttpClientFactory
    {
        private readonly HttpClient _httpClient;

        public TestHttpClientFactory(HttpClient httpClient) => _httpClient = httpClient;

        public override HttpClient CreateHttpClient(HttpMessageHandler handler) => _httpClient;
    }
}
