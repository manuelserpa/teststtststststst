# UndockContainer

## Overview

This performs the undock of a Container from a Resource Load Port.

## Input Object

The table below describes the input parameters for the service

| Name       | Type      | Description              |
| :---       | :----:    | :----------              |
| Container  | Container | Container to be undocked |

## Output Object

The table below describes the output parameters for the service

| Name       | Type      | Description            |
| :---       | :----:    | :----------            |
| Container  | Container | Undocked Container     |
| Resource   | Resource  | Resource Load Port     |

## Pre Conditions

* Container must be docked.

## How it works

The Container is undocked from the Resource Load Port.
