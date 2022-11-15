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
using System.Collections.Generic;
using Cmf.Custom.amsOSRAM.BusinessObjects;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Foundation.Common.Base;
using Newtonsoft.Json.Linq;
using amsOSRAMEIAutomaticTests.Objects.Persistence;
using Cmf.Custom.Tests.IoT.Tests.HermosLFM4xReader;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;

namespace amsOSRAMEIAutomaticTests.MechatronicMWS200
{
    [TestClass]
    public class MechatronicMWS200 : CommonTests
    {

        private const string resourceName = "ENA01";

        private int numberOfWafersPerLot = 4;

        public const string stepName = "M3-LO-Wafer Sorter-Split-01518F010_E";
        public const string flowName = "FOL-UX3_EPA";

        private int sourceLoadPortNumber;
        private int destinationLoadPortNumber;


        private CustomSorterJobDefinition customSorterJobDefinition = new CustomSorterJobDefinition();

        private static SmartTableManager smartTableManager = new SmartTableManager();
        private static GenericTableManager genericTableManager = new GenericTableManager();
        private Dictionary<int, ContainerScenario> containerScenariosUsed = new Dictionary<int, ContainerScenario>();
        private List<int> loadPortsUsed = new List<int>();
        private List<Material> materialsUsed = new List<Material>();

        private readonly Dictionary<int, string> loadPortNames = new Dictionary<int, string>()
        {
            { 1, "ENA01-LP01" },
            { 2, "ENA01-LP02" },
            { 3, "ENA01-LP03" },
            { 4, "ENA01-LP03" },
            { 5, "ENA01-LP04" },
            { 6, "ENA01-LP05" },
            { 7, "ENA01-LP06" },
            { 8, "ENA01-LP07" }
        };

        private Dictionary<int, string> containerAtLoadPort = null;

        private const int numberOfLoadPorts = 4;

        public const bool subMaterialTrackin = false;

        public string recipeName = "TestRecipeForMechatronicMWS200";
        public const string serviceName = "Sorter-Split";

        private int loadPortNumber = 1;

        private bool isOnlineRemote = true;
        public bool successWaferIdRead = true;

        public bool createControlJobReceived = false;
        public bool createControlJobDenied = false;

        public bool failAtProcessJob = false;
        public bool createProcessJobReceived = false;
        public bool createProcessJobDenied = false;
        public bool proceedWithCarrierCommandRecieved = false;
        public bool proceedWithSubstrateCommandRecieved = false;
        public bool cancelSubstrateCommandRecieved = false;

        public HermosLFM4xReader RFIDReader = new HermosLFM4xReader();
        public const string readerResourceName = "ENA01.RFID";

        [TestInitialize]
        public void TestInit()
        {
            StepPreparations();

            base.Equipment = m_Scenario.GetEquipment(m_Scenario.EquipmentToTest) as SecsGemEquipment;
            
            base.Initialize(recipeName);
            base.SubMaterialTrackin = subMaterialTrackin;

            base.Equipment.RegisterOnMessage("S3F17", OnS3F17);
            base.Equipment.RegisterOnMessage("S1F3", OnS1F3);
            base.Equipment.RegisterOnMessage("S14F9", OnS14F9);
            base.Equipment.RegisterOnMessage("S14F19", OnS14F19);
            base.Equipment.RegisterOnMessage("S16F11", OnS16F11);

            base.LoadPortNumber = loadPortNumber;
            
            loadPortNames.Values.ToList().ForEach(loadPortName =>
            {
                var loadPort = new Resource { Name = loadPortName };
                loadPort.Load();
                
                if (loadPort.ProcessingType != ProcessingType.LoadPort) {
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

            containerAtLoadPort = null;
            RFIDReader.TestInit(readerResourceName, m_Scenario);
            RFIDReader.ClearFlags();
        }

        [TestCleanup]
        public void TestCleanup()
        {

            isOnlineRemote = true;

            createControlJobReceived = false;
            createControlJobDenied = false;

            createProcessJobReceived = false;
            createProcessJobDenied = false;
            proceedWithCarrierCommandRecieved = false;
            proceedWithSubstrateCommandRecieved = false;
            cancelSubstrateCommandRecieved = false;

            containerAtLoadPort = null;

            foreach (var containerScenario in containerScenariosUsed)
            {
                
                if (containerScenario.Value != null && MESScenario.ContainerScenario?.Entity.Name != containerScenario.Value.Entity.Name)
                {
                    containerScenario.Value.TearDown();
                }
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

            loadPortsUsed.Clear();
            containerScenariosUsed.Clear();

            var resourceToAbortMaterial = new Resource { Name = resourceName };
            resourceToAbortMaterial.Load();
            EnsureMaterialStartConditions(resourceToAbortMaterial);
            
            RFIDReader.CleanUp(MESScenario);
            base.CleanUp(MESScenario);


        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ConfigureConnection(readerResourceName, 5014, prepareTestScenario: false);
            ConfigureConnection(resourceName, 5015, connectionAttributes: new Dictionary<string, object>() { { "IsSorter", true } });
            //ConfigureConnection(resourceName, 5013);

        }


        [ClassCleanup]
        public static void ClassCleanup()
        {
            Cleanup();
        }

        private void PrepareSorterScenarioInitializeAndSetup(bool ignoreCarrierIn = false, bool IgnoreMaterialScenarioSetup = true)
        {
            base.IgnoreCarrierIn = ignoreCarrierIn;
            base.IgnoreMaterialScenarioSetup = IgnoreMaterialScenarioSetup;
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, numberOfWafersPerLot);
            MESScenario.Setup(false, automaticContainerPositions: false, containerType: base.ContainerType);
            PostSetupActions(MESScenario);
            EnsureLoadPortStartConditions(MESScenario.Resource);
        }


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
            base.MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, 5);

            customSorterJobDefinition = GetCustomSorterJobDefinition(amsOSRAMConstants.CustomSorterLogisticalProcessMapCarrier, new ContainerCollection() { }, new ContainerCollection() { });
            InsertDataIntoCustomSorterJobDefinitionContextTable(stepName, customSorterJobDefinition.Name, materialName: MESScenario.Entity.Name);


            containerScenariosUsed.Add(1, MESScenario.ContainerScenario);

            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName);

            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();

            RecipeManagement.SetRecipe(recipe.ResourceRecipeName, RecipeName);
            RecipeManagement.FailOnNewBody = true;
            RecipeManagement.RecipeExistsOnList = true;

            base.RunBasicTest(MESScenario, LoadPortNumber, false, automatedMaterialOut: true, fullyAutomatedMaterialMovement: true);
        }

        /// <summary>
        /// Perform Transfer with a CustomSorterJobDefinitionContext in place
        /// </summary>
        [TestMethod,
            Description
            (
            "- Dock a container into Load Port 1" +
            "- Calls Material IN orchestration" +
            "- SorterJobDefinitionContext is resolved" +
            "- ")]
        public void MechatronicMWS200_Transfer_WithSorterJobDefinitionContext()
        {
            base.LoadPortNumber = sourceLoadPortNumber = 1;
            destinationLoadPortNumber = 2;
            loadPortsUsed.Add(1);
            loadPortsUsed.Add(2);
            PrepareSorterScenarioInitializeAndSetup();

            ContainerScenario containerScenarioForLoadPort2 = CreateEmptyContainerScenario(destinationLoadPortNumber, null, amsOSRAMConstants.ContainerSMIFPod);
            //MESContainer mesContainer = new MESContainer(containerScenarioForLoadPort2.Entity.Name);
            // m_Scenario.ScenarioContainers.Add(mesContainer.Container);

            customSorterJobDefinition = GetCustomSorterJobDefinition(amsOSRAMConstants.CustomSorterLogisticalProcessTransferWafers,
                sourceContainers: new ContainerCollection() { base.MESScenario.ContainerScenario.Entity },
                destinationContainers: new ContainerCollection() { containerScenarioForLoadPort2.Entity },
                fullTransferWafers: true);

            containerScenariosUsed.Add(sourceLoadPortNumber, base.MESScenario.ContainerScenario);
            containerScenariosUsed.Add(destinationLoadPortNumber, containerScenarioForLoadPort2);

            InsertDataIntoCustomSorterJobDefinitionContextTable(stepName, customSorterJobDefinition.Name, materialName: MESScenario.Entity.Name);



            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName);


            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();

            RecipeManagement.SetRecipe(recipe.ResourceRecipeName, RecipeName);
            RecipeManagement.FailOnNewBody = true;
            RecipeManagement.RecipeExistsOnList = true;

            SorterCarrierIn();

            Thread.Sleep(1000);

            #region TrackIn

