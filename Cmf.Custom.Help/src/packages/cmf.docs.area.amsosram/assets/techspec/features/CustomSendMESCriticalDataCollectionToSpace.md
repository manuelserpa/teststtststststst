# CustomSendMESCriticalDataCollectionToSpace

## Requirement Specification
When performing a Data Collection post a mechanism to validate the posted data and to create a xml message should be implemented.

## Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

| Name          | Type      | Is Mandatory | Data Type | Description 
| :------------ | :-------- | :----------: | :-------- | :-----------
| [Custom Report EDC To Space Handler](/AMSOsram/techspec>artifacts>deeactions>CustomReportEDCToSpaceHandler) | DEE |DEE action to validate DataCollection Post data |
| [Custom Report EDC To Space Parser](/AMSOsram/techspec>artifacts>deeactions>CustomReportEDCToSpaceParser) | DEE |DEE Action to create a xml message to be sent to Space system. |

### How it works

When a ComplexPerformDataCollection is performed the DEE Action [Custom Report EDC To Space Handler](/AMSOsram/techspec>artifacts>deeactions>CustomReportEDCToSpaceHandler) will be executed validating the posted values using the limit set provided. If the values respect the limit set defined a protocol defined in the configuration */Cmf/Custom/Protocol/Space* is opened, otherwise the main lot is put on hold with hold reason **Out Of Spec**.  
After this operation is completed, the information is sent to the Tibco gateway to process the information and execute the [Custom Report EDC To Space Parser](/AMSOsram/techspec>artifacts>deeactions>CustomReportEDCToSpaceParser) DEE Action that will create and return an Xml message.

### Assumptions


## Work items

The table below describes de user stories that affect the current functionality

| User Story |   Type    |              Title                         | Description |
| :--------- | :-------- | :----------------------------------------- | :---------- |
| 165410     | UserStory | Sent Critical MES Data Collection to SPACE |             |