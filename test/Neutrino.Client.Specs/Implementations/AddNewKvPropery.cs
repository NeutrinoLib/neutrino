using FluentBehave;
using System;
using System.Threading.Tasks;
using Xunit;
using Neutrino.Entities.Model;
using Neutrino.Entities.Response;

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
            _serverAddress = "http://localhost:5000";
        }

        [When("KV property with random key and value is being added")]
        private async Task WhenKVPropertyWithRandomKeyAndValueIsBeingAdded()
        {
            var neutrinoClientOptions = new NeutrinoClientOptions();
            neutrinoClientOptions.Addresses = new string[] { _serverAddress };
			neutrinoClientOptions.SecureToken = "4e57e961-5f2e-4b24-893f-7842c5ccff97";

			var httpRequestService = new HttpRequestService(neutrinoClientOptions);
			var neutrinoClient = new NeutrinoClient(httpRequestService);

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