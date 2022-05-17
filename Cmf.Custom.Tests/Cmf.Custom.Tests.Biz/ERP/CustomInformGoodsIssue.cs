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
        private const string StorageLocation = "TestStorageLocation";
        private const string Site = "TestSiteCode";

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            _scenario = new CustomExecutionScenario();
            _scenario.IsToAssingPOsToMaterials = true;
            _scenario.NumberOfProductionOrdersToGenerate = 1;
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.NumberOfChildMaterialsToGenerate = 5;
            _scenario.CustomReportConsumptionToSAP = new List<Dictionary<string, string>> {
                new Dictionary<string, string>
                {
                    { "Step", AMSOsramConstants.DefaultTestStepName },
                    { "Product", AMSOsramConstants.DefaultTestProductName },
                    { "Flow", AMSOsramConstants.DefaultTestFlowName },
                    { "MaterialType", AMSOsramConstants.DefaultMaterialType},
                    { "StorageLocation", StorageLocation}
                }
            };

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
        /// Description:
        ///     - Create a material associated with a production order
        ///     - Configure the Smart Table CustomReportConsumptionToSAP
        ///     - Perform Dispatch, TrackIn and Trackout
        ///
        /// Acceptance Citeria:
        ///     - On Trackout an Integration Entry must be generated
        ///     
        /// </summary>
        /// <TestCaseID>CustomInformGoodsIssue.CustomInformGoodsIssue_TrackOutOnFirstStepOfPO_CreateIntegrationEntry</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomInformGoodsIssue_TrackOutOnFirstStepOfPO_CreateIntegrationEntry()
        {
            ///<Step> Create a Production Order, a Lot and its wafers </Step>
            _scenario.Setup();

            Material material = _scenario.GeneratedLots.FirstOrDefault();

            ///<Step> Dispatch and Tracin the lot </Step>
            Resource resource = new Resource
            {
                Name = AMSOsramConstants.DefaultTestResourceName
            };
            resource.Load();

            material.ComplexDispatchAndTrackIn(resource);

            ///<Step> Trackout the lot </Step>
            material.TrackOut();
            Thread.Sleep(4000);

            ///<Step> Get the last Integration Entry generated after the trackout </Step>
            IntegrationEntry ie = CustomUtilities.GetIntegrationEntry(material.Name);
            ie.Load();

            //Necessary to load inner message
            IntegrationEntry integrationEntryInfo = new GetIntegrationEntryInput
            {
                Id = ie.Id
            }.GetIntegrationEntrySync().IntegrationEntry;
            string integrationMessage = Encoding.UTF8.GetString(integrationEntryInfo.IntegrationMessage.Message);

            ///<Step> Validate the content of the Integration Entry </Step>
            CustomReportToERPItem goodsIssueItem = CustomUtilities.DeserializeXmlToObject<CustomReportToERPItem>(integrationMessage);

            ///<ExpectedResult> The Integration Entry body contains the information regarding the material that was tracked out </ExpectedResult>
            Assert.IsTrue(goodsIssueItem.ProductionOrderNumber.Equals(_scenario.GeneratedProductionOrders[0].OrderNumber),
                $"The property ProductionOrderNumber should be equals to: {_scenario.GeneratedProductionOrders[0].OrderNumber}, instead is: {goodsIssueItem.ProductionOrderNumber}.");

            Assert.IsTrue(goodsIssueItem.ProductName.Equals(_scenario.ProductName),
                $"The property Product Name should be equals to: {_scenario.ProductName}, instead is: {goodsIssueItem.ProductName}.");

            Assert.IsTrue(goodsIssueItem.MaterialName.Equals(_scenario.GeneratedLots[0].Name),
                $"The property Material Name should be equals to: {_scenario.GeneratedLots[0].Name}, instead is: {goodsIssueItem.MaterialName}.");

            Assert.IsTrue(goodsIssueItem.Quantity.Equals(_scenario.NumberOfChildMaterialsToGenerate * _scenario.ScenarioQuantity),
                $"The property Quantity should be equals to: {_scenario.NumberOfChildMaterialsToGenerate * _scenario.ScenarioQuantity}, instead is: {goodsIssueItem.Quantity}.");

            Assert.IsTrue(goodsIssueItem.Units.Equals(_scenario.GeneratedProductionOrders[0].Units),
                $"The property Units should be equals to: {_scenario.GeneratedProductionOrders[0].Units}, instead is: {goodsIssueItem.Units}.");

            Assert.IsTrue(goodsIssueItem.MovementType.Equals("261"),
                $"The property Movement Type should be equals to: {"261"}, instead is: {goodsIssueItem.MovementType}.");

            Assert.IsTrue(goodsIssueItem.SubMaterialCount.Equals(_scenario.GeneratedLots[0].SubMaterialCount),
                $"The property Sub Material Count should be equals to: {_scenario.GeneratedLots[0].SubMaterialCount}, instead is: {goodsIssueItem.SubMaterialCount}.");

            Assert.IsTrue(goodsIssueItem.SAPStore.Equals(StorageLocation),
                $"The property Storage Location should be equals to: {StorageLocation}, instead is: {goodsIssueItem.SAPStore}.");

            Assert.IsTrue(goodsIssueItem.Site.Equals(Site),
                $"The property Site should be equals to: {Site}, instead is: {goodsIssueItem.Site}.");
        }

        /// <summary>
        /// Description:
        ///     - Create a material associated without a production order
        ///     - Configure the Smart Table CustomReportConsumptionToSAP
        ///     - Perform Dispatch, TrackIn and Trackout
        ///
        /// Acceptance Citeria:
        ///     - On Trackout an Integration Entry should not be generated
        ///     
        /// </summary>
        /// <TestCaseID>CustomInformGoodsIssue.CustomInformGoodsIssue_TrackOutLotWithoutPO_SkipIntegrationEntryCreation</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomInformGoodsIssue_TrackOutLotWithoutPO_SkipIntegrationEntryCreation()
        {

            ///<Step> Create a Production Order, a Lot and its wafers </Step>
            _scenario.IsToAssingPOsToMaterials = false;
            _scenario.Setup();

            Material material = _scenario.GeneratedLots.FirstOrDefault();

            ///<Step> Dispatch and Tracin the lot </Step>
            Resource resource = new Resource
            {
                Name = AMSOsramConstants.DefaultTestResourceName
            };
            resource.Load();

            material.ComplexDispatchAndTrackIn(resource);

            ///<Step> Trackout the lot </Step>
            material.TrackOut();
            Thread.Sleep(4000);

            ///<Step> Get the last Integration Entry generated after the trackout </Step>
            IntegrationEntry ie = CustomUtilities.GetIntegrationEntry(material.Name);
            Assert.IsNull(ie.Name, $"The Trackout operation should not have created the Integration Entry {ie.Name} for the Material {material.Name}.");
        }

        /// <summary>
        /// Description:
        ///     - Create a material associated with a production order
        ///     - Clear the Smart Table CustomReportConsumptionToSAP
        ///     - Perform Dispatch, TrackIn and Trackout
        ///
        /// Acceptance Citeria:
        ///     - On Trackout an Integration Entry should not be generated
        ///     
        /// </summary>
        /// <TestCaseID>CustomInformGoodsIssue.CustomInformGoodsIssue_TrackOutLotWithouST_SkipIntegrationEntryCreation</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomInformGoodsIssue_TrackOutLotWithouST_SkipIntegrationEntryCreation()
        {

            ///<Step> Create a Production Order, a Lot and its wafers </Step>
            _scenario.CustomReportConsumptionToSAP = new List<Dictionary<string, string>>();
            _scenario.SmartTablesToClearInSetup = new List<string> { AMSOsramConstants.CustomReportConsumptionToSAPSmartTable };
            _scenario.Setup();

            Material material = _scenario.GeneratedLots.FirstOrDefault();

            ///<Step> Dispatch and Tracin the lot </Step>
            Resource resource = new Resource
            {
                Name = AMSOsramConstants.DefaultTestResourceName
            };
            resource.Load();

            material.ComplexDispatchAndTrackIn(resource);

            ///<Step> Trackout the lot </Step>
            material.TrackOut();
            Thread.Sleep(4000);

            ///<Step> Get the last Integration Entry generated after the trackout </Step>
            IntegrationEntry ie = CustomUtilities.GetIntegrationEntry(material.Name);
            Assert.IsNull(ie.Name, $"The Trackout operation should not have created the Integration Entry {ie.Name} for the Material {material.Name}.");
        }

        /// <summary>
        /// Description:
        ///     - Create a material associated with a production order
        ///     - Configure the Smart Table CustomReportConsumptionToSAP
        ///     - Change Material to other step of the flow
        ///     - Perform Dispatch, TrackIn and Trackout
        ///
        /// Acceptance Citeria:
        ///     - On Trackout an Integration Entry should not be generated
        ///     
        /// </summary>
        /// <TestCaseID>CustomInformGoodsIssue.CustomInformGoodsIssue_TrackOutLot_SkipIntegrationEntryCreation</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomInformGoodsIssue_TrackOutLot_SkipIntegrationEntryCreation()
        {

            ///<Step> Create a Production Order, a Lot and its wafers </Step>
            _scenario.Setup();

            Material material = _scenario.GeneratedLots.FirstOrDefault();

            Step secondStep = new Step() { Name = AMSOsramConstants.DefaultTestSecondStepName };
            secondStep.Load();
            material.Flow.Load();
            material.ChangeFlowAndStep(material.Flow,secondStep);

            ///<Step> Dispatch and Tracin the lot </Step>
            Resource resource = new Resource
            {
                Name = AMSOsramConstants.DefaultTestResourceName
            };
            resource.Load();

            material.ComplexDispatchAndTrackIn(resource);

            ///<Step> Trackout the lot </Step>
            material.TrackOut();
            Thread.Sleep(4000);

            ///<Step> Get the last Integration Entry generated after the trackout </Step>
            IntegrationEntry ie = CustomUtilities.GetIntegrationEntry(material.Name);
            Assert.IsNull(ie.Name, $"The Trackout operation should not have created the Integration Entry {ie.Name} for the Material {material.Name}.");
        }
    }
}
