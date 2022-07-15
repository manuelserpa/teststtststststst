using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.ERP;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
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

            // Load Integration Entry
            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, Foundation.Common.Constants.IntegrationEntry);

            // Cast Integation Entry Message to string
            string message = Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            // Deserialize XML Message to an object
            GoodsReceiptCertificate goodsReceiptCertificate = AMSOsramUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(message);

            string certificateDataCollectionName = string.Empty;
            string certificateDataCollectionType = string.Empty;
            string certificateLimitSetName = string.Empty;
            bool hasCerticateDataCollection = false;
            bool hasWafersCertificateData = false;

            ParameterSource waferSizeParameter = null;

            MaterialData materialData = goodsReceiptCertificate.Material;

            Material incomingLot = new Material()
            {
                Name = materialData.Name
            };

            if (incomingLot.ObjectExists())
            {
                incomingLot.Load();

                if (!incomingLot.Type.Equals(materialData.Type))
                {
                    AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomUpdateMaterialDifferentType, incomingLot.Name, materialData.Type);
                }

                if (!incomingLot.Product.Name.Equals(materialData.Product))
                {
                    AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomUpdateMaterialDifferentProduct, incomingLot.Name, materialData.Product);
                }

                if (materialData.Flow != null && incomingLot.Flow != null && !incomingLot.Flow.Name.Equals(materialData.Flow, StringComparison.InvariantCultureIgnoreCase))
                {
                    AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomUpdateMaterialDifferentFlow, incomingLot.Name, materialData.Flow);
                }

                if (materialData.Step != null && incomingLot.Step != null && !incomingLot.Step.Name.Equals(materialData.Step, StringComparison.InvariantCultureIgnoreCase))
                {
                    AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomUpdateMaterialDifferentStep, incomingLot.Name, materialData.Step);
                }

                if (materialData.Wafers.Count != incomingLot.SubMaterialCount)
                {
                    AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomUpdateMaterialDifferentWafers, incomingLot.Name, incomingLot.SubMaterialCount.ToString(), materialData.Wafers.Count.ToString());
                }

                // Verify if Lot Wafers and IncomingLotWafer has the same order
                if (incomingLot.SubMaterialCount > 0)
                {
                    incomingLot.LoadChildren();

                    bool wafersHasSameOrder = WafersHasSameOrder(incomingLot.SubMaterials, materialData.Wafers);

                    if (!wafersHasSameOrder)
                    {
                        AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomWrongCertificateConfiguration, incomingLot.Name);
                    }
                }
            }
            else
            {
                Product product = new Product()
                {
                    Name = materialData.Product
                };

                product.Load();

                ParameterSourceCollection productParameters = product.GetProductParameters();

                if (productParameters != null && productParameters.Any())
                {
                    waferSizeParameter = productParameters.FirstOrDefault(productParameter => productParameter.Parameter.Name.Equals(AMSOsramConstants.CustomParameterWaferQuantity, StringComparison.InvariantCultureIgnoreCase));
                }

                if (waferSizeParameter == null)
                {
                    AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomUpdateMaterialProductWaferSizeMissing, product.Name);
                }

                Facility facility = new Facility()
                {
                    Name = materialData.Facility
                };
                facility.Load();

                incomingLot.Facility = facility;
                incomingLot.Form = materialData.Form;
                incomingLot.Type = materialData.Type;
                incomingLot.Product = product;
                incomingLot.PrimaryQuantity = materialData.Wafers.Count * Convert.ToInt32(waferSizeParameter.Value);
                incomingLot.PrimaryUnits = product.DefaultUnits;

                Flow flow = product.Flow;

                if (materialData.Flow != null)
                {
                    flow = new Flow()
                    {
                        Name = materialData.Flow
                    };
                    flow.Load();
                }

                incomingLot.Flow = flow;

                Step step = product.Step;

                if (materialData.Step != null)
                {
                    step = new Step()
                    {
                        Name = materialData.Step
                    };
                    step.Load();
                }

                incomingLot.Step = step;
            }

            NgpDataSet ngpdsCertificateContext = AMSOsramUtilities.CustomResolveCertificateDataCollectionContext(incomingLot);

            DataSet dsCertificateContext = NgpDataSet.ToDataSet(ngpdsCertificateContext);

            if (dsCertificateContext.HasData())
            {
                certificateDataCollectionName = dsCertificateContext.Tables[0].Rows[0].Field<string>(Cmf.Navigo.Common.Constants.DataCollection);

                certificateLimitSetName = dsCertificateContext.Tables[0].Rows[0].Field<string>(Cmf.Navigo.Common.Constants.DataCollectionLimitSet);

                certificateDataCollectionType = dsCertificateContext.Tables[0].Rows[0].Field<string>("DataCollectionType");

                hasCerticateDataCollection = true;
            }

            //hasWafersCertificateData = materialData.Wafers != null && materialData.Wafers.Any(wafer => wafer.MaterialEDCData != null && wafer.MaterialEDCData.Any());
            hasWafersCertificateData = WafersHasCertificateData(materialData.Wafers);

            if ((hasCerticateDataCollection && !hasWafersCertificateData) || (!hasCerticateDataCollection && hasWafersCertificateData))
            {
                AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomWrongCertificateConfiguration, incomingLot.Name);
            }

            DeeContextHelper.SetContextParameter("MaterialData", materialData);

            DeeContextHelper.SetContextParameter("IncominLot", incomingLot);

            DeeContextHelper.SetContextParameter("CertificateDataCollectionName", certificateDataCollectionName);

            DeeContextHelper.SetContextParameter("CertificateLimitSetName", certificateLimitSetName);

            DeeContextHelper.SetContextParameter("CertificateDataCollectionType", certificateDataCollectionType);

            DeeContextHelper.SetContextParameter("WaferSizeParameter", waferSizeParameter);

            // Validate if Wafers has Certificate Data
            bool WafersHasCertificateData(List<MaterialData> wafers)
            {
                if (!wafers.IsNullOrEmpty())
                {
                    foreach (MaterialData wafer in wafers)
                    {
                        if (!wafer.MaterialEDCData.IsNullOrEmpty())
                        {
                            return true;
                        }
                        else
                        {
                            return WafersHasCertificateData(wafer.Wafers);
                        }
                    }
                }

                return false;
            }

            // Validate if Lot Wafers and IncomingLotWafer has the same order
            bool WafersHasSameOrder(MaterialCollection subMaterials, List<MaterialData> incomingWafers)
            {
                List<string> subMaterialNames = subMaterials.Select(sm => sm.Name).ToList();

                List<string> incomingWafersNames = incomingWafers.Select(w => w.Name).ToList();

                bool hasSameOrder = Enumerable.SequenceEqual(subMaterialNames.OrderBy(e => e), incomingWafersNames.OrderBy(e => e));

                if (!hasSameOrder)
                {
                    return false;
                }

                // Get all the Names of Lot Wafers
                foreach (Material material in subMaterials)
                {
                    MaterialData wafer = incomingWafers.FirstOrDefault(w => w.Name.Equals(material.Name, StringComparison.InvariantCultureIgnoreCase));

                    if ((material.SubMaterialCount > 0 && (wafer.Wafers.IsNullOrEmpty() || wafer.Wafers.Count != material.SubMaterialCount)) ||
                        (material.SubMaterialCount <= 0 && !wafer.Wafers.IsNullOrEmpty()))
                    {
                        return false;
                    }

                    if (material.SubMaterialCount > 0 && !wafer.Wafers.IsNullOrEmpty() && wafer.Wafers.Count == material.SubMaterialCount)
                    {
                        material.LoadChildren();

                        return WafersHasSameOrder(material.SubMaterials, wafer.Wafers);
                    }
                }

                return true;
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

            MaterialData materialData = DeeContextHelper.GetContextParameter("MaterialData") as MaterialData;
            Material incomingLot = DeeContextHelper.GetContextParameter("IncominLot") as Material;
            ParameterSource waferSizeParameter = DeeContextHelper.GetContextParameter("WaferSizeParameter") as ParameterSource;

            string certificateDataCollectionName = DeeContextHelper.GetContextParameter("CertificateDataCollectionName") as string;
            string certificateDataCollectionType = DeeContextHelper.GetContextParameter("CertificateDataCollectionType") as string;
            string certificateLimitSetName = DeeContextHelper.GetContextParameter("CertificateLimitSetName") as string;

            string outOfSpecName = AMSOsramUtilities.GetConfig<string>(AMSOsramConstants.DefaultLotIncomingHoldReasonConfig);

            DataCollection certificateDataCollection = null;
            DataCollectionLimitSet certificateLimitSet = null;
            MaterialHoldReasonCollection incomingLotHoldReasons = new MaterialHoldReasonCollection();
            MaterialCollection wafers = new MaterialCollection();

            // Sub Materials of Materials
            Dictionary<Material, MaterialCollection> subMaterialsOfMaterials = new Dictionary<Material, MaterialCollection>();

            bool isOutOfSpec = false;

            Dictionary<string, object> incomingLotAttributes = AMSOsramUtilities.GetEntityAttributesDefinition(Cmf.Navigo.Common.Constants.Material);

            if (!incomingLot.ObjectExists())
            {
                Facility facility = incomingLot.Facility;

                Product product = incomingLot.Product;

                decimal waferSize = Convert.ToDecimal(waferSizeParameter.Value);

                incomingLot.Create();

                if (materialData.StateModel != null && materialData.State != null)
                {
                    AMSOsramUtilities.SetMaterialStateModel(incomingLot, materialData.StateModel, materialData.State);
                    incomingLot.Load();
                }

                CreateSubMaterialsObject(incomingLot, materialData.Wafers);

                MaterialCollection materialsToCreate = new MaterialCollection();

                foreach (MaterialCollection matCollection in subMaterialsOfMaterials.Values)
                {
                    materialsToCreate.AddRange(matCollection);
                }

                materialsToCreate.Create();

                foreach (KeyValuePair<Material, MaterialCollection> keyValue in subMaterialsOfMaterials)
                {
                    Material parent = keyValue.Key;

                    parent.Load();

                    parent.AddSubMaterials(keyValue.Value);
                }
            }

            if (incomingLot.HoldCount > 0)
            {
                incomingLot.LoadRelations(Cmf.Navigo.Common.Constants.MaterialHoldReason);
                incomingLotHoldReasons.AddRange(incomingLot.MaterialHoldReasons);
                incomingLot.Release(incomingLotHoldReasons, false);

                MaterialHoldReason outOfSpecHoldReason = incomingLotHoldReasons.FirstOrDefault(incomingLotHoldReason => incomingLotHoldReason.TargetEntity.Name.Equals(outOfSpecName, StringComparison.InvariantCulture));

                if (outOfSpecHoldReason != null)
                {
                    incomingLotHoldReasons.Remove(outOfSpecHoldReason);
                }
            }

            incomingLot.LoadAttributes();

            AttributeCollection lotAttributes = AMSOsramUtilities.GetMaterialAttributesFromXML(incomingLotAttributes, materialData.MaterialAttributes);

            if (lotAttributes != null && lotAttributes.Any())
            {
                incomingLot.SaveAttributes(lotAttributes);
            }

            Dictionary<string, AttributeCollection> wafersAttributes = new Dictionary<string, AttributeCollection>();

            Dictionary<string, Dictionary<string, object>> wafersEDCData = new Dictionary<string, Dictionary<string, object>>();

            foreach (MaterialData wafer in materialData.Wafers)
            {
                AttributeCollection waferAttributes = AMSOsramUtilities.GetMaterialAttributesFromXML(incomingLotAttributes, wafer.MaterialAttributes);

                Dictionary<string, object> waferEDCData = AMSOsramUtilities.GetMaterialEDCDataFromXML(wafer.MaterialEDCData);

                wafersAttributes.Add(wafer.Name, waferAttributes);

                wafersEDCData.Add(wafer.Name, waferEDCData);
            }

            incomingLot.LoadChildren();

            wafers = incomingLot.SubMaterials;

            if (!string.IsNullOrEmpty(certificateDataCollectionName))
            {
                certificateDataCollection = new DataCollection
                {
                    Name = certificateDataCollectionName
                };

                certificateDataCollection.Load();

                if (!string.IsNullOrEmpty(certificateLimitSetName))
                {
                    certificateLimitSet = new DataCollectionLimitSet()
                    {
                        Name = certificateLimitSetName
                    };

                    certificateLimitSet.Load();
                }
            }

            foreach (Material wafer in wafers)
            {
                AttributeCollection waferAttributes = wafersAttributes[wafer.Name];

                if (waferAttributes != null && waferAttributes.Any())
                {
                    wafer.SaveAttributes(waferAttributes);
                }

                Dictionary<string, object> waferEDCData = wafersEDCData[wafer.Name];

                if (certificateDataCollection != null && waferEDCData != null && waferEDCData.Any())
                {
                    DataCollectionInstance certificateDataCollectionInstance = AMSOsramUtilities.PerformCertificateDataCollection(wafer, certificateDataCollection, certificateLimitSet, certificateDataCollectionType, waferEDCData);

                    if (certificateLimitSet != null && AMSOsramUtilities.IsDataCollectionLimiSetViolated(certificateDataCollectionInstance))
                    {
                        isOutOfSpec = true;
                    }
                }
            }

            if (isOutOfSpec)
            {
                Reason outOfSpecReason = new Reason()
                {
                    Name = outOfSpecName
                };
                outOfSpecReason.Load();

                incomingLotHoldReasons.Add(new MaterialHoldReason()
                {
                    SourceEntity = incomingLot,
                    TargetEntity = outOfSpecReason
                });
            }

            if (incomingLotHoldReasons != null && incomingLotHoldReasons.Any())
            {
                incomingLot.Hold(incomingLotHoldReasons, new OperationAttributeCollection());
            }

            // Create Sub Materials
            void CreateSubMaterialsObject(Material parentMaterial, List<MaterialData> incomingWafers, List<Product> currentProducts = null)
            {
                List<Product> productList = currentProducts;

                if (parentMaterial != null && incomingWafers != null && incomingWafers.Any())
                {
                    if (productList is null)
                    {
                        productList = new List<Product>();
                    }

                    if (!productList.Any(p => p.Name.Equals(parentMaterial.Product.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        productList.Add(parentMaterial.Product);
                    }

                    MaterialCollection subMaterials = new MaterialCollection();

                    foreach (MaterialData wafer in incomingWafers)
                    {
                        Product product = productList.FirstOrDefault(p => p.Name.Equals(wafer.Product, StringComparison.InvariantCultureIgnoreCase));

                        if (product is null)
                        {
                            product = new Product()
                            {
                                Name = wafer.Product
                            };

                            if (!product.ObjectExists())
                            {
                                product = parentMaterial.Product;
                            }

                            product.Load();

                            productList.Add(product);
                        }

                        ParameterSourceCollection productParameters = product.GetProductParameters();

                        ParameterSource localWaferSizeParameter = null;

                        if (productParameters != null && productParameters.Any())
                        {
                            localWaferSizeParameter = productParameters.FirstOrDefault(productParameter => productParameter.Parameter.Name.Equals(AMSOsramConstants.CustomParameterWaferQuantity, StringComparison.InvariantCultureIgnoreCase));
                        }

                        if (localWaferSizeParameter == null)
                        {
                            AMSOsramUtilities.ThrowLocalizedException(AMSOsramConstants.LocalizedMessageCustomUpdateMaterialProductWaferSizeMissing, product.Name);
                        }

                        Material material = new Material()
                        {
                            Name = wafer.Name,
                            Product = product,
                            Facility = parentMaterial.Facility,
                            PrimaryQuantity = (wafer.Wafers.IsNullOrEmpty() ? 1 : wafer.Wafers.Count) * Convert.ToInt32(localWaferSizeParameter.Value),
                            PrimaryUnits = product.DefaultUnits,
                            Form = wafer.Form,
                            Type = string.IsNullOrWhiteSpace(wafer.Type) ? parentMaterial.Type : wafer.Type
                        };

                        if (!string.IsNullOrWhiteSpace(wafer.StateModel))
                        {
                            StateModel stateModel = new StateModel()
                            {
                                Name = wafer.StateModel
                            };

                            if (stateModel.ObjectExists())
                            {
                                stateModel.Load();
                                StateModelState stateModelState = new StateModelState();
                                stateModelState.Load(wafer.State, stateModel);

                                material.CurrentMainState = new CurrentEntityState(material, stateModel, stateModelState);
                            }
                        }

                        subMaterials.Add(material);

                        CreateSubMaterialsObject(material, wafer.Wafers, productList);
                    }

                    subMaterialsOfMaterials.Add(parentMaterial, subMaterials);
                }
            }

            //---End DEE Code---

            return Input;
        }
    }
}
