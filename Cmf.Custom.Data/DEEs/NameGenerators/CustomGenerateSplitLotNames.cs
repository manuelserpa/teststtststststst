using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using System.Collections.Generic;

namespace Cmf.Custom.AMSOsram.Actions.NameGenerators
{
    public class CustomGenerateSplitLotNames : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     - DEE used by Name Generator CustomGenerateSplitLotNames to generate names for splited Materials
            /// Depends On:
            ///     - ResolveNameGenerator
            /// </summary>
            #endregion

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            // System
            UseReference("", "System.Linq");
            UseReference("", "System.Collections.Generic");
            UseReference("", "System.Text");

            // Foundation
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");

            // Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            // Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

            // Load Name Generator
            NameGenerator nameGenerator = new NameGenerator();
            nameGenerator.Load(AMSOsramConstants.CustomGenerateSplitLotNames);

            Material materialLot = new Material();

            if (Input.ContainsKey("EntitySource"))
            {
                materialLot = Input["EntitySource"] as Material;
            }

            // Set Material name on first position of Name Generator Array
            string splitedMaterialName = materialLot.Name.Split('.')[0];

            Input.Add("Result", splitedMaterialName);

            //---End DEE Code---

            return Input;
        }
    }
}
