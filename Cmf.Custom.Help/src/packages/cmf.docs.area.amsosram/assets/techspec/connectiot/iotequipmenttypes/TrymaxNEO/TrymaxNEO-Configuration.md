Configuration
============
This section describe the setup for Trymax NEO Equipment Type

Driver Definition
=================
The following Automation Driver is referenced on the Automation Controller as **SecsGemEquipment** is the Automation Driver **TrymaxNEODriver**, this driver contains the information regarding all the items needed for the automation of the equipment.

## Automation Driver Definition - TrymaxNEODriver
### Protocol
The protocol used by the Automation Driver referenced on the Automation Controller is named **SecsGem** and used the package **criticalmanufacturing/connect-iot-driver-secsgem**, using version **8.3.3-202201212** at the time of writing.

### Properties

The table below describes the workflow properties

|Name | ID | Type | Equipment Type | Description 
:------------ | -------: | :-------- | :---------- | :-------- 
 ALARM_ID | 1312 | Integer | U4 | 
 ALARM_TEXT | 1320 | String | A | 
 CLOCK | 1006 | String | A | 
 CONTROL_STATE | 1000 | Integer | U2 | 
 CS_LOT_ID_LIST | 6001 | Object | L | 
 CS_OPERATION_MODE | 6000 | String | A | 
 CS_WAFER_ID_LIST | 6003 | Object | L | 
 LOT_ID | 1308 | String | A | 
 PM1_CURRENT_RECIPE | 4000 | String | A | 
 PM1_CURRENT_SEQUENCE | 4001 | String | A | 
 PM1_LOT_ID | 4003 | String | A | 
 PM1_OPERATION_MODE | 4002 | String | A | 
 PM1_WAFER_ID | 4005 | String | A | 
 PM2_CURRENT_RECIPE | 5000 | String | A | 
 PM2_CURRENT_SEQUENCE | 5001 | String | A | 
 PM2_LOT_ID | 5003 | String | A | 
 PM2_OPERATION_MODE | 5002 | String | A | 
 PM2_WAFER_ID | 5005 | String | A | 
 PM3_CURRENT_RECIPE | 12000 | String | A | 
 PM3_CURRENT_SEQUENCE | 12001 | String | A | 
 PM3_LOT_ID | 12003 | String | A | 
 PM3_OPERATION_MODE | 12002 | String | A | 
 PM3_WAFER_ID | 12005 | String | A | 
 PM4_CURRENT_RECIPE | 13000 | String | A | 
 PM4_CURRENT_SEQUENCE | 13001 | String | A | 
 PM4_LOT_ID | 13003 | String | A | 
 PM4_OPERATION_MODE | 13002 | String | A | 
 PM4_WAFER_ID | 13005 | String | A | 
 POD1_MATERIAL_MAP_LIST | 8002 | Object | L | 
 POD2_MATERIAL_MAP_LIST | 9002 | Object | L | 
 POD3_MATERIAL_MAP_LIST | 10002 | Object | L | 
 POD4_MATERIAL_MAP_LIST | 11002 | Object | L | 
 PORT_ID | 1309 | Integer | I1 | 
 PP_CHANGE_NAME | 1322 | String | A | 
 PP_CHANGE_STATE | 1323 | Integer | U1 | 
 PPID | 1301 | String | A | 
 PROCESS_STATE | 1317 | String | A | 
 REMOTE_COMMAND_NAME | 1310 | String | A | 
 SLOT_MAP | 1306 | Object | L | 
 SUBST_LOC_ID | 1307 | String | A | 
 SUBSTRATE_COUNT | 1311 | Integer | I8 | 
 SUBSTRATE_ID | 1300 | String | A | 
 SUCCESSFULLY_PROCESSED_SUBSTRATE_COUNT | 1324 | Integer | I8 | 
 TM_E10STATE | 7004 | String | A | 
 TM_LOT_ID_LIST | 7001 | Object | L | 
 TM_OPERATION_MODE | 7000 | String | A | 
 TM_WAFER_ID_LIST | 7003 | Object | L | 
 POD1_CARRIER_PRESENT | 8007 | String | A | 
 POD2_CARRIER_PRESENT | 9007 | String | A | 
 POD3_CARRIER_PRESENT | 10007 | String | A | 
 POD4_CARRIER_PRESENT | 11007 | String | A | 

