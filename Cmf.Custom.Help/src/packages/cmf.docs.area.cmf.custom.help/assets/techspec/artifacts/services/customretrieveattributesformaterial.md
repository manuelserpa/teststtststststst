# CustomRetrieveAttributesForMaterial

## Overview

Service to return one or more *Materials* attributes and it's *SubMaterials* attributes.

## Input Object

The table below describes the input parameters for the service

| Name | Type | Description |
| :--- | :----: | :---------- |
| MaterialList | string | Material names|
| AttributeList | string | Attribute names|
| IncludeSubMaterials | string | Include submaterials or not |
| SubMaterialAttributeList | string | Names of the Submaterials Attributes |

## Output Object

The table below describes the output parameters for the service

| Name | Type | Description |
| :--- | :----: | :---------- |
| ResultXML | string | Material names with the Attributes |

## Pre Conditions

* The requested materials must exist in the system.
* The requested attributes of the materials must have a value assigned to them.

## How it works

The system processes the input and gradually fills up the output datastructure. From this the system serializes the XML output that is returned as a string.

* When a **MaterialList** parameter is sent to the service, the information of the Material will be returned.

* When a **AttributeList** parameter is sent to the service, the information of the Material's attributes will be returned.

* When a **IncludeSubMaterials** parameter is sent to the service, the information of the Material's submaterials will be returned.

* When a **SubMaterialAttributeList** parameter is sent to the service, the information of the Submaterials attributes will be returned.