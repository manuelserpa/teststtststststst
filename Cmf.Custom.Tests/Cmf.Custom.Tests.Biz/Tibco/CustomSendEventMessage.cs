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
using Cmf.TestScenarios.Others;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

public enum CustomSendEventMessageTopics
{
    [EnumMember(Value = "CustomLotChange")]
    CustomLotChange,
    [EnumMember(Value = "CustomEquipmentStatusChange")]
    CustomEquipmentStatusChange
}

namespace Cmf.Custom.Tests.Biz.Tibco
{
    [TestClass]
    public class CustomSendEventMessage
    {
        private CustomExecutionScenario _scenario;
        private List<TibcoCustomSendEventMessage> _tibcoCustomSendEventMessages;

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

            // Material created
            Material material = _scenario.GeneratedLots.FirstOrDefault();

            string xmlMES = GetExportedObjectOfMaterial(material);

            XDocument mesDocument = XDocument.Parse(xmlMES);
            XDocument messageBusDocument = XDocument.Parse(tibcoCustomSendEventMessage.Message);

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialCreate);
            ValidateXML(mesDocument.Root.Descendants("Object").Elements(), messageBusDocument.Root.Descendants("Object").Elements());
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
            
            material.Load();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");

            string xmlMES = GetExportedObjectOfMaterial(material);

