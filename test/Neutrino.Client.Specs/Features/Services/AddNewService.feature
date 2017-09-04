Feature: Add new service
 
    Neutrino client have to add new service.
 
Scenario: Service have to be added by client when required data was entered
Given neutrino server is up and running
When service "new-service" with service type "new-service-type" and address "http://newaddress/" is being added
Then action is successfull
    And object data was returned

Scenario: Service cannot be added when service id is not specified
Given neutrino server is up and running
When service "" with service type "new-service-type" and address "http://newaddress/" is being added
Then action is failing

Scenario: Service cannot be added when service type is not specified
Given neutrino server is up and running
When service "new-service" with service type "" and address "http://newaddress/" is being added
Then action is failing

Scenario: Service cannot be added when address is not specified
Given neutrino server is up and running
When service "new-service" with service type "new-service-type" and address "" is being added
Then action is failing

Scenario: Service cannot be added when service with the same id already exists
Given neutrino server is up and running
    And service "old-service" with service type "old-service-type" and address "http://oldaddress/" exists
When service "old-service" with service type "new-service-type" and address "http://newaddress/" is being added
Then action is failing
