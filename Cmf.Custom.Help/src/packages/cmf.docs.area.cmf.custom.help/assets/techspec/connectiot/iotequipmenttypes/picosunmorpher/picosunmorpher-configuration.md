Configuration
============
This section describe the setup for Picosun Morpher Equipment Type

Driver Definitions
=================
The following Automation Drivers are referenced on the Automation Controller as:
- **SecsGemEquipment** for the Automation Driver **PicosunMorpherDriver**;

- **RFIDReader** for the Automation Driver **HermosLFM4xReaderDriver**;

Drivers contain the information regarding all the items needed for the automation of the equipment.
## Automation Driver Definition - PicosunMorpherDriver
### Protocol
The protocol used by the Automation Driver referenced on the Automation Controller is named **SecsGem** and used the package **criticalmanufacturing/connect-iot-driver-secsgem**, using version **9.1.0-202209072** at the time of writing.

### Properties

The table below describes the workflow properties

|Name | ID | Type | Equipment Type | Description 
:------------ | -------: | :-------- | :---------- | :-------- 
 CLOCK | 1004 | String | A | Clock
 CONTROL_STATE | 1005 | Integer | U1 | CONTROL STATE
 PREVIOUS_PROCESS_STATE | 1010 | Integer | U1 | PREVIOUS PROCESS STATE
 PreviousProcessStateName | 1011 | String | A | Previous Process State Name
 PROCESS_STATE | 1012 | Integer | U1 | PROCESS STATE
 ProcessStateName | 1013 | String | A | Process State Name
 PPChangeName | 3003 | String | A | PP Change Name
 PPChangeStatus | 3004 | Integer | U1 | PP Change Status
 OepratorCommand | 3006 | String | A | OepratorCommand
 MaterialID | 3010 | String | A | Material Id
 ECID | 3011 | Integer | U4 | ECID
 ECChangeValue | 3012 | String | A | ECChange Value
 ECPreviousValue | 3013 | String | A | ECPrevious Value
 LotID | 3017 | String | A | Lot Id
 WaferLayoutPlan | 3019 | Object | L | Wafer Layout Plan
 SlotDataMap | 3020 | Object | L | Slot Data Map
 PodID | 3021 | String | A | Pod Id
 StationID | 3022 | Integer | U1 | Station Id (Port Id)
 SlotValueID | 3023 | Integer | U1 | Slot Value Id
 WaferID | 3024 | String | A | Wafer Id
 OcrID | 3025 | Integer | U1 | Ocr Id
 JobID | 4000 | String | A | Job ID
 ScanQuantity | 4002 | Integer | U1 | Scan Quantity
 ScanTimeMs | 4003 | Integer | U2 | Scan Time Ms
 ProcessJobID | 4004 | String | A | Process Job ID
 CurrentRecipeSequenceIndex | 4005 | Integer | U2 | CurrentRecipeSequenceIndex
 CurrentRecipeStepSource | 4006 | String | A | Current Recipe Step Source
 AligningResult | 4009 | Integer | U2 | Aligning Result
 CarrierRole | 4010 | Integer | U1 | Carrier Role
 WaferRole | 4011 | Integer | U1 | Wafer Role
 CyclesInCurrentRecipeStep | 4012 | Integer | U2 | Cycles In Current Recipe Step
 CyclesInCurrentRecipeLoop | 4014 | Integer | U2 | CyclesInCurrentRecipeLoop
 CurrentRecipeLoopCycleIndex | 4015 | Integer | U2 | CurrentRecipeLoopCycleIndex
 SourceStationID | 4100 | Integer | U1 | Source Station Id (Port Id)
 SourceSlot | 4101 | Integer | U1 | Source Slot
 DestinationStationID | 4102 | Integer | U1 | Destination Station Id (Port Id)
 DestinationSlot | 4103 | Integer | U1 | Destination Slot

### Events

#### LoadLockIsEnteringATM

