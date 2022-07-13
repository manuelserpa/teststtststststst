# Send Critical Data Collection to Space

## Requirement Specification

When performing a Data Collection post a mechanism to validate the posted data and to create an XML message should be implemented.

## Design Specification

### Relevant Artifacts

The table below describes the properties for this entity type:

| Name          | Type      | Description |
| :------------ | :-------- | :---------- |
| [CustomReportEDCToSpaceHandler](/AMSOsram/techspec>artifacts>deeactions>CustomReportEDCToSpaceHandler) | DEE Action | DEE action to validate DataCollection and create a XML message to be sent to Space system. |

### How it works

When a ComplexPerformDataCollection is performed the DEE Action [CustomReportEDCToSpaceHandler](/AMSOsram/techspec>artifacts>deeactions>CustomReportEDCToSpaceHandler) will be executed validating the posted values using the limit set provided.

- If the values respect the limit set defined a protocol defined in the configuration */Cmf/Custom/Protocol/Space* is opened
- Otherwise, the main lot is put on hold with hold reason **Out Of Spec**.  

In the end, a message in XML format will be published on the Message Bus.

### Assumptions

## Work items

The table below describes de user stories that affect the current functionality

| User Story |   Type    |              Title                         | Description |
| :--------- | :-------- | :----------------------------------------- | :---------- |
| 165410     | UserStory | Sent Critical MES Data Collection to SPACE |             |