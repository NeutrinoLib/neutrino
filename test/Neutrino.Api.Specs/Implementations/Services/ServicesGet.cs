using FluentBehave;
using System;

namespace Neutrino.Api.Specs.Implementations.Services
{
    [Feature("ServicesGet", "Getting informaation about all registered services")]
    public class ServicesGet
    {
        [Scenario("Information about all registered services have to be returned")]
        public void InformationAboutAllRegisteredServicesHaveToBeReturned()
        {
            GivenServiceWithIdNameAddressAndTypeExists("get-service-01", "Get Service 01", "http://localhost:8200", "None");
            GivenServiceWithIdNameAddressAndTypeExists("get-service-02", "Get Service 02", "http://localhost:8200", "None");
            WhenServiceListOfServicesIsDownloaded();
            ThenResponseCodeIs(200);
            ThenListContainsService("get-service-01");
            ThenServiceNotExists("get-service-02");
        }

        [Given("Service with id name address and type exists")]
        public void GivenServiceWithIdNameAddressAndTypeExists(string serviceId, string serviceType, string address, string healthCheckType)
        {
            throw new NotImplementedException("Implement me!");
        }

        [When("Service list of services is downloaded")]
        private void WhenServiceListOfServicesIsDownloaded()
        {
            throw new NotImplementedException("Implement me!");
        }

        [Then("Response code is")]
        private void ThenResponseCodeIs(int responseCode)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Then("List contains service")]
        private void ThenListContainsService(string p0)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Then("Service not exists")]
        private void ThenServiceNotExists(string p0)
        {
            throw new NotImplementedException("Implement me!");
        }
    }
}