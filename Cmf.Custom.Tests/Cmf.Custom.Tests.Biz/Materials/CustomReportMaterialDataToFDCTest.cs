using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Xml;

namespace Cmf.Custom.Tests.Biz.Materials
{
    [TestClass]
    public class CustomReportMaterialDataToFDCTest
    {
        private CustomMaterialScenario materialScenario = null;
        private IntegrationEntry integrationEntry = null;
        private IntegrationEntryCollection integrationEntriesToTerminate = new IntegrationEntryCollection();
        private int pollingIntervalConfig = Convert.ToInt32(ConfigUtilities.GetConfigValue(AMSOsramConstants.PollingIntervalConfigValue));
        private int MaxNumberOfRetries = 30;
        private const string resourceName = "PDSP0101";
        private const string subResourceName = "PDSP0101.PM01";

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            //Creation Custom Material Scenario
            materialScenario = new CustomMaterialScenario(false)
            {
                NumberOfSubMaterials = 1,
                AssociateSubMaterialsToContainer = true
            };

            ConfigUtilities.SetConfigValue(AMSOsramConstants.FDCActiveConfigPath, true);
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            // materialScenario teardown
            if (materialScenario != null)
            {
                materialScenario.TearDown();
            }

            // Either clean integration entries here with on bulk or delete after every test - performance?
        }

        /// <summary>
        /// Description:
        ///     -Create a lot material in order to trigger a track in operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCLotStart
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_LotMaterialTrackIn</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_LotMaterialTrackIn()
        {

            Material mat = materialScenario.Entity;
            mat.ComplexTrackIn();

            IntegrationEntry integrationEntry = new IntegrationEntry();
            integrationEntry = CustomUtilities.GetIntegrationEntry(mat.Name);
            integrationEntry.Load();

            Assert.IsTrue(integrationEntry.Name.Contains(mat.Name), "Integration entry was not created.");
            Assert.AreEqual(integrationEntry.MessageType, AMSOsramConstants.MessageType_LOTIN, "Integration entry contains the wrong message type.");

        }

        /// <summary>
        /// Description:
        ///     -Create a wafer material in order to trigger a track in operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCWaferIn
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_WaferMaterialTrackIn</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_WaferMaterialTrackIn()
        {
            DateTime fromDate = DateTime.Now;

            materialScenario.Setup(true);
            Assert.IsTrue(materialScenario.Entity.SubMaterialCount > 0, $"The material {materialScenario.Entity.Name} should have submaterials.");
            IntegrationEntry integrationEntry = new IntegrationEntry();

            SetResourceFDCCommunication(subResourceName, true);
            Resource subResource = new Resource()
            {
                Name = subResourceName
            };
            subResource.Load();

            materialScenario.Entity.ComplexTrackIn();
            materialScenario.Entity.LoadChildren();
            materialScenario.Entity.SubMaterials.ComplexTrackInMaterials(subResource);
            fromDate = DateTime.Now;

            // Verify if Integration Entry for wafer material was created and contains the right messageType

            integrationEntry = IntegrationEntryUtilities.GetIntegrationEntry(AMSOsramConstants.MessageType_WAFERIN, fromDate, true,
                            AMSOsramConstants.SourceSystem_OntoFDC, AMSOsramConstants.TargetSystem_OntoFDC);
            integrationEntry.Load();

            Assert.IsTrue(integrationEntry.Name.Contains(materialScenario.SubMaterials[0].Name.Replace(" ", "_")), "Integration entry for wafer was not created.");
            Assert.AreEqual(integrationEntry.MessageType, AMSOsramConstants.MessageType_WAFERIN, "Integration entry contains the wrong message type.");
            
            // Validate Integration Entry message body with material data
            ValidateIntegrationEntry(fromDate, true, materialScenario.Entity.Name, materialScenario.SubMaterials[0].Name, subResourceName);

            // Add Integration Entry to removal list for clean up
            AddIntegrationEntryToRemoveLater(integrationEntry.MessageType, fromDate);

        }

