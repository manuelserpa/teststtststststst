# Custom Process Products From ERP

## Overview

Action used to create an Integration Entry per Product using ERP received message.

## Action Groups

* N/A

## Pre Conditions

To execute this action the **MessageType** it will be equals to **PerformProductsMasterData**.

## Action

After the action is triggered:

1. It will check if the body message is not empty.
    * If the validation is **successful** the system will create an Integration Entry per Product.
2. If any problem occurs during the process, the error will be associated with the Integration Entry.
