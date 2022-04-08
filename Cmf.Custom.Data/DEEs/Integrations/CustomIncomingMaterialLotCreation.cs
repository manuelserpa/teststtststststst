using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.ERP;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.OutputObjects;
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
            ///     - DEE Action to create or update lot incoming from ERP.
            /// Action Groups:
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            // Create a Dee Context to determine what is the action being executed
            DeeContext currentContext = DeeContextHelper.SetCurrentServiceContext("CustomIncomingMaterialLotCreationContext");
            const string CustomIncomingMaterialLotCreation_MaterialData = "CustomIncomingMaterialLotCreation_MaterialData";
            const string CustomIncomingMaterialLotCreation_IncomingLot = "CustomIncomingMaterialLotCreation_IncomingLot";
            const string CustomIncomingMaterialLotCreation_IsToCreateLot = "CustomIncomingMaterialLotCreation_IsToCreateLot";
            const string CustomIncomingMaterialLotCreation_Product = "CustomIncomingMaterialLotCreation_Product";
            const string CustomIncomingMaterialLotCreation_DataSet = "CustomIncomingMaterialLotCreation_DataSet";

            // Load Integration Entry
            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, "IntegrationEntry");

            // Cast Integation Entry Message to string
            string message = Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            // Deserialize XML Message to an object
            GoodsReceiptCertificate goodsReceiptCertificate = AMSOsramUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(message);

            MaterialData materialData = goodsReceiptCertificate.Material;
            Material incomingLot = new Material() { Name = materialData.Name };

            bool IsToCreateMaterial = true;

            Product product = new Product() { Name = materialData.Product };
            product.Load();

            NgpDataSet dataSet = null;

            if (!incomingLot.ObjectExists())
            {
                incomingLot.Product = product;  
                incomingLot.Form = materialData.Form;
                incomingLot.Type = materialData.Type;
                incomingLot.Step = product.Step;
                incomingLot.Flow = product.Flow;
                incomingLot.FlowPath = product.FlowPath;

                if (!string.IsNullOrEmpty(materialData.Flow) && !string.IsNullOrEmpty(materialData.Step))
                {
                    incomingLot.Step = new Step() { Name = materialData.Step };
                    incomingLot.Flow = new Flow() { Name = materialData.Flow };
                    incomingLot.FlowPath = FlowSearchHelper.CalculateFlowPath(materialData.Flow, materialData.Step);
                }
            }
            else
            {
                IsToCreateMaterial = false;

                // lot already exists and needs to be updated
                incomingLot.Load();

                // Validate lot wafers are the same 
                
                if (incomingLot.SubMaterialCount > 0)
                {
                    incomingLot.LoadChildren();
                    List<string> lotWafersName = incomingLot.SubMaterials.Select(sm => sm.Name).ToList();
                    List<string> incomingLotWafersName = materialData.Wafers.Select(w => w.Name).ToList();
                    if (lotWafersName.Except(incomingLotWafersName).ToList().Any() || incomingLotWafersName.Except(lotWafersName).ToList().Any())
                    {
                        AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomUpdateMaterialDifferentWaferData, incomingLot.Name);
                    }
                }

                // Validate incoming lot is on same step and flow
                if (!incomingLot.Step.Name.Equals(materialData.Step) || !incomingLot.Flow.Name.Equals(materialData.Flow))
                {
                    AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomUpdateMaterialOnDifferentFlowStep, incomingLot.Name);
                }

                if (!incomingLot.Product.Name.Equals(materialData.Product))
                {
                    AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomUpdateMaterialDifferentProduct, incomingLot.Name, materialData.Product);
                }
            }
            dataSet = AMSOsramUtilities.GetCertificateInformation(incomingLot);

            if ((dataSet != null && !materialData.Wafers.Any(wafer => wafer.MaterialEDCData.Any())) || (dataSet == null && materialData.Wafers.Any(wafer => wafer.MaterialEDCData.Any())))
            {
                AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomWrongCertificateConfiguration, incomingLot.Name);
            }

            //Set context
            DeeContextHelper.SetContextParameter(CustomIncomingMaterialLotCreation_MaterialData, materialData);
            DeeContextHelper.SetContextParameter(CustomIncomingMaterialLotCreation_IncomingLot, incomingLot);
            DeeContextHelper.SetContextParameter(CustomIncomingMaterialLotCreation_IsToCreateLot, IsToCreateMaterial);
            DeeContextHelper.SetContextParameter(CustomIncomingMaterialLotCreation_Product, product);
            DeeContextHelper.SetContextParameter(CustomIncomingMaterialLotCreation_DataSet, dataSet);

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //System
            UseReference("", "System");
            UseReference("", "System.Collections.Generic");
            UseReference("", "System.Data");
            UseReference("", "System.Linq");
            UseReference("", "System.Text");

            //Common
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            //Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.InputObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.OutputObjects");

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.ERP");

            const string CustomIncomingMaterialLotCreation_MaterialData = "CustomIncomingMaterialLotCreation_MaterialData";
            const string CustomIncomingMaterialLotCreation_IncomingLot = "CustomIncomingMaterialLotCreation_IncomingLot";
            const string CustomIncomingMaterialLotCreation_IsToCreateLot = "CustomIncomingMaterialLotCreation_IsToCreateLot";
            const string CustomIncomingMaterialLotCreation_Product = "CustomIncomingMaterialLotCreation_Product";
            const string CustomIncomingMaterialLotCreation_DataSet = "CustomIncomingMaterialLotCreation_DataSet";

            Dictionary<string, AttributeCollection> waferAttributes = new Dictionary<string, AttributeCollection>();
            Dictionary<string, Dictionary<string, object>> waferEDCData = new Dictionary<string, Dictionary<string, object>>();

            // Create lots using data received from Integration Entry
            MaterialData materialData = DeeContextHelper.GetContextParameter(CustomIncomingMaterialLotCreation_MaterialData) as MaterialData;
            Material incomingLot = DeeContextHelper.GetContextParameter(CustomIncomingMaterialLotCreation_IncomingLot) as Material;
            bool? isToCreateLot = DeeContextHelper.GetContextParameter(CustomIncomingMaterialLotCreation_IsToCreateLot) as bool?;
            Product product = DeeContextHelper.GetContextParameter(CustomIncomingMaterialLotCreation_Product) as Product;
            NgpDataSet dataSet = DeeContextHelper.GetContextParameter(CustomIncomingMaterialLotCreation_DataSet) as NgpDataSet;

            Facility facility = new Facility() { Name = materialData.Facility };
            facility.Load();

            // Wafers 
            MaterialCollection wafers = new MaterialCollection();

            // Get Primary quantity 
            ParameterSourceCollection productParameters = product.GetProductParameters();
            decimal quantity = 0;
            if (productParameters != null && productParameters.Any(pp => pp.Parameter.Name.Equals(AMSOsramConstants.CustomParameterWaferQuantity)))
            {
                string parameterQuantity = productParameters.FirstOrDefault(pp => pp.Parameter.Name.Equals(AMSOsramConstants.CustomParameterWaferQuantity)).Value;
                quantity = Convert.ToDecimal(parameterQuantity);
            }

            List<string> parametersName = new List<string>();

            // Attributes scalar type
            EntityType entityType = new EntityType();
            {
                entityType.Load(Cmf.Navigo.Common.Constants.Material);
                entityType.LoadProperties();
            }

            // Get attribute names and Scalar Type associated to Entity Type Product
            Dictionary<string, object> materialAttributes = null;
            if (entityType.Properties != null && entityType.Properties.Count > 0)
            {
                materialAttributes = entityType.Properties.Where(w => w.PropertyType == EntityTypePropertyType.Attribute).Select(s => new KeyValuePair<string, object>(s.Name, s.ScalarType)).ToDictionary(d => d.Key, d => d.Value);
            }

            // Hold Reasons
            MaterialHoldReasonCollection materialHoldReasons = new MaterialHoldReasonCollection();
            string holdReasonName = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultLotIncomingHoldReasonConfig);
            Reason holdReason = new Reason() { Name = holdReasonName };
            

            bool edcDataInvalid = false;

            // Validate if material already exists on the system
            if ((bool)isToCreateLot)
            {
                #region Create Lot and Wafers
                incomingLot.Facility = facility;
                incomingLot.PrimaryQuantity = quantity * materialData.Wafers.Count;
                incomingLot.PrimaryUnits = product.DefaultUnits;
                incomingLot.Create();

                // set attributes of lot
                incomingLot.LoadAttributes();
                incomingLot.SaveAttributes(AMSOsramUtilities.GetAttributesFromXMLMaterialAttributes(materialAttributes, materialData.MaterialAttributes));

                if (!materialData.Wafers.IsNullOrEmpty())
                {
                    foreach (Wafer waferData in materialData.Wafers)
                    {
                        // Create Sub Material to expand for main material
                        Material wafer = new Material()
                        {
                            Name = waferData.Name,
                            Product = product,
                            Facility = facility,
                            FlowPath = !string.IsNullOrEmpty(materialData.Flow) && !string.IsNullOrEmpty(materialData.Step) ? FlowSearchHelper.CalculateFlowPath(materialData.Flow, materialData.Step) : product.FlowPath,
                            PrimaryQuantity = quantity,
                            PrimaryUnits = product.DefaultUnits,
                            Form = waferData.Form,
                            Type = materialData.Type
                        };
                        wafers.Add(wafer);

                        waferAttributes.Add(wafer.Name, AMSOsramUtilities.GetAttributesFromXMLMaterialAttributes(materialAttributes, waferData.MaterialAttributes));

                        Dictionary<string, object> edcValues = new Dictionary<string, object>();
                        foreach (MaterialEDCData edcData in waferData.MaterialEDCData)
                        {
                            edcValues.Add(edcData.Name, edcData.value);
                            parametersName.Add(edcData.Name);
                        }
                        waferEDCData.Add(waferData.Name, edcValues);
                    }
                }

                if (wafers.Count > 0)
                {
                    wafers.Create();
                    incomingLot.AddSubMaterials(wafers);
                }

                wafers.Load();
                incomingLot.Load(); 
                #endregion
            }
            else
            {
                #region Update Lot and Wafers 
                // validate lot is on hold
                if (incomingLot.HoldCount > 0)
                {
                    incomingLot.LoadRelations(Cmf.Navigo.Common.Constants.MaterialHoldReason);
                    materialHoldReasons.AddRange(incomingLot.MaterialHoldReasons);
                    incomingLot.Release(materialHoldReasons, false);
                }

                // set attributes of lot
                incomingLot.LoadAttributes();
                incomingLot.SaveAttributes(AMSOsramUtilities.GetAttributesFromXMLMaterialAttributes(materialAttributes, materialData.MaterialAttributes));
                incomingLot.SubMaterials.LoadAttributes();

                foreach (Material wafer in incomingLot.SubMaterials)
                {
                    Wafer waferData = materialData.Wafers.FirstOrDefault(w => w.Name.Equals(wafer.Name));

                    waferAttributes.Add(wafer.Name, AMSOsramUtilities.GetAttributesFromXMLMaterialAttributes(materialAttributes, waferData.MaterialAttributes));

                    Dictionary<string, object> edcValues = new Dictionary<string, object>();
                    foreach (MaterialEDCData edcData in waferData.MaterialEDCData)
                    {
                        edcValues.Add(edcData.Name, edcData.value);
                        parametersName.Add(edcData.Name);
                    }
                    waferEDCData.Add(waferData.Name, edcValues);
                }
                incomingLot.SubMaterials.Save();
                wafers.AddRange(incomingLot.SubMaterials); 
                #endregion
            }

            if (materialData.StateModel != null && materialData.State != null)
            {
                AMSOsramUtilities.SetMaterialStateModel(incomingLot, materialData.StateModel, materialData.State);
            }

            parametersName = parametersName.Distinct().ToList();

            string certificateName = string.Empty;
            string certificateLimite = string.Empty;
            string executionType = string.Empty;
            DataSet materialDCContextDataSet = NgpDataSet.ToDataSet(dataSet);
            if (materialDCContextDataSet.HasData())
            {
                certificateName = materialDCContextDataSet.Tables[0].Rows[0].Field<string>(Cmf.Navigo.Common.Constants.DataCollection);
                certificateLimite = materialDCContextDataSet.Tables[0].Rows[0].Field<string>(Cmf.Navigo.Common.Constants.DataCollectionLimitSet);
                executionType = materialDCContextDataSet.Tables[0].Rows[0].Field<string>("DataCollectionType");
            }

            DataCollection certificate = new DataCollection
            {
                Name = certificateName
            };
            certificate.Load();

            ParameterCollection parametersToUse = new ParameterCollection();
            foreach (string parameterName in parametersName)
            {
                Parameter parameter = new Parameter()
                {
                    Name = parameterName
                };
                parametersToUse.Add(parameter);
            }

            if (parametersToUse.Count > 0)
            {
                parametersToUse.Load();
            }

            DataCollectionLimitSet limitSet = new DataCollectionLimitSet()
            {
                Name = certificateLimite
            };
            limitSet.Load();

            // save attributes and post edc data
            foreach (Material wafer in wafers)
            {
                // save attributes
                wafer.SaveAttributes(waferAttributes[wafer.Name]);

                // post edc data
                DataCollectionInstance dcInstance = new DataCollectionInstance()
                {
                    Material = wafer,
                    Product = wafer.Product,
                    Flow = wafer.Flow,
                    FlowPath = wafer.FlowPath,
                    Step = wafer.Step,
                    DataCollection = certificate,
                    DataCollectionLimitSet = limitSet
                };

                if (executionType.Equals("LongRunning"))
                {
                    dcInstance = AMSOsramUtilities.PostLongRunningCertificateData(dcInstance, waferEDCData[wafer.Name], parametersToUse);

                    if (!AMSOsramUtilities.ValidateDataCollectionLimitSetValues(dcInstance))
                    {
                        edcDataInvalid = true;
                    }
                }
                else if (executionType.Equals("Immediate"))
                {
                    dcInstance = AMSOsramUtilities.PostImmediateCertificateData(dcInstance, waferEDCData[wafer.Name], parametersToUse);

                    if (!AMSOsramUtilities.ValidateDataCollectionLimitSetValues(dcInstance))
                    {
                        edcDataInvalid = true;
                    }
                }
                dcInstance.Load();
                wafer.Save();

            }

            // Validate values posted

            if (!materialHoldReasons.Any(materialHoldReason => materialHoldReason.TargetEntity.Name.Equals(holdReason.Name)) && edcDataInvalid)
            {
                holdReason.Load();
                MaterialHoldReason certificateHoldReason = new MaterialHoldReason()
                {
                    SourceEntity = incomingLot,
                    TargetEntity = holdReason
                };
                materialHoldReasons.Add(certificateHoldReason);
            }
            else if (!edcDataInvalid && materialHoldReasons.Any(materialHoldReason => materialHoldReason.TargetEntity.Name.Equals(holdReason.Name)))
            {
                MaterialHoldReason certificateHoldReason = materialHoldReasons.FirstOrDefault(materialHoldReason => materialHoldReason.TargetEntity.Name.Equals(holdReason.Name));
                materialHoldReasons.Remove(certificateHoldReason);
            }

            if (materialHoldReasons.Count > 0)
            {
                incomingLot.Hold(materialHoldReasons, new OperationAttributeCollection());
            }

            //---End DEE Code---

            return Input;
        }
    }
}