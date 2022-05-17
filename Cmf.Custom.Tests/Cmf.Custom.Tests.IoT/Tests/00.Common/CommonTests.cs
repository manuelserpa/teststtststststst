using System;
using System.Collections.Generic;
using System.Linq;
using cmConnect.TestFramework.Common.Utilities;
using cmConnect.TestFramework.EquipmentSimulator.Drivers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cmf.Navigo.BusinessObjects;
using AMSOsramEIAutomaticTests.IoT.Common;
using cmConnect.TestFramework.ConnectIoT.Entities;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Newtonsoft.Json.Linq;
using System.IO;
using cmConnect.TestFramework.SystemRest.Utilities;
using Cmf.Foundation.BusinessObjects;
using System.Net;
using Cmf.SECS.Driver;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using AutomaticTests;
using Cmf.TestScenarios.ContainerManagement.ContainerScenarios;
using System.Collections.ObjectModel;
using Cmf.Custom.TestUtilities;
using AMSOsramEIAutomaticTests.Objects.Utilities;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.AMSOsram.BusinessObjects;
using AMSOsramEIAutomaticTests.Objects.Persistence;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using AMSOsramEIAutomaticTests.Objects.Extensions;
using Newtonsoft.Json;

namespace AMSOsramEIAutomaticTests
{
    public abstract class CommonTests : TestClassFramework
    {
        public SecsGemEquipment Equipment;
        public RecipeManagement RecipeManagement;
        public CustomMaterialScenario MESScenario;
        public string RecipeName;
        public Boolean TrackInMustFail = false;
        public Boolean TrackOutMustFail = false;
        public int ValidationTimeout = 90;
        public bool SubMaterialTrackin;

        // Usefull for sorter scenarios
        public bool IgnoreMaterialScenarioSetup = false;
        public bool IgnoreCarrierIn = false;

        public int LoadPortNumber = 0;

        public static Dictionary<string, object> ConnectionAttributes = new Dictionary<string, object>();

        private static Cmf.Custom.TestUtilities.SmartTableManager smartTableManager = new Cmf.Custom.TestUtilities.SmartTableManager();

        public List<Resource> LoadPorts;

        public List<CustomMaterialScenario> MESScenarios;

        public string ContainerType = AMSOsramConstants.ContainerSMIFPod;

        public virtual bool CarrierIn(CustomMaterialScenario scenario, int loadPortToSet = 0)
        {
            return true;
        }

        public virtual bool CarrierOut(CustomMaterialScenario scenario)
        {
            return true;
        }

        public virtual bool PostTrackInActions(CustomMaterialScenario scenario)
        {

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

        public virtual bool ReadyToLoad(Material subMaterial)
        {
            return true;
        }

        public virtual bool ReadyToUnload(Material subMaterial)
        {
            return true;
        }

        public virtual bool ProcessStartEvent(CustomMaterialScenario scenario)
        {
            return true;
        }

        public virtual bool ProcessCompleteEvent(CustomMaterialScenario scenario)
        {
            return true;
        }

        public virtual bool ProcessStateChange(CustomMaterialScenario scenario)
        {
            return true;
        }

        public virtual bool ValidateLoadPortState(CustomMaterialScenario scenario, string loadPortState)
        {
            return ValidateLoadPortState(scenario, loadPortState, LoadPortNumber);
        }

        public virtual bool ValidateLoadPortState(CustomMaterialScenario scenario, string loadPortState, int loadPortNumber)
        {
            Resource loadPort = GetLoadPort(scenario, loadPortNumber);


            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Load Port Sub Resource {loadPort.Name} State is not {loadPortState}"), () =>
            {
                return loadPort.GetCurrentMainStateModelState().Name.Equals(loadPortState);
            });

            return true;
        }

        public virtual bool DockContainer(CustomMaterialScenario scenario)
        {
            Resource loadPort = GetLoadPort(scenario, LoadPortNumber);

            MESScenario.DockContainer(loadPort);

            return true;
        }

        public virtual bool DockContainer(ContainerScenario scenario, int loadPortNumber, CustomMaterialScenario MESScenario)
        {
            Resource loadPort = GetLoadPort(MESScenario, loadPortNumber);

            if (scenario != null && loadPort != null)
            {
                // Undock if docked first
                loadPort.LoadRelation("ContainerResource");
                if (loadPort.ResourceContainers != null && loadPort.ResourceContainers.Count > 0)
                {
                    var containerToUndock = loadPort.ResourceContainers[0].SourceEntity;
                    containerToUndock.Load();

                    var undock = new Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects.UndockContainerInput()
                    {
                        Container = containerToUndock,
                        NumberOfRetries = 3
                    };
                    undock.UndockContainerSync();

                }

                //
                scenario.Entity.Load();
                scenario.Entity.LoadRelation("ResourceContainer");
                loadPort.Load();
                var dock = new Cmf.Navigo.BusinessOrchestration.ContainerManagement.InputObjects.DockContainerInput()
                {
                    Container = scenario.Entity,
                    Resource = loadPort,
                    IgnoreLastServiceId = true,
                    NumberOfRetries = 10,
                }.DockContainerSync();

                scenario.Entity.LoadRelations(new Collection<String>() { "ContainerResource" });
            }

            return true;
        }

        public virtual bool ValidateMaterialStateModelState(CustomMaterialScenario scenario, string stateModelState)
        {
            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material {scenario.Entity.Name} State is not {stateModelState}"), () =>
            {
                scenario.Entity.Load();
                return scenario.Entity.CurrentMainState.CurrentState.Name.Equals(stateModelState);
            });

            return true;
        }

        public virtual bool WaferComplete(Material wafer)
        {
            return true;
        }

        public virtual bool WaferStart(Material wafer)
        {
            return true;
        }

