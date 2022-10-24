# CustomSendAdHocTransferInformationToIoT

## Overview

DEE Action responsible for sending AdHoc Transfer Information to IoT in order to process containers operations.

## Action Groups

- N/A

## Pre Conditions

- N/A

## Action

The DEE will send a message to IoT with all the information needed to transfer materials from a LoadPort to another.

In order to send this information, the MES must met the following conditions:

- Inputs must be valid and entities must exist in the system
- Resource (Parent resource of LoadPorts) must have `IsRecipeManagementEnabled` set to true. Available when `Edit` a Resource
- Resource (Parent resource of LoadPorts) must be a Sorter. Available as an attribute `isSorter` and must be set to `true`
- Resource (Parent resource of LoadPorts) must be online. 
- Source LoadPort must be a SubResource of the given Resource
- Source LoadPort must not be in use. Available as an attribute `IsLoadPortInUse` must be set to `false`
- SmartTable `CustomProductContainerCapacities` must have at one resolved row.
- SmartTable `RecipeContext` must have one resolved row with the following Service, `WaferReception`.
- To be a eligible destination port:
    - The destination LoadPort must have a container docked
    - The destination LoadPort must not be in use. Available as an attribute `IsLoadPortInUse` must be set to `false`
    - Must have free positions
    - Must have a match between the product provided and the product materials on the docked container, if exists.
    - The sum of possible destination port positions must fulfill the given quantity

If any one these fails, it will throw an error with the respective failing condition. Otherwise, it will send to IoT a message with:

- MaterialId: `CarrierAtLoadPort{SourceLoadPort DisplayOrder}`
- MaterialName: `CarrierAtLoadPort{SourceLoadPort DisplayOrder}`
- ContainerId: `CarrierAtLoadPort{SourceLoadPort DisplayOrder}`
- ContainerName: `CarrierAtLoadPort{SourceLoadPort DisplayOrder}`
- LoadPortPosition: `{SourceLoadPort DisplayOrder}`
- MaterialState: `Setup`,
- ContainerOnlyProcess: `true`,
- Recipe: `Row resolved on RecipeContext`
- SorterJobInformation: based on **CustomSorterJobDefinition**. 

The movement list should have the order inverted (desc).

Currently it is being used on a custom GUI Wizard (link) to gather all the inputs necessary to call this DEE as a service.