### Events

#### CONTROL_STATE_OFFLINE

Event for * Control State change to Offline. ID: **1000**

There are no Automation Events Properties defined for this Automation Event

#### CONTROL_STATE_REMOTE

Event for * Control State change to OnlineRemote. ID: **1002**

There are no Automation Events Properties defined for this Automation Event

#### CONTROL_STATE_LOCAL

Event for * Control State change to OnlineLocal. ID: **1001**

There are no Automation Events Properties defined for this Automation Event

#### ALARM_SET

Event for * AlarmNDetected  An alarm was set. ID: **1003**
Linked Report Id: **14**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 ALARM_ID | 1312 | Integer | Yes | U4 | 
 ALARM_TEXT | 1320 | String | Yes | A | 

#### ALARM_CLEARED

Event for * AlarmNCleared  An alarm wascleared. ID: **1004**
Linked Report Id: **14**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 ALARM_ID | 1312 | Integer | Yes | U4 | 
 ALARM_TEXT | 1320 | String | Yes | A | 

#### CONSTANT_CHANGED

Event for * An equipment constant was changed by the operator. ID: **1008**

There are no Automation Events Properties defined for this Automation Event

#### REMOTE_COMMAND_SUCCESS

Event for . ID: **1100**
Linked Report Id: **11**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 REMOTE_COMMAND_NAME | 1310 | String | Yes | A | 

#### REMOTE_COMMAND_FAILURE

Event for . ID: **1101**
Linked Report Id: **11**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 REMOTE_COMMAND_NAME | 1310 | String | Yes | A | 

#### PP_CHANGED

Event for * PPChangeEvent. ID: **1200**
Linked Report Id: **13**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PP_CHANGE_NAME | 1322 | String | Yes | A | 
 PP_CHANGE_STATE | 1323 | Integer | Yes | U1 | 

#### PROCESSING_STARTED_OLD

Event for * Process is started. Occurs when the first wafer of a job is taken out of a carrier.. ID: **2000**

There are no Automation Events Properties defined for this Automation Event

#### PROCESSING_COMPLETED

Event for . ID: **2001**
Linked Report Id: **2**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 LOT_ID | 1308 | String | Yes | A | 
 PORT_ID | 1309 | Integer | Yes | I1 | 
 PPID | 1301 | String | Yes | A | 
 SUBSTRATE_COUNT | 1311 | Integer | Yes | I8 | 
 SUCCESSFULLY_PROCESSED_SUBSTRATE_COUNT | 1324 | Integer | Yes | I8 | 

#### PROCESS_STATE_CHANGED

Event for . ID: **2002**

There are no Automation Events Properties defined for this Automation Event

#### PROCESSING_STARTED

Event for * A process job has been started (switched to state Processing). ID: **2904**
Linked Report Id: **1**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 LOT_ID | 1308 | String | Yes | A | 
 PORT_ID | 1309 | Integer | Yes | I1 | 

#### TM_RECEIVED_MATERIAL

Event for . ID: **3000**
Linked Report Id: **4**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SUBSTRATE_ID | 1300 | String | Yes | A | 
 SUBST_LOC_ID | 1307 | String | Yes | A | 

#### TM_SENT_MATERIAL

Event for . ID: **3001**
Linked Report Id: **4**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SUBSTRATE_ID | 1300 | String | Yes | A | 
 SUBST_LOC_ID | 1307 | String | Yes | A | 

#### POD1_RECEIVED_MATERIAL

Event for . ID: **3008**
Linked Report Id: **4**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SUBSTRATE_ID | 1300 | String | Yes | A | 
 SUBST_LOC_ID | 1307 | String | Yes | A | 

#### POD1_SENT_MATERIAL

Event for . ID: **3009**
Linked Report Id: **4**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SUBSTRATE_ID | 1300 | String | Yes | A | 
 SUBST_LOC_ID | 1307 | String | Yes | A | 

