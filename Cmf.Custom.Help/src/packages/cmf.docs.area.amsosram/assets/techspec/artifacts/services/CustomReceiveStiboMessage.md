# Custom Receive Stibo Message

## Overview

Service to receive a Stibo Message and create an **Integration Entry** accordingly.

<Service Description>

## Input Object

The table below describes the input parameters for the service.

| Name         | Type   | Description              |
| :----------- | :----: | :----------------------- |
| MessageType  | string | Type of the Integration  |
| Message      | string | Message to be integrated |

## Output Object

The table below describes the output parameters for the service.

| Name   | Type             | Description               |
| :----- | :--------------: | :------------------------ |
| Result | IntegrationEntry | Integration Entry Created |

## Pre Conditions

* **MessageType** needs to be configured in the MessageType Lookup Table.

## How it works

The system validates the received message from the Stibo and creates  the **Integration Entry** with the required information to after be processed.

## Assumptions

* N/A.