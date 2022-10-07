Configuration
============
This section describe the setup for MechatronicMWS200 Equipment Type

Protocol
========
The protocol used by the Automation Driver referenced on the Automation Controller is named **SecsGem** and used the out-of-the-box package **criticalmanufacturing/connect-iot-driver-secsgem**, using version **9.1.0-202209072** at the time of writing.

Driver Definition
=================
The Automation Driver referenced on the Automation Controller as **SecsGemEquipment** is the Automation Driver **MechatronicMWS200Driver**, this driver contains the information regarding all the items needed for the automation of the equipment.

Properties
==========

The table below describes the workflow properties

|Name | ID | Type | Equipment Type | Description 
:------------ | -------: | :-------- | :---------- | :-------- 
 AlarmID | 1001 | Integer | U4 | * The current alarm identifier ALID
 AlarmText | 1002 | String | A | * The latest alarm ALTX value
 AlarmCode | 1003 | Binary | BI | * The latest alarm ALCD value
 ECIDChangeName | 1004 | Integer | U4 | * The equipment constant, ECID
 EventName | 1005 | String | A | * The name of the CEID event type
 LimitVariable | 1006 | Integer | U4 | * The VID for the variable whose value changed
 EventLimit | 1007 | Object | L | * The LIMITID(s) of the limit reached by LimitVariable(s)
 TransitionType | 1008 | Binary | BI | * The direction of limits zone transition 0=low2hi, 1=hi2low
 OperatorCommand | 1009 | String | A | * The last operator command issued during control_mode of REMOTE.
 PPChangeName | 1010 | String | A | * The Process Program ID, PPID, affected by the creation, edit, or delete local
 PPChangeStatus | 1011 | Integer | U1 | * The type of change for RcpChangeName, 1=created, 2=updated, 3=stored new, 4=overwritten, 5=deleted, 6=copied, 7=renamed
 PPError | 1012 | String | A | * Contains information about a failure to verify a process program.
 RcpChangeName | 1013 | String | A | * The identifier of the Stream 15 recipe affected by creation, editing, or deletion.
 RcpChangeStatus | 1014 | Integer | U1 | * The type of change for RcpChangeName, 1=created, 2=updated, 3=stored new, 4=overwritten, 5=deleted, 6=copied, 7=renamed.
 DataSetName | 1015 | String | A | * The Process Program ID or Data Set in the latest Stream 13 large file upload
 PortID | 1101 | Integer | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | L | * The PortAssociationState combined with the PortTransferState. List of 2 items
 CarrierID | 1121 | String | A | * The ID of the carrier.
 Capacity | 1122 | Integer | U1 | * E87 substrate capacity of the current carrier
 CarrierIDStatus | 1123 | Integer | U1 | * State of the carrier ID status.  Enumerated
 SlotMapStatus | 1124 | Integer | U1 | * State of the carrier slot map status. Enumerated
 CarrierAccessingStatus | 1125 | Integer | U1 | * E87 carrier accessing status; 0=not accessed, 1=in access, 2=carrier complete, 3=carrier stopped
 SlotMap | 1126 | Object | L | * E87 carrier slot map, list of U1: 0=undefined,1=empty,2=not empty,3=correctly occupied, 4=double slotted, 5=cross slotted
 Reason | 1127 | Integer | U1 | * E87 enumerated reason for carrier ID read failure, 0=verification needed,1=verification fail,2=read fail,3=out of position
 ContentMap | 1128 | Object | L | * E87 content map of the current carrier, list of lot ID and substrate ID pairs
 CarrierLocationID | 1129 | String | A | * The location identifier for the latest MaterialReceived or MaterialRemoved event.
 SubstrateCount | 1130 | Integer | U1 | * E87 count of substrates in the current carrier
 Usage | 1131 | String | A | * E87 usage value for the current carrier
 WID_Angle | 1132 | Object | L | * E87 Carrier Attribute, list of angles for ID reading for the contained substrates
 SubstID | 1151 | String | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstMtrlStatus | 1153 | Integer | U1 | * E90 Substrate Attribute, processing material status of the current substrate object
 SubstState | 1154 | Integer | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 SubstIDStatus | 1156 | Integer | U2 | * E90 Substrate Attribute, substrate ID verification status, 0=Not, 1=Waiting, 2=Ok, 3=Failed
 SubstSubstLocID | 1157 | String | A | * E90 Substrate Attribute, substrate location ID
 SubstSource | 1158 | String | A | * The starting Substrate Location for this Substrate.
 SubstDestination | 1159 | String | A | * E90 Substrate Attribute, destination location ID of the current substrate
 SubstHistory | 1160 | Object | L | * Ordered list of 3-element lists, showing the current history
 SubstUsage | 1161 | Integer | U1 | * E90 Substrate Attribute, enumerated usage of the current substrate object, 0=product, 1=test, 2=filler
 SubstType | 1162 | Integer | U1 | * E90 Substrate Attribute, enumerated type of the current substrate object, 0=wafer
 AcquiredID | 1163 | String | A | * Contains the ID read from the substrate
 SubstWID_Angle | 1166 | Integer | U2 | * E90 Substrate Attribute, angle difference from nominal for ID reading for the current substrate object
 SubstSupplierID | 1167 | String | A | * E90 Substrate Attribute, Supplier ID as read from substrate
 SubstThickness | 1168 | Decimal | F8 | * E90 Substrate Attribute, Measured substrate thickness in mm
 SubstBow | 1169 | Decimal | F8 | * E90 Substrate Attribute, Measured substrate bow in mm
 SubstLocSubstLocID | 1181 | String | A | * Identifier for substrate location, the Substrate Location at which this Substrate currently resides.
 SubstLocSubstID | 1182 | String | A | * Substrate Identifier relevant to the location. The Substrate ID of the Substrate, if any, that currently resides at this Substrate Location.
 SubstLocSubstLocState | 1183 | Integer | U1 | * State of substrate location 0 – UNOCCUPIED  1 – OCCUPIED
 SubstrateWeight | 1190 | Decimal | F8 | * Substrate SubstrateWeight
 SmithId4 | 1191 | String | A | * Numeric ID of the smith port in the current context
 PRJobID | 1201 | String | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted
 RMtlNameList | 1203 | Object | L | * E40 process job material list
 PRMtlType | 1204 | Binary | BI | * E40 process job material type, 13=carriers, 14=substrates
 RecID | 1205 | String | A | * E40 process recipe identifier (RCPSPEC)
 PRRecipeMethod | 1206 | Integer | U1 | * E40 process recipe tuning enum, 1=recipe without variable tuning, 2=recipe with variable tuning
 RecVariableList | 1207 | String | A | * E40 process job recipe parameter name value pair list (L [L:2 <RCPPARNM> <RCPPARVAL>]*)
 PRProcessStart | 1208 | Integer | U1 | * E40 job start flag, 0=manual, 1=automatic
 PauseEvent | 1209 | Object | L | * E40 A list of event CEIDs which imply ProcessJob PAUSING/PAUSED state changes
 PRResumeState | 1210 | Integer | U1 | * E40 process job state that a paused job would resume to, 0=NA, 1=setting up, 2=waiting for start, 3=processing
 PRTimeCreated | 1211 | String | A | * E40 process job Clock value when created
 PRTimeUpdated | 1212 | String | A | * E40 process job Clock value when latest state change
 CtrlJobID | 1231 | String | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED
 PRJobStatusList | 1233 | Object | L | * E94 Control Job Attribute, a list of PROBID, PRSTATE for process jobs of the current Control Job.
 CarrierInputSpec | 1234 | Object | L | * E94 Control Job Attribute, a list of CarrierIDs for the current Control Job
 MtrlOutSpec | 1235 | Object | L | * E94 Control Job Attribute, a list of source/destination pairs for the current Control Job
 MtrlOutByStatus | 1236 | Object | L | * E94 Control Job Attribute, a list of status dispositions for the current Control Job
 ProcessingCtrlSpec | 1237 | Object | L | * E94 Control Job Attribute, the list of process job specifications for the current Control Job.
 CurrentPRJob | 1238 | Object | L | * E94 Control Job Attribute, the list of Process Job ObjIDs of the current Control Job
 DataCollectionPlan | 1239 | String | A | * E94 Control Job Attribute, the DataCollectionPlan of the current Control Job
 ProcessOrderMgmt | 1240 | Integer | U1 | * E94 Control Job Attribute, the ordering value for the current Control Job, 1=ARRIVAL, 2=OPTIMIZE, 3=LIST
 StartMethod | 1241 | Boolean | BO | * E94 Control Job Attribute, the start method of the current Control Job, 0=UserStart, 1=AutoStart.
 ObjID | 1261 | String | A | * E39 identifier of the current object, E40 uses for ProcessJobID
 ObjType | 1262 | String | A | * E39 identifier of the current object type, E40 sets to ProcessJob
 ReasonSlotMapVerification | 1502 | Integer | U1 | * The reason for transition 14, SLOT MAP NOT READ to WAITING FOR HOST (0 = VERIFICATION NEEDED, 1 = VERIFICATION BY EQUIPMENT UNSUCCESSFUL, 2 = READ FAIL, 3 = IMPROPER SUBSTRATE POSITION)
 SubstrateInspectionImagePath | 1503 | Object | L | * List of absolute image path of inspected substrate
 Clock | 2001 | String | A | * The CarrierID at the ithlocationID.
 MDLN | 2002 | String | A | * Equipment Model Type
 SOFTREV | 2003 | String | A | * Equipment Software Revision ID
 EventsEnabled | 2004 | Object | L | * The list of events CEIDs enabled for reporting via S6.
 AlarmsEnabled | 2006 | Object | L | * The list of alarms ALIDs enabled for reporting via S5.
 AlarmsSet | 2007 | Object | L | * The list of alarms ALIDs in the set state, irrespective of reporting.
 ControlState | 2008 | Integer | U1 | * 1=off-line/eq off-line 2=off/seek on 3=off/host off 4=on/local 5=on/remote
 ControlMode | 2009 | Integer | U1 | * 0=Local, 1=Remote (aka E5-95 ControlState 2nd definition)
 ProcessState | 2010 | Integer | U1 | * The current processing state.
 PreviousProcessState | 2011 | Integer | U1 | * The previous processing state.
 DvvalList | 2012 | Object | L | * A list of all DVVAL variables, their IDs and names (L [L:2 <VID> <DVNAME>]+).
 EventDescriptions | 2013 | Object | L | * A list of all event CEIDs and their descriptions
 PPExecName | 2014 | String | A | * The PPID(s) of the selected Process Program(s).
 PPUsedName | 2015 | String | A | * The PPID(s) of the Process Program(s) last used and possibly still in use.
 PPFormat | 2016 | Integer | U1 | * The type(s) of process programs and recipes, unformatted PP types are: 1=all small, 5=all large, 10=both small and large.
 RcpExecName | 2017 | String | A | * The identifier of the currently selected Stream 15 recipe.
 SpoolCountActual | 2018 | Integer | U4 | * The actual # of msgs queued.
 SpoolCountTotal | 2019 | Integer | U4 | * The total # of msgs spooled and/or discarded.
 SpoolFullTime | 2020 | String | A | * The Clock value from the time the spool last became full.
 SpoolStartTime | 2021 | String | A | * The Clock value from the time spooling last became active.
 SpoolStreamFns | 2022 | String | A | * The list of SnFm message types that are spooled, set by S2F43.
 PortCount | 2101 | Integer | U1 | * E87 the total number of Load Ports
 PortTransferStateList | 2102 | Object | L | * E87 a list of transfer state model values for all Load Ports
 PortStateInfoList | 2103 | Object | L | * E87 A list of PortStateInfo for all the load ports
 LoadPortReservationStateList | 2104 | Object | L | * E87 a list of reservation state model values, U1, for the Load Ports
 PortAssociationStateList | 2105 | Object | L | * E87 a list of association states, U1, for the Load Ports
 CarrierLocationMatrix | 2106 | Object | L | * E87 a list of L:2 LocationID CarrierID pairs for every equipment location.
 AccessMode1 | 2121 | Integer | U1 | * E87 access mode of Load Port 1, 0=manual, 1=auto
 AccessMode2 | 2122 | Integer | U1 | * E87 access mode of Load Port 2, 0=manual, 1=auto
 AccessMode3 | 2123 | Integer | U1 | * E87 access mode of Load Port 3, 0=manual, 1=auto
 AccessMode4 | 2124 | Integer | U1 | * E87 access mode of Load Port 4, 0=manual, 1=auto
 AccessMode5 | 2125 | Integer | U1 | * E87 access mode of Load Port 5, 0=manual, 1=auto
 AccessMode6 | 2126 | Integer | U1 | * E87 access mode of Load Port 6, 0=manual, 1=auto
 CarrierID1 | 2131 | String | A | * E87 ID of the carrier at Load Port 1 transfer position
 CarrierID2 | 2132 | String | A | * E87 ID of the carrier at Load Port 2 transfer position
 CarrierID3 | 2133 | String | A | * E87 ID of the carrier at Load Port 3 transfer position
 CarrierID4 | 2134 | String | A | * E87 ID of the carrier at Load Port 4 transfer position
 CarrierID5 | 2135 | String | A | * E87 ID of the carrier at Load Port 5 transfer position
 CarrierID6 | 2136 | String | A | * E87 ID of the carrier at Load Port 6 transfer position
 LoadPortReservationState1 | 2141 | Integer | U1 | * E87 reservation state model value of Load Port 1
 LoadPortReservationState2 | 2142 | Integer | U1 | * E87 reservation state model value of Load Port 2
 LoadPortReservationState3 | 2143 | Integer | U1 | * E87 reservation state model value of Load Port 3
 LoadPortReservationState4 | 2144 | Integer | U1 | * E87 reservation state model value of Load Port 4
 LoadPortReservationState5 | 2145 | Integer | U1 | * E87 reservation state model value of Load Port 5
 LoadPortReservationState6 | 2146 | Integer | U1 | * E87 reservation state model value of Load Port 6
 LocationID1 | 2151 | String | A | * E87 the assigned LocationID value for Carrier Load Port 1 transfer position
 LocationID2 | 2152 | String | A | * E87 the assigned LocationID value for Carrier Load Port 2 transfer position
 LocationID3 | 2153 | String | A | * E87 the assigned LocationID value for Carrier Load Port 3 transfer position
 LocationID4 | 2154 | String | A | * E87 the assigned LocationID value for Carrier Load Port 4 transfer position
 LocationID5 | 2155 | String | A | * E87 the assigned LocationID value for Carrier Load Port 5 transfer position
 LocationID6 | 2156 | String | A | * E87 the assigned LocationID value for Carrier Load Port 6 transfer position
 LocationID7 | 2157 | String | A | * E87 the assigned LocationID value for Carrier Load Port 1 FOUP dock position
 LocationID8 | 2158 | String | A | * E87 the assigned LocationID value for Carrier Load Port 2 FOUP dock position
 LocationID9 | 2159 | String | A | * E87 the assigned LocationID value for Carrier Load Port 3 FOUP dock position
 LocationID10 | 2160 | String | A | * E87 the assigned LocationID value for Carrier Load Port 4 FOUP dock position
 LocationID11 | 2161 | String | A | * E87 the assigned LocationID value for Carrier Load Port 5 FOUP dock position
 LocationID12 | 2162 | String | A | * E87 the assigned LocationID value for Carrier Load Port 6 FOUP dock position
 PortAssociationState1 | 2171 | Integer | U1 | * E87 association state of Load Port 1
 PortAssociationState2 | 2172 | Integer | U1 | * E87 association state of Load Port 2
 PortAssociationState3 | 2173 | Integer | U1 | * E87 association state of Load Port 3
 PortAssociationState4 | 2174 | Integer | U1 | * E87 association state of Load Port 4
 PortAssociationState5 | 2175 | Integer | U1 | * E87 association state of Load Port 5
 PortAssociationState6 | 2176 | Integer | U1 | * E87 association state of Load Port 6
 PortID1 | 2181 | Integer | U1 | * E87 The PTN numeric ID value of Load Port 1
 PortID2 | 2182 | Integer | U1 | * E87 The PTN numeric ID value of Load Port 2
 PortID3 | 2183 | Integer | U1 | * E87 The PTN numeric ID value of Load Port 3
 PortID4 | 2184 | Integer | U1 | * E87 The PTN numeric ID value of Load Port 4
 PortID5 | 2185 | Integer | U1 | * E87 The PTN numeric ID value of Load Port 5
 PortID6 | 2186 | Integer | U1 | * E87 The PTN numeric ID value of Load Port 6
 PortStateInfo1 | 2191 | Object | L | * E87 the Association State and Transfer State for Load Port 1
 PortStateInfo2 | 2192 | Object | L | * E87 the Association State and Transfer State for Load Port 2
 PortStateInfo3 | 2193 | Object | L | * E87 the Association State and Transfer State for Load Port 3
 PortStateInfo4 | 2194 | Object | L | * E87 the Association State and Transfer State for Load Port 4
 PortStateInfo5 | 2195 | Object | L | * E87 the Association State and Transfer State for Load Port 5
 PortStateInfo6 | 2196 | Object | L | * E87 the Association State and Transfer State for Load Port 6
 PortTransferState1 | 2201 | Integer | U1 | * E87 transfer state of Load 1
 PortTransferState2 | 2202 | Integer | U1 | * E87 transfer state of Load 2
 PortTransferState3 | 2203 | Integer | U1 | * E87 transfer state of Load 3
 PortTransferState4 | 2204 | Integer | U1 | * E87 transfer state of Load 4
 PortTransferState5 | 2205 | Integer | U1 | * E87 transfer state of Load 5
 PortTransferState6 | 2206 | Integer | U1 | * E87 transfer state of Load 6
 PlacedCarrierPattern1 | 2211 | Integer | U1 | * Pattern of the carrier that is placed on LP1
 PlacedCarrierPattern2 | 2212 | Integer | U1 | * Pattern of the carrier that is placed on LP2
 PlacedCarrierPattern3 | 2213 | Integer | U1 | * Pattern of the carrier that is placed on LP3
 PlacedCarrierPattern4 | 2214 | Integer | U1 | * Pattern of the carrier that is placed on LP4
 PlacedCarrierPattern5 | 2215 | Integer | U1 | * Pattern of the carrier that is placed on LP5
 PlacedCarrierPattern6 | 2216 | Integer | U1 | * Pattern of the carrier that is placed on LP6
 Material1 | 2221 | String | A | * Material ID of the carrier that is placed on LP1
 Material2 | 2222 | String | A | * Material ID of the carrier that is placed on LP2
 Material3 | 2223 | String | A | * Material ID of the carrier that is placed on LP3
 Material4 | 2224 | String | A | * Material ID of the carrier that is placed on LP4
 Material5 | 2225 | String | A | * Material ID of the carrier that is placed on LP5
 Material6 | 2226 | String | A | * Material ID of the carrier that is placed on LP6
 SmithLockState4 | 2234 | String | A | * Lock status of the smith port 4
 SmithLockState5 | 2235 | String | A | * Lock status of the smith port 5
 SmithLockState6 | 2236 | String | A | * Lock status of the smith port 6
 BatchSize | 2301 | Integer | U2 | * E40 The maximum number of parts in a process job
 CarrierMode | 2302 | Integer | U4 | * E40 Describes the use of CarrierID/slots or MID values in process jobs. 1=normal carrier usage
 RecPossibleVars | 2303 | Object | L | * E40 possible process recipe parameter names for recipes that accept tuning (L [<RCPPARNM>]* )
 CjControlRuleNames | 2351 | Object | L | * E94 the list of allowed control rule names in ProcessingCtrlSpec.
 CjMaterialStatusList | 2352 | Object | L | * E94 the list of allowed MaterialStatus values in ProcessingCtrlSpec.
 CjOutputRuleValues | 2353 | Object | L | * E94 the list of allowed disposition values for Material Status values in ProcessingCtrlSpec.
 QueueAvailableSpace | 2354 | Integer | U2 | * E94 Number of Control Jobs that can be created.
 QueuedCJobs | 2355 | Object | L | * E94 Ordered list of queued control job IDs.
 SubstLocID1 | 2401 | Object | L | * E90 SubstLoc Attribute, object identifier of substrate location 1
 SubstLocID2 | 2402 | Object | L | * E90 SubstLoc Attribute, object identifier of substrate location 2
 SubstLocID3 | 2403 | Object | L | * E90 SubstLoc Attribute, object identifier of substrate location 3
 SubstLocState1 | 2421 | Integer | U1 | * E90 SubstLoc Attribute, occupancy status value of substrate location 1
 SubstLocState2 | 2422 | Integer | U1 | * E90 SubstLoc Attribute, occupancy status value of substrate location 2
 SubstLocState3 | 2423 | Integer | U1 | * E90 SubstLoc Attribute, occupancy status value of substrate location 3
 SubstLocSubstID1 | 2441 | String | A | * E90 SubstLoc Attribute, substrate ID of substrate location 1
 SubstLocSubstID2 | 2442 | String | A | * E90 SubstLoc Attribute, substrate ID of substrate location 2
 SubstLocSubstID3 | 2443 | String | A | * E90 SubstLoc Attribute, substrate ID of substrate location 3