Event for Load Lock Is Entering ATM. ID: **1000**
Linked Report Id: **120**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### LoadLockInATM

Event for Load Lock In ATM. ID: **1001**
Linked Report Id: **120**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### LoadLockDoorOpened

Event for Load Lock Door Opened. ID: **1002**
Linked Report Id: **120**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### LoadLockDoorClosed

Event for Load Lock Door Closed. ID: **1003**
Linked Report Id: **120**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### LoadLockIsEnteringVacuum

Event for Load Lock Is Entering Vacuum. ID: **1004**
Linked Report Id: **120**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### LoadLockInVacuum

Event for Load Lock In Vacuum. ID: **1005**
Linked Report Id: **120**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### StartWaferMapping

Event for Start Wafer Mapping. ID: **1006**
Linked Report Id: **120**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### WaferMappingDone

Event for Wafer Mapping Done. ID: **1007**
Linked Report Id: **122**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 SlotDataMap | 3020 | Object | Yes | L | Slot Data Map
 CLOCK | 1004 | String | Yes | A | Clock

#### ReadyToTransferToALD

Event for Ready To Transfer To ALD. ID: **1009**
Linked Report Id: **123**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 SourceStationID | 4100 | Integer | Yes | U1 | Source Station Id (Port Id)
 DestinationStationID | 4102 | Integer | Yes | U1 | Destination Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### MaterialMovedToALDPM

Event for Material Moved To ALDPM. ID: **1200**
Linked Report Id: **125**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 SourceStationID | 4100 | Integer | Yes | U1 | Source Station Id (Port Id)
 DestinationStationID | 4102 | Integer | Yes | U1 | Destination Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### StartingMoveWafersToCooler

Event for Starting Move Wafers To Cooler. ID: **1201**
Linked Report Id: **126**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 SourceStationID | 4100 | Integer | Yes | U1 | Source Station Id (Port Id)
 DestinationStationID | 4102 | Integer | Yes | U1 | Destination Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### CoolerIsVented

Event for Cooler Is Vented. ID: **1400**
Linked Report Id: **133**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### CoolerInVacuum

Event for Cooler In Vacuum. ID: **1403**
Linked Report Id: **133**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### ReadyForUnloading

Event for Ready For Unloading. ID: **1404**
Linked Report Id: **133**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### CoolingCompleted

Event for Cooling Completed. ID: **1405**
Linked Report Id: **133**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### ReadyForTransferFromCooler

Event for Ready For Transfer From Cooler. ID: **1406**
Linked Report Id: **135**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 SourceStationID | 4100 | Integer | Yes | U1 | Source Station Id (Port Id)
 DestinationStationID | 4102 | Integer | Yes | U1 | Destination Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### PodLocked

Event for Pod Locked. ID: **1500**
Linked Report Id: **124**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### PodOpened

Event for Pod Opened. ID: **1501**
Linked Report Id: **124**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### PodUnlocked

Event for Pod Unlocked. ID: **1503**
Linked Report Id: **124**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### SlotValveOpened

Event for Slot Valve Opened. ID: **1600**
Linked Report Id: **121**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 SlotValueID | 3023 | Integer | Yes | U1 | Slot Value Id
 CLOCK | 1004 | String | Yes | A | Clock

#### SlotValveClosed

Event for Slot Valve Closed. ID: **1601**
Linked Report Id: **121**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 SlotValueID | 3023 | Integer | Yes | U1 | Slot Value Id
 CLOCK | 1004 | String | Yes | A | Clock

#### EquipmentOFFLINE

Event for Equipment has gone offline. ID: **2000**
Linked Report Id: **100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CLOCK | 1004 | String | Yes | A | Clock
 CONTROL_STATE | 1005 | Integer | Yes | U1 | CONTROL STATE

#### ControlStateLOCAL

Event for Equipment has gone online local. ID: **2001**
Linked Report Id: **100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CLOCK | 1004 | String | Yes | A | Clock
 CONTROL_STATE | 1005 | Integer | Yes | U1 | CONTROL STATE

