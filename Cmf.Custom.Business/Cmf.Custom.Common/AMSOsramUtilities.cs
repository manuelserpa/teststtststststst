using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.BusinessObjects;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Custom.AMSOsram.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using Cmf.Navigo.BusinessObjects;
using Newtonsoft.Json.Linq;

namespace Cmf.Custom.AMSOsram.Common
{
	/// <summary>
	/// Support class to encapsulate methods to support the development for the business layer
	/// </summary>
    public class AMSOsramUtilities
    {

		#region Configs

		/// <summary>
		/// Retrieves the config value for the input key
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="configName"></param>
		/// <returns></returns>
		public static T GetConfig<T>(string configName)
		{
			Config config = Config.GetConfig(configName);
			if (config != null && config.Value != null)
			{
				return (T)config.Value;
			}
			else
			{
				throw new Exception(string.Format(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name, AMSOsramConstants.LocalizedMessageConfigNotFound).MessageText, configName));
			}
		}

        #endregion Configs

        #region SmartTables

		public static Dictionary<string, string> CustomResolveSTCustomMaterialNiceLabelPrintContext(string step = null,
																			string logicalFlowPath = null,
																			string product = null, 
																			string productGroup = null,
																			string flow = null,
																			string material = null,
																			string materialType = null,
																			string resource = null,
																			string resourceType = null,
																			string model = null,
																			string operation = null)
        {
			Dictionary<string, string> result = new Dictionary<string, string>();
			SmartTable customMaterialNiceLabelPrintContext = new SmartTable();
			customMaterialNiceLabelPrintContext.Load(AMSOsramConstants.CustomMaterialNiceLabelPrintContextSmartTable);

			var values = new NgpDataRow();

			// If step name is filled apply it as a filter
			if (!string.IsNullOrWhiteSpace(step))
			{
				values.Add(Cmf.Navigo.Common.Constants.Step, step);
			}

			// If logical flow path is filled apply it as a filter
			if (!string.IsNullOrWhiteSpace(logicalFlowPath))
			{
				values.Add("LogicalFlowPath", logicalFlowPath);
			}

			// If product name is filled apply it as a filter
			if (!string.IsNullOrWhiteSpace(product))
			{
				values.Add(Cmf.Navigo.Common.Constants.Product, product);
			}

			// If product group is filled apply it as a filter
			if (!string.IsNullOrWhiteSpace(productGroup))
			{
				values.Add(Cmf.Navigo.Common.Constants.ProductGroup, productGroup);
			}

			// If flow name is filled apply it as a filter
			if (!string.IsNullOrWhiteSpace(flow))
			{
				values.Add(Cmf.Navigo.Common.Constants.Flow, flow);
			}

			// If material name is filled apply it as a filter
			if (!string.IsNullOrWhiteSpace(material))
			{
				values.Add(Cmf.Navigo.Common.Constants.Material, material);
			}

			// If material type is filled apply it as a filter
			if (!string.IsNullOrWhiteSpace(materialType))
			{
				values.Add(Cmf.Navigo.Common.Constants.MaterialType, materialType);
			}

			// If resource name is filled apply it as a filter
			if (!string.IsNullOrWhiteSpace(resource))
			{
				values.Add(Cmf.Navigo.Common.Constants.Resource, resource);
			}

			// If resource type is filled apply it as a filter
			if (!string.IsNullOrWhiteSpace(resourceType))
			{
				values.Add(Cmf.Navigo.Common.Constants.ResourceType, resourceType);
			}

			// If model is filled apply it as a filter
			if (!string.IsNullOrWhiteSpace(model))
			{
				values.Add(Cmf.Navigo.Common.Constants.Model, model);
			}

			// If operation name is filled apply it as a filter
			if (!string.IsNullOrWhiteSpace(operation))
			{
				values.Add(Cmf.Navigo.Common.Constants.Operation, operation);
			}

			NgpDataSet niceLabelPrintContextNgpDataSet = customMaterialNiceLabelPrintContext.Resolve(values, true);

            if (niceLabelPrintContextNgpDataSet != null)
            {
				DataSet niceLabelPrintContextDataSet = NgpDataSet.ToDataSet(niceLabelPrintContextNgpDataSet);
                if (niceLabelPrintContextDataSet.HasData())
                {
					DataRow row = niceLabelPrintContextDataSet.Tables[0].Rows[0];

                    if (row.Field<bool>("IsEnabled"))
                    {
                        result.AddRange(new Dictionary<string, string>()
						{
							{ AMSOsramConstants.CustomMaterialNiceLabelPrintContextPrinter, row.Field<string>(AMSOsramConstants.CustomMaterialNiceLabelPrintContextPrinter) },
							{ AMSOsramConstants.CustomMaterialNiceLabelPrintContextLabel, row.Field<string>(AMSOsramConstants.CustomMaterialNiceLabelPrintContextLabel) },
							{ AMSOsramConstants.CustomMaterialNiceLabelPrintContextQuantity, row.Field<int>(AMSOsramConstants.CustomMaterialNiceLabelPrintContextQuantity).ToString() }
						}); 
                    }
				}
			}

			return result;
		}

