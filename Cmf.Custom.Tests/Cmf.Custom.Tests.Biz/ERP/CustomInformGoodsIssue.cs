using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.ERP.GoodsIssue;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmf.Custom.Tests.Biz.ERP
{
    [TestClass]
    public class CustomInformGoodsIssue
    {
        private CustomExecutionScenario _scenario;
        private CustomTearDownManager customTeardownManager = null;
        private const string StorageLocation = "TestStorageLocation";
        private const string Site = "TestSiteCode";
        private string Quantity;

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
                    { "Step", amsOSRAMConstants.DefaultTestStepName },
                    { "Product", amsOSRAMConstants.DefaultTestProductName },
                    { "Flow", amsOSRAMConstants.DefaultTestFlowName },
                    { "MaterialType", amsOSRAMConstants.DefaultMaterialType},
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
                Name = amsOSRAMConstants.DefaultTestResourceName
            };
            resource.Load();

            material.ComplexDispatchAndTrackIn(resource);

            ///<Step> Trackout the lot </Step>
            material.TrackOut();

            ///<Step> Get the last Integration Entry generated after the trackout </Step>
            IntegrationEntry ie = LoadIntegrationEntry(material.Name);
            string integrationMessage = Encoding.UTF8.GetString(ie.IntegrationMessage.Message);

            Quantity = (_scenario.NumberOfChildMaterialsToGenerate * _scenario.ScenarioQuantity).ToString();

            string movementTypeConfigValue = ConfigUtilities.GetConfigValue(amsOSRAMConstants.DefaultGoodsIssueMovementTypeConfig) as string;

            ///<Step> Validate the content of the Integration Entry </Step>
            CustomReportToERPItem goodsIssueItem = CustomUtilities.DeserializeXmlToObject<CustomReportToERPItem>(integrationMessage);

            ///<ExpectedResult> The Integration Entry body contains the information regarding the material that was tracked out </ExpectedResult>
            Assert.AreEqual(_scenario.GeneratedProductionOrders[0].OrderNumber, goodsIssueItem.ProductionOrderNumber,
                            $"The property ProductionOrderNumber should be the value {_scenario.GeneratedProductionOrders[0].OrderNumber}.");
            Assert.AreEqual(_scenario.ProductName, goodsIssueItem.ProductName,
                            $"The property Product Name should be the value {_scenario.ProductName}.");
            Assert.AreEqual(_scenario.GeneratedLots[0].Name, goodsIssueItem.MaterialName,
                            $"The property Material Name should be the value {_scenario.GeneratedLots[0].Name}.");
            Assert.AreEqual(Quantity, goodsIssueItem.Quantity,
                            $"The property Quantity should be the value {Quantity}.");
            Assert.AreEqual(_scenario.GeneratedProductionOrders[0].Units, goodsIssueItem.Units,
                            $"The property Units should be the value {_scenario.GeneratedProductionOrders[0].Units}.");
            Assert.AreEqual(Site, goodsIssueItem.Site, $"The property Site should be the value {Site}.");
            Assert.AreEqual(movementTypeConfigValue, goodsIssueItem.MovementType, $"The property Movement Type should be the value {movementTypeConfigValue}.");
            Assert.AreEqual(StorageLocation, goodsIssueItem.SAPStore, $"The property Storage Location should be the value {StorageLocation}.");

            #region Future Validation

            //Batch
            //Assert.IsTrue(goodsIssueItem.BatchName);
            //MatRecNr
            //Assert.IsTrue(goodsIssueItem.MatRecNr);
            //MatCalYear
            //Assert.IsTrue(goodsIssueItem.MatCalYear); 

            #endregion Future Validation
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
                Name = amsOSRAMConstants.DefaultTestResourceName
            };
            resource.Load();

            material.ComplexDispatchAndTrackIn(resource);

            ///<Step> Trackout the lot </Step>
            material.TrackOut();

            ///<Step> Get the last Integration Entry generated after the trackout </Step>
            IntegrationEntry ie = LoadIntegrationEntry(material.Name);
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
            _scenario.SmartTablesToClearInSetup = new List<string> { amsOSRAMConstants.CustomReportConsumptionToSAPSmartTable };
            _scenario.Setup();

            Material material = _scenario.GeneratedLots.FirstOrDefault();

            ///<Step> Dispatch and Tracin the lot </Step>
            Resource resource = new Resource
            {
                Name = amsOSRAMConstants.DefaultTestResourceName
            };
            resource.Load();

            material.ComplexDispatchAndTrackIn(resource);

            ///<Step> Trackout the lot </Step>
            material.TrackOut();

            ///<Step> Get the last Integration Entry generated after the trackout </Step>            
            IntegrationEntry ie = LoadIntegrationEntry(material.Name);
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

            Step secondStep = new Step() { Name = amsOSRAMConstants.DefaultTestSecondStepName };
            secondStep.Load();
            material.Flow.Load();
            material.ChangeFlowAndStep(material.Flow, secondStep);

            ///<Step> Dispatch and Tracin the lot </Step>
            Resource resource = new Resource
            {
                Name = amsOSRAMConstants.DefaultTestResourceName
            };
            resource.Load();

            material.ComplexDispatchAndTrackIn(resource);

            ///<Step> Trackout the lot </Step>
            material.TrackOut();

            ///<Step> Get the last Integration Entry generated after the trackout </Step>
            IntegrationEntry ie = LoadIntegrationEntry(material.Name);
            Assert.IsNull(ie.Name, $"The Trackout operation should not have created the Integration Entry {ie.Name} for the Material {material.Name}.");
        }

        /// <summary>
        /// Load Integration Entry informations
        /// </summary>
        /// <param name="materialName">Material name</param>
        /// <returns>An instance of a Integration Entrie</returns>
        private IntegrationEntry LoadIntegrationEntry(string materialName)
        {
            IntegrationEntry integrationEntry = null;

            // Validate that Integration Entry was created
            GenericUtilities.WaitFor(() =>
            {
                integrationEntry = CustomUtilities.GetIntegrationEntry(materialName);

                return integrationEntry != null;
            });

            Assert.IsNotNull(integrationEntry, $"It should have been created an Integration Entry.");

            // Load integration entry
            integrationEntry.Load();

            if (integrationEntry.Name is not null)
            {
                //Necessary to load inner message
                integrationEntry = new GetIntegrationEntryInput
                {
                    Id = integrationEntry.Id
                }.GetIntegrationEntrySync().IntegrationEntry;
            }

            return integrationEntry;
        }
    }
}