#### POD2_RECEIVED_MATERIAL

Event for . ID: **3010**
Linked Report Id: **4**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SUBSTRATE_ID | 1300 | String | Yes | A | 
 SUBST_LOC_ID | 1307 | String | Yes | A | 

#### POD2_SENT_MATERIAL

Event for . ID: **3011**
Linked Report Id: **4**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SUBSTRATE_ID | 1300 | String | Yes | A | 
 SUBST_LOC_ID | 1307 | String | Yes | A | 

#### POD3_RECEIVED_MATERIAL

Event for . ID: **3012**
Linked Report Id: **4**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SUBSTRATE_ID | 1300 | String | Yes | A | 
 SUBST_LOC_ID | 1307 | String | Yes | A | 

#### POD3_SENT_MATERIAL

Event for . ID: **3013**
Linked Report Id: **4**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SUBSTRATE_ID | 1300 | String | Yes | A | 
 SUBST_LOC_ID | 1307 | String | Yes | A | 

#### POD4_RECEIVED_MATERIAL

Event for . ID: **3014**
Linked Report Id: **4**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SUBSTRATE_ID | 1300 | String | Yes | A | 
 SUBST_LOC_ID | 1307 | String | Yes | A | 

#### POD4_SENT_MATERIAL

Event for . ID: **3015**
Linked Report Id: **4**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SUBSTRATE_ID | 1300 | String | Yes | A | 
 SUBST_LOC_ID | 1307 | String | Yes | A | 

#### PM1_PROCESSING_STARTED

Event for . ID: **4000**

There are no Automation Events Properties defined for this Automation Event

#### PM1_PROCESSING_COMPLETED

Event for . ID: **4001**

There are no Automation Events Properties defined for this Automation Event

#### PM2_PROCESSING_STARTED

Event for . ID: **5000**

There are no Automation Events Properties defined for this Automation Event

#### PM2_PROCESSING_COMPLETED

Event for . ID: **5001**

There are no Automation Events Properties defined for this Automation Event

#### PM3_PROCESSING_STARTED

Event for . ID: **13000**

There are no Automation Events Properties defined for this Automation Event

#### PM3_PROCESSING_COMPLETED

Event for . ID: **13001**

There are no Automation Events Properties defined for this Automation Event

#### PM4_PROCESSING_STARTED

Event for . ID: **14000**

There are no Automation Events Properties defined for this Automation Event

#### PM4_PROCESSING_COMPLETED

Event for . ID: **14001**

There are no Automation Events Properties defined for this Automation Event

#### CS_PROCESSING_STARTED

Event for . ID: **6000**

There are no Automation Events Properties defined for this Automation Event

#### CS_PROCESSING_COMPLETED

Event for . ID: **6001**

There are no Automation Events Properties defined for this Automation Event

#### POD1_ARRIVED

Event for . ID: **8000**

There are no Automation Events Properties defined for this Automation Event

#### POD1_DEPARTED

Event for . ID: **8001**

There are no Automation Events Properties defined for this Automation Event

#### POD1_MAPPED

Event for . ID: **8004**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 POD1_MATERIAL_MAP_LIST | 8002 | Object | Yes | L | 
 POD2_MATERIAL_MAP_LIST | 9002 | Object | Yes | L | 
 POD3_MATERIAL_MAP_LIST | 10002 | Object | Yes | L | 
 POD4_MATERIAL_MAP_LIST | 11002 | Object | Yes | L | 
 SLOT_MAP | 1306 | Object | Yes | L | 

#### POD1_MAP_STARTED

Event for . ID: **8006**

There are no Automation Events Properties defined for this Automation Event

#### POD1_UNLOAD_COMPLETE

Event for . ID: **8010**

There are no Automation Events Properties defined for this Automation Event

#### POD2_ARRIVED

Event for . ID: **9000**

There are no Automation Events Properties defined for this Automation Event

#### POD2_DEPARTED

Event for . ID: **9001**

There are no Automation Events Properties defined for this Automation Event

#### POD2_MAPPED

