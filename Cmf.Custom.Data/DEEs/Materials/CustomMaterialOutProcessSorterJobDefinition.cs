using Cmf.Custom.AMSOsram.BusinessObjects;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cmf.Custom.AMSOsram.Actions.Materials
{
	class CustomMaterialOutProcessSorterJobDefinition : DeeDevBase
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
             *     DEE Action responsible to handle custom sorter job definition information.   
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
            // Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            // Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.AMSOsram.BusinessObjects");
            // System
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
            UseReference("%MicrosoftNetPath%System.ObjectModel.dll", "");


            CustomSorterJobDefinition customSorterJobDefinitionFromEquipment = Input["CustomSorterJobDefinition"] as CustomSorterJobDefinition;
            Resource resource = Input["Resource"] as Resource;
            Material lot = Input["Lot"] as Material;
            bool deleteOnCompletion = false;

            CustomSorterJobDefinition customSorterJobDefinitionFromMES = new CustomSorterJobDefinition() { Name = customSorterJobDefinitionFromEquipment.Name };

            if (customSorterJobDefinitionFromMES.ObjectExists())
            {
                customSorterJobDefinitionFromMES.Load(customSorterJobDefinitionFromEquipment.Name);
            }
            else
            {
                throw new CmfBaseException($"Not possible to retrieve from MES the custom sorter job definition ({customSorterJobDefinitionFromEquipment.Name}).");
            }

            // Parse Custom Sorter Job Movement List Json Object
            JObject movementListObject;

            if (JObject.Parse(customSorterJobDefinitionFromMES.MovementList) is JObject parsedMovementListObj)
            {
                movementListObject = parsedMovementListObj;
            }
            else
            {
                throw new CmfBaseException($"Not possible to parse the movement list JSON Object for the given custom sorter job definition ({customSorterJobDefinitionFromMES.Name}).");
            }

            // Get the future action type defined on the custom sorter job definition
            string futureActionType = movementListObject[AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyFutureActionType].Value<string>() ?? string.Empty;

            if(movementListObject.ContainsKey(AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyDeleteOnCompletion))
			{
                deleteOnCompletion = movementListObject[AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyDeleteOnCompletion].Value<bool>();
            }

            JArray movementList = JArray.Parse(customSorterJobDefinitionFromEquipment.MovementList);

            // Check number of source and destination containers
            int numberOfSourceContainers = movementList.DistinctBy(m => m.Value<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer)).Count();
            int numberOfDestinationContainers = movementList.DistinctBy(m => m.Value<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer)).Count();

            bool isMergeOrCompose = futureActionType.Equals(AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyMergeFutureActionType, StringComparison.InvariantCultureIgnoreCase) || 
                customSorterJobDefinitionFromEquipment.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessCompose;

            // Dictionary<CONTAINER_NAME, <MATERIAL_NAME, POSITION>
            Dictionary<string, List<Tuple<string, int>>> movementListMap = new Dictionary<string, List<Tuple<string, int>>>();

            foreach (JToken movement in movementList)
            {
                // Source
                string materialName = movement.Value<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName);
                string sourceContainer = movement.Value<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer);
                int sourcePosition = movement.Value<int>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition);
                // Destination
                string destinationContainer = movement.Value<string>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer);
                int destinationPosition = movement.Value<int>(AMSOsramConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition);

                // Movement list to destination container
                if (movementListMap.ContainsKey(destinationContainer))
                {
                    movementListMap[destinationContainer].Add(Tuple.Create(materialName, destinationPosition));
                }
                else
                {
                    movementListMap[destinationContainer] = new List<Tuple<string, int>>
                    {
                        Tuple.Create(materialName, destinationPosition)
                    };
                }
            }

            Dictionary<Container, MaterialContainerCollection> materialsOnEachContainer = new Dictionary<Container, MaterialContainerCollection>();

            foreach (var movement in movementListMap)
            {
                string containerName = movement.Key;
                Container container = new Container();
                container.Load(containerName);
                container.LoadRelations("MaterialContainer");

                foreach (var materialMovement in movement.Value)
                {
                    string materialName = materialMovement.Item1;
                    int positionOnContainer = materialMovement.Item2;

                    MaterialContainer materialContainer = container?.ContainerMaterials?.FirstOrDefault(f => f.Position == positionOnContainer) ?? null;

                    if (materialContainer != null && materialContainer.SourceEntity.Name == materialName)
                    {
                        if (materialsOnEachContainer.ContainsKey(container))
                        {
                            materialsOnEachContainer[container].Add(materialContainer);
                        }
                        else
                        {
                            materialsOnEachContainer.Add(container, new MaterialContainerCollection() { materialContainer });
                        }
                    }
                }
            }

            if (isMergeOrCompose) // MERGE OR COMPOSE
            {
                foreach (var item in materialsOnEachContainer)
                {
                    List<Material> materials = item.Value.Select(s => s.SourceEntity).ToList();
                    Dictionary<string, MaterialCollection> topMostMaterialSubmaterials = new Dictionary<string, MaterialCollection>();
                    MaterialCollection materialsToAttach = new MaterialCollection();

                    foreach (Material material in materials)
                    {
                        material.Load();

                        if (customSorterJobDefinitionFromEquipment.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessCompose)
                        {
                            materialsToAttach.Add(material);
                        }
                        else // This should be a Merge
                        {
                            string topMostMaterialName = material.ParentMaterial.Name;
                            if (topMostMaterialSubmaterials.ContainsKey(topMostMaterialName))
                            {
                                topMostMaterialSubmaterials[topMostMaterialName].Add(material);
                            }
                            else
                            {
                                topMostMaterialSubmaterials.Add(topMostMaterialName, new MaterialCollection() { material });
                            }
                        }
                    }

                    // We need to detach from the material parent resource
                    if (topMostMaterialSubmaterials.Count > 0)
                    {
                        resource.Load();
                        Dictionary<Material, MergeMaterialParameters> childMaterials = new Dictionary<Material, MergeMaterialParameters>();

                        foreach (KeyValuePair<string, MaterialCollection> topMostMaterial in topMostMaterialSubmaterials)
                        {
                            Material lotToMerge = new Material() { Name = topMostMaterial.Key };
                            lotToMerge.Load();

                            MergeMaterialParameters parameters = new MergeMaterialParameters();

                            foreach (Material subMaterial in topMostMaterial.Value)
                            {
                                MergeMaterialParameter parameter = new MergeMaterialParameter();
                                parameter.SubMaterial = subMaterial;
                                parameters.Add(parameter);
                            }

                            childMaterials.Add(lotToMerge, parameters);
                        }

                        if (childMaterials.Count > 0)
                        {
                            lot.Merge(childMaterials, new OperationAttributeCollection());
                        }
                    }

                    if (materialsToAttach.Count > 0) // just need to transfer wafers because they have no parent lot
                    {
                        lot.SpecialAddSubMaterials(materialsToAttach, new OperationAttributeCollection());
                    }

                    // Clear lot compose attribute
                    if(customSorterJobDefinitionFromEquipment.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessCompose)
					{
                        item.Key.SaveAttributes(new AttributeCollection() { { AMSOsramConstants.ContainerAttributeLot, string.Empty } });
                    }
                }
            }
            else if (futureActionType.Equals(AMSOsramConstants.CustomSorterJobDefinitionJsonPropertySplitFutureActionType, StringComparison.InvariantCultureIgnoreCase)) // SPLIT
            {
                List<string> materialNamesToTrackout = new List<string>();
                ContainerCollection containers = lot.GetContainersForMaterialFamily();
                SplitInputParametersCollection splitInputParameters = new SplitInputParametersCollection();

                if (containers.Count > 0)
                {
                    int index = 0;
                    var splitNames = Cmf.Foundation.BusinessObjects.NameGenerator.GenerateName("CustomSplitLotNameGenerator", lot, materialsOnEachContainer.Count);

                    foreach (var item in materialsOnEachContainer)
                    {
                        Container destinationContainer = item.Key;
                        destinationContainer.LoadRelations("MaterialContainer");
                        SplitInputSubMaterialCollection splitInputSubMaterials = new SplitInputSubMaterialCollection();

                        materialNamesToTrackout.Add(splitNames[index]);

                        foreach (MaterialContainer subMaterialContainer in item.Value)
                        {
                            SplitInputSubMaterial splitInputSubMaterial = new SplitInputSubMaterial
                            {
                                MaterialContainer = subMaterialContainer,
                                SubMaterial = subMaterialContainer.SourceEntity
                            };

                            splitInputSubMaterials.Add(splitInputSubMaterial);
                        }

                        SplitInputParameters splitInputParameter = new SplitInputParameters
                        {
                            Name = splitNames[index],
                            SubMaterials = splitInputSubMaterials
                        };

                        splitInputParameters.Add(splitInputParameter);

                        index++;
                    }
                }

                lot.Split(splitInputParameters, containers.Count == numberOfDestinationContainers);//containers.Count == numberOfDestinationContainers);

                foreach (string materialName in materialNamesToTrackout)
                {
                    Material newLot = new Material();
                    newLot.Load(materialName);

                    Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexTrackOutAndMoveMaterialToNextStepInput complexTrackOutAndMoveMaterialToNextStepInput = new Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexTrackOutAndMoveMaterialToNextStepInput()
                    {
                        Material = newLot
                    };

                    Cmf.Navigo.BusinessOrchestration.MaterialManagement.MaterialManagementOrchestration.ComplexTrackOutAndMoveMaterialToNextStep(complexTrackOutAndMoveMaterialToNextStepInput);
                }
            }
            else if (futureActionType.Equals(AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyGradingFutureActionType, StringComparison.InvariantCultureIgnoreCase) // Grading process
                || futureActionType.Equals(AMSOsramConstants.CustomSorterJobDefinitionJsonPropertyScrapFutureActionType, StringComparison.InvariantCultureIgnoreCase)) // Scrap process
            {
                // Retrieve containers associated with the current lot
                // Since we already transferred wafers to the destination container
                //      the containers found here must be the ones defined on the sorter job movement list
                ContainerCollection containers = lot.GetContainersForMaterialFamily();

                MaterialCollection materialsToDetach = new MaterialCollection();
                // In case the target containers have a lot associated we need to attach the materials to that lot
                Dictionary<string, MaterialCollection> materialsToAttach = new Dictionary<string, MaterialCollection>();

                foreach (var item in materialsOnEachContainer)
                {
                    Container targetContainer = item.Key;

                    // Check if the target container (from the sorter job movement list) belongs
                    //      to the containers retireved from the lot above.
                    if (containers.Any(c => c.Name.Equals(targetContainer.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        targetContainer.LoadRelations("MaterialContainer");

                        // Check if all the 'ContainerMaterials' on the sorter job movement list are on
                        //      the 'ContainerMaterials' of the target container
                        if (targetContainer.ContainerMaterials != null && 
                            targetContainer.ContainerMaterials.Count > 0 &&
                            item.Value != null &&
                            item.Value.Count > 0 &&
                            item.Value.All(a => targetContainer.ContainerMaterials.Contains(a)))
                        {
                            // Add those Materials to the materials to attach collection
                            materialsToDetach.AddRange(item.Value.Select(m => m.SourceEntity));
						}
					}
                }

                // Detach materials from the lot
                if (materialsToDetach.Count > 0)
                {
                    lot.RemoveSubMaterials(materialsToDetach, true, true, new OperationAttributeCollection());
                }
                else
                {
                    throw new CmfBaseException($"{futureActionType} process must have at least one wafer to detach from the lot ({lot.Name}).");
                }

                // Attach materials to lots retrieved from the target containers
                foreach (var lotToAttachMaterials in materialsToAttach)
                {
                    Material lotToAttach = new Material();
                    lotToAttach.Load(lotToAttachMaterials.Key);
                    lotToAttach.SpecialAddSubMaterials(lotToAttachMaterials.Value, new OperationAttributeCollection());
                }
            }

            if (deleteOnCompletion)
			{
                customSorterJobDefinitionFromMES.Terminate();
            }
            //---End DEE Code---

            return Input;
        }
	}
}
