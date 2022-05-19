using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.AMSOsram.Actions.ProductionOrders
{
    public class CustomImportProductionOrdersFromERP : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---  


            #region Info
            /// <summary>
            /// Imports Production Order From ERP.
            ///     
            /// Action Groups:
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, AMSOsramConstants.EntityTypes.IntegrationEntry);

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

            //System
            UseReference("", "System.Text");

            //Foundation
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common.Integration");

            //Navigo

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.DataStructures");

            // Load Integration Entry
            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, AMSOsramConstants.EntityTypes.IntegrationEntry);

            // Cast Integation Entry Message to string
            string message = Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            // Deserialize XML Message to an object
            CustomImportProductionOrderCollection customImportProductionOrders = AMSOsramUtilities.DeserializeXmlToObject<CustomImportProductionOrderCollection>(message);

            // Create Product context using data received from Integration Entry
            foreach (CustomImportProductionOrder customImportProductionOrder in customImportProductionOrders)
            {
                string integrationMessage = AMSOsramUtilities.SerializeToXML<CustomImportProductionOrder>(customImportProductionOrder);

                IntegrationEntry productIntegrationEntry = new IntegrationEntry();
                {
                    productIntegrationEntry.Name = $"PerformProductionOrderMasterData_{customImportProductionOrder.Name}_{Guid.NewGuid().ToString("N")}";
                    productIntegrationEntry.EventName = AMSOsramConstants.CustomIntegrationInboundEventName;
                    productIntegrationEntry.SourceSystem = Constants.MesSystemDesignation;
                    productIntegrationEntry.TargetSystem = Constants.MesSystemDesignation;
                    productIntegrationEntry.MessageType = "PerformProductionOrderMasterData";
                    productIntegrationEntry.IntegrationMessage = new IntegrationMessage  { Message = Encoding.Default.GetBytes(integrationMessage) };
                    productIntegrationEntry.SystemState = IntegrationEntrySystemState.Received;
                    productIntegrationEntry.MessageDate = DateTime.Now;
                }

                productIntegrationEntry.Create();
            }

            return Input;

            //---End DEE Code---
        }
    }
}
