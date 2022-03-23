using System;
using System.Linq;
using System.Threading;
using cmConnect.TestFramework.Common.Utilities;
using cmConnect.TestFramework.EquipmentSimulator.Drivers;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects;
using Cmf.SECS.Driver;
using Cmf.TestScenarios.ContainerManagement.ContainerScenarios;
using AMSOsramEIAutomaticTests.Objects.Extensions;
using AMSOsramEIAutomaticTests.Objects.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cmf.Custom.TestUtilities;
using cmConnect.TestFramework.EquipmentSimulator.Objects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.OutputObjects;
using System.Data;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Custom.Tests.IoT.Tests.Common;
using System.Collections.Generic;
using Cmf.Custom.AMSOsram.BusinessObjects;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Foundation.Common.Base;
using Newtonsoft.Json.Linq;

namespace AMSOsramEIAutomaticTests.MechatronicMWS200
{
    [TestClass]
    public class MechatronicMWS200 : CommonTests
    {
        private const string resourceName = "ENA01";

        public const int numberOfWafersPerLot = 3;

        public const string stepName = "M3-LO-Wafer Sorter-Split-01518F010_E";
        public const string flowName = "FOL-UX3_EPA";

        private int sourceLoadPortNumber;
        private int destinationLoadPortNumber;


        private CustomSorterJobDefinition customSorterJobDefinition = new CustomSorterJobDefinition();

        private static SmartTableManager smartTableManager = new SmartTableManager();
        private static GenericTableManager genericTableManager = new GenericTableManager();

        private List<int> loadPortsUsed = new List<int>();
        private readonly Dictionary<int, string> loadPortNames = new Dictionary<int, string>()
        {
            { 1, "ENA01-LP01" },
            { 2, "ENA01-LP02" },
            { 3, "ENA01-LP03" },
            { 4, "ENA01-LP04" }
        };


        private const int numberOfLoadPorts = 4;

        public const bool subMaterialTrackin = true;

        public string recipeName = "TestRecipeForMechatronicMWS200";
        public const string serviceName = "Sorter-Split";

        private int loadPortNumber = 1;

        private bool isOnlineRemote = true;

        public bool createControlJobReceived = false;
        public bool createControlJobDenied = false;

        public bool failAtProcessJob = false;
        public bool createProcessJobReceived = false;
        public bool createProcessJobDenied = false;


        ContainerScenario containerScenarioForLoadPort2;

        #region Test Basics
        [TestInitialize]
        public void TestInit()
        {

            base.Equipment = m_Scenario.GetEquipment(m_Scenario.EquipmentToTest) as SecsGemEquipment;

            base.Initialize(recipeName);
            base.SubMaterialTrackin = subMaterialTrackin;

            base.Equipment.RegisterOnMessage("S1F3", OnS1F3);
            base.Equipment.RegisterOnMessage("S14F9", OnS14F9);
            base.Equipment.RegisterOnMessage("S16F11", OnS16F11);

            base.LoadPortNumber = loadPortNumber;
        }

        [TestCleanup]
        public void TestCleanup()
        {

            isOnlineRemote = true;

            createControlJobReceived = false;
            createControlJobDenied = false;

            createProcessJobReceived = false;
            createProcessJobDenied = false;

            if (containerScenarioForLoadPort2 != null)
            {
                containerScenarioForLoadPort2.TearDown();
            }

            #region Handle custom sorter job definition
            if (customSorterJobDefinition != null && customSorterJobDefinition.ObjectExists())
            {
                customSorterJobDefinition.Load();
                if (customSorterJobDefinition.UniversalState != UniversalState.Terminated)
                {
                    customSorterJobDefinition.Terminate();
                }
            }
            #endregion

            //regular teardown
            AfterTest();
            base.CleanUp(MESScenario);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ConfigureConnection(resourceName, 5013, connectionAttributes: new Dictionary<string, object>() { { "IsSorter", true } });
            //ConfigureConnection(resourceName, 5013);

        }


        [ClassCleanup]
        public static void ClassCleanup()
        {
            Cleanup();
        }

        #endregion Test Basic

        #region Tests FullProcessScenario   

        /// <summary>
        /// Perform MapCarrier with a CustomSorterJobDefinitionContext in place
        /// </summary>
        [TestMethod,
            Description
            (
            "- Dock a container into Load Port 1" +
            "- Calls Material IN orchestration" +
            "- SorterJobDefinitionContext is resolved" +
            "- ")]
        public void MechatronicMWS200_MapCarrier_WithSorterJobDefinitionContext()
        {
            base.LoadPortNumber = sourceLoadPortNumber = destinationLoadPortNumber = 1;
            loadPortsUsed.Add(1);
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot);

