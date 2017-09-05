Feature: Get KV properties
 
    Neutrino client have to returns KV properties.
 
Scenario: KV properties have to be returned by client
Given neutrino server is up and running
    And KV property "kv-prop-01" with value "kv-value-get-01" exists
    And KV property "kv-prop-02" with value "kv-value-get-02" exists
When list of KV properties is being retrieved
Then two KV properties are returned