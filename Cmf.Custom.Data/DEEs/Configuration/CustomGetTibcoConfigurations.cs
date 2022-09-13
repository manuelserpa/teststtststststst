using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using System.Collections.Generic;
using Cmf.Foundation.Configuration.Abstractions;

namespace Cmf.Custom.amsOSRAM.Actions.Configurations
{
    public class CustomGetTibcoConfigurations : DeeDevBase
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

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //Common
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            // Get IsEnabled config value
            Config.TryGetConfig(amsOSRAMConstants.TibcoConfigIsEnabledPath, out IConfig isEnabledConfig);

            if (isEnabledConfig == null || isEnabledConfig.Value == null || string.IsNullOrWhiteSpace(isEnabledConfig.Value.ToString()))
            {
                throw new CmfBaseException("Missing IsEnabled configuration");
            }

            // Get Host config value
            Config.TryGetConfig(amsOSRAMConstants.TibcoConfigHostPath, out IConfig hostConfig);

            if (hostConfig == null || hostConfig.Value == null || string.IsNullOrWhiteSpace(hostConfig.Value.ToString()))
            {
                throw new CmfBaseException("Missing Host configuration");
            }

            // Get Username config value
            Config.TryGetConfig(amsOSRAMConstants.TibcoConfigUsernamePath, out IConfig usernameConfig);

            if (usernameConfig == null || usernameConfig.Value == null || string.IsNullOrWhiteSpace(usernameConfig.Value.ToString()))
            {
                throw new CmfBaseException("Missing Username configuration");
            }

            // Get Password config value
            Config.TryGetConfig(amsOSRAMConstants.TibcoConfigPasswordPath, out IConfig passwordConfig);

            if (passwordConfig == null || passwordConfig.Value == null || string.IsNullOrWhiteSpace(passwordConfig.Value.ToString()))
            {
                throw new CmfBaseException("Missing Password configuration");
            }

            return new Dictionary<string, object>()
            {
                { "IsEnabled", isEnabledConfig.Value.ToString() },
                { "Host", hostConfig.Value.ToString() },
                { "Username", usernameConfig.Value.ToString() },
                { "Password",  passwordConfig.GetDecryptedConfigValue() }
            };

            //---End DEE Code---
        }
    }
}