Events
======

### ControlStateOFFLINE

Event for *CollectioneventID. ID: **1**

*(this event has no properties)*

### ControlStateLOCAL

Event for *CollectioneventID. ID: **2**

*(this event has no properties)*

### ControlStateREMOTE

Event for *CollectioneventID. ID: **3**

*(this event has no properties)*

### MATERIAL_PLACED

Event for *CollectioneventID. ID: **4**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PlacedCarrierPattern1 | 2211 | Integer | Yes | U1 | * Pattern of the carrier that is placed on LP1
 PlacedCarrierPattern2 | 2212 | Integer | Yes | U1 | * Pattern of the carrier that is placed on LP2
 PlacedCarrierPattern3 | 2213 | Integer | Yes | U1 | * Pattern of the carrier that is placed on LP3
 PlacedCarrierPattern4 | 2214 | Integer | Yes | U1 | * Pattern of the carrier that is placed on LP4

### MATERIAL_REMOVED

Event for *CollectioneventID. ID: **5**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PlacedCarrierPattern1 | 2211 | Integer | Yes | U1 | * Pattern of the carrier that is placed on LP1
 PlacedCarrierPattern2 | 2212 | Integer | Yes | U1 | * Pattern of the carrier that is placed on LP2
 PlacedCarrierPattern3 | 2213 | Integer | Yes | U1 | * Pattern of the carrier that is placed on LP3
 PlacedCarrierPattern4 | 2214 | Integer | Yes | U1 | * Pattern of the carrier that is placed on LP4