#### ControlStateREMOTE

Event for Equipment has gone online remote. ID: **2002**
Linked Report Id: **100**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CLOCK | 1004 | String | Yes | A | Clock
 CONTROL_STATE | 1005 | Integer | Yes | U1 | CONTROL STATE

#### OperatorCommandIssued

Event for Operator issued a command.. ID: **2006**
Linked Report Id: **104**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 OepratorCommand | 3006 | String | Yes | A | OepratorCommand

#### OperatorEquipmentConstantChanged

Event for Operator changed an equipment constant while in remote mode.. ID: **2007**
Linked Report Id: **105**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 ECID | 3011 | Integer | Yes | U4 | ECID
 ECChangeValue | 3012 | String | Yes | A | ECChange Value
 ECPreviousValue | 3013 | String | Yes | A | ECPrevious Value

#### MaterialReceived

Event for Material received via any port (load port or other) at the equipment.. ID: **2008**
Linked Report Id: **106**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)

#### MaterialRemoved

Event for Material removed via any port (load port or other) from the equipment.. ID: **2009**
Linked Report Id: **106**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)

#### WaferPickedFromCarrier

Event for Wafer Picked From Carrier. ID: **3000**
Linked Report Id: **111**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 MaterialID | 3010 | String | Yes | A | Material Id
 SourceStationID | 4100 | Integer | Yes | U1 | Source Station Id (Port Id)
 SourceSlot | 4101 | Integer | Yes | U1 | Source Slot
 WaferRole | 4011 | Integer | Yes | U1 | Wafer Role
 CLOCK | 1004 | String | Yes | A | Clock

#### WaferPickedFromAligner

Event for Wafer Picked From Aligner. ID: **3001**
Linked Report Id: **111**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 MaterialID | 3010 | String | Yes | A | Material Id
 SourceStationID | 4100 | Integer | Yes | U1 | Source Station Id (Port Id)
 SourceSlot | 4101 | Integer | Yes | U1 | Source Slot
 WaferRole | 4011 | Integer | Yes | U1 | Wafer Role
 CLOCK | 1004 | String | Yes | A | Clock

#### WaferPickedFromALDPM

Event for Wafer Picked From ALDPM. ID: **3003**
Linked Report Id: **111**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 MaterialID | 3010 | String | Yes | A | Material Id
 SourceStationID | 4100 | Integer | Yes | U1 | Source Station Id (Port Id)
 SourceSlot | 4101 | Integer | Yes | U1 | Source Slot
 WaferRole | 4011 | Integer | Yes | U1 | Wafer Role
 CLOCK | 1004 | String | Yes | A | Clock

#### WaferPickedFromCooler

Event for Wafer Picked From Cooler. ID: **3004**
Linked Report Id: **111**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 MaterialID | 3010 | String | Yes | A | Material Id
 SourceStationID | 4100 | Integer | Yes | U1 | Source Station Id (Port Id)
 SourceSlot | 4101 | Integer | Yes | U1 | Source Slot
 WaferRole | 4011 | Integer | Yes | U1 | Wafer Role
 CLOCK | 1004 | String | Yes | A | Clock

#### WaferPlacedInCarrier

Event for Wafer Placed In Carrier. ID: **3005**
Linked Report Id: **112**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 MaterialID | 3010 | String | Yes | A | Material Id
 DestinationStationID | 4102 | Integer | Yes | U1 | Destination Station Id (Port Id)
 DestinationSlot | 4103 | Integer | Yes | U1 | Destination Slot
 WaferRole | 4011 | Integer | Yes | U1 | Wafer Role
 CLOCK | 1004 | String | Yes | A | Clock

#### WaferPlacedInAligner

Event for Wafer Placed In Aligner. ID: **3006**
Linked Report Id: **112**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 MaterialID | 3010 | String | Yes | A | Material Id
 DestinationStationID | 4102 | Integer | Yes | U1 | Destination Station Id (Port Id)
 DestinationSlot | 4103 | Integer | Yes | U1 | Destination Slot
 WaferRole | 4011 | Integer | Yes | U1 | Wafer Role
 CLOCK | 1004 | String | Yes | A | Clock

