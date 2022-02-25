using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.ContainerManagement.OutputObjects;
using Cmf.Navigo.BusinessOrchestration.FacilityManagement.FlowManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using Cmf.TestScenarios.Others;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.Tests.Biz
{
    [TestClass]
    public class NiveLabelPrinting
    {
        private CustomTearDownManager customTeardownManager = null;
        private SmartTableManager smartTableManager = null;

        [TestInitialize]
        public void TestInitialize()
        {
            customTeardownManager = new CustomTearDownManager();
            smartTableManager = new SmartTableManager();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (customTeardownManager != null)
                customTeardownManager.TearDownSequentially();


            if (smartTableManager != null)
                smartTableManager.TearDown();
        }

        /// <summary>
        /// Description:
        ///     - Fill custom Smart Table CustomMaterialNiceLabelPrintContext with information and isEnabled = true
        ///     - Track in and track out material
        ///     
        /// Acceptance Criteria:
        ///     - Resolve the Smart Table information 
        ///     
        /// </summary>
        /// <TestCaseID>NiveLabelPrinting.NiveLabelPrinting_TrackOutMaterial_HappyPath</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void NiveLabelPrinting_TrackOutMaterial_HappyPath()
        {

            #region Setup

            #region Create Material
            ///<Step> Create a material </Step>
            Material material = new Material()
            {
                Name = Guid.NewGuid().ToString("N"),
                Facility = GenericGetsScenario.GetObjectByName<Facility>(AMSOsramConstants.DefaultFacilityName),
                Product = GenericGetsScenario.GetObjectByName<Product>(AMSOsramConstants.DefaultProductName),
                Type = AMSOsramConstants.DefaultMaterialType,
                FlowPath = new GetCorrelationIdFlowPathInput
                {
                    SequenceFlowPath = AMSOsramConstants.DefaultFlowName + ":1/" + AMSOsramConstants.DefaultMVPStepName + @":30"
                }.GetCorrelationIdFlowPathSync().CorrelationIdFlowPath,
                Form = AMSOsramConstants.DefaultMaterialLogisticalWaferForm,
                PrimaryQuantity = 10,
                PrimaryUnits = AMSOsramConstants.DefaultMaterialUnit
            };
            material.Create();
            customTeardownManager.Push(material);
            #endregion

            #region SmartTable

            ///<Ste> Configure Smart Table CustomMaterialNiceLabelPrintContext </Ste>            
            string expectedPrinter = "TestPrinter";
            string expectedLabel = "TestLabel";
            string expectedQuantity = "5";

            smartTableManager.ClearSmartTable(AMSOsramConstants.CustomMaterialNiceLabelPrintContextSmartTable);
            smartTableManager.SetSmartTableData(AMSOsramConstants.CustomMaterialNiceLabelPrintContextSmartTable,
            new Dictionary<string, string>()
            {
                    { "Step", AMSOsramConstants.DefaultMVPStepName },
                    { "Operation", "TrackOut" },
                    { "Material", material.Name },
                    { "Printer", expectedPrinter },
                    { "Label", expectedLabel },
                    { "Quantity", expectedQuantity.ToString() },
                    { "IsEnabled", "true" }
            });

            #endregion

            #region Container

            Container container = GenericGetsScenario.GetObjectByName<Container>(AMSOsramConstants.DefaultContainerName);
            container.Load();

            MaterialContainer materialContainerRelation = new MaterialContainer()
            {
                SourceEntity = material,
                TargetEntity = container,
                Position = 1
            };
            MaterialContainerCollection materialContainerCollection = new MaterialContainerCollection();
            materialContainerCollection.Add(materialContainerRelation);
            AssociateMaterialsWithContainerInput associateInput = new AssociateMaterialsWithContainerInput();
            associateInput.MaterialContainerRelations = materialContainerCollection;
            associateInput.Container = container;

            AssociateMaterialsWithContainerOutput associateOutput = new AssociateMaterialsWithContainerOutput();
            associateOutput = associateInput.AssociateMaterialsWithContainerSync();
            #endregion

            #endregion

            ///<Step> Track In Material </Step>
            material.ComplexDispatchAndTrackIn();
            material.Load(1);

            ///<Step> Execute DEE with Trackout information</Step>
            #region Execute DEE
            Foundation.Common.DynamicExecutionEngine.Action customNiceLabelPrint = DEEUtilities.GetActionByName(AMSOsramConstants.CustomNiceLabelPrintDEE);

            Dictionary<Material, ComplexTrackOutParameters> trackOutMaterials = new Dictionary<Material, ComplexTrackOutParameters>()
            {
                {material,new ComplexTrackOutParameters() }
            };

            ComplexTrackOutMaterialsInput input = new ComplexTrackOutMaterialsInput()
            {
                Material = trackOutMaterials
            };

            ExecuteActionOutput output = new ExecuteActionInput
            {
                Action = customNiceLabelPrint,
                Input = new Dictionary<string, object>()
                {
                    {"ComplexTrackOutMaterialsInput", input}
                }
            }.ExecuteActionSync(); 
            #endregion

            Dictionary<string, object> outputInformation = (Dictionary<string, object>)output.Output["NiceLabelInformation"];

            ///<ExpectedValue> The DEE should return the correct information filled on the smart table </ExpectedValue>
            foreach (string materialName in outputInformation.Keys)
            {
                Dictionary<string, object> materialInformatoinToPrint = (Dictionary<string, object>)outputInformation[materialName];
                Assert.IsTrue(materialInformatoinToPrint["LotName"].ToString().Equals(materialName), $"Column LotName should have the value: {materialName}, instead is: {materialInformatoinToPrint["LotName"]}.");
                Assert.IsTrue(materialInformatoinToPrint["Printer"].ToString().Equals(expectedPrinter), $"Column Printer should have the value: {expectedPrinter}, instead is: {materialInformatoinToPrint["Printer"]}.");
                Assert.IsTrue(materialInformatoinToPrint["Label"].ToString().Equals(expectedLabel), $"Column Printer should have the value: {expectedLabel}, instead is: {materialInformatoinToPrint["Label"]}.");
                Assert.IsTrue(materialInformatoinToPrint["Quantity"].ToString().Equals(expectedQuantity), $"Column Printer should have the value: {expectedQuantity}, instead is: {materialInformatoinToPrint["Quantity"]}.");
                Assert.IsTrue(materialInformatoinToPrint["ProductName"].ToString().Equals(AMSOsramConstants.DefaultProductName), $"Column Product should have the value: {AMSOsramConstants.DefaultProductName}, instead is: {materialInformatoinToPrint["ProductName"]}.");
                Assert.IsTrue(string.IsNullOrEmpty(materialInformatoinToPrint["ProductGroupName"]?.ToString()), $"Column Product Group should be empty.");
                Assert.IsTrue(materialInformatoinToPrint["ProductDesc"].ToString().Equals(material.Product?.Description), $"Column Product Description should have the value: {material.Product?.Description}, instead is: {materialInformatoinToPrint["ProductDesc"]}.");
                Assert.IsTrue(materialInformatoinToPrint["ProductType"].ToString().Equals(material.Product?.ProductType.ToString()), $"Column Product Product Type should have the value: {material.Product?.ProductType.ToString()}, instead is: {materialInformatoinToPrint["ProductType"]}.");
                Assert.IsTrue(materialInformatoinToPrint["Product_Type"].ToString().Equals(material.Product?.Type), $"Column Product Type should have the value: {material.Product?.Type}, instead is: {materialInformatoinToPrint["Product_Type"]}.");
                Assert.IsTrue(materialInformatoinToPrint["FlowName"].ToString().Equals(material.Flow.Name), $"Column Flow Name should have the value: {material.Flow.Name}, instead is: {materialInformatoinToPrint["FlowName"]}.");
                Assert.IsTrue(materialInformatoinToPrint["ContainerName"].ToString().Equals(AMSOsramConstants.DefaultContainerName), $"Column Container Name should have the value: {AMSOsramConstants.DefaultContainerName}, instead is: {materialInformatoinToPrint["ContainerName"]}.");
                Assert.IsTrue(materialInformatoinToPrint["ResourceName"].ToString().Equals(material.LastProcessedResource.Name), $"Column Resource Name should have the value: {material.LastProcessedResource.Name}, instead is: {materialInformatoinToPrint["ResourceName"]}.");
                Assert.IsTrue(materialInformatoinToPrint["LotPrimaryQty"].ToString().Equals("10"), $"Column Lot Primary Quantity should have the value: 10, instead is: {materialInformatoinToPrint["LotPrimaryQty"]}.");
                Assert.IsTrue(string.IsNullOrEmpty(materialInformatoinToPrint["LotSecundaryQty"]?.ToString()), $"Column Lot Secondary Quantity should have the value: {material.LastProcessedResource.Name}, instead is: {materialInformatoinToPrint["LotSecundaryQty"]}.");
                Assert.IsTrue(materialInformatoinToPrint["Lot_Type"].ToString().Equals(material.Type), $"Column Lot Type should have the value: {material.Type}, instead is: {materialInformatoinToPrint["Lot_Type"]}.");
            }

        }

        /// <summary>
        /// Description:
        ///     - Execute DEE without any information on the smart table 
        ///     
        /// Acceptance Criteria:
        ///     - Return empty result
        ///     - Does not get an error
        ///     
        /// </summary>
        /// <TestCaseID>NiveLabelPrinting.NiveLabelPrinting_EmptySmartTable_HappyPath</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void NiveLabelPrinting_EmptySmartTable_HappyPath()
        {

            #region Setup

            #region Create Material
            ///<Step> Create a material </Step>
            Material material = new Material()
            {
                Name = Guid.NewGuid().ToString("N"),
                Facility = GenericGetsScenario.GetObjectByName<Facility>(AMSOsramConstants.DefaultFacilityName),
                Product = GenericGetsScenario.GetObjectByName<Product>(AMSOsramConstants.DefaultProductName),
                Type = AMSOsramConstants.DefaultMaterialType,
                FlowPath = new GetCorrelationIdFlowPathInput
                {
                    SequenceFlowPath = AMSOsramConstants.DefaultFlowName + ":1/" + AMSOsramConstants.DefaultMVPStepName + @":30"
                }.GetCorrelationIdFlowPathSync().CorrelationIdFlowPath,
                Form = AMSOsramConstants.DefaultMaterialFormName,
                PrimaryQuantity = 10,
                PrimaryUnits = AMSOsramConstants.DefaultMaterialUnit
            };
            material.Create();
            customTeardownManager.Push(material);
            #endregion

            ///<Step> Ensure Smart Table CustomMaterialNiceLabelPrintContext is empty</Step>
            smartTableManager.ClearSmartTable(AMSOsramConstants.CustomMaterialNiceLabelPrintContextSmartTable);
            
            #endregion

            ///<Step> Track In Material </Step>
            material.ComplexDispatchAndTrackIn();
            material.Load(1);

            ///<Step> Execute DEE with Trackout information</Step>
            #region Execute DEE
            Foundation.Common.DynamicExecutionEngine.Action customNiceLabelPrint = DEEUtilities.GetActionByName(AMSOsramConstants.CustomNiceLabelPrintDEE);

            Dictionary<Material, ComplexTrackOutParameters> trackOutMaterials = new Dictionary<Material, ComplexTrackOutParameters>()
            {
                {material,new ComplexTrackOutParameters() }
            };

            ComplexTrackOutMaterialsInput input = new ComplexTrackOutMaterialsInput()
            {
                Material = trackOutMaterials
            };

            ExecuteActionOutput output = new ExecuteActionInput
            {
                Action = customNiceLabelPrint,
                Input = new Dictionary<string, object>()
                {
                    {"ComplexTrackOutMaterialsInput", input}
                }
            }.ExecuteActionSync();
            #endregion

            Dictionary<string, object> outputInformation = (Dictionary<string, object>)output.Output["NiceLabelInformation"];

            ///<ExpectedValue> The DEE should return an empty output </ExpectedValue>
            foreach (string materialName in outputInformation.Keys)
            {
                Dictionary<string, object> materialInformatoinToPrint = (Dictionary<string, object>)outputInformation[materialName];
                Assert.IsTrue(materialInformatoinToPrint.Count == 0, $"Output should be empty.");
            }
           
        }

        /// <summary>
        /// Description:
        ///     - Execute DEE with information on the smart table with column IsEnabled = false
        ///     
        /// Acceptance Criteria:
        ///     - Return empty result
        ///     - Does not get an error
        ///     
        /// </summary>
        /// <TestCaseID>NiveLabelPrinting.NiveLabelPrinting_NotEnabledAtTrackOut_HappyPath</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void NiveLabelPrinting_NotEnabledAtTrackOut_HappyPath()
        {
            #region Setup

            #region Create Material
            ///<Step> Create a material </Step>
            Material material = new Material()
            {
                Name = Guid.NewGuid().ToString("N"),
                Facility = GenericGetsScenario.GetObjectByName<Facility>(AMSOsramConstants.DefaultFacilityName),
                Product = GenericGetsScenario.GetObjectByName<Product>(AMSOsramConstants.DefaultProductName),
                Type = AMSOsramConstants.DefaultMaterialType,
                FlowPath = new GetCorrelationIdFlowPathInput
                {
                    SequenceFlowPath = AMSOsramConstants.DefaultFlowName + ":1/" + AMSOsramConstants.DefaultMVPStepName + @":30"
                }.GetCorrelationIdFlowPathSync().CorrelationIdFlowPath,
                Form = AMSOsramConstants.DefaultMaterialFormName,
                PrimaryQuantity = 10,
                PrimaryUnits = AMSOsramConstants.DefaultMaterialUnit
            };
            material.Create();
            customTeardownManager.Push(material);
            #endregion

            #region SmartTable

            ///<Ste> Configure Smart Table CustomMaterialNiceLabelPrintContext </Ste>            
            string expectedPrinter = "TestPrinter";
            string expectedLabel = "TestLabel";
            string expectedQuantity = "5";

            smartTableManager.ClearSmartTable(AMSOsramConstants.CustomMaterialNiceLabelPrintContextSmartTable);
            smartTableManager.SetSmartTableData(AMSOsramConstants.CustomMaterialNiceLabelPrintContextSmartTable,
            new Dictionary<string, string>()
            {
                    { "Step", AMSOsramConstants.DefaultMVPStepName },
                    { "Operation", "TrackOut" },
                    { "Material", material.Name },
                    { "Printer", expectedPrinter },
                    { "Label", expectedLabel },
                    { "Quantity", expectedQuantity.ToString() },
                    { "IsEnabled", "false" }
            });

            #endregion

            #endregion

            ///<Step> Track In Material </Step>
            material.ComplexDispatchAndTrackIn();
            material.Load(1);

            ///<Step> Execute DEE with Trackout information</Step>
            #region Execute DEE
            Foundation.Common.DynamicExecutionEngine.Action customNiceLabelPrint = DEEUtilities.GetActionByName(AMSOsramConstants.CustomNiceLabelPrintDEE);

            Dictionary<Material, ComplexTrackOutParameters> trackOutMaterials = new Dictionary<Material, ComplexTrackOutParameters>()
            {
                {material,new ComplexTrackOutParameters() }
            };

            ComplexTrackOutMaterialsInput input = new ComplexTrackOutMaterialsInput()
            {
                Material = trackOutMaterials
            };

            ExecuteActionOutput output = new ExecuteActionInput
            {
                Action = customNiceLabelPrint,
                Input = new Dictionary<string, object>()
                {
                    {"ComplexTrackOutMaterialsInput", input}
                }
            }.ExecuteActionSync();
            #endregion

            Dictionary<string, object> outputInformation = (Dictionary<string, object>)output.Output["NiceLabelInformation"];

            ///<ExpectedValue> The DEE should not return the correct information filled on the smart table </ExpectedValue>
            foreach (string materialName in outputInformation.Keys)
            {
                Dictionary<string, object> materialInformatoinToPrint = (Dictionary<string, object>)outputInformation[materialName];
                Assert.IsTrue(materialInformatoinToPrint.Count == 0, $"Output should be empty.");
            }
        }

    }
}
