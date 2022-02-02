# Custom Receive ERP Message

## Overview

Service to receive an ERP Message and create an **Integration Entry** accordingly.

## Input Object

The table below describes the input parameters for the service.

| Name        | Type   | Description              |
| :---------- | :----: | :----------------------- |
| MessageType | string | Type of the Integration  |
| Message     | String | Message to be integrated |

## Output Object

The table below describes the output parameters for the service.

| Name   |       Type       | Description               |
| :----- | :--------------: | :------------------------ |
| Result | IntegrationEntry | Integration Entry Created |

## Pre Conditions

* **Message Type** needs to be configured in the MessageType Lookup Table.

## How it works

The system validates the received message from the ERP and creates the **Integration Entry** with the required information to after be processed.

## Assumptions

* N/A.