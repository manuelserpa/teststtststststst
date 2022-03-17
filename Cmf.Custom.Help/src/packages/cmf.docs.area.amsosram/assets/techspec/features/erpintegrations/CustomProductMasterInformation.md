# Product Master Information

## Requirement Specification
Action that will create an Integration Entry per Product and import them.

## Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

| Name                         | Type        | Is Mandatory | Data Type | Description                             |
| ---------------------------- | ----------- | :----------: | --------- | --------------------------------------- |
| MessageType                  | LookupTable |      Yes     |     -     | Type of the Integration                 |
| IntegrationHandlerResolution | SmartTable  |      Yes     |     -     | Used to resolve the integration handler |

### How it works

The Smart Table IntegrationHandlerResolution contains the configuration so that the system execute different actions depending on the message type.

* Upon execution when a message is received the system will:
  * Validate if it is not empty;
  * If the validation is successful the system will create an **Integration Entry** per Product.
* After create this the system will create Product(s) using body message of each Integration Entries.
  * If there is any problem during the process, the error will be associated with the Integration Entry.

The table below describes the actions that are used on this feature.

| Action                                                                                               | Description                                                 |
| ---------------------------------------------------------------------------------------------------- | ----------------------------------------------------------- |
| [CustomProcessProductsFromERP](/AMSOsram/techspec>artifacts>deeactions>CustomProcessProductsFromERP) | DEE Action used to create an Integration Entry per Product. |
| [CustomProcessProduct](/AMSOsram/techspec>artifacts>deeactions>CustomProcessProduct)                 | DEE Action used to create Product using XML message body.   |

### Assumptions
N/A.

## Work items

The table below describes the user stories that affect the current functionality.

| User Story | Type       | Title                                   | Description |
| :--------: | :--------: | --------------------------------------- | ----------- |
| 161783     | User Story | Get Product Master Information from ERP |             |