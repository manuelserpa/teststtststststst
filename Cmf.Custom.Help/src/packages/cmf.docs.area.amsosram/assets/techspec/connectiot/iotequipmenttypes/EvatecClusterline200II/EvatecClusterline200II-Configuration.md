Configuration
============
This section describe the setup for EvatecClusterline200II Equipment Type

Protocol
========
The protocol used by the Automation Driver referenced on the Automation Controller is named **SecsGem** and used the out-of-the-box package **criticalmanufacturing/connect-iot-driver-secsgem**, using version **8.3.3-202201212** at the time of writing.

Driver Definition
=================
The Automation Driver referenced on the Automation Controller as **SecsGemEquipment** is the Automation Driver **EvatecClusterline200IIDriver**, this driver contains the information regarding all the items needed for the automation of the equipment.

Properties
==========

The table below describes the workflow properties

|Name | ID | Type | Equipment Type | Description 
:------------ | -------: | :-------- | :---------- | :-------- 
 $G14_CLOCK_SVID | 2301000004 | String | A | 
 TimeSinceUDT | 2504411144 | Decimal | F8 | 
 TimeInState | 2504411143 | Decimal | F8 | 
 EntityType | 2504411058 | Integer | I4 | 
 EntityName | 2504411057 | String | A | 
 NewState | 2504411078 | Integer | I4 | 
 PreviousState | 2504411091 | Integer | I4 | 
 ProcessJobManagerRecID | 12305013 | String | A | 
 ProcessJobManagerPRMtlNameList | 12305006 | Object | L | 
 ProcessJobID | 12305008 | String | A | 
 AlarmCode | 2501000002 | Binary | BI | 
 AlarmID | 2501000003 | Integer | U4 | 
 AlarmText | 2501000004 | String | A | 
 CarrierID_CarrierReport | 2500000012 | String | A | 
 CarrierLocationID | 2500000014 | String | A | 
 LocationID | 2500000037 | String | A | 
 PortID_CarrierReport | 2500000044 | Integer | U4 | 
 CarrierSubType | 2500000007 | String | A | 
 SubstrateSubType | 2500000008 | String | A | 
 CarrierAccessingStatus | 2500000009 | Integer | U1 | 
 CarrierCapacity | 2500000010 | Integer | I4 | 
 CarrierContentMap | 2500000011 | Object | L | 
 CarrierIDStatus | 2500000013 | Integer | U1 | 
 CarrierSlotMap | 2500000015 | Object | L | 
 PauseEvent | 2500000041 | Object | L | 
 PRJobID | 2500000050 | String | A | 
 PRJobState | 2500000051 | Integer | U1 | 
 NameList | 2500000052 | Object | L | 
 PRMtlType | 2500000053 | Binary | BI | 
 PRProcessStart | 2500000057 | Boolean | BO | 
 PRRecipeMethod | 2500000058 | Integer | U1 | 
 RecID | 2500000060 | String | A | 
 RecList | 2500000064 | Object | L | 
 CarrierID_WaferReport | 2504432028 | String | A | 
 PortID_WaferReport | 2504432087 | Integer | U1 | 
 SourceSlot | 2504432120 | Integer | U4 | 
 AcquiredId | 2512300001 | String | A | 
 LotId | 2512300002 | String | A | 
 SubId | 2512300005 | String | A | 
 StepValue | 2512300004 | Integer | I4 | 
 PortTransferState | 2500000046 | Integer | U1 | 
 PortReservationState | 2500000045 | Integer | U1 | 
 PortAccessMode | 2500000042 | Integer | U1 | 
 PortAssociationState | 2500000043 | Integer | U1 | 
 ControlState | 2301000005 | Integer | U1 | 
 ControlJobID | 2500000022 | String | A | 
 BlockedReason | 2500000005 | Integer | U1 | A numeric code that identifies the most recent blocked condition of the related EPT module
 BlockedReasonText | 2500000006 | String | A | A description of the most recent blocked condition of the Equipment or EPT module
 EPTClock | 2500000032 | String | A | The timestamp when the most recent transition occurred and collection event was triggered that set the tracker to its current state
 EPTState | 2500000033 | Integer | U1 | The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)
 EPTStateTime | 2500000034 | Integer | U4 | Time spent in the previous EPT state prior to entering the current EPT state
 PreviousEPTState | 2500000047 | Integer | U1 | PreviousEPTState
 PreviousTaskName | 2500000048 | String | A | PreviousTaskName
 PreviousTaskType | 2500000049 | Integer | U1 | PreviousTaskType
 TaskName | 2500000082 | String | A | Name of the EPT Task currently running on this EPT module
 TaskType | 2500000083 | Integer | U1 | The type of EPT Task currently running on this EPT module

Events
======

### E10StateChanged

Event for Event raised when an E10 state change occurs. ID: **2404411109**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 TimeSinceUDT | 2504411144 | Decimal | Yes | F8 | 
 TimeInState | 2504411143 | Decimal | Yes | F8 | 
 EntityType | 2504411058 | Integer | Yes | I4 | 
 EntityName | 2504411057 | String | Yes | A | 
 NewState | 2504411078 | Integer | Yes | I4 | 
 PreviousState | 2504411091 | Integer | Yes | I4 | 

### AlarmErrorClear_1

Event for . ID: **1401000001**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AlarmCode | 2501000002 | Binary | Yes | BI | 
 AlarmID | 2501000003 | Integer | Yes | U4 | 
 AlarmText | 2501000004 | String | Yes | A | 

