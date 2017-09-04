using FluentBehave;
using System;
using System.Threading.Tasks;
using Neutrino.Entities.Model;
using Xunit;
using System.Collections.Generic;

namespace Neutrino.Client.Specs.Implementations.KvProperties
{
    [Feature("GetKVProperties", "Get KV properties")]
    public class GetKVProperties
    {
        private NeutrinoClient _neutrinoClient;
        private IList<KvProperty> _kvProperties;

        [Scenario("KV properties have to be returned by client")]
        public async Task KVPropertiesHaveToBeReturnedByClient()
        {
            try
            {
                GivenNeutrinoServerIsUpAndRunning();
                await GivenKVPropertyWithValueExists("kv-prop-01", "kv-value-get-01");
                await GivenKVPropertyWithValueExists("kv-prop-02", "kv-value-get-02");
                await WhenListOfKVPropertiesIsBeingRetrieved();
                ThenTwoKVPropertiesAreReturned();
            }
            finally 
            {
                await _neutrinoClient.DeleteKvPropertyAsync("kv-prop-01");
                await _neutrinoClient.DeleteKvPropertyAsync("kv-prop-02");
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

        [When("List of KV properties is being retrieved")]
        private async Task WhenListOfKVPropertiesIsBeingRetrieved()
        {
            _kvProperties = await _neutrinoClient.GetKvPropertiesAsync();
        }

        [Then("Two KV properties is returned")]
        private void ThenTwoKVPropertiesAreReturned()
        {
            Assert.Equal(2, _kvProperties.Count);
        }
    }
}