            Log(String.Format("{0}: [S] Track In of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            try
            {
                TrackInEvaluator(RecipeName);
            }
            catch (Exception ex)
            {
                if (ex.Message == "TrackInFailed")
                    return;
                else
                    Assert.Fail(ex.Message);
            }
            Log(String.Format("{0}: [E] Track In of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            Log(String.Format("{0}: [S] Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
            bool postTrackInResult = PostTrackInActions(MESScenario);
            Log(String.Format("{0}: [E] Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));

            //intentional step out of the test by returning false on post track in result
            if (!postTrackInResult)
            {
                Log(String.Format("{0}: [S] Test Concluded by Returning false on Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
                return;
            }

            #endregion

            // Fetching Material Data also validates the material is in state 'Setup'
            MaterialData materialData = GetMaterialDataFromPersistence(MESScenario.Entity.Name);

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


            CustomProcessWafersInMovementList(materialData, 4, "");

            Log(String.Format("{0}: [S] Sending Process Complete Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ProcessCompleteEvent(MESScenario);
            Log(String.Format("{0}: [E] Sending Process Complete Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            Log(String.Format("{0}: [S] Validate Material is Complete on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ValidateMaterialStateModelState(MESScenario, MaterialStateModelStateEnum.Complete.ToString());
            Log(String.Format("{0}: [E] Validate Material is Complete on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));



            //Track Out occurs automatically, validate either Processed or Queued
            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material {MESScenario.Entity.Name} System State is not Processed or Queued, automatic Track Out Failed"), () =>
            {
                MESScenario.Entity.Load();
                return MESScenario.Entity.SystemState.ToString().Equals(MaterialSystemState.Processed.ToString()) || MESScenario.Entity.SystemState.ToString().Equals(MaterialSystemState.Queued.ToString());
            });

            Log(String.Format("{0}: [S] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ValidatePersistenceDoesNotExists(MESScenario.Entity.Name);
            Log(String.Format("{0}: [E] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            CustomValidateContainerNumberOfWafers(MESScenario.ContainerScenario.Entity, 0);
            CustomValidateContainerNumberOfWafers(containerScenarioForLoadPort2.Entity, 4);


            CarrierOutValidation(MESScenario, loadPortNumber);
        }

        /// <summary>
        /// Perform Transfer with a CustomSorterJobDefinitionContext in place
        /// </summary>
        [TestMethod,
            Description
            (
            "- Dock a container into Load Port 1" +
            "- Calls Material IN orchestration" +
            "- SorterJobDefinitionContext is resolved" +
            "- ")]
        public void MechatronicMWS200_SortDesc_WithSorterJobDefinitionContext()
        {
            base.LoadPortNumber = sourceLoadPortNumber = 1;
            destinationLoadPortNumber = 1;
            loadPortsUsed.Add(1);
            PrepareSorterScenarioInitializeAndSetup();

            ContainerScenario containerScenarioForLoadPort2 = CreateEmptyContainerScenario(destinationLoadPortNumber, null, amsOSRAMConstants.ContainerSMIFPod);
            // MESContainer mesContainer = new MESContainer(containerScenarioForLoadPort2.Entity.Name);
            // m_Scenario.ScenarioContainers.Add(mesContainer.Container);

            customSorterJobDefinition = GetCustomSorterJobDefinition(amsOSRAMConstants.CustomSorterLogisticalProcessTransferWafers,
                sourceContainers: new ContainerCollection() { base.MESScenario.ContainerScenario.Entity },
                destinationContainers: new ContainerCollection() { base.MESScenario.ContainerScenario.Entity },
                sortDescending: true);

            containerScenariosUsed.Add(sourceLoadPortNumber, base.MESScenario.ContainerScenario);

            InsertDataIntoCustomSorterJobDefinitionContextTable(stepName, customSorterJobDefinition.Name, materialName: MESScenario.Entity.Name);



            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName);


            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();

            RecipeManagement.SetRecipe(recipe.ResourceRecipeName, RecipeName);
            RecipeManagement.FailOnNewBody = true;
            RecipeManagement.RecipeExistsOnList = true;

            SorterCarrierIn();

            Thread.Sleep(3000);

            #region TrackIn

            Log(String.Format("{0}: [S] Track In of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            try
            {
                TrackInEvaluator(RecipeName);
            }
            catch (Exception ex)
            {
                if (ex.Message == "TrackInFailed")
                    return;
                else
                    Assert.Fail(ex.Message);
            }
            Log(String.Format("{0}: [E] Track In of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            Log(String.Format("{0}: [S] Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
            bool postTrackInResult = PostTrackInActions(MESScenario);
            Log(String.Format("{0}: [E] Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));

            //intentional step out of the test by returning false on post track in result
            if (!postTrackInResult)
            {
                Log(String.Format("{0}: [S] Test Concluded by Returning false on Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
                return;
            }

            #endregion

            // Fetching Material Data also validates the material is in state 'Setup'
            MaterialData materialData = GetMaterialDataFromPersistence(MESScenario.Entity.Name);

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


            CustomProcessWafersInMovementList(materialData, 4, "");

            Log(String.Format("{0}: [S] Sending Process Complete Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ProcessCompleteEvent(MESScenario);
            Log(String.Format("{0}: [E] Sending Process Complete Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            Log(String.Format("{0}: [S] Validate Material is Complete on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ValidateMaterialStateModelState(MESScenario, MaterialStateModelStateEnum.Complete.ToString());
            Log(String.Format("{0}: [E] Validate Material is Complete on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));



            //Track Out occurs automatically, validate either Processed or Queued
            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material {MESScenario.Entity.Name} System State is not Processed or Queued, automatic Track Out Failed"), () =>
            {
                MESScenario.Entity.Load();
                return MESScenario.Entity.SystemState.ToString().Equals(MaterialSystemState.Processed.ToString()) || MESScenario.Entity.SystemState.ToString().Equals(MaterialSystemState.Queued.ToString());
            });

            Log(String.Format("{0}: [S] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ValidatePersistenceDoesNotExists(MESScenario.Entity.Name);
            Log(String.Format("{0}: [E] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            
            CustomValidateContainerNumberOfWafers(MESScenario.ContainerScenario.Entity, 4);
            //CustomValidateContainerNumberOfWafers(containerScenarioForLoadPort2.Entity, 4);


            CarrierOutValidation(MESScenario, loadPortNumber);
        }

        /// <summary>
        /// Perform Transfer with a CustomSorterJobDefinitionContext in place
        /// </summary>
        [TestMethod,
            Description
            (
            "- Dock a container into Load Port 1" +
            "- Calls Material IN orchestration" +
            "- SorterJobDefinitionContext is resolved" +
            "- ")]
        public void MechatronicMWS200_Split_WithSorterJobDefinitionContext()
        {
            #region TestPreparation
            base.LoadPortNumber = sourceLoadPortNumber = 1;
            destinationLoadPortNumber = 2;
            loadPortsUsed.Add(1);
            loadPortsUsed.Add(2);
            loadPortsUsed.Add(3);

            CustomMaterialScenario matScenario = InitializeMaterialScenario(resourceName, flowName, stepName, 6);
            base.MESScenario = matScenario;
            base.MESScenario.Setup();

            ContainerScenario containerScenarioForLoadPort2 = CreateEmptyContainerScenario(destinationLoadPortNumber, MESScenario.Entity.Facility.Name ?? null, amsOSRAMConstants.ContainerSMIFPod);

            ContainerScenario containerScenarioForLoadPort3 = CreateEmptyContainerScenario(3, MESScenario.Entity.Facility.Name ?? null, amsOSRAMConstants.ContainerSMIFPod);

            customSorterJobDefinition = GetCustomSorterJobDefinition(
                logisticalProcess: amsOSRAMConstants.CustomSorterLogisticalProcessTransferWafers,
                sourceContainers: new ContainerCollection() { MESScenario.ContainerScenario.Entity },
                destinationContainers: new ContainerCollection() { containerScenarioForLoadPort2.Entity, containerScenarioForLoadPort3.Entity },
                futureActionType: "Split");

            InsertDataIntoCustomSorterJobDefinitionContextTable(stepName, customSorterJobDefinition.Name, materialName: MESScenario.Entity.Name);

            containerScenariosUsed.Add(1, base.MESScenario.ContainerScenario);
            containerScenariosUsed.Add(2, containerScenarioForLoadPort2);
            containerScenariosUsed.Add(3, containerScenarioForLoadPort3);


            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName);


            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();

            RecipeManagement.SetRecipe(recipe.ResourceRecipeName, RecipeName);
            RecipeManagement.FailOnNewBody = true;
            RecipeManagement.RecipeExistsOnList = true;

            #endregion


            SorterCarrierIn();

            Thread.Sleep(1000);

            #region TrackIn

            Log(String.Format("{0}: [S] Track In of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            try
            {
                TrackInEvaluator(RecipeName);
            }
            catch (Exception ex)
            {
                if (ex.Message == "TrackInFailed")
                    return;
                else
                    Assert.Fail(ex.Message);
            }
            Log(String.Format("{0}: [E] Track In of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            Log(String.Format("{0}: [S] Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
            bool postTrackInResult = PostTrackInActions(MESScenario);
            Log(String.Format("{0}: [E] Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));

            //intentional step out of the test by returning false on post track in result
            if (!postTrackInResult)
            {
                Log(String.Format("{0}: [S] Test Concluded by Returning false on Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
                return;
            }

            #endregion

            // Fetching Material Data also validates the material is in state 'Setup'
            MaterialData materialData = GetMaterialDataFromPersistence(MESScenario.Entity.Name);

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


            CustomProcessWafersInMovementList(materialData, 6, "Split");

            Log(String.Format("{0}: [S] Sending Process Complete Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ProcessCompleteEvent(MESScenario);
            Log(String.Format("{0}: [E] Sending Process Complete Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            Log(String.Format("{0}: [S] Validate Material is Complete on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ValidateMaterialStateModelState(MESScenario, MaterialStateModelStateEnum.Complete.ToString());
            Log(String.Format("{0}: [E] Validate Material is Complete on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));



            //Track Out occurs automatically, validate either Processed or Queued
            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material {MESScenario.Entity.Name} System State is not Processed or Queued, automatic Track Out Failed"), () =>
            {
                MESScenario.Entity.Load();
                return MESScenario.Entity.SystemState.ToString().Equals(MaterialSystemState.Processed.ToString()) || MESScenario.Entity.SystemState.ToString().Equals(MaterialSystemState.Queued.ToString());
            });

            Log(String.Format("{0}: [S] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ValidatePersistenceDoesNotExists(MESScenario.Entity.Name);
            Log(String.Format("{0}: [E] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            CustomValidateContainerNumberOfWafers(MESScenario.ContainerScenario.Entity, 0);

            CarrierOutValidation(MESScenario, loadPortNumber);
        }

        /// <summary>
        /// Perform Transfer with a CustomSorterJobDefinitionContext in place
        /// </summary>
        [TestMethod,
            Description
            (
            "- Dock a container into Load Port 1" +
            "- Calls Material IN orchestration" +
            "- SorterJobDefinitionContext is resolved" +
            "- ")]
        public void MechatronicMWS200_Merge_WithSorterJobDefinitionContext()
        {
            base.LoadPortNumber = sourceLoadPortNumber = 1;
            destinationLoadPortNumber = 2;
            loadPortsUsed.Add(1);
            loadPortsUsed.Add(2);
            loadPortsUsed.Add(3);

            CustomMaterialScenario matScenario = InitializeMaterialScenario(resourceName, flowName, stepName, 6);
            base.MESScenario = matScenario;
            base.MESScenario.Setup();
            IgnoreMaterialScenarioSetup = true;

            #region Create containers scenario

            ContainerScenario containerScenarioForLoadPort2 = CreateEmptyContainerScenario(2, MESScenario.Entity.Facility.Name ?? null, amsOSRAMConstants.ContainerSMIFPod);
            ContainerScenario containerScenarioForLoadPort3 = CreateEmptyContainerScenario(3, MESScenario.Entity.Facility.Name ?? null, amsOSRAMConstants.ContainerSMIFPod);

            containerAtLoadPort = new Dictionary<int, string>();
            containerAtLoadPort.Add(1, matScenario.ContainerScenario.Entity.Name);
            containerAtLoadPort.Add(2, containerScenarioForLoadPort2.Entity.Name);
            containerAtLoadPort.Add(3, containerScenarioForLoadPort3.Entity.Name);

            #endregion Create containers scenario

            #region SPLIT

            SplitInputParametersCollection splitInputParameters = new SplitInputParametersCollection();
            string lotName = DateTime.Now.Ticks.ToString();
            MESScenario.Entity.Load();
            MESScenario.LoadChildren();

            SplitInputParameters splitInputParameters1 = new SplitInputParameters
            {
                Name = lotName + ".001",
                PrimaryQuantity = 0,
                MaterialContainer = null,
                SubMaterials = new SplitInputSubMaterialCollection()
                {
                    new SplitInputSubMaterial
                    {
                        IsToDisassociate = false,
                        SubMaterial = MESScenario.Entity.SubMaterials[0],
                        MaterialContainer = new MaterialContainer()
                        {
                            SourceEntity = matScenario.Entity.SubMaterials[0],
                            TargetEntity =containerScenarioForLoadPort2.Entity,
                            Position = 1
                        }
                    },
                    new SplitInputSubMaterial
                    {
                        IsToDisassociate = false,
                        SubMaterial = MESScenario.Entity.SubMaterials[1],
                        MaterialContainer = new MaterialContainer()
                        {
                            SourceEntity = matScenario.Entity.SubMaterials[1],
                            TargetEntity = containerScenarioForLoadPort2.Entity,
                            Position = 2
                        }
                    }
                }
            };

            splitInputParameters.Add(splitInputParameters1);

            SplitInputParameters splitInputParameters2 = new SplitInputParameters
            {
                Name = lotName + ".002",
                PrimaryQuantity = 0,
                MaterialContainer = null,
                SubMaterials = new SplitInputSubMaterialCollection()
                {
                    new SplitInputSubMaterial
                    {
                        IsToDisassociate = false,
                        SubMaterial = matScenario.Entity.SubMaterials[2],
                        MaterialContainer = new MaterialContainer()
                        {
                            SourceEntity = matScenario.Entity.SubMaterials[2],
                            TargetEntity = containerScenarioForLoadPort3.Entity,
                            Position = 1
                        }
                    },
                    new SplitInputSubMaterial
                    {
                        IsToDisassociate = false,
                        SubMaterial = matScenario.Entity.SubMaterials[3],
                        MaterialContainer = new MaterialContainer()
                        {
                            SourceEntity = matScenario.Entity.SubMaterials[3],
                            TargetEntity = containerScenarioForLoadPort3.Entity,
                            Position = 2
                        }
                    }
                }
            };

            splitInputParameters.Add(splitInputParameters2);

            MESScenario.Entity.Split(splitInputParameters);
            MESScenario.Entity.Load();

            Material materialToMerge1 = new Material { Name = lotName + ".001" };
            Material materialToMerge2 = new Material { Name = lotName + ".002" };
            materialsUsed.Add(materialToMerge1);
            materialsUsed.Add(materialToMerge2);

            #endregion SPLIT

            customSorterJobDefinition = GetCustomSorterJobDefinition(amsOSRAMConstants.CustomSorterLogisticalProcessTransferWafers,
                sourceContainers: new ContainerCollection() { containerScenarioForLoadPort2.Entity, containerScenarioForLoadPort3.Entity },
                destinationContainers: new ContainerCollection() { base.MESScenario.ContainerScenario.Entity },
                futureActionType: "Merge");

            containerScenariosUsed.Add(1, base.MESScenario.ContainerScenario);
            containerScenariosUsed.Add(2, containerScenarioForLoadPort2);
            containerScenariosUsed.Add(3, containerScenarioForLoadPort3);

            InsertDataIntoCustomSorterJobDefinitionContextTable(stepName, customSorterJobDefinition.Name, materialName: matScenario.Entity.Name);



            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, serviceName);


            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();

            RecipeManagement.SetRecipe(recipe.ResourceRecipeName, RecipeName);
            RecipeManagement.FailOnNewBody = true;
            RecipeManagement.RecipeExistsOnList = true;

            SorterCarrierIn();

            Thread.Sleep(1000);

            #region TrackIn

            Log(String.Format("{0}: [S] Track In of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            try
            {
                TrackInEvaluator(RecipeName);
            }
            catch (Exception ex)
            {
                if (ex.Message == "TrackInFailed")
                    return;
                else
                    Assert.Fail(ex.Message);
            }
            Log(String.Format("{0}: [E] Track In of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            Log(String.Format("{0}: [S] Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
            bool postTrackInResult = PostTrackInActions(MESScenario);
            Log(String.Format("{0}: [E] Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));

            //intentional step out of the test by returning false on post track in result
            if (!postTrackInResult)
            {
                Log(String.Format("{0}: [S] Test Concluded by Returning false on Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
                return;
            }

            #endregion

            // Fetching Material Data also validates the material is in state 'Setup'
            MaterialData materialData = GetMaterialDataFromPersistence(MESScenario.Entity.Name);

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


            CustomProcessWafersInMovementList(materialData, 4, "Merge");


            var x = MESScenario.ContainerScenario.Entity.Name;

            Log(String.Format("{0}: [S] Sending Process Complete Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ProcessCompleteEvent(MESScenario);
            Log(String.Format("{0}: [E] Sending Process Complete Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            Log(String.Format("{0}: [S] Validate Material is Complete on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ValidateMaterialStateModelState(MESScenario, MaterialStateModelStateEnum.Complete.ToString());
            Log(String.Format("{0}: [E] Validate Material is Complete on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));



            //Track Out occurs automatically, validate either Processed or Queued
            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material {MESScenario.Entity.Name} System State is not Processed or Queued, automatic Track Out Failed"), () =>
            {
                MESScenario.Entity.Load();
                return MESScenario.Entity.SystemState.ToString().Equals(MaterialSystemState.Processed.ToString()) || MESScenario.Entity.SystemState.ToString().Equals(MaterialSystemState.Queued.ToString());
            });

            Log(String.Format("{0}: [S] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ValidatePersistenceDoesNotExists(MESScenario.Entity.Name);
            Log(String.Format("{0}: [E] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            CustomValidateContainerNumberOfWafers(MESScenario.ContainerScenario.Entity, 6);

        }


        /// <summary>
        /// Perform Transfer with a CustomSorterJobDefinitionContext in place
        /// </summary>
        [TestMethod,
            Description
            (
            "- Dock a container into Load Port 1" +
            "- Calls Material IN orchestration" +
            "- SorterJobDefinitionContext is resolved" +
            "- ")]
        public void MechatronicMWS200_WaferReception_WithSorterJobDefinitionContext()
        {
            int targetWafersFirstContainer, targetWafersSecondContainer, targetWafersThirdContainer, wafersOnSourceContainer;


            wafersOnSourceContainer = 25;
            targetWafersSecondContainer = targetWafersFirstContainer = 10;
            targetWafersThirdContainer = wafersOnSourceContainer - targetWafersSecondContainer - targetWafersFirstContainer;

            var waferName = "MaterialTest_" + DateTime.Now.Ticks.ToString();
            var productName = amsOSRAMConstants.DefaultWaferProductName;
            var sourceCapacity = wafersOnSourceContainer.ToString();
            var targetCapacity = targetWafersFirstContainer.ToString();

            base.LoadPortNumber = sourceLoadPortNumber = 1;

            loadPortsUsed.Add(1);
            loadPortsUsed.Add(2);
            loadPortsUsed.Add(3);
            loadPortsUsed.Add(4);

            IgnoreMaterialScenarioSetup = true;

            #region Pre-Conditions Setup

            Step waferReceptionStep = new Step();
            waferReceptionStep.Name = stepName;
            waferReceptionStep.Load();
            waferReceptionStep.LoadAttributes();

            waferReceptionStep.SaveAttribute("IsWaferReception", true);

            Resource resource = new Resource { Name = resourceName };
            resource.Load();
            resource.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter, true);

            smartTableManager.ClearSmartTable(amsOSRAMConstants.CustomProductContainerCapacitiesSmartTable);
            smartTableManager.SetSmartTableData(amsOSRAMConstants.CustomProductContainerCapacitiesSmartTable, new Dictionary<string, string>
                {
                    { "Product", productName },
                    { "SourceCapacity", sourceCapacity },
                    { "TargetCapacity", targetCapacity }
                });

            #endregion

            #region Material Setup


            MESScenario = InitializeMaterialScenario(resourceName, flowName, stepName, 0);
            MESScenario.CreateContainer = false;
            MESScenario.Setup(productName: productName);

            //ContainerScenario containerScenarioForLoadPort1 = CreateEmptyContainerScenario(1, MESScenario.Entity.Facility.Name ?? null, amsOSRAMConstants.ContainerPeekCassete25, totalPositions: wafersOnSourceContainer);


            MaterialCollection materialsForContainer1 = new MaterialCollection();

            for (int i = 1; i <= wafersOnSourceContainer; i++)
            {
                Material mat = new Material
                {
                    Name = waferName + "." + i,
                    Flow = MESScenario.Flow,
                    Step = waferReceptionStep,
                    Product = MESScenario.Entity.Product,
                    Type = amsOSRAMConstants.DefaultMaterialType,
                    Form = amsOSRAMConstants.FormLogicalWafer,
                    Facility = MESScenario.Facility,
                    PrimaryUnits = amsOSRAMConstants.UnitWafers,
                    PrimaryQuantity = 1
                };
                mat.Create();
                mat.ChangeFlowAndStep(MESScenario.Flow, waferReceptionStep);
                materialsForContainer1.Add(mat);
            }

            //containerScenarioForLoadPort1.AssociateMaterials(materialsForContainer1);


            #endregion

            #region Recipe Setup

            //var WaferVerificationScenario = new Parameter();
            //WaferVerificationScenario.Name = nameof(WaferVerificationScenario).ToUpper();
            //WaferVerificationScenario.Type = "Standard";

            //parameters.Add(WaferVerificationScenario, "SEMISTANDARD");
            var parameters = new Dictionary<Parameter, object>();

            var test = new Parameter();
            test.Name = "Param1";
            test.Type = "Standard";

            parameters.Add(test, "SEMISTANDARD");

            RecipeUtilities.CreateMESRecipeIfItDoesNotExist(resourceName, RecipeName, RecipeName, amsOSRAMConstants.ServiceWaferReception, parameterCollection: parameters);

            var recipe = new Recipe() { Name = RecipeName };
            recipe.Load();


            RecipeManagement.SetRecipe(recipe.ResourceRecipeName, RecipeName);
            RecipeManagement.FailOnNewBody = true;
            RecipeManagement.RecipeExistsOnList = true;

            #endregion

            #region Create containers scenario
            //
            ContainerScenario containerScenarioForLoadPort2 = CreateEmptyContainerScenario(2, MESScenario.Entity.Facility.Name ?? null, amsOSRAMConstants.ContainerSMIFPod);
            ContainerScenario containerScenarioForLoadPort3 = CreateEmptyContainerScenario(3, MESScenario.Entity.Facility.Name ?? null, amsOSRAMConstants.ContainerSMIFPod);
            ContainerScenario containerScenarioForLoadPort4 = CreateEmptyContainerScenario(4, MESScenario.Entity.Facility.Name ?? null, amsOSRAMConstants.ContainerSMIFPod);

            containerAtLoadPort = new Dictionary<int, string>();
            containerAtLoadPort.Add(1, null);
            containerAtLoadPort.Add(2, containerScenarioForLoadPort2.Entity.Name);
            containerAtLoadPort.Add(3, containerScenarioForLoadPort3.Entity.Name);
            containerAtLoadPort.Add(4, containerScenarioForLoadPort4.Entity.Name);

            #endregion Create containers scenario


            containerScenariosUsed.Add(1, null);
            containerScenariosUsed.Add(2, containerScenarioForLoadPort2);
            containerScenariosUsed.Add(3, containerScenarioForLoadPort3);
            containerScenariosUsed.Add(4, containerScenarioForLoadPort4);

            //InsertDataIntoCustomSorterJobDefinitionContextTable(stepName, customSorterJobDefinition.Name);

            SorterCarrierIn(IsWaferReception : true);

            Thread.Sleep(1000);

            #region TrackIn

            Log(String.Format("{0}: [S] Track In of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            try
            {
                //TrackInEvaluator(RecipeName);

                var action = new Cmf.Foundation.Common.DynamicExecutionEngine.Action();

                //Call DEE action called by Operator when clicking on GUI to start the process
                action = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.GetActionByNameInput()
                {
                    Name = "CustomSendAdHocTransferInformationToIoT"
                }.GetActionByNameSync().Action;

                var deeOutput = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput()
                {
                    Action = action,
                    Input = new Dictionary<string, object>()
                    {
                        { "Resource", resourceName },
                        { "SorterProcess", amsOSRAMConstants.CustomSorterProcessWaferReception},
                        { "Quantity", wafersOnSourceContainer },
                        { "Product", productName },
                        { "LoadPort", loadPortNames[sourceLoadPortNumber] }

                    }
                }.ExecuteActionSync();

                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                if (ex.Message == "TrackInFailed" || TrackInMustFail)
                    return;
                else
                    Assert.Fail(ex.Message);
            }
            Log(String.Format("{0}: [E] Track In of Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            //Log(String.Format("{0}: [S] Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
            //bool postTrackInResult = PostTrackInActions(MESScenario);
            //Log(String.Format("{0}: [E] Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));

            ////intentional step out of the test by returning false on post track in result
            //if (!postTrackInResult)
            //{
            //    Log(String.Format("{0}: [S] Test Concluded by Returning false on Post Track In Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name));
            //    return;
            //}

            #endregion
            var materialName = "CarrierAtLoadPort1";

            // Fetching Material Data also validates the material is in state 'Setup'
            MaterialData materialData = GetMaterialDataFromPersistence(materialName);

            Log(String.Format("{0}: [S] Sending Process Start Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            CustomProcessStartEvent(MESScenario, containerName: materialName);
            Log(String.Format("{0}: [E] Sending Process Start Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            System.Threading.Thread.Sleep(5000);

            //Log(String.Format("{0}: [S] Validate Material is In Progress on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            //Log(String.Format("{0}: [E] Validate Material is In Progress on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            Log(String.Format("{0}: [S] Validate Material Persistence is In Process on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            ValidatePersistenceState(materialName, MaterialStateEnum.InProcess);
            Log(String.Format("{0}: [E] Validate Material Persistence is In Process on MES Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

//            CustomProcessWafersInMovementList(materialData, 25, "AdHocTransferWafers-WaferReception");

            #region Validate MovementList
            var expectedNumberOfMovements = 25;
            JArray movementList = JArray.Parse(materialData.SorterJobInformation.MovementList);

            // Number of moves in this custom sorter job definition
            int numberOfMoves = movementList.Count;

            Assert.AreEqual(expectedNumberOfMovements, numberOfMoves, $"We where expecting {expectedNumberOfMovements} movements, but there were {numberOfMoves} movements.");

            #endregion Extract movement list on the IoT persistence and validation against the expected number of moves

            #region Process Wafers on the movement list

            foreach (JToken movement in movementList)
            {
                DateTime now;

                // Source
                string movementWaferName = movement.Value<string>("MaterialName");
                string sourceContainer = movement.Value<string>("SourceContainer");
                int sourcePosition = movement.Value<int>("SourcePosition");
                // Destination
                string destinationContainer = movement.Value<string>("DestinationContainer");
                int destinationPosition = movement.Value<int>("DestinationPosition");
                              
                #region Retrieves the load port number based on the container name currently on IoT persistence

                int sourceLoadPort = GetLoadPortFromContainerNameOnPersistence(sourceContainer);
                int destinationLoadPort = GetLoadPortFromContainerNameOnPersistence(destinationContainer);

                #endregion Retrieves the load port number based on the container name currently on IoT persistence


                SubstHistoryList substHistoryListDestination = new SubstHistoryList(base.Equipment)
                {
                    SubstHistoryInternalList = new SubstHistoryInternalList
                    {
                        SubstHistoryEntryList = new List<SubstHistoryEntry>
                        {
                            new SubstHistoryEntry
                            {
                                Location = String.Format("{0}.{1:D3}", sourceContainer, sourcePosition),
                                TimeIn = "11",
                                TimeOut = "12"
                            },
                            new SubstHistoryEntry
                            {
                                Location = String.Format("{0}.{1:D3}", destinationContainer, destinationPosition),
                                TimeIn = "11",
                                TimeOut = "12"
                            },
                        }
                    }
                };

                #region Wafer Start

                SubstHistoryList substHistoryListSource = new SubstHistoryList(base.Equipment)
                {
                    SubstHistoryInternalList = new SubstHistoryInternalList
                    {
                        SubstHistoryEntryList = new List<SubstHistoryEntry>
                        {
                            new SubstHistoryEntry
                            {
                                Location = String.Format("{0}.{1:D3}", sourceContainer, sourcePosition),
                                TimeIn = "11",
                                TimeOut = "12"
                            }
                        }
                    }
                };

                base.Equipment.Variables["SubstID"] = movementWaferName;
                base.Equipment.Variables["SubstLotID"] = materialData.MaterialName;
                base.Equipment.Variables["SubstSubstLocID"] = String.Format("{0}.{1:D2}", sourceContainer, sourcePosition);
                base.Equipment.Variables["SubstState"] = 2;
                base.Equipment.Variables["SubstProcState"] = 2;
                base.Equipment.Variables["AcquiredID"] = "";
                base.Equipment.Variables["Clock"] = DateTime.UtcNow.ToString("hh:mm:ss.fff");
                base.Equipment.Variables["SubstHistory"] = substHistoryListDestination;

                //base.Equipment.Variables["SubstID"] = String.Format("{0:D3}_{1:D3}", sourceLoadPort, waferPositionOnMES);

                //// Trigger event
                base.Equipment.SendMessage("SOSM11_NEEDSPROCESSING_INPROCESS", null);

                Thread.Sleep(2000);
                #endregion Wafer Start

                #region Wafer Read

                base.Equipment.Variables["SubstID"] = movementWaferName;
                base.Equipment.Variables["SubstLotID"] = materialData.MaterialName;
                base.Equipment.Variables["SubstSubstLocID"] = "";
                base.Equipment.Variables["SubstSource"] = String.Format("{0}.{1:D2}", sourceContainer, sourcePosition);
                base.Equipment.Variables["AcquiredID"] = materialsForContainer1[sourcePosition - 1].Name;
                base.Equipment.Variables["Clock"] = DateTime.UtcNow.ToString("hh:mm:ss.fff");
                base.Equipment.Variables["SubstHistory"] = substHistoryListDestination;

                if (successWaferIdRead)
                {
                    //// Trigger event
                    base.Equipment.SendMessage("SOSM19_NOTCONFIRMED_WAITINGFORHOST", null);

                    
                    TestUtilities.WaitFor(ValidationTimeout, "Failed to recieve ProceedWithSubstrate", () =>
                    {
                        return proceedWithSubstrateCommandRecieved;
                    });
                    proceedWithSubstrateCommandRecieved = false;
                }
                else
                {
                    //// Trigger event
                    base.Equipment.SendMessage("SOSM18_NOTCONFIRMED_WAITINGFORHOST", null);

                    TestUtilities.WaitFor(ValidationTimeout, "Failed to recieve CancelSubstrate", () =>
                    {
                        return cancelSubstrateCommandRecieved;
                    });
                    cancelSubstrateCommandRecieved = false;
                    continue;
                }

                #endregion Wafer Read

                #region Wafer complete              

                base.Equipment.Variables["SubstID"] = movementWaferName;
                base.Equipment.Variables["SubstLotID"] = materialData.MaterialName;
                base.Equipment.Variables["SubstSubstLocID"] = String.Format("{0}.{1:D2}", destinationContainer, destinationPosition);
                base.Equipment.Variables["SubstState"] = 2;
                base.Equipment.Variables["SubstProcState"] = 2;
                base.Equipment.Variables["AcquiredID"] = materialsForContainer1[sourcePosition - 1].Name; 
                base.Equipment.Variables["Clock"] = DateTime.UtcNow.ToString("hh:mm:ss.fff");
                base.Equipment.Variables["SubstHistory"] = substHistoryListDestination;

                //// Trigger event
                base.Equipment.SendMessage("SOSM12_INPROCESS_PROCESSINGCOMPLETE", null);
                Thread.Sleep(2000);
                #endregion Wafer complete
            }
            #endregion

            Log(String.Format("{0}: [S] Sending Process Complete Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            CustomProcessCompleteEvent(MESScenario, containerName: materialName);
            Log(String.Format("{0}: [E] Sending Process Complete Event Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            //Track Out occurs automatically, validate either Processed or Queued
            //TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material {MESScenario.Entity.Name} System State is not Processed or Queued, automatic Track Out Failed"), () =>
            //{
            //    MESScenario.Entity.Load();
            //    return MESScenario.Entity.SystemState.ToString().Equals(MaterialSystemState.Processed.ToString()) || MESScenario.Entity.SystemState.ToString().Equals(MaterialSystemState.Queued.ToString());
            //});

            //Log(String.Format("{0}: [S] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));
            //ValidatePersistenceDoesNotExists(materialName);
            //Log(String.Format("{0}: [E] Validate Persistence Does Not Exist Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, MESScenario.Entity.Name));

            //CustomValidateContainerNumberOfWafers(containerScenarioForLoadPort1.Entity, 0);

            CustomValidateContainerNumberOfWafers(containerScenarioForLoadPort2.Entity, 10);
            CustomValidateContainerNumberOfWafers(containerScenarioForLoadPort3.Entity, 10);
            CustomValidateContainerNumberOfWafers(containerScenarioForLoadPort4.Entity, 5);

            ContainerCarrierOut(null, 1);
            ContainerCarrierOut(containerScenarioForLoadPort2, 2);
            ContainerCarrierOut(containerScenarioForLoadPort3, 3);
            ContainerCarrierOut(containerScenarioForLoadPort4, 4);
        }

        /// <summary> 
        /// Scenario: Control State to Host Offline
        /// </summary>
        [TestMethod]
        public void MechatronicMWS200_ControlStateUpdateTest()
        {

            // Trigger event
            base.Equipment.SendMessage("ControlStateOFFLINE", null);

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


        #region Events
        public bool SorterCarrierIn(bool IsWaferReception = false)
        {
            foreach (var lp in loadPortsUsed)
            {
                base.Equipment.Variables["PortID"] = lp;

                base.Equipment.Variables["PortTransferState"] = 0;
                base.Equipment.Variables["AccessMode"] = 0;
                base.Equipment.Variables["LoadPortReservationState"] = 0;
                base.Equipment.Variables["PortAssociationState"] = 0;
                base.Equipment.Variables["PortStateInfo"] = null;
                base.Equipment.SendMessage(String.Format($"LPTSM6_READYTOLOAD_TRANSFERBLOCKED"), null);

                Thread.Sleep(300);

                var containerScenario = containerScenariosUsed[lp];

                if (containerScenario is not null)
                {
                    RFIDReader.targetIdRFID.Add(lp.ToString(), containerScenario?.Entity.Name);
                }
                else {
                    RFIDReader.targetIdRFID.Add(lp.ToString(), "");
                }

                base.Equipment.Variables["PlacedCarrierPattern1"] = 1;
                base.Equipment.Variables["PlacedCarrierPattern2"] = 2;
                base.Equipment.Variables["PlacedCarrierPattern3"] = 3;
                base.Equipment.Variables["PlacedCarrierPattern4"] = 4;

                //Trigger event
                base.Equipment.SendMessage(String.Format($"MATERIAL_PLACED"), null);


                TestUtilities.WaitFor(ValidationTimeout, "Failed to recieve S18F9", () =>
                {
                    return RFIDReader.RecievedS18F9(lp.ToString());
                });

                MESScenario.Step.LoadAttributes();
                
                if (RFIDReader.TargetIdStatusS18F9(lp.ToString()) != "NO") {
                    Assert.Fail("Unexpected S18F9 status");
                }

                RFIDReader.ClearFlags(lp.ToString());

                
                TestUtilities.WaitFor(ValidationTimeout, String.Format($"ProceedWithCarrier request was never received"), () =>
                {
                    return proceedWithCarrierCommandRecieved;
                });
                proceedWithCarrierCommandRecieved = false;

                //DockContainer(containerScenario, lp, MESScenario);

                Thread.Sleep(1000);

                containerScenario?.Entity.LoadRelation("MaterialContainer");

                int[] slotMap;

                if (IsWaferReception && lp == sourceLoadPortNumber)
                {
                    slotMap = Enumerable.Repeat(3, 25).ToArray();
                }
                else {
                    slotMap = new int[containerScenario?.Entity.TotalPositions ?? 0];
                }
                // scenario.containerScenario?.Entity
                if (containerScenario?.Entity.ContainerMaterials != null)
                {
                    for (int i = 0; i < containerScenario?.Entity.TotalPositions.Value; i++)
                    {
                        slotMap[i] = containerScenario.Entity.ContainerMaterials.Exists(p => p.Position != null && p.Position == i + 1) ? 3 : 1;
                    }
                }


                SlotMapVariable slotMapDV = new SlotMapVariable(base.Equipment) { Presence = slotMap };

                base.Equipment.Variables["PortID"] = lp;
                base.Equipment.Variables["CarrierID"] = containerScenario?.Entity.Name;
                base.Equipment.Variables["ContentMap"] = slotMapDV;
                base.Equipment.Variables["SlotMap"] = slotMapDV;


                // Trigger event
                base.Equipment.SendMessage(String.Format($"COSM14_SLOTMAPNOTREAD_SLOTMAPWAITINGFORHOST"), null);

                Thread.Sleep(200);

                base.Equipment.Variables["CarrierID"] = containerScenario?.Entity.Name;
                base.Equipment.Variables["PortID"] = lp;
                base.Equipment.Variables["ContentMap"] = slotMapDV;
                base.Equipment.Variables["SlotMap"] = slotMapDV;

                TestUtilities.WaitFor(ValidationTimeout, String.Format($"ProceedWithCarrier request was never received"), () =>
                {
                    return proceedWithCarrierCommandRecieved;
                });
                proceedWithCarrierCommandRecieved = false;

                base.Equipment.SendMessage("COSM15_SLOTMAPWAITINGFORHOST_SLOTMAPVERIFICATIONOK", null);
                                
            }

            if (!IsWaferReception)
            {
                TestUtilities.WaitFor(ValidationTimeout, String.Format($"Control or Process Job creation requests were never received"), () =>
                {
                    return (createControlJobReceived && createProcessJobReceived);
                });

                createControlJobReceived = createProcessJobReceived = false;
            }
            return true;
        }

        public override bool CarrierIn(CustomMaterialScenario scenario, int loadPortToSet)
        {

            foreach (var lp in loadPortsUsed)
            {
                base.Equipment.Variables["PortID"] = lp;

                base.Equipment.Variables["PortTransferState"] = 0;
                base.Equipment.Variables["AccessMode"] = 0;
                base.Equipment.Variables["LoadPortReservationState"] = 0;
                base.Equipment.Variables["PortAssociationState"] = 0;
                base.Equipment.Variables["PortStateInfo"] = null;
                base.Equipment.SendMessage(String.Format($"LPTSM6_READYTOLOAD_TRANSFERBLOCKED"), null);

                Thread.Sleep(200);

                RFIDReader.targetIdRFID.Add(lp.ToString(), scenario.ContainerScenario.Entity.Name);

                base.Equipment.Variables["PortID"] = lp;
                base.Equipment.Variables["PlacedCarrierPattern1"] = 1;
                base.Equipment.Variables["PlacedCarrierPattern2"] = 2;
                base.Equipment.Variables["PlacedCarrierPattern3"] = 3;
                base.Equipment.Variables["PlacedCarrierPattern4"] = 4;

                // Trigger event
                base.Equipment.SendMessage(String.Format($"MATERIAL_PLACED"), null);

                Thread.Sleep(300);

                //DockContainer(containerScenariosUsed[lp], lp, MESScenario);

            }


            return true;

        }

        public override void CarrierInValidation(CustomMaterialScenario MESScenario, int loadPortToSet)
        {
            //clamped
            base.CarrierInValidation(MESScenario, loadPortToSet);

            var slotMap = new int[MESScenario.ContainerScenario.Entity.TotalPositions.Value];
            // scenario.ContainerScenario.Entity
            if (MESScenario.ContainerScenario.Entity.ContainerMaterials != null)
            {
                for (int i = 0; i < MESScenario.ContainerScenario.Entity.TotalPositions.Value; i++)
                {
                    slotMap[i] = MESScenario.ContainerScenario.Entity.ContainerMaterials.Exists(p => p.Position != null && p.Position == i + 1) ? 3 : 1;
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

            TestUtilities.WaitFor(ValidationTimeout, String.Format($"ProceedWithCarrier request was never received"), () =>
            {
                return proceedWithCarrierCommandRecieved;
            });
            proceedWithCarrierCommandRecieved = false;

            base.Equipment.SendMessage("COSM15_SLOTMAPWAITINGFORHOST_SLOTMAPVERIFICATIONOK", null);

            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Control or Process Job creation requests were never received"), () =>
            {
                return (createControlJobReceived && createProcessJobReceived);
            });

            createControlJobReceived = createProcessJobReceived = false;
        }

        public bool ContainerCarrierOut(ContainerScenario containerScenario, int loadPortNumber)
        {

            //// wait for load 
            //TestUtilities.WaitFor(ValidationTimeout, String.Format($"Unload Container Command never received"), () =>
            //{
            //    return unloadCommandReceived;
            //});

            //CarrierSMTrans21 ready to unload

            var container = containerScenario?.Entity;

            int[] slotMap;


            if(container is null)
            {
                slotMap = Enumerable.Repeat(1, 25).ToArray();
            }
            else
            {
                slotMap = new int[containerScenario?.Entity.TotalPositions ?? 0];
            }
            // scenario.containerScenario?.Entity
            if (containerScenario?.Entity.ContainerMaterials != null)
            {
                for (int i = 0; i < containerScenario?.Entity.TotalPositions.Value; i++)
                {
                    slotMap[i] = containerScenario.Entity.ContainerMaterials.Exists(p => p.Position != null && p.Position == i + 1) ? 3 : 1;
                }
            }

            SlotMapVariable slotMapDV = new SlotMapVariable(base.Equipment) { Presence = slotMap };


            base.Equipment.Variables["PortID"] = loadPortNumber;
            base.Equipment.Variables["PortTransferState"] = 3;
            base.Equipment.Variables["AccessMode"] = 0;
            base.Equipment.Variables["LoadPortReservationState"] = 0;
            base.Equipment.Variables["PortAssociationState"] = 1;
            base.Equipment.Variables["PortStateInfo"] = slotMapDV;

            // Trigger event
            base.Equipment.SendMessage(String.Format($"LPTSM9_TRANSFERBLOCKED_READYTOUNLOAD"), null);

            Thread.Sleep(2000);

            base.Equipment.Variables["PortID"] = loadPortNumber;
            base.Equipment.Variables["PlacedCarrierPattern1"] = 1;
            base.Equipment.Variables["PlacedCarrierPattern2"] = 2;
            base.Equipment.Variables["PlacedCarrierPattern3"] = 3;
            base.Equipment.Variables["PlacedCarrierPattern4"] = 4;

            // Trigger event
            base.Equipment.SendMessage(String.Format($"MATERIAL_REMOVED"), null);

            Thread.Sleep(200);
            return true;
        }


        public override bool CarrierOut(CustomMaterialScenario scenario)
        {

            //// wait for load 
            //TestUtilities.WaitFor(ValidationTimeout, String.Format($"Unload Container Command never received"), () =>
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
                    slotMap[i] = MESScenario.ContainerScenario.Entity.ContainerMaterials.Exists(p => p.Position != null && p.Position == i + 1) ? 3 : 1;
                }
            }
            SlotMapVariable slotMapDV = new SlotMapVariable(base.Equipment) { Presence = slotMap };


            base.Equipment.Variables["PortID"] = loadPortNumber;
            base.Equipment.Variables["PortTransferState"] = 3;
            base.Equipment.Variables["AccessMode"] = 0;
            base.Equipment.Variables["LoadPortReservationState"] = 0;
            base.Equipment.Variables["PortAssociationState"] = 1;
            base.Equipment.Variables["PortStateInfo"] = slotMapDV;

            // Trigger event
            base.Equipment.SendMessage(String.Format($"LPTSM9_TRANSFERBLOCKED_READYTOUNLOAD"), null);

            Thread.Sleep(2000);

            base.Equipment.Variables["PortID"] = loadPortNumber;
            base.Equipment.Variables["PlacedCarrierPattern1"] = 1;
            base.Equipment.Variables["PlacedCarrierPattern2"] = 2;
            base.Equipment.Variables["PlacedCarrierPattern3"] = 3;
            base.Equipment.Variables["PlacedCarrierPattern4"] = 4;

            // Trigger event
            base.Equipment.SendMessage(String.Format($"MATERIAL_REMOVED"), null);

            Thread.Sleep(200);


            return true;
        }

        /// <summary>
        /// Process start event
        /// 'TransferInitiated' with CommandId IoT persistence check
        /// </summary>
        /// <param name="scenario">The material scenario</param>
        /// <returns>true</returns>
        public bool CustomProcessStartEvent(CustomMaterialScenario scenario, string containerName = null)
        {
            base.Equipment.Variables["CtrlJobID"] = $"CtrlJob_{containerName ?? scenario.Entity.Name}";
            base.Equipment.Variables["CtrlJobState"] = 3;

            //// Trigger event
            base.Equipment.SendMessage(String.Format($"CJSM5_SELECTED_EXECUTING"), null); ;

            Thread.Sleep(2000);

            SubMaterialTrackin = true;

            return true;
        }

        public bool CustomProcessCompleteEvent(CustomMaterialScenario scenario, string containerName = null)
        {
            base.Equipment.Variables["CtrlJobID"] = $"CtrlJob_{containerName ?? scenario.Entity.Name}";
            base.Equipment.Variables["CtrlJobState"] = 3;

            //// Trigger event
            base.Equipment.SendMessage(String.Format($"CJSM10_EXECUTING_COMPLETED"), null); ;

            return true;
        }

        public override bool ProcessStartEvent(CustomMaterialScenario scenario)
        {
            base.Equipment.Variables["CtrlJobID"] = $"CtrlJob_{scenario.Entity.Name}";
            base.Equipment.Variables["CtrlJobState"] = 1;

            //// Trigger event
            base.Equipment.SendMessage(String.Format($"CJSM5_SELECTED_EXECUTING"), null);

            Thread.Sleep(2000);

            SubMaterialTrackin = true;

            return true;
        }

        public override bool ProcessCompleteEvent(CustomMaterialScenario scenario)
        {
            SubMaterialTrackin = false;
            base.Equipment.Variables["CtrlJobID"] = $"CtrlJob_{scenario.Entity.Name}";
            base.Equipment.Variables["CtrlJobState"] = 3;

            //// Trigger event
            base.Equipment.SendMessage(String.Format($"CJSM10_EXECUTING_COMPLETED"), null);
            


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

        public override void TrackInEvaluator(string expectedMaterialName)
        {
            if (TrackInMustFail)
            {
                ValidatePersistenceDoesNotExists(expectedMaterialName);
            }
            else
            {
                ValidatePersistenceExists(expectedMaterialName);
            }
        }

        public override bool PostTrackInActions(CustomMaterialScenario scenario)
        {
            if (!isOnlineRemote)
            {
                Assert.Fail("Track In must fail on Online Local");
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

        public override void WaferStartValidation(Material material)
        {
            Log(String.Format("{0}: [S] Send Wafer Start Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));
            WaferStart(material);
            Log(String.Format("{0}: [E] Send Wafer Start Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));

        }

        public override bool WaferStart(Material wafer)
        {
            wafer.Load();
            wafer.LoadRelations();
            wafer.ParentMaterial.Load();

            SubstHistoryList substHistoryListSource = new SubstHistoryList(base.Equipment)
            {
                SubstHistoryInternalList = new SubstHistoryInternalList
                {
                    SubstHistoryEntryList = new List<SubstHistoryEntry>
                        {
                            new SubstHistoryEntry
                            {
                                Location = String.Format("{0}.{1:D3}", MESScenario.ContainerScenario.Entity.Name, wafer.MaterialContainer.First().Position),
                                TimeIn = "11",
                                TimeOut = "12"
                            }
                        }
                }
            };

            base.Equipment.Variables["SubstID"] = wafer.Name;
            base.Equipment.Variables["SubstLotID"] = wafer.ParentMaterial.Name;
            base.Equipment.Variables["SubstSubstLocID"] = String.Format("{0}.{1:D2}", MESScenario.ContainerScenario.Entity.Name, wafer.MaterialContainer.First().Position);
            base.Equipment.Variables["SubstState"] = 0;
            base.Equipment.Variables["SubstProcState"] = 0;
            base.Equipment.Variables["AcquiredID"] = "";
            base.Equipment.Variables["Clock"] =  DateTime.UtcNow.ToString("hh:mm:ss.fff");
            base.Equipment.Variables["SubstHistory"] = substHistoryListSource;

            //base.Equipment.Variables["SubstID"] = String.Format("{0:D3}_{1:D3}", sourceLoadPort, waferPositionOnMES);

            //// Trigger event
            base.Equipment.SendMessage("SOSM11_NEEDSPROCESSING_INPROCESS", null);
            //// Trigger event
            //base.Equipment.SendMessage($"ProcessChamber{chamberToProcess}_ProcessStarted", null);

            Thread.Sleep(2000);

            return true;
        }

        public override void WaferCompleteValidation(Material material)
        {
            Log(String.Format("{0}: [S] Send Wafer Complete Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));
            WaferComplete(material);
            Log(String.Format("{0}: [E] Send Wafer Complete Material {2} Resource {1}", DateTime.UtcNow.ToString("hh:mm:ss.fff"), MESScenario.Resource.Name, material.Name));

        }

        public override bool WaferComplete(Material wafer)
        {
            wafer.Load();
            wafer.LoadRelations();
            wafer.ParentMaterial.Load();

            base.Equipment.Variables["AcquiredId"] = "";
            base.Equipment.Variables["LotId"] = "";
            base.Equipment.Variables["SubId"] = String.Format("CarrierAtPort{0}.{1:D2}", loadPortNumber, wafer.MaterialContainer.First().Position); ;


            SubstHistoryList substHistoryListDestination = new SubstHistoryList(base.Equipment)
            {
                SubstHistoryInternalList = new SubstHistoryInternalList
                {
                    SubstHistoryEntryList = new List<SubstHistoryEntry>
                        {
                            new SubstHistoryEntry
                            {
                                Location = String.Format("{0}.{1:D3}", MESScenario.ContainerScenario.Entity.Name, wafer.MaterialContainer.First().Position),
                                TimeIn = "11",
                                TimeOut = "12"
                            },
                            new SubstHistoryEntry
                            {
                                Location = String.Format("{0}.{1:D3}", MESScenario.ContainerScenario.Entity.Name, wafer.MaterialContainer.First().Position),
                                TimeIn = "11",
                                TimeOut = "12"
                            },
                        }
                }
            };

            base.Equipment.Variables["SubstID"] = String.Format("{0}.{1:D2}", MESScenario.ContainerScenario.Entity.Name, wafer.MaterialContainer.First().Position);
            base.Equipment.Variables["SubstLotID"] = wafer.ParentMaterial.Name;
            base.Equipment.Variables["SubstSubstLocID"] = String.Format("{0}.{1:D2}", MESScenario.ContainerScenario.Entity.Name, wafer.MaterialContainer.First().Position);
            base.Equipment.Variables["SubstState"] = 2;
            base.Equipment.Variables["SubstProcState"] = 2;
            base.Equipment.Variables["AcquiredID"] = "";
            base.Equipment.Variables["Clock"] =  DateTime.UtcNow.ToString("hh:mm:ss.fff");
            base.Equipment.Variables["SubstHistory"] = substHistoryListDestination;

            //// Trigger event
            base.Equipment.SendMessage("SOSM12_INPROCESS_PROCESSINGCOMPLETE", null);
            Thread.Sleep(2000);

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
                submaterial.ParentMaterial?.Load();
                submaterial.ParentMaterial?.LoadChildren();
                if (submaterial.ParentMaterial?.SubMaterials
                    .Where(s => s.SystemState == MaterialSystemState.Queued).Count() == 0
                    && submaterial.ParentMaterial?.SubMaterials
                    .Where(s => s.SystemState == MaterialSystemState.InProcess).Count() == 1)
                {
                    return true;
                }
            }

            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Material {submaterial.Name} State is not {subMaterialState}"), () =>
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

        public virtual bool OnS3F17(SecsMessage request, SecsMessage reply)
        {
            reply.Item.Clear();
            var ack = new SecsItem { U1 = new byte[] { 0x00 } };

            var errorList = new SecsItem();
            errorList.SetTypeToList();

            proceedWithCarrierCommandRecieved = true;

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

            return (true);
        }

        public virtual bool OnS14F19(SecsMessage request, SecsMessage reply)
        {
            reply.Item.Clear();
            var ack = new SecsItem { U1 = new byte[] { 0x00 } };

            var errorList = new SecsItem();
            errorList.SetTypeToList();

            var substrateVerification = request.Item[3].GetValue();

            switch (substrateVerification) {
                case "ProceedWithSubstrate":
                    proceedWithSubstrateCommandRecieved = true;
                    break;
                case "CancelSubstrate":
                    cancelSubstrateCommandRecieved = true;
                    break;
                default:
                    Assert.Fail("Invalid S14F19");
                    break;
            }

            reply.Item.Add(ack);

            reply.Item.Add(errorList);

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

        public override bool PostSetupActions(CustomMaterialScenario MESScenario)
        {
            return true;
        }



        private ContainerScenario CreateEmptyContainerScenario(int loadPort, string facilityName, string containerType, int totalPositions = 0)
        {
            Facility facility = new Facility();

            if (string.IsNullOrWhiteSpace(facilityName))
            {
                facility.Name = amsOSRAMConstants.TestFacility;
                facility.Load();
            }
            else
            {
                facility.Name = facilityName;
                facility.Load();
            }

            if (totalPositions == 0)
            {
                totalPositions = amsOSRAMConstants.ContainerTotalPosition;
            }

            // Create Container to put the Wafers
            ContainerScenario containerScenario = new ContainerScenario();
            containerScenario.Entity.IsAutoGeneratePositionEnabled = false;
            containerScenario.Entity.Name = $"Container_{loadPort}_{DateTime.Now:yyyyMMdd_HHmmssfff}";
            containerScenario.Entity.Type = containerType; //CreeConstants.ContainerTypeBEOL;
            containerScenario.Entity.PositionUnitType = ContainerPositionUnitType.Material;
            containerScenario.Entity.Facility = facility;
            containerScenario.Entity.CapacityUnits = amsOSRAMConstants.UnitWafers;
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
            string sourceContaineType = amsOSRAMConstants.ContainerSMIFPod,
            string targetContainerType = amsOSRAMConstants.ContainerSMIFPod,
            string futureActionType = "",
            bool fullTransferWafers = false,
            bool sortDescending = false)
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

            if (logisticalProcess == amsOSRAMConstants.CustomSorterLogisticalProcessTransferWafers)
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
                    for (int i = 1; i <= 13; i++)
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
                else if (futureActionType.Equals("WaferReception", StringComparison.InvariantCulture)) {
                    Container theOneThatWillBeTranferred = sourceContainers.FirstOrDefault();
                    theOneThatWillBeTranferred.LoadRelation("MaterialContainer");

                    Container destinationContainer = destinationContainers.FirstOrDefault();
                    destinationContainer.LoadRelation("MaterialContainer");

                    Queue<string> freePositions = new Queue<string>();

                    foreach (var targetContainer in destinationContainers)
                    {
                        for (int i = 1; i <= targetContainer.TotalPositions; i++)
                        {
                            if (destinationContainer.ContainerMaterials == null || !destinationContainer.ContainerMaterials.Any(m => m.Position == i))
                            {
                                freePositions.Enqueue(destinationContainer + "." + i);
                            }
                        }
                    }

                    foreach (var movement in theOneThatWillBeTranferred.ContainerMaterials)
                    {
                        var containerPos = freePositions.Dequeue().Split('.');

                        JObject jObject = new JObject
                        {
                            ["MaterialName"] = "",
                            ["SourceContainer"] = theOneThatWillBeTranferred.Name,
                            ["SourcePosition"] = movement.Position,
                            ["DestinationContainer"] = containerPos[0],
                            ["DestinationPosition"] = containerPos[1]
                        };

                        temporaryMovementList.Add(jObject);

                    }
                }

                else // Simple transfer scenario
                {
                    if (!fullTransferWafers && !sortDescending)
                    {
                        Container theOneThatWillBeTranferred = sourceContainers.FirstOrDefault();
                        theOneThatWillBeTranferred.LoadRelation("MaterialContainer");

                        Container destinationContainer = destinationContainers.FirstOrDefault();
                        destinationContainer.LoadRelation("MaterialContainer");

                        Queue<int> freePositions = new Queue<int>();
                        for (int i = 1; i <= 13; i++)
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
                    if (sortDescending)
                    {
                        Container theOneThatWillBeTranferred = sourceContainers.FirstOrDefault();
                        theOneThatWillBeTranferred.LoadRelation("MaterialContainer");

                        Container destinationContainer = destinationContainers.FirstOrDefault();
                        destinationContainer.LoadRelation("MaterialContainer");

                        Stack<int> freePositions = new Stack<int>();
                        for (int i = 1; i <= 13; i++)
                        {
                            if (destinationContainer.ContainerMaterials == null || !destinationContainer.ContainerMaterials.Any(m => m.Position == i))
                            {
                                freePositions.Push(i);
                            }
                        }

                        foreach (var movement in theOneThatWillBeTranferred.ContainerMaterials)
                        {
                            Material material = movement.SourceEntity;
                            material.Load();

                            JObject jObject = new JObject
                            {
                                ["MaterialName"] = material.Name,
                                ["SourceContainer"] = theOneThatWillBeTranferred.Name,
                                ["SourcePosition"] = movement.Position,
                                ["DestinationContainer"] = destinationContainer.Name,
                                ["DestinationPosition"] = freePositions.Pop()
                            };

                            temporaryMovementList.Add(jObject);
                        }
                    }
                }
            }
            else if (logisticalProcess == amsOSRAMConstants.CustomSorterLogisticalProcessCompose)
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


        /// <summary>
		/// Process wafers using the material data movement list
		/// </summary>
		/// <param name="materialData">The material data</param>
		/// <param name="expectedNumberOfMovements">The expected number of movements</param>
		/// <param name="expectedFutureActionType">The expected future action type</param>
		public void CustomProcessWafersInMovementList(MaterialData materialData, int expectedNumberOfMovements, string expectedFutureActionType)
        {
            #region Custom Sorter Job Validation from data on the equipment vs MES

            CustomSorterJobDefinition customSorterJobDefinitionFromEquipment = new CustomSorterJobDefinition() { Name = materialData.SorterJobInformation.Name };
            customSorterJobDefinitionFromEquipment.Load();
            
            // Parse Custom Sorter Job Movement List Json Object
            JObject movementListObject = null;

            if (materialData.SorterJobInformation.Name != null && JObject.Parse(customSorterJobDefinitionFromEquipment.MovementList) is JObject parsedMovementListObj)
            {
                movementListObject = parsedMovementListObj;
            }
            else
            {
                Assert.Fail($"Not possible to parse the movement list JSON Object for custom sorter job definition ({customSorterJobDefinition.Name}).");
            }

            // Get the future action type defined on the custom sorter job definition
            string futureActionType = movementListObject["FutureActionType"].Value<string>() ?? string.Empty;

            Assert.IsTrue(futureActionType.Equals(expectedFutureActionType), $"Future action type must be '{expectedFutureActionType}'.");
            
            #endregion Custom Sorter Job Validation from data on the equipment vs MES

            #region Extract movement list on the IoT persistence and validation against the expected number of moves

            // extract sorter job movement list from the informartion on the material data that was on the equipment
            JArray movementList = JArray.Parse(materialData.SorterJobInformation.MovementList);

            // Number of moves in this custom sorter job definition
            int numberOfMoves = movementList.Count;

            Assert.AreEqual(expectedNumberOfMovements, numberOfMoves, $"We where expecting {expectedNumberOfMovements} movements, but there were {numberOfMoves} movements.");

            #endregion Extract movement list on the IoT persistence and validation against the expected number of moves

            #region Process Wafers on the movement list

            foreach (JToken movement in movementList)
            {
                DateTime now;

                // Material
                string materialName = movement.Value<string>("MaterialName");
                // Source
                string sourceContainer = movement.Value<string>("SourceContainer");
                int sourcePosition = movement.Value<int>("SourcePosition");
                // Destination
                string destinationContainer = movement.Value<string>("DestinationContainer");
                int destinationPosition = movement.Value<int>("DestinationPosition");

                Material wafer = new Material() { Name = materialName };
                wafer.Load();
                wafer.LoadRelations();

                int waferPositionOnMES = wafer.MaterialContainer.First().Position ?? 0;
                Assert.AreEqual(sourcePosition, waferPositionOnMES, $"Wafer position on MES ({waferPositionOnMES}) must be the same as the one on material data movement list ({sourcePosition}).");

                #region Retrieves the load port number based on the container name currently on IoT persistence

                int sourceLoadPort = GetLoadPortFromContainerNameOnPersistence(sourceContainer);
                int destinationLoadPort = GetLoadPortFromContainerNameOnPersistence(destinationContainer);

                #endregion Retrieves the load port number based on the container name currently on IoT persistence


                SubstHistoryList substHistoryListDestination = new SubstHistoryList(base.Equipment)
                {
                    SubstHistoryInternalList = new SubstHistoryInternalList
                    {
                        SubstHistoryEntryList = new List<SubstHistoryEntry>
                        {
                            new SubstHistoryEntry
                            {
                                Location = String.Format("{0}.{1:D3}", sourceContainer, waferPositionOnMES),
                                TimeIn = "11",
                                TimeOut = "12"
                            },
                            new SubstHistoryEntry
                            {
                                Location = String.Format("{0}.{1:D3}", destinationContainer, destinationPosition),
                                TimeIn = "11",
                                TimeOut = "12"
                            },
                        }
                    }
                };

                #region Wafer Start

                SubstHistoryList substHistoryListSource = new SubstHistoryList(base.Equipment)
                {
                    SubstHistoryInternalList = new SubstHistoryInternalList
                    {
                        SubstHistoryEntryList = new List<SubstHistoryEntry>
                        {
                            new SubstHistoryEntry
                            {
                                Location = String.Format("{0}.{1:D3}", sourceContainer, waferPositionOnMES),
                                TimeIn = "11",
                                TimeOut = "12"
                            }
                        }
                    }
                };

                base.Equipment.Variables["SubstID"] = wafer.Name;
                base.Equipment.Variables["SubstLotID"] = materialData.MaterialName;
                base.Equipment.Variables["SubstSubstLocID"] = String.Format("{0}.{1:D2}", sourceContainer, sourcePosition); 
                base.Equipment.Variables["SubstState"] = 2;
                base.Equipment.Variables["SubstProcState"] = 2;
                base.Equipment.Variables["AcquiredID"] = "";
                base.Equipment.Variables["Clock"] =  DateTime.UtcNow.ToString("hh:mm:ss.fff");
                base.Equipment.Variables["SubstHistory"] = substHistoryListDestination;

                //base.Equipment.Variables["SubstID"] = String.Format("{0:D3}_{1:D3}", sourceLoadPort, waferPositionOnMES);

                //// Trigger event
                base.Equipment.SendMessage("SOSM11_NEEDSPROCESSING_INPROCESS", null);

                Thread.Sleep(2000);
                #endregion Wafer Start

                //now = DateTime.Now;
                //while (DateTime.Now.Subtract(now).Seconds < 2)
                //{
                //    // wait for 2 seconds
                //    Thread.Sleep(100);
                //}

                #region Wafer Read

                base.Equipment.Variables["SubstID"] = wafer.Name;
                base.Equipment.Variables["SubstLotID"] = materialData.MaterialName;
                base.Equipment.Variables["SubstSubstLocID"] = "";
                base.Equipment.Variables["SubstSource"] = String.Format("{0}.{1:D2}", sourceContainer, sourcePosition);
                base.Equipment.Variables["AcquiredID"] = wafer.Name;
                base.Equipment.Variables["Clock"] =  DateTime.UtcNow.ToString("hh:mm:ss.fff");
                base.Equipment.Variables["SubstHistory"] = substHistoryListDestination;

                if (successWaferIdRead)
                {
                    //// Trigger event
                    base.Equipment.SendMessage("SOSM19_NOTCONFIRMED_WAITINGFORHOST", null);
                                        
                    TestUtilities.WaitFor(ValidationTimeout, "Failed to recieve ProceedWithSubstrate", () =>
                    {
                        return proceedWithSubstrateCommandRecieved;
                    });
                    proceedWithSubstrateCommandRecieved = false;
                }
                else {
                    //// Trigger event
                    base.Equipment.SendMessage("SOSM18_NOTCONFIRMED_WAITINGFORHOST", null);

                    TestUtilities.WaitFor(ValidationTimeout, "Failed to recieve CancelSubstrate", () =>
                    {
                        return cancelSubstrateCommandRecieved;
                    });
                    cancelSubstrateCommandRecieved = false;
                    continue;
                }

                #endregion Wafer Read
                //now = DateTime.Now;
                //while (DateTime.Now.Subtract(now).Seconds < 2)
                //{
                //    // wait for 2 seconds
                //    Thread.Sleep(100);
                //}

                #region Wafer complete              

                base.Equipment.Variables["SubstID"] = wafer.Name;
                base.Equipment.Variables["SubstLotID"] = materialData.MaterialName;
                base.Equipment.Variables["SubstSubstLocID"] = String.Format("{0}.{1:D2}", destinationContainer, destinationPosition);
                base.Equipment.Variables["SubstState"] = 2;
                base.Equipment.Variables["SubstProcState"] = 2;
                base.Equipment.Variables["AcquiredID"] = "";
                base.Equipment.Variables["Clock"] =  DateTime.UtcNow.ToString("hh:mm:ss.fff");
                base.Equipment.Variables["SubstHistory"] = substHistoryListDestination;

                //// Trigger event
                base.Equipment.SendMessage("SOSM12_INPROCESS_PROCESSINGCOMPLETE", null);
                Thread.Sleep(2000);
                #endregion Wafer complete

                //now = DateTime.Now;
                //while (DateTime.Now.Subtract(now).Seconds < 2)
                //{
                //    // wait for 2 seconds
                //    Thread.Sleep(100);
                //}
            }

            #endregion Process Wafers on the movement list
        }

        private void CustomValidateContainerNumberOfWafers(Container container, int expectedNumberOfWafers)
        {
            // Validate container now has the expectedNumberOfWafers
            TestUtilities.WaitFor(ValidationTimeout, String.Format($"Container {container.Name} must have ({expectedNumberOfWafers}) wafers"), () =>
            {
                container.Load();
                return container.UsedPositions.Value == expectedNumberOfWafers;
            });
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

        /// <summary>
		/// This terminates all materials currently at this step.
		/// Also, makes sures sub material track depth is 0 for this step.
		/// </summary>
		private void StepPreparations()
        {
            Step step = new Step() { Name = stepName };
            step.Load();
            var output = new LoadStepMaterialsWithResourcesInput
            {
                Step = step
            }.LoadStepMaterialsWithResourcesSync();

            System.Data.DataSet dataSet = Cmf.TestScenarios.Others.Utilities.ToDataSet(output.LoadStepMaterialsWithResourcesResults);

            if (Cmf.Custom.TestUtilities.Generalization.HasData(dataSet))
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    Material materialAtStep = new Material() { Id = (long)row["MaterialId"] };
                    materialAtStep.Load(0);

                    if (materialAtStep.ParentMaterial == null)
                    {
                        if (materialAtStep.SystemState == MaterialSystemState.InProcess)
                        {
                            try
                            {
                                AbortMaterialProcessInput abortMaterialProcessInput = new AbortMaterialProcessInput()
                                {
                                    Material = materialAtStep,
                                    NumberOfRetries = 1,
                                };

                                materialAtStep = abortMaterialProcessInput.AbortMaterialProcessSync().Material;
                            }
                            catch (Exception)
                            {
                                // Could not abort. Carry on.
                            }
                        }

                        if (materialAtStep.UniversalState != UniversalState.Terminated ||
                            materialAtStep.UniversalState != UniversalState.Frozen)
                        {
                            if (materialAtStep.HoldCount > 0)
                            {
                                MaterialHoldReasonUserInformationCollection getHoldReasons = new GetMaterialHoldReasonsWithUserInformationInput()
                                {
                                    Material = materialAtStep,
                                    LevelsToLoad = 1
                                }.GetMaterialHoldReasonsWithUserInformationSync().MaterialHoldReasonUserInformationCollection;

                                if (getHoldReasons != null)
                                {
                                    MaterialHoldReasonCollection holdReasons = new MaterialHoldReasonCollection();
                                    getHoldReasons.ForEach(a => holdReasons.Add(a.MaterialHoldReason));

                                    materialAtStep = new ReleaseMaterialInput()
                                    {
                                        Material = materialAtStep,
                                        IgnoreLastServiceId = true,
                                        MaterialHoldReasonCollection = holdReasons
                                    }.ReleaseMaterialSync().Material;
                                }
                            }
                            Reason loss = materialAtStep.Step.GetRandomReason();

                            if (loss != null)
                            {
                                materialAtStep.Terminate(reasonToUse: loss);
                            }
                            else
                            {
                                materialAtStep.Terminate();
                            }
                        }
                    }
                }
            }

            step.SubMaterialTrackStateDepth = 0;
            step.Save();
        }

    }
}
