# CustomIncomingMaterialLotCreation

## Overview

DEE action to create or update lot incoming from ERP.

## Action Groups

* NA

## Pre Conditions

These are the preconditions for the DEE to be executed:

* The message will send the information of single lot.
* If a lot or wafer field is invalid the transaction will be aborted.
* The system will throw an exception if the incoming lot exists on the system and it´s on a different Step and Flow.
* The system will throw an exception if the next iteration has different wafers compared to the first iteration.
* The system will throw an exception if no certificate Data Collection or at least one wafer has no EDC Data.
* The system will not throw an exception if no certificate Data Collection and all wafers have no EDC Data.

## Action

The system will have a Data Collection configured with one or more parameters associated with a limit set that has a Lower Error Limit or Upper Error Limit.

Upon execution, this action will identify if it is to create the lot (**Material**) when the material does not exist on the system or to update in case the lot already exists.

After the creation or update of the material a **Data Collection Instance** will be opened for each Wafer data in the ERP Message. These values are later compared with the limits defined on the limit set and if at least one of the values is out of the limits the lot will be put on hold with the reason defined on the config: /Cmf/Guis/Configuration/Material/IncomingLotAutoHoldReason.
