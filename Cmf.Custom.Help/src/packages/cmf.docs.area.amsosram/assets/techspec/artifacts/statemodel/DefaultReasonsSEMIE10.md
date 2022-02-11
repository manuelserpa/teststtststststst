# Default Reasons for SEMI E10

## Overview
For the State Model **SEMI E10** were added new transitions and default reasons.

The following table indicates the Lookup Table and Default Reason used by State Transition:

| State Transition                   | Lookup Table                                                                                                                 | Default Reason               |
| :--------------------------------- | :--------------------------------------------------------------------------------------------------------------------------- | :--------------------------- |
| Engineering to Engineering         | [ReasonsEngineeringToEngineering](/AMSOsram/tecspecs>artifacts>lookuptables>ReasonsEngineeringToEngineering)                 | ENE - Engenineering          |
| Scheduled to NonScheduled          | [ReasonsNonScheduledToNonScheduled](/AMSOsram/tecspecs>artifacts>lookuptables>ReasonsNonScheduledToNonScheduled)             | NSU - Unworked Time          |
| Productive to Productive           | [ReasonsProductiveToProductive](/AMSOsram/tecspecs>artifacts>lookuptables>ReasonsProductiveToProductive)                     | PRP - Production             |
| ScheduledDown to ScheduledDown     | [ReasonsScheduledDownToScheduledDown](/AMSOsram/tecspecs>artifacts>lookuptables>ReasonsScheduledDownToScheduledDown)         | SDP - Preventive Maintenance |
| Standby to Standby                 | [ReasonsStandbyToStandby](/AMSOsram/tecspecs>artifacts>lookuptables>ReasonsStandbyToStandby)                                 | SBI - Idle                   |
| UnscheduledDown to UnscheduledDown | [ReasonsUnscheduledDownToUnscheduledDown](/AMSOsram/tecspecs>artifacts>lookuptables>ReasonsUnscheduledDownToUnscheduledDown) | UDM - Wait for Maintenance   |