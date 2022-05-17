using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Cmf.Custom.AMSOsram.Actions.Materials
{
    class CustomMaterialInGetLotsFromDockedContainers : DeeDevBase
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
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.DataStructures");
            // System
            UseReference("%MicrosoftNetPath%System.Data.Common.dll", "System.Data");


            if (!Input.ContainsKey(Navigo.Common.Constants.Resource))
            {
                throw new ArgumentNullCmfException(Navigo.Common.Constants.Resource);
            }

            Resource resource = Input["Resource"] as Resource;

            List<(Material material, string loadPort, string container, bool mapContainerNeeded)> lotsDockedOnSorters = new List<(Material material, string loadPort, string container, bool mapContainerNeeded)>();

            List<ResourceLoadPortData> dockedContainersOnResourceLoadPorts = AMSOsramUtilities.DockedContainersOnLoadPortsByParentResource(resource);

            foreach (ResourceLoadPortData resourceLoadPortData in dockedContainersOnResourceLoadPorts.OrderBy(d => d.LoadPortModifiedOn).ThenBy(d => d.ContainerLotAttribute))
            {
                if (!resourceLoadPortData.LoadPortInUse)
                {
                    if (!string.IsNullOrWhiteSpace(resourceLoadPortData.ContainerLotAttribute))
                    {
                        Material topMostMaterial = new Material { Name = resourceLoadPortData.ContainerLotAttribute };

                        if (topMostMaterial.ObjectExists() && !lotsDockedOnSorters.Any(l => l.material.Name == topMostMaterial.Name))
                        {
                            lotsDockedOnSorters.Add((topMostMaterial, resourceLoadPortData.LoadPortName, resourceLoadPortData.ContainerName, resourceLoadPortData.ContainerMapContainerNeededAttribute));
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(resourceLoadPortData.ParentMaterialName) && resourceLoadPortData.ContainerUsedPositions > 0)
                    {
                        Material topMostMaterial = new Material { Name = resourceLoadPortData.ParentMaterialName };

                        if (topMostMaterial.ObjectExists() && !lotsDockedOnSorters.Any(l => l.material.Name == topMostMaterial.Name))
                        {
                            lotsDockedOnSorters.Add((topMostMaterial, resourceLoadPortData.LoadPortName, resourceLoadPortData.ContainerName, resourceLoadPortData.ContainerMapContainerNeededAttribute));
                        }
                    } // Forced map carrier without lot associated with the container
                    else if (string.IsNullOrWhiteSpace(resourceLoadPortData.ContainerLotAttribute) &&
                        string.IsNullOrWhiteSpace(resourceLoadPortData.ParentMaterialName) &&
                        resourceLoadPortData.ContainerMapContainerNeededAttribute)
                    {
                        lotsDockedOnSorters.Add((new Material(), resourceLoadPortData.LoadPortName, resourceLoadPortData.ContainerName, resourceLoadPortData.ContainerMapContainerNeededAttribute));
                    }
                }
            }

            Input.Add("LotsDockedOnSorters", lotsDockedOnSorters);
            //---End DEE Code---

            return Input;
        }
    }
}
