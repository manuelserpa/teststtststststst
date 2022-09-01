Configuration
============
This section describe the setup for Documentation Generator Equipment Integration Test Equipment Type

Driver Definitions
=================
The following Automation Drivers are referenced on the Automation Controller as:
- **SecsGemEquipment** for the Automation Driver **Monarch25VHFDriver**;

- **RFIDReader** for the Automation Driver **HermosLFM4xReaderDriver**;

Drivers contain the information regarding all the items needed for the automation of the equipment.
## Automation Driver Definition - Monarch25VHFDriver
### Protocol
The protocol used by the Automation Driver referenced on the Automation Controller is named **SecsGem** and used the package **criticalmanufacturing/connect-iot-driver-secsgem**, using version **8.3.3-202201212** at the time of writing.

### Properties

The table below describes the workflow properties

|Name | ID | Type | Equipment Type | Description 
:------------ | -------: | :-------- | :---------- | :-------- 
 ECIDChangeByOperator | 7 | Integer | U4 | ECID changed by the machine operator
 ECChangeName | 2052 | String | A | The name of the EC that changed
 ECChangeValue | 2053 | String | A | The value of EC that changed
 WAFER_ID | 5111 | String | A | Wafer ID
 StationID | 5113 | Integer | U1 | Station Number
 WaferNo | 5118 | String | A | Wafer Number
 PPChangeName | 3 | String | A | Name of the process program (recipe) changed by the local operator. This may be linked to CE PPChange
 PPChangeStatus | 4 | Integer | U1 | Type of change made to a process program (recipe) by the local operator. Possible values include 1 (created), 2 (edited), and 3 (deleted). This may be linked to CE PPChange
 OperatorCommand | 16 | String | A | Command issued by the machine operator.
 CARRIER_ID | 5110 | String | A | Cassette ID
 LOT_ID | 5114 | String | A | Lot ID
 PORT_ID | 6102 | Integer | U1 | Cassette Port ID
 SLOT_MAP | 5112 | Object | L | Cassette Slot Map
 WaferRecipeDV | 5101 | String | A | Wafer recipe for last wafer process. This may be linked to CE WaferStatisticalDataAvailable
 ModuleRecipeDV | 5102 | String | A | Module recipe for last wafer process. This may be linked to CE WaferStatisticalDataAvailable
 WaferStatisticalDataDV | 5100 | Object | L | Min, Max, Avg values for last wafer process. This may be linked to CE WaferStatisticalDataAvailable
 StepName | 5117 | String | A | Recipe Step Name
 RECIPE_ID | 5115 | String | A | Recipe Name
 StepID | 5116 | Integer | U1 | Recipe Step Number
 CONTROL_STATE | 28 | Integer | U1 | holds the current Control State

### Events

### AlignerWaferStatusChange

Event for Aligner wafer status has changed. ID: **510**

There are no Automation Events Properties defined for this Automation Event

### CassetteComplete

Event for Processing has completed successfully for a cassette. ID: **852**
Linked Report Id: **851**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CARRIER_ID | 5110 | String | Yes | A | Cassette ID
 LOT_ID | 5114 | String | Yes | A | Lot ID
 PORT_ID | 6102 | Integer | Yes | U1 | Cassette Port ID

### CassetteStarted

Event for Processing has started for a cassette. ID: **851**
Linked Report Id: **851**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CARRIER_ID | 5110 | String | Yes | A | Cassette ID
 LOT_ID | 5114 | String | Yes | A | Lot ID
 PORT_ID | 6102 | Integer | Yes | U1 | Cassette Port ID

### CentraliserAWaferStatusChange

Event for Mosaic300 Centraliser A wafer status has changed. ID: **511**

There are no Automation Events Properties defined for this Automation Event

### ControlStateRemote

Event for Control state has changed to Remote (from Local or Off-line). ID: **10**

There are no Automation Events Properties defined for this Automation Event

### DoorClosed1

