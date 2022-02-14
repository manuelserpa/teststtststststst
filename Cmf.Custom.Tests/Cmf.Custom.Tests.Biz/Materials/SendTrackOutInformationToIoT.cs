//<FileInfo>
//  <copyright file="SendTrackOutInformationToIoT.cs" company="Critical Manufacturing, SA">
//        <![CDATA[Copyright © Critical Manufacturing SA. All rights reserved.]]>
//  </copyright>
//  <Author>Davi Figueiredo</Author>
//</FileInfo>

using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cmf.Custom.Tests.Biz.Materials
{
    /// <summary>
    /// Summary description for SendTrackOutInformationToIoT
    /// </summary>
    [TestClass]
    public class SendTrackOutInformationToIoT
    {
        /// <summary>
        /// SendTrackOutInformationToIoT_HappyPath
        /// </summary>
        /// <TestCaseID>SendTrackOutInformationToIoT.SendTrackOutInformationToIoT_HappyPath</TestCaseID>
        /// <Author>Davi Figueiredo</Author>
        [TestMethod]
        public void SendTrackOutInformationToIoT_HappyPath()
        {
            CustomMaterialScenario materialScenario = new CustomMaterialScenario(false);
            try
            {
                // Create the material and track in
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

                // Track out the material
                materialScenario.TrackOut();

                // Validate that the material (Lot) is tracked out
                Assert.AreEqual(MaterialSystemState.Processed, materialScenario.Entity.SystemState, "Material is not in the correct state.");
            }
            finally
            {
                materialScenario.TearDown();
            }
        }
    }
}
