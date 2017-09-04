Feature: Update service
 
    Neutrino client have to update service
 
Scenario: Service have to be updated by client when required data was entered
Given neutrino server is up and running
    And service "old-service" with service type "old-service-type" and address "http://address/" exists
When service "old-service" change value service type to "new-service-type" and address to "http://newaddress/"
Then action is successfull
    And service "old-service" has service type "new-service-type" and address "http://newaddress/"