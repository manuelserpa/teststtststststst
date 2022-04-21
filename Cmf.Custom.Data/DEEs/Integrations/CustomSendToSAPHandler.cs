using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using System.Collections.Generic;

namespace Cmf.Custom.AMSOsram.Actions.Integrations
{
    public class CustomSendToSAPHandler : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     
            /// Action Groups:
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, Constants.IntegrationEntry);

            if (integrationEntry is null || integrationEntry.IntegrationMessage is null || integrationEntry.IntegrationMessage.Message is null || integrationEntry.IntegrationMessage.Message.Length <= 0)
            {
                return false;
            }

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //System
            UseReference("", "System.Linq");
            UseReference("", "System.Collections.Generic");
            UseReference("", "System.IO");
            UseReference("", "System.Threading");
            UseReference("", "System");

            //Foundation
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            //Custom

            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, Constants.IntegrationEntry);

            // Send ProductionOrder data throught AMSOsram service

            //---End DEE Code---

            return Input;
        }
    }
}
