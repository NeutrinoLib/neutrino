Feature: Get services by tags
 
    Neutrino client have to returns service by his tags.
 
Scenario: Services with proper tags have to be returned by client
Given neutrino server is up and running
    And service "service-01" with service type "service-type" and address "http://address1/" and tag "tag1" exists
    And service "service-02" with service type "service-type" and address "http://address1/" and tag "tag2" exists
    And service "service-03" with service type "service-type" and address "http://address3/" and tag "tag2" exists
When services with tag "tag2" are being retrieved
Then service "service-02" and service "service-03" are returned