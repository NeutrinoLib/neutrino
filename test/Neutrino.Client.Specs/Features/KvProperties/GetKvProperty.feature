Feature: Get KV property
 
    Neutrino client have to returns KV property.
 
Scenario: KV property have to be returned by client when correct key was specified
Given neutrino server is up and running
    And KV property "kv-prop" with value "kv-value-get" exists
When KV property "kv-prop" is being retrieved
Then KV property "kv-prop" is returned

Scenario: Nothing have to be returned by client when not existing key was specified
Given neutrino server is up and running
    And KV property "kv-prop-not-exists" not exists
When KV property "kv-prop" is being retrieved
Then nothing is returned