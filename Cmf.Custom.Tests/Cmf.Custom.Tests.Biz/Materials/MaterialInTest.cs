using System;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cmf.Custom.Tests.Biz.Materials
{
    /// <summary>
    /// Summary description for MaterialInTest
    /// </summary>
    [TestClass]
    public class MaterialInTest
    {
        private static Resource resource = null;
        private CustomMaterialScenario materialScenario = null;
        private bool? isRecipeManagementEnabled = null;

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            #region Material setup

            //Change material to a step where the resource has subResource of type Process
            materialScenario = new CustomMaterialScenario(false)
            {
                FlowName = amsOSRAMConstants.TestFlow,
                StepName = amsOSRAMConstants.TestM3MTZnOSputterCluster6in00126F008_E
            };

            // Create the Material and TrackIn
            materialScenario.Setup(true);

            // Set IsRecipeManagementEnabled with false - and save old value to restore later
            if (materialScenario.ResourceMaterialInOut.IsRecipeManagementEnabled.GetValueOrDefault())
            {
                isRecipeManagementEnabled = materialScenario.ResourceMaterialInOut.IsRecipeManagementEnabled;
                materialScenario.ResourceMaterialInOut.IsRecipeManagementEnabled = false;
                materialScenario.ResourceMaterialInOut.Save();
            }

            #endregion Material setup
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            #region CustomMaterialScenario teardown

            if (materialScenario != null)
            {
                materialScenario.Entity.Load();
                // To ensure that when updating IsRecipeManagementEnabled, the Resource does not have any material in process
                resource = materialScenario.ResourceMaterialInOut;
                materialScenario.TearDown();
            }

            #endregion CustomMaterialScenario teardown

            #region Restore Resource

            // Restore IsRecipeManagementEnabled
            if (isRecipeManagementEnabled.HasValue)
            {
                resource.Load();
                resource.IsRecipeManagementEnabled = isRecipeManagementEnabled;
                resource.Save();
            }

            #endregion Restore Resource
        }

        /// <summary>
        /// Description: This test validates that SubMaterial is TrackedIn in a Resource
        /// Acceptance Criteria: SubMaterial must be in the state InProcess
        /// </summary>
        /// <TestCaseID>MaterialInTest.MaterialInTest_HappyPath</TestCaseID>
        /// <Author>Rui Oliveira</Author>
        [TestMethod]
        public void MaterialInTest_HappyPath()
        {
            materialScenario.TrackIn();

            #region Execution and validation of MaterialIn

            Material materialWafer = materialScenario.SubMaterials[0];

            Cmf.Custom.amsOSRAM.Orchestration.OutputObjects.MaterialInOutput materialInOutput = new Custom.amsOSRAM.Orchestration.InputObjects.MaterialInInput()
            {
                MaterialName = materialWafer.Name,
                ResourceName = materialScenario.ResourceMaterialInOut.Name
            }.MaterialInSync();

            materialWafer.Load();

            Assert.AreEqual(materialInOutput.Material.SystemState, MaterialSystemState.InProcess);

            #endregion
        }

        /// <summary>
        /// Description: This test validates that when a Carrirer Id is sent to the 
        /// the service the Parent Material is TrackedIn in a Resource
        /// </summary>
        /// <TestCaseID>MaterialInTest.MaterialInTest_SubResourceOrder</TestCaseID>
        /// <Author>Rui Oliveira</Author>
        [TestMethod]
        public void MaterialInTest_SubResourceOrder()
        {
            materialScenario.TrackIn();

            #region Execution and validation of MaterialIn

            Material materialWafer = materialScenario.SubMaterials[0];

            Cmf.Custom.amsOSRAM.Orchestration.OutputObjects.MaterialInOutput materialInOutput = new Custom.amsOSRAM.Orchestration.InputObjects.MaterialInInput()
            {
                MaterialName = materialWafer.Name,
                ResourceName = materialScenario.ResourceMaterialInOut.Name,
                SubResourceOrder = 1
            }.MaterialInSync();

            materialWafer.Load();

            Assert.AreEqual(materialInOutput.Material.SystemState, MaterialSystemState.InProcess);

            #endregion
        }

        /// <summary>
        /// Description: This test validates that when a Carrirer Id is sent to the 
        /// the service the Parent Material is TrackedIn in a Resource
        /// </summary>
        /// <TestCaseID>MaterialInTest.MaterialInTest_SubResourceOrder</TestCaseID>
        /// <Author>Rui Oliveira</Author>
        [TestMethod]
        public void MaterialInTest_SubResourceOrderOutOfBound()
        {
            materialScenario.TrackIn();

            #region Execution and validation of MaterialIn

            Material materialWafer = materialScenario.SubMaterials[0];
            try
            {
                Cmf.Custom.amsOSRAM.Orchestration.OutputObjects.MaterialInOutput materialInOutput = new Custom.amsOSRAM.Orchestration.InputObjects.MaterialInInput()
                {
                    MaterialName = materialWafer.Name,
                    ResourceName = materialScenario.ResourceMaterialInOut.Name,
                    SubResourceOrder = 20
                }.MaterialInSync();

                materialWafer.Load();

                Assert.AreEqual(materialInOutput.Material.SystemState, MaterialSystemState.InProcess);

            }
            catch (Exception ex)
            {
                string expectedErrorMessage = "Sub Resource Order parameter is Out of Range";
                Assert.IsTrue(ex.Message.ToLower().Contains(expectedErrorMessage.ToLower()),
                  string.Format("Expected error message does not match. [Expected Message: {0}], [Current Message: {1}]", expectedErrorMessage, ex.Message));
            }

            #endregion
        }

        /// <summary>
        /// Description: This test validates that the Parent Material is TrackedIn in a Resource
        /// </summary>
        /// <TestCaseID>MaterialInTest.MaterialInTest_TopMost</TestCaseID>
        /// <Author>Rui Oliveira</Author>
        [TestMethod]
        public void MaterialInTest_TopMost()
        {
            #region Execution and validation of MaterialIn

            Cmf.Custom.amsOSRAM.Orchestration.OutputObjects.MaterialInOutput materialInOutput = new Custom.amsOSRAM.Orchestration.InputObjects.MaterialInInput()
            {
                MaterialName = materialScenario.Entity.Name,
                ResourceName = materialScenario.ResourceMaterialInOut.Name,
                SubResourceOrder = 0
            }.MaterialInSync();

            materialScenario.Refresh();

            Assert.AreEqual(materialInOutput.Material.SystemState, MaterialSystemState.InProcess);

            #endregion
        }

        /// <summary>
        /// Perform material in but checking if main tool E10 state has changed to 'Productive'
        /// </summary>
        [TestMethod]
        public void MaterialInTest_Scenario_05_MaterialInMainToolE10StateChange()
        {
            materialScenario.ResourceMaterialInOut.ChangeResourceState("Standby");

            #region Execution and validation of MaterialIn

            Cmf.Custom.amsOSRAM.Orchestration.OutputObjects.MaterialInOutput materialInOutput = new Custom.amsOSRAM.Orchestration.InputObjects.MaterialInInput()
            {
                MaterialName = materialScenario.Entity.Name,
                ResourceName = materialScenario.ResourceMaterialInOut.Name,
                SubResourceOrder = 0
            }.MaterialInSync();

            materialScenario.Refresh();
            materialScenario.ResourceMaterialInOut.Load();

            Assert.AreEqual(materialInOutput.Material.SystemState, MaterialSystemState.InProcess);
            Assert.AreEqual(materialScenario.ResourceMaterialInOut.CurrentMainState.CurrentState.Name, "Productive");

            #endregion Execution and validation of MaterialIn
        }
    }
}
