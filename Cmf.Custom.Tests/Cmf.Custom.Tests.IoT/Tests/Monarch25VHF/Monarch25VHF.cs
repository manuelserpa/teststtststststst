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
using amsOSRAMEIAutomaticTests.Objects.Extensions;
using amsOSRAMEIAutomaticTests.Objects.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cmf.Custom.TestUtilities;
using cmConnect.TestFramework.EquipmentSimulator.Objects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.OutputObjects;
using System.Data;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Custom.Tests.IoT.Tests.Common;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;
using Cmf.Custom.Tests.IoT.Tests.HermosLFM4xReader;
using cmConnect.TestFramework.Common.Interfaces;

namespace amsOSRAMEIAutomaticTests.Monarch25VHF
{
    [TestClass]
    public class Monarch25VHF : CommonTests
    {
        private const string resourceName = "5FVHF1";

        public const int numberOfWafersPerLot = 3;

        public const string stepName = "M2-LT-MueTec-CD-Measurement-00820F053_E";
        public const string flowName = "FOL-UX3_EPA";

        public const bool subMaterialTrackin = true;

        public string recipeName = "TestRecipeForMonarch25VHF";
        public const string serviceName = "CD-Measurement";

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

        public bool recievedStartCommand = false;
        public bool recievedClampPodCommand = false;
        public bool recievedLoadPodCommand = false;
        public bool recievedUnloadPodCommand = false;
        public bool receivedPPSelectCommand = false;


        private int chamberToProcess = 1;


        public HermosLFM4xReader RFIDReader = new HermosLFM4xReader();
        public const string readerResourceName = "5FVHF1.RFID";

