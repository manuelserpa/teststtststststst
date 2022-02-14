using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using cmConnect.TestFramework.Common.Utilities;
using cmConnect.TestFramework.EquipmentSimulator.Drivers;
using cmConnect.TestFramework.SystemRest.Entities;
using Cmf.Custom.AMSOsram.BusinessObjects;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects;
using Cmf.SECS.Driver;
using Cmf.TestScenarios.ContainerManagement.ContainerScenarios;
using AMSOsramEIAutomaticTests.Objects.Extensions;
using AMSOsramEIAutomaticTests.Objects.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using AMSOsramEIAutomaticTests;
using Cmf.Custom.TestUtilities;

namespace AMSOsramEIAutomaticTests.EvatecClusterline200II
{
    [TestClass]
    public class EvatecClusterline200II : CommonTests
    {
        private const string resourceName = "PDSP0101";

        public const int numberOfWafersPerLot = 3;

        public const string stepName = "M3-MT-ZnO-SputterCluster-6in-00126F008_E";
        public const string flowName = "FOL-UX3_EPA";

        public const bool subMaterialTrackin = true;

        public string recipeName = "TestRecipeForEvatecClusterline200II";
        public const string serviceName = "Sputtering ZnO with Etching";

        private int loadPortNumber = 1;

        private bool loadCommandReceived = false;
        private bool loadCommandDenied = false;

        private bool unloadCommandReceived = false;
        private bool unloadCommandDenied = false;

        private bool isOnlineRemote = true;

        public bool createControlJobReceived = false;
        public bool createControlJobDenied = false;

        public bool createProcessJobReceived = false;
        public bool createProcessJobDenied = false;

        private int chamberToProcess = 1;

        ContainerScenario containerScenarioForLoadPort2;

        #region Test Basics
        [TestInitialize]
        public void TestInit()
        {

            base.Equipment = m_Scenario.GetEquipment(m_Scenario.EquipmentToTest) as SecsGemEquipment;

            base.Initialize(recipeName);
            base.SubMaterialTrackin = subMaterialTrackin;

            base.Equipment.RegisterOnMessage("S2F41", OnS2F41);
            base.Equipment.RegisterOnMessage("S1F3", OnS1F3);
            base.Equipment.RegisterOnMessage("S14F9", OnS14F9);
            base.Equipment.RegisterOnMessage("S16F11", OnS16F11);

            base.LoadPortNumber = loadPortNumber;
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

            if (containerScenarioForLoadPort2 != null)
            {
                containerScenarioForLoadPort2.TearDown();
            }

            //regular teardown
            AfterTest();
            base.CleanUp(MESScenario);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ConfigureConnection(resourceName, 5007);

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
        public void EvatecClusterline200II_FullProcessRecipeExists()
        {
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot, false);

            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName);

            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();           
            RecipeManagement.SetRecipe(recipe.ResourceRecipeName, RecipeName);
            RecipeManagement.FailOnNewBody = true;
            RecipeManagement.RecipeExistsOnList = true;

            base.RunBasicTest(MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true);
        }

        /// <summary> 
        /// Scenario: Recipe Exists on Equipment
        /// </summary>
        [TestMethod]
        public void EvatecClusterline200II_SameRecipeOnlineLocal()
        {
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot, false);

