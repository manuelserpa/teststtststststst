using Cmf.Custom.AMSOsram.Actions;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cmf.Custom.AMSOsram.Actions.Automation
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

            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("", "Cmf.Custom.AMSOsram.Common.Extensions");
            UseReference("Cmf.Custom.AMSOsram.Orchestration.dll", "Cmf.Custom.AMSOsram.Orchestration.InputObjects");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json");

            if (!Input.ContainsKey("ArrayContainersToResolve"))
            {
                throw new CmfBaseException("Not a valid ArrayContainersToResolve Input!");
            }

            var arrayToResolve = new string[] { };

            if (Input["ArrayContainersToResolve"] is Newtonsoft.Json.Linq.JArray)
            {
                arrayToResolve = (Input["ArrayContainersToResolve"] as Newtonsoft.Json.Linq.JArray).ToObject<string[]>();
            }
            else
            {
                throw new CmfBaseException("Not a valid ArrayContainersToResolve Input!");
            }

            Dictionary<int, string> containersResolved = new Dictionary<int, string>();

            foreach (var containerName in arrayToResolve)
            {
                Container container = new Container() { Name = containerName };


                if (!container.ObjectExists())
                {
                    throw new ObjectNotFoundCmfException(Navigo.Common.Constants.Container, container.Name);
                }
                else
                {
                    container.Load();
                }

                container.LoadRelations(Cmf.Navigo.Common.Constants.ContainerResource);

                foreach (ContainerResource containerResource in container.ContainerResourceRelations)
                {
                    if (containerResource.SourceEntity.ResourceAssociationType == ContainerResourceAssociationType.DockedContainer)
                        containersResolved.Add(containerResource.TargetEntity.DisplayOrder ?? -1, containerName);
                }
            }

            Console.WriteLine(containersResolved.ToJsonString());

            Input.Add("ContainerResolved", containersResolved);


            //---End DEE Code---

            return Input;
        }
    }
}