Event for Door for VCE A is Closed. ID: **711**

There are no Automation Events Properties defined for this Automation Event

### DoorClosed2

Event for Door for VCE B is Closed. ID: **712**

There are no Automation Events Properties defined for this Automation Event

### DoorOpen1

Event for Door for VCE A is Open. ID: **701**

There are no Automation Events Properties defined for this Automation Event

### DoorOpen2

Event for Door for VCE B is Open. ID: **702**

There are no Automation Events Properties defined for this Automation Event

### ECChange

Event for An equipment constant value was changed locally by the operator. ID: **24**
Linked Report Id: **24**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 ECIDChangeByOperator | 7 | Integer | Yes | U4 | ECID changed by the machine operator
 ECChangeName | 2052 | String | Yes | A | The name of the EC that changed
 ECChangeValue | 2053 | String | Yes | A | The value of EC that changed

### EtchStepSummaryData

Event for Min Max and Avg and StdDev data for last wafer process (one event per step at end of process). ID: **811**

There are no Automation Events Properties defined for this Automation Event

### IdleSelectingStateChange1

Event for VCE A Process State changes from Idle to Selecting. ID: **151**

There are no Automation Events Properties defined for this Automation Event

### Lamp1StatusChanged

Event for Lamp 1 status has changed. ID: **861**

There are no Automation Events Properties defined for this Automation Event

### Lamp2StatusChanged

Event for Lamp 2 status has changed. ID: **862**

There are no Automation Events Properties defined for this Automation Event

### MaterialReceived

Event for Material arrived from a port on the equipment.. ID: **3**
Linked Report Id: **3**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PORT_ID | 6102 | Integer | Yes | U1 | Cassette Port ID

### MaterialRemoved

Event for Material was sent from a port on the equipment.. ID: **4**
Linked Report Id: **3**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PORT_ID | 6102 | Integer | Yes | U1 | Cassette Port ID

### MBCComplete1

Event for A lot has completed processing from VCE A. ID: **336**

There are no Automation Events Properties defined for this Automation Event

### MBComplete1

Event for A wafer has completed processing from VCE A. ID: **348**

There are no Automation Events Properties defined for this Automation Event

### MBCStart1

Event for A Lot has started processing from VCE A. ID: **330**

There are no Automation Events Properties defined for this Automation Event

### MBStart1

Event for A wafer has started processing from VCE A. ID: **342**

There are no Automation Events Properties defined for this Automation Event

### OperatorCommandIssued

Event for Machine operator issued a control command.. ID: **6**
Linked Report Id: **6**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PORT_ID | 6102 | Integer | Yes | U1 | Cassette Port ID
 OperatorCommand | 16 | String | Yes | A | Command issued by the machine operator.

### PM1RecipeStart

Event for PM1 Recipe started. ID: **422**
Linked Report Id: **422**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 WAFER_ID | 5111 | String | Yes | A | Wafer ID
 StationID | 5113 | Integer | Yes | U1 | Station Number
 LOT_ID | 5114 | String | Yes | A | Lot ID
 RECIPE_ID | 5115 | String | Yes | A | Recipe Name
 StepID | 5116 | Integer | Yes | U1 | Recipe Step Number
 StepName | 5117 | String | Yes | A | Recipe Step Name
 WaferNo | 5118 | String | Yes | A | Wafer Number

### PM3RecipeEnd

Event for PM3 Recipe ended. ID: **444**

There are no Automation Events Properties defined for this Automation Event

### PM3RecipeStart

Event for PM3 Recipe started. ID: **424**

There are no Automation Events Properties defined for this Automation Event

### PM3RecipeStepEnd

Event for PM3 Recipe step ended. ID: **484**

There are no Automation Events Properties defined for this Automation Event

### PM3RecipeStepStart

Event for PM3 Recipe step started. ID: **464**

There are no Automation Events Properties defined for this Automation Event

### PM3StateChange

