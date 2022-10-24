# Smart Tables

Smart tables are general purpose context resolution tables to store data records. Each smart table has its own database table in the database, and therefore, requires a unique meta-data definition. In addition, the search precedence keys and order is part of the smart table definition.

## Custom Smart Tables

The following smart tables were created to support customer requirements.

| Table                     | Description       |
| ------                    | ------            |
| [CustomProductContainerCapacities](/cmf.custom.help/techspec>artifacts>smarttables>custom_product_container_capacities) | SmartTable used to resolve the source and target capacity to be used as maximum of positions available on the container. |
| [Custom Material Nice Label Print Context](/cmf.custom.help/techspec>artifacts>smarttables>custommaterialnicelabelprintcontext) | Used to generate and print labels on Nice Label Printer. |
| [CustomReportConsumptionToSAP](/cmf.custom.help/techspec>artifacts>smarttables>customreportconsumptiontosap) | Used to identify the Storage Location |
| [CustomSorterJobDefinitionContext](/cmf.custom.help/techspec>artifacts>smarttables>customsorterjobdefinitioncontext) | Used to resolve the CustomSorterJobDefinition for the specific Sorter context |


