using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        /// <summary>
        /// Description: This test validates that SubMaterial is TrackedIn in a Resource
        /// Acceptance Criteria: SubMaterial must be in the state InProcess
        /// </summary>
        /// <TestCaseID>MaterialInTest.MaterialInTest_HappyPath</TestCaseID>
        /// <Author>Rui Oliveira</Author>
        [TestMethod]
        public void MaterialInTest_HappyPath()
        {
            CustomMaterialScenario materialScenario = new CustomMaterialScenario(false);

            try
            {
                #region Setup

                //Change material to a step where the resource has subResource of type Process
                materialScenario.FlowName = AMSOsramConstants.TestFlow;
                materialScenario.StepName = AMSOsramConstants.TestStepMTL10NitrideDeposition;

                // Create the Material and TrackIn
                materialScenario.Setup(true);

                materialScenario.TrackIn();

                #endregion

                #region Execution and validation of MaterialIn

                Material materialWafer = materialScenario.SubMaterials[0];

                Cmf.Custom.AMSOsram.Orchestration.OutputObjects.MaterialInOutput materialInOutput = new Custom.AMSOsram.Orchestration.InputObjects.MaterialInInput()
                {
                    MaterialName = materialWafer.Name,
                    ResourceName = materialScenario.ResourceMaterialInOut.Name
                }.MaterialInSync();

                materialWafer.Load();

                Assert.AreEqual(materialInOutput.Material.SystemState, MaterialSystemState.InProcess);

                #endregion
            }
            finally
            {

                materialScenario.TearDown();

            }

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
            CustomMaterialScenario materialScenario = new CustomMaterialScenario(false);

            try
            {
                #region Setup

                //Change material to a step where the resource has subResource of type Process
                materialScenario.FlowName = AMSOsramConstants.TestFlow;
                materialScenario.StepName = AMSOsramConstants.TestStepMTL10NitrideDeposition;

                // Create the Material and TrackIn
                materialScenario.Setup(true);

                materialScenario.TrackIn();

                #endregion

                #region Execution and validation of MaterialIn

                Material materialWafer = materialScenario.SubMaterials[0];

                Cmf.Custom.AMSOsram.Orchestration.OutputObjects.MaterialInOutput materialInOutput = new Custom.AMSOsram.Orchestration.InputObjects.MaterialInInput()
                {
                    MaterialName = materialWafer.Name,
                    ResourceName = materialScenario.ResourceMaterialInOut.Name,
                    SubResourceOrder=1
                }.MaterialInSync();

                materialWafer.Load();

                Assert.AreEqual(materialInOutput.Material.SystemState, MaterialSystemState.InProcess);

                #endregion
            }
            finally
            {

                materialScenario.TearDown();

            }

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
            CustomMaterialScenario materialScenario = new CustomMaterialScenario(false);

            try
            {
                #region Setup

                //Change material to a step where the resource has subResource of type Process
                materialScenario.FlowName = AMSOsramConstants.TestFlow;
                materialScenario.StepName = AMSOsramConstants.TestStepMTL10NitrideDeposition;

                // Create the Material and TrackIn
                materialScenario.Setup(true);

                materialScenario.TrackIn();

                #endregion

                #region Execution and validation of MaterialIn

                Material materialWafer = materialScenario.SubMaterials[0];
                try
                {

                Cmf.Custom.AMSOsram.Orchestration.OutputObjects.MaterialInOutput materialInOutput = new Custom.AMSOsram.Orchestration.InputObjects.MaterialInInput()
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
            finally
            {

                materialScenario.TearDown();

            }

        }

        /// <summary>
        /// Description: This test validates that the Parent Material is TrackedIn in a Resource
        /// </summary>
        /// <TestCaseID>MaterialInTest.MaterialInTest_TopMost</TestCaseID>
        /// <Author>Rui Oliveira</Author>
        [TestMethod]
        public void MaterialInTest_TopMost()
        {
            CustomMaterialScenario materialScenario = new CustomMaterialScenario(false);

            try
            {
                #region Setup

                //Change material to a step where the resource has subResource of type Process
                materialScenario.FlowName = AMSOsramConstants.TestFlow;
                materialScenario.StepName = AMSOsramConstants.TestStepMTL10NitrideDeposition;

                // Create the Material and TrackIn
                materialScenario.Setup(true);

                #endregion

                #region Execution and validation of MaterialIn

                

                Cmf.Custom.AMSOsram.Orchestration.OutputObjects.MaterialInOutput materialInOutput = new Custom.AMSOsram.Orchestration.InputObjects.MaterialInInput()
                {
                    MaterialName = materialScenario.Entity.Name,
                    ResourceName = materialScenario.ResourceMaterialInOut.Name,
                    SubResourceOrder = 0
                }.MaterialInSync();

                materialScenario.Refresh();

                Assert.AreEqual(materialInOutput.Material.SystemState, MaterialSystemState.InProcess);

                #endregion
            }
            finally
            {

                materialScenario.TearDown();

            }

        }

        /// <summary>
        /// Perform material in but checking if main tool E10 state has changed to 'Productive'
        /// </summary>
        [TestMethod]
        public void MaterialInTest_Scenario_05_MaterialInMainToolE10StateChange()
        {
            CustomMaterialScenario materialScenario = new CustomMaterialScenario(false);

            try
            {
                #region Setup

                //Change material to a step where the resource has subResource of type Process
                materialScenario.FlowName = AMSOsramConstants.TestFlow;
                materialScenario.StepName = AMSOsramConstants.TestStepMTL10NitrideDeposition;

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

                Assert.AreEqual(materialInOutput.Material.SystemState, MaterialSystemState.InProcess);
                Assert.AreEqual(materialScenario.ResourceMaterialInOut.CurrentMainState.CurrentState.Name, "Productive");

                #endregion
            }
            finally
            {

                materialScenario.TearDown();
            }
        }
    }
}
