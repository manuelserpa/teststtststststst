using Cmf.Foundation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.Automation
{
    class CustomAutomationSetCustomResourceState : DeeDevBase
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
             *     Dee action is triggered by IoT Automation to adjust the state of a Resource 
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

            if (!Input.ContainsKey("ResourceName"))
            {
                throw new ArgumentNullCmfException("ResourceName");
            }

            if (!Input.ContainsKey("StateName"))
            {
                throw new ArgumentNullCmfException("StateName");
            }

            if (!Input.ContainsKey("StateModelName"))
            {
                throw new ArgumentNullCmfException("StateModelName");
            }

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IResource resource = entityFactory.Create<IResource>();
            resource.Name = Input["ResourceName"] as string;

            string state = Input["StateName"] as string;
            string stateModelName = Input["StateModelName"] as string;

            int? loadPortNumber = null;
            if (Input.ContainsKey("LoadPortNumber") && Input["LoadPortNumber"] != null && !String.IsNullOrWhiteSpace(Input["LoadPortNumber"].ToString()))
            {
                loadPortNumber = int.Parse(Input["LoadPortNumber"].ToString());
            }

            int? chamberResourceNumber = null;
            if (Input.ContainsKey("ChamberResourceNumber") && Input["ChamberResourceNumber"] != null && !String.IsNullOrWhiteSpace(Input["ChamberResourceNumber"].ToString()))
            {
                chamberResourceNumber = int.Parse(Input["ChamberResourceNumber"].ToString());
            }

            int? componentResourceNumber = null;
            if (Input.ContainsKey("ComponentResourceNumber") && Input["ComponentResourceNumber"] != null && !String.IsNullOrWhiteSpace(Input["ComponentResourceNumber"].ToString()))
            {
                componentResourceNumber = int.Parse(Input["ComponentResourceNumber"].ToString());
            }

            string reason = String.Empty;

            if (Input.ContainsKey("Reason"))
            {
                reason = Input["Reason"] as string;
            }

            if (loadPortNumber != null && loadPortNumber > 0)
            {
                resource.Load();
                IResourceHierarchy resourceHierarchy = resource.GetDescendentResources(1);

                if (resourceHierarchy == null ||
                    resourceHierarchy.Count <= 0)
                {
                    throw new MissingMandatoryPropertyCmfException("SubResource", resource.Name); ;
                }

                resource = resourceHierarchy.FirstOrDefault(s => s.ChildResource.DisplayOrder != null && s.ChildResource.DisplayOrder.Value == loadPortNumber && s.ChildResource.ProcessingType == ProcessingType.LoadPort).ChildResource;
            }
            else if (chamberResourceNumber != null && chamberResourceNumber > 0)
            {
                resource.Load();
                IResourceHierarchy resourceHierarchy = resource.GetDescendentResources(1);

                if (resourceHierarchy == null ||
                    resourceHierarchy.Count <= 0)
                {
                    throw new MissingMandatoryPropertyCmfException("SubResource", resource.Name); ;
                }

                resource = resourceHierarchy.FirstOrDefault(s => s.ChildResource.DisplayOrder != null && s.ChildResource.DisplayOrder.Value == chamberResourceNumber && s.ChildResource.ProcessingType == ProcessingType.Process).ChildResource;
            }
            else if (componentResourceNumber != null && componentResourceNumber > 0)
            {
                resource.Load();
                IResourceHierarchy resourceHierarchy = resource.GetDescendentResources(1);

                if (resourceHierarchy == null ||
                    resourceHierarchy.Count <= 0)
                {
                    throw new MissingMandatoryPropertyCmfException("SubResource", resource.Name); ;
                }

                resource = resourceHierarchy.FirstOrDefault(s => s.ChildResource.DisplayOrder != null && s.ChildResource.DisplayOrder.Value == componentResourceNumber && s.ChildResource.ProcessingType == ProcessingType.Component).ChildResource;
            }

            resource.Load();
            resource.LoadCurrentStates();
            ICurrentEntityState current = null;

            if (resource.CurrentStates != null)
            {
                current = resource.CurrentStates.FirstOrDefault(s => s != null && s.StateModel.Name == stateModelName);
            }
            if (current == null)
            {
                IStateModel stateModelObject = entityFactory.Create<IStateModel>();
                stateModelObject.Name = stateModelName;
                stateModelObject.Load();

                IStateModelCollection stateModelCollection = entityFactory.CreateCollection<IStateModelCollection>();
                stateModelCollection.Add(stateModelObject);

                resource.AddStateModels(stateModelCollection);
                resource.LoadCurrentStates();

                current = resource.CurrentStates.FirstOrDefault(s => s.StateModel.Name == stateModelName);
            }

            if (String.IsNullOrWhiteSpace(reason))
            {
                resource.AdjustState(state, current.StateModel);
            }
            else
            {
                current.StateModel.Load();
                IStateModelTransition transition = current.StateModel.StateTransitions.FirstOrDefault(t => t.FromState.Name == current.CurrentState.Name && t.ToState.Name == state);
                resource.LogEvent(transition.Name, current.StateModel, reason);
            }

            //---End DEE Code---

            return Input;
        }
    }
}
