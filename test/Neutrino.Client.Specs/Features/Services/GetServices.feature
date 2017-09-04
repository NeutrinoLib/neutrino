Feature: Get services
 
    Neutrino client have to returns services.
 
Scenario: Services have to be returned by client
Given neutrino server is up and running
    And service "service-01" with service type "service-type" and address "http://address1/" exists
    And service "service-02" with service type "service-type" and address "http://address2/" exists
When list of services is being retrieved
Then two services are returned