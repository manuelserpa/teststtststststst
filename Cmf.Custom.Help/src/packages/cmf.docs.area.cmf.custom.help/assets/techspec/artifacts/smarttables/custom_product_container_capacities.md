# CustomProductContainerCapacities

## Overview

SmartTable used to resolve the source and target capacity to be used as maximum of positions available on the container.

## Table Columns

The following table describes the table columns:

| Name           | Is Key | Is Mandatory | Data Type | Reference                   | Description |
| :------------- | :----: | :----------: | :-------- | :-------------------------- | :---------- |
| Product        |  Yes   |      No      | NVarChar  | EntityType Entity Type Name |             |
| ProductGroup   |  Yes   |      No      | NVarChar  | EntityType Entity Type Name |             |
| SourceCapacity |   No   |     Yes      | Int       | None                        |             |
| TargetCapacity |   No   |     Yes      | Int       | None                        |             |

## Precedence Keys

The following table lists the precedence keys applicable for this Smart Table:

| Order | Precedence Keys |
| :---: | :-------------- |
|   1   | Product         |
|   2   | ProductGroup    |
