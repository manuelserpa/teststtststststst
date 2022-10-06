# Default Reasons for SEMI E10

## Overview
For the State Model **SEMI E10** were added new transitions and default reasons.

The following table indicates the Lookup Table and Default Reason used by State Transition:

| State Transition                   | Lookup Table                                                                                                                 | Default Reason               |
| :--------------------------------- | :--------------------------------------------------------------------------------------------------------------------------- | :--------------------------- |
| Engineering to Engineering         | [ReasonsEngineeringToEngineering](/cmf.custom.help/techspec>artifacts>lookuptables>ReasonsEngineeringToEngineering)                 | ENE - Engineering          |
| Scheduled to NonScheduled          | [ReasonsNonScheduledToNonScheduled](/cmf.custom.help/techspec>artifacts>lookuptables>ReasonsNonScheduledToNonScheduled)             | NSU - Unworked Time          |
| Productive to Productive           | [ReasonsProductiveToProductive](/cmf.custom.help/techspec>artifacts>lookuptables>ReasonsProductiveToProductive)                     | PRP - Production             |
| ScheduledDown to ScheduledDown     | [ReasonsScheduledDownToScheduledDown](/cmf.custom.help/techspec>artifacts>lookuptables>ReasonsScheduledDownToScheduledDown)         | SDP - Preventive Maintenance |
| Standby to Standby                 | [ReasonsStandbyToStandby](/cmf.custom.help/techspec>artifacts>lookuptables>ReasonsStandbyToStandby)                                 | SBI - Idle                   |
| UnscheduledDown to UnscheduledDown | [ReasonsUnscheduledDownToUnscheduledDown](/cmf.custom.help/techspec>artifacts>lookuptables>ReasonsUnscheduledDownToUnscheduledDown) | UDM - Wait for Maintenance   |
