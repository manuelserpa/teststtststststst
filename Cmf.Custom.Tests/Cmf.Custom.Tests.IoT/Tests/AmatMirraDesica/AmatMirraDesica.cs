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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cmf.Custom.TestUtilities;
using cmConnect.TestFramework.EquipmentSimulator.Objects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.OutputObjects;
using System.Data;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Custom.Tests.IoT.Tests.Common;
using System.Collections.Generic;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;
using Cmf.Custom.Tests.IoT.Tests.HermosLFM4xReader;
using Cmf.Foundation.BusinessObjects;
using Cmf.TestScenarios.Others;
using amsOSRAMEIAutomaticTests;
using amsOSRAMEIAutomaticTests.Objects.Utilities;
using amsOSRAMEIAutomaticTests.Objects.Extensions;

namespace AMSOsramEIAutomaticTests.AmatMirraDesica
{
    [TestClass]
    public class AmatMirraDesica : CommonTests
    {
        private const string resourceName = "5FCMP2";

        public const int numberOfWafersPerLot = 3;

        public const string stepName = "M2-LT-MueTec-CD-Measurement-00820F053_E";
        public const string flowName = "FOL-UX3_EPA";

        public const bool subMaterialTrackin = true;

        public string recipeName = "TestRecipeForAmatMirraDesica";
        public const string serviceName = "CD-Measurement";

        private string samplingPattern = "";

        private bool isOnlineRemote = true;

        public bool createControlJobReceived = false;
        public bool createControlJobDenied = false;

        public bool failAtProcessJob = false;
        public bool createProcessJobReceived = false;
        public bool createProcessJobDenied = false;

        public bool isValidProceedWithCarrier = true;
        public bool proceedWithCarriersReceived = false;

        public bool receivedPPSelectCommand = false;
        public bool receivedStartCommand = false;
        public bool receivedLockPodCommand = false;
        public bool receivedLoadPodCommand = false;
        public bool receivedUnlockPodCommand = false;
        public bool receivedUnloadPodCommand = false;
        public bool receivedSETMIDCommand = false;
        public bool receivedMAPCASSCommand = false;

        private static readonly Dictionary<int, string> loadPortNames = new Dictionary<int, string>()
        {
            { 1, "5FCMP2-LP1" },
            { 2, "5FCMP2-LP2" },
            { 3, "5FCMP2-LP3" },
            { 4, "5FCMP2-LP4" }
        };

        public HermosLFM4xReader RFIDReader = new HermosLFM4xReader();
        public const string readerResourceName = "5FCMP2.RFID";

        #region Event Names
        private string MaterialReceivedEvent = "PodArrivedPort1";
        private string ReadyToLoadEvent = "PodLockedPort1";
        private string WaferStartEvent = "WaferStartedPort1";
        private string WaferCompleteEvent = "WaferCompletedPort1";
        private string PostTrackinEvent = "ProcessingStarted";
        private string ValidateSlotMapReceivedEvent = "ProdLoadPort1";
        private string ReadyToUnloadEvent = "PodUnlockedPort1";
        private string MaterialRemovedEvent = "PodRemovedPort1";
        private string ProcessStartedEvent = "ProcessingStartedPort1";
        private string ProcessCompletedEventName = "ProcessingCompletedPort1";
        private string ProcessStateChangeEvent = "ProcessingStateChange";
        private string ControlStateEquipmentOfflineEvent = "EquipmentOffline";
        private string ControlStateLocalEvent = "ControlStateLocal";
        private string ControlStateRemoteEvent = "ControlStateRemote";
        
        #endregion

