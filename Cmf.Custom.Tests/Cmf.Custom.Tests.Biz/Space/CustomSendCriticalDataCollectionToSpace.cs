using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.ERP.Space;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Tibco;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.MessageBus.Client;
using Cmf.MessageBus.Messages;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.EdcManagement.DataCollectionManagement.OutputObjects;
using Cmf.TestScenarios.MaintenanceManagement.MaintenancePlanScenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.Space
{
    [TestClass]
    public class CustomSendCriticalDataCollectionToSpace
    {
        private const string DataCollectionName = "SpaceDCTest";
        private const string DataCollectionLimitSetName = "SpaceDCTestLimitSet";

        private CustomExecutionScenario scenario;
        
        private static Transport transport;
        private string isToUnsubscribeTopic;
        private List<Dictionary<string, string>> messageBusMessages;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            transport = new Transport(BaseContext.GetMessageBusTransportConfiguration());
            transport.Start();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (transport != null)
            {
                transport.Stop();
            }
        }

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            scenario = new CustomExecutionScenario();
            scenario.NumberOfMaterialsToGenerate = 1;
            scenario.NumberOfChildMaterialsToGenerate = 3;

            messageBusMessages = new List<Dictionary<string, string>>();
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (scenario != null)
            {
                scenario.CompleteCleanUp();
            }

            if (!String.IsNullOrEmpty(isToUnsubscribeTopic))
            {
                transport.Unsubscribe(isToUnsubscribeTopic);
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
            scenario.Setup();

            decimal defaultValue = 634;

            Material material = scenario.GeneratedLots.FirstOrDefault();
            material.LoadChildren();

            ///<Step> Open Data Collection Instance </Step>
            DataCollection dataCollection = new DataCollection() { 
                Name = DataCollectionName 
            };
            dataCollection.Load();

            DataCollectionLimitSet datacollectionLimitSet = new DataCollectionLimitSet
            {
                Name = DataCollectionLimitSetName
            };
            datacollectionLimitSet.Load();

            DataCollectionPointCollection pointsToPost = new DataCollectionPointCollection();

            int count = 0;
            dataCollection.LoadRelations(new Collection<string> { "DataCollectionParameter" });

            ParameterCollection parameters = new ParameterCollection();
            parameters.AddRange(dataCollection.DataCollectionParameters.Select(s => s.TargetEntity));
            parameters.Load();

            foreach (Parameter parameter in parameters)
            {
                DataCollectionParameterSampleKey? sampleKey = dataCollection.DataCollectionParameters.FirstOrDefault(f => f.TargetEntity.Id == parameter.Id).SampleKey;

                if (sampleKey == DataCollectionParameterSampleKey.MaterialId)
                {
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
                else
                {
                    DataCollectionPoint collectionPoint = new DataCollectionPoint();
                    collectionPoint.Value = defaultValue + count;
                    collectionPoint.TargetEntity = parameter;
                    collectionPoint.ReadingNumber = 1;
                    collectionPoint.SampleId = "1";
                    pointsToPost.Add(collectionPoint);
                    count++;
                }
            }

            string message = PostDataCollectionAndValidateSpaceMessage(material, dataCollection, datacollectionLimitSet, pointsToPost);

            ValidateMessage(message, material, pointsToPost, dataCollection, datacollectionLimitSet);

            material.Load();

            Assert.IsTrue(material.HoldCount == 1, $"Material should have 1 reason instead has {material.HoldCount}");
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
            scenario.Setup();

            Material material = scenario.GeneratedLots.FirstOrDefault();
            material.LoadChildren();

            ///<Step> Open Data Collection Instance </Step>
            DataCollection dataCollection = new DataCollection() { 
                Name = DataCollectionName 
            };
            dataCollection.Load();

            DataCollectionLimitSet datacollectionLimitSet = new DataCollectionLimitSet
            {
                Name = DataCollectionLimitSetName
            };
            datacollectionLimitSet.Load();

            DataCollectionPointCollection pointsToPost = new DataCollectionPointCollection();

            dataCollection.LoadRelations(new Collection<string> { "DataCollectionParameter" });
            
            ParameterCollection parameters = new ParameterCollection();
            parameters.AddRange(dataCollection.DataCollectionParameters.Select(s => s.TargetEntity));
            parameters.Load();

            int count = 0;
            foreach (Parameter parameter in parameters)
            {
                decimal defaultValue = 640;

                DataCollectionParameterSampleKey? sampleKey = dataCollection.DataCollectionParameters.FirstOrDefault(f => f.TargetEntity.Id == parameter.Id).SampleKey;

                if (sampleKey == DataCollectionParameterSampleKey.MaterialId)
                {
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
                else
                {
                    DataCollectionPoint collectionPoint = new DataCollectionPoint();
                    collectionPoint.Value = defaultValue + count;
                    collectionPoint.TargetEntity = parameter;
                    collectionPoint.ReadingNumber = 1;
                    collectionPoint.SampleId = "1";
                    pointsToPost.Add(collectionPoint);
                    count++;
                }
            }

            string message = PostDataCollectionAndValidateSpaceMessage(material, dataCollection, datacollectionLimitSet, pointsToPost);

            ValidateMessage(message, material, pointsToPost, dataCollection, datacollectionLimitSet);

            ///<Step> Validate Protocol Opened.</Step>
            ///<ExpectedValue> The lot should have a protocol opened.</ExpectedValue>
            material.Load();
            material.LoadRelations(new Collection<string> { "ProtocolMaterial" });
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
        private string PostDataCollectionAndValidateSpaceMessage(Material material, DataCollection dataCollection, DataCollectionLimitSet datacollectionLimitSet, DataCollectionPointCollection pointsToPost)
        {
            Func<bool> waitForMessageBus = SubscribeMessageBus(amsOSRAMConstants.CustomReportEDCToSpace, 1);

            ComplexPerformDataCollectionOutput complexPostDCDataOutput = new ComplexPerformDataCollectionInput()
            {
                Material = material,
                DataCollection = dataCollection,
                DataCollectionLimitSet = datacollectionLimitSet,
                DataCollectionPointCollection = pointsToPost
            }.ComplexPerformDataCollectionSync();

            waitForMessageBus.WaitFor();

            Dictionary<string, string> messageBusMessage = messageBusMessages.FirstOrDefault();
            Assert.IsTrue(messageBusMessage.Count > 0, "Message was not received from message bus.");

            string message;
            messageBusMessage.TryGetValue("Message", out message);

            return message;
        }

        /// <summary>
        /// Validate message received from Message Bus
        /// </summary>
        /// <param name="message">Message to validate</param>
        private void ValidateMessage(string message, Material material, DataCollectionPointCollection pointsToPost, DataCollection dataCollection, DataCollectionLimitSet dataCollectionLimitSet)
        {
            Assert.IsFalse(String.IsNullOrEmpty(message), "Message received from MessageBus cannot be null or empty");

            material.Load(1);

            Product product = material.Product;
            product.LoadAttribute(amsOSRAMConstants.ProductAttributeBasicType);

            Area area = null;
            Resource subResource = null;
            Resource resource = material.LastProcessedResource;
            
            string ldCode = String.Empty;
            string productBasicType = String.Empty;
            if (material.Product.Attributes.ContainsKey(amsOSRAMConstants.ProductAttributeBasicType))
            {
                productBasicType = (string)material.Product.Attributes.GetValueOrDefault(amsOSRAMConstants.ProductAttributeBasicType);
            }

            if (resource != null)
            {
                resource.LoadRelations(new Collection<string> { "SubResource" });

                if (resource.RelationCollection.ContainsKey("SubResource"))
                {
                    subResource = resource.SubResources.FirstOrDefault();
                }

                area = resource.Area;

                if (area != null)
                {
                    ldCode = area.Attributes.GetValueOrDefault(amsOSRAMConstants.AreaAttributeAreaCode).ToString();
                }
            }

            if (String.IsNullOrEmpty(ldCode))
            {
                // Load Site
                Site site = material.Facility?.Site;
                site.Load();

                site.LoadAttribute(amsOSRAMConstants.SiteAttributeSiteCode);

                // Get SiteCode attribute value
                ldCode = site.Attributes.GetValueOrDefault(amsOSRAMConstants.SiteAttributeSiteCode).ToString();
            }

            Dictionary<string, string> expectedKeys = new Dictionary<string, string>()
            {
                {"PROCESS",  product.Name},
                {"BASIC_TYPE",productBasicType },
                {"AREA", area != null ? area.Name : String.Empty },
                {"OWNER", material.ProductionOrder != null ? material.ProductionOrder.Name : String.Empty},
                {"ROUTE", $"{material.Flow.Name}_{material.Flow.Version}"},
                {"OPERATION",material.Step.Name},
                {"PROCESS_SPS", material.RequiredService != null ? material.RequiredService.Name : String.Empty},
                {"EQUIPMENT", resource != null ? resource.Name: String.Empty },
                {"CHAMBER",  subResource != null ? subResource.Name : String.Empty},
                {"RECIPE", material.CurrentRecipeInstance != null ? material.CurrentRecipeInstance.ParentEntity.Name : String.Empty},
                {"MEAS_EQUIPMENT", resource != null && resource.Type.Equals("Measure") ? resource.Type : String.Empty },
                {"BATCH_NAME", String.Empty },
                {"LOT",$"{material.Name}"},
                {"QTY",$"{material.PrimaryQuantity + material.SubMaterialsPrimaryQuantity}"},
                {"WAFER","."},
                {"PUNKT","."},
                {"X","."},
                {"Y","."}
            };

            CustomReportEDCToSpace spaceInformation = CustomUtilities.DeserializeXmlToObject<CustomReportEDCToSpace>(message);

            // Validate Parameter Names that should be sent to SPACE
            ParameterCollection parameters = new ParameterCollection();
            parameters.AddRange(dataCollection.DataCollectionParameters.Select(s => s.TargetEntity));
            parameters.Load();

            foreach(Parameter parameter in parameters)
            {
                parameter.LoadAttributes(new Collection<string> { amsOSRAMConstants.ParameterAttributeSendToSpace });
            }

            ParameterCollection elegibleParameters = new ParameterCollection();
            elegibleParameters.AddRange(
                parameters.Where(s => 
                    s.HasAttributeDefined(amsOSRAMConstants.ParameterAttributeSendToSpace) && 
                    s.GetAttributeValue(amsOSRAMConstants.ParameterAttributeSendToSpace, false) &&
                    (s.DataType == ParameterDataType.Decimal || s.DataType == ParameterDataType.Long)));

            Assert.AreEqual(elegibleParameters.Count, spaceInformation.Samples.Count, $"Should have {elegibleParameters.Count} samples but got {spaceInformation.Samples.Count} instead");
            Assert.IsTrue(elegibleParameters.Select(s => s.Name).ToList().Except(spaceInformation.Samples.Select(s => s.ParameterName)).ToList().Count == 0, "There is a mismatch between the parameters that should be send to space and the ones sent to space");

            // Validate Sender
            Assert.AreEqual(CustomUtilities.GetEnvironmentName(), spaceInformation.Sender.Value, $"The Sender should be {CustomUtilities.GetEnvironmentName()} instead of {spaceInformation.Sender.Value}");
            
            // Validate LDS
            Assert.AreEqual(1, spaceInformation.Lds.Count, $"Should have 1 LD instead of {spaceInformation.Lds.Count}");
            Assert.AreEqual(ldCode, spaceInformation.Lds.FirstOrDefault().Id, $"The LDSCode should be {ldCode} instead of {spaceInformation.Lds.FirstOrDefault().Id}");

            // Validate SampleDate
            Assert.IsNotNull(spaceInformation.SampleDate, "The SampleDate cannot be null or empty");

            dataCollectionLimitSet.LoadRelations(new Collection<string> { "DataCollectionParameterLimit" }, 1);

            foreach (Sample sample in spaceInformation.Samples)
            {
                // Validate Parameter name and unit
                DataCollectionPointCollection dataCollectionPoints = new DataCollectionPointCollection();
                dataCollectionPoints.AddRange(pointsToPost.Where(f => f.TargetEntity.Name == sample.ParameterName));

                DataCollectionParameterLimit limits = dataCollectionLimitSet.DataCollectionParameterLimits.FirstOrDefault(f => f.TargetEntity.Name == sample.ParameterName);
                Parameter parameter = parameters.FirstOrDefault(f => f.Name == sample.ParameterName);

                if (!String.IsNullOrEmpty(parameter.DataUnit))
                {
                    Assert.AreEqual(parameter.DataUnit, sample.ParameterUnit, $"Sample field for parameter {sample.ParameterName} should have the unit {parameter.DataUnit} but got {sample.ParameterUnit} instead.");
                }

                foreach (Key key in sample.Keys)
                {
                    if (!string.IsNullOrEmpty(expectedKeys[key.Name]))
                    {
                        Assert.AreEqual(expectedKeys[key.Name], key.Value, $"Sample field for parameter {sample.ParameterName} should have the key with name {key.Name} with the value {expectedKeys[key.Name]}, but got {key.Value} instead.");
                    }
                }

                if (limits != null)
                {
                    if (limits.LowerWarningLimit != null && limits.LowerWarningLimit.HasValue)
                    {
                        Assert.AreEqual(Decimal.Truncate((decimal)limits.LowerWarningLimit.Value), Decimal.Truncate(Decimal.Parse(sample.Lower)), $"Sample field for parameter {sample.ParameterName} should have the lower limit error equal to {Decimal.Truncate((decimal)limits.LowerWarningLimit.Value)} instead is {Decimal.Truncate(Decimal.Parse(sample.Lower))}.");
                    } else if (limits.LowerErrorLimit != null && limits.LowerErrorLimit.HasValue)
                    {
                        Assert.AreEqual(Decimal.Truncate((decimal)limits.LowerErrorLimit.Value), Decimal.Truncate(Decimal.Parse(sample.Lower)), $"Sample field for parameter {sample.ParameterName} should have the lower limit error equal to {Decimal.Truncate((decimal)limits.LowerErrorLimit.Value)} instead is {Decimal.Truncate(Decimal.Parse(sample.Lower))}.");
                    }

                    if (limits.UpperWarningLimit != null && limits.UpperWarningLimit.HasValue)
                    {
                        Assert.AreEqual(Decimal.Truncate((decimal)limits.UpperWarningLimit.Value), Decimal.Truncate(Decimal.Parse(sample.Upper)), $"Sample field for parameter {sample.ParameterName} should have the lower limit error equal to {Decimal.Truncate((decimal)limits.UpperWarningLimit.Value)} instead is {Decimal.Truncate(Decimal.Parse(sample.Upper))}.");
                    }
                    else if (limits.UpperErrorLimit != null && limits.UpperErrorLimit.HasValue)
                    {
                        Assert.AreEqual(Decimal.Truncate((decimal)limits.UpperErrorLimit.Value), Decimal.Truncate(Decimal.Parse(sample.Upper)), $"Sample field for parameter {sample.ParameterName} should have the lower limit error equal to {Decimal.Truncate((decimal)limits.UpperErrorLimit.Value)} instead is {Decimal.Truncate(Decimal.Parse(sample.Upper))}.");
                    }
                }

                Assert.AreEqual(dataCollectionPoints.Count, sample.Raws.raws.Count, $"Sample field for parameter {sample.ParameterName} should have {dataCollectionPoints.Count} raws instead of {sample.Raws.raws.Count}.");
                Assert.AreEqual("True", sample.Raws.StoreRaws, $"Sample field for parameter {sample.ParameterName} should have the StoreRaws set to True instead of {sample.Raws.StoreRaws}.");

                int counter = 0;
                bool isSampleTypeMaterialId = dataCollection.DataCollectionParameters.FirstOrDefault(f => f.TargetEntity.Name == sample.ParameterName).SampleKey == DataCollectionParameterSampleKey.MaterialId;

                foreach (Raw sampleRaw in sample.Raws.raws)
                {
                    DataCollectionPoint point = dataCollectionPoints[counter];
                    Assert.AreEqual(point.Value, sampleRaw.RawValue, $"Sample field for parameter {sample.ParameterName} should have the raw{counter} as {point.Value} but got {sampleRaw.RawValue} instead.");
                    counter++;

                    
                    if (isSampleTypeMaterialId)
                    {
                        Assert.AreEqual(point.Value, sampleRaw.RawValue, $"Sample field for parameter {sample.ParameterName} should have the raw{counter} as {point.Value} but got {sampleRaw.RawValue} instead.");
                    }
                }
            }
        }

        private Func<bool> SubscribeMessageBus(string topic, int numberOfMessages = 1)
        {

            transport.Subscribe(topic.ToString(), (string subject, MbMessage message) =>
            {
                if (message != null && !string.IsNullOrWhiteSpace(message.Data))
                {
                    messageBusMessages.Add(JsonConvert.DeserializeObject<Dictionary<string, string>>(message.Data));
                }
            });

            isToUnsubscribeTopic = topic.ToString();

            Func<bool> waitForMessageBus = () =>
            {
                bool isDone = messageBusMessages.Count == numberOfMessages;

                if (isDone)
                {
                    transport.Unsubscribe(topic.ToString());
                    isToUnsubscribeTopic = null;
                }

                return isDone;
            };

            return waitForMessageBus;
        }
    }
}
