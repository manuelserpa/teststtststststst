# Custom Process Product

## Overview

Action used to create or update Product using body message of an Integration Entry.

## Action Groups

* N/A

## Pre Conditions

To execute this action the **MessageType** it will be equals to **PerformProductMasterData**.

## Action

After the action is triggered:

1. It will check if the body message is not empty.
    * If the validation is **successful** the system will create/update Product using body message.
2. If any problem occurs during the process, the error will be associated with the Integration Entry.

![IECreateProductError](/assets/techspec/images/IECreateProductError.png)