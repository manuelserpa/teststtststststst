Features
============
This section describe the Features delivered.

Create files with dynamic content based on MES Request
============

### Requirement Specification
Upon receiving a System Event with Action Group **NiceLabelPrintInformation** create a *.prn file on the **_Generic_Directory** directory with the contents received on the payload of the request as key-value pair, one per line, and a dynamically generated name.
MES trigger this by executing the Action **CustomNiceLabelPrint**.

### Design Specification

### Relevant Artifacts
The table below describes the properties for this feature:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------
NiceLabelPrintInformation|Action Group|Yes|-|Trigger Action Group
_Generic_Directory|Persistence|Yes|String|Stored Directory Path
AutomationEquipmentAddress|Resource Attribute|Yes|String|Attribute that contains the directory path
CustomNiceLabelPrint|DEE Action|Yes|-|DEE Action that triggers the request

### How it works
Upon receiving the request create a file.

### Assumptions
N/A

### Work items

The table below describes the user stories that affect the current functionality

User Story | Type | Title | Description
:--------- | :--- | :---- | :----------
158635|User Story| [IOT] Generate Files for NiceLabel System| The IOT will receive the label name and the related parameters and then it will generate the file for Nice Label System.