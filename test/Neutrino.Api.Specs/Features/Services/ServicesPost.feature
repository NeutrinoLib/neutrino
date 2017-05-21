Feature: Registering new service
 
    Neutrino have to register new services if the have all required properties.
 
Scenario: Service have to be registered successfully when all required properties are given
Given service with id "new-service-01"
    And service with service type "New Service 01"
    And service with address "http://localhost:8200"
When service is registering
Then response code is "201"
    And location "api/services/new-service-01" is returned in headers
    And service "new-service-01" was registered

Scenario: Service cannot be registered when id is not specified
Given service with id ""
    And service with service type "New Service 02"
    And service with address "http://localhost:8200"
When service is registering
Then response code is "400"
    And error message contains message "Service id wasn't specified"

Scenario: Service cannot be registered when type is not specified
Given service with id "new-service-03"
    And service with service type ""
    And service with address "http://localhost:8200"
When service is registering
Then response code is "400"
    And error message contains message "Service type wasn't specified"

Scenario: Service cannot be registered when address is not specified
Given service with id "new-service-04"
    And service with service type "New Service 02"
    And service with address ""
When service is registering
Then response code is "400"
    And error message contains message "Service adress wasn't specified"