#### WaferPlacedInALDPM

Event for Wafer Placed In ALDPM. ID: **3008**
Linked Report Id: **112**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 MaterialID | 3010 | String | Yes | A | Material Id
 DestinationStationID | 4102 | Integer | Yes | U1 | Destination Station Id (Port Id)
 DestinationSlot | 4103 | Integer | Yes | U1 | Destination Slot
 WaferRole | 4011 | Integer | Yes | U1 | Wafer Role
 CLOCK | 1004 | String | Yes | A | Clock

#### WaferPlacedInCooler

Event for Wafer Placed In Cooler. ID: **3009**
Linked Report Id: **112**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 MaterialID | 3010 | String | Yes | A | Material Id
 DestinationStationID | 4102 | Integer | Yes | U1 | Destination Station Id (Port Id)
 DestinationSlot | 4103 | Integer | Yes | U1 | Destination Slot
 WaferRole | 4011 | Integer | Yes | U1 | Wafer Role
 CLOCK | 1004 | String | Yes | A | Clock

#### WaferAligningStart

Event for Wafer Aligning Start. ID: **3010**
Linked Report Id: **113**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 MaterialID | 3010 | String | Yes | A | Material Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 WaferRole | 4011 | Integer | Yes | U1 | Wafer Role
 CLOCK | 1004 | String | Yes | A | Clock

#### WaferAligningEnd

Event for Wafer Aligning End. ID: **3011**

There are no Automation Events Properties defined for this Automation Event

#### WafersForCarrierPickedFromCooler

Event for Wafers For Carrier Picked From Cooler. ID: **3012**
Linked Report Id: **119**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 MaterialID | 3010 | String | Yes | A | Material Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 WaferRole | 4011 | Integer | Yes | U1 | Wafer Role
 CLOCK | 1004 | String | Yes | A | Clock

#### CjNoStateToCreated

Event for (PP-SELECTED). ID: **4000**
Linked Report Id: **108**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 WaferLayoutPlan | 3019 | Object | Yes | L | Wafer Layout Plan
 CLOCK | 1004 | String | Yes | A | Clock

#### CjQueuedToJobExecuting

Event for Cj Queued To Job Executing. ID: **4003**
Linked Report Id: **109**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 CLOCK | 1004 | String | Yes | A | Clock

#### CarrierPickedFromLoadPort

Event for Carrier Picked From Load Port. ID: **4100**
Linked Report Id: **127**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 SourceStationID | 4100 | Integer | Yes | U1 | Source Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### CarrierPickedFromLoadLock

Event for Carrier Picked From Load Lock. ID: **4101**
Linked Report Id: **127**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 SourceStationID | 4100 | Integer | Yes | U1 | Source Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### CarrierPlacedToLoadLock

Event for Carrier Placed To Load Lock. ID: **4102**
Linked Report Id: **128**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 DestinationStationID | 4102 | Integer | Yes | U1 | Destination Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### CarrierPlacedToPod

Event for Carrier Placed To Pod. ID: **4103**
Linked Report Id: **128**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 DestinationStationID | 4102 | Integer | Yes | U1 | Destination Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### PM1_PjNoStateToCreated

Event for PM1_Pj No State To Created. ID: **5000**
Linked Report Id: **129**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 ProcessJobID | 4004 | String | Yes | A | Process Job ID
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### PM1_PjCreatedToWaitingForStart

Event for PM1_Pj Created To Waiting For Start. ID: **5002**
Linked Report Id: **129**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 ProcessJobID | 4004 | String | Yes | A | Process Job ID
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### PM1_PjWaitingForStartToProcessing

Event for PM1_Pj Waiting For Start To Processing. ID: **5003**
Linked Report Id: **129**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 ProcessJobID | 4004 | String | Yes | A | Process Job ID
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### PM1_PjPostActiveToNoState

