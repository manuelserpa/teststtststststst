using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.ERP;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.AMSOsram.Actions.Integrations
{
    public class CustomProcessProductsFromERP : DeeDevBase
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

            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, Constants.IntegrationEntry);

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
            UseReference("", "System");
            UseReference("", "System.Collections.Generic");
            UseReference("", "System.Text");

            //Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common");
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common.Integration");

            //Navigo

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.ERP");

            // Load Integration Entry
            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, Constants.IntegrationEntry);

            // Cast Integation Entry Message to string
            string message = Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            // Deserialize XML Message to an object
            ProductDataOutput productDataOutput = AMSOsramUtilities.DeserializeXmlToObject<ProductDataOutput>(message);

            // Create Product context using data received from Integration Entry
            foreach (ProductData productData in productDataOutput.ProductsData)
            {
                string productDataMessage = AMSOsramUtilities.SerializeToXML<ProductData>(productData);

                IntegrationEntry productIntegrationEntry = new IntegrationEntry();
                {
                    productIntegrationEntry.Name = $"PerformProductMasterData_{productData.Name}_{Guid.NewGuid().ToString("N")}";
                    productIntegrationEntry.EventName = AMSOsramConstants.CustomIntegrationInboundEventName;
                    productIntegrationEntry.SourceSystem = Constants.MesSystemDesignation;
                    productIntegrationEntry.TargetSystem = Constants.MesSystemDesignation;
                    productIntegrationEntry.MessageType = "PerformProductMasterData";
                    productIntegrationEntry.IntegrationMessage = new IntegrationMessage
                    {
                        Message = Encoding.Default.GetBytes(productDataMessage)
                    };
                    productIntegrationEntry.SystemState = IntegrationEntrySystemState.Received;
                    productIntegrationEntry.MessageDate = DateTime.Now;
                }

                productIntegrationEntry.Create();
            }

            //---End DEE Code---

            return Input;
        }
    }
}