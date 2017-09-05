Feature: Update KV property
 
    Neutrino client have to update KV property.
 
Scenario: KV property have to be updated by client when required data was entered
Given neutrino server is up and running
    And KV property "kv-prop" with value "kv-value" exists
When KV property "kv-prop" change value to "kv-new-value"
Then action is successfull
    And KV property "kv-prop" has value "kv-new-value"