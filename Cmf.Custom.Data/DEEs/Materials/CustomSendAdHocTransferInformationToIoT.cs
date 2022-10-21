using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessObjects.LocalizationService;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Foundation.Common.LocalizationService;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;

namespace Cmf.Custom.amsOSRAM.Actions.Materials
{
    public class CustomSendAdHocTransferInformationToIoT : DeeDevBase
    {
        /// <summary>
        /// Dee test condition.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <returns></returns>
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     Dee action to Trigger IoT call to process container operations
             *          sent by UI Page to execute AdHoc Sorter Jobs
             *
             * Action Groups:
             *      N/A
             *
            */

            #endregion Info

            return true;

            //---End DEE Condition Code---
        }

        /// <summary>
        /// Dee action code.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <returns></returns>
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            // Foundation
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common.LocalizationService");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.DataStructures");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.Extensions");
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            // System
            UseReference("", "System.Data");
            UseReference("", "System.Threading");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");

            #region Check if all arguments exist

            if (!Input.TryGetValueAs("Resource", out string resourceName))
            {
                throw new ArgumentNullCmfException("Resource");
            }

            if (!Input.TryGetValueAs("SorterProcess", out string sorterProcess))
            {
                throw new ArgumentNullCmfException("SorterProcess");
            }

            if (!Input.TryGetValueAs("Quantity", out long quantityToTransfer))
            {
                throw new ArgumentNullCmfException("Quantity");
            }

            if (!Input.TryGetValueAs("Product", out string productName))
            {
                throw new ArgumentNullCmfException("Product");
            }

            if (!Input.TryGetValueAs("LoadPort", out string loadPortName))
            {
                throw new ArgumentNullCmfException("LoadPort");
            }

            #endregion Check if all arguments exist

            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();
            ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();

            #region Validate arguments

            IResource resource = entityFactory.Create<IResource>();
            resource.Name = resourceName;

            if (!resource.ObjectExists())
            {
                throw new ObjectNotFoundCmfException("Resource", resource.Name);
            }

            IProduct product = entityFactory.Create<IProduct>();
            product.Name = productName;

            if (!product.ObjectExists())
            {
                throw new ObjectNotFoundCmfException("Product", product.Name);
            }

            IResource sourceLoadPort = entityFactory.Create<IResource>();
            sourceLoadPort.Name = loadPortName;

            if (!sourceLoadPort.ObjectExists())
            {
                throw new ObjectNotFoundCmfException("Resource", sourceLoadPort.Name);
            }

            #endregion Validate arguments

            #region Validate sorter process

            LookupTable lookupTable = new LookupTable();
            lookupTable.Load(amsOSRAMConstants.LookupTableCustomSorterProcess);

            if (!lookupTable.Values.Any(val => val.Value == sorterProcess))
            {
                throw new CmfBaseException(
                    string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomValueDoesNotExistLookupTable), 
                    sorterProcess, amsOSRAMConstants.LookupTableCustomSorterProcess));
            }

            #endregion Validate sorter process

            #region Validate if resource is sorter

            resource.Load();
            resource.LoadAttributes(new Collection<string> { amsOSRAMConstants.ResourceAttributeIsSorter });

            bool isSorter = false;
            resource.Attributes.TryGetValueAs(amsOSRAMConstants.ResourceAttributeIsSorter, out isSorter);

            if (!isSorter)
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomResourceIsNotSorter), resource.Name));
            }

            if (resource.AutomationMode != ResourceAutomationMode.Online)
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomResourceNotOnline), resource.Name));
            }

            #endregion Validate if resource is sorter

            #region Validate if source load port is in use and if is descendent of the given resource

            // Check if the selected load port is a sub resource of the selected Material
            IResourceHierarchy resourceHierarchy = resource.GetDescendentResources(1);

            if (!resourceHierarchy.Any(s => s.ChildResource.Name == sourceLoadPort.Name))
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomResourceNotDescendant), sourceLoadPort.Name, resource.Name));
            }

            // Throw error if load port is being used
            sourceLoadPort.Load();
            sourceLoadPort.LoadAttributes(new Collection<string> { amsOSRAMConstants.ResourceAttributeIsLoadPortInUse });

            bool isSourceLoadPortInUse = false;
            sourceLoadPort.Attributes.TryGetValueAs(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, out isSourceLoadPortInUse);

            if (isSourceLoadPortInUse)
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomResourceInUse), sourceLoadPort.Name));
            }

            #endregion Validate if source load port is in use and if is descendent of the given resource

            #region Resolve container quantities

            ISmartTable smartTable = entityFactory.Create<ISmartTable>();
            smartTable.Load(amsOSRAMConstants.CustomProductContainerCapacitiesSTName);

            product.Load();

            INgpDataRow ngpDataRow = new NgpDataRow()
            {
                { Navigo.Common.Constants.Product, product.Name },
                { Navigo.Common.Constants.ProductGroup, product.ProductGroup?.Name }
            };

            INgpDataSet ngpSmartDataSet = smartTable.Resolve(ngpDataRow, true);
            DataSet smartDataSet = NgpDataSet.ToDataSet(ngpSmartDataSet);

            if (!smartDataSet.HasData())
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomSmartTableNoResolution), "Capacity", amsOSRAMConstants.CustomProductContainerCapacitiesSTName));
            }

            DataRow resolvedRow = smartDataSet.Tables[0].Rows[0];

            int containerMaxCapacity;

            if (!int.TryParse(resolvedRow[amsOSRAMConstants.SmartTablePropertyTargetCapacity].ToString(), out containerMaxCapacity))
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomSmartTableNoResolution), resolvedRow[amsOSRAMConstants.SmartTablePropertyTargetCapacity], "Integer"));
            }

            #endregion Resolve container quantities

            #region Resolve Recipe

            DataSet recipeDataSet = amsOSRAMUtilities.ResolveRecipeContext(service: "WaferReception", product: product.Name, productGroup: product.ProductGroup.Name, resource: resource.Name);

            if (recipeDataSet == null || !recipeDataSet.HasData())
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomSmartTableNoResolution), Navigo.Common.Constants.Recipe, Navigo.Common.Constants.RecipeContext));
            }

            IRecipe recipeToSet = entityFactory.Create<IRecipe>();
            recipeToSet.Name = recipeDataSet.Tables[0].Rows[0][Navigo.Common.Constants.Recipe] as string;

            if (!recipeToSet.ObjectExists())
            {
                throw new ObjectNotFoundCmfException("Recipe", recipeToSet.Name);
            }

            recipeToSet.Load();
            recipeToSet.LoadRelations(Navigo.Common.Constants.RecipeParameter);

            RecipeData recipeDataToReturn = new RecipeData()
            {
                RecipeId = recipeToSet.Id.ToString(),
                RecipeName = recipeToSet.Name,
                NameOnEquipment = recipeToSet.ResourceRecipeName,
                Checksum = recipeToSet.BodyChecksum
            };

            List<KeyValuePair<string, IEntityRelationCollection<IEntityRelation>>> recipeParameters = recipeToSet.RelationCollection.Where(r => r.Key == Navigo.Common.Constants.RecipeParameter).ToList();

            if (recipeParameters != null && recipeParameters.Count > 0)
            {
                recipeDataToReturn.RecipeParameters = new List<RecipeParameterData>();

                foreach (KeyValuePair<string, IEntityRelationCollection<IEntityRelation>> riP in recipeParameters)
                {
                    IEntityRelationCollection<IEntityRelation> recipeValueCollection = riP.Value;

                    foreach (IEntityRelation recipeValueRelation in recipeValueCollection)
                    {
                        IRecipeParameter recipeValue = recipeValueRelation as IRecipeParameter;

                        RecipeParameterData recipeParameterToSet = new RecipeParameterData
                        {
                            Name = recipeValue.TargetEntity.Name.ToString(),
                            Value = recipeValue.Value.ToString()
                        };

                        recipeDataToReturn.RecipeParameters.Add(recipeParameterToSet);
                    }
                }
            }

            #endregion Resolve Recipe

            #region Get eligible containers (cannot have LoadPortInUse and the Container should have space and must have the same product)

            List<ResourceLoadPortData> dockedContainers = amsOSRAMUtilities.DockedContainersOnLoadPortsByParentResource(resource);

            if (dockedContainers == null || dockedContainers.Count == 0)
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomResourceNoDockerContainer), resource.Name));
            }

            IContainerCollection containers = entityFactory.CreateCollection<IContainerCollection>();
            containers.AddRange(dockedContainers
                .Where(w =>
                    w.ContainerTotalPositions > w.ContainerUsedPositions &&
                    containerMaxCapacity - w.ContainerUsedPositions > 0 &&
                    w.LoadPortInUse == false
                ).Select(s =>
                {
                    IContainer container = entityFactory.Create<IContainer>();
                    container.Name = s.ContainerName;
                    return container;
                }));

            if (containers.Count == 0)
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomResourceNoEnoughPositionsOrInUse), resource.Name));
            }

            containers.Load();
            containers.LoadRelations(Navigo.Common.Constants.MaterialContainer);

            IContainerCollection eligibleContainers = entityFactory.CreateCollection<IContainerCollection>();
            eligibleContainers.AddRange(containers.Where(w =>
                w.ContainerMaterials == null ||
                w.ContainerMaterials.Count == 0 ||
                !w.ContainerMaterials.Any(s => s.SourceEntity.Product.Name != product.Name)));

            if (eligibleContainers.Count == 0)
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomResourceContainerDockedDifferentProducts), resource.Name, product.Name));
            }

            #endregion Get eligible containers (cannot have LoadPortInUse and the Container should have space and must have the same product)

            #region Check if is possible to transfer the desire quantity

            int remainingSpace = eligibleContainers.Select(s => containerMaxCapacity - s.UsedPositions.Value).Aggregate((acc, s) => acc + s);

            if (remainingSpace < quantityToTransfer)
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomResourceContainersNoEnoughPositions), quantityToTransfer, remainingSpace));
            }

            #endregion Check if is possible to transfer the desire quantity

            #region Create the movement list

            JArray movementList = new JArray();
            int currentMovement = 0;
            Dictionary<string, IResource> containerLoadPortDestinationMap = new Dictionary<string, IResource>();

            foreach (IContainer container in eligibleContainers.OrderByDescending(o => o.UsedPositions))
            {
                if (movementList.Count == quantityToTransfer)
                {
                    break;
                }

                int startPosition = container.TotalPositions.Value;
                int containerMaterialsCount = container.UsedPositions.Value;
                int maxDestinationPosition = container.TotalPositions.Value - containerMaxCapacity;

                for (int i = startPosition; startPosition >= 0; i--)
                {
                    bool isPositionFilled = container.ContainerMaterials != null && container.ContainerMaterials.Any(c => c.Position == i);

                    if (!isPositionFilled && containerMaterialsCount != containerMaxCapacity)
                    {
                        currentMovement++;

                        JObject jObject = new JObject
                        {
                            [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyMaterialName] = $"Material_#{currentMovement}",
                            [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourceContainer] = $"CarrierAtLoadPort{sourceLoadPort.DisplayOrder.Value}",
                            [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertySourcePosition] = currentMovement,
                            [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer] = container.Name,
                            [amsOSRAMConstants.CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition] = i
                        };

                        
                        if (!containerLoadPortDestinationMap.ContainsKey(container.Name))
                        {
                            IResource loadPortDestination = entityFactory.Create<IResource>();
                            loadPortDestination.Name = dockedContainers.FirstOrDefault(f => f.ContainerName == container.Name).LoadPortName;

                            containerLoadPortDestinationMap.Add(container.Name, loadPortDestination);
                        }

                        movementList.Add(jObject);
                        containerMaterialsCount++;
                    }

                    if (i <= maxDestinationPosition && isPositionFilled)
                    {
                        throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomResourceContainersWrongPositions), i, container.Name));
                    }

                    startPosition--;

                    if (movementList.Count == quantityToTransfer)
                    {
                        break;
                    }
                }
            }

            if (containerLoadPortDestinationMap.Count > 0)
            {
                IResourceCollection resourcesToUpdate = entityFactory.CreateCollection<IResourceCollection>();
                resourcesToUpdate.AddRange(containerLoadPortDestinationMap.Values);
                resourcesToUpdate.Load();

                resourcesToUpdate.SaveAttributes(new AttributeCollection
                {
                    { amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, true }
                });
            }

            #endregion Create the movement list

            #region Create In-Memory Custom Sorter Job Definition and send it to IoT

            ICustomSorterJobDefinition customSorterJobDefinition = entityFactory.Create<ICustomSorterJobDefinition>();
            customSorterJobDefinition.LogisticalProcess = "AdHocTransferWafers-WaferReception";
            customSorterJobDefinition.FlipWafer = false;
            customSorterJobDefinition.AlignWafer = false;
            customSorterJobDefinition.WaferIdOnBottom = true;
            customSorterJobDefinition.ReadWaferId = true;
            customSorterJobDefinition.MovementList = movementList.ToString();

            MaterialData materialData = new MaterialData
            {
                MaterialId = $"CarrierAtLoadPort{sourceLoadPort.DisplayOrder.Value}",
                MaterialName = $"CarrierAtLoadPort{sourceLoadPort.DisplayOrder.Value}",
                ContainerId = $"CarrierAtLoadPort{sourceLoadPort.DisplayOrder.Value}",
                ContainerName = $"CarrierAtLoadPort{sourceLoadPort.DisplayOrder.Value}",
                LoadPortPosition = sourceLoadPort.DisplayOrder.ToString(),
                MaterialState = "Setup",
                ContainerOnlyProcess = true,
                Recipe = recipeDataToReturn,
                SorterJobInformation = customSorterJobDefinition
            };

            List<MaterialData> materialDataToIot = new List<MaterialData>();
            materialDataToIot.Add(materialData);

            // DO NOT DELETE: This is a hook for test purposes
            IAutomationControllerInstance controllerInstance = resource.GetAutomationControllerInstance();

            if (controllerInstance != null)
            {
                // Get EI default timeout
                //  --> /Cmf/Custom/Automation/TrackInTimeout
                int requestTimeout = amsOSRAMUtilities.GetConfig<int>(amsOSRAMConstants.AutomationTrackInTimeoutConfigurationPath);

                // Send Synchronous request to automation TrackIn the Material in the Equipment
                string requestType = amsOSRAMConstants.AutomationRequestTypeTrackIn;
                object obj = controllerInstance.SendRequest(requestType, materialDataToIot.ToJsonString(), requestTimeout);

                if (obj == null)
                {
                    throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageIoTConnectionTimeout), requestType));
                }

                if (obj.ToString().Contains("Error"))
                {
                    throw new CmfBaseException(obj.ToString());
                }
            }

            #endregion Create In-Memory Custom Sorter Job Definition and send it to IoT

            //---End DEE Code---

            return Input;
        }
    }
}