Event for PM1_Pj Post Active To No State. ID: **5015**
Linked Report Id: **129**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 ProcessJobID | 4004 | String | Yes | A | Process Job ID
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CLOCK | 1004 | String | Yes | A | Clock

#### IdleToJobActive

Event for IdleToJobActive. ID: **7004**
Linked Report Id: **101**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PREVIOUS_PROCESS_STATE | 1010 | Integer | Yes | U1 | PREVIOUS PROCESS STATE
 PreviousProcessStateName | 1011 | String | Yes | A | Previous Process State Name
 PROCESS_STATE | 1012 | Integer | Yes | U1 | PROCESS STATE
 ProcessStateName | 1013 | String | Yes | A | Process State Name
 CLOCK | 1004 | String | Yes | A | Clock

#### JobActiveToIdle

Event for JobActiveToIdle. ID: **7006**
Linked Report Id: **101**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PREVIOUS_PROCESS_STATE | 1010 | Integer | Yes | U1 | PREVIOUS PROCESS STATE
 PreviousProcessStateName | 1011 | String | Yes | A | Previous Process State Name
 PROCESS_STATE | 1012 | Integer | Yes | U1 | PROCESS STATE
 ProcessStateName | 1013 | String | Yes | A | Process State Name
 CLOCK | 1004 | String | Yes | A | Clock

#### CarrierRemoved

Event for Carrier Removed. ID: **9002**
Linked Report Id: **137**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 PodID | 3021 | String | Yes | A | Pod Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CarrierRole | 4010 | Integer | Yes | U1 | Carrier Role
 CLOCK | 1004 | String | Yes | A | Clock

#### CarrierReceived

Event for Carrier Received. ID: **9003**
Linked Report Id: **138**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CarrierRole | 4010 | Integer | Yes | U1 | Carrier Role
 CLOCK | 1004 | String | Yes | A | Clock

#### LoadingStart

Event for Loading Start. ID: **9004**
Linked Report Id: **139**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CarrierRole | 4010 | Integer | Yes | U1 | Carrier Role
 CLOCK | 1004 | String | Yes | A | Clock

#### UnloadingStart

Event for Unloading Start. ID: **9005**
Linked Report Id: **139**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CarrierRole | 4010 | Integer | Yes | U1 | Carrier Role
 CLOCK | 1004 | String | Yes | A | Clock

#### UnloadingCompleted

Event for Unloading Completed. ID: **9007**
Linked Report Id: **140**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 CLOCK | 1004 | String | Yes | A | Clock

#### PreparingUnloadingCarrier

Event for Preparing Unloading Carrier. ID: **9008**
Linked Report Id: **141**
##### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 JobID | 4000 | String | Yes | A | Job ID
 LotID | 3017 | String | Yes | A | Lot Id
 StationID | 3022 | Integer | Yes | U1 | Station Id (Port Id)
 CarrierRole | 4010 | Integer | Yes | U1 | Carrier Role
 CLOCK | 1004 | String | Yes | A | Clock

### Commands

#### CREATEJOB

Command for This command instructs the tool to create a job with the given job template program and number wafers. Intended to be used in multi-pod loading scenario.. ID: **CREATEJOB**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobTemplateName | Yes | A | 
 NumberOfProductionWafers | Yes | U1 | 

#### CREATEJOBWAFERLIST

Command for This command instructs the tool to create a job with the given job template program and wafer selection list. Intended to be used with basic loading scenario and PicoOS R1 software.. ID: **CREATEJOBWAFERLIST**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobTemplateName | Yes | A | 
 WaferSelectionList | Yes | L | 

#### STARTJOB

Command for Available to the host/operator when a job is created, and the processing state is IDLE.
The START command instructs the tool to initiate processing.. ID: **STARTJOB**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 

#### PAUSE

Command for Pause the selected job.. ID: **PAUSE**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 

