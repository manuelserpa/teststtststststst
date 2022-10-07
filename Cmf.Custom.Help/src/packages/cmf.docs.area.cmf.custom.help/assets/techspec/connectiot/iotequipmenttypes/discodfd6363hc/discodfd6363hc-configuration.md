Configuration
============
This section describe the setup for DISCO DFD 6363 HC Equipment Type

Driver Definitions
=================
The following Automation Drivers are referenced on the Automation Controller as:
- **SecsGemEquipment** for the Automation Driver **DISCODFD6363HCDriver**;

- **RFIDReader** for the Automation Driver **HermosLFM4xReaderDriver**;

- **FileDriver** for the Automation Driver **FileRawDriver**;

Drivers contain the information regarding all the items needed for the automation of the equipment.
## Automation Driver Definition - DISCODFD6363HCDriver
### Protocol
The protocol used by the Automation Driver referenced on the Automation Controller is named **SecsGem** and used the package **criticalmanufacturing/connect-iot-driver-secsgem**, using version **9.1.0-202209072** at the time of writing.

### Properties

The table below describes the workflow properties

|Name | ID | Type | Equipment Type | Description 
:------------ | -------: | :-------- | :---------- | :-------- 
 CLOCK | 1004 | String | A | Clock
 SLOT_ID_ROBOT_PICK | 3096 | Integer | U1 | SlotNo on Robot Pick
 SLOT_ID_PT | 3095 | Integer | U1 | Slot No on PT
 MAP_STATE_1 | 12066 | Integer | U1 | Slot1 state of FOUP Mapping Measure
 MAP_STATE_2 | 12065 | Integer | U1 | Slot2 state of FOUP Mapping Measure
 MAP_STATE_3 | 12064 | Integer | U1 | Slot3 state of FOUP Mapping Measure
 MAP_STATE_4 | 12063 | Integer | U1 | Slot4 state of FOUP Mapping Measure
 MAP_STATE_5 | 12062 | Integer | U1 | Slot5 state of FOUP Mapping Measure
 MAP_STATE_6 | 12061 | Integer | U1 | Slot6 state of FOUP Mapping Measure
 MAP_STATE_7 | 12060 | Integer | U1 | Slot7 state of FOUP Mapping Measure
 MAP_STATE_8 | 12059 | Integer | U1 | Slot8 state of FOUP Mapping Measure
 MAP_STATE_9 | 12058 | Integer | U1 | Slot9 state of FOUP Mapping Measure
 MAP_STATE_10 | 12057 | Integer | U1 | Slot10 state of FOUP Mapping Measure
 MAP_STATE_11 | 12056 | Integer | U1 | Slot11 state of FOUP Mapping Measure
 MAP_STATE_12 | 12055 | Integer | U1 | Slot12 state of FOUP Mapping Measure
 MAP_STATE_13 | 12054 | Integer | U1 | Slot13 state of FOUP Mapping Measure
 MAP_STATE_14 | 12053 | Integer | U1 | Slot14 state of FOUP Mapping Measure
 MAP_STATE_15 | 12052 | Integer | U1 | Slot15 state of FOUP Mapping Measure
 MAP_STATE_16 | 12051 | Integer | U1 | Slot16 state of FOUP Mapping Measure
 MAP_STATE_17 | 12050 | Integer | U1 | Slot17 state of FOUP Mapping Measure
 MAP_STATE_18 | 12049 | Integer | U1 | Slot18 state of FOUP Mapping Measure
 MAP_STATE_19 | 12048 | Integer | U1 | Slot19 state of FOUP Mapping Measure
 MAP_STATE_20 | 12047 | Integer | U1 | Slot20 state of FOUP Mapping Measure
 MAP_STATE_21 | 12046 | Integer | U1 | Slot21 state of FOUP Mapping Measure
 MAP_STATE_22 | 12045 | Integer | U1 | Slot22 state of FOUP Mapping Measure
 MAP_STATE_23 | 12044 | Integer | U1 | Slot23 state of FOUP Mapping Measure
 MAP_STATE_24 | 12043 | Integer | U1 | Slot24 state of FOUP Mapping Measure
 MAP_STATE_25 | 12042 | Integer | U1 | Slot25 state of FOUP Mapping Measure
 WORK_1 | 1700 | String | A | Cassette Status 1
 WORK_2 | 1701 | String | A | Cassette Status 2
 WORK_3 | 1702 | String | A | Cassette Status 3
 WORK_4 | 1703 | String | A | Cassette Status 4
 WORK_5 | 1704 | String | A | Cassette Status 5
 WORK_6 | 1705 | String | A | Cassette Status 6
 WORK_7 | 1706 | String | A | Cassette Status 7
 WORK_8 | 1707 | String | A | Cassette Status 8
 WORK_9 | 1708 | String | A | Cassette Status 9
 WORK_10 | 1709 | String | A | Cassette Status 10
 WORK_11 | 1710 | String | A | Cassette Status 11
 WORK_12 | 1711 | String | A | Cassette Status 12
 WORK_13 | 1712 | String | A | Cassette Status 13
 WORK_14 | 1713 | String | A | Cassette Status 14
 WORK_15 | 1714 | String | A | Cassette Status 15
 WORK_16 | 1715 | String | A | Cassette Status 16
 WORK_17 | 1716 | String | A | Cassette Status 17
 WORK_18 | 1717 | String | A | Cassette Status 18
 WORK_19 | 1718 | String | A | Cassette Status 19
 WORK_20 | 1719 | String | A | Cassette Status 20
 WORK_21 | 1720 | String | A | Cassette Status 21
 WORK_22 | 1721 | String | A | Cassette Status 22
 WORK_23 | 1722 | String | A | Cassette Status 23
 WORK_24 | 1723 | String | A | Cassette Status 24
 WORK_25 | 1724 | String | A | Cassette Status 25
 WORK_INFO_1 | 13652 | Integer | U4 | Work Information 1
 WORK_INFO_2 | 13653 | Integer | U4 | Work Information 2
 WORK_INFO_3 | 13654 | Integer | U4 | Work Information 3
 WORK_INFO_4 | 13655 | Integer | U4 | Work Information 4
 WORK_INFO_5 | 13656 | Integer | U4 | Work Information 5
 WORK_INFO_6 | 13657 | Integer | U4 | Work Information 6
 WORK_INFO_7 | 13658 | Integer | U4 | Work Information 7
 WORK_INFO_8 | 13659 | Integer | U4 | Work Information 8
 WORK_INFO_9 | 13660 | Integer | U4 | Work Information 9
 WORK_INFO_10 | 13661 | Integer | U4 | Work Information 10
 WORK_INFO_11 | 13662 | Integer | U4 | Work Information 11
 WORK_INFO_12 | 13663 | Integer | U4 | Work Information 12
 WORK_INFO_13 | 13664 | Integer | U4 | Work Information 13
 WORK_INFO_14 | 13665 | Integer | U4 | Work Information 14
 WORK_INFO_15 | 13666 | Integer | U4 | Work Information 15
 WORK_INFO_16 | 13667 | Integer | U4 | Work Information 16
 WORK_INFO_17 | 13668 | Integer | U4 | Work Information 17
 WORK_INFO_18 | 13669 | Integer | U4 | Work Information 18
 WORK_INFO_19 | 13670 | Integer | U4 | Work Information 19
 WORK_INFO_20 | 13671 | Integer | U4 | Work Information 20
 WORK_INFO_21 | 13672 | Integer | U4 | Work Information 21
 WORK_INFO_22 | 13673 | Integer | U4 | Work Information 22
 WORK_INFO_23 | 13674 | Integer | U4 | Work Information 23
 WORK_INFO_24 | 13675 | Integer | U4 | Work Information 24
 WORK_INFO_25 | 13676 | Integer | U4 | Work Information 25
 WORK_POS_DEV_NO_0 | 1232 | String | A | Device  No. of wafer on C/T
 WORK_POS_DEV_NO_1 | 1222 | String | A | Device  No. of wafer on centering guide
 WORK_POS_DEV_NO_2 | 1252 | String | A | Device  No. of wafer on load arm
 WORK_POS_DEV_NO_3 | 1262 | String | A | Device  No. of wafer on clean arm
 WORK_POS_DEV_NO_4 | 1242 | String | A | Device  No. of wafer on S/T
 WORK_POS_DEV_NO_5 | 1272 | String | A | Device  No. of wafer on inspection stage
 WORK_POS_DEV_NO_6 | 1282 | String | A | Device  No. of wafer onUV stage
 SLOT_ID_WORK_GUIDE | 1223 | Integer | U2 | Slot No. on centering guide
 SLOT_ID_WORK_CT | 1233 | Integer | U4 | Slot No. on C/T
 SLOT_ID_WORK_ST | 1243 | Integer | U2 | Slot No. on S/T
 ST_DEV | 1244 | String | A | Device  No. of wafer on S/T
 SLOT_ID_WORK_LOADARM | 1253 | Integer | U4 | Slot No. of wafer on load arm
 SLOT_ID_WORK_CLNARM | 1263 | Integer | U2 | Slot No. of wafer on clean arm
 SLOT_ID_WORK_INSPEC | 1273 | Integer | U2 | Slot No. of wafer on inspection stage
 SLOT_ID_WORK_UV | 1283 | Integer | U2 | Slot No. of wafer on UV stage
 PROCESS_STATE | 1009 | Integer | U1 | Process State
 PREVIOUS_PROCESS_STATE | 1008 | Integer | U1 | Previous Process State
 CONTROL_STATE | 1005 | Integer | U1 | Control State