### AlarmErrorClear_2

Event for . ID: **1401000002**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AlarmCode | 2501000002 | Binary | Yes | BI | 
 AlarmID | 2501000003 | Integer | Yes | U4 | 
 AlarmText | 2501000004 | String | Yes | A | 

### AlarmErrorClear_3

Event for . ID: **1401000005**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AlarmCode | 2501000002 | Binary | Yes | BI | 
 AlarmID | 2501000003 | Integer | Yes | U4 | 
 AlarmText | 2501000004 | String | Yes | A | 

### AlarmErrorClear_4

Event for . ID: **1401000006**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AlarmCode | 2501000002 | Binary | Yes | BI | 
 AlarmID | 2501000003 | Integer | Yes | U4 | 
 AlarmText | 2501000004 | String | Yes | A | 

### CarrierClamped

Event for . ID: **2400000010**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 CarrierLocationID | 2500000014 | String | Yes | A | 
 LocationID | 2500000037 | String | Yes | A | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### CarrierClosed

Event for . ID: **2400000011**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 CarrierLocationID | 2500000014 | String | Yes | A | 
 LocationID | 2500000037 | String | Yes | A | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### CarrierOpen

Event for . ID: **2400000014**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 CarrierLocationID | 2500000014 | String | Yes | A | 
 LocationID | 2500000037 | String | Yes | A | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### CarrierReadSuccessEvent

Event for . ID: **2400000020**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierSubType | 2500000007 | String | Yes | A | 
 SubstrateSubType | 2500000008 | String | Yes | A | 
 CarrierAccessingStatus | 2500000009 | Integer | Yes | U1 | 
 CarrierCapacity | 2500000010 | Integer | Yes | I4 | 
 CarrierContentMap | 2500000011 | Object | Yes | L | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 CarrierIDStatus | 2500000013 | Integer | Yes | U1 | 
 CarrierLocationID | 2500000014 | String | Yes | A | 
 CarrierSlotMap | 2500000015 | Object | Yes | L | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### CarrierReadUnSuccessEvent

Event for . ID: **2400000021**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierSubType | 2500000007 | String | Yes | A | 
 SubstrateSubType | 2500000008 | String | Yes | A | 
 CarrierAccessingStatus | 2500000009 | Integer | Yes | U1 | 
 CarrierCapacity | 2500000010 | Integer | Yes | I4 | 
 CarrierContentMap | 2500000011 | Object | Yes | L | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 CarrierIDStatus | 2500000013 | Integer | Yes | U1 | 
 CarrierLocationID | 2500000014 | String | Yes | A | 
 CarrierSlotMap | 2500000015 | Object | Yes | L | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### ProcessWithCarrierReceivedEvent

Event for . ID: **2400000022**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierSubType | 2500000007 | String | Yes | A | 
 SubstrateSubType | 2500000008 | String | Yes | A | 
 CarrierAccessingStatus | 2500000009 | Integer | Yes | U1 | 
 CarrierCapacity | 2500000010 | Integer | Yes | I4 | 
 CarrierContentMap | 2500000011 | Object | Yes | L | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 CarrierIDStatus | 2500000013 | Integer | Yes | U1 | 
 CarrierLocationID | 2500000014 | String | Yes | A | 
 CarrierSlotMap | 2500000015 | Object | Yes | L | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### SlotMapReadVerifiedWaitHostEvent

Event for . ID: **2400000028**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierSubType | 2500000007 | String | Yes | A | 
 SubstrateSubType | 2500000008 | String | Yes | A | 
 CarrierAccessingStatus | 2500000009 | Integer | Yes | U1 | 
 CarrierCapacity | 2500000010 | Integer | Yes | I4 | 
 CarrierContentMap | 2500000011 | Object | Yes | L | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 CarrierIDStatus | 2500000013 | Integer | Yes | U1 | 
 CarrierLocationID | 2500000014 | String | Yes | A | 
 CarrierSlotMap | 2500000015 | Object | Yes | L | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### EqAccessingCarrierEvent

Event for . ID: **2400000032**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierSubType | 2500000007 | String | Yes | A | 
 SubstrateSubType | 2500000008 | String | Yes | A | 
 CarrierAccessingStatus | 2500000009 | Integer | Yes | U1 | 
 CarrierCapacity | 2500000010 | Integer | Yes | I4 | 
 CarrierContentMap | 2500000011 | Object | Yes | L | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 CarrierIDStatus | 2500000013 | Integer | Yes | U1 | 
 CarrierLocationID | 2500000014 | String | Yes | A | 
 CarrierSlotMap | 2500000015 | Object | Yes | L | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### EqFinishedNormalEvent

Event for . ID: **2400000033**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierSubType | 2500000007 | String | Yes | A | 
 SubstrateSubType | 2500000008 | String | Yes | A | 
 CarrierAccessingStatus | 2500000009 | Integer | Yes | U1 | 
 CarrierCapacity | 2500000010 | Integer | Yes | I4 | 
 CarrierContentMap | 2500000011 | Object | Yes | L | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 CarrierIDStatus | 2500000013 | Integer | Yes | U1 | 
 CarrierLocationID | 2500000014 | String | Yes | A | 
 CarrierSlotMap | 2500000015 | Object | Yes | L | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### EqAccessingCarrierEventAbnormal

