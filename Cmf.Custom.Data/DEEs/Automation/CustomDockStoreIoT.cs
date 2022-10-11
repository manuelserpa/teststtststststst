using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.Common;
using System.Collections.Generic;
using System.Linq;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using System;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Navigo.BusinessObjects;
using Cmf.Foundation.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.Automation
{
    public class CustomDockStoreIoT : DeeDevBase
	{
		public override bool DeeTestCondition(Dictionary<string, object> Input)
		{
			//---Start DEE Condition Code---

			#region Info

			/* Description:
             *     DEE Action is triggered by IoT Automation to dock or store carrier into resource.
             *
             * Action Groups:
             *      None
             *
            */

			#endregion Info

			return true;
			//---End DEE Condition Code---
		}

		public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
		{
			//---Start DEE Code---

			// Custom
			UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
			UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.DataStructures");
			UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");

			IContainer container = null;
			IResource resource = null;
			int? loadPortNumber = null;
			bool fromOnlineLoadPort = false;
			bool fromMCS = false;
			bool isStorageGroup = false;
			bool isTransportInvalid = false;

			// Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

			#region Validate Input Parameters

			// mandatory parameters
			string inputKey = "CarrierId";

			string containerName = amsOSRAMUtilities.GetInputItem<string>(Input, inputKey);

			if (string.IsNullOrWhiteSpace(containerName))
			{
				throw new ArgumentNullCmfException(inputKey);
			}

            container = entityFactory.Create<IContainer>();
			container.Name = containerName;

			if (!container.ObjectExists())
			{
				throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Container, container.Name);
			}

            container.Load();

			// Optional Parameter
			inputKey = "StorageGroup";
			string storageGroup = amsOSRAMUtilities.GetInputItem<string>(Input, inputKey);

            resource = entityFactory.Create<IResource>();
			resource.Name = storageGroup;

			// Mandatory Parameter
			inputKey = "ResourceId";
			string resourceName = amsOSRAMUtilities.GetInputItem<string>(Input, inputKey);

			if (string.IsNullOrWhiteSpace(resourceName))
			{
				throw new ArgumentNullCmfException(inputKey);
			}

			if (resource.ObjectExists())
			{
				// Load StorageGroup Resource
				resource.Load();

				isStorageGroup = resource.ProcessingType == ProcessingType.Storage;
			}

			if (!isStorageGroup)
			{
                resource = entityFactory.Create<IResource>();
                resource.Name = resourceName;

				if (!resource.ObjectExists())
				{
					throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Resource, resource.Name);
				}
				else
				{
					resource.Load();
				}
			}

			// Optional Parameters
			inputKey = "LoadPortNumber";

			if (Input.ContainsKey(inputKey) &&
				Input[inputKey] != null)
			{
				loadPortNumber = int.Parse(Input[inputKey].ToString());
				Input.Add("LoadPort", loadPortNumber);
			}

			// Optional Parameters
			inputKey = "FromOnlineLoadPort";
			fromOnlineLoadPort = amsOSRAMUtilities.GetInputItem(Input, inputKey, false);

			inputKey = "FromTransportSystem";
			fromMCS = amsOSRAMUtilities.GetInputItem(Input, inputKey, false);

			#endregion Validate Input Parameters

			#region Calculate load port based on loadPortNumber

			if (loadPortNumber != null)
			{
				IResourceHierarchy resourceHierarchy = resource.GetDescendentResources(1);

				if (resourceHierarchy != null &&
					resourceHierarchy.Count > 0)
				{
					IResource possibleLoadPort = resourceHierarchy.FirstOrDefault(s => s.ChildResource.DisplayOrder != null &&
						s.ChildResource.DisplayOrder.Value == loadPortNumber &&
						s.ChildResource.ProcessingType == ProcessingType.LoadPort).ChildResource;

					if (possibleLoadPort != null)
					{
						resource = possibleLoadPort;
						resource.Load();
					}
				}
			}

			#endregion Calculate load port based on loadPortNumber

			if (container.ResourceAssociationType == ContainerResourceAssociationType.StoredContainer)
			{
				container.Retrieve();
			}
			else if (container.ResourceAssociationType == ContainerResourceAssociationType.DockedContainer)
			{
				if (resource.AutomationMode != ResourceAutomationMode.Online)
				{
					container.Undock();
				}
			}

			switch (resource.ProcessingType)
			{
				case ProcessingType.Storage:
					if (isStorageGroup)
					{
						container.Store(
							resource: resource,
							position: null,
							positionName: resourceName,
							new OperationAttributeCollection());
					}
					else
					{
						container.Store(resource);
					}

					container.SaveAttributes(
						new AttributeCollection() {
							{ "TransportRequested", false }
						}
					);
					break;

				case ProcessingType.LoadPort:
					if ((resource.AutomationMode == ResourceAutomationMode.Online && !fromMCS) ||
						(resource.AutomationMode != ResourceAutomationMode.Online && fromMCS))
					{
						container.Dock(resource);

						#region Validate Dock

						container.LoadRelations(Navigo.Common.Constants.MaterialContainer);

						if (container.ContainerMaterials != null &&
								container.ContainerMaterials.Count > 0)
						{
							JArray containerMaterials = new JArray();

							foreach (MaterialContainer materialInContainer in container.ContainerMaterials)
							{
								JObject slotInformation = new JObject
								{
									["Slot"] = materialInContainer.Position,
									["EquipmentWaferId"] = null,
									["MaterialWaferId"] = materialInContainer.SourceEntity.Name,
									["ParentMaterialName"] = materialInContainer.SourceEntity.ParentMaterial?.Name ?? String.Empty
								};

								containerMaterials.Add(slotInformation);
							}

							Input.Add("ContainerMaterials", containerMaterials.ToString());

							// var parentMaterials = container.ContainerMaterials.Where(c => c.SourceEntity.ParentMaterial != null).Select(s => s.SourceEntity.ParentMaterial).DistinctBy(m => m.Id);

							// List<MaterialData> materials = new List<MaterialData>();

							// foreach (Material material in parentMaterials)
							// {
							// 	material.Load();
							// 	material.LoadChildren(1);
							// 	material.LoadRelations(Navigo.Common.Constants.MaterialContainer);
							// 	material.SubMaterials.LoadRelations(Navigo.Common.Constants.MaterialContainer);

							// 	MaterialData materialData = new MaterialData
							// 	{
							// 		MaterialId = material.Id.ToString(),
							// 		MaterialName = material.Name,
							// 		MaterialState = material.SystemState.ToString(),
							// 		ContainerId = container.Id.ToString(),
							// 		ContainerName = container.Name
							// 	};

							// 	if (material.SubMaterialCount > 0)
							// 	{
							// 		materialData.SubMaterials = new List<MaterialData>();

							// 		foreach (var subMaterial in material.SubMaterials)
							// 		{
							// 			MaterialData subMaterialData = new MaterialData()
							// 			{
							// 				MaterialId = subMaterial.Id.ToString(),
							// 				MaterialName = subMaterial.Name,
							// 				MaterialState = subMaterial.SystemState.ToString()
							// 			};

							// 			if (subMaterial.MaterialContainer != null &&
							// 				subMaterial.MaterialContainer.Count > 0)
							// 			{
							// 				if (subMaterial.MaterialContainer.First().TargetEntity.Id == container.Id)
							// 				{
							// 					subMaterialData.Slot = subMaterial.MaterialContainer.First().Position.ToString();
							// 					materialData.SubMaterials.Add(subMaterialData);
							// 				}
							// 			}
							// 		}
							// 	}

							// 	materials.Add(materialData);
							// }

							// Input.Add("ContainerMaterialData", materials.ToJsonString());
						}


						#endregion Validate Dock
					}
					break;

				default:
					break;
			}

			if (isTransportInvalid)
			{
				Input.Add("DockIsInvalid", isTransportInvalid);
			}
			else
			{
				Input.Add("DockIsValid", !isTransportInvalid);
			}

			//---End DEE Code---

			return Input;
		}
	}
}
