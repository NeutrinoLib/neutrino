using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Neutrino.Entities;
using Flurl;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;

namespace Neutrino.Client
{
    public class NeutrinoClient : INeutrinoClient
    {
        private readonly IHttpRequestService _httpRequestService;
        private readonly INeutrinoClientOptions _neutrinoClientOptions;

        private string _neutrinoLeaderAddress = null;
        

        public NeutrinoClient(IHttpRequestService httpRequestService, INeutrinoClientOptions neutrinoClientOptions)
        {
            _httpRequestService = httpRequestService;
            _neutrinoClientOptions = neutrinoClientOptions;
        }

        public async Task<ActionConfirmation<KvProperty>> AddKvPropertyAsync(KvProperty kvProperty)
        {
            var neutrinoAddress = GetNeutrinoLeaderAddress();

            var url = neutrinoAddress.AppendPathSegment("/api/key-values");

            var responseMessage = await SendRequest(() => CreateHttpRequestMessage(HttpMethod.Post, url, kvProperty));
            if (responseMessage.IsSuccessStatusCode)
            {
                var kvPropertyStringFromResponse = await responseMessage.Content.ReadAsStringAsync();
                var kvPropertyFromResponse = JsonConvert.DeserializeObject<KvProperty>(kvPropertyStringFromResponse);

                return ActionConfirmation<KvProperty>.CreateSuccessful(kvPropertyFromResponse);
            }

            var errorFromResponse = await responseMessage.Content.ReadAsStringAsync();
            var errorMessage = $"Request finished with status code: '{responseMessage.StatusCode}. Message from server: {errorFromResponse}.";
            return ActionConfirmation.CreateError<KvProperty>(errorMessage);
        }

        public Task<ActionConfirmation<Service>> AddServiceAsync(Service service)
        {
            throw new NotImplementedException();
        }

        public Task<ActionConfirmation> DeleteKvPropertyAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<ActionConfirmation> DeleteServiceAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<KvProperty> GetKvPropertyAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceHealth> GetLastServiceHealthAsync(string serviceId)
        {
            throw new NotImplementedException();
        }

        public Task<Service> GetServiceByAddressAsync(string address)
        {
            throw new NotImplementedException();
        }

        public Task<Service> GetServiceByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ServiceHealth>> GetServiceHealthAsync(string serviceId)
        {
            throw new NotImplementedException();
        }

        public Task<PageList<ServiceHealth>> GetServiceHealthAsync(string serviceId, int offset, int limit)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Service>> GetServicesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<Service>> GetServicesByServiceTypeAsync(string serviceType)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Service>> GetServicesByTagsAsync(params string[] tags)
        {
            throw new NotImplementedException();
        }

        public Task<ActionConfirmation> UpdateKvPropertyAsync(string key, KvProperty kvProperty)
        {
            throw new NotImplementedException();
        }

        public Task<ActionConfirmation> UpdateServiceAsync(string id, Service service)
        {
            throw new NotImplementedException();
        }

		private HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, Url url, object objectToSend)
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

        private async Task<HttpResponseMessage> SendRequest(Func<HttpRequestMessage> httpRequestMessageFunc)
        {
            var httpRequestMessage = httpRequestMessageFunc();

            var httpClient = _httpRequestService.GetHttpClient();
            var responseMessage = await httpClient.SendAsync(httpRequestMessage);

            if(responseMessage.StatusCode == HttpStatusCode.Redirect)
            {
                var redirectedUri = responseMessage.Headers.Location;
                SetNeutrinoLeaderAddress(redirectedUri);

                responseMessage = await RedirectHttpRequest(httpRequestMessageFunc, redirectedUri);
            }

            return responseMessage;
        }

        private async Task<HttpResponseMessage> RedirectHttpRequest(Func<HttpRequestMessage> httpRequestMessageFunc, Uri redirectedUri)
        {
            var httpRequestMessage = httpRequestMessageFunc();
            httpRequestMessage.RequestUri = redirectedUri;

            var httpClient = _httpRequestService.GetHttpClient();
            var responseMessage = await httpClient.SendAsync(httpRequestMessage);

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
