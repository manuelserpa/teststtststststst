using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessOrchestration.Abstractions;
using Cmf.Navigo.BusinessOrchestration.DispatchManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.DispatchManagement.OutputObjects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Cmf.Custom.amsOSRAM.Actions.Materials
{
    public class CustomMaterialInGetLotsFromDockedContainers : DeeDevBase
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
             *     DEE Action to retrive lots on containers docked on the resource load ports.
             *
             * Action Groups:
             *     N/A
            */

            #endregion Info

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

            // Navigo
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.DispatchManagement.InputObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.DispatchManagement.OutputObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.Abstractions");

            // Custom
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.DataStructures");

            // System
            UseReference("%MicrosoftNetPath%System.Data.Common.dll", "System.Data");

            if (!Input.ContainsKey(Navigo.Common.Constants.Resource))
            {
                throw new ArgumentNullCmfException(Navigo.Common.Constants.Resource);
            }

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IResource resource = Input["Resource"] as IResource;

            List<(IMaterial material, string loadPort, string container, bool mapContainerNeeded)> lotsDockedOnSorters = new List<(IMaterial material, string loadPort, string container, bool mapContainerNeeded)>();

            List<ResourceLoadPortData> dockedContainersOnResourceLoadPorts = amsOSRAMUtilities.DockedContainersOnLoadPortsByParentResource(resource);

            foreach (ResourceLoadPortData resourceLoadPortData in dockedContainersOnResourceLoadPorts.OrderBy(d => d.LoadPortModifiedOn).ThenBy(d => d.ContainerLotAttribute))
            {
                if (!resourceLoadPortData.LoadPortInUse)
                {
                    if (!string.IsNullOrWhiteSpace(resourceLoadPortData.ContainerLotAttribute))
                    {
                        IMaterial topMostMaterial = entityFactory.Create<IMaterial>();
                        topMostMaterial.Name = resourceLoadPortData.ContainerLotAttribute;

                        if (topMostMaterial.ObjectExists() && !lotsDockedOnSorters.Any(l => l.material.Name == topMostMaterial.Name))
                        {
                            lotsDockedOnSorters.Add((topMostMaterial, resourceLoadPortData.LoadPortName, resourceLoadPortData.ContainerName, resourceLoadPortData.ContainerMapContainerNeededAttribute));
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(resourceLoadPortData.ParentMaterialName) && resourceLoadPortData.ContainerUsedPositions > 0)
                    {
                        IMaterial topMostMaterial = entityFactory.Create<IMaterial>();
                        topMostMaterial.Name = resourceLoadPortData.ParentMaterialName;

                        if (topMostMaterial.ObjectExists() && !lotsDockedOnSorters.Any(l => l.material.Name == topMostMaterial.Name))
                        {
                            lotsDockedOnSorters.Add((topMostMaterial, resourceLoadPortData.LoadPortName, resourceLoadPortData.ContainerName, resourceLoadPortData.ContainerMapContainerNeededAttribute));
                        }
                    } // Forced map carrier without lot associated with the container
                    else if (string.IsNullOrWhiteSpace(resourceLoadPortData.ContainerLotAttribute) &&
                        string.IsNullOrWhiteSpace(resourceLoadPortData.ParentMaterialName))
                    {
                        if (resourceLoadPortData.ContainerMapContainerNeededAttribute)
                        {
                            lotsDockedOnSorters.Add((entityFactory.Create<IMaterial>(), resourceLoadPortData.LoadPortName, resourceLoadPortData.ContainerName, resourceLoadPortData.ContainerMapContainerNeededAttribute));
                        }
                        else
                        {
                            IContainer container = entityFactory.Create<IContainer>();
                            container.Name = resourceLoadPortData.ContainerName;
                            container.Load();
                            container.LoadRelations(Cmf.Navigo.Common.Constants.MaterialContainer);

                            IStepCollection steps = entityFactory.CreateCollection<IStepCollection>();
                            var stepsToAdd = container.ContainerMaterials?.Select(s => s.SourceEntity.Step)?.DistinctBy(d => d.Id);
                            
                            if (stepsToAdd != null && stepsToAdd.Any())
                            {
                                steps.AddRange(stepsToAdd);

                                steps.LoadAttributes(new Collection<string> { amsOSRAMConstants.StepAttributeIsWaferReception });

                                // Ensure that all steps materials are WaferReception steps
                                if (steps.All(step => step.GetAttributeValueOrDefault<bool>(amsOSRAMConstants.StepAttributeIsWaferReception, false, false)))
                                {
                                    IDispatchOrchestration dispatchOrchestration = serviceProvider.GetService<IDispatchOrchestration>();
                                   
                                    GetDispatchListForResourceOutput getDispatchListForResourceOutput = dispatchOrchestration.GetDispatchListForResource(new GetDispatchListForResourceInput {
                                        Resource = resource
                                    });

                                    if (getDispatchListForResourceOutput != null 
                                        && getDispatchListForResourceOutput.Materials != null
                                        && getDispatchListForResourceOutput.Materials.Any())
                                    {
                                        // Use collection for future implementation when multiple lots per container will be in place.
                                        // For now, it is only consider one lot only
                                        IMaterialCollection lots = entityFactory.CreateCollection<IMaterialCollection>();
                                        lots.Add(getDispatchListForResourceOutput.Materials
                                            .Where(w => w.Form == amsOSRAMConstants.MaterialLotForm)
                                            .First());

                                        foreach (IMaterial lot in lots)
                                        {
                                            lotsDockedOnSorters.Add((lot, resourceLoadPortData.LoadPortName, resourceLoadPortData.ContainerName, resourceLoadPortData.ContainerMapContainerNeededAttribute));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Input.Add("LotsDockedOnSorters", lotsDockedOnSorters);

            //---End DEE Code---

            return Input;
        }
    }
}
