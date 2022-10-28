using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.ERP.Space;
using Cmf.Custom.Tests.Biz.Common.ERP.Spaces.CustomReportEDCToSpaceDto;
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

namespace Cmf.Custom.Tests.Biz.Space
{
    [TestClass]
    public class CustomSendCriticalDataCollectionToSpace
    {
        private static Dictionary<string, object> rollbackResourceProperties = new Dictionary<string, object>();
        private static Cmf.Foundation.Common.DynamicExecutionEngine.Action rollbackDEEAction;
        private static CustomExecutionScenario classExecutionScenario = new CustomExecutionScenario();
        private static string ProcessRecipeName = amsOSRAMConstants.DefaultRecipeName;
        private static string ProcessServiceName = amsOSRAMConstants.DefaultServiceName;

        private const string DataCollectionName = amsOSRAMConstants.DefaultSpaceDataCollectionName;
        private const string DataCollectionLimitSetName = amsOSRAMConstants.DefaultSpaceDataCollectionLimitSetName;

        private const string ProcessResourceName = amsOSRAMConstants.DefaultTestProcessResourceName;
        private const string ProcessSubResourceName = amsOSRAMConstants.DefaultTestProcessSubResourceName;
        private const string MeasurementResourceName = amsOSRAMConstants.DefaultTestMeasurementResourceAlternativeName;
        private const string MeasurementSubResourceName = amsOSRAMConstants.DefaultTestMeasurementSubResourceAlternativeName;

        private const string FacilityName = amsOSRAMConstants.DefaultFacilityName;

        private const int Quantity = 2;

        private CustomExecutionScenario scenario;

        private static Transport transport;
        private string isToUnsubscribeTopic;
        private List<Dictionary<string, object>> messageBusMessages;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            transport = new Transport(BaseContext.GetMessageBusTransportConfiguration());
            transport.Start();

            // Set IsRecipeManagementEnabled on the resource
            Resource resource = new Resource();
            resource.Name = ProcessResourceName;
            resource.Load();

            rollbackResourceProperties.Add("IsRecipeManagementEnabled", resource.IsRecipeManagementEnabled);
            resource.IsRecipeManagementEnabled = true;

            resource.Save();

            classExecutionScenario = new CustomExecutionScenario
            {
                RecipeContext = new List<Dictionary<string, string>>
                {
                    {
                        new Dictionary<string, string> {
                            { "Service", ProcessServiceName },
                            { "Resource",ProcessResourceName },
                            { "Recipe", ProcessRecipeName }
                        }
                    }
                }
            };

            classExecutionScenario.Setup();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (transport != null)
            {
                transport.Stop();
            }

            if (classExecutionScenario != null)
            {
                classExecutionScenario.CompleteCleanUp();
            }

            // Rollback IsRecipeManagementEnabled on the resource
            Resource resource = new Resource();
            resource.Name = amsOSRAMConstants.DefaultSorterResourceName;
            resource.Load();

            resource.IsRecipeManagementEnabled = (bool?)rollbackResourceProperties["IsRecipeManagementEnabled"];

            resource.Save();
        }

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            scenario = new CustomExecutionScenario();
            scenario.NumberOfMaterialsToGenerate = 1;
            scenario.NumberOfChildMaterialsToGenerate = Quantity;
            scenario.FacilityName = FacilityName;

            messageBusMessages = new List<Dictionary<string, object>>();
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

            Material lot = scenario.GeneratedLots.FirstOrDefault();
            lot.LoadChildren();

            Material firstLogicalWafer = lot.SubMaterials[0];
            (Material firstSubstrate, firstLogicalWafer) = scenario.GenerateWafer(parentMaterial: firstLogicalWafer, type: amsOSRAMConstants.MaterialWaferSubstrateType);
            (Material firstCrystal, firstLogicalWafer) = scenario.GenerateWafer(parentMaterial: firstLogicalWafer, type: amsOSRAMConstants.MaterialWaferCrystalType);

