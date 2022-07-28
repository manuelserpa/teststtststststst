using System;
using System.Collections.Generic;
using System.Text;
using Cmf.Custom.AMSOsram.Common.Extensions;
//using Cmf.Custom.OntoFDC;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;

namespace Cmf.Custom.AMSOsram.Actions.Integrations
{
    public class CustomFDCIntegrationMaterialHandler : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *    DEE action is triggered by Integration Entry Handler in order to process Integration Entries from MES to FDC.
             *    Transaction Types need to be defined in Smart Table: IntegrationHandlerResolution
             *  
             * Action Groups:
             *      None
            */

            #endregion

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code--- 
            //Foundation
            UseReference("", "Cmf.Foundation.BusinessObjects");
            // System
            UseReference("%MicrosoftNetPath%System.Private.Xml.dll", "System.Xml");
            UseReference("", "System.Globalization");
            UseReference("", "System.Text");
            UseReference("System.Private.ServiceModel.dll", "System.ServiceModel");
            UseReference("System.ServiceModel.Primitives.dll", "System.ServiceModel");
            // Cree
            UseReference("Cmf.Custom.Cree.Common.dll", "Cmf.Custom.Cree.Common");
            UseReference("Cmf.Custom.Cree.Common.dll", "Cmf.Custom.Cree.Common.Extensions");
            UseReference("Cmf.Custom.ERP.ExternalServices.dll", "CreeERPWebService");

            //#region Input validation

            //if (!Input.ContainsKey("IntegrationEntry"))
            //{
            //    throw new ArgumentNullCmfException("IntegrationEntry");
            //}

            //#endregion
            //string endpointAddress = string.Empty;
            //string username = string.Empty;
            //string password = string.Empty;
            //string distributionList = string.Empty;

            //IntegrationEntry ie = Input["IntegrationEntry"] as IntegrationEntry;
            //ie.LoadIntegrationMessage();

            //string integrationMessage = System.Text.Encoding.UTF8.GetString(ie.IntegrationMessage.Message);

            //// Load records from xml
            //var requestTransactions = integrationMessage.FromXml<FdcLotInfo>();

            ////Fetch endpoint
            //if (Config.TryGetConfig(CreeConstantsERP.ERPWebServiceEndpointConfigurationPath, out Config endpointAddressConfig) &&
            //        !string.IsNullOrWhiteSpace(endpointAddressConfig.GetConfigValue<string>()))
            //{
            //    endpointAddress = endpointAddressConfig.GetConfigValue<string>();
            //}

            ////Fetch credentials configs
            //if (Config.TryGetConfig(CreeConstantsERP.ERPCredentialsUsernameConfigurationPath, out Config usernameConfig) &&
            //        !string.IsNullOrWhiteSpace(usernameConfig.GetConfigValue<string>()))
            //{
            //    username = usernameConfig.GetConfigValue<string>();
            //}

            ////Fetch password
            //if (Config.TryGetConfig(CreeConstantsERP.ERPCredentialsPasswordConfigurationPath, out Config passwordConfig) &&
            //        !string.IsNullOrWhiteSpace(passwordConfig.GetConfigValue<string>()))
            //{
            //    password = passwordConfig.GetDecryptedConfigValue();
            //}

            ////Fetch distribution list to send notification in case there is an error
            //if (Config.TryGetConfig(CreeConstantsERP.ERPDistributionListConfigurationPath, out Config distributionListConfig) &&
            //        !string.IsNullOrWhiteSpace(distributionListConfig.GetConfigValue<string>()))
            //{
            //    distributionList = distributionListConfig.GetConfigValue<string>();
            //}

            //if (!string.IsNullOrWhiteSpace(endpointAddress) && !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            //{
            //    // Create the binding.
            //    System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
            //    binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            //    binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
            //    binding.MaxReceivedMessageSize = 2147483647;
            //    binding.MaxBufferPoolSize = 524288;
            //    binding.MaxBufferSize = 2147483647;

            //    try
            //    {
            //        System.ServiceModel.EndpointAddress endpoint = new System.ServiceModel.EndpointAddress(endpointAddress);
            //        CreeERPWebService.MES_INBOUND_NEWClient client = new CreeERPWebService.MES_INBOUND_NEWClient(binding, endpoint);
            //        client.ClientCredentials.UserName.UserName = username;
            //        client.ClientCredentials.UserName.Password = password;

            //        var responseTask = client.MESAsync(requestTransactions);
            //        responseTask.Wait();
            //    }
            //    catch (Exception e)
            //    {
            //        //If error is thrown send mail to distribution list with the error message
            //        if (!string.IsNullOrWhiteSpace(distributionList))
            //        {
            //            CmfMail mail = new CmfMail()
            //            {
            //                Subject = string.Format("Error calling webservice endpoint \"{0}\"", endpointAddress),
            //                Message = e.ToString(),
            //                To = distributionList,
            //                IsBodyHtml = true
            //            };

            //            try
            //            {
            //                mail.SendMail();
            //            }
            //            catch (Exception ex)
            //            {
            //                Utilities.WriteLogError(string.Format("Failed sending e-mail to \"{0}\" on CustomSendERPInboundTransaction:\n {1}", mail.To, ex.ToString()));
            //            }
            //        }
            //        throw new Exception(string.Format("Error calling webservice endpoint \"{0}\"", endpointAddress), e);
            //    }
            //}
            //else
            //{
            //    string errorMessage = $"Could not send transaction { ie.MessageType } ({ ie.Name }) to SAP due to missing configuration for WebServiceEndpoint or Credentials at /Cree/ERP/Inbound/";

            //    //If error is thrown send mail to distribution list with the error message
            //    if (!string.IsNullOrWhiteSpace(distributionList))
            //    {
            //        CmfMail mail = new CmfMail()
            //        {
            //            Subject = "Not able to process Integration Entry for SAP - missing configuration",
            //            Message = errorMessage,
            //            To = distributionList,
            //            IsBodyHtml = true
            //        };

            //        try
            //        {
            //            mail.SendMail();
            //        }
            //        catch (Exception ex)
            //        {
            //            Utilities.WriteLogError(string.Format("Failed sending e-mail to \"{0}\" on CustomSendERPInboundTransaction:\n {1}", mail.To, ex.ToString()));
            //        }
            //    }

            //    throw new Exception(errorMessage);
            //}

            //---End DEE Code---
            return Input;
        }
    }
}
