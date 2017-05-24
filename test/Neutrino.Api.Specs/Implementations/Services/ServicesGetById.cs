using FluentBehave;
using System;

namespace Neutrino.Api.Specs.Implementations.Services
{
    [Feature("ServicesGetById", "Updating service information")]
    public class ServicesGetById
    {
        [Scenario("Service data have to be returned when user get service by his id")]
        public void ServiceDataHaveToBeReturnedWhenUserGetServiceByHisId()
        {
            GivenServiceWithIdNameAddressAndTypeExists("get-service-01", "Get Service 01", "http://localhost:8200", "None");
            WhenGettingByServiceIdIsExecuted("get-service-01");
            ThenResponseCodeIs(200);
            ThenServiceIdIs("get-service-01");
            ThenServiceNameIs("Get Service 01");
            ThenServiceAddressIs("http://localhost:8200");
        }

        [Scenario("Not found have to be returned when not existed service is downloading")]
        public void NotFoundHaveToBeReturnedWhenNotExistedServiceIsDownloading()
        {
            GivenServiceWithIdNameAddressAndTypeExists("get-service-02", "Get Service 02", "http://localhost:8200", "None");
            WhenGettingByServiceIdIsExecuted("not-existed-service");
            ThenResponseCodeIs(404);
        }

        [Given("Service with id name address and type exists")]
        public void GivenServiceWithIdNameAddressAndTypeExists(string serviceId, string serviceType, string address, string healthCheckType)
        {
            throw new NotImplementedException("Implement me!");
        }

        [When("Getting by service id is executed")]
        private void WhenGettingByServiceIdIsExecuted(string serviceId)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Then("Response code is")]
        private void ThenResponseCodeIs(int responseCode)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Then("Service id is")]
        private void ThenServiceIdIs(string serviceId)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Then("Service name is")]
        private void ThenServiceNameIs(string serviceName)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Then("Service address is")]
        private void ThenServiceAddressIs(string serviceAddress)
        {
            throw new NotImplementedException("Implement me!");
        }

    }
}