### OperatorCommandIssued

Event for *CollectioneventID. ID: **6**

*(this event has no properties)*

### OperatorECVChange

Event for *CollectioneventID. ID: **7**

*(this event has no properties)*

### ProcessProgramChange

Event for *CollectioneventID. ID: **8**

*(this event has no properties)*

### ProcessProgramInvalid

Event for *CollectioneventID. ID: **9**

*(this event has no properties)*

### ProcessProgramSelected

Event for *CollectioneventID. ID: **10**

*(this event has no properties)*

### ProcessingStarted

Event for *CollectioneventID. ID: **11**

*(this event has no properties)*

### ProcessingCompleted

Event for *CollectioneventID. ID: **12**

*(this event has no properties)*

### ProcessStopped

Event for *CollectioneventID. ID: **13**

*(this event has no properties)*

### ProcessStateUpdate

Event for *CollectioneventID. ID: **14**

*(this event has no properties)*

### SpoolingActivated

Event for *CollectioneventID. ID: **15**

*(this event has no properties)*

### SpoolingDeactivated

Event for *CollectioneventID. ID: **16**

*(this event has no properties)*

### SpoolTransmitFailure

Event for *CollectioneventID. ID: **17**

*(this event has no properties)*

### ExecutionRecipeNew

Event for *CollectioneventID. ID: **18**