### Events

#### CUT_START

Event for Cut Start. ID: **1**
Linked Report Id: **44**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SLOT_ID_ROBOT_PICK | 3096 | Integer | Yes | U1 | SlotNo on Robot Pick

#### CUT_END

Event for Cut End. ID: **2**
Linked Report Id: **44**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SLOT_ID_ROBOT_PICK | 3096 | Integer | Yes | U1 | SlotNo on Robot Pick

#### PRECUT_START

Event for Pre-Cut Start. ID: **3**

There are no Automation Events Properties defined for this Automation Event

#### PRECUT_end

Event for Pre-Cut End. ID: **4**

There are no Automation Events Properties defined for this Automation Event

#### ALIGNMENT_START

Event for Alignment Start. ID: **5**

There are no Automation Events Properties defined for this Automation Event

#### ALIGNMENT_END

Event for Alignment End. ID: **6**

There are no Automation Events Properties defined for this Automation Event

#### KERF_CHECK_START

Event for Kerf Check Start. ID: **7**

There are no Automation Events Properties defined for this Automation Event

#### KERF_CHECK_END

Event for Kerf Check End. ID: **8**

There are no Automation Events Properties defined for this Automation Event

#### CLEAN_START

Event for Clean Start. ID: **9**

There are no Automation Events Properties defined for this Automation Event

