# CustomSendAbortInformationToIoT

## Overview

DEE action to Trigger IoT call to send information about Aborted Materials.

## Action Groups

* MaterialManagement.MaterialManagementOrchestration.AbortMaterialsProcess.Post

## Pre Conditions

* The Resource Automation Mode is set to Online.
* The Material being aborted is the Top-Most (this rule must not execute/send request on sub-material abort).

## Action
This DEE sends the Material Id and Name to IoT.

It will send a synchronous request with request type "Abort", considering a timeout defined in the configuration path /Cmf/Custom/Automation/GenericRequestTimeout.
