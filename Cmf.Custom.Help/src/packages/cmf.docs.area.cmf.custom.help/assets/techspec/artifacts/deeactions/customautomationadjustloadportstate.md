# CustomAutomationAdjustLoadPortState

## Overview

This DEE Action is triggered by IoT Automation in order to adjust the state of a Load Port.

## Action Groups

N/A

## Pre Conditions

N/A

## Action

This DEE Action is responsible for adjusting the state of a Load Port based on:
*  Load Port display order and Parent Resource
*  Or Load Port Resource Name

**Notes**: 
* If the load port state model state is adjusted to **Available** state, then:
  * **IsLoadPortInUse** resource attribute is set to **false**
  * If a container is docked on the Load Port then the **undock** will be performed
  
* If the load port state model state is adjusted to **ReadyToUnload** state, then:
  * **MapContainerNeeded** container attribute is set to **false**
  * **Product** container attribute is set to empty