using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.OutputObjects;
using DeeAction = Cmf.Foundation.Common.DynamicExecutionEngine.Action;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Custom.Tests.Biz.Common.ERP.Space;
using System.Collections.ObjectModel;

namespace Cmf.Custom.Tests.Biz.Space
{
    [TestClass]
    public class CustomSendCriticalDataCollectionToSpace
    {
        private CustomExecutionScenario _scenario;
        private CustomTearDownManager customTeardownManager = null;
        private MaterialCollection materials;
        private DataCollectionInstance dataCollectionInstance = null;

        private const string StorageLocation = "TestStorageLocation";
        private const string Site = "TestSiteCode";
        private const string DataCollectionName = "SpaceDCTest";
        private const string DataCollectionLimitSetName = "SpaceDCTestLimitSet";
        private List<string> parametersName = new List<string>() { "SScOTest", "QScOTest" };
        

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            _scenario = new CustomExecutionScenario();
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.NumberOfChildMaterialsToGenerate = 3;

            materials = new MaterialCollection();

            customTeardownManager = new CustomTearDownManager();
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            foreach (Material material in materials)
            {
                if (material.HoldCount > 0)
                {
                    material.LoadRelation("MaterialHoldReason");

                    EntityRelationCollection materialHoldReasons = material.RelationCollection["MaterialHoldReason"];

                    foreach (MaterialHoldReason materialHoldReason in materialHoldReasons)
                    {
                        material.ReleaseByReason(materialHoldReason);
                    }
                }
            }

            if (dataCollectionInstance != null) dataCollectionInstance.Terminate();

            if (customTeardownManager != null) customTeardownManager.TearDownSequentially();

            if (_scenario != null) _scenario.CompleteCleanUp();
        }

        /// <summary>
        /// Description:
        ///     - Execute a ComplexPerformDataCollection operation with parameter type equals to material Id
        ///     - Post with values outside the limits 
        ///
        /// Acceptance Citeria:
        ///     - Lot is on hold with reason Out of Spec
        ///     - Validate created message structure
        ///     
        /// </summary>
        /// <TestCaseID>CustomSendCriticalDataCollectionToSpace.CustomSendCriticalDataCollectionToSpace_PostEDCDataOutsideLimits_ValidateAndCreateXMLMessage</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomSendCriticalDataCollectionToSpace_PostEDCDataOutsideLimits_ValidateAndCreateXMLMessage()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.Setup();

            decimal defaultValue = 634;

            Material material = _scenario.GeneratedLots.FirstOrDefault();
            materials.Add(material);
            material.LoadChildren();

            Dictionary<string, string> expectedKeys = new Dictionary<string, string>() {
                {"PROCESS",  AMSOsramConstants.DefaultTestProductName},
                {"BASIC_TYPE",string.Empty },
                {"AREA",string.Empty },
                {"OWNER", string.Empty},
                {"ROUTE",$"{AMSOsramConstants.DefaultTestFlowName}_1"},
                {"OPERATION",AMSOsramConstants.DefaultTestStepName},
                {"PROCESS_SPS","CMFTestService"},
                {"EQUIPMENT", string.Empty},
                {"CHAMBER",  string.Empty},
                {"RECIPE", string.Empty},
                {"MEAS_EQUIPMENT", string.Empty },
                {"BATCH_NAME", string.Empty },
                {"LOT",$"{material.Name}"},
                {"QTY","3.00000000"},
                {"WAFER","."},
                {"PUNKT","."},
                {"X","."},
                {"Y","."}
            };

            ///<Step> Open Data Collection Instance </Step>
            DataCollection dataCollection = new DataCollection(){ Name = DataCollectionName };
            dataCollection.Load();

            DataCollectionLimitSet datacollectionLimitSet = new DataCollectionLimitSet
            {
                Name = DataCollectionLimitSetName
            };
            datacollectionLimitSet.Load();

            DataCollectionPointCollection pointsToPost = new DataCollectionPointCollection();

