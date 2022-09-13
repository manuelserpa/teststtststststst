using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.Configuration.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.LocalizationService;

namespace Cmf.Custom.amsOSRAM.Actions.Containers
{
    public class CustomUndockContainerValidation : DeeDevBase
    {
        /// <summary>
        /// DEE Test Condition.
        /// </summary>
        /// <param name="Input">The Input.</param>
        /// <returns></returns>
		public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info
            /// <summary>
            /// Summary text
            ///     - DEE Action to validate if a Container can be undocked based on its Type being or not configured as a VendorContainerType.
            /// Action Groups:
            /// BusinessObjects.Container.Undock.Pre
            /// BusinessObjects.Container.Undock.Post
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            bool canExecute = false;

            Input.TryGetValueAs(Navigo.Common.Constants.Container, out IContainer container);
            
            if (container != null && Config.TryGetConfig(amsOSRAMConstants.DefaultVendorContainerTypesConfig, out IConfig containerTypesConfig) &&
                !string.IsNullOrWhiteSpace(containerTypesConfig.GetConfigValue<string>()))
            {
                List<string> containerTypes = (containerTypesConfig.GetConfigValue<string>()).Split(new string[] { "," },
                    StringSplitOptions.RemoveEmptyEntries).ToList();

                if (containerTypes != null && containerTypes.Any(c => c.Trim().Equals(container.Type, StringComparison.InvariantCultureIgnoreCase)))
                {
                    canExecute = true;
                }
            }

            return canExecute;

            //---End DEE Condition Code---
        }

        /// <summary>
        /// DEE Action Code.
        /// </summary>
        /// <param name="Input">The Input.</param>
        /// <returns></returns>
		public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            // System
            UseReference("", "System.Threading");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.Extensions");

            // Foundation
            UseReference("", "Cmf.Foundation.Common.LocalizationService");

            Input.TryGetValueAs(Navigo.Common.Constants.Container, out IContainer container);
            string actionGroup = Input["ActionGroupName"].ToString();

            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();

            if (container.UsedPositions > 0)
            {
                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name,
                    amsOSRAMConstants.LocalizedMessageContainerCannotBeUndocked), container.Name, amsOSRAMConstants.DefaultVendorContainerTypesConfig));
            }
            else if (actionGroup.EndsWith(".Post"))
            {
                // We only can terminate the container after it has been undocked
                container.Terminate();
            }

            //---End DEE Code---

            return Input;
        }
    }
}