        public virtual bool ValidateSubMaterialState(Material submaterial, string subMaterialState)
        {
            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material {submaterial.Name} State is not {subMaterialState}"), () =>
            {
                submaterial.Load();
                return submaterial.SystemState.ToString().Equals(subMaterialState);
            });

            return true;
        }

        public static void ConfigureConnection(string resourceName, int? port = null, string library = null, Dictionary<string, object> connectionAttributes = null, bool isEnableAllAlarms = false, bool isEnableAllEvents = false, bool prepareTestScenario = true)
        {

            if (!port.HasValue)
            {
                port = 5006;
            }

            Console.WriteLine("Preparing to run test, setting equipment address and re[S]");
            Resource resource = new Resource() { Name = resourceName };

            resource.Load();
            resource.LoadAttributes(new string[] { "AutomationEquipmentAddress", "AutomationEquipmentIPPort", "AutomationEquipmentEnableDisableEventsMode", "AutomationEquipmentEnableDisableAlarmsMode" });
            resource.Attributes["AutomationEquipmentAddress"] = Dns.GetHostName();
            resource.Attributes["AutomationEquipmentIPPort"] = port;
            if (library != null)
            {
                resource.LoadAttributes(new string[] { "AutomationEquipmentCustomLibrary" });

                resource.Attributes["AutomationEquipmentCustomLibrary"] = library;
            }

            if (connectionAttributes != null)
            {
                resource.LoadAttributes(connectionAttributes.Keys.ToArray());
                foreach (var connectionAttribute in connectionAttributes)
                {
                    resource.Attributes[connectionAttribute.Key] = connectionAttribute.Value;
                }
            }

            if (isEnableAllAlarms)
            {
                resource.Attributes["AutomationEquipmentEnableDisableAlarmsMode"] = "EnableAll";
            }

            if (isEnableAllEvents)
            {
                resource.Attributes["AutomationEquipmentEnableDisableEventsMode"] = "EnableAll";
            }

            resource.SaveAttributes(resource.Attributes);

            if (prepareTestScenario)
            {
                PrepareTestScenario(resourceName);
            }
        }

        public virtual CustomMaterialScenario InitializeMaterialScenario(string resourceName, string flowName, string stepName, int numberOfWafersPerLot = 25, bool? allowDownloadRecipeAtTrackIn = null)
        {
            var MaterialScenario = new CustomMaterialScenario();
            MaterialScenario.Resource = new Resource() { Name = resourceName };
            MaterialScenario.Resource.Load();

            if (allowDownloadRecipeAtTrackIn.HasValue)
            {
                AttributeCollection attrCollection = new AttributeCollection();
                attrCollection.Add("AllowDownloadRecipeAtTrackIn", allowDownloadRecipeAtTrackIn.Value);
                MaterialScenario.Resource.SaveAttributes(attrCollection);
                MaterialScenario.Resource.Load();
            }

            MaterialScenario.FlowName = flowName;
            MaterialScenario.StepName = stepName;
            MaterialScenario.NumberOfSubMaterials = numberOfWafersPerLot;

            EnsureMaterialStartConditions(MaterialScenario.Resource);
            EnsureLoadPortStartConditions(MaterialScenario.Resource);

            return MaterialScenario;
        }

        protected void EnsureMaterialStartConditions(Resource resource)
        {
            if (resource.MaterialsInProcessCount > 0)
            {
                Cmf.Custom.TestUtilities.Generalization.AbortAllMaterials(resource);
            }
        }

        protected void EnsureLoadPortStartConditions(Resource resource)
        {
            var resourceHierarchy = resource.GetDescendentResources();

            var loadPorts = resourceHierarchy.Where(s => s.ChildResource.ProcessingType == ProcessingType.LoadPort).Select(s => s.ChildResource).ToList();

            if (loadPorts.Count > 0)
            {
                foreach (var lp in loadPorts)
                {
                    lp.Load();
                    if (lp.CurrentMainState.StateModel.Name == "CustomLoadPortStateModel")
                    {
                        lp.AdjustState("ReadyToUnload");
                        lp.Load();
                        lp.LoadAttribute("IsLoadPortInUse");
                        lp.Attributes["IsLoadPortInUse"] = false;
                        lp.SaveAttributes(lp.Attributes);

                    }

                }
            }

        }

        public void Initialize(string recipeName = "")
        {

            //initialize recipe management
            RecipeManagement = new RecipeManagement(Equipment);

            //Crete recipe name for test
            RecipeName = String.Format("{0}_{1}", recipeName, Guid.NewGuid());

            Equipment.RegisterOnMessage("S1F13", OnEstablishCommunication);
            Equipment.RegisterOnMessage("S1F17", OnGoOnline);



            try
            {
                TestUtilities.WaitFor(120, $"Driver never connected", () =>
                {
                    var instance = SystemUtilities.GetObjectById<AutomationDriverInstance>(((IoTEquipment)Equipment.BaseImplementation).EntityInstance.Id);
                    return (instance.CommunicationState == AutomationCommunicationState.Communicating);
                });
            }
            catch
            {
                Assert.Inconclusive("Test could not connect to driver");
            }
        }

        public void CleanUp(CustomMaterialScenario MESScenario)
        {
            if (MESScenario != null)
            {
                if (MESScenario.Entity != null && MESScenario.Entity.LastRecipe != null
                    && MESScenario.Entity.LastRecipe.UniversalState != Cmf.Foundation.Common.Base.UniversalState.Terminated)
                {

                    if (MESScenario.Entity.SystemState == MaterialSystemState.InProcess)
                    {
                        AbortMaterialProcessInput abortMaterialProcessInput = new AbortMaterialProcessInput()
                        {
                            Material = MESScenario.Entity,
                            NumberOfRetries = 3,

                        };
                        abortMaterialProcessInput.AbortMaterialProcessSync();


                    }
                    MESScenario.Entity.LastRecipe.Load();
                    MESScenario.Entity.LastRecipe.Terminate();
                }

                MESScenario.TearDown();
            }

            RecipeUtilities.TerminateRecipeHierarchy(RecipeName);
            Equipment.UnregisterAllHandlers();
        }

        public virtual bool PostSetupActions(CustomMaterialScenario MESScenario)
        {
            return true;
        }



        public virtual void RunBasicTest(CustomMaterialScenario MESScenario, int loadPortToSet, bool enableSubMaterial = false, string recipeName = null, bool fullyAutomatedMaterialMovement = false, bool fullyAutomatedLoadPorts = false, bool automatedMaterialOut = false)
        {
            if (!IgnoreMaterialScenarioSetup)
            {
                //Create Scenario on MES
                Log(String.Format("{0}: [S] MES Scenario Setup for Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
                MESScenario.Setup(enableSubMaterial, automaticContainerPositions: false, containerType: ContainerType);
                Log(String.Format("{0}: [E] MES Scenario Setup for Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
                Log(String.Format("{0}: Material '{1}' ", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Entity.Name));

                Log(String.Format("{0}: [S] Post Setup Actions Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
                PostSetupActions(MESScenario);
                Log(String.Format("{0}: [E] Post Setup Actions Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
            }

            Log(String.Format("{0}: [S] Process State Change Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
            MESScenario.Resource.Load();
            ProcessStateChange(MESScenario);
            Log(String.Format("{0}: [E] Process State Change Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));

            if (!IgnoreCarrierIn)
            {
                CarrierInValidation(MESScenario, loadPortToSet);
            }

            if (!fullyAutomatedLoadPorts)
            {
                Log(String.Format("{0}: [S] MES Dock Container {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.ContainerScenario.Entity.Name));
                DockContainer(MESScenario);
                Log(String.Format("{0}: [E] MES Dock Container {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.ContainerScenario.Entity.Name));
            }

            if (!fullyAutomatedMaterialMovement)
            {
                Log(String.Format("{0}: [S] Track In of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
                try
                {
                    TrackInEvaluator(recipeName);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "TrackInFailed")
                        return;
                    else
                        Assert.Fail(ex.Message);
                }
                Log(String.Format("{0}: [E] Track In of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            }

            Log(String.Format("{0}: [S] Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
            bool postTrackInResult = PostTrackInActions(MESScenario);
            Log(String.Format("{0}: [E] Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));

            //intentional step out of the test by returning false on post track in result
            if (!postTrackInResult)
            {
                Log(String.Format("{0}: [S] Test Concluded by Returning false on Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
                return;
            }
            //System.Threading.Thread.Sleep(15000);

            Log(String.Format("{0}: [S] Validate Persistence Exists Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            GetMaterialPersistence(MESScenario.Entity.Name);
            Log(String.Format("{0}: [E] Validate Persistence Exists Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            Log(String.Format("{0}: [S] Validate Material Persistence Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ValidatePersistenceState(MESScenario.Entity.Name, MaterialStateEnum.Setup);
            Log(String.Format("{0}: [E] Validate Material Persistence Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            Log(String.Format("{0}: [S] Sending Process Start Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ProcessStartEvent(MESScenario);
            Log(String.Format("{0}: [E] Sending Process Start Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            System.Threading.Thread.Sleep(5000);

            Log(String.Format("{0}: [S] Validate Material is In Progress on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ValidateMaterialStateModelState(MESScenario, "In Progress");
            Log(String.Format("{0}: [E] Validate Material is In Progress on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            Log(String.Format("{0}: [S] Validate Material Persistence is In Process on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ValidatePersistenceState(MESScenario.Entity.Name, MaterialStateEnum.InProcess);
            Log(String.Format("{0}: [E] Validate Material Persistence is In Process on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            if (SubMaterialTrackin)
            {
                Log(String.Format("{0}: [S] SubMaterial Process Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
                foreach (Material material in MESScenario.SubMaterials)
                {
                    Log(String.Format("{0}: [S] SubMaterial Wafer Start Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));
                    WaferStartValidation(material);
                    Log(String.Format("{0}: [E] SubMaterial Wafer Start Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));

                    Log(String.Format("{0}: [S] SubMaterial Wafer Complete Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));
                    WaferCompleteValidation(material);
                    Log(String.Format("{0}: [E] SubMaterial Wafer Complete Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));

                }
                Log(String.Format("{0}: [E] SubMaterial Process Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            }
            else
            {
                Log(String.Format("{0}: [S][E] No SubMaterial Process for Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));

            }

            Log(String.Format("{0}: [S] Sending Process Complete Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ProcessCompleteEvent(MESScenario);
            Log(String.Format("{0}: [E] Sending Process Complete Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            Log(String.Format("{0}: [S] Validate Material is Complete on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ValidateMaterialStateModelState(MESScenario, MaterialStateModelStateEnum.Complete.ToString());
            Log(String.Format("{0}: [E] Validate Material is Complete on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            if (!fullyAutomatedMaterialMovement && !automatedMaterialOut)
            {
                Log(String.Format("{0}: [S] Validate Material Persistence is Complete on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
                ValidatePersistenceState(MESScenario.Entity.Name, MaterialStateEnum.Complete);
                Log(String.Format("{0}: [E] Validate Material Persistence is Complete on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));


                Log(String.Format("{0}: [S] Track Out of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
                if(!TrackOutEvaluator())
                {
                    return;
                }
                Log(String.Format("{0}: [E] Track Out of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));


                Log(String.Format("{0}: [S] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
                ValidatePersistenceDoesNotExists(MESScenario.Entity.Name);
                Log(String.Format("{0}: [E] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            } 
            else 
            {
                //Track Out occurs automatically, validate either Processed or Queued
                TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material {MESScenario.Entity.Name} System State is not Processed or Queued, automatic Track Out Failed"), () =>
                {
                    MESScenario.Entity.Load();
                    return MESScenario.Entity.SystemState.ToString().Equals(MaterialSystemState.Processed.ToString()) || MESScenario.Entity.SystemState.ToString().Equals(MaterialSystemState.Queued.ToString());
                });

                Log(String.Format("{0}: [S] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
                ValidatePersistenceDoesNotExists(MESScenario.Entity.Name);
                Log(String.Format("{0}: [E] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            }


            CarrierOutValidation(MESScenario, loadPortToSet);
        }

        public void SetRecipeBodyDefault(string resourceName, string serviceName)
        {
            RecipeManagement.SetRecipe("AnotherRecipe", new Byte[] { 0x32 });

            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName, ".\\RecipeBinaryFiles\\HEX-TEST");

            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();
            var recipeBody = recipe.Body;
            recipeBody.Load();
            var version = recipe.Version;

            RecipeManagement.RecipeSize = recipeBody.Body.Length;
            RecipeManagement.ValidateRecipeSize = true;
            RecipeManagement.ReplyWithCorrectBody = true;
            RecipeManagement.RecipeBody = recipeBody.Body;

            //Call DEE action called by Operator when clicking on GUI to Download Recipe to Equipment
            var action = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.GetActionByNameInput()
            {
                Name = "CustomAutomationSetRecipeBody"
            }.GetActionByNameSync().Action;

            var execute = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput()
            {
                Action = action,
                Input = new Dictionary<string, object>()
                 {
                     { "ResourceName", resourceName},
                     { "RecipeName", RecipeName},
                     { "RecipeVersion", version }
                 }
            }.ExecuteActionSync();

            Assert.IsTrue(execute.Output.ContainsKey("ResourceName"));
            Assert.IsTrue(execute.Output["ResourceName"].ToString() == resourceName);

            Assert.IsTrue(execute.Output.ContainsKey("RecipeName"));
            Assert.IsTrue(execute.Output["RecipeName"].ToString() == RecipeName);

            recipe.Load();

            Assert.IsTrue(execute.Output.ContainsKey("Checksum"));
            Assert.IsTrue(execute.Output["Checksum"].ToString() == recipe.BodyChecksum);
        }

        public void GetRecipeChecksumDefault(string resourceName, string serviceName)
        {
            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName, ".\\RecipeBinaryFiles\\HEX-TEST");

            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();
            var recipeBody = recipe.Body;
            recipeBody.Load();

            RecipeManagement.SetRecipe(RecipeName, recipeBody.Body);
            RecipeManagement.FailOnNewBody = true;
            RecipeManagement.RecipeExistsOnList = true;

            //Call DEE action called by Operator when clicking on GUI to Download Recipe to Equipment
            var action = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.GetActionByNameInput()
            {
                Name = "CustomAutomationGetRecipeChecksum"
            }.GetActionByNameSync().Action;

            var execute = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput()
            {
                Action = action,
                Input = new Dictionary<string, object>()
                 {
                     { "ResourceName", resourceName},
                     { "ResourceRecipeName", RecipeName}
                 }
            }.ExecuteActionSync();

            Assert.IsTrue(execute.Output.ContainsKey("Checksum"));
            recipe.Load();
            Assert.IsTrue(execute.Output["Checksum"].ToString() == recipe.BodyChecksum);
        }

        public virtual void TrackInEvaluator(string recipeName)
        {
            try
            {
                //Trackin Lot
                if (!String.IsNullOrEmpty(recipeName))
                {
                    var recipe = new Recipe() { Name = recipeName };
                    recipe.Load();
                    MESScenario.Resource.Load();

                    MESScenario.TrackIn(MESScenario.Resource, recipe, new Dictionary<Parameter, object>());
                }
                else
                {
                    MESScenario.Resource.Load();
                    MESScenario.TrackIn();
                }
            }
            catch (Exception ex)
            {
                if (TrackInMustFail)
                {
                    ValidatePersistenceDoesNotExists(MESScenario.Entity.Name);
                    throw new Exception("TrackInFailed");
                }
                //Assert.Fail(ex.ToString());
                throw ex;
            }

            if (TrackInMustFail)
            {
                //Assert.Fail("Track In should have failed with error");
                throw new Exception("Track In should have failed with error");
            }
        }


        public virtual bool TrackOutEvaluator()
        {
            try
            {
                MESScenario.TrackOut();
            }
            catch (Exception ex)
            {
                if (TrackOutMustFail)
                {
                    ValidatePersistenceExists(MESScenario.Entity.Name);
                    return false;
                }
            }

            if (TrackOutMustFail)
            {
                //Assert.Fail("Track In should have failed with error");
                throw new Exception("Track In should have failed with error");
            }

            return true;
        }


        public virtual void WaferStartValidation(Material material)
        {
            Log(String.Format("{0}: [S] Send Wafer Start Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));
            WaferStart(material);
            Log(String.Format("{0}: [E] Send Wafer Start Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));

            //Sleep to allow wafer start
            System.Threading.Thread.Sleep(500);

            Log(String.Format("{0}: [S] Validate MES Material {2} is In Process Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));
            ValidateSubMaterialState(material, MaterialStateModelStateEnum.InProcess.ToString());
            Log(String.Format("{0}: [E] Validate MES Material {2} is In Process Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));

        }

        public virtual void WaferCompleteValidation(Material material)
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

        public virtual void CarrierInValidation(CustomMaterialScenario MESScenario, int loadPortToSet)
        {
            Log(String.Format("{0}: [S] Carrier In Resource {1} Load Port {2}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, loadPortToSet));
            CarrierIn(MESScenario, loadPortToSet);
            Log(String.Format("{0}: [E] Carrier In Resource {1} Load Port {2}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, loadPortToSet));

            Log(String.Format("{0}: [S] Validating Load Port State Changed State Change to Transfer Blocked Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
            ValidateLoadPortState(MESScenario, LoadPortStateModelStateEnum.TransferBlocked.ToString());
            Log(String.Format("{0}: [E] Validating Load Port State Changed State Change to Transfer Blocked Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
        }

        public virtual void CarrierOutValidation(CustomMaterialScenario MESScenario, int loadPortToSet)
        {
            Log(String.Format("{0}: [S] Carrier Out Resource {1} Load Port {2}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, loadPortToSet));
            CarrierOut(MESScenario);
            Log(String.Format("{0}: [E] Carrier Out Resource {1} Load Port {2}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, loadPortToSet));

            Log(String.Format("{0}: [S] Validating Load Port State Changed State Change to Ready To Load Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
            ValidateLoadPortState(MESScenario, LoadPortStateModelStateEnum.ReadyToLoad.ToString());
            Log(String.Format("{0}: [E] Validating Load Port State Changed State Change to Ready To Load Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));

        }


        /// <summary>
        /// Retrieves Material Data from persistence
        /// </summary>
        /// <param name="materialName">The material name</param>
        /// <returns>The material data</returns>
        public MaterialData GetMaterialDataFromPersistence(string materialName)
		{
            MaterialData materialData = null;
            var orderPersistenceObj = new StoredItem();

            TestUtilities.WaitFor(ValidationTimeout, $"MaterialData for Material name {materialName} does not exist", () =>
            {
                try
                {
                    orderPersistenceObj = (this.Equipment.BaseImplementation as IoTEquipment).Persistency.StoredItems.FirstOrDefault(p => p.Identifier.Contains(MESScenario.Entity.Id.ToString()));
                    return orderPersistenceObj != null;
                }
                catch
                {
                    return false;
                }
            });

            JObject persistenceObject = JObject.Parse(File.ReadAllText(Path.Combine((this.Equipment.BaseImplementation as IoTEquipment).Persistency.StorePath, orderPersistenceObj.Value.ToString())));
            string json = persistenceObject.ToString(Formatting.None);
            if (persistenceObject != null)
			{
                materialData = persistenceObject.ToObject<MaterialData>();
            }           

            return materialData;
        }

        /// <summary>
        /// Retrieve the load port number from the persistence using the container name
        /// </summary>
        /// <param name="containerName">The container name</param>
        /// <returns>The load port number</returns>
        public int GetLoadPortFromContainerNameOnPersistence(string containerName)
		{
            int loadPortNumber = 0;
            var loadPortPersistenceObj = new StoredItem();

            try
            {
                loadPortPersistenceObj = (this.Equipment.BaseImplementation as IoTEquipment).Persistency.StoredItems.FirstOrDefault(p => p.Value.Equals(containerName));
                
                if(loadPortPersistenceObj != null)
				{
                    return (int)Char.GetNumericValue(loadPortPersistenceObj.Identifier[9]);
                }
            }
            catch
            {
                return loadPortNumber;
            }

            return loadPortNumber;
        }

        #region Order Persistence Validation    
        public MaterialData GetMaterialPersistence(string materialName, bool waitForMaterialHasValue = false)
        {

            var orderPersistenceObj = new StoredItem();
            TestUtilities.WaitFor(ValidationTimeout, String.Format($"MaterialData for Material name {materialName} does not exist"), () =>
            {
                try
                {
                    orderPersistenceObj = (this.Equipment.BaseImplementation as IoTEquipment).Persistency.StoredItems.FirstOrDefault(p => p.Identifier.Contains(MESScenario.Entity.Id.ToString()));
                    return orderPersistenceObj != null;
                }
                catch
                {
                    return false;
                }
            });


            JObject persistenceObject = JObject.Parse(File.ReadAllText(Path.Combine((this.Equipment.BaseImplementation as IoTEquipment).Persistency.StorePath, orderPersistenceObj.Value.ToString())));


            TestUtilities.WaitFor(ValidationTimeout, String.Format($"MaterialData for Material name {materialName} is not in Setup State"), () =>
            {
                return (persistenceObject["MaterialState"].ToString() == "Setup");
            });

            MaterialData structure = null;
            TestUtilities.WaitFor(ValidationTimeout, String.Format($"MaterialData for Material name {materialName} is either null or failed to load"), () =>
            {
                try
                {
                    structure = (this.Equipment.BaseImplementation as IoTEquipment).Persistency.GetValue<MaterialData>(orderPersistenceObj.Value, null);
                    if (waitForMaterialHasValue && structure == null)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            });
            return structure;
            //return (this.BaseImplementation as IoTEquipment).Persistency.GetValue<OrderStructure>(orderPersistenceObj.Identifier, null);

            //return null;
        }

        public void ValidatePersistenceState(string materialName, MaterialStateEnum state, int timeout = 30)
        {

            TestUtilities.WaitFor(timeout, String.Format($"Order persistence for {materialName} did not change to expected {state}"), () =>
            {
                //var material = GetMaterialPersistence(materialName);

                var orderPersistenceObj = new StoredItem();
                TestUtilities.WaitFor(ValidationTimeout, String.Format($"MaterialData for Material name {materialName} does not exist"), () =>
                {
                    try
                    {
                        orderPersistenceObj = (this.Equipment.BaseImplementation as IoTEquipment).Persistency.StoredItems.FirstOrDefault(p => p.Identifier.Contains(MESScenario.Entity.Id.ToString()));
                        return orderPersistenceObj != null;
                    }
                    catch
                    {
                        return false;
                    }
                });


                JObject persistenceObject = JObject.Parse(File.ReadAllText(Path.Combine((this.Equipment.BaseImplementation as IoTEquipment).Persistency.StorePath, orderPersistenceObj.Value.ToString())));


                return state.ToString().Equals(persistenceObject["MaterialState"].ToString());
            });


        }

        public void ValidatePersistenceDoesNotExists(string materialName)
        {
            var materialPersistenceObj = new StoredItem();
            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material Data {materialName} should not exist on persistence"), () =>
            {
                try
                {
                    materialPersistenceObj = (this.Equipment.BaseImplementation as IoTEquipment).Persistency.StoredItems.FirstOrDefault(p => p.Identifier.Contains(MESScenario.Entity.Id.ToString()));
                }
                catch
                {
                    return false;
                }
                return materialPersistenceObj == null;
            });
        }

        public void ValidatePersistenceExists(string materialName)
        {
            var materialPersistenceObj = new StoredItem();
            TestUtilities.WaitForNotChanged(ValidationTimeout, String.Format($"Material Data {materialName} should exist on persistence"), () =>
            {
                try
                {
                    materialPersistenceObj = (this.Equipment.BaseImplementation as IoTEquipment).Persistency.StoredItems.FirstOrDefault(p => p.Identifier.Contains(MESScenario.Entity.Id.ToString()));
                }
                catch
                {
                    return false;
                }
                return materialPersistenceObj == null;
            });


        }

        #endregion

        #region Container Peristency Validation
        public void ValidatePersistenceContainerExists(int loadPortNumber, string containerName = "")
        {
            if(string.IsNullOrEmpty(containerName))
            {
                var containerPersistenceObj = new StoredItem();
                TestUtilities.WaitFor(ValidationTimeout, String.Format($"Container for LP {loadPortNumber} should exist on persistence"), () =>
                {
                    try
                    {
                        containerPersistenceObj = (this.Equipment.BaseImplementation as IoTEquipment).Persistency.StoredItems.FirstOrDefault(p => p.Identifier.EndsWith($"LoadPort_{loadPortNumber}"));
                    }
                    catch
                    {
                        return false;
                    }
                    return (containerPersistenceObj != null);
                });
            }  

        }
        #endregion

        public void ValidateNotification(
            string expectedEvent,
            string expectedResource,
            int expectedNotificationCount = 1)
        {
            NotificationCollection notifications = new NotificationCollection();

            TestUtilities.WaitFor(30, "Notification mismatch.", () =>
            {
                notifications = NotificationUtilities.GetActiveNotifications();
                bool notificationsCount = true;
                if (expectedNotificationCount > 0)
                {
                    notificationsCount = notifications.Count == expectedNotificationCount;
                }
                return notificationsCount &&
                    notifications.Any(n => n.Title.Contains(expectedEvent) && n.Title.Contains(expectedResource));
            });

        }

        public void ValidateContainerIsDocked(CustomMaterialScenario MESScenario, int loadPortNumber)
        {
            ValidateContainerIsDocked(MESScenario.ContainerScenario.Entity, loadPortNumber);
        }

        public void ValidateContainerIsDocked(Container container, int loadPortNumber)
        {
            TestUtilities.WaitFor(45, String.Format($"Container {container.Name} was not correctly docked"), () =>
            {
                container.Load();
                container.LoadRelations();

                if (container.ResourceAssociationType == ContainerResourceAssociationType.DockedContainer
                && container.ContainerResourceRelations != null && container.ContainerResourceRelations.Count() > 0)
                {
                    var loadPort = container.ContainerResourceRelations.First().TargetEntity;
                    loadPort.Load();
                    if(loadPort.DisplayOrder == loadPortNumber)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            });
        }

        

        #region Connection Validation
        public virtual SecsMessage EstablishCommunicationActions(SecsMessage request, SecsMessage reply)
        {
            return reply;
        }

        public virtual SecsMessage GoOnlineActions(SecsMessage request, SecsMessage reply)
        {
            return reply;
        }

        private bool OnEstablishCommunication(SecsMessage request, SecsMessage reply)
        {
            bool? skipEstablishCommunication = ConnectionAttributes.ContainsKey("AutomationEquipmentSkipEstablishCommunication") ? (bool?)ConnectionAttributes.FirstOrDefault(c => c.Key == "").Value : false;

            if (skipEstablishCommunication.HasValue && skipEstablishCommunication.Value)
            {
                Assert.Fail("Establish Communication must not have been received");
            }

            reply = EstablishCommunicationActions(request, reply);

            return (true);
        }

        private bool OnGoOnline(SecsMessage request, SecsMessage reply)
        {
            bool? skipEstablishCommunication = ConnectionAttributes.ContainsKey("AutomationEquipmentSkipSetOnline") ? (bool?)ConnectionAttributes.FirstOrDefault(c => c.Key == "").Value : false;

            if (skipEstablishCommunication.HasValue && skipEstablishCommunication.Value)
            {
                Assert.Fail("Go Online must not have been received");
            }

            reply = GoOnlineActions(request, reply);

            return (true);
        }
        #endregion

        #region Utilities
        public Resource GetLoadPort(CustomMaterialScenario scenario, int loadPortNumber)
        {
            Resource loadPort;
            if (LoadPorts == null || LoadPorts.Count == 0)
            {
                LoadPorts = new List<Resource>();
                var resourceHierarchy = new Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects.GetDescendentResourcesInput()
                {
                    Resource = scenario.Resource,
                    Depth = 1
                }.GetDescendentResourcesSync().DescendentResources;

                var loadPorts = resourceHierarchy.Where(s => s.ChildResource.DisplayOrder != null && s.ChildResource.ProcessingType == ProcessingType.LoadPort).Select(s => s.ChildResource);
                LoadPorts.AddRange(loadPorts);
            }
            loadPort = LoadPorts.First(l => l.DisplayOrder == loadPortNumber);
            loadPort.Load();
            return loadPort;
        }


        ///// <summary>
        ///// Inserts data into the smart table Custom Resource Notification Configuration
        ///// </summary>
        ///// <param name="equipmentTypeNotification">Equipment Type Notification</param>
        ///// <param name="severity">Severity</param>
        ///// <param name="role">Role</param>
        ///// <param name="action">Action</param>
        ///// <param name="resource">Resource</param>
        ///// <param name="resourceType">Resource Type</param>
        ///// <param name="clearSmartTable">Clear Smart Table</param>
        //public void InsertDataIntoCustomResourceNotificationConfiguration(
        //    string equipmentTypeNotification,
        //    string severity,
        //    string role = "Administrators",
        //    string action = "Notification",
        //    string resource = null,
        //    string resourceType = null,
        //    bool clearSmartTable = true,
        //    string isEnable = "true")
        //{
        //    if (clearSmartTable)
        //    {
        //        smartTableManager.ClearSmartTable(AMSOsramConstants.STCustomResourceActionNotificationsName);
        //    }

        //    Dictionary<string, string> data = new Dictionary<string, string>()
        //    {
        //        { AMSOsramConstants.STCustomNotificationTriggerProperty, equipmentTypeNotification },
        //        { AMSOsramConstants.STCustomNotificationActionProperty, action },
        //        { AMSOsramConstants.STCustomNotificationTargetRoleProperty, role },
        //        { AMSOsramConstants.STCustomResourceActionNotificationResource, resource },
        //        { AMSOsramConstants.STCustomResourceActionNotificationResourceType, resourceType },
        //        { AMSOsramConstants.STCustomActionNotificationSeverity, severity },
        //        { AMSOsramConstants.STCustomNotificationIsEnableProperty, isEnable },
        //    };

        //    smartTableManager.SetSmartTableData(AMSOsramConstants.STCustomResourceActionNotificationsName, data);
        //}


        public void Log(string log)
        {
            Equipment.Environment.Log(log);
        }

        /// <summary>
		/// Creates a empty container scenario
		/// </summary>
		/// <param name="loadPort"></param>
		/// <param name="facility"></param>
		/// <returns></returns>
		public ContainerScenario CreateEmptyContainerScenario(int loadPort, string facilityName, string containerType)
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

            // Create Container to put the Wafers
            ContainerScenario containerScenario = new ContainerScenario();
            containerScenario.Entity.IsAutoGeneratePositionEnabled = false;
            containerScenario.Entity.Name = $"Container_{loadPort}_{DateTime.Now:yyyyMMdd_HHmmssfff}";
            containerScenario.Entity.Type = containerType; //AMSOsramConstants.ContainerTypeBEOL;
            containerScenario.Entity.PositionUnitType = ContainerPositionUnitType.Material;
            containerScenario.Entity.Facility = facility;
            containerScenario.Entity.CapacityUnits = AMSOsramConstants.UnitWafers;
            containerScenario.Entity.CapacityPerPosition = 1;
            containerScenario.Entity.TotalPositions = AMSOsramConstants.ContainerTotalPosition;
            containerScenario.Setup();

            return containerScenario;
        }


        ///// <summary>
        ///// Creates a Custom Sorter Job Definition based on the received parameters
        ///// </summary>
        ///// <param name="logisticalProcess">The logistical process</param>
        ///// <param name="sourceContainers">The source container</param>
        ///// <param name="destinationContainers">The destination container</param>
        ///// <param name="sourceContaineType">The source container type</param>
        ///// <param name="targetContainerType">The target container type</param>
        ///// <param name="futureActionType">The future action type</param>
        ///// <param name="fullTransferWafers">Flag if it is a full transfer wafer scenario</param>
        ///// <returns></returns>
        //public CustomSorterJobDefinition GetCustomSorterJobDefinition(string logisticalProcess,
        //    ContainerCollection sourceContainers,
        //    ContainerCollection destinationContainers,
        //    string sourceContaineType = AMSOsramConstants.ContainerSMIFPod,
        //    string targetContainerType = AMSOsramConstants.ContainerSMIFPod,
        //    string futureActionType = "",
        //    bool fullTransferWafers = false)
        //{
        //    CustomSorterJobDefinition customSorterJobDefinition = new CustomSorterJobDefinition
        //    {
        //        Name = Guid.NewGuid().ToString(),
        //        SourceCarrierType = sourceContaineType,
        //        TargetCarrierType = targetContainerType,
        //        LogisticalProcess = logisticalProcess
        //    };

        //    JArray temporaryMovementList = new JArray();

        //    JObject mainObj = new JObject
        //    {
        //        ["FutureActionType"] = futureActionType,
        //        ["Moves"] = temporaryMovementList,
        //    };

        //    if (logisticalProcess == AMSOsramConstants.CustomSorterLogisticalProcessTransferWafers)
        //    {
        //        if (futureActionType.Equals("Split", StringComparison.InvariantCulture)) // Split scenario
        //        {
        //            mainObj["DeleteOnCompletion"] = true;
        //            Container theOneThatWillBeSplitted = sourceContainers.FirstOrDefault();
        //            theOneThatWillBeSplitted.LoadRelation("MaterialContainer");

        //            int controlIndex = 1;
        //            int containerIndex = 0;

        //            foreach (var movement in theOneThatWillBeSplitted.ContainerMaterials)
        //            {
        //                Material material = movement.SourceEntity;
        //                material.Load();

        //                JObject jObject = new JObject
        //                {
        //                    ["MaterialName"] = material.Name,
        //                    ["SourceContainer"] = theOneThatWillBeSplitted.Name,
        //                    ["SourcePosition"] = movement.Position,
        //                    ["DestinationContainer"] = "#" + containerIndex + 1,
        //                    ["DestinationPosition"] = controlIndex
        //                };

        //                temporaryMovementList.Add(jObject);
        //                controlIndex++;

        //                if (controlIndex == 4)
        //                {
        //                    containerIndex++;
        //                    continue;
        //                }
        //            }
        //        }
        //        else if (futureActionType.Equals("Merge", StringComparison.InvariantCulture)) // Merge scenario
        //        {
        //            Container theOneOthersWillMergeInto = destinationContainers.FirstOrDefault();

        //            Queue<int> freePositions = new Queue<int>();
        //            for (int i = 1; i <= 25; i++)
        //            {
        //                if (theOneOthersWillMergeInto.ContainerMaterials == null || !theOneOthersWillMergeInto.ContainerMaterials.Any(m => m.Position == i))
        //                {
        //                    freePositions.Enqueue(i);
        //                }
        //            }

        //            foreach (Container container in sourceContainers)
        //            {
        //                container.LoadRelation("MaterialContainer");

        //                foreach (var movement in container.ContainerMaterials)
        //                {
        //                    Material material = movement.SourceEntity;
        //                    material.Load();

        //                    JObject jObject = new JObject
        //                    {
        //                        ["MaterialName"] = material.Name,
        //                        ["SourceContainer"] = container.Name,
        //                        ["SourcePosition"] = movement.Position,
        //                        ["DestinationContainer"] = theOneOthersWillMergeInto.Name,
        //                        ["DestinationPosition"] = freePositions.Dequeue()
        //                    };

        //                    temporaryMovementList.Add(jObject);
        //                }
        //            }
        //        }
        //        else // Simple transfer scenario
        //        {
        //            if (!fullTransferWafers)
        //            {
        //                Container theOneThatWillBeTranferred = sourceContainers.FirstOrDefault();
        //                theOneThatWillBeTranferred.LoadRelation("MaterialContainer");

        //                Container destinationContainer = destinationContainers.FirstOrDefault();
        //                destinationContainer.LoadRelation("MaterialContainer");

        //                Queue<int> freePositions = new Queue<int>();
        //                for (int i = 1; i <= 25; i++)
        //                {
        //                    if (destinationContainer.ContainerMaterials == null || !destinationContainer.ContainerMaterials.Any(m => m.Position == i))
        //                    {
        //                        freePositions.Enqueue(i);
        //                    }
        //                }

        //                foreach (var movement in theOneThatWillBeTranferred.ContainerMaterials)
        //                {
        //                    JObject jObject = new JObject
        //                    {
        //                        ["MaterialName"] = "",
        //                        ["SourceContainer"] = theOneThatWillBeTranferred.Name,
        //                        ["SourcePosition"] = movement.Position,
        //                        ["DestinationContainer"] = destinationContainer.Name,
        //                        ["DestinationPosition"] = freePositions.Dequeue()
        //                    };

        //                    temporaryMovementList.Add(jObject);
        //                }
        //            }
        //        }
        //    }
        //    else if (logisticalProcess == AMSOsramConstants.CustomSorterLogisticalProcessCompose)
        //    {
        //        JArray substitutes = new JArray();
        //        JObject jObjectSub = new JObject
        //        {
        //            ["Substitute"] = "",
        //            ["Priority"] = 0
        //        };
        //        substitutes.Add(jObjectSub);

        //        JObject jObject = new JObject
        //        {
        //            ["Product"] = "",
        //            ["Substitutes"] = substitutes,
        //            ["MaterialName"] = "",
        //            ["SourceContainer"] = "",
        //            ["SourcePosition"] = 0,
        //            ["DestinationContainer"] = "",
        //            ["DestinationPosition"] = 0
        //        };

        //        temporaryMovementList.Add(jObject);
        //    }

        //    customSorterJobDefinition.MovementList = mainObj.ToString();

        //    customSorterJobDefinition.Create();
        //    customSorterJobDefinition.Load();

        //    return customSorterJobDefinition;
        //}

        ///// <summary>
        ///// Inserts data into the Sorter context table
        ///// </summary>
        ///// <param name="stepName">The step</param>
        ///// <param name="customSorterJobDefinitionName">The CustomSorterJobDefinition</param>
        ///// <param name="productName">The product</param>
        ///// <param name="productGroupName">The product group</param>
        ///// <param name="flowName">The flow</param>
        ///// <param name="materialName">The material</param>
        ///// <param name="materialTypeName">The material type</param>
        ///// <param name="clearSmartTable">Flag to clear smart table</param>
        //public void InsertDataIntoCustomSorterJobDefinitionContextTable(string stepName,
        //    string customSorterJobDefinitionName,
        //    string productName = null,
        //    string productGroupName = null,
        //    string flowName = null,
        //    string materialName = null,
        //    string materialTypeName = null,
        //    bool clearSmartTable = true)
        //{
        //    string tableName = "CustomSorterJobDefinitionContext";

        //    if (clearSmartTable)
        //    {
        //        smartTableManager.ClearSmartTable(tableName);
        //    }

        //    Dictionary<string, string> data = new Dictionary<string, string>()
        //    {
        //        { "Step", stepName },
        //        { "Product", productName },
        //        { "ProductGroup", productGroupName },
        //        { "Flow", flowName },
        //        { "Material", materialName },
        //        { "MaterialType", materialTypeName },
        //        { "CustomSorterJobDefinition", customSorterJobDefinitionName }
        //    };

        //    smartTableManager.SetSmartTableData(tableName, data);
        //}
        #endregion
    }
}