            customSorterJobDefinition = GetCustomSorterJobDefinition(AMSOsramConstants.CustomSorterLogisticalProcessMapCarrier, new ContainerCollection() { }, new ContainerCollection() { });
            InsertDataIntoCustomSorterJobDefinitionContextTable(stepName, customSorterJobDefinition.Name, materialName: MESScenario.Entity.Name);

            base.RunBasicTest(MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true);
        }


        #endregion Tests FullProcessScenario 

        #region Test State and Data Collection
        /// <summary> 
        /// Scenario: Control State to Host Offline
        /// </summary>
        [TestMethod]
        public void MechatronicMWS200_ControlStateUpdateTest()
        {

            base.Equipment.Variables["ControlState"] = 3;
            // Trigger event
            base.Equipment.SendMessage("EquipmentOFFLINE", null);

            //
            TestUtilities.WaitFor(10/*ValidationTimeout*/, "Control State was not updated to Host Offline", () =>
            {
                Resource resource = new Resource { Name = resourceName };
                resource.Load();

                var input = new LoadResourceStateModelsInput
                {
                    Resource = resource
                }.LoadResourceStateModelsSync();

                resource = input.Resource;

                if (resource.CurrentStates == null)
                    return false;

                return resource.CurrentStates.FirstOrDefault(s => s.StateModel.Name == "CustomSecsGemControlStateModel" && s.CurrentState.Name == "HostOffline") != null;
            });
            Thread.Sleep(1000);
            base.Equipment.Variables["ControlState"] = 5;
            // Trigger event
            base.Equipment.SendMessage("ControlStateREMOTE", null);


            TestUtilities.WaitFor(ValidationTimeout, "Control State was not updated to Online Remote", () =>
            {
                Resource resource = new Resource { Name = resourceName };
                resource.Load();

                var input = new LoadResourceStateModelsInput
                {
                    Resource = resource
                }.LoadResourceStateModelsSync();

                resource = input.Resource;

                if (resource.CurrentStates == null)
                    return false;

                return resource.CurrentStates.FirstOrDefault(s => s.StateModel.Name == "CustomSecsGemControlStateModel" && s.CurrentState.Name == "OnlineRemote") != null;
            });

            Thread.Sleep(1000);
            base.Equipment.Variables["ControlState"] = 4;
            // Trigger event
            base.Equipment.SendMessage("ControlStateLOCAL", null);

            TestUtilities.WaitFor(ValidationTimeout, "Control State was not updated to Online Local", () =>
            {
                Resource resource = new Resource { Name = resourceName };
                resource.Load();

                var input = new LoadResourceStateModelsInput
                {
                    Resource = resource
                }.LoadResourceStateModelsSync();

                resource = input.Resource;

                if (resource.CurrentStates == null)
                    return false;

                return resource.CurrentStates.FirstOrDefault(s => s.StateModel.Name == "CustomSecsGemControlStateModel" && s.CurrentState.Name == "OnlineLocal") != null;
            });

        }