*(this event has no properties)*

### ExecutionRecipeChange

Event for *CollectioneventID. ID: **19**

*(this event has no properties)*

### TerminalServicesOperatorAck

Event for *CollectioneventID. ID: **20**

*(this event has no properties)*

### UploadFailure

Event for *CollectioneventID. ID: **21**

*(this event has no properties)*

### UploadSuccess

Event for *CollectioneventID. ID: **22**

*(this event has no properties)*

### UploadTimeout

Event for *CollectioneventID. ID: **23**

*(this event has no properties)*

### LPTSM1_NOSTATE_INSERVICE

Event for *CollectioneventID. ID: **101**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPTSM2_OUTOFSERVICE_INSERVICE

Event for *CollectioneventID. ID: **102**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPTSM3_INSERVICE_OUTOFSERVICE

Event for *CollectioneventID. ID: **103**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPTSM4_INSERVICE_TRANSFERREADY_OR_TRANSFERBLOCKED

Event for *CollectioneventID. ID: **104**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPTSM5_TRANSFERREADY_READYTOLOAD_OR_READYTOUNLOAD

Event for *CollectioneventID. ID: **105**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPTSM6_READYTOLOAD_TRANSFERBLOCKED

Event for *CollectioneventID. ID: **106**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPTSM7_READYTOUNLOAD_TRANSFERBLOCKED

