# Neutrino

Neutrino is minimalistic service discovery application.

## Continuous integration

| branch  | status |
|---------|--------|
| master  | [![Build status](https://ci.appveyor.com/api/projects/status/n7r9kcrxkec28y2n/branch/master?svg=true)](https://ci.appveyor.com/project/marcinczachurski/neutrino/branch/master) |
| develop | [![Build status](https://ci.appveyor.com/api/projects/status/n7r9kcrxkec28y2n/branch/develop?svg=true)](https://ci.appveyor.com/project/marcinczachurski/neutrino/branch/develop) |

Neutrino API
============
Neutrino provides API to manage registered services and K/V properties.

**Version:** v1

**Authentication:**  
All endpoint's in API requires specify special token. Value of that token is set in appsettings.json. The same token have to be specify in HTTP request in headers section. If in appsettings.json we have that value:

```json
{
  "SecureToken": "0efd0f67-f641-427e-98eb-cd8a07e879c6",
}
```

in HTTP headers we have to send:

```http
Authorization: SecureToken 0efd0f67-f641-427e-98eb-cd8a07e879c6
```

All request without token or with invalid token will be finished with status code: `401 (Unauthorized)`.

### /api/key-values
---
##### ***GET***
**Summary:** Returns all key-value properties.

**Description:** Endpoint returns all registered key-value properties.

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | Success | [ [KvProperty](#kvProperty) ] |

##### ***POST***
**Summary:** Creates new key-value property..

**Description:** Endpoint for creating new key-value property.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| kvProperty | body | Key-value property. | No | [KvProperty](#kvProperty) |

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 201 | Success | [KvProperty](#kvProperty) |
| 400 | Bad Request |

### /api/key-values/{key}
---
##### ***GET***
**Summary:** Returns key-value item by key.

**Description:** Endpoint returns specific key-value property by key.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| key | path | Key of item. | Yes | string |

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | Success | [KvProperty](#kvProperty) |
| 404 | Not Found |

##### ***PUT***
**Summary:** Updates key-value property.

**Description:** Endpoint for updating key-value property.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| key | path | Key-value property key to update. | Yes | string |
| kvProperty | body | New key-value property. | No | [KvProperty](#kvProperty) |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 400 | Bad Request |
| 404 | Not Found |

##### ***DELETE***
**Summary:** Deletes key-value property.

**Description:** Endpoint for deleting key-value property.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| key | path | Key-value property key. | Yes | string |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 404 | Not Found |

### /api/nodes
---
##### ***GET***
**Summary:** Returns list of all defined nodes.

**Description:** Endpoint returns all nodes wich are registered in current node.

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | Success | [ [NodeInfo](#nodeInfo) ] |

### /api/nodes/current
---
##### ***GET***
**Summary:** Returns current node information.

**Description:** Endpoint returns information about current node.

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | Success | [NodeInfo](#nodeInfo) |

### /api/nodes/current/state
---
##### ***GET***
**Summary:** Returns current node state.

**Description:** Endpoint returns current node state. Node can be at one of following state:
- Follower,
- Candidate,
- Leader.

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /api/raft/append-entries
---
##### ***POST***
**Summary:** Append log entries.

**Description:** Endpoint for appending log entries. It's alos used for heartbeats.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| appendEntriesEvent | body | New log data. | No | [AppendEntriesEvent](#appendEntriesEvent) |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /api/raft/request-vote
---
##### ***POST***
**Summary:** Voting for new leader.

**Description:** Endpoint is used during election new leader.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| requestVoteEvent | body | Voting information. | No | [RequestVoteEvent](#requestVoteEvent) |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /api/raft/full-log
---
##### ***GET***
**Summary:** Returns full log.

**Description:** Endpoint is used for retrieve full log from node.

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Success |

### /api/services
---
##### ***GET***
**Summary:** Returns all registered services.

**Description:** Endpoint returns all registered services.

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | Success | [ [Service](#service) ] |

##### ***POST***
**Summary:** Creates new service.

**Description:** Endpoint for registering new service information.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| service | body | Service information. | No | [Service](#service) |

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 201 | Success | [Service](#service) |
| 400 | Bad Request |

### /api/services/{serviceId}
---
##### ***GET***
**Summary:** Returns service by id.

**Description:** Endpoint returns specific service information.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| serviceId | path | Service id. | Yes | string |

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | Success | [Service](#service) |
| 404 | Not Found |

##### ***PUT***
**Summary:** Updates service information.

**Description:** Endpoint for updating service information.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| serviceId | path | Service id to update. | Yes | string |
| service | body | New service information. | No | [Service](#service) |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 400 | Bad Request |
| 404 | Not Found |

##### ***DELETE***
**Summary:** Deletes service.

**Description:** Endpoint for deleting service.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| serviceId | path | Service id. | Yes | string |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Success |
| 404 | Not Found |

### /api/services/{serviceId}/health
---
##### ***GET***
**Summary:** Returns service health.

**Description:** Endpoint returns all health information for specific service.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| serviceId | path | Service id. | Yes | string |

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | Success | [ [ServiceHealth](#serviceHealth) ] |

### /api/services/{serviceId}/health/current
---
##### ***GET***
**Summary:** Returns current service health.

**Description:** Endpoint returns only current healt status for specific endpoint.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| serviceId | path | Service id. | Yes | string |

**Responses**

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | Success | [ServiceHealth](#serviceHealth) |

### Models
---

<a name="kvProperty"></a>**KvProperty**  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| key | string |  | No |
| value | string |  | No |
| createdDate | dateTime |  | No |

<a name="nodeInfo"></a>**NodeInfo**  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| id | string |  | No |
| name | string |  | No |
| address | string |  | No |
| tags | object |  | No |

<a name="appendEntriesEvent"></a>**AppendEntriesEvent**  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| term | integer |  | No |
| leaderNode | [NodeInfo](#nodeInfo) |  | No |
| entries | [ [Entry](#entry) ] |  | No |

<a name="entry"></a>**Entry**  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| objectType | string |  | No |
| method | integer |  | No |
| value | object |  | No |

<a name="requestVoteEvent"></a>**RequestVoteEvent**  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| term | integer |  | No |
| node | [NodeInfo](#nodeInfo) |  | No |

<a name="service"></a>**Service**  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| serviceType | string |  | No |
| address | string |  | No |
| tags | [ string ] |  | No |
| healthCheck | [HealthCheck](#healthCheck) |  | No |
| id | string |  | No |
| createdDate | dateTime |  | No |

<a name="healthCheck"></a>**HealthCheck**  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| healthCheckType | integer |  | No |
| address | string |  | No |
| interval | integer |  | No |
| deregisterCriticalServiceAfter | integer |  | No |

<a name="serviceHealth"></a>**ServiceHealth**  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| healthState | integer |  | No |
| statusCode | integer |  | No |
| responseMessage | string |  | No |
| serviceId | string |  | No |
| id | string |  | No |
| createdDate | dateTime |  | No |