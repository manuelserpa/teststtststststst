using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Foundation.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Cmf.Custom.amsOSRAM.Actions
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

            IIntegrationEntry integrationEntry = amsOSRAMUtilities.GetInputItem<IIntegrationEntry>(Input, amsOSRAMConstants.IntegrationEntry);

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

            //Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.DataStructures");

            // Load Integration Entry
            IIntegrationEntry integrationEntry = amsOSRAMUtilities.GetInputItem<IIntegrationEntry>(Input, amsOSRAMConstants.IntegrationEntry);

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            // Cast Integation Entry Message to string
            string message = Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            // Deserialize XML Message to an object
            CustomImportProductionOrderCollection customImportProductionOrders = amsOSRAMUtilities.DeserializeXmlToObject<CustomImportProductionOrderCollection>(message);

            // Create Product context using data received from Integration Entry
            foreach (CustomImportProductionOrder customImportProductionOrder in customImportProductionOrders)
            {
                IIntegrationEntry productIntegrationEntry = entityFactory.Create<IIntegrationEntry>();
                productIntegrationEntry.Name = $"PerformProductionOrderMasterData_{customImportProductionOrder.Name}_{Guid.NewGuid().ToString("N")}";
                productIntegrationEntry.EventName = amsOSRAMConstants.CustomIntegrationInboundEventName;
                productIntegrationEntry.SourceSystem = Constants.MesSystemDesignation;
                productIntegrationEntry.TargetSystem = Constants.MesSystemDesignation;
                productIntegrationEntry.MessageType = "PerformProductionOrderMasterData";
                productIntegrationEntry.SystemState = IntegrationEntrySystemState.Received;
                productIntegrationEntry.MessageDate = DateTime.Now;

                IIntegrationMessage integrationMessage = entityFactory.Create<IIntegrationMessage>();
                integrationMessage.Message = Encoding.Default.GetBytes(amsOSRAMUtilities.SerializeToXML(customImportProductionOrder));

                productIntegrationEntry.IntegrationMessage = integrationMessage;
                
                productIntegrationEntry.Create();
            }

            //---End DEE Code---

            return Input;
        }
    }
}