Event for *CollectioneventID. ID: **107**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPTSM8_TRANSFERBLOCKED_READYTOLOAD

Event for *CollectioneventID. ID: **108**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPTSM9_TRANSFERBLOCKED_READYTOUNLOAD

Event for *CollectioneventID. ID: **109**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPTSM10_TRANSFERBLOCKED_TRANSFERREADY

Event for *CollectioneventID. ID: **110**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPAMSM1_NOSTATE_AUTO_MANUAL

Event for *CollectioneventID. ID: **121**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated

### LPAMSM2_MANUAL_AUTO

Event for *CollectioneventID. ID: **122**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated

### LPAMSM3_AUTO_MANUAL

Event for *CollectioneventID. ID: **123**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated

### LPRSM1_NOSTATE_NOTRESERVED

Event for *CollectioneventID. ID: **141**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPRSM2_NOTRESERVED_RESERVED

Event for *CollectioneventID. ID: **142**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPRSM3_RESERVED_NOTRESERVED

Event for *CollectioneventID. ID: **143**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPCASM1_NOSTATE_NOTASSOCIATED

Event for *CollectioneventID. ID: **161**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPCASM2_NOTASSOCIATED_ASSOCIATED

Event for *CollectioneventID. ID: **162**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPCASM3_ASSOCIATED_NOTASSOCIATED

Event for *CollectioneventID. ID: **163**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### LPCASM4_ASSOCIATED_ASSOCIATED

Event for *CollectioneventID. ID: **164**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 PortTransferState | 1102 | Integer | Yes | U1 | * The current transfer state of a load port. Enumerated
 AccessMode | 1103 | Integer | Yes | U1 | * The access mode of the loadport.     Enumerated
 LoadPortReservationState | 1104 | Integer | Yes | U1 | * The reservation state of a Load Port. Enumerated
 PortAssociationState | 1105 | Integer | Yes | U1 | * The association state of a load port. Enumerated
 PortStateInfo | 1106 | Object | Yes | L | * The PortAssociationState combined with the PortTransferState. List of 2 items

### COSM1_NOSTATE_CARRIER

Event for *CollectioneventID. ID: **181**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierIDStatus | 1123 | Integer | Yes | U1 | * State of the carrier ID status.  Enumerated
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 SlotMapStatus | 1124 | Integer | Yes | U1 | * State of the carrier slot map status. Enumerated

### COSM2_NOSTATE_IDNOTREAD

Event for *CollectioneventID. ID: **182**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierIDStatus | 1123 | Integer | Yes | U1 | * State of the carrier ID status.  Enumerated
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 SlotMapStatus | 1124 | Integer | Yes | U1 | * State of the carrier slot map status. Enumerated

### COSM3_NOSTATE_IDWAITINGFORHOST

Event for *CollectioneventID. ID: **183**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierIDStatus | 1123 | Integer | Yes | U1 | * State of the carrier ID status.  Enumerated
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 SlotMapStatus | 1124 | Integer | Yes | U1 | * State of the carrier slot map status. Enumerated

### COSM4_NOSTATE_IDVERIFICATIONOK

Event for *CollectioneventID. ID: **184**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierIDStatus | 1123 | Integer | Yes | U1 | * State of the carrier ID status.  Enumerated
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 SlotMapStatus | 1124 | Integer | Yes | U1 | * State of the carrier slot map status. Enumerated

### COSM5_NOSTATE_IDVERIFICATIONFAIL

Event for *CollectioneventID. ID: **185**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierIDStatus | 1123 | Integer | Yes | U1 | * State of the carrier ID status.  Enumerated
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 SlotMapStatus | 1124 | Integer | Yes | U1 | * State of the carrier slot map status. Enumerated

### COSM6_IDNOTREAD_IDVERIFICATIONOK

Event for *CollectioneventID. ID: **186**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierIDStatus | 1123 | Integer | Yes | U1 | * State of the carrier ID status.  Enumerated
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 SlotMapStatus | 1124 | Integer | Yes | U1 | * State of the carrier slot map status. Enumerated

### COSM7_IDNOTREAD_IDWAITINGFORHOST

