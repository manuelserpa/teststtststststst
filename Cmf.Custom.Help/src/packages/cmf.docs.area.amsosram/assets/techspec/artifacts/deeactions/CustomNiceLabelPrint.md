# Custom Nice Label Print

## Overview

DEE Action to be triggered on material track out to send retrive and send information for the nice label printing.

## Action Groups

* MaterialManagement.MaterialManagementOrchestration.ComplexTrackInMaterials.Post
* MaterialManagement.MaterialManagementOrchestration.ComplexTrackOutMaterials.Post
* MaterialManagement.MaterialManagementOrchestration.MoveMaterialsToNextStep.Post

## Pre Conditions

* Smart Table **[CustomMaterialNiceLabelPrintContext](/AMSOsram/techspec>artifacts>smarttables>CustomMaterialNiceLabelPrintContext)** must have the label information to be printed.

## Action

Upon execution this DEE action, using the available information related to the current material, will resolve the information registered on the Smart Table **[CustomMaterialNiceLabelPrintContext](/AMSOsram/techspec>artifacts>smarttables>CustomMaterialNiceLabelPrintContext)**:
* Label - The label to be printed;
* Printer - The printer to be used;
* Quantity - Quantity of labels to print;
* Zeile1 -
* Zeile2 -
* Zeile3 -
* Zeile4 -
* Zeile5 -
* Zeile6 -
* Zeile7 -
* Zeile5RB -
* Barcode -