Event for . ID: **2400000034**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierSubType | 2500000007 | String | Yes | A | 
 SubstrateSubType | 2500000008 | String | Yes | A | 
 CarrierAccessingStatus | 2500000009 | Integer | Yes | U1 | 
 CarrierCapacity | 2500000010 | Integer | Yes | I4 | 
 CarrierContentMap | 2500000011 | Object | Yes | L | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 CarrierIDStatus | 2500000013 | Integer | Yes | U1 | 
 CarrierLocationID | 2500000014 | String | Yes | A | 
 CarrierSlotMap | 2500000015 | Object | Yes | L | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### PRJobCompleteEvent

Event for . ID: **2400000076**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PauseEvent | 2500000041 | Object | Yes | L | 
 PRJobID | 2500000050 | String | Yes | A | 
 PRJobState | 2500000051 | Integer | Yes | U1 | 
 NameList | 2500000052 | Object | Yes | L | 
 PRMtlType | 2500000053 | Binary | Yes | BI | 
 PRProcessStart | 2500000057 | Boolean | Yes | BO | 
 PRRecipeMethod | 2500000058 | Integer | Yes | U1 | 
 RecID | 2500000060 | String | Yes | A | 
 RecList | 2500000064 | Object | Yes | L | 

### PRJobProcessingEvent

Event for . ID: **2400000077**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PauseEvent | 2500000041 | Object | Yes | L | 
 PRJobID | 2500000050 | String | Yes | A | 
 PRJobState | 2500000051 | Integer | Yes | U1 | 
 NameList | 2500000052 | Object | Yes | L | 
 PRMtlType | 2500000053 | Binary | Yes | BI | 
 PRProcessStart | 2500000057 | Boolean | Yes | BO | 
 PRRecipeMethod | 2500000058 | Integer | Yes | U1 | 
 RecID | 2500000060 | String | Yes | A | 
 RecList | 2500000064 | Object | Yes | L | 

### PRJobProcessingCompleteEvent

Event for . ID: **2400000078**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PauseEvent | 2500000041 | Object | Yes | L | 
 PRJobID | 2500000050 | String | Yes | A | 
 PRJobState | 2500000051 | Integer | Yes | U1 | 
 NameList | 2500000052 | Object | Yes | L | 
 PRMtlType | 2500000053 | Binary | Yes | BI | 
 PRProcessStart | 2500000057 | Boolean | Yes | BO | 
 PRRecipeMethod | 2500000058 | Integer | Yes | U1 | 
 RecID | 2500000060 | String | Yes | A | 
 RecList | 2500000064 | Object | Yes | L | 

### PRJobSetupEvent

Event for . ID: **2400000079**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PauseEvent | 2500000041 | Object | Yes | L | 
 PRJobID | 2500000050 | String | Yes | A | 
 PRJobState | 2500000051 | Integer | Yes | U1 | 
 NameList | 2500000052 | Object | Yes | L | 
 PRMtlType | 2500000053 | Binary | Yes | BI | 
 PRProcessStart | 2500000057 | Boolean | Yes | BO | 
 PRRecipeMethod | 2500000058 | Integer | Yes | U1 | 
 RecID | 2500000060 | String | Yes | A | 
 RecList | 2500000064 | Object | Yes | L | 

### PRJobMaterialProcessCompleteEvent

Event for . ID: **2400000085**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PauseEvent | 2500000041 | Object | Yes | L | 
 PRJobID | 2500000050 | String | Yes | A | 
 PRJobState | 2500000051 | Integer | Yes | U1 | 
 NameList | 2500000052 | Object | Yes | L | 
 PRMtlType | 2500000053 | Binary | Yes | BI | 
 PRProcessStart | 2500000057 | Boolean | Yes | BO | 
 PRRecipeMethod | 2500000058 | Integer | Yes | U1 | 
 RecID | 2500000060 | String | Yes | A | 
 RecList | 2500000064 | Object | Yes | L | 

### WaferEndEvent

Event for . ID: **2404432382**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierID_WaferReport | 2504432028 | String | Yes | A | 
 PortID_WaferReport | 2504432087 | Integer | Yes | U1 | 
 SourceSlot | 2504432120 | Integer | Yes | U4 | 

### WaferStartEvent

Event for . ID: **2404432383**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierID_WaferReport | 2504432028 | String | Yes | A | 
 PortID_WaferReport | 2504432087 | Integer | Yes | U1 | 
 SourceSlot | 2504432120 | Integer | Yes | U4 | 

### PortTransferToTransferBlocked

Event for . ID: **2400000067**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortTransferState | 2500000046 | Integer | Yes | U1 | 
 PortReservationState | 2500000045 | Integer | Yes | U1 | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 PortAccessMode | 2500000042 | Integer | Yes | U1 | 
 PortAssociationState | 2500000043 | Integer | Yes | U1 | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### PortTransferToTransferReady

Event for . ID: **2400000068**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortTransferState | 2500000046 | Integer | Yes | U1 | 
 PortReservationState | 2500000045 | Integer | Yes | U1 | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 PortAccessMode | 2500000042 | Integer | Yes | U1 | 
 PortAssociationState | 2500000043 | Integer | Yes | U1 | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### PortTransferToReadyLoad

