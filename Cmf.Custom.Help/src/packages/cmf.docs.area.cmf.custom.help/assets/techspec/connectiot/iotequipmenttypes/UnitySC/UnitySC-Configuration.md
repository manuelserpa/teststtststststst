Configuration
============
This section describe the setup for Documentation Generator Equipment Integration Test Equipment Type

Driver Definitions
=================
The following Automation Drivers are referenced on the Automation Controller as:
- **SecsGemEquipment** for the Automation Driver **UnitySCDriver**;

- **RFIDReader** for the Automation Driver **HermosLFM4xReaderDriver**;

Drivers contain the information regarding all the items needed for the automation of the equipment.
## Automation Driver Definition - UnitySCDriver
### Protocol
The protocol used by the Automation Driver referenced on the Automation Controller is named **ProtocolNamePlaceHolder** and used the package **ProtocolPackagePlaceHolder**, using version **ProtocolVersionPlaceHolder** at the time of writing.

### Properties

The table below describes the workflow properties

|Name | ID | Type | Equipment Type | Description 
:------------ | -------: | :-------- | :---------- | :-------- 
 GemClock | 2004 | String | A | Machine’s internal clock in the format YYYYMMDDhhmmsscc
 ProcessState | 2032 | String | A | State of the Processing State Machine
 CarrierID | 9101 | String | A | The ID of the carrier
 BlockedReasonText | 11605 | String | A | Description of the blocked reason
 TaskName | 11606 | String | A | EPT Task name currently running
 PreviousTaskName | 11608 | String | A | Task name completed by this module or equipment at the occurrence of EPT state transition
 TaskName_1 | 11650 | String | A | EPT Task name currently running module 1
 PreviousTaskName_1 | 11652 | String | A | Task name completed by this module or equipment at the occurrence of EPT state transition module 1
 BlockedReasonText_1 | 11655 | String | A | Description of the blocked reason module 1
 TaskName_2 | 11670 | String | A | EPT Task name currently running module 2
 PreviousTaskName_2 | 11672 | String | A | Task name completed by this module or equipment at the occurrence of EPT state transition module 2
 BlockedReasonText_2 | 11675 | String | A | Description of the blocked reason module 2
 TaskName_3 | 11690 | String | A | EPT Task name currently running module 3
 PreviousTaskName_3 | 11692 | String | A | Task name completed by this module or equipment at the occurrence of EPT state transition module 3
 BlockedReasonText_3 | 11695 | String | A | Description of the blocked reason module 3
 TaskName_4 | 11710 | String | A | EPT Task name currently running module 4
 PreviousTaskName_4 | 11712 | String | A | Task name completed by this module or equipment at the occurrence of EPT state transition module 4
 BlockedReasonText_4 | 11715 | String | A | Description of the blocked reason module 4
 SubstID | 900102 | String | A | Identifier of the Substrate
 SubstLocID | 900103 | String | A | The Substrate Location at which this Substrate currently resides
 SubstSource | 900107 | String | A | The starting Substrate Location for this Substrate
 AcquiredID | 900113 | String | A | Contains the ID read from the substrate
 CtrlJobID | 940000 | String | A | Identifier of the ControlJob
 ContentMap | 9209 | Object | L | Identifies the substrate contained in each Slot of the Carrier
 SlotMap | 9210 | Object | L | The carrier slot map
 PROCESS_STATE | 2031 | Integer | U1 | State number of the Processing State Machine
 PortID | 9102 | Integer | U1 | ID of a load port
 PortTransferState | 9110 | Integer | U1 | The current transfer state of a load port. Enumerated: OUT OF SERVICE, TRANSFER BLOCKED, READY TO LOAD, READY TO UNLOAD Super states are not included, only sub states
 CarrierIDStatus | 9206 | Integer | U1 | State of the carrier ID status.  Enumerated: ID NOT READ, [ID]WAITING FOR HOST,ID VERIFICATION OK, ID VERIFICATION FAILED
 SlotMapStatus | 9208 | Integer | U1 | State of the carrier slot map status. Enumerated: SLOT MAP NOT READ, [SLOT]WAITING FOR HOST, SLOT MAP VERIFICATION OK, SLOT MAP VERIFICATION FAILED
 EPTState | 11601 | Integer | U1 | EPT State
 PreviousEPTState | 11602 | Integer | U1 | Previous EPT State
 BlockedReason | 11604 | Integer | U1 | Identifies the blocked condition of an EPT module
 TaskType | 11607 | Integer | U1 | EPT Task type currently running
 PreviousTaskType | 11609 | Integer | U1 | Previous EPT Task type
 EPTState_1 | 11647 | Integer | U1 | EPT State Module State change for Process module 1
 PreviousEPTState_1 | 11648 | Integer | U1 | Previous EPT State module 1
 TaskType_1 | 11651 | Integer | U1 | EPT Task type currently running module 1
 PreviousTaskType_1 | 11653 | Integer | U1 | Previous EPT Task type module 1
 BlockedReason_1 | 11654 | Integer | U1 | Identifies the blocked condition of an EPT module 1
 EPTState_2 | 11667 | Integer | U1 | EPT State Module State change for Process module 2
 PreviousEPTState_2 | 11668 | Integer | U1 | Previous EPT State module 2
 TaskType_2 | 11671 | Integer | U1 | EPT Task type currently running module 2
 PreviousTaskType_2 | 11673 | Integer | U1 | Previous EPT Task type module 2
 BlockedReason_2 | 11674 | Integer | U1 | Identifies the blocked condition of an EPT module 2
 EPTState_3 | 11687 | Integer | U1 | EPT State Module State change for Process module 3
 PreviousEPTState_3 | 11688 | Integer | U1 | Previous EPT State module 3
 TaskType_3 | 11691 | Integer | U1 | EPT Task type currently running module 3
 PreviousTaskType_3 | 11693 | Integer | U1 | Previous EPT Task type module 3
 BlockedReason_3 | 11694 | Integer | U1 | Identifies the blocked condition of an EPT module 3
 EPTState_4 | 11707 | Integer | U1 | EPT State Module State change for Process module 4
 PreviousEPTState_4 | 11708 | Integer | U1 | Previous EPT State module 4
 TaskType_4 | 11711 | Integer | U1 | EPT Task type currently running module 4
 PreviousTaskType_4 | 11713 | Integer | U1 | Previous EPT Task type module 4
 BlockedReason_4 | 11714 | Integer | U1 | Identifies the blocked condition of an EPT module 4
 SubstProcState | 900106 | Integer | U1 | The Substrate's current Processing State. It is enumerated
 SubstState | 900108 | Integer | U1 | The Transport State of this Substrate. It is enumerated
 CtrlJobState | 940002 | Integer | U1 | The current state of the ControlJob Enumerated as: 0 – QUEUED 1 – SELECTED 2 – WAITINGFORSTART 3 – EXECUTING 4 – PAUSED 5 – COMPLETED
 EPTStateTime | 11603 | Integer | U4 | Time spent in previous EPT state for the equipment or module
 EPTStateTime_1 | 11649 | Integer | U4 | Time spent in previous EPT state for the equipment or module 1
 EPTStateTime_2 | 11669 | Integer | U4 | Time spent in previous EPT state for the equipment or module 2
 EPTStateTime_3 | 11689 | Integer | U4 | Time spent in previous EPT state for the equipment or module 3
 EPTStateTime_4 | 11709 | Integer | U4 | Time spent in previous EPT state for the equipment or module 4
 ControlState | 2028 | Integer | U1 | State of the Processing State Machine

