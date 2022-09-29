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
using Cmf.Custom.Tests.IoT.Tests.HermosLFM4xReader;

namespace amsOSRAMEIAutomaticTests.BrukerInsightCAP
{
    [TestClass]
    public class BrukerInsightCAP : CommonTests
    {
        private const string resourceName = "5FAFM1";

        public const int numberOfWafersPerLot = 3;

        //TODO:Check this & Service
        public const string stepName = "M3-MT-ZnO-SputterCluster-6in-00126F008_E";
        public const string flowName = "FOL-UX3_EPA";

        public const bool subMaterialTrackin = true;

        public string recipeName = "TestRecipeForBrukerInsightCAP";
        public const string serviceName = "Sputtering ZnO with Etching";

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
        
        public bool isValidProceedWithCarrier = false;

        public bool proceedWithCarriersReceived = false;


        public HermosLFM4xReader RFIDReader = new HermosLFM4xReader();
        public const string readerResourceName = "5FAFM1.RFID";



        ContainerScenario containerScenarioForLoadPort2;

        #region Test Basics
        [TestInitialize]
        public void TestInit()
        {

            base.Equipment = m_Scenario.GetEquipment(m_Scenario.EquipmentToTest) as SecsGemEquipment;

            base.Initialize(recipeName);
            base.SubMaterialTrackin = subMaterialTrackin;

            base.Equipment.RegisterOnMessage("S3F17", OnS3F17);
            base.Equipment.RegisterOnMessage("S1F3", OnS1F3);
            base.Equipment.RegisterOnMessage("S14F9", OnS14F9);
            base.Equipment.RegisterOnMessage("S16F11", OnS16F11);

            base.LoadPortNumber = 1; //Default LP

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

            if (containerScenarioForLoadPort2 != null)
            {
                containerScenarioForLoadPort2.TearDown();
            }

            //regular teardown
            AfterTest();
            RFIDReader.CleanUp(MESScenario);
            base.CleanUp(MESScenario);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
			ConfigureConnection(readerResourceName, 5015, prepareTestScenario: false);
			ConfigureConnection(resourceName, 5014, killProcess: false);            

            Resource lp1 = new Resource() { Name = "5FAFM1-LP1" };
            lp1.Load();
            lp1.AutomationMode = ResourceAutomationMode.Online;
            lp1.AutomationAddress = ".";
            lp1.Save();

            Resource lp2 = new Resource() { Name = "5FAFM1-LP2" };
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
        /// Scenario: Recipe Exists on Equipment LP1
        /// </summary>
        [TestMethod]
        public void BrukerInsightCAP_FullProcessRecipeExists_LP1()
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


            base.RunBasicTest(MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true, fullyAutomatedLoadPorts: true, fullyAutomatedMaterialMovement: true);
        }

        /// <summary> 
        /// Scenario: Recipe Exists on Equipment LP2
        /// </summary>
        [TestMethod]
        public void BrukerInsightCAP_FullProcessRecipeExists_LP2()
        {
            LoadPortNumber = 2;

            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot, false);

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
        public void BrukerInsightCAP_SameRecipeOnlineLocal()
        {
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot, false);

            isOnlineRemote = false;
            TrackInMustFail = true;

            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName);

            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();
            RecipeManagement.SetRecipe(recipe.ResourceRecipeName, RecipeName);
            RecipeManagement.FailOnNewBody = true;
            RecipeManagement.RecipeExistsOnList = true;


