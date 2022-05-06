using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace Cmf.Custom.AMSOsram.Actions.Notification
{
    public class CustomResourceNotificationControlCenter : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---
            UseReference("%MicrosoftNetPath%System.Private.CoreLib.dll", "System.Threading");
            UseReference("%MicrosoftNetPath%System.Data.Common.dll", "System.Data");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.SmartTables");
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");

            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.Cultures");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

            const string triggerTypeSPCViolationValue = "SPCViolation";
            const string triggerTypeResourceStateChangeValue = "ResourceStateChange";

            string triggerType = "";
            Resource resource = null;
            string stateModel = "", fromState = "", toState = "";
            string contextMessageEquipment = "";
            string contextMessageSPC = "";
            string titleMessageSPC = "";
            string resourceName = "";
            Area area = null;

            if (Input.ContainsKey("ActionGroupName"))
            {
                string actionGroup = Input["ActionGroupName"].ToString();

                if (Input.ContainsKey("LogResourceEventInput"))
                {
                    LogResourceEventInput logResourceEventInput = Input["LogResourceEventInput"] as LogResourceEventInput;
                    resource = logResourceEventInput.Resource;
                    resourceName = resource.Name;
                    stateModel = logResourceEventInput.StateModel.Name;
                    fromState = logResourceEventInput.StateModelTransition.FromState.Name;
                    toState = logResourceEventInput.StateModelTransition.ToState.Name;
                    triggerType = triggerTypeResourceStateChangeValue;
                }
                else if (actionGroup.Equals("BusinessObjects.Resource.AdjustState.Pre"))
                {
                    resource = Input[Navigo.Common.Constants.Resource] as Resource;
                    resourceName = resource.Name;
                    stateModel = (Input[Foundation.Common.Constants.StateModel] as StateModel).Name;
                    fromState = resource.CurrentMainState.CurrentState.Name;
                    toState = Input[AMSOsramConstants.smartTablePropertyStateName] as string;
                    triggerType = triggerTypeResourceStateChangeValue;
                }
                else if (actionGroup.Equals("IoTRequest"))
                {
                    resourceName = Input[Navigo.Common.Constants.Resource].ToString();
                    resource = new Resource() { Name = resourceName };
                    triggerType = Input[AMSOsramConstants.smartTablePropertyNotificationTrigger] as string;
                    contextMessageEquipment = Input[AMSOsramConstants.smartTablePropertyNotificationBodyMessage] as string;
                }
                else if (actionGroup.Equals(triggerTypeSPCViolationValue))
                {
                    if (Input.ContainsKey(Cmf.Navigo.Common.Constants.Resource))
                    {
                        resource = Input[Navigo.Common.Constants.Resource] as Resource;
                        if (resource != null)
                        {
                            area = resource.Area;
                            resourceName = resource.Name;
                        }
                    }
                    triggerType = triggerTypeSPCViolationValue;

                    if (Input.ContainsKey(AMSOsramConstants.smartTablePropertyNotificationTitleMessage))
                    {
                        titleMessageSPC = Input[AMSOsramConstants.smartTablePropertyNotificationTitleMessage] as string;
                    }

                    if (Input.ContainsKey(AMSOsramConstants.smartTablePropertyNotificationBodyMessage))
                    {
                        contextMessageSPC = Input[AMSOsramConstants.smartTablePropertyNotificationBodyMessage] as string;
                    }
                }
            }

            if (!String.IsNullOrEmpty(triggerType))
            {

                //Load SmartTable
                SmartTable materialNotificationSmartTable = new SmartTable() { Name = AMSOsramConstants.CustomResourceNotificationSTName };

                materialNotificationSmartTable.Load();
                materialNotificationSmartTable.LoadData();

                NgpDataRow resolveKeys = new NgpDataRow();
                if (resource != null && resource.ObjectExists())
                {
                    resolveKeys.Add(Navigo.Common.Constants.Resource, resource.Name);
                }

                if (area != null)
                {
                    resolveKeys.Add(Navigo.Common.Constants.Area, area.Name);
                }

                if (!string.IsNullOrEmpty(stateModel))
                {
                    resolveKeys.Add(Foundation.Common.Constants.StateModel, stateModel);
                }

                if (!string.IsNullOrEmpty(fromState))
                {
                    resolveKeys.Add(AMSOsramConstants.smartTablePropertyFromState, fromState);
                }

                if (!string.IsNullOrEmpty(toState))
                {
                    resolveKeys.Add(AMSOsramConstants.smartTablePropertyToState, toState);
                }

                resolveKeys.Add(AMSOsramConstants.smartTablePropertyNotificationTrigger, triggerType);

                NgpDataSet resolvedData = materialNotificationSmartTable.Resolve(resolveKeys, false);

                //Get Action,Role
                DataSet dataset = NgpDataSet.ToDataSet(resolvedData);

                if (dataset != null && dataset.Tables != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rowResult in dataset.Tables[0].Rows)
                    {
                        if (rowResult[AMSOsramConstants.smartTablePropertyIsEnable] != DBNull.Value && (bool)rowResult[AMSOsramConstants.smartTablePropertyIsEnable])
                        {
                            string notificationAction = rowResult[AMSOsramConstants.smartTablePropertyNotificationAction].ToString();
                            string severity = rowResult[AMSOsramConstants.smartTablePropertySeverity].ToString();
                            string roleName = rowResult[AMSOsramConstants.smartTablePropertyTargetRole].ToString();
                            string distribuitionList = rowResult[AMSOsramConstants.smartTablePropertyTargetDistributionList].ToString();
                            string notificationTitleMessage = rowResult[AMSOsramConstants.smartTablePropertyNotificationTitleMessage].ToString();
                            string notificationBodyMessage = rowResult[AMSOsramConstants.smartTablePropertyNotificationBodyMessage].ToString();

                            List<KeyValuePair<string, object>> deeInput = new List<KeyValuePair<string, object>>();

                            if (!string.IsNullOrEmpty(roleName))
                            {
                                deeInput.Add(new KeyValuePair<string, object>(AMSOsramConstants.smartTablePropertyTargetRole, roleName));
                            }
                            else if (!string.IsNullOrEmpty(distribuitionList))
                            {
                                deeInput.Add(new KeyValuePair<string, object>(AMSOsramConstants.smartTablePropertyTargetDistributionList, distribuitionList));
                            }
                            if (!string.IsNullOrEmpty(roleName) || !string.IsNullOrEmpty(distribuitionList))
                            {

                                string defaultTitleMessage = Cmf.Navigo.Common.Constants.Resource + ":" + resourceName + " - " + triggerType;
                                if (!string.IsNullOrEmpty(notificationTitleMessage))
                                {
                                    LocalizedMessage titleMessage = LocalizedMessage.GetLocalizedMessage(notificationTitleMessage);
                                    deeInput.Add(new KeyValuePair<string, object>(AMSOsramConstants.smartTablePropertyNotificationTitleMessage, titleMessage.MessageText + "<br>" + defaultTitleMessage));
                                }
                                else if (!string.IsNullOrEmpty(titleMessageSPC))
                                {
                                    deeInput.Add(new KeyValuePair<string, object>(AMSOsramConstants.smartTablePropertyNotificationTitleMessage, titleMessageSPC + "<br>" + defaultTitleMessage));
                                }
                                else if (string.IsNullOrEmpty(notificationTitleMessage))
                                {
                                    deeInput.Add(new KeyValuePair<string, object>(AMSOsramConstants.smartTablePropertyNotificationTitleMessage, defaultTitleMessage));
                                }

                                var context = new System.Text.StringBuilder();
                                context.AppendFormat($"{Navigo.Common.Constants.Resource}: {resourceName}<br>");

                                if (!string.IsNullOrEmpty(stateModel))
                                {
                                    context.AppendFormat($"{Foundation.Common.Constants.StateModel}: {stateModel}<br>");
                                }

                                if (!string.IsNullOrEmpty(fromState))
                                {
                                    context.AppendFormat($"{AMSOsramConstants.smartTablePropertyFromState}: {fromState}<br>");
                                }

                                if (!string.IsNullOrEmpty(toState))
                                {
                                    context.AppendFormat($"{AMSOsramConstants.smartTablePropertyToState}: {toState}<br>");
                                }

                                if (!string.IsNullOrEmpty(contextMessageEquipment))
                                {
                                    context.AppendFormat($"{"Equipment Message"}: {contextMessageEquipment}<br>");
                                }

                                if (!string.IsNullOrEmpty(contextMessageSPC))
                                {
                                    context.AppendFormat($"{"Message"}: {contextMessageSPC}<br>");
                                }

                                context.AppendFormat($"{AMSOsramConstants.smartTablePropertyNotificationTrigger}: {triggerType}<br>");
                                context.AppendFormat($"User: {Foundation.Common.Utilities.DomainUserName}");

                                if (!string.IsNullOrEmpty(notificationBodyMessage))
                                {
                                    var bodyMessage = LocalizedMessage.GetLocalizedMessage(notificationBodyMessage);
                                    deeInput.Add(new KeyValuePair<string, object>(AMSOsramConstants.smartTablePropertyNotificationBodyMessage, bodyMessage.MessageText + "<br>" + context.ToString()));
                                }
                                else
                                {
                                    deeInput.Add(new KeyValuePair<string, object>(AMSOsramConstants.smartTablePropertyNotificationBodyMessage, context.ToString()));
                                }

                                deeInput.Add(new KeyValuePair<string, object>(AMSOsramConstants.smartTablePropertyNotificationType, "Equipment"));
                                deeInput.Add(new KeyValuePair<string, object>(AMSOsramConstants.smartTablePropertyNotificationAction, notificationAction));
                                deeInput.Add(new KeyValuePair<string, object>(AMSOsramConstants.smartTablePropertyNotificationTrigger, triggerType));
                                deeInput.Add(new KeyValuePair<string, object>(AMSOsramConstants.smartTablePropertySeverity, severity));

                                Cmf.Foundation.Common.DynamicExecutionEngine.Action.ExecuteAction(
                                    "CustomTriggerNotifications",
                                    deeInput.ToArray()

                                    );
                            }
                        }
                    }
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
             *     Dee action to trigger notifications for control center
             *  
             * Action Groups:
             *    BusinessObjects.Resource.AdjustState.Pre
             *    ResourceManagement.ResourceManagementOrchestration.LogResourceEvent.Post
             *     
            */

            #endregion

            return true;

            //---End DEE Condition Code---
        }
    }
}