#### CLEAN_END

Event for Clean End. ID: **10**

There are no Automation Events Properties defined for this Automation Event

#### SETUP_START

Event for Setup Start. ID: **11**

There are no Automation Events Properties defined for this Automation Event

#### SETUP_END

Event for Setup End. ID: **12**

There are no Automation Events Properties defined for this Automation Event

#### BLADE_CHANGE_START

Event for Blade Change Start. ID: **13**

There are no Automation Events Properties defined for this Automation Event

#### BLADE_CHANGE_END

Event for Blade Change End. ID: **14**

There are no Automation Events Properties defined for this Automation Event

#### BLADE_DRESS_START

Event for Blade Dress Start. ID: **15**

There are no Automation Events Properties defined for this Automation Event

#### BLADE_DRESS_END

Event for Blade Dress End. ID: **16**

There are no Automation Events Properties defined for this Automation Event

#### WAFER_LOAD_START

Event for Start to Wafer Load. ID: **20**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SLOT_ID_WORK_LOADARM | 1253 | Integer | Yes | U4 | Slot No. of wafer on load arm
 SLOT_ID_ROBOT_PICK | 3096 | Integer | Yes | U1 | SlotNo on Robot Pick

#### WAFER_LOAD_END

Event for Wafer/Work has been loaded. ID: **21**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SLOT_ID_WORK_LOADARM | 1253 | Integer | Yes | U4 | Slot No. of wafer on load arm
 SLOT_ID_ROBOT_PICK | 3096 | Integer | Yes | U1 | SlotNo on Robot Pick

#### WAFER_UNLOAD_END

Event for Wafer/Work has been Unloaded. ID: **22**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SLOT_ID_WORK_LOADARM | 1253 | Integer | Yes | U4 | Slot No. of wafer on load arm
 SLOT_ID_ROBOT_PICK | 3096 | Integer | Yes | U1 | SlotNo on Robot Pick

#### WAFER_LOADING_START

Event for Start to Supply works (Loader Restart). ID: **23**

There are no Automation Events Properties defined for this Automation Event