        #region Test Basics
        [TestInitialize]
        public void TestInit()
        {

            base.Equipment = m_Scenario.GetEquipment(m_Scenario.EquipmentToTest) as SecsGemEquipment;

            base.Initialize(recipeName);
            base.SubMaterialTrackin = subMaterialTrackin;

            base.Equipment.RegisterOnMessage("S1F3", OnS1F3);
            base.Equipment.RegisterOnMessage("S2F41", OnS2F41);

            base.Equipment.RegisterOnMessage("S3F17", OnS3F17);
            base.Equipment.RegisterOnMessage("S14F9", OnS14F9);
            base.Equipment.RegisterOnMessage("S16F11", OnS16F11);

            base.LoadPortNumber = 1;

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

            recievedStartCommand = false;
            recievedClampPodCommand = false;
            recievedLoadPodCommand = false;
            recievedUnloadPodCommand = false;
            receivedPPSelectCommand = false;

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

            Resource lp1 = new Resource() { Name = "5FVHF1-LP1" };
            lp1.Load();
            lp1.AutomationMode = ResourceAutomationMode.Online;
            lp1.AutomationAddress = ".";
            lp1.Save();

            Resource lp2 = new Resource() { Name = "5FVHF1-LP2" };
            lp2.Load();
            lp2.AutomationMode = ResourceAutomationMode.Online;
            lp2.AutomationAddress = ".";
            lp2.Save();
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
        public void Monarch25VHF_FullProcessRecipeExists()
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
        public void Monarch25VHF_SameRecipeOnlineLocal()
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
        public void Monarch25VHF_RecipeDoesNotExist()
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
        public void Monarch25VHF_ControlStateUpdateTest()
        {
            base.Equipment.Variables["CONTROL_STATE"] = 1;
            // Trigger event
            base.Equipment.SendMessage("EquipmentOffline", null);

            //
            TestUtilities.WaitFor(ValidationTimeout, "Control State was not updated to Equipment Offline", () =>
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

            base.Equipment.Variables["CONTROL_STATE"] = 5;
            // Trigger event
            base.Equipment.SendMessage("ControlStateRemote", null);

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

            base.Equipment.Variables["CONTROL_STATE"] = 4;
            // Trigger event
            base.Equipment.SendMessage("ControlStateLocal", null);

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
        public void Monarch25VHF_EPTStateChangeTest()
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
        public void Monarch25VHF_AlarmDataCollection()
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
            base.Equipment.Variables.Clear();
            base.Equipment.Variables["PORT_ID"] = LoadPortNumber;
            // Trigger event

            switch (loadPortToSet)
            {
                case 1:
                    base.Equipment.SendMessage(String.Format($"SMIFPodPresent1"), null);
                    break;
                case 2:
                    base.Equipment.SendMessage(String.Format($"SMIFPodPresent2"), null);
                    break;

                default:
                    break;
            }


            return true;
        }

        public override void CarrierInValidation(CustomMaterialScenario MESScenario, int loadPortToSet)
        {
            //add carrier id to load port on rfid reader
            RFIDReader.targetIdRFID.Add(loadPortToSet.ToString(), MESScenario.ContainerScenario.Entity.Name);

            base.CarrierInValidation(MESScenario, loadPortToSet);

            //   Thread.Sleep(300);

            //if carried id read succesfull container must now be docked
            ValidatePersistenceContainerExists(LoadPortNumber, MESScenario.ContainerScenario.Entity.Name);
            ValidateContainerIsDocked(MESScenario, loadPortToSet);

            TestUtilities.WaitFor(ValidationTimeout, "Failed to recieve Clamp command", () => {
                return recievedClampPodCommand;
            });

            recievedClampPodCommand = false;

            base.Equipment.Variables.Clear();

            base.Equipment.Variables["PORT_ID"] = LoadPortNumber;

            switch (loadPortToSet)
            {
                case 1:
                    base.Equipment.SendMessage("SMIFPodClamped1", null);
                    break;
                case 2:
                    base.Equipment.SendMessage("SMIFPodClamped2", null);
                    break;

                default:
                    break;
            }

            var slotMap = new SecsItem();
            slotMap.SetTypeToList();
            // scenario.ContainerScenario.Entity
            if (MESScenario.ContainerScenario.Entity.ContainerMaterials != null)
            {
                for (int i = 0; i < 13; i++)
                {
                    var temp = new SecsItem();

                    if (MESScenario.ContainerScenario.Entity.ContainerMaterials.Exists(p => p.Position != null && p.Position == i + 1))
                    {
                        temp.U1 = new byte[] { 0x01 };
                    }
                    else
                    {
                        temp.U1 = new byte[] { 0x00 };
                    }

                    slotMap.Add(temp);
                }
            }

            //SlotMapVariable slotMapDV = new SlotMapVariable(base.Equipment) { Presence = slotMap };

            TestUtilities.WaitFor(ValidationTimeout, "Failed to recieve LOAD_POD command", () => {
                return recievedLoadPodCommand;
            });

            recievedLoadPodCommand = false;

            SecsItem outerList = new SecsItem();
            outerList.Add(slotMap);

            base.Equipment.Variables["SLOT_MAP"] = outerList;
            base.Equipment.Variables["PORT_ID"] = new SecsItem() { U1 = new byte[] { (byte)loadPortToSet } };

            // Trigger event
            base.Equipment.SendMessage(sendCustomMessage(String.Format($"SlotMapRead")), null);

            ValidatePersistenceContainerExists(loadPortToSet);
        }


        private SecsTransaction sendCustomMessage(string messageName)
        {
            IEvent @event = base.Equipment.Events[messageName];
            SecsTransaction secsTransaction = base.Equipment.Driver.Library.GetTransaction("S6F11").Duplicate();

            secsTransaction.Primary.Item.GetChildList()[0].U4 = new uint[] { 1 };

            secsTransaction.Primary.Item.GetChildList()[1].U4 = new uint[] { uint.Parse(@event.DataItemId) };

            SecsItem secsItem = secsTransaction.Primary.Item.GetChildList()[2];
            secsItem.Clear();
            foreach (IReport report in @event.Reports)
            {
                SecsItem secsItem2 = new SecsItem();
                secsItem2.SetTypeToList();
                SecsItem secsItem3 = new SecsItem();
                secsItem3.U4 = new uint[] { uint.Parse(report.DataItemId) };
                secsItem2.Add(secsItem3);
                SecsItem secsItem4 = new SecsItem();
                secsItem4.SetTypeToList();

                foreach (IVariable variable in report.Variables)
                {
                    if (base.Equipment.Variables.ContainsKey(variable.AbstractName))
                    {
                        secsItem4.Add(base.Equipment.Variables[variable.AbstractName] as SecsItem);
                    }
                }

                secsItem2.Add(secsItem4);

                secsItem.Add(secsItem2);
            }

            return secsTransaction;
        }





        public override bool CarrierOut(CustomMaterialScenario scenario)
        {

            base.Equipment.Variables["PORT_ID"] = LoadPortNumber;

            // Trigger event
            switch (LoadPortNumber)
            {
                case 1:
                    base.Equipment.SendMessage("VCEAUnloadComplete", null);
                    break;
                case 2:
                    base.Equipment.SendMessage("VCEBUnloadComplete", null);
                    break;

                default:
                    break;
            }


            ValidateLoadPortState(scenario, LoadPortStateModelStateEnum.ReadyToUnload.ToString());
            // MaterialRemoved

            Thread.Sleep(2000);
            base.Equipment.Variables["PORT_ID"] = LoadPortNumber;

            // Trigger event
            switch (LoadPortNumber)
            {
                case 1:
                    base.Equipment.SendMessage("SMIFPodAbsent1", null);
                    break;
                case 2:
                    base.Equipment.SendMessage("SMIFPodAbsent2", null);
                    break;

                default:
                    break;
            }

            ValidateLoadPortState(scenario, LoadPortStateModelStateEnum.ReadyToLoad.ToString());

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

            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Failed to recieve PP-Select"), () =>
            {
                return receivedPPSelectCommand;
            });
            receivedPPSelectCommand = false;

            Thread.Sleep(500);

            switch (LoadPortNumber)
            {
                case 1:
                    base.Equipment.SendMessage("ReadyForProcessA", null);
                    break;
                case 2:
                    base.Equipment.SendMessage("ReadyForProcessB", null);
                    break;

                default:
                    break;
            }

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



            return true;
        }



        public override bool ProcessStartEvent(CustomMaterialScenario scenario)
        {
            TestUtilities.WaitFor(ValidationTimeout, "Failed to recieve START command", () =>
            {
                return recievedStartCommand;
            });

            recievedStartCommand = false;

            base.Equipment.Variables.Clear();

            base.Equipment.Variables["CARRIER_ID"] = MESScenario.ContainerScenario.Entity.Name;
            base.Equipment.Variables["LOT_ID"] = MESScenario.Entity.Name;
            base.Equipment.Variables["PORT_ID"] = LoadPortNumber;

            base.Equipment.SendMessage("CassetteStarted", null);

            return true;
        }

        public override bool ProcessCompleteEvent(CustomMaterialScenario scenario)
        {

            base.Equipment.Variables["CARRIER_ID"] = MESScenario.ContainerScenario.Entity.Name;
            base.Equipment.Variables["LOT_ID"] = MESScenario.Entity.Name;
            base.Equipment.Variables["PORT_ID"] = LoadPortNumber;

            //// Trigger event
            base.Equipment.SendMessage("CassetteComplete", null);

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


        public override bool WaferStart(Material wafer)
        {
            wafer.Load();
            wafer.LoadRelations();
            wafer.ParentMaterial.Load();
            wafer.MaterialContainer.First().TargetEntity.Load();

            base.Equipment.Variables["WAFER_ID"] = wafer.Name;
            base.Equipment.Variables["LOT_ID"] = wafer.ParentMaterial.Name;

            var lpId = LoadPortNumber == 1 ? "A" : "B";

            base.Equipment.Variables["WaferNo"] = lpId + MESScenario.ContainerScenario.Entity.ContainerMaterials.Where(x => x.Name.Contains(wafer.Name)).FirstOrDefault().Position;

            ////// Trigger event
            base.Equipment.SendMessage("WaferStarted", null);

            return true;
        }

        public override bool WaferComplete(Material wafer)
        {
            wafer.Load();
            wafer.LoadRelations();
            wafer.ParentMaterial.Load();
            wafer.MaterialContainer.First().TargetEntity.Load();

            base.Equipment.Variables["WAFER_ID"] = wafer.Name;
            base.Equipment.Variables["LOT_ID"] = wafer.ParentMaterial.Name;

            var lpId = LoadPortNumber == 1 ? "A" : "B";

            base.Equipment.Variables["WaferNo"] = lpId + MESScenario.ContainerScenario.Entity.ContainerMaterials.Where(x => x.Name.Contains(wafer.Name)).FirstOrDefault().Position;

            ////// Trigger event
            base.Equipment.SendMessage("WaferComplete", null);

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
                if (base.Equipment.EquipmentVariables["CONTROL_STATE"].DataItemId == ecid.ToString())
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
                recievedStartCommand = true;
                CommandSuccess = true;
            }
            if (command == "CLAMP")
            {
                recievedClampPodCommand = true;
                CommandSuccess = true;
            }
            if (command == "LOAD")
            {
                recievedLoadPodCommand = true;
                CommandSuccess = true;
            }

            if (command == "UNLOAD")
            {
                recievedUnloadPodCommand = true;
                CommandSuccess = true;
            }
            reply.Item.GetChildList()[0].Binary = new byte[] { (byte)(CommandSuccess ? 0x00 : 0x02) };
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