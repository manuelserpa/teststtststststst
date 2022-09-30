# CustomGetResourceLoadPortsData

## Overview

This query returns the load ports and container related data for a resource.

## Input Parameters

The table below describes the input parameters for the query:

| Name          | Type          | Description           |
| :------       | :------       | :------               |
| SourceEntity  | Long      | Parent Resource Id    |
| ContainerResourceAssociationType | String | Resource association type |
| MainStateModelStateId   | Long      | Load Port State Model State Id |

## Output

The table below describes the output for the query:

| Name                                                                                      | Type          | Description                               |
| :------                                                                                   | :------       | :------                                   |
| Id                                                                                        | SubResource   | SubResource Id                            |
| Name                                                                                      | SubResource   | SubResource Name                          |
| SourceEntityId                                                                            | Resource      | Resource Id                               |
| SourceEntityName                                                                          | Resource      | Resource Name                             |
| TargetEntityId                                                                            | Resource      | Resource Id                               |
| TargetEntityName                                                                          | Resource      | Resource Name                             |
| TargetEntityLoadPortType                                                                  | Resource      | Resource LoadPortType                     |
| TargetEntityIsLoadPortInUse                                                               | Resource      | Resource IsLoadPortInUse attribute        |
| TargetEntityModifiedOn                                                                    | Resource      | Resource ModifiedOn                       |
| TargetEntityContainerResourceSourceEntityId                                               | Resource      | Resource Container Id                     |
| TargetEntityContainerResourceSourceEntityName                                             | Resource      | Resource Container Name                   |
| TargetEntityContainerResourceSourceEntityType                                             | Container     | Container Type                            |
| TargetEntityContainerResourceSourceEntityResourceAssociationType                          | Container     | Container ResourceAssociationType         |
| TargetEntityContainerResourceSourceEntityTotalPositions                                   | Container     | Container TotalPositions                  |
| TargetEntityContainerResourceSourceEntityUsedPositions                                    | Container     | Container UsedPositions                   |
| TargetEntityContainerResourceSourceEntityLot                                              | Container     | Container Lot attribute                   |
| TargetEntityContainerResourceSourceEntityProduct                                          | Container     | Container Product attribute               |
| TargetEntityContainerResourceSourceEntityTransportRequested                               | Container     | Container TransportRequested attribute    |
| TargetEntityContainerResourceSourceEntityMapContainerNeeded                               | Container     | Container MapContainerNeeded attribute    |
| TargetEntityContainerResourceSourceEntityMaterialContainerSourceEntityParentMaterialId    | Material      | Parent Material Id                        |
| TargetEntityContainerResourceSourceEntityMaterialContainerSourceEntityParentMaterialName  | Material      | Parent Material Name                      |
| TargetEntityMainStateModelStateId                                                         | Resource      | Resource MainStateModelStateId            |


## Filters

* Resource Id **equals to Resource Id input parameter**
* Resource ProcessingType **is equal to Load Port**
* Universal State **is active**
* Container Resource Association Type **equals to ContainerResourceAssociationType input parameter**
* MainStateModelStateId **equals to LoadPortStateModelStateId input parameter**
