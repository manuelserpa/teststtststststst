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

                    bool wafersHasSameOrder = this.WafersHasSameOrder(incomingLot.SubMaterials, materialData.Wafers);

                    //List<string> lotWafers = incomingLot.SubMaterials.Select(sm => sm.Name).ToList();

                    //List<string> incomingLotWafers = materialData.Wafers.Select(w => w.Name).ToList();

                    //bool areEquals = Enumerable.SequenceEqual(lotWafers.OrderBy(e => e), incomingLotWafers.OrderBy(e => e));

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
            hasWafersCertificateData = this.WafersHasCertificateData(materialData.Wafers);

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
            MaterialCollection subMaterials = new MaterialCollection();

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

                foreach (Wafer waferData in materialData.Wafers)
                {
                    Material wafer = new Material()
                    {
                        Name = waferData.Name,
                        Product = product,
                        Facility = facility,
                        PrimaryQuantity = waferSize,
                        PrimaryUnits = product.DefaultUnits,
                        Form = waferData.Form,
                        Type = materialData.Type
                    };

                    foreach (Wafer subMaterialData in waferData.Wafers)
                    {
                        Material subMaterial = new Material()
                        {
                            Name = subMaterialData.Name,
                            Product = product,
                            Facility = facility,
                            PrimaryQuantity = waferSize,
                            PrimaryUnits = product.DefaultUnits,
                            Form = subMaterialData.Form,
                            Type = materialData.Type
                        };

                        subMaterials.Add(subMaterial);
                    }

                    wafers.Add(wafer);
                }

                subMaterials.Create();

                wafers.Create();

                // Associate the SubMaterials to the Wafer
                foreach (Material wafer in wafers)
                {
                    wafer.AddSubMaterials(subMaterials);
                }

                // Associate the Wafers to the Incoming Lot
                incomingLot.AddSubMaterials(wafers);
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

            foreach (Wafer wafer in materialData.Wafers)
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

            //---End DEE Code---

            return Input;
        }

        /// <summary>
        /// Names of Lot Wafers
        /// </summary>
        List<string> LotWafersNames = new List<string>();

        /// <summary>
        /// Names of Incoming Lot Wafers
        /// </summary>
        List<string> IncomingLotWafersNames = new List<string>();

        /// <summary>
        /// Validate if Wafers has Certificate Data
        /// </summary>
        bool WafersHasCertificateData(List<Wafer> wafers)
        {
            if (!wafers.IsNullOrEmpty())
            {
                foreach (Wafer wafer in wafers)
                {
                    if (!wafer.MaterialEDCData.IsNullOrEmpty())
                    {
                        return true;
                    }
                    else
                    {
                        this.WafersHasCertificateData(wafer.Wafers);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Validate if Lot Wafers and IncomingLotWafer has the same order
        /// </summary>
        bool WafersHasSameOrder(MaterialCollection lotWafers = null, List<Wafer> incomingLotWafers = null)
        {
            // Get all the Names of Lot Wafers
            if (!lotWafers.IsNullOrEmpty())
            {
                foreach (Material lotWafer in lotWafers)
                {
                    this.LotWafersNames.Add(lotWafer.Name);

                    if (lotWafer.SubMaterialCount > 0)
                    {
                        lotWafer.LoadChildren();

                        this.WafersHasSameOrder(lotWafers: lotWafer.SubMaterials);
                    }
                }
            }

            // Get all the Names of Incoming Lot Wafers
            if (!incomingLotWafers.IsNullOrEmpty())
            {
                foreach (Wafer incomingLotWafer in incomingLotWafers)
                {
                    this.IncomingLotWafersNames.Add(incomingLotWafer.Name);

                    if (!incomingLotWafer.Wafers.IsNullOrEmpty())
                    {
                        this.WafersHasSameOrder(incomingLotWafers: incomingLotWafer.Wafers);
                    }
                }
            }

            // Compare the sequence between the two Names Lists 
            return Enumerable.SequenceEqual(this.LotWafersNames.OrderBy(order => order), this.IncomingLotWafersNames.OrderBy(order => order));
        }
    }
}
