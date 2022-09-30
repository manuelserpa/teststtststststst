# Custom Receive Stibo Message

## Overview

Service to receive a Stibo Message and create an **Integration Entry** accordingly.

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

The system validates the message received and creates one **Integration Entry** for asynchronous processing.

This integration entries will have different processing rules depending on the **source system**, **target system** and **message type**.

## Assumptions

* N/A.