using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.ERP;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Cmf.Custom.amsOSRAM.Actions.Integrations
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
            IIntegrationEntry integrationEntry = amsOSRAMUtilities.GetInputItem<IIntegrationEntry>(Input, Constants.IntegrationEntry);
            
            // Cast Integation Entry Message to string
            string message = Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            // Deserialize XML Message to an object
            GoodsReceiptCertificate goodsReceiptCertificate = amsOSRAMUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(message);

            string certificateDataCollectionName = string.Empty;
            string certificateDataCollectionType = string.Empty;
            string certificateLimitSetName = string.Empty; 
            bool hasCerticateDataCollection = false;
            bool hasWafersCertificateData = false;

            IParameterSource waferSizeParameter = null;

            MaterialData materialData = goodsReceiptCertificate.Material;

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            IMaterial incomingLot = entityFactory.Create<IMaterial>();
            incomingLot.Name = materialData.Name;

            if (incomingLot.ObjectExists())
            {
                incomingLot.Load();

                if (!incomingLot.Type.Equals(materialData.Type))
                {
                    amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomUpdateMaterialDifferentType, incomingLot.Name, materialData.Type);
                }

                if (!incomingLot.Product.Name.Equals(materialData.Product))
                {
                    amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomUpdateMaterialDifferentProduct, incomingLot.Name, materialData.Product);
                }

                if (materialData.Flow != null && incomingLot.Flow != null && !incomingLot.Flow.Name.Equals(materialData.Flow, StringComparison.InvariantCultureIgnoreCase))
                {
                    amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomUpdateMaterialDifferentFlow, incomingLot.Name, materialData.Flow);
                }

                if (materialData.Step != null && incomingLot.Step != null && !incomingLot.Step.Name.Equals(materialData.Step, StringComparison.InvariantCultureIgnoreCase))
                {
                    amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomUpdateMaterialDifferentStep, incomingLot.Name, materialData.Step);
                }

                if (materialData.Wafers.Count != incomingLot.SubMaterialCount)
                {
                    amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomUpdateMaterialDifferentWafers, incomingLot.Name, incomingLot.SubMaterialCount.ToString(), materialData.Wafers.Count.ToString());
                }

                // Verify if Lot Wafers and IncomingLotWafer has the same order
                if (incomingLot.SubMaterialCount > 0)
                {
                    incomingLot.LoadChildren();

                    bool wafersHasSameOrder = WafersHasSameOrder(incomingLot.SubMaterials, materialData.Wafers);

                    if (!wafersHasSameOrder)
                    {
                        amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomWrongCertificateConfiguration, incomingLot.Name);
                    }
                }
            }
            else
            {
                IProduct product = entityFactory.Create<IProduct>();
                product.Name = materialData.Product;

                product.Load();

                IParameterSourceCollection productParameters = product.GetProductParameters();

                if (productParameters != null && productParameters.Any())
                {
                    waferSizeParameter = productParameters.FirstOrDefault(productParameter => productParameter.Parameter.Name.Equals(amsOSRAMConstants.CustomParameterWaferQuantity, StringComparison.InvariantCultureIgnoreCase));
                }

                if (waferSizeParameter == null)
                {
                    amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomUpdateMaterialProductWaferSizeMissing, product.Name);
                }

                if (!decimal.TryParse(materialData.PrimaryQuantity, out decimal primaryQuantity))
                {
                    amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomInvalidPrimaryQuantity, materialData.Name);
                }

                if (string.IsNullOrWhiteSpace(materialData.PrimaryUnit))
                {
                    amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomPrimaryUnitObjectNull, materialData.Name);
                }

                IFacility facility = entityFactory.Create<IFacility>();
                facility.Name = materialData.Facility;
                facility.Load();

                incomingLot.Facility = facility;
                incomingLot.Form = materialData.Form;
                incomingLot.Type = materialData.Type;
                incomingLot.Product = product;
                incomingLot.PrimaryQuantity = primaryQuantity;
                incomingLot.PrimaryUnits = materialData.PrimaryUnit;
                //prod order exists checking
                
                if (!string.IsNullOrWhiteSpace(materialData.ProductionOrder))
                {
                    IProductionOrder prodOrder = entityFactory.Create<IProductionOrder>();
                    prodOrder.Name = materialData.ProductionOrder;

                    if (prodOrder.ObjectExists())
                    {
                        prodOrder.Load();
                        incomingLot.ProductionOrder = prodOrder;
                    }
                    else
                    {
                        amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomProductionOrderDoesNotExists, prodOrder.Name);
                    }
                }

                IFlow flow = product.Flow;

                if (materialData.Flow != null)
                {
                    flow = entityFactory.Create<IFlow>();
                    flow.Name = materialData.Flow;

                    flow.Load();
                }

                incomingLot.Flow = flow;

                IStep step = product.Step;

                if (materialData.Step != null)
                {
                    step = entityFactory.Create<IStep>();
                    step.Name = materialData.Step;

                    step.Load();
                }

                incomingLot.Step = step;
            }

            INgpDataSet ngpdsCertificateContext = amsOSRAMUtilities.CustomResolveCertificateDataCollectionContext(incomingLot);

            DataSet dsCertificateContext = NgpDataSet.ToDataSet(ngpdsCertificateContext);

            if (dsCertificateContext.HasData())
            {
                certificateDataCollectionName = dsCertificateContext.Tables[0].Rows[0].Field<string>(Navigo.Common.Constants.DataCollection);

                certificateLimitSetName = dsCertificateContext.Tables[0].Rows[0].Field<string>(Navigo.Common.Constants.DataCollectionLimitSet);

                certificateDataCollectionType = dsCertificateContext.Tables[0].Rows[0].Field<string>("DataCollectionType");

                hasCerticateDataCollection = true;
            }

            hasWafersCertificateData = WafersHasCertificateData(materialData.Wafers);

            if (!hasCerticateDataCollection || !hasWafersCertificateData)
            {
                amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomWrongCertificateConfiguration, incomingLot.Name);
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
                if (GenericExtensions.IsNullOrEmpty(wafers))
                {
                    return false;
                }

                foreach (MaterialData wafer in wafers)
                {
                    if (wafer.MaterialEDCData == null || wafer.MaterialEDCData.Count() == 0)
                    {
                        return WafersHasCertificateData(wafer.Wafers);
                    }

                    return true;
                }

                return false;
            }

            // Validate if Lot Wafers and IncomingLotWafer has the same order
            bool WafersHasSameOrder(IMaterialCollection subMaterials, List<MaterialData> incomingWafers)
            {
                List<string> subMaterialNames = subMaterials.Select(sm => sm.Name).ToList();

                List<string> incomingWafersNames = incomingWafers.Select(w => w.Name).ToList();

                bool hasSameOrder = Enumerable.SequenceEqual(subMaterialNames.OrderBy(e => e), incomingWafersNames.OrderBy(e => e));

                if (!hasSameOrder)
                {
                    return false;
                }

                // Get all the Names of Lot Wafers
                foreach (IMaterial material in subMaterials)
                {
                    MaterialData wafer = incomingWafers.FirstOrDefault(w => w.Name.Equals(material.Name, StringComparison.InvariantCultureIgnoreCase));

                    if ((material.SubMaterialCount > 0 && (GenericExtensions.IsNullOrEmpty(wafer.Wafers) || wafer.Wafers.Count != material.SubMaterialCount)) ||
                        (material.SubMaterialCount <= 0 && !GenericExtensions.IsNullOrEmpty(wafer.Wafers)))
                    {
                        return false;
                    }

                    if (material.SubMaterialCount > 0 && !GenericExtensions.IsNullOrEmpty(wafer.Wafers) && wafer.Wafers.Count == material.SubMaterialCount)
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
            UseReference("", "System.Data");
            UseReference("", "System.Text");

            //Common
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            //Navigo
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.InputObjects");
            UseReference("Cmf.Navigo.BusinessOrchestration.dll", "Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.OutputObjects");

            //Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.ERP");

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            MaterialData materialData = DeeContextHelper.GetContextParameter("MaterialData") as MaterialData;
            IMaterial incomingLot = DeeContextHelper.GetContextParameter("IncominLot") as IMaterial;
            IParameterSource waferSizeParameter = DeeContextHelper.GetContextParameter("WaferSizeParameter") as IParameterSource;

            string certificateDataCollectionName = DeeContextHelper.GetContextParameter("CertificateDataCollectionName") as string;
            string certificateDataCollectionType = DeeContextHelper.GetContextParameter("CertificateDataCollectionType") as string;
            string certificateLimitSetName = DeeContextHelper.GetContextParameter("CertificateLimitSetName") as string;

            string outOfSpecName = amsOSRAMUtilities.GetConfig<string>(amsOSRAMConstants.DefaultLotIncomingHoldReasonConfig);

            IDataCollection certificateDataCollection = null;
            IDataCollectionLimitSet certificateLimitSet = null;
            IMaterialHoldReasonCollection incomingLotHoldReasons = entityFactory.CreateCollection<IMaterialHoldReasonCollection>();
            IMaterialCollection wafers = entityFactory.CreateCollection<IMaterialCollection>();

            // Sub Materials of Materials
            Dictionary<IMaterial, IMaterialCollection> subMaterialsOfMaterials = new Dictionary<IMaterial, IMaterialCollection>();

            bool isOutOfSpec = false;

            Dictionary<string, object> incomingLotAttributes = amsOSRAMUtilities.GetEntityAttributesDefinition(Navigo.Common.Constants.Material);

            if (!incomingLot.ObjectExists())
            {
                IFacility facility = incomingLot.Facility;

                IProduct product = incomingLot.Product;

                decimal waferSize = Convert.ToDecimal(waferSizeParameter.Value);

                incomingLot.Create();

                if (materialData.StateModel != null && materialData.State != null)
                {
                    amsOSRAMUtilities.SetMaterialStateModel(incomingLot, materialData.StateModel, materialData.State);
                    incomingLot.Load();
                }

                CreateSubMaterialsObject(incomingLot, materialData.Wafers);

                IMaterialCollection materialsToCreate = entityFactory.CreateCollection<IMaterialCollection>();

                foreach (IMaterialCollection matCollection in subMaterialsOfMaterials.Values)
                {
                    materialsToCreate.AddRange(matCollection);
                }

                materialsToCreate.Create();

                foreach (KeyValuePair<IMaterial, IMaterialCollection> keyValue in subMaterialsOfMaterials)
                {
                    IMaterial parent = keyValue.Key;

                    parent.Load();

                    parent.AddSubMaterials(keyValue.Value);
                }
            }

            if (incomingLot.HoldCount > 0)
            {
                incomingLot.LoadRelations(Navigo.Common.Constants.MaterialHoldReason);
                incomingLotHoldReasons.AddRange(incomingLot.MaterialHoldReasons);
                incomingLot.Release(incomingLotHoldReasons, false);

                IMaterialHoldReason outOfSpecHoldReason = incomingLotHoldReasons.FirstOrDefault(incomingLotHoldReason => incomingLotHoldReason.TargetEntity.Name.Equals(outOfSpecName, StringComparison.InvariantCulture));

                if (outOfSpecHoldReason != null)
                {
                    incomingLotHoldReasons.Remove(outOfSpecHoldReason);
                }
            }

            incomingLot.LoadAttributes();

            IAttributeCollection lotAttributes = amsOSRAMUtilities.GetMaterialAttributesFromXML(incomingLotAttributes, materialData.MaterialAttributes);

            if (lotAttributes != null && lotAttributes.Any())
            {
                incomingLot.SaveAttributes(lotAttributes);
            }

            Dictionary<string, IAttributeCollection> wafersAttributes = new Dictionary<string, IAttributeCollection>();

            Dictionary<string, Dictionary<string, object>> wafersEDCData = new Dictionary<string, Dictionary<string, object>>();

            foreach (MaterialData wafer in materialData.Wafers)
            {
                IAttributeCollection waferAttributes = amsOSRAMUtilities.GetMaterialAttributesFromXML(incomingLotAttributes, wafer.MaterialAttributes);

                Dictionary<string, object> waferEDCData = amsOSRAMUtilities.GetMaterialEDCDataFromXML(wafer.MaterialEDCData);

                wafersAttributes.Add(wafer.Name, waferAttributes);

                wafersEDCData.Add(wafer.Name, waferEDCData);
            }

            incomingLot.LoadChildren();

            wafers = incomingLot.SubMaterials;

            if (!string.IsNullOrEmpty(certificateDataCollectionName))
            {
                
                certificateDataCollection = entityFactory.Create<IDataCollection>();
                certificateDataCollection.Name = certificateDataCollectionName;

                certificateDataCollection.Load();

                if (!string.IsNullOrEmpty(certificateLimitSetName))
                {
                    certificateLimitSet = entityFactory.Create<IDataCollectionLimitSet>();
                    certificateLimitSet.Name = certificateLimitSetName;

                    certificateLimitSet.Load();
                }
            }

            foreach (IMaterial wafer in wafers)
            {
                IAttributeCollection waferAttributes = wafersAttributes[wafer.Name];

                if (waferAttributes != null && waferAttributes.Any())
                {
                    wafer.SaveAttributes(waferAttributes);
                }

                Dictionary<string, object> waferEDCData = wafersEDCData[wafer.Name];

                if (certificateDataCollection != null && waferEDCData != null && waferEDCData.Any())
                {
                    // In this case we do not want to report the EDC data to SPACE
                    ApplicationContext.CallContext.SetInformationContext("ReportEDCToSpace", false);

                    IDataCollectionInstance certificateDataCollectionInstance = amsOSRAMUtilities.PerformCertificateDataCollection(wafer, certificateDataCollection, certificateLimitSet, certificateDataCollectionType, waferEDCData);

                    if (certificateLimitSet != null && amsOSRAMUtilities.IsDataCollectionLimiSetViolated(certificateDataCollectionInstance))
                    {
                        isOutOfSpec = true;
                    }
                }
            }

            if (isOutOfSpec)
            {
                IReason outOfSpecReason = entityFactory.Create<IReason>();
                outOfSpecReason.Name = outOfSpecName;
                outOfSpecReason.Load();

                IMaterialHoldReason materialHoldReason = entityFactory.Create<IMaterialHoldReason>();
                materialHoldReason.SourceEntity = incomingLot;
                materialHoldReason.TargetEntity = outOfSpecReason;

                incomingLotHoldReasons.Add(materialHoldReason);
            }

            if (incomingLotHoldReasons != null && incomingLotHoldReasons.Any())
            {
                incomingLot.Hold(incomingLotHoldReasons, new OperationAttributeCollection());
            }

            // Create Sub Materials
            void CreateSubMaterialsObject(IMaterial parentMaterial, List<MaterialData> incomingWafers, List<IProduct> currentProducts = null)
            {
                List<IProduct> productList = currentProducts;

                if (parentMaterial != null && incomingWafers != null && incomingWafers.Any())
                {
                    if (productList is null)
                    {
                        productList = new List<IProduct>();
                    }

                    if (!productList.Any(p => p.Name.Equals(parentMaterial.Product.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        productList.Add(parentMaterial.Product);
                    }

                    IMaterialCollection subMaterials = entityFactory.CreateCollection<IMaterialCollection>();

                    foreach (MaterialData wafer in incomingWafers)
                    {
                        IProduct product = productList.FirstOrDefault(p => p.Name.Equals(wafer.Product, StringComparison.InvariantCultureIgnoreCase));

                        if (product is null)
                        {
                            product = entityFactory.Create<IProduct>();
                            product.Name = wafer.Product;

                            if (!product.ObjectExists())
                            {
                                product = parentMaterial.Product;
                            }

                            product.Load();

                            productList.Add(product);
                        }

                        IParameterSourceCollection productParameters = product.GetProductParameters();

                        IParameterSource localWaferSizeParameter = null;

                        if (productParameters != null && productParameters.Any())
                        {
                            localWaferSizeParameter = productParameters.FirstOrDefault(productParameter => productParameter.Parameter.Name.Equals(amsOSRAMConstants.CustomParameterWaferQuantity, StringComparison.InvariantCultureIgnoreCase));
                        }

                        if (localWaferSizeParameter == null)
                        {
                            amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomUpdateMaterialProductWaferSizeMissing, product.Name);
                        }

                        if (!decimal.TryParse(wafer.PrimaryQuantity, out decimal waferPrimaryQuantity))
                        {
                            amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomInvalidPrimaryQuantity, wafer.Name);
                        }

                        if (string.IsNullOrWhiteSpace(wafer.PrimaryUnit))
                        {
                            amsOSRAMUtilities.ThrowLocalizedException(amsOSRAMConstants.LocalizedMessageCustomPrimaryUnitObjectNull, wafer.Name);
                        }

                        IMaterial material = entityFactory.Create<IMaterial>();
                        material.Name = wafer.Name;
                        material.Product = product;
                        material.Facility = parentMaterial.Facility;
                        material.PrimaryQuantity = waferPrimaryQuantity;
                        material.PrimaryUnits = wafer.PrimaryUnit;
                        material.Form = wafer.Form;
                        material.Type = string.IsNullOrWhiteSpace(wafer.Type) ? parentMaterial.Type : wafer.Type;

                        if (!string.IsNullOrWhiteSpace(wafer.StateModel))
                        {
                            IStateModel stateModel = new StateModel()
                            {
                                Name = wafer.StateModel
                            };

                            if (stateModel.ObjectExists())
                            {
                                stateModel.Load();
                                IStateModelState stateModelState = entityFactory.Create<IStateModelState>();
                                stateModelState.Load(wafer.State, stateModel);

                                ICurrentEntityState currentEntityState = entityFactory.Create<ICurrentEntityState>();
                                currentEntityState.Entity = material;
                                currentEntityState.StateModel = stateModel;
                                currentEntityState.CurrentState = stateModelState;

                                material.CurrentMainState = currentEntityState;
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
