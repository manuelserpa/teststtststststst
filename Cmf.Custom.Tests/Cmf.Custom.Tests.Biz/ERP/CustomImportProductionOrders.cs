using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.ERP.ProductionOrder;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
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
            //productionOrdersMessage[0].DueDate = DateTime.Today.AddDays(1).ToString();
            //productionOrdersMessage[0].PlannedStartDate = DateTime.Now.ToString();
            //productionOrdersMessage[0].PlannedEndDate = productionOrdersMessage[0].DueDate;

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
            Assert.IsTrue(po.Type.Equals(productionOrdersMessage[0].Type), $"Production Order Type should be {productionOrdersMessage[0].Type}, but instead is: {po.Type}");
            Assert.IsTrue(po.OrderNumber.Equals(productionOrdersMessage[0].OrderNumber), $"Production Order Order Number should be {productionOrdersMessage[0].OrderNumber}, but instead is: {po.Product.Name}");
            Assert.IsTrue(po.Facility.Name.Equals(productionOrdersMessage[0].Facility), $"Production Order Facility should be {productionOrdersMessage[0].Facility}, but instead is: {po.Facility.Name}");
            Assert.IsTrue(po.Product.Name.Equals(productionOrdersMessage[0].Product), $"Production Order Product should be {productionOrdersMessage[0].Product}, but instead is: {po.Product.Name}");
            Assert.IsTrue(string.Format("{0:0}", po.Quantity).Equals(productionOrdersMessage[0].Quantity.ToString()), $"Production Order Quantity should be {productionOrdersMessage[0].Quantity}, but instead is: {string.Format("{0:0}", po.Quantity.ToString())}");
            Assert.IsTrue(po.Units.Equals(productionOrdersMessage[0].Units), $"Production Order Units should be {productionOrdersMessage[0].Units}, but instead is: {po.Units}");
            //Assert.IsTrue(po.DueDate.ToString().Equals(productionOrdersMessage[0].DueDate), $"Production Order DueDate should be {productionOrdersMessage[0].DueDate}, but instead is: {po.DueDate.ToString()}");
            Assert.IsTrue(po.RestrictOnComplete.ToString().Equals(productionOrdersMessage[0].RestrictOnComplete), $"Production Order RestrictOnComplete should be {productionOrdersMessage[0].RestrictOnComplete}, but instead is: {po.RestrictOnComplete}");
            Assert.IsTrue(string.Format("{0:0.##}", po.UnderDeliveryTolerance).Equals(productionOrdersMessage[0].UnderDeliveryTolerance.ToString()), $"Production Order UnderDeliveryTolerance should be {productionOrdersMessage[0].UnderDeliveryTolerance}, but instead is: {string.Format("{0:0.##}", po.UnderDeliveryTolerance.ToString())}");
            Assert.IsTrue(string.Format("{0:0.##}", po.OverDeliveryTolerance).Equals(productionOrdersMessage[0].OverDeliveryTolerance.ToString()), $"Production Order OverDeliveryTolerance should be {productionOrdersMessage[0].OverDeliveryTolerance}, but instead is: {string.Format("{0:0.##}", po.OverDeliveryTolerance.ToString())}");
            //Assert.IsTrue(po.PlannedStartDate.ToString().Equals(productionOrdersMessage[0].PlannedStartDate), $"Production Order PlannedStartDate should be {productionOrdersMessage[0].PlannedStartDate}, but instead is: {po.PlannedStartDate.ToString()}");
            //Assert.IsTrue(po.PlannedEndDate.ToString().Equals(productionOrdersMessage[0].PlannedEndDate), $"Production Order OverDeliveryTolerance should be {productionOrdersMessage[0].PlannedEndDate}, but instead is: {po.PlannedEndDate.ToString()}");
            Assert.IsTrue(po.SystemState.Equals(ProductionOrderSystemState.Released), $"Production Order SystemState should be {ProductionOrderSystemState.Released}, but instead is: {po.SystemState}");

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
            Assert.IsTrue(po.Type.Equals(productionOrdersMessage[0].Type), $"Production Order Type should be {productionOrdersMessage[0].Type}, but instead is: {po.Type}");
            Assert.IsTrue(po.OrderNumber.Equals(productionOrdersMessage[0].OrderNumber), $"Production Order Order Number should be {productionOrdersMessage[0].OrderNumber}, but instead is: {po.Product.Name}");
            Assert.IsTrue(po.Facility.Name.Equals(productionOrdersMessage[0].Facility), $"Production Order Facility should be {productionOrdersMessage[0].Facility}, but instead is: {po.Facility.Name}");
            Assert.IsTrue(po.Product.Name.Equals(productionOrdersMessage[0].Product), $"Production Order Product should be {productionOrdersMessage[0].Product}, but instead is: {po.Product.Name}");
            Assert.IsTrue(string.Format("{0:0}", po.Quantity).Equals(productionOrdersMessage[0].Quantity.ToString()), $"Production Order Quantity should be {productionOrdersMessage[0].Quantity}, but instead is: {string.Format("{0:0}", po.Quantity.ToString())}");
            Assert.IsTrue(po.Units.Equals(productionOrdersMessage[0].Units), $"Production Order Units should be {productionOrdersMessage[0].Units}, but instead is: {po.Units}");
            //Assert.IsTrue(po.DueDate.ToString().Equals(productionOrdersMessage[0].DueDate), $"Production Order DueDate should be {productionOrdersMessage[0].DueDate}, but instead is: {po.DueDate.ToString()}");
            Assert.IsTrue(po.RestrictOnComplete.ToString().Equals(productionOrdersMessage[0].RestrictOnComplete), $"Production Order RestrictOnComplete should be {productionOrdersMessage[0].RestrictOnComplete}, but instead is: {po.RestrictOnComplete}");
            Assert.IsTrue(string.Format("{0:0.##}", po.UnderDeliveryTolerance).Equals(productionOrdersMessage[0].UnderDeliveryTolerance.ToString()), $"Production Order UnderDeliveryTolerance should be {productionOrdersMessage[0].UnderDeliveryTolerance}, but instead is: {string.Format("{0:0.##}", po.UnderDeliveryTolerance.ToString())}");
            Assert.IsTrue(string.Format("{0:0.##}", po.OverDeliveryTolerance).Equals(productionOrdersMessage[0].OverDeliveryTolerance.ToString()), $"Production Order OverDeliveryTolerance should be {productionOrdersMessage[0].OverDeliveryTolerance}, but instead is: {string.Format("{0:0.##}", po.OverDeliveryTolerance.ToString())}");
            //Assert.IsTrue(po.PlannedStartDate.ToString().Equals(productionOrdersMessage[0].PlannedStartDate), $"Production Order PlannedStartDate should be {productionOrdersMessage[0].PlannedStartDate}, but instead is: {po.PlannedStartDate.ToString()}");
            //Assert.IsTrue(po.PlannedEndDate.ToString().Equals(productionOrdersMessage[0].PlannedEndDate), $"Production Order OverDeliveryTolerance should be {productionOrdersMessage[0].PlannedEndDate}, but instead is: {po.PlannedEndDate.ToString()}");
            Assert.IsTrue(po.SystemState.Equals(ProductionOrderSystemState.Released), $"Production Order SystemState should be {ProductionOrderSystemState.Released}, but instead is: {po.SystemState}");

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
            Assert.IsTrue(po.Type.Equals(productionOrdersMessage[1].Type), $"Production Order Type should be {productionOrdersMessage[1].Type}, but instead is: {po.Type}");
            Assert.IsTrue(po.OrderNumber.Equals(productionOrdersMessage[1].OrderNumber), $"Production Order Order Number should be {productionOrdersMessage[1].OrderNumber}, but instead is: {po.Product.Name}");
            Assert.IsTrue(po.Facility.Name.Equals(productionOrdersMessage[1].Facility), $"Production Order Facility should be {productionOrdersMessage[1].Facility}, but instead is: {po.Facility.Name}");
            Assert.IsTrue(po.Product.Name.Equals(productionOrdersMessage[1].Product), $"Production Order Product should be {productionOrdersMessage[1].Product}, but instead is: {po.Product.Name}");
            Assert.IsTrue(string.Format("{0:0}", po.Quantity).Equals(productionOrdersMessage[1].Quantity.ToString()), $"Production Order Quantity should be {productionOrdersMessage[1].Quantity}, but instead is: {string.Format("{0:0}", po.Quantity.ToString())}");
            Assert.IsTrue(po.Units.Equals(productionOrdersMessage[1].Units), $"Production Order Units should be {productionOrdersMessage[0].Units}, but instead is: {po.Units}");
            //Assert.IsTrue(po.DueDate.ToString().Equals(productionOrdersMessage[1].DueDate), $"Production Order DueDate should be {productionOrdersMessage[0].DueDate}, but instead is: {po.DueDate.ToString()}");
            Assert.IsTrue(po.RestrictOnComplete.ToString().Equals(productionOrdersMessage[1].RestrictOnComplete), $"Production Order RestrictOnComplete should be {productionOrdersMessage[1].RestrictOnComplete}, but instead is: {po.RestrictOnComplete}");
            Assert.IsTrue(string.Format("{0:0.##}", po.UnderDeliveryTolerance).Equals(productionOrdersMessage[1].UnderDeliveryTolerance.ToString()), $"Production Order UnderDeliveryTolerance should be {productionOrdersMessage[1].UnderDeliveryTolerance}, but instead is: {string.Format("{0:0.##}", po.UnderDeliveryTolerance.ToString())}");
            Assert.IsTrue(string.Format("{0:0.##}", po.OverDeliveryTolerance).Equals(productionOrdersMessage[1].OverDeliveryTolerance.ToString()), $"Production Order OverDeliveryTolerance should be {productionOrdersMessage[1].OverDeliveryTolerance}, but instead is: {string.Format("{0:0.##}", po.OverDeliveryTolerance.ToString())}");
            //Assert.IsTrue(po.PlannedStartDate.ToString().Equals(productionOrdersMessage[1].PlannedStartDate), $"Production Order PlannedStartDate should be {productionOrdersMessage[1].PlannedStartDate}, but instead is: {po.PlannedStartDate.ToString()}");
            //Assert.IsTrue(po.PlannedEndDate.ToString().Equals(productionOrdersMessage[1].PlannedEndDate), $"Production Order OverDeliveryTolerance should be {productionOrdersMessage[1].PlannedEndDate}, but instead is: {po.PlannedEndDate.ToString()}");
            Assert.IsTrue(po.SystemState.Equals(ProductionOrderSystemState.Released), $"Production Order SystemState should be {ProductionOrderSystemState.Released}, but instead is: {po.SystemState}");

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
            //productionOrdersMessage[0].DueDate = DateTime.Today.AddDays(1).ToString();
            //productionOrdersMessage[0].PlannedStartDate = DateTime.Now.ToString();
            //productionOrdersMessage[0].PlannedEndDate = productionOrdersMessage[0].DueDate;

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
            Assert.IsTrue(po.Type.Equals(productionOrdersMessage[0].Type), $"Production Order Type should be {productionOrdersMessage[0].Type}, but instead is: {po.Type}");
            Assert.IsTrue(po.OrderNumber.Equals(productionOrdersMessage[0].OrderNumber), $"Production Order Order Number should be {productionOrdersMessage[0].OrderNumber}, but instead is: {po.Product.Name}");
            Assert.IsTrue(po.Facility.Name.Equals(productionOrdersMessage[0].Facility), $"Production Order Facility should be {productionOrdersMessage[0].Facility}, but instead is: {po.Facility.Name}");
            Assert.IsTrue(po.Product.Name.Equals(productionOrdersMessage[0].Product), $"Production Order Product should be {productionOrdersMessage[0].Product}, but instead is: {po.Product.Name}");
            Assert.IsTrue(string.Format("{0:0}", po.Quantity).Equals(productionOrdersMessage[0].Quantity.ToString()), $"Production Order Quantity should be {productionOrdersMessage[0].Quantity}, but instead is: {string.Format("{0:0}", po.Quantity.ToString())}");
            Assert.IsTrue(po.Units.Equals(productionOrdersMessage[0].Units), $"Production Order Units should be {productionOrdersMessage[0].Units}, but instead is: {po.Units}");
            //Assert.IsTrue(po.DueDate.ToString().Equals(productionOrdersMessage[0].DueDate), $"Production Order DueDate should be {productionOrdersMessage[0].DueDate}, but instead is: {po.DueDate.ToString()}");
            Assert.IsTrue(po.RestrictOnComplete.ToString().Equals(productionOrdersMessage[0].RestrictOnComplete), $"Production Order RestrictOnComplete should be {productionOrdersMessage[0].RestrictOnComplete}, but instead is: {po.RestrictOnComplete}");
            Assert.IsTrue(string.Format("{0:0.##}", po.UnderDeliveryTolerance).Equals(productionOrdersMessage[0].UnderDeliveryTolerance.ToString()), $"Production Order UnderDeliveryTolerance should be {productionOrdersMessage[0].UnderDeliveryTolerance}, but instead is: {string.Format("{0:0.##}", po.UnderDeliveryTolerance.ToString())}");
            Assert.IsTrue(string.Format("{0:0.##}", po.OverDeliveryTolerance).Equals(productionOrdersMessage[0].OverDeliveryTolerance.ToString()), $"Production Order OverDeliveryTolerance should be {productionOrdersMessage[0].OverDeliveryTolerance}, but instead is: {string.Format("{0:0.##}", po.OverDeliveryTolerance.ToString())}");
            //Assert.IsTrue(po.PlannedStartDate.ToString().Equals(productionOrdersMessage[0].PlannedStartDate), $"Production Order PlannedStartDate should be {productionOrdersMessage[0].PlannedStartDate}, but instead is: {po.PlannedStartDate.ToString()}");
            //Assert.IsTrue(po.PlannedEndDate.ToString().Equals(productionOrdersMessage[0].PlannedEndDate), $"Production Order OverDeliveryTolerance should be {productionOrdersMessage[0].PlannedEndDate}, but instead is: {po.PlannedEndDate.ToString()}");
            Assert.IsTrue(po.SystemState.Equals(ProductionOrderSystemState.Released), $"Production Order SystemState should be {ProductionOrderSystemState.Released}, but instead is: {po.SystemState}");

            Thread.Sleep(18000);
            #region UpdatePO

            ///<Step> Create Message to send to MES System </Step>
            productionOrdersSample = FileUtilities.LoadFile($@"ERP\Samples\SampleImportProductionOrder.xml");
            CustomImportProductionOrderCollection updateProductionOrdersMessage = new CustomImportProductionOrderCollection();
            updateProductionOrdersMessage = CustomUtilities.DeserializeXmlToObject<CustomImportProductionOrderCollection>(productionOrdersSample);

            updateProductionOrdersMessage[0].Name = productionOrdersMessage[0].Name;
            updateProductionOrdersMessage[0].OrderNumber = productionOrdersMessage[0].OrderNumber;
            //updateProductionOrdersMessage[0].DueDate = DateTime.Now.AddDays(2).ToString();
            //updateProductionOrdersMessage[0].PlannedStartDate = DateTime.Now.ToString();
            //updateProductionOrdersMessage[0].PlannedEndDate = updateProductionOrdersMessage[0].DueDate;
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
            Assert.IsTrue(po.Type.Equals(updateProductionOrdersMessage[0].Type), $"Production Order Type should be {updateProductionOrdersMessage[0].Type}, but instead is: {po.Type}");
            Assert.IsTrue(po.OrderNumber.Equals(updateProductionOrdersMessage[0].OrderNumber), $"Production Order Order Number should be {updateProductionOrdersMessage[0].OrderNumber}, but instead is: {po.Product.Name}");
            Assert.IsTrue(po.Facility.Name.Equals(updateProductionOrdersMessage[0].Facility), $"Production Order Facility should be {updateProductionOrdersMessage[0].Facility}, but instead is: {po.Facility.Name}");
            Assert.IsTrue(po.Product.Name.Equals(updateProductionOrdersMessage[0].Product), $"Production Order Product should be {updateProductionOrdersMessage[0].Product}, but instead is: {po.Product.Name}");
            Assert.IsTrue(string.Format("{0:0}", po.Quantity).Equals(updateProductionOrdersMessage[0].Quantity.ToString()), $"Production Order Quantity should be {updateProductionOrdersMessage[0].Quantity}, but instead is: {string.Format("{0:0}", po.Quantity.ToString())}");
            Assert.IsTrue(po.Units.Equals(updateProductionOrdersMessage[0].Units), $"Production Order Units should be {updateProductionOrdersMessage[0].Units}, but instead is: {po.Units}");
            //Assert.IsTrue(po.DueDate.ToString().Equals(updateProductionOrdersMessage[0].DueDate), $"Production Order DueDate should be {updateProductionOrdersMessage[0].DueDate}, but instead is: {po.DueDate.ToString()}");
            Assert.IsTrue(po.RestrictOnComplete.ToString().Equals(updateProductionOrdersMessage[0].RestrictOnComplete), $"Production Order RestrictOnComplete should be {updateProductionOrdersMessage[0].RestrictOnComplete}, but instead is: {po.RestrictOnComplete}");
            Assert.IsTrue(string.Format("{0:0.##}", po.UnderDeliveryTolerance).Equals(updateProductionOrdersMessage[0].UnderDeliveryTolerance.ToString()), $"Production Order UnderDeliveryTolerance should be {updateProductionOrdersMessage[0].UnderDeliveryTolerance}, but instead is: {string.Format("{0:0.##}", po.UnderDeliveryTolerance.ToString())}");
            Assert.IsTrue(string.Format("{0:0.##}", po.OverDeliveryTolerance).Equals(updateProductionOrdersMessage[0].OverDeliveryTolerance.ToString()), $"Production Order OverDeliveryTolerance should be {updateProductionOrdersMessage[0].OverDeliveryTolerance}, but instead is: {string.Format("{0:0.##}", po.OverDeliveryTolerance.ToString())}");
            //Assert.IsTrue(po.PlannedStartDate.ToString().Equals(updateProductionOrdersMessage[0].PlannedStartDate), $"Production Order PlannedStartDate should be {updateProductionOrdersMessage[0].PlannedStartDate}, but instead is: {po.PlannedStartDate.ToString()}");
            //Assert.IsTrue(po.PlannedEndDate.ToString().Equals(updateProductionOrdersMessage[0].PlannedEndDate), $"Production Order OverDeliveryTolerance should be {updateProductionOrdersMessage[0].PlannedEndDate}, but instead is: {po.PlannedEndDate.ToString()}");
            Assert.IsTrue(po.SystemState.Equals(ProductionOrderSystemState.Released), $"Production Order SystemState should be {ProductionOrderSystemState.Released}, but instead is: {po.SystemState}");


            #endregion
        }

    }
}
