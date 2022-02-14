# MaterialIn

## Overview

This performs the Material TrackIn into a resource.

## Input Object

The table below describes the input parameters for the service

| Name | Type | Description |
| :--- | :----: | :---------- |
| MaterialName | String | The Material Name |
| ResourceName | String | The Resource Name |
| ContainerName | String | The COntainer Name |
| SubResourceOrder | Int | The Sub Resource Order |


## Output Object

The table below describes the output parameters for the service

| Name | Type | Description |
| :--- | :----: | :---------- |
| Material | Material | Reference to Tracked In Material |

## Pre Conditions

* MaterialName or ContainerId is not null.
* ResourceName is not null.
* Material must exist and should not be in *InProcess* state, neither terminated.
* Resource must exist.

## How it works

When a MaterialName and a resource is sent to the service the Material is tracked in the resource. If a Container Name is sent then the Parent Material is tracked in in the resource.

## Assumptions

* Resource, Sub Resources and Steps are properly configured for Sub-Material tracking.
