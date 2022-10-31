# CustomReportEDCToSpaceHandler

## Overview

DEE action to validate DataCollection and create a XML message to be sent to Space system.

## Action Groups

- BusinessObjects.DataCollectionInstance.PerformImmediate.Post
- BusinessObjects.DataCollectionInstance.Close.Post

## Pre Conditions

- N/A

## Action

This DEE Action will first validate if the values of the **Data Collection Points** are inside the established **Data Collection Limit Set**.

- If there is any value outside of the limit the main Lot is put on hold with reason defined on the Configuration */Cmf/Guis/Configuration/Material/IncomingLotAutoHoldReason*
- Otherwise, if the limits are respected a **Protocol** is opened.

Finally, an XML message will be created to be sent to the Space system by subscribing to the CustomReportEDCToSpace subject in the Message Bus.

If a *ReplyTo* is set in the GenericTable **CustomTibcoEMSGatewayResolver** for the topic/queue CustomReportEDCToSpace a listener will be created to wait for a reply for the published request. As soon Tibco Gateway as a reply it will call a DEE **CustomTibcoEMSReplyHandler** to handle it accordingly.