            base.RunBasicTest(MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true, fullyAutomatedLoadPorts: true, fullyAutomatedMaterialMovement: true);
        }


        /// <summary> 
        /// Scenario: Recipe Exists on Equipment
        /// </summary>
        [TestMethod]
        public void BrukerInsightCAP_RecipeDoesNotExist()
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
        public void BrukerInsightCAP_ControlStateUpdateTest()
        {

            // Trigger event
            base.Equipment.SendMessage("TosEquipmentOffline", null);

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

            // Trigger event
            base.Equipment.SendMessage("TosControlStateRemote", null);


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
            // Trigger event
            base.Equipment.SendMessage("TosControlStateLocal", null);

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
        public void BrukerInsightCAP_EPTStateChangeTest()
        {
            base.Equipment.Variables["EPT_STATE"] = 0;

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

            base.Equipment.Variables["EPT_STATE"] = 1;

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

            base.Equipment.Variables["EPT_STATE"] = 2;

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
        public void BrukerInsightCAP_AlarmDataCollection()
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
            base.Equipment.Variables["TosPortID"] = loadPortToSet;
            // Trigger event
            base.Equipment.SendMessage(String.Format($"TosMaterialReceived"), null);            

            return true;

        }

        public override void CarrierInValidation(CustomMaterialScenario MESScenario, int loadPortToSet)
        {
            //add carrier id to load port on rfid reader
            RFIDReader.targetIdRFID.Add(loadPortToSet.ToString(), MESScenario.ContainerScenario.Entity.Name);

            //material received MaterialReceived
            base.CarrierInValidation(MESScenario, loadPortToSet);


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

            //SecsItem carrierContentMap = new SecsItem();
            //carrierContentMap.SetTypeToList();
            //carrierContentMap.Add(new SecsItem() { U1 = new byte[] { (byte)0x01 } });
            //carrierContentMap.Add(new SecsItem() { U1 = new byte[] { (byte)0x01 } });

            base.Equipment.Variables.Clear();

            base.Equipment.Variables["TosPortID"] = loadPortToSet;
            base.Equipment.Variables["TosCarrierID"] = MESScenario.ContainerScenario.Entity.Name;
            base.Equipment.Variables["TosSlotMap"] = slotMapDV;
            base.Equipment.Variables["CarrierContentMap"] = new SlotMapVariable(base.Equipment) { Presence = new int[] { 0,0 } };

            // Trigger event

            TestUtilities.WaitFor(ValidationTimeout, "Failed to recieve S18F9", () =>
            {
                return RFIDReader.RecievedS18F9();
            });
            RFIDReader.ClearFlags();


            TestUtilities.WaitFor(ValidationTimeout, "Failed to recieve ProceedWithCarrier", () =>
            {
                return proceedWithCarriersReceived;
            });

            proceedWithCarriersReceived = false;


            base.Equipment.SendMessage(String.Format($"TosSlotMapReadSuccessful"), null);

            
        }

        public override bool CarrierOut(CustomMaterialScenario scenario)
        {
            base.Equipment.Variables["TosPortID"] = LoadPortNumber;
            base.Equipment.Variables["PortTransferState"] = 0;
            // Trigger event
            base.Equipment.SendMessage(String.Format($"LPTSM9_TRANSFERBLOCKED_READYTOUNLOAD"), null);
            Thread.Sleep(300);

            // MaterialRemoved
            base.Equipment.Variables["TosPortID"] = LoadPortNumber;

            // Trigger event
            base.Equipment.SendMessage(String.Format($"TosMaterialRemoved"), null);

            Thread.Sleep(200);

            return true;
        }


        public override bool ProcessStartEvent(CustomMaterialScenario scenario)
        {
            base.Equipment.Variables["PRJobID"] = $"PrJob_{scenario.Entity.Name}";
            base.Equipment.Variables["PRJobState"] = 0;

            //// Trigger event
            base.Equipment.SendMessage(String.Format($"PROCESSING_STARTED"), null);


            return true;
        }

        public override bool ProcessCompleteEvent(CustomMaterialScenario scenario)
        {
            base.Equipment.Variables["PRJobID"] = $"PrJob_{scenario.Entity.Name}";
            base.Equipment.Variables["PRJobState"] = 0;

            //// Trigger event
            base.Equipment.SendMessage(String.Format($"PROCESSING_COMPLETED"), null);

            return true;
        }

        public override bool WaferStart(Material wafer)
        {
            wafer.Load();

            base.Equipment.Variables["SubstId"] = wafer.Name;
            base.Equipment.Variables["SubstLocId"] = "";
            base.Equipment.Variables["SubstState"] = 0;
            base.Equipment.Variables["SubstProcState"] = 0;
            base.Equipment.Variables["TosClock"] = DateTime.UtcNow.ToString("hh:mm:ss.fff");


            //// Trigger event
            base.Equipment.SendMessage($"WaferStart", null);

            Thread.Sleep(2000);

            return true;
        }

        public override bool WaferComplete(Material wafer)
        {
            wafer.Load();
            wafer.ParentMaterial.Load();

            SendMetrics(wafer);

            base.Equipment.Variables["SubstId"] = wafer.Name;
            base.Equipment.Variables["SubstLocId"] = "";
            base.Equipment.Variables["SubstState"] = 0;
            base.Equipment.Variables["SubstProcState"] = 0;
            base.Equipment.Variables["TosClock"] = DateTime.UtcNow.ToString("hh:mm:ss.fff");

            //// Trigger event
            base.Equipment.SendMessage($"WaferEnd", null);

            Thread.Sleep(2000);

            return true;
        }

        #endregion Events

        #region Test actions
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

            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Failed to recieve Proceed with Carrier for Material {scenario.Entity.Name}"), () =>
            {
                return proceedWithCarriersReceived;
            });

            proceedWithCarriersReceived = false;

            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Process Job Creation failed for Material {scenario.Entity.Name}"), () =>
            {                
                return createProcessJobReceived;
            });


            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Control Job Creation failed for Material {scenario.Entity.Name}"), () =>
            {
                return createControlJobReceived;
            });

            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material {scenario.Entity.Name} State is not {MaterialStateModelStateEnum.Setup.ToString()}"), () =>
            {
                scenario.Entity.Load();
                if (scenario.Entity.CurrentMainState == null || scenario.Entity.CurrentMainState.CurrentState == null)
                {
                    return false;
                }

                return scenario.Entity.CurrentMainState.CurrentState.Name.Equals(MaterialStateModelStateEnum.Setup.ToString());
            });

            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material {scenario.Entity.Name} System State is not {MaterialSystemState.InProcess.ToString()}"), () =>
            {
                scenario.Entity.Load();
                return scenario.Entity.SystemState.ToString().Equals(MaterialSystemState.InProcess.ToString());
            });

            return true;
        }
        #endregion

        public bool SendMetrics(Material wafer) {

            base.Equipment.Variables["TosCarrierID"] = wafer.ParentMaterial.Name;
            base.Equipment.Variables["PORT_ID"] = 2;
            base.Equipment.Variables["SubstLocID"] = "";//Stage;
            base.Equipment.Variables["SubstID"] = wafer.Name;
            base.Equipment.Variables["CtrlJobID"] = $"CtrlJob_{base.MESScenario.Entity.Name}";
            base.Equipment.Variables["PRJobID"] = $"PrJob_{base.MESScenario.Entity.Name}";
            base.Equipment.Variables["DV_DEPTH_f4DEPTH_AT_MAX"] = 31.676783;
            base.Equipment.Variables["DV_DEPTH_f4FILTER_CUTOFF"] = "";
            base.Equipment.Variables["DV_DEPTH_f4MAX_PEAK"] = 30.917986;
            base.Equipment.Variables["DV_DEPTH_f4MIN_PEAK"] = 81.84327;
            base.Equipment.Variables["DV_DEPTH_f4PEAK_TO_PEAK"] = 50.925285;
            base.Equipment.Variables["DV_DEPTH_f4TOTAL_PEAKS"] = 2.0;
            base.Equipment.Variables["DV_DEPTH_u2AUTOPROGRAM_COUNT"] = 1;
            base.Equipment.Variables["DV_RECIPE_u1TYPE"] = 5;
            base.Equipment.Variables["DV_SITE_f4COLUMN"] = 0.0;
            base.Equipment.Variables["DV_SITE_f4ROW"] = 0.0;
            base.Equipment.Variables["DV_SITE_f8X_COORD"] = 52.767436098339644;
            base.Equipment.Variables["DV_SITE_f8X_M20_COORDINATE"] = 52.767436098339644;
            base.Equipment.Variables["DV_SITE_f8Y_COORD"] = 58.52608286868775;
            base.Equipment.Variables["DV_SITE_f8Y_M20_COORDINATE"] = 58.52608286868775;
            base.Equipment.Variables["DV_SITE_szAUTOPROGRAM_NAME"] = "Measurement1_normal";
            base.Equipment.Variables["DV_SITE_szMASTERSITENAME"] = base.MESScenario.Facility.Name;
            base.Equipment.Variables["DV_SITE_szSITENAME"] = ""; 
            base.Equipment.Variables["DV_SITE_szSUBSTRATE_LOTID"] = wafer.ParentMaterial.Name;
            base.Equipment.Variables["DV_SITE_szSUBSTRATE_WAFERID"] = wafer.Name;
            base.Equipment.Variables["DV_SITE_u1SITE"] = 1;
            base.Equipment.Variables["DV_SITE_u1SLOT"] = 2;
            base.Equipment.Variables["DV_SITE_u4TOTAL_SITES_PER_WAFER"] = 1;
            base.Equipment.Variables["DV_DEPTH_f4MIN_PEAK_FWHM"] = 0.492075;
            base.Equipment.Variables["DV_DEPTH_f4MAX_PEAK_FWHM"] = 13.039988;

            base.Equipment.SendMessage("CE_DEPTH_ANALYSIS_DATA_AVAIL", null);

            return true;
        }

        #region Stream and Function actions
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

        public virtual bool OnS2F41(SecsMessage request, SecsMessage reply)
        {
            //TODO: Validate MSG

            string command = request.Item.GetChildList()[0].GetValue().ToString();


            if (command == "LOADCARRIER")
            {

                loadCommandReceived = true;

                if (!loadCommandDenied)
                {
                    reply.Item.GetChildList()[0].Binary = new byte[] { 0x00 };
                }
                else
                {
                    reply.Item.GetChildList()[0].Binary = new byte[] { 0x02 };
                }
            }
            else if (command == "UNLOADCARRIER")
            {

                unloadCommandReceived = true;

                if (!unloadCommandDenied)
                {
                    reply.Item.GetChildList()[0].Binary = new byte[] { 0x00 };
                }
                else
                {
                    reply.Item.GetChildList()[0].Binary = new byte[] { 0x02 };
                }
            }
            else
            {
                Assert.Fail("Unrecognized Command");
            }

            return (true);
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
            var ack = new SecsItem { U1 = new byte[] { !isValidProceedWithCarrier ? (byte) 0x00 : (byte) 0x01 } };

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
                if (base.Equipment.EquipmentVariables["TosControlState"].DataItemId == ecid.ToString())
                {
                    reply.Item.Add(new SecsItem { U1 = new byte[] { (byte)(isOnlineRemote ? 5 : 4) } });
                }
            }

            return true;
        }
        #endregion

        #region Validations
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
        #endregion
    }
}