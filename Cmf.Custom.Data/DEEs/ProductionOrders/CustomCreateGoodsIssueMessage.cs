using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.ERP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.Abstractions;

namespace Cmf.Custom.amsOSRAM.Actions
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

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IMaterialCollection lots = entityFactory.CreateCollection<IMaterialCollection>();

            string siteCode = null;

            IMaterialCollection materials = Input["MaterialCollection"] as IMaterialCollection;

            // Contains the material and the storage Location
            Dictionary<IMaterial, string> lotsStorageLocation = new Dictionary<IMaterial, string>();

            foreach (IMaterial material in materials)
            {
                string materialFlowPath = material.FlowPath;

                string productFlowPath = material.Product.FlowPath;

                // Material must have the same flow path as the product and must be of form equal to Lot
                if (!string.IsNullOrEmpty(materialFlowPath) && !string.IsNullOrEmpty(productFlowPath) && materialFlowPath.Equals(productFlowPath) && material.Form.Equals(amsOSRAMConstants.MaterialLotForm) && material.ProductionOrder != null)
                {
                    lots.Add(material);
                }
            }

            if (lots.Any())
            {
                foreach (IMaterial lot in lots)
                {
                    string storageLocation = amsOSRAMUtilities.CustomResolveSTCustomReportConsumptionToSAP(lot);

                    if (!string.IsNullOrEmpty(storageLocation))
                    {
                        lotsStorageLocation.Add(lot, storageLocation); 
                    }
                }

                ISite site = lots.First().Facility.Site;

                if (site != null)
                {
                    site.LoadAttributes(new Collection<string>() { amsOSRAMConstants.CustomSiteCodeAttribute });
                }

                if (site.Attributes !=null && site.Attributes.ContainsKey(amsOSRAMConstants.CustomSiteCodeAttribute))
                {
                    siteCode = site.Attributes[amsOSRAMConstants.CustomSiteCodeAttribute].ToString();

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

            //Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.ERP");
            UseReference("", "Cmf.Custom.amsOSRAM.Common.Extensions");

            // Get availableMaterials based on the context
            Dictionary<IMaterial, string> lotsStorageLocation = DeeContextHelper.GetContextParameter("LotsStorageLocation") as Dictionary<IMaterial, string>;

            string siteCode = DeeContextHelper.GetContextParameter("SiteCode") as string;

            string movementType = amsOSRAMUtilities.GetConfig<string>(amsOSRAMConstants.DefaultGoodsIssueMovementTypeConfig);

            foreach (IMaterial lot in lotsStorageLocation.Keys)
            {
                IProductionOrder productionOrder = lot.ProductionOrder;

                // Create an object with information to send to SAP by ERP Movement Type
                CustomReportToERPItem customReportToERPItem = amsOSRAMUtilities.CreateInfoForERP(movementType, lotsStorageLocation[lot], siteCode, productionOrder, lot);
                
                // Serialize object to XML 
                string message = customReportToERPItem.SerializeToXML();

                string name = $"GoodsIssue_{lot.Name}_{Guid.NewGuid().ToString("N")}";

                // Create an IntegrationEntry to Inform SAP about the Goods Issue
                amsOSRAMUtilities.CreateOutboundIntegrationEntry(message, amsOSRAMConstants.CustomPerformConsumption, name);
            }

            //---End DEE Code---

            return Input;
        }
    }
}
