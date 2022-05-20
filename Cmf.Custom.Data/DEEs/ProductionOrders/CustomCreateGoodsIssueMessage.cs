using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.ERP;
using Cmf.Custom.AMSOsram.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Cmf.Custom.AMSOsram.Actions
{
    public class CustomCreateGoodsIssueMessage : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     - DEE Action to create an Integration Entry with Goods Issue information.
            /// Action Groups:
            /// BusinessObjects.MaterialCollection.TrackOut.Post
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            MaterialCollection lots = new MaterialCollection();

            string siteCode = null;

            MaterialCollection materials = Input["MaterialCollection"] as MaterialCollection;


            // Contains the material and the storage Location
            Dictionary<Material, string> lotsStorageLocation = new Dictionary<Material, string>();

            foreach (Material material in materials)
            {
                string materialFlowPath = material.FlowPath;

                string productFlowPath = material.Product.FlowPath;

                // Material must have the same flow path as the product and must be of form equal to Lot
                if (!string.IsNullOrEmpty(materialFlowPath) && !string.IsNullOrEmpty(productFlowPath) && materialFlowPath.Equals(productFlowPath) && material.Form.Equals(AMSOsramConstants.MaterialLotForm) && material.ProductionOrder != null)
                {
                    lots.Add(material);
                }
            }

            if (lots.Any())
            {
                foreach (Material lot in lots)
                {
                    string storageLocation = AMSOsramUtilities.CustomResolveSTCustomReportConsumptionToSAP(lot);

                    if (!string.IsNullOrEmpty(storageLocation))
                    {
                        lotsStorageLocation.Add(lot, storageLocation); 
                    }
                }

                Site site = lots.First().Facility.Site;

                if (site != null)
                {
                    site.LoadAttributes(new Collection<string>() { AMSOsramConstants.CustomSiteCodeAttribute });
                }

                if (site.Attributes !=null && site.Attributes.ContainsKey(AMSOsramConstants.CustomSiteCodeAttribute))
                {
                    siteCode = site.Attributes[AMSOsramConstants.CustomSiteCodeAttribute].ToString();

                    DeeContextHelper.SetContextParameter("SiteCode", siteCode);
                }

                DeeContextHelper.SetContextParameter("LotsStorageLocation", lotsStorageLocation);
            }

            return lotsStorageLocation.Any() && !string.IsNullOrEmpty(siteCode);

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---
            
            //System
            UseReference("", "System");

            //Common
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.ERP");
            UseReference("", "Cmf.Custom.AMSOsram.Common.Extensions");

            // Get availableMaterials based on the context
            Dictionary<Material, string> lotsStorageLocation = DeeContextHelper.GetContextParameter("LotsStorageLocation") as Dictionary<Material, string>;

            string siteCode = DeeContextHelper.GetContextParameter("SiteCode") as string;

            string movementType = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultGoodsIssueMovementTypeConfig);

            foreach (Material lot in lotsStorageLocation.Keys)
            {
                ProductionOrder productionOrder = lot.ProductionOrder;

                // Create an object with information to send to SAP by ERP Movement Type
                CustomReportToERPItem customReportToERPItem = AMSOsramUtilities.CreateInfoForERP(movementType, lotsStorageLocation[lot], siteCode, productionOrder, lot);
                
                // Serialize object to XML 
                string message = customReportToERPItem.SerializeToXML();

                string name = $"GoodsIssue_{lot.Name}_{Guid.NewGuid().ToString("N")}";

                // Create an IntegrationEntry to Inform SAP about the Goods Issue
                AMSOsramUtilities.CreateOutboundIntegrationEntry(message, AMSOsramConstants.CustomPerformConsumption, name);
            }

            //---End DEE Code---

            return Input;
        }
    }
}
