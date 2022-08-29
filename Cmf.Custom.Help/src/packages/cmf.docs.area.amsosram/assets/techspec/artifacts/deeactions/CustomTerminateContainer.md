# CustomTerminateContainer

## Overview

DEE Action used to terminate a Container from a specific type configured as a VendorContainerType.

## Action Groups

* BusinessObjects.Container.DisassociateMaterials.Post

## Pre Conditions

* N/A

## Action

When the materials are disassociated from the container (by performing the **Empty/Transfer Materials/Manage Positions** operations), this DEE action will be triggered and validate if the container type is configured at /AMSOsram/Container/VendorContainerTypes/.

In that case, if the container **is empty and not docked**, it will be terminated. 