### Events

#### ControlStateLocal

Event for Control State Machine switched to local (operator) control. ID: **0**

There are no Automation Events Properties defined for this Automation Event

#### ControlStateRemote

Event for Control State Machine switched to remote (host) control. ID: **1**

There are no Automation Events Properties defined for this Automation Event

#### EquipmentOffline

Event for Control State Machine switched to the offline state by the local operator. ID: **2**

There are no Automation Events Properties defined for this Automation Event

#### MaterialReceived

Event for Material arrived from a port on the equipment. ID: **3**
Linked Report Id: **1**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 9102 | Integer | Yes | U1 | ID of a load port

#### MaterialRemoved

Event for Material was sent from a port on the equipment. ID: **4**
Linked Report Id: **1**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 9102 | Integer | Yes | U1 | ID of a load port

#### ProcessingStateChange

Event for The state of the Processing State Machine has changed. ID: **11**
Linked Report Id: **8**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PROCESS_STATE | 2031 | Integer | Yes | U1 | State number of the Processing State Machine
 ProcessState | 2032 | String | Yes | A | State of the Processing State Machine

#### E87TransferBlocked2ReadytoUnload

Event for Processing for substrates contained within the carrier has completed, or a CancelCarrier. ID: **9011**
Linked Report Id: **3070**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 9102 | Integer | Yes | U1 | ID of a load port
 PortTransferState | 9110 | Integer | Yes | U1 | The current transfer state of a load port. Enumerated: OUT OF SERVICE, TRANSFER BLOCKED, READY TO LOAD, READY TO UNLOAD Super states are not included, only sub states

