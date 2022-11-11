# CustomRetrieveAttributesForMaterial

## Overview

Service to return a(n) *Material(s)* attributes and it's *SubMaterials* attributes.

## Input Object

The table below describes the input parameters for the service

| Name | Type | Description |
| :--- | :----: | :---------- |
| MaterialName | MaterialCollection | Material(s) name(s)|
| AttributeName | AttributeCollection | Attribute(s) name(s)|
| IncludeSubMaterial | bool | Include submaterials or not |

Parameters are taken in as an XML.

## Output Object

The table below describes the output parameters for the service

| Name | Type | Description |
| :--- | :----: | :---------- |
| Result | MaterialCollcetion | Material name(s) with the Attributes |

The output is given as an XML.

## Pre Conditions

* N/A

## How it works

The system processes the input XML into a datastructure and loads the *Materials* and it's attributes to the Result XML.

## Assumptions

* N/A