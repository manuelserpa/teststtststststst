using Cmf.Custom.AMSOsram.Actions;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using System.Collections.Generic;
using System.Linq;

namespace Cmf.Custom.AMSOsram.Actions.Automation
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
			// Foundation
			UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
			UseReference("", "Cmf.Foundation.BusinessObjects.SmartTables");
			UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
			UseReference("", "Cmf.Foundation.Common.Exceptions");
			UseReference("", "Cmf.Foundation.Common");
			// Navigo
			UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
			UseReference("Cmf.Navigo.Common.dll", "Cmf.Navigo.Common");
			UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
			// System
			UseReference("", "System.Data");
			UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");

			Container container = null;
			Resource resource = null;
			int? loadPortNumber = null;
			bool fromOnlineLoadPort = false;
			bool fromMCS = false;
			bool isStorageGroup = false;
			bool isTransportInvalid = false;

			#region Validate Input Parameters

			// mandatory parameters
			string inputKey = "CarrierId";

			
			string containerName = AMSOsramUtilities.GetInputItem<string>(Input, inputKey);

			if (string.IsNullOrWhiteSpace(containerName))
			{
				throw new ArgumentNullCmfException(inputKey);
			}
			else
			{
				container = new Container() { Name = containerName };

				if (!container.ObjectExists())
				{
					throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Container, container.Name);
				}
				else
				{
					container.Load();
				}
			}

			// Optional Parameter
			inputKey = "StorageGroup";
			string storageGroup = AMSOsramUtilities.GetInputItem<string>(Input, inputKey);
			resource = new Resource() { Name = storageGroup };

			// Mandatory Parameter
			inputKey = "ResourceId";
			string resourceName = AMSOsramUtilities.GetInputItem<string>(Input, inputKey);

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
				resource = new Resource() { Name = resourceName };

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
			fromOnlineLoadPort = AMSOsramUtilities.GetInputItem<bool>(Input, inputKey, false);

			inputKey = "FromTransportSystem";
			fromMCS = AMSOsramUtilities.GetInputItem<bool>(Input, inputKey, false);

			#endregion Validate Input Parameters

			#region Calculate load port based on loadPortNumber

			if (loadPortNumber != null)
			{
				var resourceHierarchy = resource.GetDescendentResources(1);

				if (resourceHierarchy != null &&
					resourceHierarchy.Count > 0)
				{
					var possibleLoadPort = resourceHierarchy.FirstOrDefault(s => s.ChildResource.DisplayOrder != null &&
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
							new Foundation.BusinessObjects.OperationAttributeCollection());
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

						container.LoadRelations(Cmf.Navigo.Common.Constants.MaterialContainer);

						if (container.ContainerMaterials != null &&
							container.ContainerMaterials.Count > 0)
						{
							if (resource.LoadPortType == LoadPortType.Output) // CHECK IF CONTAINER HAS CASSETTE (TODO)
							{
								isTransportInvalid = true;
								break;
							}

							Material topMostMaterial = (container.RelationCollection[Cmf.Navigo.Common.Constants.MaterialContainer][0] as MaterialContainer).SourceEntity;

							if (topMostMaterial.TopMostMaterial != null)
							{
								topMostMaterial = topMostMaterial.TopMostMaterial;
							}

							if (topMostMaterial.HoldCount > 0)
							{
								isTransportInvalid = true;
								break;
							}

							Step step = topMostMaterial.Step;
							ResourceCollection resourcesOnStep = step.GetResourcesForStep();
							Resource parentResource = resource.GetTopMostResource();

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