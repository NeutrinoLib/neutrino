using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Flurl;
using Neutrino.Entities.Response;
using Newtonsoft.Json;

namespace Neutrino.Client
{
    public class HttpRequestService : IHttpRequestService
    {
        private readonly HttpClient _httpClient;
        private readonly INeutrinoClientOptions _neutrinoClientOptions;

        private string _neutrinoLeaderAddress = null;
        
        public HttpRequestService(INeutrinoClientOptions neutrinoClientOptions)
        {
            _neutrinoClientOptions = neutrinoClientOptions;

            var handler = new HttpClientHandler() 
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            _httpClient = new HttpClient(handler);
        }

        public async Task<ActionConfirmation<T>> SendRequest<T>(HttpMethod httpMethod, string path) where T: class
        {
            return await SendRequest<T>(httpMethod, path, null, null, null);
        }

        public async Task<ActionConfirmation<T>> SendRequest<T>(HttpMethod httpMethod, string path, string id) where T: class
        {
            return await SendRequest<T>(httpMethod, path, id, null, null);
        }

        public async Task<ActionConfirmation<T>> SendRequest<T>(HttpMethod httpMethod, string path, string id, string query) where T: class
        {
            return await SendRequest<T>(httpMethod, path, id, query, null);
        }

        public async Task<ActionConfirmation<T>> SendRequest<T>(HttpMethod httpMethod, string path, string id, T objectData) where T: class
        {
            return await SendRequest<T>(httpMethod, path, id, null, objectData);
        }

        public async Task<ActionConfirmation<T>> SendRequest<T>(HttpMethod httpMethod, string path, T objectData) where T: class
        {
            return await SendRequest<T>(httpMethod, path, null, null, objectData);
        }

        public async Task<ActionConfirmation<T>> SendRequest<T>(HttpMethod httpMethod, string path, string id, string query, T objectData) where T: class
        {
            var neutrinoAddress = GetNeutrinoLeaderAddress();
            var url = neutrinoAddress.AppendPathSegment(path);

            if(!string.IsNullOrWhiteSpace(id))
            {
                url = url.AppendPathSegment(id);
            }

            if(!string.IsNullOrWhiteSpace(query))
            {
                url = url.AppendPathSegment(query);
            }

            var responseMessage = await SendRequest(() => CreateHttpRequestMessage(httpMethod, url, objectData));
            if (responseMessage.IsSuccessStatusCode)
            {
                var objectStringFromResponse = await responseMessage.Content.ReadAsStringAsync();
                var objectFromResponse = default(T);

                if(!string.IsNullOrWhiteSpace(objectStringFromResponse))
                {
                    objectFromResponse = JsonConvert.DeserializeObject<T>(objectStringFromResponse);
                }

                return ActionConfirmation<T>.CreateSuccessful(objectFromResponse);
            }

            var errorFromResponse = await responseMessage.Content.ReadAsStringAsync();
            var errorMessage = $"Request finished with status code: '{responseMessage.StatusCode}. Message from server: {errorFromResponse}.";
            return ActionConfirmation.CreateError<T>(errorMessage);
        }

        public async Task<ActionConfirmation> SendRequest(HttpMethod httpMethod, string path, string id = null)
        {
            var neutrinoAddress = GetNeutrinoLeaderAddress();
            var url = neutrinoAddress.AppendPathSegment(path);

            if(!string.IsNullOrWhiteSpace(id))
            {
                url = url.AppendPathSegment(id);
            }

            var responseMessage = await SendRequest(() => CreateHttpRequestMessage(httpMethod, url, null));
            if (responseMessage.IsSuccessStatusCode)
            {
                return ActionConfirmation.CreateSuccessful();
            }

            var errorFromResponse = await responseMessage.Content.ReadAsStringAsync();
            var errorMessage = $"Request finished with status code: '{responseMessage.StatusCode}. Message from server: {errorFromResponse}.";
            return ActionConfirmation.CreateError(errorMessage);
        }

        public async Task<HttpResponseMessage> SendRequest(Func<HttpRequestMessage> httpRequestMessageFunc)
        {
            var httpRequestMessage = httpRequestMessageFunc();

            var responseMessage = await _httpClient.SendAsync(httpRequestMessage);

            if(responseMessage.StatusCode == HttpStatusCode.Redirect)
            {
                var redirectedUri = responseMessage.Headers.Location;
                SetNeutrinoLeaderAddress(redirectedUri);

                responseMessage = await RedirectHttpRequest(httpRequestMessageFunc, redirectedUri);
            }

            return responseMessage;
        }

		public HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, Url url, object objectToSend)
		{
			var httpRequestMessage = new HttpRequestMessage(httpMethod, url);
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue($"SecureToken", _neutrinoClientOptions.SecureToken);

			if (objectToSend != null)
			{
				var json = JsonConvert.SerializeObject(objectToSend);
				var content = new JsonContent(json);
				httpRequestMessage.Content = content;
			}

			return httpRequestMessage;
		}

        public async Task<HttpResponseMessage> RedirectHttpRequest(Func<HttpRequestMessage> httpRequestMessageFunc, Uri redirectedUri)
        {
            var httpRequestMessage = httpRequestMessageFunc();
            httpRequestMessage.RequestUri = redirectedUri;

            var responseMessage = await _httpClient.SendAsync(httpRequestMessage);

            return responseMessage;
        }

        private void SetNeutrinoLeaderAddress(Uri redirectedUri)
        {
            _neutrinoLeaderAddress = redirectedUri.OriginalString.Replace(redirectedUri.PathAndQuery, string.Empty);
        }

        private string GetNeutrinoLeaderAddress()
        {
            if(string.IsNullOrWhiteSpace(_neutrinoLeaderAddress))
            {
                return _neutrinoClientOptions.Addresses.FirstOrDefault();
            }

            return _neutrinoLeaderAddress;
        }
    }
}