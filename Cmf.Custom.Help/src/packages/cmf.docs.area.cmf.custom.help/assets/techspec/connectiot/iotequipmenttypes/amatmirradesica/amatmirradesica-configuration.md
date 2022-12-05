Configuration
============
This section describe the setup for Mirra Desica Equipment Type

Driver Definitions
=================
The following Automation Drivers are referenced on the Automation Controller as:
- **SecsGemEquipment** for the Automation Driver **AmatMirraDesicaDriver**;

- **RFIDReader** for the Automation Driver **HermosLFM4xReaderDriver**;

Drivers contain the information regarding all the items needed for the automation of the equipment.
## Automation Driver Definition - AmatMirraDesicaDriver
### Protocol
The protocol used by the Automation Driver referenced on the Automation Controller is named **SecsGem** and used the package **criticalmanufacturing/connect-iot-driver-secsgem**, using version **9.1.0-202209072** at the time of writing.

### Properties

The table below describes the workflow properties

|Name | ID | Type | Equipment Type | Description 
:------------ | -------: | :-------- | :---------- | :-------- 
 PPID | 33 | String | A | PPEXECNAME
 JOB_ID_1 | 301 | String | A | JOB_ID_1
 JOB_ID_2 | 302 | String | A | JOB_ID_2
 JOB_ID_3 | 303 | String | A | JOB_ID_3
 WAFERSTATUSCASS1 | 676 | Integer | I2 | WAFERSTATUSCASS1
 WAFERSTATUSCASS2 | 677 | Integer | I2 | WAFERSTATUSCASS2
 WAFERSTATUSCASS3 | 678 | Integer | I2 | WAFERSTATUSCASS3
 WAFERSTATUSCASS4 | 679 | Integer | I2 | WAFERSTATUSCASS4
 ControlState | 28 | Integer | U1 | The current Control State

### Events

#### PodArrivedPort1

Event for Port1ReadyToSetup. ID: **4108**

There are no Automation Events Properties defined for this Automation Event

#### PodArrivedPort2

Event for Port2ReadyToSetup. ID: **4208**

There are no Automation Events Properties defined for this Automation Event

#### PodArrivedPort3

Event for Port3ReadyToSetup. ID: **4308**

There are no Automation Events Properties defined for this Automation Event

#### PodArrivedPort4

Event for Port4ReadyToSetup. ID: **4408**

There are no Automation Events Properties defined for this Automation Event

#### PodRemovedPort1

Event for Port1ReadyToLoad. ID: **4104**

There are no Automation Events Properties defined for this Automation Event

#### PodRemovedPort2

Event for Port2ReadyToLoad. ID: **4204**

There are no Automation Events Properties defined for this Automation Event

#### PodRemovedPort3

Event for Port3ReadyToLoad. ID: **4304**

There are no Automation Events Properties defined for this Automation Event

#### PodRemovedPort4

Event for Port4ReadyToLoad. ID: **4404**

There are no Automation Events Properties defined for this Automation Event

#### ReadyToRunPort1

Event for Port1ReadyToRun. ID: **4106**

There are no Automation Events Properties defined for this Automation Event

#### ReadyToRunPort2

Event for Port2ReadyToRun. ID: **4206**

There are no Automation Events Properties defined for this Automation Event

#### ReadyToRunPort3

Event for Port3ReadyToRun. ID: **4306**

There are no Automation Events Properties defined for this Automation Event

#### ReadyToRunPort4

Event for Port4ReadyToRun. ID: **4406**

There are no Automation Events Properties defined for this Automation Event

#### GroupSetPort1

Event for Port1GroupSet. ID: **4111**

There are no Automation Events Properties defined for this Automation Event

#### GroupSetPort2

Event for Port2GroupSet. ID: **4211**

There are no Automation Events Properties defined for this Automation Event

#### GroupSetPort3

Event for Port3GroupSet. ID: **4311**

There are no Automation Events Properties defined for this Automation Event

#### GroupSetPort4

Event for Port4GroupSet. ID: **4411**

