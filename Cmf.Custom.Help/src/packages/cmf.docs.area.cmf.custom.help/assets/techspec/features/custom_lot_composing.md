# Lot Composing

## Requirement Specification

Lot composing shall only consist of map carrier job without physical wafer transfer to another container

When the source container is placed and slot map received, system should find the right lot intended for the lot composition based on below rule:

- Lots with planned start date equals or approximate with the current date
  - If exists more than one lot with the same planned start date, it should take in consideration the priority
- BOM of the lot matched (assuming the BOM defined the required quantity and substitute product)
- SlotMap matched with the intended lot's BOM product quantity

If any of above condition are not met, the sorter job should not be started and clear error message to be shown to operator

When above conditions are met and carrier is accepted, sorter should perform wafer OCR read and verify the wafer ID is valid (at wafer reception step, and the right product)

When all the wafers are verified, the system should perform wafer to lot assignment at the end of the job and proceed to automatically track out when below condition met:

- Wafer product matched with BOM specification of the lot

When the lot successfully track out, the system should create a nice label printer for automated barcode printing related to the lot that had been composed for operator to stick to the container

## Design Specification

### Relevant Artifacts

The table below describes the relevant artifacts for this feature:

| Name                                                                    | Type    | Description                                                                                          |
| ----------------------------------------------------------------------- | ------- | ---------------------------------------------------------------------------------------------------- |
| [MaterialIn](/cmf.custom.help/techspec>artifacts>services>materialin)   | Service | Performs the Material TrackIn into a resource.                                                       |
| [MaterialOut](/cmf.custom.help/techspec>artifacts>services>materialout) | Service | Performs Material or Sub Material TrackOut considering if Materials are associated with a container. |

### How it works

The MaterialIn handles the Dispatch and TrackIn or just TrackIn of a Material or Sub Material for a variety of scenarios depending on different configurations.

The MaterialOut handles the TrackOut and MoveNext or just TrackOut of a Material or Sub Material for a variety of scenarios depending on different configurations.

One of those scenarios is **Lot Composing**.

**Lot Composing**

Before running the MaterialIn, MES expects:

- Wafers on a container docked to a LoadPort that is not in use
- Lots on the same Step as the wafers
- BOM defined for the wafers Step
- CustomSorterJobDefinition defined for the wafers Step

For this scenario it should be provided the **Resource** that will be used to TrackIn the Lot.

It starts by getting all the containers docked on the LoadPorts and the respective Wafers. After gathering all the possible containers checks if the container does not have the attribute **Lot** and does not require a MapContainer, the container is only eligible if all the wafers attached are on a Step set as **Wafer Reception**

Then, decides which **Lot** to TrackIn based on the _Planned StartDate_ and _Priority_. For this to be possible it was created a **Sort Rule** (MaterialSortRuleSorterProcess) that defines this order. 

**NOTE:** To sort by _Planned StartDate_ it was created a **Rule** (CustomSortRuleMaterialPossibleStartDate) that calls a **DEE** (CustomMaterialPossibleStartDate) to perform this specific sort.

After choosing the **Lot**, it validates if the BOM can be satisfied (with substitutes if exist) with the Product and number of wafers docked on the container.

If everything is as expected it should create a MovementList with the same origin and destination container and with the same respective position and send it to IoT.

Before running the MaterialOut, expects:

- Resource
- Lot TrackIn in the Resource
- CustomSorterJobDefinition with the MovementList created on MaterialIn

At this point, checks if the MovementList was respected and creates the **Logical Wafer** for each **Wafer** and attaches it to the **Lot**, following the structure below:

- Lot
  - Logical Wafer with the following Name: *{LotName}\_{PositionOnContainer}* and Form: *Logical Wafer*
    - Wafer

If the Logical Wafer is already created the MES will throw an Exception if:

- Has a Parent ID different from the supposed one
- Has child Materials (sub materials) of the same type and form as the wafers
- Has a container associated

At end, the system performs the TrackOut and MoveNext the **Lot** to the following Step.

### Assumptions

- Resource must have the **Material Sort Rule Set** set to **MaterialSortRuleSorterProcess**
- Wafers should be in a **Wafer Reception** Step
- Wafers should be associated to a container docked on a **LoadPort** of the desired **Resource**
- **CustomSorterJobDefinition** defined for the **Step** on the SmartTable **CustomSorterJobDefinitionContext**
- **BOM** defined for the **Step** with *Assembly Type* as **Explicit Add**

## Work items

The table below describes the user stories that affect the current functionality.

| User Story | Type       | Title                                                      |
| :--------- | :--------- | :--------------------------------------------------------- |
| 201257     | User Story | Lot Composing - Orchestration and Movement List Generation |