Event for . ID: **2400000069**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortTransferState | 2500000046 | Integer | Yes | U1 | 
 PortReservationState | 2500000045 | Integer | Yes | U1 | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 PortAccessMode | 2500000042 | Integer | Yes | U1 | 
 PortAssociationState | 2500000043 | Integer | Yes | U1 | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### PortTransferToReadyUnload

Event for . ID: **2400000070**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortTransferState | 2500000046 | Integer | Yes | U1 | 
 PortReservationState | 2500000045 | Integer | Yes | U1 | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 PortAccessMode | 2500000042 | Integer | Yes | U1 | 
 PortAssociationState | 2500000043 | Integer | Yes | U1 | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### StartOfLoadTransfer

Event for . ID: **2400000071**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortTransferState | 2500000046 | Integer | Yes | U1 | 
 PortReservationState | 2500000045 | Integer | Yes | U1 | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 PortAccessMode | 2500000042 | Integer | Yes | U1 | 
 PortAssociationState | 2500000043 | Integer | Yes | U1 | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### StartOfUnloadTransfer

Event for . ID: **2400000072**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortTransferState | 2500000046 | Integer | Yes | U1 | 
 PortReservationState | 2500000045 | Integer | Yes | U1 | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 PortAccessMode | 2500000042 | Integer | Yes | U1 | 
 PortAssociationState | 2500000043 | Integer | Yes | U1 | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### UnloadTransferComplete

Event for . ID: **2400000073**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortTransferState | 2500000046 | Integer | Yes | U1 | 
 PortReservationState | 2500000045 | Integer | Yes | U1 | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 PortAccessMode | 2500000042 | Integer | Yes | U1 | 
 PortAssociationState | 2500000043 | Integer | Yes | U1 | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### ProcessingSubstrateComplete

Event for . ID: **2400000074**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortTransferState | 2500000046 | Integer | Yes | U1 | 
 PortReservationState | 2500000045 | Integer | Yes | U1 | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 PortAccessMode | 2500000042 | Integer | Yes | U1 | 
 PortAssociationState | 2500000043 | Integer | Yes | U1 | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### TransferUnsuccess

Event for . ID: **2400000075**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortTransferState | 2500000046 | Integer | Yes | U1 | 
 PortReservationState | 2500000045 | Integer | Yes | U1 | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 PortAccessMode | 2500000042 | Integer | Yes | U1 | 
 PortAssociationState | 2500000043 | Integer | Yes | U1 | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### MaterialReceived

Event for . ID: **2404401174**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortTransferState | 2500000046 | Integer | Yes | U1 | 
 PortReservationState | 2500000045 | Integer | Yes | U1 | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 PortAccessMode | 2500000042 | Integer | Yes | U1 | 
 PortAssociationState | 2500000043 | Integer | Yes | U1 | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### MaterialRemoved

Event for . ID: **2404401175**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortTransferState | 2500000046 | Integer | Yes | U1 | 
 PortReservationState | 2500000045 | Integer | Yes | U1 | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 PortAccessMode | 2500000042 | Integer | Yes | U1 | 
 PortAssociationState | 2500000043 | Integer | Yes | U1 | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### ProcessChamber1_Chamber protection is disabledClear

Event for This event occurs when the alarm PM1.ProcessChamber.Chamber protection is disabled transitions to the Cleared state.. ID: **2412300001**

*(this event has no properties)*

### ProcessChamber1_Chamber protection is disabledSet

Event for This event occurs when the alarm PM1.ProcessChamber.Chamber protection is disabled transitions to the Set state.. ID: **2412300002**

*(this event has no properties)*

### ProcessChamber1_ProcessFinished

Event for Raised when the manual control level of the chamber changes. ID: **2412300007**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber1_ProcessStarted

Event for Raised when the manual control level of the chamber changes. ID: **2412300008**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber1_RgaStateOff

Event for Chamber Processing Step changed. ID: **2412300009**

*(this event has no properties)*

### ProcessChamber1_RgaStateOn

Event for Chamber Processing Step changed. ID: **2412300010**

*(this event has no properties)*

### ProcessChamber1_StepInformationChanged

Event for Chamber Processing Step changed. ID: **2412300011**

*(this event has no properties)*

### ProcessChamber1_StepStatisticsAvailable

Event for Event raised when step information changes. ID: **2412300012**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber2_Chamber protection is disabledClear

Event for This event occurs when the alarm PM2.ProcessChamber.Chamber protection is disabled transitions to the Cleared state.. ID: **2413300001**

*(this event has no properties)*

### ProcessChamber2_Chamber protection is disabledSet

Event for This event occurs when the alarm PM2.ProcessChamber.Chamber protection is disabled transitions to the Set state.. ID: **2413300002**

*(this event has no properties)*

### ProcessChamber2_ProcessFinished

Event for Raised when the manual control level of the chamber changes. ID: **2413300007**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber2_ProcessStarted

Event for Raised when the manual control level of the chamber changes. ID: **2413300008**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber2_RgaStateOff

Event for Chamber Processing Step changed. ID: **2413300009**

*(this event has no properties)*

### ProcessChamber2_RgaStateOn

Event for Chamber Processing Step changed. ID: **2413300010**

*(this event has no properties)*

### ProcessChamber2_StepInformationChanged

Event for Chamber Processing Step changed. ID: **2413300011**

*(this event has no properties)*

### ProcessChamber2_StepStatisticsAvailable

Event for Event raised when step information changes. ID: **2413300012**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber3_Chamber protection is disabledClear

