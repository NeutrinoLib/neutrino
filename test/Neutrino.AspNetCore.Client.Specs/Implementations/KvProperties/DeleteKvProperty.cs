using FluentBehave;
using System;
using System.Threading.Tasks;
using Xunit;
using Neutrino.Entities.Model;
using Neutrino.Entities.Response;

namespace Neutrino.AspNetCore.Client.Specs.Implementations.KvProperties
{
    [Feature("DeleteKvProperty", "Delete KV property")]
    public class DeleteKvProperty
    {
        private NeutrinoClient _neutrinoClient;
        private ActionConfirmation _actionConfirmation;

        [Scenario("KV property have to be deleted by client when correct key was specified")]
        public async Task KVPropertyHaveToBeDeletedByClientWhenCorrectKeyWasSpecified()
        {
            GivenNeutrinoServerIsUpAndRunning();
            await GivenKVPropertyWithValueExists("kv-prop", "kv-value-delete");
            await WhenKVPropertyIsBeingDeleted("kv-prop");
            ThenActionIsSuccessfull();
            await ThenKVPropertyNotExists("kv-prop");
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

        [When("KV property (.*) is being deleted")]
        private async Task WhenKVPropertyIsBeingDeleted(string key)
        {
            _actionConfirmation = await _neutrinoClient.DeleteKvPropertyAsync(key);
        }

        [Then("Action is successfull")]
        private void ThenActionIsSuccessfull()
        {
			Assert.NotNull(_actionConfirmation);
			Assert.True(_actionConfirmation.WasSuccessful);
        }

        [Then("KV property (.*) not exists")]
        private async Task ThenKVPropertyNotExists(string key)
        {
            var result = await _neutrinoClient.GetKvPropertyAsync(key);
            Assert.Null(result);
        }
    }
}