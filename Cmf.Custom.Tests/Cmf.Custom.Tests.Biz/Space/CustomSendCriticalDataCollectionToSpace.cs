using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.ERP.Space;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.MessageBus.Client;
using Cmf.MessageBus.Messages;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.OutputObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Cmf.Custom.Tests.Biz.Space
{
    [TestClass]
    public class CustomSendCriticalDataCollectionToSpace
    {
        private const string DataCollectionName = "SpaceDCTest";
        private const string DataCollectionLimitSetName = "SpaceDCTestLimitSet";
        private List<string> parametersName = new List<string>() { "SScOTest", "QScOTest" };

        private CustomExecutionScenario _scenario;
        private CustomTearDownManager customTeardownManager = null;
        private MaterialCollection materials;
        private DataCollectionInstance dataCollectionInstance = null;
        private Material material = null;
        private decimal defaultValue = 0;

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

            if (dataCollectionInstance != null && dataCollectionInstance.UniversalState != Foundation.Common.Base.UniversalState.Terminated)
            {
                dataCollectionInstance.Terminate();
            }

            if (customTeardownManager != null)
            {
                customTeardownManager.TearDownSequentially();
            }

            if (_scenario != null)
            {
                _scenario.CompleteCleanUp();
            }
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

            defaultValue = 634;

            material = _scenario.GeneratedLots.FirstOrDefault();
            materials.Add(material);
            material.LoadChildren();

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

            PostDataCollectionAndValidateSpaceMessage(dataCollection, datacollectionLimitSet, pointsToPost);

            ///<Step> Validate Material Hold Reasons.</Step>
            ///<ExpectedValue> The Material has one Hold Reasons (Out of Spec).</ExpectedValue>
            material.Load();
            material.LoadRelations(new Collection<string>
            {
                "MaterialHoldReason"
            });

            Assert.IsTrue(material.HoldCount == 1, $"Material should have 1 reason instead has {material.HoldCount}");
            MaterialHoldReason outOfSpecHoldReason = material.MaterialHoldReasons.FirstOrDefault();
            outOfSpecHoldReason.TargetEntity.Load();
            Assert.IsTrue(outOfSpecHoldReason.TargetEntity.Name.Equals("Out Of Spec"), $"Material Hold Reason Name should be: Out of Spec instead is: {outOfSpecHoldReason.TargetEntity.Name}");

            Assert.IsTrue(material.OpenExceptionProtocolsCount == 0, $"Material shouldn't have protocol instance opened, instead has {material.OpenExceptionProtocolsCount}.");
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

            defaultValue = 640;

            material = _scenario.GeneratedLots.FirstOrDefault();
            materials.Add(material);
            material.LoadChildren();

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

            PostDataCollectionAndValidateSpaceMessage(dataCollection, datacollectionLimitSet, pointsToPost);

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
        }

        /// <summary>
        /// Post DataCollection and validate Space Message
        /// </summary>
        /// <param name="dataCollection">DataCollection</param>
        /// <param name="datacollectionLimitSet">DataCollection Limit Set</param>
        /// <param name="pointsToPost">Points to Post</param>
        private void PostDataCollectionAndValidateSpaceMessage(DataCollection dataCollection, DataCollectionLimitSet datacollectionLimitSet, DataCollectionPointCollection pointsToPost)
        {
            bool messageBusMessageWasReceived = false;

            // Initialize message bus.
            Transport messageBusTransport = new Transport(BaseContext.GetMessageBusTransportConfiguration());

            // Connect to message bus.
            messageBusTransport.Start();

            // Subscribe to event.
            messageBusTransport.Subscribe(AMSOsramConstants.CustomReportEDCToSpace, (string subject, MbMessage message) =>
            {
                if (message != null && !string.IsNullOrWhiteSpace(message.Data))
                {
                    // Deserialize MessageBus message received to a Dictionary
                    // - Key: PropertieName
                    // - Value: PropertieValue (MessageToSend)
                    Dictionary<string, string> receivedMessage = JsonConvert.DeserializeObject<Dictionary<string, string>>(message.Data);

                    ValidateMessage(receivedMessage["Message"]);

                    messageBusTransport.Unsubscribe(AMSOsramConstants.CustomReportEDCToSpace);

                    messageBusMessageWasReceived = true;
                }
            });

            ComplexPerformDataCollectionInput complexPostDCDataInput = new ComplexPerformDataCollectionInput()
            {
                Material = material,
                DataCollection = dataCollection,
                DataCollectionLimitSet = datacollectionLimitSet,
                DataCollectionPointCollection = pointsToPost
            };

            ComplexPerformDataCollectionOutput complexPostDCDataOutput = complexPostDCDataInput.ComplexPerformDataCollectionSync();

            Func<bool> waitForMessageBus = () =>
            {
                return messageBusMessageWasReceived;
            };

            waitForMessageBus.WaitFor();

            Assert.IsTrue(messageBusMessageWasReceived, "Message was not received from message bus.");
        }

        /// <summary>
        /// Validate message received from Message Bus
        /// </summary>
        /// <param name="message">Message to validate</param>
        private void ValidateMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Dictionary<string, string> expectedKeys = new Dictionary<string, string>()
                {
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

                ///<Step> Validate the content of the Integration Entry </Step>
                CustomReportEDCToSpace spaceInformation = CustomUtilities.DeserializeXmlToObject<CustomReportEDCToSpace>(message);

                foreach (Key key in spaceInformation.Keys)
                {
                    if (!string.IsNullOrEmpty(expectedKeys[key.Name]))
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
                        Assert.IsTrue(sample.Upper.Equals("665.0000000000"), $"Sample field for parameter {sample.ParameterName} should have the upper limit error equal to 665.0000000000 instead is {sample.Upper}.");
                    }

                    foreach (Raw sampleRaw in sample.Raws.raws)
                    {
                        decimal value = defaultValue + sampleCount;
                        Assert.IsTrue(sampleRaw.RawValue.Equals(value), $"Value for parameter {sample.ParameterName} should be {value} instead is {sampleRaw.RawValue}.");
                        sampleCount++;
                    }
                }
            }
        }
    }
}