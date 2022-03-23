using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.ERP;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmf.Custom.AMSOsram.Actions.Integrations
{
    public class CustomIncomingMaterialLotCreation : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     - DEE Action to 
            /// Action Groups:
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //System
            UseReference("", "System.IO");
            UseReference("", "System.Threading");
            UseReference("", "System.Text");

            //Foundation
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration");
            //UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.ERP");

            // Load Integration Entry
            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, "IntegrationEntry");


            // Cast Integation Entry Message to string
            string message = Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            // Deserialize XML Message to an object
            GoodsReceiptCertificate goodsReceiptCertificate = AMSOsramUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(message);

            Dictionary<string, AttributeCollection> waferAttributes = new Dictionary<string, AttributeCollection>();
            Dictionary<string, Dictionary<string, object>> waferEDCData = new Dictionary<string, Dictionary<string, object>>();

            // Create lots using data received from Integration Entry
            Material lot = new Material();
            MaterialData materialData = goodsReceiptCertificate.Material;

            Product product = new Product()
            {
                Name = materialData.Product
            };
            product.Load();

            Facility facility = new Facility()
            {
                Name = materialData.Facility
            };
            facility.Load();

            lot = new Material
            {
                Name = materialData.Name,
                Product = product,
                Facility = facility,
                FlowPath = string.IsNullOrEmpty(product.FlowPath) ? FlowSearchHelper.CalculateFlowPath(materialData.Flow, materialData.Step) : product.FlowPath,
                PrimaryQuantity = 0,
                PrimaryUnits = product.DefaultUnits,
                Form = materialData.Form,
                Type = materialData.Type
            };

            lot.Create();

            // set attributes of lot
            lot.LoadAttributes();

            AttributeCollection lotAttributes = new AttributeCollection();

            foreach (MaterialAttributes attribute in materialData.MaterialAttributes)
            {
                lotAttributes.Add(attribute.Name, attribute.value);
            }
            lot.SaveAttributes(lotAttributes);

            MaterialCollection wafers = new MaterialCollection();
            foreach (Wafer waferData in materialData.Wafers)
            {
                // Create Sub Material to expand for main material
                Material wafer = new Material()
                {
                    Name = waferData.Name,
                    Product = product,
                    Facility = facility,
                    FlowPath = FlowSearchHelper.CalculateFlowPath(materialData.Flow, materialData.Step),
                    PrimaryQuantity = 1,
                    PrimaryUnits = product.DefaultUnits,
                    Form = waferData.Form,
                    Type = materialData.Type
                };
                wafers.Add(wafer);

                AttributeCollection attributes = new AttributeCollection();
                foreach (MaterialAttributes attribute in waferData.MaterialAttributes)
                {
                    attributes.Add(attribute.Name, attribute.value);
                }
                waferAttributes.Add(wafer.Name, attributes);

                Dictionary<string, object> edcValues = new Dictionary<string, object>();
                foreach (MaterialEDCData edcData in waferData.MaterialEDCData)
                {
                    edcValues.Add(edcData.Name,edcData.value);
                }
                waferEDCData.Add(waferData.Name,edcValues);
            }

            if (wafers.Count > 0)
            {
                wafers.Create();
                lot.AddSubMaterials(wafers);
            }

            // save attributes and post edc data
            foreach (Material wafer in wafers)
            {
                // save attributes
                wafer.SaveAttributes(waferAttributes[wafer.Name]);

                // post edc data
                // how to get the right DC create a custom ST to resolve the DC to use
            }
            

            //---End DEE Code---

            return Input;
        }
    }
}
