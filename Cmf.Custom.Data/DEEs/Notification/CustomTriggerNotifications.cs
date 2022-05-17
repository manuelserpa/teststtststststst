using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.AMSOsram.Actions.Notification
{
    public class CustomTriggerNotifications : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.SmartTables");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.GenericServiceManagement");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");

            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");


            //Validate Title Message
            string notificationTitleMessage;
            if (Input.ContainsKey(AMSOsramConstants.smartTablePropertyNotificationTitleMessage))
            {
                notificationTitleMessage = Input[AMSOsramConstants.smartTablePropertyNotificationTitleMessage].ToString();
            }
            else
            {
                notificationTitleMessage = "Notification Triggered without Context";
            }

            //Validate Subject Message
            string notificationBodyMessage;
            if (Input.ContainsKey(AMSOsramConstants.smartTablePropertyNotificationBodyMessage))
            {
                notificationBodyMessage = Input[AMSOsramConstants.smartTablePropertyNotificationBodyMessage].ToString();
            }
            else
            {
                notificationBodyMessage = "";
            }

            string distributionList = null;
            Role notificationRole = null;
            if (Input.ContainsKey(AMSOsramConstants.smartTablePropertyTargetRole))
            {
                notificationRole = new Role() { Name = Input[AMSOsramConstants.smartTablePropertyTargetRole].ToString() };
                notificationRole.Load();
                distributionList = notificationRole.DistributionList;
            }
            else if (Input.ContainsKey(AMSOsramConstants.smartTablePropertyTargetDistributionList))
            {
                distributionList = Input[AMSOsramConstants.smartTablePropertyTargetDistributionList].ToString();
            }

            //Validate 

            if (Input[AMSOsramConstants.smartTablePropertyNotificationAction].ToString().Equals(AMSOsramConstants.smartTableResultActionNotification) && notificationRole != null)
            {
                Cmf.Navigo.BusinessObjects.Notification notification = new Cmf.Navigo.BusinessObjects.Notification()
                {
                    Severity = Input[AMSOsramConstants.smartTablePropertySeverity].ToString(),
                    Type = Input[AMSOsramConstants.smartTablePropertyNotificationType].ToString(),
                    AssignmentType = Navigo.BusinessObjects.AssignmentType.Role,
                    AssignedToRole = notificationRole
                };

                notification.Title = notificationTitleMessage;

                notification.Details = notificationBodyMessage;

                notification.Create();
            }
            else if (Input[AMSOsramConstants.smartTablePropertyNotificationAction].ToString().Equals(AMSOsramConstants.smartTableResultActionEmail) && !string.IsNullOrWhiteSpace(distributionList))
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
