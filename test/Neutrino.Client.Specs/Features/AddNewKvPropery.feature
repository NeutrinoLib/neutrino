Feature: Add new KV property
 
    Neutrino client have to add new KV property.
 
Scenario: KV property have to be added by client when required data was entered
Given neutrino server is up and running
When KV property with random key and value is being added
Then action is successfull
    And object data was returned