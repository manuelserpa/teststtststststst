# CustomReclaimContainerType

## Overview

Generic table used to map a Container type to its equivalent Reclaim Container type. Used in the automatic grading process.

## Table Columns

The following table describes the table columns:

|          Name          | Data Type | Reference   |                Description              |
| :--------------------- | :-------: | :---------- | :-------------------------------------- |
| Source Container Type  | String    | LookupValue | Reference to ContainerType Lookup table |
| Reclaim Container Type | String    | LookupValue | Reference to ContainerType Lookup table |
