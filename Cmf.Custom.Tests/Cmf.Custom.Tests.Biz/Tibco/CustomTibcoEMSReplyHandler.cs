using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.Tests.Biz.Space;
using Cmf.Custom.TestUtilities;
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

namespace Cmf.Custom.Tests.Biz.Tibco
{
    [TestClass]
    public class CustomTibcoEMSReplyHandler
    {
        private const string ActionName = "CustomTibcoEMSReplyHandler";
        
        private CustomExecutionScenario scenario;
        private static Foundation.Common.DynamicExecutionEngine.Action action;

        private static Transport transport;
        private string isToUnsubscribeTopic;
        private List<Dictionary<string, object>> messageBusMessages;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            transport = new Transport(BaseContext.GetMessageBusTransportConfiguration());
            transport.Start();

            action = new Foundation.Common.DynamicExecutionEngine.Action();
            action.Name = ActionName;
            action.Load();
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
        ///     - Performs a TrackOut and MoveNext operation with a DataCollection that should be sent to 
        ///     - Calls the Reply handler with a valid mocked response
        ///
        /// Acceptance Criteria:
        ///     - Lot has a protocol instance after performing the TrackOut and MoveNext
        ///     - After handling the reply:
        ///         - The Material should have the protocol closed
        ///         - The Material should be in the next step
        ///
        /// </summary>
        /// <TestCaseID>CustomTibcoEMSReplyHandler_CustomReportEDCToSpace_ValidMessage</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomTibcoEMSReplyHandler_CustomReportEDCToSpace_ValidMessage()
        {
            string flowPath = amsOSRAMConstants.FlowPathSpace;
            string dataCollectionName = amsOSRAMConstants.DefaultSpaceDataCollectionName;
            string dataCollectionLimitSetName = amsOSRAMConstants.DefaultSpaceDataCollectionLimitSetName;
            string facilityName = amsOSRAMConstants.DefaultFacilityName;
            string productName = amsOSRAMConstants.ProductLotProduct;
            int logicalWaferQuantity = 1;
            string protocolName = ConfigUtilities.GetConfigValue(amsOSRAMConstants.DefaultProtocolSpaceConfig) as string;
            string processResourceName = amsOSRAMConstants.DefaultTestProcessResourceName;
            string processSubResourceName = amsOSRAMConstants.DefaultTestProcessSubResourceName;

            scenario.NumberOfMaterialsToGenerate = 1;
            scenario.NumberOfChildMaterialsToGenerate = logicalWaferQuantity;
            scenario.FacilityName = facilityName;
            scenario.ProductName = productName;
            scenario.FlowPath = flowPath;
            scenario.SmartTablesToClearInSetup.Add("RecipeContext");
            scenario.MaterialDCContext.Add(new Dictionary<string, string>
            {
                { "Step", "CMFTestProcessStep" },
                { "Operation", "TrackOut" },
                { "DataCollection", dataCollectionName },
                { "DataCollectionLimitSet", dataCollectionLimitSetName },
                { "DataCollectionType", "Immediate" }
            });
            scenario.Setup();

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

            DataCollectionPointCollection pointsToPost = CustomSendCriticalDataCollectionToSpace.CreateDataCollectionPointCollection(dataCollection, dataCollectionLimitSet, lot);

            Resource processResource = new Resource()
            {
                Name = processResourceName
            };
            processResource.Load();

            lot.Load();

            lot.ComplexDispatchAndTrackIn(processResource);

            lot.LoadChildren();

            Resource processSubResource = new Resource()
            {
                Name = processSubResourceName
            };
            processSubResource.Load();

            foreach (Material logicalWafer in lot.SubMaterials)
            {
                logicalWafer.ComplexTrackIn(processSubResource);
                logicalWafer.ComplexTrackOutMaterial();
                processSubResource.Load();
            }

            TrackOutAndMoveNextAndValidateSpaceMessage(lot, pointsToPost);

            lot.Load();
            lot.LoadRelations(new Collection<string> { "ProtocolMaterial" });
            Assert.IsTrue(lot.OpenExceptionProtocolsCount == 1, $"Material should have one protocol instance opened, instead has {lot.OpenExceptionProtocolsCount}.");

            ProtocolInstance protocolInstance = lot.MaterialProtocols.FirstOrDefault().SourceEntity;
            protocolInstance.Load();
            protocolInstance.ParentEntity.Load();
            Assert.IsTrue(protocolInstance.ParentEntity.Name.Equals(protocolName), $"Protocol should be ${protocolName} instead is {protocolInstance.ParentEntity.Name}");

            CustomUtilities.RunDEEForTests(action, new Dictionary<string, object>
            {
                { "ReplyMessage", GetValidSpaceResponse() },
                { "Context", 
                    new Dictionary<string, object>
                    {
                        { "Subject", amsOSRAMConstants.CustomReportEDCToSpace },
                        { "Lot", lot.Name},
                        { "ProtocolInstance", protocolInstance.Name },
                        { "ActionGroupName", "MaterialManagement.MaterialManagementOrchestration.ComplexTrackOutAndMoveMaterialsToNextStep.Post" },
                    }
                },
            });

            lot.Load();
            Assert.AreEqual(0, lot.OpenExceptionProtocolsCount, $"Material should have zero protocol instance opened, instead has {lot.OpenExceptionProtocolsCount}.");
            Assert.AreEqual(MaterialSystemState.Queued, lot.SystemState, "Material should be Queued");
        }

