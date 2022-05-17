using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.ERP;
using Cmf.Custom.AMSOsram.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

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
            ///     - DEE Action to create an Integration Entry with Goods Issue information.
            /// Action Groups:
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            MaterialCollection materials = Input["MaterialCollection"] as MaterialCollection;

            MaterialCollection availableMaterials = new MaterialCollection();

            // Contains the material and the storage Location
            Dictionary<Material, string> materialsToProcess = new Dictionary<Material, string>();

            foreach (Material material in materials)
            {
                // Material must have the same flow path as the product and must be of form equal to Lot
                if (material.FlowPath.Equals(material.Product.FlowPath) && material.Form.Equals(AMSOsramConstants.CustomDefaultMaterialLotForm) && material.ProductionOrder != null)
                {
                    availableMaterials.Add(material);
                }
            }

            if (availableMaterials.Any())
            {
                availableMaterials.Load();
                foreach (Material material in availableMaterials)
                {
                    // Validate smart table
                    string storageLocation = AMSOsramUtilities.CustomResolveSTCustomReportConsumptionToSAP(material);
                    if (!string.IsNullOrEmpty(storageLocation))
                    {
                        materialsToProcess.Add(material, storageLocation); 
                    }
                }
                DeeContextHelper.SetContextParameter("MaterialsToProcess", materialsToProcess);
            }

            return !materialsToProcess.IsNullOrEmpty();

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
            Dictionary<Material, string> materials = DeeContextHelper.GetContextParameter("MaterialsToProcess") as Dictionary<Material, string>;

            foreach (Material material in materials.Keys)
            {
                ProductionOrder productionOrder = material.ProductionOrder;

                productionOrder.Load();

                // Load material site Attributes
                material.Facility.Load();
                Site site = material.Facility.Site;
                site.LoadAttributes(new Collection<string>() { AMSOsramConstants.CustomSiteCodeAttribute });
                string siteCode = string.Empty ;
                if (site.Attributes != null)
                {
                    if (site.Attributes.ContainsKey(AMSOsramConstants.CustomSiteCodeAttribute))
                    {
                        site.Attributes.TryGetValueAs(AMSOsramConstants.CustomSiteCodeAttribute, out siteCode);
                    }
                }

                // Create an object with information to send to SAP by ERP Movement Type
                string movementType = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultGoodsIssueMovementTypeConfig);
                CustomReportToERPItem customReportToERPItem = AMSOsramUtilities.CreateInfoForERP(movementType, materials[material], siteCode, productionOrder, material);

                if (customReportToERPItem != null)
                {
                    // Serialize object to XML 
                    string message = customReportToERPItem.SerializeToXML();
                    string name = $"GoodsIssue_{material.Name}_{Guid.NewGuid().ToString("N")}";

                    // Create an IntegrationEntry to Inform SAP about the Goods Issue
                    AMSOsramUtilities.CreateOutboundIntegrationEntry(message, AMSOsramConstants.CustomPerformConsumption, name);
                }
            }
            //---End DEE Code---

            return Input;
        }
    }
}
