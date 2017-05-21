using FluentBehave;
using System;
using Neutrino.Api.Specs.Infrastructure;
using Neutrino.Entities;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Text;
using System.Linq;

namespace Neutrino.Api.Specs.Implementations.Services
{
    [Feature("ServicesPost", "Registering new service")]
    public class ServicesPost
    {
        private string _serviceId;
        private string _serviceType;
        private string _serviceAddress;
        private HttpResponseMessage _response;

        [Scenario("Service have to be registered successfully when all required properties are given")]
        public async Task ServiceHaveToBeRegisteredSuccessfullyWhenAllRequiredPropertiesAreGiven()
        {
            GivenServiceWithId("new-service-01");
            GivenServiceWithServiceType("New Service 01");
            await WhenServiceIsRegistering();
            ThenResponseCodeIs(201);
            ThenLocationIsReturnedInHeaders("api/services/new-service-01");
            await ThenServiceWasRegistered("new-service-01");
        }

        [Given("Service with id")]
        private void GivenServiceWithId(string id)
        {
            _serviceId = id;
        }

        [Given("Service with name")]
        private void GivenServiceWithServiceType(string serviceType)
        {
            _serviceType = serviceType;
        }

        [When("Service is registering")]
        private async Task WhenServiceIsRegistering()
        {
            var service = new Service
            {
                Id = _serviceId,
                ServiceType = _serviceType,
                Address = _serviceAddress
            };

            var jsonString = JsonConvert.SerializeObject(service);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var httpClient = ApiTestServer.Instance.CreateClient();
            _response = await httpClient.PostAsync("/api/services", httpContent);
        }

        [Then("Response code is")]
        private void ThenResponseCodeIs(int statusCode)
        {
            Assert.Equal(statusCode, (int) _response.StatusCode);
        }

        [Then("Location is returned in headers")]
        private void ThenLocationIsReturnedInHeaders(string location)
        {
            var locationInHeader = _response.Headers.GetValues("location").FirstOrDefault();
            Assert.Equal(location, locationInHeader);
        }

        [Then("Service was registered")]
        private async Task ThenServiceWasRegistered(string serviceId)
        {
            var httpClient = ApiTestServer.Instance.CreateClient();
            var serviceResponse = await httpClient.GetStringAsync($"/api/services/{serviceId}");

            var service = JsonConvert.DeserializeObject<Service>(serviceResponse);
            Assert.NotNull(service);
        }
    }
}