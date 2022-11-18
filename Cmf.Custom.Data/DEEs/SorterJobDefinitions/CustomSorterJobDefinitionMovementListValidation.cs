using Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.LocalizationService;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Cmf.Custom.amsOSRAM.Actions.SorterJobDefinitions
{
    public class CustomSorterJobDefinitionMovementListValidation : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     DEE Action responsible to validate the JSON structure for the Custom Sorter Job Definition Movement List.
             *
             * Action Groups:
             *     - BusinessObjects.CustomSorterJobDefinition.Create.Pre
             *     - BusinessObjects.CustomSorterJobDefinition.Save.Pre
            */

            #endregion Info

            ICustomSorterJobDefinition customSorterJobDefinition = null;

            if (Input.ContainsKey("CustomSorterJobDefinition") && Input["CustomSorterJobDefinition"] is ICustomSorterJobDefinition inputCustomSorterJobDefinition)
            {
                customSorterJobDefinition = inputCustomSorterJobDefinition;
                ApplicationContext.CallContext.SetInformationContext("CustomSorterJobDefinition", customSorterJobDefinition);
            }

            return customSorterJobDefinition != null;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            // Foundation
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects");
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common.LocalizationService");

            // Custom
            UseReference("Cmf.Custom.amsOSRAM.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");

            // System
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Schema");
            UseReference("%MicrosoftNetPath%System.Private.CoreLib.dll", "System.Threading");

            string schemaJson = string.Empty;
            ICustomSorterJobDefinition customSorterJobDefinition = ApplicationContext.CallContext.GetInformationContext("CustomSorterJobDefinition") as ICustomSorterJobDefinition;

            if (customSorterJobDefinition.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessMapCarrier)
            {
                schemaJson = amsOSRAMConstants.CustomSorterJobDefinitionMapCarrierSchema;
            }
            else if (customSorterJobDefinition.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessTransferWafers)
            {
                schemaJson = amsOSRAMConstants.CustomSorterJobDefinitionTransferWafersSchema;
            }
            else if (customSorterJobDefinition.LogisticalProcess == amsOSRAMConstants.LookupTableCustomSorterLogisticalProcessCompose)
            {
                schemaJson = amsOSRAMConstants.CustomSorterJobDefinitionComposeSchema;
            }

            JsonSchema schema = JsonSchema.Parse(schemaJson);

            try
            {
                JObject movementListObject = JObject.Parse(customSorterJobDefinition.MovementList);
                bool valid = movementListObject.IsValid(schema, out IList<string> messages);

                if (!valid)
                {
                    string exceptionMessage = string.Empty;
                    foreach (string message in messages)
                    {
                        exceptionMessage += message + Environment.NewLine;
                    }

                    throw new CmfBaseException(exceptionMessage);
                }
            }
            catch (JsonReaderException ex)
            {
                // Get services provider information
                IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
                ILocalizationService localizationService = serviceProvider.GetService<ILocalizationService>();

                throw new CmfBaseException(string.Format(localizationService.Localize(Thread.CurrentThread.CurrentCulture.Name, amsOSRAMConstants.LocalizedMessageCustomSorterJobDefinitionInvalidMovementList), Environment.NewLine, ex.Message));
            }

            //---End DEE Code---

            return Input;
        }
    }
}