        /// <summary>
        /// Description:
        ///     - Performs a TrackOut and MoveNext operation with a DataCollection that should be sent to 
        ///     - Calls the Reply handler with several invalid mocked response
        ///
        /// Acceptance Criteria:
        ///     - Lot has a protocol instance after performing the TrackOut and MoveNext
        ///     - After handling the reply:
        ///         - The Material should remaining with the protocol opened
        ///         - The Material should be in the same step on the same state
        ///
        /// </summary>
        /// <TestCaseID>CustomTibcoEMSReplyHandler_CustomReportEDCToSpace_InvalidMessage</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomTibcoEMSReplyHandler_CustomReportEDCToSpace_InvalidMessage()
        {
            string flowPath = amsOSRAMConstants.FlowPathSpace;
            string dataCollectionName = amsOSRAMConstants.DefaultSpaceDataCollectionName;
            string dataCollectionLimitSetName = amsOSRAMConstants.DefaultSpaceDataCollectionLimitSetName;
            string facilityName = amsOSRAMConstants.DefaultFacilityName;
            string productName = amsOSRAMConstants.ProductLotProduct;
            int logicalWaferQuantity = 1;
            string protocolName = ConfigUtilities.GetConfigValue(amsOSRAMConstants.DefaultProtocolSpaceConfig) as string;

            scenario.NumberOfMaterialsToGenerate = 1;
            scenario.NumberOfChildMaterialsToGenerate = logicalWaferQuantity;
            scenario.FacilityName = facilityName;
            scenario.ProductName = productName;
            scenario.FlowPath = flowPath;
            scenario.SmartTablesToClearInSetup.Add("RecipeContext");
            scenario.Setup();

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

            DataCollectionPointCollection pointsToPost = CustomSendCriticalDataCollectionToSpace.CreateDataCollectionPointCollection(dataCollection, dataCollectionLimitSet, lot);

            PostDataCollectionAndValidateSpaceMessage(lot, dataCollection, dataCollectionLimitSet, pointsToPost);

            lot.Load();
            lot.LoadRelations(new Collection<string> { "ProtocolMaterial" });
            Assert.AreEqual(1, lot.OpenExceptionProtocolsCount, $"Material should have one protocol instance opened, instead has {lot.OpenExceptionProtocolsCount}.");

            ProtocolInstance protocolInstance = lot.MaterialProtocols.FirstOrDefault().SourceEntity;
            protocolInstance.Load();
            protocolInstance.ParentEntity.Load();
            Assert.IsTrue(protocolInstance.ParentEntity.Name.Equals(protocolName), $"Protocol should be ${protocolName} instead is {protocolInstance.ParentEntity.Name}");

            List<string> invalidMessages = new List<string>
            {
                GetInvalidSampleSpaceResponse(),
                GetInvalidUploadSuccessSpaceResponse(),
                GetInvalidValidationSuccessSpaceResponse()
            };

            foreach (string invalidMessage in invalidMessages) 
            {
                CustomUtilities.RunDEEForTests(action, new Dictionary<string, object>
                {
                    { "ReplyMessage", invalidMessage },
                    { "Context",
                        new Dictionary<string, object>
                        {
                            { "Subject", amsOSRAMConstants.CustomReportEDCToSpace },
                            { "Lot", lot.Name},
                            { "ProtocolInstance", protocolInstance.Name },
                            { "ActionGroupName", "" },
                        }
                    },
                });

                lot.Load();
                Assert.AreEqual(1, lot.OpenExceptionProtocolsCount, $"Material should have one protocol instance opened, instead has {lot.OpenExceptionProtocolsCount}.");
            }
        }

