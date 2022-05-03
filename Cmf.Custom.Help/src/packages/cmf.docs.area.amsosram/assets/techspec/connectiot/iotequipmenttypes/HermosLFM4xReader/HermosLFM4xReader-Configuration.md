Configuration
============
This section describe the setup for Hermos LFM 4x Reader Equipment Type

Driver Definition
=================
The following Automation Driver is referenced on the Automation Controller as **RFIDReader** is the Automation Driver **HermosLFM4xReaderDriver**, this driver contains the information regarding all the items needed for the automation of the equipment.

## Automation Driver Definition - HermosLFM4xReaderDriver
### Protocol
The protocol used by the Automation Driver referenced on the Automation Controller is named **SecsGem** and used the package **criticalmanufacturing/connect-iot-driver-secsgem**, using version **8.3.3-202201212** at the time of writing.

### Properties

The table below describes the workflow properties

|Name | ID | Type | Equipment Type | Description 
:------------ | -------: | :-------- | :---------- | :-------- 
 ProtocolChangeAllow | 98 | Integer | U1 | Used to Allow a detected Protocol Change

### Events

There are no Automation Events defined for this Automation Driver Definition

### Commands

There are no Automation Commands defined for this Automation Driver Definition

