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
							if (resource.LoadPortType == LoadPortType.Output) // CHECK IF CONTAINER HAS CASSETTE (TODO)
							{
								isTransportInvalid = true;
								break;
							}

							IMaterial topMostMaterial = (container.RelationCollection[Navigo.Common.Constants.MaterialContainer][0] as IMaterialContainer).SourceEntity;

							if (topMostMaterial.TopMostMaterial != null)
							{
								topMostMaterial = topMostMaterial.TopMostMaterial;
							}

							if (topMostMaterial.HoldCount > 0)
							{
								isTransportInvalid = true;
								break;
							}

							IStep step = topMostMaterial.Step;
							IResourceCollection resourcesOnStep = step.GetResourcesForStep();
							IResource parentResource = resource.GetTopMostResource();

							if (!resourcesOnStep.Any(r => r.Name == parentResource.Name))
							{
								isTransportInvalid = true;
								break;
							}
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