		#endregion

		#region Sorter

		/// <summary>
		/// Resolve CustomSorterJobDefinitionContext
		/// </summary>
		/// <param name="material">The material to trackin</param>
		/// <returns>The custom sorter job definition</returns>
		public static CustomSorterJobDefinition GetSorterJobDefinition(Material material)
		{
			CustomSorterJobDefinition customSorterJobDefinition = null;

			string stepName = material.Step.Name;
			string materialName = material.Name;
			string productName = material.Product.Name;
			string flowName = material.GetStepParentFlow().Name;
			string materialType = material.Type;
			// Product group isn't mandatory, we have to null check it
			string productGroup = material.Product.ProductGroup?.Name ?? null;

			//Load SmartTable
			SmartTable productTypeRoleSmartTable = new SmartTable() { Name = AMSOsramConstants.CustomSorterJobDefinitionContextName };

			productTypeRoleSmartTable.Load();

			NgpDataRow resolveKeys = new NgpDataRow
			{
				{ AMSOsramConstants.CustomSorterJobDefinitionContextColumnStep, stepName },
				{ AMSOsramConstants.CustomSorterJobDefinitionContextColumnMaterial, materialName },
				{ AMSOsramConstants.CustomSorterJobDefinitionContextColumnProduct, productName },
				{ AMSOsramConstants.CustomSorterJobDefinitionContextColumnFlow, flowName },
				{ AMSOsramConstants.CustomSorterJobDefinitionContextColumnMaterialType, materialType },
				{ AMSOsramConstants.CustomSorterJobDefinitionContextColumnProductGroup, productGroup }
			};

			NgpDataSet resolvedData = productTypeRoleSmartTable.Resolve(resolveKeys, true);

			DataSet dataset = NgpDataSet.ToDataSet(resolvedData);

			if (dataset != null && dataset.Tables != null && dataset.Tables.Count != 0 && dataset.Tables[0].Rows.Count > 0)
			{
				string customSorterJobDefinitionName = dataset.Tables[0].Rows[0][AMSOsramConstants.CustomSorterJobDefinitionContextColumnCustomSorterJobDefinition].ToString();
				customSorterJobDefinition = new CustomSorterJobDefinition { Name = customSorterJobDefinitionName };
				customSorterJobDefinition.Load();

				// Set the CustomSorterJobDefinitionName in the context for future usage
				ApplicationContext.CallContext.SetInformationContext("CustomSorterJobDefinitionName", customSorterJobDefinition.Name);
			}

			return customSorterJobDefinition;
		}

