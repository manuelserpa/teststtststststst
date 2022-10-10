using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Foundation.Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Cmf.Navigo.BusinessObjects.Abstractions;
using System;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Cmf.Custom.amsOSRAM.Actions.Materials
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

            // Custom
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
                        string.IsNullOrWhiteSpace(resourceLoadPortData.ParentMaterialName) &&
                        resourceLoadPortData.ContainerMapContainerNeededAttribute)
                    {
                        lotsDockedOnSorters.Add((entityFactory.Create<IMaterial>(), resourceLoadPortData.LoadPortName, resourceLoadPortData.ContainerName, resourceLoadPortData.ContainerMapContainerNeededAttribute));
                    }
                }
            }

            Input.Add("LotsDockedOnSorters", lotsDockedOnSorters);

            //---End DEE Code---

            return Input;
        }
    }
}
