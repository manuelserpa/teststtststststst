# Get Flow Information for ERP

## Requirement Specification

Service/API to provide Flow information to ERP.

## Design Specification

### Relevant Artifacts

N/A

### How it works

Considering the Input parameters sent to the service, the returned XML message will contain information related to the following entities:

* Site
* Product
* Flow
* Step

The table below describes the services that are used on this feature.

| Service | Description |
| ------- | ----------- |
| [GetFlowInformationForERP](/cmf.custom.help/techspec>artifacts>services>GetFlowInformationForERP) | Service to provide Flow information to ERP. |

### Assumptions

N/A

## Work items

The table below describes the user stories that affect the current functionality.

| User Story | Type       | Title                                             | Description |
| :--------- | :--------- | :------------------------------------------------ | :---------- |
| 185561     | User Story | Create an API to provide flow information to ERP - Flow outbond interface |  |
