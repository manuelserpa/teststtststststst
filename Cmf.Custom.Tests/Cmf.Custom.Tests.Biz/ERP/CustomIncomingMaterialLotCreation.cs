using Cmf.Custom.Tests.Biz.Common.ERP.Material;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (materials.Count > 0)
            {
                materials.Load();

                foreach (Material material in materials)
                {
                    if (material.HoldCount > 0)
                    {
                        material.LoadRelation("MaterialHoldReason");

                        EntityRelationCollection materialHoldReasons = material.RelationCollection["MaterialHoldReason"];

                        foreach (MaterialHoldReason materialHoldReason in materialHoldReasons)
                        {
                            material.ReleaseByReason(materialHoldReason);
                        }
                    }
                }

                materials.TerminateMaterialCollection();
            }

            if (customTearDownManager != null)
            {
                customTearDownManager.TearDownSequentially();
            }

            if (customExecutionScenario != null)
            {
                customExecutionScenario.CompleteCleanUp();
            }
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

            string goodsUncertificatedSample = FileUtilities.LoadXmlFile($@"ERP\Samples\SampleGoodsReceiptUncertificated.xml");

            string goodsCertificateSample = FileUtilities.LoadXmlFile($@"ERP\Samples\SampleGoodsReceiptCertificate.xml");

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

            materials.Add(uncertificatedMaterial);

            ///<Step> Validate material properties. </Step>
            ///<ExpectedValue> Material should have the information sent on the ERP message. </ExpectedValue>
            Assert.IsTrue(uncertificatedMaterial.Name.Equals(goodsUncertificated.Material.Name), $"Material Name should be: {uncertificatedMaterial.Name}, instead is: {goodsUncertificated.Material.Name}");

            Assert.IsTrue(uncertificatedMaterial.Product.Name.Equals(goodsUncertificated.Material.Product), $"Product Name should be: {uncertificatedMaterial.Product.Name}, instead is: {goodsUncertificated.Material.Product}");

            Assert.IsTrue(uncertificatedMaterial.Type.Equals(goodsUncertificated.Material.Type), $"Material Type should be: {uncertificatedMaterial.Type}, instead is: {goodsUncertificated.Material.Type}");

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

            Assert.IsTrue(certificatedMaterial.HoldCount == 1, $"Material should have one reason instead has {uncertificatedMaterial.HoldCount}");

            MaterialHoldReason certificateHoldReason = certificatedMaterial.MaterialHoldReasons.FirstOrDefault();

            certificateHoldReason.TargetEntity.Load();

            Assert.IsTrue(!certificateHoldReason.TargetEntity.Name.Equals("Out Of Spec"), $"Material Hold Reason Name should be: {certificateHoldReason.TargetEntity.Name} instead is: Out Of Spec");

            Reason certificatedTestHoldReason = new Reason();

            certificatedTestHoldReason.Load("TestHoldReason2");

            certificatedMaterial.HoldMaterial(certificatedTestHoldReason);

            #endregion
        }
    }
}
