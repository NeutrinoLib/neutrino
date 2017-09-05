using FluentBehave;
using System;
using Neutrino.Entities.Response;
using System.Threading.Tasks;
using Neutrino.Entities.Model;
using Xunit;

namespace Neutrino.AspNetCore.Client.Specs.Implementations.Services
{
    [Feature("DeleteService", "Delete service")]
    public class DeleteService
    {
        private NeutrinoClient _neutrinoClient;
        private ActionConfirmation _actionConfirmation;

        [Scenario("Service have to be deleted by client when correct service id was specified")]
        public async Task ServiceHaveToBeDeletedByClientWhenCorrectServiceIdWasSpecified()
        {
            GivenNeutrinoServerIsUpAndRunning();
            await GivenServiceWithServiceTypeAndAddressExists("old-service", "old-service-type", "http://address/");
            await WhenServiceIsBeingDeleted("old-service");
            ThenActionIsSuccessfull();
            await ThenServiceNotExists("old-service");
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

        [When("Service (.*) is being deleted")]
        private async Task WhenServiceIsBeingDeleted(string serviceId)
        {
            _actionConfirmation = await _neutrinoClient.DeleteServiceAsync(serviceId);
        }

        [Then("Action is successfull")]
        private void ThenActionIsSuccessfull()
        {
			Assert.NotNull(_actionConfirmation);
			Assert.True(_actionConfirmation.WasSuccessful);
        }

        [Then("Service (.*) not exists")]
        private async Task ThenServiceNotExists(string serviceId)
        {
            var service = await _neutrinoClient.GetServiceByIdAsync(serviceId);
            Assert.Null(service);
        }
    }
}