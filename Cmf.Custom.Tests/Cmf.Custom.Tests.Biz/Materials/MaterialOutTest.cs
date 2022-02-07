using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            CustomMaterialScenario materialScenario = new CustomMaterialScenario(false);

            try
            {
                #region Setup

                // Create the Material and TrackIn
                materialScenario.Setup(true);

                materialScenario.TrackIn();

                #endregion

                #region SubMaterial is not Tracked In before calling MaterialOut

                Material materialWafer = materialScenario.SubMaterials[0];

                Cmf.Custom.AMSOsram.Orchestration.OutputObjects.MaterialOutOutput materialOutOutput = new Custom.AMSOsram.Orchestration.InputObjects.MaterialOutInput()
                {
                    MaterialName = materialWafer.Name,
                    ResourceName = materialScenario.ResourceMaterialInOut.Name
                }.MaterialOutSync();

                materialWafer.Load();

                Assert.AreEqual(MaterialSystemState.Processed, materialWafer.SystemState, "Material state does not match!");

                #endregion

                #region SubMaterial is Tracked In before calling MaterialOut

                Material materialWafer2 = materialScenario.SubMaterials[1];
                materialWafer2.Load();

                Cmf.Custom.AMSOsram.Orchestration.OutputObjects.MaterialInOutput materialInOutput = new MaterialInInput()
                {
                    MaterialName = materialWafer2.Name,
                    ResourceName = materialScenario.ResourceMaterialInOut.Name,
                }.MaterialInSync();


                materialWafer2.Load();

                materialOutOutput = new Custom.AMSOsram.Orchestration.InputObjects.MaterialOutInput()
                {
                    MaterialName = materialWafer2.Name,
                    ResourceName = materialScenario.ResourceMaterialInOut.Name
                }.MaterialOutSync();

                materialWafer2.Load();

                Assert.AreEqual(MaterialSystemState.Processed, materialWafer2.SystemState, "Material state does not match!");

                #endregion
            }
            finally
            {

                materialScenario.TearDown();

            }

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
            CustomMaterialScenario materialScenario = new CustomMaterialScenario(false);
            Cmf.Custom.AMSOsram.Orchestration.OutputObjects.MaterialOutOutput materialOutOutput;

            try
            {
                #region Setup

                //Change material to a step where the resource has subResource of type Process
                materialScenario.FlowName = AMSOsramConstants.TestFlow;
                materialScenario.StepName = AMSOsramConstants.TestStepMTL10NitrideDeposition;
                materialScenario.NumberOfSubMaterials = 1;

                // Create the Material and TrackIn
                materialScenario.Setup(true);

                materialScenario.TrackIn();

                #endregion

                #region Track In and TrackOut SubMaterials

                foreach (Material item in materialScenario.SubMaterials)
                {
                    materialOutOutput = new Custom.AMSOsram.Orchestration.InputObjects.MaterialOutInput()
                    {
                        MaterialName = item.Name,
                        ResourceName = materialScenario.ResourceMaterialInOut.Name
                    }.MaterialOutSync();
                }
                #endregion

                #region TrackOut And MoveNext Parent Material

                materialOutOutput = new Custom.AMSOsram.Orchestration.InputObjects.MaterialOutInput()
                {
                   CarrierId= materialScenario.ContainerScenario.Entity.Name,
                    ResourceName = materialScenario.ResourceMaterialInOut.Name
                }.MaterialOutSync();

                materialScenario.Entity.Load();

                Assert.AreEqual(MaterialSystemState.Queued, materialScenario.Entity.SystemState, "Material state does not match!");

                #endregion

            }
            finally
            {

                materialScenario.TearDown();

            }
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
            CustomMaterialScenario materialScenario = new CustomMaterialScenario(false);
            Cmf.Custom.AMSOsram.Orchestration.OutputObjects.MaterialOutOutput materialOutOutput;

            try
            {
                #region Setup
                materialScenario.NumberOfSubMaterials = 1;
                materialScenario.Setup();

                //Change material to a step where the resource has subResource of type Process
                Flow flow = new Flow() { Name = AMSOsramConstants.TestFlow };
                flow.Load();

                Step step = new Step() { Name = AMSOsramConstants.TestStepMTL1Scrub62 };
                step.Load();

                string flowPath = FlowExtensionMethods.CustomGetFlowPath(flow, step.Name);

                materialScenario.ChangeFlowAndStep(flow, step, flowPath);

                materialOutOutput = new Custom.AMSOsram.Orchestration.InputObjects.MaterialOutInput()
                {
                    CarrierId = materialScenario.ContainerScenario.Entity.Name,
                    ResourceName = "ENW06"
                }.MaterialOutSync();

                materialScenario.Entity.Load();

                Assert.AreEqual(MaterialSystemState.Queued, materialScenario.Entity.SystemState, "Material state does not match!");

                #endregion

            }
            finally
            {

                materialScenario.TearDown();

            }
        }

        /// <summary>
        /// Perform material out but checking if main tool E10 state has changed to 'Standby'
        /// </summary>
        [TestMethod]
        public void MaterialInTest_Scenario_04_MaterialInAndOutMainToolE10StateChange()
        {
            CustomMaterialScenario materialScenario = new CustomMaterialScenario(false);

            try
            {
                #region Setup

                //Change material to a step where the resource has subResource of type Process
                materialScenario.FlowName = AMSOsramConstants.TestFlow;
                materialScenario.StepName = AMSOsramConstants.TestStepMTL10NitrideDeposition;
                materialScenario.NumberOfSubMaterials = 0;

                // Create the Material and TrackIn
                materialScenario.Setup(true);
                materialScenario.ResourceMaterialInOut.ChangeResourceState("Standby");

                #endregion

                #region Execution and validation of MaterialIn

                Cmf.Custom.AMSOsram.Orchestration.OutputObjects.MaterialInOutput materialInOutput = new Custom.AMSOsram.Orchestration.InputObjects.MaterialInInput()
                {
                    MaterialName = materialScenario.Entity.Name,
                    ResourceName = materialScenario.ResourceMaterialInOut.Name,
                    SubResourceOrder = 0
                }.MaterialInSync();

                materialScenario.Refresh();
                materialScenario.ResourceMaterialInOut.Load();

                Assert.AreEqual(materialScenario.Entity.SystemState, MaterialSystemState.InProcess);
                Assert.AreEqual(materialScenario.ResourceMaterialInOut.CurrentMainState.CurrentState.Name, "Productive");

                Cmf.Custom.AMSOsram.Orchestration.OutputObjects.MaterialOutOutput materialOutOutput = new Custom.AMSOsram.Orchestration.InputObjects.MaterialOutInput()
                {
                    MaterialName = materialScenario.Entity.Name,
                    ResourceName = materialScenario.ResourceMaterialInOut.Name
                }.MaterialOutSync();

                materialScenario.Refresh();
                materialScenario.ResourceMaterialInOut.Load();

                Assert.AreEqual(materialScenario.Entity.SystemState, MaterialSystemState.Queued);
                Assert.AreEqual(materialScenario.ResourceMaterialInOut.CurrentMainState.CurrentState.Name, "Standby");

                #endregion
            }
            finally
            {
                materialScenario.TearDown();
            }
        }
    }
}
