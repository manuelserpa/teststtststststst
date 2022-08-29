# Terminate Vendor Container Type

## Requirement Specification

Terminate a container whose type is configured as a VendorContainerType after materials being disassociated from the container.

## Design Specification

The system should be able to terminate a container whose type is configured at */AMSOsram/Container/VendorContainerTypes/* after **Empty/Transfer Materials/Manage Positions** operations being performed.

### Relevant Artifacts

The table below describes the relevant artifacts for this feature:

| Name          | Type      | Description |
| :------------ | :-------- | :---------- |
| [CustomTerminateContainer](/AMSOsram/TechSpec>Artifacts>deeactions>CustomTerminateContainer) | DEE Action | DEE Action used to terminate a Container from a specific type configured as a VendorContainerType. |
| /AMSOsram/Container/VendorContainerTypes/ | Configuration | Vendor Container Types splitted by ',' |


### How it works

After **Empty/Transfer Materials/Manage Positions** operations being triggered, the system will validate if the container type is configured at **/AMSOsram/Container/VendorContainerTypes/**.
In that case:
- If the container is **empty and not docked**, it will be terminated.
- If the container is docked, the container will remain as **Active**.

**Note**: The configuration **/AMSOsram/Container/VendorContainerTypes/** allows to configure more than one container type **splitted by a comma (',')**.

### Assumptions
N/A.

## Work items

The table below describes the user stories that affect the current functionality.

User Story | Type       | Title                                             | Description
:--------- | :--------- | :------------------------------------------------ | :----------
190613     | User Story | Container Validations for Vendor boxes            | Container Validations for Vendor boxes