using FluentBehave;
using System;
using Neutrino.Entities.Model;
using System.Threading.Tasks;
using Xunit;
using Neutrino.Entities.Response;

namespace Neutrino.Client.Specs.Implementations.KvProperties
{
    [Feature("UpdateKvProperty", "Update KV property")]
    public class UpdateKvProperty
    {
        private NeutrinoClient _neutrinoClient;
        private ActionConfirmation _actionConfirmation;

        [Scenario("KV property have to be updated by client when required data was entered")]
        public async Task KVPropertyHaveToBeUpdatedByClientWhenRequiredDataWasEntered()
        {
            try
            {
                GivenNeutrinoServerIsUpAndRunning();
                await GivenKVPropertyWithValueExists("kv-prop", "kv-value");
                await WhenKVPropertyChangeValueTo("kv-prop", "kv-new-value");
                ThenActionIsSuccessfull();
                await ThenKVPropertyHasValue("kv-prop", "kv-new-value");
            }
            finally
            {
                await _neutrinoClient.DeleteKvPropertyAsync("kv-prop");
            }
        }

        [Given("Neutrino server is up and running")]
        private void GivenNeutrinoServerIsUpAndRunning()
        {
            _neutrinoClient = NeutrinoClientFactory.GetNeutrinoClient();
        }

        [Given("KV property (.*) with value (.*) exists")]
        private async Task GivenKVPropertyWithValueExists(string key, string value)
        {
            var result = await _neutrinoClient.AddKvPropertyAsync(new KvProperty { Key = key, Value = value });
            Assert.True(result.WasSuccessful);
        }

        [When("KV property (.*) change value to (.*)")]
        private async Task WhenKVPropertyChangeValueTo(string key, string value)
        {
            _actionConfirmation = await _neutrinoClient.UpdateKvPropertyAsync(key, new KvProperty { Key = key, Value = value });
        }

        [Then("Action is successfull")]
        private void ThenActionIsSuccessfull()
        {
			Assert.NotNull(_actionConfirmation);
			Assert.True(_actionConfirmation.WasSuccessful);
        }

        [Then("KV property (.*) has value (.*)")]
        private async Task ThenKVPropertyHasValue(string key, string value)
        {
            var kvProperty = await _neutrinoClient.GetKvPropertyAsync(key);
            Assert.Equal(value, kvProperty.Value);
        }
    }
}