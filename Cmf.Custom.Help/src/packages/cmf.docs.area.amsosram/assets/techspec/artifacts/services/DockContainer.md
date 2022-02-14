# DockContainer

## Overview

This performs the dock of a Container to a Resource Load Port.

## Input Object

The table below describes the input parameters for the service

| Name       | Type      | Description            |
| :---       | :----:    | :----------            |
| Container  | Container | Container to be docked |
| Resource   | Resource  | Resource Load Port     |

## Output Object

The table below describes the output parameters for the service

| Name       | Type      | Description            |
| :---       | :----:    | :----------            |
| Container  | Container | Docked Container       |
| Resource   | Resource  | Resource Load Port     |

## Pre Conditions

* Container must exist.
* Resource must exist and its Processing Type must be LoadPort.

## How it works

The Container is docked to the Resource Load Port.
