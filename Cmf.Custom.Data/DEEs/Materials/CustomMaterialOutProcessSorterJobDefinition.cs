using Cmf.Custom.amsOSRAM.BusinessObjects;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessOrchestration.Abstractions;
using System.Collections.ObjectModel;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.LocalizationService;
using Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects;

namespace Cmf.Custom.amsOSRAM.Actions.Materials
{
    public class CustomMaterialOutProcessSorterJobDefinition : DeeDevBase
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

            // Foundation
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common.LocalizationService");

            // Navigo
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.Abstractions");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.amsOSRAM.BusinessObjects");
            UseReference("Cmf.Custom.amsOSRAM.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions");
            
            // System
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
            UseReference("%MicrosoftNetPath%System.ObjectModel.dll", "");

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();
            ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();

            CustomSorterJobDefinition customSorterJobDefinitionFromEquipment = Input["CustomSorterJobDefinition"] as CustomSorterJobDefinition;
            IResource resource = Input["Resource"] as IResource;
            IMaterial lot = Input["Lot"] as IMaterial;
            bool deleteOnCompletion = false;

            ICustomSorterJobDefinition customSorterJobDefinitionFromMES = entityFactory.Create<ICustomSorterJobDefinition>();
            customSorterJobDefinitionFromMES.Name = customSorterJobDefinitionFromEquipment.Name;

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
            string futureActionType = movementListObject[amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyFutureActionType].Value<string>() ?? string.Empty;

            if(movementListObject.ContainsKey(amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyDeleteOnCompletion))
			{
                deleteOnCompletion = movementListObject[amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyDeleteOnCompletion].Value<bool>();
            }

            JArray movementList = JArray.Parse(customSorterJobDefinitionFromEquipment.MovementList);

            // Check number of source and destination containers
            int numberOfSourceContainers = movementList.DistinctBy(m => m.Value<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer)).Count();
            int numberOfDestinationContainers = movementList.DistinctBy(m => m.Value<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer)).Count();

            bool isMergeOrCompose = futureActionType.Equals(amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyMergeFutureActionType, StringComparison.InvariantCultureIgnoreCase) || 
                customSorterJobDefinitionFromEquipment.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessCompose;

            // Dictionary<CONTAINER_NAME, <MATERIAL_NAME, POSITION>
            Dictionary<string, List<Tuple<string, int>>> movementListMap = new Dictionary<string, List<Tuple<string, int>>>();

            foreach (JToken movement in movementList)
            {
                // Source
                string materialName = movement.Value<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName);
                string sourceContainer = movement.Value<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer);
                int sourcePosition = movement.Value<int>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition);
                // Destination
                string destinationContainer = movement.Value<string>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer);
                int destinationPosition = movement.Value<int>(amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition);

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

            Dictionary<IContainer, IMaterialContainerCollection> materialsOnEachContainer = new Dictionary<IContainer, IMaterialContainerCollection>();
            IContainerCollection destinationContainers = entityFactory.CreateCollection<IContainerCollection>();
            destinationContainers.AddRange(movementListMap.Keys.Select(containerName =>
            {
                IContainer container = entityFactory.Create<IContainer>();
                container.Name = containerName;
                return container;
            }));
            destinationContainers.Load();
            destinationContainers.LoadRelations("MaterialContainer");

            foreach (KeyValuePair<string, List<Tuple<string, int>>> movement in movementListMap)
            {
                IContainer container = destinationContainers.FirstOrDefault(f => f.Name == movement.Key);

                foreach (Tuple<string, int> materialMovement in movement.Value)
                {
                    string materialName = materialMovement.Item1;
                    int positionOnContainer = materialMovement.Item2;

                    IMaterialContainer materialContainer = container.ContainerMaterials?.FirstOrDefault(f => f.SourceEntity.Name == materialName);

                    if (materialContainer != null && materialContainer.Position == positionOnContainer)
                    {
                        if (materialsOnEachContainer.ContainsKey(container))
                        {
                            materialsOnEachContainer[container].Add(materialContainer);
                        }
                        else
                        {
                            IMaterialContainerCollection materialContainerCollection = entityFactory.CreateCollection<IMaterialContainerCollection>();
                            materialContainerCollection.Add(materialContainer);
                            materialsOnEachContainer.Add(container, materialContainerCollection);
                        }
                    } else if (customSorterJobDefinitionFromEquipment.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessCompose)
                    {
                        throw new CmfBaseException(localizationService.Localize(amsOSRAMConstants.LocalizedMessageCustomMismatchMovementList));
                    }
                }
            }

            if (isMergeOrCompose) // MERGE OR COMPOSE
            {
                foreach (KeyValuePair<IContainer, IMaterialContainerCollection> item in materialsOnEachContainer)
                {
                    IContainer currentContainer = item.Key;
                    List<IMaterial> materials = item.Value.Select(s => s.SourceEntity).ToList();
                    Dictionary<string, IMaterialCollection> topMostMaterialSubMaterials = new Dictionary<string, IMaterialCollection>();
                    IMaterialCollection materialsToAttach = entityFactory.CreateCollection<IMaterialCollection>();

                    foreach (IMaterial material in materials)
                    {
                        material.Load();

                        if (customSorterJobDefinitionFromEquipment.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessCompose)
                        {
                            materialsToAttach.Add(material);
                        }
                        else // This should be a Merge
                        {
                            string topMostMaterialName = material.ParentMaterial.Name;
                            if (topMostMaterialSubMaterials.ContainsKey(topMostMaterialName))
                            {
                                topMostMaterialSubMaterials[topMostMaterialName].Add(material);
                            }
                            else
                            {
                                IMaterialCollection topMostMaterialCollection = entityFactory.CreateCollection<IMaterialCollection>();
                                topMostMaterialCollection.Add(material);
                                topMostMaterialSubMaterials.Add(topMostMaterialName, topMostMaterialCollection);
                            }
                        }
                    }

                    // We need to detach from the material parent resource
                    if (topMostMaterialSubMaterials.Count > 0)
                    {
                        resource.Load();
                        Dictionary<IMaterial, IMergeMaterialParameters> childMaterials = new Dictionary<IMaterial, IMergeMaterialParameters>();

                        foreach (KeyValuePair<string, IMaterialCollection> topMostMaterial in topMostMaterialSubMaterials)
                        {
                            IMaterial lotToMerge = entityFactory.Create<IMaterial>();
                            lotToMerge.Name = topMostMaterial.Key;
                            lotToMerge.Load();

                            IMergeMaterialParameters parameters = new MergeMaterialParameters();

                            foreach (IMaterial subMaterial in topMostMaterial.Value)
                            {
                                IMergeMaterialParameter parameter = new MergeMaterialParameter();
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

                    if (materialsToAttach.Count > 0)
                    {
                        IMaterialCollection logicalWafers = entityFactory.CreateCollection<IMaterialCollection>();
                        Dictionary<string, string> mapLogicalWaferToWafer = new Dictionary<string, string>();
                        Dictionary<string, int> mapLogicalWaferToContainerPosition = new Dictionary<string, int>();
                        
                        foreach (IMaterial wafer in materialsToAttach)
                        {
                            IMaterial logicalWafer = entityFactory.Create<IMaterial>();
                            int position = item.Value.FirstOrDefault(f => f.SourceEntity.Name == wafer.Name).Position.Value;

                            logicalWafer.Name = $"{lot.Name}_{position}";
                            logicalWafer.PrimaryQuantity = 0;
                            logicalWafer.PrimaryUnits = lot.PrimaryUnits;
                            logicalWafer.Form = amsOSRAMConstants.MaterialLogicalWaferForm;

                            bool isAssociatedWithTheLot = false;

                            if (logicalWafer.ObjectExists())
                            {
                                logicalWafer.Load();
                                
                                // Validates if Logical Wafer has a different Parent Lot
                                if (logicalWafer.ParentMaterial != null && 
                                    (logicalWafer.ParentMaterial.Id != lot.Id || logicalWafer.ParentMaterial.Form != amsOSRAMConstants.MaterialLotForm))
                                {
                                    throw new CmfBaseException(string.Format(
                                        localizationService.Localize(amsOSRAMConstants.LocalizedMessageCustomLogicalWaferDifferentLot),
                                        logicalWafer.Name, 
                                        lot.Name));
                                }

                                isAssociatedWithTheLot = logicalWafer.ParentMaterial?.Id == lot.Id;

                                // Validates if there is a Wafer of the same type as the one we are attaching
                                // E.g.: Cannot attach more than one Wafer of the type "Substrate".
                                if (logicalWafer.SubMaterialCount > 0)
                                {
                                    logicalWafer.LoadChildren();

                                    if (logicalWafer.SubMaterials.Any(m => m.Type == wafer.Type && m.Form == wafer.Form))
                                    {
                                        throw new CmfBaseException(string.Format(
                                            localizationService.Localize(amsOSRAMConstants.LocalizedMessageCustomLogicalWaferWaferAlreadyAssociated),
                                            logicalWafer.Name, 
                                            logicalWafer.SubMaterials.First(m => m.Type == wafer.Type && m.Form == wafer.Form).Name));
                                    }
                                }

                                // Validates if Logical Wafer is associated with a container
                                logicalWafer.LoadRelations(Cmf.Navigo.Common.Constants.MaterialContainer);

                                if (logicalWafer.MaterialContainer != null && logicalWafer.MaterialContainer.Count > 0)
                                {
                                    throw new CmfBaseException(String.Format(
                                        localizationService.Localize(amsOSRAMConstants.LocalizedMessageCustomLogicalWaferContainerAlreadyAssociated),
                                        logicalWafer.Name,
                                        logicalWafer.MaterialContainer.FirstOrDefault()?.TargetEntity?.Name));
                                }
                            } 

                            if (!isAssociatedWithTheLot)
                            {
                                logicalWafers.Add(logicalWafer);
                            }

                            mapLogicalWaferToWafer.Add(logicalWafer.Name, wafer.Name);
                            mapLogicalWaferToContainerPosition.Add(logicalWafer.Name, position);
                        }

                        logicalWafers = lot.Expand(logicalWafers, amsOSRAMConstants.MaterialLogicalWaferForm, serviceProvider.GetService<IOperationAttributeCollection>());

                        // Attach Wafer to Logical Wafers
                        foreach (KeyValuePair<string, string> values in mapLogicalWaferToWafer)
                        {
                            IMaterial logicalWafer = logicalWafers.FirstOrDefault(f => f.Name == values.Key);
                            IMaterialCollection wafers = entityFactory.CreateCollection<IMaterialCollection>();
                            wafers.Add(materialsToAttach.FirstOrDefault(f => f.Name == values.Value));

                            logicalWafer.SpecialAddSubMaterials(wafers, new OperationAttributeCollection());
                        }

                        // Empty container and associate logical wafers instead
                        IContainerOrchestration containerOrchestration = serviceProvider.GetService<IContainerOrchestration>();
                        currentContainer = containerOrchestration.EmptyContainer(new EmptyContainerInput
                        {
                            Container = currentContainer
                        }).Container;

                        IMaterialContainerCollection materialContainers = serviceProvider.GetService<IMaterialContainerCollection>();

                        foreach(KeyValuePair<string, int> logicalWaferToContainerPosition in mapLogicalWaferToContainerPosition)
                        {
                            IMaterialContainer materialContainer = serviceProvider.GetService<IMaterialContainer>();
                            materialContainer.TargetEntity = currentContainer;
                            materialContainer.SourceEntity = logicalWafers.FirstOrDefault(f => f.Name == logicalWaferToContainerPosition.Key);
                            materialContainer.Position = logicalWaferToContainerPosition.Value;

                            materialContainers.Add(materialContainer);
                        }

                        currentContainer.AssociateMaterials(materialContainers);
                    }

                    // Clear lot compose attribute
                    if(customSorterJobDefinitionFromEquipment.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessCompose)
					{
                        currentContainer.SaveAttributes(new AttributeCollection() { { amsOSRAMConstants.ContainerAttributeLot, string.Empty } });
                    }
                }
            }
            else if (futureActionType.Equals(amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertySplitFutureActionType, StringComparison.InvariantCultureIgnoreCase)) // SPLIT
            {
                List<string> materialNamesToTrackout = new List<string>();
                IContainerCollection containers = lot.GetContainersForMaterialFamily();
                ISplitInputParametersCollection splitInputParameters = new SplitInputParametersCollection();

                if (containers.Count > 0)
                {
                    foreach (KeyValuePair<IContainer, IMaterialContainerCollection> item in materialsOnEachContainer)
                    {
                        var splitName = NameGenerator.GenerateName("CustomGenerateSplitLotNames", lot);
                        
                        IContainer destinationContainer = item.Key;
                        destinationContainer.LoadRelations("MaterialContainer");
                        ISplitInputSubMaterialCollection splitInputSubMaterials = new SplitInputSubMaterialCollection();

                        materialNamesToTrackout.Add(splitName);

                        foreach (IMaterialContainer subMaterialContainer in item.Value)
                        {
                            ISplitInputSubMaterial splitInputSubMaterial = new SplitInputSubMaterial
                            {
                                MaterialContainer = subMaterialContainer,
                                SubMaterial = subMaterialContainer.SourceEntity
                            };

                            splitInputSubMaterials.Add(splitInputSubMaterial);
                        }

                        ISplitInputParameters splitInputParameter = new SplitInputParameters
                        {
                            Name = splitName,
                            SubMaterials = splitInputSubMaterials
                        };

                        splitInputParameters.Add(splitInputParameter);
                        }
                }

                lot.Split(splitInputParameters, containers.Count == numberOfDestinationContainers);//containers.Count == numberOfDestinationContainers);

                foreach (string materialName in materialNamesToTrackout)
                {
                    IMaterial newLot = entityFactory.Create<IMaterial>();
                    newLot.Load(materialName);

                    Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexTrackOutAndMoveMaterialToNextStepInput complexTrackOutAndMoveMaterialToNextStepInput = new Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexTrackOutAndMoveMaterialToNextStepInput()
                    {
                        Material = newLot
                    };

                    IMaterialOrchestration materialOrchestration = serviceProvider.GetService<IMaterialOrchestration>();
                    materialOrchestration.ComplexTrackOutAndMoveMaterialToNextStep(complexTrackOutAndMoveMaterialToNextStepInput);
                }
            }
            else if (futureActionType.Equals(amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyGradingFutureActionType, StringComparison.InvariantCultureIgnoreCase) // Grading process
                || futureActionType.Equals(amsOSRAMConstants.CustomSorterJobDefinitionJsonPropertyScrapFutureActionType, StringComparison.InvariantCultureIgnoreCase)) // Scrap process
            {
                // Retrieve containers associated with the current lot
                // Since we already transferred wafers to the destination container
                //      the containers found here must be the ones defined on the sorter job movement list
                IContainerCollection containers = lot.GetContainersForMaterialFamily();

                IMaterialCollection materialsToDetach = entityFactory.CreateCollection<IMaterialCollection>();
                // In case the target containers have a lot associated we need to attach the materials to that lot
                Dictionary<string, IMaterialCollection> materialsToAttach = new Dictionary<string, IMaterialCollection>();

                foreach (KeyValuePair<IContainer, IMaterialContainerCollection> item in materialsOnEachContainer)
                {
                    IContainer targetContainer = item.Key;

                    // Check if the target container (from the sorter job movement list) belongs
                    //      to the containers retrieved from the lot above.
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
                foreach (KeyValuePair<string, IMaterialCollection> lotToAttachMaterials in materialsToAttach)
                {
                    IMaterial lotToAttach = entityFactory.Create<IMaterial>();
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
