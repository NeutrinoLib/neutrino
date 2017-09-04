Feature: Get service by id
 
    Neutrino client have to returns service.
 
Scenario: Service have to be returned by client when correct id was specified
Given neutrino server is up and running
    And service "old-service" with service type "old-service-type" and address "http://address/" exists
When service "old-service" is being retrieved
Then service "old-service" is returned

Scenario: Nothing have to be returned by client when not existing service was specified
Given neutrino server is up and running
    And service "service-not-exists" not exists
When service "service-not-exists" is being retrieved
Then nothing is returned