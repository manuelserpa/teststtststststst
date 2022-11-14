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
| Result | string | Material names with the Attributes |

## Pre Conditions

* N/A

## How it works

The system processes the input and gradually fills up the *CustomGetMaterialAttributesDS* datastructure. From the *CustomGetMaterialAttributesDS* datastructure the system serializes the XML output that is returned as a string.

## Assumptions

* N/A