            XDocument mesDocument = XDocument.Parse(xmlMES);
            XDocument messageBusDocument = XDocument.Parse(tibcoCustomSendEventMessage.Message);

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialTerminate);
            ValidateXML(mesDocument.Root.Descendants("Object").Elements(), messageBusDocument.Root.Descendants("Object").Elements());
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

            string xmlMES = GetExportedObjectOfMaterial(material);

            XDocument mesDocument = XDocument.Parse(xmlMES);
            XDocument messageBusDocument = XDocument.Parse(tibcoCustomSendEventMessage.Message);

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialDispatch);
            ValidateXML(mesDocument.Root.Descendants("Object").Elements(), messageBusDocument.Root.Descendants("Object").Elements());
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

            string xmlMES = GetExportedObjectOfMaterial(material);

            XDocument mesDocument = XDocument.Parse(xmlMES);
            XDocument messageBusDocument = XDocument.Parse(tibcoCustomSendEventMessage.Message);

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialTrackIn);
            ValidateXML(mesDocument.Root.Descendants("Object").Elements(), messageBusDocument.Root.Descendants("Object").Elements());
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

            string xmlMES = GetExportedObjectOfMaterial(material);

            XDocument mesDocument = XDocument.Parse(xmlMES);
            XDocument messageBusDocument = XDocument.Parse(tibcoCustomSendEventMessage.Message);

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialTrackOut);
            ValidateXML(mesDocument.Root.Descendants("Object").Elements(), messageBusDocument.Root.Descendants("Object").Elements());
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

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomLotChange);

            material.ComplexMoveNext();

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");

            material.Load();
            string xmlMES = GetExportedObjectOfMaterial(material);

            XDocument mesDocument = XDocument.Parse(xmlMES);
            XDocument messageBusDocument = XDocument.Parse(tibcoCustomSendEventMessage.Message);

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialMoveNext);
            ValidateXML(mesDocument.Root.Descendants("Object").Elements(), messageBusDocument.Root.Descendants("Object").Elements());
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
            Assert.IsNotNull(tibcoCustomSendEventMessageTerminate, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");

            string xmlMESSplit = GetExportedObjectOfMaterial(material);

            XDocument mesDocumentSplit = XDocument.Parse(xmlMESSplit);
            XDocument messageBusDocumentSplit = XDocument.Parse(tibcoCustomSendEventMessageSplit.Message);

            ValiteHeaderMessage(material, tibcoCustomSendEventMessageSplit, CustomTransactionTypes.MaterialSplit);
            ValidateXML(mesDocumentSplit.Root.Descendants("Object").Elements(), messageBusDocumentSplit.Root.Descendants("Object").Elements());

            string xmlMESTerminate = GetExportedObjectOfMaterial(material);

            XDocument mesDocumentTerminate = XDocument.Parse(xmlMESTerminate);
            XDocument messageBusDocumentTerminate = XDocument.Parse(tibcoCustomSendEventMessageTerminate.Message);

            ValiteHeaderMessage(material, tibcoCustomSendEventMessageTerminate, CustomTransactionTypes.MaterialTerminate);
            ValidateXML(mesDocumentTerminate.Root.Descendants("Object").Elements(), messageBusDocumentTerminate.Root.Descendants("Object").Elements());
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
            Assert.IsNotNull(tibcoCustomSendEventMessageTerminated, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");

            string xmlMESMerged = GetExportedObjectOfMaterial(materialMerged);

            XDocument mesDocumentMerged = XDocument.Parse(xmlMESMerged);
            XDocument messageBusDocumentMerged = XDocument.Parse(tibcoCustomSendEventMessageMerged.Message);

            ValiteHeaderMessage(materialMerged, tibcoCustomSendEventMessageMerged, CustomTransactionTypes.MaterialMerge);
            ValidateXML(mesDocumentMerged.Root.Descendants("Object").Elements(), messageBusDocumentMerged.Root.Descendants("Object").Elements());

            string xmlMESTerminated = GetExportedObjectOfMaterial(materialTerminated);

            XDocument mesDocumentTerminated = XDocument.Parse(xmlMESTerminated);
            XDocument messageBusDocumentMerge = XDocument.Parse(tibcoCustomSendEventMessageTerminated.Message);

            ValiteHeaderMessage(materialTerminated, tibcoCustomSendEventMessageTerminated, CustomTransactionTypes.MaterialTerminate);
            ValidateXML(mesDocumentTerminated.Root.Descendants("Object").Elements(), messageBusDocumentMerge.Root.Descendants("Object").Elements());
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

            string xmlMES = GetExportedObjectOfMaterial(material);

            XDocument mesDocument = XDocument.Parse(xmlMES);
            XDocument messageBusDocument = XDocument.Parse(tibcoCustomSendEventMessage.Message);

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialLoss);
            ValidateXML(mesDocument.Root.Descendants("Object").Elements(), messageBusDocument.Root.Descendants("Object").Elements());
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

            string xmlMES = GetExportedObjectOfMaterial(material);

            XDocument mesDocument = XDocument.Parse(xmlMES);
            XDocument messageBusDocument = XDocument.Parse(tibcoCustomSendEventMessage.Message);

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialBonus);
            ValidateXML(mesDocument.Root.Descendants("Object").Elements(), messageBusDocument.Root.Descendants("Object").Elements());
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

            string xmlMES = GetExportedObjectOfMaterial(material);

            XDocument mesDocument = XDocument.Parse(xmlMES);
            XDocument messageBusDocument = XDocument.Parse(tibcoCustomSendEventMessage.Message);

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialHold);
            ValidateXML(mesDocument.Root.Descendants("Object").Elements(), messageBusDocument.Root.Descendants("Object").Elements());
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

            new Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects.ReleaseMaterialInput()
            {
                Material = material,
                MaterialHoldReasonCollection = materialHoldReasons
            }.ReleaseMaterialSync();

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomLotChange}");

            string xmlMES = GetExportedObjectOfMaterial(material);

            XDocument mesDocument = XDocument.Parse(xmlMES);
            XDocument messageBusDocument = XDocument.Parse(tibcoCustomSendEventMessage.Message);

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.MaterialRelease);
            ValidateXML(mesDocument.Root.Descendants("Object").Elements(), messageBusDocument.Root.Descendants("Object").Elements());
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
            }

            AssociateMaterialsWithContainerInput associateInput = new AssociateMaterialsWithContainerInput();
            associateInput.MaterialContainerRelations = materialContainerCollection;
            associateInput.Container = container;

            Func<bool> waitForMessageBus = SuscribeMessageBus(CustomSendEventMessageTopics.CustomEquipmentStatusChange);

            AssociateMaterialsWithContainerOutput associateOutput = associateInput.AssociateMaterialsWithContainerSync();

            waitForMessageBus.WaitFor();

            TibcoCustomSendEventMessage tibcoCustomSendEventMessage = _tibcoCustomSendEventMessages.FirstOrDefault();

            Assert.IsNotNull(tibcoCustomSendEventMessage, $"No Message received from MessageBus for the topic {CustomSendEventMessageTopics.CustomEquipmentStatusChange}");

            string xmlMES = GetExportedObjectOfMaterial(material);

            XDocument mesDocument = XDocument.Parse(xmlMES);
            XDocument messageBusDocument = XDocument.Parse(tibcoCustomSendEventMessage.Message);

            ValiteHeaderMessage(material, tibcoCustomSendEventMessage, CustomTransactionTypes.ContainerAssociation);
            ValidateXML(mesDocument.Root.Descendants("Object").Elements(), messageBusDocument.Root.Descendants("Object").Elements());
        }

        private string GetExportedObjectOfMaterial(Material material)
        {
            Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.OutputObjects.GetObjectByNameOutput o = new Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects.GetObjectByNameInput
            {
                Type = "Material",
                Name = material.Name
            }.GetObjectByNameSync();

            CoreBaseCollection materials = new CoreBaseCollection();
            materials.Add((CoreBase)o.Instance);

            ExportObjectsOutput output = new Cmf.Foundation.BusinessOrchestration.ImportExportManagement.InputObjects.ExportObjectsInput
            {
                Objects = materials
            }.ExportObjectsSync();

            Assert.IsNotNull(output.Xml, "XML response should not be null");
            Assert.IsFalse(String.Empty == output.Xml, "XML response should not be empty");

            return output.Xml;
        }

        private void ValiteHeaderMessage(Material material, TibcoCustomSendEventMessage message, CustomTransactionTypes customTransactionType)
        {
            // stdObjectName
            Assert.AreEqual(message.Header.stdObjectName, material.Name, $"The Header message doesnt have the correct material. Should be {material.Name} instead of {message.Header.stdObjectName}");

            // stdProductType
            material.Product.LoadAttribute(amsOSRAMConstants.ProductAttributeSAPProductType);
            string expectedAttribute = (string)material.Product.Attributes.GetValueOrDefault(amsOSRAMConstants.ProductAttributeSAPProductType, String.Empty);
            Assert.AreEqual(message.Header.stdProductType, expectedAttribute, $"The Header message doesnt have the correct product type. Should be {expectedAttribute} instead of {message.Header.stdProductType}");

            // stdDataOrigin
            Assert.AreEqual(Environment.MachineName, message.Header.stdDataOrigin, $"The Header message doesnt have the correct data origin. Should be {Environment.MachineName} instead of {message.Header.stdDataOrigin}");

            // stdFrom  / stdTo
            material.Facility.LoadAttribute(amsOSRAMConstants.FacilityAttributeFacilityCode);
            string expectedFacilityCode = (string)material.Facility.Attributes.GetValueOrDefault(amsOSRAMConstants.FacilityAttributeFacilityCode, String.Empty);

            string expectedSiteCode = String.Empty;
            if (material.Facility.Site != null)
            {
                material.Facility.Site.LoadAttribute(amsOSRAMConstants.SiteAttributeSiteCode);
                expectedSiteCode = (string)material.Facility.Site.Attributes.GetValueOrDefault(amsOSRAMConstants.SiteAttributeSiteCode, String.Empty);
            }

            string expetedStepLogicalName = String.Empty;
            if (material.Step.ContainsLogicalNames)
            {
                material.Flow.LoadRelation("FlowStep");

                FlowStep flowStep = material.Flow.FlowSteps.FirstOrDefault(f => f.TargetEntity.Name == material.Step.Name);
                expetedStepLogicalName = flowStep.LogicalName;
            }

            string expectedPath = $"{expectedSiteCode}.{expectedFacilityCode}.{expetedStepLogicalName}";

            // Assert.AreEqual(expectedPath, message.Header.stdFrom, $"The Header message doesnt have the correct origin path. Should be {expectedPath} instead of {message.Header.stdFrom}");
            // Assert.AreEqual(expectedPath, message.Header.stdTo, $"The Header message doesnt have the correct destination path. Should be {expectedPath} instead of {message.Header.stdTo}");

            // stdTransaction
            Assert.AreEqual(message.Header.stdTransaction, customTransactionType.ToString(), $"The Header message doesnt have the correct transaction name. Should be {customTransactionType.ToString()} instead of {message.Header.stdTransaction}");
        }

        private void ValidateXML(IEnumerable<XElement> elementsA, IEnumerable<XElement> elementsB)
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
                    "ExportId",
                    "RefId"
                };

            IEnumerable<XElement> filteredElementsA = elementsA.Where(w => !xmlElementsToDiscard.Contains(w.Name.ToString()));
            IEnumerable<XElement> filteredElementsB = elementsB.Where(w => !xmlElementsToDiscard.Contains(w.Name.ToString()));

            foreach (XElement elementA in filteredElementsA)
            {
                XElement elementB = filteredElementsB.FirstOrDefault(f => f.Name == elementA.Name);

                IEnumerable<XAttribute> elementAAttributes = elementA.Attributes().Where(w => !xmlAttributesToDiscard.Contains(w.Name.ToString()));
                IEnumerable<XAttribute> elementBAttributes = elementB.Attributes().Where(w => !xmlAttributesToDiscard.Contains(w.Name.ToString()));

                Assert.AreEqual(elementAAttributes.Count(), elementBAttributes.Count(), $"Mismatch between the number of attributes of MES and MessageBus for {elementA.Name}");

                foreach (XAttribute elementAAttribute in elementAAttributes)
                {
                    Assert.AreEqual(elementBAttributes.FirstOrDefault(s => s.Name == elementAAttribute.Name).Value, elementAAttribute.Value, $"Mismatch between MES and MessageBus for {elementA.Name}");
                }
            }
        }

        private Func<bool> SuscribeMessageBus(CustomSendEventMessageTopics topic, int numberOfMessages = 1)
        {
            Transport messageBusTransport = new Transport(BaseContext.GetMessageBusTransportConfiguration());

            messageBusTransport.Start();

            messageBusTransport.Subscribe(topic.ToString(), (string subject, MbMessage message) =>
            {
                if (message != null && !string.IsNullOrWhiteSpace(message.Data))
                {
                    _tibcoCustomSendEventMessages.Add(JsonConvert.DeserializeObject<TibcoCustomSendEventMessage>(message.Data));
                }
            });

            Func<bool> waitForMessageBus = () =>
            {
                bool isDone = _tibcoCustomSendEventMessages.Count == numberOfMessages;

                if (isDone)
                {
                    messageBusTransport.Unsubscribe(topic.ToString());
                }

                return isDone;
            };

            return waitForMessageBus;
        }
    }
}