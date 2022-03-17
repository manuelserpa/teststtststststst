using Cmf.Custom.AMSOsram.BusinessObjects;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Custom.AMSOsram.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Cmf.Custom.AMSOsram.Actions.Materials
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
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");
            UseReference("", "Cmf.Foundation.BusinessObjects.GenericTables");
            // Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            // Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("", "Cmf.Custom.AMSOsram.Common.Extensions");
            UseReference("Cmf.Custom.AMSOsram.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.AMSOsram.BusinessObjects");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.DataStructures");
            // System
            UseReference("%MicrosoftNetPath%System.Data.Common.dll", "System.Data");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
            UseReference("%MicrosoftNetPath%System.ObjectModel.dll", "");

            const int ContainerMaxNumberOfWafers = 25;
            bool canStartProcess = true;
            CustomSorterJobDefinition customSorterJobDefinition = Input["CustomSorterJobDefinition"] as CustomSorterJobDefinition;
            Container currentContainer = Input["Container"] as Container;
            Resource resource = Input["Resource"] as Resource;
            Resource currentLoadPort = Input["LoadPort"] as Resource;
            string temporaryMovementList = string.Empty;
            string futureActionType = string.Empty;
            List<ResourceLoadPortData> dockedContainers = null;

            // Only need to take into account scenarios other than 'Map Carrier'
            if (customSorterJobDefinition.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessTransferWafers ||
                customSorterJobDefinition.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessCompose)
            {
                // Fetch all docked containers on the current resource load ports
                dockedContainers = AMSOsramUtilities.DockedContainersOnLoadPortsByParentResource(resource);

                if (customSorterJobDefinition.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessTransferWafers)
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
                    futureActionType = movementListObject[AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyFutureActionType].Value<string>() ?? string.Empty;

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
                                var otherContainersDocked = dockedContainers.Where(d => !d.ContainerName.Equals(currentContainer.Name, StringComparison.InvariantCultureIgnoreCase) &&
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
                                        foreach (var containerDocked in otherContainersDocked)
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
                                        foreach (MaterialContainer materialInContainer in currentContainer.ContainerMaterials)
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
                        List<string> sourceContainers = movementList.DistinctBy(m => m.Value<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer)).Values<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer).ToList();
                        List<string> destinationContainers = movementList.DistinctBy(m => m.Value<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer)).Values<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer).ToList();

                        // We need to updated movement list with the actual destination containers when
                        //      the future action type is a Split
                        if (futureActionType.Equals(AMSOsramConstants.CustomSorterJobDefinitionJsonPropertySplitFutureActionType, StringComparison.InvariantCultureIgnoreCase))
                        {
                            ContainerCollection destinationContainersThatMatchTheCriteria = new ContainerCollection();
                            List<string> updatedDestinationContainers = new List<string>();
                            int numberOfDestinationContainersNeeded = destinationContainers.Count;
                            var dockedContainersWithoutSourceContainer = dockedContainers.Where(
                                d => !d.ContainerName.Equals(currentContainer.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                d.ContainerUsedPositions == 0 &&
                                !d.LoadPortInUse);                           

                            foreach (var dockedContainer in dockedContainersWithoutSourceContainer.DistinctBy(d => d.ContainerName))
                            {
                                string containerName = dockedContainer.ContainerName;
                                Container container = new Container() { Name = containerName };

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

                                foreach (var movement in movementList)
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
                        else if (futureActionType.Equals(AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyGradingFutureActionType, StringComparison.InvariantCultureIgnoreCase)) // Grading process
                        {
                            // Get the reclaim product
                            string reclaimProduct = movementListObject[AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyReclaimProduct].Value<string>() ?? string.Empty;
                            string reclaimContainerType = string.Empty;
                            List<string> updatedDestinationContainers = new List<string>();

                            #region Custom Reclaim Container type generic table search

                                if (!string.IsNullOrWhiteSpace(reclaimProduct))
                                {
                                    GenericTable table = new GenericTable() { Name = AMSOsramConstants.GenericTableCustomReclaimContainerType };
                                    table.Load();

                                    // filter by Product
                                    Cmf.Foundation.BusinessObjects.QueryObject.FilterCollection filters = new Cmf.Foundation.BusinessObjects.QueryObject.FilterCollection
                                    {
                                        new Cmf.Foundation.BusinessObjects.QueryObject.Filter()
                                        {
                                            Name = AMSOsramConstants.GenericTableCustomReclaimContainerTypeSourceContainerTypeProperty,
                                            Operator = Cmf.Foundation.Common.FieldOperator.IsEqualTo,
                                            Value = currentContainer.Type,
                                            LogicalOperator = Cmf.Foundation.Common.LogicalOperator.Nothing,
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
                                                if (row[AMSOsramConstants.GenericTableCustomReclaimContainerTypeSourceContainerTypeProperty] != DBNull.Value &&
                                                    row[AMSOsramConstants.GenericTableCustomReclaimContainerTypeSourceContainerTypeProperty] != null)
                                                {
                                                reclaimContainerType = row[AMSOsramConstants.GenericTableCustomReclaimContainerTypeReclaimContainerTypeProperty].ToString();
                                                }
                                            }
                                        }
                                    }

                                    if (string.IsNullOrWhiteSpace(reclaimContainerType))
                                    {
                                        throw new CmfBaseException($"It was not possible to resolve a reclaim container type on the table ({AMSOsramConstants.GenericTableCustomReclaimContainerType}) using the source container type ({currentContainer.Type}).");
                                    }
                                }
                            #endregion

                            JArray jArray = new JArray();

                            // Get the different products on the movement list
                            List<string> products = movementList.DistinctBy(m => m.Value<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyProductName)).Values<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyProductName).ToList();

                            List<ResourceLoadPortData> dockedContainersWithoutSourceContainer = dockedContainers.Where(d => !d.ContainerName.Equals(currentContainer.Name, StringComparison.InvariantCultureIgnoreCase) && !d.LoadPortInUse).ToList();
                            List<(string containerName, int usedPositions, string containerType, Queue<int> freePositions, string productAssigned)> containerInfo = new List<(string containerName, int usedPositions, string containerType, Queue<int> freePositions, string productAssigned)>();

                            if (dockedContainersWithoutSourceContainer.Count > 0)
                            {
                                foreach (var containerDocked in dockedContainersWithoutSourceContainer)
                                {
                                    Container container = new Container();
                                    container.Load(containerDocked.ContainerName);
                                    container.LoadRelations("MaterialContainer");
                                    string productAssociatedWithThisContainer = string.Empty;

                                    if (containerDocked.ContainerUsedPositions > 0)
                                    {
                                        var materialsWithDistincProducts = container.ContainerMaterials.Select(c => c.SourceEntity).DistinctBy(d => d.Product.Name);

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
                                    var movementsByProduct = movementList.Where(m => m.Value<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyProductName) == product);
                                    var movements = new Queue<(string MaterialName, string SourceContainer, int SourcePosition)>();

                                    foreach (var movement in movementsByProduct)
                                    {
                                        string materialName = movement.Value<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName);
                                        string sourceContainer = movement.Value<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer);
                                        int sourcePosition = movement.Value<int>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition);
                                        movements.Enqueue((materialName, sourceContainer, sourcePosition));
                                    }
                                    #endregion

                                    string targetContainerTypeCheck = customSorterJobDefinition.TargetCarrierType;

                                    if (product == reclaimProduct)
                                    {
                                        targetContainerTypeCheck = reclaimContainerType;
                                    }

                                    var containersForTargetContainerType = containerInfo.Where(c => c.containerType == targetContainerTypeCheck && !destinationContainersUsed.Contains(c.containerName)).ToList();

                                    if (containersForTargetContainerType.Count > 0)
                                    {
                                        // Check if any of this containers has a product assigned
                                        if (containersForTargetContainerType.Any(c => c.productAssigned.Equals(product, StringComparison.InvariantCultureIgnoreCase)))
                                        {
                                            foreach (var containerWithProductAssociated in containersForTargetContainerType.Where(c => c.productAssigned.Equals(product)))
                                            {
                                                // add this container in a temporary list so it can not be used anymore
                                                destinationContainersUsed.Add(containerWithProductAssociated.containerName);

                                                while (containerWithProductAssociated.freePositions.Count > 0 && movements.Count > 0)
                                                {
                                                    var movement = movements.Dequeue();

                                                    JObject jObject = new JObject
                                                    {
                                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = movement.MaterialName,
                                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = movement.SourceContainer,
                                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = movement.SourcePosition,
                                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = containerWithProductAssociated.containerName,
                                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = containerWithProductAssociated.freePositions.Dequeue()
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
                                            foreach (var emptyTargetContainer in containersForTargetContainerType.Where(c => c.usedPositions == 0))
                                            {
                                                // add this container in a temporary list so it can not be used anymore
                                                destinationContainersUsed.Add(emptyTargetContainer.containerName);

                                                while (emptyTargetContainer.freePositions.Count > 0 && movements.Count > 0)
                                                {
                                                    var movement = movements.Dequeue();

                                                    JObject jObject = new JObject
                                                    {
                                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = movement.MaterialName,
                                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = movement.SourceContainer,
                                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = movement.SourcePosition,
                                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = emptyTargetContainer.containerName,
                                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = emptyTargetContainer.freePositions.Dequeue()
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
                        else if (futureActionType.Equals(AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyScrapFutureActionType, StringComparison.InvariantCultureIgnoreCase)) // Scrap Process
                        {
                            ContainerCollection destinationContainersThatMatchTheCriteria = new ContainerCollection();
                            List<(string containerName, int usedPositions, Queue<int> freePositions)> containerInfo = new List<(string containerName, int usedPositions, Queue<int> freePositions)>();
                            int numberOfDestinationContainersNeeded = destinationContainers.Count;
                            var targetContainers = dockedContainers.Where(
                                d => !d.ContainerName.Equals(currentContainer.Name, StringComparison.InvariantCultureIgnoreCase) &&
                                string.IsNullOrEmpty(d.ContainerLotAttribute) &&
                                string.IsNullOrEmpty(d.ParentMaterialName) &&
                                !d.LoadPortInUse &&
                                d.ContainerType.Equals(customSorterJobDefinition.TargetCarrierType, StringComparison.InvariantCultureIgnoreCase)).OrderByDescending(o => o.ContainerUsedPositions);

                            if (targetContainers.Count() > 0)
                            {
                                foreach (var containerDocked in targetContainers)
                                {
                                    Container container = new Container();
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

                                var movements = new Queue<(string MaterialName, string SourceContainer, int SourcePosition)>();

                                foreach (var movement in movementList)
                                {
                                    string materialName = movement.Value<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName);
                                    string sourceContainer = movement.Value<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer);
                                    int sourcePosition = movement.Value<int>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition);
                                    movements.Enqueue((materialName, sourceContainer, sourcePosition));
                                }

                                List<string> destinationContainersUsed = new List<string>();

                                if (containerInfo.Count > 0)
                                {
                                    foreach (var targetContainerDocked in containerInfo)
                                    {
                                        // add this container in a temporary list so it can not be used anymore
                                        destinationContainersUsed.Add(targetContainerDocked.containerName);

                                        while (targetContainerDocked.freePositions.Count > 0 && movements.Count > 0)
                                        {
                                            var movement = movements.Dequeue();

                                            JObject jObject = new JObject
                                            {
                                                [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = movement.MaterialName,
                                                [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = movement.SourceContainer,
                                                [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = movement.SourcePosition,
                                                [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = targetContainerDocked.containerName,
                                                [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = targetContainerDocked.freePositions.Dequeue()
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
                                futureActionType.Equals(AMSOsramConstants.CustomSorterJobDefinitionJsonPropertySplitFutureActionType, StringComparison.InvariantCultureIgnoreCase))
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
                                        Container destinationContainer = new Container() { Name = containerName };

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
                            else if (futureActionType.Equals(AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyMergeFutureActionType, StringComparison.InvariantCultureIgnoreCase)) // This is a Merge
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
                                        Container sourceContainer = new Container() { Name = containerName };

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
                else if (customSorterJobDefinition.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessCompose && Input["BOM"] is BOM bom)
                {
                    // Check if target carrier type matches the one defined on custom sorter job definition
                    if (currentContainer.Type != customSorterJobDefinition.TargetCarrierType)
                    {
                        throw new CmfBaseException($"Target carrier type ({currentContainer.Type}) is different from the custom sorter job target carrier type ({customSorterJobDefinition.TargetCarrierType}).");
                    }

                    bom.LoadRelations("BOMProduct");

                    Dictionary<string, List<string>> subs = new Dictionary<string, List<string>>();
                    Dictionary<string, int> parentBomProd = new Dictionary<string, int>();

                    foreach (BOMProduct prod in bom.BomProducts.OrderBy(b => b.Order))
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

                    for (int i = 1; i <= ContainerMaxNumberOfWafers; i++)
                    {
                        if (currentContainer.ContainerMaterials == null || !currentContainer.ContainerMaterials.Any(m => m.Position == i))
                        {
                            freePositions.Enqueue(i);
                        }
                    }

                    List<Container> containers = AMSOsramUtilities.GetContainersDockedOnResourceLoadPortsReadyForProcess(resource);
                    JArray jArray = new JArray();
                    MaterialContainerCollection materialsInContainers = new MaterialContainerCollection();

                    foreach (Container container in containers.Where(c => c.Name != currentContainer.Name && c.Type == customSorterJobDefinition.SourceCarrierType))
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
                                [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyProductName] = wafer?.SourceEntity?.Product?.Name ?? "",
                                [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = wafer.TargetEntity.Name,
                                [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = wafer?.SourceEntity?.Name ?? "",
                                [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = currentContainer.Name,
                                [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = wafer.Position,
                                [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = freePositions.Dequeue()
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
                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySubstitute] = wafer?.SourceEntity?.Product?.Name ?? "",
                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = wafer.TargetEntity.Name,
                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = wafer?.SourceEntity?.Name ?? "",
                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = currentContainer.Name,
                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = wafer.Position,
                                        [AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = freePositions.Dequeue()
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
