using FluentBehave;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using Neutrino.Entities.Model;
using System.Linq;

namespace Neutrino.AspNetCore.Client.Specs.Implementations.Services
{
    [Feature("GetServicesByTags", "Get services by tags")]
    public class GetServicesByTags
    {
        private NeutrinoClient _neutrinoClient;
        private IList<Service> _services;

        [Scenario("Services with proper tags have to be returned by client")]
        public async Task ServicesWithProperTagsHaveToBeReturnedByClient()
        {
            try
            {
                GivenNeutrinoServerIsUpAndRunning();
                await GivenServiceWithServiceTypeAndAddressExists("service-01", "service-type", "http://address1", "tag1");
                await GivenServiceWithServiceTypeAndAddressExists("service-02", "service-type", "http://address2", "tag2");
                await GivenServiceWithServiceTypeAndAddressExists("service-03", "service-type", "http://address3", "tag2");
                await WhenServicesWithTagAreBeingRetrieved("tag2");
                ThenServiceAndServiceAreReturned("service-02", "service-03");
            }
            finally
            {
                await _neutrinoClient.DeleteServiceAsync("service-01");
                await _neutrinoClient.DeleteServiceAsync("service-02");
                await _neutrinoClient.DeleteServiceAsync("service-03");
            }
        }

        [Given("Neutrino server is up and running")]
        private void GivenNeutrinoServerIsUpAndRunning()
        {
            _neutrinoClient = NeutrinoClientFactory.GetNeutrinoClient();
        }

        [Given("Service (.*) with service type (.*) and address (.*) and tag (.*) exists")]
        private async Task GivenServiceWithServiceTypeAndAddressExists(string serviceId, string serviceType, string address, string tag)
        {
            var result = await _neutrinoClient.AddServiceAsync(new Service 
            { 
                Id = serviceId, ServiceType = serviceType, Address = address, Tags = new[] { tag } 
            });
            Assert.True(result.WasSuccessful);
        }

        [When("Services with tag (.*) are being retrieved")]
        private async Task WhenServicesWithTagAreBeingRetrieved(string tag)
        {
            _services = await _neutrinoClient.GetServicesByTagsAsync(tag);
        }

        [Then("Service (.*) and service (.*) are returned")]
        private void ThenServiceAndServiceAreReturned(string service1, string service2)
        {
            Assert.NotNull(_services);
            Assert.True(_services.Any(x => x.Id == service1));
            Assert.True(_services.Any(x => x.Id == service2));
        }
    }
}