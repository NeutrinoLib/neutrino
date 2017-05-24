using FluentBehave;
using System;

namespace Neutrino.Api.Specs.Implementations.Services
{
    [Feature("ServicesDelete", "Deleting service")]
    public class ServicesDelete
    {
        [Scenario("Service have to be deleted when user specify service which exists")]
        public void ServiceHaveToBeDeletedWhenUserSpecifyServiceWhichExists()
        {
            GivenServiceWithIdNameAddressAndTypeExists("delete-service-01", "Delete Service 01", "http://localhost:8200", "None");
            WhenServiceIsDeleting("service-01");
            ThenResponseCodeIs(200);
            ThenServiceNotExists("service-01");
        }

        [Scenario("Not found have to be returned when service not exists")]
        public void NotFoundHaveToBeReturnedWhenServiceNotExists()
        {
            GivenServiceWithIdNameAddressAndTypeExists("delete-service-02", "Delete Service 02", "http://localhost:8200", "None");
            WhenServiceIsDeleting("not-existed-service");
            ThenResponseCodeIs(404);
        }

        [Given("Service with id name address and type exists")]
        public void GivenServiceWithIdNameAddressAndTypeExists(string serviceId, string serviceType, string address, string healthCheckType)
        {
            throw new NotImplementedException("Implement me!");
        }

        [When("Service is deleting")]
        private void WhenServiceIsDeleting(string serviceId)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Then("Response code is")]
        private void ThenResponseCodeIs(int responseCode)
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