#### E87NoState2WaitingForHost

Event for A carrierID not currently existing at the equipment is successfully read. ID: **9015**
Linked Report Id: **3020**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 9102 | Integer | Yes | U1 | ID of a load port
 CarrierIDStatus | 9206 | Integer | Yes | U1 | State of the carrier ID status.  Enumerated: ID NOT READ, [ID]WAITING FOR HOST,ID VERIFICATION OK, ID VERIFICATION FAILED
 CarrierID | 9101 | String | Yes | A | The ID of the carrier
 SlotMapStatus | 9208 | Integer | Yes | U1 | State of the carrier slot map status. Enumerated: SLOT MAP NOT READ, [SLOT]WAITING FOR HOST, SLOT MAP VERIFICATION OK, SLOT MAP VERIFICATION FAILED

#### E87SlotMapNotRead2WaitingForHost

Event for Slot Map is read successfully and the equipment is waiting for host verification. ID: **9026**
Linked Report Id: **3030**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 9102 | Integer | Yes | U1 | ID of a load port
 CarrierID | 9101 | String | Yes | A | The ID of the carrier
 ContentMap | 9209 | Object | Yes | L | Identifies the substrate contained in each Slot of the Carrier
 SlotMap | 9210 | Object | Yes | L | The carrier slot map

#### EPTStateChange

Event for The state of EPT module has changed. ID: **11601**
Linked Report Id: **4000**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 EPTState | 11601 | Integer | Yes | U1 | EPT State
 PreviousEPTState | 11602 | Integer | Yes | U1 | Previous EPT State
 EPTStateTime | 11603 | Integer | Yes | U4 | Time spent in previous EPT state for the equipment or module
 TaskName | 11606 | String | Yes | A | EPT Task name currently running
 TaskType | 11607 | Integer | Yes | U1 | EPT Task type currently running
 PreviousTaskName | 11608 | String | Yes | A | Task name completed by this module or equipment at the occurrence of EPT state transition
 PreviousTaskType | 11609 | Integer | Yes | U1 | Previous EPT Task type
 BlockedReason | 11604 | Integer | Yes | U1 | Identifies the blocked condition of an EPT module
 BlockedReasonText | 11605 | String | Yes | A | Description of the blocked reason

#### EPTStateChange_1