            isOnlineRemote = false;
            TrackInMustFail = true;

            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName, ".\\RecipeBinaryFiles\\FTMTL14PYCUR.bin");

            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();
            RecipeManagement.SetRecipe(recipe.ResourceRecipeName, RecipeName);
            RecipeManagement.FailOnNewBody = true;
            RecipeManagement.RecipeExistsOnList = true;


            base.RunBasicTest(MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true);
        }


        /// <summary> 
        /// Scenario: Recipe Exists on Equipment
        /// </summary>
        [TestMethod]
        public void EvatecClusterline200II_RecipeDoesNotExist()
        {
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot, false);
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot, false);

            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName);
            RecipeManagement.SetRecipe("AnotherRecipe", new Byte[] { 0x32 });

            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();

            TrackInMustFail = true;
            base.RunBasicTest(base.MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true);
        }
        
        /// <summary> 
        /// Scenario: Control State to Host Offline
        /// </summary>
        [TestMethod]
        public void EvatecClusterline200II_ControlStateUpdateTest()
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


        #endregion Tests FullProcessScenario 


        #region Events
        public override bool CarrierIn(CustomMaterialScenario scenario, int loadPortToSet)
        {

            //CarrierClamped

            base.Equipment.Variables["CarrierID_CarrierReport"] = $"CarrierAtPort{loadPortToSet}";
            base.Equipment.Variables["CarrierLocationID"] = $"LP{loadPortToSet}";
            base.Equipment.Variables["LocationID"] = $"LP{loadPortToSet}";
            base.Equipment.Variables["PortID_CarrierReport"] = loadPortToSet;

            // Trigger event
            base.Equipment.SendMessage(String.Format($"CarrierClamped"), null);

            
            return true;

        }

        public override void CarrierInValidation(CustomMaterialScenario MESScenario, int loadPortToSet)
        {
            //clamped
            CarrierIn(MESScenario, loadPortToSet);

            Log(String.Format("{0}: [S] Validating Load Port State Changed State Change to Occupied Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
            ValidateLoadPortState(MESScenario, LoadPortStateModelStateEnum.Occupied.ToString(), 2);
            Log(String.Format("{0}: [E] Validating Load Port State Changed State Change to Occupied Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));

            //material received MaterialReceived
            base.Equipment.Variables["PortTransferState"] = 1;
            base.Equipment.Variables["PortReservationState"] = 0;
            base.Equipment.Variables["CarrierID_CarrierReport"] = "";
            base.Equipment.Variables["PortAccessMode"] = 0;
            base.Equipment.Variables["PortAssociationState"] = 0;
            base.Equipment.Variables["PortID_CarrierReport"] = loadPortToSet;

            // Trigger event
            base.Equipment.SendMessage(String.Format($"MaterialReceived"), null);

            // wait for load 
            TestUtilities.WaitFor(60, String.Format($"Load Container Command never received"), () =>
            {
                return loadCommandReceived;
            });

            //prepare slot maps 
            var slotMapForMaterialContainer = new int[25];

            // scenario.ContainerScenario.Entity
            if (MESScenario.ContainerScenario.Entity.ContainerMaterials != null)
            {
                for (int i = 0; i < 25; i++)
                {
                    if (MESScenario.ContainerScenario.Entity.ContainerMaterials.Exists(p => p.Position != null && p.Position == i + 1))
                    {
                        slotMapForMaterialContainer[i] = 3;
                    }
                    else
                    {
                        slotMapForMaterialContainer[i] = 1;
                    }

                }
            }

            //SlotMapReadVerifiedWaitHostEvent
            var CarrierContentMap = new SecsItem();
            CarrierContentMap.SetTypeToList();
           
            base.Equipment.Variables["CarrierSubType"] = MESScenario.ContainerScenario.Entity.Name;
            base.Equipment.Variables["SubstrateSubType"] = "Substrate";
            base.Equipment.Variables["CarrierAccessingStatus"] = 0;
            base.Equipment.Variables["CarrierCapacity"] = 25;
            base.Equipment.Variables["CarrierContentMap"] = CarrierContentMap;
            base.Equipment.Variables["CarrierID_CarrierReport"] = $"CarrierAtPort{loadPortToSet}";
            base.Equipment.Variables["CarrierIDStatus"] = 2;
            base.Equipment.Variables["CarrierLocationID"] = $"FIMS{loadPortToSet}";
            base.Equipment.Variables["CarrierSlotMap"] = slotMapForMaterialContainer;
            base.Equipment.Variables["PortID_CarrierReport"] = loadPortToSet;

            Thread.Sleep(200);
            
        }

        public override bool CarrierOut(CustomMaterialScenario scenario)
        {

            // wait for load 
            TestUtilities.WaitFor(60, String.Format($"Load Container Command never received"), () =>
            {
                return unloadCommandReceived;
            });

            //CarrierSMTrans21 ready to unload
            var CarrierContentMap = new SecsItem();
            CarrierContentMap.SetTypeToList();
            base.Equipment.Variables["CarrierSubType"] = MESScenario.ContainerScenario.Entity.Name;
            base.Equipment.Variables["SubstrateSubType"] = "Substrate";
            base.Equipment.Variables["CarrierAccessingStatus"] = 0;
            base.Equipment.Variables["CarrierCapacity"] = 25;
            base.Equipment.Variables["CarrierContentMap"] = CarrierContentMap;
            base.Equipment.Variables["CarrierID_CarrierReport"] = $"CarrierAtPort{loadPortNumber}";
            base.Equipment.Variables["CarrierIDStatus"] = 2;
            base.Equipment.Variables["CarrierLocationID"] = $"FIMS{loadPortNumber}";
            base.Equipment.Variables["CarrierSlotMap"] = CarrierContentMap; 
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

        public override void CarrierOutValidation(CustomMaterialScenario MESScenario, int loadPortToSet)
        {
            Log(String.Format("{0}: [S] Carrier Out Resource {1} Load Port {2}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, loadPortToSet));
            CarrierOut(MESScenario);
            Log(String.Format("{0}: [E] Carrier Out Resource {1} Load Port {2}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, loadPortToSet));

            Log(String.Format("{0}: [S] Validating Load Port State Changed State Change to Available Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
            ValidateLoadPortState(MESScenario, LoadPortStateModelStateEnum.Available.ToString());
            Log(String.Format("{0}: [E] Validating Load Port State Changed State Change to Available Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
                      
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
                if(!createControlJobReceived || !createProcessJobReceived)
                {
                    Assert.Fail("Control or Process Job creation requests were never received");                }
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

            base.Equipment.Variables["AcquiredId"] = "";
            base.Equipment.Variables["LotId"] = "";
            base.Equipment.Variables["SubId"] = String.Format("CarrierAtPort{0}.{1:D2}", loadPortNumber, wafer.MaterialContainer.First().Position); ;

            //// Trigger event
            base.Equipment.SendMessage($"ProcessChamber{chamberToProcess}_ProcessStarted", null);

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
            base.Equipment.SendMessage($"ProcessChamber{chamberToProcess}_ProcessFinished", null);

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
            var mainList = new SecsItem();
            mainList.SetTypeToList();

            var jobItem = new SecsItem { ASCII = "Job1" };

            mainList.Add(jobItem);

            var reportList = new SecsItem();
            reportList.SetTypeToList();

            mainList.Add(reportList);

            var ackList = new SecsItem();
            ackList.SetTypeToList();

            var ack = new SecsItem { U1 = new byte[] { 0x00 } };

            var errorList = new SecsItem();
            errorList.SetTypeToList();

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

            ackList.Add(ack);

            ackList.Add(errorList);

            mainList.Add(ackList);

            reply.Item.Add(mainList);
            return true;
        }

        protected bool OnS16F11(SecsMessage request, SecsMessage reply)
        {
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
    }
}