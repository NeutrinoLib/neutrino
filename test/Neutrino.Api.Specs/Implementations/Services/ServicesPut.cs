using FluentBehave;
using System;

namespace Neutrino.Api.Specs.Implementations.Services
{
    [Feature("ServicesPut", "Updating service information")]
    public class ServicesPut
    {
        [Scenario("Service type have to updated successfully when new type was specified")]
        public void ServiceTypeHaveToUpdatedSuccessfullyWhenNewTypeWasSpecified()
        {
            GivenServiceWithIdNameAddressAndTypeExists("service-01", "Service 01", "http://localhost:8200", "None");
            GivenNewServiceNameIs("New Service 01");
            WhenServiceIsUpdating("service-01");
            ThenResponseCodeIs(200);
            ThenServiceHasName("New Service 01");
        }

        [Scenario("Service cannot be updated when new type is empty")]
        public void ServiceCannotBeUpdatedWhenNewTypeIsEmpty()
        {
            GivenServiceWithIdNameAddressAndTypeExists("service-02", "Service 02", "http://localhost:8200", "None");
            GivenNewServiceNameIs("");
            WhenServiceIsUpdating("service-02");
            ThenResponseCodeIs(400);
            ThenErrorMessageContainsMessage("Service type wasn't specified");
        }

        [Scenario("Service type have to updated successfully when new address was specified")]
        public void ServiceTypeHaveToUpdatedSuccessfullyWhenNewAddressWasSpecified()
        {
            GivenServiceWithIdNameAddressAndTypeExists("service-03", "Service 03", "http://localhost:8200", "None");
            GivenNewServiceAddressIs("Service 03");
            WhenServiceIsUpdating("service-03");
            ThenResponseCodeIs(200);
            ThenServiceHasAddress("New Service 03");
        }

        [Scenario("Service cannot be updated when new address is empty")]
        public void ServiceCannotBeUpdatedWhenNewAddressIsEmpty()
        {
            GivenServiceWithIdNameAddressAndTypeExists("service-04", "Service 04", "http://localhost:8200", "None");
            GivenNewServiceAddressIs("");
            WhenServiceIsUpdating("service-4");
            ThenResponseCodeIs(400);
            ThenErrorMessageContainsMessage("Service adress wasn't specified");
        }

        [Scenario("After changing healt type from None to HttpRest healt should be checked")]
        public void AfterChangingHealtTypeFromNoneToHttpRestHealtShouldBeChecked()
        {
            GivenServiceWithIdNameAddressAndTypeExists("service-05", "Service 05", "http://httpbin.org/get", "None");
            GivenNewServiceHealthCheckTypeIs("HttpRest");
            WhenServiceIsUpdating("service-05");
            ThenResponseCodeIs(200);
            ThenServiceHealthIs("Passing");
        }

        [Scenario("Not found have to be returned when not existed service is updating")]
        public void NotFoundHaveToBeReturnedWhenNotExistedServiceIsUpdating()
        {
            GivenServiceWithIdNameAddressAndTypeExists("service-06", "Service 06", "http://localhost:8200", "None");
            GivenNewServiceHealthCheckTypeIs("HttpRest");
            WhenServiceIsUpdating("not-existed-service");
            ThenResponseCodeIs(404);
        }

        [Given("Service with id name address and type exists")]
        public void GivenServiceWithIdNameAddressAndTypeExists(string serviceId, string serviceType, string address, string healthCheckType)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Given("New service name is")]
        private void GivenNewServiceNameIs(string serviceName)
        {
            throw new NotImplementedException("Implement me!");
        }

        [When("Service is updating")]
        private void WhenServiceIsUpdating(string serviceId)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Then("Response code is")]
        private void ThenResponseCodeIs(int responseCode)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Then("Service has name")]
        private void ThenServiceHasName(string serviceName)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Then("Error message contains message")]
        private void ThenErrorMessageContainsMessage(string errorMessage)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Given("New service address is")]
        private void GivenNewServiceAddressIs(string serviceAddress)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Then("Service has address")]
        private void ThenServiceHasAddress(string serviceAddress)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Given("New service health check type is")]
        private void GivenNewServiceHealthCheckTypeIs(string healtCheckType)
        {
            throw new NotImplementedException("Implement me!");
        }

        [Then("Service health is")]
        private void ThenServiceHealthIs(string serviceHealth)
        {
            throw new NotImplementedException("Implement me!");
        }

    }
}