There are no Automation Events Properties defined for this Automation Event

#### MirraIdle

Event for MIRRA_Idle. ID: **411**

There are no Automation Events Properties defined for this Automation Event

#### ProcessingStartedPort1

Event for Cass1Start (Cassette #1 starts). ID: **1100**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3

#### ProcessingStartedPort2

Event for Cass2Start (Cassette #2 starts). ID: **1200**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3

#### ProcessingStartedPort3

Event for Cass3Start (Cassette #3 starts). ID: **1300**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3

#### ProcessingStartedPort4

Event for Cass4Start (Cassette #4 starts). ID: **1400**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3

#### WaferStartedPort1

Event for Port1Busy (Port 1 is processing). ID: **4107**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3

#### WaferStartedPort2

Event for Port2Busy (Port 2 is processing). ID: **4207**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3

#### WaferStartedPort3

Event for Port3Busy (Port 3 is processing). ID: **4307**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3

#### WaferStartedPort4

Event for Port4Busy (Port 4 is processing). ID: **4407**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3

#### WaferCompletedPort1

Event for WaferToC1 (Wafer is put into Cassette #1). ID: **1102**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3

#### WaferCompletedPort2

Event for WaferToC2 (Wafer is put into Cassette #2). ID: **1202**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3

#### WaferCompletedPort3

Event for WaferToC3 (Wafer is put into Cassette #3). ID: **1302**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3

#### WaferCompletedPort4

Event for WaferToC4 (Wafer is put into Cassette #4). ID: **1402**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3

#### ProcessingCompletedPort1

Event for Cass1Finish (Cassette #1 finished). ID: **1101**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3
 WAFERSTATUSCASS1 | 676 | Integer | Yes | I2 | WAFERSTATUSCASS1

#### ProcessingCompletedPort2

Event for Cass2Finish (Cassette #2 finished). ID: **1201**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3
 WAFERSTATUSCASS2 | 677 | Integer | Yes | I2 | WAFERSTATUSCASS2

#### ProcessingCompletedPort3

Event for Cass3Finish (Cassette #3 finished). ID: **1301**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3
 WAFERSTATUSCASS3 | 678 | Integer | Yes | I2 | WAFERSTATUSCASS3

#### ProcessingCompletedPort4

Event for Cass4Finish (Cassette #4 finished). ID: **1401**
Linked Report Id: **3**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPID | 33 | String | Yes | A | PPEXECNAME
 JOB_ID_1 | 301 | String | Yes | A | JOB_ID_1
 JOB_ID_2 | 302 | String | Yes | A | JOB_ID_2
 JOB_ID_3 | 303 | String | Yes | A | JOB_ID_3
 WAFERSTATUSCASS4 | 679 | Integer | Yes | I2 | WAFERSTATUSCASS4

#### SMIFPort1Raised

Event for SMIFPort1Raised. ID: **4125**

There are no Automation Events Properties defined for this Automation Event

#### SMIFPort2Raised

Event for SMIFPort2Raised. ID: **4225**

There are no Automation Events Properties defined for this Automation Event

#### SMIFPort3Raised

Event for SMIFPort3Raised. ID: **4325**

There are no Automation Events Properties defined for this Automation Event

#### SMIFPort4Raised

Event for SMIFPort4Raised. ID: **4425**

There are no Automation Events Properties defined for this Automation Event

#### ProdLoadPort1

Event for Port1MapDone. ID: **4110**

There are no Automation Events Properties defined for this Automation Event

#### ProdLoadPort2

Event for Port2MapDone. ID: **4210**

There are no Automation Events Properties defined for this Automation Event

#### ProdLoadPort3

Event for Port3MapDone. ID: **4310**

There are no Automation Events Properties defined for this Automation Event

#### ProdLoadPort4

Event for Port4MapDone. ID: **4410**

There are no Automation Events Properties defined for this Automation Event

#### ProdUnloadPort1

Event for Port1ReadyToUnload. ID: **4105**

There are no Automation Events Properties defined for this Automation Event

#### ProdUnloadPort2

Event for Port2ReadyToUnload. ID: **4205**

There are no Automation Events Properties defined for this Automation Event

#### ProdUnloadPort3

Event for Port3ReadyToUnload. ID: **4305**

There are no Automation Events Properties defined for this Automation Event

#### ProdUnloadPort4

Event for Port4ReadyToUnload. ID: **4405**

There are no Automation Events Properties defined for this Automation Event

#### PodLockedPort1

Event for SMIFPod1Clamped. ID: **4121**

There are no Automation Events Properties defined for this Automation Event

#### PodLockedPort2

Event for SMIFPod2Clamped. ID: **4221**

There are no Automation Events Properties defined for this Automation Event

#### PodLockedPort3

Event for SMIFPod3Clamped. ID: **4321**

There are no Automation Events Properties defined for this Automation Event

#### PodLockedPort4

Event for SMIFPod4Clamped. ID: **4421**

There are no Automation Events Properties defined for this Automation Event

#### PodUnlockedPort1

Event for SMIFPod1Unclamped. ID: **4122**

There are no Automation Events Properties defined for this Automation Event

#### PodUnlockedPort2

Event for SMIFPod2Unclamped. ID: **4222**

There are no Automation Events Properties defined for this Automation Event

#### PodUnlockedPort3

Event for SMIFPod3Unclamped. ID: **4322**

There are no Automation Events Properties defined for this Automation Event

#### PodUnlockedPort4

Event for SMIFPod4Unclamped. ID: **4422**

There are no Automation Events Properties defined for this Automation Event

#### ProcessingStarted

Event for ProcessingStarted. ID: **1000**

There are no Automation Events Properties defined for this Automation Event

#### EquipmentOFFLINE

Event for EquipmentOFFLINE. ID: **3**

There are no Automation Events Properties defined for this Automation Event

#### ControlStateLOCAL

Event for ControlStateLOCAL. ID: **4**

There are no Automation Events Properties defined for this Automation Event

#### ControlStateREMOTE

Event for ControlStateREMOTE. ID: **5**

There are no Automation Events Properties defined for this Automation Event

#### PodCarrierLoadedPort1

Event for PodCarrierLoadedPort1. ID: **4123**

There are no Automation Events Properties defined for this Automation Event

#### PodCarrierLoadedPort2

Event for PodCarrierLoadedPort2. ID: **4223**

There are no Automation Events Properties defined for this Automation Event

#### PodCarrierLoadedPort3

Event for PodCarrierLoadedPort3. ID: **4323**

There are no Automation Events Properties defined for this Automation Event

#### PodCarrierLoadedPort4

Event for PodCarrierLoadedPort4. ID: **4423**

There are no Automation Events Properties defined for this Automation Event

### Commands

#### PP_SELECT

Command for PP Select. ID: **PP_SELECT**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PPID | Yes | A | 
 PORT_ID | Yes | U4 | 

#### SETGROUP

Command for SETGROUP. ID: **SETGROUP**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 GROUPFLAGS | Yes | U4 | 
 PORT_ID | Yes | U4 | 

#### LOCK_POD

Command for LOCK_POD. ID: **LOCK_POD**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | U4 | 

#### LOAD_POD

Command for LOAD_POD. ID: **LOAD_POD**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | U4 | 

#### SETMID

Command for SETMID. ID: **SETMID**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 LOT_ID | Yes | A | 
 PORT_ID | Yes | U4 | 

#### MAPCASS

Command for MAPCASS. ID: **MAPCASS**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | U4 | 

#### START

Command for START. ID: **START**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | U4 | 

#### UNLOAD_POD

Command for UNLOAD_POD. ID: **UNLOAD_POD**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | U4 | 

#### UNLOCK_POD

Command for UNLOCK_POD. ID: **UNLOCK_POD**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | U4 | 

#### STOP

Command for STOP. ID: **STOP**

There are no Automation Command Parameters defined for this Automation Command

## Automation Driver Definition - HermosLFM4xReaderDriver
Information on this driver is contained on an additional driver related document.

