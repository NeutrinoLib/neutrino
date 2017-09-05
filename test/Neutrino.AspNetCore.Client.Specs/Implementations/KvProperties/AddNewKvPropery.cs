using FluentBehave;
using System;
using System.Threading.Tasks;
using Xunit;
using Neutrino.Entities.Model;
using Neutrino.Entities.Response;

namespace Neutrino.AspNetCore.Client.Specs.Implementations.KvProperties
{
    [Feature("AddNewKvPropery", "Add new KV property")]
    public class AddNewKvPropery
    {
        private NeutrinoClient _neutrinoClient;
        private ActionConfirmation<KvProperty> _actionConfirmation;

        [Scenario("KV property have to be added by client when required data was entered")]
        public async Task KVPropertyHaveToBeAddedByClientWhenRequiredDataWasEntered()
        {
            try
            {
                GivenNeutrinoServerIsUpAndRunning();
                await WhenKVPropertyWithRandomKeyAndValueIsBeingAdded("new-kv-property", "new-value");
                ThenActionIsSuccessfull();
                ThenObjectDataWasReturned();
            }
            finally
            {
                await _neutrinoClient.DeleteKvPropertyAsync("new-kv-property");
            }
        }

        [Scenario("KV property cannot be added when key is not specify")]
        public async Task KVPropertyCannotBeAddedWhenKeyIsNotSpecify()
        {
            GivenNeutrinoServerIsUpAndRunning();
            await WhenKVPropertyWithRandomKeyAndValueIsBeingAdded("", "new-value");
            ThenActionIsFailed();
        }

        [Scenario("KV property cannot be added when key already exists")]
        public async Task KVPropertyCannotBeAddedWhenKeyAlreadyExists()
        {
            try
            {
                GivenNeutrinoServerIsUpAndRunning();
                await GivenKvPropertyWithValueExists("old-kv-property", "old-value");
                await WhenKVPropertyWithRandomKeyAndValueIsBeingAdded("old-kv-property", "new-value");
                ThenActionIsFailed();
            }
            finally
            {
                await _neutrinoClient.DeleteKvPropertyAsync("old-kv-property");
            }
        }

        [Given("Neutrino server is up and running")]
        private void GivenNeutrinoServerIsUpAndRunning()
        {
            _neutrinoClient = NeutrinoClientFactory.GetNeutrinoClient();
        }

        [Given("Kv property (.*) with value (.*) exists")]
        private async Task GivenKvPropertyWithValueExists(string key, string value)
        {
            var result = await _neutrinoClient.AddKvPropertyAsync(new KvProperty { Key = key, Value = value });
            Assert.True(result.WasSuccessful);
        }

        [When("KV property (.*) with value (.*) is beeing added")]
        private async Task WhenKVPropertyWithRandomKeyAndValueIsBeingAdded(string key, string value)
        {
            _actionConfirmation = await _neutrinoClient.AddKvPropertyAsync(new KvProperty { Key = key, Value = value });
        }

        [Then("Action is successfull")]
        private void ThenActionIsSuccessfull()
        {
			Assert.NotNull(_actionConfirmation);
			Assert.True(_actionConfirmation.WasSuccessful);
        }

        [Then("Action is failed")]
        private void ThenActionIsFailed()
        {
			Assert.NotNull(_actionConfirmation);
			Assert.False(_actionConfirmation.WasSuccessful);
        }

        [Then("Object data was returned")]
        private void ThenObjectDataWasReturned()
        {
            Assert.NotNull(_actionConfirmation.ObjectData);
        }
    }
}