        #region Test Basics
        [TestInitialize]
        public void TestInit()
        {

            base.Equipment = m_Scenario.GetEquipment(m_Scenario.EquipmentToTest) as SecsGemEquipment;

            base.Initialize(recipeName);
            base.SubMaterialTrackin = subMaterialTrackin;

            base.Equipment.RegisterOnMessage("S1F3", OnS1F3);
            base.Equipment.RegisterOnMessage("S2F41", OnS2F41);

            base.LoadPortNumber = 1;

            loadPortNames.Values.ToList().ForEach(loadPortName =>
            {
                var loadPort = new Resource { Name = loadPortName };
                loadPort.Load();

                if (loadPort.ProcessingType != ProcessingType.LoadPort)
                {
                    Assert.Inconclusive($"LoadPort with name {loadPortName} is not properly set-up");
                }
                loadPort.LoadRelation("ContainerResource");

                if (loadPort.ResourceContainers != null && loadPort.ResourceContainers.Count > 0)
                {
                    loadPort.ResourceContainers.ForEach((container) =>
                    {
                        var containerToUndock = container.SourceEntity;
                        containerToUndock.Load();

                        var undock = new Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects.UndockContainerInput()
                        {
                            Container = containerToUndock,
                            NumberOfRetries = 3
                        };
                        undock.UndockContainerSync();
                    });
                }
            });

            RFIDReader.TestInit(readerResourceName, m_Scenario);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            isOnlineRemote = true;

            receivedStartCommand = false;
            receivedLockPodCommand = false;
            receivedLoadPodCommand = false;
            receivedUnlockPodCommand = false;
            receivedUnloadPodCommand = false;
            receivedSETMIDCommand = false;
            receivedMAPCASSCommand = false;

            proceedWithCarriersReceived = false;

            //regular teardown
            AfterTest();
            RFIDReader.CleanUp(MESScenario);
            base.CleanUp(MESScenario);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ConfigureConnection(readerResourceName, 5014, prepareTestScenario: false);
            ConfigureConnection(resourceName, 5013, isEnableAllAlarms: true, killProcess: false);


            loadPortNames.Values.ToList().ForEach(loadPortName =>
            {
                Resource lp = new Resource() { Name = loadPortName };
                lp.Load();
                lp.AutomationMode = ResourceAutomationMode.Online;
                lp.AutomationAddress = ".";
                lp.Save();
            });

        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Cleanup();
        }

        #endregion Test Basic

        #region Tests FullProcessScenario

        /// <summary> 
        /// Scenario: Recipe Exists on Equipment
        /// </summary>
        [TestMethod]
        public void AmatMirraDesica_FullProcessRecipeExists()
        {
            updateEvents();
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot, false);

            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName, ".\\RecipeBinaryFiles\\testRecipe");

            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();
            var recipeBody = recipe.Body;
            recipeBody.Load();
            RecipeManagement.SetRecipe(recipe.ResourceRecipeName, recipeBody.Body);
            RecipeManagement.FailOnNewBody = true;
            RecipeManagement.RecipeExistsOnList = true;
            samplingPattern = "ALL";
            this.SetSamplingPatternContext(samplingPattern);