        /// <summary>
        /// Description:
        ///     -Create a lot material in order to trigger a track in and track out operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCLotEnd
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_LotMaterialTrackOut</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_LotMaterialTrackOut()
        {

            materialScenario.Setup(true);
            Material mat = new Material();
            mat = materialScenario.Entity;

            // ERROR: Running these manually works, but during automatic run test will not have enough time and therefore the wrong IntegrationEntry (TrackIn) is fetched

            mat.ComplexTrackIn();
            mat.Load();
            mat.ComplexTrackOutMaterial();


            IntegrationEntry integrationEntry = new IntegrationEntry();
            integrationEntry = CustomUtilities.GetIntegrationEntry(mat.Name);


            Assert.IsTrue(integrationEntry.Name.Contains(mat.Name), "Integration entry was not created.");
            Assert.AreEqual(integrationEntry.MessageType, AMSOsramConstants.MessageType_LOTOUT, "Integration entry contains the wrong message type.");
        }

        /// <summary>
        /// Description:
        ///     -Create a wafer material in order to trigger a track in and track out operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCWaferOut
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_WaferMaterialTrackOut</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_WaferMaterialTrackOut()
        {

            materialScenario.Setup(true);
            Material mat = new Material();
            mat = materialScenario.Entity;

            // ERROR: Running these manually works, but during automatic run test will not have enough time and therefore the wrong IntegrationEntry (TrackIn) is fetched

            mat.ComplexTrackIn();
            mat.Load();
            mat.ComplexTrackOutMaterial();


            IntegrationEntry integrationEntry = new IntegrationEntry();
            integrationEntry = CustomUtilities.GetIntegrationEntry(mat.Name);


            Assert.IsTrue(integrationEntry.Name.Contains(mat.Name), "Integration entry was not created.");
            Assert.AreEqual(integrationEntry.MessageType, AMSOsramConstants.MessageType_LOTOUT, "Integration entry contains the wrong message type.");
        }


        /// <summary>
        /// Description:
        ///     -Create a lot material in order to trigger a track in and abort operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCLotEnd
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_LotMaterialAbort</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_LotMaterialAbort()
        {
            materialScenario.Setup(true);
            Material mat = new Material();
            mat = materialScenario.Entity;

            // ERROR: Running these manually works, but during automatic run test will not have enough time and therefore the wrong IntegrationEntry (TrackIn) is fetched

            mat.ComplexTrackIn();
            mat.Load();
            mat.Abort();

            IntegrationEntry integrationEntry = new IntegrationEntry();
            integrationEntry = CustomUtilities.GetIntegrationEntry(mat.Name);


            Assert.IsTrue(integrationEntry.Name.Contains(mat.Name), "Integration entry was not created.");
            Assert.AreEqual(integrationEntry.MessageType, AMSOsramConstants.MessageType_LOTOUT, "Integration entry contains the wrong message type.");
        }

        /// <summary>
        /// Description:
        ///     -Create a wafer material in order to trigger a track in and abort operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCWaferOut
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_WaferMaterialAbort</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_WaferMaterialAbort()
        {
            materialScenario.Setup(true);
            Material mat = new Material();
            mat = materialScenario.Entity;

            // ERROR: Running these manually works, but during automatic run test will not have enough time and therefore the wrong IntegrationEntry (TrackIn) is fetched

            mat.ComplexTrackIn();
            mat.Load();
            mat.Abort();

            IntegrationEntry integrationEntry = new IntegrationEntry();
            integrationEntry = CustomUtilities.GetIntegrationEntry(mat.Name);


            Assert.IsTrue(integrationEntry.Name.Contains(mat.Name), "Integration entry was not created.");
            Assert.AreEqual(integrationEntry.MessageType, AMSOsramConstants.MessageType_LOTOUT, "Integration entry contains the wrong message type.");
        }

        #region Help methods

