# Custom Import Production Orders 

## Requirement Specification

Feature to receive information regarding Production Orders. It is required that the system receives information and then creates or updates the corresponding **ProductionOrders**.

## Design Specification

### Relevant Artifacts
The table below describes the artifacts for this entity type:

| Name                            | Type        | Is Mandatory | Data Type      | Description  |
| :------------------------------ | :---------- | :----------: | :------------- | :----------- |
| MessageType                     | LookupTable |      Yes     |       -        | Type of the Integration         
| IntegrationHandlerResolution    | Smart Table |      Yes     |       -        | Used to resolve the integration handler
| [Custom Import Production Orders From ERP](/AMSOsram/techspec>artifacts>deeactions>CustomImportProductionOrdersFromERP) | DEE Action | Yes | | ction used to Imports Production Order From ERP. |
| [Custom Process Production Order](/AMSOsram/techspec>artifacts>deeactions>CustomProcessProductionOrder) | DEE Action | Yes |  | Action used to Create or Update a Production Order From ERP. |

### How it works
The system initially receives the information from the ERP creating an Integration Entry with the list of **ProductionOrders**. This information is xml format. once created the DEE Action [Custom Import Production Orders From ERP](/AMSOsram/techspec>artifacts>deeactions>CustomImportProductionOrdersFromERP) will get the content with the list of **ProductionOrders** and split them in new Integration Entries. After the list being processed the DEE Action [Custom Process Production Order](/AMSOsram/techspec>artifacts>deeactions>CustomProcessProductionOrder) will trigger creating or updating the **Production Order** depending if it already exists on the system.

### Assumptions
N/A.

## Work items

The table below describes de user stories that affect the current functionality

| User Story |  Type     |          Title                | Description |
| :--------- | :-------- | :---------------------------- | :---------- | 
|   152659   | UserStory | Get Production Orders from ERP|             |
