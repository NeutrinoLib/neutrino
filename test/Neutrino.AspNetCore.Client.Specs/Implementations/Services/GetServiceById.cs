using FluentBehave;
using System;
using Neutrino.Entities.Response;
using Neutrino.Entities.Model;
using System.Threading.Tasks;
using Xunit;

namespace Neutrino.AspNetCore.Client.Specs.Implementations.Services
{
    [Feature("GetServiceById", "Get service by id")]
    public class GetServiceById
    {
        private NeutrinoClient _neutrinoClient;
        private Service _service;

        [Scenario("Service have to be returned by client when correct id was specified")]
        public async Task ServiceHaveToBeReturnedByClientWhenCorrectIdWasSpecified()
        {
            try
            {
                GivenNeutrinoServerIsUpAndRunning();
                await GivenServiceWithServiceTypeAndAddressExists("old-service", "old-service-type", "http://address/");
                await WhenServiceIsBeingRetrieved("old-service");
                ThenServiceIsReturned("old-service");
            }
            finally
            {
                await _neutrinoClient.DeleteServiceAsync("old-service");
            }
        }

        [Scenario("Nothing have to be returned by client when not existing service was specified")]
        public async Task NothingHaveToBeReturnedByClientWhenNotExistingServiceWasSpecified()
        {
            GivenNeutrinoServerIsUpAndRunning();
            GivenServiceNotExists("service-not-exists");
            await WhenServiceIsBeingRetrieved("service-not-exists");
            ThenNothingIsReturned();
        }

        [Given("Neutrino server is up and running")]
        private void GivenNeutrinoServerIsUpAndRunning()
        {
            _neutrinoClient = NeutrinoClientFactory.GetNeutrinoClient();
        }

        [Given("Service (.*) with service type (.*) and address (.*) exists")]
        private async Task GivenServiceWithServiceTypeAndAddressExists(string serviceId, string serviceType, string address)
        {
            var result = await _neutrinoClient.AddServiceAsync(new Service { Id = serviceId, ServiceType = serviceType, Address = address });
            Assert.True(result.WasSuccessful);
        }

        [When("Service (.*) is being retrieved")]
        private async Task WhenServiceIsBeingRetrieved(string serviceId)
        {
            _service = await _neutrinoClient.GetServiceByIdAsync(serviceId);
        }

        [Then("Service (.*) is returned")]
        private void ThenServiceIsReturned(string serviceId)
        {
            Assert.NotNull(_service);
            Assert.Equal(serviceId, _service.Id);
        }

        [Given("Service (.*) not exists")]
        private void GivenServiceNotExists(string serviceId)
        {
        }

        [Then("Nothing is returned")]
        private void ThenNothingIsReturned()
        {
            Assert.Null(_service);
        }
    }
}