Event for PM3 state has changed. ID: **404**

There are no Automation Events Properties defined for this Automation Event

### PM3WaferIn

Event for A wafer has arrived in PM3. ID: **882**

There are no Automation Events Properties defined for this Automation Event

### PM3WaferOut

Event for A wafer has been removed from PM3. ID: **888**

There are no Automation Events Properties defined for this Automation Event

### PM3WaferStatusChange

Event for PM3 wafer status has changed. ID: **505**

There are no Automation Events Properties defined for this Automation Event

### PM4RecipeEnd

Event for PM4 Recipe ended. ID: **445**

There are no Automation Events Properties defined for this Automation Event

### PM4RecipeStart

Event for PM4 Recipe started. ID: **425**

There are no Automation Events Properties defined for this Automation Event

### PM4RecipeStepEnd

Event for PM4 Recipe step ended. ID: **485**

There are no Automation Events Properties defined for this Automation Event

### PM4RecipeStepStart

Event for PM4 Recipe step started. ID: **465**

There are no Automation Events Properties defined for this Automation Event

### PM4StateChange

Event for PM4 state has changed. ID: **405**

There are no Automation Events Properties defined for this Automation Event

### PM4WaferIn

Event for A wafer has arrived in PM4. ID: **883**

There are no Automation Events Properties defined for this Automation Event

### PM4WaferOut

Event for A wafer has been removed from PM4. ID: **889**

There are no Automation Events Properties defined for this Automation Event

### PM4WaferStatusChange

Event for PM4 wafer status has changed. ID: **506**

There are no Automation Events Properties defined for this Automation Event

### PMWaferIn

Event for A wafer has arrived in a process module. ID: **853**
Linked Report Id: **853**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 WAFER_ID | 5111 | String | Yes | A | Wafer ID
 StationID | 5113 | Integer | Yes | U1 | Station Number
 WaferNo | 5118 | String | Yes | A | Wafer Number

### PMWaferOut

Event for A wafer has been removed from a process module. ID: **854**

There are no Automation Events Properties defined for this Automation Event

### PPChange

Event for A process program (recipe) has been created changed or deleted. ID: **7**
Linked Report Id: **7**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PPChangeName | 3 | String | Yes | A | Name of the process program (recipe) changed by the local operator. This may be linked to CE PPChange
 PPChangeStatus | 4 | Integer | Yes | U1 | Type of change made to a process program (recipe) by the local operator. Possible values include 1 (created), 2 (edited), and 3 (deleted). This may be linked to CE PPChange

### ProcessingFinished

Event for Processing has finished in a process module. ID: **856**

There are no Automation Events Properties defined for this Automation Event

### ProcessingStarted

Event for Processing has started in a process module. ID: **855**

There are no Automation Events Properties defined for this Automation Event

### ProcessingStateChange1

Event for VCE A has changed Process State. ID: **100**

There are no Automation Events Properties defined for this Automation Event

### ReadyForProcessA

Event for The Lot in VCE A is ready to start processing. ID: **520**

There are no Automation Events Properties defined for this Automation Event

### ReadyForProcessB

Event for The Lot in VCE B is ready to start processing. ID: **521**

There are no Automation Events Properties defined for this Automation Event

### RecipeStepEnd

Event for A new recipe step has finished in a process module. ID: **858**

There are no Automation Events Properties defined for this Automation Event

### RecipeStepStart

Event for A new recipe step has started in a process module. ID: **857**

There are no Automation Events Properties defined for this Automation Event

### RunningSelectedStateChange1

Event for VCE A Process State changes from Running to Selected. ID: **158**

There are no Automation Events Properties defined for this Automation Event

### RunningStoppingStateChange1

Event for VCE A Process State changes from Running to Stopping. ID: **159**

There are no Automation Events Properties defined for this Automation Event

### SelectedIdleStateChange1

Event for VCE A Process State changes from Selected to Idle. ID: **169**