        /// <summary> 
        /// Scenario: Control State to Host Offline
        /// </summary>
        [TestMethod]
        public void MechatronicMWS200_EPTStateChangeTest()
        {

            base.Equipment.Variables["BlockedReason"] = 0;
            base.Equipment.Variables["BlockedReasonText"] = "NotBlocked";
            base.Equipment.Variables["EPTClock"] = "x";
            base.Equipment.Variables["EPTState"] = 0;
            base.Equipment.Variables["EPTStateTime"] = 3;
            base.Equipment.Variables["PreviousEPTState"] = 1;
            base.Equipment.Variables["PreviousTaskName"] = "x";
            base.Equipment.Variables["PreviousTaskType"] = 1;
            base.Equipment.Variables["TaskName"] = "x";
            base.Equipment.Variables["TaskType"] = 1;

            // Trigger event
            base.Equipment.SendMessage("EquipmentEPTStateChangeEvent", null);

            //
            TestUtilities.WaitFor(10/*ValidationTimeout*/, "Equipment State was not updated to Idle", () =>
            {
                Resource resource = new Resource { Name = resourceName };
                resource.Load();

                var input = new LoadResourceStateModelsInput
                {
                    Resource = resource
                }.LoadResourceStateModelsSync();

                resource = input.Resource;

                if (resource.CurrentStates == null)
                    return false;

                return resource.CurrentStates.FirstOrDefault(s => s.StateModel.Name == "CustomEquipmentPerformanceTrackingStateModel" && s.CurrentState.Name == "Idle") != null;
            });
            Thread.Sleep(1000);

            base.Equipment.Variables["BlockedReason"] = 0;
            base.Equipment.Variables["BlockedReasonText"] = "NotBlocked";
            base.Equipment.Variables["EPTClock"] = "x";
            base.Equipment.Variables["EPTState"] = 1;
            base.Equipment.Variables["EPTStateTime"] = 3;
            base.Equipment.Variables["PreviousEPTState"] = 1;
            base.Equipment.Variables["PreviousTaskName"] = "x";
            base.Equipment.Variables["PreviousTaskType"] = 1;
            base.Equipment.Variables["TaskName"] = "x";
            base.Equipment.Variables["TaskType"] = 1;

            // Trigger event
            base.Equipment.SendMessage("EquipmentEPTStateChangeEvent", null);

            ////
            TestUtilities.WaitFor(10/*ValidationTimeout*/, "Equipment State was not updated to Busy", () =>
            {
                Resource resource = new Resource { Name = resourceName };
                resource.Load();

                var input = new LoadResourceStateModelsInput
                {
                    Resource = resource
                }.LoadResourceStateModelsSync();

                resource = input.Resource;

                if (resource.CurrentStates == null)
                    return false;

                return resource.CurrentStates.FirstOrDefault(s => s.StateModel.Name == "CustomEquipmentPerformanceTrackingStateModel" && s.CurrentState.Name == "Busy") != null;
            });

            Thread.Sleep(1000);

            base.Equipment.Variables["BlockedReason"] = 8;
            base.Equipment.Variables["BlockedReasonText"] = "Cenas";
            base.Equipment.Variables["EPTClock"] = "x";
            base.Equipment.Variables["EPTState"] = 2;
            base.Equipment.Variables["EPTStateTime"] = 3;
            base.Equipment.Variables["PreviousEPTState"] = 1;
            base.Equipment.Variables["PreviousTaskName"] = "x";
            base.Equipment.Variables["PreviousTaskType"] = 1;
            base.Equipment.Variables["TaskName"] = "x";
            base.Equipment.Variables["TaskType"] = 1;

            // Trigger event
            base.Equipment.SendMessage("EquipmentEPTStateChangeEvent", null);

            //
            TestUtilities.WaitFor(10/*ValidationTimeout*/, "Equipment State was not updated to Blocked", () =>
            {
                Resource resource = new Resource { Name = resourceName };
                resource.Load();

                var input = new LoadResourceStateModelsInput
                {
                    Resource = resource
                }.LoadResourceStateModelsSync();

                resource = input.Resource;

                if (resource.CurrentStates == null)
                    return false;

                return resource.CurrentStates.FirstOrDefault(s => s.StateModel.Name == "CustomEquipmentPerformanceTrackingStateModel" && s.CurrentState.Name == "Blocked") != null;
            });

        }

        /// <summary> 
        /// Scenario: Alarm occurrs, validate ollection of alarm
        /// </summary>
        [TestMethod]
        public void MechatronicMWS200_AlarmDataCollection()
        {

            Resource resource = new Resource { Name = resourceName };
            resource.Load();

            //Load the instances and see how much is the count for the DataCollectionInstances
            var dataCollectionInstancesBefore = this.GetDataCollectionInstanceByResourceId(resource.Id).Count;

            Alarm alarmExample = new Alarm
            {
                AbstractName = "AName",
                DataItemId = "DataItemId",
                Id = 6,
                Name = "NameOfAlarm",
                Text = "Text of Alarm"
            };

            base.Equipment.SendAlarm(alarmExample, 0x01, null);


            TestUtilities.WaitFor(30/*ValidationTimeout*/, "Alarm was not received", () =>
            {
                var dataCollectionInstancesAfter = this.GetDataCollectionInstanceByResourceId(resource.Id).Count;
                return ((dataCollectionInstancesBefore + 1) == dataCollectionInstancesAfter);
            });
        }
        #endregion Tests FullProcessScenario 


        #region Events
        public override bool CarrierIn(CustomMaterialScenario scenario, int loadPortToSet)
        {

            //CarrierClamped
            base.Equipment.Variables["PortID"] = loadPortToSet;
            base.Equipment.Variables["PlacedCarrierPattern1"] = 1;
            base.Equipment.Variables["PlacedCarrierPattern2"] = 2;
            base.Equipment.Variables["PlacedCarrierPattern3"] = 3;
            base.Equipment.Variables["PlacedCarrierPattern4"] = 4;

            // Trigger event
            base.Equipment.SendMessage(String.Format($"MATERIAL_PLACED"), null);

            return true;

        }

