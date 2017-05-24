Feature: Updating service information
 
    Neutrino have return information about specific service.
 
Scenario: Service data have to be returned when user get service by his id
Given service with id "get-service-01" name "Get Service 01" address "http://localhost:8200" and type "None" exists
When getting by service id "get-service-01" is executed
Then response code is "200"
    And service id is "get-service-01"
    And service name is "Get Service 01"
    And service address is "http://localhost:8200"

Scenario: Not found have to be returned when not existed service is downloading
Given service with id "get-service-02" name "Get Service 02" address "http://localhost:8200" and type "None" exists
When getting by service id "not-existed-service" is executed
Then response code is "404"