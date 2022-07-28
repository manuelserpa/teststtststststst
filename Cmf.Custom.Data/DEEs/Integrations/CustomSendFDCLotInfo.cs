using System;
using System.Collections.Generic;
using System.Text;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.OntoFDC;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;

namespace Cmf.Custom.AMSOsram.Actions.Integrations
{
    class CustomSendFDCLotInfo : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *    DEE action is triggered by Integration Entry Handler in order to process Integration Entries and send Lot Info to Onto FDC.
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

            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("", "System.Text");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.Extensions");
            UseReference("Cmf.Custom.OntoFDC.dll", "Cmf.Custom.OntoFDC");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");

            #region Input validation

            if (!Input.ContainsKey("IntegrationEntry"))
            {
                throw new ArgumentNullCmfException("IntegrationEntry");
            }

            #endregion
            bool fdcActive = false;
            bool fdcMandatory = false;
            string fdcServer = string.Empty;
            short? fdcPort = null;

            IntegrationEntry ie = Input["IntegrationEntry"] as IntegrationEntry;
            ie.LoadIntegrationMessage();

            string integrationMessage = System.Text.Encoding.UTF8.GetString(ie.IntegrationMessage.Message);

            // Load records from xml
            var fdcLotInfo = AMSOsramUtilities.DeserializeXmlToObject<FdcLotInfo>(integrationMessage);

            // Fetch FDC Active config
            if (Config.TryGetConfig(AMSOsramConstants.FDCConfigActivePath, out Config fdcActiveConfig) &&
                    fdcActiveConfig.GetConfigValue<bool>())
            {
                fdcActive = fdcActiveConfig.GetConfigValue<bool>();
            }

            //Fetch FDC Mandatory config
            if (Config.TryGetConfig(AMSOsramConstants.FDCConfigMandatoryPath, out Config fdcMandatoryConfig) &&
                    fdcMandatoryConfig.GetConfigValue<bool>())
            {
                fdcMandatory = fdcMandatoryConfig.GetConfigValue<bool>();
            }

            // Fetch FDC Server config
            if (Config.TryGetConfig(AMSOsramConstants.FDCConfigServerPath, out Config fdcServerConfig) &&
                    !string.IsNullOrWhiteSpace(fdcServerConfig.GetConfigValue<string>()))
            {
                fdcServer = fdcServerConfig.GetConfigValue<string>();
            }

            // Fetch FDC Port config
            if (Config.TryGetConfig(AMSOsramConstants.FDCConfigPortPath, out Config fdcPortConfig) &&
                    fdcPortConfig.GetConfigValue<short>() > 0)
            {
                fdcPort = fdcPortConfig.GetConfigValue<short>();
            }

            if (fdcActive && !string.IsNullOrWhiteSpace(fdcServer) && fdcPort != null)
            {                
                // Create the binding.
                //System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                //binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                //binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
                //binding.MaxReceivedMessageSize = 2147483647;
                //binding.MaxBufferPoolSize = 524288;
                //binding.MaxBufferSize = 2147483647;

                try
                {
                    FDC_API_Onto fdcApi = new FDC_API_Onto(fdcActive, fdcMandatory, fdcServer, (int)fdcPort);

                    if (ie.MessageType.Equals(AMSOsramConstants.MessageType_LOTIN))
                    {
                        //fdcApi.SendFdcLotStart();
                    }
                    else if (ie.MessageType.Equals(AMSOsramConstants.MessageType_LOTOUT))
                    {
                        //fdcApi.SendFdcLotEnd();
                    }
                    //System.ServiceModel.EndpointAddress endpoint = new System.ServiceModel.EndpointAddress(fdcActive);
                    //CreeERPWebService.MES_INBOUND_NEWClient client = new CreeERPWebService.MES_INBOUND_NEWClient(binding, endpoint);
                    //client.ClientCredentials.UserName.UserName = fdcMandatory;
                    //client.ClientCredentials.UserName.Password = fdcServer;

                    //var responseTask = client.MESAsync(requestTransactions);
                    //responseTask.Wait();
                }
                catch (Exception e)
                {
                    throw new CmfBaseException($"Error calling Onto FDC api { fdcServer }:{ fdcPort }", e);
                }
            }
            else
            {
                throw new CmfBaseException($"Could not send transaction { ie.MessageType } ({ ie.Name }) to Onto FDC due to " +
                    $"FDC be not active or missing configuration for FDC server or port at /AMSOsram/FDC/");
            }

            //---End DEE Code---
            return Input;
        }
    }
}
