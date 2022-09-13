# Nice Label Print

## Requirement Specification
When performing a track out a DEE action will be executed resolving the information present on the Smart Table

## Design Specification

### Relevant Artifacts
The table below describes the artifacts for this feature:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------
[CustomMaterialNiceLabelPrintContext](/cmf.custom.help/cmf.custom.help.techspec>cmf.custom.help.artifacts>cmf.custom.help.smarttables>CustomMaterialNiceLabelPrintContext) | Smart Table | Yes | -- |


### How it works
Upon execution, this feature collects the available information to resolve the Smart Table **[CustomMaterialNiceLabelPrintContext](/cmf.custom.help/cmf.custom.help.techspec>cmf.custom.help.artifacts>cmf.custom.help.smarttables>CustomMaterialNiceLabelPrintContext)**.  
The information that results is attached to the other required fields to later be sent to the printer.
The result of this functionality should be a list containing the following fields:
* Label - The label to be printed;
* Printer - The printer to be used;
* Quantity - Quantity of labels to print;
* LotName - Material Name;
* LotAlias
* ProductName
* ProductDesc
* ProductType
* Product_Type
* ProductGroupName
* ProductGroup_Type
* FlowName
* BatchName
* ContainerName
* ExperimentName
* ProductionOrder
* LotOwner                                        
* ResourceName
* WaferLogicalName
* WaferSlotPosition
* WaferCrystalName
* LotWaferCount
* LotPrimaryQty
* LotSecundaryQty
* WaferPrimaryQty
* WaferSecundaryQty
* Lot_Type

The first three fields are configured on the Smart Table mentioned [above](#relevant-artifacts).  


### Assumptions


## Work items

The table below describes de user stories that affect the current functionality

| User Story | Type       | Title                                                  | Description |
| :--------- | :--------- | :----------------------------------------------------- | :---------- |
| 154257     | User Story | Trigger the NiceLabel printing on Lot Start (TrackOut) |             |