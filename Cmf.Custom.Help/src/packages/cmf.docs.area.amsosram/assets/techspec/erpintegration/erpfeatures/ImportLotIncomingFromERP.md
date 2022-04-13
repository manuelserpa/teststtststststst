# Import Lot Incoming From ERP

## Requirement Specification

Materials can be imported to MES from the ERP. To do this, MES should parse a message sent by the ERP and create or update the necessary objects after validating the file content.

## Design Specification

### Relevant Artifacts

The table below describes the properties for this entity type:

| Name                         | Type        | Is Mandatory | Data Type | Description                             |
| ---------------------------- | ----------- | :----------: | --------- | --------------------------------------- |
| MessageType                  | LookupTable |      Yes     |     -     | Type of the Integration                 |
| IntegrationHandlerResolution | SmartTable  |      Yes     |     -     | Used to resolve the integration handler |

### How it works

The MES will receive a message format in XML through the **CustomReceiveERPMessage** API. Then, based on the MessageType, a specific **Action** will be called and the XML message will be interpreted and the necessary objects will be created. For the lot creation the MessageType is *PerformIncomingMaterialMasterData*. 

These are the mandatory fields validated by the system:

| Name          | Description                       | Mandatory | MES Entity   |  Attribute | Property/Attribute    |
| :-----------: | :-------------------------------: | :-------: | :----------: | :--------: | :-------------------: |
| Name          | Material Name                     |    Yes    |   Material   |    Yes     | Name                  |
| Product       | Material Product Name             |    Yes    |   Product    |    No      | Description           |
| Type          | Material Type                     |    Yes    |   Material   |    No      | Type                  |
| StateModel    | The state model                   |    No     |   Material   |    No      | ProductType           |
| State         | The default State                 |    No     |   Material   |    No      | DefaultUnits          |
| Form          | Default Form                      |    Yes    |   Material   |    No      | IsEnabled             |
| Facility      | Facility                          |    Yes    |   Facility   |    No      | Yield                 |
| Flow          | Flow                              |    No     |   Flow       |    No      | ProductGroup          |
| Step          | Step                              |    No     |   Step       |    No      | MaximumMaterialSize   |

The Attribute field can be present on the Material and on the wafer information. 
The EDCData fild is available on the wafer, this property contains the data to be posted and validated against the certificate.

### Assumptions

N/A

## Work items

The table below describes de user stories that affect the current functionality

| User Story | Type       | Title                                   | Description |
| :--------: | :--------: | --------------------------------------- | ----------- |
|   160142   | User Story | Incoming material lot creation          |             |