Event for The state of EPT module has changed. ID: **11602**
Linked Report Id: **4010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 EPTState_1 | 11647 | Integer | Yes | U1 | EPT State Module State change for Process module 1
 PreviousEPTState_1 | 11648 | Integer | Yes | U1 | Previous EPT State module 1
 EPTStateTime_1 | 11649 | Integer | Yes | U4 | Time spent in previous EPT state for the equipment or module 1
 TaskName_1 | 11650 | String | Yes | A | EPT Task name currently running module 1
 TaskType_1 | 11651 | Integer | Yes | U1 | EPT Task type currently running module 1
 PreviousTaskName_1 | 11652 | String | Yes | A | Task name completed by this module or equipment at the occurrence of EPT state transition module 1
 PreviousTaskType_1 | 11653 | Integer | Yes | U1 | Previous EPT Task type module 1
 BlockedReason_1 | 11654 | Integer | Yes | U1 | Identifies the blocked condition of an EPT module 1
 BlockedReasonText_1 | 11655 | String | Yes | A | Description of the blocked reason module 1

#### EPTStateChange_2

Event for The state of EPT module has changed. ID: **11603**
Linked Report Id: **4020**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 EPTState_2 | 11667 | Integer | Yes | U1 | EPT State Module State change for Process module 2
 PreviousEPTState_2 | 11668 | Integer | Yes | U1 | Previous EPT State module 2
 EPTStateTime_2 | 11669 | Integer | Yes | U4 | Time spent in previous EPT state for the equipment or module 2
 TaskName_2 | 11670 | String | Yes | A | EPT Task name currently running module 2
 TaskType_2 | 11671 | Integer | Yes | U1 | EPT Task type currently running module 2
 PreviousTaskName_2 | 11672 | String | Yes | A | Task name completed by this module or equipment at the occurrence of EPT state transition module 2
 PreviousTaskType_2 | 11673 | Integer | Yes | U1 | Previous EPT Task type module 2
 BlockedReason_2 | 11674 | Integer | Yes | U1 | Identifies the blocked condition of an EPT module 2
 BlockedReasonText_2 | 11675 | String | Yes | A | Description of the blocked reason module 2

#### EPTStateChange_3

Event for The state of EPT module has changed. ID: **11604**
Linked Report Id: **4030**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 EPTState_3 | 11687 | Integer | Yes | U1 | EPT State Module State change for Process module 3
 PreviousEPTState_3 | 11688 | Integer | Yes | U1 | Previous EPT State module 3
 EPTStateTime_3 | 11689 | Integer | Yes | U4 | Time spent in previous EPT state for the equipment or module 3
 TaskName_3 | 11690 | String | Yes | A | EPT Task name currently running module 3
 TaskType_3 | 11691 | Integer | Yes | U1 | EPT Task type currently running module 3
 PreviousTaskName_3 | 11692 | String | Yes | A | Task name completed by this module or equipment at the occurrence of EPT state transition module 3
 PreviousTaskType_3 | 11693 | Integer | Yes | U1 | Previous EPT Task type module 3
 BlockedReason_3 | 11694 | Integer | Yes | U1 | Identifies the blocked condition of an EPT module 3
 BlockedReasonText_3 | 11695 | String | Yes | A | Description of the blocked reason module 3

#### EPTStateChange_4

Event for The state of EPT module has changed. ID: **11605**
Linked Report Id: **4040**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 EPTState_4 | 11707 | Integer | Yes | U1 | EPT State Module State change for Process module 4
 PreviousEPTState_4 | 11708 | Integer | Yes | U1 | Previous EPT State module 4
 EPTStateTime_4 | 11709 | Integer | Yes | U4 | Time spent in previous EPT state for the equipment or module 4
 TaskName_4 | 11710 | String | Yes | A | EPT Task name currently running module 4
 TaskType_4 | 11711 | Integer | Yes | U1 | EPT Task type currently running module 4
 PreviousTaskName_4 | 11712 | String | Yes | A | Task name completed by this module or equipment at the occurrence of EPT state transition module 4
 PreviousTaskType_4 | 11713 | Integer | Yes | U1 | Previous EPT Task type module 4
 BlockedReason_4 | 11714 | Integer | Yes | U1 | Identifies the blocked condition of an EPT module 4
 BlockedReasonText_4 | 11715 | String | Yes | A | Description of the blocked reason module 4

