using Cmf.Foundation.Common;
using System;
using System.Collections.Generic;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.Automation
{
    class CustomResolveContainerLoadPortPair : DeeDevBase
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
             *     
             *  
             * Action Groups:
             *      None
             *     
            */

            #endregion

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

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            // Newtonsoft
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json");

            if (!Input.ContainsKey("ArrayContainersToResolve"))
            {
                throw new CmfBaseException("Not a valid ArrayContainersToResolve Input!");
            }

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            string[] arrayToResolve = new string[] { };

            if (Input["ArrayContainersToResolve"] is Newtonsoft.Json.Linq.JArray)
            {
                arrayToResolve = (Input["ArrayContainersToResolve"] as Newtonsoft.Json.Linq.JArray).ToObject<string[]>();
            }
            else
            {
                throw new CmfBaseException("Not a valid ArrayContainersToResolve Input!");
            }

            Dictionary<int, string> containersResolved = new Dictionary<int, string>();

            foreach (string containerName in arrayToResolve)
            {
                IContainer container = entityFactory.Create<IContainer>();
                container.Name = containerName;

                if (!container.ObjectExists())
                {
                    throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Container, container.Name);
                }
                
                container.Load();

                container.LoadRelations(Navigo.Common.Constants.ContainerResource);

                foreach (IContainerResource containerResource in container.ContainerResourceRelations)
                {
                    if (containerResource.SourceEntity.ResourceAssociationType == ContainerResourceAssociationType.DockedContainer)
                        containersResolved.Add(containerResource.TargetEntity.DisplayOrder ?? -1, containerName);
                }
            }

            Input.Add("ContainerResolved", containersResolved);

            //---End DEE Code---

            return Input;
        }
    }
}
