# CustomSendEventMessage

## Overview

DEE Action used to publish Lot event messages to MessageBus based on Material action. E.g.: Material.TrackIn, Material.TrackOut, Material.MoveNext.

## Action Groups

* BusinessObjects.MaterialCollection.Create.Post
* BusinessObjects.MaterialCollection.Dispatch.Post
* BusinessObjects.MaterialCollection.TrackIn.Post
* BusinessObjects.MaterialCollection.TrackOut.Post
* BusinessObjects.MaterialCollection.MoveToNextStep.Pre
* BusinessObjects.MaterialCollection.MoveToNextStep.Post
* BusinessObjects.MaterialCollection.Split.Post
* BusinessObjects.MaterialCollection.RecordLoss.Post
* BusinessObjects.MaterialCollection.RecordBonus.Post
* BusinessObjects.MaterialCollection.Hold.Post
* BusinessObjects.MaterialCollection.Terminate.Post
* BusinessObjects.Material.Release.Post
* BusinessObjects.Material.Merge.Post
* BusinessObjects.Container.AssociateMaterials.Post

## Pre Conditions

* The Transactions from Lookup Table [CustomTransactions](/cmf.custom.help/techspec>artifacts>lookuptables>custom_transactions) (which are mapped to above Action Groups) must be configured and enabled in the Generic Table [CustomTransactionsToTibco](/cmf.custom.help/techspec>artifacts>generictables>custom_transactions_to_tibco).

## Action

The DEE will publish a message on the Message Bus with Material information on each Transaction.

The published message will contain the following information:

* Headers, used to filter the messages on TibcoEMS.
* Message with all Material information in XML format.
