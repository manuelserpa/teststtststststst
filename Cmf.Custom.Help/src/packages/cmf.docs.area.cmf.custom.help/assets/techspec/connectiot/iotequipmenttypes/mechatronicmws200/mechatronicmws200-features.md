Features
============
This section describe the Features delivered.

Material Received
============

### Requirement Specification
When a carrier arrives to Load Port, the Load Port must be set to Transfer Blocked.

### Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------
CustomLoadPortStateModel| State Model|Yes| N/A| Load Port State Model
CustomAutomationAdjustLoadPortState|DEE Action| Yes| N/A| Action responsible for updating State Model State on Load Port

### How it works
When Material Received equivalent event is received set Load Port Resource Main State to Transfer Blocked.

### Assumptions
Load Port Resource must be using CustomLoadPortStateModel and have defined the Property Display Order with the expected value for the Equipment Integration.

### Work items

The table below describes the user stories that affect the current functionality

User Story | Type | Title | Description
:--------- | :--- | :---- | :----------

Material Removed
============

### Requirement Specification
When a carrier is removed from the Load Port, the Load Port must be set to Ready To Load.
If Load Port Automation Mode is set to Online, Undock Container on MES.

### Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------
CustomLoadPortStateModel| State Model|Yes| N/A| Load Port State Model
CustomAutomationAdjustLoadPortState|DEE Action| Yes| N/A| Action responsible for updating State Model State on Load Port

### How it works
When Material Removed equivalent event is received set Load Port Resource Main State to Ready To Unload.
If the Load Port Automation Mode property is set to Online, undock the Container.

### Assumptions
Load Port Resource must be using CustomLoadPortStateModel and have defined the Property Display Order with the expected value for the Equipment Integration.

### Work items

The table below describes the user stories that affect the current functionality

User Story | Type | Title | Description
:--------- | :--- | :---- | :----------

Ready To Unload
============

### Requirement Specification
When a Carrier is ready to be removed from Load Port, the Load Port must be set to Ready To Unload.

### Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------
CustomLoadPortStateModel| State Model|Yes| N/A| Load Port State Model
CustomAutomationAdjustLoadPortState|DEE Action| Yes| N/A| Action responsible for updating State Model State on Load Port

### How it works
When a Ready To Unload equivalent event is received set Load Port Resource Main State to Ready To Unload.

### Assumptions
Load Port Resource must be using CustomLoadPortStateModel and have defined the Property Display Order with the expected value for the Equipment Integration.

### Work items

The table below describes the user stories that affect the current functionality

User Story | Type | Title | Description
:--------- | :--- | :---- | :----------

Slot Map Received
============

### Requirement Specification
When a Slot Map is received from the equipment, store it for the corresponding Container Data.

### Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------
Container Persistence| Persisted Object |Yes| ContainerData| Container Information

### How it works
Upon receiving the Slot Map convert it to the Template expected format and store it.

### Assumptions
Slot Map must be converted to the expected format (1 for Occupied, 0 for Empty) in either an Array or String format.

### Work items

The table below describes the user stories that affect the current functionality

User Story | Type | Title | Description
:--------- | :--- | :---- | :----------

Track In
============

### Requirement Specification
At Track In:

- Verify Slot Map on tool matches MES Slot Map;
- Verify Control State is Online Remote;
- Verify Recipe matches MES information;
- Execute Creation of Process and Control Job;
- Persisted received Material object data;

If any of the actions described before fails, fail Track In.

### Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------
Material object| Material object| Yes| MaterialData| Object containing all the information needed for the Material, will be persisted;
Recipe data| Recipe Object| Yes| RecipeData| Contained on MaterialData contains the information needed to validate the Recipe
TrackIn| Action Group| Yes| N/A| Action group that triggers the Track In

### How it works

Upon being triggered by the Track In request, the Track In workflow:

1. Gets the current value of the Control State equivalent variable;
2. Activates the standalone Workflow TrackInInitialValidations which is responsible for validating the Slot Map and Control State;
3. As no Id is read for the Container from the equipment, updates the Container on the Load Port Position with the MES Container Name;
4. Validate that the Recipe exists on the Tool by calling the sub-workflow RecipeValidation_01_UnformattedRecipeMainFlow, which executes a S7F19 and verifies the existence of the PPID on the received list;
5. Executes the Process Job creation (S16F11) by triggering the sub-workflow CreateProcessJob_01_CreateJob
6. Executes the Control Job creation (S14F9) by triggering the sub-workflow CreateControlJob_01_CreateJob;
7. Stores the Material Data on Persistence;
8. Returns the Success of the operation.

If one of these steps fail, Track In will be stopped and the MES transaction will fail with an Error.

If a Sampling Plan is defined the Process Job will only consider the sub-materials that are set to be Processed, this is done so by only considering Queued wafers.

