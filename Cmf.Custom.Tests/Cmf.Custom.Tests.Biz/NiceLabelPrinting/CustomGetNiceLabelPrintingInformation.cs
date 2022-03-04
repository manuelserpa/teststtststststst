using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Cmf.Custom.Tests.Biz.NiceLabelPrinting
{
    [TestClass]
    public class CustomGetNiceLabelPrintingInformation
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
        ///     - Track in and track out lot
        ///     
        /// Acceptance Criteria:
        ///     - Resolve the Smart Table information
        ///     - Valida the file contents
        ///     
        /// </summary>
        /// <TestCaseID>CustomGetNiceLabelPrintingInformation.CustomGetNiceLabelPrintingInformation_TrackOutMaterial_HappyPath</TestCaseID>
        /// <Author>David Guilherme</Author>
        // [TestMethod]
        public void CustomGetNiceLabelPrintingInformation_TrackOutMaterial_HappyPath()
        {
            int subMaterialQuantity = 2;

            #region Setup

            #region Create Material
            ///<Step> Create a lot </Step>
            Material lot = new Material()
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
            lot.Create();
            customTeardownManager.Push(lot);

            // Expand MainMaterial to create 3 sub-materials
            MaterialCollection subMaterials = new MaterialCollection();
            for (var i = 0; i < subMaterialQuantity; i++)
            {
                // Create Sub Material to expand for main lot
                var wafer = new Material()
                {
                    PrimaryQuantity = 1,
                    Product = GenericGetsScenario.GetObjectByName<Product>(AMSOsramConstants.DefaultProductName),
                };
                subMaterials.Add(wafer);
            }

            ExpandMaterialInput input = new ExpandMaterialInput()
            {
                Material = lot,
                SubMaterials = subMaterials,
                Form = AMSOsramConstants.DefaultMaterialLogisticalWaferForm
            };

            subMaterials = input.ExpandMaterialSync().ExpandedSubMaterials;

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
                    { "Material", lot.Name },
                    { "Printer", expectedPrinter },
                    { "Label", expectedLabel },
                    { "Quantity", expectedQuantity.ToString() },
                    { "IsEnabled", "true" }
            });

            #endregion

            #region Container

            Container container = GenericGetsScenario.GetObjectByName<Container>(AMSOsramConstants.DefaultContainerName);
            container.Load();
            int position = 1;

            MaterialContainerCollection materialContainerCollection = new MaterialContainerCollection();
            foreach (Material subMaterial in subMaterials)
            {
                MaterialContainer materialContainerRelation = new MaterialContainer()
                {
                    SourceEntity = subMaterial,
                    TargetEntity = container,
                    Position = position
                };
                position ++;
                materialContainerCollection.Add(materialContainerRelation);
            }

            AssociateMaterialsWithContainerInput associateInput = new AssociateMaterialsWithContainerInput();
            associateInput.MaterialContainerRelations = materialContainerCollection;
            associateInput.Container = container;

            AssociateMaterialsWithContainerOutput associateOutput = associateInput.AssociateMaterialsWithContainerSync();

            #endregion

            Resource printer = GenericGetsScenario.GetObjectByName<Resource>("NiceLabelPrinter");
            printer.Load();
            printer.LoadAttributes(new string[] { "AutomationEquipmentAddress" });

            Resource processResource = GenericGetsScenario.GetObjectByName<Resource>("LOWS0101");
            processResource.Load();

            Area area = processResource.Area;
            area.Load();

            #endregion

            ///<Step> Track In Material </Step>
            lot.Load(1);
            lot.ComplexDispatchAndTrackIn(processResource);
            
            ///<Step> Execute Trackout </Step>
            lot.ComplexTrackOutMaterial();


            ///<Step> Get Generated File for Nice Label Printing </Step> 
            Thread.Sleep(1000);
            string directoryPath = printer.Attributes["AutomationEquipmentAddress"].ToString();
            FileInfo file = new DirectoryInfo(directoryPath).GetFiles().FirstOrDefault(f => f.Name.Contains(lot.Name));
            Assert.IsTrue(file != null, $"Generated file for label printing should container the lot name.");
            
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            string text;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(file.FullName, encoding))
            {
                text = sr.ReadToEnd();
            }

            string[] lines = text.Split(
                new string[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            lot.Product.Load();
            lot.Product.ProductGroup.Load();

            ///<ExpectedValue> The DEE should return the correct information filled on the smart table </ExpectedValue>
            Dictionary<string, string> expectedFileValues = new Dictionary<string, string>()
            {
                { "LABEL_NAME", expectedLabel },
                { "PRINTER_NAME", expectedPrinter },
                { "LABEL_QUANTITY", expectedQuantity },
                { "LotName", lot.Name },
                { "LotAlias", string.Empty },
                { "ProductName", AMSOsramConstants.DefaultProductName },
                { "ProductDesc", lot.Product.Description },
                { "ProductType", lot.Product.ProductType.ToString() },
                { "Product_Type", lot.Product.Type },
                { "ProductGroupName", lot.Product.ProductGroup.Name },
                { "ProductGroup_Type", lot.Product.ProductGroup.Type },
                { "FlowName", AMSOsramConstants.DefaultFlowName },
                { "BatchName", string.Empty },
                { "ContainerName", AMSOsramConstants.DefaultContainerName },
                { "ExperimentName", string.Empty },
                { "ProductionOrder", string.Empty },
                { "LotOwner", string.Empty },
                { "ResourceName", processResource.Name },
                { "LotWaferCount", string.Empty },
                { "LotPrimaryQty", "8" },
                { "LotSecondaryQty", string.Empty },
                { "Lot_Type", lot.Type },
                { "Area", area.Name },
                { "Facility", AMSOsramConstants.DefaultFacilityName }
            };

            foreach (string line in lines)
            {
                if (!line.Equals("END"))
                {
                    string column = line.Split("=")[0];
                    string value = column.Equals("LotPrimaryQty") ? string.Format("{0:0.##}", Convert.ToDecimal(line.Split("=")[1])) : line.Split("=")[1];
                    Assert.IsTrue(value.Equals(expectedFileValues[column]), $"Column {column} should have the value: {expectedFileValues[column]}, but instead has the value: {value}");
                }
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
        /// <TestCaseID>CustomGetNiceLabelPrintingInformation.CustomGetNiceLabelPrintingInformation_EmptySmartTable_HappyPath</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomGetNiceLabelPrintingInformation_EmptySmartTable_HappyPath()
        {

            #region Setup

            #region Create Material
            ///<Step> Create a lot </Step>
            Material lot = new Material()
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
            lot.Create();
            customTeardownManager.Push(lot);
            #endregion

            ///<Step> Ensure Smart Table CustomMaterialNiceLabelPrintContext is empty</Step>
            smartTableManager.ClearSmartTable(AMSOsramConstants.CustomMaterialNiceLabelPrintContextSmartTable);


            Resource printer = GenericGetsScenario.GetObjectByName<Resource>("NiceLabelPrinter");
            printer.Load();
            printer.LoadAttributes(new string[] { "AutomationEquipmentAddress" });

            #endregion

            ///<Step> Track In Material </Step>
            lot.ComplexDispatchAndTrackIn();
            lot.Load(1);

            ///<Step> Execute Trackout </Step>
            lot.ComplexTrackOutMaterial();


            ///<Step> Get Generated File for Nice Label Printing </Step> 
            Thread.Sleep(1000);
            string directoryPath = printer.Attributes["AutomationEquipmentAddress"].ToString();
            FileInfo file = new DirectoryInfo(directoryPath).GetFiles().FirstOrDefault(f => f.Name.Contains(lot.Name));
            Assert.IsTrue(file == null, $"Generated file for label printing should container the lot name.");
           
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
        /// <TestCaseID>CustomGetNiceLabelPrintingInformation.CustomGetNiceLabelPrintingInformation_NotEnabledAtTrackOut_HappyPath</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomGetNiceLabelPrintingInformation_NotEnabledAtTrackOut_HappyPath()
        {
            #region Setup

            #region Create Material
            ///<Step> Create a lot </Step>
            Material lot = new Material()
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
            lot.Create();
            customTeardownManager.Push(lot);
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
                    { "Material", lot.Name },
                    { "Printer", expectedPrinter },
                    { "Label", expectedLabel },
                    { "Quantity", expectedQuantity.ToString() },
                    { "IsEnabled", "false" }
            });

            #endregion

            Resource printer = GenericGetsScenario.GetObjectByName<Resource>("NiceLabelPrinter");
            printer.Load();
            printer.LoadAttributes(new string[] { "AutomationEquipmentAddress" });

            #endregion

            ///<Step> Track In Material </Step>
            lot.ComplexDispatchAndTrackIn();
            lot.Load(1);

            ///<Step> Execute Trackout </Step>
            lot.ComplexTrackOutMaterial();


            ///<Step> Get Generated File for Nice Label Printing </Step> 
            Thread.Sleep(1000);
            string directoryPath = printer.Attributes["AutomationEquipmentAddress"].ToString();
            FileInfo file = new DirectoryInfo(directoryPath).GetFiles().FirstOrDefault(f => f.Name.Contains(lot.Name));
            Assert.IsTrue(file == null, $"Generated file for label printing should container the lot name.");
        }

    }
}
