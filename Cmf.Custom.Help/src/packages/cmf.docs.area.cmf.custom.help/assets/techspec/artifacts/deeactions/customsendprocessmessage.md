# CustomSendProcessMessage

## Overview

DEE action to be triggered by the Integration Entry Handler to process Integration Entries and send Inform Goods Issue message to ERP.

## Action Groups

* N/A

## Pre Conditions

* Configuration **/Cmf/Custom/ERP/WebServiceEndpoint/** must be confured with the endpoint provided by OSRAM
* Configuration **/Cmf/Custom/ERP/Credentials/Username/** must be configured with the proper username for the web service
* Configuration **/Cmf/Custom/ERP/Credentials/Password/** must be configured with the proper password for the web service
* To execute this action the **MessageType** must be **CustomPerformConsumption**

The new entrie for the **IntegrationHandlerResolution** is the following:

| FromSystem | ToSystem | MessageType              | HandlerType               | ActionName                | ErrorHandlingActionName | IsEnabled |
| :--------- | :------: | :---------:              | :----------               | :---------                | :---------------------- | :-------: |
| MES        | ERP      | CustomPerformConsumption | GenericIntegrationHandler | CustomSendProcessMessage  | -                       | Yes       |

## Action
When there is an Integration Entry to be processed, this DEE action will be executed and process the Integration Entry message in order to send a new request to the ERP WebService.
The request will only be sent if the following configurations are set:
* **/Cmf/Custom/ERP/WebServiceEndpoint/**
* **/Cmf/Custom/ERP/Credentials/Username/**
* **/Cmf/Custom/ERP/Credentials/Password/** values are filled.

In case of an error with calling the web service endpoint, or because of missing configurations the action will send an e-mail to a distribution list that is stored in the configuration: **/Cmf/Custom/ERP/DistributionList/**.