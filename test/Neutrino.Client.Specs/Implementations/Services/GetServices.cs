using FluentBehave;
using System;
using System.Threading.Tasks;
using Xunit;
using Neutrino.Entities.Model;
using System.Collections.Generic;

namespace Neutrino.Client.Specs.Implementations.Services
{
    [Feature("GetServices", "Get services")]
    public class GetServices
    {
        private NeutrinoClient _neutrinoClient;
        private IList<Service> _services;
        
        [Scenario("Services have to be returned by client")]
        public async Task ServicesHaveToBeReturnedByClient()
        {
            try
            {
                GivenNeutrinoServerIsUpAndRunning();
                await GivenServiceWithServiceTypeAndAddressExists("service-01", "service-type", "http://address1/");
                await GivenServiceWithServiceTypeAndAddressExists("service-02", "service-type", "http://address2/");
                await WhenListOfServicesIsBeingRetrieved();
                ThenTwoServicesAreReturned();
            }
            finally
            {
                await _neutrinoClient.DeleteServiceAsync("service-01");
                await _neutrinoClient.DeleteServiceAsync("service-02");
            }
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

        [When("List of services is being retrieved")]
        private async Task WhenListOfServicesIsBeingRetrieved()
        {
            _services = await _neutrinoClient.GetServicesAsync();
        }

        [Then("Two services are returned")]
        private void ThenTwoServicesAreReturned()
        {
            Assert.Equal(2, _services.Count);
        }
    }
}