        /// <summary>
        /// Validate Integration Entry
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="transactionSuccess"></param>
        /// <param name="expectedMaterialName"></param>
        private void ValidateIntegrationEntry(DateTime fromDate, bool transactionSuccess, string expectedLotName = "", string expectedWaferName = "", string expectedChamberName = "",
            string expectedOperationName = "", string expectedSPSName = "", string expectedRecipeName = "", string expectedProductName = "", string expectedProductRoute = "", string expectedNumberOfWafersInBatch = "",
            string expectedFacilityName = "")
        {
            if (transactionSuccess){
                // Validate that Integration Entry was created and processed
                GenericUtilities.WaitFor(() =>
                {
                    // Get the IntegrationEntry
                    integrationEntry = IntegrationEntryUtilities.GetIntegrationEntry(AMSOsramConstants.MessageType_WAFERIN, fromDate, true,
                                AMSOsramConstants.SourceSystem_OntoFDC, AMSOsramConstants.TargetSystem_OntoFDC);
                    return integrationEntry != null;
                }, MaxNumberOfRetries, pollingIntervalConfig / MaxNumberOfRetries);

                // Load Integration Entry
                Assert.IsNotNull(integrationEntry, $"It should have been created an Integration Entry.");

                // Load integration entry
                integrationEntry.Load();

                //Necessary to load inner message
                IntegrationEntry integrationEntry_innerMessage = new GetIntegrationEntryInput
                {
                    Id = integrationEntry.Id
                }.GetIntegrationEntrySync().IntegrationEntry;

                // To remove later
                integrationEntriesToTerminate.Add(integrationEntry_innerMessage);

                //Fetch  Integration Entry Message
                string integrationEntry_innerMessage_Message = Encoding.UTF8.GetString(integrationEntry_innerMessage.IntegrationMessage.Message);

                // Load XML
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(integrationEntry_innerMessage_Message);

                switch (integrationEntry.MessageType)
                {

                    case "WAFERIN:":
                        foreach (XmlNode item in xml.DocumentElement.ChildNodes)
                        {
                            switch (item.Name)
                            {
                                case "WaferName":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property WaferName should not be empty!");

                                    Assert.AreEqual(expectedWaferName, item.InnerText.Trim(),
                                        $"The property WaferName should be {expectedWaferName}!");
                                    break;
                                case "LotName":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property LotName should not be empty!");

                                    Assert.AreEqual(expectedLotName, item.InnerText.Trim(),
                                        $"The property LotName should be {expectedLotName}!");
                                    break;
                                case "Chamber":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property Chamber should not be empty!");

                                    Assert.AreEqual(expectedChamberName, item.InnerText.Trim(),
                                        $"The property Chamber should be {expectedChamberName}!");
                                    break;
                                case "SlotPos":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property SlotPos should not be empty!");

                                    Assert.AreEqual("1", item.InnerText.Trim(),
                                        $"The property SlotPos should be 1!");
                                    break;
                                case "LotPos":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property LotPos should not be empty!");

                                    Assert.AreEqual("1", item.InnerText.Trim(),
                                        $"The property LotPos should be 1!");
                                    break;
                                case "QtyIn":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property QtyIn should not be empty!");

                                    Assert.AreEqual("1", item.InnerText.Trim(),
                                        $"The property QtyIn should be 1!");
                                    break;
                            }
                        }
                        break;
                    case "WAFEROUT":
                        foreach (XmlNode item in xml.DocumentElement.ChildNodes)
                        {
                            switch (item.Name)
                            {
                                case "WaferName":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property WaferName should not be empty!");

                                    Assert.AreEqual(expectedWaferName, item.InnerText.Trim(),
                                        $"The property WaferName should be {expectedWaferName}!");
                                    break;
                                case "LotName":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property LotName should not be empty!");

                                    Assert.AreEqual(expectedLotName, item.InnerText.Trim(),
                                        $"The property LotName should be {expectedLotName}!");
                                    break;
                                case "Chamber":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property Chamber should not be empty!");

                                    Assert.AreEqual(expectedChamberName, item.InnerText.Trim(),
                                        $"The property Chamber should be {expectedChamberName}!");
                                    break;
                                case "Processed":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property Processed should not be empty!");

                                    Assert.AreEqual("true", item.InnerText.Trim(),
                                        $"The property Processed should be true!");
                                    break;
                                case "WaferState":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property WaferState should not be empty!");

                                    Assert.AreEqual("Processed", item.InnerText.Trim(),
                                        $"The property WaferState should be true!");
                                    break;
                            }
                        }
                        break;
                    case "LOTIN":
                        foreach (XmlNode item in xml.DocumentElement.ChildNodes)
                        {
                            switch (item.Name)
                            {
                                case "LotName":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property LotName should not be empty!");

                                    Assert.AreEqual(expectedLotName, item.InnerText.Trim(),
                                        $"The property LotName should be {expectedLotName}!");
                                    break;
                                case "Operation":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property Operation should not be empty!");

                                    Assert.AreEqual(expectedOperationName, item.InnerText.Trim(),
                                        $"The property Operation should be {expectedOperationName}!");
                                    break;
                                case "SPS":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property SPS should not be empty!");

                                    Assert.AreEqual(expectedOperationName, item.InnerText.Trim(),
                                        $"The property SPS should be {expectedOperationName}!");
                                    break;
                                case "Chamber":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property Chamber should not be empty!");

                                    Assert.AreEqual(expectedChamberName, item.InnerText.Trim(),
                                        $"The property Chamber should be {expectedChamberName}!");
                                    break;
                                case "SlotPos":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property SlotPos should not be empty!");

                                    Assert.AreEqual("1", item.InnerText.Trim(),
                                        $"The property SlotPos should be 1!");
                                    break;
                                case "LotPos":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property LotPos should not be empty!");

                                    Assert.AreEqual("1", item.InnerText.Trim(),
                                        $"The property LotPos should be 1!");
                                    break;
                                case "QtyIn":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property QtyIn should not be empty!");

                                    Assert.AreEqual("1", item.InnerText.Trim(),
                                        $"The property QtyIn should be 1!");
                                    break;
                            }
                        }
                        break;
                    case "LOTOUT":
                        foreach (XmlNode item in xml.DocumentElement.ChildNodes)
                        {
                            switch (item.Name)
                            {
                                case "Operation":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property Operation should not be empty!");

                                    Assert.AreEqual(expectedOperationName, item.InnerText.Trim(),
                                        $"The property Operation should be {expectedOperationName}!");
                                    break;
                                case "LotName":
                                    Assert.IsTrue(item.InnerText != null && !string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property LotName should not be empty!");

                                    Assert.AreEqual(expectedLotName, item.InnerText.Trim(),
                                        $"The property LotName should be {expectedLotName}!");
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Add Integration Entry to integrationEntriesToTerminate list in order to remove it later on cleanup
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="fromDate"></param>
        private void AddIntegrationEntryToRemoveLater(string messageType, DateTime fromDate)
        {
            // Validate that Integration Entry was created
            GenericUtilities.WaitFor(() =>
            {
                // Get the IntegrationEntry
                integrationEntry = IntegrationEntryUtilities.GetIntegrationEntry(messageType, fromDate, true,
                    AMSOsramConstants.SourceSystem_OntoFDC, AMSOsramConstants.TargetSystem_OntoFDC);

                return integrationEntry != null;
            }, MaxNumberOfRetries, pollingIntervalConfig / MaxNumberOfRetries);

            // Load Integration Entry
            Assert.IsNotNull(integrationEntry, $"It should have been created an Integration Entry.");

            // Load integration entry
            integrationEntry.Load();

            // To remove later
            integrationEntriesToTerminate.Add(integrationEntry);
        }

        /// <summary>
        /// Sets the FDCCommunication value of the given resource
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="fdcCommunicationValue"></param>
        private void SetResourceFDCCommunication(string resourceName, bool fdcCommunicationValue)
        {
            Resource resource = new Resource
            {
                Name = resourceName
            };
            resource.Load();
            resource.SaveAttribute("FDCCommunication", fdcCommunicationValue);
        }

            #endregion Help methods
        }
}