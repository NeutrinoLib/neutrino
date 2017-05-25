using FluentBehave;
using System;
using System.Threading.Tasks;
using Neutrino.Entities;
using Newtonsoft.Json;
using System.Net.Http;
using Neutrino.Api.Specs.Infrastructure;
using System.Text;
using Xunit;
using System.Net;

namespace Neutrino.Api.Specs.Implementations.Services
{
    [Feature("ServicesDelete", "Deleting service")]
    public class ServicesDelete
    {
        private string _serviceId;
        private string _serviceType;
        private string _serviceAddress;
        private HealthCheckType _healthCheckType;
        private HttpResponseMessage _response;

        [Scenario("Service have to be deleted when user specify service which exists")]
        public async Task ServiceHaveToBeDeletedWhenUserSpecifyServiceWhichExists()
        {
            await GivenServiceWithIdNameAddressAndTypeExists("delete-service-01", "Delete Service 01", "http://localhost:8200", "None");
            await WhenServiceIsDeleting("delete-service-01");
            ThenResponseCodeIs(200);
            await ThenServiceNotExists("delete-service-01");
        }

        [Scenario("Not found have to be returned when service not exists")]
        public async Task NotFoundHaveToBeReturnedWhenServiceNotExists()
        {
            await GivenServiceWithIdNameAddressAndTypeExists("delete-service-02", "Delete Service 02", "http://localhost:8200", "None");
            await WhenServiceIsDeleting("not-existed-service");
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

        [When("Service (.*) is deleting")]
        private async Task WhenServiceIsDeleting(string serviceId)
        {
            var httpClient = ApiTestServer.Instance.CreateClient();
            _response = await httpClient.DeleteAsync($"/api/services/{serviceId}");
        }

        [Then("Response code is (.*)")]
        private void ThenResponseCodeIs(int statusCode)
        {
            Assert.Equal(statusCode, (int) _response.StatusCode);
        }

        [Then("Service (.*) not exists")]
        private async Task ThenServiceNotExists(string serviceId)
        {
            var httpClient = ApiTestServer.Instance.CreateClient();
            var response = await httpClient.GetAsync($"/api/services/{serviceId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

            var httpClient = ApiTestServer.Instance.CreateClient();
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