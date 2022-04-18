﻿using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Navigo.BusinessObjects;
using System.Collections.Generic;

namespace Cmf.Custom.AMSOsram.Actions.ProductionOrders
{
    public class CustomCreateGoodsIssueMessage : DeeDevBase
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

            MaterialCollection materials = Input["MaterialCollection"] as MaterialCollection;

            MaterialCollection availableMaterials = new MaterialCollection();

            foreach (Material material in materials)
            {
                if (material.FlowPath.Equals(material.Product.FlowPath))
                {
                    availableMaterials.Add(material);
                }
            }

            DeeContextHelper.SetContextParameter("AvailableMaterials", availableMaterials);

            return availableMaterials.Count > 0;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //System
            UseReference("", "System.Collections.Generic");

            //Common
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");

            // Get MaterialCollection based on the context
            MaterialCollection materials = DeeContextHelper.GetContextParameter("AvailableMaterials") as MaterialCollection;

            foreach (Material material in materials)
            {
                ProductionOrder productionOrder = new ProductionOrder();

                if (material.ProductionOrder != null)
                {
                    // Set productionOrder object based on ProductionOrder associated to the Material
                    productionOrder = material.ProductionOrder;
                }
                else
                {
                    // Set productionOrder object based on Custom Query result
                    productionOrder = AMSOsramUtilities.GetMaterialProductionOrder(material);

                    if (productionOrder == null)
                    {
                        // Throw exception if Custom Query doesn't returns ProductionOrder data
                        AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomStorageLocationMissing, material.Product.Name);
                    }
                }

                string message = productionOrder.SerializeToXML();

                // Create an IntegrationEntry to Inform SAP about the Goods Issue
                AMSOsramUtilities.CreateOutboundIntegrationEntry(message, AMSOsramConstants.MessageTypes.CustomPerformConsumptionToSAP);
            }

            //---End DEE Code---

            return Input;
        }
    }
}
