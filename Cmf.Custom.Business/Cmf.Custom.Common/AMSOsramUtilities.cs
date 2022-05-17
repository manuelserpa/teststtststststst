using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.BusinessObjects;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Custom.AMSOsram.Common.ERP;
using Cmf.Custom.AMSOsram.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Integration;
using Cmf.Foundation.Configuration;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement;
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
using static Cmf.Custom.AMSOsram.Common.AMSOsramConstants;

namespace Cmf.Custom.AMSOsram.Common
{
    /// <summary>
    /// Support class to encapsulate methods to support the development for the business layer
    /// </summary>
    public static class AMSOsramUtilities
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
            var collection = enumerable as ICollection<T>;

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
        public static dynamic GetAttributeValueAsDataType(ScalarType scalarType, string value)
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

            NgpDataSet niceLabelPrintContextNgpDataSet = customMaterialNiceLabelPrintContext.Resolve(values, true);

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
        public static NgpDataSet CustomResolveMaterialDataCollectionContext(string step = null,
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
            SmartTable materialDatacollectionContext = new SmartTable();
            materialDatacollectionContext.Load(Cmf.Navigo.Common.Constants.MaterialDataCollectionContext);

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

            NgpDataSet materialDCContextNgpDataSet = materialDatacollectionContext.Resolve(values, true);



            return materialDCContextNgpDataSet;
        }