Event for . ID: **9004**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 POD1_MATERIAL_MAP_LIST | 8002 | Object | Yes | L | 
 POD2_MATERIAL_MAP_LIST | 9002 | Object | Yes | L | 
 POD3_MATERIAL_MAP_LIST | 10002 | Object | Yes | L | 
 POD4_MATERIAL_MAP_LIST | 11002 | Object | Yes | L | 
 SLOT_MAP | 1306 | Object | Yes | L | 

#### POD2_MAP_STARTED

Event for . ID: **9006**

There are no Automation Events Properties defined for this Automation Event

#### POD2_UNLOAD_COMPLETE

Event for . ID: **9010**

There are no Automation Events Properties defined for this Automation Event

#### POD3_ARRIVED

Event for . ID: **10000**

There are no Automation Events Properties defined for this Automation Event

#### POD3_DEPARTED

Event for . ID: **10001**

There are no Automation Events Properties defined for this Automation Event

#### POD3_MAPPED

Event for . ID: **10004**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 POD1_MATERIAL_MAP_LIST | 8002 | Object | Yes | L | 
 POD2_MATERIAL_MAP_LIST | 9002 | Object | Yes | L | 
 POD3_MATERIAL_MAP_LIST | 10002 | Object | Yes | L | 
 POD4_MATERIAL_MAP_LIST | 11002 | Object | Yes | L | 
 SLOT_MAP | 1306 | Object | Yes | L | 

#### POD3_MAP_STARTED

Event for . ID: **10006**

There are no Automation Events Properties defined for this Automation Event

#### POD3_UNLOAD_COMPLETE

Event for . ID: **10010**

There are no Automation Events Properties defined for this Automation Event

#### POD4_ARRIVED

Event for . ID: **11000**

There are no Automation Events Properties defined for this Automation Event

#### POD4_DEPARTED

Event for . ID: **11001**

There are no Automation Events Properties defined for this Automation Event

#### POD4_MAPPED

Event for . ID: **11004**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 POD1_MATERIAL_MAP_LIST | 8002 | Object | Yes | L | 
 POD2_MATERIAL_MAP_LIST | 9002 | Object | Yes | L | 
 POD3_MATERIAL_MAP_LIST | 10002 | Object | Yes | L | 
 POD4_MATERIAL_MAP_LIST | 11002 | Object | Yes | L | 
 SLOT_MAP | 1306 | Object | Yes | L | 

#### POD4_MAP_STARTED

Event for . ID: **11006**

There are no Automation Events Properties defined for this Automation Event

#### POD4_UNLOAD_COMPLETE

Event for . ID: **11010**

There are no Automation Events Properties defined for this Automation Event

### Commands

#### SET_CARRIER_ID

Command for . ID: **SETCARRIERID**

There are no Automation Command Parameters defined for this Automation Command

#### LOAD_POD

Command for . ID: **STARTMAPPING**

There are no Automation Command Parameters defined for this Automation Command

#### PP_SELECT

Command for . ID: **PP_SELECT**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 RecipeID | Yes | A | 
 LotID | Yes | A | 
 PodID | Yes | I1 | 
 UseSlot1 | Yes | BO | 
 UseSlot2 | Yes | BO | 
 UseSlot3 | Yes | BO | 
 UseSlot4 | Yes | BO | 
 UseSlot5 | Yes | BO | 
 UseSlot6 | Yes | BO | 
 UseSlot7 | Yes | BO | 
 UseSlot8 | Yes | BO | 
 UseSlot9 | Yes | BO | 
 UseSlot10 | Yes | BO | 
 UseSlot11 | Yes | BO | 
 UseSlot12 | Yes | BO | 
 UseSlot13 | Yes | BO | 

#### START

Command for . ID: **START**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 LotID | Yes | A | 

#### PAUSE

Command for . ID: **PAUSE**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 LotID | Yes | A | 

#### RESUME

Command for . ID: **RESUME**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 LotID | Yes | A | 

#### STOP

Command for . ID: **STOP**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 LotID | Yes | A | 

#### ABORT

Command for . ID: **ABORT**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 LotID | Yes | A | 

