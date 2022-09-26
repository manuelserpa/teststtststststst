# Send Critical Data Collection to Space

## Requirement Specification

When performing a Data Collection create a mechanism to validate the posted data and send a XML message to Space System.

## Design Specification

### Relevant Artifacts

The table below describes the relevat artifacts for this feature:

| Name          | Type      | Description |
| :------------ | :-------- | :---------- |
| [CustomReportEDCToSpaceHandler](/cmf.custom.help/cmf.custom.help.techspec>cmf.custom.help.artifacts>cmf.custom.help.deeactions>CustomReportEDCToSpaceHandler) | DEE Action | DEE action to validate DataCollection and create a XML message to be sent to Space system. |
| /amsOSRAM/Protocol/Space | Config | Default Protocol when sending information to Space. |

### How it works

When a ComplexPerformDataCollection is performed the DEE Action [CustomReportEDCToSpaceHandler](/cmf.custom.help/cmf.custom.help.techspec>cmf.custom.help.artifacts>cmf.custom.help.deeactions>CustomReportEDCToSpaceHandler) will be executed validating the posted values using the limit set provided.

- If the values respect the limit set defined a protocol defined in the configuration */amsOSRAM/Protocol/Space* is opened
- Otherwise, the main lot is put on hold with hold reason **Out Of Spec**.  

In the end, a message in XML format will be published on the Message Bus to be sent to Space System.

### Assumptions

## Work items

The table below describes de user stories that affect the current functionality

| User Story |   Type    |              Title                         | Description |
| :--------- | :-------- | :----------------------------------------- | :---------- |
| 165410     | UserStory | Sent Critical MES Data Collection to SPACE |             |
