//<FileInfo>
//  <copyright file="SendTrackInInformationToIoT.cs" company="Critical Manufacturing, SA">
//        <![CDATA[Copyright © Critical Manufacturing SA. All rights reserved.]]>
//  </copyright>
//  <Author>Davi Figueiredo</Author>
//</FileInfo>

using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cmf.Custom.Tests.Biz.Materials
{
    /// <summary>
    /// Summary description for SendTrackInInformationToIoT
    /// </summary>
    [TestClass]
    public class SendTrackInInformationToIoT
    {
        /// <summary>
        /// SendTrackInInformationToIoT_HappyPath
        /// </summary>
        /// <TestCaseID>SendTrackInInformationToIoT.SendTrackInInformationToIoT_HappyPath</TestCaseID>
        /// <Author>Davi Figueiredo</Author>
        [TestMethod]
        public void SendTrackInInformationToIoT_HappyPath()
        {
            CustomMaterialScenario materialScenario = new CustomMaterialScenario(false);
            try
            {
                materialScenario.Setup();

                Flow flow = new Flow() { Id = materialScenario.Entity.Flow.Id };
                flow.Load();

                Step step = new Step()
                {
                    Name = AMSOsramConstants.TestM3MTZnOSputterCluster6in00126F008_E
                };

                step.Load();

                // Change Flow and Step to a step that allows SubMaterial TrackIn
                string flowPath = flow.GetFlowPath(step.Name);

                materialScenario.ChangeFlowAndStep(flow, step, flowPath);

                // It is necessary to set automation mode now because the  flow/step changed the so the material will be possible track-in in another resource
                materialScenario.SetResourceAutomationModeOnline();

                materialScenario.DispatchAndTrackIn();

                // Validate that the material (Lot) is tracked in
                Assert.AreEqual(MaterialSystemState.InProcess, materialScenario.Entity.SystemState, "Material is not in the correct state.");

                // Validate that the material (Lot) has 25 submaterials
                Assert.AreEqual(25, materialScenario.Entity.SubMaterialCount, "Number of Sub Materials does not match.");

                // Validate that all submaterials has primary quantities 1 and are inside the same container
                if (materialScenario.Entity.SubMaterials == null)
                {
                    materialScenario.Entity.LoadChildren();
                }

                Assert.IsTrue(materialScenario.Entity.SubMaterials.TrueForAll(m => m.PrimaryQuantity == 1), "Number of Sub Materials does not match.");

                materialScenario.Refresh();

                // Validate the state model
                Assert.IsNotNull(materialScenario.Entity.CurrentMainState, "MainStateModel was not set!");

                Assert.AreEqual(AMSOsramConstants.MaterialStateModel, materialScenario.Entity.CurrentMainState.StateModel.Name, "Incorrect state model!");
            }
            finally
            {
                materialScenario.TearDown();
            }
        }

    }
}

