using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentBehave;
using Neutrino.Api.Specs.Infrastructure;
using Neutrino.Entities.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Neutrino.Api.Specs.Implementations.Services
{
    [Feature("ServicesPost", "Registering new service")]
    public class ServicesPost
    {
        private string _serviceId;
        private string _serviceType;
        private string _serviceAddress;
        private int _healthInterval;
        private string _healthEndpoint;
        private int _deregisterCriticalServiceAfter;
        private HealthCheckType _healthCheckType;
        private HttpResponseMessage _response;

        [Scenario("Service have to be registered successfully when all required properties are given")]
        public async Task ServiceHaveToBeRegisteredSuccessfullyWhenAllRequiredPropertiesAreGiven()
        {
            GivenServiceWithId("new-service-01");
            GivenServiceWithServiceType("New Service 01");
            GivenServiceWithAddress("http://localhost:8200");
            GivenServiceHealthCheckTypeIs("None");
            await WhenServiceIsRegistering();
            ThenResponseCodeIs(201);
            ThenLocationIsReturnedInHeaders("api/services/new-service-01");
            await ThenServiceWasRegistered("new-service-01");
        }

        [Scenario("Service cannot be registered when id is not specified")]
        public async Task ServiceCannotBeRegisteredWhenIdIsNotSpecified()
        {
            GivenServiceWithId("");
            GivenServiceWithServiceType("New Service 02");
            GivenServiceWithAddress("http://localhost:8200");
            GivenServiceHealthCheckTypeIs("None");
            await WhenServiceIsRegistering();
            ThenResponseCodeIs(400);
            await ThenErrorMessageContainsMessage("Service id wasn't specified");
        }

        [Scenario("Service cannot be registered when type is not specified")]
        public async Task ServiceCannotBeRegisteredWhenTypeIsNotSpecified()
        {
            GivenServiceWithId("new-service-03");
            GivenServiceWithServiceType("");
            GivenServiceWithAddress("http://localhost:8200");
            GivenServiceHealthCheckTypeIs("None");
            await WhenServiceIsRegistering();
            ThenResponseCodeIs(400);
            await ThenErrorMessageContainsMessage("Service type wasn't specified");
        }

        [Scenario("Service cannot be registered when address is not specified")]
        public async Task ServiceCannotBeRegisteredWhenAddressIsNotSpecified()
        {
            GivenServiceWithId("new-service-04");
            GivenServiceWithServiceType("New Service 04");
            GivenServiceWithAddress("");
            GivenServiceHealthCheckTypeIs("None");
            await WhenServiceIsRegistering();
            ThenResponseCodeIs(400);
            await ThenErrorMessageContainsMessage("Service address wasn't specified");
        }

        [Scenario("After registering helth of service should be passing when service is alive")]
        public async Task AfterRegisteringHelthOfServiceShouldBePassingWhenServiceIsAlive()
        {
            GivenServiceWithId("new-service-05");
            GivenServiceWithServiceType("New Service 05");
            GivenServiceWithAddress("http://httpbin.org");
            GivenServiceHealthCheckTypeIs("HttpRest");
            GivenHelthEndpointIs("http://httpbin.org/get");
            GivenHelthIntervalIs(30);
            GivenDeregisteringCriticalServiceIs(60);
            await WhenServiceIsRegistering();
            ThenResponseCodeIs(201);
            await ThenServiceHealthIs("Passing");
        }

        [Scenario("After registering helth of service should be unknown when health check type is None")]
        public async Task AfterRegisteringHelthOfServiceShouldBeUnknownWhenHealthCheckTypeIsNone()
        {
            GivenServiceWithId("new-service-06");
            GivenServiceWithServiceType("New Service 06");
            GivenServiceWithAddress("http://httpbin.org");
            GivenServiceHealthCheckTypeIs("None");
            GivenHelthEndpointIs("http://httpbin.org/get");
            GivenHelthIntervalIs(30);
            await WhenServiceIsRegistering();
            ThenResponseCodeIs(201);
            await ThenServiceHealthIs("Unknown");
        }

        [Scenario("After registering helth of service should be error when service return 400")]
        public async Task AfterRegisteringHelthOfServiceShouldBeErrorWhenServiceReturn400()
        {
            GivenServiceWithId("new-service-07");
            GivenServiceWithServiceType("New Service 07");
            GivenServiceWithAddress("http://httpbin.org");
            GivenServiceHealthCheckTypeIs("HttpRest");
            GivenHelthEndpointIs("http://httpbin.org/status/400");
            GivenHelthIntervalIs(30);
            GivenDeregisteringCriticalServiceIs(60);
            await WhenServiceIsRegistering();
            ThenResponseCodeIs(201);
            await ThenServiceHealthIs("Error");
        }

        [Scenario("After registering helth of service should be critical when service is not responding")]
        public async Task AfterRegisteringHelthOfServiceShouldBeCriticalWhenServiceIsNotResponding()
        {
            GivenServiceWithId("new-service-08");
            GivenServiceWithServiceType("New Service 08");
            GivenServiceWithAddress("http://notexistingaddress-qazwsx123.org");
            GivenServiceHealthCheckTypeIs("HttpRest");
            GivenHelthEndpointIs("http://notexistingaddress-qazwsx123.org/health");
            GivenHelthIntervalIs(30);
            GivenDeregisteringCriticalServiceIs(60);
            await WhenServiceIsRegistering();
            ThenResponseCodeIs(201);
            await ThenServiceHealthIs("Critical");
        }

        [Scenario("Service cannot be registered when service with same id exists")]
        public async Task ServiceCannotBeRegisteredWhenServiceWithSameIdExists()
        {
            await GivenServiceWithIdNameAddressAndTypeExists("new-service-09", "New Service 01", "http://localhost:8200", "None");
            await WhenServiceWithIdIsRegistering("new-service-09");
            ThenResponseCodeIs(400);
            await ThenErrorMessageContainsMessage("Service with id 'new-service-09' already exists.");
        }

        [Scenario("Service cannot be registered when id contains unacceptable characters")]
        public async Task ServiceCannotBeRegisteredWhenIdContainsUnacceptableCharacters()
        {
            GivenServiceWithId("this $%^ service");
            GivenServiceWithServiceType("New Service 10");
            GivenServiceWithAddress("http://localhost:8200");
            GivenServiceHealthCheckTypeIs("None");
            await WhenServiceIsRegistering();
            ThenResponseCodeIs(400);
            await ThenErrorMessageContainsMessage("Service id contains unacceptable characters (only alphanumeric letters and dash is acceptable).");
        }

        [Given("Service with id (.*)")]
        private void GivenServiceWithId(string id)
        {
            _serviceId = id;
        }

        [Given("Service with name (.*)")]
        private void GivenServiceWithServiceType(string serviceType)
        {
            _serviceType = serviceType;
        }

        [Given("Service with address (.*)")]
        private void GivenServiceWithAddress(string serviceAddress)
        {
            _serviceAddress = serviceAddress;
        }

        [Given("Helth interval is (.*)")]
        private void GivenHelthIntervalIs(int healthInterval)
        {
            _healthInterval = healthInterval;
        }

        [Given("Helth endpoint is (.*)")]
        private void GivenHelthEndpointIs(string healthEndpoint)
        {
            _healthEndpoint = healthEndpoint;
        }

        [Given("Service health check type is (.*)")]
        private void GivenServiceHealthCheckTypeIs(string healthCheckType)
        {
            SetHealthCheckType(healthCheckType);
        }

        [Given("Deregistering critical service is (.*)")]
        private void GivenDeregisteringCriticalServiceIs(int deregisterCriticalServiceAfter)
        {
            _deregisterCriticalServiceAfter = deregisterCriticalServiceAfter;
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

        [When("Service is registering")]
        private async Task WhenServiceIsRegistering()
        {
            await RegisterService();
        }

        [When("Service with id (.*) is registering")]
        private async Task WhenServiceWithIdIsRegistering(string serviceId)
        {
            _serviceId = serviceId;
            await RegisterService();
        }

        [Then("Response code is (.*)")]
        private void ThenResponseCodeIs(int statusCode)
        {
            Assert.Equal(statusCode, (int) _response.StatusCode);
        }

        [Then("Location (.*) is returned in headers")]
        private void ThenLocationIsReturnedInHeaders(string location)
        {
            var locationInHeader = _response.Headers.GetValues("location").FirstOrDefault();
            Assert.Equal(location, locationInHeader);
        }

        [Then("Service (.*) was registered")]
        private async Task ThenServiceWasRegistered(string serviceId)
        {
            var httpClient = ApiTestServer.GetHttpClient();
            var serviceResponse = await httpClient.GetStringAsync($"/api/services/{serviceId}");

            var service = JsonConvert.DeserializeObject<Service>(serviceResponse);
            Assert.NotNull(service);
        }

        [Then("Error message contains message (.*)")]
        private async Task ThenErrorMessageContainsMessage(string errorMessage)
        {
            var response = await _response.Content.ReadAsStringAsync();
            Assert.True(response.Contains(errorMessage));
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