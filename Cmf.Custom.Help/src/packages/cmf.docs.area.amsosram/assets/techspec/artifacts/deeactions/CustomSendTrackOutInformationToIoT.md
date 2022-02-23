# CustomSendTrackOutInformationToIoT

## Overview

DEE action to Trigger IoT call to send the Materials Track Out related information.

## Action Groups

* MaterialManagement.MaterialManagementOrchestration.ComplexTrackOutMaterials.Post

## Pre Conditions

* The Resource Automation Mode is set to Online.
* The Material being Tracked Out is the Top-Most (this rule must not execute/send request on sub-material track out).

## Action
This DEE sends the Material Id and Name to IoT.

It will send a synchronous request with request type "TrackOut", considering a timeout defined in the configuration path /Cmf/Custom/Automation/GenericRequestTimeout.