		/// <summary>
		/// Check if it is possible to process the Sorter Job Definition
		/// If so, sets up contexts for future reference
		/// </summary>
		/// <param name="customSorterJobDefinition">Sorter Job Definition</param>
		/// <param name="currentContainer">Container</param>
		/// <param name="resource">Resource</param>
		/// <param name="currentLoadPort">Load Port</param>
		/// <returns>True or false if defined criteria is met</returns>
		public static bool CanProcessSorterJobDefinition(ref CustomSorterJobDefinition customSorterJobDefinition, Container currentContainer, Resource resource, Resource currentLoadPort, BOM bom = null)
		{
			bool canStartProcess = true;

			// Current Container must match with Source Carrier type defined on custom Sorter Job Definition
			// Otherwise we cannot do the track in
			if (customSorterJobDefinition.SourceCarrierType == currentContainer.Type)
			{
				// Only need to take into account scenarios other than 'Map Carrier'
				if (customSorterJobDefinition.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessTransferWafers)
				{
					JArray movementList = JArray.Parse(customSorterJobDefinition.MovementList);

					if (movementList != null)
					{
						// with this kind of mapping we have the flexibility to know the number of free position that we will need
						// in the destination container
						Dictionary<string, List<Tuple<int, int>>> movementListMap = new Dictionary<string, List<Tuple<int, int>>>();
						Dictionary<string, List<Tuple<int, int>>> movementListMapSource = new Dictionary<string, List<Tuple<int, int>>>();
						Dictionary<string, List<Tuple<int, int>>> finalMovementMap = new Dictionary<string, List<Tuple<int, int>>>();
						bool isMerge = false;

						foreach (JToken movement in movementList)
						{
							// Source
							string sourceContainer = movement.Value<string>("SourceContainer");
							int sourcePosition = movement.Value<int>("SourcePosition");
							// Destination
							string destinationContainer = movement.Value<string>("DestinationContainer");
							int destinationPosition = movement.Value<int>("DestinationPosition");

							// For Destination
							if (movementListMap.ContainsKey(destinationContainer))
							{
								movementListMap[destinationContainer].Add(Tuple.Create(sourcePosition, destinationPosition));
							}
							else
							{
								movementListMap[destinationContainer] = new List<Tuple<int, int>>
								{
									Tuple.Create(sourcePosition, destinationPosition)
								};
							}

							// For Source
							if (movementListMapSource.ContainsKey(sourceContainer))
							{
								movementListMapSource[sourceContainer].Add(Tuple.Create(sourcePosition, destinationPosition));
							}
							else
							{
								movementListMapSource[sourceContainer] = new List<Tuple<int, int>>
								{
									Tuple.Create(sourcePosition, destinationPosition)
								};
							}
						}

						int numberOfContainersNeeded = movementListMap.Keys.Count;

						// Merge detected
						if (movementListMapSource.Keys.Count > 1)
						{
							numberOfContainersNeeded = movementListMapSource.Keys.Count;
							movementListMap = movementListMapSource;
							isMerge = true;
						}

						movementListMap = movementListMap.OrderByDescending(o => o.Value.Count()).ToDictionary(x => x.Key, x => x.Value); ;

						if (numberOfContainersNeeded > 0)
						{
							int numberOfContainersFoundThatMatchTheCriteria = 0;
							Dictionary<Resource, Container> loadPortsThatMatchTheCriteria = new Dictionary<Resource, Container>();
							// get resource descendents
							ResourceHierarchy descendents = resource.GetDescendentResources(1);
							List<Resource> loadPorts = descendents.Where(s => s.ChildResource.ProcessingType == ProcessingType.LoadPort)
								.Select(s => s.ChildResource)
								.ToList();

							List<Container> containersOnResourceLoadPortReadyForProcessing = new List<Container>();
							foreach (Resource loadPort in loadPorts.Where(l => l.Id != currentLoadPort.Id))
							{
								loadPort.Load();
								loadPort.LoadAttributes(new Collection<string> { AMSOsramConstants.ResourceAttributeIsLoadPortInUse });
								bool isLoadPortInUse = false;

								if (loadPort.Attributes != null)
								{
									loadPort.Attributes.TryGetValueAs(AMSOsramConstants.ResourceAttributeIsLoadPortInUse, out isLoadPortInUse);
								}

								// If load port is available we'll check if exists a valid lot for processing
								if (!isLoadPortInUse)
								{
									loadPort.LoadRelations("ContainerResource");

									// If exists any resource container collection
									if (loadPort.RelationCollection.ContainsKey("ContainerResource"))
									{
										ContainerResource containerResourceOnOtherLoadPort = loadPort.ResourceContainers.FirstOrDefault();
										Container containerOnOtherLoadPort = containerResourceOnOtherLoadPort.SourceEntity;
										containerOnOtherLoadPort.Load();

										if (customSorterJobDefinition.TargetCarrierType == containerOnOtherLoadPort.Type)
										{
											containersOnResourceLoadPortReadyForProcessing.Add(containerOnOtherLoadPort);
										}
									}
								}
							}

							if (numberOfContainersNeeded <= containersOnResourceLoadPortReadyForProcessing.Count)
							{
								foreach (var movement in movementListMap)
								{
									Container container = null;

									if (isMerge)
									{
										container = containersOnResourceLoadPortReadyForProcessing.Where(s => (s.UsedPositions ?? 0) >= movement.Value.Count()).FirstOrDefault();
									}
									else
									{
										container = containersOnResourceLoadPortReadyForProcessing.Where(s => 25 - (s.UsedPositions ?? 0) >= movement.Value.Count()).FirstOrDefault();
									}

									if (container != null)
									{
										numberOfContainersFoundThatMatchTheCriteria++;
										finalMovementMap[container.Name] = movement.Value;
										containersOnResourceLoadPortReadyForProcessing.Remove(container);
									}

									if (numberOfContainersNeeded == numberOfContainersFoundThatMatchTheCriteria)
									{
										break;
									}
								}
							}

							if (numberOfContainersNeeded != numberOfContainersFoundThatMatchTheCriteria)
							{
								canStartProcess = false;
							}
							else
							{
								// We need to build the new updated JArray
								JArray jArray = new JArray();

								if (!isMerge)
								{
									currentContainer.LoadRelations("MaterialContainer");
								}

								foreach (var movement in finalMovementMap)
								{
									Container container = new Container();
									container.Load(movement.Key);
									container.LoadRelations("ContainerResource");

									if (isMerge)
									{
										container.LoadRelations("MaterialContainer");
									}

									foreach (Tuple<int, int> position in movement.Value)
									{
										JObject jObject = new JObject();

										if (isMerge)
										{
											jObject["SourceContainer"] = container.Name;
											jObject["MaterialName"] = container?.ContainerMaterials.Where(m => (m.Position ?? 0) == position.Item1).FirstOrDefault()?.SourceEntity.Name ?? "";
											jObject["DestinationContainer"] = currentContainer.Name;
										}
										else
										{
											jObject["SourceContainer"] = currentContainer.Name;
											jObject["MaterialName"] = currentContainer?.ContainerMaterials.Where(m => (m.Position ?? 0) == position.Item1).FirstOrDefault()?.SourceEntity.Name ?? "";
											jObject["DestinationContainer"] = container.Name;
										}

										jObject["SourcePosition"] = position.Item1;
										jObject["DestinationPosition"] = position.Item2;

										jArray.Add(jObject);
									}
								}

								// Set context
								customSorterJobDefinition.MovementList = jArray.ToString();
							}
						}
					}
				}
				else if (customSorterJobDefinition.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessCompose)
				{
					if (bom == null)
					{
						return false;
					}

					bom.LoadRelations("BOMProduct");

					Dictionary<string, List<string>> subs = new Dictionary<string, List<string>>();
					Dictionary<string, int> parentBomProd = new Dictionary<string, int>();

					foreach (BOMProduct prod in bom.BomProducts)
					{
						prod.Load();

						if (prod.Parent == null)
						{
							parentBomProd.Add(prod.TargetEntity.Name, Convert.ToInt32(prod.Quantity ?? 0));
						}
						else
						{
							if (subs.ContainsKey(prod.Parent.TargetEntity.Name))
							{
								subs[prod.Parent.TargetEntity.Name].Add(prod.TargetEntity.Name);
							}
							else
							{
								subs[prod.Parent.TargetEntity.Name] = new List<string>
								{
									prod.TargetEntity.Name
								};
							}
						}
					}

					currentContainer.LoadRelations("MaterialContainer");

					// Updated needed count from parentBomProd
					Dictionary<string, int> currentBomProdNeeds = new Dictionary<string, int>();
					if (currentContainer.ContainerMaterials != null && currentContainer.ContainerMaterials.Count > 0)
					{
						foreach (var parent in parentBomProd)
						{
							string parentBomProductName = parent.Key;
							int numberOfWafersNeeded = parent.Value;
							MaterialContainerCollection materialsInContainer = new MaterialContainerCollection();

							materialsInContainer.AddRange(currentContainer.ContainerMaterials.Where(c => c.SourceEntity.Product.Name == parentBomProductName));

							if (subs.ContainsKey(parent.Key))
							{
								foreach (string childrenBomProductName in subs[parent.Key])
								{
									materialsInContainer.AddRange(currentContainer.ContainerMaterials.Where(c => c.SourceEntity.Product.Name == childrenBomProductName));
								}
							}

							currentBomProdNeeds.Add(parentBomProductName, numberOfWafersNeeded - materialsInContainer.Count);
						}
					}
					else
					{
						currentBomProdNeeds = parentBomProd;
					}

					// fill queue free positions
					Queue<int> freePositions = new Queue<int>();
					for (int i = 1; i <= 25; i++)
					{
						if (currentContainer.ContainerMaterials == null || !currentContainer.ContainerMaterials.Any(m => m.Position == i))
						{
							freePositions.Enqueue(i);
						}
					}

					List<Container> containers = GetContainersDockedOnResourceLoadPortsReadyForProcess(resource);
					JArray jArray = new JArray();
					MaterialContainerCollection materialsInContainers = new MaterialContainerCollection();

					foreach (Container container in containers.Where(c => c.Name != currentContainer.Name))
					{
						container.LoadAttributes(new Collection<string> { AMSOsramConstants.ContainerAttributeLot });
						container.LoadRelations("MaterialContainer");

						if (container.Attributes != null)
						{
							string currentLot = string.Empty;

							if (container.Attributes.ContainsKey(AMSOsramConstants.ContainerAttributeLot))
							{
								container.Attributes.TryGetValueAs(AMSOsramConstants.ContainerAttributeLot, out currentLot);
							}

							if (string.IsNullOrWhiteSpace(currentLot))
							{
								if (container.ContainerMaterials != null && container.ContainerMaterials.Count > 0)
								{
									materialsInContainers.AddRange(container.ContainerMaterials);
								}
							}
						}
					}

					Dictionary<string, int> finalBomProdNeeds = new Dictionary<string, int>();
					foreach (var parentBomProductNeeded in currentBomProdNeeds)
					{
						string parentBomProductNeededName = parentBomProductNeeded.Key;
						int numberOfNeededWafers = parentBomProductNeeded.Value;
						finalBomProdNeeds.Add(parentBomProductNeededName, numberOfNeededWafers);

						List<MaterialContainer> materialsWithNeededParent = materialsInContainers.Where(m => m.SourceEntity.Product.Name == parentBomProductNeededName).ToList();
						foreach (var wafer in materialsWithNeededParent)
						{
							JObject jObject = new JObject
							{
								["PRODUCT"] = wafer?.SourceEntity?.Product?.Name ?? "",
								["SourceContainer"] = wafer.TargetEntity.Name,
								["MaterialName"] = wafer?.SourceEntity?.Name ?? "",
								["DestinationContainer"] = currentContainer.Name,
								["SourcePosition"] = wafer.Position,
								["DestinationPosition"] = freePositions.Dequeue()
							};

							jArray.Add(jObject);

							numberOfNeededWafers--;

							if (numberOfNeededWafers == 0)
							{
								break;
							}
						}

						if (numberOfNeededWafers != 0 && subs.ContainsKey(parentBomProductNeededName))
						{
							foreach (string substitute in subs[parentBomProductNeededName])
							{
								List<MaterialContainer> materialsWithNeededChild = materialsInContainers.Where(m => m.SourceEntity.Product.Name == substitute).ToList();

								foreach (var wafer in materialsWithNeededChild)
								{
									JObject jObject = new JObject
									{
										["PRODUCT_SUBSTITUTE"] = wafer?.SourceEntity?.Product?.Name ?? "",
										["SourceContainer"] = wafer.TargetEntity.Name,
										["MaterialName"] = wafer?.SourceEntity?.Name ?? "",
										["DestinationContainer"] = currentContainer.Name,
										["SourcePosition"] = wafer.Position,
										["DestinationPosition"] = freePositions.Dequeue()
									};

									jArray.Add(jObject);

									numberOfNeededWafers--;

									if (numberOfNeededWafers == 0)
									{
										break;
									}
								}

								if (numberOfNeededWafers == 0)
								{
									break;
								}
							}
						}

						finalBomProdNeeds[parentBomProductNeededName] = numberOfNeededWafers;
					}

					if (finalBomProdNeeds.Sum(c => c.Value) == 0)
					{
						// TODO Set load ports in use.
						//
						// Update current load port to be in use
						//currentLoadPort.SaveAttributes(new AttributeCollection() { { AMSOsramConstants.ResourceAttributeIsLoadPortInUse, true } });
						// Set context
						customSorterJobDefinition.MovementList = jArray.ToString();
					}
					else
					{
						canStartProcess = false;
					}
				}
			}
			else
			{
				canStartProcess = false;
			}

			return canStartProcess;
		}

