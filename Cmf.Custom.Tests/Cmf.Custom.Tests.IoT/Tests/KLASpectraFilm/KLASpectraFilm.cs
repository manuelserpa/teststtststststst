using amsOSRAMEIAutomaticTests.Objects.Extensions;
using amsOSRAMEIAutomaticTests.Objects.Utilities;
using cmConnect.TestFramework.Common.Utilities;
using cmConnect.TestFramework.EquipmentSimulator.Drivers;
using cmConnect.TestFramework.EquipmentSimulator.Objects;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.Tests.IoT.Tests.Common;
using Cmf.Custom.Tests.IoT.Tests.HermosLFM4xReader;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.OutputObjects;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects;
using Cmf.SECS.Driver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;

namespace amsOSRAMEIAutomaticTests.KLASpectraFilm
{
    [TestClass]
    public class KLASpectraFilm : CommonTests
    {
        private const string resourceName = "5FELL1";

        public const int numberOfWafersPerLot = 3;

        public const string stepName = "M2-LT-MueTec-CD-Measurement-00820F053_E";
        public const string flowName = "FOL-UX3_EPA";

        public const bool subMaterialTrackin = true;

        public string recipeName = "TestRecipeForKLASpectraFilm";
        public const string serviceName = "CD-Measurement";

        private int loadPortNumber = 1;
        private string samplingPattern = "";

        private bool loadCommandReceived = false;
        private bool loadCommandDenied = false;

        private bool unloadCommandReceived = false;
        private bool unloadCommandDenied = false;

        private bool isOnlineRemote = true;

        public bool createControlJobReceived = false;
        public bool createControlJobDenied = false;

        public bool failAtProcessJob = false;
        public bool createProcessJobReceived = false;
        public bool createProcessJobDenied = false;

        public bool isValidProceedWithCarrier = true;
        public bool proceedWithCarriersReceived = false;


        private int chamberToProcess = 1;


        public HermosLFM4xReader RFIDReader = new HermosLFM4xReader();
        public const string readerResourceName = "5FELL1.RFID";
        #region Event Names
        private const string MaterialReceivedEvent = "CarrierPlaced";
        private const string WaferStartEvent = "WaferStart";
        private const string WaferCompleteEvent = "WaferComplete";
        //private const string PostTrackinEvent = "E87WaitingForHost2SlotMapVerificationOk";
        private const string SlotMapReceivedEvent = "SlotMapNotReadToWaitingForHost";
        private const string ReadyToUnloadEvent = "ReadyToUnload";
        private const string MaterialRemovedEvent = "MaterialRemove";
        private const string ProcessStartedEvent = "ProcessStart";
        private const string ProcessCompletedEvent = "ProcessComplete";
        private const string ProcessStateChangeEvent = "PRJobStateChange";
        //private const string EPTStateChangeEvent = "EPTStateChange";
        private const string ControlStateEquipmentOfflineEvent = "EquipmentOffline";
        private const string ControlStateLocalEvent = "ControlStateLocal";
        private const string ControlStateRemoteEvent = "ControlStateRemote";
        #endregion

        private enum ControlJobStates { QUEUED = 0, SELECTED, WAITINGFORSTART, EXECUTING, PAUSED, COMPLETED }
        private enum SubstrateStates { ATSource = 0, ATWork, ATDestination }
        private enum SubstratProcessStates { NeedProcessing = 0, InProcess, Processed, Aborted, Stopped, Rejected, Lost, Skipped }
       
        #region Test Basics

        [TestInitialize]
        public void TestInit()
        {

            base.Equipment = m_Scenario.GetEquipment(m_Scenario.EquipmentToTest) as SecsGemEquipment;

            base.Initialize(recipeName);
            base.SubMaterialTrackin = subMaterialTrackin;

            base.Equipment.RegisterOnMessage("S1F3", OnS1F3);

            base.Equipment.RegisterOnMessage("S3F17", OnS3F17);
            base.Equipment.RegisterOnMessage("S14F9", OnS14F9);
            base.Equipment.RegisterOnMessage("S16F11", OnS16F11);

            base.LoadPortNumber = loadPortNumber;

            RFIDReader.TestInit(readerResourceName, m_Scenario);
        }

