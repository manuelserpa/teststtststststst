# Custom Create Goods Issue Message

## Overview

DEE Action to create an Integration Entry with Goods Issue information.

## Action Groups

* BusinessObjects.MaterialCollection.TrackOut.Post

## Pre Conditions

* The material must have a Production order Associated 
* The material must be on the initial step of the Production Order
* The material must be of type lot
* The Smart Table CustomReportConsumptionToSAP is configured

## Action
Upon execution this DEE action will check the material(s) that is being tracked out and validate the preconditions mentioned above. 
In case all validations pass the information about the goods issue is passed to an Integration Entry.