#### WAFER_LOADING_STOP

Event for Stop/Pause to Supply works (Loader Stop). ID: **24**

There are no Automation Events Properties defined for this Automation Event

#### SYSTEM_INITIALIZE_START

Event for Press <SYS INI>. ID: **25**

There are no Automation Events Properties defined for this Automation Event

#### SYSTEM_INITIALIZE_END

Event for System has been initialized. ID: **26**

There are no Automation Events Properties defined for this Automation Event

#### NEW_CASSETTE_SET

Event for System has been initialized. ID: **27**

There are no Automation Events Properties defined for this Automation Event

#### WAFER_UNLOAD_START

Event for Wafer Unload Start. ID: **32**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SLOT_ID_WORK_LOADARM | 1253 | Integer | Yes | U4 | Slot No. of wafer on load arm
 SLOT_ID_ROBOT_PICK | 3096 | Integer | Yes | U1 | SlotNo on Robot Pick

#### WAFER_TO_CLEAN_START

Event for Wafer to Clean Start. ID: **33**

There are no Automation Events Properties defined for this Automation Event

#### WAFER_TO_CLEAN_END

Event for Wafer to Clean End. ID: **34**

There are no Automation Events Properties defined for this Automation Event

#### MAPPING_START

Event for Mapping Start. ID: **35**

There are no Automation Events Properties defined for this Automation Event

#### MAPPING_END

Event for Mapping End. ID: **36**
Linked Report Id: **36**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 MAP_STATE_1 | 12066 | Integer | Yes | U1 | Slot1 state of FOUP Mapping Measure
 MAP_STATE_2 | 12065 | Integer | Yes | U1 | Slot2 state of FOUP Mapping Measure
 MAP_STATE_3 | 12064 | Integer | Yes | U1 | Slot3 state of FOUP Mapping Measure
 MAP_STATE_4 | 12063 | Integer | Yes | U1 | Slot4 state of FOUP Mapping Measure
 MAP_STATE_5 | 12062 | Integer | Yes | U1 | Slot5 state of FOUP Mapping Measure
 MAP_STATE_6 | 12061 | Integer | Yes | U1 | Slot6 state of FOUP Mapping Measure
 MAP_STATE_7 | 12060 | Integer | Yes | U1 | Slot7 state of FOUP Mapping Measure
 MAP_STATE_8 | 12059 | Integer | Yes | U1 | Slot8 state of FOUP Mapping Measure
 MAP_STATE_9 | 12058 | Integer | Yes | U1 | Slot9 state of FOUP Mapping Measure
 MAP_STATE_10 | 12057 | Integer | Yes | U1 | Slot10 state of FOUP Mapping Measure
 MAP_STATE_11 | 12056 | Integer | Yes | U1 | Slot11 state of FOUP Mapping Measure
 MAP_STATE_12 | 12055 | Integer | Yes | U1 | Slot12 state of FOUP Mapping Measure
 MAP_STATE_13 | 12054 | Integer | Yes | U1 | Slot13 state of FOUP Mapping Measure
 MAP_STATE_14 | 12053 | Integer | Yes | U1 | Slot14 state of FOUP Mapping Measure
 MAP_STATE_15 | 12052 | Integer | Yes | U1 | Slot15 state of FOUP Mapping Measure
 MAP_STATE_16 | 12051 | Integer | Yes | U1 | Slot16 state of FOUP Mapping Measure
 MAP_STATE_17 | 12050 | Integer | Yes | U1 | Slot17 state of FOUP Mapping Measure
 MAP_STATE_18 | 12049 | Integer | Yes | U1 | Slot18 state of FOUP Mapping Measure
 MAP_STATE_19 | 12048 | Integer | Yes | U1 | Slot19 state of FOUP Mapping Measure
 MAP_STATE_20 | 12047 | Integer | Yes | U1 | Slot20 state of FOUP Mapping Measure
 MAP_STATE_21 | 12046 | Integer | Yes | U1 | Slot21 state of FOUP Mapping Measure
 MAP_STATE_22 | 12045 | Integer | Yes | U1 | Slot22 state of FOUP Mapping Measure
 MAP_STATE_23 | 12044 | Integer | Yes | U1 | Slot23 state of FOUP Mapping Measure
 MAP_STATE_24 | 12043 | Integer | Yes | U1 | Slot24 state of FOUP Mapping Measure
 MAP_STATE_25 | 12042 | Integer | Yes | U1 | Slot25 state of FOUP Mapping Measure

