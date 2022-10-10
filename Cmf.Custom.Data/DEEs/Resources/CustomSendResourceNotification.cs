using System;
using System.Collections.Generic;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Cmf.Custom.amsOSRAM.Actions.Resources
{
    public class CustomSendResourceNotification : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            if (!Input.ContainsKey(Navigo.Common.Constants.Resource))
            {
                throw new ArgumentNullCmfException(Navigo.Common.Constants.Resource);
            }

            if (!Input.ContainsKey("EquipmentTypeError"))
            {
                throw new ArgumentNullCmfException("EquipmentTypeError");
            }

            if (!Input.ContainsKey(amsOSRAMConstants.smartTablePropertySeverity))
            {
                throw new ArgumentNullCmfException(amsOSRAMConstants.smartTablePropertySeverity);
            }

            if (!Input.ContainsKey("Message"))
            {
                throw new ArgumentNullCmfException("Message");
            }

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IResource resource = entityFactory.Create<IResource>();
            resource.Name = Input[Navigo.Common.Constants.Resource] as string;
            resource.Load();

            List<KeyValuePair<string, object>> deeInput = new List<KeyValuePair<string, object>>();

            deeInput.Add(new KeyValuePair<string, object>(Navigo.Common.Constants.Resource, resource.Name));
            deeInput.Add(new KeyValuePair<string, object>(amsOSRAMConstants.smartTablePropertyNotificationTrigger, Input["EquipmentTypeError"] as string));
            deeInput.Add(new KeyValuePair<string, object>(amsOSRAMConstants.smartTablePropertyNotificationBodyMessage, Input["Message"] as string));
            deeInput.Add(new KeyValuePair<string, object>("ActionGroupName", "IoTRequest"));

            Foundation.Common.DynamicExecutionEngine.Action.ExecuteAction(
                "CustomResourceNotificationControlCenter",
                deeInput.ToArray()
                );

            //Cmf.Custom.amsOSRAM.Common.amsOSRAMUtilities.SendNotification(Input["Resource"].ToString(), Input["EquipmentTypeError"].ToString(), Input["Severity"].ToString(), Input["Message"].ToString(), notificationType);

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
