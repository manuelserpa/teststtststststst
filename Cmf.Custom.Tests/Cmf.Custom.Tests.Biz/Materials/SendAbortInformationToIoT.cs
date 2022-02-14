//<FileInfo>
//  <copyright file="SendAbortInformationToIoT.cs" company="Critical Manufacturing, SA">
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
    /// Summary description for SendAbortInformationToIoT
    /// </summary>
    [TestClass]
    public class SendAbortInformationToIoT
    {
        /// <summary>
        /// SendAbortInformationToIoT_HappyPath
        /// </summary>
        /// <TestCaseID>SendAbortInformationToIoT.SendAbortInformationToIoT_HappyPath</TestCaseID>
        /// <Author>Davi Figueiredo</Author>
        [TestMethod]
        public void SendAbortInformationToIoT_HappyPath()
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

                // Abort the material
                materialScenario.AbortProcess();

                // Validate that the material (Lot) is Aborted
                Assert.AreEqual(LastProcessState.Aborted, materialScenario.Entity.LastProcessState, "Material LastProcessState is not in the correct state.");

                // Validate that the material (Lot) is Queued
                Assert.AreEqual(MaterialSystemState.Queued, materialScenario.Entity.SystemState, "Material is not in the correct state.");
            }
            finally
            {
                materialScenario.TearDown();
            }
        }
    }
}
