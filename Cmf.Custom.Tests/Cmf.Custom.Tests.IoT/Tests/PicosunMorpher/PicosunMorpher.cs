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
using System.Collections.Generic;

namespace amsOSRAMEIAutomaticTests.PicosunMorpher
{
    [TestClass]
    public class PicosunMorpher : CommonTests
    {
        private const string resourceName = "5FALD2";

        public const int numberOfWafersPerLot = 3;

        public const string stepName = "M2-LT-MueTec-CD-Measurement-00820F053_E";
        public const string flowName = "FOL-UX3_EPA";

        public const bool subMaterialTrackin = true;

        public string recipeName = "TestRecipeForPicosunMorpher";
        public const string serviceName = "CD-Measurement";

        private string samplingPattern = "";

        private bool isOnlineRemote = true;

        public bool recievedStartJobCommand = false;
        public bool receivedCreateJobWaferListCommand = false;

        public int LoadPort1 = 14;
        public int LoadPort2 = 15;
        public int LoadPort3 = 16;
        public int LoadPort = 0;

        public string jobId = "TestJobIdForPicosunMorpher";


        public HermosLFM4xReader RFIDReader = new HermosLFM4xReader();
        public const string readerResourceName = "5FALD2.RFID";

        #region Test Basics
        [TestInitialize]
        public void TestInit()
        {

            base.Equipment = m_Scenario.GetEquipment(m_Scenario.EquipmentToTest) as SecsGemEquipment;

            base.Initialize(recipeName);
            base.SubMaterialTrackin = subMaterialTrackin;

            base.Equipment.RegisterOnMessage("S1F3", OnS1F3);
            base.Equipment.RegisterOnMessage("S2F49", OnS2F49);

            base.LoadPortNumber = 1;

            RFIDReader.TestInit(readerResourceName, m_Scenario);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            isOnlineRemote = true;

            recievedStartJobCommand = false;
            receivedCreateJobWaferListCommand = false;

            LoadPort = 0;

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

            Resource lp1 = new Resource() { Name = "5FALD2-LP1" };
            lp1.Load();
            lp1.AutomationMode = ResourceAutomationMode.Online;
            lp1.AutomationAddress = ".";
            lp1.Save();

            Resource lp2 = new Resource() { Name = "5FALD2-LP2" };
            lp2.Load();
            lp2.AutomationMode = ResourceAutomationMode.Online;
            lp2.AutomationAddress = ".";
            lp2.Save();

            Resource lp3 = new Resource() { Name = "5FALD2-LP3" };
            lp3.Load();
            lp3.AutomationMode = ResourceAutomationMode.Online;
            lp3.AutomationAddress = ".";
            lp3.Save();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Cleanup();
        }

        #endregion Test Basic

        #region Tests FullProcessScenario

        /// <summary> 
        /// Scenario: Recipe Exists on Equipment for LP1
        /// </summary>
        [TestMethod]
        public void PicosunMorpher_FullProcessRecipeExistsLoadPort1()
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

            LoadPort = LoadPort1;
            base.LoadPortNumber = 1;

            base.RunBasicTest(MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true, fullyAutomatedLoadPorts: true, fullyAutomatedMaterialMovement: true);
        }

        /// <summary> 
        /// Scenario: Recipe Exists on Equipment for LP2
        /// </summary>
        [TestMethod]
        public void PicosunMorpher_FullProcessRecipeExistsLoadPort2()
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

            LoadPort = LoadPort2;
            base.LoadPortNumber = 2;