        public override void CarrierInValidation(CustomMaterialScenario MESScenario, int loadPortToSet)
        {
            //clamped
            base.CarrierInValidation(MESScenario, loadPortToSet);


            var slotMap = new int[13];
            // scenario.ContainerScenario.Entity
            if (MESScenario.ContainerScenario.Entity.ContainerMaterials != null)
            {
                for (int i = 0; i < 13; i++)
                {
                    slotMap[i] = MESScenario.ContainerScenario.Entity.ContainerMaterials.Exists(p => p.Position != null && p.Position == i + 1) ? 1 : 0;
                }
            }
            SlotMapVariable slotMapDV = new SlotMapVariable(base.Equipment) { Presence = slotMap };





            base.Equipment.Variables["CarrierID"] = MESScenario.ContainerScenario.Entity.Name;
            base.Equipment.Variables["SlotMap"] = slotMapDV;
            base.Equipment.Variables["ContentMap"] = slotMapDV;
            base.Equipment.Variables["PortID"] = loadPortToSet;

            // Trigger event
            base.Equipment.SendMessage(String.Format($"COSM14_SLOTMAPNOTREAD_SLOTMAPWAITINGFORHOST"), null);


            Thread.Sleep(200);

        }

        public override bool CarrierOut(CustomMaterialScenario scenario)
        {

            //// wait for load 
            //TestUtilities.WaitFor(60, String.Format($"Unload Container Command never received"), () =>
            //{
            //    return unloadCommandReceived;
            //});

            //CarrierSMTrans21 ready to unload

            var slotMap = new int[13];
            // scenario.ContainerScenario.Entity
            if (MESScenario.ContainerScenario.Entity.ContainerMaterials != null)
            {
                for (int i = 0; i < 13; i++)
                {
                    slotMap[i] = MESScenario.ContainerScenario.Entity.ContainerMaterials.Exists(p => p.Position != null && p.Position == i + 1) ? 1 : 0;
                }
            }
            SlotMapVariable slotMapDV = new SlotMapVariable(base.Equipment) { Presence = slotMap };


            base.Equipment.Variables["CarrierSubType"] = MESScenario.ContainerScenario.Entity.Name;
            base.Equipment.Variables["SubstrateSubType"] = "Substrate";
            base.Equipment.Variables["CarrierAccessingStatus"] = 0;
            base.Equipment.Variables["CarrierCapacity"] = 25;
            base.Equipment.Variables["CarrierContentMap"] = slotMapDV;
            base.Equipment.Variables["CarrierID_CarrierReport"] = $"CarrierAtPort{loadPortNumber}";
            base.Equipment.Variables["CarrierIDStatus"] = 2;
            base.Equipment.Variables["CarrierLocationID"] = $"FIMS{loadPortNumber}";
            base.Equipment.Variables["CarrierSlotMap"] = slotMapDV;
            base.Equipment.Variables["PortID_CarrierReport"] = loadPortNumber;


            // Trigger event
            base.Equipment.SendMessage(String.Format($"CarrierSMTrans21"), null);

            // MaterialRemoved
            base.Equipment.Variables["PortTransferState"] = 1;
            base.Equipment.Variables["PortReservationState"] = 0;
            base.Equipment.Variables["CarrierID_CarrierReport"] = "";
            base.Equipment.Variables["PortAccessMode"] = 0;
            base.Equipment.Variables["PortAssociationState"] = 0;
            base.Equipment.Variables["PortID_CarrierReport"] = loadPortNumber;

            // Trigger event
            base.Equipment.SendMessage(String.Format($"MaterialRemoved"), null);

            Thread.Sleep(200);


            return true;
        }


        public override bool ProcessStartEvent(CustomMaterialScenario scenario)
        {
            base.Equipment.Variables["ControlJobID"] = $"CtrlJob_{scenario.Entity.Name}";

            //// Trigger event
            base.Equipment.SendMessage(String.Format($"CtrlJobSMTrans05"), null);

            Thread.Sleep(2000);

            return true;
        }

        public override bool ProcessCompleteEvent(CustomMaterialScenario scenario)
        {
            base.Equipment.Variables["ControlJobID"] = $"CtrlJob_{scenario.Entity.Name}";

            //// Trigger event
            base.Equipment.SendMessage(String.Format($"CtrlJobSMTrans10"), null);

            return true;
        }

        public override bool ProcessStateChange(CustomMaterialScenario scenario)
        {
            /*
            //TODO:Update for correct values
            base.Equipment.Variables["ProcessState"] = 4;// Executing
            base.Equipment.Variables["PreviousProcessState"] = 3;

            base.Equipment.SendMessage("ProcessStateChange", null);
            */
            return true;
        }
        #endregion Events



