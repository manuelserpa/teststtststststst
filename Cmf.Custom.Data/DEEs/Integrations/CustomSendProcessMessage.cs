using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.Common;
using System.Collections.Generic;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.Configuration;
using Cmf.Foundation.Configuration.Abstractions;
using Cmf.Foundation.BusinessObjects;
using System;

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

            //Foundation
            UseReference("", "Cmf.Foundation.BusinessObjects");
            //System
            UseReference("System.Private.ServiceModel.dll", "System.ServiceModel");
            UseReference("System.ServiceModel.Primitives.dll", "System.ServiceModel");
            UseReference("%MicrosoftNetPath%System.Private.Xml.dll", "System.Xml");
            UseReference("", "System.Text");
            //Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.ERP.ExternalServices.dll", "ERPWebService");

            System.ServiceModel.EndpointAddress endpointAddress = null;
            string username = null;
            string password = null;
            string distributionList = null;

            IIntegrationEntry integrationEntry = amsOSRAMUtilities.GetInputItem<IIntegrationEntry>(Input, Constants.IntegrationEntry);
            integrationEntry.LoadIntegrationMessage();

            string integrationMessage = System.Text.Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            ERPWebService.GoodsIssueRow[] goodsIssueRow = amsOSRAMUtilities.DeserializeXmlToObject<ERPWebService.GoodsIssueRow[]>(integrationMessage);

            // Fetch endpoint address
            if (Config.TryGetConfig(amsOSRAMConstants.ERPWebServiceEndpointConfigurationPath, out IConfig endpointAddressConfig) &&
                    !string.IsNullOrWhiteSpace(endpointAddressConfig.GetConfigValue<string>()))
            {
                endpointAddress = endpointAddressConfig.GetConfigValue<System.ServiceModel.EndpointAddress>();
            }

            // Fetch username
            if(Config.TryGetConfig(amsOSRAMConstants.ERPCredentialsUsernameConfigurationPath, out IConfig usernameConfig) &&
                !string.IsNullOrWhiteSpace(usernameConfig.GetConfigValue<string>()))
            {
                username = usernameConfig.GetConfigValue<string>();
            }

            // Fetch password
            if(Config.TryGetConfig(amsOSRAMConstants.ERPCredentialsPasswordConfigurationPath, out IConfig passwordConfig) &&
                !string.IsNullOrWhiteSpace(passwordConfig.GetConfigValue<string>()))
            {
                password = passwordConfig.GetDecryptedConfigValue();
            }
            
            // Fetch distribution list to send notifications to in case of an error
            if(Config.TryGetConfig(amsOSRAMConstants.ERPWebServiceDistributionListConfigurationPath, out IConfig distributionListConfig) &&
                !string.IsNullOrWhiteSpace(distributionListConfig.GetConfigValue<string>()))
            {
                distributionList = distributionListConfig.GetConfigValue<string>();
            }

            if(!string.IsNullOrWhiteSpace(endpointAddress.ToString()) && string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxBufferSize = int.MaxValue;
                binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.AllowCookies = true;
                try
                {
                    ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTClient client = new ERPWebService.MI_OSRBG_PP_SFC_SAPBOOKINGS_GOODSISSUE_OUTClient(binding, endpointAddress);
                    client.ClientCredentials.UserName.UserName = username;
                    client.ClientCredentials.UserName.Password = password;
                    var responseTask = client.MI_OSRBG_PP_SFC_SAPBOOKINGS_GoodsIssue_OUTAsync(goodsIssueRow);
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
                        catch(Exception ex)
                        {
                            Utilities.WriteLogError(string.Format("Failed sending e-mail to \"{0}\" on CustomSendProcessMessage:\n {1}", distributionList, ex.Message));
                        }
                    }
                    throw new Exception(string.Format("Failed calling ERP web service endpoint \"{0}\"", endpointAddress), e);
                }
            }
            else
            {
                string errorMessage = $"Could not send transaction {integrationEntry.MessageType} ({integrationEntry.Name}) To SAP due to missing configuration for WebServiceEndpoint or Credentials at /Cmf/Custom/ERP/";

                if (!string.IsNullOrEmpty(distributionList))
                {
                    CmfMail cmfMail = new CmfMail()
                    {
                        Subject = "Not able to process Integration Entry for SAP - missing Configuration",
                        Message = errorMessage,
                        To = distributionList,
                        IsBodyHtml= true
                    };

                    try
                    {
                        cmfMail.SendMail();
                    }
                    catch(Exception ex)
                    {
                        Utilities.WriteLogError(string.Format("Failed sending e-mail to \"{0}\" on CustomSendProcessMessage:\n {1}", distributionList, ex.Message));
                    }
                }

                throw new Exception(errorMessage);
            }
            // Send Goods Issue data throught amsOSRAM service

            //---End DEE Code---

            return Input;
        }
    }
}
