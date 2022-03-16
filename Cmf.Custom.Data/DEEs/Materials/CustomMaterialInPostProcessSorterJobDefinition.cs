using Cmf.Custom.AMSOsram.BusinessObjects;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cmf.Custom.AMSOsram.Actions.Materials
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
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");
            // Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            // Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.AMSOsram.BusinessObjects");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.DataStructures");
            // System
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
            UseReference("%MicrosoftNetPath%System.ObjectModel.dll", "");


            CustomSorterJobDefinition customSorterJobDefinition = Input["CustomSorterJobDefinition"] as CustomSorterJobDefinition;
            string currentContainerName = Input["CurrentContainerName"] as string;
            string currentMaterialName = Input["CurrentMaterialName"] as string;
            string currentLoadPort = Input["CurrentLoadPort"] as string;
            string futureActionType = Input["FutureActionType"] as string;
            Resource resource = Input["Resource"] as Resource;
            List<ResourceLoadPortData> dockedContainers = Input["ContainersDocked"] as List<ResourceLoadPortData>;                       

            ResourceCollection loadPortsToSetInUse = new ResourceCollection();

            if (customSorterJobDefinition.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessMapCarrier)
            {
                loadPortsToSetInUse.Add(new Resource() { Name = currentLoadPort });
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
                            var dockedContainer = dockedContainers.FirstOrDefault(c => c.ContainerName.Equals(containerName, StringComparison.InvariantCultureIgnoreCase));

                            if (dockedContainer != null)
							{
                                loadPortsToSetInUse.Add(new Resource() { Name = dockedContainer.LoadPortName });
                            }                          
                        }

                        if (customSorterJobDefinition.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessTransferWafers &&
                            futureActionType.Equals("Merge", StringComparison.InvariantCultureIgnoreCase))
                        {
                            List<string> materialsToTrackIn = new List<string>();

                            foreach (string containerName in allContainersNeeded.Where(c => c != currentContainerName))
                            {
                                var dockedContainer = dockedContainers.FirstOrDefault(l => !string.IsNullOrWhiteSpace(l.ParentMaterialName) && l.ContainerName.Equals(containerName, StringComparison.InvariantCultureIgnoreCase));

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
                                    Material otherMaterialToTrackIn = new Material();
                                    otherMaterialToTrackIn.Load(materialName);

                                    if (otherMaterialToTrackIn.SystemState == MaterialSystemState.Dispatched)
                                    {
                                        resource.Load();

                                        Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexTrackInMaterialInput complexTrackIn = new Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexTrackInMaterialInput()
                                        {
                                            Material = otherMaterialToTrackIn,
                                            Resource = resource
                                        };
                                        ApplicationContext.CallContext.SetInformationContext("TrackInForChildLot", true);
                                        Cmf.Navigo.BusinessOrchestration.MaterialManagement.MaterialManagementOrchestration.ComplexTrackInMaterial(complexTrackIn);
                                        ApplicationContext.CallContext.SetInformationContext("TrackInForChildLot", false);
                                    }
                                    else if (otherMaterialToTrackIn.SystemState == MaterialSystemState.Queued)
                                    {
                                        resource.Load();

                                        Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexDispatchAndTrackInMaterialInput complexDispatchTrackIn = new Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ComplexDispatchAndTrackInMaterialInput()
                                        {
                                            Material = otherMaterialToTrackIn,
                                            Resource = resource,
                                            ScheduleOverride = true
                                        };

                                        ApplicationContext.CallContext.SetInformationContext("TrackInForChildLot", true);
                                        Cmf.Navigo.BusinessOrchestration.MaterialManagement.MaterialManagementOrchestration.ComplexDispatchAndTrackInMaterial(complexDispatchTrackIn);
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

                foreach (Resource loadPort in loadPortsToSetInUse)
                {
                    loadPort.SaveAttributes(new AttributeCollection() { { AMSOsramConstants.ResourceAttributeIsLoadPortInUse, true } });
                }
            }
            //---End DEE Code---

            return Input;
        }
	}
}
