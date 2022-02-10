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
    class CustomAutomationSetMaterialState : DeeDevBase
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


            if (!Input.ContainsKey("MaterialName"))
            {
                throw new ArgumentNullCmfException("MaterialName");
            }
            Material material = new Material() { Name = Input["MaterialName"] as String };

            if (!Input.ContainsKey("StateName"))
            {
                throw new ArgumentNullCmfException("StateName");
            }
            String state = Input["StateName"] as String;

            material.Load();

            Console.WriteLine(string.Format("{0} > MaterialNAme {1} > State {2} > UniversalState {3}", DateTime.Now.ToString("HHmmssfff"), material.Name, state, material.UniversalState.ToString()));
            material.AdjustState(state, material.CurrentMainState.StateModel);



            //---End DEE Code---

            return Input;
        }
    }
}
