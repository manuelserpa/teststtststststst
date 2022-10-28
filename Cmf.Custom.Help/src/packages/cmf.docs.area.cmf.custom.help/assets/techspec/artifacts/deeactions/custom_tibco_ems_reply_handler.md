# CustomTibcoEMSReplyHandler

## Overview

DEE Action to handle the reply send from Tibco EMS.

## Action Groups

- BusinessObjects.DataCollectionInstance.PerformImmediate.Post
- BusinessObjects.DataCollectionInstance.Close.Post

## Pre Conditions

- N/A

## Action

This DEE Action is responsible for handling all the replies from Tibco EMS. It is expected that we should have as Input:

- ReplyMessage: The message sent by Tibco (in XML format)
- Context: Additional information regarding the request and information needed to handle the reply

It must be provided on the Context:

- Subject: Subject used on the request

### Subject: CustomReportEDCToSpace

To handle **CustomReportEDCToSpace** as expected we should provide:

- ProtocolInstance: Name of protocol instanced opened when closing the DataCollection
- Lot: Name of material where the DataCollection was performed
- ActionGroup: Name of ActionGroup who triggered the DEE **CustomReportEDCToSpaceHandler**

In this case will check if the Protocol instance is Active, and if thats the case it will parse the XML response and check if the reply has all the requirements to be considered valid.

If everything is OK, it will close the provided Protocol instance and if the MoveNext was blocked it will perform the MoveNext on the provided lot (Material).