Event for This event occurs when the alarm PM3.ProcessChamber.Chamber protection is disabled transitions to the Cleared state.. ID: **2414300001**

*(this event has no properties)*

### ProcessChamber3_Chamber protection is disabledSet

Event for This event occurs when the alarm PM3.ProcessChamber.Chamber protection is disabled transitions to the Set state.. ID: **2414300002**

*(this event has no properties)*

### ProcessChamber3_ProcessFinished

Event for Raised when the manual control level of the chamber changes. ID: **2414300007**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber3_ProcessStarted

Event for Raised when the manual control level of the chamber changes. ID: **2414300008**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber3_RgaStateOff

Event for Chamber Processing Step changed. ID: **2414300009**

*(this event has no properties)*

### ProcessChamber3_RgaStateOn

Event for Chamber Processing Step changed. ID: **2414300010**

*(this event has no properties)*

### ProcessChamber3_StepInformationChanged

Event for Chamber Processing Step changed. ID: **2414300011**

*(this event has no properties)*

### ProcessChamber3_StepStatisticsAvailable

Event for Event raised when step information changes. ID: **2414300012**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber4_Chamber protection is disabledClear

Event for This event occurs when the alarm PM4.ProcessChamber.Chamber protection is disabled transitions to the Cleared state.. ID: **2415300001**

*(this event has no properties)*

### ProcessChamber4_Chamber protection is disabledSet

Event for This event occurs when the alarm PM4.ProcessChamber.Chamber protection is disabled transitions to the Set state.. ID: **2415300002**

*(this event has no properties)*

### ProcessChamber4_ProcessFinished

Event for Raised when the manual control level of the chamber changes. ID: **2415300007**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber4_ProcessStarted

Event for Raised when the manual control level of the chamber changes. ID: **2415300008**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber4_RgaStateOff

Event for Chamber Processing Step changed. ID: **2415300009**

*(this event has no properties)*

### ProcessChamber4_RgaStateOn

Event for Chamber Processing Step changed. ID: **2415300010**

*(this event has no properties)*

### ProcessChamber4_StepInformationChanged

Event for Chamber Processing Step changed. ID: **2415300011**

*(this event has no properties)*

### ProcessChamber4_StepStatisticsAvailable

Event for Event raised when step information changes. ID: **2415300012**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber5_Chamber protection is disabledClear

Event for This event occurs when the alarm PM5.ProcessChamber.Chamber protection is disabled transitions to the Cleared state.. ID: **2416300001**

*(this event has no properties)*

### ProcessChamber5_Chamber protection is disabledSet

Event for This event occurs when the alarm PM5.ProcessChamber.Chamber protection is disabled transitions to the Set state.. ID: **2416300002**

*(this event has no properties)*

### ProcessChamber5_ProcessFinished

Event for Raised when the manual control level of the chamber changes. ID: **2416300007**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber5_ProcessStarted

Event for Raised when the manual control level of the chamber changes. ID: **2416300008**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber5_RgaStateOff

Event for Chamber Processing Step changed. ID: **2416300009**

*(this event has no properties)*

### ProcessChamber5_RgaStateOn

Event for Chamber Processing Step changed. ID: **2416300010**

*(this event has no properties)*

### ProcessChamber5_StepInformationChanged

Event for Chamber Processing Step changed. ID: **2416300011**

*(this event has no properties)*

### ProcessChamber5_StepStatisticsAvailable

Event for Event raised when step information changes. ID: **2416300012**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber6_Chamber protection is disabledClear

Event for This event occurs when the alarm PM6.ProcessChamber.Chamber protection is disabled transitions to the Cleared state.. ID: **2417300001**

*(this event has no properties)*

### ProcessChamber6_Chamber protection is disabledSet

Event for This event occurs when the alarm PM6.ProcessChamber.Chamber protection is disabled transitions to the Set state.. ID: **2417300002**

*(this event has no properties)*

### ProcessChamber6_ProcessFinished

Event for Raised when the manual control level of the chamber changes. ID: **2417300007**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber6_ProcessStarted

Event for Raised when the manual control level of the chamber changes. ID: **2417300008**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### ProcessChamber6_RgaStateOff

Event for Chamber Processing Step changed. ID: **2417300009**

*(this event has no properties)*

### ProcessChamber6_RgaStateOn

Event for Chamber Processing Step changed. ID: **2417300010**

*(this event has no properties)*

### ProcessChamber6_StepInformationChanged

Event for Chamber Processing Step changed. ID: **2417300011**

*(this event has no properties)*

### ProcessChamber6_StepStatisticsAvailable

Event for Event raised when step information changes. ID: **2417300012**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 AcquiredId | 2512300001 | String | Yes | A | 
 LotId | 2512300002 | String | Yes | A | 
 SubId | 2512300005 | String | Yes | A | 

### CtrlJobSMTrans05

Event for Material for the first ProcessJob arrives or in the case where the first ProcessJob does not require material; this transition shall be taken as soon as the processing resource for that ProcessJob becomes available; and StartMethod is set for Auto. ID: **2400000045**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 ControlJobID | 2500000022 | String | Yes | A | 

### CtrlJobSMTrans07

Event for User Start command received. ID: **2400000047**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 ControlJobID | 2500000022 | String | Yes | A | 

### CtrlJobSMTrans10

