# Update Compose CustomSorterJobDefinition

## Requirement Specification

Update a Compose CustomSorterJobDefinition when a BOM is set as effective.

## Design Specification

### Relevant Artifacts

The table below describes the relevant artifacts for this feature:

| Name                                                                                                                                             | Type        | Description                                                                   |
| ------------------------------------------------------------------------------------------------------------------------------------------------ | ----------- | ----------------------------------------------------------------------------- |
| [CustomCreateComposeSorterJobDefinition](/cmf.custom.help/techspec>artifacts>artifacts>custom_create_compose_sorter_job_definition)              | DEE Action  | Responsible for create compose sorter job definitions based on BOMs.          |
| [CustomUpdateComposeCustomSorterJobDefinition](/cmf.custom.help/techspec>artifacts>artifacts>custom_update_compose_custom_sorter_job_definition) | DEE Action  | Responsible for updating the Compose CustomSorterJobDefinition based on BOMs. |
| [CustomSorterJobDefinitionContext](/cmf.custom.help/techspec>artifacts>smarttables>customsorterjobdefinitioncontext)                             | Smart Table | Used to resolve the CustomSorterJobDefinition for the specific Sorter context |

### How it works

After set a BOM as effective, validates if the BOM is set to be used for lot compose using the **IsForLotCompose** attribute and if the **StartingCarrierType** attribute contains a value.

If the previous conditions are met a new CustomSorterJobDefinition will be created (in case it does not exist yet) for the current BOM effective version and the system will validate if a context is found in **CustomSorterJobDefinitionContext** SmartTable for the previous CustomSorterJobDefinition:

- If a context for the previous BOM effective version exists in **CustomSorterJobDefinitionContext** SmartTable **it will be updated with the new CustomSorterJobDefinition**.
- If it is not configured, a warning message will be shown, informing that the CustomSorterJobDefinition **must be manually defined in CustomSorterJobDefinitionContext** SmartTable.

### Assumptions

N/A

## Work items

The table below describes the user stories that affect the current functionality.

| User Story | Type       | Title                                                  |
| :--------- | :--------- | :----------------------------------------------------- |
| 197299     | User Story | Lot Composing - BOM and CustomSorterJobDefinition CRUD |
