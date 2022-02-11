using Cmf.Custom.AMSOsram.Actions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cmf.Custom.AMSOsram.Actions.Automation
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
            // Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common");
            UseReference("", "Cmf.Foundation.BusinessObjects.Cultures");
            // Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            // Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

            if (!Input.ContainsKey("ResourceName"))
            {
                throw new ArgumentNullCmfException("ResourceName");
            }
            Resource resource = new Resource() { Name = Input["ResourceName"] as String };

            if (!Input.ContainsKey("StateName"))
            {
                throw new ArgumentNullCmfException("StateName");
            }
            String state = Input["StateName"] as String;
            if (!Input.ContainsKey("StateModelName"))
            {
                throw new ArgumentNullCmfException("StateModelName");
            }
            String stateModelName = Input["StateModelName"] as String;

            int? loadPortNumber = null;
            if (Input.ContainsKey("LoadPortNumber") && Input["LoadPortNumber"] != null && String.IsNullOrEmpty(Input["LoadPortNumber"].ToString()))
            {
                loadPortNumber = int.Parse(Input["LoadPortNumber"].ToString());
            }

            if (loadPortNumber != null && loadPortNumber > 0)
            {
                resource.Load();
                var resourceHierarchy = resource.GetDescendentResources(1);

                if (resourceHierarchy == null ||
                    resourceHierarchy.Count <= 0)
                {
                    throw new MissingMandatoryPropertyCmfException("SubResource", resource.Name); ;
                }

                resource = resourceHierarchy.FirstOrDefault(s => s.ChildResource.DisplayOrder != null && s.ChildResource.DisplayOrder.Value == loadPortNumber && s.ChildResource.ProcessingType == ProcessingType.LoadPort).ChildResource;
            }

            resource.Load();
            resource.LoadCurrentStates();
            CurrentEntityState current = null;
            if (resource.CurrentStates != null)
            {
                current = resource.CurrentStates.FirstOrDefault(s => s != null && s.StateModel.Name == stateModelName);
            }
            if (current == null)
            {
                var stateModelObject = new StateModel() { Name = stateModelName };
                stateModelObject.Load();
                resource.AddStateModels(new StateModelCollection() { stateModelObject });        
                resource.LoadCurrentStates();               
                current = resource.CurrentStates.FirstOrDefault(s => s.StateModel.Name == stateModelName);

            }
          
            resource.AdjustState(state, current.StateModel);

            //---End DEE Code---

            return Input;
        }
    }
}