#### EPTStateChange_5

Event for The state of EPT module has changed. ID: **11606**

There are no Automation Events Properties defined for this Automation Event

#### E90_Subst_NeedsProcessing2InProcess

Event for Substrate starts actual processing. ID: **900110**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 900102 | String | Yes | A | Identifier of the Substrate
 SubstLocID | 900103 | String | Yes | A | The Substrate Location at which this Substrate currently resides
 SubstState | 900108 | Integer | Yes | U1 | The Transport State of this Substrate. It is enumerated
 SubstProcState | 900106 | Integer | Yes | U1 | The Substrate's current Processing State. It is enumerated
 GemClock | 2004 | String | Yes | A | Machine’s internal clock in the format YYYYMMDDhhmmsscc

#### E90_Subst_InProcess2ProcessingComplete

Event for The processing completes because the processing activities were aborted, stopped or completed. ID: **900111**
Linked Report Id: **3100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 900102 | String | Yes | A | Identifier of the Substrate
 SubstLocID | 900103 | String | Yes | A | The Substrate Location at which this Substrate currently resides
 SubstState | 900108 | Integer | Yes | U1 | The Transport State of this Substrate. It is enumerated
 SubstProcState | 900106 | Integer | Yes | U1 | The Substrate's current Processing State. It is enumerated
 GemClock | 2004 | String | Yes | A | Machine’s internal clock in the format YYYYMMDDhhmmsscc

#### E90_ID_NotConfirmed2WaitingForHost2

Event for Substrate ID was successfully read but the acquired ID is different from the one the equipment used to instantiate the substrate object. ID: **900118**
Linked Report Id: **3110**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 900102 | String | Yes | A | Identifier of the Substrate
 SubstLocID | 900103 | String | Yes | A | The Substrate Location at which this Substrate currently resides
 SubstSource | 900107 | String | Yes | A | The starting Substrate Location for this Substrate
 AcquiredID | 900113 | String | Yes | A | Contains the ID read from the substrate
 GemClock | 2004 | String | Yes | A | Machine’s internal clock in the format YYYYMMDDhhmmsscc

#### E94_Selected2Executing

Event for Control Job transitioned from the SELECTED state to the EXECUTING state. ID: **940104**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 940000 | String | Yes | A | Identifier of the ControlJob
 CtrlJobState | 940002 | Integer | Yes | U1 | The current state of the ControlJob Enumerated as: 0 – QUEUED 1 – SELECTED 2 – WAITINGFORSTART 3 – EXECUTING 4 – PAUSED 5 – COMPLETED

#### E94_Executing2Completed

Event for Control Job transitioned from the EXECUTING state to the COMPLETED state. ID: **940109**
Linked Report Id: **3010**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 940000 | String | Yes | A | Identifier of the ControlJob
 CtrlJobState | 940002 | Integer | Yes | U1 | The current state of the ControlJob Enumerated as: 0 – QUEUED 1 – SELECTED 2 – WAITINGFORSTART 3 – EXECUTING 4 – PAUSED 5 – COMPLETED

#### E87WaitingForHost2SlotMapVerificationOk

Event for A ProceedWithCarrier service is received. ID: **9027**
Linked Report Id: **3030**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 9102 | Integer | Yes | U1 | ID of a load port
 CarrierID | 9101 | String | Yes | A | The ID of the carrier
 ContentMap | 9209 | Object | Yes | L | Identifies the substrate contained in each Slot of the Carrier
 SlotMap | 9210 | Object | Yes | L | The carrier slot map

### Commands

There are no Automation Commands defined for this Automation Driver Definition

## Automation Driver Definition - HermosLFM4xReaderDriver
Information on this driver is contained on an additional driver related document.