            Material secondLogicalWafer = lot.SubMaterials[1];
            (Material secondSubstrate, secondLogicalWafer) = scenario.GenerateWafer(parentMaterial: secondLogicalWafer, type: amsOSRAMConstants.MaterialWaferSubstrateType);

            lot = PerformMaterialProcessToMeasurement(lot);

            DataCollection dataCollection = new DataCollection
            {
                Name = DataCollectionName
            };
            dataCollection.Load();

            DataCollectionLimitSet datacollectionLimitSet = new DataCollectionLimitSet
            {
                Name = DataCollectionLimitSetName
            };
            datacollectionLimitSet.Load();

            (DataCollectionPointCollection pointsToPost, Dictionary<string, Dictionary<string, List<decimal>>> mapParameterSamplePoints) = CreateDataCollectionPointCollection(dataCollection, datacollectionLimitSet, lot, false);

            string message = PostDataCollectionAndValidateSpaceMessage(lot, dataCollection, datacollectionLimitSet, pointsToPost);

            ValidateMessage(message, lot, pointsToPost, dataCollection, datacollectionLimitSet, mapParameterSamplePoints, lastRecipeName: ProcessRecipeName);

            lot.Load();

            Assert.IsTrue(lot.HoldCount == 1, $"Material should have 1 reason instead has {lot.HoldCount}");
            Assert.IsTrue(lot.OpenExceptionProtocolsCount == 0, $"Material shouldn't have protocol instance opened, instead has {lot.OpenExceptionProtocolsCount}.");
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

            Material lot = scenario.GeneratedLots.FirstOrDefault();
            lot.LoadChildren();

            Material firstLogicalWafer = lot.SubMaterials[0];
            (Material firstSubstrate, firstLogicalWafer) = scenario.GenerateWafer(parentMaterial: firstLogicalWafer, type: amsOSRAMConstants.MaterialWaferSubstrateType);
            (Material firstCrystal, firstLogicalWafer) = scenario.GenerateWafer(parentMaterial: firstLogicalWafer, type: amsOSRAMConstants.MaterialWaferCrystalType);

            Material secondLogicalWafer = lot.SubMaterials[1];
            (Material secondSubstrate, secondLogicalWafer) = scenario.GenerateWafer(parentMaterial: secondLogicalWafer, type: amsOSRAMConstants.MaterialWaferSubstrateType);

            lot = PerformMaterialProcessToMeasurement(lot);

            DataCollection dataCollection = new DataCollection
            {
                Name = DataCollectionName
            };
            dataCollection.Load();

            DataCollectionLimitSet datacollectionLimitSet = new DataCollectionLimitSet
            {
                Name = DataCollectionLimitSetName
            };
            datacollectionLimitSet.Load();

            (DataCollectionPointCollection pointsToPost, Dictionary<string, Dictionary<string, List<decimal>>> mapParameterSamplePoints) = CreateDataCollectionPointCollection(dataCollection, datacollectionLimitSet, lot);

            string message = PostDataCollectionAndValidateSpaceMessage(lot, dataCollection, datacollectionLimitSet, pointsToPost);

            ValidateMessage(message, lot, pointsToPost, dataCollection, datacollectionLimitSet, mapParameterSamplePoints, lastRecipeName: ProcessRecipeName);

            ///<Step> Validate Protocol Opened.</Step>
            ///<ExpectedValue> The lot should have a protocol opened.</ExpectedValue>
            lot.Load();
            lot.LoadRelations(new Collection<string> { "ProtocolMaterial" });
            Assert.IsTrue(lot.OpenExceptionProtocolsCount == 1, $"Material should have one protocol instance opened, instead has {lot.OpenExceptionProtocolsCount}.");

