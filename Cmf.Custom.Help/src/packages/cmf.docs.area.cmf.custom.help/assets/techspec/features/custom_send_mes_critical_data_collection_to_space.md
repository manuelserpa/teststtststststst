# Send Critical Data Collection to Space

## Requirement Specification

When performing a Data Collection create a mechanism to validate the posted data and send a XML message to Space System.

## Design Specification

### Relevant Artifacts

The table below describes the relevant artifacts for this feature:

| Name                                                                                                          | Type       | Description                                                                                |
| :------------------------------------------------------------------------------------------------------------ | :--------- | :----------------------------------------------------------------------------------------- |
| [CustomReportEDCToSpaceHandler](/cmf.custom.help/techspec>artifacts>deeactions>CustomReportEDCToSpaceHandler) | DEE Action | DEE action to validate DataCollection and create a XML message to be sent to Space system. |
| /amsOSRAM/Protocol/Space                                                                                      | Config     | Default Protocol when sending information to Space.                                        |

### How it works

When a ComplexPerformDataCollection is performed the DEE Action [CustomReportEDCToSpaceHandler](/cmf.custom.help/techspec>artifacts>deeactions>CustomReportEDCToSpaceHandler) will be executed validating the posted values using the limit set provided.

- If the values respect the limit set defined a protocol defined in the configuration _/amsOSRAM/Protocol/Space_ is opened
- Otherwise, the main lot is put on hold with hold reason **Out Of Spec**.

In the end, a message in XML format will be published on the Message Bus to be sent to Space System and if it has a reply configured will be created a listener on Tibco EMS Gateway to handle the response from the SPACE.

As soon as the Tibco EMS Gateway receives the reply, it will call the DEE action [CustomTibcoEMSReplyHandler](/cmf.custom.help/techspec>artifacts>deeactions>CustomReportEDCToSpaceHandler). This DEE expects a reply on a certain format to check if the values are valid or invalid.

If everything is as expected it will close the protocol and if this was triggered when performing the TrackOut and MoveNext, it will move the material to next step.

### Assumptions

- The Protocol to be used must have the following configurations:
  - Inhibit Move From Step
  - Cannot have actions to be performed in order to be closed by the DEE
  - It is mandatory to have an Employee configured to close the Protocol

- The User that handles this part of the process should have an Employee associated with him.

## Work items

The table below describes de user stories that affect the current functionality

| User Story | Type      | Title                                      | Description |
| :--------- | :-------- | :----------------------------------------- | :---------- |
| 165410     | UserStory | Sent Critical MES Data Collection to SPACE |             |
