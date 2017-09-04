Feature: Get services by service type
 
    Neutrino client have to returns services by service type.
 
Scenario: Proper services have to be returned by client when we want to retrieve services by service type
Given neutrino server is up and running
    And service "service-01" with service type "service-type-a" and address "http://address1/" exists
    And service "service-02" with service type "service-type-b" and address "http://address1/" exists
    And service "service-03" with service type "service-type-b" and address "http://address3/" exists
When services with service type "service-type-b" are being retrieved
Then service "service-02" and service "service-03" are returned