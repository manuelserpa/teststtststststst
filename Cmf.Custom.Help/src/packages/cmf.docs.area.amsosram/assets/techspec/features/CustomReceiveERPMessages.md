# Custom Receive ERP Messages

## Requirement Specification
Mechanism to receive a message from ERP system and create an **Integration Entry** on the MES system.

## Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

Name          | Type        | Is Mandatory | Data Type      | Description 
:------------ | :---------- | :----------: | :------------- | :-----------------------
MessageType   | LookupTable |      No      |     String     |  Type of the Integration         

### How it works
Upon execution when a message is received the system will validate if it is not empty and if the validation is successful the system will create an **Integration Entry**.

### Assumptions
N/A.

## Work items

The table below describes de user stories that affect the current functionality

User Story | Type       | Title                                             | Description
:--------- | :--------- | :------------------------------------------------ | :----------
154247     | User Story | Create the Infrastructure to Receive ERP Messages |