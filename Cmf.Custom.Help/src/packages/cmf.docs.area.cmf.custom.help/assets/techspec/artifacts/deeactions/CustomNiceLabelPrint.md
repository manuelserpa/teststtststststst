# Custom Nice Label Print

## Overview

DEE Action to be triggered on material track out to send retrive and send information for the nice label printing.

## Action Groups

* MaterialManagement.MaterialManagementOrchestration.ComplexTrackInMaterials.Post
* MaterialManagement.MaterialManagementOrchestration.ComplexTrackOutMaterials.Post
* MaterialManagement.MaterialManagementOrchestration.MoveMaterialsToNextStep.Post

## Pre Conditions

* Smart Table **[CustomMaterialNiceLabelPrintContext](/cmf.custom.help/cmf.custom.help.techspec>cmf.custom.help.artifacts>cmf.custom.help.smarttables>CustomMaterialNiceLabelPrintContext)** must have the label information to be printed.

## Action

Upon execution this DEE action, using the available information related to the current material, will resolve the information registered on the Smart Table **[CustomMaterialNiceLabelPrintContext](/cmf.custom.help/cmf.custom.help.techspec>cmf.custom.help.artifacts>cmf.custom.help.smarttables>CustomMaterialNiceLabelPrintContext)**:
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