            int count = 0;
            foreach (string parameterName in parametersName)
            {
                Parameter parameter = new Parameter() { Name = parameterName };
                parameter.Load();

                foreach (Material wafer in material.SubMaterials)
                {
                    DataCollectionPoint collectionPoint = new DataCollectionPoint();
                    collectionPoint.Value = defaultValue + count;
                    collectionPoint.TargetEntity = parameter;
                    collectionPoint.ReadingNumber = 1;
                    collectionPoint.SampleId = wafer.Name;
                    pointsToPost.Add(collectionPoint);
                    count++;
                }
            }

            ComplexPerformDataCollectionInput complexPostDCDataInput = new ComplexPerformDataCollectionInput()
            {
                Material = material,
                DataCollection = dataCollection,
                DataCollectionLimitSet = datacollectionLimitSet,
                DataCollectionPointCollection = pointsToPost
            };

            ComplexPerformDataCollectionOutput complexPostDCDataOutput = complexPostDCDataInput.ComplexPerformDataCollectionSync();

            ///<Step> Validate Material Hold Reasons.</Step>
            ///<ExpectedValue> The Material has one Hold Reasons (Out of Spec).</ExpectedValue>
            material.Load();
            material.LoadRelations(new Collection<string> { "MaterialHoldReason" });
            Assert.IsTrue(material.HoldCount == 1, $"Material should have 1 reason instead has {material.HoldCount}");
            MaterialHoldReason outOfSpecHoldReason = material.MaterialHoldReasons.FirstOrDefault();
            outOfSpecHoldReason.TargetEntity.Load();
            Assert.IsTrue(outOfSpecHoldReason.TargetEntity.Name.Equals("Out Of Spec"), $"Material Hold Reason Name should be: Out of Spec instead is: {outOfSpecHoldReason.TargetEntity.Name}");

            Assert.IsTrue(material.OpenExceptionProtocolsCount == 0, $"Material shouldn't have protocol instance opened, instead has {material.OpenExceptionProtocolsCount}.");

            Dictionary<string, object> deeActionInput = new Dictionary<string, object>()
            {
                {"DataCollectionInstance", complexPostDCDataOutput.DataCollectionInstances.FirstOrDefault().Name }, 
                {"LimitSet", datacollectionLimitSet.Name}, 
                {"Material", material.Name}
            };

            ExecuteAction("CustomReportEDCToSpaceParser", deeActionInput);

            IntegrationEntry ie = CustomUtilities.GetIntegrationEntry($"SpaceEDC_{ material.Name}");
            ie.Load();

            //Necessary to load inner message
            IntegrationEntry integrationEntryInfo = new GetIntegrationEntryInput
            {
                Id = ie.Id
            }.GetIntegrationEntrySync().IntegrationEntry;
            string integrationMessage = Encoding.UTF8.GetString(integrationEntryInfo.IntegrationMessage.Message);

            ///<Step> Validate the content of the Integration Entry </Step>
            CustomReportEDCToSpace spaceInformation = CustomUtilities.DeserializeXmlToObject<CustomReportEDCToSpace>(integrationMessage);

            foreach (Key key in spaceInformation.Keys)
            {
                if (!string.IsNullOrEmpty(expectedKeys[key.Name]))
                {
                    Assert.IsTrue(key.Value.Equals(expectedKeys[key.Name]), $"The value for key with name {key.Name} is {key.Value}, but should be {expectedKeys[key.Name]}.");
                }
            }

            Assert.IsTrue(spaceInformation.Samples.Count == 1,$"The number of samples present on the message is {spaceInformation.Samples.Count}, but should be 1.");
            Assert.IsTrue(spaceInformation.Samples[0].Raws.raws.Count == material.SubMaterialCount, $"Each wafer should be present on the sample data." );

