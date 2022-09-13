# Custom Material Nice Label Print Context

## Overview
Used to generate and print labels on Nice Label Printer.

## Table Columns
The following table describes the table columns:

| Name            | Is Key | Is Mandatory | Data Type | Reference        | Description             |
| :-------------- | :----: | :----------: | :-------- | :--------------- | :---------------------- |
| Step            | true   | false        | String    | EntityType       | Step Reference          |
| LogicalFlowPath | true   | false        | String    | None             | The Logical Flow Path   |
| Product         | true   | false        | String    | EntityDefinition | Product Reference       |
| ProductGroup    | true   | false        | String    | EntityDefinition | Product Group Reference |
| Flow            | true   | false        | String    | EntityDefinition | Flow Reference          |
| Material        | true   | false        | String    | EntityType       | Material Reference      |
| MaterialType    | true   | false        | String    | LookupValue      | Material Type           |
| Resource        | true   | false        | String    | EntityType       | Resource Reference      |
| ResourceType    | true   | false        | String    | LookupValue      | Resource Type           |
| Model           | true   | false        | String    | LookupValue      | Resource Model          |
| Operation       | true   | false        | String    | LookupValue      | Operation Name          |
| Printer         | false  | true         | String    | None             | Printer Name            |
| Label           | false  | true         | String    | None             | Label                   |
| Quantity        | false  | true         | Integer   | None             | Quantity                |
| IsEnabled       | false  | true         | Boolean   | None             | Enabled                 |

## Precedence Keys
The following table lists the precedence keys applicable for this Smart Table:

| Order | Precedence Keys                                                  |
| :---: | :--------------------------------------------------------------- |
| 1     | Step + Material + Operation                                      |
| 2     | Step + Product + LogicalFlowPath + MaterialType + Operation      |
| 3     | Step + Product + Flow + MaterialType + Operation                 |
| 4     | Step + ProductGroup + LogicalFlowPath + MaterialType + Operation |
| 5     | Step + ProductGroup + Flow + MaterialType + Operation            |
| 6     | Step + Product + LogicalFlowPath + Operation                     |
| 7     | Step + Product + Flow + Operation                                |
| 8     | Step + Product + MaterialType + Operation                        |
| 9     | Step + Product + Resource + Operation                            |
| 10    | Step + Product + ResourceType + Operation                        |
| 11    | Step + Product + Model + Operation                               |
| 12    | Step + Product + Operation                                       |
| 13    | Step + ProductGroup + LogicalFlowPath + Operation                |
| 14    | Step + ProductGroup + Flow + Operation                           |
| 15    | Step + ProductGroup + MaterialType + Operation                   |
| 16    | Step + ProductGroup + Resource + Operation                       |
| 17    | Step + ProductGroup + ResourceType + Operation                   |
| 18    | Step + ProductGroup + Model + Operation                          |
| 19    | Step + ProductGroup + Operation                                  |
| 20    | Step + MaterialType + LogicalFlowPath + Operation                |
| 21    | Step + MaterialType + Flow + Operation                           |
| 22    | Step + MaterialType + Resource + Operation                       |
| 23    | Step + MaterialType + ResourceType + Operation                   |
| 24    | Step + MaterialType + Model + Operation                          |
| 25    | Step + MaterialType + Operation                                  |
| 26    | Step + Resource + Operation                                      |
| 27    | Step + ResourceType + Operation                                  |
| 28    | Step + Model + Operation                                         |
| 29    | Step + LogicalFlowPath + Operation                               |
| 30    | Step + Flow + Operation                                          |
| 31    | Step + Operation                                                 |