### Assumptions

Track In message will only be sent to an Equipment Integration if the Automation Mode of the Resource is set to Online.
A Recipe will have to be defined on the Recipe context.
The Resource as to have the flag Recipe Management Enabled to execute Recipe Context resolution.
Slot Map must have been received from the tool prior to the Track In action else it will fail.

### Work items

The table below describes the user stories that affect the current functionality

User Story | Type | Title | Description
:--------- | :--- | :---- | :----------

Track Out
============

### Requirement Specification
When Track Out request is received clear the persisted Material Data.

### Design Specification

### Relevant Artifacts

The table below describes the properties for this entity type:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------
Material object| Material object| Yes| MaterialData| Object containing all the information needed for the Material;
TrackOut| Action Group| Yes| N/A| Action group that triggers the Track Out

### How it works
Upon receiving the Track Out request delete the persistence.
Fail if Material Data does not match a know object.

### Assumptions
Track Out message will only be sent to an Equipment Integration if the Automation Mode of the Resource is set to Online.
Track In for the Material must have been executed successfully with Automation Mode Online beforehand.

### Work items

The table below describes the user stories that affect the current functionality

User Story | Type | Title | Description
:--------- | :--- | :---- | :----------

Process Stated
============

### Requirement Specification
When a Process Started equivalent event is detected:

- Persisted Material Data must be updated to In Process;
- MES Material entity instance state must be updated to In Progress.

### Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------
Material object| Material object| Yes| MaterialData| Object containing all the information needed for the Material, will be persisted;
CustomMaterialStateModel|State Model| Yes| N/A| State Model for the Material entity instance
CustomAutomationSetMaterialState| DEE Action| Yes| N/A| Action responsible for updating the state of Material on MES

### How it works

Upon receiving the Process Started equivalent event, update Material Data on persistence and then update it on MES.

If the SorterJob is of type MapCarrier or Compose the container will also be cleared on MES, so it can later be recreated.

### Assumptions
Material must have been tracked in previously, and have defined the CustomMaterialStateModel as main state.

### Work items

The table below describes the user stories that affect the current functionality

User Story | Type | Title | Description
:--------- | :--- | :---- | :----------

Process Complete
============

### Requirement Specification
When a Process Complete equivalent event is received:

- Persisted Material Data must be updated to Complete;
- MES Material entity instance state must be updated to Complete;
- Execute Material Track Out.

### Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------
Material object| Material object| Yes| MaterialData| Object containing all the information needed for the Material, will be persisted;
CustomMaterialStateModel|State Model| Yes| N/A| State Model for the Material entity instance
CustomAutomationSetMaterialState| DEE Action| Yes| N/A| Action responsible for updating the state of Material on MES
Material Out| MES Service| Yes| N/A| Service responsible for executing the Track Out of the Material

### How it works

Upon receiving the Process Complete equivalent event, if it is not a container only process, it will update Material Data on persistence and then update it on MES.
After the state is updated, or skipped in case it is just a container only process, it will execute the Material Out service to trigger the automated Track Out.

### Assumptions
Material must have been tracked in previously, and have defined the CustomMaterialStateModel as main state.

### Work items

The table below describes the user stories that affect the current functionality

User Story | Type | Title | Description
:--------- | :--- | :---- | :----------


Wafer Id Read
============

### Requirement Specification
When a Wafer Id Read equivalent event is received:

- ProceedWithSubstrate or CancelSubstrate remote commands must be sent

### Design Specification

### Relevant Artifacts
The table below describes the properties for this entity type:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------
Material object| Material object| Yes| MaterialData| Object containing all the information needed for the Material, will be persisted;
CustomMaterialStateModel|State Model| Yes| N/A| State Model for the Material entity instance
CustomValidateMaterialReceptionSubstrate| DEE Action| Yes| N/A| Action responsible for updating the state of Material on MES

### How it works

Upon receiving the Wafer Id Read equivalent event that requires host input, the ProceedWithSubstrate sub-workflow will be called. This workflow will retrieve the MaterialData from the persistance and, with its information, call the CustomValidateMaterialReceptionSubstrate DEE to validate the Acquired ID on MES (Wafer ID read from the machine). After the DEE execution the persistance will then be updated.

At the end, if the DEE is **successfull**, and the event is a **missmatch event**, a **ProceedWithCarrier** remote command will be sent. 

However, if the DEE **fails**, or if the triggering event is a **failed to read event**, a **CancelSubstrate** remote command will be sent


### Assumptions


### Work items

The table below describes the user stories that affect the current functionality

User Story | Type | Title | Description
:--------- | :--- | :---- | :----------
201255 | UserStory | Lot Composing - EI | Lot Composing Implementation