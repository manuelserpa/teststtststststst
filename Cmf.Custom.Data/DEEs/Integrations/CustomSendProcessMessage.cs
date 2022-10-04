using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.Common;
using System.Collections.Generic;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.Configuration;
using Cmf.Foundation.Configuration.Abstractions;

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

            //Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            string endpointAddress = null;
            string username = null;
            string password = null;

            IIntegrationEntry integrationEntry = amsOSRAMUtilities.GetInputItem<IIntegrationEntry>(Input, Constants.IntegrationEntry);
            integrationEntry.LoadIntegrationMessage();

            string integrationMessage = System.Text.Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            // Fetch endpoint address
            if (Config.TryGetConfig(amsOSRAMConstants.ERPWebServiceEndpointConfigurationPath, out IConfig endpointAddressConfig) &&
                    !string.IsNullOrWhiteSpace(endpointAddressConfig.GetConfigValue<string>()))
            {
                endpointAddress = endpointAddressConfig.GetConfigValue<string>();
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
            
            if(!string.IsNullOrWhiteSpace(endpointAddress) && string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
                binding.MaxReceivedMessageSize = 2147483647;
                binding.MaxBufferPoolSize = 524288;
                binding.MaxBufferSize = 2147483647;

                try
                {
                    System.ServiceModel.EndpointAddress endpoint = new System.ServiceModel.EndpointAddress(endpointAddress);
                }
                finally
                {

                }
            }
            // Send Goods Issue data throught amsOSRAM service

            //---End DEE Code---

            return Input;
        }
    }
}
