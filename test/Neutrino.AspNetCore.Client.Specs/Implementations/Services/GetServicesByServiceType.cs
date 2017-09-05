using FluentBehave;
using System;
using System.Collections.Generic;
using Neutrino.Entities.Model;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace Neutrino.AspNetCore.Client.Specs.Implementations.Services
{
    [Feature("GetServicesByServiceType", "Get services by service type")]
    public class GetServicesByServiceType
    {
        private NeutrinoClient _neutrinoClient;
        private IList<Service> _services;

        [Scenario("Proper services have to be returned by client when we want to retrieve services by service type")]
        public async Task ProperServicesHaveToBeReturnedByClientWhenWeWantToRetrieveServicesByServiceType()
        {
            try
            {
                GivenNeutrinoServerIsUpAndRunning();
                await GivenServiceWithServiceTypeAndAddressExists("service-01", "service-type-a", "http://address1/");
                await GivenServiceWithServiceTypeAndAddressExists("service-02", "service-type-b", "http://address2/");
                await GivenServiceWithServiceTypeAndAddressExists("service-03", "service-type-b", "http://address3/");
                await WhenServicesWithServiceTypeAreBeingRetrieved("service-type-b");
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

        [Given("Service (.*) with service type (.*) and address (.*) exists")]
        private async Task GivenServiceWithServiceTypeAndAddressExists(string serviceId, string serviceType, string address)
        {
            var result = await _neutrinoClient.AddServiceAsync(new Service { Id = serviceId, ServiceType = serviceType, Address = address });
            Assert.True(result.WasSuccessful);
        }

        [When("Services with service type (.*) are being retrieved")]
        private async Task WhenServicesWithServiceTypeAreBeingRetrieved(string serviceType)
        {
            _services = await _neutrinoClient.GetServicesByServiceTypeAsync(serviceType);
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