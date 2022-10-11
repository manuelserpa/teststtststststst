using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.ERP;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using Cmf.Foundation.Configuration.Abstractions;
using System;
using System.Collections.Generic;

namespace Cmf.Custom.amsOSRAM.Actions.Integrations
{
    public class CustomSendProcessMessage : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            ///
            /// DEE action is triggered by Integration Entry Handler in order to process Integration Entries.
            /// Transaction Types need to be defined in Smart Table: IntegrationHandlerResolution
            /// 
            /// </summary>
            #endregion

            IIntegrationEntry integrationEntry = amsOSRAMUtilities.GetInputItem<IIntegrationEntry>(Input, Constants.IntegrationEntry);

            return (integrationEntry != null &&
                integrationEntry.IntegrationMessage != null &&
                integrationEntry.IntegrationMessage.Message != null &&
                integrationEntry.IntegrationMessage.Message.Length > 0);

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //Foundation
            UseReference("", "Cmf.Foundation.BusinessObjects");
            //System
            UseReference("System.Private.ServiceModel.dll", "System.ServiceModel");
            UseReference("System.ServiceModel.Primitives.dll", "System.ServiceModel");
            UseReference("%MicrosoftNetPath%System.Private.Xml.dll", "System.Xml");
            UseReference("", "System.Text");
            //Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.ERP");
            UseReference("Cmf.Custom.ERP.ExternalServices.dll", "ERPWebService");

            System.ServiceModel.EndpointAddress endpointAddress = null;
            string username = null;
            string password = null;
            string distributionList = null;

            IIntegrationEntry integrationEntry = amsOSRAMUtilities.GetInputItem<IIntegrationEntry>(Input, Constants.IntegrationEntry);
            integrationEntry.LoadIntegrationMessage();

            string integrationMessage = System.Text.Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            CustomReportToERPItem customReportToERPItem = amsOSRAMUtilities.DeserializeXmlToObject<CustomReportToERPItem>(integrationMessage);

            ERPWebService.GoodsIssueRow goodsIssueRow = new ERPWebService.GoodsIssueRow
            {
                id = customReportToERPItem.Id,
                Batch = customReportToERPItem.BatchName,
                CostCenter = customReportToERPItem.CostCenter,
                LotNumber = customReportToERPItem.MaterialName,
                ProductionOrderNr = customReportToERPItem.ProductionOrderNumber,
                Quantity = customReportToERPItem.Quantity.ToString(),
                QuantityUnit = customReportToERPItem.Units,
                MovementType = customReportToERPItem.MovementType,
                Site = customReportToERPItem.Site,
                SapStore = customReportToERPItem.SAPStore,
                SapToStore = customReportToERPItem.SAPToStore,
                MatCalYear = customReportToERPItem.MatCalYear,
                MaterialNr = customReportToERPItem.ProductName,
                MatRecNr = customReportToERPItem.MatRecNr
            };

            ERPWebService.GoodsIssueRow[] goodsIssueRows = new ERPWebService.GoodsIssueRow[]
            {
                goodsIssueRow
            };

            // Fetch endpoint address
            if (Config.TryGetConfig(amsOSRAMConstants.ERPWebServiceEndpointConfigurationPath, out IConfig endpointAddressConfig) &&
                    !string.IsNullOrWhiteSpace(endpointAddressConfig.GetConfigValue<string>()))
            {
                endpointAddress = new System.ServiceModel.EndpointAddress(endpointAddressConfig.GetConfigValue<string>());
            }

            // Fetch username
            if (Config.TryGetConfig(amsOSRAMConstants.ERPCredentialsUsernameConfigurationPath, out IConfig usernameConfig) &&
                !string.IsNullOrWhiteSpace(usernameConfig.GetConfigValue<string>()))
            {
                username = usernameConfig.GetConfigValue<string>();
            }

            // Fetch password
            if (Config.TryGetConfig(amsOSRAMConstants.ERPCredentialsPasswordConfigurationPath, out IConfig passwordConfig) &&
                !string.IsNullOrWhiteSpace(passwordConfig.GetConfigValue<string>()))
            {
                password = passwordConfig.GetDecryptedConfigValue();
            }

            // Fetch distribution list to send notifications to in case of an error
            if (Config.TryGetConfig(amsOSRAMConstants.ERPWebServiceDistributionListConfigurationPath, out IConfig distributionListConfig) &&
                !string.IsNullOrWhiteSpace(distributionListConfig.GetConfigValue<string>()))
            {
                distributionList = distributionListConfig.GetConfigValue<string>();
            }

            if (!string.IsNullOrWhiteSpace(endpointAddress.ToString()) && !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxBufferSize = int.MaxValue;
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.AllowCookies = true;
                try
                {
                    ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTClient client = new ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTClient(binding, endpointAddress);
                    client.ClientCredentials.UserName.UserName = username;
                    client.ClientCredentials.UserName.Password = password;
                    var responseTask = client.MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTAsync(goodsIssueRows);
                    responseTask.Wait();
                }
                catch (Exception e)
                {
                    if (!string.IsNullOrEmpty(distributionList))
                    {
                        CmfMail cmfMail = new CmfMail()
                        {
                            Subject = String.Format("Error calling the ERP web service endpoint \"{0}\"", endpointAddress),
                            Message = e.Message,
                            To = distributionList,
                            IsBodyHtml = true
                        };

                        try
                        {
                            cmfMail.SendMail();
                        }
                        catch (Exception ex)
                        {
                            Utilities.WriteLogError(string.Format("Failed sending e-mail to \"{0}\" on CustomSendProcessMessage:\n {1}", distributionList, ex.Message));
                        }
                    }
                    throw new Exception(string.Format("Failed calling ERP web service endpoint \"{0}\"", endpointAddress), e);
                }
            }
            else
            {
                string errorMessage = $"Could not send transaction {integrationEntry.MessageType} ({integrationEntry.Name}) to SAP due to missing configuration for WebServiceEndpoint or Credentials at /amsOSRAM/ERP/";

                if (!string.IsNullOrEmpty(distributionList))
                {
                    CmfMail cmfMail = new CmfMail()
                    {
                        Subject = "Not able to process Integration Entry for SAP - missing Configuration",
                        Message = errorMessage,
                        To = distributionList,
                        IsBodyHtml = true
                    };

                    try
                    {
                        cmfMail.SendMail();
                    }
                    catch (Exception ex)
                    {
                        Utilities.WriteLogError(string.Format("Failed sending e-mail to \"{0}\" on CustomSendProcessMessage:\n {1}", distributionList, ex.Message));
                    }
                }

                throw new Exception(errorMessage);
            }

            //---End DEE Code---

            return Input;
        }
    }
}
