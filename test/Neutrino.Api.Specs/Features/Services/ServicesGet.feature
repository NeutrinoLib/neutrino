Feature: Getting informaation about all registered services
 
    Neutrino have to return information about all registered services.
 
Scenario: Information about all registered services have to be returned
Given service with id "get-service-01" name "Get Service 01" address "http://localhost:8200" and type "None" exists
    And service with id "get-service-02" name "Get Service 02" address "http://localhost:8200" and type "None" exists
When list of services is downloaded
Then response code is "200"
    And list contains service "get-service-01"
    And list contains service "get-service-02"