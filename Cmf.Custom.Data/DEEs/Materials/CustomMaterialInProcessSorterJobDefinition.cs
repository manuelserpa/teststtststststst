using Cmf.Custom.amsOSRAM.BusinessObjects;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.Materials
{
    class CustomMaterialInProcessSorterJobDefinition : DeeDevBase
    {
        /// <summary>
        /// DEE Test Condition.
        /// </summary>
        /// <param name="Input">The Input.</param>
        /// <returns></returns>
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---
            #region Info

            /* Description:
             *     DEE Action responsible to check if it's posible to start the execution of the given sorter job definition
             *     Checks 
             *
             * Action Groups:
             *     N/A
            */

            #endregion
            return true;
            //---End DEE Condition Code---
        }

        /// <summary>
        /// DEE Action Code.
        /// </summary>
        /// <param name="Input">The Input.</param>
        /// <returns></returns>
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            // Foundation
            UseReference("", "Cmf.Foundation.BusinessObjects.GenericTables");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("", "Cmf.Custom.amsOSRAM.Common.Extensions");
            UseReference("Cmf.Custom.amsOSRAM.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.amsOSRAM.BusinessObjects");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.DataStructures");
            
            // System
            UseReference("%MicrosoftNetPath%System.Data.Common.dll", "System.Data");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
            UseReference("%MicrosoftNetPath%System.ObjectModel.dll", "");


            const int ContainerMaxNumberOfWafers = 25;
            bool canStartProcess = true;
            CustomSorterJobDefinition customSorterJobDefinition = Input["CustomSorterJobDefinition"] as CustomSorterJobDefinition;
            IContainer currentContainer = Input["Container"] as IContainer;
            IResource resource = Input["Resource"] as IResource;
            IResource currentLoadPort = Input["LoadPort"] as IResource;
            string temporaryMovementList = string.Empty;
            string futureActionType = string.Empty;
            List<ResourceLoadPortData> dockedContainers = null;

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            // Only need to take into account scenarios other than 'Map Carrier'
            if (customSorterJobDefinition.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessTransferWafers ||
                customSorterJobDefinition.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessCompose)
            {
                // Fetch all docked containers on the current resource load ports
                dockedContainers = amsOSRAMUtilities.DockedContainersOnLoadPortsByParentResource(resource);

                if (customSorterJobDefinition.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessTransferWafers)
                {
                    // Parse Custom Sorter Job Movement List Json Object
                    JObject movementListObject;

                    if (JObject.Parse(customSorterJobDefinition.MovementList) is JObject parsedMovementListObj)
                    {
                        movementListObject = parsedMovementListObj;
                    }
                    else
                    {
                        throw new CmfBaseException($"Not possible to parse the movement list JSON Object for custom sorter job definition ({customSorterJobDefinition.Name}).");
                    }

                    // Parse movement list from the object above
                    JArray movementList;

                    if (movementListObject["Moves"] is JArray movementListJArray)
                    {
                        movementList = movementListJArray;
                    }
                    else
                    {
                        throw new CmfBaseException($"Not possible to parse the moves from the movement list JSON object for custom sorter job definition ({customSorterJobDefinition.Name}).");
                    }

                    // Get the future action type defined on the custom sorter job definition
                    futureActionType = movementListObject[amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyFutureActionType].Value<string>() ?? string.Empty;

                    // Number of moves in this custom sorter job definition
                    int numberOfMoves = movementList.Count;

                    // Check movement list number of moves
                    if (numberOfMoves > ContainerMaxNumberOfWafers)
                    {
                        throw new CmfBaseException($"The number of movements ({numberOfMoves}) exceeds a carrier limits.");
                    }

                    // Check we have moves to do
                    if (numberOfMoves == 0)
                    {
                        // Full transfer from the current container to a target container defined by its target carrier type.
                        if (string.IsNullOrWhiteSpace(futureActionType))
                        {
                            // Check if current container matched the source carrier type defined on the custom sorter job definition
                            if (currentContainer.Type != customSorterJobDefinition.SourceCarrierType)
                            {
                                // Source carrier type is different from the custom sorter job definition source carrier type
                                canStartProcess = false;
                            }
                            else
                            {
                                IEnumerable<ResourceLoadPortData> otherContainersDocked = dockedContainers.Where(d => !d.ContainerName.Equals(currentContainer.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                                            d.ContainerUsedPositions == 0 &&
                                                            !d.LoadPortInUse);

                                // No target containers available to transfer wafers
                                if (otherContainersDocked.Count() > 0)
                                {
                                    JArray fullTransferMovementList = new JArray();
                                    currentContainer.LoadRelations("MaterialContainer");
                                    string destinationContainer = string.Empty;

                                    if (currentContainer.ContainerMaterials != null && currentContainer.ContainerMaterials.Count > 0)
                                    {
                                        foreach (ResourceLoadPortData containerDocked in otherContainersDocked)
                                        {
                                            if (containerDocked.ContainerType == customSorterJobDefinition.TargetCarrierType)
                                            {
                                                destinationContainer = containerDocked.ContainerName;
                                                canStartProcess = true;
                                                break;
                                            }

                                            canStartProcess = false;
                                        }
                                    }
                                    else
                                    {
                                        // The source container does not have wafers to transfer.
                                        canStartProcess = false;
                                    }

                                    if (canStartProcess && !string.IsNullOrWhiteSpace(destinationContainer))
                                    {
                                        foreach (IMaterialContainer materialInContainer in currentContainer.ContainerMaterials)
                                        {
                                            JObject jObject = new JObject
                                            {
                                                ["MaterialName"] = materialInContainer.SourceEntity.Name,
                                                ["SourceContainer"] = currentContainer.Name,
                                                ["SourcePosition"] = materialInContainer.Position,
                                                ["DestinationContainer"] = destinationContainer,
                                                ["DestinationPosition"] = materialInContainer.Position
                                            };

                                            fullTransferMovementList.Add(jObject);
                                        }

                                        temporaryMovementList = fullTransferMovementList.ToString();
                                    }
                                }
                                else
                                {
                                    canStartProcess = false;
                                }
                            }
                        }
                        else
                        {
                            // The custom sorter job definition movement list is empty.
                            canStartProcess = false;
                        }
                    }
                    else
                    {
                        // Fetch the source containers and the destination containers
                        List<string> sourceContainers = movementList.DistinctBy(m => m.Value<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer)).Values<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer).ToList();
                        List<string> destinationContainers = movementList.DistinctBy(m => m.Value<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer)).Values<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer).ToList();

                        // We need to updated movement list with the actual destination containers when
                        //      the future action type is a Split
                        if (futureActionType.Equals(amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertySplitFutureActionType, StringComparison.InvariantCultureIgnoreCase))
                        {
                            IContainerCollection destinationContainersThatMatchTheCriteria = entityFactory.CreateCollection<IContainerCollection>();
                            List<string> updatedDestinationContainers = new List<string>();
                            int numberOfDestinationContainersNeeded = destinationContainers.Count;
                            IEnumerable<ResourceLoadPortData> dockedContainersWithoutSourceContainer = dockedContainers.Where(
                                d => !d.ContainerName.Equals(currentContainer.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                d.ContainerUsedPositions == 0 &&
                                !d.LoadPortInUse);                           

                            foreach (ResourceLoadPortData dockedContainer in dockedContainersWithoutSourceContainer.DistinctBy(d => d.ContainerName))
                            {
                                string containerName = dockedContainer.ContainerName;
                                IContainer container = entityFactory.Create<IContainer>();
                                container.Name = containerName;

                                if (!string.IsNullOrWhiteSpace(containerName) && container.ObjectExists())
                                {
                                    container.Load();
                                    if (container.Type.Equals(customSorterJobDefinition.TargetCarrierType, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        destinationContainersThatMatchTheCriteria.Add(container);
                                        updatedDestinationContainers.Add(containerName);
                                    }
                                }

                                if (numberOfDestinationContainersNeeded == destinationContainersThatMatchTheCriteria.Count)
                                {
                                    // We have met the target
                                    break;
                                }
                            }

                            if (numberOfDestinationContainersNeeded == destinationContainersThatMatchTheCriteria.Count)
                            {
                                JArray updatedMovementList = new JArray();

                                foreach (JToken movement in movementList)
                                {
                                    string destContainer = movement.Value<string>("DestinationContainer");
                                    string sourceContainer = movement.Value<string>("SourceContainer");
                                    string materialName = movement.Value<string>("MaterialName");
                                    int sourcePosition = movement.Value<int>("SourcePosition");
                                    int destinationPosition = movement.Value<int>("DestinationPosition");

                                    int indexOfContainer = destinationContainers.IndexOf(destContainer);

                                    JObject jObject = new JObject
                                    {
                                        ["MaterialName"] = materialName,
                                        ["SourceContainer"] = sourceContainer,
                                        ["SourcePosition"] = sourcePosition,
                                        ["DestinationContainer"] = updatedDestinationContainers[indexOfContainer],
                                        ["DestinationPosition"] = destinationPosition
                                    };

                                    updatedMovementList.Add(jObject);
                                }

                                destinationContainers = updatedDestinationContainers;
                                movementList = updatedMovementList;
                            }
                            else
                            {
                                canStartProcess = false;
                            }
                        }
                        else if (futureActionType.Equals(amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyGradingFutureActionType, StringComparison.InvariantCultureIgnoreCase)) // Grading process
                        {
                            // Get the reclaim product
                            string reclaimProduct = movementListObject[amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyReclaimProduct].Value<string>() ?? string.Empty;
                            string reclaimContainerType = string.Empty;
                            List<string> updatedDestinationContainers = new List<string>();

                            #region Custom Reclaim Container type generic table search

                                if (!string.IsNullOrWhiteSpace(reclaimProduct))
                                {
                                    IGenericTable table = new GenericTable() { Name = amsOSRAMConstants.GenericTableCustomReclaimContainerType };
                                    table.Load();

                                // filter by Product
                                IFilterCollection filters = new Foundation.BusinessObjects.QueryObject.FilterCollection
                                    {
                                        new Foundation.BusinessObjects.QueryObject.Filter()
                                        {
                                            Name = amsOSRAMConstants.GenericTableCustomReclaimContainerTypeSourceContainerTypeProperty,
                                            Operator = FieldOperator.IsEqualTo,
                                            Value = currentContainer.Type,
                                            LogicalOperator = LogicalOperator.Nothing,
                                        }
                                    };

                                    table.LoadData(filters);

                                    if (table.HasData)
                                    {
                                        DataSet productContainerTypeDataSet = NgpDataSet.ToDataSet(table.Data);
                                        // check if results are available
                                        if (productContainerTypeDataSet.Tables != null && productContainerTypeDataSet.Tables.Count > 0 &&
                                            productContainerTypeDataSet.Tables[0].Rows != null && productContainerTypeDataSet.Tables[0].Rows.Count > 0)
                                        {
                                            foreach (DataRow row in productContainerTypeDataSet.Tables[0].Rows)
                                            {
                                                if (row[amsOSRAMConstants.GenericTableCustomReclaimContainerTypeSourceContainerTypeProperty] != DBNull.Value &&
                                                    row[amsOSRAMConstants.GenericTableCustomReclaimContainerTypeSourceContainerTypeProperty] != null)
                                                {
                                                reclaimContainerType = row[amsOSRAMConstants.GenericTableCustomReclaimContainerTypeReclaimContainerTypeProperty].ToString();
                                                }
                                            }
                                        }
                                    }

                                    if (string.IsNullOrWhiteSpace(reclaimContainerType))
                                    {
                                        throw new CmfBaseException($"It was not possible to resolve a reclaim container type on the table ({amsOSRAMConstants.GenericTableCustomReclaimContainerType}) using the source container type ({currentContainer.Type}).");
                                    }
                                }
                            #endregion

                            JArray jArray = new JArray();

                            // Get the different products on the movement list
                            List<string> products = movementList.DistinctBy(m => m.Value<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyProductName)).Values<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyProductName).ToList();

                            List<ResourceLoadPortData> dockedContainersWithoutSourceContainer = dockedContainers.Where(d => !d.ContainerName.Equals(currentContainer.Name, StringComparison.InvariantCultureIgnoreCase) && !d.LoadPortInUse).ToList();
                            List<(string containerName, int usedPositions, string containerType, Queue<int> freePositions, string productAssigned)> containerInfo = new List<(string containerName, int usedPositions, string containerType, Queue<int> freePositions, string productAssigned)>();

                            if (dockedContainersWithoutSourceContainer.Count > 0)
                            {
                                foreach (ResourceLoadPortData containerDocked in dockedContainersWithoutSourceContainer)
                                {
                                    IContainer container = entityFactory.Create<IContainer>();
                                    container.Load(containerDocked.ContainerName);
                                    container.LoadRelations("MaterialContainer");
                                    string productAssociatedWithThisContainer = string.Empty;

                                    if (containerDocked.ContainerUsedPositions > 0)
                                    {
                                        IEnumerable<IMaterial> materialsWithDistincProducts = container.ContainerMaterials.Select(c => c.SourceEntity).DistinctBy(d => d.Product.Name);

                                        if (materialsWithDistincProducts.Count() == 1)
                                        {
                                            productAssociatedWithThisContainer = materialsWithDistincProducts.First().Product.Name;
                                        }
                                        else
                                        {
                                            continue;   // Skip because we can only use empty container or container with only one product assigned
                                        }
                                    }

                                    // fill queue free positions
                                    Queue<int> freePositions = new Queue<int>();

                                    for (int i = 1; i <= containerDocked.ContainerTotalPositions; i++)
                                    {
                                        if (container.ContainerMaterials == null || !container.ContainerMaterials.Any(m => m.Position == i))
                                        {
                                            freePositions.Enqueue(i);
                                        }
                                    }

                                    containerInfo.Add((container.Name, containerDocked.ContainerUsedPositions, container.Type, freePositions, productAssociatedWithThisContainer));
                                }

                                List<string> destinationContainersUsed = new List<string>();

                                foreach (string product in products)
                                {
                                    #region Movements needed by product
                                    IEnumerable<JToken> movementsByProduct = movementList.Where(m => m.Value<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyProductName) == product);
                                    Queue<(string MaterialName, string SourceContainer, int SourcePosition)> movements = new Queue<(string MaterialName, string SourceContainer, int SourcePosition)>();

                                    foreach (JToken movement in movementsByProduct)
                                    {
                                        string materialName = movement.Value<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName);
                                        string sourceContainer = movement.Value<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer);
                                        int sourcePosition = movement.Value<int>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition);
                                        movements.Enqueue((materialName, sourceContainer, sourcePosition));
                                    }
                                    #endregion

                                    string targetContainerTypeCheck = customSorterJobDefinition.TargetCarrierType;

                                    if (product == reclaimProduct)
                                    {
                                        targetContainerTypeCheck = reclaimContainerType;
                                    }

                                    List<(string containerName, int usedPositions, string containerType, Queue<int> freePositions, string productAssigned)> containersForTargetContainerType = containerInfo.Where(c => c.containerType == targetContainerTypeCheck && !destinationContainersUsed.Contains(c.containerName)).ToList();

                                    if (containersForTargetContainerType.Count > 0)
                                    {
                                        // Check if any of this containers has a product assigned
                                        if (containersForTargetContainerType.Any(c => c.productAssigned.Equals(product, StringComparison.InvariantCultureIgnoreCase)))
                                        {
                                            foreach ((string containerName, int usedPositions, string containerType, Queue<int> freePositions, string productAssigned) containerWithProductAssociated in containersForTargetContainerType.Where(c => c.productAssigned.Equals(product)))
                                            {
                                                // add this container in a temporary list so it can not be used anymore
                                                destinationContainersUsed.Add(containerWithProductAssociated.containerName);

                                                while (containerWithProductAssociated.freePositions.Count > 0 && movements.Count > 0)
                                                {
                                                    (string MaterialName, string SourceContainer, int SourcePosition) movement = movements.Dequeue();

                                                    JObject jObject = new JObject
                                                    {
                                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = movement.MaterialName,
                                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = movement.SourceContainer,
                                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = movement.SourcePosition,
                                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = containerWithProductAssociated.containerName,
                                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = containerWithProductAssociated.freePositions.Dequeue()
                                                    };

                                                    jArray.Add(jObject);
                                                }

                                                if (movements.Count == 0)
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        // There are stil movements available, lets use empty containers
                                        if (movements.Count > 0)
                                        {
                                            foreach ((string containerName, int usedPositions, string containerType, Queue<int> freePositions, string productAssigned) emptyTargetContainer in containersForTargetContainerType.Where(c => c.usedPositions == 0))
                                            {
                                                // add this container in a temporary list so it can not be used anymore
                                                destinationContainersUsed.Add(emptyTargetContainer.containerName);

                                                while (emptyTargetContainer.freePositions.Count > 0 && movements.Count > 0)
                                                {
                                                    (string MaterialName, string SourceContainer, int SourcePosition) movement = movements.Dequeue();

                                                    JObject jObject = new JObject
                                                    {
                                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = movement.MaterialName,
                                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = movement.SourceContainer,
                                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = movement.SourcePosition,
                                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = emptyTargetContainer.containerName,
                                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = emptyTargetContainer.freePositions.Dequeue()
                                                    };

                                                    jArray.Add(jObject);
                                                }

                                                if (movements.Count == 0)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        canStartProcess = false;
                                        break;
                                    }

                                    if (movements.Count > 0)
                                    {
                                        canStartProcess = false;
                                        break;
                                    }
                                }

                                if (canStartProcess)
                                {
                                    temporaryMovementList = jArray.ToString();
                                }
                            }
                            else
                            {
                                canStartProcess = false;
                            }
                        }
                        else if (futureActionType.Equals(amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyScrapFutureActionType, StringComparison.InvariantCultureIgnoreCase)) // Scrap Process
                        {
                            IContainerCollection destinationContainersThatMatchTheCriteria = entityFactory.CreateCollection<IContainerCollection>();
                            List<(string containerName, int usedPositions, Queue<int> freePositions)> containerInfo = new List<(string containerName, int usedPositions, Queue<int> freePositions)>();
                            int numberOfDestinationContainersNeeded = destinationContainers.Count;
                            IOrderedEnumerable<ResourceLoadPortData> targetContainers = dockedContainers.Where(
                                d => !d.ContainerName.Equals(currentContainer.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                string.IsNullOrEmpty(d.ContainerLotAttribute) &&
                                string.IsNullOrEmpty(d.ParentMaterialName) &&
                                !d.LoadPortInUse &&
                                d.ContainerType.Equals(customSorterJobDefinition.TargetCarrierType, StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(o => o.ContainerUsedPositions);

                            if (targetContainers.Count() > 0)
                            {
                                foreach (ResourceLoadPortData containerDocked in targetContainers)
                                {
                                    IContainer container = entityFactory.Create<IContainer>();
                                    container.Load(containerDocked.ContainerName);
                                    container.LoadRelations("MaterialContainer");

                                    // fill queue free positions
                                    Queue<int> freePositions = new Queue<int>();

                                    for (int i = 1; i <= containerDocked.ContainerTotalPositions; i++)
                                    {
                                        if (container.ContainerMaterials == null || !container.ContainerMaterials.Any(m => m.Position == i))
                                        {
                                            freePositions.Enqueue(i);
                                        }
                                    }

                                    containerInfo.Add((container.Name, containerDocked.ContainerUsedPositions, freePositions));
                                }

                                JArray updatedMovementList = new JArray();

                                Queue<(string MaterialName, string SourceContainer, int SourcePosition)> movements = new Queue<(string MaterialName, string SourceContainer, int SourcePosition)>();

                                foreach (JToken movement in movementList)
                                {
                                    string materialName = movement.Value<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName);
                                    string sourceContainer = movement.Value<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer);
                                    int sourcePosition = movement.Value<int>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition);
                                    movements.Enqueue((materialName, sourceContainer, sourcePosition));
                                }

                                List<string> destinationContainersUsed = new List<string>();

                                if (containerInfo.Count > 0)
                                {
                                    foreach ((string containerName, int usedPositions, Queue<int> freePositions) targetContainerDocked in containerInfo)
                                    {
                                        // add this container in a temporary list so it can not be used anymore
                                        destinationContainersUsed.Add(targetContainerDocked.containerName);

                                        while (targetContainerDocked.freePositions.Count > 0 && movements.Count > 0)
                                        {
                                            (string MaterialName, string SourceContainer, int SourcePosition) movement = movements.Dequeue();

                                            JObject jObject = new JObject
                                            {
                                                [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = movement.MaterialName,
                                                [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = movement.SourceContainer,
                                                [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = movement.SourcePosition,
                                                [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = targetContainerDocked.containerName,
                                                [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = targetContainerDocked.freePositions.Dequeue()
                                            };

                                            updatedMovementList.Add(jObject);
                                        }

                                        if (movements.Count == 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    canStartProcess = false;
                                }

                                if (movements.Count > 0)
                                {
                                    canStartProcess = false;
                                }


                                if (canStartProcess)
                                {
                                    temporaryMovementList = updatedMovementList.ToString();
                                }
                            }
                            else
                            {
                                canStartProcess = false;
                            }
                        }
                        else
                        {
                            // Join all container names
                            List<string> allContainersNeeded = new List<string>();
                            allContainersNeeded.AddRange(sourceContainers);
                            allContainersNeeded.AddRange(destinationContainers);

                            // Check all needed containers are docked
                            // If not we cannot start the process
                            foreach (string neededContainerName in allContainersNeeded)
                            {
                                if (!dockedContainers.Any(a => a.ContainerName == neededContainerName))
                                {
                                    canStartProcess = false;
                                    break;
                                }
                            }
                        }

                        if (canStartProcess)
                        {
                            // Use case for a simple transfer or a Split
                            if ((string.IsNullOrEmpty(futureActionType) && numberOfMoves > 0) ||
                                futureActionType.Equals(amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertySplitFutureActionType, StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (currentContainer.Type != customSorterJobDefinition.SourceCarrierType)
                                {
                                    throw new CmfBaseException($"Source carrier type ({currentContainer.Type}) is different from the custom sorter job definition source carrier type ({customSorterJobDefinition.SourceCarrierType}).");
                                }

                                if (sourceContainers.Count > 1)
                                {
                                    throw new CmfBaseException($"For this operation type, only one source carrier is allowed. Current number of source carriers = {sourceContainers.Count}");
                                }

                                string sourceContainerInSorterJobDefinition = sourceContainers.FirstOrDefault();

                                // The source container must be the same as the one on the current container
                                if (sourceContainerInSorterJobDefinition != null &&
                                    sourceContainerInSorterJobDefinition.Equals(currentContainer.Name, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    int sourceContainerUsedPositions = currentContainer?.UsedPositions ?? 0;

                                    // The number of wafers needed to be transferred must lower or equal than the used positions on the source container
                                    if (sourceContainerUsedPositions < numberOfMoves)
                                    {
                                        throw new CmfBaseException($"There isn't enough wafers on source carrier ({sourceContainerUsedPositions}) to be transferred ({numberOfMoves}).");
                                    }

                                    foreach (string containerName in destinationContainers)
                                    {
                                        IContainer destinationContainer = entityFactory.Create<IContainer>();
                                        destinationContainer.Name = containerName;

                                        if (!string.IsNullOrWhiteSpace(containerName) && destinationContainer.ObjectExists())
                                        {
                                            // Check if for the carrier docked, the load port is in use
                                            if (dockedContainers.Any(d => d.ContainerName == containerName && d.LoadPortInUse))
                                            {
                                                throw new CmfBaseException($"The load port ({dockedContainers.FirstOrDefault(c => c.ContainerName.Equals(containerName, StringComparison.InvariantCultureIgnoreCase)).LoadPortName}) where carrier ({containerName}) is docked, is currently in use.");
                                            }

                                            destinationContainer.Load();

                                            if (destinationContainer.Type != customSorterJobDefinition.TargetCarrierType)
                                            {
                                                throw new CmfBaseException($"Target carrier type ({destinationContainer.Type}) is different from the custom sorter job target carrier type ({customSorterJobDefinition.TargetCarrierType}).");
                                            }

                                            int destinationContainerUsedPositions = destinationContainer?.UsedPositions ?? 0;
                                            int numberOfWafersToTransfer = movementList.Where(m => m.Value<string>("DestinationContainer") == containerName).Count();

                                            if (destinationContainerUsedPositions != 0 && (destinationContainerUsedPositions + numberOfWafersToTransfer > ContainerMaxNumberOfWafers))
                                            {
                                                throw new CmfBaseException($"Destination container wafer count ({destinationContainerUsedPositions}) plus the number of wafers to be transfered ({numberOfWafersToTransfer}) exceeds the limit of the carrier.");
                                            }
                                        }
                                        else
                                        {
                                            throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Container, containerName);
                                        }
                                    }

                                    // Update the temporary movement list
                                    temporaryMovementList = movementList.ToString();
                                }
                                else
                                {
                                    canStartProcess = false;
                                }
                            }
                            else if (futureActionType.Equals(amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyMergeFutureActionType, StringComparison.InvariantCultureIgnoreCase)) // This is a Merge
                            {
                                if (currentContainer.Type != customSorterJobDefinition.TargetCarrierType)
                                {
                                    throw new CmfBaseException($"Target carrier type ({currentContainer.Type}) is different from the custom sorter job definition target carrier type ({customSorterJobDefinition.TargetCarrierType}).");
                                }

                                if (destinationContainers.Count > 1)
                                {
                                    throw new CmfBaseException($"For this operation type, only one target carrier is allowed. Current number of target carriers = {destinationContainers.Count}");
                                }

                                string targetContainerInSorterJobDefinition = destinationContainers.FirstOrDefault();

                                // The destination container must be the same as the one on the current container
                                if (targetContainerInSorterJobDefinition.Equals(currentContainer.Name, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    int destinationContainerUsedPositions = currentContainer?.UsedPositions ?? 0;

                                    // The number of wafers needed to be transferred plus the number of wafers on the destination container must be lower or equal than 25
                                    //      which is the limit of the carrier
                                    if (destinationContainerUsedPositions + numberOfMoves > ContainerMaxNumberOfWafers)
                                    {
                                        throw new CmfBaseException($"The number of wafers to be transferred ({numberOfMoves}) exceeds the capacity of the destination container, currently with {destinationContainerUsedPositions} wafers.");
                                    }

                                    foreach (string containerName in sourceContainers)
                                    {
                                        IContainer sourceContainer = entityFactory.Create<IContainer>();
                                        sourceContainer.Name = containerName;

                                        if (!string.IsNullOrWhiteSpace(containerName) && sourceContainer.ObjectExists())
                                        {
                                            // Check if for the carrier docked, the load port is in use
                                            if (dockedContainers.Any(d => d.ContainerName == containerName && d.LoadPortInUse))
                                            {
                                                throw new CmfBaseException($"The load port ({dockedContainers.FirstOrDefault(c => c.ContainerName.Equals(containerName, StringComparison.InvariantCultureIgnoreCase)).LoadPortName}) where carrier ({containerName}) is docked, is currently in use.");
                                            }

                                            sourceContainer.Load();

                                            if (sourceContainer.Type != customSorterJobDefinition.SourceCarrierType)
                                            {
                                                throw new CmfBaseException($"Source carrier type ({sourceContainer.Type}) is different from the custom sorter job source carrier type ({customSorterJobDefinition.SourceCarrierType}).");
                                            }

                                            int sourceContainerUsedPositions = sourceContainer?.UsedPositions ?? 0;
                                            int numberOfWafersToTransfer = movementList.Where(m => m.Value<string>("SourceContainer") == containerName).Count();

                                            if (sourceContainerUsedPositions != 0)
                                            {
                                                if (sourceContainerUsedPositions < numberOfWafersToTransfer)
                                                {
                                                    throw new CmfBaseException($"Source container wafer count ({sourceContainerUsedPositions}) is lower than the quantity needed to be transfered ({numberOfWafersToTransfer}).");
                                                }
                                            }
                                            else // Source container used position = 0. Container is empty
                                            {
                                                throw new CmfBaseException($"Source container ({containerName}) is currently empty and the number of wafers to transfer is {numberOfWafersToTransfer}.");
                                            }
                                        }
                                        else
                                        {
                                            throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Container, containerName);
                                        }
                                    }

                                    // Update the temporary movement list
                                    temporaryMovementList = movementList.ToString();
                                }
                                else
                                {
                                    canStartProcess = false;
                                }
                            }
                        }
                    }
                }
                else if (customSorterJobDefinition.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessCompose && Input["BOM"] is IBOM bom)
                {
                    // Check if target carrier type matches the one defined on custom sorter job definition
                    if (currentContainer.Type != customSorterJobDefinition.TargetCarrierType)
                    {
                        throw new CmfBaseException($"Target carrier type ({currentContainer.Type}) is different from the custom sorter job target carrier type ({customSorterJobDefinition.TargetCarrierType}).");
                    }

                    bom.LoadRelations("BOMProduct");

                    Dictionary<string, List<string>> subs = new Dictionary<string, List<string>>();
                    Dictionary<string, int> parentBomProd = new Dictionary<string, int>();

                    foreach (IBOMProduct prod in bom.BomProducts.OrderBy(b => b.Order))
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

                    for (int i = 1; i <= ContainerMaxNumberOfWafers; i++)
                    {
                        if (currentContainer.ContainerMaterials == null || !currentContainer.ContainerMaterials.Any(m => m.Position == i))
                        {
                            freePositions.Enqueue(i);
                        }
                    }

                    List<IContainer> containers = amsOSRAMUtilities.GetContainersDockedOnResourceLoadPortsReadyForProcess(resource);
                    JArray jArray = new JArray();
                    IMaterialContainerCollection materialsInContainers = entityFactory.CreateCollection<IMaterialContainerCollection>();

                    foreach (IContainer container in containers.Where(c => c.Name != currentContainer.Name && c.Type == customSorterJobDefinition.SourceCarrierType))
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
                                [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyProductName] = wafer?.SourceEntity?.Product?.Name ?? "",
                                [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = wafer.TargetEntity.Name,
                                [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = wafer?.SourceEntity?.Name ?? "",
                                [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = currentContainer.Name,
                                [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = wafer.Position,
                                [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = freePositions.Dequeue()
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
                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySubstitute] = wafer?.SourceEntity?.Product?.Name ?? "",
                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = wafer.TargetEntity.Name,
                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = wafer?.SourceEntity?.Name ?? "",
                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = currentContainer.Name,
                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = wafer.Position,
                                        [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = freePositions.Dequeue()
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
                        temporaryMovementList = jArray.ToString();
                    }
                    else
                    {
                        canStartProcess = false;
                    }
                }
                else
                {
                    canStartProcess = false;
                }
            }

            if (canStartProcess)
            {
                customSorterJobDefinition.MovementList = temporaryMovementList;
            }

            Input.Add("CanStartProcess", canStartProcess.ToString());
            Input.Add("FutureActionType", futureActionType);
            Input.Add("ContainersDocked", dockedContainers);

            //---End DEE Code---

            return Input;
        }
    }
}
