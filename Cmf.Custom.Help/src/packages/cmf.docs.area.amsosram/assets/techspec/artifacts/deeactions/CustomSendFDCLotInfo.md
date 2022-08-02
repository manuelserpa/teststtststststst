# CustomSendFDCLotInfo

## Overview

DEE action to be triggered by the Integration Entry Handler to process Integration Entries and send Lot data to Onto FDC.

## Action Groups

* N/A

## Pre Conditions

* There must be an entry for each Transaction type, **LOTIN** and **LOTOUT**, in the Smart Table [IntegrationHandlerResolution](Help/UserGuide/administration>tables>system_smart_tables>integration_handler_resolution).

The new entries for the **IntegrationHandlerResolution** are the following:

| FromSystem | ToSystem | MessageType   | HandlerType               | ActionName                | ErrorHandlingActionName | IsEnabled |
| :--------- | :------: | :---------:   | :----------               | :---------                | :---------------------- | :-------: |
| MES        | OntoFDC  | LOTIN         | GenericIntegrationHandler | CustomSendFDCLotInfo      | -                       | Yes       |
| MES        | OntoFDC  | LOTOUT        | GenericIntegrationHandler | CustomSendFDCWaferInfo    | -                       | Yes       |


## Action

When there is an Integration Entry of transaction type **LOTIN** or **LOTOUT** to be processed, this DEE action will be executed and process the Integration Entry message in order to send the material data to Onto FDC.

The request will only be sent if the following configurations are set:
* **/AMSOsram/FDC/Active/** configuration must be set with **true**
* **/AMSOsram/FDC/Server/** configuration must be set with a valid server
* **/AMSOsram/FDC/Port/** configuration must be set with a valid port