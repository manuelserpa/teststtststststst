using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.Extensions;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cmf.Custom.AMSOsram.Actions.Automation
{
	class CustomSetWaferToContainerSlot : DeeDevBase
	{
		/// <summary>
		/// Dee test condition.
		/// </summary>
		/// <param name="Input">The input.</param>
		/// <returns></returns>
		public override bool DeeTestCondition(Dictionary<string, object> Input)
		{
			//---Start DEE Condition Code---

			#region Info

			/* Description:
             *     Dee action is triggered by IoT Automation to put a wafer based on:
             *     - MapCarrier (wafer transfer in the same container)
             *     - TransferWafers (wafer transfer beetween different containers)            
             *  
             * Action Groups:
             *      None
             *     
            */

			#endregion

			return true;

			//---End DEE Condition Code---
		}

		/// <summary>
		/// Dee action code.
		/// </summary>
		/// <param name="Input">The input.</param>
		/// <returns></returns>
		public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
		{
			//---Start DEE Code---
			UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
			UseReference("", "Cmf.Foundation.BusinessObjects.SmartTables");
			UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
			UseReference("", "Cmf.Foundation.Common.Exceptions");
			UseReference("", "Cmf.Foundation.Common");
			UseReference("Cmf.Foundation.Configuration.dll", "Cmf.Foundation.Configuration");
			// Navigo
			UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
			UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.FacilityManagement.FlowManagement.InputObjects");
			// Custom
			UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
			UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.Extensions");

			bool createInventory = false;
			string resourceName = string.Empty;

			if (Input.ContainsKey("CreateInventory"))
			{
				createInventory = (bool)Input["CreateInventory"];
			}

			if (Input.ContainsKey("ResourceName"))
			{
				resourceName = (Input["ResourceName"] as String).Trim();
			}

			bool createInventoryFailed = createInventory;

			// WaferId
			if (!Input.ContainsKey("WaferId"))
			{
				throw new ArgumentNullCmfException("WaferId");
			}

			Material material = new Material() { Name = (Input["WaferId"] as String).Trim() };

			// CarrierId
			if (!Input.ContainsKey("CarrierId"))
			{
				throw new ArgumentNullCmfException("CarrierId");
			}

			Container targetContainer = new Container() { Name = Input["CarrierId"] as String };

			// SlotNumber
			if (!Input.ContainsKey("SlotNumber"))
			{
				throw new ArgumentNullCmfException("SlotNumber");
			}

			int.TryParse(Input["SlotNumber"].ToString(), out int slotNumber);

			bool skipAssociate = false;
			targetContainer.Load();

			if (createInventory)
			{
				#region Create material on MES if does not exists
				if (!material.ObjectExists())
				{
					targetContainer.LoadAttributes(new Collection<string> {
						AMSOsramConstants.ContainerAttributeProduct
					});

					targetContainer.Attributes.TryGetValueAs(AMSOsramConstants.ContainerAttributeProduct, out string productName);

					Product product = new Product { Name = productName };

					if (product.ObjectExists())
					{
						product.Load();
						product.LoadAttributes(new Collection<string> {
							AMSOsramConstants.ProductAttributeCanCreateInventory
						});

						product.RelatedAttributes.TryGetValueAs(AMSOsramConstants.ProductAttributeCanCreateInventory, out bool productCanCreateInventory);

						if (productCanCreateInventory)
						{
							Step step = product.Step;
							ResourceCollection resourcesOnStep = step.GetResourcesForStep();

							Resource resource = resourcesOnStep.FirstOrDefault(r => r.Name == resourceName);

							if (resource != null && resource.Area != null)
							{
								Area area = resource.Area;
								area.Load();

								material.Description = product.Description;
								material.Facility = area.Facility;
								material.Product = product;
								material.Form = product.DefaultMaterialForm;
								material.Type = product.DefaultMaterialType;
								material.Flow = product.Flow;
								material.Step = product.Step;
								material.FlowPath = product.FlowPath;
								material.PrimaryQuantity = 1;
								material.PrimaryUnits = product.DefaultUnits; // CHECK

								// Create the material
								material.Create();
								createInventoryFailed = false;
							}
						}
					}
				}
				#endregion
			}

			if (createInventoryFailed)
			{
				Input.Add("CreateInventoryFailed", createInventoryFailed);
			}
			else
			{
				material.Load();
				material.LoadRelations("MaterialContainer");

				targetContainer.LoadRelations("MaterialContainer");
				MaterialContainerCollection materialsOnContainer = targetContainer.ContainerMaterials ?? new MaterialContainerCollection();

				if (material.MaterialContainer != null && material.MaterialContainer.Count > 0)
				{
					// if the target slot is occupied, then we need to remove the existing material from position
					var previousSlot = materialsOnContainer.FirstOrDefault(s => (s.Position ?? 0) == slotNumber);
					if (previousSlot != null)
					{
						// if target slot is occupied by the correct material, then we can skip the rest of the execution
						// Otherwise remove existing wafer from container
						if (previousSlot.GetNativeValue<long>("SourceEntity") != material.Id)
						{
							targetContainer.DisassociateMaterials(new MaterialContainerCollection { previousSlot });
							materialsOnContainer = targetContainer.ContainerMaterials ?? new MaterialContainerCollection();
						}
						else
						{
							// Skip Associate
							skipAssociate = true;
						}
					}

					if (!skipAssociate)
					{
						MaterialContainer currentMaterialContainer = materialsOnContainer.FirstOrDefault(s => s.GetNativeValue<long>("SourceEntity") == material.Id);

						if (currentMaterialContainer != null)
						{
							// if material is already in the container, update slot position
							if (currentMaterialContainer.Position != slotNumber)
							{
								currentMaterialContainer.Position = slotNumber;
								targetContainer.UpdateMaterialPositions(new MaterialContainerCollection { currentMaterialContainer });
							}
							// else (slot position is already the correct one) no change will be needed
						}
						else
						{
							// new Material Container need to be created
							currentMaterialContainer = new MaterialContainer()
							{
								TargetEntity = targetContainer,
								SourceEntity = material,
								Position = slotNumber
							};

							// If material is in other container, then Transfer
							material.LoadRelations("MaterialContainer");
							if (material.RelationCollection.ContainsKey("MaterialContainer"))
							{
								// Container exists in the material, need to verify if is different from the target container
								MaterialContainer existingMaterialContainer = material.RelationCollection["MaterialContainer"].FirstOrDefault() as MaterialContainer;
								if (existingMaterialContainer != null &&
									existingMaterialContainer.GetNativeValue<long>("TargetEntity") != targetContainer.Id)
								{
									// Transfer Material to new Container
									existingMaterialContainer.TargetEntity.TransferMaterials(new MaterialContainerCollection { currentMaterialContainer });
								}
							}
							else
							{
								// Associate material to container
								targetContainer.AssociateMaterials(new MaterialContainerCollection { currentMaterialContainer });
							}
						}
					}
				}
				else // The material is not associated with any container whatsoever
				{
					MaterialContainer materialContainer = new MaterialContainer()
					{
						TargetEntity = targetContainer,
						SourceEntity = material,
						Position = slotNumber
					};

					// Associate material to container
					targetContainer.AssociateMaterials(new MaterialContainerCollection { materialContainer });
				}
			}
			//---End DEE Code---

			return Input;
		}
	}
}
