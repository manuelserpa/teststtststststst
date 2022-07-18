# Import Lot Incoming From ERP

## Requirement Specification

Materials can be imported to MES from the ERP. To do this, MES should parse a message sent by the ERP and create or update the necessary objects after validating the file content.

## Design Specification

### Relevant Artifacts

The table below describes the relevat artifacts for this feature:

| Name                         | Type        | Is Mandatory | Data Type | Description                             |
| ---------------------------- | ----------- | :----------: | --------- | --------------------------------------- |
| MessageType                  | LookupTable |      Yes     |     -     | Type of the Integration                 |
| IntegrationHandlerResolution | SmartTable  |      Yes     |     -     | Used to resolve the integration handler |

### How it works

The MES will receive a message format in XML through the **CustomReceiveERPMessage** API. Then, based on the MessageType, a specific **Action** will be called and the XML message will be interpreted and the necessary objects will be created. For the lot creation the MessageType is *PerformIncomingMaterialMasterData*.

These are the mandatory fields validated on **Lot** creation by the system:

| Name          | Description   | Mandatory | MES Entity   |
| :-----------: | :-----------: | :-------: | :----------: |
| Name          | Material Name | Yes       |  Material    |
| Product       | Product Name  | Yes       |  Product     |
| Type          | Material Type | Yes       |  Material    |
| StateModel    | State Model   | No        |  Material    |
| State         | Default State | No        |  Material    |
| Form          | Default Form  | Yes       |  Material    |
| Facility      | Facility      | Yes       |  Facility    |
| Flow          | Flow          | No        |  Flow        |
| Step          | Step          | No        |  Step        |

These are the mandatory fields validated on **Wafer** creation by the system:

| Name          | Description   | Mandatory | MES Entity   |
| :-----------: | :-----------: | :-------: | :----------: |
| Name          | Material Name | Yes       |  Material    |
| Product       | Product Name  | No        |  Product     |
| Type          | Material Type | No        |  Material    |
| StateModel    | State Model   | No        |  Material    |
| State         | Default State | No        |  Material    |
| Form          | Default Form  | Yes       |  Material    |
| Facility      | Facility      | No        |  Facility    |
| Flow          | Flow          | No        |  Flow        |
| Step          | Step          | No        |  Step        |

The Attribute field can be present on the Lot and on the Wafer information.

The EDCData field is available on the Wafer, this property contains the data to be posted and validated against the certificate.

### Assumptions

N/A

## Work items

The table below describes de user stories that affect the current functionality

| User Story | Type       | Title                          | Description |
| :--------: | :--------: | ------------------------------ | ----------- |
| 160142     | User Story | Incoming material lot creation |             |
| 183602     | User Story | Certificate Lot - Improvement  |             |
