using Cmf.Custom.amsOSRAM.BusinessObjects;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Foundation.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Navigo.BusinessOrchestration.Abstractions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.Materials
{
    class CustomMaterialInPostProcessSorterJobDefinition : DeeDevBase
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
             *     DEE Action to set the necessary load ports to in Use.
             *     Also, if the Custom Sorter Job Definition Movement List FutureAction Type is 'Merge'
             *			we Track-In associated child lots.
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

            // Navigo
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.Abstractions");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.amsOSRAM.BusinessObjects");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.DataStructures");

            // System
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
            UseReference("%MicrosoftNetPath%System.ObjectModel.dll", "");

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            CustomSorterJobDefinition customSorterJobDefinition = Input["CustomSorterJobDefinition"] as CustomSorterJobDefinition;
            string currentContainerName = Input["CurrentContainerName"] as string;
            string currentMaterialName = Input["CurrentMaterialName"] as string;
            string currentLoadPort = Input["CurrentLoadPort"] as string;
            string futureActionType = Input["FutureActionType"] as string;
            IResource resource = Input["Resource"] as IResource;
            List<ResourceLoadPortData> dockedContainers = Input["ContainersDocked"] as List<ResourceLoadPortData>;                       

            IResourceCollection loadPortsToSetInUse = entityFactory.CreateCollection<IResourceCollection>();

            if (customSorterJobDefinition.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessMapCarrier)
            {
                IResource currentLoadPortResource = entityFactory.Create<IResource>();
                currentLoadPortResource.Name = currentLoadPort;

                loadPortsToSetInUse.Add(currentLoadPortResource);
            }
            else
            {
                if (dockedContainers == null)
                {
                    throw new CmfBaseException("It must exist at least one docked container.");
                }

                if (!string.IsNullOrWhiteSpace(customSorterJobDefinition.MovementList))
                {
                    JArray movementList = JArray.Parse(customSorterJobDefinition.MovementList);

                    if (movementList != null)
                    {
                        List<string> sourceContainers = movementList.DistinctBy(m => m.Value<string>("SourceContainer")).Values<string>("SourceContainer").ToList();
                        List<string> destinationContainers = movementList.DistinctBy(m => m.Value<string>("DestinationContainer")).Values<string>("DestinationContainer").ToList();
                        List<string> allContainersNeeded = new List<string>();
                        allContainersNeeded.AddRange(sourceContainers);
                        allContainersNeeded.AddRange(destinationContainers);

                        foreach (string containerName in allContainersNeeded)
                        {
                            ResourceLoadPortData dockedContainer = dockedContainers.FirstOrDefault(c => c.ContainerName.Equals(containerName, StringComparison.InvariantCultureIgnoreCase));

                            if (dockedContainer != null)
                            {
                                IResource resourceToSet = entityFactory.Create<IResource>();
                                resourceToSet.Name = dockedContainer.LoadPortName;
                                resourceToSet.Load();

                                if (!loadPortsToSetInUse.Contains(resourceToSet))
                                {
                                    loadPortsToSetInUse.Add(resourceToSet);
                                }
                            }
                        }

                        if (customSorterJobDefinition.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessTransferWafers &&
                            futureActionType.Equals("Merge", StringComparison.InvariantCultureIgnoreCase))
                        {
                            List<string> materialsToTrackIn = new List<string>();

                            foreach (string containerName in allContainersNeeded.Where(c => c != currentContainerName))
                            {
                                ResourceLoadPortData dockedContainer = dockedContainers.FirstOrDefault(l => !string.IsNullOrWhiteSpace(l.ParentMaterialName) && l.ContainerName.Equals(containerName, StringComparison.InvariantCultureIgnoreCase));

                                if (dockedContainer != null)
								{
                                    materialsToTrackIn.Add(dockedContainer.ParentMaterialName);
                                }
                            }

                            if (materialsToTrackIn.Count > 0)
                            {
                                // Just to make sure we are not track in the current material
                                // And we are not tracking in the same material more than one time
                                foreach (string materialName in materialsToTrackIn.Where(m => m != currentMaterialName).Distinct())
                                {
                                    IMaterial otherMaterialToTrackIn = entityFactory.Create<IMaterial>();
                                    otherMaterialToTrackIn.Name = materialName;
                                    otherMaterialToTrackIn.Load();

                                    IMaterialOrchestration materialOrchestration = serviceProvider.GetService<IMaterialOrchestration>();

                                    if (otherMaterialToTrackIn.SystemState == MaterialSystemState.Dispatched)
                                    {
                                        resource.Load();

                                        Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexTrackInMaterialInput complexTrackIn = new Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexTrackInMaterialInput()
                                        {
                                            Material = otherMaterialToTrackIn,
                                            Resource = resource
                                        };

                                        ApplicationContext.CallContext.SetInformationContext("TrackInForChildLot", true);

                                        materialOrchestration.ComplexTrackInMaterial(complexTrackIn);
                                        
                                        ApplicationContext.CallContext.SetInformationContext("TrackInForChildLot", false);
                                    }
                                    else if (otherMaterialToTrackIn.SystemState == MaterialSystemState.Queued)
                                    {
                                        resource.Load();

                                        Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexDispatchAndTrackInMaterialInput complexDispatchTrackIn = new Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexDispatchAndTrackInMaterialInput()
                                        {
                                            Material = otherMaterialToTrackIn,
                                            Resource = resource,
                                            ScheduleOverride = true
                                        };

                                        ApplicationContext.CallContext.SetInformationContext("TrackInForChildLot", true);
                                        materialOrchestration.ComplexDispatchAndTrackInMaterial(complexDispatchTrackIn);
                                        ApplicationContext.CallContext.SetInformationContext("TrackInForChildLot", false);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Set load ports attribute 'LoadPortInUse' = TRUE
            if (loadPortsToSetInUse.Count > 0)
            {
                loadPortsToSetInUse.Load();

                foreach (IResource loadPort in loadPortsToSetInUse)
                {
                    loadPort.SaveAttributes(new AttributeCollection() { { amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, true } });
                }
            }

            //---End DEE Code---

            return Input;
        }
	}
}
