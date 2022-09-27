# Undock Vendor Container Type

## Requirement Specification

Undock not allowed to containers whose type is configured as a VendorContainerType.

## Design Specification

The system should be able to not allow to undock a container whose type is configured at */amsOSRAM/Container/VendorContainerTypes/*.

### Relevant Artifacts

The table below describes the relevant artifacts for this feature:

| Name          | Type      | Description |
| :------------ | :-------- | :---------- |
| [CustomUndockContainerValidation](/cmf.custom.help/techspec>artifacts>deeactions>CustomUndockContainerValidation) | DEE Action | DEE Action used to validate if a Container can be undocked based on its Type being or not configured as a VendorContainerType. |
| /amsOSRAM/Container/VendorContainerTypes/ | Configuration | Vendor Container Types splitted by ',' |


### How it works

When the Undock operation is triggered, the system will validate if the container type is configured at **/amsOSRAM/Container/VendorContainerTypes/**.
In that case:
- If the container has some of its positions being used, then an exception will be thrown and the Undock operation **will not be performed**.
- If the container is empty, then the Undock operation will be performed and the container will be terminated.

If the container type is not configured, then the Undock operation will be normally performed.

**Note**: The configuration **/amsOSRAM/Container/VendorContainerTypes/** allows to configure more than one container type **splitted by a comma (',')**.

### Assumptions
N/A.

## Work items

The table below describes the user stories that affect the current functionality.

User Story | Type       | Title                                             | Description
:--------- | :--------- | :------------------------------------------------ | :----------
190613     | User Story | Container Validations for Vendor boxes            | Container Validations for Vendor boxes
