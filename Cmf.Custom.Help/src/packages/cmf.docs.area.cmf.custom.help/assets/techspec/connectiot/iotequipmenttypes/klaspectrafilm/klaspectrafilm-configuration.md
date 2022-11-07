Configuration
============
This section describe the setup for KLA SpectraFilm Ellipsometer Equipment Type

Driver Definitions
=================
The following Automation Drivers are referenced on the Automation Controller as:
- **SecsGemEquipment** for the Automation Driver **KLASpectraFilmDriver**;

- **RFIDReader** for the Automation Driver **HermosLFM4xReaderDriver**;

Drivers contain the information regarding all the items needed for the automation of the equipment.
## Automation Driver Definition - KLASpectraFilmDriver
### Protocol
The protocol used by the Automation Driver referenced on the Automation Controller is named **SecsGem** and used the package **criticalmanufacturing/connect-iot-driver-secsgem**, using version **9.1.0-202209072** at the time of writing.

### Properties

The table below describes the workflow properties

|Name | ID | Type | Equipment Type | Description 
:------------ | -------: | :-------- | :---------- | :-------- 
 PortXID | 50300 | Integer | U1 | PortID of the 200mm port that triggered the 200mm port Event. Valid values: 1 to 4
 SlotMap | 3009 | String | A | CarrierSlotMap
 Substrate | 3070 | Object | L | Substrate. The current Substrate Object
 PortID | 3041 | Integer | U1 | The Load Port Number
 DP_SECD_LotSummary | 10002 | Object | L | DP_SECD_LotSummary
 DP_SE_RawMeasurementsEx | 10003 | Object | L | DP_SE_RawMeasurementsEx
 DP_SE_RawMeasurementsDeepFormat | 10004 | Object | L | DP_SE_RawMeasurementsDeepFormat
 CarrierID | 3002 | Object | L | When a Carrier-related event occurs
 ControlState | 105 | Integer | U1 | Current Control state 1: Equipment Offline 4: OnlineLocal 5: OnlineRemote
 NotifyWaferIDRead | 4901 | Integer | U1 | The EC, in conjunction with EC: 4900(CfgWIDAngle), allows modifying the standard E90 logic for Wafer ID Verification. Values are: 0 = The equipment uses standard E90 logic. 1 = Upon reading a Wafer ID by a Wafer ID (CfgWIDAngle) is set to 1. Default = 0.
 PRJobID | 5101 | String | A | Identifier of this Process Job
 PRJobState | 5103 | Integer | U1 | The Process State of the Job
 CurrentPortNumber | 20518 | Integer | U1 | Indicates the current port number

### Events

#### EquipmentOffline

Event for Control State Machine switched to the offline state by the local operator. ID: **101**
Linked Report Id: **2**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 ControlState | 105 | Integer | Yes | U1 | Current Control state 1: Equipment Offline 4: OnlineLocal 5: OnlineRemote

#### ControlStateLocal

Event for Control State Machine switched to local (operator) control. ID: **102**
Linked Report Id: **2**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 ControlState | 105 | Integer | Yes | U1 | Current Control state 1: Equipment Offline 4: OnlineLocal 5: OnlineRemote

#### ControlStateRemote

Event for Control State Machine switched to remote (host) control. ID: **103**
Linked Report Id: **2**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 ControlState | 105 | Integer | Yes | U1 | Current Control state 1: Equipment Offline 4: OnlineLocal 5: OnlineRemote

#### CarrierPlaced

Event for A 200mm carrier was placed on a 200mm open cassette or SMIF load station. ID: **50120**
Linked Report Id: **100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortXID | 50300 | Integer | Yes | U1 | PortID of the 200mm port that triggered the 200mm port Event. Valid values: 1 to 4

#### ProcessStart

Event for Setting Up state to Processing state. ID: **3003**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 5101 | String | Yes | A | Identifier of this Process Job

#### WaferStart

Event for At Source state to At Work. ID: **2071**
Linked Report Id: **26**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 Substrate | 3070 | Object | Yes | L | Substrate. The current Substrate Object

#### WaferComplete

Event for In Process state to Processing Complete. ID: **2081**
Linked Report Id: **26**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 Substrate | 3070 | Object | Yes | L | Substrate. The current Substrate Object

#### ProcessComplete

Event for Processing to Process complete. ID: **3005**
Linked Report Id: **3000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 5101 | String | Yes | A | Identifier of this Process Job

#### ReadyToUnload

Event for TRANSFER BLOCKED state to READY TO UNLOAD. ID: **2058**
Linked Report Id: **1**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 3041 | Integer | Yes | U1 | The Load Port Number

#### MaterialRemove

Event for This GEM-required event indicates that a Carrier has been unloaded from the Equipment. ID: **2099**
Linked Report Id: **1**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 3041 | Integer | Yes | U1 | The Load Port Number

#### SlotMapNotReadToWaitingForHost

Event for State model transition, Slot Map Not Read To Waiting For Host. ID: **2023**
Linked Report Id: **17**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SlotMap | 3009 | String | Yes | A | CarrierSlotMap
 PortID | 3041 | Integer | Yes | U1 | The Load Port Number
 CarrierID | 3002 | Object | Yes | L | When a Carrier-related event occurs

#### Lot_Start_Event

Event for Indicates the 200mm carrier lot start event. ID: **18052**
Linked Report Id: **5**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CurrentPortNumber | 20518 | Integer | Yes | U1 | Indicates the current port number

#### PRJobStateChange

Event for Equipment Processing State transition. ID: **3059**
Linked Report Id: **20**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 5101 | String | Yes | A | Identifier of this Process Job
 PRJobState | 5103 | Integer | Yes | U1 | The Process State of the Job

#### Wafer_MeasurementDataReady

Event for Measurement data for the wafer being measured is available. ID: **704**
Linked Report Id: **999**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 DP_SECD_LotSummary | 10002 | Object | Yes | L | DP_SECD_LotSummary
 DP_SE_RawMeasurementsEx | 10003 | Object | Yes | L | DP_SE_RawMeasurementsEx
 DP_SE_RawMeasurementsDeepFormat | 10004 | Object | Yes | L | DP_SE_RawMeasurementsDeepFormat

### Commands

There are no Automation Commands defined for this Automation Driver Definition

## Automation Driver Definition - HermosLFM4xReaderDriver
Information on this driver is contained on an additional driver related document.

