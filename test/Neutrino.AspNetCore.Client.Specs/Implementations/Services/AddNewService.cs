using FluentBehave;
using System;
using Neutrino.Entities.Response;
using Neutrino.Entities.Model;
using System.Threading.Tasks;
using Xunit;

namespace Neutrino.AspNetCore.Client.Specs.Implementations.Services
{
    [Feature("AddNewService", "Add new service")]
    public class AddNewService
    {
        private NeutrinoClient _neutrinoClient;
        private ActionConfirmation<Service> _actionConfirmation;

        [Scenario("Service have to be added by client when required data was entered")]
        public async Task ServiceHaveToBeAddedByClientWhenRequiredDataWasEntered()
        {
            try
            {
                GivenNeutrinoServerIsUpAndRunning();
                await WhenServiceWithServiceTypeAndAddressIsBeingAdded("new-service", "new-service-type", "http://newaddress/");
                ThenActionIsSuccessfull();
                ThenObjectDataWasReturned();
            }
            finally
            {
                await _neutrinoClient.DeleteServiceAsync("new-service");
            }
        }

        [Scenario("Service cannot be added when service id is not specified")]
        public async Task ServiceCannotBeAddedWhenServiceIdIsNotSpecified()
        {
            GivenNeutrinoServerIsUpAndRunning();
            await WhenServiceWithServiceTypeAndAddressIsBeingAdded("", "new-service-type", "http://newaddress/");
            ThenActionIsFailing();
        }

        [Scenario("Service cannot be added when service type is not specified")]
        public async Task ServiceCannotBeAddedWhenServiceTypeIsNotSpecified()
        {
            GivenNeutrinoServerIsUpAndRunning();
            await WhenServiceWithServiceTypeAndAddressIsBeingAdded("new-service", "", "http://newaddress/");
            ThenActionIsFailing();
        }

        [Scenario("Service cannot be added when address is not specified")]
        public async Task ServiceCannotBeAddedWhenAddressIsNotSpecified()
        {
            GivenNeutrinoServerIsUpAndRunning();
            await WhenServiceWithServiceTypeAndAddressIsBeingAdded("new-service", "new-service-type", "");
            ThenActionIsFailing();
        }

        [Scenario("Service cannot be added when service with the same id already exists")]
        public async Task ServiceCannotBeAddedWhenServiceWithTheSameIdAlreadyExists()
        {
            try
            {
                GivenNeutrinoServerIsUpAndRunning();
                await GivenServiceWithServiceTypeAndAddressExists("old-service", "old-service-type", "http://newaddress/");
                await WhenServiceWithServiceTypeAndAddressIsBeingAdded("old-service", "new-service-type", "http://newaddress/");
                ThenActionIsFailing();
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

        [When("Service (.*) with service type (.*) and address (.*) is being added")]
        private async Task WhenServiceWithServiceTypeAndAddressIsBeingAdded(string serviceId, string serviceType, string address)
        {
            _actionConfirmation = await _neutrinoClient.AddServiceAsync(new Service { Id = serviceId, ServiceType = serviceType, Address = address });
        }

        [Then("Action is successfull")]
        private void ThenActionIsSuccessfull()
        {
			Assert.NotNull(_actionConfirmation);
			Assert.True(_actionConfirmation.WasSuccessful);
        }

        [Then("Object data was returned")]
        private void ThenObjectDataWasReturned()
        {
            Assert.NotNull(_actionConfirmation.ObjectData);
        }

        [Then("Action is failing")]
        private void ThenActionIsFailing()
        {
			Assert.NotNull(_actionConfirmation);
			Assert.False(_actionConfirmation.WasSuccessful);
        }

        [Given("Service (.*) with service type (.*) and address (.*) exists")]
        private async Task GivenServiceWithServiceTypeAndAddressExists(string serviceId, string serviceType, string address)
        {
            var result = await _neutrinoClient.AddServiceAsync(new Service { Id = serviceId, ServiceType = serviceType, Address = address });
            Assert.True(result.WasSuccessful);
        }
    }
}