#### STOP

Command for Stop the selected job.. ID: **STOP**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 

#### RESUME

Command for Resume the selected job.. ID: **RESUME**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 

#### ABORT

Command for Abort the selected job. ID: **ABORT**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 

#### DELETE

Command for Delete the selected job from the job queue.. ID: **DELETE**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 

#### PROCEEDWITHCARRIER

Command for This command tells to equipment that the carrier which is on the load port is the expected one and it can continue to process with this carrier.. ID: **PROCEEDWITHCARRIER**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 
 StationID | Yes | U1 | 
 LotID | Yes | A | 
 PodID | Yes | A | 

#### REJECTCARRIER

Command for This command tells to equipment that the carrier which is on the load port is not the expected one. An operator must operate on the equipment.. ID: **REJECTCARRIER**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 
 StationID | Yes | U1 | 
 LotID | Yes | A | 
 PodID | Yes | A | 

#### PROCEEDWITHSLOTMAP

Command for This command tells to equipment that the slotmap of the carrier is the expected one and it can continue to process with this slotmap.. ID: **PROCEEDWITHSLOTMAP**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 
 StationID | Yes | U1 | 
 LotID | Yes | A | 
 PodID | Yes | A | 

#### REJECTSLOTMAP

Command for This command tells to equipment that the slotmap of the carrier is not the expected one. Current job goes to error state.. ID: **REJECTSLOTMAP**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 
 StationID | Yes | U1 | 
 LotID | Yes | A | 
 PodID | Yes | A | 

#### CONFIGURESLOTMAP

Command for This command tells equipment that the slotmap needs to be configured. Host can set metadata (wafer role, material, Diameter, Shape, Thickness, Primary Flat Type, Secondary Flat Type) for slot map items, but not add or delete items in the slotmap.. ID: **CONFIGURESLOTMAP**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 
 StationID | Yes | U1 | 
 LotID | Yes | A | 
 PodID | Yes | A | 
 SlotDataMap | Yes | L | 

#### PROCEEDWITHWAFER

Command for This command tells to equipment that the current wafer is the expected one and it can continue to process with this wafer.. ID: **PROCEEDWITHWAFER**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 
 StationID | Yes | U1 | 
 MaterialID | Yes | A | 
 WaferID | Yes | A | 

#### REJECTWAFER

Command for This command tells to equipement that the current wafer is not the expected one. There a multiple options with this command depending on the JobTemplate.. ID: **REJECTWAFER**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 
 StationID | Yes | U1 | 
 MaterialID | Yes | A | 
 WaferID | Yes | A | 

#### STARTPROCESSJOB

Command for This command is used to start process job.. ID: **STARTPROCESSJOB**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 
 ProcessJobID | Yes | A | 
 StationID | Yes | U1 | 

#### ABORTPROCESSJOB

Command for This command is used to abort process job.. ID: **ABORTPROCESSJOB**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 JobID | Yes | A | 
 ProcessJobID | Yes | A | 
 StationID | Yes | U1 | 

#### LOADCARRIER

Command for Requests equipment to open pod in load port, move carrier cassette to load lock, close load lock door, evacuate load lock and finally map wafers.
Use load port station IDs with this command.
LotID parameter is optional. Empty or non-existent LotID replaces possible previously set LotID for the station.
To be used with R1.1 control job only.. ID: **LOADCARRIER**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 StationID | Yes | U1 | 

#### UNLOADCARRIER

Command for Requests equipment to ventilate load lock, open pod in load port, open load lock door, move carrier cassette to pod in load lock, close load lock door and pod.
LotID parameter is optional. It rewrites possibly earlier LotID set by LOADCARRIER remote command for the station.
Use load lock station IDs with this command.
To be used with R1.1 control job only.. ID: **UNLOADCARRIER**

##### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 StationID | Yes | U1 | 

## Automation Driver Definition - HermosLFM4xReaderDriver
Information on this driver is contained on an additional driver related document.

