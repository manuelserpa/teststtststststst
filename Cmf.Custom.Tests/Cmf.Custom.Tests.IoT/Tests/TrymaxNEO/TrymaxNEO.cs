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
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;

namespace AMSOsramEIAutomaticTests.TrymaxNEO
{
    [TestClass]
    public class TrymaxNEO : CommonTests
    {
        private const string resourceName = "5FASH1";

        public const int numberOfWafersPerLot = 3;

        public const string stepName = "M2-LT-MueTec-CD-Measurement-00820F053_E";
        public const string flowName = "FOL-UX3_EPA";

        public const bool subMaterialTrackin = true;

        public string recipeName = "TestRecipeForTrymaxNEO";
        public const string serviceName = "CD-Measurement";

        private int loadPortNumber = 1;
        private string samplingPattern = "";

        private bool setCarrierIDReceived = false;
        private bool setCarrierIDDenied = false;

        private bool startMappingReceived = false;
        private bool startMappingDenied = false;

        private bool ppSelectReceived = false;
        private bool ppSelectDenied = false;

        private bool startReceived = false;
        private bool startDenied = false;

        private bool isOnlineRemote = true;

        public bool createControlJobReceived = false;
        public bool createControlJobDenied = false;

        public bool failAtProcessJob = false;
        public bool createProcessJobReceived = false;
        public bool createProcessJobDenied = false;

        public bool isValidProceedWithCarrier = true;
        public int proceedWithCarriersReceived = 0;


        private int chamberToProcess = 1;

        ContainerScenario containerScenarioForLoadPort2;

        #region Test Basics
        [TestInitialize]
        public void TestInit()
        {

            base.Equipment = m_Scenario.GetEquipment(m_Scenario.EquipmentToTest) as SecsGemEquipment;

            base.Initialize(recipeName);
            base.SubMaterialTrackin = subMaterialTrackin;

            base.Equipment.RegisterOnMessage("S1F3", OnS1F3);
            base.Equipment.RegisterOnMessage("S2F41", OnS2F41);


            base.LoadPortNumber = loadPortNumber;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            setCarrierIDReceived = false;
            setCarrierIDDenied = false;

            startMappingReceived = false;
            startMappingDenied = false;

            ppSelectReceived = false;
            ppSelectDenied = false;

            startReceived = false;
            startDenied = false;
            isOnlineRemote = true;


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
            var step = new Step { Name = stepName };
            step.Load();
            step.UseInStepSampling = true;
            step.Save();

            ConfigureConnection(resourceName, 5012);

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
        public void TrymaxNEO_FullProcessRecipeExists()
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
        public void TrymaxNEO_SameRecipeOnlineLocal()
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


            base.RunBasicTest(MESScenario, LoadPortNumber, subMaterialTrackin, automatedMaterialOut: true);
        }


        /// <summary> 
        /// Scenario: Recipe Exists on Equipment
        /// </summary>
        [TestMethod]
        public void TrymaxNEO_RecipeDoesNotExist()
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
        #endregion Tests FullProcessScenario 

        #region Test State and Data Collection
        /// <summary> 
        /// Scenario: Control State to Host Offline
        /// </summary>
        [TestMethod]
        public void TrymaxNEO_ControlStateUpdateTest()
        {

            //base.Equipment.Variables["ControlState"] = 3;
            // Trigger event
            base.Equipment.SendMessage("CONTROL_STATE_OFFLINE", null);

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
            base.Equipment.SendMessage("CONTROL_STATE_REMOTE", null);


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
            //base.Equipment.Variables["ControlState"] = 4;
            // Trigger event
            base.Equipment.SendMessage("CONTROL_STATE_LOCAL", null);

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
        public void TrymaxNEO_EPTStateChangeTest()
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
        public void TrymaxNEO_AlarmDataCollection()
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
            base.Equipment.SendMessage(String.Format($"POD{loadPortToSet}_ARRIVED"), null);


            return true;

        }

        public override void CarrierInValidation(CustomMaterialScenario MESScenario, int loadPortToSet)
        {
            base.CarrierInValidation(MESScenario, loadPortToSet);

            //Send the event for UnknownCarrierID
            base.Equipment.Variables["REMOTE_COMMAND_NAME"] = "SETCARRIERID";

            // Trigger event
            base.Equipment.SendMessage(String.Format($"REMOTE_COMMAND_SUCCESS"), null);

            Thread.Sleep(300);

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
            
            base.Equipment.Variables["POD1_MATERIAL_MAP_LIST"] = slotMapDV;
            base.Equipment.Variables["POD2_MATERIAL_MAP_LIST"] = slotMapDV;
            base.Equipment.Variables["POD3_MATERIAL_MAP_LIST"] = slotMapDV;
            base.Equipment.Variables["POD4_MATERIAL_MAP_LIST"] = slotMapDV;
            base.Equipment.Variables["SLOT_MAP"] = slotMapDV;

            // Trigger event
            base.Equipment.SendMessage(String.Format($"POD{loadPortToSet}_MAPPED"), null);

            ValidatePersistenceContainerExists(loadPortToSet);
        }