Event for *CollectioneventID. ID: **187**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierIDStatus | 1123 | Integer | Yes | U1 | * State of the carrier ID status.  Enumerated
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 SlotMapStatus | 1124 | Integer | Yes | U1 | * State of the carrier slot map status. Enumerated

### COSM8_IDWAITINGFORHOST_IDVERIFICATIONOK

Event for *CollectioneventID. ID: **188**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierIDStatus | 1123 | Integer | Yes | U1 | * State of the carrier ID status.  Enumerated
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 SlotMapStatus | 1124 | Integer | Yes | U1 | * State of the carrier slot map status. Enumerated

### COSM9_IDWAITINGFORHOST_IDVERIFICATIONFAIL

Event for *CollectioneventID. ID: **189**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierIDStatus | 1123 | Integer | Yes | U1 | * State of the carrier ID status.  Enumerated
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 SlotMapStatus | 1124 | Integer | Yes | U1 | * State of the carrier slot map status. Enumerated

### COSM10_IDNOTREAD_IDWAITINGFORHOST

Event for *CollectioneventID. ID: **190**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierIDStatus | 1123 | Integer | Yes | U1 | * State of the carrier ID status.  Enumerated
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 SlotMapStatus | 1124 | Integer | Yes | U1 | * State of the carrier slot map status. Enumerated

### COSM11_IDNOTREAD_IDVERIFICATIONOK

Event for *CollectioneventID. ID: **191**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierIDStatus | 1123 | Integer | Yes | U1 | * State of the carrier ID status.  Enumerated
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 SlotMapStatus | 1124 | Integer | Yes | U1 | * State of the carrier slot map status. Enumerated

### COSM12_NOSTATE_SLOTMAPNOTREAD

Event for *CollectioneventID. ID: **192**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 ContentMap | 1128 | Object | Yes | L | * E87 content map of the current carrier, list of lot ID and substrate ID pairs
 SlotMap | 1126 | Object | Yes | L | * E87 carrier slot map, list of U1: 0=undefined,1=empty,2=not empty,3=correctly occupied, 4=double slotted, 5=cross slotted

### COSM13_SLOTMAPNOTREAD_SLOTMAPVERIFICATIONOK

Event for *CollectioneventID. ID: **193**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 ContentMap | 1128 | Object | Yes | L | * E87 content map of the current carrier, list of lot ID and substrate ID pairs
 SlotMap | 1126 | Object | Yes | L | * E87 carrier slot map, list of U1: 0=undefined,1=empty,2=not empty,3=correctly occupied, 4=double slotted, 5=cross slotted

### COSM14_SLOTMAPNOTREAD_SLOTMAPWAITINGFORHOST

Event for *CollectioneventID. ID: **194**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 ContentMap | 1128 | Object | Yes | L | * E87 content map of the current carrier, list of lot ID and substrate ID pairs
 SlotMap | 1126 | Object | Yes | L | * E87 carrier slot map, list of U1: 0=undefined,1=empty,2=not empty,3=correctly occupied, 4=double slotted, 5=cross slotted

### COSM15_SLOTMAPWAITINGFORHOST_SLOTMAPVERIFICATIONOK

Event for *CollectioneventID. ID: **195**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 ContentMap | 1128 | Object | Yes | L | * E87 content map of the current carrier, list of lot ID and substrate ID pairs
 SlotMap | 1126 | Object | Yes | L | * E87 carrier slot map, list of U1: 0=undefined,1=empty,2=not empty,3=correctly occupied, 4=double slotted, 5=cross slotted

### COSM16_SLOTMAPWAITINGFORHOST_SLOTMAPVERIFICATIONFAIL

Event for *CollectioneventID. ID: **196**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 ContentMap | 1128 | Object | Yes | L | * E87 content map of the current carrier, list of lot ID and substrate ID pairs
 SlotMap | 1126 | Object | Yes | L | * E87 carrier slot map, list of U1: 0=undefined,1=empty,2=not empty,3=correctly occupied, 4=double slotted, 5=cross slotted

### COSM17_NOSTATE_NOTACCESSED

Event for *CollectioneventID. ID: **197**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 CarrierAccessingStatus | 1125 | Integer | Yes | U1 | * E87 carrier accessing status; 0=not accessed, 1=in access, 2=carrier complete, 3=carrier stopped
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO

### COSM18_NOTACCESSED_INACCESS

Event for *CollectioneventID. ID: **198**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 CarrierAccessingStatus | 1125 | Integer | Yes | U1 | * E87 carrier accessing status; 0=not accessed, 1=in access, 2=carrier complete, 3=carrier stopped
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO

### COSM19_INACCESS_CARRIERCOMPLETE

Event for *CollectioneventID. ID: **199**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 CarrierAccessingStatus | 1125 | Integer | Yes | U1 | * E87 carrier accessing status; 0=not accessed, 1=in access, 2=carrier complete, 3=carrier stopped
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO

### COSM20_INACCESS_CARRIERSTOPPED

Event for *CollectioneventID. ID: **200**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 CarrierAccessingStatus | 1125 | Integer | Yes | U1 | * E87 carrier accessing status; 0=not accessed, 1=in access, 2=carrier complete, 3=carrier stopped
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO

### COSM21_CARRIER_NOSTATE

Event for *CollectioneventID. ID: **201**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 CarrierAccessingStatus | 1125 | Integer | Yes | U1 | * E87 carrier accessing status; 0=not accessed, 1=in access, 2=carrier complete, 3=carrier stopped
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO

### CarrierIDReadFail

Event for *CollectioneventID. ID: **221**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 CarrierLocationID | 1129 | String | Yes | A | * The location identifier for the latest MaterialReceived or MaterialRemoved event.

### UnknownCarrierID

Event for *CollectioneventID. ID: **222**

*(this event has no properties)*

### IDReaderAvailable

Event for *CollectioneventID. ID: **223**

*(this event has no properties)*

### IDReaderUnavailable

Event for *CollectioneventID. ID: **224**