        public override bool PostTrackInActions(CustomMaterialScenario scenario)
        {
            if (!isOnlineRemote)
            {
                Assert.Fail("Track In must fail on Online Local");
            }
            else
            {
                if (!createControlJobReceived || !createProcessJobReceived)
                {
                    Assert.Fail("Control or Process Job creation requests were never received");
                }
            }

            TestUtilities.WaitFor(60, String.Format($"Material {scenario.Entity.Name} State is not {MaterialStateModelStateEnum.Setup.ToString()}"), () =>
            {
                scenario.Entity.Load();
                return scenario.Entity.CurrentMainState.CurrentState.Name.Equals(MaterialStateModelStateEnum.Setup.ToString());
            });

            TestUtilities.WaitFor(60, String.Format($"Material {scenario.Entity.Name} System State is not {MaterialSystemState.InProcess.ToString()}"), () =>
            {
                scenario.Entity.Load();
                return scenario.Entity.SystemState.ToString().Equals(MaterialSystemState.InProcess.ToString());
            });

            return true;
        }

        public override bool WaferStart(Material wafer)
        {
            wafer.Load();
            wafer.LoadRelations();
            wafer.ParentMaterial.Load();

            var subId = String.Format("CarrierAtPort{0}.{1:D2}", loadPortNumber, wafer.MaterialContainer.First().Position);

            base.Equipment.Variables["AcquiredId"] = "x";
            base.Equipment.Variables["LotId"] = "y";
            base.Equipment.Variables["SubId"] = subId;

            //// Trigger event
            //base.Equipment.SendMessage($"ProcessChamber{chamberToProcess}_ProcessStarted", null);

            Thread.Sleep(2000);

            return true;
        }

        public override bool WaferComplete(Material wafer)
        {
            wafer.Load();
            wafer.LoadRelations();
            wafer.ParentMaterial.Load();

            base.Equipment.Variables["AcquiredId"] = "";
            base.Equipment.Variables["LotId"] = "";
            base.Equipment.Variables["SubId"] = String.Format("CarrierAtPort{0}.{1:D2}", loadPortNumber, wafer.MaterialContainer.First().Position); ;

            //// Trigger event
            //base.Equipment.SendMessage($"ProcessChamber{chamberToProcess}_ProcessFinished", null);

            Thread.Sleep(2000);

            return true;
        }


        public override bool ValidateSubMaterialState(Material submaterial, string subMaterialState)
        {
            if (MaterialSystemState.Processed.ToString().Equals(subMaterialState))
            {
                submaterial.Load();
                submaterial.LoadRelations();
                submaterial.ParentMaterial.Load();
                submaterial.ParentMaterial.LoadChildren();
                if (submaterial.ParentMaterial.SubMaterials
                    .Where(s => s.SystemState == MaterialSystemState.Queued).Count() == 0
                    && submaterial.ParentMaterial.SubMaterials
                    .Where(s => s.SystemState == MaterialSystemState.InProcess).Count() == 1)
                {
                    return true;
                }
            }

            TestUtilities.WaitFor(90, String.Format($"Material {submaterial.Name} State is not {subMaterialState}"), () =>
            {
                submaterial.Load();
                return submaterial.SystemState.ToString().Equals(subMaterialState);
            });

            return true;
        }

        //Control Job 
        protected bool OnS14F9(SecsMessage request, SecsMessage reply)
        {
            //Create Control Job
            var secsItems = request.Item.GetChildList()[2].GetChildList();

            var controlJobItem = secsItems[0];
            string controlJobName = controlJobItem.GetChildList()[1].GetValue().ToString();

            var carrierInputSpec = secsItems[1];
            string carrierName = carrierInputSpec.GetChildList()[1].GetChildList()[0].GetValue().ToString();

            createControlJobReceived = true;
            /*
            if (!(controlJobName == MESScenario.Entity.Name))
            {
                Assert.Fail($"Control job name does not match with the lot: {controlJobName}");
            }

            if (!(carrierName == MESScenario.ContainerScenario.Entity.Name))
            {
                Assert.Fail($"Carrier name does not match with the expected: {carrierName}");
            }*/

            reply.Item.Clear();
            var attributeList = new SecsItem();
            attributeList.SetTypeToList();

            var jobItem = new SecsItem { ASCII = controlJobName };

            //mainList.Add(jobItem);

            var reportList = new SecsItem();
            reportList.SetTypeToList();


            var ackList = new SecsItem();
            ackList.SetTypeToList();

            var ack = new SecsItem { U1 = new byte[] { 0x00 } };

            var errorList = new SecsItem();
            errorList.SetTypeToList();

            reportList.Add(ack);
            reportList.Add(errorList);


            if (createControlJobDenied)
            {
                ack = new SecsItem { U1 = new byte[] { 0x01 } };

                errorList = new SecsItem();
                errorList.SetTypeToList();
                var error = new SecsItem();
                error.SetTypeToList();
                error.Add(new SecsItem() { U1 = new byte[] { 7 } });
                error.Add(new SecsItem() { ASCII = $"{MESScenario.Entity.Name} : RecID : IllegalValue'" });
                errorList.Add(error);
            }

            reply.Item.Add(jobItem);
            reply.Item.Add(attributeList);
            reply.Item.Add(reportList);
            return true;
        }