There are no Automation Events Properties defined for this Automation Event

### SelectedStartingStateChange1

Event for VCE A Process State changes from Selected to Starting. ID: **155**

There are no Automation Events Properties defined for this Automation Event

### SelectingSelectedStateChange1

Event for VCE A Process State changes from Selecting to Selected. ID: **153**

There are no Automation Events Properties defined for this Automation Event

### SlotMapRead

Event for Slot Map Read. ID: **850**
Linked Report Id: **850**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SLOT_MAP | 5112 | Object | Yes | L | Cassette Slot Map
 PORT_ID | 6102 | Integer | Yes | U1 | Cassette Port ID

### SMIFPodAbsent1

Event for SMIF Pod for VCE A is absent. ID: **731**

There are no Automation Events Properties defined for this Automation Event

### SMIFPodAbsent2

Event for SMIF Pod for VCE B is absent. ID: **732**

There are no Automation Events Properties defined for this Automation Event

### SMIFPodClamped1

Event for SMIF Pod for VCE A is clamped. ID: **741**

There are no Automation Events Properties defined for this Automation Event

### SMIFPodClamped2

Event for SMIF Pod for VCE B is clamped. ID: **742**

There are no Automation Events Properties defined for this Automation Event

### SMIFPodPresent1

Event for SMIF Pod for VCE A is present. ID: **721**

There are no Automation Events Properties defined for this Automation Event

### SMIFPodPresent2

Event for SMIF Pod for VCE B is present. ID: **722**

There are no Automation Events Properties defined for this Automation Event

### SMIFPodUnClamped1

Event for SMIF Pod for VCE A is unclamped. ID: **751**

There are no Automation Events Properties defined for this Automation Event

### SMIFPodUnClamped2

Event for SMIF Pod for VCE B is unclamped. ID: **752**

There are no Automation Events Properties defined for this Automation Event

### StartingRunningStateChange1

Event for VCE A Process State changes from Starting to Running. ID: **157**

There are no Automation Events Properties defined for this Automation Event

### StoppedIdleStateChange1

Event for VCE A Process State changes from Stopped to Idle. ID: **174**

There are no Automation Events Properties defined for this Automation Event

### StoppingStoppedStateChange1

Event for VCE A Process State changes from Stopping to Stopped. ID: **163**

There are no Automation Events Properties defined for this Automation Event

### ToolAGVOff

Event for E23 Access mode changed to Manual. ID: **843**

There are no Automation Events Properties defined for this Automation Event

### TransportStateChange

Event for Transport state has changed. ID: **400**

There are no Automation Events Properties defined for this Automation Event

### VacAlignDataAvailable

Event for Alignment data available from optical vac aligner. ID: **821**

There are no Automation Events Properties defined for this Automation Event

### VacArmAWaferStatusChange

Event for Vacuum Arm A wafer status has changed. ID: **500**

There are no Automation Events Properties defined for this Automation Event

### VacArmBWaferStatusChange

Event for Vacuum Arm B wafer status has changed. ID: **501**

There are no Automation Events Properties defined for this Automation Event

### VCEALoadComplete

Event for Load Completed on VCE A. ID: **791**

There are no Automation Events Properties defined for this Automation Event

### VCEAMaterialAbsent

Event for Material has left at VCE A. ID: **781**

There are no Automation Events Properties defined for this Automation Event

### VCEAMaterialPresent

Event for Material has arrived VCE A. ID: **771**

There are no Automation Events Properties defined for this Automation Event

### VCEAStateChange

Event for VCE A state has changed. ID: **401**

There are no Automation Events Properties defined for this Automation Event

### VCEAUnloadComplete

Event for Unload Completed on VCE A. ID: **801**

There are no Automation Events Properties defined for this Automation Event

### VCEAWaferStatusChange

Event for VCE A wafer status has changed. ID: **502**

There are no Automation Events Properties defined for this Automation Event

### VCEBLoadComplete

