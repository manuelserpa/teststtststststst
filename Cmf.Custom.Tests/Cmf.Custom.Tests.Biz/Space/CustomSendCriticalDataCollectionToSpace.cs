using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.ERP.Space;
using Cmf.Custom.Tests.Biz.Common.ERP.Spaces.CustomReportEDCToSpaceDto;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessOrchestration.SecurityManagement.InputObjects;
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
        private const string LotProduct = amsOSRAMConstants.ProductLotProduct;
        private const string FlowPath = amsOSRAMConstants.FlowPathSpace;
        private const string FacilityName = amsOSRAMConstants.DefaultFacilityName;

        private const string ProcessServiceName = amsOSRAMConstants.ServiceCMFTestProcessService;
        private const string MeasurementServiceName = amsOSRAMConstants.ServiceCMFTestMeasurementService;
        private const string RecipeName = amsOSRAMConstants.DefaultRecipeName;

        private const string ProcessResourceName = amsOSRAMConstants.DefaultTestProcessResourceName;
        private const string ProcessSubResourceName = amsOSRAMConstants.DefaultTestProcessSubResourceName;
        private const string MeasurementResourceName = amsOSRAMConstants.DefaultTestMeasurementResourceAlternativeName;
        private const string MeasurementSubResourceName = amsOSRAMConstants.DefaultTestMeasurementSubResourceAlternativeName;

        private const string DataCollectionName = amsOSRAMConstants.DefaultSpaceDataCollectionName;
        private const string DataCollectionLimitSetName = amsOSRAMConstants.DefaultSpaceDataCollectionLimitSetName;

        private const string BusinessParterName = amsOSRAMConstants.BusinessPartnerSpaceSupplier;

        private CustomExecutionScenario scenario;
        private Dictionary<string, bool?> rollbackIsRecipeManagementEnabled;

        private static Transport transport;
        private string isToUnsubscribeTopic;
        private List<Dictionary<string, object>> messageBusMessages;

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
            messageBusMessages = new List<Dictionary<string, object>>();
            rollbackIsRecipeManagementEnabled = new Dictionary<string, bool?>();
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

            foreach (KeyValuePair<string, bool?> rollback in rollbackIsRecipeManagementEnabled)
            {
                Resource resource = new Resource
                {
                    Name = rollback.Key
                };

                resource.Load();
                resource.IsRecipeManagementEnabled = rollback.Value;
                resource.Save();
            }

            if (!String.IsNullOrEmpty(isToUnsubscribeTopic))
            {
                transport.Unsubscribe(isToUnsubscribeTopic);
            }
        }


        /// <summary>
        /// Description:
        ///     - Creates a lot with one logical wafer with a substrate without recipes
        ///     - Execute a ComplexPerformDataCollection operation on a given DataCollection with the parameters with different configurations without DataCollectionLimitSet
        ///     - Post with values inside the limits
        ///
        /// Acceptance Criteria:
        ///     - Lot is not on hold
        ///     - Lot has a protocol instance
        ///     - Validate created message structure
        ///
        /// </summary>
        /// <TestCaseID>CustomSendCriticalDataCollectionToSpace_PostEDCDataWithoutDataCollectionLimitSet_ValidateAndCreateXMLMessage</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendCriticalDataCollectionToSpace_PostEDCDataWithoutDataCollectionLimitSet_ValidateAndCreateXMLMessage()
        {
            string dataCollectionName = DataCollectionName;
            string processResourceName = ProcessResourceName;
            string processSubResourceName = ProcessSubResourceName;
            string measurementResourceName = MeasurementResourceName;
            string measurementSubResourceName = MeasurementSubResourceName;
            string facilityName = FacilityName;
            string productName = LotProduct;
            int logicalWaferQuantity = 1;
            string protocolName = ConfigUtilities.GetConfigValue(amsOSRAMConstants.DefaultProtocolSpaceConfig) as string;

            scenario.NumberOfMaterialsToGenerate = 1;
            scenario.NumberOfChildMaterialsToGenerate = logicalWaferQuantity;
            scenario.FacilityName = facilityName;
            scenario.ProductName = productName;
            scenario.FlowPath = FlowPath;
            scenario.SmartTablesToClearInSetup.Add("RecipeContext");
            scenario.Setup();

            Resource processResource = new Resource()
            {
                Name = processResourceName
            };
            processResource.Load();

            rollbackIsRecipeManagementEnabled.Add(processResource.Name, processResource.IsRecipeManagementEnabled.GetValueOrDefault());
            processResource.IsRecipeManagementEnabled = false;
            processResource.Save();

            Material lot = scenario.GeneratedLots.FirstOrDefault();
            lot.LoadChildren();

            Material firstLogicalWafer = lot.SubMaterials[0];
            (Material firstSubstrate, firstLogicalWafer) = scenario.GenerateWafer(parentMaterial: firstLogicalWafer, type: amsOSRAMConstants.MaterialWaferSubstrateType);

            Resource processSubResource = new Resource
            {
                Name = processSubResourceName
            };
            processSubResource.Load();

            Resource measurementResource = new Resource
            {
                Name = measurementResourceName
            };
            measurementResource.Load();

            Resource measurementSubResource = new Resource
            {
                Name = measurementSubResourceName
            };
            measurementSubResource.Load();

            lot = PerformMaterialProcessToMeasurement(lot, processResource, processSubResource, measurementResource, measurementSubResource);

            DataCollection dataCollection = new DataCollection
            {
                Name = dataCollectionName
            };
            dataCollection.Load();

            DataCollectionPointCollection pointsToPost = CreateDataCollectionPointCollection(dataCollection, lot);

            string message = PostDataCollectionAndValidateSpaceMessage(lot, dataCollection, pointsToPost);

            ValidateMessage(message, lot, pointsToPost, dataCollection, logicalWaferQuantity, facilityName, productName,
                processResourceName: processResourceName,
                processSubResourceName: processSubResourceName,
                measurementResourceName: measurementResourceName,
                measurementSubResourceName: measurementSubResourceName);

            lot.Load();
            lot.LoadRelations(new Collection<string> { "ProtocolMaterial" });
            Assert.IsTrue(lot.OpenExceptionProtocolsCount == 1, $"Material should have one protocol instance opened, instead has {lot.OpenExceptionProtocolsCount}.");

            ProtocolInstance protocolInstance = lot.MaterialProtocols.FirstOrDefault().SourceEntity;
            protocolInstance.Load();
            protocolInstance.ParentEntity.Load();
            Assert.IsTrue(protocolInstance.ParentEntity.Name.Equals(protocolName), $"Protocol should be {protocolName} instead is {protocolInstance.ParentEntity.Name}");
            Assert.IsTrue(lot.HoldCount == 0, $"Material should have 1 reason instead has {lot.HoldCount}");
        }

        /// <summary>
        /// Description:
        ///     - Creates a lot with one logical wafer with a substrate without recipes
        ///     - Execute a ComplexPerformDataCollection operation on a given DataCollection with the parameters with different configurations
        ///     - Post with values outside the limits
        ///
        /// Acceptance Criteria:
        ///     - Validate created message structure
        ///     - Lot is on hold with reason Out of Spec
        ///
        /// </summary>
        /// <TestCaseID>CustomSendCriticalDataCollectionToSpace_PostEDCDataOutsideLimits_ValidateAndCreateXMLMessage</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendCriticalDataCollectionToSpace_PostEDCDataOutsideLimits_ValidateAndCreateXMLMessage()
        {
            string dataCollectionName = DataCollectionName;
            string dataCollectionLimitSetName = DataCollectionLimitSetName;
            string processResourceName = ProcessResourceName;
            string processSubResourceName = ProcessSubResourceName;
            string measurementResourceName = MeasurementResourceName;
            string measurementSubResourceName = MeasurementSubResourceName;
            string facilityName = FacilityName;
            string productName = LotProduct;
            int logicalWaferQuantity = 1;

            scenario.NumberOfMaterialsToGenerate = 1;
            scenario.NumberOfChildMaterialsToGenerate = logicalWaferQuantity;
            scenario.FacilityName = facilityName;
            scenario.ProductName = productName;
            scenario.FlowPath = FlowPath;
            scenario.SmartTablesToClearInSetup.Add("RecipeContext");
            scenario.Setup();

            Resource processResource = new Resource()
            {
                Name = processResourceName
            };
            processResource.Load();

            rollbackIsRecipeManagementEnabled.Add(processResource.Name, processResource.IsRecipeManagementEnabled.GetValueOrDefault());
            processResource.IsRecipeManagementEnabled = false;
            processResource.Save();

            Material lot = scenario.GeneratedLots.FirstOrDefault();
            lot.LoadChildren();

            Material firstLogicalWafer = lot.SubMaterials[0];
            (Material firstSubstrate, firstLogicalWafer) = scenario.GenerateWafer(parentMaterial: firstLogicalWafer, type: amsOSRAMConstants.MaterialWaferSubstrateType);

            Resource processSubResource = new Resource
            {
                Name = processSubResourceName
            };
            processSubResource.Load();

            Resource measurementResource = new Resource
            {
                Name = measurementResourceName
            };
            measurementResource.Load();

            Resource measurementSubResource = new Resource
            {
                Name = measurementSubResourceName
            };
            measurementSubResource.Load();

            lot = PerformMaterialProcessToMeasurement(lot, processResource, processSubResource, measurementResource, measurementSubResource);

            DataCollection dataCollection = new DataCollection
            {
                Name = dataCollectionName
            };
            dataCollection.Load();

            DataCollectionLimitSet dataCollectionLimitSet = new DataCollectionLimitSet
            {
                Name = dataCollectionLimitSetName
            };
            dataCollectionLimitSet.Load();

            DataCollectionPointCollection pointsToPost = CreateDataCollectionPointCollection(dataCollection, lot, dataCollectionLimitSet, false);

            string message = PostDataCollectionAndValidateSpaceMessage(lot, dataCollection, pointsToPost, dataCollectionLimitSet);

            ValidateMessage(message, lot, pointsToPost, dataCollection, logicalWaferQuantity, facilityName, productName,
                dataCollectionLimitSet: dataCollectionLimitSet,
                processResourceName: processResourceName,
                processSubResourceName: processSubResourceName,
                measurementResourceName: measurementResourceName,
                measurementSubResourceName: measurementSubResourceName);

            lot.Load();

            Assert.IsTrue(lot.HoldCount == 1, $"Material should have 1 reason instead has {lot.HoldCount}");
            Assert.IsTrue(lot.OpenExceptionProtocolsCount == 0, $"Material shouldn't have protocol instance opened, instead has {lot.OpenExceptionProtocolsCount}.");
        }

        /// <summary>
        /// Description:
        ///     - Creates a lot with one logical wafer with a substrate without recipes
        ///     - Execute a ComplexPerformDataCollection operation on a given DataCollection with the parameters with different configurations
        ///     - Post with values inside the limits
        ///
        /// Acceptance Criteria:
        ///     - Lot is not on hold
        ///     - Lot has a protocol instance
        ///     - Validate created message structure
        ///
        /// </summary>
        /// <TestCaseID>CustomSendCriticalDataCollectionToSpace_PostEDCDataInsideLimits_ValidateAndCreateXMLMessage</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendCriticalDataCollectionToSpace_PostEDCDataInsideLimits_ValidateAndCreateXMLMessage()
        {
            string dataCollectionName = DataCollectionName;
            string dataCollectionLimitSetName = DataCollectionLimitSetName;
            string processResourceName = ProcessResourceName;
            string processSubResourceName = ProcessSubResourceName;
            string measurementResourceName = MeasurementResourceName;
            string measurementSubResourceName = MeasurementSubResourceName;
            string facilityName = FacilityName;
            string productName = LotProduct;
            int logicalWaferQuantity = 1;
            string protocolName = ConfigUtilities.GetConfigValue(amsOSRAMConstants.DefaultProtocolSpaceConfig) as string;

            scenario.NumberOfMaterialsToGenerate = 1;
            scenario.NumberOfChildMaterialsToGenerate = logicalWaferQuantity;
            scenario.FacilityName = facilityName;
            scenario.ProductName = productName;
            scenario.FlowPath = FlowPath;
            scenario.SmartTablesToClearInSetup.Add("RecipeContext");
            scenario.Setup();

            Resource processResource = new Resource()
            {
                Name = processResourceName
            };
            processResource.Load();

            rollbackIsRecipeManagementEnabled.Add(processResource.Name, processResource.IsRecipeManagementEnabled.GetValueOrDefault());
            processResource.IsRecipeManagementEnabled = false;
            processResource.Save();

            Material lot = scenario.GeneratedLots.FirstOrDefault();
            lot.LoadChildren();

            Material firstLogicalWafer = lot.SubMaterials[0];
            (Material firstSubstrate, firstLogicalWafer) = scenario.GenerateWafer(parentMaterial: firstLogicalWafer, type: amsOSRAMConstants.MaterialWaferSubstrateType);

            Resource processSubResource = new Resource
            {
                Name = processSubResourceName
            };
            processSubResource.Load();

            Resource measurementResource = new Resource
            {
                Name = measurementResourceName
            };
            measurementResource.Load();

            Resource measurementSubResource = new Resource
            {
                Name = measurementSubResourceName
            };
            measurementSubResource.Load();

            lot = PerformMaterialProcessToMeasurement(lot, processResource, processSubResource, measurementResource, measurementSubResource);

            DataCollection dataCollection = new DataCollection
            {
                Name = dataCollectionName
            };
            dataCollection.Load();

            DataCollectionLimitSet dataCollectionLimitSet = new DataCollectionLimitSet
            {
                Name = dataCollectionLimitSetName
            };
            dataCollectionLimitSet.Load();

            DataCollectionPointCollection pointsToPost = CreateDataCollectionPointCollection(dataCollection, lot, dataCollectionLimitSet);

            string message = PostDataCollectionAndValidateSpaceMessage(lot, dataCollection, pointsToPost, dataCollectionLimitSet);

            ValidateMessage(message, lot, pointsToPost, dataCollection, logicalWaferQuantity, facilityName, productName,
                dataCollectionLimitSet: dataCollectionLimitSet,
                processResourceName: processResourceName,
                processSubResourceName: processSubResourceName,
                measurementResourceName: measurementResourceName,
                measurementSubResourceName: measurementSubResourceName);

            lot.Load();
            lot.LoadRelations(new Collection<string> { "ProtocolMaterial" });
            Assert.IsTrue(lot.OpenExceptionProtocolsCount == 1, $"Material should have one protocol instance opened, instead has {lot.OpenExceptionProtocolsCount}.");

            ProtocolInstance protocolInstance = lot.MaterialProtocols.FirstOrDefault().SourceEntity;
            protocolInstance.Load();
            protocolInstance.ParentEntity.Load();
            Assert.IsTrue(protocolInstance.ParentEntity.Name.Equals(protocolName), $"Protocol should be {protocolName} instead is {protocolInstance.ParentEntity.Name}");
            Assert.IsTrue(lot.HoldCount == 0, $"Material should have 1 reason instead has {lot.HoldCount}");
        }

        /// <summary>
        /// Description:
        ///     - Creates a lot with one logical wafer with a substrate without recipes and without any process or measurement
        ///     - Execute a ComplexPerformDataCollection operation on a given DataCollection with the parameters with different configurations
        ///
        /// Acceptance Criteria:
        ///     - Validate created message structure
        ///
        /// </summary>
        /// <TestCaseID>CustomSendCriticalDataCollectionToSpace_ValidateAndCreateXMLMessage_OnlyMandatoryFields</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendCriticalDataCollectionToSpace_ValidateAndCreateXMLMessage_OnlyMandatoryFields()
        {
            string dataCollectionName = DataCollectionName;
            string dataCollectionLimitSetName = DataCollectionLimitSetName;
            string processResourceName = ProcessResourceName;
            string facilityName = FacilityName;
            string productName = LotProduct;
            int logicalWaferQuantity = 1;

            scenario.NumberOfMaterialsToGenerate = 1;
            scenario.NumberOfChildMaterialsToGenerate = logicalWaferQuantity;
            scenario.FacilityName = facilityName;
            scenario.ProductName = productName;
            scenario.FlowPath = FlowPath;
            scenario.SmartTablesToClearInSetup.Add("RecipeContext");
            scenario.Setup();

            Resource processResource = new Resource()
            {
                Name = processResourceName
            };
            processResource.Load();

            rollbackIsRecipeManagementEnabled.Add(processResource.Name, processResource.IsRecipeManagementEnabled.GetValueOrDefault());
            processResource.IsRecipeManagementEnabled = false;
            processResource.Save();

            Material lot = scenario.GeneratedLots.FirstOrDefault();
            lot.LoadChildren();

            DataCollection dataCollection = new DataCollection
            {
                Name = dataCollectionName
            };
            dataCollection.Load();

            DataCollectionLimitSet dataCollectionLimitSet = new DataCollectionLimitSet
            {
                Name = dataCollectionLimitSetName
            };
            dataCollectionLimitSet.Load();

            DataCollectionPointCollection pointsToPost = CreateDataCollectionPointCollection(dataCollection, lot, dataCollectionLimitSet);

            string message = PostDataCollectionAndValidateSpaceMessage(lot, dataCollection, pointsToPost, dataCollectionLimitSet);

            ValidateMessage(message, lot, pointsToPost, dataCollection, logicalWaferQuantity, facilityName, productName, dataCollectionLimitSet: dataCollectionLimitSet);
        }

        /// <summary>
        /// Description:
        ///     - Creates a lot with two logical wafer
        ///         - First one with a substrate, carrier and crystal
        ///         - Second with only a crystal
        ///     - Sets Recipes for process and measurement process
        ///     - Performs the process and measurement of the Lot and Logical Wafers on theirs resources
        ///     - Execute a ComplexPerformDataCollection operation on a given DataCollection with the parameters with different configurations
        ///
        /// Acceptance Criteria:
        ///     - Validate created message structure
        ///
        /// </summary>
        /// <TestCaseID>CustomSendCriticalDataCollectionToSpace_ValidateAndCreateXMLMessage_AllFields</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendCriticalDataCollectionToSpace_ValidateAndCreateXMLMessage_AllFields()
        {
            string dataCollectionName = DataCollectionName;
            string dataCollectionLimitSetName = DataCollectionLimitSetName;
            string processResourceName = ProcessResourceName;
            string processSubResourceName = ProcessSubResourceName;
            string measurementResourceName = MeasurementResourceName;
            string measurementSubResourceName = MeasurementSubResourceName;
            string processRecipeName = RecipeName;
            string measurementRecipeName = RecipeName;
            string vendor = BusinessParterName;
            string facilityName = FacilityName;
            string productName = LotProduct;
            string materialSapOwner = "SapOwner123";
            int logicalWaferQuantity = 2;

            scenario.NumberOfMaterialsToGenerate = 1;
            scenario.NumberOfChildMaterialsToGenerate = logicalWaferQuantity;
            scenario.FacilityName = facilityName;
            scenario.ProductName = productName;
            scenario.FlowPath = FlowPath;

            scenario.RecipeContext = new List<Dictionary<string, string>>
                {
                    {
                        new Dictionary<string, string> {
                            { "Service", ProcessServiceName },
                            { "Resource",processResourceName },
                            { "Recipe", processRecipeName }
                        }
                    },
                    {
                        new Dictionary<string, string> {
                            { "Service", MeasurementServiceName },
                            { "Resource",measurementResourceName },
                            { "Recipe", measurementRecipeName }
                        }
                    }
                };

            Resource processResource = new Resource()
            {
                Name = processResourceName
            };
            processResource.Load();

            Resource processSubResource = new Resource
            {
                Name = processSubResourceName
            };
            processSubResource.Load();

            Resource measurementResource = new Resource
            {
                Name = measurementResourceName
            };
            measurementResource.Load();

            Resource measurementSubResource = new Resource
            {
                Name = measurementSubResourceName
            };
            measurementSubResource.Load();

            rollbackIsRecipeManagementEnabled.Add(processResource.Name, processResource.IsRecipeManagementEnabled.GetValueOrDefault());
            processResource.IsRecipeManagementEnabled = true;
            processResource.Save();

            rollbackIsRecipeManagementEnabled.Add(measurementResource.Name, measurementResource.IsRecipeManagementEnabled.GetValueOrDefault());
            measurementResource.IsRecipeManagementEnabled = true;
            measurementResource.Save();

            scenario.Setup();

            // Create 3 wafers for the first logicar wafer and set BusinessPartner
            Material lot = scenario.GeneratedLots.FirstOrDefault();
            lot.SaveAttribute(amsOSRAMConstants.MaterialAttributeSAPOwner, materialSapOwner);
            lot.LoadChildren();

            Material firstLogicalWafer = lot.SubMaterials[0];
            (Material firstSubstrate, firstLogicalWafer) = scenario.GenerateWafer(parentMaterial: firstLogicalWafer, type: amsOSRAMConstants.MaterialWaferSubstrateType);
            (Material firstCrystal, firstLogicalWafer) = scenario.GenerateWafer(parentMaterial: firstLogicalWafer, type: amsOSRAMConstants.MaterialWaferCrystalType);
            (Material firstCarrier, firstLogicalWafer) = scenario.GenerateWafer(parentMaterial: firstLogicalWafer, type: amsOSRAMConstants.MaterialWaferCarrierType);

            scenario.GenerateContainer(subMaterials: new MaterialCollection { firstLogicalWafer });
            firstLogicalWafer.Load();

            BusinessPartner businessPartner = new BusinessPartner();
            businessPartner.Name = vendor;
            businessPartner.Load();
            firstSubstrate.Supplier = businessPartner;
            firstSubstrate.Save();

            // Create only substrate for the second logical wafer
            Material secondLogicalWafer = lot.SubMaterials[1];
            (Material secondSubstrate, secondLogicalWafer) = scenario.GenerateWafer(parentMaterial: secondLogicalWafer, type: amsOSRAMConstants.MaterialWaferSubstrateType);

            lot = PerformMaterialProcessToMeasurement(lot, processResource, processSubResource, measurementResource, measurementSubResource);

            DataCollection dataCollection = new DataCollection
            {
                Name = dataCollectionName
            };
            dataCollection.Load();

            DataCollectionLimitSet dataCollectionLimitSet = new DataCollectionLimitSet
            {
                Name = dataCollectionLimitSetName
            };
            dataCollectionLimitSet.Load();

            DataCollectionPointCollection pointsToPost = CreateDataCollectionPointCollection(dataCollection, lot, dataCollectionLimitSet);

            string message = PostDataCollectionAndValidateSpaceMessage(lot, dataCollection, pointsToPost, dataCollectionLimitSet);

            ValidateMessage(message, lot, pointsToPost, dataCollection, logicalWaferQuantity, facilityName, productName,
                dataCollectionLimitSet: dataCollectionLimitSet,
                materialSapOwner: materialSapOwner,
                lastRecipeName: processRecipeName,
                currentRecipeName: measurementRecipeName,
                processResourceName: processResourceName,
                processSubResourceName: processSubResourceName,
                measurementResourceName: measurementResourceName,
                measurementSubResourceName: measurementSubResourceName);
        }

        /// <summary>
        /// Performs the steps of Processing a Material on the Process flow on a given Material
        /// </summary>
        /// <param name="lot"></param>
        /// <returns>Material with Process steps performed</returns>
        public static Material PerformMaterialProcessToMeasurement(Material lot, Resource processResource, Resource processSubResource, Resource measurementResource, Resource measurementSubResource)
        {
            lot.Load();

            lot.ComplexDispatchAndTrackIn(processResource);

            lot.LoadChildren();
            foreach (Material logicalWafer in lot.SubMaterials)
            {
                logicalWafer.ComplexTrackIn(processSubResource);
                logicalWafer.ComplexTrackOutMaterial();
                processSubResource.Load();
            }

            lot.ComplexTrackOutAndMoveNext();

            lot = lot.ComplexDispatchAndTrackIn(measurementResource).Material;
            lot.LoadChildren();

            foreach (Material logicalWafer in lot.SubMaterials)
            {
                logicalWafer.ComplexTrackIn(measurementSubResource);
                logicalWafer.ComplexTrackOutMaterial();
                measurementSubResource.Load();
            }

            lot.LoadChildren();

            return lot;
        }

        /// <summary>
        /// Generates a Collection of DataCollectionPoint using a given DataCollection and DataCollectionLimitSet to be performed on a given Material. If has the possibility to generate inside or outside the limits
        /// </summary>
        /// <param name="dataCollection"></param>
        /// <param name="dataCollectionLimitSet"></param>
        /// <param name="material"></param>
        /// <param name="insideLimits"></param>
        /// <returns>Collection of DataCollectionPoint</returns>
        public static DataCollectionPointCollection CreateDataCollectionPointCollection(DataCollection dataCollection, Material material, DataCollectionLimitSet dataCollectionLimitSet = null, bool insideLimits = true)
        {
            DataCollectionPointCollection pointsToPost = new DataCollectionPointCollection();

            dataCollection.LoadRelation("DataCollectionParameter");

            ParameterCollection parameters = new ParameterCollection();
            parameters.AddRange(dataCollection.DataCollectionParameters.Select(s => s.TargetEntity));
            parameters.Load();

            if (dataCollectionLimitSet != null)
            {
                dataCollectionLimitSet.LoadRelation("DataCollectionParameterLimit");
            }

            decimal defaultValue = 640;

            foreach (Parameter parameter in parameters)
            {
                parameter.LoadAttributes(new Collection<string> { amsOSRAMConstants.ParameterAttributeSendToSpace });

                DataCollectionParameterLimit parameterLimit = dataCollectionLimitSet?.DataCollectionParameterLimits?.FirstOrDefault(f => f.TargetEntity.Id == parameter.Id);

                decimal insideLimitValue = parameterLimit != null && parameterLimit.MinValue.HasValue ? parameterLimit.MinValue.Value + 1 : defaultValue;
                decimal outsideLimitValue = parameterLimit != null && parameterLimit.MaxValue.HasValue ? parameterLimit.MaxValue.Value + 1 : defaultValue;

                decimal pointValue = insideLimits ? insideLimitValue : outsideLimitValue;

                DataCollectionParameter dcParameter = dataCollection.DataCollectionParameters.FirstOrDefault(f => f.TargetEntity.Id == parameter.Id);
                DataCollectionParameterSampleKey? sampleKey = dcParameter.SampleKey;

                if (sampleKey == DataCollectionParameterSampleKey.MaterialId)
                {
                    Dictionary<string, List<decimal>> waferValues = new Dictionary<string, List<decimal>>();

                    foreach (Material wafer in material.SubMaterials)
                    {
                        for (int readingNumber = 1; dcParameter.MaximumSampleReadings >= readingNumber; readingNumber++)
                        {
                            DataCollectionPoint collectionPoint = new DataCollectionPoint();
                            collectionPoint.Value = pointValue;
                            collectionPoint.TargetEntity = parameter;
                            collectionPoint.ReadingNumber = readingNumber;
                            collectionPoint.SampleId = wafer.Name;
                            pointsToPost.Add(collectionPoint);
                        }
                    }
                }
                else
                {
                    Dictionary<string, List<decimal>> waferValues = new Dictionary<string, List<decimal>>();

                    for (int sampleId = 1; dcParameter.MaximumSamples >= sampleId; sampleId++)
                    {
                        for (int readingNumber = 1; dcParameter.MaximumSampleReadings >= readingNumber; readingNumber++)
                        {
                            DataCollectionPoint collectionPoint = new DataCollectionPoint();
                            collectionPoint.Value = pointValue;
                            collectionPoint.TargetEntity = parameter;
                            collectionPoint.ReadingNumber = 1;
                            collectionPoint.SampleId = $"Sample {sampleId}";
                            pointsToPost.Add(collectionPoint);
                        }
                    }
                }
            }

            return pointsToPost;
        }

        /// <summary>Post DataCollection and validate Space Message</summary>
        /// <param name="material"></param>
        /// <param name="dataCollection">DataCollection</param>
        /// <param name="dataCollectionLimitSet">DataCollection Limit Set</param>
        /// <param name="pointsToPost">Points to Post</param>
        /// <returns>Message sent to message bus</returns>
        private string PostDataCollectionAndValidateSpaceMessage(Material material, DataCollection dataCollection, DataCollectionPointCollection pointsToPost, DataCollectionLimitSet dataCollectionLimitSet = null)
        {
            Func<bool> waitForMessageBus = SubscribeMessageBus(amsOSRAMConstants.CustomReportEDCToSpace, 1);

            ComplexPerformDataCollectionOutput complexPostDCDataOutput = new ComplexPerformDataCollectionInput()
            {
                Material = material,
                DataCollection = dataCollection,
                DataCollectionLimitSet = dataCollectionLimitSet,
                DataCollectionPointCollection = pointsToPost
            }.ComplexPerformDataCollectionSync();

            waitForMessageBus.WaitFor();

            Dictionary<string, object> messageBusMessage = messageBusMessages.FirstOrDefault();
            Assert.IsTrue(messageBusMessage != null && messageBusMessage.Count > 0, "Message was not received from message bus.");

            object message;
            messageBusMessage.TryGetValue("Message", out message);

            return (string)message;
        }

        /// <summary>Validate message received from Message Bus</summary>
        /// <param name="message">Message to validate</param>
        /// <param name="material"></param>
        /// <param name="pointsToPost"></param>
        /// <param name="dataCollection"></param>
        /// <param name="dataCollectionLimitSet"></param>
        /// <param name="logicalWaferQuantity"></param>
        /// <param name="facilityName"></param>
        /// <param name="lastRecipeName"></param>
        /// <param name="currentRecipeName"></param>
        /// <param name="processResourceName"></param>
        /// <param name="processSubResourceName"></param>
        /// <param name="measurementResourceName"></param>
        /// <param name="measurementSubResourceName"></param>
        private void ValidateMessage(string message,
                                     Material material,
                                     DataCollectionPointCollection pointsToPost,
                                     DataCollection dataCollection,
                                     int logicalWaferQuantity,
                                     string facilityName,
                                     string productName,
                                     DataCollectionLimitSet dataCollectionLimitSet = null,
                                     string materialSapOwner = "-",
                                     string lastRecipeName = "-",
                                     string currentRecipeName = "-",
                                     string processResourceName = "",
                                     string processSubResourceName = "-",
                                     string measurementResourceName = "-",
                                     string measurementSubResourceName = "-"
            )
        {
            Assert.IsFalse(String.IsNullOrEmpty(message), "Message received from MessageBus cannot be null or empty");

            material.Load(1);

            Resource processEquipment = null;

            if (!String.IsNullOrEmpty(processResourceName))
            {
                processEquipment = new Resource
                {
                    Name = processResourceName
                };
                processEquipment.Load();
            }

            Product product = new Product
            {
                Name = productName
            };
            product.Load(1);

            if (material.HasAttributeDefined(amsOSRAMConstants.MaterialAttributeSAPOwner))
            {
                materialSapOwner = (string)material.Attributes[amsOSRAMConstants.MaterialAttributeSAPOwner];
            }

            Area area = material.GetArea();

            string ldCode = String.Empty;
            if (area.HasAttributeDefined(amsOSRAMConstants.AreaAttributeLdsId))
            {
                ldCode = (string)area.Attributes[amsOSRAMConstants.AreaAttributeLdsId];
            }

            string stepLogicalName = material.Step.Name;
            if (material.Step.ContainsLogicalNames)
            {
                material.Flow.LoadRelation("FlowStep");
                stepLogicalName = material.Flow.FlowSteps.FirstOrDefault(w => w.TargetEntity.Id == material.Step.Id).LogicalName;
            }

            Foundation.Security.User currentUser = new GetCurrentUserInput().GetCurrentUserSync().User;
            Employee employee = new Employee();
            employee.Name = currentUser.UserName;

            ShiftDefinitionShift shift = null;

            if (employee.ObjectExists())
            {
                employee.Load();
                employee.Calendar.Load();
                employee.Calendar.LoadRelations();

                shift = employee.Calendar.GetShiftDefinitionShift();
            }

            Dictionary<string, string> materialExpectedKeys = new Dictionary<string, string>()
            {
                {"LOT",  material.Name},
                {"BATCH", "-"},
                {"LOT TYPE", materialSapOwner },
                {"PROCESS EQUIPMENT 1", processEquipment?.Name },
                {"EQUIPMENT PLATFORM", processEquipment?.Model },
                {"PROCESS RECIPE 1", lastRecipeName },
                {"MEASUREMENT EQUIPMENT", measurementResourceName },
                {"MEASUREMENT RECIPE", currentRecipeName },
                {"QUANTITY", logicalWaferQuantity.ToString() },
                {"ACCESSORY", "-" },
                {"OPERATOR", employee.EmployeeNumber },
                {"SHIFT", shift?.Name ?? "-" },
                {"SENDER", "CMF" },
                {"AREA", facilityName },
                {"WILDCARD DA1", "-" },
                {"WILDCARD DA2", "-" },
            };

            CustomReportEDCToSpace spaceInformation = CustomUtilities.DeserializeXmlToObject<CustomReportEDCToSpace>(message);

            Dictionary<string, List<decimal>> listPointsPerParameterSamples = new Dictionary<string, List<decimal>>();

            ParameterCollection parameters = new ParameterCollection();
            parameters.AddRange(pointsToPost
                .Select(s =>
                {
                    s.TargetEntity.LoadAttribute(amsOSRAMConstants.ParameterAttributeSendToSpace);
                    return s.TargetEntity;
                })
                .Where(p =>
                    p.GetAttributeValue(amsOSRAMConstants.ParameterAttributeSendToSpace, false) &&
                    (p.DataType == ParameterDataType.Decimal || p.DataType == ParameterDataType.Long)
                ));

            foreach (DataCollectionPoint point in pointsToPost)
            {
                Parameter parameter = parameters.FirstOrDefault(s => s.Id == point.TargetEntity.Id);

                if (parameter != null)
                {
                    bool isSampleTypeMaterialId = dataCollection.DataCollectionParameters.FirstOrDefault(f => f.TargetEntity.Name == parameter.Name).SampleKey == DataCollectionParameterSampleKey.MaterialId;

                    string key = parameter.Name + "-" + (isSampleTypeMaterialId ? point.SampleId : $"{point.SampleId}");
                    decimal value = (decimal)point.Value;

                    if (listPointsPerParameterSamples.ContainsKey(key))
                    {
                        listPointsPerParameterSamples[key].Add(value);
                    }
                    else
                    {
                        listPointsPerParameterSamples.Add(key, new List<decimal> { value });
                    }
                }
            }

            Assert.AreEqual(listPointsPerParameterSamples.Count, spaceInformation.Samples.Count, $"Should have {listPointsPerParameterSamples.Count} samples but got {spaceInformation.Samples.Count} instead");

            // Validate Sender
            Assert.AreEqual(CustomUtilities.GetEnvironmentName(), spaceInformation.Sender.Value, $"The Sender should be {CustomUtilities.GetEnvironmentName()} instead of {spaceInformation.Sender.Value}");

            // Validate LDS
            Assert.AreEqual(1, spaceInformation.Lds.Count, $"Should have 1 LD instead of {spaceInformation.Lds.Count}");
            Assert.AreEqual(ldCode, spaceInformation.Lds.FirstOrDefault().Id, $"The LDSCode should be {ldCode} instead of {spaceInformation.Lds.FirstOrDefault().Id}");

            // Validate SampleDate
            Assert.IsNotNull(spaceInformation.SampleDate, "The SampleDate cannot be null or empty");

            if (dataCollectionLimitSet != null)
            {
                dataCollectionLimitSet.LoadRelations(new Collection<string> { "DataCollectionParameterLimit" }, 1);
            }

            material.LoadChildren();

            foreach (Sample sample in spaceInformation.Samples)
            {
                Parameter parameter = parameters.FirstOrDefault(f => f.Name == sample.ParameterName);

                if (!String.IsNullOrEmpty(parameter.DataUnit))
                {
                    Assert.AreEqual(parameter.DataUnit, sample.ParameterUnit, $"Sample field for parameter {sample.ParameterName} should have the unit {parameter.DataUnit} but got {sample.ParameterUnit} instead.");
                }

                DataCollectionParameterLimit limits = dataCollectionLimitSet?.DataCollectionParameterLimits?.FirstOrDefault(f => f.TargetEntity.Name == sample.ParameterName);

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
                } else
                {
                    Assert.IsNull(sample.SpecificationLimits.Upper);
                    Assert.IsNull(sample.SpecificationLimits.Lower);
                }

                bool isSampleTypeMaterialId = dataCollection.DataCollectionParameters.FirstOrDefault(f => f.TargetEntity.Name == parameter.Name).SampleKey == DataCollectionParameterSampleKey.MaterialId;
                string sampleName = isSampleTypeMaterialId ? sample.Keys.FirstOrDefault(f => f.Name == "WAFER").Value : sample.Keys.FirstOrDefault(f => f.Name == "WILDCARD EX1").Value;

                List<decimal> dcPoints = listPointsPerParameterSamples[parameter.Name + "-" + sampleName];

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
                        Name = sampleName
                    };

                    logicalWafer.Load(1);
                    logicalWafer.LoadRelation("MaterialContainer");
                    waferContainer = logicalWafer.MaterialContainer?.FirstOrDefault(f => f.SourceEntity.Id == logicalWafer.Id);

                    logicalWafer.LoadChildren();
                }

                Material substrate = logicalWafer?.SubMaterials?.FirstOrDefault(f => f.Form == amsOSRAMConstants.FormWafer && f.Type == amsOSRAMConstants.MaterialWaferSubstrateType);

                if (substrate?.Supplier != null)
                {
                    substrate.Supplier.Load();
                }

                Dictionary<string, string> waferExpectedKeys = new Dictionary<string, string>(materialExpectedKeys) {
                    { "WAFER",  logicalWafer?.Name ?? "-"},
                    { "PRODUCT", logicalWafer?.Product?.Name ?? product.Name },
                    { "PRODUCT VERSION", logicalWafer?.Product?.Version.ToString() ?? product.Version.ToString() },
                    { "PRODUCT TECHNOLOGY", logicalWafer?.Product.ProductGroup?.Name },
                    { "POSITION 1", waferContainer?.Position.Value.ToString() ?? "-" },
                    { "POSITION 2", "-" },
                    { "FLOW", logicalWafer?.Flow?.Name ?? material.Flow.Name },
                    { "SINGLE PROCESS", stepLogicalName },
                    { "PROCESS EQUIPMENT CHAMBER", logicalWafer == null ? "-" : processSubResourceName ?? "-" },
                    { "MEASUREMENT EQUIPMENT CHAMBER", logicalWafer?.SystemState == MaterialSystemState.Processed ? measurementSubResourceName ?? "-" : "-" },
                    { "WILDCARD EX1", logicalWafer == null ? sampleName : "-" },
                    { "WILDCARD EX2", "-" },
                    { "CRYSTAL", logicalWafer?.SubMaterials?.FirstOrDefault(f => f.Form == amsOSRAMConstants.FormWafer && f.Type == amsOSRAMConstants.MaterialWaferCrystalType)?.Name ?? "-" },
                    { "SUBSTRATE", substrate?.Name ?? "-" },
                    { "CARRIER", logicalWafer?.SubMaterials?.FirstOrDefault(f => f.Form == amsOSRAMConstants.FormWafer && f.Type == amsOSRAMConstants.MaterialWaferCarrierType)?.Name ?? "-" },
                    { "VENDOR", substrate?.Supplier?.Name ?? "-" }
                };

                foreach (Key key in sample.Keys)
                {
                    if (!string.IsNullOrEmpty(waferExpectedKeys[key.Name]))
                    {
                        Assert.AreEqual(waferExpectedKeys[key.Name], key.Value, $"Sample field for parameter {sample.ParameterName} should have the key with name {key.Name} with the value {waferExpectedKeys[key.Name]}, but got {key.Value} instead.");
                    }
                }
            }
        }

        /// <summary>Subscribes the message bus.</summary>
        /// <param name="topic">The topic.</param>
        /// <param name="numberOfMessages">The number of messages.</param>
        /// <returns>Function to wait for response</returns>
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