            base.RunBasicTest(MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true, fullyAutomatedLoadPorts: true, fullyAutomatedMaterialMovement: true);
        }

        /// <summary> 
        /// Scenario: Recipe Exists on Equipment for LP3
        /// </summary>
        [TestMethod]
        public void PicosunMorpher_FullProcessRecipeExistsLoadPort3()
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

            LoadPort = LoadPort3;
            base.LoadPortNumber = 3;

            base.RunBasicTest(MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true, fullyAutomatedLoadPorts: true, fullyAutomatedMaterialMovement: true);
        }

        /// <summary> 
        /// Scenario: Recipe Exists on Equipment
        /// </summary>
        [TestMethod]
        public void PicosunMorpher_SameRecipeOnlineLocal()
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

            LoadPort = LoadPort1;
            base.LoadPortNumber = 1;

            base.RunBasicTest(MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true, fullyAutomatedLoadPorts: true, fullyAutomatedMaterialMovement: true);
        }


        /// <summary> 
        /// Scenario: Recipe Exists on Equipment
        /// </summary>
        [TestMethod]
        public void PicosunMorpher_RecipeDoesNotExist()
        {
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot, false);
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot, false);

            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName);
            RecipeManagement.SetRecipe("AnotherRecipe", new Byte[] { 0x32 });

            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();

            LoadPort = LoadPort1;
            base.LoadPortNumber = 1;

            TrackInMustFail = true;
            base.RunBasicTest(base.MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true, fullyAutomatedLoadPorts: true, fullyAutomatedMaterialMovement: true);
        }
        #endregion Tests FullProcessScenario 

        #region Test State and Data Collection
        /// <summary> 
        /// Scenario: Control State to Host Offline
        /// </summary>
        [TestMethod]
        public void PicosunMorpher_ControlStateUpdateTest()
        {
            base.Equipment.Variables["CONTROL_STATE"] = 1;
            // Trigger event
            base.Equipment.SendMessage("EquipmentOFFLINE", null);

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

            base.Equipment.Variables["CONTROL_STATE"] = 4;
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
        //[TestMethod]
        public void PicosunMorpher_EPTStateChangeTest()
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
        public void PicosunMorpher_AlarmDataCollection()
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
            base.Equipment.Variables["JobID"] = jobId;
            base.Equipment.Variables["StationID"] = LoadPort;
            base.Equipment.Variables["CarrierRole"] = 0;
            base.Equipment.Variables["CLOCK"] = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
            // Trigger event
            base.Equipment.SendMessage("CarrierReceived", null);

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

            /*
            var SlotDataMap = new int[13];

            // scenario.ContainerScenario.Entity
            if (MESScenario.ContainerScenario.Entity.ContainerMaterials != null)
            {
                for (int i = 0; i < 13; i++)
                {
                    SlotDataMap[i] = MESScenario.ContainerScenario.Entity.ContainerMaterials.Exists(p => p.Position != null && p.Position == i + 1) ? 1 : 0;
                }
            }

            SlotMapVariable slotMapDV = new SlotMapVariable(base.Equipment) { Presence = SlotDataMap };
            */

            SecsItem slotMap = new SecsItem();
            slotMap.SetTypeToList();
            // scenario.ContainerScenario.Entity
            if (MESScenario.ContainerScenario.Entity.ContainerMaterials != null)
            {
                for (int i = 0; i < 13; i++)
                {
                    var seconfList = new SecsItem();
                    seconfList.SetTypeToList();

                    var temp = new SecsItem();

                    if (MESScenario.ContainerScenario.Entity.ContainerMaterials.Exists(p => p.Position != null && p.Position == i + 1))
                    {
                        temp.U1 = new byte[] { 0x01 };
                    }
                    else
                    {
                        temp.U1 = new byte[] { 0x00 };
                    }

                    seconfList.Add(temp);
                    slotMap.Add(seconfList);
                }
            }

            SecsItem outerList = new SecsItem();
            outerList.Add(slotMap);
            
            base.Equipment.Variables["JobID"] = new SecsItem() { ASCII = jobId };
            base.Equipment.Variables["LotID"] = new SecsItem() { ASCII = MESScenario.Entity.Name.ToString() };
            base.Equipment.Variables["StationID"] = new SecsItem() { U1 = new byte[] { (byte)(LoadPort - 13)} }; // for WaferMappingDone envent the number of load port is direct
            base.Equipment.Variables["SlotDataMap"] = slotMap;
            base.Equipment.Variables["CLOCK"] = new SecsItem() { ASCII = DateTime.UtcNow.ToString("yyyyMMddhhmmss") };

            // Trigger event
            base.Equipment.SendMessage(sendCustomMessage("WaferMappingDone"), null);

            ValidatePersistenceContainerExists(loadPortToSet);
        }


        private SecsTransaction sendCustomMessage(string messageName) {
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

            base.Equipment.Variables["JobID"] = jobId;
            base.Equipment.Variables["LotID"] = MESScenario.Entity.Name;
            base.Equipment.Variables["DestinationStationID"] = LoadPort;
            base.Equipment.Variables["CLOCK"] = DateTime.UtcNow.ToString("yyyyMMddhhmmss");

            // Trigger event
            base.Equipment.SendMessage("CarrierPlacedToPod", null);

            ValidateLoadPortState(scenario, LoadPortStateModelStateEnum.ReadyToUnload.ToString());
            // MaterialRemoved

            Thread.Sleep(2000);

            base.Equipment.Variables["JobID"] = jobId;
            base.Equipment.Variables["LotID"] = MESScenario.Entity.Name;
            base.Equipment.Variables["PodID"] = "";
            base.Equipment.Variables["StationID"] = LoadPort;
            base.Equipment.Variables["CarrierRole"] = 0;
            base.Equipment.Variables["CLOCK"] = DateTime.UtcNow.ToString("hh:mm:ss.fff");

            // Trigger event
            base.Equipment.SendMessage("CarrierRemoved", null);

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

            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Failed to recieve CREATEJOBWAFERLIST"), () =>
            {
                return receivedCreateJobWaferListCommand;
            });
            receivedCreateJobWaferListCommand = false;

            Thread.Sleep(500);           

            base.Equipment.Variables["JobID"] = jobId;
            base.Equipment.Variables["WaferLayoutPlan"] = "0";
            base.Equipment.Variables["CLOCK"] = DateTime.UtcNow.ToString("yyyyMMddhhmmss");

            // Trigger event
            base.Equipment.SendMessage("CjNoStateToCreated", null);            

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
            TestUtilities.WaitFor(ValidationTimeout, "Failed to recieve STARTJOB command", () =>
            {
                return recievedStartJobCommand;
            });

            recievedStartJobCommand = false;

            base.Equipment.Variables["JobID"] = jobId;
            base.Equipment.Variables["CLOCK"] = DateTime.UtcNow.ToString("yyyyMMddhhmmss");

            // Trigger event
            base.Equipment.SendMessage("CjQueuedToJobExecuting", null);

            return true;
        }

        public override bool ProcessCompleteEvent(CustomMaterialScenario scenario)
        {

            base.Equipment.Variables["JobID"] = jobId;
            base.Equipment.Variables["CLOCK"] = DateTime.UtcNow.ToString("yyyyMMddhhmmss");

            // Trigger event
            base.Equipment.SendMessage("UnloadingCompleted", null);

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

            base.Equipment.Variables["JobID"] = jobId;
            base.Equipment.Variables["LotID"] = MESScenario.Entity.Name;
            base.Equipment.Variables["MaterialID"] = "";
            base.Equipment.Variables["SourceStationID"] = LoadPort - 13;    // for WaferPickedFromCarrier envent the number of load port is direct
            base.Equipment.Variables["SourceSlot"] = 0;
            base.Equipment.Variables["WaferRole"] = 0;
            base.Equipment.Variables["CLOCK"] = DateTime.UtcNow.ToString("yyyyMMddhhmmss");

            // Trigger event
            base.Equipment.SendMessage("WaferPickedFromCarrier", null);

            return true;
        }

        public override bool WaferComplete(Material wafer)
        {
            wafer.Load();
            wafer.LoadRelations();
            wafer.ParentMaterial.Load();
            wafer.MaterialContainer.First().TargetEntity.Load();

            base.Equipment.Variables["JobID"] = jobId;
            base.Equipment.Variables["LotID"] = MESScenario.Entity.Name;
            base.Equipment.Variables["MaterialID"] = "";
            base.Equipment.Variables["DestinationStationID"] = LoadPort - 13;   // for WaferPlacedInCarrier envent the number of load port is direct
            base.Equipment.Variables["DestinationSlot"] = 0;
            base.Equipment.Variables["WaferRole"] = 0;
            base.Equipment.Variables["CLOCK"] = DateTime.UtcNow.ToString("yyyyMMddhhmmss");

            // Trigger event
            base.Equipment.SendMessage("WaferPlacedInCarrier", null);

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
                if (base.Equipment.EquipmentVariables["CONTROL_STATE"].DataItemId == ecid.ToString())
                {
                    reply.Item.Add(new SecsItem { U1 = new byte[] { (byte)(isOnlineRemote ? 5 : 4) } });
                }
            }

            return true;
        }

        protected virtual bool OnS2F49(SecsMessage request, SecsMessage reply)
        {
            string command = request.Item.GetChildList()[0].GetValue().ToString();
            var CommandSuccess = false;
            if (command == "CREATEJOBWAFERLIST")
            {
                receivedCreateJobWaferListCommand = true;
                CommandSuccess = true;
            }
            if (command == "STARTJOB")
            {
                recievedStartJobCommand = true;
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