        /// <summary>Post DataCollection and validate Space Message</summary>
        /// <param name="material"></param>
        /// <param name="dataCollection">DataCollection</param>
        /// <param name="dataCollectionLimitSet">DataCollection Limit Set</param>
        /// <param name="pointsToPost">Points to Post</param>
        /// <returns>Message sent to message bus</returns>
        private string PostDataCollectionAndValidateSpaceMessage(Material material, DataCollection dataCollection, DataCollectionLimitSet dataCollectionLimitSet, DataCollectionPointCollection pointsToPost)
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

        /// <summary>Post DataCollection and validate Space Message</summary>
        /// <param name="material"></param>
        /// <param name="dataCollection">DataCollection</param>
        /// <param name="dataCollectionLimitSet">DataCollection Limit Set</param>
        /// <param name="pointsToPost">Points to Post</param>
        /// <returns>Message sent to message bus</returns>
        private string TrackOutAndMoveNextAndValidateSpaceMessage(Material material, DataCollectionPointCollection pointsToPost)
        {
            Func<bool> waitForMessageBus = SubscribeMessageBus(amsOSRAMConstants.CustomReportEDCToSpace, 1);

            material.ComplexTrackOutAndMoveNext(currentDataCollectionPoints: pointsToPost);

            waitForMessageBus.WaitFor();

            Dictionary<string, object> messageBusMessage = messageBusMessages.FirstOrDefault();
            Assert.IsTrue(messageBusMessage != null && messageBusMessage.Count > 0, "Message was not received from message bus.");

            object message;
            messageBusMessage.TryGetValue("Message", out message);

            return (string)message;
        }

        private string GetValidSpaceResponse()
        {
            return "<?xml version=\"1.1\" encoding=\"UTF-8\" standalone=\"no\"?><response uploadSuccess=\"true\" validationSuccess=\"true\">\t<samples accepted=\"true\" excluded=\"0\" parameterName=\"NZ-A8-AM-Flattening-MS-Test\" parameterUnit=\"mm\" sampleId=\"215916825\">\t\t<keys>\t\t\t<key id=\"1\" name=\"Materialsystem\" type=\"ExKey\">InGaN</key>\t\t\t<key id=\"2\" name=\"Equipment\" type=\"ExKey\">EQ1</key>\t\t\t<key id=\"4\" name=\"Epi_Typ\" type=\"ExKey\">Bestseller</key>\t\t\t<key id=\"12\" name=\"X\" type=\"DatKey\">.</key>\t\t\t<key id=\"13\" name=\"Y\" type=\"DatKey\">.</key>\t\t</keys>\t\t<channelCkcIds>\t\t\t<channelCkcId channelId=\"119178\" ckcId=\"1\"/>\t\t</channelCkcIds>\t\t<violations>\t\t\t<violation id=\"1\" name=\"Raw above specification\"/>\t\t</violations>\t</samples></response>";
        }