            ProtocolInstance protocolInstance = lot.MaterialProtocols.FirstOrDefault().SourceEntity;
            protocolInstance.Load();
            protocolInstance.ParentEntity.Load();
            Assert.IsTrue(protocolInstance.ParentEntity.Name.Equals("8D"), $"Protocol should be 8D instead is {protocolInstance.ParentEntity.Name}");
            Assert.IsTrue(lot.HoldCount == 0, $"Material should have 1 reason instead has {lot.HoldCount}");
        }

        private Material PerformMaterialProcessToMeasurement(Material lot)
        {
            lot.Load();

            Resource processResource = new Resource
            {
                Name = ProcessResourceName
            };
            processResource.Load();

            lot.ComplexDispatchAndTrackIn(processResource);

            Resource processSubResource = new Resource
            {
                Name = ProcessSubResourceName
            };
            processSubResource.Load();

            lot.LoadChildren();
            foreach (Material logicaWafer in lot.SubMaterials)
            {
                logicaWafer.ComplexTrackIn(processSubResource);
                logicaWafer.ComplexTrackOutMaterial();
                processSubResource.Load();
            }

            lot.ComplexTrackOutAndMoveNext();

            Resource measurementResource = new Resource
            {
                Name = MeasurementResourceName
            };
            measurementResource.Load();

            lot = lot.ComplexDispatchAndTrackIn(measurementResource).Material;
            lot.LoadChildren();

            return lot;
        }

        private Tuple<DataCollectionPointCollection, Dictionary<string, Dictionary<string, List<decimal>>>> CreateDataCollectionPointCollection(DataCollection dataCollection, DataCollectionLimitSet dataCollectionLimitSet, Material material, bool insideLimits = true)
        {
            DataCollectionPointCollection pointsToPost = new DataCollectionPointCollection();

            dataCollection.LoadRelation("DataCollectionParameter");

            ParameterCollection parameters = new ParameterCollection();
            parameters.AddRange(dataCollection.DataCollectionParameters.Select(s => s.TargetEntity));
            parameters.Load();

            dataCollectionLimitSet.LoadRelation("DataCollectionParameterLimit");

            Dictionary<string, Dictionary<string, List<decimal>>> mapParameterSamplePoints = new Dictionary<string, Dictionary<string, List<decimal>>>();
            decimal defaultValue = 640;

            foreach (Parameter parameter in parameters)
            {
                parameter.LoadAttributes(new Collection<string> { amsOSRAMConstants.ParameterAttributeSendToSpace });

                DataCollectionParameterLimit parameterLimit = dataCollectionLimitSet.DataCollectionParameterLimits.FirstOrDefault(f => f.TargetEntity.Id == parameter.Id);

                decimal insideLimitValue = parameterLimit != null && parameterLimit.MinValue.HasValue ? parameterLimit.MinValue.Value + 1 : defaultValue;
                decimal outsideLimitValue = parameterLimit != null && parameterLimit.MaxValue.HasValue ? parameterLimit.MaxValue.Value + 1 : defaultValue;

                decimal pointValue = insideLimits ? insideLimitValue : outsideLimitValue;

                DataCollectionParameter dcParameter = dataCollection.DataCollectionParameters.FirstOrDefault(f => f.TargetEntity.Id == parameter.Id);
                DataCollectionParameterSampleKey? sampleKey = dcParameter.SampleKey;

                bool isParameterElegible = parameter.HasAttributeDefined(amsOSRAMConstants.ParameterAttributeSendToSpace) &&
                    parameter.GetAttributeValue(amsOSRAMConstants.ParameterAttributeSendToSpace, false) &&
                    (parameter.DataType == ParameterDataType.Decimal || parameter.DataType == ParameterDataType.Long);

                if (sampleKey == DataCollectionParameterSampleKey.MaterialId)
                {
                    Dictionary<string, List<decimal>> waferValues = new Dictionary<string, List<decimal>>();

                    foreach (Material wafer in material.SubMaterials)
                    {
                        List<decimal> values = new List<decimal>();

                        for (int readingNumber = 1; dcParameter.MaximumSampleReadings >= readingNumber; readingNumber++)
                        {
                            DataCollectionPoint collectionPoint = new DataCollectionPoint();
                            collectionPoint.Value = pointValue;
                            collectionPoint.TargetEntity = parameter;
                            collectionPoint.ReadingNumber = readingNumber;
                            collectionPoint.SampleId = wafer.Name;
                            pointsToPost.Add(collectionPoint);

                            values.Add(pointValue);
                        }

                        waferValues.Add(wafer.Name, values);
                    }

                    if (isParameterElegible)
                    {
                        mapParameterSamplePoints.Add(parameter.Name, waferValues);
                    }
                }
                else
                {
                    Dictionary<string, List<decimal>> waferValues = new Dictionary<string, List<decimal>>();

                    for (int sampleId = 1; dcParameter.MaximumSamples >= sampleId; sampleId++)
                    {
                        List<decimal> values = new List<decimal>();

                        for (int readingNumber = 1; dcParameter.MaximumSampleReadings >= readingNumber; readingNumber++)
                        {
                            DataCollectionPoint collectionPoint = new DataCollectionPoint();
                            collectionPoint.Value = pointValue;
                            collectionPoint.TargetEntity = parameter;
                            collectionPoint.ReadingNumber = 1;
                            collectionPoint.SampleId = sampleId.ToString();
                            pointsToPost.Add(collectionPoint);

                            values.Add(pointValue);
                        }

                        waferValues.Add(sampleId.ToString(), values);
                    }

                    if (isParameterElegible)
                    {
                        mapParameterSamplePoints.Add(parameter.Name, waferValues);
                    }
                }
            }

            return Tuple.Create(pointsToPost, mapParameterSamplePoints);
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

            Dictionary<string, object> messageBusMessage = messageBusMessages.FirstOrDefault();
            Assert.IsTrue(messageBusMessage.Count > 0, "Message was not received from message bus.");

            object message;
            messageBusMessage.TryGetValue("Message", out message);

            return (string)message;
        }

        /// <summary>
        /// Validate message received from Message Bus
        /// </summary>
        /// <param name="message">Message to validate</param>
        private void ValidateMessage(string message,
                                     Material material,
                                     DataCollectionPointCollection pointsToPost,
                                     DataCollection dataCollection,
                                     DataCollectionLimitSet dataCollectionLimitSet,
                                     Dictionary<string, Dictionary<string, List<decimal>>> mapParametersSamplePoints,
                                     string lastRecipeName = "-",
                                     string currentRecipeName = "-",
                                     string operatorName = "-",
                                     string shiftName = "-",
                                     string processResourceName = ProcessResourceName,
                                     string processSubResourceName = ProcessSubResourceName,
                                     string measurementResourceName = MeasurementResourceName,
                                     string measurementSubResourceName = MeasurementSubResourceName
            )
        {
            Assert.IsFalse(String.IsNullOrEmpty(message), "Message received from MessageBus cannot be null or empty");

            material.Load(1);

            Product product = material.Product;
            product.LoadAttribute(amsOSRAMConstants.ProductAttributeSAPProductType);

            Resource processEquipment = new Resource
            {
                Name = processResourceName
            };
            processEquipment.Load();

            Resource measurementEquipment = new Resource
            {
                Name = measurementResourceName
            };
            measurementEquipment.Load();

            string sapProductType = "-";
            if (material.Product.Attributes.ContainsKey(amsOSRAMConstants.ProductAttributeSAPProductType))
            {
                sapProductType = (string)material.Product.Attributes.GetValueOrDefault(amsOSRAMConstants.ProductAttributeSAPProductType);
            }

            Area area = measurementEquipment.Area;
            string ldCode = String.Empty;
            if (area != null)
            {
                ldCode = area.Attributes.GetValueOrDefault(amsOSRAMConstants.AreaAttributeAreaCode, ldCode) as string;
            }

            if (String.IsNullOrEmpty(ldCode))
            {
                // Load Site
                Site site = material.Facility?.Site;
                site.Load();

                site.LoadAttribute(amsOSRAMConstants.SiteAttributeSiteCode);

                // Get SiteCode attribute value
                ldCode = site.Attributes.GetValueOrDefault(amsOSRAMConstants.SiteAttributeSiteCode, ldCode) as string;
            }

            string stepLogicalName = material.Step.Name;

            if (material.Step.ContainsLogicalNames)
            {
                material.Flow.LoadRelation("FlowStep");
                stepLogicalName = material.Flow.FlowSteps.FirstOrDefault(w => w.TargetEntity.Id == material.Step.Id).LogicalName;
            }

            Dictionary<string, string> materialExpectedKeys = new Dictionary<string, string>()
            {
                {"LOT",  material.Name},
                {"BATCH", "-"},
                {"LOT TYPE", sapProductType },
                {"PROCESS EQUIPMENT 1", ProcessResourceName },
                {"EQUIPMENT PLATFORM", processEquipment.Model },
                {"PROCESS RECIPE 1", lastRecipeName },
                {"MEASUREMENT EQUIPMENT", MeasurementResourceName },
                {"MEASUREMENT RECIPE", currentRecipeName },
                {"QUANTITY", Quantity.ToString() },
                {"ACCESSORY", "-" },
                {"OPERATOR", operatorName },
                {"SHIFT", shiftName },
                {"SENDER", "CMF" },
                {"AREA", FacilityName },
                {"WILDCARD DA1", "-" },
                {"WILDCARD DA2", "-" },
            };

            CustomReportEDCToSpace spaceInformation = CustomUtilities.DeserializeXmlToObject<CustomReportEDCToSpace>(message);

            List<Tuple<string, string, List<decimal>>> listPointsPerParameterSamples = new List<Tuple<string, string, List<decimal>>>();

            foreach (KeyValuePair<string, Dictionary<string, List<decimal>>> mapParameterSamplePoints in mapParametersSamplePoints)
            {
                foreach (Dictionary<string, List<decimal>> samplesPoints in mapParametersSamplePoints.Values)
                {
                    foreach (KeyValuePair<string, List<decimal>> samplePoints in samplesPoints)
                    {
                        listPointsPerParameterSamples.Add(Tuple.Create(mapParameterSamplePoints.Key, samplePoints.Key, samplePoints.Value));
                    }
                }
            }

            Assert.AreEqual(listPointsPerParameterSamples.Count, spaceInformation.Samples.Count, $"Should have {listPointsPerParameterSamples.Count} samples but got {spaceInformation.Samples.Count} instead");
            Assert.IsTrue(mapParametersSamplePoints.Keys.Except(spaceInformation.Samples.Select(s => s.ParameterName)).ToList().Count == 0, "There is a mismatch between the parameters that should be send to space and the ones sent to space");

            // Validate Sender
            Assert.AreEqual(CustomUtilities.GetEnvironmentName(), spaceInformation.Sender.Value, $"The Sender should be {CustomUtilities.GetEnvironmentName()} instead of {spaceInformation.Sender.Value}");

            // Validate LDS
            Assert.AreEqual(1, spaceInformation.Lds.Count, $"Should have 1 LD instead of {spaceInformation.Lds.Count}");
            Assert.AreEqual(ldCode, spaceInformation.Lds.FirstOrDefault().Id, $"The LDSCode should be {ldCode} instead of {spaceInformation.Lds.FirstOrDefault().Id}");

            // Validate SampleDate
            Assert.IsNotNull(spaceInformation.SampleDate, "The SampleDate cannot be null or empty");

            dataCollectionLimitSet.LoadRelations(new Collection<string> { "DataCollectionParameterLimit" }, 1);
            material.LoadChildren();

            ParameterCollection parameters = new ParameterCollection();
            parameters.AddRange(mapParametersSamplePoints.Keys.Select(s => new Parameter { Name = s }));
            parameters.Load();

            int count = 0;
            foreach (Sample sample in spaceInformation.Samples)
            {
                Parameter parameter = parameters.FirstOrDefault(f => f.Name == sample.ParameterName);

                if (!String.IsNullOrEmpty(parameter.DataUnit))
                {
                    Assert.AreEqual(parameter.DataUnit, sample.ParameterUnit, $"Sample field for parameter {sample.ParameterName} should have the unit {parameter.DataUnit} but got {sample.ParameterUnit} instead.");
                }

                DataCollectionParameterLimit limits = dataCollectionLimitSet.DataCollectionParameterLimits.FirstOrDefault(f => f.TargetEntity.Name == sample.ParameterName);

                if (limits != null)
                {
                    if (limits.LowerWarningLimit != null && limits.LowerWarningLimit.HasValue)
                    {
                        Assert.AreEqual(Decimal.Truncate((decimal)limits.LowerWarningLimit.Value), Decimal.Truncate(Decimal.Parse(sample.SpecificationLimits.Lower)), $"Sample field for parameter {sample.ParameterName} should have the lower limit error equal to {Decimal.Truncate((decimal)limits.LowerWarningLimit.Value)} instead is {Decimal.Truncate(Decimal.Parse(sample.SpecificationLimits.Lower))}.");
                    }
                    else if (limits.LowerErrorLimit != null && limits.LowerErrorLimit.HasValue)
                    {
                        Assert.AreEqual(Decimal.Truncate((decimal)limits.LowerErrorLimit.Value), Decimal.Truncate(Decimal.Parse(sample.SpecificationLimits.Lower)), $"Sample field for parameter {sample.ParameterName} should have the lower limit error equal to {Decimal.Truncate((decimal)limits.LowerErrorLimit.Value)} instead is {Decimal.Truncate(Decimal.Parse(sample.SpecificationLimits.Lower))}.");
                    }

                    if (limits.UpperWarningLimit != null && limits.UpperWarningLimit.HasValue)
                    {
                        Assert.AreEqual(Decimal.Truncate((decimal)limits.UpperWarningLimit.Value), Decimal.Truncate(Decimal.Parse(sample.SpecificationLimits.Upper)), $"Sample field for parameter {sample.ParameterName} should have the lower limit error equal to {Decimal.Truncate((decimal)limits.UpperWarningLimit.Value)} instead is {Decimal.Truncate(Decimal.Parse(sample.SpecificationLimits.Upper))}.");
                    }
                    else if (limits.UpperErrorLimit != null && limits.UpperErrorLimit.HasValue)
                    {
                        Assert.AreEqual(Decimal.Truncate((decimal)limits.UpperErrorLimit.Value), Decimal.Truncate(Decimal.Parse(sample.SpecificationLimits.Upper)), $"Sample field for parameter {sample.ParameterName} should have the lower limit error equal to {Decimal.Truncate((decimal)limits.UpperErrorLimit.Value)} instead is {Decimal.Truncate(Decimal.Parse(sample.SpecificationLimits.Upper))}.");
                    }
                }

                Dictionary<string, List<decimal>> mapSamplesReadings = mapParametersSamplePoints[parameter.Name];

                bool isSampleTypeMaterialId = dataCollection.DataCollectionParameters.FirstOrDefault(f => f.TargetEntity.Name == parameter.Name).SampleKey == DataCollectionParameterSampleKey.MaterialId;
                Tuple<string, string, List<decimal>> reading = listPointsPerParameterSamples[count];

                List<decimal> dcPoints = reading.Item3;

                Assert.AreEqual(dcPoints.Count, sample.Raws.RawCollection.Count, $"Sample field for parameter {sample.ParameterName} should have {dcPoints.Count} raws instead of {sample.Raws.RawCollection.Count}.");
                Assert.AreEqual("True", sample.Raws.StoreRaws, $"Sample field for parameter {sample.ParameterName} should have the StoreRaws set to True instead of {sample.Raws.StoreRaws}.");

                int counter = 0;

                foreach (Raw sampleRaw in sample.Raws.RawCollection)
                {
                    Assert.AreEqual(dcPoints[counter], sampleRaw.RawValue, $"Sample field for parameter {sample.ParameterName} should have the raw{counter} as {dcPoints[counter]} but got {sampleRaw.RawValue} instead.");
                    counter++;
                }

                Material logicalWafer = null;
                MaterialContainer waferContainer = null;

                if (isSampleTypeMaterialId)
                {
                    logicalWafer = new Material
                    {
                        Name = reading.Item2
                    };

                    logicalWafer.Load(1);
                    logicalWafer.LoadRelation("MaterialContainer");
                    waferContainer = logicalWafer.MaterialContainer?.FirstOrDefault(f => f.SourceEntity.Name == logicalWafer.Name);

                    logicalWafer.LoadChildren();
                }

                Dictionary<string, string> waferExpectedKeys = new Dictionary<string, string>(materialExpectedKeys) {
                    { "WAFER",  logicalWafer?.Name ?? "-"},
                    { "PRODUCT", logicalWafer?.Product?.Name ?? material.Product.Name },
                    { "PRODUCT VERSION", logicalWafer?.Product?.Version.ToString() ?? material.Product.Version.ToString() },
                    { "PRODUCT TECHNOLOGY", logicalWafer.Product.ProductGroup?.Name },
                    { "POSITION 1", waferContainer?.Position.Value.ToString() ?? "-" },
                    { "POSITION 2", "-" },
                    { "FLOW", logicalWafer?.Flow?.Name ?? material.Flow.Name },
                    { "SINGLE PROCESS", stepLogicalName },
                    { "PROCESS EQUIPMENT CHAMBER", processSubResourceName ?? "-" },
                    { "MEASUREMENT EQUIPMENT CHAMBER", logicalWafer.SystemState == MaterialSystemState.Processed ? measurementSubResourceName ?? "-" : "-" },
                    { "WILDCARD EX1", logicalWafer == null ? reading.Item2 : "-" },
                    { "WILDCARD EX2", "-" },
                    { "CRYSTAL", logicalWafer?.SubMaterials.FirstOrDefault(f => f.Form == "Wafer" && f.Type == "Crystal")?.Name ?? "-" },
                    { "SUBSTRATE", logicalWafer?.SubMaterials.FirstOrDefault(f => f.Form == "Wafer" && f.Type == "Substrate")?.Name ?? "-" },
                    { "CARRIER", logicalWafer?.SubMaterials.FirstOrDefault(f => f.Form == "Wafer" && f.Type == "Carrier")?.Name ?? "-" },
                    { "VENDOR", logicalWafer?.Manufacturer != null && logicalWafer.Manufacturer.IsSupplier ? logicalWafer.Manufacturer.Name : "-" }
                };

                foreach (Key key in sample.Keys)
                {
                    if (!string.IsNullOrEmpty(waferExpectedKeys[key.Name]))
                    {
                        Assert.AreEqual(waferExpectedKeys[key.Name], key.Value, $"Sample field for parameter {sample.ParameterName} should have the key with name {key.Name} with the value {waferExpectedKeys[key.Name]}, but got {key.Value} instead.");
                    }
                }

                count++;
            }
        }

        private Func<bool> SubscribeMessageBus(string topic, int numberOfMessages = 1)
        {
            transport.Subscribe(topic.ToString(), (string subject, MbMessage message) =>
            {
                if (message != null && !string.IsNullOrWhiteSpace(message.Data))
                {
                    messageBusMessages.Add(JsonConvert.DeserializeObject<Dictionary<string, object>>(message.Data));
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