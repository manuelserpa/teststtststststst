using System;
using System.Collections.Generic;
using System.Text;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.AMSOsram.Actions.Resources
{
    public class CustomSendResourceNotification : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            // Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");
            // Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Navigo.Common.dll", "Cmf.Navigo.Common");

            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

            Resource resource = new Resource();


            if (!Input.ContainsKey(Cmf.Navigo.Common.Constants.Resource))
            {
                throw new ArgumentNullCmfException(Cmf.Navigo.Common.Constants.Resource);
            }
            else
            {
                resource.Name = Input[Cmf.Navigo.Common.Constants.Resource] as string;
                resource.Load();
            }

            if (!Input.ContainsKey("EquipmentTypeError"))
            {
                throw new ArgumentNullCmfException("EquipmentTypeError");
            }

            if (!Input.ContainsKey(AMSOsramConstants.smartTablePropertySeverity))
            {
                throw new ArgumentNullCmfException(AMSOsramConstants.smartTablePropertySeverity);
            }

            if (!Input.ContainsKey("Message"))
            {
                throw new ArgumentNullCmfException("Message");
            }

            List<KeyValuePair<string, object>> deeInput = new List<KeyValuePair<string, object>>();

            deeInput.Add(new KeyValuePair<string, object>(Cmf.Navigo.Common.Constants.Resource, resource.Name));
            deeInput.Add(new KeyValuePair<string, object>(AMSOsramConstants.smartTablePropertyNotificationTrigger, Input["EquipmentTypeError"] as string));
            deeInput.Add(new KeyValuePair<string, object>(AMSOsramConstants.smartTablePropertyNotificationBodyMessage, Input["Message"] as string));
            deeInput.Add(new KeyValuePair<string, object>("ActionGroupName", "IoTRequest"));

            Cmf.Foundation.Common.DynamicExecutionEngine.Action.ExecuteAction(
                "CustomResourceNotificationControlCenter",
                deeInput.ToArray()
                );

            //Cmf.Custom.AMSOsram.Common.AMSOsramUtilities.SendNotification(Input["Resource"].ToString(), Input["EquipmentTypeError"].ToString(), Input["Severity"].ToString(), Input["Message"].ToString(), notificationType);

            //---End DEE Code---

            return Input;
        }

        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     Dee action is triggered by IoT Automation to send a notification about a resource:
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
