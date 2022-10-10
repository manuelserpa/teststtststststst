# CustomSendEventMessage

## Overview

DEE action to publish a Material message to Message Bus when an Action on Material is triggered.

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

* Before the *MaterialMoveNext* action is executed the Material path will be saved in Context.
* This DEE action must be executed when the Transaction associated to ActionGroup is active on Generic Table [CustomTransactionsToTibco](/cmf.custom.help/techspec>artifacts>generictables>custom_transactions_to_tibco).

## Action

The DEE will publish on the Message Bus a message with all the information associated to the Material on which the Action was triggered.

The published message contains the following information:

* Headers, used to filter the messages on TibcoEMS.
* Message with all Material information in XML format.
