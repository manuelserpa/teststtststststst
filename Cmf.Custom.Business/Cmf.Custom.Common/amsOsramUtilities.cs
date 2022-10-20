using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.amsOSRAM.Common.ERP;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.OutputObjects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions;
using Cmf.Foundation.Configuration.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.LocalizationService;
using Cmf.Navigo.BusinessOrchestration.Abstractions;

namespace Cmf.Custom.amsOSRAM.Common
{
    /// <summary>
    /// Support class to encapsulate methods to support the development for the business layer
    /// </summary>
    public static class amsOSRAMUtilities
    {
        #region Generic

        /// <summary>
        /// Get Value as nullable decimal.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns></returns>
        public static decimal? GetValueAsNullableDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return decimal.Parse(value.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), NumberStyles.Number | NumberStyles.AllowExponent);
        }

        /// <summary>
        /// Get Value as nullable boolean
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns></returns>
        public static bool? GetValueAsNullableBoolean(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            // True: Possible values 
            string[] positiveValues = { "y", "true", "yes", "1" };

            if (positiveValues.Contains(value.Trim(), StringComparer.InvariantCultureIgnoreCase))
            {
                return true;
            }

            // False: Possible values
            string[] negativeValues = { "n", "false", "no", "0" };

            if (negativeValues.Contains(value.Trim(), StringComparer.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return default(bool?);
        }

        /// <summary>
        /// Get Value as decimal.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns></returns>
        public static decimal GetValueAsDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(decimal);
            }

            return decimal.Parse(value.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), NumberStyles.Number | NumberStyles.AllowExponent);
        }

        /// <summary>
        /// Get Value as boolean.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns></returns>
        public static bool GetValueAsBoolean(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string[] booleanValues = { "y", "true", "yes", "1" };

            if (booleanValues.Contains(value.Trim(), StringComparer.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return default(bool);
        }

        /// <summary>
        /// Get the Value as DateTime.
        /// </summary>
        /// <param name="val">Value to be converted.</param>
        /// <returns>Return the value as datetime.</returns>
        public static DateTime? GetValueAsDateTime(string val)
        {
            DateTime? result = null;
            DateTime exactDateTime;
            if (DateTime.TryParse(val, out exactDateTime))
            {
                result = DateTime.SpecifyKind(exactDateTime, DateTimeKind.Local);
            }

            return result;
        }


        /// <summary>
        /// Gets the value as enum.
        /// </summary>
        /// <typeparam name="T">Type of the enum.</typeparam>
        /// <param name="value">Value to be converted.</param>
        /// <returns>Return the value as enum value.</returns>
        public static T GetValueAsEnum<T>(string value) where T : struct
        {
            T result;

            if (Enum.TryParse<T>(value, out result))
            {
                return result;
            }

            return default(T);
        }

        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            /* If this is a list, use the Count property for efficiency.
			 * The Count property is O(1) while IEnumerable.Count() is O(N). */
            ICollection<T> collection = enumerable as ICollection<T>;

            if (collection != null)
            {
                return collection.Count < 1;
            }

            return !enumerable.Any();
        }

        /// <summary>
        /// Get Value as dynamic DataType.
        /// </summary>
        /// <param name="parameterDataType">Parameter Data Type.</param>
        /// <param name="value">Value to be converted.</param>
        /// <returns></returns>
        public static dynamic GetParameterValueAsDataType(ParameterDataType parameterDataType, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(dynamic);
            }

            switch (parameterDataType)
            {
                case ParameterDataType.Decimal:
                    return GetValueAsNullableDecimal(value);

                case ParameterDataType.Boolean:
                    return GetValueAsNullableBoolean(value);

                default:
                    return value;
            }
        }

        /// <summary>
        /// Get Value as dynamic DataType.
        /// </summary>
        /// <param name="scalarType">Scalar Type.</param>
        /// <param name="value">Value to be converted.</param>
        /// <returns></returns>
        public static dynamic GetAttributeValueAsDataType(IScalarType scalarType, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(dynamic);
            }

            switch (scalarType.NativeType)
            {
                case "System.Decimal":
                    return GetValueAsNullableDecimal(value);

                case "System.Boolean":
                    return GetValueAsNullableBoolean(value);

                default:
                    return value;
            }
        }

        #endregion

        #region Configs

        /// <summary>
        /// Retrieves the config value for the input key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static T GetConfig<T>(string configName)
        {
            IConfig config = Config.GetConfig(configName);
            if (config != null && config.Value != null)
            {
                return (T)config.Value;
            }
            else
            {
                IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
                ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();
                throw new Exception(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageConfigNotFound), configName));
            }
        }

        #endregion Configs

        #region SmartTables

        public static DataRow CustomResolveSTCustomMaterialNiceLabelPrintContext(string step = null,
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
            ISmartTable customMaterialNiceLabelPrintContext = new SmartTable();
            customMaterialNiceLabelPrintContext.Load(amsOSRAMConstants.CustomMaterialNiceLabelPrintContextSmartTable);

            NgpDataRow values = new NgpDataRow();

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

            // If lot name is filled apply it as a filter
            if (!string.IsNullOrWhiteSpace(material))
            {
                values.Add(Cmf.Navigo.Common.Constants.Material, material);
            }

            // If lot type is filled apply it as a filter
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

            INgpDataSet niceLabelPrintContextNgpDataSet = customMaterialNiceLabelPrintContext.Resolve(values, true);

            DataRow row = null;

            if (niceLabelPrintContextNgpDataSet != null)
            {
                DataSet niceLabelPrintContextDataSet = NgpDataSet.ToDataSet(niceLabelPrintContextNgpDataSet);
                if (niceLabelPrintContextDataSet.HasData())
                {
                    row = niceLabelPrintContextDataSet.Tables[0].Rows[0];
                }
            }

            return row;
        }

        /// <summary>
        /// Method to resolve Material Data Collection Context 
        /// </summary>
        /// <param name="step"></param>
        /// <param name="logicalFlowPath"></param>
        /// <param name="product"></param>
        /// <param name="productGroup"></param>
        /// <param name="flow"></param>
        /// <param name="material"></param>
        /// <param name="materialType"></param>
        /// <param name="resource"></param>
        /// <param name="resourceType"></param>
        /// <param name="model"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static INgpDataSet CustomResolveMaterialDataCollectionContext(string step = null,
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
            ISmartTable materialDatacollectionContext = new SmartTable();
            materialDatacollectionContext.Load(Cmf.Navigo.Common.Constants.MaterialDataCollectionContext);

            NgpDataRow values = new NgpDataRow();

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

            // If lot name is filled apply it as a filter
            if (!string.IsNullOrWhiteSpace(material))
            {
                values.Add(Cmf.Navigo.Common.Constants.Material, material);
            }

            // If lot type is filled apply it as a filter
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

            INgpDataSet materialDCContextNgpDataSet = materialDatacollectionContext.Resolve(values, true);

            return materialDCContextNgpDataSet;
        }

        /// <summary>
        /// Result custom ST CustomReportConsumptionToSAP
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public static string CustomResolveSTCustomReportConsumptionToSAP(IMaterial material)
        {
            string storageLocation = string.Empty;
            ISmartTable smartTable = new SmartTable();
            smartTable.Load(amsOSRAMConstants.CustomReportConsumptionToSAPSmartTable);

            INgpDataRow values = new NgpDataRow();
            if (material.Product != null)
            {
                values.Add(Navigo.Common.Constants.Product, material.Product.Name);
            }

            if (material.Product.ProductGroup?.Name != null)
            {
                values.Add(Navigo.Common.Constants.ProductGroup, material.Product.ProductGroup?.Name);
            }

            if (material.Flow.Name != null)
            {
                values.Add(Navigo.Common.Constants.Flow, material.Flow.Name);
            }

            if (material.Step.Name != null)
            {
                values.Add(Navigo.Common.Constants.Step, material.Step.Name);
            }

            if (material.Type != null)
            {
                values.Add(Navigo.Common.Constants.MaterialType, material.Type);
            }

            INgpDataSet ngpDataSet = smartTable.Resolve(values, true);
            if (ngpDataSet != null && ngpDataSet.Tables != null && ngpDataSet.Tables.Count > 0)
            {
                DataSet dataSet = NgpDataSet.ToDataSet(ngpDataSet);
                if (dataSet.HasData())
                {
                    storageLocation = dataSet.Tables[0].Rows[0][amsOSRAMConstants.CustomStorageLocation].ToString();
                }
            }
            return storageLocation;
        }

        #endregion

        #region Sorter

        /// <summary>
        /// Resolve CustomSorterJobDefinitionContext
        /// </summary>
        /// <param name="material">The lot to trackin</param>
        /// <returns>The custom sorter job definition</returns>
        public static ICustomSorterJobDefinition GetSorterJobDefinition(IMaterial material)
        {
            // Get services provider information
            IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            ICustomSorterJobDefinition customSorterJobDefinition = null;

            string stepName = material.Step.Name;
            string materialName = material.Name;
            string productName = material.Product.Name;
            string flowName = material.GetStepParentFlow().Name;
            string materialType = material.Type;
            // Product group isn't mandatory, we have to null check it
            string productGroup = material.Product.ProductGroup?.Name ?? null;

            //Load SmartTable
            ISmartTable productTypeRoleSmartTable = new SmartTable() { Name = amsOSRAMConstants.CustomSorterJobDefinitionContextName };

            productTypeRoleSmartTable.Load();

            INgpDataRow resolveKeys = new NgpDataRow
            {
                { amsOSRAMConstants.CustomSorterJobDefinitionContextColumnStep, stepName },
                { amsOSRAMConstants.CustomSorterJobDefinitionContextColumnMaterial, materialName },
                { amsOSRAMConstants.CustomSorterJobDefinitionContextColumnProduct, productName },
                { amsOSRAMConstants.CustomSorterJobDefinitionContextColumnFlow, flowName },
                { amsOSRAMConstants.CustomSorterJobDefinitionContextColumnMaterialType, materialType },
                { amsOSRAMConstants.CustomSorterJobDefinitionContextColumnProductGroup, productGroup }
            };

            INgpDataSet resolvedData = productTypeRoleSmartTable.Resolve(resolveKeys, true);

            DataSet dataset = NgpDataSet.ToDataSet(resolvedData);

            if (dataset != null && dataset.Tables != null && dataset.Tables.Count != 0 && dataset.Tables[0].Rows.Count > 0)
            {
                string customSorterJobDefinitionName = dataset.Tables[0].Rows[0][amsOSRAMConstants.CustomSorterJobDefinitionContextColumnCustomSorterJobDefinition].ToString();
                customSorterJobDefinition = entityFactory.Create<ICustomSorterJobDefinition>();
                customSorterJobDefinition.Name = customSorterJobDefinitionName;
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
        public static bool CanProcessSorterJobDefinition(ref ICustomSorterJobDefinition customSorterJobDefinition, IContainer currentContainer, IResource resource, IResource currentLoadPort, IBOM bom = null)
        {
            bool canStartProcess = true;

            IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            // Current Container must match with Source Carrier type defined on custom Sorter Job Definition
            // Otherwise we cannot do the track in
            if (customSorterJobDefinition.SourceCarrierType == currentContainer.Type)
            {
                // Only need to take into account scenarios other than 'Map Carrier'
                if (customSorterJobDefinition.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessTransferWafers)
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
                            Dictionary<IResource, IContainer> loadPortsThatMatchTheCriteria = new Dictionary<IResource, IContainer>();
                            // get resource descendents
                            IResourceHierarchy descendents = resource.GetDescendentResources(1);
                            List<IResource> loadPorts = descendents.Where(s => s.ChildResource.ProcessingType == ProcessingType.LoadPort)
                                .Select(s => s.ChildResource)
                                .ToList();

                            List<IContainer> containersOnResourceLoadPortReadyForProcessing = new List<IContainer>();
                            foreach (IResource loadPort in loadPorts.Where(l => l.Id != currentLoadPort.Id))
                            {
                                loadPort.Load();
                                loadPort.LoadAttributes(new Collection<string> { amsOSRAMConstants.ResourceAttributeIsLoadPortInUse });
                                bool isLoadPortInUse = false;

                                if (loadPort.Attributes != null)
                                {
                                    loadPort.Attributes.TryGetValueAs(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, out isLoadPortInUse);
                                }

                                // If load port is available we'll check if exists a valid lot for processing
                                if (!isLoadPortInUse)
                                {
                                    loadPort.LoadRelations("ContainerResource");

                                    // If exists any resource container collection
                                    if (loadPort.RelationCollection.ContainsKey("ContainerResource"))
                                    {
                                        IContainerResource containerResourceOnOtherLoadPort = loadPort.ResourceContainers.FirstOrDefault();
                                        IContainer containerOnOtherLoadPort = containerResourceOnOtherLoadPort.SourceEntity;
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
                                foreach (KeyValuePair<string, List<Tuple<int, int>>> movement in movementListMap)
                                {
                                    IContainer container = null;

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

                                foreach (KeyValuePair<string, List<Tuple<int, int>>> movement in finalMovementMap)
                                {
                                    IContainer container = entityFactory.Create<IContainer>();
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
                else if (customSorterJobDefinition.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessCompose)
                {
                    if (bom == null)
                    {
                        return false;
                    }

                    bom.LoadRelations("BOMProduct");

                    Dictionary<string, List<string>> subs = new Dictionary<string, List<string>>();
                    Dictionary<string, int> parentBomProd = new Dictionary<string, int>();

                    foreach (IBOMProduct prod in bom.BomProducts)
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
                        foreach (KeyValuePair<string, int> parent in parentBomProd)
                        {
                            string parentBomProductName = parent.Key;
                            int numberOfWafersNeeded = parent.Value;
                            IMaterialContainerCollection materialsInContainer = entityFactory.CreateCollection<IMaterialContainerCollection>();

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

                    List<IContainer> containers = GetContainersDockedOnResourceLoadPortsReadyForProcess(resource);
                    JArray jArray = new JArray();
                    IMaterialContainerCollection materialsInContainers = entityFactory.CreateCollection<IMaterialContainerCollection>();

                    foreach (IContainer container in containers.Where(c => c.Name != currentContainer.Name))
                    {
                        container.LoadAttributes(new Collection<string> { amsOSRAMConstants.ContainerAttributeLot });
                        container.LoadRelations("MaterialContainer");

                        if (container.Attributes != null)
                        {
                            string currentLot = string.Empty;

                            if (container.Attributes.ContainsKey(amsOSRAMConstants.ContainerAttributeLot))
                            {
                                container.Attributes.TryGetValueAs(amsOSRAMConstants.ContainerAttributeLot, out currentLot);
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
                    foreach (KeyValuePair<string, int> parentBomProductNeeded in currentBomProdNeeds)
                    {
                        string parentBomProductNeededName = parentBomProductNeeded.Key;
                        int numberOfNeededWafers = parentBomProductNeeded.Value;
                        finalBomProdNeeds.Add(parentBomProductNeededName, numberOfNeededWafers);

                        List<IMaterialContainer> materialsWithNeededParent = materialsInContainers.Where(m => m.SourceEntity.Product.Name == parentBomProductNeededName).ToList();
                        foreach (IMaterialContainer wafer in materialsWithNeededParent)
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
                                List<IMaterialContainer> materialsWithNeededChild = materialsInContainers.Where(m => m.SourceEntity.Product.Name == substitute).ToList();

                                foreach (IMaterialContainer wafer in materialsWithNeededChild)
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
                        //currentLoadPort.SaveAttributes(new AttributeCollection() { { amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, true } });
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
        /// Retrieves the container and load port for the given lot
        /// </summary>
        /// <param name="material">The lot</param>
        /// <param name="container">The container</param>
        /// <param name="loadPort">The load port</param>
        /// <returns>True if container and loadport exists, false otherwise</returns>
        public static bool RetrieveContainerAndLoadPortFromMaterial(IMaterial material, ref IContainer container, ref IResource loadPort)
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
        public static List<ResourceLoadPortData> GetResourceLoadPortData(IResource resource, IQueryParameterCollection parameters, IFilterCollection filters = null)
        {
            List<ResourceLoadPortData> dockedContainers = new List<ResourceLoadPortData>();
            IQueryObject query = new QueryObject() { Name = amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPorts };

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
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerTransportRequestedAttributeColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerTransportRequestedAttributeColumn] is string strContainerTransportRequested)
                        {
                            resourceLoadPortInformation.ContainerTransportRequestedAttribute = bool.Parse(strContainerTransportRequested);
                        }

                        // Container 'MapContainerNeeded' Attribute
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerMapContainerNeededAttributeColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerMapContainerNeededAttributeColumn] is string strContainerMapContainerNeeded)
                        {
                            resourceLoadPortInformation.ContainerMapContainerNeededAttribute = bool.Parse(strContainerMapContainerNeeded);
                        }

                        // Resource (LoadPort) 'IsLoadPortInUse' Attribute
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsIsLoadPortInUseColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsIsLoadPortInUseColumn] is string strIsLoadPortInUse)
                        {
                            resourceLoadPortInformation.LoadPortInUse = bool.Parse(strIsLoadPortInUse);
                        }

                        #endregion Handle Boolean values

                        #region Handle Integer Cast

                        // Container Used Positions
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerUsedPositionsColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerUsedPositionsColumn] is int usedPositions)
                        {
                            resourceLoadPortInformation.ContainerUsedPositions = usedPositions;
                        }

                        // Container Total Position
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerTotalPositionsColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerTotalPositionsColumn] is int totalPositions)
                        {
                            resourceLoadPortInformation.ContainerTotalPositions = totalPositions;
                        }

                        // Load Port Load Port Type
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortTypeColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortTypeColumn] is int intLoadPortLoadPortType)
                        {
                            resourceLoadPortInformation.LoadPortLoadPortType = (LoadPortType)intLoadPortLoadPortType;
                        }

                        // Container Resource Association Type
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerResourceAssociationTypeColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerResourceAssociationTypeColumn] is int intContainerResourceAssociationType)
                        {
                            resourceLoadPortInformation.ContainerResourceAssociationType = (ContainerResourceAssociationType)intContainerResourceAssociationType;
                        }

                        #endregion Handle Integer Cast

                        #region Handle Long Cast

                        // Parent Resource Id
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsParentResourceIdColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsParentResourceIdColumn] is long parentResourceId)
                        {
                            resourceLoadPortInformation.ParentResourceId = parentResourceId;
                        }

                        // Load Port Id
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortIdColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortIdColumn] is long loadPortId)
                        {
                            resourceLoadPortInformation.LoadPortId = loadPortId;
                        }

                        // Container Id
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerIdColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerIdColumn] is long containerId)
                        {
                            resourceLoadPortInformation.ContainerId = containerId;
                        }

                        // Parente Material Id
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerParentMaterialIdColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerParentMaterialIdColumn] is long parentMaterialId)
                        {
                            resourceLoadPortInformation.ParentMaterialId = parentMaterialId;
                        }

                        #endregion Handle Long Cast

                        #region Handle String Properties

                        // Container Lot Attribute
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerLotAttributeColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerLotAttributeColumn] is string strContainerLot)
                        {
                            resourceLoadPortInformation.ContainerLotAttribute = strContainerLot;
                        }

                        // Container Name
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerNameColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerNameColumn] is string strContainerName)
                        {
                            resourceLoadPortInformation.ContainerName = strContainerName;
                        }

                        // Load Port Name
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortNameColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortNameColumn] is string strLoadPortName)
                        {
                            resourceLoadPortInformation.LoadPortName = strLoadPortName;
                        }

                        // Parent Material Name
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerParentMaterialNameColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerParentMaterialNameColumn] is string strParentMaterialName)
                        {
                            resourceLoadPortInformation.ParentMaterialName = strParentMaterialName;
                        }

                        // Container Type
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerTypeColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerTypeColumn] is string strContainerType)
                        {
                            resourceLoadPortInformation.ContainerType = strContainerType;
                        }

                        // Container Product Attribute
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerProductAttributeColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerProductAttributeColumn] is string strContainerProduct)
                        {
                            resourceLoadPortInformation.ContainerProductAttribute = strContainerProduct;
                        }

                        // Load Port State Model State Id (name)
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortStateModelStateIdColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortStateModelStateIdColumn] is string strLoadPortStateModelStateId)
                        {
                            resourceLoadPortInformation.LoadPortStateModelStateId = strLoadPortStateModelStateId;
                        }

                        #endregion Handle String Properties

                        #region Handle DateTime property

                        // Resource (LoadPort) Modified On Property
                        if (row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortModifiedOnColumn] != null &&
                            row[amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortModifiedOnColumn] is DateTime strLoadPortModifiedOn)
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
        public static List<ResourceLoadPortData> DockedContainersOnLoadPortsByParentResource(IResource resource)
        {
            IQueryParameter parameterResource = new QueryParameter
            {
                FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
                Name = amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsParentResourceParameter,
                Value = resource.Id
            };

            IQueryParameter parameterContainerResourceAssociationType = new QueryParameter
            {
                FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
                Name = amsOSRAMConstants.QueryCustomGetContainersDockedOnResourceLoadPortsContainerResourceAssociationTypeParameter,
                Value = ContainerResourceAssociationType.DockedContainer
            };

            return GetResourceLoadPortData(resource, new QueryParameterCollection() { parameterResource, parameterContainerResourceAssociationType });
        }

        /// <summary>
        /// Retrives docked containers that are ready to process.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>Containers ready to start process</returns>
        public static List<IContainer> GetContainersDockedOnResourceLoadPortsReadyForProcess(IResource resource)
        {
            // Get services provider information
            IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IContainerCollection containers = entityFactory.CreateCollection<IContainerCollection>();
            List<ResourceLoadPortData> dockedContainers = DockedContainersOnLoadPortsByParentResource(resource);

            foreach (ResourceLoadPortData dockedContainer in dockedContainers)
            {
                if (!dockedContainer.LoadPortInUse)
                {
                    IContainer containerOnLoadPort = entityFactory.Create<IContainer>();
                    containerOnLoadPort.Name = dockedContainer.ContainerName;

                    containers.Add(containerOnLoadPort);
                }
            }

            containers.Load();
            return containers.ToList();
        }

        /// <summary>
        /// Retrives a BOM from BOM context for a specific lot.
        /// </summary>
        /// <param name="material"></param>
        /// <returns>The resolved BOM</returns>
        public static IBOM ResolveBOMContext(IMaterial material)
        {
            string stepName = material.Step.Name;
            string materialName = material.Name;
            string productName = material.Product.Name;
            string logicalFlowPath = material.LogicalFlowPath;
            // Product group isn't mandatory, we have to null check it
            string productGroupName = material.Product.ProductGroup?.Name ?? null;

            //Load SmartTable
            ISmartTable productTypeRoleSmartTable = new SmartTable() { Name = "BOMContext" };

            productTypeRoleSmartTable.Load();

            INgpDataRow resolveKeys = new NgpDataRow
            {
                { "Step", stepName },
                { "LogicalFlowPath",  logicalFlowPath },
                { "Product", productName },
                { "ProductGroup", productGroupName },
                { "Material", materialName },
            };

            INgpDataSet resolvedData = productTypeRoleSmartTable.Resolve(resolveKeys, true);

            DataSet dataset = NgpDataSet.ToDataSet(resolvedData);

            if (dataset != null && dataset.Tables != null && dataset.Tables.Count != 0 && dataset.Tables[0].Rows.Count > 0)
            {
                // Get services provider information
                IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
                IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

                IBOM bom = entityFactory.Create<IBOM>();
                bom.Load(dataset.Tables[0].Rows[0]["BOM"].ToString());
                return bom;
            }
            else
            {
                return null;
            }
        }

        #endregion Sorter

        #region Space

        /// <summary>
        /// Method to create XML message with Lot and Data Collection Info to be sent to Space system
        /// </summary>
        public static CustomReportEDCToSpace CreateSpaceInfoDefaultValues(IMaterial material)
        {
            material.Load(1);

            IProduct product = material.Product;
            product.LoadAttributes(new Collection<string>
            {
                amsOSRAMConstants.ProductAttributeBasicType
            });

            string productBasicType = string.Empty;

            if (product.Attributes.ContainsKey(amsOSRAMConstants.ProductAttributeBasicType))
            {
                product.Attributes.TryGetValueAs(amsOSRAMConstants.ProductAttributeBasicType, out productBasicType);
            }

            IArea area = null;

            IResource subResource = null;

            IResource resource = material.LastProcessedResource;

            if (resource != null)
            {
                resource.LoadRelations(Cmf.Navigo.Common.Constants.SubResource);

                if (resource.RelationCollection.ContainsKey(Cmf.Navigo.Common.Constants.SubResource))
                {
                    subResource = resource.RelationCollection[Navigo.Common.Constants.SubResource].FirstOrDefault().TargetEntity as IResource;
                }

                area = resource.Area;
            }

            // Load Site
            ISite site = material.Facility?.Site;
            site.Load();

            // Get SiteCode attribute value
            string siteCode = site.GetAttributeValue(amsOSRAMConstants.CustomSiteCodeAttribute, true).ToString();

            CustomReportEDCToSpace customReportEDCToSpace = new CustomReportEDCToSpace()
            {
                SampleDate = DateTime.Now.ToString(),
                Sender = new Sender()
                {
                    Value = Environment.MachineName
                },
                Ids = new List<SiteCode>()
                {
                    new SiteCode()
                    {
                        Value = !string.IsNullOrWhiteSpace(siteCode) ? siteCode : string.Empty
                    }
                },
                Keys = new List<Key>()
                {
                    new Key()
                    {
                        Name = "PROCESS",
                        Value = product != null ? product.Name : string.Empty
                    },
                    new Key()
                    {
                        Name = "BASIC_TYPE",
                        Value = productBasicType
                    },
                    new Key()
                    {
                        Name = "AREA",
                        Value = area != null ? area.Name : string.Empty
                    },
                    new Key()
                    {
                        Name = "OWNER",
                        Value = material.ProductionOrder != null ? material.ProductionOrder.Type : string.Empty
                    },
                    new Key()
                    {
                        Name = "ROUTE",
                        Value = material.Flow != null ? $"{material.Flow.Name}_{material.Flow.Version}" : string.Empty
                    },
                    new Key()
                    {
                        Name = "OPERATION",
                        Value = material.Step != null ? material.Step.Name : string.Empty
                    },
                    new Key()
                    {
                        Name = "PROCESS_SPS",
                        Value = material.RequiredService != null ? material.RequiredService.Name : string.Empty
                    },
                    new Key()
                    {
                        Name = "EQUIPMENT",
                        Value = resource != null ? resource.Name: string.Empty
                    },
                    new Key()
                    {
                        Name = "CHAMBER",
                        Value = subResource != null ? subResource.Name : string.Empty
                    },
                    new Key()
                    {
                        Name = "RECIPE",
                        Value = !string.IsNullOrWhiteSpace(material.CurrentRecipeInstance?.ParentEntity?.Name) ? material.CurrentRecipeInstance?.ParentEntity?.Name :string.Empty
                    },
                    new Key()
                    {
                        Name = "MEAS_EQUIPMENT",
                        Value = resource != null && resource.Type.Equals("Measure") ? resource.Type : string.Empty
                    },
                    new Key()
                    {
                        Name = "BATCH_NAME",
                        Value = Guid.NewGuid().ToString("N")
                    },
                    new Key()
                    {
                        Name = "LOT",
                        Value = material.Name
                    },
                    new Key()
                    {
                        Name = "QTY",
                        Value = $"{material.PrimaryQuantity + material.SubMaterialsPrimaryQuantity}"
                    },
                    new Key()
                    {
                        Name = "WAFER",
                        Value = "."
                    },
                    new Key()
                    {
                        Name = "PUNKT",
                        Value = "."
                    },
                    new Key()
                    {
                        Name = "X",
                        Value = "."
                    },
                    new Key()
                    {
                        Name = "Y",
                        Value = "."
                    }
                }
            };

            return customReportEDCToSpace;
        }


        /// <summary>
        /// Method to create xml message with Wafer and Data Collection Info to be sent to Space system
        /// </summary>
        /// <param name="wafer"></param>
        /// <param name="dataCollectionInstance"></param>
        /// <returns></returns>
        public static CustomReportEDCToSpace CreateSpaceInfoWaferValues(IMaterial wafer, IDataCollectionInstance dataCollectionInstance, IDataCollectionLimitSet limitSet)
        {
            // Get services provider information
            IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            CustomReportEDCToSpace customReportEDCToSpace = CreateSpaceInfoDefaultValues(wafer);

            List<Sample> samples = new List<Sample>();

            // get distinct parameters
            IParameterCollection parameters = entityFactory.CreateCollection<IParameterCollection>();

            dataCollectionInstance.LoadRelations();

            IDataCollection dc = dataCollectionInstance.DataCollection;
            dc.LoadRelations(Navigo.Common.Constants.DataCollectionParameter);

            parameters.AddRange(dc.DataCollectionParameters.Select(p => p.TargetEntity));
            parameters.Load();

            foreach (IParameter parameter in parameters)
            {
                if (parameter.DataType == ParameterDataType.Decimal || parameter.DataType == ParameterDataType.Long)
                {
                    Sample sample = new Sample();

                    // Get the DC Points for the specific parameter
                    IDataCollectionPointCollection points = entityFactory.CreateCollection<IDataCollectionPointCollection>();
                    points.AddRange(dataCollectionInstance.DataCollectionPoints.Where(p => p.TargetEntity.Name.Equals(parameter.Name)));

                    if (limitSet.DataCollectionParameterLimits.Any(ls => ls.TargetEntity.Name.Equals(parameter.Name)))
                    {
                        IDataCollectionParameterLimit parameterLimit = limitSet.DataCollectionParameterLimits.FirstOrDefault(ls => ls.TargetEntity.Name.Equals(parameter.Name));

                        sample.ParameterName = parameter.Name;
                        sample.ParameterUnit = parameter.DataUnit;

                        List<Key> sampleKeys = new List<Key>();
                        sampleKeys.Add(new Key()
                        {
                            Name = "Recipe",
                            Value = !string.IsNullOrWhiteSpace(wafer.CurrentRecipeInstance?.ParentEntity?.Name) ? wafer.CurrentRecipeInstance?.ParentEntity?.Name : string.Empty
                        });

                        if (parameterLimit != null)
                        {
                            if (parameterLimit.LowerErrorLimit != null && parameterLimit.UpperErrorLimit != null)
                            {
                                sample.Upper = parameterLimit.UpperErrorLimit.ToString();
                                sample.Lower = parameterLimit.LowerErrorLimit.ToString();
                            }

                            if (parameterLimit.LowerWarningLimit != null && parameterLimit.UpperWarningLimit != null)
                            {
                                sample.Upper = parameterLimit.UpperWarningLimit.ToString();
                                sample.Lower = parameterLimit.LowerWarningLimit.ToString();
                            }
                        }
                    }

                    Raws raws = new Raws();
                    raws.raws = new List<Raw>();
                    raws.StoreRaws = "True";

                    // Add the readings values
                    foreach (IDataCollectionPoint dcPoint in points)
                    {
                        Raw raw = new Raw();
                        raw.RawValue = Convert.ToDecimal(dcPoint.Value);

                        List<Key> rawKeys = new List<Key>();
                        rawKeys.Add(new Key()
                        {
                            Name = "WAFER",
                            Value = dcPoint.SampleId
                        });

                        raw.Keys = rawKeys;

                        raws.raws.Add(raw);
                    }

                    sample.Raws = raws;

                    samples.Add(sample);
                }
            }

            customReportEDCToSpace.Samples = samples;

            return customReportEDCToSpace;
        }

        #endregion

        #region Data Collection

        /// <summary>
        /// Method to validate if one of the posted points do not respect the configured limit set 
        /// </summary>
        /// <param name="dataCollectionInstance"></param>
        /// <returns></returns>
        public static bool IsDataCollectionLimiSetViolated(IDataCollectionInstance dataCollectionInstance)
        {
            dataCollectionInstance.LoadRelations("DataCollectionPoint");

            IDataCollectionLimitSet dataCollectionLimitSet = dataCollectionInstance.DataCollectionLimitSet;

            IDataCollectionPointCollection dataCollectionPoints = dataCollectionInstance.DataCollectionPoints;



            foreach (IDataCollectionParameterLimit parameterLimit in dataCollectionLimitSet.DataCollectionParameterLimits)
            {
                IDataCollectionPoint dcPoint = dataCollectionPoints.FirstOrDefault(dcp => dcp.GetNativeValue<long>(Constants.TargetEntity).Equals(parameterLimit.GetNativeValue<long>(Constants.TargetEntity)));

                decimal value = Convert.ToDecimal(dcPoint.Value);

                if ((parameterLimit.UpperErrorLimit != null && value > parameterLimit.UpperErrorLimit) || (parameterLimit.LowerErrorLimit != null && value < parameterLimit.LowerErrorLimit))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Method to post Certificate data for each wafer
        /// Execution method is Immediate
        /// </summary>
        /// <param name="dcInstance"></param>
        /// <param name="waferPoints"></param>
        /// <param name="parametersToUse"></param>
        /// <returns></returns>
        public static IDataCollectionInstance PostImmediateCertificateData(IDataCollectionInstance dcInstance, Dictionary<string, object> waferPoints, IParameterCollection parametersToUse = null)
        {
            //insert dc point values
            IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IDataCollectionPointCollection dcPoints = entityFactory.CreateCollection<IDataCollectionPointCollection>();

            foreach (IParameter parameter in parametersToUse)
            {
                IDataCollectionPoint point = entityFactory.Create<IDataCollectionPoint>();
                point.SampleId = "Sample 1";
                point.ReadingNumber = 1;
                point.TargetEntity = parameter;
                point.Value = amsOSRAMUtilities.GetParameterValueAsDataType(parameter.DataType, waferPoints[parameter.Name].ToString());

                dcPoints.Add(point);

                if (dcInstance.RelationCollection == null)
                    dcInstance.RelationCollection = new CmfEntityRelationCollection();

                dcInstance.RelationCollection.Add(point);
            }

            IDataCollectionInstanceOrchestration dataCollectionInstanceManagementOrchestration = serviceProvider.GetService<IDataCollectionInstanceOrchestration>();
            PerformImmediateDataCollectionOutput dataCollectionInstanceResult = dataCollectionInstanceManagementOrchestration.PerformImmediateDataCollection(
                new PerformImmediateDataCollectionInput()
                {
                    DataCollectionInstance = dcInstance,
                    SkipDCValidation = false,
                    IsToIgnoreInSPC = true,
                    IgnoreLastServiceId = false
                }
            );

            return dataCollectionInstanceResult.DataCollectionInstance;
        }

        /// <summary>
        /// Method to post Certificate data for each wafer
        /// Execution method is LongRunning
        /// </summary>
        /// <param name="dcInstance"></param>
        /// <param name="waferPoints"></param>
        /// <param name="parametersToUse"></param>
        /// <returns></returns>
        public static IDataCollectionInstance PostLongRunningCertificateData(IDataCollectionInstance dcInstance, Dictionary<string, object> waferPoints, IParameterCollection parametersToUse)
        {
            OpenDataCollectionInstanceInput openDCInstanceInput = new OpenDataCollectionInstanceInput()
            {
                DataCollectionInstance = dcInstance,
                IsToIgnoreInSPC = true,
            };

            IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();
            IDataCollectionInstanceOrchestration dataCollectionInstanceManagementOrchestration = serviceProvider.GetService<IDataCollectionInstanceOrchestration>();

            OpenDataCollectionInstanceOutput openDCInstanceOutput = dataCollectionInstanceManagementOrchestration.OpenDataCollectionInstance(openDCInstanceInput);
            dcInstance = openDCInstanceOutput.DataCollectionInstance;

            //insert dc point values 
            IDataCollectionPointCollection dcPoints = entityFactory.CreateCollection<IDataCollectionPointCollection>();
            foreach (IParameter parameter in parametersToUse)
            {
                IDataCollectionPoint point = entityFactory.Create<IDataCollectionPoint>();
                point.SampleId = "Sample 1";
                point.ReadingNumber = 1;
                point.TargetEntity = parameter;
                point.SourceEntity = dcInstance;
                point.Value = amsOSRAMUtilities.GetParameterValueAsDataType(parameter.DataType, waferPoints[parameter.Name].ToString());

                dcPoints.Add(point);

                if (dcInstance.RelationCollection == null)
                    dcInstance.RelationCollection = new CmfEntityRelationCollection();

                dcInstance.RelationCollection.Add(point);

            }
            dcInstance.Load();

            PostDataCollectionPointsInput postDCPointsInput = new PostDataCollectionPointsInput()
            {
                DataCollectionInstance = dcInstance,
                DataCollectionPoints = dcPoints,
                SkipDCValidation = false
            };

            PostDataCollectionPointsOutput postDataCollectionPointsOutput = dataCollectionInstanceManagementOrchestration.PostDataCollectionPoints(postDCPointsInput);

            return postDataCollectionPointsOutput.DataCollectionInstance;
        }

        public static IDataCollectionInstance PerformCertificateDataCollection(IMaterial wafer, IDataCollection dataCollection, IDataCollectionLimitSet limitSet, string executionType, Dictionary<string, object> waferPoints)
        {
            IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IDataCollectionInstance dataCollectionInstance = entityFactory.Create<IDataCollectionInstance>();

            dataCollectionInstance.Material = wafer;
            dataCollectionInstance.DataCollection = dataCollection;
            dataCollectionInstance.DataCollectionLimitSet = limitSet;

            IParameterCollection parameters = entityFactory.CreateCollection<IParameterCollection>();

            foreach (KeyValuePair<string, object> waferPoint in waferPoints)
            {
                IParameter parameter = entityFactory.Create<IParameter>();
                parameter.Name = waferPoint.Key;
                parameters.Add(parameter);
            }

            parameters.Load();

            if (executionType == "LongRunning")
            {
                return PostLongRunningCertificateData(dataCollectionInstance, waferPoints, parameters);
            }
            if (executionType == "Immediate")
            {
                return PostImmediateCertificateData(dataCollectionInstance, waferPoints, parameters);
            }

            return null;
        }

        #endregion

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

        /// <summary>
        /// Method to get the available information for nice label printing
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="resource"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDataForNiceLabelPrinting(IMaterial lot, IResource resource, string operation)
        {
            // Get Material information
            string stepName = lot.Step.Name;
            string materialName = lot.Name;
            string productName = lot.Product.Name;
            string logicalFlowPath = lot.LogicalFlowPath != null ? lot.LogicalFlowPath : string.Empty;
            string productGroupName = lot.Product.ProductGroup != null ? lot.Product.ProductGroup.Name : string.Empty;
            string flowName = lot.Flow.Name;
            string resourceName = resource != null ? resource.Name : string.Empty;
            string resourceType = resource != null ? resource.Type : string.Empty;
            string resourceModel = resource != null ? resource.Model : string.Empty;

            DataRow row = CustomResolveSTCustomMaterialNiceLabelPrintContext(stepName, logicalFlowPath, productName, productGroupName, flowName, materialName, lot.Type, resourceName, resourceType, resourceModel, operation);

            Dictionary<string, string> materialNiceLabelPrintInformation = new Dictionary<string, string>();

            if (row != null && row.Field<bool>("IsEnabled"))
            {
                IContainer container = null;

                if (lot.SubMaterialCount > 0)
                {
                    lot.LoadChildren();

                    IMaterial logicalWafer = lot.SubMaterials.First();

                    logicalWafer.LoadRelations("MaterialContainer");

                    if (logicalWafer.RelationCollection != null && logicalWafer.RelationCollection.ContainsKey("MaterialContainer") && logicalWafer.RelationCollection["MaterialContainer"].Count > 0)
                    {
                        container = logicalWafer.RelationCollection["MaterialContainer"].First().TargetEntity as IContainer;
                    }
                }

                // add addictional information about the lot
                // TODO: Missing information to map: LotAlias; BatchName; LotOwner; LotWaferCount; 
                materialNiceLabelPrintInformation.AddRange(new Dictionary<string, string>()
                {
                    { "LABEL_NAME", row.Field<string>(amsOSRAMConstants.CustomMaterialNiceLabelPrintContextLabel) },
                    { "PRINTER_NAME", row.Field<string>(amsOSRAMConstants.CustomMaterialNiceLabelPrintContextPrinter) },
                    { "LABEL_QUANTITY", row.Field<int>(amsOSRAMConstants.CustomMaterialNiceLabelPrintContextQuantity).ToString() },
                    { "LotName", lot.Name },
                    { "LotAlias", "" },
                    { "ProductName", productName },
                    { "ProductDesc", lot.Product.Description },
                    { "ProductType", lot.Product.ProductType.ToString() },
                    { "Product_Type", lot.Product.Type },
                    { "ProductGroupName", string.IsNullOrEmpty(productGroupName) ? string.Empty : productGroupName },
                    { "ProductGroup_Type", lot.Product.ProductGroup != null ? lot.Product.ProductGroup.Type : string.Empty },
                    { "FlowName", flowName },
                    { "BatchName", "" },
                    { "ContainerName", container != null ? container.Name : string.Empty },
                    { "ExperimentName", lot.Experiment != null ? lot.Experiment.Name : string.Empty},
                    { "ProductionOrder", lot.ProductionOrder != null ? lot.ProductionOrder.Name : string.Empty },
                    { "LotOwner", "" },
                    { "ResourceName", string.IsNullOrEmpty(resourceName) ? string.Empty : resourceName },
                    { "LotWaferCount", lot.SubMaterialCount.ToString() },
                    { "LotPrimaryQty", lot.PrimaryQuantity.HasValue ? lot.PrimaryQuantity.ToString() : string.Empty },
                    { "LotSecondaryQty", lot.SecondaryQuantity.HasValue ? lot.SecondaryQuantity.ToString() : string.Empty },
                    { "Lot_Type", lot.Type },
                    { "Area", resource != null ? resource.Area.Name : string.Empty },
                    { "Facility", lot.Facility.Name }
                });
            }

            return materialNiceLabelPrintInformation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lot"></param>
        /// <returns></returns>
        public static INgpDataSet CustomResolveCertificateDataCollectionContext(IMaterial lot)
        {
            // Get Material information
            string stepName = lot.Step?.Name;
            string materialName = lot.Name;
            string productName = lot.Product.Name;
            string logicalFlowPath = lot.LogicalFlowPath != null ? lot.LogicalFlowPath : string.Empty;
            string productGroupName = lot.Product.ProductGroup != null ? lot.Product.ProductGroup.Name : string.Empty;
            string flowName = lot.Flow.Name;
            IResource resource = lot.LastProcessedResource;

            string resourceName = resource != null ? resource.Name : string.Empty;
            string resourceType = resource != null ? resource.Type : string.Empty;
            string resourceModel = resource != null ? resource.Model : string.Empty;
            string operation = amsOSRAMConstants.CustomIncomingLotCreationOperation;

            return amsOSRAMUtilities.CustomResolveMaterialDataCollectionContext(stepName, logicalFlowPath, productName, productGroupName, flowName, materialName, lot.Type, resourceName, resourceType, resourceModel, operation);
        }

        public static void SetMaterialStateModel(IMaterial material, string stateModelName, string state)
        {
            if (material.CurrentMainState == null
                || !material.CurrentMainState.CurrentState.Name.Equals(state))
            {
                IStateModel stateModel = new StateModel()
                {
                    Name = stateModelName
                };

                stateModel.Load();

                IStateModelState stateModelState = new StateModelState();
                stateModelState.Load(state, stateModel);

                IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
                IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

                ICurrentEntityState currentEntityState = entityFactory.Create<ICurrentEntityState>();
                currentEntityState.Entity = material;
                currentEntityState.StateModel = stateModel;
                currentEntityState.CurrentState = stateModelState;

                material.SetMainStateModel(currentEntityState);
            }
        }

        /// <summary>
        /// Gets the Entity Attributes Definition (Name; Type)
        /// </summary>
        /// <param name="entityName">The Entity Name.</param>
        /// <returns>A Dictionary of the Attributes Definition.</returns>
        public static Dictionary<string, object> GetEntityAttributesDefinition(string entityName)
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();

            IEntityType entityType = new EntityType();

            entityType.Load(entityName);

            entityType.LoadProperties();

            if (entityType.Properties != null && entityType.Properties.Any())
            {
                IEnumerable<IEntityTypeProperty> attributesDefinition = entityType.Properties.Where(w => w.PropertyType == EntityTypePropertyType.Attribute);

                if (attributesDefinition != null && attributesDefinition.Any())
                {
                    attributes = attributesDefinition.Select(s => new KeyValuePair<string, object>(s.Name, s.ScalarType)).ToDictionary(d => d.Key, d => d.Value);
                }
            }

            return attributes;
        }

        /// <summary>
        /// Gets the Material Attributes From XML.
        /// </summary>
        /// <param name="lotAttributes"></param>
        /// <param name="xmlAttributes"></param>
        /// <returns></returns>
        public static IAttributeCollection GetMaterialAttributesFromXML(Dictionary<string, object> lotAttributes, List<MaterialAttributes> xmlAttributes)
        {
            IAttributeCollection attributes = new AttributeCollection();

            foreach (MaterialAttributes attribute in xmlAttributes)
            {
                if (lotAttributes != null && lotAttributes.ContainsKey(attribute.Name))
                {
                    IScalarType scalarType = lotAttributes[attribute.Name] as IScalarType;
                    attributes.Add(attribute.Name, amsOSRAMUtilities.GetAttributeValueAsDataType(scalarType, attribute.value));
                }
            }

            return attributes;
        }

        public static Dictionary<string, object> GetMaterialEDCDataFromXML(List<MaterialEDCData> xmlEDCCollection)
        {
            Dictionary<string, object> edcData = new Dictionary<string, object>();

            foreach (MaterialEDCData xmlEDC in xmlEDCCollection)
            {
                edcData.Add(xmlEDC.Name, xmlEDC.Value);
            }

            return edcData;
        }

        /// <summary>
        /// Creates an outbound integration 
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="messageType">Type of message</param>
        /// <param name="headerAttributes">Message header attributes to be added as attributes of Integration Entry if existent</param>
        /// <returns>Created Integration Entry</returns>
        public static IIntegrationEntry CreateOutboundIntegrationEntry(string message, string messageType, string name = null, IAttributeCollection headerAttributes = null)
        {
            return CreateIntegrationEntry(message, messageType, BrokerMessageDirection.Outbound, name, headerAttributes);
        }

        /// <summary>
        /// Creates an inbound integration entry
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="messageType">Type of message</param>
        /// <param name="headerAttributes">Message header attributes to be added as attributes of Integration Entry if existent</param>
        /// <returns></returns>
        public static IIntegrationEntry CreateInboundIntegrationEntry(string message, string messageType, string name = null, IAttributeCollection headerAttributes = null)
        {
            return CreateIntegrationEntry(message, messageType, BrokerMessageDirection.Inbound, name, headerAttributes);
        }

        /// <summary>
        /// Creates an integration entry 
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="messageType">Type of message</param>
        /// <param name="messageDirection">Direction of message</param>
        /// <param name="headerAttributes">Message header attributes to be added as attributes of Integration Entry if existent</param>
        /// <returns>Created Integration Entry</returns>
        private static IIntegrationEntry CreateIntegrationEntry(string message, string messageType, BrokerMessageDirection messageDirection, string name = null, IAttributeCollection headerAttributes = null)
        {
            IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IIntegrationEntry ie = entityFactory.Create<IIntegrationEntry>();

            ie.Name = name != null ? name : Guid.NewGuid().ToString();
            ie.MessageType = messageType;
            ie.MessageDate = DateTime.Now;
            ie.IntegrationMessage.Message = Encoding.UTF8.GetBytes(message);

            ie.EventName = (messageDirection == BrokerMessageDirection.Inbound ? amsOSRAMConstants.ERPInfoReceivedEventName : amsOSRAMConstants.ERPInfoSentEventName);
            ie.SourceSystem = (messageDirection == BrokerMessageDirection.Inbound ? amsOSRAMConstants.CustomERPSystem : Constants.MesSystemDesignation);
            ie.TargetSystem = (messageDirection == BrokerMessageDirection.Inbound ? Constants.MesSystemDesignation : amsOSRAMConstants.CustomERPSystem);

            ie.NumberOfRetries = 0;
            ie.IsRetriable = true;
            ie.SystemState = IntegrationEntrySystemState.Received;


            if (headerAttributes != null)
            {
                foreach (KeyValuePair<string, object> attr in headerAttributes)
                {
                    ie.Attributes[attr.Key] = attr.Value;
                }
            }

            CreateObjectInput createInput = new CreateObjectInput
            {
                Object = ie
            };

            ie.Create();

            return ie;
        }

        #endregion

        #region Localized Messages

        /// <summary>
        /// Constructs a new Message, using Localized Messages
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GetLocalizedMessage(string key, params string[] parameters)
        {
            IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
            ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();

            return string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, key), parameters);
        }

        /// <summary>
        /// Constructs a new Exception Message, using Localized Messages
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static void ThrowLocalizedException(string key, params string[] parameters)
        {
            string exceptionMessage = GeneralUtilities.GetLocalizedMessage(key, parameters);

            throw new Exception(exceptionMessage);
        }

        /// <summary>
        /// Constructs a new Message, using Localized Messages
        /// </summary>
        /// <param name="key">Name of Localized Message</param>
        public static void ThrowLocalizedException(string key)
        {
            IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
            ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();

            throw new Exception(localizationService.Localize(key));
        }

        #endregion Localized Messages

        #region Material

        /// <summary>
        /// Hold Material with associated Hold Reason
        /// </summary>
        /// <param name="material">The Material</param>
        /// <param name="reasonName">Hold Reason Name</param>
        public static void HoldMaterial(this IMaterial material, string reasonName)
        {
            // Check if reason exists
            if (string.IsNullOrWhiteSpace(reasonName))
            {
                throw new ArgumentNullCmfException("ReasonName");
            }

            // Load Material Hold Reasons
            material.LoadRelations(Navigo.Common.Constants.MaterialHoldReason);

            // Check if Material has that Hold Reason
            if (material.MaterialHoldReasons is null || !material.MaterialHoldReasons.Any(holdReason => holdReason.TargetEntity.Name.Equals(reasonName)))
            {
                IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
                IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

                // Load hold Reason
                IReason reason = entityFactory.Create<IReason>();
                reason.Load(reasonName);

                IMaterialHoldReason materialHoldReason = entityFactory.Create<IMaterialHoldReason>();
                materialHoldReason.SourceEntity = material;
                materialHoldReason.TargetEntity = reason;

                IMaterialHoldReasonCollection materialHoldReasons = entityFactory.CreateCollection<IMaterialHoldReasonCollection>();
                materialHoldReasons.Add(materialHoldReason);

                // Put Material on Hold
                material.Hold(materialHoldReasons);
            }
        }

        /// <summary>
        /// Get Material Source Path
        /// </summary>
        /// <param name="material">Material</param>
        /// <returns></returns>
        public static string GetMaterialSourcePath(IMaterial material)
        {
            string stepLogicalName = string.Empty;
            string facilityCode, siteCode;
            facilityCode = siteCode = "EMPTY";

            // Get FacilityCode attribute value
            if (material.Facility.HasAttribute(amsOSRAMConstants.CustomFacilityCodeAttribute, true))
            {
                facilityCode = material.Facility.GetAttributeValue(amsOSRAMConstants.CustomFacilityCodeAttribute) as string;
            }

            // Get SiteCode attribute value
            if (material.Facility.Site != null)
            {
                material.Facility.Site.Load();
                if (material.Facility.Site.HasAttribute(amsOSRAMConstants.CustomSiteCodeAttribute, true))
                {
                    siteCode = material.Facility.Site.GetAttributeValue(amsOSRAMConstants.CustomSiteCodeAttribute) as string;
                }
            }
            // Get Step LogicalName value
            if (material.Step.ContainsLogicalNames)
            {
                IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
                IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

                material.Flow.LoadRelations(Navigo.Common.Constants.FlowStep);
                IFlowStep flowStep = entityFactory.Create<IFlowStep>();
                IStep step = entityFactory.Create<IStep>();

                material.Flow.GetFlowAndStepFromFlowpath(material.FlowPath, ref step, ref flowStep);
                stepLogicalName = flowStep.LogicalName;
            }
            else
            {
                stepLogicalName = material.Step.Name;
            }

            // Build in a string the MaterialPath
            return string.Format("{0}.{1}.{2}", siteCode, facilityCode, stepLogicalName);
        }

        #endregion

        #region XML 

        /// <summary>
        /// Deserialize Xml To Object
        /// </summary>
        /// <typeparam name="T">Serializable Class</typeparam>
        /// <param name="xml">XML</param>
        /// <returns>Object</returns>
        public static T DeserializeXmlToObject<T>(string xml)
        {
            T newObject;
            // Construct an instance of the XmlSerializer with the type
            // of object that is being deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(xml))
            {
                // Call the Deserialize method and cast to the object type.
                newObject = (T)serializer.Deserialize(reader);
            }
            return newObject;
        }

        /// <summary>
        /// Serialize Object to XML
        /// </summary>
        /// <typeparam name="T">Serializable Type</typeparam>
        /// <param name="value">Object to be serialized</param>
        /// <returns></returns>
        public static string SerializeToXML<T>(this T value)
        {
            string output = string.Empty;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, value);
                output = writer.ToString();
            }
            return output;
        }

        #endregion

        #region Queries

        /// <summary>
        /// Get Production Order by Product associated with a Material
        /// </summary>
        /// <param name="productName">Product Name</param>
        /// <param name="trackOutDate">Trackout Date</param>
        /// <returns>Return Production Order based on Product and TrackOut Date associated with a Material</returns>
        public static IProductionOrder GetMaterialProductionOrder(string productName, DateTime? trackOutDate)
        {
            IQueryObject query = new QueryObject();
            query.Description = "";
            query.EntityTypeName = "ProductionOrder";
            query.Name = "CustomMaterialProductionOrder";
            query.Query = new Query();
            query.Query.Distinct = false;
            query.Query.Filters = new FilterCollection()
            {
                new Filter()
                {
                    Name = "Name",
                    ObjectName = "Product",
                    ObjectAlias = "ProductionOrder_Product_2",
                    Operator = FieldOperator.IsEqualTo,
                    Value = productName,
                    LogicalOperator = LogicalOperator.AND,
                    FilterType = Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal
                },
                new Filter()
                {
                    Name = "PlannedStartDate",
                    ObjectName = "ProductionOrder",
                    ObjectAlias = "ProductionOrder_1",
                    Operator = FieldOperator.LessThanOrEqualTo,
                    Value = trackOutDate,
                    LogicalOperator = LogicalOperator.AND,
                    FilterType = Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal
                },
                new Filter()
                {
                    Name = "UniversalState",
                    ObjectName = "ProductionOrder",
                    ObjectAlias = "ProductionOrder_1",
                    Operator = FieldOperator.IsEqualTo,
                    Value = 2,
                    LogicalOperator = LogicalOperator.Nothing,
                    FilterType = Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal
                }
            };
            query.Query.Fields = new FieldCollection()
            {
                new Field()
                {
                    Alias = "Id",
                    ObjectName = "ProductionOrder",
                    ObjectAlias = "ProductionOrder_1",
                    IsUserAttribute = false,
                    Name = "Id",
                    Position = 0,
                    Sort = FieldSort.NoSort
                },
                new Field()
                {
                    Alias = "Name",
                    ObjectName = "ProductionOrder",
                    ObjectAlias = "ProductionOrder_1",
                    IsUserAttribute = false,
                    Name = "Name",
                    Position = 1,
                    Sort = FieldSort.NoSort
                },
                new Field()
                {
                    Alias = "PlannedStartDate",
                    ObjectName = "ProductionOrder",
                    ObjectAlias = "ProductionOrder_1",
                    IsUserAttribute = false,
                    Name = "PlannedStartDate",
                    Position = 2,
                    Sort = FieldSort.Descending
                }
            };
            query.Query.Relations = new RelationCollection()
            {
                new Relation()
                {
                    Alias = "",
                    IsRelation = false,
                    Name = "",
                    SourceEntity = "ProductionOrder",
                    SourceEntityAlias = "ProductionOrder_1",
                    SourceJoinType = Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
                    SourceProperty = "ProductId",
                    TargetEntity = "Product",
                    TargetEntityAlias = "ProductionOrder_Product_2",
                    TargetJoinType = Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
                    TargetProperty = "Id"
                }
            };
            query.Query.Top = 1;

            DataSet dataSet = query.Execute(false, null);

            IProductionOrder productionOrder = null;

            if (dataSet.HasData())
            {
                IServiceProvider serviceProvider = ApplicationContext.CurrentServiceProvider;
                IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

                productionOrder = entityFactory.Create<IProductionOrder>();

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    productionOrder.Name = row["Name"].ToString();

                    productionOrder.Load();
                }
            }

            return productionOrder;
        }

        #endregion

        #region ERP

        /// <summary>
        /// Create information to send for ERP
        /// </summary>
        /// <param name="movementType">ERP Movement Type</param>
        /// <param name="productionOrder">Production Order</param>
        /// <param name="material">Material</param>
        /// <returns>Returns an object associated with Movement Type</returns>
        public static CustomReportToERPItem CreateInfoForERP(string movementType, string storageLocation, string siteCode, IProductionOrder productionOrder = null, IMaterial material = null)
        {
            CustomReportToERPItem customReportToERPItem = null;

            if (string.IsNullOrEmpty(movementType))
            {
                ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomMovementTypeEmpty);
            }

            switch (movementType)
            {
                case amsOSRAMConstants.Type261:

                    if (productionOrder is null)
                    {
                        ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomProductionOrderObjectNull);
                    }

                    if (material is null)
                    {
                        ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomMaterialObjectNull);
                    }

                    customReportToERPItem = new CustomReportToERPItem()
                    {
                        Id = DateTime.Now.ToString("yyyyMMdd_HHmmssfff"),
                        ProductionOrderNumber = productionOrder.OrderNumber,
                        MaterialName = material.Name,
                        ProductName = material.Product.Name,
                        Quantity = (int)((material.PrimaryQuantity ?? 0) + (material.SubMaterialsPrimaryQuantity ?? 0)),
                        Units = material.PrimaryUnits,
                        MovementType = movementType,
                        SAPStore = storageLocation,
                        Site = siteCode
                    };

                    break;
            }

            return customReportToERPItem;
        }

        #endregion
    }
}
