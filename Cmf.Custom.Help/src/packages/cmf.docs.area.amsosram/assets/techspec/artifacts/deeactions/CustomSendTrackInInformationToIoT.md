# CustomSendTrackInInformationToIoT

## Overview

DEE action to Trigger IoT call to send the Materials TrackIn related information.

## Action Groups

* MaterialManagement.MaterialManagementOrchestration.ComplexTrackInMaterials.Post

## Pre Conditions

* The Resource Automation Mode is set to Online.
* The Material being Track In is the Top-Most (this rule must not execute/send request on sub-material tracking).

## Action
This DEE sends the following TrackIn information to IoT:

* Material and Sub-Materials Id, Name and StateModelState
* Container Id and Name
* Recipe Id, Name, Body Checksum and NameOnEquipment

In case of Material has no StateModel assigned, it will be set with the CustoMaterialStateModel when the StateModelState will be the "Setup".

It will send a synchronous request with a request type "TrackIn", considering a timeout defined in the configuration path /Cmf/Custom/Automation/TrackInTimeout.