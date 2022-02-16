using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.TestScenarios.Others;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Cmf.Custom.Tests.Biz.SmartTables
{
    [TestClass]
    public class CustomMaterialNiceLabelPrintContext
    {
        // Get SmartTable CustomMaterialNiceLabelPrintContext
        private SmartTable smartTable = GenericGetsScenario.GetObjectByName<SmartTable>("CustomMaterialNiceLabelPrintContext");

        /// <summary>
        /// Description:
        ///     - Validate Properties of SmartTable CustomMaterialNiceLabelPrintContext
        ///
        /// Acceptance Criteria:
        ///     - Properties on list expectedPropreties matches to list SmartTableProperties on Name and Position of both lists.
        ///
        /// </summary>
        /// <TestCaseID>CustomMaterialNiceLabelPrintContext.CustomMaterialNiceLabelPrintContext_CheckSmartTableProperties_HappyPath</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomMaterialNiceLabelPrintContext_CheckSmartTableProperties_HappyPath()
        {
            List<string> expectedProperties = new List<string>()
            {
                "Step",
                "LogicalFlowPath",
                "Product",
                "ProductGroup",
                "Flow",
                "Material",
                "MaterialType",
                "Resource",
                "ResourceType",
                "Model",
                "Operation",
                "Printer",
                "Label",
                "Quantity",
                "IsEnabled"
            };

            /// <Step>
            /// Iterate SmartTableProperties.
            /// </Step>
            for (int i = 0; i <= smartTable.SmartTableProperties.Count - 1; i++)
            {
                /// <ExpectedResult>
                /// The expected Propertie is equals to Name value on SmartTableProperties.
                /// </ExpectedResult>
                Assert.IsTrue(expectedProperties[i].Equals(smartTable.SmartTableProperties[i].Name),
                              $"The property {smartTable.SmartTableProperties[i].Name} match to expected property on iteration {i}.");
            }
        }

        /// <summary>
        /// Description:
        ///     - Validate Precedence Keys of SmartTable CustomMaterialNiceLabelPrintContext
        ///
        /// Acceptance Criteria:
        ///     - Properties on list expectedPrecedenceKeys matches to list SmartTablePrecedenceKeys on RuleName and Position of both lists.
        ///
        /// </summary>
        /// <TestCaseID>CustomMaterialNiceLabelPrintContext.CustomMaterialNiceLabelPrintContext_CheckSmartTablePrecedenceKeys_HappyPath</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomMaterialNiceLabelPrintContext_CheckSmartTablePrecedenceKeys_HappyPath()
        {
            List<string> expectedPrecedenceKeys = new List<string>()
            {
                "Step+Material+Operation",
                "Step+Product+LogicalFlowPath+MaterialType+Operation",
                "Step+Product+Flow+MaterialType+Operation",
                "Step+ProductGroup+LogicalFlowPath+MaterialType+Operation",
                "Step+ProductGroup+Flow+MaterialType+Operation",
                "Step+Product+LogicalFlowPath+Operation",
                "Step+Product+Flow+Operation",
                "Step+Product+MaterialType+Operation",
                "Step+Product+Resource+Operation",
                "Step+Product+ResourceType+Operation",
                "Step+Product+Model+Operation",
                "Step+Product+Operation",
                "Step+ProductGroup+LogicalFlowPath+Operation",
                "Step+ProductGroup+Flow+Operation",
                "Step+ProductGroup+MaterialType+Operation",
                "Step+ProductGroup+Resource+Operation",
                "Step+ProductGroup+ResourceType+Operation",
                "Step+ProductGroup+Model+Operation",
                "Step+ProductGroup+Operation",
                "Step+MaterialType+LogicalFlowPath+Operation",
                "Step+MaterialType+Flow+Operation",
                "Step+MaterialType+Resource+Operation",
                "Step+MaterialType+ResourceType+Operation",
                "Step+MaterialType+Model+Operation",
                "Step+MaterialType+Operation",
                "Step+Resource+Operation",
                "Step+ResourceType+Operation",
                "Step+Model+Operation",
                "Step+LogicalFlowPath+Operation",
                "Step+Flow+Operation",
                "Step+Operation"
            };

            /// <Step>
            /// Iterate SmartTablePrecedenceKeys.
            /// </Step>
            for (int i = 0; i < smartTable.SmartTablePrecedenceKeys.Count - 1; i++)
            {
                /// <ExpectedResult>
                /// The expected Precedence Key is equals to Rule value on SmartTablePrecedenceKeys.
                /// </ExpectedResult>
                Assert.IsTrue(expectedPrecedenceKeys[i].Equals(smartTable.SmartTablePrecedenceKeys[i].Rule),
                              $"The precedence key {smartTable.SmartTablePrecedenceKeys[i].Rule} match to expected predence key on iteration {i}.");
            }
        }
    }
}
