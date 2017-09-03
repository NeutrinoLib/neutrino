Feature: Delete KV property
 
    Neutrino client have to delete KV property.
 
Scenario: KV property have to be deleted by client when correct key was specified
Given neutrino server is up and running
    And KV property "kv-prop" with value "kv-value-delete" exists
When KV property "kv-prop" is being deleted
Then action is successfull
    And KV property "kv-prop" not exists