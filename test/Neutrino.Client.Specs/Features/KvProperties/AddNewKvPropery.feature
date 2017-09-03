Feature: Add new KV property
 
    Neutrino client have to add new KV property.
 
Scenario: KV property have to be added by client when required data was entered
Given neutrino server is up and running
When KV property "new-kv-property" with value "new-value" is being added
Then action is successfull
    And object data was returned

Scenario: KV property cannot be added when key is not specify
Given neutrino server is up and running
When KV property "" with value "new-value" is being added
Then action is failed

Scenario: KV property cannot be added when key already exists
Given neutrino server is up and running
    And Kv property "old-kv-property" with value "old-value" exists
When KV property "old-kv-property"" with value "new-value" is being added
Then action is failed
