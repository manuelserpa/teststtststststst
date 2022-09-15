# GetFlowInformationForERP

## Overview

Service to provide Flow information to ERP.

## Input Object

The table below describes the input parameters for the service:

| Name        | Type   | Description             |
| :---------- | :----: | :---------------------- |
| ProductName | String | Product Name            |
| FlowName    | String | Flow Name               |
| FlowVersion | String | Flow Version (optional) |

## Output Object

The table below describes the output parameters for the service:

| Name      | Type   | Description                        |
| :-------- | :----: | :--------------------------------- |
| ResultXml | String | Returns Flow Details in XML format |

## Pre Conditions

* The input parameters ProductName or FlowName must have a value.
* If FlowName is provided the ProductName must be empty.

## How it works

The service returns information about the Product/Flow, in XML message, depending on the Input parameters are sent.

* When a **ProductName** parameter is sent to the service, the information of the Product and to the effective version of the associated Flow is returned.

* When a **FlowName** parameter is sent to the service without **FlowVersion**, the information associated to the effective version of the Flow is returned.

* When a **FlowName** and a **FlowVersion** parameters are sent to the service, the information associated to the specified version of the Flow is returned.