Event for All the ProcessJobs specified for the ControlJob have completed. ID: **2400000050**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 ControlJobID | 2500000022 | String | Yes | A | 

### ControlStateLOCAL

Event for Equipment control state changed to the Online Local state. ID: **2401000039**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 ControlState | 2301000005 | Integer | Yes | U1 | 

### ControlStateREMOTE

Event for Equipment control state changed to the Online Remote state. ID: **2401000040**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 ControlState | 2301000005 | Integer | Yes | U1 | 

### EquipmentOFFLINE

Event for Equipment control state changed to the Offline state. ID: **2401000056**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 ControlState | 2301000005 | Integer | Yes | U1 | 

### CarrierSMTrans21

Event for Carrier is unloaded from the equipment; CancelBind or CancelCarrierNotification service is received prior to the carrier load; or an equipment based verification fails and the equipment performs a self-initiated CancelBind service. The carrier object is destroyed. ID: **2400000035**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierSubType | 2500000007 | String | Yes | A | 
 SubstrateSubType | 2500000008 | String | Yes | A | 
 CarrierAccessingStatus | 2500000009 | Integer | Yes | U1 | 
 CarrierCapacity | 2500000010 | Integer | Yes | I4 | 
 CarrierContentMap | 2500000011 | Object | Yes | L | 
 CarrierID_CarrierReport | 2500000012 | String | Yes | A | 
 CarrierIDStatus | 2500000013 | Integer | Yes | U1 | 
 CarrierLocationID | 2500000014 | String | Yes | A | 
 CarrierSlotMap | 2500000015 | Object | Yes | L | 
 PortID_CarrierReport | 2500000044 | Integer | Yes | U4 | 

### EquipmentEPTStateChangeEvent

Event for Equipment Performance State Model for Equipment. ID: **2401000055**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 BlockedReason | 2500000005 | Integer | Yes | U1 | A numeric code that identifies the most recent blocked condition of the related EPT module
 BlockedReasonText | 2500000006 | String | Yes | A | A description of the most recent blocked condition of the Equipment or EPT module
 EPTClock | 2500000032 | String | Yes | A | The timestamp when the most recent transition occurred and collection event was triggered that set the tracker to its current state
 EPTState | 2500000033 | Integer | Yes | U1 | The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)
 EPTStateTime | 2500000034 | Integer | Yes | U4 | Time spent in the previous EPT state prior to entering the current EPT state
 PreviousEPTState | 2500000047 | Integer | Yes | U1 | PreviousEPTState
 PreviousTaskName | 2500000048 | String | Yes | A | PreviousTaskName
 PreviousTaskType | 2500000049 | Integer | Yes | U1 | PreviousTaskType
 TaskName | 2500000082 | String | Yes | A | Name of the EPT Task currently running on this EPT module
 TaskType | 2500000083 | Integer | Yes | U1 | The type of EPT Task currently running on this EPT module

### LLAEptTrackerEPTStateChangeEvent

Event for Equipment Performance State Model for Modules. ID: **2408200001**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 BlockedReason | 2500000005 | Integer | Yes | U1 | A numeric code that identifies the most recent blocked condition of the related EPT module
 BlockedReasonText | 2500000006 | String | Yes | A | A description of the most recent blocked condition of the Equipment or EPT module
 EPTClock | 2500000032 | String | Yes | A | The timestamp when the most recent transition occurred and collection event was triggered that set the tracker to its current state
 EPTState | 2500000033 | Integer | Yes | U1 | The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)
 EPTStateTime | 2500000034 | Integer | Yes | U4 | Time spent in the previous EPT state prior to entering the current EPT state
 PreviousEPTState | 2500000047 | Integer | Yes | U1 | PreviousEPTState
 PreviousTaskName | 2500000048 | String | Yes | A | PreviousTaskName
 PreviousTaskType | 2500000049 | Integer | Yes | U1 | PreviousTaskType
 TaskName | 2500000082 | String | Yes | A | Name of the EPT Task currently running on this EPT module
 TaskType | 2500000083 | Integer | Yes | U1 | The type of EPT Task currently running on this EPT module

### LLBEptTrackerEPTStateChangeEvent

Event for Equipment Performance State Model for Modules. ID: **2409200001**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 BlockedReason | 2500000005 | Integer | Yes | U1 | A numeric code that identifies the most recent blocked condition of the related EPT module
 BlockedReasonText | 2500000006 | String | Yes | A | A description of the most recent blocked condition of the Equipment or EPT module
 EPTClock | 2500000032 | String | Yes | A | The timestamp when the most recent transition occurred and collection event was triggered that set the tracker to its current state
 EPTState | 2500000033 | Integer | Yes | U1 | The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)
 EPTStateTime | 2500000034 | Integer | Yes | U4 | Time spent in the previous EPT state prior to entering the current EPT state
 PreviousEPTState | 2500000047 | Integer | Yes | U1 | PreviousEPTState
 PreviousTaskName | 2500000048 | String | Yes | A | PreviousTaskName
 PreviousTaskType | 2500000049 | Integer | Yes | U1 | PreviousTaskType
 TaskName | 2500000082 | String | Yes | A | Name of the EPT Task currently running on this EPT module
 TaskType | 2500000083 | Integer | Yes | U1 | The type of EPT Task currently running on this EPT module

### TU1BufferEptTrackerEPTStateChangeEvent