            int sampleCount = 0;
            foreach (Sample sample in spaceInformation.Samples)
            {
                if(sample.ParameterName.Equals("SScOTest"))
                {
                    Assert.IsTrue(sample.Lower.Equals("635.0000000000"), $"Sample field for parameter {sample.ParameterName} should have the lower limit error equal to 635.0000000000 instead is {sample.Lower}.");
                    Assert.IsTrue( sample.Upper.Equals("665.0000000000"), $"Sample field for parameter {sample.ParameterName} should have the upper limit error equal to 665.0000000000 instead is { sample.Upper}.");
                }

                foreach (Raw sampleRaw in sample.Raws.raws)
                {
                    decimal value = defaultValue + sampleCount;
                    Assert.IsTrue(sampleRaw.RawValue.Equals(value), $"Value for parameter {sample.ParameterName} should be {value} instead is {sampleRaw.RawValue}.");
                    sampleCount++;
                }
            }
        }

        /// <summary>
        /// Description:
        ///     - Execute a ComplexPerformDataCollection operation with parameter type equals to material Id
        ///     - Post with values inside the limits 
        ///
        /// Acceptance Citeria:
        ///     - Lot is not on hold with reason Out of Spec
        ///     - Lot has a protocol instance
        ///     - Validate created message structure
        ///     
        /// </summary>
        /// <TestCaseID>CustomSendCriticalDataCollectionToSpace.CustomSendCriticalDataCollectionToSpace_PostEDCDataInsideLimits_ValidateAndCreateXMLMessage</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomSendCriticalDataCollectionToSpace_PostEDCDataInsideLimits_ValidateAndCreateXMLMessage()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.Setup();

            decimal defaultValue = 640;

            Material material = _scenario.GeneratedLots.FirstOrDefault();
            materials.Add(material);
            material.LoadChildren();

            Dictionary<string, string> expectedKeys = new Dictionary<string, string>() {
                {"PROCESS",  AMSOsramConstants.DefaultTestProductName},
                {"BASIC_TYPE",string.Empty },
                {"AREA",string.Empty },
                {"OWNER", string.Empty},
                {"ROUTE",$"{AMSOsramConstants.DefaultTestFlowName}_1"},
                {"OPERATION",AMSOsramConstants.DefaultTestStepName},
                {"PROCESS_SPS","CMFTestService"},
                {"EQUIPMENT", string.Empty},
                {"CHAMBER",  string.Empty},
                {"RECIPE", string.Empty},
                {"MEAS_EQUIPMENT", string.Empty },
                {"BATCH_NAME", string.Empty },
                {"LOT",$"{material.Name}"},
                {"QTY","3.00000000"},
                {"WAFER","."},
                {"PUNKT","."},
                {"X","."},
                {"Y","."}
            };

            ///<Step> Open Data Collection Instance </Step>
            DataCollection dataCollection = new DataCollection() { Name = DataCollectionName };
            dataCollection.Load();

            DataCollectionLimitSet datacollectionLimitSet = new DataCollectionLimitSet
            {
                Name = DataCollectionLimitSetName
            };
            datacollectionLimitSet.Load();

            DataCollectionPointCollection pointsToPost = new DataCollectionPointCollection();

            int count = 0;
            foreach (string parameterName in parametersName)
            {
                Parameter parameter = new Parameter() { Name = parameterName };
                parameter.Load();

                foreach (Material wafer in material.SubMaterials)
                {
                    DataCollectionPoint collectionPoint = new DataCollectionPoint();
                    collectionPoint.Value = defaultValue + count;
                    collectionPoint.TargetEntity = parameter;
                    collectionPoint.ReadingNumber = 1;
                    collectionPoint.SampleId = wafer.Name;
                    pointsToPost.Add(collectionPoint);
                    count++;
                }
            }

            ComplexPerformDataCollectionInput complexPostDCDataInput = new ComplexPerformDataCollectionInput()
            {
                Material = material,
                DataCollection = dataCollection,
                DataCollectionLimitSet = datacollectionLimitSet,
                DataCollectionPointCollection = pointsToPost
            };

            ComplexPerformDataCollectionOutput complexPostDCDataOutput = complexPostDCDataInput.ComplexPerformDataCollectionSync();

            ///<Step> Validate Protocol Opened.</Step>
            ///<ExpectedValue> The lot should have a protocol opened.</ExpectedValue>
            material.Load();
            material.LoadRelations(new Collection<string> { "MaterialHoldReason", "ProtocolMaterial" });
            Assert.IsTrue(material.OpenExceptionProtocolsCount == 1, $"Material should have one protocol instance opened, instead has {material.OpenExceptionProtocolsCount}.");

            ProtocolInstance protocolInstance = material.MaterialProtocols.FirstOrDefault().SourceEntity;
            protocolInstance.Load();
            protocolInstance.ParentEntity.Load();
            Assert.IsTrue(protocolInstance.ParentEntity.Name.Equals("8D"), $"Protocol should be 8D instead is {protocolInstance.ParentEntity.Name}");

            Assert.IsTrue(material.HoldCount == 0, $"Material should have 1 reason instead has {material.HoldCount}");

            Dictionary<string, object> deeActionInput = new Dictionary<string, object>()
            {
                {"DataCollectionInstance", complexPostDCDataOutput.DataCollectionInstances.FirstOrDefault().Name },
                {"LimitSet", datacollectionLimitSet.Name},
                {"Material", material.Name}
            };

            ExecuteAction("CustomReportEDCToSpaceParser", deeActionInput);

            IntegrationEntry ie = CustomUtilities.GetIntegrationEntry($"SpaceEDC_{ material.Name}");
            ie.Load();

            //Necessary to load inner message
            IntegrationEntry integrationEntryInfo = new GetIntegrationEntryInput
            {
                Id = ie.Id
            }.GetIntegrationEntrySync().IntegrationEntry;
            string integrationMessage = Encoding.UTF8.GetString(integrationEntryInfo.IntegrationMessage.Message);

            ///<Step> Validate the content of the Integration Entry </Step>
            CustomReportEDCToSpace spaceInformation = CustomUtilities.DeserializeXmlToObject<CustomReportEDCToSpace>(integrationMessage);

            foreach (Key key in spaceInformation.Keys)
            {
                if ( !string.IsNullOrEmpty(expectedKeys[key.Name]))
                {
                    Assert.IsTrue(key.Value.Equals(expectedKeys[key.Name]), $"The value for key with name {key.Name} is {key.Value}, but should be {expectedKeys[key.Name]}."); 
                }
            }

            Assert.IsTrue(spaceInformation.Samples.Count == 1, $"The number of samples present on the message is {spaceInformation.Samples.Count}, but should be 1.");
            Assert.IsTrue(spaceInformation.Samples[0].Raws.raws.Count == material.SubMaterialCount, $"Each wafer should be present on the sample data.");

            int sampleCount = 0;
            foreach (Sample sample in spaceInformation.Samples)
            {
                if (sample.ParameterName.Equals("SScOTest"))
                {
                    Assert.IsTrue(sample.Lower.Equals("635.0000000000"), $"Sample field for parameter {sample.ParameterName} should have the lower limit error equal to 635.0000000000 instead is {sample.Lower}.");
                    Assert.IsTrue(sample.Upper.Equals("665.0000000000"), $"Sample field for parameter {sample.ParameterName} should have the upper limit error equal to 665.0000000000 instead is { sample.Upper}.");
                }

                foreach (Raw sampleRaw in sample.Raws.raws)
                {
                    decimal value = defaultValue + sampleCount;
                    Assert.IsTrue(sampleRaw.RawValue.Equals(value), $"Value for parameter {sample.ParameterName} should be {value} instead is {sampleRaw.RawValue}.");
                    sampleCount++;
                }
            }
        }

        private void ExecuteAction(string actionName, Dictionary<string, object> deeActionInput)
        {
            // Call CustomTriggerTransportJob DEE
            Foundation.Common.DynamicExecutionEngine.Action deeAction = new GetActionByNameInput()
            {
                Name = actionName
            }.GetActionByNameSync().Action;

            Dictionary<string, object> outputDee = new ExecuteActionInput()
            {
                Action = deeAction,
                Input = deeActionInput
            }.ExecuteActionSync().Output;
        }
    }
}
