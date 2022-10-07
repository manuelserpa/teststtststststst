# CustomSorterJobDefinitionContext

## Overview
Used to resolve the CustomSorterJobDefinition for the specific Sorter context

## Table Columns
The following table describes the table columns:

| Name | Is Key | Is Mandatory | Data Type | Reference | Description |
| :--- | :----: | :----------: | :-------- | :-------- | :---------- |
|Step | Yes | Yes | String | Step | Step Reference |
|Product | Yes | No | String | Product | Product Reference |
|ProductGroup | Yes | No | String | ProductGroup | Product Group Reference |
|Flow | Yes | No | String | Flow | Flow Reference |
|Material | Yes | No | String | Material | Material Reference |
|MaterialType | Yes | No | String | MaterialType | Material Type |
|CustomSorterJobDefinition | No | Yes | String | CustomSorterJobDefinition | Custom Sorter Job Definition |

## Precedence Keys
The following table lists the precedence keys applicable for this Smart Table:

| Order | Precedence Keys |
| :---: | :-------------- |
| 1 | Step+Material|
| 2 | Step+Product+Flow+MaterialType |
| 3 | Step+ProductGroup+Flow+MaterialType |
| 4 | Step+Product+Flow |
| 5 | Step+Product+MaterialType |
| 6 | Step+Product |
| 7 | Step+ProductGroup+Flow |
| 8 | Step+ProductGroup+MaterialType |
| 9 | Step+ProductGroup |
| 10 | Step+Flow+MaterialType |
| 11 | Step+MaterialType |
| 12 | Step+Flow |
| 13 | Step |
