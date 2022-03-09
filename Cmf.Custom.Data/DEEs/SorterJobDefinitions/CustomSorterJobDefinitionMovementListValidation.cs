using Cmf.Custom.AMSOsram.BusinessObjects;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Cmf.Custom.AMSOsram.Actions.SorterJobDefinitions
{
	internal class CustomSorterJobDefinitionMovementListValidation : DeeDevBase
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

			CustomSorterJobDefinition customSorterJobDefinition = null;

			if (Input.ContainsKey("CustomSorterJobDefinition") && Input["CustomSorterJobDefinition"] is CustomSorterJobDefinition inputCustomSorterJobDefinition)
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
			UseReference("%MicrosoftNetPath%System.Private.CoreLib.dll", "System.Threading");
			UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.Cultures");
			UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects");
			UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
			UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
			UseReference("", "Cmf.Foundation.Common.Exceptions");
			UseReference("", "Cmf.Foundation.Common");
			// Custom
			UseReference("Cmf.Custom.AMSOsram.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.AMSOsram.BusinessObjects");
			UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
			// System
			UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
			UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json");
			UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Schema");
			UseReference("%MicrosoftNetPath%System.Private.CoreLib.dll", "System.Threading");

			string schemaJson = string.Empty;
			CustomSorterJobDefinition customSorterJobDefinition = ApplicationContext.CallContext.GetInformationContext("CustomSorterJobDefinition") as CustomSorterJobDefinition;

			if (customSorterJobDefinition.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessMapCarrier)
			{
				schemaJson = AMSOsramConstants.CustomSorterJobDefinitionMapCarrierSchema;
			}
			else if (customSorterJobDefinition.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessTransferWafers)
			{
				schemaJson = AMSOsramConstants.CustomSorterJobDefinitionTransferWafersSchema;
			}
			else if (customSorterJobDefinition.LogisticalProcess == AMSOsramConstants.LookupTableCustomSorterLogisticalProcessCompose)
			{
				schemaJson = AMSOsramConstants.CustomSorterJobDefinitionComposeSchema;
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
				throw new CmfBaseException(string.Format(LocalizedMessage.GetLocalizedMessage(Thread.CurrentThread.CurrentCulture.Name, AMSOsramConstants.LocalizedMessageCustomSorterJobDefinitionInvalidMovementList).MessageText, Environment.NewLine, ex.Message));
			}
			//---End DEE Code---

			return Input;
		}
	}
}