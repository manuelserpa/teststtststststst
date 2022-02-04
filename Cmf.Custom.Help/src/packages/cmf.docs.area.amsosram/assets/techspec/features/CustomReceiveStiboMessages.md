# Stibo System Integration

## Requirement Specification
Mechanism to integrate Stibo system with MES system.

## Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

Name                         | Type             | Is Mandatory | Data Type | Description 
:--------------------------- | :--------------- | :----------: | :-------- | :-----------------------------------------------------------
MessageType                  | LookupTable      | Yes          | -         | Type of the Integration
IntegrationSystem            | LookupTable      | Yes          | -         | Possible types of systems that are available for integration
IntegrationHandlerResolution | SmartTable       | Yes          | -         | Used to resolve the integration handler

### How it works
The Smart Table IntegrationHandlerResolution contains the configuration so that the system execute different services depending on the message type.

* Upon execution when a message is received the system will:
  * Validate if it is not empty;
  * If the validation is successful the system will create an **Integration Entry**.
* After create the **Integration Entry** the system will execute the service that is configure on Smart Table IntegrationHandlerResolution using the message type.

The table below describes the services that are used on this feature.

| Service                                                                                      | Description                                                                          |
| -------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ |
| [CustomReceiveStiboMessage](/AMSOsram/tecspecs>artifacts>services>CustomReceiveStiboMessage) | Service to receive an Stibo Message and create an **Integration Entry** accordingly. |

### Assumptions
N/A.

## Work items

The table below describes the user stories that affect the current functionality

User Story | Type       | Title                                               | Description
:--------- | :--------- | :-------------------------------------------------- | :----------
155643     | User Story | Create the Infrastructure to Receive Stibo Messages |