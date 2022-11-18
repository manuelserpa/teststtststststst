using Cmf.Navigo.BusinessObjects.Abstractions;
using System;
using System.Collections.Generic;

namespace Cmf.Custom.amsOSRAM.Actions.SortRules
{
    internal class CustomMaterialPossibleStartDate : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     DEE Action to sort material by PossibleStartDate
             *
            */

            #endregion Info

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            UseReference("", "Cmf.Foundation.Common.Exceptions");

            if (!Input.ContainsKey("Material") || !(Input["Material"] is IMaterial))
            {
                throw new ArgumentNullException("Material");
            }

            IMaterial material = (IMaterial)Input["Material"];

            Input.Add("Result", material.PossibleStartDate);

            //---End DEE Code---

            return Input;
        }
    }
}