*(this event has no properties)*

### CarrierApproachingComplete

Event for *CollectioneventID. ID: **225**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 CarrierLocationID | 1129 | String | Yes | A | * The location identifier for the latest MaterialReceived or MaterialRemoved event.

### CarrierClamped

Event for *CollectioneventID. ID: **226**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 CarrierLocationID | 1129 | String | Yes | A | * The location identifier for the latest MaterialReceived or MaterialRemoved event.

### CarrierUnclamped

Event for *CollectioneventID. ID: **227**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 CarrierLocationID | 1129 | String | Yes | A | * The location identifier for the latest MaterialReceived or MaterialRemoved event.

### CarrierLocationChange

Event for *CollectioneventID. ID: **228**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 CarrierLocationID | 1129 | String | Yes | A | * The location identifier for the latest MaterialReceived or MaterialRemoved event.

### CarrierOpened

Event for *CollectioneventID. ID: **229**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 CarrierLocationID | 1129 | String | Yes | A | * The location identifier for the latest MaterialReceived or MaterialRemoved event.

### CarrierClosed

Event for *CollectioneventID. ID: **230**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 CarrierLocationID | 1129 | String | Yes | A | * The location identifier for the latest MaterialReceived or MaterialRemoved event.

### CarrierTagReleased

Event for *CollectioneventID. ID: **231**

*(this event has no properties)*

### DuplicateCarrierIDInProcess

Event for *CollectioneventID. ID: **232**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PortID | 1101 | Integer | Yes | U1 | * ID of a load port.  Positive integer  RO
 CarrierID | 1121 | String | Yes | A | * The ID of the carrier.
 CarrierLocationID | 1129 | String | Yes | A | * The location identifier for the latest MaterialReceived or MaterialRemoved event.

### SOSM1_NOSTATE_ATSOURCE

Event for *CollectioneventID. ID: **301**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM2_ATSOURCE_ATWORK

Event for *CollectioneventID. ID: **302**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM3_ATWORK_ATSOURCE

Event for *CollectioneventID. ID: **303**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM4_ATWORK_ATWORK

Event for *CollectioneventID. ID: **304**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM5_ATWORK_ATDESTINATION

Event for *CollectioneventID. ID: **305**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM6_ATDESTINATION_ATWORK

Event for *CollectioneventID. ID: **306**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM7_ATDESTINATION_NOSTATE

Event for *CollectioneventID. ID: **307**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM8_ATDESTINATION_ATSOURCE

Event for *CollectioneventID. ID: **308**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM9_ANYSTATE_NOSTATE

Event for *CollectioneventID. ID: **309**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM10_NOSTATE_NEEDSPROCESSING

Event for *CollectioneventID. ID: **310**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM11_NEEDSPROCESSING_INPROCESS

Event for *CollectioneventID. ID: **311**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM12_INPROCESS_PROCESSINGCOMPLETE

Event for *CollectioneventID. ID: **312**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM13_INPROCESS_NEEDSPROCESSING

Event for *CollectioneventID. ID: **313**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM14_NEEDSPROCESSING_PROCESSINGCOMPLETE

Event for *CollectioneventID. ID: **314**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM16_NOSTATE_NOTCONFIRMED

Event for *CollectioneventID. ID: **316**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstState | 1154 | Integer | Yes | U1 | * E90 Substrate Attribute, transport state of the current substrate object, 0=source, 1=at work, 2=destination
 SubstProcState | 1155 | Integer | Yes | U1 | * E90 Substrate Attribute, processing state of the current substrate object, 0=Needs, 1=InProcess, 2=Done, 3=Aborted, 4=Stopped, 5=Rejected, 6=Lost, 7=Skipped
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM17_NOTCONFIRMED_CONFIRMED

Event for *CollectioneventID. ID: **317**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstSource | 1158 | String | Yes | A | * The starting Substrate Location for this Substrate.
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM18_NOTCONFIRMED_WAITINGFORHOST

Event for *CollectioneventID. ID: **318**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstSource | 1158 | String | Yes | A | * The starting Substrate Location for this Substrate.
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM19_NOTCONFIRMED_WAITINGFORHOST

Event for *CollectioneventID. ID: **319**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstSource | 1158 | String | Yes | A | * The starting Substrate Location for this Substrate.
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM20_WAITINGFORHOST_CONFIRMED

Event for *CollectioneventID. ID: **320**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstSource | 1158 | String | Yes | A | * The starting Substrate Location for this Substrate.
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SOSM21_WAITINGFORHOST_CONFIRMATIONFAILED

Event for *CollectioneventID. ID: **321**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstID | 1151 | String | Yes | A | * Identifier of the Substrate.
 SubstLotID | 1152 | String | Yes | A | * The Identifier of the Lot associated with this Substrate. If unknown, the value is zero-length.
 SubstSubstLocID | 1157 | String | Yes | A | * E90 Substrate Attribute, substrate location ID
 SubstSource | 1158 | String | Yes | A | * The starting Substrate Location for this Substrate.
 AcquiredID | 1163 | String | Yes | A | * Contains the ID read from the substrate
 SubstHistory | 1160 | Object | Yes | L | * Ordered list of 3-element lists, showing the current history
 Clock | 2001 | String | Yes | A | * The CarrierID at the ithlocationID.

### SLOSM1_UNOCCUPIED_OCCUPIED

Event for *CollectioneventID. ID: **341**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstLocSubstID | 1182 | String | Yes | A | * Substrate Identifier relevant to the location. The Substrate ID of the Substrate, if any, that currently resides at this Substrate Location.
 SubstLocSubstLocID | 1181 | String | Yes | A | * Identifier for substrate location, the Substrate Location at which this Substrate currently resides.
 SubstLocSubstLocState | 1183 | Integer | Yes | U1 | * State of substrate location 0 – UNOCCUPIED  1 – OCCUPIED

### SLOSM2_OCCUPIED_UNOCCUPIED

