using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cmf.Custom.Tests.Biz.Materials
{
    [TestClass]
    public class CustomReportMaterialDataToFDC
    {
        private CustomMaterialScenario materialScenario = new CustomMaterialScenario(false);

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            materialScenario.NumberOfSubMaterials = 0;
            materialScenario.AssociateSubMaterialsToContainer = false;
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            // Either clean integration entries here with on bulk or delete after every test - performance?
        }

        /// <summary>
        /// Description:
        ///     -Create a material scenario in order to trigger a track in operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_MaterialTrackIn</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_MaterialTrackIn()
        {
            materialScenario.Setup(true);
            Material mat = new Material();
            mat = materialScenario.Entity;
            mat.ComplexTrackIn();

            IntegrationEntry integrationEntry = new IntegrationEntry();
            integrationEntry = CustomUtilities.GetIntegrationEntry(mat.Name);
            integrationEntry.Load();

            Assert.IsTrue(integrationEntry.Name.Contains(mat.Name), "Integration entry was not created.");
            Assert.AreEqual(integrationEntry.MessageType, AMSOsramConstants.MessageType_LOTIN, "Integration entry contains the wrong message type.");

        }

        /// <summary>
        /// Description:
        ///     -Create a material scenario in order to trigger a track in and track out operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_MaterialTrackOut</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_MaterialTrackOut()
        {

            materialScenario.Setup(true);
            Material mat = new Material();
            mat = materialScenario.Entity;

            // ERROR: Running these manually works, but during automatic run test will not have enough time and therefore the wrong IntegrationEntry (TrackIn) is fetched

            mat.ComplexTrackIn();
            mat.Load();
            mat.ComplexTrackOutMaterial();


            IntegrationEntry integrationEntry = new IntegrationEntry();
            integrationEntry = CustomUtilities.GetIntegrationEntry(mat.Name);


            Assert.IsTrue(integrationEntry.Name.Contains(mat.Name), "Integration entry was not created.");
            Assert.AreEqual(integrationEntry.MessageType, AMSOsramConstants.MessageType_LOTOUT, "Integration entry contains the wrong message type.");
        }


        /// <summary>
        /// Description:
        ///     -Create a material scenario in order to trigger a track in and abort operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_MaterialAbort</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_MaterialAbort()
        {
            materialScenario.Setup(true);
            Material mat = new Material();
            mat = materialScenario.Entity;

            // ERROR: Running these manually works, but during automatic run test will not have enough time and therefore the wrong IntegrationEntry (TrackIn) is fetched

            mat.ComplexTrackIn();
            mat.Load();
            mat.Abort();

            IntegrationEntry integrationEntry = new IntegrationEntry();
            integrationEntry = CustomUtilities.GetIntegrationEntry(mat.Name);


            Assert.IsTrue(integrationEntry.Name.Contains(mat.Name), "Integration entry was not created.");
            Assert.AreEqual(integrationEntry.MessageType, AMSOsramConstants.MessageType_LOTOUT, "Integration entry contains the wrong message type.");
        }

    }
}