        public override bool CarrierOut(CustomMaterialScenario scenario)
        {

            base.Equipment.SendMessage(String.Format($"POD{loadPortNumber}_UNLOAD_COMPLETE"), null);

            Thread.Sleep(200);


            return true;
        }


        public override bool ProcessStartEvent(CustomMaterialScenario scenario)
        {
            base.Equipment.Variables["LOT_ID"] = scenario.Entity.Name;
            base.Equipment.Variables["PORT_ID"] = loadPortNumber;

            //// Trigger event
            base.Equipment.SendMessage(String.Format($"PROCESSING_STARTED"), null);

            Thread.Sleep(2000);

            return true;
        }

        public override bool ProcessCompleteEvent(CustomMaterialScenario scenario)
        {
            base.Equipment.Variables["LOT_ID"] = scenario.Entity.Name;
            base.Equipment.Variables["PORT_ID"] = loadPortNumber;

            //// Trigger event
            base.Equipment.SendMessage(String.Format($"PROCESSING_COMPLETED"), null);

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

            ValidateProceedWithCarrierReceived(2);

            if (!isOnlineRemote)
            {
                Assert.Fail("Track In must fail on Online Local");
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


            TestUtilities.WaitFor(60, String.Format($"PP_SELECT or START Command never received."), () =>
            {
                return (ppSelectReceived && startReceived);
            });


            return true;
        }

        public override bool WaferStart(Material wafer)
        {
            wafer.Load();
            wafer.LoadRelations();
            wafer.ParentMaterial.Load();
            wafer.MaterialContainer.First().TargetEntity.Load();

            base.Equipment.Variables["SUBSTRATE_ID"] = String.Format("{0}{1}.{2:D2}", "CarrierAtLoadPort", loadPortNumber, wafer.MaterialContainer.First().Position);
            base.Equipment.Variables["SUBST_LOC_ID"] = String.Format(".{0:D2}", wafer.MaterialContainer.First().Position);

            ////// Trigger event
            base.Equipment.SendMessage($"POD{loadPortNumber}_SENT_MATERIAL", null);

            Thread.Sleep(2000);

            return true;
        }

        public override bool WaferComplete(Material wafer)
        {
            wafer.Load();
            wafer.LoadRelations();
            wafer.ParentMaterial.Load();
            wafer.MaterialContainer.First().TargetEntity.Load();

            base.Equipment.Variables["SUBSTRATE_ID"] = String.Format("{0}{1}.{2:D2}", "CarrierAtLoadPort", loadPortNumber, wafer.MaterialContainer.First().Position);
            base.Equipment.Variables["SUBST_LOC_ID"] = String.Format(".{0:D2}", wafer.MaterialContainer.First().Position);

            ////// Trigger event
            base.Equipment.SendMessage($"POD{loadPortNumber}_RECEIVED_MATERIAL", null);

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
                if (base.Equipment.EquipmentVariables["POD1_CARRIER_PRESENT"].DataItemId == ecid.ToString())
                {
                    reply.Item.Add(new SecsItem { ASCII = ((loadPortNumber == 1) ? "present": "absent") } );
                }
                if (base.Equipment.EquipmentVariables["POD2_CARRIER_PRESENT"].DataItemId == ecid.ToString())
                {
                    reply.Item.Add(new SecsItem { ASCII = ((loadPortNumber == 2) ? "present" : "absent") });
                }
                if (base.Equipment.EquipmentVariables["POD3_CARRIER_PRESENT"].DataItemId == ecid.ToString())
                {
                    reply.Item.Add(new SecsItem { ASCII = ((loadPortNumber == 3) ? "present" : "absent") });
                }
                if (base.Equipment.EquipmentVariables["POD4_CARRIER_PRESENT"].DataItemId == ecid.ToString())
                {
                    reply.Item.Add(new SecsItem { ASCII = ((loadPortNumber == 4) ? "present" : "absent") });
                }

            }

            return true;
        }

        protected virtual bool OnS2F41(SecsMessage request, SecsMessage reply)
        {
            string command = request.Item.GetChildList()[0].GetValue().ToString();


            if (command == "SETCARRIERID")
            {

                setCarrierIDReceived = true;

                if (!setCarrierIDDenied)
                {
                    reply.Item.GetChildList()[0].Binary = new byte[] { 0x00 };
                }
                else
                {
                    reply.Item.GetChildList()[0].Binary = new byte[] { 0x02 };
                }
            }
            else if (command == "STARTMAPPING")
            {

                startMappingReceived = true;

                if (!startMappingDenied)
                {
                    reply.Item.GetChildList()[0].Binary = new byte[] { 0x00 };
                }
                else
                {
                    reply.Item.GetChildList()[0].Binary = new byte[] { 0x02 };
                }
            }
            else if (command == "PP_SELECT")
            {
                ppSelectReceived = true;

                if (!ppSelectDenied)
                {
                    reply.Item.GetChildList()[0].Binary = new byte[] { 0x00 };
                }
                else
                {
                    reply.Item.GetChildList()[0].Binary = new byte[] { 0x02 };
                }
            }
            else if (command == "START")
            {
                startReceived = true;

                if (!startDenied)
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

        private void ValidateProceedWithCarrierReceived(int numberExpected = -1)
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
        }
    }
}