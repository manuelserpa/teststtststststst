# CustomReportEDCToSpaceHandler

## Overview

DEE action to validate DataCollection Post data

## Action Groups

*  DataCollectionManagement.DataCollectionInstanceManagementOrchestration.PostDataCollectionPoints.Post
*  DataCollectionManagement.DataCollectionInstanceManagementOrchestration.CloseDataCollectionInstance.Post
*  EdcManagement.DataCollectionManagement.ComplexPerformDataCollection.Post

## Pre Conditions

* N/A

## Action
Upon execution, this DEE Action will first validate if the values of the **Data Collection Points**are inside the established **Data Collection Limit Set**.  
Then if there is any value outside of the limit the main lot is put on hold with reason = *Out of Spec*.  
On the other hand, if the limits are respected a **Protocol** is opened. The config `/Cmf/Custom/Protocol/Space` defines the **Protocol** that should be opened.