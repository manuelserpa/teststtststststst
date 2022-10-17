using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Tibco;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.ImportExportManagement.OutputObjects;
using Cmf.Foundation.Common.Base;
using Cmf.MessageBus.Client;
using Cmf.MessageBus.Messages;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.ContainerManagement.OutputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.TestScenarios.Others;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Cmf.Custom.Tests.Biz.Tibco
{
    [TestClass]
    public class CustomSendEventMessage
    {
        private static CustomExecutionScenario _classScenario;
        private CustomExecutionScenario _scenario;
        private List<TibcoCustomSendEventMessage> _tibcoCustomSendEventMessages;
        private static Transport _transport;
        private string isToUnsubscribeTopic;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _transport = new Transport(BaseContext.GetMessageBusTransportConfiguration());
            _transport.Start();

            LookupTable lookupTable = new LookupTable();
            lookupTable.Name = amsOSRAMConstants.CustomTransactionsLookupTable;

            lookupTable.Load();

            _classScenario = new CustomExecutionScenario();
            
            foreach (LookupTableValue lookupTableValue in lookupTable.Values)
            {
                _classScenario.GenericTableManager.SetGenericTableData(amsOSRAMConstants.GenericTableCustomTransactionsToTibco, new Dictionary<string, object>
                {
                    { amsOSRAMConstants.GenericTableCustomTransactionsToTibcoTransactionProperty, lookupTableValue.Value },
                    { amsOSRAMConstants.GenericTableCustomTransactionsToTibcoIsEnabledProperty, true },
                });
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (_transport != null)
            {
                _transport.Stop();
            }

            if (_classScenario != null)
            {
                _classScenario.GenericTableManager.TearDown();
            }
        }

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            _scenario = new CustomExecutionScenario();
            _tibcoCustomSendEventMessages = new List<TibcoCustomSendEventMessage>();
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (_scenario != null)
            {
                _scenario.CompleteCleanUp();
            }

            if (isToUnsubscribeTopic != null)
            {
                _transport.Unsubscribe(isToUnsubscribeTopic);
            }

            if (_tibcoCustomSendEventMessages.Count > 0)
            {
                _tibcoCustomSendEventMessages.Clear();
            }
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when a material is created with submaterials
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_SubMaterial</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_SubMaterial()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.NumberOfChildMaterialsToGenerate = 1;
            _scenario.Setup();

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();
            material.SaveAttribute("GoodsReceiptNo", "TestGoodsReceiptNo");

            material.Load();
            material.LoadChildren();

            Assert.AreEqual(1, material.SubMaterials.Count, "Should have been created the Material with one SubMaterial");

            Material subMaterial = material.SubMaterials.FirstOrDefault();

            // TODO: To be tested with the product fix after MES upgrade to 9.1
            // It is missing exporting attributes on the SubMaterials
            //subMaterial.SaveAttribute("GoodsReceiptNo", "TestSubMaterialGoodsReceiptNo");

            Resource resource = new Resource
            {
                Name = amsOSRAMConstants.DefaultTestResourceName
            };
            resource.Load();

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange);

            material.Dispatch(resource);

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessage.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessage.Message == null || tibcoCustomSendEventMessage.Message == String.Empty, $"The Message should not be null or empty");

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialDispatch);
            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(material)), XDocument.Parse(tibcoCustomSendEventMessage.Message));
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when creating a material
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_MaterialCreate</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_MaterialCreate()
        {
            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange);

            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.Setup();

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessage.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessage.Message == null || tibcoCustomSendEventMessage.Message == String.Empty, $"The Message should not be null or empty");

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialCreate);
            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(material)), XDocument.Parse(tibcoCustomSendEventMessage.Message));
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when terminating a material
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_MaterialTerminate</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_MaterialTerminate()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.Setup();

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange);

            material.SpecialTerminate();

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessage.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessage.Message == null || tibcoCustomSendEventMessage.Message == String.Empty, $"The Message should not be null or empty");

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialTerminate);

            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(material)), XDocument.Parse(tibcoCustomSendEventMessage.Message));
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when dispatching a material
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_MaterialDispatch</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_MaterialDispatch()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.Setup();

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();

            Resource resource = new Resource
            {
                Name = amsOSRAMConstants.DefaultTestResourceName
            };
            resource.Load();

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange);

            material.Dispatch(resource);

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessage.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessage.Message == null || tibcoCustomSendEventMessage.Message == String.Empty, $"The Message should not be null or empty");

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialDispatch);
            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(material)), XDocument.Parse(tibcoCustomSendEventMessage.Message));
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when tracking in a material
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_MaterialTrackIn</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_MaterialTrackIn()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.Setup();

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();

            Resource resource = new Resource
            {
                Name = amsOSRAMConstants.DefaultTestResourceName
            };
            resource.Load();

            material.Dispatch(resource);

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange);

            material.TrackIn();

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessage.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessage.Message == null || tibcoCustomSendEventMessage.Message == String.Empty, $"The Message should not be null or empty");

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialTrackIn);
            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(material)), XDocument.Parse(tibcoCustomSendEventMessage.Message));
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when tracking out a material
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_MaterialTrackOut</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_MaterialTrackOut()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.Setup();

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();

            Resource resource = new Resource
            {
                Name = amsOSRAMConstants.DefaultTestResourceName
            };
            resource.Load();

            material.Dispatch(resource);
            material.TrackIn(resource);

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange);

            material.TrackOut();

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessage.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessage.Message == null || tibcoCustomSendEventMessage.Message == String.Empty, $"The Message should not be null or empty");

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialTrackOut);
            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(material)), XDocument.Parse(tibcoCustomSendEventMessage.Message));
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when moving next a material
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_MaterialMoveNext</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_MaterialMoveNext()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.Setup();

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();

            Resource resource = new Resource
            {
                Name = amsOSRAMConstants.DefaultTestResourceName
            };
            resource.Load();

            material.Dispatch(resource);
            material.TrackIn(resource);
            material.TrackOut();

            string previousPath = GetMessageHeaderComposedPath(material);

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange);

            material.ComplexMoveNext();

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessage.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessage.Message == null || tibcoCustomSendEventMessage.Message == String.Empty, $"The Message should not be null or empty");

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialMoveNext, previousPath);
            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(material)), XDocument.Parse(tibcoCustomSendEventMessage.Message));
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when spliting a material
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_MaterialSplit</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_MaterialSplit()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.Setup();

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange, 2);

            MaterialCollection splitedMaterials = material.Split(new SplitInputParametersCollection
            {
                new SplitInputParameters
                {
                    PrimaryQuantity = 1,
                    MaterialContainer = null
                }
            });

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessageSplit = _tibcoCustomSendEventMessages.FirstOrDefault(f => f.Header.stdTransaction == CustomTransactionTypes.MaterialSplit.ToString());
            TibcoCustomSendEventMessage tibcoCustomSendEventMessageTerminate = _tibcoCustomSendEventMessages.FirstOrDefault(f => f.Header.stdTransaction == CustomTransactionTypes.MaterialTerminate.ToString());

            Assert.IsNotNull(tibcoCustomSendEventMessageSplit, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessageSplit.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessageSplit.Message == null || tibcoCustomSendEventMessageSplit.Message == String.Empty, $"The Message should not be null or empty");

            Assert.IsNotNull(tibcoCustomSendEventMessageTerminate, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessageTerminate.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessageTerminate.Message == null || tibcoCustomSendEventMessageTerminate.Message == String.Empty, $"The Message should not be null or empty");

            ValiteHeaderMessage(material, tibcoCustomSendEventMessageSplit, CustomTransactionTypes.MaterialSplit);
            ValiteHeaderMessage(material, tibcoCustomSendEventMessageTerminate, CustomTransactionTypes.MaterialTerminate);

            XDocument mesDocument = XDocument.Parse(GetExportedObjectOfMaterial(material));

            ValidateXML(mesDocument, XDocument.Parse(tibcoCustomSendEventMessageSplit.Message));
            ValidateXML(mesDocument, XDocument.Parse(tibcoCustomSendEventMessageTerminate.Message));
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when merging a material
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_MaterialMerge</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_MaterialMerge()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 2;
            _scenario.ScenarioQuantity = 1;
            _scenario.Setup();

            // Material created
            Material materialMerged = _scenario.GeneratedLots.FirstOrDefault();
            Material materialTerminated = _scenario.GeneratedLots.LastOrDefault();

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange, 2);

            materialMerged.Merge(new MaterialCollection
            {
                materialTerminated
            });

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessageMerged = _tibcoCustomSendEventMessages.FirstOrDefault(f => f.Header.stdTransaction == CustomTransactionTypes.MaterialMerge.ToString());
            TibcoCustomSendEventMessage tibcoCustomSendEventMessageTerminated = _tibcoCustomSendEventMessages.FirstOrDefault(f => f.Header.stdTransaction == CustomTransactionTypes.MaterialTerminate.ToString());

            Assert.IsNotNull(tibcoCustomSendEventMessageMerged, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessageMerged.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessageMerged.Message == null || tibcoCustomSendEventMessageMerged.Message == String.Empty, $"The Message should not be null or empty");

            Assert.IsNotNull(tibcoCustomSendEventMessageTerminated, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessageTerminated.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessageTerminated.Message == null || tibcoCustomSendEventMessageTerminated.Message == String.Empty, $"The Message should not be null or empty");

            ValiteHeaderMessage(materialMerged, tibcoCustomSendEventMessageMerged, CustomTransactionTypes.MaterialMerge);
            ValiteHeaderMessage(materialTerminated, tibcoCustomSendEventMessageTerminated, CustomTransactionTypes.MaterialTerminate);

            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(materialMerged)), XDocument.Parse(tibcoCustomSendEventMessageMerged.Message));
            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(materialTerminated)), XDocument.Parse(tibcoCustomSendEventMessageTerminated.Message));
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when setting a material loss
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_MaterialLoss</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_MaterialLoss()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.Setup();

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();

            Reason reason = new Reason() { Name = "TestLossReason" };
            reason.Load();

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange);

            material.RecordLoss(material.PrimaryQuantity.Value, reasonToUse: reason);

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessage.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessage.Message == null || tibcoCustomSendEventMessage.Message == String.Empty, $"The Message should not be null or empty");

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialLoss);
            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(material)), XDocument.Parse(tibcoCustomSendEventMessage.Message));
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when setting a material bonus
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_MaterialBonus</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_MaterialBonus()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.Setup();

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();

            Reason reason = new Reason() { Name = "Bonus" };
            reason.Load();

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange);

            material.RecordBonus(material.PrimaryQuantity.Value, reasonToUse: reason);

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessage.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessage.Message == null || tibcoCustomSendEventMessage.Message == String.Empty, $"The Message should not be null or empty");

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialBonus);
            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(material)), XDocument.Parse(tibcoCustomSendEventMessage.Message));
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when holding a material
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_MaterialHold</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_MaterialHold()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.Setup();

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();

            Reason reason = new Reason() { Name = "Out Of Spec" };
            reason.Load();

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange);

            material = material.HoldMaterial(reason);

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessage.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessage.Message == null || tibcoCustomSendEventMessage.Message == String.Empty, $"The Message should not be null or empty");

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialHold);
            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(material)), XDocument.Parse(tibcoCustomSendEventMessage.Message));
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when releasing a material
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_MaterialRelease</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_MaterialRelease()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.Setup();

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();

            Reason reason = new Reason() { Name = "Out Of Spec" };
            reason.Load();

            material = material.HoldMaterial(reason);
            material.LoadRelation("MaterialHoldReason");

            EntityRelationCollection materialHoldReasonsEntity = material.RelationCollection["MaterialHoldReason"];
            MaterialHoldReasonCollection materialHoldReasons = new MaterialHoldReasonCollection();

            foreach (MaterialHoldReason materialHoldReason in materialHoldReasons)
            {
                materialHoldReasons.Add(materialHoldReason);
            }

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange);

            new ReleaseMaterialInput()
            {
                Material = material,
                MaterialHoldReasonCollection = materialHoldReasons
            }.ReleaseMaterialSync();

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessage.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessage.Message == null || tibcoCustomSendEventMessage.Message == String.Empty, $"The Message should not be null or empty");

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialRelease);
            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(material)), XDocument.Parse(tibcoCustomSendEventMessage.Message));
        }

        /// <summary>
        /// Description:
        ///     - Validates Header and Body message sent to Tibco when addiing a material to a container
        ///
        /// Acceptance Citeria:
        ///     - Validate created message information
        ///
        /// </summary>
        /// <TestCaseID>CustomSendEventMessage_ValidateMessage_ContainerAssociation</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendEventMessage_ValidateMessage_ContainerAssociation()
        {
            ///<Step> Create a Lot and its wafers </Step>
            _scenario.NumberOfMaterialsToGenerate = 1;
            _scenario.NumberOfChildMaterialsToGenerate = 1;
            _scenario.Setup();

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();
            material.LoadChildren();

            Container container = GenericGetsScenario.GetObjectByName<Container>(amsOSRAMConstants.DefaultContainerName);
            container.Load();

            int position = 1;

            MaterialContainerCollection materialContainerCollection = new MaterialContainerCollection();
            Material childMaterial = new Material();
            foreach (Material subMaterial in material.SubMaterials)
            {
                MaterialContainer materialContainerRelation = new MaterialContainer()
                {
                    SourceEntity = subMaterial,
                    TargetEntity = container,
                    Position = position
                };
                position++;
                materialContainerCollection.Add(materialContainerRelation);
                childMaterial = subMaterial;
            }

            AssociateMaterialsWithContainerInput associateInput = new AssociateMaterialsWithContainerInput();
            associateInput.MaterialContainerRelations = materialContainerCollection;
            associateInput.Container = container;

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange);

            AssociateMaterialsWithContainerOutput associateOutput = associateInput.AssociateMaterialsWithContainerSync();

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");
            Assert.IsNotNull(tibcoCustomSendEventMessage.Header, $"The Header should not be null");
            Assert.IsFalse(tibcoCustomSendEventMessage.Message == null || tibcoCustomSendEventMessage.Message == String.Empty, $"The Message should not be null or empty");

            childMaterial.Load();

            ValiteHeaderMessage(childMaterial, tibcoCustomSendEventMessage, CustomTransactionTypes.ContainerAssociation);
            ValidateXML(XDocument.Parse(GetExportedObjectOfMaterial(childMaterial)), XDocument.Parse(tibcoCustomSendEventMessage.Message));
        }

        private void ValidateXML(XDocument document1, XDocument document2)
        {
            // List of ElementNames to discard on output XML
            List<string> xmlElementsToDiscard = new List<string>()
                {
                    "CurrentBOMAssemblyType",
                    "CurrentBOMVersion",
                    "LineFlowVersion",
                    "LineValidationMode",
                    "LineAssemblyMode",
                    "InhibitMoveFromStep",
                    "OverrideProductBlock",
                    "InhibitShip",
                    "InTransitFromState",
                    "InTransitType",
                    "IsInNonSequentialBlock",
                    "LocationAltitude",
                    "LocationLatitude",
                    "LocationLongitude",
                    "ResourceAssociationType",
                    "SplitMergeRestrictionType",
                    "NotificationCount",
                    "TimeConstraintsCount",
                    "CurrentSamplingPattern",
                    "SamplingSequence",
                    "RequiredFutureAction",
                    "MaterialTransferFromFacility",
                    "MaterialTransferCostCenter",
                    "MaximumAssembleDate",
                    "CurrentBOMTrackOutLossesMode",
                    "MaintenanceHoldCount",
                    "AccountsToProductionOrderQuantity",
                    "ExcludeFromScheduling",
                    "HasFromDependencies",
                    "HasToDependencies",
                    "CurrentBOMWeighAndDispenseMode",
                    "ExperimentMaterialGroupName",
                    "ExperimentSubMaterialNumber",
                    "TargetMaterialQuantity",
                    "TargetMaterialUnits",
                    "CurrentBOMUnits",
                    "PickListItemCount",
                    "IsInTransferOrderItem",
                    "MoistureSensitivityLevel",
                    "FloorLifeOpenDate",
                    "FloorLifeCounterState",
                    "FloorLifeRemainingHours",
                    "FloorLifeSealed",
                    "ManufacturerPartNumber",
                    "ManufacturerLotNumber",
                    "DateCode",
                    "CapacityClass",
                    "IsRoHSCompliant",
                    "IsApproved",
                    "RequiredResource",
                    "OpenInspectionOrderCount",
                    "OpenInspectionOrderStepSampleCount",
                    "PendingLineReworkReturn",
                    "InTransitToFacility",
                    "IsDispatchable",
                    "Priority",
                    "RequiredService",
                    "ShippingLabel",
                    "SynchronizeMapUnits",
                    "MasterMap",
                    "IsTemplate",
                    "RelationCollection",
                    "DocumentationURL",
                    "Image",
                    "DataGroupName"
                };

            // List of AttributeNames associated to the Elements to discard on output XML
            List<string> xmlAttributesToDiscard = new List<string>()
                {
                    "type",
                    "actualtype",
                    "ExportId"
                };

            // Remove Material discarded Elements
            document1.Root.Descendants().Where(e => xmlElementsToDiscard.Contains(e.Name.LocalName)).Remove();

            // Remove Material discarded Attributes
            document1.Root.Descendants().Attributes().Where(a => xmlAttributesToDiscard.Contains(a.Name.LocalName)).Remove();

            Assert.AreEqual(document1.ToString().Trim(), document2.ToString().Trim());
        }

        private string GetExportedObjectOfMaterial(Material material)
        {
            CoreBaseCollection materials = new CoreBaseCollection();

            material.LoadRelations();
            material.LoadMaterialOffFlows();
            material.LoadChildren();
            material.LoadAttributes();

            // TODO: To be tested with the product fix after MES upgrade to 9.1
            // It is missing exporting attributes on the SubMaterials

            //foreach (Material sub in material.SubMaterials)
            //{
            //    sub.LoadAttributes();
            //}

            materials.Add(material);

            ExportObjectsOutput output = new Cmf.Foundation.BusinessOrchestration.ImportExportManagement.InputObjects.ExportObjectsInput
            {
                Objects = materials
            }.ExportObjectsSync();

            Assert.IsNotNull(output.Xml, "XML response should not be null");
            Assert.IsFalse(String.Empty == output.Xml, "XML response should not be empty");

            return output.Xml;
        }

        private void ValiteHeaderMessage(Material material, TibcoCustomSendEventMessage message, CustomTransactionTypes customTransactionType, string previousPath = null)
        {
            material.Load();

            // stdObjectName
            Assert.AreEqual(message.Header.stdObjectName, material.Name, $"The Header message doesnt have the correct material. Should be {material.Name} instead of {message.Header.stdObjectName}");

            // stdProductType
            material.Product.Load();
            material.Product.LoadAttribute(amsOSRAMConstants.ProductAttributeSAPProductType);
            string expectedAttribute = (string)material.Product.Attributes.GetValueOrDefault(amsOSRAMConstants.ProductAttributeSAPProductType, String.Empty);
            Assert.AreEqual(message.Header.stdProductType, expectedAttribute, $"The Header message doesnt have the correct product type. Should be {expectedAttribute} instead of {message.Header.stdProductType}");

            // stdDataOrigin
            Assert.AreEqual(CustomUtilities.GetEnvironmentName(), message.Header.stdDataOrigin, $"The Header message doesnt have the correct data origin. Should be {Environment.MachineName} instead of {message.Header.stdDataOrigin}");

            // stdFrom  / stdTo
            string expectedPathTo = GetMessageHeaderComposedPath(material);
            string expectedPathFrom = previousPath ?? expectedPathTo;

            Assert.AreEqual(expectedPathFrom, message.Header.stdFrom, $"The Header message doesnt have the correct origin path. Should be {expectedPathFrom} instead of {message.Header.stdFrom}");
            Assert.AreEqual(expectedPathTo, message.Header.stdTo, $"The Header message doesnt have the correct destination path. Should be {expectedPathTo} instead of {message.Header.stdTo}");

            // stdTransaction
            Assert.AreEqual(message.Header.stdTransaction, customTransactionType.ToString(), $"The Header message doesnt have the correct transaction name. Should be {customTransactionType.ToString()} instead of {message.Header.stdTransaction}");
        }

        private string GetMessageHeaderComposedPath(Material material)
        {
            material.Facility.Load();
            material.Facility.LoadAttribute(amsOSRAMConstants.FacilityAttributeFacilityCode);

            string expectedFacilityCode = (string)material.Facility.Attributes.GetValueOrDefault(amsOSRAMConstants.FacilityAttributeFacilityCode, "EMPTY");
            string expectedSiteCode = "EMPTY";

            if (material.Facility.Site != null)
            {
                material.Facility.Site.Load();
                material.Facility.Site.LoadAttribute(amsOSRAMConstants.SiteAttributeSiteCode);
                expectedSiteCode = (string)material.Facility.Site.Attributes.GetValueOrDefault(amsOSRAMConstants.SiteAttributeSiteCode, "EMPTY");
            }

            material.Step.Load();
            string expectedStepLogicalName = material.Step.Name;

            if (material.Step.ContainsLogicalNames)
            {
                material.Flow.Load();
                material.Flow.LoadRelation("FlowStep");

                FlowStep flowStep = material.Flow.FlowSteps.FirstOrDefault(f => f.TargetEntity.Id == material.Step.Id);
                expectedStepLogicalName = flowStep.LogicalName;
            }

            return $"{expectedSiteCode}.{expectedFacilityCode}.{expectedStepLogicalName}";
        }

        private Func<bool> SuscribeMessageBus(CustomSendEventMessageTopics topic, int numberOfMessages = 1)
        {
            _transport.Subscribe(topic.ToString(), (string subject, MbMessage message) =>
            {
                if (message != null && !string.IsNullOrWhiteSpace(message.Data))
                {
                    _tibcoCustomSendEventMessages.Add(JsonConvert.DeserializeObject<TibcoCustomSendEventMessage>(message.Data));
                }
            });

            isToUnsubscribeTopic = topic.ToString();

            Func<bool> waitForMessageBus = () =>
            {
                bool isDone = _tibcoCustomSendEventMessages.Count == numberOfMessages;

                if (isDone)
                {
                    _transport.Unsubscribe(topic.ToString());
                    isToUnsubscribeTopic = null;
                }

                return isDone;
            };

            return waitForMessageBus;
        }
    }
}
