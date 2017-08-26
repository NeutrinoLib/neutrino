using System.Net;
using System.Net.Http;

namespace Neutrino.Client
{
    public class HttpRequestService : IHttpRequestService
    {
        private readonly HttpClient _httpClient;
        
        public HttpRequestService()
        {
            var handler = new HttpClientHandler() 
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            _httpClient = new HttpClient(handler);
        }

        public HttpClient GetHttpClient()
        {
            return _httpClient;
        }
    }
}