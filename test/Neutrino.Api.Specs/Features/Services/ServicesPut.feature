Feature: Updating service information
 
    Neutrino have to update information about service.
 
Scenario: Service type have to updated successfully when new type was specified
Given service with id "service-01" name "Service 01" address "http://localhost:8200" and type "None" exists
    And new service name is "New Service 01"
When service "service-01" is updating
Then response code is "200"
    And service has name "New Service 01"

Scenario: Service cannot be updated when new type is empty
Given service with id "service-02" name "Service 02" address "http://localhost:8200" and type "None" exists
    And new service name is ""
When service "service-02" is updating
Then response code is "400"
    And error message contains message "Service type wasn't specified"

Scenario: Service type have to updated successfully when new address was specified
Given service with id "service-03" name "New Service 03" address "http://localhost:8200" and type "None" exists
    And new service address is "Service 03"
When service "service-03" is updating
Then response code is "200"
    And service has address "New Service 03"

Scenario: Service cannot be updated when new address is empty
Given service with id "service-04" name "Service 04" address "http://localhost:8200" and type "None" exists
    And new service address is ""
When service "service-4" is updating
Then response code is "400"
    And error message contains message "Service adress wasn't specified"

Scenario: After changing healt type from None to HttpRest healt should be checked
Given service with id "service-05" name "Service 05" address "http://httpbin.org/get" and type "None" exists
    And new service health check type is "HttpRest"
When service "service-05" is updating
Then response code is "200"
    And service health is "Passing"

Scenario: Not found have to be returned when not existed service is updating
Given service with id "service-06" name "Service 06" address "http://localhost:8200" and type "None" exists
    And new service health check type is "HttpRest"
When service "not-existed-service" is updating
Then response code is "404"