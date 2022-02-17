# MaterialOut

## Overview

This service performs Material or Sub Material TrackOut considering if Materials are associated with a container.

## Input Object

The table below describes the input parameters for the service

| Name | Type | Description |
| :--- | :----: | :---------- |
| MaterialName | String | The Material Name |
| ResourceName | String | The Resource Name |
| CarrierId    | String | The Carrier Name  |

## Output Object

The table below describes the output parameters for the service

| Name | Type | Description |
| :--- | :----: | :---------- |
| MaterialName | String | The Material Name |
| ResourceName | String | The Resource Name |

## Pre Conditions

* MaterialName or CarrierId is required.
* ResourceName is required.
* Material must exist and should not be terminated.
* Resource must exist and allow the Sub-Material tracking when Tracking Out Wafers.
* The Step where the Material is must have the *"Sub-Material Track State Depth"* property set to a value higher than 0 when Tracking Out Wafers.

## How it works

The Track Out will be performed according to the following input precedences:

1) MaterialName filled 

    * When the Material Name is available and the Material is in *InProcess* state then it will be tracked out.

2) MaterialName is empty and CarrierId is filled

    * When the CarrierId is available and Material Name is empty then the TopMost Material will be tracked out and moved to the next step.

## Assumptions

* Resource, Sub Resources and Steps are properly configured for Sub-Material tracking.
