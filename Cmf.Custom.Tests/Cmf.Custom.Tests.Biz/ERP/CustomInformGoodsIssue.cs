using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.ERP.GoodsIssue;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace Cmf.Custom.Tests.Biz.ERP
{
    [TestClass]
    public class CustomInformGoodsIssue
    {
        private CustomExecutionScenario _scenario;
        private CustomTearDownManager customTeardownManager = null;

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            _scenario = new CustomExecutionScenario();
            customTeardownManager = new CustomTearDownManager();
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (customTeardownManager != null)
                customTeardownManager.TearDownSequentially();

            if (_scenario != null)
            {
                _scenario.CompleteCleanUp();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <TestCaseID>CustomInformGoodsIssue.CustomInformGoodsIssue_TrackOutOnFirstStepOfPO_CreateIntegrationEntry</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomInformGoodsIssue_TrackOutOnFirstStepOfPO_CreateIntegrationEntry()
        {
            ///<Step> Create a Production Order, a Lot and its wafers </Step>
            _scenario.IsToAssingPOsToMaterials = true;
            _scenario.NumberOfProductionOrdersToGenerate = 1;
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.NumberOfChildMaterialsToGenerate = 5;
            _scenario.Setup();

            Material material = _scenario.GeneratedLots.FirstOrDefault();

            // dispatch and Trackin
            Resource resource = new Resource
            {
                Name = AMSOsramConstants.DefaultTestResourceName
            };

            resource.Load();

            material.ComplexDispatchAndTrackIn(resource);

            material.TrackOut();

            Thread.Sleep(4000);

            IntegrationEntry ie = CustomUtilities.GetLastIntegrationEntry(AMSOsramConstants.CustomPostGoodsIssueToSAPMessageType);

            //Necessary to load inner message
            IntegrationEntry integrationEntryInfo = new GetIntegrationEntryInput
            {
                Id = ie.Id
            }.GetIntegrationEntrySync().IntegrationEntry;

            string integrationMessage = Encoding.UTF8.GetString(integrationEntryInfo.IntegrationMessage.Message);

            CustomReportToERPItem goodsIssueItem = CustomUtilities.DeserializeXmlToObject<CustomReportToERPItem>(integrationMessage);

            Assert.IsTrue(goodsIssueItem.ProductionOrderNumber.Equals(_scenario.GeneratedProductionOrders[0].OrderNumber),
                $"The property ProductionOrderNumber should be equals to: {_scenario.GeneratedProductionOrders[0].OrderNumber}, instead is: {goodsIssueItem.ProductionOrderNumber}.");

            Assert.IsTrue(goodsIssueItem.ProductName.Equals(_scenario.ProductName),
                $"The property Product Name should be equals to: {_scenario.ProductName}, instead is: {goodsIssueItem.ProductName}.");

            Assert.IsTrue(goodsIssueItem.MaterialName.Equals(_scenario.GeneratedLots[0].Name),
                $"The property Product Name should be equals to: {_scenario.GeneratedLots[0].Name}, instead is: {goodsIssueItem.MaterialName}.");

            Assert.IsTrue(goodsIssueItem.Quantity.Equals(_scenario.NumberOfChildMaterialsToGenerate * _scenario.ScenarioQuantity),
                $"The property Product Name should be equals to: {_scenario.NumberOfChildMaterialsToGenerate * _scenario.ScenarioQuantity}, instead is: {goodsIssueItem.Quantity}.");

            Assert.IsTrue(goodsIssueItem.Units.Equals(_scenario.GeneratedProductionOrders[0].Units),
                $"The property Product Name should be equals to: {_scenario.GeneratedProductionOrders[0].Units}, instead is: {goodsIssueItem.Units}.");

            Assert.IsTrue(goodsIssueItem.MovementType.Equals("261"),
                $"The property Product Name should be equals to: {"261"}, instead is: {goodsIssueItem.MovementType}.");

            Assert.IsTrue(goodsIssueItem.SubMaterialCount.Equals(_scenario.GeneratedLots[0].SubMaterialCount),
                $"The property Product Name should be equals to: {_scenario.GeneratedLots[0].SubMaterialCount}, instead is: {goodsIssueItem.SubMaterialCount}.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <TestCaseID>CustomInformGoodsIssue.CustomInformGoodsIssue_TrackOutLotWithoutPO_CreateIntegrationEntry</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomInformGoodsIssue_TrackOutLotWithoutPO_CreateIntegrationEntry()
        {
            ///<Step> Create a Production Order, a Lot and its wafers </Step>
            _scenario.IsToAssingPOsToMaterials = false;
            _scenario.NumberOfProductionOrdersToGenerate = 1;
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.NumberOfChildMaterialsToGenerate = 5;
            _scenario.Setup();

            Material material = _scenario.GeneratedLots.FirstOrDefault();

            // dispatch and Trackin
            Resource resource = new Resource
            {
                Name = AMSOsramConstants.DefaultTestResourceName
            };

            resource.Load();

            material.ComplexDispatchAndTrackIn(resource);

            material.TrackOut();

            Thread.Sleep(4000);

            IntegrationEntry ie = CustomUtilities.GetLastIntegrationEntry(AMSOsramConstants.CustomPostGoodsIssueToSAPMessageType);

            //Necessary to load inner message
            IntegrationEntry integrationEntryInfo = new GetIntegrationEntryInput
            {
                Id = ie.Id
            }.GetIntegrationEntrySync().IntegrationEntry;

            string integrationMessage = Encoding.UTF8.GetString(integrationEntryInfo.IntegrationMessage.Message);

            CustomReportToERPItem goodsIssueItem = CustomUtilities.DeserializeXmlToObject<CustomReportToERPItem>(integrationMessage);

            Assert.IsTrue(goodsIssueItem.ProductionOrderNumber.Equals(_scenario.GeneratedProductionOrders[0].OrderNumber),
                $"The property ProductionOrderNumber should be equals to: {_scenario.GeneratedProductionOrders[0].OrderNumber}, instead is: {goodsIssueItem.ProductionOrderNumber}.");

            Assert.IsTrue(goodsIssueItem.ProductName.Equals(_scenario.ProductName),
                $"The property Product Name should be equals to: {_scenario.ProductName}, instead is: {goodsIssueItem.ProductName}.");

            Assert.IsTrue(goodsIssueItem.MaterialName.Equals(_scenario.GeneratedLots[0].Name),
                $"The property Product Name should be equals to: {_scenario.GeneratedLots[0].Name}, instead is: {goodsIssueItem.MaterialName}.");

            Assert.IsTrue(goodsIssueItem.Quantity.Equals(_scenario.NumberOfChildMaterialsToGenerate * _scenario.ScenarioQuantity),
                $"The property Product Name should be equals to: {_scenario.NumberOfChildMaterialsToGenerate * _scenario.ScenarioQuantity}, instead is: {goodsIssueItem.Quantity}.");

            Assert.IsTrue(goodsIssueItem.Units.Equals(_scenario.GeneratedProductionOrders[0].Units),
                $"The property Product Name should be equals to: {_scenario.GeneratedProductionOrders[0].Units}, instead is: {goodsIssueItem.Units}.");

            Assert.IsTrue(goodsIssueItem.MovementType.Equals("261"),
                $"The property Product Name should be equals to: {"261"}, instead is: {goodsIssueItem.MovementType}.");

            Assert.IsTrue(goodsIssueItem.SubMaterialCount.Equals(_scenario.GeneratedLots[0].SubMaterialCount),
                $"The property Product Name should be equals to: {_scenario.GeneratedLots[0].SubMaterialCount}, instead is: {goodsIssueItem.SubMaterialCount}.");
        }
    }
}
