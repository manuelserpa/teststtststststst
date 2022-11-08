# CustomValidateMaterialReceptionSubstrate

## Overview

DEE action responsible for validate if the wafer is valid to proceed with the process. 

## Action Groups

- N/A

## Pre Conditions

- N/A

## Action

This DEE expects as Input:

- WaferID: The name of the wafer
- TargetContainer: The target container
- SorterProcess: The sorter process. It default to: `WaferReception`

Checks if the transfer meets all below conditions:

- Wafer ID is available and exists on the MES
- Wafer is at wafer reception step and proceed with substrate
- Wafer is `Active` and not on `Hold`
- Wafer's product is the same as existing placed wafer's product in the intended destination container

If the wafer is cleared to proceed with the transfer, the DEE won't throw any error. 
Otherwise, it will abort the sorter job with clear error message for operator acknowledgement with the correspondent failing condition. 