Event for Equipment Performance State Model for Modules. ID: **2420510001**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 BlockedReason | 2500000005 | Integer | Yes | U1 | A numeric code that identifies the most recent blocked condition of the related EPT module
 BlockedReasonText | 2500000006 | String | Yes | A | A description of the most recent blocked condition of the Equipment or EPT module
 EPTClock | 2500000032 | String | Yes | A | The timestamp when the most recent transition occurred and collection event was triggered that set the tracker to its current state
 EPTState | 2500000033 | Integer | Yes | U1 | The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)
 EPTStateTime | 2500000034 | Integer | Yes | U4 | Time spent in the previous EPT state prior to entering the current EPT state
 PreviousEPTState | 2500000047 | Integer | Yes | U1 | PreviousEPTState
 PreviousTaskName | 2500000048 | String | Yes | A | PreviousTaskName
 PreviousTaskType | 2500000049 | Integer | Yes | U1 | PreviousTaskType
 TaskName | 2500000082 | String | Yes | A | Name of the EPT Task currently running on this EPT module
 TaskType | 2500000083 | Integer | Yes | U1 | The type of EPT Task currently running on this EPT module

### TU2EptTrackerEPTStateChangeEvent

Event for Equipment Performance State Model for Modules. ID: **2421500002**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 BlockedReason | 2500000005 | Integer | Yes | U1 | A numeric code that identifies the most recent blocked condition of the related EPT module
 BlockedReasonText | 2500000006 | String | Yes | A | A description of the most recent blocked condition of the Equipment or EPT module
 EPTClock | 2500000032 | String | Yes | A | The timestamp when the most recent transition occurred and collection event was triggered that set the tracker to its current state
 EPTState | 2500000033 | Integer | Yes | U1 | The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)
 EPTStateTime | 2500000034 | Integer | Yes | U4 | Time spent in the previous EPT state prior to entering the current EPT state
 PreviousEPTState | 2500000047 | Integer | Yes | U1 | PreviousEPTState
 PreviousTaskName | 2500000048 | String | Yes | A | PreviousTaskName
 PreviousTaskType | 2500000049 | Integer | Yes | U1 | PreviousTaskType
 TaskName | 2500000082 | String | Yes | A | Name of the EPT Task currently running on this EPT module
 TaskType | 2500000083 | Integer | Yes | U1 | The type of EPT Task currently running on this EPT module

### EvatecAlignerEptTrackerEPTStateChangeEvent

Event for Equipment Performance State Model for Modules. ID: **2422502008**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 BlockedReason | 2500000005 | Integer | Yes | U1 | A numeric code that identifies the most recent blocked condition of the related EPT module
 BlockedReasonText | 2500000006 | String | Yes | A | A description of the most recent blocked condition of the Equipment or EPT module
 EPTClock | 2500000032 | String | Yes | A | The timestamp when the most recent transition occurred and collection event was triggered that set the tracker to its current state
 EPTState | 2500000033 | Integer | Yes | U1 | The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)
 EPTStateTime | 2500000034 | Integer | Yes | U4 | Time spent in the previous EPT state prior to entering the current EPT state
 PreviousEPTState | 2500000047 | Integer | Yes | U1 | PreviousEPTState
 PreviousTaskName | 2500000048 | String | Yes | A | PreviousTaskName
 PreviousTaskType | 2500000049 | Integer | Yes | U1 | PreviousTaskType
 TaskName | 2500000082 | String | Yes | A | Name of the EPT Task currently running on this EPT module
 TaskType | 2500000083 | Integer | Yes | U1 | The type of EPT Task currently running on this EPT module

### TU6EptTrackerEPTStateChangeEvent

Event for Equipment Performance State Model for Modules. ID: **2425500002**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 BlockedReason | 2500000005 | Integer | Yes | U1 | A numeric code that identifies the most recent blocked condition of the related EPT module
 BlockedReasonText | 2500000006 | String | Yes | A | A description of the most recent blocked condition of the Equipment or EPT module
 EPTClock | 2500000032 | String | Yes | A | The timestamp when the most recent transition occurred and collection event was triggered that set the tracker to its current state
 EPTState | 2500000033 | Integer | Yes | U1 | The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)
 EPTStateTime | 2500000034 | Integer | Yes | U4 | Time spent in the previous EPT state prior to entering the current EPT state
 PreviousEPTState | 2500000047 | Integer | Yes | U1 | PreviousEPTState
 PreviousTaskName | 2500000048 | String | Yes | A | PreviousTaskName
 PreviousTaskType | 2500000049 | Integer | Yes | U1 | PreviousTaskType
 TaskName | 2500000082 | String | Yes | A | Name of the EPT Task currently running on this EPT module
 TaskType | 2500000083 | Integer | Yes | U1 | The type of EPT Task currently running on this EPT module

### VTMVacuumEptTrackerEPTStateChangeEvent

Event for Equipment Performance State Model for Modules. ID: **2438200002**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 BlockedReason | 2500000005 | Integer | Yes | U1 | A numeric code that identifies the most recent blocked condition of the related EPT module
 BlockedReasonText | 2500000006 | String | Yes | A | A description of the most recent blocked condition of the Equipment or EPT module
 EPTClock | 2500000032 | String | Yes | A | The timestamp when the most recent transition occurred and collection event was triggered that set the tracker to its current state
 EPTState | 2500000033 | Integer | Yes | U1 | The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)
 EPTStateTime | 2500000034 | Integer | Yes | U4 | Time spent in the previous EPT state prior to entering the current EPT state
 PreviousEPTState | 2500000047 | Integer | Yes | U1 | PreviousEPTState
 PreviousTaskName | 2500000048 | String | Yes | A | PreviousTaskName
 PreviousTaskType | 2500000049 | Integer | Yes | U1 | PreviousTaskType
 TaskName | 2500000082 | String | Yes | A | Name of the EPT Task currently running on this EPT module
 TaskType | 2500000083 | Integer | Yes | U1 | The type of EPT Task currently running on this EPT module

