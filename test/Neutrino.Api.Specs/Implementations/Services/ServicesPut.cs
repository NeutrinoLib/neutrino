using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentBehave;
using Neutrino.Api.Specs.Infrastructure;
using Neutrino.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Neutrino.Api.Specs.Implementations.Services
{
    [Feature("ServicesPut", "Updating service information")]
    public class ServicesPut
    {
        private string _serviceId;
        private string _serviceType;
        private string _serviceAddress;
        private int _healthInterval;
        private string _healthEndpoint;
        private int _deregisterCriticalServiceAfter;
        private HealthCheckType _healthCheckType;
        private HttpResponseMessage _response;
        private string _responseContent;
        private dynamic _responseObject;

        [Scenario("Service type have to updated successfully when new type was specified")]
        public async Task ServiceTypeHaveToUpdatedSuccessfullyWhenNewTypeWasSpecified()
        {
            await GivenServiceWithIdNameAddressAndTypeExists("service-01", "Service 01", "http://localhost:8200", "None");
            GivenNewServiceNameIs("New Service 01");
            await WhenServiceIsUpdating("service-01");
            ThenResponseCodeIs(200);
            ThenServiceHasName("New Service 01");
        }

        [Scenario("Service cannot be updated when new type is empty")]
        public async Task ServiceCannotBeUpdatedWhenNewTypeIsEmpty()
        {
            await GivenServiceWithIdNameAddressAndTypeExists("service-02", "Service 02", "http://localhost:8200", "None");
            GivenNewServiceNameIs("");
            await WhenServiceIsUpdating("service-02");
            ThenResponseCodeIs(400);
            ThenErrorMessageContainsMessage("Service type wasn't specified");
        }

        [Scenario("Service address have to updated successfully when new address was specified")]
        public async Task ServiceAddressHaveToUpdatedSuccessfullyWhenNewAddressWasSpecified()
        {
            await GivenServiceWithIdNameAddressAndTypeExists("service-03", "Service 03", "http://localhost:8200", "None");
            GivenNewServiceAddressIs("http://localhost:9000");
            await WhenServiceIsUpdating("service-03");
            ThenResponseCodeIs(200);
            ThenServiceHasAddress("http://localhost:9000");
        }

        [Scenario("Service cannot be updated when new address is empty")]
        public async Task ServiceCannotBeUpdatedWhenNewAddressIsEmpty()
        {
            await GivenServiceWithIdNameAddressAndTypeExists("service-04", "Service 04", "http://localhost:8200", "None");
            GivenNewServiceAddressIs("");
            await WhenServiceIsUpdating("service-04");
            ThenResponseCodeIs(400);
            ThenErrorMessageContainsMessage("Service address wasn't specified");
        }

        [Scenario("After changing healt type from None to HttpRest healt should be checked")]
        public async Task AfterChangingHealtTypeFromNoneToHttpRestHealtShouldBeChecked()
        {
            await GivenServiceWithIdNameAddressAndTypeExists("service-05", "Service 05", "http://httpbin.org/get", "None");
            GivenNewServiceHealthCheckTypeIs("HttpRest");
            GivenNewHelthEndpointIs("http://httpbin.org/get");
            GivenNewHelthIntervalIs(30);
            GivenNewDeregisteringCriticalServiceIs(60);
            await WhenServiceIsUpdating("service-05");
            ThenResponseCodeIs(200);
            await ThenServiceHealthIs("Passing");
        }

        [Scenario("Not found have to be returned when not existed service is updating")]
        public async Task NotFoundHaveToBeReturnedWhenNotExistedServiceIsUpdating()
        {
            await GivenServiceWithIdNameAddressAndTypeExists("service-06", "Service 06", "http://localhost:8200", "None");
            GivenNewServiceHealthCheckTypeIs("HttpRest");
            await WhenServiceIsUpdating("not-existed-service");
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

        [Given("New service name is (.*)")]
        private void GivenNewServiceNameIs(string serviceType)
        {
            _serviceType = serviceType;
        }

        [When("Service is updating (.*)")]
        private async Task WhenServiceIsUpdating(string serviceId)
        {
            await UpdateService(serviceId);
        }

        [Then("Response code is (.*)")]
        private void ThenResponseCodeIs(int statusCode)
        {
            Assert.Equal(statusCode, (int) _response.StatusCode);
        }

        [Then("Service has name (.*)")]
        private void ThenServiceHasName(string serviceType)
        {
            Assert.Equal(serviceType, (string) _responseObject.serviceType);
        }

        [Then("Error message contains message (.*)")]
        private void ThenErrorMessageContainsMessage(string errorMessage)
        {
            Assert.True(_responseContent.Contains(errorMessage));
        }

        [Given("New service address is (.*)")]
        private void GivenNewServiceAddressIs(string serviceAddress)
        {
            _serviceAddress = serviceAddress;
        }

        [Then("Service has address (.*)")]
        private void ThenServiceHasAddress(string serviceAddress)
        {
            Assert.Equal(serviceAddress, (string) _responseObject.address);
        }

        [Given("New service health check type is (.*)")]
        private void GivenNewServiceHealthCheckTypeIs(string healtCheckType)
        {
            SetHealthCheckType(healtCheckType);
        }

        [Given("New Helth endpoint is (.*)")]
        private void GivenNewHelthEndpointIs(string healthEndpoint)
        {
            _healthEndpoint = healthEndpoint;
        }

        [Given("New deregistering critical service is (.*)")]
        private void GivenNewDeregisteringCriticalServiceIs(int deregisterCriticalServiceAfter)
        {
            _deregisterCriticalServiceAfter = deregisterCriticalServiceAfter;
        }

        [Given("New helth interval is (.*)")]
        private void GivenNewHelthIntervalIs(int healthInterval)
        {
            _healthInterval = healthInterval;
        }

        [Then("Service health is (.*)")]
        private async Task ThenServiceHealthIs(string healthStatus)
        {
            Thread.Sleep(2000);
            var httpClient = ApiTestServer.GetHttpClient();
            var httpResponseMessage = await httpClient.GetAsync($"/api/services/{_serviceId}/health/current");

            var response = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic parsedResponse = JObject.Parse(response);

            Assert.Equal(healthStatus, (string) parsedResponse.healthState);
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
                    HealthCheckType = _healthCheckType,
                    Address = _healthEndpoint,
                    Interval = _healthInterval,
                    DeregisterCriticalServiceAfter = _deregisterCriticalServiceAfter
                }
            };

            var jsonString = JsonConvert.SerializeObject(service);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var httpClient = ApiTestServer.GetHttpClient();
            _response = await httpClient.PostAsync("/api/services", httpContent);
        }

        private async Task UpdateService(string serviceId)
        {
            var service = new Service
            {
                Id = _serviceId,
                ServiceType = _serviceType,
                Address = _serviceAddress,
                HealthCheck = new HealthCheck
                {
                    HealthCheckType = _healthCheckType,
                    Address = _healthEndpoint,
                    Interval = _healthInterval,
                    DeregisterCriticalServiceAfter = _deregisterCriticalServiceAfter
                }
            };

            var jsonString = JsonConvert.SerializeObject(service);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var httpClient = ApiTestServer.GetHttpClient();
            _response = await httpClient.PutAsync($"/api/services/{serviceId}", httpContent);
            _responseContent = await _response.Content.ReadAsStringAsync();

            var response = await httpClient.GetAsync($"/api/services/{serviceId}");
            if(response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _responseObject = JObject.Parse(responseContent);
            }
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