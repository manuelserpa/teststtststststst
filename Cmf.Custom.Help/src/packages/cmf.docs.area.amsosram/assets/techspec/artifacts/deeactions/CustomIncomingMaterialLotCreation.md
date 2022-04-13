# CustomIncomingMaterialLotCreation

## Overview

DEE action to create or update lot incoming from ERP.

## Action Groups

* NA

## Pre Conditions

* The message will send the information of single lot instead of multiples.

* If a lot or wafer field is invalid the transaction will be aborted. 

* The system will throw an exception if the incoming lot exists on the system and it´s on a different step and flow.

* The system will throw an exception if no certificate data collection or at least one wafer has no Edc Data.

* The system will not throw an exception if no certificate data collection and all wafers have no Edc Data.

* The system will throw an exception if the next iteration has different wafers compared to the first iteration.


## Action

The system will have a data collection configured with one or more parameters associated with a limit set that has a lower error limit or upper error limit. Upon execution, this action will identify if it is to create the lot (**Material**) when the material does not exist on the system or to update in case the lot already exists. 
After the creation or update of the material a **Data Collection Instance** will be opened for each wafer data in the ERP Message. These values are later compared with the limits defined on the limit set and if at least one of the values is out of the limits the lot will be put on hold with the reason defined on the config: /Cmf/Guis/Configuration/Material/IncomingLotAutoHoldReason.