        [TestCleanup]
        public void TestCleanup()
        {

            loadCommandReceived = false;
            loadCommandDenied = false;

            unloadCommandReceived = false;
            unloadCommandDenied = false;

            isOnlineRemote = true;

            createControlJobReceived = false;
            createControlJobDenied = false;

            createProcessJobReceived = false;
            createProcessJobDenied = false;

            proceedWithCarriersReceived = false;

            //regular teardown
            AfterTest();
            RFIDReader.CleanUp(MESScenario);
            base.CleanUp(MESScenario);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var step = new Step { Name = stepName };
            step.Load();
            step.UseInStepSampling = true;
            step.Save();

            ConfigureConnection(readerResourceName, 5012, prepareTestScenario: false);
            ConfigureConnection(resourceName, 5011, killProcess: false);

            Resource lp1 = new Resource() { Name = $"{resourceName}-LP1" };
            lp1.Load();
            lp1.AutomationMode = ResourceAutomationMode.Online;
            lp1.AutomationAddress = ".";
            lp1.Save();
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
        public void KLASpectraFilm_FullProcessRecipeExists()
        {
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
        public void KLASpectraFilm_SameRecipeOnlineLocal()
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
        public void KLASpectraFilm_RecipeDoesNotExist()
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
        public void KLASpectraFilm_ControlStateUpdateTest()
        {

            base.Equipment.Variables["ControlState"] = EquipmentControlStateEnum.EquipmentOffline;
            // Trigger event
            base.Equipment.SendMessage(ControlStateEquipmentOfflineEvent, null);

            WaitForControlState("EquipmentOffline", "Control State was not updated to Host Offline");
            //Thread.Sleep(1000);

            base.Equipment.Variables["ControlState"] = EquipmentControlStateEnum.OnlineLocal;
            // Trigger event
            base.Equipment.SendMessage(ControlStateLocalEvent, null);
            WaitForControlState("OnlineLocal", "Control State was not updated to Online Local");
            //Thread.Sleep(1000);

            base.Equipment.Variables["ControlState"] = EquipmentControlStateEnum.OnlineRemote;
            // Trigger event
            base.Equipment.SendMessage(ControlStateRemoteEvent, null);
            WaitForControlState("OnlineRemote", "Control State was not updated to Online Remote");
        }


        /// <summary> 
        /// Scenario: Control State to Host Offline
        /// </summary>
        //[TestMethod]
        //public void KLASpectraFilm_EPTStateChangeTest()
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
        public void KLASpectraFilm_AlarmDataCollection()
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

            base.Equipment.Variables["PortXID"] = loadPortToSet;
            base.Equipment.SendMessage(MaterialReceivedEvent, null);

            return true;

        }

        public override void CarrierInValidation(CustomMaterialScenario MESScenario, int loadPortToSet)
        {
            //add carrier id to load port on rfid reader
            RFIDReader.targetIdRFID.Add(loadPortToSet.ToString(), MESScenario.ContainerScenario.Entity.Name);

            base.CarrierInValidation(MESScenario, loadPortToSet);

            //Send the event for UnknownCarrierID
            base.Equipment.Variables["PortID"] = loadPortToSet;

            // Trigger event
            // base.Equipment.SendMessage(String.Format($"UnknownCarrierID"), null);

            // Thread.Sleep(300);

            //if carried id read succesfull container must now be docked
            ValidatePersistenceContainerExists(LoadPortNumber, MESScenario.ContainerScenario.Entity.Name);
            ValidateContainerIsDocked(MESScenario, loadPortToSet);

            // ValidateProceedWithCarrierReceived(1);

            TestUtilities.WaitFor(30, "Proceed With Carrier (Accept Container) not Received", () =>
            {
                return proceedWithCarriersReceived;
            });
            //setting to false to next proceed with carrier
            proceedWithCarriersReceived = false;
            if (!isValidProceedWithCarrier)
                Assert.Fail("Wrong ProceedWithCarrier Format!");

            var slotMap = new int[13];
            // scenario.ContainerScenario.Entity
            if (MESScenario.ContainerScenario.Entity.ContainerMaterials != null)
            {
                for (int i = 0; i < 13; i++)
                {
                    slotMap[i] = MESScenario.ContainerScenario.Entity.ContainerMaterials.Exists(p => p.Position != null && p.Position == i + 1) ? 3 : 1;
                }
            }
            SlotMapVariable slotMapDV = new SlotMapVariable(base.Equipment) { Presence = slotMap };



            base.Equipment.Variables["CarrierID"] = MESScenario.ContainerScenario.Entity.Name;
            base.Equipment.Variables["SlotMap"] = slotMapDV;
            base.Equipment.Variables["PortID"] = loadPortToSet;

            // Trigger event
            base.Equipment.SendMessage(SlotMapReceivedEvent, null);

            ValidatePersistenceContainerExists(loadPortToSet);
        }



        public override bool CarrierOut(CustomMaterialScenario scenario)
        {

            base.Equipment.Variables["PortXID"] = loadPortNumber;

            // Trigger event
            base.Equipment.SendMessage(ReadyToUnloadEvent, null);

            ValidateLoadPortState(scenario, LoadPortStateModelStateEnum.ReadyToUnload.ToString());
            // MaterialRemoved

            Thread.Sleep(2000);
            base.Equipment.Variables["PortID"] = loadPortNumber;

            // Trigger event
            base.Equipment.SendMessage(MaterialRemovedEvent, null);

            Thread.Sleep(200);


            return true;
        }


        public override bool ProcessStartEvent(CustomMaterialScenario scenario)
        {
            //base.Equipment.Variables["CtrlJobID"] = $"CtrlJob_{scenario.Entity.Name}";
            //base.Equipment.Variables["CtrlJobState"] = (int)ControlJobStates.EXECUTING;

            base.Equipment.Variables["PRJobID"] = $"PrJob_{scenario.Entity.Name}";
            
            //// Trigger event
            base.Equipment.SendMessage(ProcessStartedEvent, null);

            Thread.Sleep(2000);

            return true;
        }

        public override bool ProcessCompleteEvent(CustomMaterialScenario scenario)
        {
            base.Equipment.Variables["PRJobID"] = $"PrJob_{scenario.Entity.Name}";

            //// Trigger event
            base.Equipment.SendMessage(ProcessCompletedEvent, null);

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


            if (!isOnlineRemote)
            {
                Assert.Fail("Track In must fail on Online Local");
            }

            //TestUtilities.WaitFor(30, "Proceed With Carrier (Accept Slot Map) not Received", () =>
            //{
            //    return proceedWithCarriersReceived;
            //});

            TestUtilities.WaitFor(60, String.Format($"Material {scenario.Entity.Name} State is not {MaterialStateModelStateEnum.Setup.ToString()}"), () =>
            {
                scenario.Entity.Load();

                if (scenario.Entity.CurrentMainState == null || scenario.Entity.CurrentMainState.CurrentState == null)
                {
                    return false;
                }

                return scenario.Entity.CurrentMainState.CurrentState.Name.Equals(MaterialStateModelStateEnum.Setup.ToString());
            });

            TestUtilities.WaitFor(60, String.Format($"Material {scenario.Entity.Name} System State is not {MaterialSystemState.InProcess.ToString()}"), () =>
            {
                scenario.Entity.Load();
                return scenario.Entity.SystemState.ToString().Equals(MaterialSystemState.InProcess.ToString());
            });

            scenario.Entity.LoadChildren();
            scenario.ContainerScenario.Entity.LoadRelations(levelsToLoad: 2);

            if (samplingPattern == "FIRST")
            {
                var material = scenario.Entity.SubMaterials.Single(c => c.SystemState.ToString().Equals(MaterialSystemState.Queued.ToString()));

                var materialContainer = scenario.ContainerScenario.Entity.ContainerMaterials.Single(p => p.SourceEntity.Name == material.Name && p.Position == 1);

            }

            if (samplingPattern == "MIDDLE")
            {
                var material = scenario.Entity.SubMaterials.Single(c => c.SystemState.ToString().Equals(MaterialSystemState.Queued.ToString()));

                int halfIndex = (scenario.Entity.SubMaterials.Count()) / 2;
                //There is a bug here... Just for the sake of the validation
                var materialContainer = scenario.ContainerScenario.Entity.ContainerMaterials.Single(p => p.SourceEntity.Name == material.Name && p.Position == (halfIndex + 2));
            }

            if (samplingPattern == "LAST")
            {
                var material = scenario.Entity.SubMaterials.Single(c => c.SystemState.ToString().Equals(MaterialSystemState.Queued.ToString()));

                var materialContainer = scenario.ContainerScenario.Entity.ContainerMaterials.First(p => p.SourceEntity.Name == material.Name && p.Position == scenario.Entity.SubMaterials.Count);
            }



            //base.Equipment.Variables["CarrierID"] = scenario.ContainerScenario.Entity.Name;
            //base.Equipment.Variables["PortID"] = loadPortNumber;

            //base.Equipment.SendMessage(PostTrackinEvent, null);

            TestUtilities.WaitFor(60, String.Format($"Process Job creation requests were never received"), () =>
            {
                return createProcessJobReceived;
            });

            TestUtilities.WaitFor(60, String.Format($"Control creation requests were never received"), () =>
            {
                return createControlJobReceived;
            });

            Thread.Sleep(100);
            return true;
        }

        public bool SendMetrics()
        {
            var message = new SecsTransaction(this.Equipment.Driver);
            message.Primary = new SecsMessage();
            message.Primary.Stream = 6;
            message.Primary.Function = 11;
            message.Primary.Item = new SecsItem();
            message.Primary.Item.SetTypeToList();
            message.Primary.Item.Add(new SecsItem { Name = "DATAID", U4 = new uint[] { 157443 } });
            message.Primary.Item.Add(new SecsItem { Name = "CEID", U4 = new uint[] { 704 } });
            var eventreports = new SecsItem();
            eventreports.SetTypeToList();
            eventreports.Add(new SecsItem());
            eventreports[0].Add(new SecsItem() { Name = "RPTID", U4 = new uint[] { 999 } });
            message.Primary.Item.Add(eventreports);
            eventreports[0].Add(new SecsItem());
            eventreports[0][1].SetTypeToList();
            var data = File.ReadAllText(@"Tests\KLASpectraFilm\KLASpectraFilmDataCollection.json");
            dynamic ser = JsonConvert.DeserializeObject<dynamic>(data);
            foreach (var item in ser["values"])
            {
                eventreports[0][1].Add(GetSecsItemFromValue(item["originalValue"]));
            }
            base.Equipment.SendMessage(message);
            Thread.Sleep(5000);
            return true;
        }

        private SecsItem GetSecsItemFromValue(dynamic originalValue)
        {
            var newItem = new SecsItem();

            if (originalValue.type == "L")
            {
                //var values = new List<SecsItem>();
                newItem.SetTypeToList();
                foreach (var val in originalValue["value"])
                {
                    newItem.Add(GetSecsItemFromValue(val));
                    //values.Add(GetSecsItemFromValue(val));
                }
                //newItem.SetValue(SecsItem.ItemType.List, values);
            }
            else if (originalValue.type == "A")
            {
                newItem.ASCII = originalValue["value"];
            }
            else if (originalValue.type == "U1")
            {
                newItem.U1 = new byte[] { originalValue["value"] };
            }
            else if (originalValue.type == "I4")
            {
                var value = originalValue["value"];
                var valArray = new List<int>();
                if (value.GetType().Name == "JArray")
                {
                    foreach (var val in value)
                    {
                        valArray.Add((int)val);
                    }
                }
                else
                {
                    valArray.Add((int)value);
                }
                newItem.I4 = valArray.ToArray();
            }
            else if (originalValue.type == "I2")
            {
                var value = originalValue["value"];
                var valArray = new List<short>();
                if (value.GetType().Name == "JArray")
                {
                    foreach (var val in value)
                    {
                        valArray.Add((short)val);
                    }
                }
                else
                {
                    valArray.Add((short)value);
                }
                newItem.I2 = valArray.ToArray();
            }
            else if (originalValue.type == "F8")
            {
                var value = originalValue["value"];
                var valArray = new List<double>();
                if (value.GetType().Name == "JArray")
                {
                    foreach (var val in value)
                    {
                        valArray.Add((double)val);
                    }
                }
                else
                {
                    valArray.Add((double)value);
                }
                newItem.F8 = valArray.ToArray();
            }
            else if (originalValue.type == "U4")
            {
                var value = originalValue["value"];
                var valArray = new List<uint>();
                if (value.GetType().Name == "JArray")
                {
                    foreach (var val in value)
                    {
                        valArray.Add((uint)val);
                    }
                }
                else
                {
                    valArray.Add((uint)value);
                }
                newItem.U4 = valArray.ToArray();
            }
            else if (originalValue.type == "U2")
            {
                var value = originalValue["value"];
                var valArray = new List<ushort>();
                if (value.GetType().Name == "JArray")
                {
                    foreach (var val in value)
                    {
                        valArray.Add((ushort)val);
                    }
                }
                else
                {
                    valArray.Add((ushort)value);
                }
                newItem.U2 = valArray.ToArray();
            }

            return newItem;
        }

        public override bool WaferStart(Material wafer)
        {
            //wafer.Load();
            //wafer.LoadRelations();
            //wafer.ParentMaterial.Load();
            //wafer.MaterialContainer.First().TargetEntity.Load();
            //var position = (wafer.MaterialContainer.First().Position ?? 0).ToString("00");
            //var containerId = MESScenario.ContainerScenario.Entity.Name;
            //var subLocId = $"{containerId}.{position}";

            //base.Equipment.Variables["SlotId"] = position;
            ////// Trigger event
            //base.Equipment.SendMessage(WaferStartEvent, null);
            SendWaferMessage(wafer, 2071);

            Thread.Sleep(2000);
            try
            {
                SendMetrics();
            }
            catch
            {
                Assert.Inconclusive("Error send Metrics");
            }

            return true;
        }

        public override bool WaferComplete(Material wafer)
        {
            //wafer.Load();
            //wafer.LoadRelations();
            //wafer.ParentMaterial.Load();
            //wafer.MaterialContainer.First().TargetEntity.Load();
            //var position = (wafer.MaterialContainer.First().Position ?? 0).ToString("00");

            SendWaferMessage(wafer, 2081);

            Thread.Sleep(2000);
            return true;
        }

        private void SendWaferMessage(Material wafer, uint ceid)
        {
            wafer.ParentMaterial.Load();
            var message = new SecsTransaction(this.Equipment.Driver);
            message.Primary = new SecsMessage();
            message.Primary.Stream = 6;
            message.Primary.Function = 11;
            message.Primary.Item = new SecsItem();
            message.Primary.Item.SetTypeToList();
            message.Primary.Item.Add(new SecsItem { Name = "DATAID", U4 = new uint[] { 157443 } });
            message.Primary.Item.Add(new SecsItem { Name = "CEID", U4 =  new uint[] { ceid } });
            var eventreports = new SecsItem();
            eventreports.SetTypeToList();
            message.Primary.Item.Add(eventreports);
            eventreports.Add(new SecsItem());
            eventreports[0].Add(new SecsItem() { Name = "RPTID", U4 = new uint[] { 26 } });

            var listLb = new SecsItem();
            listLb.SetTypeToList();
            eventreports[0].Add(listLb);

            var listaSubstrate = new SecsItem();
            listaSubstrate.SetTypeToList();
            listLb.Add(listaSubstrate);
            listaSubstrate.Add(new SecsItem());
            listaSubstrate.Add(new SecsItem());
            listaSubstrate[0].Add(new SecsItem() { ASCII = "LotID" });
            listaSubstrate[0].Add(new SecsItem() { ASCII = wafer.ParentMaterial.Name });
            listaSubstrate[1].Add(new SecsItem() { ASCII = "ObjID"});
            listaSubstrate[1].Add(new SecsItem() { ASCII = wafer.Name });

            base.Equipment.SendMessage(message);
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

        public virtual bool OnS3F17(SecsMessage request, SecsMessage reply)
        {
            var requestDataForContainer = request.Item.GetChildList()[2].GetValue().ToString();
            var requestDataForLoadPort = request.Item.GetChildList()[2].GetValue().ToString();
            //var requestDataForParameters = request.Item.GetChildList()[4].GetValue().ToString();

            if (!requestDataForContainer.Equals(base.MESScenario.ContainerScenario.Entity.Name))
            {
                isValidProceedWithCarrier = false;
            }


            reply.Item.Clear();
            var ack = new SecsItem { U1 = new byte[] { 0x00 } };

            var errorList = new SecsItem();
            errorList.SetTypeToList();

            //if (createControlJobDenied)
            //{
            //    ack = new SecsItem { U1 = new byte[] { 0x01 } };

            //    errorList = new SecsItem();
            //    errorList.SetTypeToList();
            //    var error = new SecsItem();
            //    error.SetTypeToList();
            //    error.Add(new SecsItem() { U1 = new byte[] { 7 } });
            //    error.Add(new SecsItem() { ASCII = $"{MESScenario.Entity.Name} : RecID : IllegalValue'" });
            //    errorList.Add(error);
            //}

            reply.Item.Add(ack);

            reply.Item.Add(errorList);

            proceedWithCarriersReceived = true;

            return (true);
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

        private void WaitForControlState(string finalState, string errorMessage)
        {
            TestUtilities.WaitFor(ValidationTimeout, errorMessage, () =>
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

                return resource.CurrentStates.FirstOrDefault(s => s.StateModel.Name == "CustomSecsGemControlStateModel" && s.CurrentState.Name == finalState) != null;
            });
        }
        /*private void ValidateProceedWithCarrierReceived(int numberExpected = -1)
        {
            TestUtilities.WaitFor(30, "Notification mismatch.", () =>
            {
                if (numberExpected >= 0)
                {
                    return numberExpected == proceedWithCarriersReceived;
                }
                else
                {
                    return true;
                }


            });
        }*/
    }
}