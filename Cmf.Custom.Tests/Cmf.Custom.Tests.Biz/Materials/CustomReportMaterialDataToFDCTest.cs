using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
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
        private static Resource resource;
        private static Resource subResource;
        private int pollingIntervalConfig = Convert.ToInt32(ConfigUtilities.GetConfigValue(amsOSRAMConstants.PollingIntervalConfigValue));
        private int MaxNumberOfRetries = 30;
        private bool fdcActiveConfig;
        private bool? isRecipeManagementEnabled = null;
        private Dictionary<Resource, bool> oldResourceFDCCommunicationValue = new Dictionary<Resource, bool>();
        private const string recipeName = "P-CLN024-TITIW";
        private const string serviceName = "Sputtering ZnO with Etching";

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            #region Material setup

            string resourceName = "PDSP0101";
            string subResourceName = "PDSP0101.PM01";

            resource = new Resource()
            {
                Name = resourceName
            };
            resource.Load();

            // Set IsRecipeManagementEnabled with true - and save old value to restore later
            isRecipeManagementEnabled = resource.IsRecipeManagementEnabled.GetValueOrDefault();
            resource.IsRecipeManagementEnabled = true;
            resource.Save();

            subResource = new Resource()
            {
                Name = subResourceName
            };
            subResource.Load();

            //Creation Custom Material Scenario
            materialScenario = new CustomMaterialScenario(false)
            {
                NumberOfSubMaterials = 1,
                AssociateSubMaterialsToContainer = true,
                Resource = resource
            };

            #endregion Material setup

            #region Configurations

            // Set resources FDC Communication attribute to true
            SetResourceFDCCommunication(resource, true);
            SetResourceFDCCommunication(subResource, true);


            // Get original config value for FDCActiveConfigPath to restore later
            fdcActiveConfig = (bool)ConfigUtilities.GetConfigValue(amsOSRAMConstants.FDCActiveConfigPath);
            ConfigUtilities.SetConfigValue(amsOSRAMConstants.FDCActiveConfigPath, true);
            #endregion Configurations
            
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            #region Terminate Integration Entries

            // Terminate integration entries created
            if (integrationEntriesToTerminate != null && integrationEntriesToTerminate.Count > 0)
            {
                foreach (IntegrationEntry entry in integrationEntriesToTerminate)
                {
                    entry.Load();
                    if (entry.UniversalState != Foundation.Common.Base.UniversalState.Terminated)
                    {
                        new TerminateIntegrationEntryInput()
                        {
                            IntegrationEntry = entry
                        }.TerminateIntegrationEntrySync();
                    }
                }
            }

            #endregion Terminate Integration Entries

            #region Restore Resource

            // Restore FDCCommunication attribute
            if (oldResourceFDCCommunicationValue.Count > 0) 
            {
                foreach (var keyValue in oldResourceFDCCommunicationValue)
                {
                    keyValue.Key.Load();
                    keyValue.Key.SaveAttribute(amsOSRAMConstants.CustomFDCCommunicationAttribute, keyValue.Value);
                }
            }

            // Restore IsRecipeManagementEnabled
            if (isRecipeManagementEnabled.HasValue)
            {
                resource.Load();
                resource.IsRecipeManagementEnabled = isRecipeManagementEnabled;
                resource.Save();
            }

            #endregion Restore Resource

            // Reset FDC Active Configuration to original value
            ConfigUtilities.SetConfigValue(amsOSRAMConstants.FDCActiveConfigPath, fdcActiveConfig);

            if (materialScenario != null)
            {
                materialScenario.Entity.Load();
                materialScenario.TearDown();
            }
        }

        /// <summary>
        /// Description:
        ///     -Set the FDC Active Configuration to false
        ///     -Create a lot material in order to trigger a track in operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCLotStart
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_FDCActiveConfigurationTest</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_FDCActiveConfigurationTest()
        {
            ConfigUtilities.SetConfigValue(amsOSRAMConstants.FDCActiveConfigPath, false);

            materialScenario.Setup(true);

            // Set the recipe context
            SetupRecipeContext(serviceName, materialScenario.Entity.Name, recipeName);

            DateTime fromDate = DateTime.Now;
            materialScenario.Entity.ComplexTrackIn();

            IntegrationEntry integrationEntry = new IntegrationEntry();

            GenericUtilities.WaitFor(() =>
            {
                // Get the IntegrationEntry
                integrationEntry = IntegrationEntryUtilities.GetIntegrationEntry(amsOSRAMConstants.MessageType_LOTIN, fromDate, true,
                            amsOSRAMConstants.SourceSystem_OntoFDC, amsOSRAMConstants.TargetSystem_OntoFDC);
                return integrationEntry != null;
            }, 2, pollingIntervalConfig / 10);

            Assert.IsNull(integrationEntry, "Integration entry should not have been created.");
        }

        /// <summary>
        /// Description:
        ///     -Set the FDC Active Configuration to false
        ///     -Create a lot material in order to trigger a track in operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCLotStart
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_FDCActiveConfigurationTest</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_ResourceFDCCommunicationAttributeTest()
        {
            resource.SaveAttribute(amsOSRAMConstants.CustomFDCCommunicationAttribute, false);

            materialScenario.Setup(true);

            // Set the recipe context
            SetupRecipeContext(serviceName, materialScenario.Entity.Name, recipeName);

            DateTime fromDate = DateTime.Now;
            materialScenario.Entity.ComplexTrackIn();
            
            IntegrationEntry integrationEntry = new IntegrationEntry();

            GenericUtilities.WaitFor(() =>
            {
                // Get the IntegrationEntry
                integrationEntry = IntegrationEntryUtilities.GetIntegrationEntry(amsOSRAMConstants.MessageType_LOTIN, fromDate, true,
                            amsOSRAMConstants.SourceSystem_OntoFDC, amsOSRAMConstants.TargetSystem_OntoFDC);
                return integrationEntry != null;
            }, 2, pollingIntervalConfig / 10);

            Assert.IsNull(integrationEntry, "Integration entry should not have been created.");
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
            materialScenario.Setup(true);

            // Set the recipe context
            SetupRecipeContext(serviceName, materialScenario.Entity.Name, recipeName);

            DateTime fromDate = DateTime.Now;
            materialScenario.Entity.ComplexTrackIn();

            ValidateIntegrationEntry(amsOSRAMConstants.MessageType_LOTIN, fromDate, true, materialScenario.Entity.Name, "", resource.Name, "", "", "", materialScenario.Entity.Step.Name, materialScenario.Entity.LastProcessedResource.LastService.Name, materialScenario.Entity.CurrentRecipeInstance?.ParentEntity?.Name, materialScenario.Entity.Product.Name, materialScenario.Entity.Flow.Name, materialScenario.Entity.PrimaryQuantity.ToString(), materialScenario.Entity.Facility.Name);
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
            materialScenario.Setup(true);

            // Set the recipe context
            SetupRecipeContext(serviceName, materialScenario.Entity.Name, recipeName);

            Assert.IsTrue(materialScenario.Entity.SubMaterialCount > 0, $"The material {materialScenario.Entity.Name} should have submaterials.");
            // Set config with false to avoid create a new integration entry
            ConfigUtilities.SetConfigValue(amsOSRAMConstants.FDCActiveConfigPath, false);
            materialScenario.Entity.ComplexTrackIn();
            materialScenario.Entity.LoadChildren();

            ConfigUtilities.SetConfigValue(amsOSRAMConstants.FDCActiveConfigPath, true);
            DateTime fromDate = DateTime.Now;
            materialScenario.Entity.SubMaterials.ComplexTrackInMaterials(subResource);
            materialScenario.SubMaterials[0].LoadRelation("MaterialContainer");

            ValidateIntegrationEntry(amsOSRAMConstants.MessageType_WAFERIN, fromDate, true, materialScenario.Entity.Name, materialScenario.SubMaterials[0].Name, subResource.Name, materialScenario.SubMaterials[0].MaterialContainer.First().Position.ToString(), materialScenario.SubMaterials[0].MaterialContainer.First().Position.ToString(), materialScenario.SubMaterials[0].PrimaryQuantity.Value.ToString());
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
            materialScenario.NumberOfSubMaterials = 0;
            materialScenario.Setup(true);

            // Set the recipe context
            SetupRecipeContext(serviceName, materialScenario.Entity.Name, recipeName);

            // Set config with false to avoid create a new integration entry
            ConfigUtilities.SetConfigValue(amsOSRAMConstants.FDCActiveConfigPath, false);
            materialScenario.Entity.ComplexTrackIn();
            materialScenario.Entity.Load();

            ConfigUtilities.SetConfigValue(amsOSRAMConstants.FDCActiveConfigPath, true);
            DateTime fromDate = DateTime.Now;
            materialScenario.Entity.ComplexTrackOutMaterial();
            materialScenario.Entity.Step.Load();

            ValidateIntegrationEntry(amsOSRAMConstants.MessageType_LOTOUT, fromDate, true, materialScenario.Entity.Name, "", "", "", "", "", materialScenario.Entity.Step.Name);
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

            // Set the recipe context
            SetupRecipeContext(serviceName, materialScenario.Entity.Name, recipeName);

            // Set config with false to avoid create a new integration entry
            ConfigUtilities.SetConfigValue(amsOSRAMConstants.FDCActiveConfigPath, false);
            materialScenario.Entity.ComplexTrackIn();
            materialScenario.Entity.LoadChildren();

            materialScenario.Entity.SubMaterials.ComplexTrackInMaterials(subResource);
            ConfigUtilities.SetConfigValue(amsOSRAMConstants.FDCActiveConfigPath, true);
            DateTime fromDate = DateTime.Now;
            materialScenario.Entity.SubMaterials[0].ComplexTrackOutMaterial();
            materialScenario.Entity.SubMaterials[0].Load();

            ValidateIntegrationEntry(amsOSRAMConstants.MessageType_WAFEROUT, fromDate, true, materialScenario.Entity.Name, materialScenario.SubMaterials[0].Name, subResource.Name, "", "", "", "", "", "", "", "", "", "", materialScenario.Entity.SubMaterials[0].SystemState.ToString());
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
            materialScenario.NumberOfSubMaterials = 0;
            materialScenario.Setup(true);

            // Set the recipe context
            SetupRecipeContext(serviceName, materialScenario.Entity.Name, recipeName);

            DateTime fromDate = DateTime.Now;
            materialScenario.Entity.ComplexTrackIn();
            materialScenario.Entity.Abort();
            materialScenario.Entity.Step.Load();

            ValidateIntegrationEntry(amsOSRAMConstants.MessageType_LOTOUT, fromDate, true, materialScenario.Entity.Name, "", "", "", "", "", materialScenario.Entity.Step.Name);
        }

        #region Help methods

        /// <summary>
        /// Validate Integration Entry
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="transactionSuccess"></param>
        /// <param name="expectedMaterialName"></param>
        private void ValidateIntegrationEntry(string messageType, DateTime fromDate, bool transactionSuccess, string expectedLotName = "", string expectedWaferName = "", string expectedChamberName = "", string expectedSlotPos = "",
            string expectedLotPos = "", string expectedQty = "", string expectedOperationName = "", string expectedSPSName = "", string expectedRecipeName = "", string expectedProductName = "", string expectedProductRouteName = "", string expectedNumberOfWafersInBatch = "",
            string expectedFacilityName = "", string expectedWaferState = "")
        {
            if (transactionSuccess)
            {
                // Validate that Integration Entry was created and processed
                GenericUtilities.WaitFor(() =>
                {
                    // Get the IntegrationEntry
                    integrationEntry = IntegrationEntryUtilities.GetIntegrationEntry(messageType, fromDate, true,
                                amsOSRAMConstants.SourceSystem_OntoFDC, amsOSRAMConstants.TargetSystem_OntoFDC);
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

                    case "WAFERIN":
                        foreach (XmlNode item in xml.DocumentElement.ChildNodes)
                        {
                            switch (item.Name)
                            {
                                case "WaferName":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property WaferName should not be empty!");

                                    Assert.AreEqual(expectedWaferName, item.InnerText.Trim(),
                                        $"The property WaferName should be {expectedWaferName}!");
                                    break;
                                case "LotName":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property LotName should not be empty!");

                                    Assert.AreEqual(expectedLotName, item.InnerText.Trim(),
                                        $"The property LotName should be {expectedLotName}!");
                                    break;
                                case "Chamber":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property Chamber should not be empty!");

                                    Assert.AreEqual(expectedChamberName, item.InnerText.Trim(),
                                        $"The property Chamber should be {expectedChamberName}!");
                                    break;
                                case "SlotPos":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property SlotPos should not be empty!");

                                    Assert.AreEqual(expectedSlotPos, item.InnerText.Trim(),
                                        $"The property SlotPos should be {expectedSlotPos}!");
                                    break;
                                case "LotPos":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property LotPos should not be empty!");

                                    Assert.AreEqual(expectedLotPos, item.InnerText.Trim(),
                                        $"The property LotPos should be {expectedLotPos}!");
                                    break;
                                case "QtyIn":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property QtyIn should not be empty!");

                                    Assert.AreEqual(Math.Round(Convert.ToDouble(expectedQty)).ToString(), item.InnerText.Trim(),
                                        $"The property QtyIn should be {expectedQty}!");
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
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property WaferName should not be empty!");

                                    Assert.AreEqual(expectedWaferName, item.InnerText.Trim(),
                                        $"The property WaferName should be {expectedWaferName}!");
                                    break;
                                case "LotName":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property LotName should not be empty!");

                                    Assert.AreEqual(expectedLotName, item.InnerText.Trim(),
                                        $"The property LotName should be {expectedLotName}!");
                                    break;
                                case "Chamber":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property Chamber should not be empty!");

                                    Assert.AreEqual(expectedChamberName, item.InnerText.Trim(),
                                        $"The property Chamber should be {expectedChamberName}!");
                                    break;
                                case "Processed":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property Processed should not be empty!");

                                    Assert.AreEqual("true", item.InnerText.Trim(),
                                        $"The property Processed should be true!");
                                    break;
                                case "WaferState":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property WaferState should not be empty!");

                                    Assert.AreEqual(expectedWaferState, item.InnerText.Trim(),
                                        $"The property WaferState should be {expectedWaferState}!");
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
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property LotName should not be empty!");

                                    Assert.AreEqual(expectedLotName, item.InnerText.Trim(),
                                        $"The property LotName should be {expectedLotName}!");
                                    break;
                                case "Operation":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property Operation should not be empty!");

                                    Assert.AreEqual(expectedOperationName, item.InnerText.Trim(),
                                        $"The property Operation should be {expectedOperationName}!");
                                    break;
                                case "SPS":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property SPS should not be empty!");

                                    Assert.AreEqual(expectedSPSName, item.InnerText.Trim(),
                                        $"The property SPS should be {expectedSPSName}!");
                                    break;
                                case "ProductName":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property ProductName should not be empty!");

                                    Assert.AreEqual(expectedProductName, item.InnerText.Trim(),
                                        $"The property ProductName should be {expectedProductName}!");
                                    break;
                                case "ProductRoute":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property ProductRoute should not be empty!");

                                    Assert.AreEqual(expectedProductRouteName, item.InnerText.Trim(),
                                        $"The property ProductRoute should be {expectedProductRouteName}!");
                                    break;
                                case "NumbersOfWafersInBatch":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property NumbersOfWafersInBatch should not be empty!");

                                    Assert.AreEqual(expectedNumberOfWafersInBatch, item.InnerText.Trim(),
                                        $"The property NumbersOfWafersInBatch should be {expectedNumberOfWafersInBatch}!");
                                    break;
                                case "FacilityName":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property FacilityName should not be empty!");

                                    Assert.AreEqual(expectedFacilityName, item.InnerText.Trim(),
                                        $"The property FacilityName should be {expectedFacilityName}!");
                                    break;
                                case "RecipeName":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property RecipeName should not be empty!");

                                    Assert.AreEqual(expectedRecipeName, item.InnerText.Trim(),
                                        $"The property RecipeName should be {expectedRecipeName}!");
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
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
                                        $"The property Operation should not be empty!");

                                    Assert.AreEqual(expectedOperationName, item.InnerText.Trim(),
                                        $"The property Operation should be {expectedOperationName}!");
                                    break;
                                case "LotName":
                                    Assert.IsTrue(!string.IsNullOrWhiteSpace(item.InnerText),
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
        /// Sets the FDCCommunication value of the given resource
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="fdcCommunicationValue"></param>
        private void SetResourceFDCCommunication(Resource resource, bool fdcCommunicationValue)
        {
            resource.LoadAttributes(new Collection<string> { amsOSRAMConstants.CustomFDCCommunicationAttribute });

            if (resource.Attributes != null &&
                resource.Attributes.ContainsKey(amsOSRAMConstants.CustomFDCCommunicationAttribute) &&
                resource.Attributes[amsOSRAMConstants.CustomFDCCommunicationAttribute] != null)
            {
                oldResourceFDCCommunicationValue.Add(resource, (bool)resource.Attributes[amsOSRAMConstants.CustomFDCCommunicationAttribute]);
            }

            resource.SaveAttribute(amsOSRAMConstants.CustomFDCCommunicationAttribute, fdcCommunicationValue);
            resource.Load();
        }

        /// <summary>
        /// Setup the recipe context smart table
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="materialName"></param>
        /// <param name="recipeName"></param>
        private void SetupRecipeContext(string serviceName, string materialName, string recipeName)
        {
            //add new constant
            SmartTable smartTable = TestScenarios.Others.TableUtilities.GetSmartTable("RecipeContext");
            smartTable.LoadData();

            DataSet ds = TestScenarios.Others.Utilities.ToDataSet(smartTable.Data);
            
            DataRow dr = ds.Tables[0].NewRow();
            dr["RecipeContextId"] = 0; // Stored Proc that manages the ST data filters by Id = 0 , not -1
            dr["LastServiceHistoryId"] = -1;
            dr["LastOperationHistorySeq"] = -1;
            dr["Service"] = serviceName;
            dr["Material"] = materialName;
            dr["Recipe"] = recipeName;

            ds.Tables[0].Rows.Add(dr);

            NgpDataSet ngpDataSetLimit = Cmf.TestScenarios.Others.Utilities.FromDataSet(ds);
            smartTable.InsertOrUpdateData(ngpDataSetLimit);
        }

        #endregion Help methods
    }
}