        protected bool OnS16F11(SecsMessage request, SecsMessage reply)
        {
            reply.Item.Clear();
            var jobItem = new SecsItem { ASCII = request.Item.GetChildList()[1].GetValue().ToString() };

            var reportList = new SecsItem();
            reportList.SetTypeToList();

            var ack = new SecsItem { Bool = new bool[] { true } };

            var errorList = new SecsItem();
            errorList.SetTypeToList();

            if (failAtProcessJob)
            {
                ack = new SecsItem { Bool = new bool[] { false } };
                var errCode = new SecsItem { ASCII = "cenas1" };
                var errText = new SecsItem { ASCII = "cenas2" };

                errorList.Add(errCode);
                errorList.Add(errText);
            }


            reportList.Add(ack);
            reportList.Add(errorList);

            reply.Item.Add(jobItem);
            reply.Item.Add(reportList);

            //Process Job Request
            createProcessJobReceived = true;
            return true;
        }

        

        protected virtual bool OnS1F3(SecsMessage request, SecsMessage reply)
        {
            reply.Item.Clear();
            for (int i = 0; i < request.Item.Count; ++i)
            {
                uint ecid = request.Item[i].U4.Single();
                if (base.Equipment.EquipmentVariables["ControlState"].DataItemId == ecid.ToString())
                {
                    reply.Item.Add(new SecsItem { U1 = new byte[] { (byte)(isOnlineRemote ? 5 : 4) } });
                }
            }

            return true;
        }

