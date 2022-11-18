# CustomUpdateComposeCustomSorterJobDefinition

## Overview

Responsible for updating the Compose CustomSorterJobDefinition based on BOMs.

## Action Groups

* BusinessObjects.BOM.MakeEffective.Pre
* BusinessObjects.BOM.MakeEffective.Post

## Pre Conditions

* There is a BOM defined that has BOM products associated to it that can be used for the movement list.
* The BOM is set to use for lot compose by setting the **IsForLotCompose** attribute to true.
* The BOM that contains a value for the **StartingCarrierType** attribute.

## Action

When a BOM version is set as effective, this DEE will be triggered and validates if the BOM is set to be used for lot compose using the **IsForLotCompose** attribute and if the **StartingCarrierType** attribute contains a value.

- If a context for the previous BOM effective version exists in **CustomSorterJobDefinitionContext** SmartTable **it will be updated with the new CustomSorterJobDefinition**.
- If it is not configured, a warning message will be shown, informing that the CustomSorterJobDefinition **must be manually defined in CustomSorterJobDefinitionContext** SmartTable.