        /// <summary>
        /// Result custom ST CustomReportConsumptionToSAP
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public static string CustomResolveSTCustomReportConsumptionToSAP(Material material)
        {
            string storageLocation = string.Empty;
            SmartTable smartTable = new SmartTable();
            smartTable.Load(AMSOsramConstants.CustomReportConsumptionToSAPSmartTable);

            NgpDataRow values = new NgpDataRow();
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
            
            NgpDataSet ngpDataSet = smartTable.Resolve(values, true);
            if (ngpDataSet != null && ngpDataSet.Tables != null && ngpDataSet.Tables.Count > 0)
            {
                DataSet dataSet = NgpDataSet.ToDataSet(ngpDataSet);
                if (dataSet.HasData())
                {
                    storageLocation = dataSet.Tables[0].Rows[0][AMSOsramConstants.CustomStorageLocation].ToString();
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
        /// Retrieves the container and load port for the given lot
        /// </summary>
        /// <param name="material">The lot</param>
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
        /// Retrives a BOM from BOM context for a specific lot.
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

        #region Data Collection

        /// <summary>
        /// Method to validate if one of the posted points do not respect the configured limit set 
        /// </summary>
        /// <param name="dataCollectionInstance"></param>
        /// <returns></returns>
        public static bool IsDataCollectionLimiSetViolated(DataCollectionInstance dataCollectionInstance)
        {
            dataCollectionInstance.LoadRelations("DataCollectionPoint");

            DataCollectionLimitSet dataCollectionLimitSet = dataCollectionInstance.DataCollectionLimitSet;

            DataCollectionPointCollection dataCollectionPoints = dataCollectionInstance.DataCollectionPoints;



            foreach (DataCollectionParameterLimit parameterLimit in dataCollectionLimitSet.DataCollectionParameterLimits)
            {
                DataCollectionPoint dcPoint = dataCollectionPoints.FirstOrDefault(dcp => dcp.GetNativeValue<long>(Constants.TargetEntity).Equals(parameterLimit.GetNativeValue<long>(Constants.TargetEntity)));

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
        public static DataCollectionInstance PostImmediateCertificateData(DataCollectionInstance dcInstance, Dictionary<string, object> waferPoints, ParameterCollection parametersToUse = null)
        {
            //insert dc point values 
            DataCollectionPointCollection dcPoints = new DataCollectionPointCollection();

            foreach (Parameter parameter in parametersToUse)
            {
                DataCollectionPoint point = new DataCollectionPoint()
                {
                    SampleId = "Sample 1",
                    ReadingNumber = 1,
                    TargetEntity = parameter,
                    Value = AMSOsramUtilities.GetParameterValueAsDataType(parameter.DataType, waferPoints[parameter.Name].ToString())
                };

                dcPoints.Add(point);

                if (dcInstance.RelationCollection == null)
                    dcInstance.RelationCollection = new CmfEntityRelationCollection();

                dcInstance.RelationCollection.Add(point);
            }

            PerformImmediateDataCollectionOutput dataCollectionInstanceResult = DataCollectionInstanceManagementOrchestration.PerformImmediateDataCollection(
                new Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.InputObjects.PerformImmediateDataCollectionInput()
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
        public static DataCollectionInstance PostLongRunningCertificateData(DataCollectionInstance dcInstance, Dictionary<string, object> waferPoints, ParameterCollection parametersToUse)
        {
            OpenDataCollectionInstanceInput openDCInstanceInput = new OpenDataCollectionInstanceInput()
            {
                DataCollectionInstance = dcInstance,
                IsToIgnoreInSPC = true,
            };

            OpenDataCollectionInstanceOutput openDCInstanceOutput = DataCollectionInstanceManagementOrchestration.OpenDataCollectionInstance(openDCInstanceInput);
            dcInstance = openDCInstanceOutput.DataCollectionInstance;

            //insert dc point values 
            DataCollectionPointCollection dcPoints = new DataCollectionPointCollection();
            foreach (Parameter parameter in parametersToUse)
            {
                DataCollectionPoint point = new DataCollectionPoint()
                {
                    SampleId = "Sample 1",
                    ReadingNumber = 1,
                    TargetEntity = parameter,
                    SourceEntity = dcInstance,
                    Value = AMSOsramUtilities.GetParameterValueAsDataType(parameter.DataType, waferPoints[parameter.Name].ToString())
                };
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

            PostDataCollectionPointsOutput postDataCollectionPointsOutput = DataCollectionInstanceManagementOrchestration.PostDataCollectionPoints(postDCPointsInput);

            return postDataCollectionPointsOutput.DataCollectionInstance;
        }

        public static DataCollectionInstance PerformCertificateDataCollection(Material wafer, DataCollection dataCollection, DataCollectionLimitSet limitSet, string executionType, Dictionary<string, object> waferPoints)
        {
            DataCollectionInstance dataCollectionInstance = new DataCollectionInstance();

            dataCollectionInstance.Material = wafer;
            dataCollectionInstance.DataCollection = dataCollection;
            dataCollectionInstance.DataCollectionLimitSet = limitSet;

            ParameterCollection parameters = new ParameterCollection();

            foreach (var waferPoint in waferPoints)
            {
                parameters.Add(new Parameter() { Name = waferPoint.Key });
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
        public static Dictionary<string, string> GetDataForNiceLabelPrinting(Material lot, Resource resource, string operation)
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
                Container container = null;

                if (lot.SubMaterialCount > 0)
                {
                    lot.LoadChildren();

                    Material logicalWafer = lot.SubMaterials.First();

                    logicalWafer.LoadRelations("MaterialContainer");

                    if (logicalWafer.RelationCollection != null && logicalWafer.RelationCollection.ContainsKey("MaterialContainer") && logicalWafer.RelationCollection["MaterialContainer"].Count > 0)
                    {
                        container = logicalWafer.RelationCollection["MaterialContainer"].First().TargetEntity as Container;
                    }
                }

                // add addictional information about the lot
                // TODO: Missing information to map: LotAlias; BatchName; LotOwner; LotWaferCount; 
                materialNiceLabelPrintInformation.AddRange(new Dictionary<string, string>()
                {
                    { "LABEL_NAME", row.Field<string>(AMSOsramConstants.CustomMaterialNiceLabelPrintContextLabel) },
                    { "PRINTER_NAME", row.Field<string>(AMSOsramConstants.CustomMaterialNiceLabelPrintContextPrinter) },
                    { "LABEL_QUANTITY", row.Field<int>(AMSOsramConstants.CustomMaterialNiceLabelPrintContextQuantity).ToString() },
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
        public static NgpDataSet CustomResolveCertificateDataCollectionContext(Material lot)
        {
            // Get Material information
            string stepName = lot.Step?.Name;
            string materialName = lot.Name;
            string productName = lot.Product.Name;
            string logicalFlowPath = lot.LogicalFlowPath != null ? lot.LogicalFlowPath : string.Empty;
            string productGroupName = lot.Product.ProductGroup != null ? lot.Product.ProductGroup.Name : string.Empty;
            string flowName = lot.Flow.Name;
            Resource resource = lot.LastProcessedResource;

            string resourceName = resource != null ? resource.Name : string.Empty;
            string resourceType = resource != null ? resource.Type : string.Empty;
            string resourceModel = resource != null ? resource.Model : string.Empty;
            string operation = AMSOsramConstants.CustomIncomingLotCreationOperation;

            return AMSOsramUtilities.CustomResolveMaterialDataCollectionContext(stepName, logicalFlowPath, productName, productGroupName, flowName, materialName, lot.Type, resourceName, resourceType, resourceModel, operation);
        }

        public static void SetMaterialStateModel(Material material, string stateModelName, string state)
        {
            if (material.CurrentMainState == null
                || !material.CurrentMainState.CurrentState.Name.Equals(state))
            {
                StateModel stateModel = new StateModel()
                {
                    Name = stateModelName
                };

                stateModel.Load();
                StateModelState stateModelState = new StateModelState();
                stateModelState.Load(state, stateModel);

                CurrentEntityState currentEntityState = new CurrentEntityState(material, stateModel, stateModelState);
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

            EntityType entityType = new EntityType();

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
        public static AttributeCollection GetMaterialAttributesFromXML(Dictionary<string, object> lotAttributes, List<MaterialAttributes> xmlAttributes)
        {
            AttributeCollection attributes = new AttributeCollection();

            foreach (MaterialAttributes attribute in xmlAttributes)
            {
                if (lotAttributes != null && lotAttributes.ContainsKey(attribute.Name))
                {
                    ScalarType scalarType = lotAttributes[attribute.Name] as ScalarType;
                    attributes.Add(attribute.Name, AMSOsramUtilities.GetAttributeValueAsDataType(scalarType, attribute.value));
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
        public static IntegrationEntry CreateOutboundIntegrationEntry(string message, string messageType, string name = null, AttributeCollection headerAttributes = null)
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
        public static IntegrationEntry CreateInboundIntegrationEntry(string message, string messageType, string name = null, AttributeCollection headerAttributes = null)
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
        private static IntegrationEntry CreateIntegrationEntry(string message, string messageType, BrokerMessageDirection messageDirection, string name = null, AttributeCollection headerAttributes = null)
        {
            IntegrationEntry ie = new IntegrationEntry();

            ie.Name = name != null ? name : Guid.NewGuid().ToString();
            ie.MessageType = messageType;
            ie.MessageDate = DateTime.Now;
            ie.IntegrationMessage.Message = Encoding.UTF8.GetBytes(message);

            ie.EventName = (messageDirection == BrokerMessageDirection.Inbound ? AMSOsramConstants.ERPInfoReceivedEventName : AMSOsramConstants.ERPInfoSentEventName);
            ie.SourceSystem = (messageDirection == BrokerMessageDirection.Inbound ? AMSOsramConstants.CustomERPSystem : Constants.MesSystemDesignation);
            ie.TargetSystem = (messageDirection == BrokerMessageDirection.Inbound ? Constants.MesSystemDesignation : AMSOsramConstants.CustomERPSystem);

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

            var createInput = new CreateObjectInput
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
            LocalizedMessage localizedMessageObj = LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name, key);
            return string.Format(localizedMessageObj.MessageText, parameters);
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
            string exceptionMessage = LocalizedMessage.GetLocalizedMessage(key).MessageText;

            throw new Exception(exceptionMessage);
        }

        #endregion Localized Messages

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
        public static ProductionOrder GetMaterialProductionOrder(string productName, DateTime? trackOutDate)
        {
            QueryObject query = new QueryObject();
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

            ProductionOrder productionOrder = null;

            if (dataSet.HasData())
            {
                productionOrder = new ProductionOrder();

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
        public static CustomReportToERPItem CreateInfoForERP(string movementType,string storageLocation, string siteCode, ProductionOrder productionOrder = null, Material material = null)
        {
            CustomReportToERPItem customReportToERPItem = null;

            if (string.IsNullOrEmpty(movementType))
            {
                ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomMovementTypeEmpty);
            }

            switch (movementType)
            {
                case AMSOsramConstants.Type261:

                    if (productionOrder is null)
                    {
                        ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomProductionOrderObjectNull);
                    }

                    if (material is null)
                    {
                        ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomMaterialObjectNull);
                    }

                    customReportToERPItem = new CustomReportToERPItem()
                    {
                        CreatedOn = DateTime.Now,
                        ProductionOrderNumber = productionOrder.OrderNumber,
                        MaterialName = material.Name,
                        ProductName = material.Product.Name,
                        Quantity = material.PrimaryQuantity + material.SubMaterialsPrimaryQuantity,
                        Units = material.PrimaryUnits,
                        MovementType = AMSOsramConstants.Type261,
                        SubMaterialCount = material.SubMaterialCount,
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