Event for *CollectioneventID. ID: **342**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 SubstLocSubstID | 1182 | String | Yes | A | * Substrate Identifier relevant to the location. The Substrate ID of the Substrate, if any, that currently resides at this Substrate Location.
 SubstLocSubstLocID | 1181 | String | Yes | A | * Identifier for substrate location, the Substrate Location at which this Substrate currently resides.
 SubstLocSubstLocState | 1183 | Integer | Yes | U1 | * State of substrate location 0 – UNOCCUPIED  1 – OCCUPIED

### PJSM1_NOSTATE_QUEUED

Event for *CollectioneventID. ID: **401**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM2_QUEUED_SETTINGUP

Event for *CollectioneventID. ID: **402**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM3_SETTINGUP_WAITINGFORSTART

Event for *CollectioneventID. ID: **403**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM4_SETTINGUP_PROCESSING

Event for *CollectioneventID. ID: **404**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM5_WAITINGFORSTART_PROCESSING

Event for *CollectioneventID. ID: **405**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM6_PROCESSING_PROCESSCOMPLETE

Event for *CollectioneventID. ID: **406**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM7_PROCESSCOMPLETE_NOSTATE

Event for *CollectioneventID. ID: **407**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM8_EXECUTING_PAUSING

Event for *CollectioneventID. ID: **408**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM9_PAUSING_PAUSED

Event for *CollectioneventID. ID: **409**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM10_PAUSE_EXECUTING

Event for *CollectioneventID. ID: **410**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM11_EXECUTING_STOPPING

Event for *CollectioneventID. ID: **411**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM12_PAUSE_STOPPING

Event for *CollectioneventID. ID: **412**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM13_EXECUTING_ABORTING

Event for *CollectioneventID. ID: **413**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM14_STOPPING_ABORTING

Event for *CollectioneventID. ID: **414**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM15_PAUSE_ABORTING

Event for *CollectioneventID. ID: **415**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM16_ABORTING_NOSTATE

Event for *CollectioneventID. ID: **416**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM17_STOPPING_NOSTATE

Event for *CollectioneventID. ID: **417**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### PJSM18_QUEUED_NOSTATE

Event for *CollectioneventID. ID: **418**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 PRJobID | 1201 | String | Yes | A | * E40 ObjID identifier of the current ProcessJob
 PRJobState | 1202 | Integer | Yes | U1 | * E40 process job enumerated state, 0=queued/pooled, 1=setting up,2=waiting, 3=processing, 4=complete, 5=na, 6=pausing, 7=paused, 8=stopping, 9=aborting, 10=stopped, 11=aborted

### CJSM1_NOSTATE_QUEUED

Event for *CollectioneventID. ID: **501**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 1231 | String | Yes | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | Yes | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED

### CJSM2_QUEUED_NOSTATE

Event for *CollectioneventID. ID: **502**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 1231 | String | Yes | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | Yes | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED

### CJSM3_QUEUED_SELECTED

Event for *CollectioneventID. ID: **503**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 1231 | String | Yes | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | Yes | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED

### CJSM4_SELECTED_QUEUED

Event for *CollectioneventID. ID: **504**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 1231 | String | Yes | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | Yes | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED

### CJSM5_SELECTED_EXECUTING

Event for *CollectioneventID. ID: **505**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 1231 | String | Yes | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | Yes | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED

### CJSM6_SELECTED_WAITINGFORSTART

Event for *CollectioneventID. ID: **506**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 1231 | String | Yes | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | Yes | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED

### CJSM7_WAITINGFORSTART_EXECUTING

Event for *CollectioneventID. ID: **507**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 1231 | String | Yes | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | Yes | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED

### CJSM8_EXECUTING_PAUSED

Event for *CollectioneventID. ID: **508**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 1231 | String | Yes | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | Yes | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED

### CJSM9_PAUSED_EXECUTING

Event for *CollectioneventID. ID: **509**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 1231 | String | Yes | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | Yes | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED

### CJSM10_EXECUTING_COMPLETED

Event for *CollectioneventID. ID: **510**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 1231 | String | Yes | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | Yes | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED

### CJSM11_ACTIVE_COMPLETED

Event for *CollectioneventID. ID: **511**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 1231 | String | Yes | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | Yes | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED

### CJSM12_ACTIVE_COMPLETED

Event for *CollectioneventID. ID: **512**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 1231 | String | Yes | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | Yes | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED

### CJSM13_COMPLETED_NOSTATE

Event for *CollectioneventID. ID: **513**
#### *Event Properties*

Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------: | :-------- | :-----------
 CtrlJobID | 1231 | String | Yes | A | * Identifier of the ControlJob
 CtrlJobState | 1232 | Integer | Yes | U1 | * E94 Control Job Attribute, the state of the current Control Job, 0=QUEUED, 1=SELECTED, 2=WAITING FOR START, 3=EXECUTING, 4=PAUSED, 5=COMPLETED

### Alarm1002Cleared

Event for *CollectioneventID. ID: **3004**

*(this event has no properties)*

### Alarm100010001Cleared

Event for *CollectioneventID. ID: **10001001**

*(this event has no properties)*

Commands
========

### LOCAL

Command for Set Equipment to Online Local. ID: **LOCAL**

*(this command has no parameters)*

### REMOTE

Command for Set Equipment to Online Remote. ID: **REMOTE**

*(this command has no parameters)*

### LOCK_POD

Command for Locks a pod on a smith loadport. ID: **LOCK_POD**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORTID | Yes | U1 | 

### UNLOCK_POD

Command for Unlocks a pod on a smith loadport. ID: **UNLOCK_POD**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORTID | Yes | U1 | 

### LOAD_POD

Command for Loads a pod on a smith loadport. ID: **LOAD_POD**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORTID | Yes | U1 | 

### UNLOAD_POD

Command for Unloads a pod on a smith loadport. ID: **UNLOAD_POD**

#### *Command Parameters*

Name          | Is Mandatory | Equipment Data Type | Description
:------------ | :- | :-------- | :----------
 PORTID | Yes | U1 | 
