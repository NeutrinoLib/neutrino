using FluentBehave;
using System;
using Neutrino.Entities.Response;
using System.Threading.Tasks;
using Neutrino.Entities.Model;
using Xunit;

namespace Neutrino.Client.Specs.Implementations.Services
{
    [Feature("UpdateService", "Update service")]
    public class UpdateService
    {
        private NeutrinoClient _neutrinoClient;
        private ActionConfirmation _actionConfirmation;

        [Scenario("Service have to be updated by client when required data was entered")]
        public async Task ServiceHaveToBeUpdatedByClientWhenRequiredDataWasEntered()
        {
            try
            {
                GivenNeutrinoServerIsUpAndRunning();
                await GivenServiceWithServiceTypeAndAddressExists("old-service", "old-service-type", "http://address/");
                await WhenServiceChangeValueServiceTypeToAndAddressTo("old-service", "new-service-type", "http://newaddress/");
                ThenActionIsSuccessfull();
                await ThenServiceHasServiceTypeAndAddress("old-service", "new-service-type", "http://newaddress/");
            }
            finally
            {
                await _neutrinoClient.DeleteServiceAsync("old-service");
            }
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

        [When("Service (.*) change value service type to (.*) and address to (.*)")]
        private async Task WhenServiceChangeValueServiceTypeToAndAddressTo(string serviceId, string serviceType, string address)
        {
            _actionConfirmation = await _neutrinoClient.UpdateServiceAsync(serviceId, new Service { Id = serviceId, ServiceType = serviceType, Address = address });
        }

        [Then("Action is successfull")]
        private void ThenActionIsSuccessfull()
        {
			Assert.NotNull(_actionConfirmation);
			Assert.True(_actionConfirmation.WasSuccessful);
        }

        [Then("Service (.*) has service type (.*) and address (.*)")]
        private async Task ThenServiceHasServiceTypeAndAddress(string serviceId, string serviceType, string address)
        {
            var service = await _neutrinoClient.GetServiceByIdAsync(serviceId);

            Assert.NotNull(service);
            Assert.Equal(serviceType, service.ServiceType);
            Assert.Equal(address, service.Address);
        }
    }
}