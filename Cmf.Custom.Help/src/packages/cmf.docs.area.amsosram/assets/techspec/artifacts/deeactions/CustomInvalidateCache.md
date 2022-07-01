# CustomInvalidateCache

## Overview

DEE action to publish an **Invalidate Cache** message to Message Bus when Generic Table CustomTibcoEMSGatewayResolver data is changed..

## Action Groups

* GenericTables.GenericTable.InsertOrUpdateRows.Post
* GenericTables.GenericTable.RemoveRows.Post

## Pre Conditions

* This DEE action must be executed when one of the two action groups is executed on the Generic Table CustomTibcoEMSGatewayResolver.

## Action

This DEE will publish a message on Message Bus with the associated subject CustomTibcoEMSGatewayInvalidateCache.