#### CT_PROCESS_COMPLETED

Event for Dicer finished the process on the table. ID: **37**

There are no Automation Events Properties defined for this Automation Event

#### FULL_AUTOMATION_INITIALIZE_START

Event for <F1>: Single dicing device
<F2>: Multi dicing device. ID: **41**

There are no Automation Events Properties defined for this Automation Event

#### FULL_AUTOMATION_INITIALIZE_END

Event for System was initialize for full automation. ID: **42**

There are no Automation Events Properties defined for this Automation Event

#### FULL_AUTOMATION_START

Event for Press <START>. ID: **43**

There are no Automation Events Properties defined for this Automation Event

#### FULL_AUTOMATION_END

Event for Cassettes have been processed. ID: **44**

There are no Automation Events Properties defined for this Automation Event

#### FULL_AUTOMATION_PAUSE_STOP

Event for Press <STOP> while full automation is processing. ID: **45**

There are no Automation Events Properties defined for this Automation Event

#### FULL_AUTOMATION_RESTART

Event for Press <START> while full automation is pause. ID: **46**

There are no Automation Events Properties defined for this Automation Event

#### HOST_MESSAGE_RECOGNIZE

Event for Operator recognized the arrived host message. ID: **71**

There are no Automation Events Properties defined for this Automation Event

#### PROCESS_PROGRAM_CHANGE

Event for Device data was revised. ID: **72**

There are no Automation Events Properties defined for this Automation Event

#### PP_SELECTED

Event for Device data was selected. ID: **73**

There are no Automation Events Properties defined for this Automation Event

#### OFF-LINE

Event for . ID: **74**

There are no Automation Events Properties defined for this Automation Event

#### LOCAL

Event for . ID: **75**

There are no Automation Events Properties defined for this Automation Event

#### REMOTE

Event for . ID: **76**

There are no Automation Events Properties defined for this Automation Event

#### NEUTRAL

Event for . ID: **77**

There are no Automation Events Properties defined for this Automation Event

#### Orifla_Notch_Search_Start

Event for . ID: **85**

There are no Automation Events Properties defined for this Automation Event

#### Orifla_Notch_Search_End

Event for . ID: **86**

There are no Automation Events Properties defined for this Automation Event

#### Process_Program_Created

Event for . ID: **121**

There are no Automation Events Properties defined for this Automation Event

#### Process_Program_Rename

Event for . ID: **122**

There are no Automation Events Properties defined for this Automation Event

#### Process_Program_Deleted

Event for . ID: **123**

There are no Automation Events Properties defined for this Automation Event

#### Process_Program_Moved

Event for . ID: **124**

There are no Automation Events Properties defined for this Automation Event

#### PROCESS_STATE_CHANGED

Event for . ID: **150**

There are no Automation Events Properties defined for this Automation Event

#### FrameTransferSubProcessStateChange

Event for . ID: **151**

There are no Automation Events Properties defined for this Automation Event

#### CTArmSubProcessStateChange

Event for . ID: **152**

There are no Automation Events Properties defined for this Automation Event

#### STArmSubProcessStateChange

Event for . ID: **153**

There are no Automation Events Properties defined for this Automation Event

#### ClnArmSubProcessStateChange

Event for . ID: **154**

There are no Automation Events Properties defined for this Automation Event

#### SpoolingActivated

Event for Spooling is activated. ID: **170**

There are no Automation Events Properties defined for this Automation Event

#### SpoolingDeactivated

Event for Spooling is deactivated. ID: **171**

There are no Automation Events Properties defined for this Automation Event

#### SpoolTransmitFailuer

Event for Spooling transmission was failed. ID: **172**

There are no Automation Events Properties defined for this Automation Event

#### ALI_NSD_START

Event for . ID: **178**

There are no Automation Events Properties defined for this Automation Event

#### ALI_NSD_END

Event for . ID: **179**

There are no Automation Events Properties defined for this Automation Event

#### WaferIDRead

Event for . ID: **202**
Linked Report Id: **202**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SLOT_ID_PT | 3095 | Integer | Yes | U1 | Slot No on PT
 SLOT_ID_ROBOT_PICK | 3096 | Integer | Yes | U1 | SlotNo on Robot Pick

#### Smif_Load_Start

