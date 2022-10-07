using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.ERP.Material;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.ERP
{
    [TestClass]
    public class CustomIncomingMaterialLotCreation
    {
        private CustomExecutionScenario customExecutionScenario;

        private CustomTearDownManager customTearDownManager;

        private MaterialCollection materials;

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            this.customExecutionScenario = new CustomExecutionScenario();

            this.customTearDownManager = new CustomTearDownManager();

            this.materials = new MaterialCollection();
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            //if (this.materials.Count > 0)
            //{
            //    this.materials.Load();

            //    foreach (Material material in this.materials)
            //    {
            //        if (material.HoldCount > 0)
            //        {
            //            material.LoadRelation("MaterialHoldReason");

            //            EntityRelationCollection materialHoldReasons = material.RelationCollection["MaterialHoldReason"];

            //            foreach (MaterialHoldReason materialHoldReason in materialHoldReasons)
            //            {
            //                material.ReleaseByReason(materialHoldReason);
            //            }
            //        }
            //    }

            //    this.materials.TerminateMaterialCollection();
            //}

            //if (customTearDownManager != null)
            //{
            //    customTearDownManager.TearDownSequentially();
            //}

            //if (customExecutionScenario != null)
            //{
            //    customExecutionScenario.CompleteCleanUp();
            //}
        }

        /// <summary>
        /// Description:
        ///     - Create a Material
        ///     - The information when the Material is created is not certified
        ///     - The information when the Material is updated is certified
        /// 
        /// Acceptance Citeria:
        ///     - Process Material and associate Hold Reason "Out of Spec"
        ///     - Process Material and disassociate Hold Reason "Out of Spec"
        ///  
        /// </summary>
        /// <TestCaseID>CustomIncomingMaterialLotCreation_SendERPMessage_HappyPath</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomIncomingMaterialLotCreation_SendERPMessage_HappyPath()
        {
            this.materials = new MaterialCollection();

            #region Read XML Samples

            string goodsUncertificatedSample = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptUncertificated.xml");

            string goodsCertificateSample = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptCertificate.xml");

            GoodsReceiptCertificate goodsUncertificated = new GoodsReceiptCertificate();

            goodsUncertificated = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(goodsUncertificatedSample);

            string materialName = goodsUncertificated.Material.Name = Guid.NewGuid().ToString("N");

            string subMaterialName = goodsUncertificated.Material.Wafers[0].Name = Guid.NewGuid().ToString("N");

            GoodsReceiptCertificate goodsCertificate = new GoodsReceiptCertificate();

            goodsCertificate = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(goodsCertificateSample);

            goodsCertificate.Material.Name = materialName;

            goodsCertificate.Material.Wafers[0].Name = subMaterialName;

            #endregion

            #region Setup Uncertificated Sample

            customExecutionScenario.SmartTablesToClearInSetup = new List<string> { "MaterialDataCollectionContext" };

            customExecutionScenario.MaterialDCContext = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string> {
                        { "Step", "M2-SL-Wafer-Start-07301F001_E" },
                        { "Operation", "Certificate" },
                        { "DataCollection", "ProductCertificateDataUnitTest" },
                        { "DataCollectionLimitSet", "ProductCertificateDataLimitSetUnitTest" },
                        { "DataCollectionType", "Immediate" }
                    }
            };

            customExecutionScenario.IsToSendIncomingMaterial = true;

            customExecutionScenario.GoodsReceiptCertificate = goodsUncertificated;

            customExecutionScenario.Setup();

            Collection<string> attributes = new Collection<string>()
            {
                    goodsUncertificated.Material.MaterialAttributes[0].Name,
                    goodsUncertificated.Material.MaterialAttributes[1].Name
            };

            ///<Step> Material product properties.</Step>
            ///<ExpectedValue> Material should have the information sent on the ERP message.</ExpectedValue>
            Material uncertificatedMaterial = new Material();

            uncertificatedMaterial.Load(materialName, 1);

            uncertificatedMaterial.LoadAttributes(attributes);

            uncertificatedMaterial.LoadChildren();

            customTearDownManager.Push(uncertificatedMaterial);

            this.materials.Add(uncertificatedMaterial);

            ///<Step> Validate material properties. </Step>
            ///<ExpectedValue> Material should have the information sent on the ERP message. </ExpectedValue>
            Assert.IsTrue(uncertificatedMaterial.Name.Equals(goodsUncertificated.Material.Name), $"Material Name should be: {uncertificatedMaterial.Name}, instead is: {goodsUncertificated.Material.Name}");

            Assert.IsTrue(uncertificatedMaterial.Product.Name.Equals(goodsUncertificated.Material.Product), $"Product Name should be: {uncertificatedMaterial.Product.Name}, instead is: {goodsUncertificated.Material.Product}");

            Assert.IsTrue(uncertificatedMaterial.Type.Equals(goodsUncertificated.Material.Type), $"Material Type should be: {uncertificatedMaterial.Type}, instead is: {goodsUncertificated.Material.Type}");

            Assert.IsTrue(uncertificatedMaterial.CurrentMainState.StateModel.Name.Equals(goodsUncertificated.Material.StateModel), $"State Model should be: {uncertificatedMaterial.CurrentMainState.StateModel.Name}, instead is: {goodsUncertificated.Material.StateModel}");

            Assert.IsTrue(uncertificatedMaterial.CurrentMainState.CurrentState.Name.Equals(goodsUncertificated.Material.State), $"Material State should be: {uncertificatedMaterial.CurrentMainState.CurrentState.Name}, instead is: {goodsUncertificated.Material.State}");

            Assert.IsTrue(uncertificatedMaterial.Form.Equals(goodsUncertificated.Material.Form), $"Material Form should be: {uncertificatedMaterial.Form}, instead is: {goodsUncertificated.Material.Form}");

            Assert.IsTrue(uncertificatedMaterial.Facility.Name.Equals(goodsUncertificated.Material.Facility), $"Facility should be: {uncertificatedMaterial.Facility.Name}, instead is: {goodsUncertificated.Material.Facility}");

            Assert.IsTrue(uncertificatedMaterial.Flow.Name.Equals(goodsUncertificated.Material.Flow), $"Flow should be: {uncertificatedMaterial.Flow.Name}, instead is: {goodsUncertificated.Material.Flow}");

            Assert.IsTrue(uncertificatedMaterial.Step.Name.Equals(goodsUncertificated.Material.Step), $"Step should be: {uncertificatedMaterial.Step.Name}, instead is: {goodsUncertificated.Material.Step}");

            ///<Step> Validate Material attributes. </Step>
            ///<ExpectedValue> The 2 Material attributes should be updated. </ExpectedValue>
            Assert.IsTrue(uncertificatedMaterial.Attributes[goodsUncertificated.Material.MaterialAttributes[0].Name].Equals(goodsUncertificated.Material.MaterialAttributes[0].value), $"Material attribute {goodsUncertificated.Material.MaterialAttributes[0].Name} should be {goodsUncertificated.Material.MaterialAttributes[0].value}, but was {uncertificatedMaterial.Attributes[goodsUncertificated.Material.MaterialAttributes[0].Name]}");

            Assert.IsTrue(uncertificatedMaterial.Attributes[goodsUncertificated.Material.MaterialAttributes[1].Name].Equals(goodsUncertificated.Material.MaterialAttributes[1].value), $"Material attribute {goodsUncertificated.Material.MaterialAttributes[1].Name} should be {goodsUncertificated.Material.MaterialAttributes[1].value}, but was {uncertificatedMaterial.Attributes[goodsUncertificated.Material.MaterialAttributes[1].Name]}");

            for (int i = 0; i < uncertificatedMaterial.SubMaterials.Count(); i++)
            {
                Material wafer = uncertificatedMaterial.SubMaterials[i];

                wafer.Load();

                wafer.LoadAttributes(new Collection<string> { goodsUncertificated.Material.Wafers[i].MaterialAttributes[0].Name });

                Assert.IsTrue(wafer.Name.Equals(goodsUncertificated.Material.Wafers[i].Name), $"Sub-Material Name should be: {wafer.Name} instead is: {goodsUncertificated.Material.Wafers[i].Name}");

                Assert.IsTrue(wafer.Form.Equals(goodsUncertificated.Material.Wafers[i].Form), $"Sub-Material Form should be: {wafer.Form} instead is: {goodsUncertificated.Material.Wafers[i].Form}");

                Assert.IsTrue(wafer.Attributes[goodsUncertificated.Material.Wafers[i].MaterialAttributes[0].Name].Equals(goodsUncertificated.Material.Wafers[i].MaterialAttributes[0].value), $"Sub-Material attribute {goodsUncertificated.Material.Wafers[i].MaterialAttributes[0].Name} should be {goodsUncertificated.Material.Wafers[i].MaterialAttributes[0].value}, but was {wafer.Attributes[goodsUncertificated.Material.Wafers[i].MaterialAttributes[0].Name]}");
            }

            ///<Step> Validate Material Hold Reasons.</Step>
            ///<ExpectedValue> The Material 2 Hold Reasons (Out of Spec & TestHoldReason1).</ExpectedValue>
            uncertificatedMaterial.LoadRelations(new Collection<string> { "MaterialHoldReason" });

            Assert.IsTrue(uncertificatedMaterial.HoldCount == 1, $"Material should have 1 reason instead has {uncertificatedMaterial.HoldCount}");

            MaterialHoldReason uncertificatedHoldReason = uncertificatedMaterial.MaterialHoldReasons.FirstOrDefault();

            uncertificatedHoldReason.TargetEntity.Load();

            Assert.IsTrue(uncertificatedHoldReason.TargetEntity.Name.Equals("Out Of Spec"), $"Material Hold Reason Name should be: Out of Spec instead is: {uncertificatedHoldReason.TargetEntity.Name}");

            Reason uncertificatedTestHoldReason = new Reason();

            uncertificatedTestHoldReason.Load("TestHoldReason1");

            uncertificatedMaterial.HoldMaterial(uncertificatedTestHoldReason);

            #endregion

            #region Setup Certificate Sample

            customExecutionScenario.IsToSendIncomingMaterial = true;

            customExecutionScenario.GoodsReceiptCertificate = goodsCertificate;

            customExecutionScenario.Setup();

            Material certificatedMaterial = new Material();

            certificatedMaterial.Load(goodsCertificate.Material.Name);

            customTearDownManager.Push(certificatedMaterial);

            ///<Step> Validate Material Hold Reasons.</Step>
            ///<ExpectedValue> The Material 2 Hold Reasons (TestHoldReason1 & TestHoldReason2).</ExpectedValue>

            certificatedMaterial.LoadRelations(new Collection<string> { "MaterialHoldReason" });

            Assert.IsTrue(certificatedMaterial.HoldCount == 1, $"Material should have one reason instead has {certificatedMaterial.HoldCount}");

            MaterialHoldReason certificateHoldReason = certificatedMaterial.MaterialHoldReasons.FirstOrDefault();

            certificateHoldReason.TargetEntity.Load();

            Assert.IsTrue(!certificateHoldReason.TargetEntity.Name.Equals("Out Of Spec"), $"Material Hold Reason Name should be: {certificateHoldReason.TargetEntity.Name} instead is: Out Of Spec");

            Reason certificatedTestHoldReason = new Reason();

            certificatedTestHoldReason.Load("TestHoldReason2");

            certificatedMaterial.HoldMaterial(certificatedTestHoldReason);

            #endregion
        }

        /// <summary>
        /// Description:
        ///     - Create lot through a Integration Entry
        ///         - The lot wafers information have EDC Data outside the limits 
        ///     - Another message is sent to update lot wafers
        ///         - The list of wafers is different
        /// Acceptance Citeria:
        ///     - The lot is put on hold with Out Of Spec Reason after creation
        ///     - Second message is not processed
        ///         - Error message should be: Material {lotName} wafers are different than the incoming wafer data. The error was reported by action CustomIncomingMaterialLotCreation.
        /// </summary>
        /// <TestCaseID>CustomIncomingMaterialLotCreation.CustomIncomingMaterialLotCreation_UpdateLotERPMessage_ErrorDifferentWaferList</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomIncomingMaterialLotCreation_UpdateLotERPMessage_ErrorDifferentWaferList()
        {
            ///<Step> Prepare another message to create a lot with edc data outside the limits </Step>
            string lotMessageSample = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptUncertificated.xml");
            GoodsReceiptCertificate lotMessage = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(lotMessageSample);
            string lotName = lotMessage.Material.Name = Guid.NewGuid().ToString("N");
            string waferlName = lotMessage.Material.Wafers[0].Name = Guid.NewGuid().ToString("N");

            ///<Step> Send ERP Message </Step>
            customExecutionScenario.SmartTablesToClearInSetup = new List<string> { "MaterialDataCollectionContext" };
            customExecutionScenario.MaterialDCContext = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string> {
                        { "Step", "M2-SL-Wafer-Start-07301F001_E" },
                        { "Operation", "Certificate" },
                        { "DataCollection", "ProductCertificateDataUnitTest" },
                        { "DataCollectionLimitSet", "ProductCertificateDataLimitSetUnitTest" },
                        { "DataCollectionType", "Immediate" }
                    }
            };
            customExecutionScenario.IsToSendIncomingMaterial = true;
            customExecutionScenario.GoodsReceiptCertificate = lotMessage;
            customExecutionScenario.Setup();

            ///<ExpecteResult> Material on Hold with reason out of spec </ExpecteResult>
            Material uncertificatedLot = new Material();
            uncertificatedLot.Load(lotName, 1);
            customTearDownManager.Push(uncertificatedLot);
            this.materials.Add(uncertificatedLot);

            Assert.IsTrue(uncertificatedLot.ObjectExists(), $"Lot with name {lotName} should have been created.");
            Assert.IsTrue(uncertificatedLot.HoldCount == 1, $"Material should have 1 reason instead has {uncertificatedLot.HoldCount}");

            uncertificatedLot.LoadRelations(new Collection<string> { "MaterialHoldReason" });
            MaterialHoldReason outOfSpec = uncertificatedLot.MaterialHoldReasons.FirstOrDefault();
            outOfSpec.TargetEntity.Load();
            Assert.IsTrue(outOfSpec.TargetEntity.Name.Equals("Out Of Spec"), $"Material Hold Reason Name should be: Out of Spec instead is: {outOfSpec.TargetEntity.Name}");

            ///<Step> Prepare another message to update the lot with different wafer list </Step>
            lotMessageSample = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptDifferentWafer.xml");
            lotMessage = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(lotMessageSample);
            lotMessage.Material.Name = lotName;
            lotMessage.Material.Wafers[0].Name = waferlName;

            ///<Step> Send ERP Message </Step>
            ///<ExpecteResult> Integration entry should not be processed </ExpecteResult>
            customExecutionScenario.IsToSendIncomingMaterial = true;
            customExecutionScenario.GoodsReceiptCertificate = lotMessage;
            customExecutionScenario.Setup();

            IntegrationEntry ie = customExecutionScenario.IntegrationEntries.Last();
            ie.Load();
            string expectedErrorMessage = $"The lot {lotName} contains 1 wafers instead of 2";
            Assert.IsTrue(ie.ResultDescription.Contains(expectedErrorMessage), $"Response for integration entry message should be {expectedErrorMessage}, instead is {ie.ResultDescription}.");

            ///<Step> Prepare another message to update the lot without wafers </Step>
            lotMessageSample = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptWithoutWafer.xml");
            lotMessage = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(lotMessageSample);
            lotMessage.Material.Name = lotName;

            ///<Step> Send ERP Message </Step>
            ///<ExpecteResult> Integration entry should not be processed </ExpecteResult>
            customExecutionScenario.IsToSendIncomingMaterial = true;
            customExecutionScenario.GoodsReceiptCertificate = lotMessage;
            customExecutionScenario.Setup();

            ie = customExecutionScenario.IntegrationEntries.Last();
            ie.Load();
            expectedErrorMessage = $"The lot {lotName} contains 1 wafers instead of 0";
            Assert.IsTrue(ie.ResultDescription != null && ie.ResultDescription.Contains(expectedErrorMessage),
                $"Response for integration entry message should be {expectedErrorMessage}, instead is {ie.ResultDescription}.");
        }

        /// <summary>
        /// Description:
        ///     - Create lot through a Integration Entry
        ///         - The lot wafers information have EDC Data outside the limits 
        ///     - Another message is sent to update lot Step
        /// Acceptance Citeria:
        ///     - The lot is put on hold with Out Of Spec Reason after creation
        ///     - Second message is not processed
        ///         - Error message should be: Material {lotName} is on different flow or step. The error was reported by action CustomIncomingMaterialLotCreation.
        /// </summary>
        /// <TestCaseID>CustomIncomingMaterialLotCreation.CustomIncomingMaterialLotCreation_UpdateLotERPMessage_ErrorDifferentStep</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomIncomingMaterialLotCreation_UpdateLotERPMessage_ErrorDifferentStep()
        {
            ///<Step> Prepare another message to create a lot with edc data outside the limits </Step>
            string lotMessageSample = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptUncertificated.xml");
            GoodsReceiptCertificate lotMessage = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(lotMessageSample);
            string lotName = lotMessage.Material.Name = Guid.NewGuid().ToString("N");
            lotMessage.Material.Wafers[0].Name = Guid.NewGuid().ToString("N");

            ///<Step> Send ERP Message </Step>
            customExecutionScenario.SmartTablesToClearInSetup = new List<string> { "MaterialDataCollectionContext" };
            customExecutionScenario.MaterialDCContext = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string> {
                        { "Step", "M2-SL-Wafer-Start-07301F001_E" },
                        { "Operation", "Certificate" },
                        { "DataCollection", "ProductCertificateDataUnitTest" },
                        { "DataCollectionLimitSet", "ProductCertificateDataLimitSetUnitTest" },
                        { "DataCollectionType", "Immediate" }
                    }
            };
            customExecutionScenario.IsToSendIncomingMaterial = true;
            customExecutionScenario.GoodsReceiptCertificate = lotMessage;
            customExecutionScenario.Setup();

            ///<ExpecteResult> Material on Hold with reason out of spec </ExpecteResult>
            Material uncertificatedLot = new Material();
            uncertificatedLot.Load(lotName, 1);
            customTearDownManager.Push(uncertificatedLot);
            this.materials.Add(uncertificatedLot);

            Assert.IsTrue(uncertificatedLot.ObjectExists(), $"Lot with name {lotName} should have been created.");
            Assert.IsTrue(uncertificatedLot.HoldCount == 1, $"Material should have 1 reason instead has {uncertificatedLot.HoldCount}");

            uncertificatedLot.LoadRelations(new Collection<string> { "MaterialHoldReason" });
            MaterialHoldReason outOfSpec = uncertificatedLot.MaterialHoldReasons.FirstOrDefault();
            outOfSpec.TargetEntity.Load();
            Assert.IsTrue(outOfSpec.TargetEntity.Name.Equals("Out Of Spec"), $"Material Hold Reason Name should be: Out of Spec instead is: {outOfSpec.TargetEntity.Name}");

            ///<Step> Prepare another message to update the lot with a different step </Step>
            ///<Step> Send ERP Message </Step>
            ///<ExpecteResult> Integration entry should not be processed </ExpecteResult>
            lotMessage.Material.Step = "R2D WAFER TRANSFER-00204F001_E";
            customExecutionScenario.IsToSendIncomingMaterial = true;
            customExecutionScenario.GoodsReceiptCertificate = lotMessage;
            customExecutionScenario.Setup();

            IntegrationEntry ie = customExecutionScenario.IntegrationEntries.Last();
            ie.Load();
            string expectedErrorMessage = $"The material {lotName} step can not be changed to {lotMessage.Material.Step}";
            Assert.IsTrue(ie.ResultDescription != null && ie.ResultDescription.Contains(expectedErrorMessage),
                $"Response for integration entry message should be {expectedErrorMessage}, instead is {ie.ResultDescription}.");
        }

        /// <summary>
        /// Description:
        ///     - The MaterialDataCollectionContext is cleared
        ///     - Send a message to create a lot 
        ///         - The lot wafers information have EDC Data outside the limits 
        /// Acceptance Citeria:
        ///     - Error message should be: Material {lotName} certification configuration is missing the certificate or the EDC Data.
        /// </summary>
        /// <TestCaseID>CustomIncomingMaterialLotCreation.CustomIncomingMaterialLotCreation_CreateLotERPMessage_ErrorCertificateContext</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomIncomingMaterialLotCreation_CreateLotERPMessage_ErrorCertificateContext()
        {
            ///<Step> Prepare another message to create a lot with edc data </Step>
            string lotMessageSample = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptUncertificated.xml");
            GoodsReceiptCertificate lotMessage = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(lotMessageSample);
            string lotName = lotMessage.Material.Name = Guid.NewGuid().ToString("N");
            lotMessage.Material.Wafers[0].Name = Guid.NewGuid().ToString("N");

            ///<Step> Send ERP Message </Step>
            customExecutionScenario.SmartTablesToClearInSetup = new List<string> { "MaterialDataCollectionContext" };
            customExecutionScenario.IsToSendIncomingMaterial = true;
            customExecutionScenario.GoodsReceiptCertificate = lotMessage;
            customExecutionScenario.Setup();

            ///<ExpecteResult> Integration entry should not be processed </ExpecteResult>
            IntegrationEntry ie = customExecutionScenario.IntegrationEntries.Last();
            ie.Load();
            string expectedErrorMessage = $"The material {lotName} certification configuration is missing the certificate or the EDC Data.";
            Assert.IsTrue(ie.ResultDescription != null && ie.ResultDescription.Contains(expectedErrorMessage),
                $"Response for integration entry message should be {expectedErrorMessage}, instead is {ie.ResultDescription}.");
        }

        /// <summary>
        /// Description:
        ///     - The MaterialDataCollectionContext has the correct configuration
        ///     - Send a message to create a lot 
        ///         - The does not have information for the wafers
        /// Acceptance Citeria:
        ///     - Error message should be: Material {lotName} certification configuration is missing the certificate or the EDC Data.
        /// </summary>
        /// <TestCaseID>CustomIncomingMaterialLotCreation.CustomIncomingMaterialLotCreation_CreateLotERPMessage_ErrorNoEDCData</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomIncomingMaterialLotCreation_CreateLotERPMessage_ErrorNoEDCData()
        {
            ///<Step> Prepare another message to create a lot with edc data </Step>
            string lotMessageSample = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptUncertificated.xml");
            GoodsReceiptCertificate lotMessage = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(lotMessageSample);
            string lotName = lotMessage.Material.Name = Guid.NewGuid().ToString("N");

            ///<Step> Send ERP Message </Step>
            customExecutionScenario.SmartTablesToClearInSetup = new List<string> { "MaterialDataCollectionContext" };
            customExecutionScenario.IsToSendIncomingMaterial = true;
            customExecutionScenario.GoodsReceiptCertificate = lotMessage;
            customExecutionScenario.Setup();

            ///<ExpecteResult> Integration entry should not be processed </ExpecteResult>
            IntegrationEntry ie = customExecutionScenario.IntegrationEntries.Last();
            ie.Load();
            string expectedErrorMessage = $"The material {lotName} certification configuration is missing the certificate or the EDC Data.";
            Assert.IsTrue(ie.ResultDescription != null && ie.ResultDescription.Contains(expectedErrorMessage),
                $"Response for integration entry message should be {expectedErrorMessage}, instead is {ie.ResultDescription}.");
        }

        /// <summary>
        /// Description:
        ///     - Create lot through a Integration Entry
        ///         - The lot wafers information have EDC Data outside the limits 
        ///     - Another message is sent to update lot Product
        /// Acceptance Citeria:
        ///     - The lot is put on hold with Out Of Spec Reason after creation
        ///     - Second message is not processed
        ///         - Error message should be: The material {lotName} product can not be changed to {newProduct}.
        /// </summary>
        /// <TestCaseID>CustomIncomingMaterialLotCreation.CustomIncomingMaterialLotCreation_UpdateLotERPMessage_ErrorDifferentProduct</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomIncomingMaterialLotCreation_UpdateLotERPMessage_ErrorDifferentProduct()
        {
            ///<Step> Prepare another message to create a lot with edc data outside the limits </Step>
            string lotMessageSample = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptUncertificated.xml");
            GoodsReceiptCertificate lotMessage = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(lotMessageSample);
            string lotName = lotMessage.Material.Name = Guid.NewGuid().ToString("N");
            lotMessage.Material.Wafers[0].Name = Guid.NewGuid().ToString("N");

            ///<Step> Send ERP Message </Step>
            customExecutionScenario.SmartTablesToClearInSetup = new List<string> { "MaterialDataCollectionContext" };
            customExecutionScenario.MaterialDCContext = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string> {
                        { "Step", "M2-SL-Wafer-Start-07301F001_E" },
                        { "Operation", "Certificate" },
                        { "DataCollection", "ProductCertificateDataUnitTest" },
                        { "DataCollectionLimitSet", "ProductCertificateDataLimitSetUnitTest" },
                        { "DataCollectionType", "Immediate" }
                    }
            };
            customExecutionScenario.IsToSendIncomingMaterial = true;
            customExecutionScenario.GoodsReceiptCertificate = lotMessage;
            customExecutionScenario.Setup();

            ///<ExpecteResult> Material on Hold with reason out of spec </ExpecteResult>
            Material uncertificatedLot = new Material();
            uncertificatedLot.Load(lotName, 1);
            customTearDownManager.Push(uncertificatedLot);
            this.materials.Add(uncertificatedLot);

            Assert.IsTrue(uncertificatedLot.ObjectExists(), $"Lot with name {lotName} should have been created.");
            Assert.IsTrue(uncertificatedLot.HoldCount == 1, $"Material should have 1 reason instead has {uncertificatedLot.HoldCount}");

            uncertificatedLot.LoadRelations(new Collection<string> { "MaterialHoldReason" });
            MaterialHoldReason outOfSpec = uncertificatedLot.MaterialHoldReasons.FirstOrDefault();
            outOfSpec.TargetEntity.Load();
            Assert.IsTrue(outOfSpec.TargetEntity.Name.Equals("Out Of Spec"), $"Material Hold Reason Name should be: Out of Spec instead is: {outOfSpec.TargetEntity.Name}");

            ///<Step> Prepare another message to update the lot with a differente product </Step>
            ///<Step> Send ERP Message </Step>
            ///<ExpecteResult> Integration entry should not be processed </ExpecteResult>
            lotMessage.Material.Product = "11063210";
            customExecutionScenario.IsToSendIncomingMaterial = true;
            customExecutionScenario.GoodsReceiptCertificate = lotMessage;
            customExecutionScenario.Setup();

            IntegrationEntry ie = customExecutionScenario.IntegrationEntries.Last();
            ie.Load();
            string expectedErrorMessage = $"The material {lotName} product can not be changed to {lotMessage.Material.Product}";
            Assert.IsTrue(ie.ResultDescription != null && ie.ResultDescription.Contains(expectedErrorMessage),
                $"Response for integration entry message should be {expectedErrorMessage}, instead is {ie.ResultDescription}.");
        }

        /// <summary>
        /// Description:
        ///     - Create a Material received from XML message
        /// 
        /// Acceptance Criteria:
        ///     - The Material has the structure defined in the XML message
        ///  
        /// </summary>
        /// <TestCaseID>CustomIncomingMaterialLotCreation_CreateLotFromMessage_CreateMultipleWafers</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomIncomingMaterialLotCreation_CreateLotFromMessage_CreateMultipleWafers()
        {
            // Load Incoming Lot message
            string incomingLotMessage = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptMultipleWafers.xml");

            // Deserialize message in a MaterialData object
            GoodsReceiptCertificate incomingLot = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(incomingLotMessage);

            // Set random name to Lot
            incomingLot.Material.Name = Guid.NewGuid().ToString("N");

            // Set random name to Logical Wafer
            incomingLot.Material.Wafers[0].Name = Guid.NewGuid().ToString("N");

            // Set random name to Wafer
            incomingLot.Material.Wafers[0].Wafers[0].Name = Guid.NewGuid().ToString("N");

            // Set scenario IsToSendIncomingMaterial property
            customExecutionScenario.IsToSendIncomingMaterial = true;

            // Set scenario GoodsReceiptCertificate property
            customExecutionScenario.GoodsReceiptCertificate = incomingLot;

            // Setup scenario
            customExecutionScenario.Setup();

            // Load created Lot
            Material createdLot = new Material()
            {
                Name = incomingLot.Material.Name
            };
            createdLot.Load(1);

            // Load created Lot attributes
            Collection<string> attributesNames = new Collection<string>();

            foreach (MaterialAttributes attribute in incomingLot.Material.MaterialAttributes)
            {
                attributesNames.Add(attribute.Name);
            }

            createdLot.LoadAttributes(attributesNames);

            // Push created Lot to TearDownManager
            customTearDownManager.Push(createdLot);

            // Validate created Lot
            ValidateCreatedMaterial(incomingLot.Material, createdLot);

            // Validate created Wafers
            ValidateCreatedWafers(incomingLot.Material.Wafers);
        }

        /// <summary>
        /// Description:
        ///     - Create a Lot through an Integration Entry
        ///         - The Production Order does not exists on MES
        /// Acceptance Criteria:
        ///     - Integration Entry will return a message about the Production Order does not exist in the MES.
        /// </summary>
        /// <TestCaseID>CustomIncomingMaterialLotCreation_CreateLotFromMessage_ErrorNonExistentProductionOrder</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomIncomingMaterialLotCreation_CreateLotFromMessage_ErrorNonExistentProductionOrder()
        {
            // Load Incoming Lot message
            string incomingLotMessage = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptCertificate.xml");

            // Deserialize message in a MaterialData object
            GoodsReceiptCertificate incomingLot = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(incomingLotMessage);

            // Set random name to Lot
            incomingLot.Material.Name = Guid.NewGuid().ToString("N");
            // Set random name to Logical Wafer
            incomingLot.Material.Wafers[0].Name = Guid.NewGuid().ToString("N");
            // Set non existing Production Order
            incomingLot.Material.ProductionOrder = "CMFNonExistentProductionOrder";

            // Set scenario IsToSendIncomingMaterial property
            customExecutionScenario.IsToSendIncomingMaterial = true;

            // Set scenario GoodsReceiptCertificate property
            customExecutionScenario.GoodsReceiptCertificate = incomingLot;

            // Setup scenario
            customExecutionScenario.Setup();

            // Get created Integration Entry context
            IntegrationEntry integrationEntry = customExecutionScenario.IntegrationEntries.Last();
            integrationEntry.Load();

            // Validate throw Message associated to Integration Entry
            string localizedMessage = string.Format(CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomProductionOrderDoesNotExists));
            StringAssert.Contains(integrationEntry.ResultDescription, string.Format(localizedMessage, incomingLot.Material.ProductionOrder), "The returned message is not as expected.");
        }

        /// <summary>
        /// Description:
        ///     - Create a Lot through an Integration Entry
        ///         - The Primary Quantity is missing
        /// Acceptance Criteria:
        ///     - Integration Entry will return a message about the Primary Quantity being missing/invalid.
        /// </summary>
        /// <TestCaseID>CustomIncomingMaterialLotCreation_CreateLotFromMessage_ErrorNonExistentPrimaryQuantity</TestCaseID>
        [TestMethod]
        public void CustomIncomingMaterialLotCreation_CreateLotFromMessage_ErrorNonExistentPrimaryQuantity()
        {
            // Load Incoming Lot message
            string incomingLotMessage = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptCertificate.xml");

            // Deserialize message in a MaterialData object
            GoodsReceiptCertificate incomingLot = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(incomingLotMessage);

            // Set random name to Lot
            incomingLot.Material.Name = Guid.NewGuid().ToString("N");

            // Set random name to Logical Wafer
            incomingLot.Material.Wafers[0].Name = Guid.NewGuid().ToString("N");

            // Set Primary Quantity null
            incomingLot.Material.PrimaryQuantity = null;

            // Set scenario IsToSendIncomingMaterial property
            customExecutionScenario.IsToSendIncomingMaterial = true;

            // Set scenario GoodsReceiptCertificate property
            customExecutionScenario.GoodsReceiptCertificate = incomingLot;

            // Setup scenario
            customExecutionScenario.Setup();

            // Get created Integration Entry context
            IntegrationEntry integrationEntry = customExecutionScenario.IntegrationEntries.Last();
            integrationEntry.Load();

            // Validate throw Message associated to Integration Entry
            string localizedMessage = string.Format(CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomInvalidPrimaryQuantity), incomingLot.Material.Name);
            StringAssert.Contains(integrationEntry.ResultDescription, string.Format(localizedMessage, incomingLot.Material.Name), "The returned message is not as expected.");
        }

        /// <summary>
        /// Description:
        ///     - Create a Lot through an Integration Entry
        ///         - The Primary Unit is missing
        /// Acceptance Criteria:
        ///     - Integration Entry will return a message about the Primary Unit being missing.
        /// </summary>
        /// <TestCaseID>CustomIncomingMaterialLotCreation_CreateLotFromMessage_ErrorNonExistentPrimaryUnit</TestCaseID>
        [TestMethod]
        public void CustomIncomingMaterialLotCreation_CreateLotFromMessage_ErrorNonExistentPrimaryUnit()
        {
            // Load Incoming Lot message
            string incomingLotMessage = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptCertificate.xml");

            // Deserialize message in a MaterialData object
            GoodsReceiptCertificate incomingLot = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(incomingLotMessage);

            // Set random name to Lot
            incomingLot.Material.Name = Guid.NewGuid().ToString("N");

            // Set random name to Logical Wafer
            incomingLot.Material.Wafers[0].Name = Guid.NewGuid().ToString("N");

            // Set Primary Unit null
            incomingLot.Material.PrimaryUnit = null;

            // Set scenario IsToSendIncomingMaterial property
            customExecutionScenario.IsToSendIncomingMaterial = true;

            // Set scenario GoodsReceiptCertificate property
            customExecutionScenario.GoodsReceiptCertificate = incomingLot;

            // Setup scenario
            customExecutionScenario.Setup();
            
            // Get created Integration Entry context
            IntegrationEntry integrationEntry = customExecutionScenario.IntegrationEntries.Last();
            integrationEntry.Load();

            // Validate throw Message associated to Integration Entry
            string localizedMessage = string.Format(CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomPrimaryUnitObjectNull), incomingLot.Material.Name);
            StringAssert.Contains(integrationEntry.ResultDescription, string.Format(localizedMessage, incomingLot.Material.Name), "The returned message is not as expected.");
        }

        /// <summary>
        /// Description:
        ///     - The MaterialDataCollectionContext has no configuration
        ///     - Send a message to create a lot 
        ///         - The Lot does not have information for the wafers
        /// Acceptance Criteria:
        ///     - Error message should be: Material {lotName} certification configuration is missing the certificate or the EDC Data.
        /// </summary>
        /// <TestCaseID>CustomIncomingMaterialLotCreation.CustomIncomingMaterialLotCreation_CreateLotERPMessage_ErrorNoSubMaterials</TestCaseID>
        [TestMethod]
        public void CustomIncomingMaterialLotCreation_CreateLotERPMessage_ErrorNoSubMaterials()
        {
            ///<Step> Prepare another message to create a lot with edc data </Step>
            string lotMessageSample = FileUtilities.LoadFile($@"ERP\Samples\SampleGoodsReceiptWithoutWafer.xml");
            GoodsReceiptCertificate lotMessage = CustomUtilities.DeserializeXmlToObject<GoodsReceiptCertificate>(lotMessageSample);
            string lotName = lotMessage.Material.Name = Guid.NewGuid().ToString("N");

            ///<Step> Send ERP Message </Step>
            customExecutionScenario.SmartTablesToClearInSetup = new List<string> { "MaterialDataCollectionContext" };
            customExecutionScenario.IsToSendIncomingMaterial = true;
            customExecutionScenario.GoodsReceiptCertificate = lotMessage;
            customExecutionScenario.Setup();

            ///<ExpecteResult> Integration entry should not be processed </ExpecteResult>
            IntegrationEntry ie = customExecutionScenario.IntegrationEntries.Last();
            ie.Load();
            string expectedErrorMessage = $"The material {lotName} certification configuration is missing the certificate or the EDC Data.";
            Assert.IsTrue(ie.ResultDescription != null && ie.ResultDescription.Contains(expectedErrorMessage),
                $"Response for integration entry message should be {expectedErrorMessage}, instead is {ie.ResultDescription}.");
        }

        #region Help methods

        /// <summary>
        /// Validate Created Wafers
        /// </summary>
        private void ValidateCreatedWafers(List<MaterialData> wafers)
        {
            for (int i = 0; i < wafers.Count; i++)
            {
                MaterialData wafer = wafers[i];

                Material subMaterial = new Material()
                {
                    Name = wafers[i].Name
                };

                subMaterial.Load(1);

                Collection<string> attributesNames = new Collection<string>();

                foreach (MaterialAttributes attribute in wafers[i].MaterialAttributes)
                {
                    attributesNames.Add(attribute.Name);
                }

                subMaterial.LoadAttributes(attributesNames);

                ValidateCreatedMaterial(wafer, subMaterial);

                if (subMaterial.SubMaterialCount > 0 && wafer.Wafers.Count > 0)
                {
                    ValidateCreatedWafers(wafer.Wafers);
                }
            }
        }

        /// <summary>
        /// Validate created Material
        /// </summary>
        private void ValidateCreatedMaterial(MaterialData materialData, Material material)
        {
            Assert.AreEqual(materialData.Name, material.Name, $"Material Name should be {materialData.Name}");

            Assert.AreEqual(materialData.Product, material.Product.Name, $"Product Name should be {materialData.Product}");

            Assert.AreEqual(materialData.Type, material.Type, $"Material Type should be {materialData.Type}");

            Assert.AreEqual(materialData.StateModel, material.CurrentMainState.StateModel.Name, $"State Model should be {materialData.StateModel}");

            Assert.AreEqual(materialData.State, material.CurrentMainState.CurrentState.Name, $"Material State should be {materialData.State}");

            Assert.AreEqual(materialData.Form, material.Form, $"Material Form should be {materialData.Form}");

            Assert.AreEqual(materialData.Facility, material.Facility.Name, $"Facility should be {materialData.Facility}");

            Assert.AreEqual(materialData.Flow, material.Flow.Name, $"Flow should be {materialData.Flow}");

            Assert.AreEqual(materialData.Step, material.Step.Name, $"Step should be {materialData.Step}");

            Assert.AreEqual(decimal.Parse(materialData.PrimaryQuantity), material.PrimaryQuantity, $"Primary Quantity should be {string.Format("{0:0.00}", materialData.PrimaryQuantity)}");

            Assert.AreEqual(materialData.PrimaryUnit, material.PrimaryUnits, $"Primary Units should be {materialData.PrimaryUnit}");

            // Validate SecondaryQuantity and SecondaryUnits
            if (!string.IsNullOrWhiteSpace(materialData.SecondaryQuantity) && !string.IsNullOrWhiteSpace(materialData.SecondaryUnit))
            {
                Assert.AreEqual(string.Format("{0:0.00}", materialData.SecondaryQuantity), string.Format("{0:0.00}", material.SecondaryQuantity), $"Primary Quantity should be {string.Format("{0:0.00}", materialData.SecondaryQuantity)}");

                Assert.AreEqual(materialData.SecondaryUnit, material.SecondaryUnits, $"Primary Units should be {materialData.SecondaryUnit}");
            }

            // Validate ProductionOrder
            if (!string.IsNullOrWhiteSpace(materialData.ProductionOrder))
            {
                Assert.AreEqual(materialData.ProductionOrder, material.ProductionOrder.Name, $"Production Order should be {materialData.ProductionOrder}");
            }

            // Validate the Material Attributes
            if (material.Attributes.Count > 0 && materialData.MaterialAttributes.Count > 0)
            {
                for (int i = 0; i < materialData.MaterialAttributes.Count; i++)
                {
                    Assert.IsTrue(material.AttributeEquals(materialData.MaterialAttributes[i].Name, materialData.MaterialAttributes[i].value), $"Sub-Material attribute {materialData.MaterialAttributes[i].Name} should have the value {materialData.MaterialAttributes[i].value}.");
                }
            }
        }

        #endregion Help methods
    }
}
