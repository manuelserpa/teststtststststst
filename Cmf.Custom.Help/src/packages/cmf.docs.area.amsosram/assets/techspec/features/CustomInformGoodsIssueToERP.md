# Inform Goods Issue to ERP

## Requirement Specification
Feature to send the information to ERP related to Goods issue.

## Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

| Name                          | Type          | Is Mandatory  | Data Type | Description                                                               | 
| :---------------------------- | :------------ | :-----------: | :-------- | :------------------------------------------------------------------------ |  
| GoodsIssue                    | Configuration |       No      |   String  | Configuration with the movement Type                                      |
| Site_Code                     | Attribute     |       No      |   String  | Attribute that contains the site code to be sent to the ERP               |
|  [Custom Report Consumption To SAP](/AMSOsram/techspec>artifacts>smarttables>CustomReportConsumptionToSAP)  | Smart Table   |       Yes     |           | Smart Table containing the StorageLocation                                |
| MessageType                   | Lookup Table  |       Yes     |           | Lookup Table that contains the type of the Integration Entry to generate  |
| [Custom Create Goods Issue Message](/AMSOsram/techspec>artifacts>deeactions>CustomCreateGoodsIssueMessage) | DEE Action    |       Yes     |           | DEE Action to create an Integration Entry with Goods Issue information    |
| CustomSendProcessMessage      | DEE Action    |       Yes     |           | DEE Action to send the information about the Goods Issue to SAP           |

### How it works
When a material is tracked in the DEE [Custom Create Goods Issue Message](/AMSOsram/techspec>artifacts>deeactions>CustomCreateGoodsIssueMessage) is executed validating that each of the materials that are tracked in corresponde with the followinf validations:
* CustomReportConsumptionToSAP Smart Table resolves information about the **Storage Location**.
* The material has a Production Order asssociated and is on the **initial Step**.
* The material is of type **Lot**.
Only the materials that satisfy these conditions will have generated an Integration Entry (*One Integration Entry per material!*).
Once the **Integration Entries** are created then the CustomSendProcessMessage Will send each one to the ERP.

### Assumptions
N/A.

## Work items

The table below describes de user stories that affect the current functionality

| User Story | Type | Title                     | Description
| :--------- | :--- | :------------------------ | :----------
| 165389     | US   | Inform Goods Issue to ERP | 