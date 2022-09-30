# CustomReportDataToFDC

## Overview

Dee action is triggered to create an integration entry with the material data to send to FDC.

## Action Groups

- MaterialManagement.MaterialManagementOrchestration.AbortMaterialsProcess.Post
- MaterialManagement.MaterialManagementOrchestration.ComplexTrackInMaterials.Post
- MaterialManagement.MaterialManagementOrchestration.ComplexTrackOutMaterials.Post

## Pre Conditions

- **/amsOSRAM/FDC/Active/** configuration must be set with **true**
- **FDCCommunication** Resource attribute must be set with **true**

## Action

This DEE Action will first validate if the configuration FDC Active configured at /amsOSRAM/FDC/Active/ is enabled and if the Resource requires data to be sent to FDC (FDCCommunication attribute).

In that case and based on the material system state, a new integration entry will be created with the material data serialized in order to be processed and sent to Onto FDC.