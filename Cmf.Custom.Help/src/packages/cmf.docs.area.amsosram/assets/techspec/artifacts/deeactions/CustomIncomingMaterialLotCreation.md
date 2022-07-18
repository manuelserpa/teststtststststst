# CustomIncomingMaterialLotCreation

## Overview

DEE action to create or update (on hold) lot incoming from ERP.

## Action Groups

* NA

## Pre Conditions

These are the preconditions for the DEE to be executed:

* The mandatory fields for the Lot or Wafers are valid.
* The incoming Lot exists in the system and it's on the same Step or Flow.
* The incoming Lot exists in the system and Incoming Wafers matches the existing.
* There a Data Collection in the system for the Context and incoming Wafers have EDC Data.

## Action

The system will have a Data Collection configured with one or more parameters associated with a limit set that has a Lower Error Limit or Upper Error Limit.

Upon execution, this action will identify if it is to create the lot (**Material**) when the material does not exist on the system or to update in case the lot already exists.

After the creation or update of the material a **Data Collection Instance** will be opened for each Wafer data in the ERP Message. These values are later compared with the limits defined on the limit set and if at least one of the values is out of the limits the lot will be put on hold with the reason defined on the config: /Cmf/Guis/Configuration/Material/IncomingLotAutoHoldReason.
