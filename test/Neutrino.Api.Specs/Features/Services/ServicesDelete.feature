Feature: Deleting service
 
    Neutrino have to delete service information.
 
Scenario: Service have to be deleted when user specify service which exists
Given service with id "delete-service-01" name "Delete Service 01" address "http://localhost:8200" and type "None" exists
When service "service-01" is deleting
Then response code is "200"
    And service "service-01" not exists

Scenario: Not found have to be returned when service not exists
Given service with id "delete-service-02" name "Delete Service 02" address "http://localhost:8200" and type "None" exists
When service "not-existed-service" is deleting
Then response code is "404"