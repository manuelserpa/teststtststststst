# Wafer Reception (AdHoc Sorter Job)

## Requirement Specification

The system should be able to:

- Provide interface for user to define load port and intended wafer product to be used in the wafer reception job since the source containers are not automatically identifiable via RFID.
- Configuration to specify the capacity of the source container and the max slot capacity for wafer transfer to destination container. This should be configured by product level
- If the operator specifies the source capacity of the source container, the system will override the default one

- If there is not enough slot capacity available in the destination container(s), the system should not start sorter job with clear error message for operator acknowledgement

- When source container's slot maps available, system shall validate for enough slot capacity to transfer to destination container(s)
- System should prioritized the wafer transfer to least available slot in the destination containers

- System should perform wafer validation to proceed the transfer only when all below conditions is met:

  - wafer ID is available and registered in MES
  - wafer is at wafer reception step
  - wafer is active and not on hold
  - wafer's product is the same as existing placed wafer's product in the intended destination container

- When each wafer is placed on the destination container, system should register the wafer into the new container ID immediately
- When source container's wafers are completely transferred, the destination containers should be unclamp automatically

## Design Specification

#### **New SmartTable to define source and target capacities `CustomProductContainerCapacities`**

Columns:

- Keys: 
  - Product Group
  - Product
- Values:
  - Source Capacity
  - Target Capacity

#### **SmartTable to define recipe `RecipeContext`**

Columns:

- Keys: 
  - Service (WaferReception)
  - Product
  - Product Group
  - Resource
- Values:
  - Recipe

#### **New LookupTable to list sorter process `CustomSorterProcess`**

Values:

- WaferReception

#### **New Wizard for Adhoc Transfer (only available on a Resource with `IsSorter` attribute)**

Inputs:

- Resource (readonly): current resource
- Source LoadPort (required): list of LoadPorts with the setting attribute `IsLoadPortInUse` set to false
- Product (required): on product selection resolve the SmartTable `CustomProductContainerCapacities`
- Sorter Process (required) : Values of LookupTable `WaferReception`
- Quantity (required): Quantity to transfer. Defaults to the source capacity resolved on the SmartTable `CustomProductContainerCapacities`

Action:

Call below DEE (`CustomSendAdhocTransferInformationToIoT`) on execution with all the information in the wizard

#### **New DEE `CustomSendAdhocTransferInformationToIoT`:**

Inputs:

- Resource
- Source LoadPort
- Product
- SorterProcess (defaults to `WaferReception`)
- Quantity

Action:

1. Receives the information above
2. Validates the Resource and the correspondent LoadPorts with containers
3. Validates possible positions with max capacity defined on the SmartTable `CustomProductContainerCapacities`
4. Generates `CustomSorterJobDefinition` with a MovementList inverting the positions

#### **New DEE `CustomValidateMaterialReceptionSubstrate`**

Inputs:

- Substrate Identifier
- TargetContainer
- SorterProcess (defaults to `WaferReception`)

Action:

1. Substrate needs to exist in the system. Needs to be in `Active` State and not be on `Hold`
2. Product needs to match with existing container substrate products

### Relevant Artifacts

The table below describes the properties for this feature:

| Name                                                                                                                                   | Type         | Description                                                                                                             |
| :------------------------------------------------------------------------------------------------------------------------------------- | :----------- | :---------------------------------------------------------------------------------------------------------------------- |
| [CustomSendAdhocTransferInformationToIoT](cmf.custom.help/techspec>artifacts>deeactions>custom_send_adhoc_transfer_information_to_iot) | DEE Action   | DEE Action responsible for sending AdHoc Transfer Information to IoT in order to process containers operations          |
| [CustomValidateMaterialReceptionSubstrate](cmf.custom.help/techspec>artifacts>deeactions>custom_validate_material_reception_substrate) | DEE Action   | DEE action responsible for validate if the wafer is valid to proceed with the process                                   |
| [CustomWizardAdhocTransfer](cmf.custom.help/techspec>artifacts>html>components>custom_wizard_adhoc_transfer)                           | UI Page      | UI Page responsible for gathering the necessary input and triggering the adhoc transfer                                 |
| [CustomProductContainerCapacities](cmf.custom.help/techspec>artifacts>smarttables>custom_product_container_capacities)                 | Smart Table  | SmartTable used to resolve the source and target capacity to be used as maximum of positions available on the container |
| [CustomSorterProcess](cmf.custom.help/techspec>artifacts>lookuptables>custom_sorter_process)                                           | Lookup Table | Lookup table with available processes for Ad Hoc Sorter Jobs                                                            |

### How it works

On a given Resource with `IsSorter` attribute set to `true` the user should be able to see and click the button open the new custom wizard [CustomWizardAdhocTransfer](cmf.custom.help/techspec>artifacts>html>components>custom_wizard_adhoc_transfer)

After clicking on that button, the user should provide all the necessary information and at end the system will send instructions (**MaterialData**) to the **Sorter** equipment to be processed (if all the condition on the DEE are checked).

At the same point, equipment integration will use the instructions given by the DEE and will trigger the DEE `CustomValidateMaterialReceptionSubstrate` to validate the equipment can process with the movement. If it can, the sorter equipment will transfer **all** wafers from the source container to the target container.

### Assumptions

It is assumed the following actions are performed by the **Operator**:

- Set the Source Load Port where the **Source Container** is docked.
- Set the Product.
- Set the Sorter Process.
- Set the Quantity.
- Must have docked containers not in use with the sufficient number of available position to fullfil the desired quantity

## Work items

The table below describes de user stories that affect the current functionality

| User Story | Type       | Title                 | Description                                                           |
| :--------- | :--------- | :-------------------- | :-------------------------------------------------------------------- |
| 199466     | User Story | Wafer Reception - MES | Possibility to transfer wafer between containers using the Adhoc GUI. |
