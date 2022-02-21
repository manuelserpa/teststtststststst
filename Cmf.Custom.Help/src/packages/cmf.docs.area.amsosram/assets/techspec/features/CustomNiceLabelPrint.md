# Custom Nice Label Print

## Requirement Specification
When performing a track out a DEE action will be executed resolving the information present on the Smart Table

## Design Specification

### Relevant Artifacts
The table below describes the artifacts for this feature:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------
[CustomMaterialNiceLabelPrintContext](/AMSOsram/techspec>artifacts>smarttables>CustomMaterialNiceLabelPrintContext) | Smart Table | Yes | -- |


### How it works
Upon execution, this feature collects the available information to resolve the Smart Table **[CustomMaterialNiceLabelPrintContext](/AMSOsram/techspec>artifacts>smarttables>CustomMaterialNiceLabelPrintContext)**.  
The information that results is attached to the other required fields to later be sent to the printer.
The result of this functionality should be a list containing the following fields:
* Label - The label to be printed;
* Printer - The printer to be used;
* Quantity - Quantity of labels to print;
* Zeile1 -
* Zeile2 -
* Zeile3 -
* Zeile4 -
* Zeile5 -
* Zeile6 -
* Zeile7 -
* Zeile5RB -
* Barcode -

The first three fields are configured on the Smart Table mentioned [above](#relevant-artifacts).  


### Assumptions


## Work items

The table below describes de user stories that affect the current functionality

| User Story | Type       | Title                                                  | Description |
| :--------- | :--------- | :----------------------------------------------------- | :---------- |
| 154257     | User Story | Trigger the NiceLabel printing on Lot Start (TrackOut) |             |