# Generate Sorter Job Definition From Future Action

## Requirement Specification
Generate Sorter Job Definitions from Future Actions created on MES of type Split and Merge

## Design Specification
The user should be able to add a Future Action of type Split or Merge and the System should convert those Future Actions into a Sorter Job Definition and also insert the Sorter Job Definition with the given context, extracted from the Future Action, into the Smart Table 'CustomSorterJobDefinitionContext'.
### Relevant Artifacts
The table below describes the properties for this entity type:

Name          | Type      | Is Mandatory | Data Type | Description 
:------------ | :-------- | :----------: | :-------- | :-----------
[CustomGenerateSorterJobDefinitionFromFutureAction](/cmf.custom.help/techspec>artifacts>deeactions>CustomGenerateSorterJobDefinitionFromFutureAction) | DEE Action | Dee action to Generate a Custom Sorter Job Definition if exists a Required Future Action for a given material.
[CustomSorterJobDefinition](/cmf.custom.help/techspec>artifacts>entitytypes>CustomSorterJobDefinition) | Entity Type | Custom entity type Custom Sorter Job Definition
[CustomSorterJobDefinitionContext](/cmf.custom.help/techspec>artifacts>smarttables>CustomSorterJobDefinitionContext) | Smart Table | Holds Custom Sorter Job Definition Contexts

### How it works
For the following system operations:

- Change Material Flow And Step
- Move Materials To Next Step
- Release Material
- Special Release Material

The system will check for the current material if it has a required future action, if this is true, the system will convert the required future action created by the user into a Custom Sorter Job Definition and add that information to the smart table 'CustomSorterJobDefinitionContext'.

### Assumptions
Future Actions must be created with Manual Execution and on the Queued State.

## Work items

The table below describes de user stories that affect the current functionality

User Story  | Type       | Title                                    | Description
:---------- | :--------- | :--------------------------------------- | :----------
162134      | User Story | Custom Sorter Job creation               | N/A
