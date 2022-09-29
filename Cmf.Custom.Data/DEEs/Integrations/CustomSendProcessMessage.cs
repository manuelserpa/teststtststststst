using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.Common;
using System.Collections.Generic;
using Cmf.Foundation.BusinessObjects.Abstractions;

namespace Cmf.Custom.amsOSRAM.Actions.Integrations
{
    public class CustomSendProcessMessage : DeeDevBase
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

            IIntegrationEntry integrationEntry = amsOSRAMUtilities.GetInputItem<IIntegrationEntry>(Input, Constants.IntegrationEntry);

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

            //Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            IIntegrationEntry integrationEntry = amsOSRAMUtilities.GetInputItem<IIntegrationEntry>(Input, Constants.IntegrationEntry);

            // Send Goods Issue data throught amsOSRAM service

            //---End DEE Code---

            return Input;
        }
    }
}
