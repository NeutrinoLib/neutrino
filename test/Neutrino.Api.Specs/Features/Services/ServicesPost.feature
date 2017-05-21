Feature: Registering new service
 
    Neutrino have to register new services if the have all required properties.
 
Scenario: Service have to be registered successfully when all required properties are given
Given service with id "new-service-01"
    And service with service type "New Service 01"
When service is registering
Then response code is "201"
    And location "api/services/new-service-01" is returned in headers
    And service "new-service-01" was registered
