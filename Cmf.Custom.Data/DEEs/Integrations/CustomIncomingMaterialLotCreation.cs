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
using Cmf.Navigo.Common;
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

            List<string> parametersName = new List<string>();

            // Attributes scalar type
            EntityType entityType = new EntityType();
            {
                entityType.Load(Cmf.Navigo.Common.Constants.Material);
                entityType.LoadProperties();
            }

            // Get attribute names and Scalar Type associated to Entity Type Product
            Dictionary<string, object> materialAttributes = entityType.Properties.Where(w => w.PropertyType == EntityTypePropertyType.Attribute).Select(s => new KeyValuePair<string, object>(s.Name, s.ScalarType)).ToDictionary(d => d.Key, d => d.Value);


            // Hold Reasons
            MaterialHoldReasonCollection materialHoldReasons = new MaterialHoldReasonCollection();
            string holdReasonName = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultLotIncomingHoldReasonConfig);
            Reason holdReason = new Reason() { Name = holdReasonName };
            holdReason.Load();

            bool edcDataInvalid = false;

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
                        ScalarType scalarType = materialAttributes[attribute.Name] as ScalarType;
                        attributes.Add(attribute.Name, AMSOsramUtilities.GetAttributeValueAsDataType(scalarType, attribute.value));
                    }
                    waferAttributes.Add(wafer.Name, attributes);

                    Dictionary<string, object> edcValues = new Dictionary<string, object>();
                    foreach (MaterialEDCData edcData in waferData.MaterialEDCData)
                    {
                        edcValues.Add(edcData.Name, edcData.value);
                        parametersName.Add(edcData.Name);
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
                incomingLot.Load();

                
            }
            else
            {
                // lot already exists and needs to be updated
                incomingLot.Load();

                // validate lot is on hold
                if (incomingLot.HoldCount > 0)
                {
                    incomingLot.LoadRelations(Cmf.Navigo.Common.Constants.MaterialHoldReason);
                    materialHoldReasons.AddRange(incomingLot.MaterialHoldReasons);
                    incomingLot.Release(materialHoldReasons,false);
                }

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

                if (!incomingLot.Form.Equals(materialData.Form))
                {
                    incomingLot.Form = materialData.Form; 
                }

                if (!incomingLot.Product.Name.Equals(materialData.Product))
                {
                    AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomUpdateMaterialDifferentProduct, incomingLot.Name, materialData.Product);
                }

                incomingLot.Save();
                incomingLot.SubMaterials.LoadAttributes();

                foreach (Material wafer in incomingLot.SubMaterials)
                {
                    Wafer waferData = materialData.Wafers.FirstOrDefault(w => w.Name.Equals(wafer.Name));
                    
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
                        parametersName.Add(edcData.Name);
                    }
                    waferEDCData.Add(waferData.Name, edcValues);
                }
                incomingLot.SubMaterials.Save();
                wafers.AddRange(incomingLot.SubMaterials);
            }

            if (materialData.StateModel != null && materialData.State != null)
            {
                AMSOsramUtilities.SetMaterialStateModel(incomingLot, materialData.StateModel, materialData.State);
            }

            NgpDataSet dataSet = AMSOsramUtilities.GetCertificateInformation(incomingLot);
            parametersName = parametersName.Distinct().ToList();

            if ((dataSet != null && parametersName.Count == 0) || (dataSet == null && parametersName.Count > 0))
            {
                AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomWrongCertificateConfiguration, incomingLot.Name);
            }

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

            if (parametersToUse.Count > 0 )
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
                    foreach (Parameter parameter in parametersToUse)
                    {
                        DataCollectionPoint point = new DataCollectionPoint()
                        {
                            SampleId = "Sample 1",
                            ReadingNumber = 1,
                            TargetEntity = parameter,
                            SourceEntity = dcInstance,
                            Value = waferPoints[parameter.Name]
                        };
                        dcPoints.Add(point);

                        if (dcInstance.RelationCollection == null)
                            dcInstance.RelationCollection = new CmfEntityRelationCollection();

                        dcInstance.RelationCollection.Add(point);

                    }
                    dcInstance.Load();

                    PostDataCollectionPointsInput postDCPointsInput = new PostDataCollectionPointsInput()
                    {
                        DataCollectionInstance = dcInstance,
                        DataCollectionPoints = dcPoints,
                        SkipDCValidation = false
                    };

                    PostDataCollectionPointsOutput postDataCollectionPointsOutput = DataCollectionInstanceManagementOrchestration.PostDataCollectionPoints(postDCPointsInput);

                    dcInstance = postDataCollectionPointsOutput.DataCollectionInstance;

                    if (!AMSOsramUtilities.ValidateDataCollectionLimitSetValues(dcInstance))
                    {
                        edcDataInvalid = true;
                    }
                }
                else if (executionType.Equals("Immediate"))
                {
                    //insert dc point values 
                    DataCollectionPointCollection dcPoints = new DataCollectionPointCollection();
                    Dictionary<string, object> waferPoints = waferEDCData[wafer.Name];
                    foreach (Parameter parameter in parametersToUse)
                    {
                        DataCollectionPoint point = new DataCollectionPoint()
                        {
                            SampleId = "Sample 1",
                            ReadingNumber = 1,
                            TargetEntity = parameter,
                            Value = waferPoints[parameter.Name]
                        };
                        dcPoints.Add(point);

                        if (dcInstance.RelationCollection == null)
                            dcInstance.RelationCollection = new CmfEntityRelationCollection();

                        dcInstance.RelationCollection.Add(point);
                    }

                    PerformImmediateDataCollectionOutput dataCollectionInstanceResult = DataCollectionInstanceManagementOrchestration.PerformImmediateDataCollection(
                        new Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.InputObjects.PerformImmediateDataCollectionInput()
                        {
                            DataCollectionInstance = dcInstance,
                            SkipDCValidation = false,
                            IsToIgnoreInSPC = true,
                            IgnoreLastServiceId = false
                        }
                    );

                    dcInstance = dataCollectionInstanceResult.DataCollectionInstance;

                    if (!AMSOsramUtilities.ValidateDataCollectionLimitSetValues(dcInstance))
                    {
                        edcDataInvalid = true;
                    }
                }
                dcInstance.Load();

                wafer.CurrentDataCollectionInstance = dcInstance;
                wafer.Save();

            }

            // Validate values posted
            
            if (!materialHoldReasons.Any(materialHoldReason => materialHoldReason.TargetEntity.Name.Equals(holdReason.Name)) && edcDataInvalid)
            {
                MaterialHoldReason certificateHoldReason = new MaterialHoldReason()
                {
                    SourceEntity = incomingLot,
                    TargetEntity = holdReason
                };
                materialHoldReasons.Add(certificateHoldReason);
            }
            else if(!edcDataInvalid && materialHoldReasons.Any(materialHoldReason => materialHoldReason.TargetEntity.Name.Equals(holdReason.Name)))            
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