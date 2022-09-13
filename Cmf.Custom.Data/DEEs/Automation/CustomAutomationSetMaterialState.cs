using Cmf.Foundation.Common;
using System;
using System.Collections.Generic;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.Abstractions;

namespace Cmf.Custom.amsOSRAM.Actions.Automation
{
    class CustomAutomationSetMaterialState : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---
            
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            
            if (!Input.ContainsKey("MaterialName"))
            {
                throw new ArgumentNullCmfException("MaterialName");
            }

            if (!Input.ContainsKey("StateName"))
            {
                throw new ArgumentNullCmfException("StateName");
            }

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IMaterial material = entityFactory.Create<IMaterial>();
            material.Name = Input["MaterialName"] as string;

            string state = Input["StateName"] as string;

            material.Load();

            material.AdjustState(state, material.CurrentMainState.StateModel);

            //---End DEE Code---

            return Input;
        }

        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     Dee action is triggered by IoT Automation to adjust the state of a Material 
             *  
             * Action Groups:
             *      None
             *     
            */

            #endregion

            return true;

            //---End DEE Condition Code---
        }
    }
}
