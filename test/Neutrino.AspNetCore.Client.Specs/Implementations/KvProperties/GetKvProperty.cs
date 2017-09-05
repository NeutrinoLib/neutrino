using FluentBehave;
using System;
using Neutrino.Entities.Response;
using System.Threading.Tasks;
using Xunit;
using Neutrino.Entities.Model;

namespace Neutrino.AspNetCore.Client.Specs.Implementations.KvProperties
{
    [Feature("GetKvProperty", "Get KV property")]
    public class GetKvProperty
    {
        private NeutrinoClient _neutrinoClient;
        private KvProperty _kvProperty;
        
        [Scenario("KV property have to be returned by client when correct key was specified")]
        public async Task KVPropertyHaveToBeReturnedByClientWhenCorrectKeyWasSpecified()
        {
            try
            {
                GivenNeutrinoServerIsUpAndRunning();
                await GivenKVPropertyWithValueExists("kv-prop", "kv-value-get");
                await WhenKVPropertyIsBeingRetrieved("kv-prop");
                ThenKVPropertyIsReturned("kv-prop");
            }
            finally
            {
                await _neutrinoClient.DeleteKvPropertyAsync("kv-prop");
            }
        }

        [Scenario("Nothing have to be returned by client when not existing key was specified")]
        public async Task NothingHaveToBeReturnedByClientWhenNotExistingKeyWasSpecified()
        {
            GivenNeutrinoServerIsUpAndRunning();
            GivenKVPropertyNotExists("kv-prop-not-exists");
            await WhenKVPropertyIsBeingRetrieved("kv-prop");
            ThenNothingIsReturned();
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

        [When("KV property (.*) is being retrieved")]
        private async Task WhenKVPropertyIsBeingRetrieved(string key)
        {
            _kvProperty = await _neutrinoClient.GetKvPropertyAsync(key);
        }

        [Then("KV property is returned")]
        private void ThenKVPropertyIsReturned(string p0)
        {
            Assert.NotNull(_kvProperty);
            Assert.Equal("kv-prop", _kvProperty.Key);
            Assert.Equal("kv-value-get", _kvProperty.Value);
        }

        [Given("KV property (.*) not exists")]
        private void GivenKVPropertyNotExists(string key)
        {
        }

        [Then("Nothing is returned")]
        private void ThenNothingIsReturned()
        {
            Assert.Null(_kvProperty);
        }
    }
}