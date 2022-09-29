using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects;
using System;
using System.Collections.Generic;
using System.Data;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.LocalizationService;

namespace Cmf.Custom.amsOSRAM.Actions.Notification
{
    public class CustomResourceNotificationControlCenter : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            // System
            UseReference("%MicrosoftNetPath%System.Private.CoreLib.dll", "System.Threading");
            UseReference("%MicrosoftNetPath%System.Data.Common.dll", "System.Data");
            
            // Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.SmartTables");
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common.LocalizationService");

            // Navigo
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects");
            
            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            const string triggerTypeSPCViolationValue = "SPCViolation";
            const string triggerTypeResourceStateChangeValue = "ResourceStateChange";

            string triggerType = "";
            IResource resource = null;
            string stateModel = "", fromState = "", toState = "";
            string contextMessageEquipment = "";
            string contextMessageSPC = "";
            string titleMessageSPC = "";
            string resourceName = "";
            IArea area = null;

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];

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
                    resource = Input[Navigo.Common.Constants.Resource] as IResource;
                    resourceName = resource.Name;
                    stateModel = (Input[Foundation.Common.Constants.StateModel] as IStateModel).Name;
                    fromState = resource.CurrentMainState.CurrentState.Name;
                    toState = Input[amsOSRAMConstants.smartTablePropertyStateName] as string;
                    triggerType = triggerTypeResourceStateChangeValue;
                }
                else if (actionGroup.Equals("IoTRequest"))
                {
                    resourceName = Input[Navigo.Common.Constants.Resource].ToString();
                    IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

                    resource = entityFactory.Create<IResource>();
                    resource.Name = resourceName;
                    triggerType = Input[amsOSRAMConstants.smartTablePropertyNotificationTrigger] as string;
                    contextMessageEquipment = Input[amsOSRAMConstants.smartTablePropertyNotificationBodyMessage] as string;
                }
                else if (actionGroup.Equals(triggerTypeSPCViolationValue))
                {
                    if (Input.ContainsKey(Navigo.Common.Constants.Resource))
                    {
                        resource = Input[Navigo.Common.Constants.Resource] as IResource;
                        if (resource != null)
                        {
                            area = resource.Area;
                            resourceName = resource.Name;
                        }
                    }
                    triggerType = triggerTypeSPCViolationValue;

                    if (Input.ContainsKey(amsOSRAMConstants.smartTablePropertyNotificationTitleMessage))
                    {
                        titleMessageSPC = Input[amsOSRAMConstants.smartTablePropertyNotificationTitleMessage] as string;
                    }

                    if (Input.ContainsKey(amsOSRAMConstants.smartTablePropertyNotificationBodyMessage))
                    {
                        contextMessageSPC = Input[amsOSRAMConstants.smartTablePropertyNotificationBodyMessage] as string;
                    }
                }
            }

            if (!String.IsNullOrEmpty(triggerType))
            {

                //Load SmartTable
                ISmartTable materialNotificationSmartTable = new SmartTable() { Name = amsOSRAMConstants.CustomResourceNotificationSTName };

                materialNotificationSmartTable.Load();
                materialNotificationSmartTable.LoadData();

                INgpDataRow resolveKeys = new NgpDataRow();
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
                    resolveKeys.Add(amsOSRAMConstants.smartTablePropertyFromState, fromState);
                }

                if (!string.IsNullOrEmpty(toState))
                {
                    resolveKeys.Add(amsOSRAMConstants.smartTablePropertyToState, toState);
                }

                resolveKeys.Add(amsOSRAMConstants.smartTablePropertyNotificationTrigger, triggerType);

                INgpDataSet resolvedData = materialNotificationSmartTable.Resolve(resolveKeys, false);

                //Get Action,Role
                DataSet dataset = NgpDataSet.ToDataSet(resolvedData);

                if (dataset != null && dataset.Tables != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rowResult in dataset.Tables[0].Rows)
                    {
                        if (rowResult[amsOSRAMConstants.smartTablePropertyIsEnable] != DBNull.Value && (bool)rowResult[amsOSRAMConstants.smartTablePropertyIsEnable])
                        {
                            string notificationAction = rowResult[amsOSRAMConstants.smartTablePropertyNotificationAction].ToString();
                            string severity = rowResult[amsOSRAMConstants.smartTablePropertySeverity].ToString();
                            string roleName = rowResult[amsOSRAMConstants.smartTablePropertyTargetRole].ToString();
                            string distribuitionList = rowResult[amsOSRAMConstants.smartTablePropertyTargetDistributionList].ToString();
                            string notificationTitleMessage = rowResult[amsOSRAMConstants.smartTablePropertyNotificationTitleMessage].ToString();
                            string notificationBodyMessage = rowResult[amsOSRAMConstants.smartTablePropertyNotificationBodyMessage].ToString();

                            List<KeyValuePair<string, object>> deeInput = new List<KeyValuePair<string, object>>();

                            if (!string.IsNullOrEmpty(roleName))
                            {
                                deeInput.Add(new KeyValuePair<string, object>(amsOSRAMConstants.smartTablePropertyTargetRole, roleName));
                            }
                            else if (!string.IsNullOrEmpty(distribuitionList))
                            {
                                deeInput.Add(new KeyValuePair<string, object>(amsOSRAMConstants.smartTablePropertyTargetDistributionList, distribuitionList));
                            }
                            if (!string.IsNullOrEmpty(roleName) || !string.IsNullOrEmpty(distribuitionList))
                            {

                                string defaultTitleMessage = Navigo.Common.Constants.Resource + ":" + resourceName + " - " + triggerType;
                                if (!string.IsNullOrEmpty(notificationTitleMessage))
                                {
                                    ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();
                                    string titleMessage = localizationService.Localize(notificationTitleMessage);
                                    deeInput.Add(new KeyValuePair<string, object>(amsOSRAMConstants.smartTablePropertyNotificationTitleMessage, titleMessage + "<br>" + defaultTitleMessage));
                                }
                                else if (!string.IsNullOrEmpty(titleMessageSPC))
                                {
                                    deeInput.Add(new KeyValuePair<string, object>(amsOSRAMConstants.smartTablePropertyNotificationTitleMessage, titleMessageSPC + "<br>" + defaultTitleMessage));
                                }
                                else if (string.IsNullOrEmpty(notificationTitleMessage))
                                {
                                    deeInput.Add(new KeyValuePair<string, object>(amsOSRAMConstants.smartTablePropertyNotificationTitleMessage, defaultTitleMessage));
                                }

                                System.Text.StringBuilder context = new System.Text.StringBuilder();
                                context.AppendFormat($"{Navigo.Common.Constants.Resource}: {resourceName}<br>");

                                if (!string.IsNullOrEmpty(stateModel))
                                {
                                    context.AppendFormat($"{Foundation.Common.Constants.StateModel}: {stateModel}<br>");
                                }

                                if (!string.IsNullOrEmpty(fromState))
                                {
                                    context.AppendFormat($"{amsOSRAMConstants.smartTablePropertyFromState}: {fromState}<br>");
                                }

                                if (!string.IsNullOrEmpty(toState))
                                {
                                    context.AppendFormat($"{amsOSRAMConstants.smartTablePropertyToState}: {toState}<br>");
                                }

                                if (!string.IsNullOrEmpty(contextMessageEquipment))
                                {
                                    context.AppendFormat($"{"Equipment Message"}: {contextMessageEquipment}<br>");
                                }

                                if (!string.IsNullOrEmpty(contextMessageSPC))
                                {
                                    context.AppendFormat($"{"Message"}: {contextMessageSPC}<br>");
                                }

                                context.AppendFormat($"{amsOSRAMConstants.smartTablePropertyNotificationTrigger}: {triggerType}<br>");
                                context.AppendFormat($"User: {Foundation.Common.Utilities.DomainUserName}");

                                if (!string.IsNullOrEmpty(notificationBodyMessage))
                                {
                                    ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();
                                    string bodyMessage = localizationService.Localize(notificationBodyMessage);
                                    deeInput.Add(new KeyValuePair<string, object>(amsOSRAMConstants.smartTablePropertyNotificationBodyMessage, bodyMessage + "<br>" + context.ToString()));
                                }
                                else
                                {
                                    deeInput.Add(new KeyValuePair<string, object>(amsOSRAMConstants.smartTablePropertyNotificationBodyMessage, context.ToString()));
                                }

                                deeInput.Add(new KeyValuePair<string, object>(amsOSRAMConstants.smartTablePropertyNotificationType, "Equipment"));
                                deeInput.Add(new KeyValuePair<string, object>(amsOSRAMConstants.smartTablePropertyNotificationAction, notificationAction));
                                deeInput.Add(new KeyValuePair<string, object>(amsOSRAMConstants.smartTablePropertyNotificationTrigger, triggerType));
                                deeInput.Add(new KeyValuePair<string, object>(amsOSRAMConstants.smartTablePropertySeverity, severity));

                                Foundation.Common.DynamicExecutionEngine.Action.ExecuteAction(
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
