# Custom Receive Stibo Messages

## Requirement Specification
Mechanism to receive a message from Stibo system and create an **Integration Entry** on the MES system.

## Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

Name              | Type             | Is Mandatory | Data Type | Description 
:---------------- | :--------------- | :----------: | :-------- | :-----------------------------------------------------------
MessageType       | LookupTable      | No           | string    | Type of the Integration
IntegrationSystem | LookupTable      | Yes          | string    | Possible types of systems that are available for integration



### How it works
Upon execution when a message is received the system will validate if it is not empty and if the validation is successfully the system will create an **Integration Empty**.

### Assumptions
N/A.

## Work items

The table below describes de user stories that affect the current functionality

User Story | Type       | Title                                               | Description
:--------- | :--------- | :-------------------------------------------------- | :----------
155643     | User Story | Create the Infrastructure to Receive Stibo Messages |