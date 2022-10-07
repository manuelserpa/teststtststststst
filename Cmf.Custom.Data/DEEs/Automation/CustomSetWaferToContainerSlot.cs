using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.Abstractions;

namespace Cmf.Custom.amsOSRAM.Actions.Automation
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

			// Custom
			UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
			UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.Extensions");

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

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

			IMaterial material = entityFactory.Create<IMaterial>();
            material.Name = (Input["WaferId"] as string).Trim();

			// CarrierId
			if (!Input.ContainsKey("CarrierId"))
			{
				throw new ArgumentNullCmfException("CarrierId");
			}

			IContainer targetContainer = entityFactory.Create<IContainer>();
            targetContainer.Name = Input["CarrierId"] as String;

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
						amsOSRAMConstants.ContainerAttributeProduct
					});

					targetContainer.Attributes.TryGetValueAs(amsOSRAMConstants.ContainerAttributeProduct, out string productName);

					IProduct product = entityFactory.Create<IProduct>();
                    product.Name = productName;

					if (product.ObjectExists())
					{
						product.Load();
						product.LoadAttributes(new Collection<string> {
							amsOSRAMConstants.ProductAttributeCanCreateInventory
						});

						product.RelatedAttributes.TryGetValueAs(amsOSRAMConstants.ProductAttributeCanCreateInventory, out bool productCanCreateInventory);

						if (productCanCreateInventory)
						{
							IStep step = product.Step;
							IResourceCollection resourcesOnStep = step.GetResourcesForStep();

							IResource resource = resourcesOnStep.FirstOrDefault(r => r.Name == resourceName);

							if (resource != null && resource.Area != null)
							{
								IArea area = resource.Area;
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
				IMaterialContainerCollection materialsOnContainer = targetContainer.ContainerMaterials ?? entityFactory.CreateCollection<IMaterialContainerCollection>();

				if (material.MaterialContainer != null && material.MaterialContainer.Count > 0)
				{
					// if the target slot is occupied, then we need to remove the existing material from position
					IMaterialContainer previousSlot = materialsOnContainer.FirstOrDefault(s => (s.Position ?? 0) == slotNumber);
					if (previousSlot != null)
					{
						// if target slot is occupied by the correct material, then we can skip the rest of the execution
						// Otherwise remove existing wafer from container
						if (previousSlot.GetNativeValue<long>("SourceEntity") != material.Id)
						{
                            IMaterialContainerCollection previousSlotColletion = entityFactory.CreateCollection<IMaterialContainerCollection>();
							previousSlotColletion.Add(previousSlot);

                            targetContainer.DisassociateMaterials(previousSlotColletion);
							materialsOnContainer = targetContainer.ContainerMaterials ?? entityFactory.CreateCollection<IMaterialContainerCollection>();
						}
						else
						{
							// Skip Associate
							skipAssociate = true;
						}
					}

					if (!skipAssociate)
					{
						IMaterialContainer currentMaterialContainer = materialsOnContainer.FirstOrDefault(s => s.GetNativeValue<long>("SourceEntity") == material.Id);

						if (currentMaterialContainer != null)
						{
							// if material is already in the container, update slot position
							if (currentMaterialContainer.Position != slotNumber)
							{
								currentMaterialContainer.Position = slotNumber;

								IMaterialContainerCollection currentMaterialContainerCollection = entityFactory.CreateCollection<IMaterialContainerCollection>();
								currentMaterialContainerCollection.Add(currentMaterialContainer);

                                targetContainer.UpdateMaterialPositions(currentMaterialContainerCollection);
							}
							// else (slot position is already the correct one) no change will be needed
						}
						else
						{
							// new Material Container need to be created

							currentMaterialContainer = entityFactory.Create<IMaterialContainer>();
							currentMaterialContainer.TargetEntity = targetContainer;
							currentMaterialContainer.SourceEntity = material;
                            currentMaterialContainer.Position = slotNumber;

							// If material is in other container, then Transfer
							material.LoadRelations("MaterialContainer");
							if (material.RelationCollection.ContainsKey("MaterialContainer"))
							{
								// Container exists in the material, need to verify if is different from the target container
								IMaterialContainer existingMaterialContainer = material.RelationCollection["MaterialContainer"].FirstOrDefault() as IMaterialContainer;
								if (existingMaterialContainer != null &&
									existingMaterialContainer.GetNativeValue<long>("TargetEntity") != targetContainer.Id)
								{
                                    // Transfer Material to new Container
                                    IMaterialContainerCollection materialContainerCollection = entityFactory.CreateCollection<IMaterialContainerCollection>();
									materialContainerCollection.Add(currentMaterialContainer);

                                    existingMaterialContainer.TargetEntity.TransferMaterials(materialContainerCollection);
								}
							}
							else
							{
                                // Associate material to container
                                IMaterialContainerCollection materialContainerCollection = entityFactory.CreateCollection<IMaterialContainerCollection>();
                                materialContainerCollection.Add(currentMaterialContainer);

                                targetContainer.AssociateMaterials(materialContainerCollection);
							}
						}
					}
				}
				else // The material is not associated with any container whatsoever
				{
					IMaterialContainer materialContainer = entityFactory.Create<IMaterialContainer>();
					materialContainer.TargetEntity = targetContainer;
					materialContainer.SourceEntity = material;
                    materialContainer.Position = slotNumber;

                    IMaterialContainerCollection materialContainerCollection = entityFactory.CreateCollection<IMaterialContainerCollection>();
					materialContainerCollection.Add(materialContainer);

                    // Associate material to container
                    targetContainer.AssociateMaterials(materialContainerCollection);
				}
			}

			//---End DEE Code---

			return Input;
		}
	}
}
