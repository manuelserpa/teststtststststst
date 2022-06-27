using Cmf.Custom.Tests.Biz.Common.ERP.ProductionOrder;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace Cmf.Custom.Tests.Biz.ERP
{
    [TestClass]
    public class CustomImportProductionOrders
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
        /// Description:
        ///     - Create 1 Integration Entry with the list of Production Orders to create
        ///     - Create 1 Integration Entry for the Production Order
        ///     - Create 1 Production Order
        /// 
        /// Acceptance Citeria:
        ///     - Both Integration Entries are processed
        ///     - The Production Order is created successfully 
        ///  
        /// </summary>
        /// <TestCaseID>CustomImportProductionOrders.CustomImportProductionOrders_SendPOImport_HappyPath</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomImportProductionOrders_SendPOImport_HappyPath()
        {
            ///<Step> Create Message to send to MES System </Step>
            string productionOrdersSample = FileUtilities.LoadFile($@"ERP\Samples\SampleImportProductionOrder.xml");
            CustomImportProductionOrderCollection productionOrdersMessage = new CustomImportProductionOrderCollection();
            productionOrdersMessage = CustomUtilities.DeserializeXmlToObject<CustomImportProductionOrderCollection>(productionOrdersSample);

            productionOrdersMessage[0].Name = Guid.NewGuid().ToString("N");
            productionOrdersMessage[0].OrderNumber = Guid.NewGuid().ToString("N").Substring(0, 3);

            _scenario.CustomImportProductionOrderCollection = productionOrdersMessage;
            _scenario.Setup();

            ///<Step> Validate integration entry with the list of Production Orders </Step>
            ///<ExpectedResult> Integration entry should be processed </ExpectedResult>
            Assert.IsTrue(_scenario.IntegrationEntries.Count > 0, "Integration Entries should have been created");
            foreach (IntegrationEntry ie in _scenario.IntegrationEntries)
            {
                Assert.IsTrue(ie.IsIntegrationEntryProcessed(), "Integration Entry was not processed. Error Message: {0}", ie.ResultDescription);
            }

            ///<Step> Validate integration entry with the Production Order </Step>
            ///<ExpectedResult> Integration entry should be processed </ExpectedResult>
            IntegrationEntry productionOrderIntegrationEntry = CustomUtilities.GetIntegrationEntry(productionOrdersMessage[0].Name);
            Assert.IsTrue(productionOrderIntegrationEntry.IsIntegrationEntryProcessed(), "Integration Entry was not processed. Error Message: {0}", productionOrderIntegrationEntry.ResultDescription);

            ProductionOrder po = new ProductionOrder()
            {
                Name = productionOrdersMessage[0].Name
            };

            Assert.IsTrue(po.ObjectExists(), $"Production Order named {productionOrdersMessage[0].Name} should have been created.");

            customTeardownManager.Push(po);
            po.Load();
            po.Product.Load();
            po.Facility.Load();

            ///<Step> Validate Production Order created </Step>
            ///<ExpectedResult> Production Order should have the correct information </ExpectedResult>
            ValidateProductionOrder(productionOrdersMessage[0], po);
        }

        /// <summary>
        /// Description:
        ///     - Create 1 Integration Entry with the list of Production Orders to create
        ///     - Create 1 Integration Entry for each Production Order
        ///     - Create 2 Production Order
        /// 
        /// Acceptance Citeria:
        ///     - All Integration Entries are processed
        ///     - The Production Orders are created successfully 
        ///  
        /// </summary>
        /// <TestCaseID>CustomImportProductionOrders.CustomImportProductionOrders_SendPOImport_CreateSeveralPOs</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomImportProductionOrders_SendPOImport_CreateSeveralPOs()
        {
            ///<Step> Create Message to send to MES System </Step>
            string productionOrdersSample = FileUtilities.LoadFile($@"ERP\Samples\SampleImportProductionOrders.xml");
            CustomImportProductionOrderCollection productionOrdersMessage = new CustomImportProductionOrderCollection();
            productionOrdersMessage = CustomUtilities.DeserializeXmlToObject<CustomImportProductionOrderCollection>(productionOrdersSample);

            productionOrdersMessage[0].Name = Guid.NewGuid().ToString("N");
            productionOrdersMessage[0].OrderNumber = Guid.NewGuid().ToString("N").Substring(0, 3);

            productionOrdersMessage[1].Name = Guid.NewGuid().ToString("N");
            productionOrdersMessage[1].OrderNumber = Guid.NewGuid().ToString("N").Substring(0, 3);

            _scenario.CustomImportProductionOrderCollection = productionOrdersMessage;
            _scenario.Setup();

            ///<Step> Validate integration entry with the list of Production Orders </Step>
            ///<ExpectedResult> Integration entry should be processed </ExpectedResult>
            Assert.IsTrue(_scenario.IntegrationEntries.Count > 0, "Integration Entries should have been created");
            foreach (IntegrationEntry ie in _scenario.IntegrationEntries)
            {
                Assert.IsTrue(ie.IsIntegrationEntryProcessed(), "Integration Entry was not processed. Error Message: {0}", ie.ResultDescription);
            }

            ///<Step> Validate integration entry with the Production Order </Step>
            ///<ExpectedResult> Integration entry should be processed </ExpectedResult>
            IntegrationEntry productionOrderIntegrationEntry = CustomUtilities.GetIntegrationEntry(productionOrdersMessage[0].Name);
            Assert.IsTrue(productionOrderIntegrationEntry.IsIntegrationEntryProcessed(), "Integration Entry was not processed. Error Message: {0}", productionOrderIntegrationEntry.ResultDescription);

            ProductionOrder po = new ProductionOrder()
            {
                Name = productionOrdersMessage[0].Name
            };

            Assert.IsTrue(po.ObjectExists(), $"Production Order named {productionOrdersMessage[0].Name} should have been created.");

            customTeardownManager.Push(po);
            po.Load();
            po.Product.Load();
            po.Facility.Load();

            ///<Step> Validate Production Order created </Step>
            ///<ExpectedResult> Production Order should have the correct information </ExpectedResult>
            ValidateProductionOrder(productionOrdersMessage[0], po);

            ///<Step> Validate integration entry with the Production Order </Step>
            ///<ExpectedResult> Integration entry should be processed </ExpectedResult>
            productionOrderIntegrationEntry = CustomUtilities.GetIntegrationEntry(productionOrdersMessage[1].Name);
            Assert.IsTrue(productionOrderIntegrationEntry.IsIntegrationEntryProcessed(), "Integration Entry was not processed. Error Message: {0}", productionOrderIntegrationEntry.ResultDescription);

            po = new ProductionOrder()
            {
                Name = productionOrdersMessage[1].Name
            };

            Assert.IsTrue(po.ObjectExists(), $"Production Order named {productionOrdersMessage[1].Name} should have been created.");

            customTeardownManager.Push(po);
            po.Load();
            po.Product.Load();
            po.Facility.Load();

            ///<Step> Validate Production Order created </Step>
            ///<ExpectedResult> Production Order should have the correct information </ExpectedResult>
            ValidateProductionOrder(productionOrdersMessage[1], po);
        }

        /// <summary>
        /// Description:
        ///     - Create 1 Integration Entry with the list of Production Orders to create
        ///     - Create 1 Integration Entry for the Production Order
        ///     - Create 1 Production Order
        ///     - Create 1 Integration Entry with the list of Production Orders to update
        ///     - Create 1 Integration Entry to update the previous Production Order
        /// 
        /// Acceptance Citeria:
        ///     - Both Integration Entries are processed
        ///     - The Production Order is created successfully 
        ///     - The Production Order was updated successfully
        ///  
        /// </summary>
        /// <TestCaseID>CustomImportProductionOrders.CustomImportProductionOrders_UpdatePO_HappyPath</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomImportProductionOrders_UpdatePO_HappyPath()
        {
            ///<Step> Create Message to send to MES System </Step>
            string productionOrdersSample = FileUtilities.LoadFile($@"ERP\Samples\SampleImportProductionOrder.xml");
            CustomImportProductionOrderCollection productionOrdersMessage = new CustomImportProductionOrderCollection();
            productionOrdersMessage = CustomUtilities.DeserializeXmlToObject<CustomImportProductionOrderCollection>(productionOrdersSample);

            productionOrdersMessage[0].Name = Guid.NewGuid().ToString("N");
            productionOrdersMessage[0].OrderNumber = Guid.NewGuid().ToString("N").Substring(0, 3);

            _scenario.CustomImportProductionOrderCollection = productionOrdersMessage;
            _scenario.Setup();

            ///<Step> Validate integration entry with the list of Production Orders </Step>
            ///<ExpectedResult> Integration entry should be processed </ExpectedResult>
            Assert.IsTrue(_scenario.IntegrationEntries.Count > 0, "Integration Entries should have been created");
            foreach (IntegrationEntry ie in _scenario.IntegrationEntries)
            {
                Assert.IsTrue(ie.IsIntegrationEntryProcessed(), "Integration Entry was not processed. Error Message: {0}", ie.ResultDescription);
            }

            ///<Step> Validate integration entry with the Production Order </Step>
            ///<ExpectedResult> Integration entry should be processed </ExpectedResult>
            IntegrationEntry productionOrderIntegrationEntry = CustomUtilities.GetIntegrationEntry(productionOrdersMessage[0].Name);
            Assert.IsTrue(productionOrderIntegrationEntry.IsIntegrationEntryProcessed(), "Integration Entry was not processed. Error Message: {0}", productionOrderIntegrationEntry.ResultDescription);

            ProductionOrder po = new ProductionOrder()
            {
                Name = productionOrdersMessage[0].Name
            };

            Assert.IsTrue(po.ObjectExists(), $"Production Order named {productionOrdersMessage[0].Name} should have been created.");

            customTeardownManager.Push(po);
            po.Load();
            po.Product.Load();
            po.Facility.Load();

            ///<Step> Validate Production Order created </Step>
            ///<ExpectedResult> Production Order should have the correct information </ExpectedResult>
            ValidateProductionOrder(productionOrdersMessage[0], po);

            Thread.Sleep(18000);

            #region UpdatePO

            ///<Step> Create Message to send to MES System </Step>
            productionOrdersSample = FileUtilities.LoadFile($@"ERP\Samples\SampleImportProductionOrder.xml");
            CustomImportProductionOrderCollection updateProductionOrdersMessage = new CustomImportProductionOrderCollection();
            updateProductionOrdersMessage = CustomUtilities.DeserializeXmlToObject<CustomImportProductionOrderCollection>(productionOrdersSample);

            updateProductionOrdersMessage[0].Name = productionOrdersMessage[0].Name;
            updateProductionOrdersMessage[0].OrderNumber = productionOrdersMessage[0].OrderNumber;
            updateProductionOrdersMessage[0].Units = "BARS";
            updateProductionOrdersMessage[0].UnderDeliveryTolerance = (decimal?)0.5;
            updateProductionOrdersMessage[0].OverDeliveryTolerance = (decimal?)0.6;

            CustomExecutionScenario updateScenario = new CustomExecutionScenario();
            updateScenario.CustomImportProductionOrderCollection = updateProductionOrdersMessage;
            updateScenario.Setup();

            ///<Step> Validate integration entry with the list of Production Orders </Step>
            ///<ExpectedResult> Integration entry should be processed </ExpectedResult>
            Assert.IsTrue(updateScenario.IntegrationEntries.Count > 0, "Integration Entries should have been created");
            foreach (IntegrationEntry ie in updateScenario.IntegrationEntries)
            {
                Assert.IsTrue(ie.IsIntegrationEntryProcessed(), "Integration Entry was not processed. Error Message: {0}", ie.ResultDescription);
            }

            ///<Step> Validate integration entry with the Production Order to be updated </Step>
            ///<ExpectedResult> Integration entry should be processed </ExpectedResult>
            productionOrderIntegrationEntry = CustomUtilities.GetIntegrationEntry(updateProductionOrdersMessage[0].Name);
            Assert.IsTrue(productionOrderIntegrationEntry.IsIntegrationEntryProcessed(), "Integration Entry was not processed. Error Message: {0}", productionOrderIntegrationEntry.ResultDescription);

            po = new ProductionOrder()
            {
                Name = updateProductionOrdersMessage[0].Name
            };

            Assert.IsTrue(po.ObjectExists(), $"Production Order named {updateProductionOrdersMessage[0].Name} should have been created.");

            customTeardownManager.Push(po);
            po.Load();
            po.Product.Load();
            po.Facility.Load();

            ///<Step> Validate Production Order was updated </Step>
            ///<ExpectedResult> Production Order should have the correct updated information </ExpectedResult>
            ValidateProductionOrder(updateProductionOrdersMessage[0], po);

            #endregion
        }

        /// <summary>
        /// Validate the created Production Order against the imported XML
        /// </summary>
        /// <param name="productionOrdersMessage">XML Data Production Order</param>
        /// <param name="createdPO">Created Production Order</param>
        private static void ValidateProductionOrder(CustomImportProductionOrder productionOrdersMessage, ProductionOrder createdPO)
        {
            Assert.IsTrue(createdPO.Type.Equals(productionOrdersMessage.Type), $"Production Order Type should be {productionOrdersMessage.Type}, but instead is: {createdPO.Type}");
            Assert.IsTrue(createdPO.OrderNumber.Equals(productionOrdersMessage.OrderNumber), $"Production Order Order Number should be {productionOrdersMessage.OrderNumber}, but instead is: {createdPO.Product.Name}");
            Assert.IsTrue(createdPO.Facility.Name.Equals(productionOrdersMessage.Facility), $"Production Order Facility should be {productionOrdersMessage.Facility}, but instead is: {createdPO.Facility.Name}");
            Assert.IsTrue(createdPO.Product.Name.Equals(productionOrdersMessage.Product), $"Production Order Product should be {productionOrdersMessage.Product}, but instead is: {createdPO.Product.Name}");
            Assert.IsTrue(string.Format("{0:0}", createdPO.Quantity).Equals(productionOrdersMessage.Quantity.ToString()), $"Production Order Quantity should be {productionOrdersMessage.Quantity}, but instead is: {string.Format("{0:0}", createdPO.Quantity.ToString())}");
            Assert.IsTrue(createdPO.Units.Equals(productionOrdersMessage.Units), $"Production Order Units should be {productionOrdersMessage.Units}, but instead is: {createdPO.Units}");
            Assert.IsTrue((createdPO.DueDate?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty).Equals(string.Format(productionOrdersMessage.DueDate, "yyyy-MM-dd HH:mm")), $"Production Order DueDate should be {string.Format(productionOrdersMessage.DueDate, "yyyy-MM-dd HH:mm")}, but instead is: {createdPO.DueDate?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty}");
            Assert.IsTrue(createdPO.RestrictOnComplete.ToString().Equals(productionOrdersMessage.RestrictOnComplete), $"Production Order RestrictOnComplete should be {productionOrdersMessage.RestrictOnComplete}, but instead is: {createdPO.RestrictOnComplete}");
            Assert.IsTrue(string.Format("{0:0.##}", createdPO.UnderDeliveryTolerance).Equals(productionOrdersMessage.UnderDeliveryTolerance.ToString()), $"Production Order UnderDeliveryTolerance should be {productionOrdersMessage.UnderDeliveryTolerance}, but instead is: {string.Format("{0:0.##}", createdPO.UnderDeliveryTolerance.ToString())}");
            Assert.IsTrue(string.Format("{0:0.##}", createdPO.OverDeliveryTolerance).Equals(productionOrdersMessage.OverDeliveryTolerance.ToString()), $"Production Order OverDeliveryTolerance should be {productionOrdersMessage.OverDeliveryTolerance}, but instead is: {string.Format("{0:0.##}", createdPO.OverDeliveryTolerance.ToString())}");
            Assert.IsTrue((createdPO.PlannedStartDate?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty).Equals(string.Format(productionOrdersMessage.PlannedStartDate, "yyyy-MM-dd HH:mm")), $"Production Order PlannedStartDate should be {string.Format(productionOrdersMessage.PlannedStartDate, "yyyy-MM-dd HH:mm")}, but instead is: {createdPO.PlannedStartDate?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty}");
            Assert.IsTrue((createdPO.PlannedEndDate?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty).Equals(string.Format(productionOrdersMessage.PlannedEndDate, "yyyy-MM-dd HH:mm")), $"Production Order OverDeliveryTolerance should be {string.Format(productionOrdersMessage.PlannedEndDate, "yyyy-MM-dd HH:mm")}, but instead is: {createdPO.PlannedEndDate?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty}");
            Assert.IsTrue(createdPO.SystemState.Equals(ProductionOrderSystemState.Released), $"Production Order SystemState should be {ProductionOrderSystemState.Released}, but instead is: {createdPO.SystemState}");
        }
    }
}
