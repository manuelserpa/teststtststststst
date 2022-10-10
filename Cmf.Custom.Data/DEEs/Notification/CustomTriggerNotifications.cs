using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Security;
using System;
using System.Collections.Generic;
using Cmf.Foundation.Security.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Navigo.BusinessObjects.Abstractions;

namespace Cmf.Custom.amsOSRAM.Actions.Notification
{
    public class CustomTriggerNotifications : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            // Foudation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.SmartTables");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            //Validate Title Message
            string notificationTitleMessage;
            if (Input.ContainsKey(amsOSRAMConstants.smartTablePropertyNotificationTitleMessage))
            {
                notificationTitleMessage = Input[amsOSRAMConstants.smartTablePropertyNotificationTitleMessage].ToString();
            }
            else
            {
                notificationTitleMessage = "Notification Triggered without Context";
            }

            //Validate Subject Message
            string notificationBodyMessage;
            if (Input.ContainsKey(amsOSRAMConstants.smartTablePropertyNotificationBodyMessage))
            {
                notificationBodyMessage = Input[amsOSRAMConstants.smartTablePropertyNotificationBodyMessage].ToString();
            }
            else
            {
                notificationBodyMessage = "";
            }

            string distributionList = null;
            IRole notificationRole = null;
            if (Input.ContainsKey(amsOSRAMConstants.smartTablePropertyTargetRole))
            {
                notificationRole = new Role() { Name = Input[amsOSRAMConstants.smartTablePropertyTargetRole].ToString() };
                notificationRole.Load();
                distributionList = notificationRole.DistributionList;
            }
            else if (Input.ContainsKey(amsOSRAMConstants.smartTablePropertyTargetDistributionList))
            {
                distributionList = Input[amsOSRAMConstants.smartTablePropertyTargetDistributionList].ToString();
            }

            //Validate 

            if (Input[amsOSRAMConstants.smartTablePropertyNotificationAction].ToString().Equals(amsOSRAMConstants.smartTableResultActionNotification) && notificationRole != null)
            {
                // Get services provider information
                IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
                IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

                INotification notification = entityFactory.Create<INotification>();
                notification.Severity = Input[amsOSRAMConstants.smartTablePropertySeverity].ToString();
                notification.Type = Input[amsOSRAMConstants.smartTablePropertyNotificationType].ToString();
                notification.AssignmentType = Navigo.BusinessObjects.AssignmentType.Role;
                notification.AssignedToRole = notificationRole;
                notification.Title = notificationTitleMessage;
                notification.Details = notificationBodyMessage;

                notification.Create();
            }
            else if (Input[amsOSRAMConstants.smartTablePropertyNotificationAction].ToString().Equals(amsOSRAMConstants.smartTableResultActionEmail) && !string.IsNullOrWhiteSpace(distributionList))
            {
                CmfMail mail = new CmfMail()
                {
                    Subject = notificationTitleMessage,
                    Message = notificationBodyMessage,
                    To = distributionList,
                    IsBodyHtml = true
                };
                try
                {
                    mail.SendTransactionalMail();
                }
                catch (Exception ex)
                {
                    Utilities.WriteLogError(ex.Message);
                }
            }

            //---End DEE Code---

            return Input;
        }

        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     Dee action that triggers a new notificatoin or email
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