        public override void WaferCompleteValidation(Material material)
        {
            Log(String.Format("{0}: [S] Send Wafer Start Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));
            WaferComplete(material);
            Log(String.Format("{0}: [E] Send Wafer Start Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));

            //Sleep to allow wafer complete
            System.Threading.Thread.Sleep(500);
            Log(String.Format("{0}: [S] Validate MES Material {2} is Processed Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));
            ValidateSubMaterialState(material, MaterialStateModelStateEnum.Processed.ToString());
            Log(String.Format("{0}: [E] Validate MES Material {2} is Processed Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));

        }
        public override bool PostSetupActions(CustomMaterialScenario MESScenario)
        {
            return true;
        }



        #region Sorter Exclusive
        private ContainerScenario CreateEmptyContainerScenario(int loadPort, string facilityName, string containerType, int totalPositions = 0)
        {
            Facility facility = new Facility();

            if (string.IsNullOrWhiteSpace(facilityName))
            {
                facility.Name = AMSOsramConstants.TestFacility;
                facility.Load();
            }
            else
            {
                facility.Name = facilityName;
                facility.Load();
            }

            if (totalPositions == 0)
            {
                totalPositions = AMSOsramConstants.ContainerTotalPosition;
            }

            // Create Container to put the Wafers
            ContainerScenario containerScenario = new ContainerScenario();
            containerScenario.Entity.IsAutoGeneratePositionEnabled = false;
            containerScenario.Entity.Name = $"Container_{loadPort}_{DateTime.Now:yyyyMMdd_HHmmssfff}";
            containerScenario.Entity.Type = containerType; //CreeConstants.ContainerTypeBEOL;
            containerScenario.Entity.PositionUnitType = ContainerPositionUnitType.Material;
            containerScenario.Entity.Facility = facility;
            containerScenario.Entity.CapacityUnits = AMSOsramConstants.UnitWafers;
            containerScenario.Entity.CapacityPerPosition = 1;
            containerScenario.Entity.TotalPositions = totalPositions;
            containerScenario.Setup();

            return containerScenario;
        }


        /// <summary>
        /// Inserts data into the Sorter context table
        /// </summary>
        /// <param name="stepName">The step</param>
        /// <param name="customSorterJobDefinitionName">The CustomSorterJobDefinition</param>
        /// <param name="productName">The product</param>
        /// <param name="productGroupName">The product group</param>
        /// <param name="flowName">The flow</param>
        /// <param name="materialName">The material</param>
        /// <param name="materialTypeName">The material type</param>
        /// <param name="clearSmartTable">Flag to clear smart table</param>
        private void InsertDataIntoCustomSorterJobDefinitionContextTable(string stepName,
            string customSorterJobDefinitionName,
            string productName = null,
            string productGroupName = null,
            string flowName = null,
            string materialName = null,
            string materialTypeName = null,
            bool clearSmartTable = true)
        {
            string tableName = "CustomSorterJobDefinitionContext";

            if (clearSmartTable)
            {
                smartTableManager.ClearSmartTable(tableName);
            }

            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "Step", stepName },
                { "Product", productName },
                { "ProductGroup", productGroupName },
                { "Flow", flowName },
                { "Material", materialName },
                { "MaterialType", materialTypeName },
                { "CustomSorterJobDefinition", customSorterJobDefinitionName }
            };

            smartTableManager.SetSmartTableData(tableName, data);
        }

        /// <summary>
        /// Creates a Custom Sorter Job Definition based on the received parameters
        /// </summary>
        /// <param name="logisticalProcess">The logistical process</param>
        /// <param name="sourceContainers">The source container</param>
        /// <param name="destinationContainers">The destination container</param>
        /// <param name="sourceContaineType">The source container type</param>
        /// <param name="targetContainerType">The target container type</param>
        /// <param name="futureActionType">The future action type</param>
        /// <param name="fullTransferWafers">Flag if it is a full transfer wafer scenario</param>
        /// <returns></returns>
        private CustomSorterJobDefinition GetCustomSorterJobDefinition(string logisticalProcess,
            ContainerCollection sourceContainers,
            ContainerCollection destinationContainers,
            string sourceContaineType = AMSOsramConstants.ContainerSMIFPod,
            string targetContainerType = AMSOsramConstants.ContainerSMIFPod,
            string futureActionType = "",
            bool fullTransferWafers = false)
        {
            CustomSorterJobDefinition customSorterJobDefinition = new CustomSorterJobDefinition
            {
                Name = Guid.NewGuid().ToString(),
                SourceCarrierType = sourceContaineType,
                TargetCarrierType = targetContainerType,
                LogisticalProcess = logisticalProcess
            };

            JArray temporaryMovementList = new JArray();

            JObject mainObj = new JObject
            {
                ["FutureActionType"] = futureActionType,
                ["Moves"] = temporaryMovementList,
            };

            if (logisticalProcess == AMSOsramConstants.CustomSorterLogisticalProcessTransferWafers)
            {
                if (futureActionType.Equals("Split", StringComparison.InvariantCulture)) // Split scenario
                {
                    mainObj["DeleteOnCompletion"] = true;
                    Container theOneThatWillBeSplitted = sourceContainers.FirstOrDefault();
                    theOneThatWillBeSplitted.LoadRelation("MaterialContainer");

                    int controlIndex = 1;
                    int containerIndex = 0;

                    foreach (var movement in theOneThatWillBeSplitted.ContainerMaterials)
                    {
                        Material material = movement.SourceEntity;
                        material.Load();

                        JObject jObject = new JObject
                        {
                            ["MaterialName"] = material.Name,
                            ["SourceContainer"] = theOneThatWillBeSplitted.Name,
                            ["SourcePosition"] = movement.Position,
                            ["DestinationContainer"] = "#" + containerIndex + 1,
                            ["DestinationPosition"] = controlIndex
                        };

                        temporaryMovementList.Add(jObject);
                        controlIndex++;

                        if (controlIndex == 4)
                        {
                            containerIndex++;
                            continue;
                        }
                    }
                }
                else if (futureActionType.Equals("Merge", StringComparison.InvariantCulture)) // Merge scenario
                {
                    Container theOneOthersWillMergeInto = destinationContainers.FirstOrDefault();

                    Queue<int> freePositions = new Queue<int>();
                    for (int i = 1; i <= 25; i++)
                    {
                        if (theOneOthersWillMergeInto.ContainerMaterials == null || !theOneOthersWillMergeInto.ContainerMaterials.Any(m => m.Position == i))
                        {
                            freePositions.Enqueue(i);
                        }
                    }

                    foreach (Container container in sourceContainers)
                    {
                        container.LoadRelation("MaterialContainer");

                        foreach (var movement in container.ContainerMaterials)
                        {
                            Material material = movement.SourceEntity;
                            material.Load();

                            JObject jObject = new JObject
                            {
                                ["MaterialName"] = material.Name,
                                ["SourceContainer"] = container.Name,
                                ["SourcePosition"] = movement.Position,
                                ["DestinationContainer"] = theOneOthersWillMergeInto.Name,
                                ["DestinationPosition"] = freePositions.Dequeue()
                            };

                            temporaryMovementList.Add(jObject);
                        }
                    }
                }
                else // Simple transfer scenario
                {
                    if (!fullTransferWafers)
                    {
                        Container theOneThatWillBeTranferred = sourceContainers.FirstOrDefault();
                        theOneThatWillBeTranferred.LoadRelation("MaterialContainer");

                        Container destinationContainer = destinationContainers.FirstOrDefault();
                        destinationContainer.LoadRelation("MaterialContainer");

                        Queue<int> freePositions = new Queue<int>();
                        for (int i = 1; i <= 25; i++)
                        {
                            if (destinationContainer.ContainerMaterials == null || !destinationContainer.ContainerMaterials.Any(m => m.Position == i))
                            {
                                freePositions.Enqueue(i);
                            }
                        }

                        foreach (var movement in theOneThatWillBeTranferred.ContainerMaterials)
                        {
                            JObject jObject = new JObject
                            {
                                ["MaterialName"] = "",
                                ["SourceContainer"] = theOneThatWillBeTranferred.Name,
                                ["SourcePosition"] = movement.Position,
                                ["DestinationContainer"] = destinationContainer.Name,
                                ["DestinationPosition"] = freePositions.Dequeue()
                            };

                            temporaryMovementList.Add(jObject);
                        }
                    }
                }
            }
            else if (logisticalProcess == AMSOsramConstants.CustomSorterLogisticalProcessCompose)
            {
                JArray substitutes = new JArray();
                JObject jObjectSub = new JObject
                {
                    ["Substitute"] = "",
                    ["Priority"] = 0
                };
                substitutes.Add(jObjectSub);

                JObject jObject = new JObject
                {
                    ["Product"] = "",
                    ["Substitutes"] = substitutes,
                    ["MaterialName"] = "",
                    ["SourceContainer"] = "",
                    ["SourcePosition"] = 0,
                    ["DestinationContainer"] = "",
                    ["DestinationPosition"] = 0
                };

                temporaryMovementList.Add(jObject);
            }

            customSorterJobDefinition.MovementList = mainObj.ToString();

            customSorterJobDefinition.Create();
            customSorterJobDefinition.Load();

            return customSorterJobDefinition;
        }


        #endregion

        private DataCollectionInstanceCollection GetDataCollectionInstanceByResourceId(long resourceId)
        {
            QueryObject query = new QueryObject();
            query.Description = "";
            query.EntityTypeName = "DataCollectionInstance";
            query.Name = "getDCIbyRes";
            query.Query = new Query();
            query.Query.Distinct = false;
            query.Query.Filters = new FilterCollection() {
                new Filter()
                {
                    Name = "Id",
                    ObjectName = "Resource",
                    ObjectAlias = "DataCollectionInstance_Resource_2",
                    Operator = Cmf.Foundation.Common.FieldOperator.IsEqualTo,
                    Value = resourceId,
                    LogicalOperator = Cmf.Foundation.Common.LogicalOperator.Nothing,
                    FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
                }
            };
            query.Query.Fields = new FieldCollection() {
                new Field()
                {
                    Alias = "Id",
                    ObjectName = "DataCollectionInstance",
                    ObjectAlias = "DataCollectionInstance_1",
                    IsUserAttribute = false,
                    Name = "Id",
                    Position = 0,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort
                },
                new Field()
                {
                    Alias = "Name",
                    ObjectName = "DataCollectionInstance",
                    ObjectAlias = "DataCollectionInstance_1",
                    IsUserAttribute = false,
                    Name = "Name",
                    Position = 1,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort
                }
            };
            query.Query.Relations = new RelationCollection() {
                new Relation()
                {
                    Alias = "",
                    IsRelation = false,
                    Name = "",
                    SourceEntity = "DataCollectionInstance",
                    SourceEntityAlias = "DataCollectionInstance_1",
                    SourceJoinType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
                    SourceProperty = "ResourceId",
                    TargetEntity = "Resource",
                    TargetEntityAlias = "DataCollectionInstance_Resource_2",
                    TargetJoinType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
                    TargetProperty = "Id"
                }
            };

            var executeInput = new ExecuteQueryInput
            {
                QueryObject = query
            };
            ExecuteQueryOutput executeOutput = executeInput.ExecuteQuerySync();

            DataSet ds = Cmf.TestScenarios.Others.Utilities.ToDataSet(executeOutput.NgpDataSet);

            DataCollectionInstanceCollection dcic = new DataCollectionInstanceCollection();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRowCollection dataRows = ds.Tables[0].Rows;

                foreach (DataRow dr in dataRows)
                {
                    DataCollectionInstance dci = new DataCollectionInstance
                    {
                        Id = (long)dr["Id"]
                    };
                    if (dci.Exists())
                    {
                        dcic.Add(dci);
                    }
                }
            }

            return dcic;
        }
    }
}