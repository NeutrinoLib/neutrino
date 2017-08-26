using FluentBehave;
using System;
using Neutrino.Entities;
using System.Threading.Tasks;
using Xunit;

namespace Neutrino.Client.Specs
{
    [Feature("AddNewKvPropery", "Add new KV property")]
    public class AddNewKvPropery
    {
        private string _serverAddress;
        private ActionConfirmation<KvProperty> _actionConfirmation;

        [Scenario("KV property have to be added by client when required data was entered")]
        public async Task KVPropertyHaveToBeAddedByClientWhenRequiredDataWasEntered()
        {
            GivenNeutrinoServerIsUpAndRunning();
            await WhenKVPropertyWithRandomKeyAndValueIsBeingAdded();
            ThenActionIsSuccessfull();
            ThenObjectDataWasReturned();
        }

        [Given("Neutrino server is up and running")]
        private void GivenNeutrinoServerIsUpAndRunning()
        {
            _serverAddress = "http://u4neutrino-dev-01.azurewebsites.net";
        }

        [When("KV property with random key and value is being added")]
        private async Task WhenKVPropertyWithRandomKeyAndValueIsBeingAdded()
        {
			var httpRequestService = new HttpRequestService();
			var neutrinoClientOptions = new NeutrinoClientOptions();
			neutrinoClientOptions.Addresses = new string[] { _serverAddress };
			neutrinoClientOptions.SecureToken = "733ecbd2-8d6c-4546-80a8-8e1874dd3889";
			var neutrinoClient = new NeutrinoClient(httpRequestService, neutrinoClientOptions);

            _actionConfirmation = await neutrinoClient.AddKvPropertyAsync(new KvProperty { Key = Guid.NewGuid().ToString(), Value = "Wartosc" });
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
    }
}