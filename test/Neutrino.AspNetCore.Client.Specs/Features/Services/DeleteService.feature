Feature: Delete service
 
    Neutrino client have to delete service.
 
Scenario: Service have to be deleted by client when correct service id was specified
Given neutrino server is up and running
    And service "old-service" with service type "old-service-type" and address "http://address/" exists
When service "old-service" is being deleted
Then action is successfull
    And service "old-service" not exists