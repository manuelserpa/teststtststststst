using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.Extensions;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.Common;
using Cmf.Foundation.Configuration;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.AMSOsram.Actions.Containers
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

            Input.TryGetValueAs(Navigo.Common.Constants.Container, out Container container);
            
            if (container != null && Config.TryGetConfig(AMSOsramConstants.DefaultVendorContainerTypesConfig, out Config containerTypesConfig) &&
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

            UseReference("", "System.Threading");
            UseReference("", "Cmf.Foundation.BusinessObjects.Cultures");
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.Extensions");

            Input.TryGetValueAs(Navigo.Common.Constants.Container, out Container container);
            string actionGroup = Input["ActionGroupName"].ToString();

            if (container.UsedPositions > 0)
            {
                throw new CmfBaseException(string.Format(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name,
                    AMSOsramConstants.LocalizedMessageContainerCannotBeUndocked).MessageText, container.Name, AMSOsramConstants.DefaultVendorContainerTypesConfig));
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
