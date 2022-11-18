# CustomCreateComposeSorterJobDefinition

## Overview

Responsible for create compose sorter job definitions based on BOMs.

## Action Groups

* BusinessObjects.MaterialCollection.TrackOut.Post

## Pre Conditions

* There is a BOM defined that has BOM products associated to it that can be used for the movement list.
* The BOM is set to use for lot compose by setting the **IsForLotCompose** attribute to true.
* The BOM that contains a value for the **StartingCarrierType** attribute.

## Action

When this DEE is triggered and a BOM with BOM products is provided, the system resolves the BOM context and creates a CustomSorterJobDefinition based on the BOM products in the BOM but only if the BOM has been set for lot compose (**IsForLotCompose** attribute) and contains a value for the **StartingCarrierType** attribute.

After creating the CustomSorterJobDefinition or if it already exists, the DEEs loads it and sets the **CustomSorterJobDefinition** on the **Input**.

**Note:** This DEE is called by:
- DEE **CustomUpdateComposeCustomSorterJobDefinition** when a BOM version is set as effective.
