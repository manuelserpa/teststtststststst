using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.EntityTypeManagement;
using Cmf.Foundation.BusinessOrchestration.EntityTypeManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Base;

namespace Cmf.Custom.AMSOsram.Actions.ProcessRules.EntityTypes
{
	class CustomSorterJobDefinition : DeeDevBase
	{
		public override bool DeeTestCondition(Dictionary<string, object> Input)
		{
			//---Start DEE Condition Code---

			/* Description:
             *     DEE Action to create a CustomSorterJobDefinition Entity
            */

			return true;

			//---End DEE Condition Code---
		}

		public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
		{
			//---Start DEE Code---
			UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
			UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
			UseReference("", "Cmf.Foundation.BusinessOrchestration.EntityTypeManagement");
			UseReference("", "Cmf.Foundation.BusinessOrchestration.EntityTypeManagement.InputObjects");
			UseReference("", "Cmf.Foundation.Common.Exceptions");
			UseReference("", "Cmf.Foundation.Common");
			UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

			// Name of the entity to be generated
			string newEntityName = "CustomSorterJobDefinition";
			string propertyNameLogisticalProcess = "LogisticalProcess";
			string propertyNameMovementList = "MovementList";
			string propertyNameReadWaferId = "ReadWaferId";
			string propertyNameFlipWafer = "FlipWafer";
			string propertyNameAlignWafer = "AlignWafer";
			string propertyNameWaferIdOnBottom = "WaferIdOnBottom";
			string propertyNameTargetCarrierType = "TargetCarrierType";
			string propertyNameSourceCarrierType = "SourceCarrierType";

			//Only makes sense to proceed if entity type doesn't exist yet
			EntityType foundEntity = (EntityType)EntityTypeManagementOrchestration.GetAllEntityTypes(
				new GetAllEntityTypesInput()).EntityTypes.FirstOrDefault(E => string.Equals(E.Name, newEntityName, StringComparison.InvariantCultureIgnoreCase));

			if (foundEntity == null)
			{
				#region Create Entity Type

				// set new entity type to be created
				foundEntity = new EntityType()
				{
					Name = newEntityName,
					EntityTypeTypeName = newEntityName,
					Description = "Custom Sorter Job Definition",
					IsRelation = false,
					IsUniqueNameRequired = true,
					ReplicateToODS = true,
					IsHistoryEnabled = true,
					IsVisible = true,
					AllowAttributes = true,
					AllowOperationAttributes = true,
					HistoryRetentionTime = 30,
					HistoryDefaultInterval = 180
				};

				foundEntity = EntityTypeManagementOrchestration.CreateEntityType(new CreateEntityTypeInput() { EntityType = foundEntity }).EntityType;

				#endregion
			}

			#region Add properties

			// if entity is still in Created state, check if all necessary properties are already added
			if (foundEntity.UniversalState == UniversalState.Created)
			{
				// check if required properties 
				foundEntity.LoadAllProperties();

				ScalarType jsonScalarType = new ScalarType();
				jsonScalarType.Load("JSON");

				ScalarType stringScalarType = new ScalarType();
				stringScalarType.Load("NVarChar");

				ScalarType bitScalarType = new ScalarType();
				bitScalarType.Load("Bit");

				LookupTable lookupLogisticalProcess = new LookupTable();
				LookupTable lookupContainerType = new LookupTable();

				// Load all lookup tables
				LookupTableCollection lookupTables = new LookupTableCollection();
				lookupTables.LoadAll();

				// Key = Name, Value = Description
				Dictionary<string, string> lookupTablesToCreate = new Dictionary<string, string>
				{
					{ AMSOsramConstants.LookupTableCustomSorterLogisticalProcess, "Custom Sorter Logistical Processes" },
					{ AMSOsramConstants.LookupTableContainerType, "Possible types for Container"}
				};

				foreach (var lookupTableToCreate in lookupTablesToCreate)
				{
					var lookupTable = lookupTables.FirstOrDefault(E => string.Equals(E.Name, lookupTableToCreate.Key));

					// Check if table exists, if not creates the lookup table
					if (lookupTable == null)
					{
						lookupTable = new LookupTable
						{
							Name = lookupTableToCreate.Key,
							Description = lookupTableToCreate.Value
						};

						lookupTable.Create();
					}

					if (lookupTable.Name == AMSOsramConstants.LookupTableCustomSorterLogisticalProcess)
					{
						lookupLogisticalProcess = lookupTable;
					}

					if (lookupTable.Name == AMSOsramConstants.LookupTableContainerType)
					{
						lookupContainerType = lookupTable;
					}
				}

				#region Entity Type Properties

				EntityTypePropertyCollection propertiesToAdd = new EntityTypePropertyCollection();

				// LogisticalProcess
				if (!foundEntity.Properties.Any(E => String.Equals(E.Name, propertyNameLogisticalProcess, StringComparison.InvariantCultureIgnoreCase)))
				{
					// isERPFinalConfirmation
					EntityTypeProperty propLogisticalProcess = new EntityTypeProperty()
					{
						Name = propertyNameLogisticalProcess,
						Description = "Logistical Process",
						PropertyType = EntityTypePropertyType.Property,
						IsEnabled = true,
						ScalarType = stringScalarType,
						ScalarSize = 256,
						ReferenceType = ReferenceType.LookupValue,
						ReferenceName = lookupLogisticalProcess.Name,
						ReferencedObjectId = lookupLogisticalProcess.Id,
						IsMandatory = true,
						IsHistoryEnable = true
					};

					// add property to list of properties to add...
					propertiesToAdd.Add(propLogisticalProcess);
				}

				// TargetCarrierType
				if (!foundEntity.Properties.Any(E => String.Equals(E.Name, propertyNameTargetCarrierType, StringComparison.InvariantCultureIgnoreCase)))
				{
					// isERPFinalConfirmation
					EntityTypeProperty propLogisticalProcess = new EntityTypeProperty()
					{
						Name = propertyNameTargetCarrierType,
						Description = "Possible type for target Carrier",
						PropertyType = EntityTypePropertyType.Property,
						IsEnabled = true,
						ScalarType = stringScalarType,
						ScalarSize = 256,
						ReferenceType = ReferenceType.LookupValue,
						ReferenceName = lookupContainerType.Name,
						ReferencedObjectId = lookupContainerType.Id,
						IsMandatory = true,
						IsHistoryEnable = true
					};

					// add property to list of properties to add...
					propertiesToAdd.Add(propLogisticalProcess);
				}

				// SourceCarrierType
				if (!foundEntity.Properties.Any(E => String.Equals(E.Name, propertyNameSourceCarrierType, StringComparison.InvariantCultureIgnoreCase)))
				{
					// isERPFinalConfirmation
					EntityTypeProperty propLogisticalProcess = new EntityTypeProperty()
					{
						Name = propertyNameSourceCarrierType,
						Description = "Possible type for source Carrier",
						PropertyType = EntityTypePropertyType.Property,
						IsEnabled = true,
						ScalarType = stringScalarType,
						ScalarSize = 256,
						ReferenceType = ReferenceType.LookupValue,
						ReferenceName = lookupContainerType.Name,
						ReferencedObjectId = lookupContainerType.Id,
						IsMandatory = true,
						IsHistoryEnable = true
					};

					// add property to list of properties to add...
					propertiesToAdd.Add(propLogisticalProcess);
				}

				// IsValidated
				if (!foundEntity.Properties.Any(E => String.Equals(E.Name, propertyNameMovementList, StringComparison.InvariantCultureIgnoreCase)))
				{
					// isERPFinalConfirmation
					EntityTypeProperty propMovementList = new EntityTypeProperty()
					{
						Name = propertyNameMovementList,
						Description = "Movement List",
						PropertyType = EntityTypePropertyType.Property,
						IsEnabled = true,
						ScalarType = jsonScalarType,
						ReferenceType = ReferenceType.None,
						IsMandatory = false,
						IsHistoryEnable = true,
						ScalarSize = -1,
					};

					// add property to list of properties to add...
					propertiesToAdd.Add(propMovementList);
				}

				// Read Wafer Id
				if (!foundEntity.Properties.Any(E => String.Equals(E.Name, propertyNameReadWaferId, StringComparison.InvariantCultureIgnoreCase)))
				{
					// isERPFinalConfirmation
					EntityTypeProperty propReadWaferId = new EntityTypeProperty()
					{
						Name = propertyNameReadWaferId,
						Description = "Read Wafer Id",
						PropertyType = EntityTypePropertyType.Property,
						IsEnabled = true,
						ScalarType = bitScalarType,
						ReferenceType = ReferenceType.None,
						IsMandatory = true,
						IsHistoryEnable = true,
						DefaultValue = "0"
					};

					// add property to list of properties to add...
					propertiesToAdd.Add(propReadWaferId);
				}

				// Flip Wafer
				if (!foundEntity.Properties.Any(E => String.Equals(E.Name, propertyNameFlipWafer, StringComparison.InvariantCultureIgnoreCase)))
				{
					// isERPFinalConfirmation
					EntityTypeProperty propFlipWafer = new EntityTypeProperty()
					{
						Name = propertyNameFlipWafer,
						Description = "Flip Wafer",
						PropertyType = EntityTypePropertyType.Property,
						IsEnabled = true,
						ScalarType = bitScalarType,
						ReferenceType = ReferenceType.None,
						IsMandatory = true,
						IsHistoryEnable = true,
						DefaultValue = "0"
					};

					// add property to list of properties to add...
					propertiesToAdd.Add(propFlipWafer);
				}

				// Align Wafer
				if (!foundEntity.Properties.Any(E => String.Equals(E.Name, propertyNameAlignWafer, StringComparison.InvariantCultureIgnoreCase)))
				{
					// isERPFinalConfirmation
					EntityTypeProperty propAlignWafer = new EntityTypeProperty()
					{
						Name = propertyNameAlignWafer,
						Description = "Align Wafer",
						PropertyType = EntityTypePropertyType.Property,
						IsEnabled = true,
						ScalarType = bitScalarType,
						ReferenceType = ReferenceType.None,
						IsMandatory = true,
						IsHistoryEnable = true,
						DefaultValue = "0"
					};

					// add property to list of properties to add...
					propertiesToAdd.Add(propAlignWafer);
				}

				// Wafer Id On Bottom 
				if (!foundEntity.Properties.Any(E => String.Equals(E.Name, propertyNameWaferIdOnBottom, StringComparison.InvariantCultureIgnoreCase)))
				{
					// isERPFinalConfirmation
					EntityTypeProperty propWaferIdOnBottom = new EntityTypeProperty()
					{
						Name = propertyNameWaferIdOnBottom,
						Description = "Wafer Id On Bottom",
						PropertyType = EntityTypePropertyType.Property,
						IsEnabled = true,
						ScalarType = bitScalarType,
						ReferenceType = ReferenceType.None,
						IsMandatory = true,
						IsHistoryEnable = true,
						DefaultValue = "0"
					};

					// add property to list of properties to add...
					propertiesToAdd.Add(propWaferIdOnBottom);
				}

				#endregion

				// Add properties to entity
				if (propertiesToAdd.Count > 0)
				{
					foundEntity = EntityTypeManagementOrchestration.AddEntityTypeProperties(new AddEntityTypePropertiesInput()
					{
						EntityType = foundEntity,
						EntityTypeProperties = propertiesToAdd
					}).EntityType;

					bool isToUpdateEntity = false;

					FullUpdateEntityTypeInput input = new FullUpdateEntityTypeInput()
					{
						EntityType = foundEntity,
						EntityTypePropertiesToAddOrUpdate = new EntityTypePropertyCollection()
					};

					// FOR READ WAFER ID
					if (propertiesToAdd.Any(p => p.Name == propertyNameReadWaferId))
					{
						// Update the DefaultValue of property with type Bit because create service is not setting it.
						var propReadWaferId = foundEntity.Properties.FirstOrDefault(p => p.Name == propertyNameReadWaferId);

						if (propReadWaferId != null && string.IsNullOrWhiteSpace(propReadWaferId.DefaultValue))
						{
							propReadWaferId.DefaultValue = "0";
							input.EntityTypePropertiesToAddOrUpdate.Add(propReadWaferId);
							isToUpdateEntity = true;
						}
					}

					// FOR FLIP WAFER
					if (propertiesToAdd.Any(p => p.Name == propertyNameFlipWafer))
					{
						// Update the DefaultValue of property with type Bit because create service is not setting it.
						var propFlipWafer = foundEntity.Properties.FirstOrDefault(p => p.Name == propertyNameFlipWafer);

						if (propFlipWafer != null && string.IsNullOrWhiteSpace(propFlipWafer.DefaultValue))
						{
							propFlipWafer.DefaultValue = "0";
							input.EntityTypePropertiesToAddOrUpdate.Add(propFlipWafer);
							isToUpdateEntity = true;
						}
					}

					// FOR ALIGN WAFER 
					if (propertiesToAdd.Any(p => p.Name == propertyNameAlignWafer))
					{
						// Update the DefaultValue of property with type Bit because create service is not setting it.
						var propAlignWafer = foundEntity.Properties.FirstOrDefault(p => p.Name == propertyNameAlignWafer);

						if (propAlignWafer != null && string.IsNullOrWhiteSpace(propAlignWafer.DefaultValue))
						{
							propAlignWafer.DefaultValue = "0";
							input.EntityTypePropertiesToAddOrUpdate.Add(propAlignWafer);
							isToUpdateEntity = true;
						}
					}

					// FOR WAFER ID ON BOTTOM 
					if (propertiesToAdd.Any(p => p.Name == propertyNameWaferIdOnBottom))
					{
						// Update the DefaultValue of property with type Bit because create service is not setting it.
						var propWaferIdOnBottom = foundEntity.Properties.FirstOrDefault(p => p.Name == propertyNameWaferIdOnBottom);

						if (propWaferIdOnBottom != null && string.IsNullOrWhiteSpace(propWaferIdOnBottom.DefaultValue))
						{
							propWaferIdOnBottom.DefaultValue = "0";
							input.EntityTypePropertiesToAddOrUpdate.Add(propWaferIdOnBottom);
							isToUpdateEntity = true;
						}
					}

					if (isToUpdateEntity)
					{
						EntityTypeManagementOrchestration.FullUpdateEntityType(input);
					}
				}

				#region Generate Schema

				EntityTypeManagementOrchestration.GenerateEntityTypeDBSchema(new GenerateEntityTypeDBSchemaInput
				{
					EntityType = foundEntity
				});

				#endregion
			}

			#endregion

			//---End DEE Code---
			return Input;
		}
	}
}
