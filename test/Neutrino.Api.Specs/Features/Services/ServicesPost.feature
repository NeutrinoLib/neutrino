Feature: Registering new service
 
    Neutrino have to register new services if the have all required properties.
 
Scenario: Service have to be registered successfully when all required properties are given
Given service with id "new-service-01"
    And service with service type "New Service 01"
    And service with address "http://localhost:8200"
    And service health check type is "None"
When service is registering
Then response code is "201"
    And location "api/services/new-service-01" is returned in headers
    And service "new-service-01" was registered

Scenario: Service cannot be registered when id is not specified
Given service with id ""
    And service with service type "New Service 02"
    And service with address "http://localhost:8200"
    And service health check type is "None"
When service is registering
Then response code is "400"
    And error message contains message "Service id wasn't specified"

Scenario: Service cannot be registered when type is not specified
Given service with id "new-service-03"
    And service with service type ""
    And service with address "http://localhost:8200"
    And service health check type is "None"
When service is registering
Then response code is "400"
    And error message contains message "Service type wasn't specified"

Scenario: Service cannot be registered when address is not specified
Given service with id "new-service-04"
    And service with service type "New Service 04"
    And service with address ""
    And service health check type is "None"
When service is registering
Then response code is "400"
    And error message contains message "Service adress wasn't specified"

Scenario: After registering helth of service should be passing when service is alive
Given service with id "new-service-05"
    And service with service type "New Service 05"
    And service with address "http://httpbin.org"
    And service health check type is "HttpRest"
    And helth endpoint is "http://httpbin.org/get"
    And helth interval is "30"
    And deregistering critical service is "60"
When service is registering
Then response code is "201"
    And service health is "Passing"

Scenario: After registering helth of service should be unknown when health check type is None
Given service with id "new-service-06"
    And service with service type "New Service 06"
    And service with address "http://httpbin.org"
    And service health check type is "None"
    And helth endpoint is "http://httpbin.org/get"
    And helth interval is "30"
When service is registering
Then response code is "201"
    And service health is "Unknown"

Scenario: After registering helth of service should be error when service return 400
Given service with id "new-service-07"
    And service with service type "New Service 07"
    And service with address "http://httpbin.org"
    And service health check type is "HttpRest"
    And helth endpoint is "http://httpbin.org/status/400"
    And helth interval is "30"
    And deregistering critical service is "60"
When service is registering
Then response code is "201"
    And service health is "Error"

Scenario: After registering helth of service should be critical when service is not responding
Given service with id "new-service-08"
    And service with service type "New Service 08"
    And service with address "http://notexistingaddress-qazwsx123.org"
    And service health check type is "HttpRest"
    And helth endpoint is "http://notexistingaddress-qazwsx123.org/health"
    And helth interval is "30"
    And deregistering critical service is "60"
When service is registering
Then response code is "201"
    And service health is "Critical"

Scenario: Service cannot be registered when service with same id exists
Given service with id "new-service-09" name "New Service 01" address "http://localhost:8200" and type "None" exists
When service with id "new-service-09" is registering
Then response code is "400"
    And error message contains message "Service with id 'new-service-09' already exists."

Scenario: Service cannot be registered when id contains unacceptable characters
Given service with id "this $%^ service"
    And service with service type "New Service 10"
    And service with address "http://localhost:8200"
    And service health check type is "None"
When service is registering
Then response code is "400"
    And error message contains message "Service id contains unacceptable characters (only alphanumeric letters and dash is acceptable)."