        private string GetInvalidUploadSuccessSpaceResponse()
        {
            return "<?xml version=\"1.1\" encoding=\"UTF-8\" standalone=\"no\"?><response uploadSuccess=\"false\" validationSuccess=\"true\">\t<samples accepted=\"true\" excluded=\"0\" parameterName=\"NZ-A8-AM-Flattening-MS-Test\" parameterUnit=\"mm\" sampleId=\"215916825\">\t\t<keys>\t\t\t<key id=\"1\" name=\"Materialsystem\" type=\"ExKey\">InGaN</key>\t\t\t<key id=\"2\" name=\"Equipment\" type=\"ExKey\">EQ1</key>\t\t\t<key id=\"4\" name=\"Epi_Typ\" type=\"ExKey\">Bestseller</key>\t\t\t<key id=\"12\" name=\"X\" type=\"DatKey\">.</key>\t\t\t<key id=\"13\" name=\"Y\" type=\"DatKey\">.</key>\t\t</keys>\t\t<channelCkcIds>\t\t\t<channelCkcId channelId=\"119178\" ckcId=\"1\"/>\t\t</channelCkcIds>\t\t<violations>\t\t\t<violation id=\"1\" name=\"Raw above specification\"/>\t\t</violations>\t</samples></response>";
        }

        private string GetInvalidValidationSuccessSpaceResponse()
        {
            return "<?xml version=\"1.1\" encoding=\"UTF-8\" standalone=\"no\"?><response uploadSuccess=\"true\" validationSuccess=\"false\">\t<samples accepted=\"true\" excluded=\"0\" parameterName=\"NZ-A8-AM-Flattening-MS-Test\" parameterUnit=\"mm\" sampleId=\"215916825\">\t\t<keys>\t\t\t<key id=\"1\" name=\"Materialsystem\" type=\"ExKey\">InGaN</key>\t\t\t<key id=\"2\" name=\"Equipment\" type=\"ExKey\">EQ1</key>\t\t\t<key id=\"4\" name=\"Epi_Typ\" type=\"ExKey\">Bestseller</key>\t\t\t<key id=\"12\" name=\"X\" type=\"DatKey\">.</key>\t\t\t<key id=\"13\" name=\"Y\" type=\"DatKey\">.</key>\t\t</keys>\t\t<channelCkcIds>\t\t\t<channelCkcId channelId=\"119178\" ckcId=\"1\"/>\t\t</channelCkcIds>\t\t<violations>\t\t\t<violation id=\"1\" name=\"Raw above specification\"/>\t\t</violations>\t</samples></response>";
        }

        private string GetInvalidSampleSpaceResponse()
        {
            return "<?xml version=\"1.1\" encoding=\"UTF-8\" standalone=\"no\"?><response uploadSuccess=\"true\" validationSuccess=\"true\">\t<samples accepted=\"false\" excluded=\"0\" parameterName=\"NZ-A8-AM-Flattening-MS-Test\" parameterUnit=\"mm\" sampleId=\"215916825\">\t\t<keys>\t\t\t<key id=\"1\" name=\"Materialsystem\" type=\"ExKey\">InGaN</key>\t\t\t<key id=\"2\" name=\"Equipment\" type=\"ExKey\">EQ1</key>\t\t\t<key id=\"4\" name=\"Epi_Typ\" type=\"ExKey\">Bestseller</key>\t\t\t<key id=\"12\" name=\"X\" type=\"DatKey\">.</key>\t\t\t<key id=\"13\" name=\"Y\" type=\"DatKey\">.</key>\t\t</keys>\t\t<channelCkcIds>\t\t\t<channelCkcId channelId=\"119178\" ckcId=\"1\"/>\t\t</channelCkcIds>\t\t<violations>\t\t\t<violation id=\"1\" name=\"Raw above specification\"/>\t\t</violations>\t</samples></response>";
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
