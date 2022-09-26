# Import Product From ERP

## Requirement Specification

Products can be imported to MES from the ERP. To do this, MES should parse a message sent by the ERP and create or update the necessary objects after validating the file content.

## Design Specification

### Relevant Artifacts

The table below describes the properties for this entity type:

| Name                         | Type        | Is Mandatory | Data Type | Description                             |
| ---------------------------- | ----------- | :----------: | --------- | --------------------------------------- |
| MessageType                  | LookupTable |      Yes     |     -     | Type of the Integration                 |
| IntegrationHandlerResolution | SmartTable  |      Yes     |     -     | Used to resolve the integration handler |

### How it works

The MES will receive a message format in XML through the **CustomReceiveERPMessage** API. Then, based on the MessageType, a specific **Action** will be called and the XML message will be interpreted and the necessary objects will be created.

These are the mandatory fields validated by the system:

- Name
- Description
- Type
- ProductType
- DefaultUnits
- IsEnabled
- Yield
- ProductGroup
- MaximumMaterialSize

| Name | Description | Mandatory | MES Entity | Attribute | Property/Attribute |
| --- | --- | :---: | --- | :---: | --- |
| Name | Product Name | Yes | Product | Yes | Name |
| Description | Product Description | No | Product | No | Description |
| Type | ProductVersion Type | Yes | Product | No | Type |
| ProductType | The product type of the product | Yes | Product | No | ProductType
| DefaultUnits | The default units of measure, used for display in Material Transfers | No | Product | No | DefaultUnits |
| IsEnabled | Product Is Enabled | Yes | Product | No | IsEnabled |
| Yield | Standard  Yield. Must be >=0 and <=1 | Yes | Product | No | Yield |
| ProductGroup | Product Product Group | No | ProductGroup | No | ProductGroup |
| MaximumMaterialSize | The maximum material size for Scheduling purposes | No | Product | No | MaximumMaterialSize |
| FlowPath | Product Flow Path | No | Product | No | FlowPath |
| InitialUnitCost | Standard Initial Product unit cost | Yes | Product | No | InitialUnitCost |
| FinishedUnitCost | Standard Finished Product unit cost | Yes | Product | No | FinishedUnitCost |
| CycleTime | Standard Product Cycle time | Yes | Product | No | CycleTime |
| IncludeInSchedule | Whether the product should be included in scheduling or not | Yes | Product | No | IncludeInSchedule |
| CapacityClass | The capacity class of the product, for scheduling reasons - used in Resources of type Tunnel and Batch | No | Product | No | CapacityClass |
| MaterialTransferMode | The MaterialTransfer mode, for Warehouse management | Yes | Product | No | MaterialTransferMode |
| MaterialTransferApprovalMode | Whether MaterialTransfers must be approved or not | Yes | Product | No | MaterialTransferApprovalMode |
| MaterialTransferAllowedPickup | Used for restricting MaterialTransfer pickups (Any (default), Requester, CostCenterEmployee, AreaEmployee) | Yes | Product | No | MaterialTransferAllowedPickup |
| IsEnabledForMaintenanceManagement | Whether the product can be used for maintenance management or not | Yes | Product | No | IsEnableForMaintenanceManagement |
| MaintenanceManagementConsumeQuantity | In case that the product can be used for maintenance management, whether quantity is to be consumed or not | Yes | Product | No | MaintenanceManagementConsumerQuantity |
| IsDiscrete | Whether the product quantities cannot be decimal | Yes | Product | No | IsDiscrete |
| FloorLife | The floor life of the Product | No | Product | No | FloorLife |
| FloorLifeUnitOfTime | The units of time of the Floor Life | No | Product | No | FloorLifeUnitOfTime |
| RequiresApproval | Whether Materials created for this product require an explicit approval before they can be used | No | Product | No | RequiresApproval |
| ApprovalRole | If defined, only users of this role will be able to approve Materials of this product | No | Role | No | ApprovalRole |
| CanSplitForPicking | Defines whether Materials from this Product can be split for picking, in case that the Material Logistics mode is Material | Yes | Product | No | CanSplitForPicking |
| MaterialLogisticsDefaultRequestQuantity | Specifies the default transfer requirement required quantity | No | Product | No | MaterialLogisticsDefaultRequestQuantity |
| ConsumptionScrap | The consumption scrap (or reject rate), represented as a percentage | Yes | Product | No | ConsumptionScrap |
| AdditionalConsumptionQuantity | A safety quantity to be considered in material requirements calculation (expressed in the default units) | Yes | Product | No | AdditionalConsumptionQuantity |
| IsEnabledForMaterialLogistics | Defines whether the Product is enabled for Material Logistics | Yes | Product | No | IsEnabledForMaterialLogistics |
| DefaultBOM | The Product default BOM | No | BOM | No | DefaultBOM |
| Name | Product Manufacturer Name | Yes | ProductManufacturer | No | Name |
| Note | A user note | No | ProductManufacturer | No | Note |
| FromUnit | From Unit | Yes | GenericTable (ProductUnitConversionFactors) | No | FromUnit |
| ToUnit | To Unit | Yes | GenericTable (ProductUnitConversionFactors) | No | ToUnit |
| ConversionFactor | Product Conversion Factor | Yes | GenericTable (ProductUnitConversionFactors) | No | ConversionFactor |

### Assumptions

N/A

## Work items

The table below describes de user stories that affect the current functionality

| User Story | Type       | Title                                   | Description |
| :--------: | :--------: | --------------------------------------- | ----------- |
| 161783     | User Story | Get Product Master Information from ERP |             |
