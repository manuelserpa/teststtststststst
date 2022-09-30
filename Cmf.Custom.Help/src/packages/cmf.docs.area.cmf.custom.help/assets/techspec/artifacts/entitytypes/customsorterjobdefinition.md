# CustomSorterJobDefinition

## Overview

Custom Sorter Job Definition entity.

## Properties

The table below describes the properties for this entity type:

| Name             | Type      | Is Mandatory | Data Type | Description                                        |
| :--------------- | :-------- | :----------: | :-------- | :------------------------------------------------- |
| AlignWafer | Property | Yes      | Bit  | Align Wafer |
| FlipWafer | Property |  Yes      |  Bit | Flip Wafer |
| LogisticalProcess | Property |  Yes          |  CustomSorterLogisticalProcess | Logistical Process |
| MovementList | Property |    No        | JSON  | Movement List |
| ReadWaferId | Property |      Yes      | Bit  |  Read Wafer Id|
|  SourceCarrierType| Property |   Yes         |  ContainerType | Possible type for source Carrier |
| TargetCarrierType | Property |      Yes      |  ContainerType | Possible type for target Carrier |
| WaferIdOnBottom | Property |       Yes     |  Bit |  Wafer Id On Bottom |
