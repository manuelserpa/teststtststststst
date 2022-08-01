using System;
using System.Collections.Generic;
using System.Text;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.FDC;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;

namespace Cmf.Custom.AMSOsram.Actions.Integrations
{
    class CustomSendFDCWaferInfo : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *    DEE action is triggered by Integration Entry Handler in order to process Integration Entries and send Wafer Info to Onto FDC.
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

            UseReference("", "System.Globalization");
            UseReference("", "System.Text");
            UseReference("%MicrosoftNetPath%System.Private.Xml.dll", "System.Xml");
            UseReference("", "Cmf.Foundation.BusinessObjects");
            UseReference("", "Cmf.Foundation.Common.Exceptions");
            UseReference("", "Cmf.Foundation.Common");
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.Extensions");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.FDC");

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
            FDCWaferInfo fdcWaferInfo = AMSOsramUtilities.DeserializeXmlToObject<FDCWaferInfo>(integrationMessage);

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
                try
                {
                    //FDC_API_Onto fdcApi = new FDC_API_Onto(fdcActive, fdcMandatory, fdcServer, (int)fdcPort);

                    if (ie.MessageType.Equals(AMSOsramConstants.MessageType_WAFERIN))
                    {
                        //fdcApi.SendFdcWaferIn();
                    }
                    else if (ie.MessageType.Equals(AMSOsramConstants.MessageType_WAFEROUT))
                    {
                        //fdcApi.SendFdcWaferOut();
                    }
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
