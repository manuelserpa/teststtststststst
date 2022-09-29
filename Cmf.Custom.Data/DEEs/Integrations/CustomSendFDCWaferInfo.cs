using System;
using System.Collections.Generic;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.FDC;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using Cmf.Foundation.Configuration.Abstractions;
using Cmf.Foundation.BusinessObjects.Abstractions;

namespace Cmf.Custom.amsOSRAM.Actions.Integrations
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

            // System
            UseReference("%MicrosoftNetPath%System.Private.Xml.dll", "System.Xml");

            // Navigo
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.Extensions");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.FDC");

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

            IIntegrationEntry ie = Input["IntegrationEntry"] as IIntegrationEntry;
            ie.LoadIntegrationMessage();

            string integrationMessage = System.Text.Encoding.UTF8.GetString(ie.IntegrationMessage.Message);

            // Load records from xml
            FDCWaferInfo fdcWaferInfo = amsOSRAMUtilities.DeserializeXmlToObject<FDCWaferInfo>(integrationMessage);

            // Fetch FDC Active config
            if (Config.TryGetConfig(amsOSRAMConstants.FDCConfigActivePath, out IConfig fdcActiveConfig) &&
                    fdcActiveConfig.GetConfigValue<bool>())
            {
                fdcActive = fdcActiveConfig.GetConfigValue<bool>();
            }

            //Fetch FDC Mandatory config
            if (Config.TryGetConfig(amsOSRAMConstants.FDCConfigMandatoryPath, out IConfig fdcMandatoryConfig) &&
                    fdcMandatoryConfig.GetConfigValue<bool>())
            {
                fdcMandatory = fdcMandatoryConfig.GetConfigValue<bool>();
            }

            // Fetch FDC Server config
            if (Config.TryGetConfig(amsOSRAMConstants.FDCConfigServerPath, out IConfig fdcServerConfig) &&
                    !string.IsNullOrWhiteSpace(fdcServerConfig.GetConfigValue<string>()))
            {
                fdcServer = fdcServerConfig.GetConfigValue<string>();
            }

            // Fetch FDC Port config
            if (Config.TryGetConfig(amsOSRAMConstants.FDCConfigPortPath, out IConfig fdcPortConfig) &&
                    fdcPortConfig.GetConfigValue<short>() > 0)
            {
                fdcPort = fdcPortConfig.GetConfigValue<short>();
            }

            if (fdcActive && !string.IsNullOrWhiteSpace(fdcServer) && fdcPort != null)
            {
                try
                {
                    //FDC_API_Onto fdcApi = new FDC_API_Onto(fdcActive, fdcMandatory, fdcServer, (int)fdcPort);

                    if (ie.MessageType.Equals(amsOSRAMConstants.MessageType_WAFERIN))
                    {
                        //fdcApi.SendFdcWaferIn();
                    }
                    else if (ie.MessageType.Equals(amsOSRAMConstants.MessageType_WAFEROUT))
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
                    $"FDC be not active or missing configuration for FDC server or port at /amsOSRAM/FDC/");
            }

            //---End DEE Code---
            return Input;
        }
    }
}
