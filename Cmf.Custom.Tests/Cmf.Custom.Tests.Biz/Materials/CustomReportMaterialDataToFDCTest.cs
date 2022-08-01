using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Cmf.Custom.Tests.Biz.Materials
{
    [TestClass]
    public class CustomReportMaterialDataToFDCTest
    {
        private CustomMaterialScenario materialScenario = null;

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            //Creation Custom Material Scenario
            materialScenario = new CustomMaterialScenario(false)
            {
                NumberOfSubMaterials = 1,
                AssociateSubMaterialsToContainer = false
            };
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            // materialScenario teardown
            if (materialScenario != null)
            {
                materialScenario.TearDown();
            }

            // Either clean integration entries here with on bulk or delete after every test - performance?
        }

        /// <summary>
        /// Description:
        ///     -Create a lot material in order to trigger a track in operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCLotStart
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_LotMaterialTrackIn</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_LotMaterialTrackIn()
        {
            
            Material mat = materialScenario.Entity;
            mat.ComplexTrackIn();

            IntegrationEntry integrationEntry = new IntegrationEntry();
            integrationEntry = CustomUtilities.GetIntegrationEntry(mat.Name);
            integrationEntry.Load();

            Assert.IsTrue(integrationEntry.Name.Contains(mat.Name), "Integration entry was not created.");
            Assert.AreEqual(integrationEntry.MessageType, AMSOsramConstants.MessageType_LOTIN, "Integration entry contains the wrong message type.");

        }

        /// <summary>
        /// Description:
        ///     -Create a wafer material in order to trigger a track in operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCWaferIn
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_WaferMaterialTrackIn</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_WaferMaterialTrackIn()
        {
            /*
             * Problems with wafers: materialScenario wafers are created as logical wafers which are not dispatchible
             * Need to create a wafer and associate it to a logical wafer which is associated to a parent lot?
             * How to make sure that the wafer (not the logical one) and the parent (parent) lot are at the same step (and both are dispatchible)
             * Automated way to track in parent lot so all wafers get tracked in too?
             * 
             */
            // Create a wafer material with a parent lot that is dispatchible and can be tracked in

            // Trigger material track in for wafer material
            materialScenario.Setup(true);
            Assert.IsTrue(materialScenario.Entity.SubMaterialCount > 0, $"The material {materialScenario.Entity.Name} should have submaterials.");
            IntegrationEntry integrationEntry = new IntegrationEntry();

            Resource resource = new Resource
            {
                Name = "PDSP0101"
            };
            resource.Load();

            Resource subResource = new Resource
            {
                Name = "PDSP0101.PM01"
            };
            subResource.Load();

            materialScenario.Entity.TrackIn(resource);
            materialScenario.SubMaterials[0].Load();
            materialScenario.SubMaterials[0].TrackIn(subResource);

            // Verify if Integration Entry for wafer material was created and contains the right messageType

            integrationEntry = CustomUtilities.GetIntegrationEntry(materialScenario.SubMaterials[0].Name);
            integrationEntry.Load();
            Assert.IsTrue(integrationEntry.Name.Contains(materialScenario.SubMaterials[0].Name), "Integration entry was not created.");
            Assert.AreEqual(integrationEntry.MessageType, AMSOsramConstants.MessageType_LOTIN, "Integration entry contains the wrong message type.");

            /*
            materialScenario.Setup(true);
            MaterialCollection materials = new MaterialCollection();
            materials.Add(materialScenario.Entity);
            UndispatchMaterialsOutput undispatchRes = new UndispatchMaterialsInput()
            {
                Materials = materials,
                ServiceComments = null
            }.UndispatchMaterialsSync();

            AttachWafersToLot(materialScenario.Entity.Name);
            materialScenario.Entity.ComplexDispatchAndTrackIn();
            materialScenario.Entity.LoadChildren();
            materialScenario.Entity.SubMaterials[0].ComplexDispatchAndTrackIn();
            */





        }

        /// <summary>
        /// Description:
        ///     -Create a lot material in order to trigger a track in and track out operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCLotEnd
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_LotMaterialTrackOut</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_LotMaterialTrackOut()
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
        ///     -Create a wafer material in order to trigger a track in and track out operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCWaferOut
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_WaferMaterialTrackOut</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_WaferMaterialTrackOut()
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
        ///     -Create a lot material in order to trigger a track in and abort operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCLotEnd
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_LotMaterialAbort</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_LotMaterialAbort()
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

        /// <summary>
        /// Description:
        ///     -Create a wafer material in order to trigger a track in and abort operation
        ///     -Verify if the integration entry was created successfully
        ///     -Verify if the integration entry has the correct message type
        ///     -Onto FDC: SendFDCWaferOut
        /// </summary>
        /// <TestCaseID>CustomReportDataToFDCTests_WaferMaterialAbort</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomReportDataToFDCTests_WaferMaterialAbort()
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

        /// <summary>
        /// Attach wafers to Lot
        /// </summary>
        private void AttachWafersToLot(string materialName)
        {
            Material mat = new Material()
            {
                Name = materialName
            };
            mat.Load();
            MaterialCollection wafersScenario = new MaterialCollection();
            int numberOfWafers = 1;
            Facility facility = new Facility()
            {
                Name = "Regensburg Production"
            };
            facility.Load();
            Flow flow = new Flow()
            {
                Name = "FOL-UX3_EPA"
            };
            flow.Load();
            Step step = new Step()
            {
                Name = "M3-MT-ZnO-SputterCluster-6in-00126F008_E"
            };
            step.Load();

            #region Sub-Materials Setup

            for (int i = 0; i < numberOfWafers; i++)
            {
                Material material = new Material();

                material.Name = "MESTest_Material_" + DateTime.Now.ToString("yyyyMMdd_HHmmssffffff");
                material.Facility = facility;
                material.Flow = flow;
                material.Step = step;
                material.FlowPath = FlowExtensionMethods.CustomGetFlowPath(flow, step.Name);
                material.Product = materialScenario.Entity.Product;
                material.Form = "Wafer";
                material.Type = materialScenario.Entity.Type;
                material.PrimaryUnits = "Wafers";
                material.PrimaryQuantity = 1;

                material.Create();
                wafersScenario.Add(material);
            }

            #endregion Sub-Materials Setup

            #region Attach sub-materials to the Lot

            materialScenario.Entity.Load();
            //Attach the wafers to the material (Lot)
            new AttachMaterialsInput()
            {
                Material = materialScenario.Entity,
                SubMaterials = wafersScenario
            }.AttachMaterialsSync();

            // Validate if the wafers were attached
            materialScenario.Entity.Load();
            materialScenario.Entity.LoadChildren();

            Assert.AreEqual(materialScenario.Entity.SubMaterialCount, wafersScenario.Count, "The wafers should have been attached to the Material!");

            #endregion Attach sub-materials to the Lot

        }
    }
}