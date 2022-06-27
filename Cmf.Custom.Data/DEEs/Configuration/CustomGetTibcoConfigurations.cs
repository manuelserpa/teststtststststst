using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using System;
using System.Collections.Generic;

namespace Cmf.Custom.AMSOsram.Actions.Configurations
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

            //System
            UseReference("", "System.Linq");
            UseReference("", "System.Collections.Generic");
            UseReference("", "System.IO");
            UseReference("", "System.Threading");
            UseReference("", "System");

            //Foundation
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            //Custom

            // Get IsEnabled config value
            string configIsEnabledPath = "/AMSOsram/TibcoEMS/IsEnabled";
            Config.TryGetConfig(configIsEnabledPath, out Config isEnabledConfig);

            if (isEnabledConfig == null || isEnabledConfig.Value == null || string.IsNullOrWhiteSpace(isEnabledConfig.Value.ToString()))
            {
                throw new CmfBaseException("Missing IsEnabled configuration");
            }

            // Get Host config value
            string configHostPath = "/AMSOsram/TibcoEMS/Host";
            Config.TryGetConfig(configHostPath, out Config hostConfig);

            if (hostConfig == null || hostConfig.Value == null || string.IsNullOrWhiteSpace(hostConfig.Value.ToString()))
            {
                throw new CmfBaseException("Missing Host configuration");
            }

            // Get Username config value
            string configUsernamePath = "/AMSOsram/TibcoEMS/Username";
            Config.TryGetConfig(configUsernamePath, out Config usernameConfig);

            if (usernameConfig == null || usernameConfig.Value == null || string.IsNullOrWhiteSpace(usernameConfig.Value.ToString()))
            {
                throw new CmfBaseException("Missing Username configuration");
            }

            // Get Password config value
            string configPasswordPath = "/AMSOsram/TibcoEMS/Password";
            Config.TryGetConfig(configPasswordPath, out Config passwordConfig);

            if (passwordConfig == null || passwordConfig.Value == null || string.IsNullOrWhiteSpace(passwordConfig.Value.ToString()))
            {
                throw new CmfBaseException("Missing Password configuration");
            }

            //---End DEE Code---

            return new Dictionary<string, object>()
            {
                { "IsEnabled", isEnabledConfig.Value.ToString() },
                { "Host", hostConfig.Value.ToString() },
                { "Username", usernameConfig.Value.ToString() },
                { "Password", Utilities.Decrypt(passwordConfig.Value.ToString(),"","", 3, "", 256) }
            };
        }
    }
}