            base.RunBasicTest(MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true, fullyAutomatedLoadPorts: true, fullyAutomatedMaterialMovement: true);
        }

        /// <summary> 
        /// Scenario: Recipe Exists on Equipment
        /// </summary>
        [TestMethod]
        public void AmatMirraDesica_SameRecipeOnlineLocal()
        {
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot, false);

            isOnlineRemote = false;
            TrackInMustFail = true;

            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName, ".\\RecipeBinaryFiles\\testRecipe");

            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();
            var recipeBody = recipe.Body;
            recipeBody.Load();
            RecipeManagement.SetRecipe(recipe.ResourceRecipeName, recipeBody.Body);
            RecipeManagement.FailOnNewBody = true;
            RecipeManagement.RecipeExistsOnList = true;


            base.RunBasicTest(MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true, fullyAutomatedLoadPorts: true, fullyAutomatedMaterialMovement: true);
        }


        /// <summary> 
        /// Scenario: Recipe Exists on Equipment
        /// </summary>
        [TestMethod]
        public void AmatMirraDesica_RecipeDoesNotExist()
        {
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot, false);
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot, false);

            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName);
            RecipeManagement.SetRecipe("AnotherRecipe", new Byte[] { 0x32 });

            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();

            TrackInMustFail = true;
            base.RunBasicTest(base.MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true, fullyAutomatedLoadPorts: true, fullyAutomatedMaterialMovement: true);
        }
        #endregion Tests FullProcessScenario 

        #region Test State and Data Collection
        /// <summary> 
        /// Scenario: Control State to Host Offline
        /// </summary>
        [TestMethod]
        public void AmatMirraDesica_ControlStateUpdateTest()
        {

            base.Equipment.Variables["ControlState"] = 1;
            // Trigger event
            base.Equipment.SendMessage(ControlStateEquipmentOfflineEvent, null);

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

                return resource.CurrentStates.FirstOrDefault(s => s.StateModel.Name == "CustomSecsGemControlStateModel" && s.CurrentState.Name == "EquipmentOffline") != null;
            });
            Thread.Sleep(1000);
            base.Equipment.Variables["ControlState"] = 5;
            // Trigger event
            base.Equipment.SendMessage(ControlStateRemoteEvent, null);


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
            base.Equipment.SendMessage(ControlStateLocalEvent, null);

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
        //[TestMethod]
        //public void AmatMirraDesica_EPTStateChangeTest()
        //{

        //    base.Equipment.Variables["BlockedReason"] = 0;
        //    base.Equipment.Variables["BlockedReasonText"] = "NotBlocked";
        //    base.Equipment.Variables["EPTClock"] = "x";
        //    base.Equipment.Variables["EPTState"] = 0;
        //    base.Equipment.Variables["EPTStateTime"] = 3;
        //    base.Equipment.Variables["PreviousEPTState"] = 1;
        //    base.Equipment.Variables["PreviousTaskName"] = "x";
        //    base.Equipment.Variables["PreviousTaskType"] = 1;
        //    base.Equipment.Variables["TaskName"] = "x";
        //    base.Equipment.Variables["TaskType"] = 1;

        //    // Trigger event
        //    base.Equipment.SendMessage(EPTStateChangeEvent, null);

        //    //
        //    TestUtilities.WaitFor(10/*ValidationTimeout*/, "Equipment State was not updated to Idle", () =>
        //    {
        //        Resource resource = new Resource { Name = resourceName };
        //        resource.Load();

        //        var input = new LoadResourceStateModelsInput
        //        {
        //            Resource = resource
        //        }.LoadResourceStateModelsSync();

        //        resource = input.Resource;

        //        if (resource.CurrentStates == null)
        //            return false;

        //        return resource.CurrentStates.FirstOrDefault(s => s.StateModel.Name == "CustomEquipmentPerformanceTrackingStateModel" && s.CurrentState.Name == "Idle") != null;
        //    });
        //    Thread.Sleep(1000);

        //    base.Equipment.Variables["BlockedReason"] = 0;
        //    base.Equipment.Variables["BlockedReasonText"] = "NotBlocked";
        //    base.Equipment.Variables["EPTClock"] = "x";
        //    base.Equipment.Variables["EPTState"] = 1;
        //    base.Equipment.Variables["EPTStateTime"] = 3;
        //    base.Equipment.Variables["PreviousEPTState"] = 1;
        //    base.Equipment.Variables["PreviousTaskName"] = "x";
        //    base.Equipment.Variables["PreviousTaskType"] = 1;
        //    base.Equipment.Variables["TaskName"] = "x";
        //    base.Equipment.Variables["TaskType"] = 1;

        //    // Trigger event
        //    base.Equipment.SendMessage(EPTStateChangeEvent, null);

        //    ////
        //    TestUtilities.WaitFor(10/*ValidationTimeout*/, "Equipment State was not updated to Busy", () =>
        //    {
        //        Resource resource = new Resource { Name = resourceName };
        //        resource.Load();

        //        var input = new LoadResourceStateModelsInput
        //        {
        //            Resource = resource
        //        }.LoadResourceStateModelsSync();

        //        resource = input.Resource;

        //        if (resource.CurrentStates == null)
        //            return false;

        //        return resource.CurrentStates.FirstOrDefault(s => s.StateModel.Name == "CustomEquipmentPerformanceTrackingStateModel" && s.CurrentState.Name == "Busy") != null;
        //    });

        //    Thread.Sleep(1000);

        //    base.Equipment.Variables["BlockedReason"] = 8;
        //    base.Equipment.Variables["BlockedReasonText"] = "Cenas";
        //    base.Equipment.Variables["EPTClock"] = "x";
        //    base.Equipment.Variables["EPTState"] = 2;
        //    base.Equipment.Variables["EPTStateTime"] = 3;
        //    base.Equipment.Variables["PreviousEPTState"] = 1;
        //    base.Equipment.Variables["PreviousTaskName"] = "x";
        //    base.Equipment.Variables["PreviousTaskType"] = 1;
        //    base.Equipment.Variables["TaskName"] = "x";
        //    base.Equipment.Variables["TaskType"] = 1;

        //    // Trigger event
        //    base.Equipment.SendMessage(EPTStateChangeEvent, null);

        //    //
        //    TestUtilities.WaitFor(10/*ValidationTimeout*/, "Equipment State was not updated to Blocked", () =>
        //    {
        //        Resource resource = new Resource { Name = resourceName };
        //        resource.Load();

        //        var input = new LoadResourceStateModelsInput
        //        {
        //            Resource = resource
        //        }.LoadResourceStateModelsSync();

        //        resource = input.Resource;

        //        if (resource.CurrentStates == null)
        //            return false;

        //        return resource.CurrentStates.FirstOrDefault(s => s.StateModel.Name == "CustomEquipmentPerformanceTrackingStateModel" && s.CurrentState.Name == "Blocked") != null;
        //    });

        //}


        /// <summary> 
        /// Scenario: Alarm occurrs, validate ollection of alarm
        /// </summary>
        [TestMethod]
        public void AmatMirraDesica_AlarmDataCollection()
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
            // Trigger event
            base.Equipment.SendMessage(MaterialReceivedEvent, null);

            return true;

        }

        public override void CarrierInValidation(CustomMaterialScenario MESScenario, int loadPortToSet)
        {
            //add carrier id to load port on rfid reader
            RFIDReader.targetIdRFID.Add(loadPortToSet.ToString(), MESScenario.ContainerScenario.Entity.Name);

            base.CarrierInValidation(MESScenario, loadPortToSet);

            //if carried id read succesfull container must now be docked
            ValidatePersistenceContainerExists(LoadPortNumber, MESScenario.ContainerScenario.Entity.Name);
            ValidateContainerIsDocked(MESScenario, loadPortToSet);

            TestUtilities.WaitFor(ValidationTimeout, "Failed to recieve Clamp command", () =>
            {
                return receivedLockPodCommand;
            });

            receivedLockPodCommand = false;

            // Trigger event
            base.Equipment.SendMessage(ReadyToLoadEvent, null);

            TestUtilities.WaitFor(ValidationTimeout, "Failed to recieve Load command", () =>
            {
                return receivedLoadPodCommand;
            });

            receivedLoadPodCommand = false;

            // Trigger event
            base.Equipment.SendMessage(ValidateSlotMapReceivedEvent, null);
        }



        public override bool CarrierOut(CustomMaterialScenario scenario)
        {
            // Trigger event
            base.Equipment.SendMessage(ReadyToUnloadEvent, null);

            ValidateLoadPortState(scenario, LoadPortStateModelStateEnum.ReadyToUnload.ToString());

            // MaterialRemoved
            Thread.Sleep(1000);

            // Trigger event
            base.Equipment.SendMessage(MaterialRemovedEvent, null);

            Thread.Sleep(200);

            return true;
        }

        public override bool PostTrackInActions(CustomMaterialScenario scenario)
        {
            if (TrackInMustFail)
            {
                TestUtilities.WaitForNotChanged(30, String.Format($"Material {scenario.Entity.Name} State is not {MaterialStateModelStateEnum.Setup.ToString()}"), () =>
                {
                    scenario.Entity.Load();
                    if (scenario.Entity.CurrentMainState == null || scenario.Entity.CurrentMainState.CurrentState == null)
                    {
                        return false;
                    }

                    return scenario.Entity.CurrentMainState.CurrentState.Name.Equals(MaterialStateModelStateEnum.Setup.ToString());
                });

                return false;
            }

            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Failed to recieve PP-Select command"), () =>
            {
                return receivedPPSelectCommand;
            });

            receivedPPSelectCommand = false;

            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Failed to recieve SETMID command"), () =>
            {
                return receivedSETMIDCommand;
            });

            receivedSETMIDCommand = false;

            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Failed to recieve MAPCASS command"), () =>
            {
                return receivedMAPCASSCommand;
            });

            receivedMAPCASSCommand = false;

            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Failed to recieve Start command"), () =>
            {
                return receivedStartCommand;
            });

            receivedStartCommand = false;



            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material {scenario.Entity.Name} State is not {MaterialStateModelStateEnum.Setup.ToString()}"), () =>
            {
                scenario.Entity.Load();
                return scenario.Entity.CurrentMainState.CurrentState.Name.Equals(MaterialStateModelStateEnum.Setup.ToString());
            });


            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material {scenario.Entity.Name} System State is not {MaterialSystemState.InProcess.ToString()}"), () =>
            {
                scenario.Entity.Load();
                return scenario.Entity.SystemState.ToString().Equals(MaterialSystemState.InProcess.ToString());
            });

            // Trigger event
            base.Equipment.SendMessage(PostTrackinEvent, null);

            return true;
        }

        public override bool ProcessStartEvent(CustomMaterialScenario scenario)
        {
            base.Equipment.Variables["PPID"] = RecipeName;
            base.Equipment.Variables["JOB_ID_1"] = "No Lot ID";
            base.Equipment.Variables["JOB_ID_2"] = "No Lot ID";
            base.Equipment.Variables["JOB_ID_3"] = "No Lot ID";
            base.Equipment.Variables[$"WAFERSTATUSCASS{LoadPortNumber}"] = 1;
            // Trigger event
            base.Equipment.SendMessage(ProcessStartedEvent, null);

            return true;
        }

        public override bool ProcessCompleteEvent(CustomMaterialScenario scenario)
        {
            base.Equipment.Variables["PPID"] = RecipeName;
            base.Equipment.Variables["JOB_ID_1"] = "No Lot ID";
            base.Equipment.Variables["JOB_ID_2"] = "No Lot ID";
            base.Equipment.Variables["JOB_ID_3"] = "No Lot ID";
            base.Equipment.Variables[$"WAFERSTATUSCASS{LoadPortNumber}"] = 1;
            //// Trigger event
            base.Equipment.SendMessage(ProcessCompletedEventName, null);

            return true;
        }

        public override bool ProcessStateChange(CustomMaterialScenario scenario)
        {
            /*
            //TODO:Update for correct values
            base.Equipment.Variables["ProcessState"] = 4;// Executing
            base.Equipment.Variables["PreviousProcessState"] = 3;

            base.Equipment.SendMessage(ProcessStateChangeEvent, null);
            */
            return true;
        }
        #endregion Events


        public override bool WaferStart(Material wafer)
        {
            wafer.Load();
            wafer.LoadRelations();
            wafer.ParentMaterial.Load();

            base.Equipment.Variables["PPID"] = wafer.ParentMaterial.CurrentRecipeInstance;
            base.Equipment.Variables["JOB_ID_1"] = "No Lot ID";
            base.Equipment.Variables["JOB_ID_2"] = "No Lot ID";
            base.Equipment.Variables["JOB_ID_3"] = "No Lot ID";
           ////// Trigger event
           base.Equipment.SendMessage(WaferStartEvent, null);

            Thread.Sleep(2000);

            return true;
        }

        public override bool WaferComplete(Material wafer)
        {
            wafer.Load();
            wafer.LoadRelations();
            wafer.ParentMaterial.Load();
            wafer.MaterialContainer.First().TargetEntity.Load();

            base.Equipment.Variables["PPID"] = wafer.ParentMaterial.CurrentRecipeInstance.Name;
            base.Equipment.Variables["JOB_ID_1"] = "No Lot ID";
            base.Equipment.Variables["JOB_ID_2"] = "No Lot ID";
            base.Equipment.Variables["JOB_ID_3"] = "No Lot ID";
            ////// Trigger event
            base.Equipment.SendMessage(WaferCompleteEvent, null);

            Thread.Sleep(2000);

            return true;
        }


        public override bool ValidateSubMaterialState(Material submaterial, string subMaterialState)
        {
            //if (MaterialSystemState.Processed.ToString().Equals(subMaterialState))
            //{
            //    submaterial.Load();
            //    submaterial.LoadRelations();
            //    submaterial.ParentMaterial.Load();
            //    submaterial.ParentMaterial.LoadChildren();
            //    if (submaterial.ParentMaterial.SubMaterials
            //        .Where(s => s.SystemState == MaterialSystemState.Queued).Count() == 0
            //        && submaterial.ParentMaterial.SubMaterials
            //        .Where(s => s.SystemState == MaterialSystemState.InProcess).Count() == 1)
            //    {
            //        return true;
            //    }
            //}

            //TestUtilities.WaitFor(90, String.Format($"Material {submaterial.Name} State is not {subMaterialState}"), () =>
            //{
            //    submaterial.Load();
            //    return submaterial.SystemState.ToString().Equals(subMaterialState);
            //});

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

        protected virtual bool OnS2F41(SecsMessage request, SecsMessage reply)
        {
            string command = request.Item.GetChildList()[0].GetValue().ToString();
            var CommandSuccess = false;

            if (command == "PP_SELECT")
            {
                receivedPPSelectCommand = true;
                CommandSuccess = true;
            }
            if (command == "START")
            {
                receivedStartCommand = true;
                CommandSuccess = true;
            }
            if (command == "LOCK_POD")
            {
                receivedLockPodCommand = true;
                CommandSuccess = true;
            }
            if (command == "LOAD_POD")
            {
                receivedLoadPodCommand = true;
                CommandSuccess = true;
            }
            if (command == "UNLOAD_POD")
            {
                receivedUnloadPodCommand = true;
                CommandSuccess = true;
            }
            if (command == "UNLOCK_POD")
            {
                receivedUnlockPodCommand = true;
                CommandSuccess = true;
            }
            if (command == "SETMID")
            {
                receivedSETMIDCommand = true;
                CommandSuccess = true;
            }
            if (command == "MAPCASS")
            {
                receivedMAPCASSCommand = true;
                CommandSuccess = true;
            }
            reply.Item.GetChildList()[0].Binary = new byte[] { (byte)(CommandSuccess ? 0x04 : 0x02) };
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

        private void SetSamplingPatternContext(string patternName, bool clearTable = true)
        {
            SmartTable table = new GetSmartTableByNameInput { SmartTableName = "StepSamplingPatternContext", LoadData = true }.GetSmartTableByNameSync().SmartTable;

            //table = new LoadSmartTableDataInput { SmartTable = (table as SmartTable) }.LoadSmartTableDataSync().SmartTable;
            DataSet ds = Cmf.TestScenarios.Others.Utilities.ToDataSet((table as SmartTable).Data);



            if (clearTable)
            {

                var drToDelete = ds.Tables[0].AsEnumerable().FirstOrDefault(drow => drow["Step"].ToString() == stepName);

                if (drToDelete != null)
                {
                    var dsToDelete = ds.Clone();

                    dsToDelete.Tables[0].Rows.Add(drToDelete.ItemArray);

                    var inputClear = new RemoveSmartTableRowsInput { SmartTable = table, Table = Cmf.TestScenarios.Others.Utilities.FromDataSet(dsToDelete) };
                    inputClear.RemoveSmartTableRowsSync();
                }
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                ds.Tables[0].Rows.Clear();
                ds.Tables[0].AcceptChanges();
            }

            if (patternName != "ALL")
            {
                DataRow dr = ds.Tables[0].NewRow();
                dr["SamplingPattern"] = patternName;
                dr["StepSamplingPatternContextId"] = 0;
                dr["LastServiceHistoryId"] = -1;
                dr["LastOperationHistorySeq"] = -1;
                dr["Step"] = stepName;
                ds.Tables[0].Rows.Add(dr);

            }

            try
            {


                var input = new InsertOrUpdateSmartTableRowsInput { SmartTable = table, Table = Cmf.TestScenarios.Others.Utilities.FromDataSet(ds) };
                var insert = input.InsertOrUpdateSmartTableRowsSync();
            }
            catch (Exception)
            {
            }
        }

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

        public void updateEvents()
        {
            MaterialReceivedEvent = $"PodArrivedPort{LoadPortNumber}";
            ReadyToLoadEvent = $"PodLockedPort{LoadPortNumber}";
            WaferStartEvent = $"WaferStartedPort{LoadPortNumber}";
            WaferCompleteEvent = $"WaferCompletedPort{LoadPortNumber}";
            ValidateSlotMapReceivedEvent = $"ProdLoadPort{LoadPortNumber}";
            ReadyToUnloadEvent = $"PodUnlockedPort{LoadPortNumber}";
            MaterialRemovedEvent = $"PodRemovedPort{LoadPortNumber}";
            ProcessStartedEvent = $"ProcessingStartedPort{LoadPortNumber}";
            ProcessCompletedEventName = $"ProcessingCompletedPort{LoadPortNumber}";
        }

    }

    
}