		/// <summary>
		/// Retrieves the container and load port for the given material
		/// </summary>
		/// <param name="material">The material</param>
		/// <param name="container">The container</param>
		/// <param name="loadPort">The load port</param>
		/// <returns>True if container and loadport exists, false otherwise</returns>
		public static bool RetrieveContainerAndLoadPortFromMaterial(Material material, ref Container container, ref Resource loadPort)
		{
			if (material.MaterialContainer != null && material.MaterialContainer.Count > 0)
			{
				container = material.MaterialContainer.First().TargetEntity;
				container.Load();
				container.LoadRelations(Navigo.Common.Constants.ContainerResource);

				if (container.ContainerResourceRelations != null && container.ContainerResourceRelations.Count > 0)
				{
					loadPort = container.ContainerResourceRelations.First().TargetEntity;
					loadPort.Load();
				}
			}

			return container != null && loadPort != null;
		}

		/// <summary>
		/// Retrieves for a resource, load ports and container related data.
		/// Eg. Containers docked, Lots associated with those lots and some container attributes.
		/// </summary>
		/// <param name="resource">The parente resource</param>
		/// <param name="parameters">Parameters to be used in the query</param>
		/// <returns>A list of containers associated with load ports for a given resource</returns>
		public static List<ResourceLoadPortData> GetResourceLoadPortData(Resource resource, QueryParameterCollection parameters, FilterCollection filters = null)
		{
			List<ResourceLoadPortData> dockedContainers = new List<ResourceLoadPortData>();
			QueryObject query = new QueryObject() { Name = AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPorts };

			if (resource == null || !resource.ObjectExists())
			{
				throw new ArgumentNullCmfException("resource");
			}

			if (parameters == null)
			{
				throw new ArgumentNullCmfException("parameters");
			}

			if (query.ObjectExists())
			{
				query.Load();

				if (filters != null &&
					filters.Count > 0)
				{
					query.Query.Filters.AddRange(filters);
				}

				DataSet dataSet = query.Execute(false, parameters);

				if (dataSet.Tables != null && dataSet.Tables.Count > 0 &&
					dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						ResourceLoadPortData resourceLoadPortInformation = new ResourceLoadPortData
						{
							ParentResourceId = resource.Id,
							ParentResourceName = resource.Name
						};

						#region Handle Boolean values

						// Container 'TransportRequested' Attribute
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerTransportRequestedAttributeColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerTransportRequestedAttributeColumn] is string strContainerTransportRequested)
						{
							resourceLoadPortInformation.ContainerTransportRequestedAttribute = bool.Parse(strContainerTransportRequested);
						}

						// Container 'MapContainerNeeded' Attribute
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerMapContainerNeededAttributeColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerMapContainerNeededAttributeColumn] is string strContainerMapContainerNeeded)
						{
							resourceLoadPortInformation.ContainerMapContainerNeededAttribute = bool.Parse(strContainerMapContainerNeeded);
						}

						// Resource (LoadPort) 'IsLoadPortInUse' Attribute
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsIsLoadPortInUseColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsIsLoadPortInUseColumn] is string strIsLoadPortInUse)
						{
							resourceLoadPortInformation.LoadPortInUse = bool.Parse(strIsLoadPortInUse);
						}

						#endregion Handle Boolean values

						#region Handle Integer Cast

						// Container Used Positions
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerUsedPositionsColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerUsedPositionsColumn] is int usedPositions)
						{
							resourceLoadPortInformation.ContainerUsedPositions = usedPositions;
						}

						// Container Total Position
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerTotalPositionsColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerTotalPositionsColumn] is int totalPositions)
						{
							resourceLoadPortInformation.ContainerTotalPositions = totalPositions;
						}

						// Load Port Load Port Type
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortTypeColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortTypeColumn] is int intLoadPortLoadPortType)
						{
							resourceLoadPortInformation.LoadPortLoadPortType = (LoadPortType)intLoadPortLoadPortType;
						}

						// Container Resource Association Type
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerResourceAssociationTypeColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerResourceAssociationTypeColumn] is int intContainerResourceAssociationType)
						{
							resourceLoadPortInformation.ContainerResourceAssociationType = (ContainerResourceAssociationType)intContainerResourceAssociationType;
						}

						#endregion Handle Integer Cast

						#region Handle Long Cast

						// Parent Resource Id
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsParentResourceIdColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsParentResourceIdColumn] is long parentResourceId)
						{
							resourceLoadPortInformation.ParentResourceId = parentResourceId;
						}

						// Load Port Id
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortIdColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortIdColumn] is long loadPortId)
						{
							resourceLoadPortInformation.LoadPortId = loadPortId;
						}

						// Container Id
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerIdColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerIdColumn] is long containerId)
						{
							resourceLoadPortInformation.ContainerId = containerId;
						}

						// Parente Material Id
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerParentMaterialIdColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerParentMaterialIdColumn] is long parentMaterialId)
						{
							resourceLoadPortInformation.ParentMaterialId = parentMaterialId;
						}

						#endregion Handle Long Cast

						#region Handle String Properties

						// Container Lot Attribute
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerLotAttributeColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerLotAttributeColumn] is string strContainerLot)
						{
							resourceLoadPortInformation.ContainerLotAttribute = strContainerLot;
						}

						// Container Name
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerNameColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerNameColumn] is string strContainerName)
						{
							resourceLoadPortInformation.ContainerName = strContainerName;
						}

						// Load Port Name
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortNameColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortNameColumn] is string strLoadPortName)
						{
							resourceLoadPortInformation.LoadPortName = strLoadPortName;
						}

						// Parent Material Name
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerParentMaterialNameColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerParentMaterialNameColumn] is string strParentMaterialName)
						{
							resourceLoadPortInformation.ParentMaterialName = strParentMaterialName;
						}

						// Container Type
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerTypeColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerTypeColumn] is string strContainerType)
						{
							resourceLoadPortInformation.ContainerType = strContainerType;
						}

						// Container Product Attribute
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerProductAttributeColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerProductAttributeColumn] is string strContainerProduct)
						{
							resourceLoadPortInformation.ContainerProductAttribute = strContainerProduct;
						}

						// Load Port State Model State Id (name)
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortStateModelStateIdColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortStateModelStateIdColumn] is string strLoadPortStateModelStateId)
						{
							resourceLoadPortInformation.LoadPortStateModelStateId = strLoadPortStateModelStateId;
						}

						#endregion Handle String Properties

						#region Handle DateTime property

						// Resource (LoadPort) Modified On Property
						if (row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortModifiedOnColumn] != null &&
							row[AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortModifiedOnColumn] is DateTime strLoadPortModifiedOn)
						{
							resourceLoadPortInformation.LoadPortModifiedOn = strLoadPortModifiedOn;
						}

						#endregion Handle DateTime property

						dockedContainers.Add(resourceLoadPortInformation);
					}
				}
			}
			else
			{
				throw new ObjectNotFoundCmfException(query.GetType().Name, query.Name);
			}

			return dockedContainers;
		}

		/// <summary>
		/// Query to retrieve docked containers on resource load ports.
		/// </summary>
		/// <param name="resource">The parent resource</param>
		/// <returns>A list of docked containers for the given resource load ports</returns>
		public static List<ResourceLoadPortData> DockedContainersOnLoadPortsByParentResource(Resource resource)
		{
			QueryParameter parameterResource = new QueryParameter
			{
				FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
				Name = AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsParentResourceParameter,
				Value = resource.Id
			};

			QueryParameter parameterContainerResourceAssociationType = new QueryParameter
			{
				FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
				Name = AMSOsramConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerResourceAssociationTypeParameter,
				Value = ContainerResourceAssociationType.DockedContainer
			};

			return GetResourceLoadPortData(resource, new QueryParameterCollection() { parameterResource, parameterContainerResourceAssociationType });
		}

		/// <summary>
		/// Retrives docked containers that are ready to process.
		/// </summary>
		/// <param name="resource"></param>
		/// <returns>Containers ready to start process</returns>
		public static List<Container> GetContainersDockedOnResourceLoadPortsReadyForProcess(Resource resource)
		{
			ContainerCollection containers = new ContainerCollection();
			List<ResourceLoadPortData> dockedContainers = DockedContainersOnLoadPortsByParentResource(resource);

			foreach (ResourceLoadPortData dockedContainer in dockedContainers)
			{
				if (!dockedContainer.LoadPortInUse)
				{
					Container containerOnLoadPort = new Container() { Name = dockedContainer.ContainerName };
					containers.Add(containerOnLoadPort);
				}
			}

			containers.Load();
			return containers.ToList();
		}

		/// <summary>
		/// Retrives a BOM from BOM context for a specific material.
		/// </summary>
		/// <param name="material"></param>
		/// <returns>The resolved BOM</returns>
		public static BOM ResolveBOMContext(Material material)
		{
			string stepName = material.Step.Name;
			string materialName = material.Name;
			string productName = material.Product.Name;
			string logicalFlowPath = material.LogicalFlowPath;
			// Product group isn't mandatory, we have to null check it
			string productGroupName = material.Product.ProductGroup?.Name ?? null;

			//Load SmartTable
			SmartTable productTypeRoleSmartTable = new SmartTable() { Name = "BOMContext" };

			productTypeRoleSmartTable.Load();

			NgpDataRow resolveKeys = new NgpDataRow
			{
				{ "Step", stepName },
				{ "LogicalFlowPath",  logicalFlowPath },
				{ "Product", productName },
				{ "ProductGroup", productGroupName },
				{ "Material", materialName },
			};

			NgpDataSet resolvedData = productTypeRoleSmartTable.Resolve(resolveKeys, true);

			DataSet dataset = NgpDataSet.ToDataSet(resolvedData);

			if (dataset != null && dataset.Tables != null && dataset.Tables.Count != 0 && dataset.Tables[0].Rows.Count > 0)
			{
				BOM bom = new BOM();
				bom.Load(dataset.Tables[0].Rows[0]["BOM"].ToString());
				return bom;
			}
			else
			{
				return null;
			}
		}

        #endregion Sorter

        #region DEEActionUtilities
        /// <summary>
        /// Checks if current action group (present in Input dicionary) is valid based on list of given action groups
        /// </summary>
        /// <param name="Input">Dictionary where in theory action group is defined</param>
        /// <param name="ValidActionGroups">List of valid action groups</param>
        /// <param name="defaultBehavior">default behavior of method. Defaults to false but can be changed by invoker</param>
        /// <returns></returns>
        public static bool IsActionGroupValid(Dictionary<string, object> Input, Collection<string> ValidActionGroups, bool defaultBehavior = false)
		{
			// by default action group is not valid unless explicitly defined otherwise
			bool isValid = defaultBehavior;

			// proceed if Action group name can be extracted from Input
			string actionGroupName = GetActionGroup(Input);
			if (!String.IsNullOrWhiteSpace(actionGroupName) && ValidActionGroups != null && ValidActionGroups.Any())
			{
				isValid = ValidActionGroups.Any(E => String.Equals(E, actionGroupName, StringComparison.InvariantCultureIgnoreCase));
			}

			return isValid;
		}

		/// <summary>
		/// Retrieves the action group
		/// </summary>
		/// <param name="Input"></param>
		/// <returns></returns>
		public static string GetActionGroup(Dictionary<string, object> Input)
		{
			string returnValue = GetInputItem<string>(Input, "ActionGroupName", String.Empty);

			return returnValue;
		}

		/// <summary>
		/// Tries to retrieve a given item from the Input dictionary, based on the item name and passed type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Input"></param>
		/// <param name="inputEntryName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static T GetInputItem<T>(Dictionary<string, object> Input, string inputEntryName, T defaultValue = default(T))
		{
			// define return value
			T returnObject = defaultValue;

			// try to extract the element
			object oExtractedItem = null;
			if (Input.TryGetValue(inputEntryName, out oExtractedItem) && oExtractedItem is T)
			{
				returnObject = (T)oExtractedItem;
			}

			return returnObject;
		}

        #endregion
    }
}
