using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.ERP;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.OutputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using System;
using System.Collections.Generic;
using System.Data;
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
            MaterialData materialData = goodsReceiptCertificate.Material;
            Material incomingLot = new Material() { Name = materialData.Name };

            Product product = new Product() { Name = materialData.Product };
            product.Load();

            Facility facility = new Facility() { Name = materialData.Facility };
            facility.Load();

            // Wafers 
            MaterialCollection wafers = new MaterialCollection();

            // Get Primary quantity 
            ParameterSourceCollection productParameters = product.GetProductParameters();
            decimal quantity = 0;
            if (productParameters.Any(pp => pp.Parameter.Name.Equals(AMSOsramConstants.CustomParameterWaferQuantity)))
            {
                string parameterQuantity = productParameters.FirstOrDefault(pp => pp.Parameter.Name.Equals(AMSOsramConstants.CustomParameterWaferQuantity)).Value;
                quantity = Convert.ToDecimal(parameterQuantity);
            }

            // Validate if material already exists on the system
            if (!incomingLot.ObjectExists())
            {
                Material lot = new Material
                {
                    Name = materialData.Name,
                    Product = product,
                    Facility = facility,
                    FlowPath = string.IsNullOrEmpty(product.FlowPath) ? FlowSearchHelper.CalculateFlowPath(materialData.Flow, materialData.Step) : product.FlowPath,
                    PrimaryQuantity = quantity * materialData.Wafers.Count,
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

                foreach (Wafer waferData in materialData.Wafers)
                {
                    // Create Sub Material to expand for main material
                    Material wafer = new Material()
                    {
                        Name = waferData.Name,
                        Product = product,
                        Facility = facility,
                        FlowPath = FlowSearchHelper.CalculateFlowPath(materialData.Flow, materialData.Step),
                        PrimaryQuantity = quantity,
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
                        edcValues.Add(edcData.Name, edcData.value);
                    }
                    waferEDCData.Add(waferData.Name, edcValues);
                }

                if (wafers.Count > 0)
                {
                    wafers.Create();
                    lot.AddSubMaterials(wafers);
                }

                wafers.Load();
                incomingLot = lot;
            }
            else
            {
                // lot already exists and needs to be updated
                incomingLot.Load();

                // Validate lot wafers are the same 
                incomingLot.LoadChildren();
                List<string> lotWafersName = incomingLot.SubMaterials.Select(sm => sm.Name).ToList();
                List<string> incomingLotWafersName = materialData.Wafers.Select(w => w.Name).ToList();
                if (lotWafersName.Except(incomingLotWafersName).ToList().Any() || incomingLotWafersName.Except(lotWafersName).ToList().Any())
                {
                    AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomUpdateMaterialDifferentWaferData, incomingLot.Name);
                }

                // Validate incoming lot is on same step and flow
                if (!incomingLot.Step.Name.Equals(materialData.Step) || !incomingLot.Flow.Name.Equals(materialData.Flow))
                {
                    AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomUpdateMaterialOnDifferentFlowStep, incomingLot.Name);
                }

                incomingLot.Product = product;
                incomingLot.PrimaryUnits = product.DefaultUnits;
                incomingLot.Form = materialData.Form;
                incomingLot.Type = materialData.Type;

                incomingLot.Save();

                incomingLot.SubMaterials.LoadAttributes();

                foreach (Material wafer in incomingLot.SubMaterials)
                {
                    Wafer waferData = (Wafer)materialData.Wafers.Where(w => w.Name.Equals(wafer.Name));

                    wafer.Product = product;
                    wafer.PrimaryQuantity = 1;
                    wafer.PrimaryUnits = product.DefaultUnits;
                    wafer.Form = waferData.Form;
                    wafer.Type = materialData.Type;
                    
                    AttributeCollection attributes = new AttributeCollection();
                    foreach (MaterialAttributes attribute in waferData.MaterialAttributes)
                    {
                        attributes.Add(attribute.Name, attribute.value);
                    }
                    waferAttributes.Add(wafer.Name, attributes);

                    Dictionary<string, object> edcValues = new Dictionary<string, object>();
                    foreach (MaterialEDCData edcData in waferData.MaterialEDCData)
                    {
                        edcValues.Add(edcData.Name, edcData.value);
                    }
                    waferEDCData.Add(waferData.Name, edcValues);
                }
                incomingLot.SubMaterials.Save();

            }

            //Dictionary<string, object> parametersData = (Dictionary<string, object>)waferEDCData.Values.Select(d => d.Keys).Distinct();
            incomingLot.Load();
            NgpDataSet dataSet = AMSOsramUtilities.GetCertificateInformation(incomingLot);
            List<MaterialEDCData> parametersName = (List<MaterialEDCData>)materialData.Wafers.Select(w => w.MaterialEDCData).Distinct();
            if ((dataSet != null && parametersName.Count == 0) || (dataSet == null && parametersName.Count > 0))
            {
                AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomWrongCertificateConfiguration, incomingLot.Name);
            }

            string certificateName = string.Empty;
            string certificateLimite = string.Empty;
            DataSet materialDCContextDataSet = NgpDataSet.ToDataSet(dataSet);
            if (materialDCContextDataSet.HasData())
            {
                certificateName = materialDCContextDataSet.Tables[0].Rows[0].Field<string>(Cmf.Navigo.Common.Constants.DataCollection);
                certificateLimite = materialDCContextDataSet.Tables[0].Rows[0].Field<string>(Cmf.Navigo.Common.Constants.DataCollectionLimitSet);
            }
            DataCollection certificate = new DataCollection
            {
                Name = certificateName
            };
            certificate.Load();

            ParameterCollection parametersToUse = new ParameterCollection();
            foreach (MaterialEDCData parameterName in parametersName)
            {
                Parameter parameter = new Parameter()
                {
                    Name = parameterName.Name
                };
                parametersToUse.Add(parameter);
            }

            if (parametersToUse.Count > 0 )
            {
                parametersToUse.Load();
            }

            // save attributes and post edc data
            foreach (Material wafer in wafers)
            {
                // save attributes
                wafer.SaveAttributes(waferAttributes[wafer.Name]);

                // post edc data

                DataCollectionInstance dcInstance = new DataCollectionInstance()
                {
                    Material = wafer,
                    DataCollection = certificate
                };

                OpenDataCollectionInstanceInput openDCInstanceInput = new OpenDataCollectionInstanceInput()
                {
                    DataCollectionInstance = dcInstance,
                    IsToIgnoreInSPC = true,
                };

                OpenDataCollectionInstanceOutput openDCInstanceOutput = DataCollectionInstanceManagementOrchestration.OpenDataCollectionInstance(openDCInstanceInput);
                dcInstance = openDCInstanceOutput.DataCollectionInstance;

                //insert dc point values 
                DataCollectionPointCollection dcPoints = new DataCollectionPointCollection();
                Dictionary<string, object> waferPoints = waferEDCData[wafer.Name];
                foreach (string parameterName in waferPoints.Keys)
                {
                    Parameter parameter = (Parameter)parametersToUse.Where(p => p.Name.Equals(parameterName));
                    DataCollectionPoint point = new DataCollectionPoint()
                    {
                        TargetEntity = parameter,
                        SourceEntity = dcInstance,
                        Value = waferPoints[parameterName]
                    };
                    dcPoints.Add(point);
                }

                PostDataCollectionPointsInput postDCPointsInput = new PostDataCollectionPointsInput()
                {
                    DataCollectionInstance = dcInstance,
                    DataCollectionPoints = dcPoints,
                    SkipDCValidation = false
                };

                PostDataCollectionPointsOutput postDataCollectionPointsOutput = DataCollectionInstanceManagementOrchestration.PostDataCollectionPoints(postDCPointsInput);

            }

            // Validate values posted 

            //---End DEE Code---

            return Input;
        }
    }
}