Event for Load Completed on VCE B. ID: **792**

There are no Automation Events Properties defined for this Automation Event

### VCEBMaterialAbsent

Event for Material has left at VCE B. ID: **782**

There are no Automation Events Properties defined for this Automation Event

### VCEBMaterialPresent

Event for Material has arrived VCE B. ID: **772**

There are no Automation Events Properties defined for this Automation Event

### VCEBStateChange

Event for VCE B state has changed. ID: **408**

There are no Automation Events Properties defined for this Automation Event

### VCEBUnloadComplete

Event for Unload Completed on VCE B. ID: **802**

There are no Automation Events Properties defined for this Automation Event

### VCEBWaferStatusChange

Event for VCE B wafer status has changed. ID: **509**

There are no Automation Events Properties defined for this Automation Event

### WaferComplete

Event for A wafer has completed processing. ID: **860**
Linked Report Id: **859**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 WAFER_ID | 5111 | String | Yes | A | Wafer ID
 LOT_ID | 5114 | String | Yes | A | Lot ID
 WaferNo | 5118 | String | Yes | A | Wafer Number

### WaferStarted

Event for A wafer has started processing. ID: **859**
Linked Report Id: **859**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 WAFER_ID | 5111 | String | Yes | A | Wafer ID
 LOT_ID | 5114 | String | Yes | A | Lot ID
 WaferNo | 5118 | String | Yes | A | Wafer Number

### WaferStatisticalDataAvailable

Event for Min Max and Avg data for last wafer process step. ID: **810**
Linked Report Id: **810**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PORT_ID | 6102 | Integer | Yes | U1 | Cassette Port ID
 WaferRecipeDV | 5101 | String | Yes | A | Wafer recipe for last wafer process. This may be linked to CE WaferStatisticalDataAvailable
 ModuleRecipeDV | 5102 | String | Yes | A | Module recipe for last wafer process. This may be linked to CE WaferStatisticalDataAvailable
 WaferStatisticalDataDV | 5100 | Object | Yes | L | Min, Max, Avg values for last wafer process. This may be linked to CE WaferStatisticalDataAvailable
 StepName | 5117 | String | Yes | A | Recipe Step Name
 LOT_ID | 5114 | String | Yes | A | Lot ID

### ControlStateLocal

Event for Control state has changed to Local (from Remote or Off-line). ID: **9**

There are no Automation Events Properties defined for this Automation Event

### EquipmentOffline

Event for Control state is about to change to Off-line. ID: **8**

There are no Automation Events Properties defined for this Automation Event

### Commands

### GO_REMOTE

Command for Set Equipment to Online Remote. ID: **REMOTE**

There are no Automation Command Parameters defined for this Automation Command

### LOCK_POD

Command for Lock Pod. ID: **CLAMP**

There are no Automation Command Parameters defined for this Automation Command

### LOAD_POD

Command for Load Pod. ID: **LOAD**

There are no Automation Command Parameters defined for this Automation Command

### UNLOAD_POD

Command for Unload Pod. ID: **UNLOAD**

There are no Automation Command Parameters defined for this Automation Command

### PP-SELECT

Command for PP Select. ID: **PP_SELECT**

There are no Automation Command Parameters defined for this Automation Command

### START

Command for Start Command. ID: **START**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | A | 
 LOT_ID | Yes | A | 

### UNLOCK_POD

Command for Unlock Command. ID: **UNCLAMP**

There are no Automation Command Parameters defined for this Automation Command

### STOP

Command for Stop Command. ID: **STOP**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | A | 

### ABORT

Command for Abort Command. ID: **ABORT**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | A | 

### RESUME

Command for Resume Command. ID: **RESUME**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | A | 

### CANCEL

Command for Cancel Command. ID: **CANCEL**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORT_ID | Yes | A | 

## Automation Driver Definition - HermosLFM4xReaderDriver
Information on this driver is contained on an additional driver related document.

