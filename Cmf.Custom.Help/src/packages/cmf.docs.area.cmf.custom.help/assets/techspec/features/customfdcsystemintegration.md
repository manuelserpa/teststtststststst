# FDC System Integration

## Requirement Specification
Mechanism to integrate Onto FDC system with MES system.

## Design Specification

### Relevant Artifacts
The table below describes the relevant artifacts for this feature:

Name                         | Type             | Is Mandatory | Data Type | Description 
:--------------------------- | :--------------- | :----------: | :-------- | :-----------------------------------------------------------
MessageType                  | LookupTable      | Yes          | -         | Integration entry message type.
IntegrationSystem            | LookupTable      | Yes          | -         | Possible types of systems that are available for integration.
IntegrationHandlerResolution | SmartTable       | Yes          | -         | Used to resolve the integration handler.
CustomReportDataToFDC        | DEE Action       | -            | -         | Dee action is triggered to create an integration entry with the material data to send to FDC.
CustomSendFDCWaferInfo       | DEE Action       | -            | -         | DEE action is triggered by Integration Entry Handler in order to process Integration Entries and send Wafer Info to Onto FDC.
CustomSendFDCLotInfo         | DEE Action       | -            | -         | DEE action is triggered by Integration Entry Handler in order to process Integration Entries and send Lot Info to Onto FDC.
/amsOSRAM/FDC/Active/        | Configuration    | Yes          | Boolean   | Enables the Onto FDC integration.
/amsOSRAM/FDC/Mandatory/     | Configuration    | Yes          | Boolean   | FDC mandatory.
/amsOSRAM/FDC/Server/        | Configuration    | Yes          | String    | FDC server.
/amsOSRAM/FDC/Port/          | Configuration    | Yes          | Int       | FDC port.


### How it works
The Smart Table **IntegrationHandlerResolution** contains the configurations so that the system execute different actions depending on the message type.

* When a *Track-In, Track-Out or an Abort* operation are performed upon a material, the system will:
  * Validate if the FDC **Active** configuration (/amsOSRAM/FDC/Active/) is set with **true**
  * Validate if the current Resource has its **FDCCommunication** attribute set with **true**
  * If the validation is successful the system will create an **Integration Entry** with the material data serialized. 
  * This integration entry will be created with a message type based on the material operation that is being processed:
    * Lot Abort - LOTOUT
    * Lot Track-In - LOTIN
    * Lot Track-Out - LOTOUT
    * Wafer Track-In - WAFERIN
    * Wafer Track-Out - WAFEROUT
* After create the **Integration Entry** the system will execute the DEE Action that is configured on Smart Table **IntegrationHandlerResolution** taking in consideration the message type.

### Assumptions
N/A

## Work items

The table below describes the user stories that affect the current functionality

User Story | Type       | Title                                               | Description
:--------- | :--------- | :-------------------------------------------------- | :----------
176442     | User Story | FDC - Sending Material Logistic Data to Onto FDC    | FDC - Sending Material Logistic Data to Onto FDC