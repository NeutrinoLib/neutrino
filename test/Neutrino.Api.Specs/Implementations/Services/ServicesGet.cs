using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentBehave;
using Neutrino.Api.Specs.Infrastructure;
using Neutrino.Entities.Model;
using Newtonsoft.Json;
using Xunit;

namespace Neutrino.Api.Specs.Implementations.Services
{
    [Feature("ServicesGet", "Getting informaation about all registered services")]
    public class ServicesGet
    {
        private string _serviceId;
        private string _serviceType;
        private string _serviceAddress;
        private HealthCheckType _healthCheckType;
        private HttpResponseMessage _response;
        private string _responseContent;

        [Scenario("Information about all registered services have to be returned")]
        public async Task InformationAboutAllRegisteredServicesHaveToBeReturned()
        {
            await GivenServiceWithIdNameAddressAndTypeExists("get-service-01", "Get Service 01", "http://localhost:8200", "None");
            await GivenServiceWithIdNameAddressAndTypeExists("get-service-02", "Get Service 02", "http://localhost:8200", "None");
            await WhenListOfServicesIsDownloaded();
            ThenResponseCodeIs(200);
            ThenListContainsService("get-service-01");
            ThenListContainsService("get-service-02");
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

        [When("List of services is downloaded")]
        private async Task WhenListOfServicesIsDownloaded()
        {
            var httpClient = ApiTestServer.GetHttpClient();
            _response = await httpClient.GetAsync($"/api/services");
            _responseContent = await _response.Content.ReadAsStringAsync();
        }

        [Then("Response code is (.*)")]
        private void ThenResponseCodeIs(int statusCode)
        {
            Assert.Equal(statusCode, (int) _response.StatusCode);
        }

        [Then("List contains service (.*)")]
        private void ThenListContainsService(string serviceId)
        {
            Assert.True(_responseContent.Contains(serviceId));
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