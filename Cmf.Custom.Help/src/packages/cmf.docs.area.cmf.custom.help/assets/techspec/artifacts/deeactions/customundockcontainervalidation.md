# CustomUndockContainerValidation

## Overview

DEE Action used to validate if a Container can be undocked based on its Type being or not configured as a VendorContainerType.

## Action Groups

* BusinessObjects.Container.Undock.Pre
* BusinessObjects.Container.Undock.Post

## Pre Conditions

* N/A

## Action

When the Undock operation is triggered, this DEE action will be executed and validate if the container type is configured at /amsOSRAM/Container/VendorContainerTypes/.
In that case, if the container has positions being occupied with materials, an exception will be thrown and the Undock operation **will not be allowed**. 

Otherwise, if the container is empty it will be undocked and terminated as well.