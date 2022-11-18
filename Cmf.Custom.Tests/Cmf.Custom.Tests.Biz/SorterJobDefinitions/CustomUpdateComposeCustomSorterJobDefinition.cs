using Cmf.Custom.amsOSRAM.BusinessObjects;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.BusinessOrchestration;
using Cmf.Foundation.BusinessOrchestration.ChangeSetManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.ChangeSetManagement.OutputObjects;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.OutputObjects;
using Cmf.Foundation.BusinessOrchestration.LocalizationManagement.InputObjects;
using Cmf.LightBusinessObjects.Infrastructure.Errors;
using Cmf.Navigo.BusinessObjects;
using Cmf.TestScenarios.MaterialManagement.MaterialScenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.SorterJobDefinitions
{
    [TestClass]
    public class CustomUpdateComposeCustomSorterJobDefinition
    {
        private static string BOMName = amsOSRAMConstants.BOM_BOM_11018814;
        private static string FlowPath = amsOSRAMConstants.FlowPathEPI_EPISorting;
        private static string SmartTableToClear = amsOSRAMConstants.CustomSorterJobDefinitionContextSmartTable;

        private static Step step = null;
        private static SmartTableManager classSmartTableManager = null;

        private static SmartTableManager testSmartTableManager = null;
        private static CustomBOMScenario testCustomBOMScenario = null;

        /// <summary>
        /// Classes the initialize.
        /// </summary>
        /// <param name="context">The context.</param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            step = GenericUtilities.GetStepFromFlowPath(FlowPath);

            classSmartTableManager = new SmartTableManager();
            classSmartTableManager.ClearSmartTable(SmartTableToClear);
        }

        /// <summary>
        /// Classes the cleanup.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // Rollback Scenario
            if (classSmartTableManager != null)
            {
                classSmartTableManager.UndoSmartTableChanges(SmartTableToClear);
            }
        }
        
        /// <summary>
        /// Tests initialization.
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            testCustomBOMScenario = new CustomBOMScenario();
        }

        /// <summary>
        /// Cleans up scenario.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (testCustomBOMScenario != null)
            {
                testCustomBOMScenario.CompleteCleanUp();
            }

            testSmartTableManager = new SmartTableManager();
            testSmartTableManager.ClearSmartTable(SmartTableToClear);
        }

        /// <summary>
        /// Description: 
        ///     Validates if CustomSorterJob are automatically updated when a new BOM is created and/or set effective with "StartingCarrierType" and "IsForLotCompose" attribute
        /// Acceptance criteria: 
        ///     CustomSorterJob is created on BOM set effective
        /// </summary>
        /// <TestCaseId>CustomUpdateComposeCustomSorterJobDefinition_NewBOMEffective_WithLotComposeAndStartingCarrierType</TestCaseId>
        /// <author>Oliverio Sousa</author>
        [TestMethod]
        public void CustomUpdateComposeCustomSorterJobDefinition_NewBOMEffective_WithLotComposeAndStartingCarrierType()
        {
            (BOM bom, RequestChangeSetApprovalOutput requestChangeSetApprovalOutput, CustomSorterJobDefinition customSorterJobDefinition) = testCustomBOMScenario.GenerateClonedBOM(BOMName, startingCarrierType: amsOSRAMConstants.ContainerSMIFPod, isForLotCompose: true);

            ValidateBOMCloneOrCreation(requestChangeSetApprovalOutput?.FeedbackMessages, customSorterJobDefinition);
            ValidateCustomSorterJobDefinitionCreation(customSorterJobDefinition);
        }

        /// <summary>
        /// Description: 
        ///     Validates if throws an error if a new BOM is created and/or set effective with "IsForLotCompose" attribute but without "StartingCarrierType" 
        /// Acceptance criteria: 
        ///     Should throw an error because BOM used for Lot Compose requires the StartingCarrierType attribute.
        /// </summary>
        /// <TestCaseId>CustomUpdateComposeCustomSorterJobDefinition_NewBOMEffective_WithLotComposeAndWithoutStartingCarrierType</TestCaseId>
        /// <author>Oliverio Sousa</author>
        [TestMethod]
        public void CustomUpdateComposeCustomSorterJobDefinition_NewBOMEffective_WithLotComposeAndWithoutStartingCarrierType()
        {
            string bomName = "TestBOM_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

            CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => testCustomBOMScenario.GenerateClonedBOM(BOMName, bomName: bomName, isForLotCompose: true));
            string errorMessage = CustomUtilities.GetLocalizedMessageByName("CustomLocalizedMessageBomForLotComposeRequiresStartingCarrierType", bomName);

            Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Missing/Wrong feedback message. Should have the following message: {errorMessage}");
        }

        /// <summary>
        /// Description: 
        ///     Validates if CustomSorterJob is not created if a new BOM is created and/or set effective without "IsForLotCompose" attribute 
        /// Acceptance criteria: 
        ///     CustomSorterJob is not created on BOM set effective
        /// </summary>
        /// <TestCaseId>CustomUpdateComposeCustomSorterJobDefinition_NewBOMEffective_WithoutLotCompose</TestCaseId>
        /// <author>Oliverio Sousa</author>
        [TestMethod]
        public void CustomUpdateComposeCustomSorterJobDefinition_NewBOMEffective_WithoutLotCompose()
        {
            (BOM bom, RequestChangeSetApprovalOutput requestChangeSetApprovalOutput, CustomSorterJobDefinition customSorterJobDefinition) = testCustomBOMScenario.GenerateClonedBOM(BOMName, startingCarrierType: amsOSRAMConstants.ContainerSMIFPod, isForLotCompose: false);

            Assert.IsFalse(customSorterJobDefinition.ObjectExists(), $"The CustomSorterJobDefinition {customSorterJobDefinition.Name} should not exist in the system!");
        }

        /// <summary>
        /// Description: 
        ///     Validates if CustomSorterJob are automatically updated when a new BOM is created and/or set effective with "StartingCarrierType" and "IsForLotCompose" attribute
        ///     Creates a new version of the BOM
        ///     Terminates the first CustomSorterJob and set the first version as effective
        ///     Creates a third version of the BOM
        ///     Set the second version as the effective
        ///     
        /// Acceptance criteria: 
        ///     CustomSorterJob is created on BOM set effective
        ///     A second CustomSorterJob is created with the BOM update
        ///     Should have a message to warn the user about the termination of the CustomSorterJob
        ///     A third CustomSorterJob is created with the BOM update
        ///     A forth CustomSorterJob is created with the second version of the BOM
        ///     
        /// </summary>
        /// <TestCaseId>CustomUpdateComposeCustomSorterJobDefinition_SwitchBOMVersionsWithSmartTableEntry</TestCaseId>
        /// <author>Oliverio Sousa</author>
        [TestMethod]
        public void CustomUpdateComposeCustomSorterJobDefinition_SwitchBOMVersionsWithSmartTableEntry()
        {
            (BOM bomVersion1, RequestChangeSetApprovalOutput requestChangeSetApprovalOutput, CustomSorterJobDefinition customSorterJobDefinition1) = testCustomBOMScenario.GenerateClonedBOM(BOMName, startingCarrierType: amsOSRAMConstants.ContainerSMIFPod, isForLotCompose: true);

            ValidateBOMCloneOrCreation(requestChangeSetApprovalOutput?.FeedbackMessages, customSorterJobDefinition1);
            ValidateCustomSorterJobDefinitionCreation(customSorterJobDefinition1);

            // Adding the entry to the smart table
            CustomSorterJobContextInsertRow(step.Name, customSorterJobDefinition1.Name);
            ValidateCustomSorterJobDefinitionContextSmartTableUpdate(customSorterJobDefinition1);

            // Creating a new version of the BOM
            (BOM bomVersion2, requestChangeSetApprovalOutput, CustomSorterJobDefinition customSorterJobDefinition2) = testCustomBOMScenario.CreateNewBOMVersion(bomVersion1);
            ValidateBOMUpdate(requestChangeSetApprovalOutput?.FeedbackMessages, customSorterJobDefinition2);
            ValidateCustomSorterJobDefinitionContextSmartTableUpdate(customSorterJobDefinition2);

            // Terminate the Custom Sorter Job Definition for BOM version 1
            customSorterJobDefinition1.Load();
            customSorterJobDefinition1.Terminate();

            // Set BOM version 1 effective
            SetObjectVersionEffectiveOutput setObjectVersionEffectiveOutput = new SetObjectVersionEffectiveInput
            {
                Object = bomVersion1,
                IgnoreLastServiceId = true
            }.SetObjectVersionEffectiveSync();

            ValidateCustomSorterJobDefinitionTermination(setObjectVersionEffectiveOutput?.FeedbackMessages, customSorterJobDefinition1);
            ValidateCustomSorterJobDefinitionContextSmartTableUpdate(customSorterJobDefinition2);

            // Create a BOM version 3
            (BOM bomVersion3, requestChangeSetApprovalOutput, CustomSorterJobDefinition customSorterJobDefinition3) = testCustomBOMScenario.CreateNewBOMVersion(bomVersion2);
            ValidateBOMCloneOrCreation(requestChangeSetApprovalOutput?.FeedbackMessages, customSorterJobDefinition3);
            ValidateCustomSorterJobDefinitionCreation(customSorterJobDefinition3);

            // Set BOM version 2 effective
            setObjectVersionEffectiveOutput = new SetObjectVersionEffectiveInput
            {
                Object = bomVersion2,
                IgnoreLastServiceId = true
            }.SetObjectVersionEffectiveSync();

            ValidateBOMCloneOrCreation(setObjectVersionEffectiveOutput.FeedbackMessages, customSorterJobDefinition2);
            ValidateCustomSorterJobDefinitionContextSmartTableUpdate(customSorterJobDefinition2);
        }

        /// <summary>
        /// Description: 
        ///     Validates if CustomSorterJob are automatically updated when a new BOM is created and/or set effective with "StartingCarrierType" and "IsForLotCompose" attribute
        ///     Adds two rows to SmartTable
        ///     Creates a new version of the BOM
        ///     
        /// Acceptance criteria: 
        ///     CustomSorterJob is created on BOM set effective
        ///     A second CustomSorterJob is created with the BOM update
        ///     Should update all row from SmartTable
        ///     
        /// </summary>
        /// <TestCaseId>CustomUpdateComposeCustomSorterJobDefinition_UpdateTwoSmartTableRowsForNewBOMVersion</TestCaseId>
        /// <author>Oliverio Sousa</author>
        [TestMethod]
        public void CustomUpdateComposeCustomSorterJobDefinition_UpdateTwoSmartTableRowsForNewBOMVersion()
        {
            (BOM bomVersion1, RequestChangeSetApprovalOutput requestChangeSetApprovalOutput, CustomSorterJobDefinition customSorterJobDefinition1) = testCustomBOMScenario.GenerateClonedBOM(BOMName, startingCarrierType: amsOSRAMConstants.ContainerSMIFPod, isForLotCompose: true);

            ValidateBOMCloneOrCreation(requestChangeSetApprovalOutput?.FeedbackMessages, customSorterJobDefinition1);
            ValidateCustomSorterJobDefinitionCreation(customSorterJobDefinition1);

            // Add two entries to the smart table
            CustomSorterJobContextInsertRow(step.Name, customSorterJobDefinition1.Name);
            CustomSorterJobContextInsertRow("EPI Proposal", customSorterJobDefinition1.Name);

            ValidateCustomSorterJobDefinitionContextSmartTableUpdate(customSorterJobDefinition1);

            // Creating a new version of the BOM
            (BOM bomVersion2, requestChangeSetApprovalOutput, CustomSorterJobDefinition customSorterJobDefinition2) = testCustomBOMScenario.CreateNewBOMVersion(bomVersion1);

            ValidateBOMUpdate(requestChangeSetApprovalOutput?.FeedbackMessages, customSorterJobDefinition2);
            ValidateCustomSorterJobDefinitionCreation(customSorterJobDefinition2);

            ValidateCustomSorterJobDefinitionContextSmartTableUpdate(customSorterJobDefinition2);
        }

        #region HelpMethods

        /// <summary>
        /// Validates FeedbackMessage after creating or cloning a BOM
        /// </summary>
        /// <param name="feedbackMessages">The feedback messages.</param>
        /// <param name="customSorterJobDefinition">The custom sorter job definition.</param>
        private void ValidateBOMCloneOrCreation(Collection<FeedbackMessage> feedbackMessages, CustomSorterJobDefinition customSorterJobDefinition)
        {          
            string errorMessage = CustomUtilities.GetLocalizedMessageByName("CustomLocalizedMessageCustomSorterJobDefinitionContextConfigurationNeeded",
                                                                            customSorterJobDefinition.Name, amsOSRAMConstants.CustomSorterJobDefinitionContextSmartTable);

            Assert.IsTrue(feedbackMessages.Any(fm => fm.Message.Equals(errorMessage, StringComparison.InvariantCultureIgnoreCase)),
                $"Missing/Wrong feedback message. Should have the following message: {errorMessage}");
        }

        /// <summary>
        /// Validates FeedbackMessage after BOM update
        /// </summary>
        /// <param name="feedbackMessages">The feedback messages.</param>
        /// <param name="customSorterJobDefinition">The custom sorter job definition.</param>
        private void ValidateBOMUpdate(Collection<FeedbackMessage> feedbackMessages, CustomSorterJobDefinition customSorterJobDefinition)
        {
            string errorMessage = CustomUtilities.GetLocalizedMessageByName("CustomLocalizedMessageCustomSorterJobDefinitionContextUpdated",
                                                                            amsOSRAMConstants.CustomSorterJobDefinitionContextSmartTable, customSorterJobDefinition.Name);

            Assert.IsTrue(feedbackMessages.Any(fm => fm.Message.Equals(errorMessage, StringComparison.InvariantCultureIgnoreCase)),
                $"Missing/Wrong feedback message. Should have the following message: {errorMessage}");
        }

        /// <summary>
        /// Validates the custom sorter job definition termination.
        /// </summary>
        /// <param name="feedbackMessages">The feedback messages.</param>
        /// <param name="customSorterJobDefinition">The custom sorter job definition.</param>
        private void ValidateCustomSorterJobDefinitionTermination(Collection<FeedbackMessage> feedbackMessages, CustomSorterJobDefinition customSorterJobDefinition)
        {
            string errorMessage = CustomUtilities.GetLocalizedMessageByName("CustomLocalizedMessageCustomSorterJobDefinitionTerminated",
                                                                            customSorterJobDefinition.Name, amsOSRAMConstants.CustomSorterJobDefinitionContextSmartTable);

            Assert.IsTrue(feedbackMessages.Any(fm => fm.Message.Equals(errorMessage, StringComparison.InvariantCultureIgnoreCase)),
                $"Missing/Wrong feedback message. Should have the following message: {errorMessage}");
        }

        /// <summary>
        /// Validates if CustomSorterJobDefinition was created
        /// </summary>
        /// <param name="customSorterJobDefinition">The custom sorter job definition.</param>
        private void ValidateCustomSorterJobDefinitionCreation(CustomSorterJobDefinition customSorterJobDefinition)
        {
            Assert.IsNotNull(customSorterJobDefinition, $"A CustomSorterJobDefinition should be created");
            Assert.IsTrue(customSorterJobDefinition.ObjectExists(), $"The CustomSorterJobDefinition {customSorterJobDefinition.Name} does not exist in the system!");
        }

        /// <summary>
        /// Validates the custom sorter job definition context smart table update.
        /// </summary>
        /// <param name="customSorterJobDefinition">The custom sorter job definition.</param>
        private void ValidateCustomSorterJobDefinitionContextSmartTableUpdate(CustomSorterJobDefinition customSorterJobDefinition)
        {
            SmartTable smartTable = Cmf.TestScenarios.Others.TableUtilities.GetSmartTable(amsOSRAMConstants.CustomSorterJobDefinitionContextSmartTable);
            smartTable.LoadData();

            DataSet dataSet = Cmf.TestScenarios.Others.Utilities.ToDataSet(smartTable.Data);
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Assert.IsTrue(row["CustomSorterJobDefinition"].ToString() == customSorterJobDefinition.Name, "The Custom Sorter Job Definition should have been updated");
            }
        }

        /// <summary>
        /// Inserts a new row in the CustomSorterJobDefinitionContext Smart Table
        /// </summary>
        /// <param name="stepName">Name of the step inserted into the Smart Table</param>
        /// <param name="customSorterJobDefinition">CustomSorterJobDefinition inserted into the Smart Table</param>
        private void CustomSorterJobContextInsertRow(string stepName, string customSorterJobDefinition)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("CustomSorterJobDefinitionContextId", "-1");
            data.Add("LastServiceHistoryId", "-1");
            data.Add("LastOperationHistorySeq", "-1");
            data.Add("Step", stepName);
            data.Add("CustomSorterJobDefinition", customSorterJobDefinition);

            TableUtilities.GenericInsertOnSmartTable(amsOSRAMConstants.CustomSorterJobDefinitionContextSmartTable, data);
        }

        #endregion HelpMethods
    }
}
