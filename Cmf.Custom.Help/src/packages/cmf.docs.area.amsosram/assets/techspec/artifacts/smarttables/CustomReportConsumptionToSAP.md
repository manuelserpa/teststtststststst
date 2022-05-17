# CustomReportConsumptionToSAP

## Overview
Used to identify the Storage Location

## Table Columns
The following table describes the table columns:

| Name              | Is Key | Is Mandatory | Data Type | Reference                     | Description   |
| :---------------- | :----: | :----------: | :-------- | :---------------------------- | :------------ |
| Step              |   Yes  |      No      |   String  | EntityType Entity Type Name   |               |               
| Product           |   Yes  |      No      |   String  | EntityType Entity Type Name   |               |
| Flow              |   Yes  |      No      |   String  | EntityType Entity Type Name   |               |
| MaterialType      |   Yes  |      No      |   String  | EntityType Entity Type Name   |               | 
| ProductGroup      |   Yes  |      No      |   String  | EntityType Entity Type Name   |               | 
| StorageLocation   |   No   |      Yes     |   String  |              None             |               | 

## Precedence Keys
The following table lists the precedence keys applicable for this Smart Table:

| Order | Precedence Keys                       |
| :---: | :------------------------------------ |
|   1   | Step+Product+Flow+MaterialType        |
|   2   | Step+Flow+ProductGroup+MaterialType   |
|   3   | Step+Product+Flow                     |
|   4   | Step+Flow+ProductGroup                |
|   5   | Step+Flow+MaterialType                |
|   6   | Step+Product                          |
|   7   | Step+Flow                             |
|   8   | Step                                  |