### VTMRobotEptTrackerEPTStateChangeEvent

Event for Equipment Performance State Model for Modules. ID: **2438600002**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 BlockedReason | 2500000005 | Integer | Yes | U1 | A numeric code that identifies the most recent blocked condition of the related EPT module
 BlockedReasonText | 2500000006 | String | Yes | A | A description of the most recent blocked condition of the Equipment or EPT module
 EPTClock | 2500000032 | String | Yes | A | The timestamp when the most recent transition occurred and collection event was triggered that set the tracker to its current state
 EPTState | 2500000033 | Integer | Yes | U1 | The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)
 EPTStateTime | 2500000034 | Integer | Yes | U4 | Time spent in the previous EPT state prior to entering the current EPT state
 PreviousEPTState | 2500000047 | Integer | Yes | U1 | PreviousEPTState
 PreviousTaskName | 2500000048 | String | Yes | A | PreviousTaskName
 PreviousTaskType | 2500000049 | Integer | Yes | U1 | PreviousTaskType
 TaskName | 2500000082 | String | Yes | A | Name of the EPT Task currently running on this EPT module
 TaskType | 2500000083 | Integer | Yes | U1 | The type of EPT Task currently running on this EPT module

### LoadPort1EptTrackerEPTStateChangeEvent

Event for Equipment Performance State Model for Modules. ID: **2475112001**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 BlockedReason | 2500000005 | Integer | Yes | U1 | A numeric code that identifies the most recent blocked condition of the related EPT module
 BlockedReasonText | 2500000006 | String | Yes | A | A description of the most recent blocked condition of the Equipment or EPT module
 EPTClock | 2500000032 | String | Yes | A | The timestamp when the most recent transition occurred and collection event was triggered that set the tracker to its current state
 EPTState | 2500000033 | Integer | Yes | U1 | The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)
 EPTStateTime | 2500000034 | Integer | Yes | U4 | Time spent in the previous EPT state prior to entering the current EPT state
 PreviousEPTState | 2500000047 | Integer | Yes | U1 | PreviousEPTState
 PreviousTaskName | 2500000048 | String | Yes | A | PreviousTaskName
 PreviousTaskType | 2500000049 | Integer | Yes | U1 | PreviousTaskType
 TaskName | 2500000082 | String | Yes | A | Name of the EPT Task currently running on this EPT module
 TaskType | 2500000083 | Integer | Yes | U1 | The type of EPT Task currently running on this EPT module

### LoadPort2EptTrackerEPTStateChangeEvent

Event for Equipment Performance State Model for Modules. ID: **2476113001**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 BlockedReason | 2500000005 | Integer | Yes | U1 | A numeric code that identifies the most recent blocked condition of the related EPT module
 BlockedReasonText | 2500000006 | String | Yes | A | A description of the most recent blocked condition of the Equipment or EPT module
 EPTClock | 2500000032 | String | Yes | A | The timestamp when the most recent transition occurred and collection event was triggered that set the tracker to its current state
 EPTState | 2500000033 | Integer | Yes | U1 | The current EPT state of this equipment or EPT module.  This parameter merges the EPTTracker E116 EPTState attribute and the EPTState DV (since EDA identifies instances with the event sourceID)
 EPTStateTime | 2500000034 | Integer | Yes | U4 | Time spent in the previous EPT state prior to entering the current EPT state
 PreviousEPTState | 2500000047 | Integer | Yes | U1 | PreviousEPTState
 PreviousTaskName | 2500000048 | String | Yes | A | PreviousTaskName
 PreviousTaskType | 2500000049 | Integer | Yes | U1 | PreviousTaskType
 TaskName | 2500000082 | String | Yes | A | Name of the EPT Task currently running on this EPT module
 TaskType | 2500000083 | Integer | Yes | U1 | The type of EPT Task currently running on this EPT module

Commands
========

### LOADCARRIER

Command for Loads Carrier. ID: **LOADCARRIER**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORTID | Yes | A | 
 CARRIERID | Yes | A | 

### UNLOADCARRIER

Command for Unloads Carrier. ID: **UNLOADCARRIER**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORTID | Yes | A | 

### PP-SELECT

Command for PP Select S2F41. ID: **PP-SELECT**

*(this command has no parameters)*

### SPECIFYACQUIREDIDS

Command for . ID: **SPECIFYACQUIREDIDS**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 BATCHID | Yes | A | 
 ACQUIREDIDLIST | Yes | L | 

### START

Command for Start Command. ID: **START**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 BATCHID | Yes | A | 

### ABORT

Command for Abort Command. ID: **ABORT**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 BATCHID | Yes | A | 

### LOCAL

Command for Set Equipment to Online Local. ID: **LOCAL**

*(this command has no parameters)*

### REMOTE

Command for Set Equipment to Online Remote. ID: **REMOTE**

*(this command has no parameters)*

