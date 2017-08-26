using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentBehave;
using Neutrino.Api.Specs.Infrastructure;
using Neutrino.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Neutrino.Api.Specs.Implementations.Services
{
    [Feature("ServicesGetById", "Updating service information")]
    public class ServicesGetById
    {
        private string _serviceId;
        private string _serviceType;
        private string _serviceAddress;
        private HealthCheckType _healthCheckType;
        private HttpResponseMessage _response;
        private string _responseContent;
        private dynamic _serviceObject;

        [Scenario("Service data have to be returned when user get service by his id")]
        public async Task ServiceDataHaveToBeReturnedWhenUserGetServiceByHisId()
        {
            await GivenServiceWithIdNameAddressAndTypeExists("getbyid-service-01", "Get By Id Service 01", "http://localhost:8200", "None");
            await WhenGetServiceWithId("getbyid-service-01");
            ThenResponseCodeIs(200);
            ThenServiceIdIs("getbyid-service-01");
            ThenServiceNameIs("Get By Id Service 01");
            ThenServiceAddressIs("http://localhost:8200");
        }

        [Scenario("Not found have to be returned when not existed service is downloading")]
        public async Task NotFoundHaveToBeReturnedWhenNotExistedServiceIsDownloading()
        {
            await GivenServiceWithIdNameAddressAndTypeExists("getbyid-service-02", "Get By Id Service 02", "http://localhost:8200", "None");
            await WhenGetServiceWithId("not-existed-service");
            ThenResponseCodeIs(404);
        }

        [Given("Service with id (.*) name (.*) address (.*) and type (.*) exists")]
        public async Task GivenServiceWithIdNameAddressAndTypeExists(string serviceId, string serviceType, string address, string healthCheckType)
        {
            _serviceId = serviceId;
            _serviceType = serviceType;
            _serviceAddress = address;
            SetHealthCheckType(healthCheckType);

            await RegisterService();
            Assert.Equal(HttpStatusCode.Created, _response.StatusCode);
        }

        [When("Get service with id (.*)")]
        private async Task WhenGetServiceWithId(string serviceId)
        {
            var httpClient = ApiTestServer.GetHttpClient();
            _response = await httpClient.GetAsync($"/api/services/{serviceId}");
            _responseContent = await _response.Content.ReadAsStringAsync();

            if(_response.IsSuccessStatusCode)
            {
                _serviceObject = JObject.Parse(_responseContent);
            }
        }

        [Then("Response code is (.*)")]
        private void ThenResponseCodeIs(int statusCode)
        {
            Assert.Equal(statusCode, (int) _response.StatusCode);
        }

        [Then("Service id is (.*)")]
        private void ThenServiceIdIs(string serviceId)
        {
            Assert.Equal(serviceId, (string) _serviceObject.id);
        }

        [Then("Service name is (.*)")]
        private void ThenServiceNameIs(string serviceName)
        {
            Assert.Equal(serviceName, (string) _serviceObject.serviceType);
        }

        [Then("Service address is (.*)")]
        private void ThenServiceAddressIs(string serviceAddress)
        {
            Assert.Equal(serviceAddress, (string) _serviceObject.address);
        }

        private async Task RegisterService()
        {
            var service = new Service
            {
                Id = _serviceId,
                ServiceType = _serviceType,
                Address = _serviceAddress,
                HealthCheck = new HealthCheck
                {
                    HealthCheckType = _healthCheckType
                }
            };

            var jsonString = JsonConvert.SerializeObject(service);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var httpClient = ApiTestServer.GetHttpClient();
            _response = await httpClient.PostAsync("/api/services", httpContent);
        }

        private void SetHealthCheckType(string healthCheckType)
        {
            if (healthCheckType == "None")
            {
                _healthCheckType = HealthCheckType.None;
            }
            else if (healthCheckType == "HttpRest")
            {
                _healthCheckType = HealthCheckType.HttpRest;
            }
        }
    }
}