Event for . ID: **472**

There are no Automation Events Properties defined for this Automation Event

#### Smif_Load_End

Event for . ID: **473**

There are no Automation Events Properties defined for this Automation Event

#### Smif_UnLoad_Start

Event for . ID: **474**

There are no Automation Events Properties defined for this Automation Event

#### Smif_UnLoad_End

Event for . ID: **475**

There are no Automation Events Properties defined for this Automation Event

#### Smif_Pod_Arrived

Event for . ID: **481**

There are no Automation Events Properties defined for this Automation Event

#### Smif_Pod_Removed

Event for . ID: **482**

There are no Automation Events Properties defined for this Automation Event

#### Smif_Pod_Locked

Event for . ID: **483**

There are no Automation Events Properties defined for this Automation Event

#### Smif_Pod_UnLocked

Event for . ID: **484**

There are no Automation Events Properties defined for this Automation Event

#### SMIF_POD_READY_TO_LOAD

Event for . ID: **485**

There are no Automation Events Properties defined for this Automation Event

#### SMIF_POD_READY_TO_UNLOAD

Event for . ID: **486**

There are no Automation Events Properties defined for this Automation Event

#### Smif_Cass_UnloadedFromPod

Event for . ID: **487**

There are no Automation Events Properties defined for this Automation Event

#### Smif_Cass_LoadedToPod

Event for . ID: **488**

There are no Automation Events Properties defined for this Automation Event

### Commands

#### GO_LOCAL

Command for Local state transition. ID: **GO_LOCAL**

There are no Automation Command Parameters defined for this Automation Command

#### GO_REMOTE

Command for Remote state transition. ID: **GO_REMOTE**

There are no Automation Command Parameters defined for this Automation Command

#### LOCK_POD

Command for Lock Pod. ID: **LOCK_POD**

There are no Automation Command Parameters defined for this Automation Command

#### LOAD_POD

Command for Load Pod. ID: **UNLOAD_CASS_FRM_PORT**

There are no Automation Command Parameters defined for this Automation Command

#### INIT_S

Command for Single device full automation initialization processing. ID: **INIT_S**

There are no Automation Command Parameters defined for this Automation Command

#### NEW

Command for New cassette setting. ID: **NEW**

There are no Automation Command Parameters defined for this Automation Command

#### PP_SELECT_S

Command for Single device process program selection. ID: **PP_SELECT_S**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | U1 | 
 PPID | Yes | A | 

#### START_S

Command for Single device full automation process stop. ID: **START_S**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | U1 | 

#### GRANT_DENY

Command for Grant Deny. ID: **GRANT_DENY**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | U1 | 

#### UNLOAD_POD

Command for Unload Pod. ID: **LOAD_CASS_TO_PORT**

There are no Automation Command Parameters defined for this Automation Command

#### UNLOCK_POD

Command for Unlock Pod. ID: **UNLOCK_POD**

There are no Automation Command Parameters defined for this Automation Command

#### STOP

Command for Full Automation Stop. ID: **STOP**

There are no Automation Command Parameters defined for this Automation Command

#### PAUSE

Command for Full Automation Pause. ID: **PAUSE**

There are no Automation Command Parameters defined for this Automation Command

#### RESUME

Command for Full Automation Resume. ID: **RESUME**

There are no Automation Command Parameters defined for this Automation Command

#### PAUSE_H

Command for Full Automation Pause (host control). ID: **PAUSE_H**

There are no Automation Command Parameters defined for this Automation Command

#### RESUME_H

Command for Full Automation Resume (host control). ID: **RESUME_H**

There are no Automation Command Parameters defined for this Automation Command

#### ABORT

Command for Forced system initialization. ID: **ABORT**

There are no Automation Command Parameters defined for this Automation Command

#### EMERGENCY

Command for Z-EM. ID: **EMERGENCY**

There are no Automation Command Parameters defined for this Automation Command

#### CLEAR

Command for Unloading of all the wafers. ID: **CLEAR**

There are no Automation Command Parameters defined for this Automation Command

#### RECOVERY

Command for Error Recovery Display. ID: **RECOVERY**

There are no Automation Command Parameters defined for this Automation Command

## Automation Driver Definition - HermosLFM4xReaderDriver
Information on this driver is contained on an additional driver related document.

## Automation Driver Definition - FileRawDriver
Information on this driver is contained on an additional driver related document.

