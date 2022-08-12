using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cmf.Custom.Tests.Biz.Materials
{
    /// <summary>
    /// Summary description for MaterialOutTest
    /// </summary>
    [TestClass]
    public class MaterialOutTest
    {
        private Resource resource = null;
        private CustomMaterialScenario materialScenario = null;
        private bool? isRecipeManagementEnabled = null;

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
        /// Description: This test validates that SubMaterial is TrackedOut in the following scenarios:
        ///     1) SubMaterial is not in the state InProcess
        ///     2) SubMaterial is already in the state InProcess
        /// Acceptance Criteria: SubMaterial must be in the state Processed 
        /// </summary>
        /// <TestCaseID>MaterialOutTest.MaterialOutTest_HappyPath</TestCaseID>
        /// <Author>Rui Oliveira</Author>
        [TestMethod]
        public void MaterialOutTest_HappyPath()
        {
            #region Setup

            materialScenario = new CustomMaterialScenario(false);
            // Create the Material and TrackIn
            materialScenario.Setup(true);

            // To ensure that when updating IsRecipeManagementEnabled, the Resource does not have any material in process
            resource = materialScenario.ResourceMaterialInOut;

            // Set IsRecipeManagementEnabled with false - and save old value to restore later
            if (resource.IsRecipeManagementEnabled.GetValueOrDefault())
            {
                isRecipeManagementEnabled = resource.IsRecipeManagementEnabled;
                resource.IsRecipeManagementEnabled = false;
                resource.Save();
            }

            materialScenario.TrackIn();

            #endregion

            #region SubMaterial is not Tracked In before calling MaterialOut

            Material materialWafer = materialScenario.SubMaterials[0];

            new MaterialOutInput()
            {
                MaterialName = materialWafer.Name,
                ResourceName = resource.Name
            }.MaterialOutSync();

            materialWafer.Load();

            Assert.AreEqual(MaterialSystemState.Processed, materialWafer.SystemState, "Material state does not match!");

            #endregion

            #region SubMaterial is Tracked In before calling MaterialOut

            Material materialWafer2 = materialScenario.SubMaterials[1];
            materialWafer2.Load();

            new MaterialInInput()
            {
                MaterialName = materialWafer2.Name,
                ResourceName = resource.Name,
            }.MaterialInSync();


            materialWafer2.Load();

            new MaterialOutInput()
            {
                MaterialName = materialWafer2.Name,
                ResourceName = resource.Name
            }.MaterialOutSync();

            materialWafer2.Load();

            Assert.AreEqual(MaterialSystemState.Processed, materialWafer2.SystemState, "Material state does not match!");

            #endregion
        }

        /// <summary>
        /// Description: This test validates that when a Material is TackedIn
        /// all the SubMaterials can be TrackedOut and then a TrackOut and Move Next
        /// is performed on the TopMostMaterial
        /// </summary>
        /// <TestCaseID>MaterialOutTest.MaterialOutTest_WithTrackedInContainer</TestCaseID>
        /// <Author>Rui Oliveira</Author>
        [TestMethod]
        public void MaterialOutTest_WithTrackedInContainer()
        {
            #region Setup

            materialScenario = new CustomMaterialScenario(false);

            //Change material to a step where the resource has subResource of type Process
            materialScenario.FlowName = AMSOsramConstants.TestFlow;
            materialScenario.StepName = AMSOsramConstants.TestM3MTZnOSputterCluster6in00126F008_E;
            materialScenario.NumberOfSubMaterials = 1;

            // Create the Material and TrackIn
            materialScenario.Setup(true);

            resource = materialScenario.ResourceMaterialInOut;

            // Set IsRecipeManagementEnabled with false - and save old value to restore later
            if (resource.IsRecipeManagementEnabled.GetValueOrDefault())
            {
                isRecipeManagementEnabled = resource.IsRecipeManagementEnabled;
                resource.IsRecipeManagementEnabled = false;
                resource.Save();
            }

            materialScenario.TrackIn();

            #endregion

            #region Track In and TrackOut SubMaterials

            foreach (Material item in materialScenario.SubMaterials)
            {
                new MaterialOutInput()
                {
                    MaterialName = item.Name,
                    ResourceName = resource.Name
                }.MaterialOutSync();
            }
            #endregion

            #region TrackOut And MoveNext Parent Material

            new MaterialOutInput()
            {
                CarrierId = materialScenario.ContainerScenario.Entity.Name,
                ResourceName = resource.Name
            }.MaterialOutSync();

            materialScenario.Entity.Load();

            Assert.AreEqual(MaterialSystemState.Queued, materialScenario.Entity.SystemState, "Material state does not match!");

            #endregion
        }

        /// <summary>
        /// Description: This test validates that when a Material is not TackedIn
        /// and SubMaterial Tracking is false then the TopMostMaterial is 
        /// Dispatched and TrackedIn and the TrackedOut and moved to the Next Step
        /// </summary>
        /// <TestCaseID>MaterialOutTest.MaterialOutTest_WithOutTrackedInContainer</TestCaseID>
        /// <Author>Rui Oliveira</Author>
        [TestMethod]
        public void MaterialOutTest_WithOutTrackedInContainer()
        {
            #region Setup

            materialScenario = new CustomMaterialScenario(false);
            materialScenario.NumberOfSubMaterials = 1;
            materialScenario.Setup();

            resource = new Resource() { Name = "PDSP0101" };
            resource.Load();

            // Set IsRecipeManagementEnabled with false - and save old value to restore later
            if (resource.IsRecipeManagementEnabled.GetValueOrDefault())
            {
                isRecipeManagementEnabled = resource.IsRecipeManagementEnabled;
                resource.IsRecipeManagementEnabled = false;
                resource.Save();
            }

            //Change material to a step where the resource has subResource of type Process
            Flow flow = new Flow() { Name = AMSOsramConstants.TestFlow };
            flow.Load();

            Step step = new Step() { Name = AMSOsramConstants.TestM3MTZnOSputterCluster6in00126F008_E };
            step.Load();

            string flowPath = FlowExtensionMethods.CustomGetFlowPath(flow, step.Name);

            materialScenario.ChangeFlowAndStep(flow, step, flowPath);

            new MaterialOutInput()
            {
                CarrierId = materialScenario.ContainerScenario.Entity.Name,
                ResourceName = resource.Name
            }.MaterialOutSync();

            materialScenario.Entity.Load();

            Assert.AreEqual(MaterialSystemState.Queued, materialScenario.Entity.SystemState, "Material state does not match!");

            #endregion
        }

        /// <summary>
        /// Perform material out but checking if main tool E10 state has changed to 'Standby'
        /// </summary>
        [TestMethod]
        public void MaterialInTest_Scenario_04_MaterialInAndOutMainToolE10StateChange()
        {
            #region Setup

            materialScenario = new CustomMaterialScenario(false);
            //Change material to a step where the resource has subResource of type Process
            materialScenario.FlowName = AMSOsramConstants.TestFlow;
            materialScenario.StepName = AMSOsramConstants.TestM3MTZnOSputterCluster6in00126F008_E;
            materialScenario.NumberOfSubMaterials = 0;

            // Create the Material and TrackIn
            materialScenario.Setup(true);

            // Set IsRecipeManagementEnabled with false - and save old value to restore later
            resource = materialScenario.ResourceMaterialInOut;

            // Set IsRecipeManagementEnabled with false - and save old value to restore later
            if (materialScenario.ResourceMaterialInOut.IsRecipeManagementEnabled.GetValueOrDefault())
            {
                isRecipeManagementEnabled = resource.IsRecipeManagementEnabled;
                resource.IsRecipeManagementEnabled = false;
                resource.Save();
            }

            resource.ChangeResourceState("Standby");

            #endregion

            #region Execution and validation of MaterialIn

            new MaterialInInput()
            {
                MaterialName = materialScenario.Entity.Name,
                ResourceName = resource.Name,
                SubResourceOrder = 0
            }.MaterialInSync();

            materialScenario.Refresh();
            materialScenario.ResourceMaterialInOut.Load();

            Assert.AreEqual(materialScenario.Entity.SystemState, MaterialSystemState.InProcess);
            Assert.AreEqual(materialScenario.ResourceMaterialInOut.CurrentMainState.CurrentState.Name, "Productive");

            new MaterialOutInput()
            {
                MaterialName = materialScenario.Entity.Name,
                ResourceName = resource.Name
            }.MaterialOutSync();

            materialScenario.Refresh();
            materialScenario.ResourceMaterialInOut.Load();

            Assert.AreEqual(materialScenario.Entity.SystemState, MaterialSystemState.Queued);
            Assert.AreEqual(resource.CurrentMainState.CurrentState.Name, "Standby");

            #endregion
        }
    }
}
