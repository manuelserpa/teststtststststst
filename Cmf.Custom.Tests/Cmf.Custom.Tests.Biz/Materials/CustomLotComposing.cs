using Cmf.Custom.amsOSRAM.BusinessObjects;
using Cmf.Custom.amsOSRAM.Orchestration.InputObjects;
using Cmf.Custom.amsOSRAM.Orchestration.OutputObjects;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.LightBusinessObjects.Infrastructure.Errors;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.Materials
{
    [TestClass]
    public class CustomLotComposing
    {
        private static CustomExecutionScenario executionScenario = null;
        private static List<long> RollbackDEEActions = new List<long>();

        private static bool? IsWaferReception;
        private static bool? IsSorter;
        private static bool? IsLoadPortInUse;
        private static Step Step;
        private static Resource Resource;
        private static Resource LoadPort;
        private static CustomSorterJobDefinition CustomSorterJobDefinition;

        private static string ResourceName = amsOSRAMConstants.DefaultSorterResourceName;
        private static string LoadPortName = amsOSRAMConstants.DefaultSorterLoadPort1Name;
        private static string BOMName = amsOSRAMConstants.BOM_BOM_11018814;
        private static int NumberSubstratesToCreate = 5;
        private static int NumberSubstratesSubstituteToCreate = 5;
        private static string ProductName = amsOSRAMConstants.DefaultWaferProductName;
        private static string ProductSubstituteName = amsOSRAMConstants.Product_11065179;
        private static string FlowPath = amsOSRAMConstants.FlowPathEPI_EPISorting;
        private static string FlowPathNoReceptionStep = amsOSRAMConstants.FlowPathEPI_EPIProposal;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Step = GenericUtilities.GetStepFromFlowPath(FlowPath);

            if (!Step.HasAttributeDefined(amsOSRAMConstants.StepAttributeIsWaferReception)
                || (bool)Step.Attributes.GetValueOrDefault(amsOSRAMConstants.StepAttributeIsWaferReception, false) == false)
            {
                IsWaferReception = false;
                Step.SaveAttribute(amsOSRAMConstants.StepAttributeIsWaferReception, true);
            }

            Resource = new Resource
            {
                Name = ResourceName
            };
            Resource.Load();

            if (!Resource.HasAttributeDefined(amsOSRAMConstants.ResourceAttributeIsSorter)
                || (bool)Resource.Attributes.GetValueOrDefault(amsOSRAMConstants.ResourceAttributeIsSorter, false) == false)
            {
                IsSorter = false;
                Resource.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter, true);
            }

            executionScenario = new CustomExecutionScenario();
            executionScenario.BOMContext.Add(new Dictionary<string, string>
            {
                { "Step", Step.Name },
                { "BOM", BOMName },
                { "AssemblyType", ((int)BOMAssemblyType.ExplicitAdd).ToString() }
            });

            CustomSorterJobDefinition = executionScenario.GenerateCustomSorterJobDefinition("Compose");

            executionScenario.CustomSorterJobDefinitionContext.Add(new Dictionary<string, string>
            {
                 { "Step", Step.Name },
                 { "CustomSorterJobDefinition", CustomSorterJobDefinition.Name }
            });

            executionScenario.Setup();

            executionScenario.SetResourceAutomationModeOnlineOrOffline(resource: Resource);

            LoadPort = new Resource
            {
                Name = LoadPortName
            };
            LoadPort.Load();

            if (!LoadPort.HasAttributeDefined(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse)
                || (bool)LoadPort.Attributes.GetValueOrDefault(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false) == true)
            {
                IsLoadPortInUse = true;
                LoadPort.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false);
            }

            // Run DEE with our custom hook to capture the message sent to IoT
            foreach (string deeName in new List<string> { "CustomSendTrackInInformationToIoT", "CustomSendTrackOutInformationToIoT" })
            {
                Foundation.Common.DynamicExecutionEngine.Action action = new GetActionByNameInput()
                {
                    Name = deeName
                }.GetActionByNameSync().Action;

                RollbackDEEActions.Add(action.Id);
                CustomUtilities.UpdateOrCreateDEE(action, "return Input;");
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (Step != null && IsWaferReception.HasValue)
            {
                Step.Load();
                Step.SaveAttribute(amsOSRAMConstants.StepAttributeIsWaferReception, IsWaferReception);
            }

            if (Resource != null && IsSorter.HasValue)
            {
                Resource.Load();
                Resource.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsSorter, IsSorter);
            }

            if (LoadPort != null && IsLoadPortInUse.HasValue)
            {
                LoadPort.Load();
                LoadPort.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, IsLoadPortInUse);
            }

            if (executionScenario != null)
            {
                executionScenario.CompleteCleanUp();
            }

            // Rollback DEE
            foreach (long actionId in RollbackDEEActions)
            {
                CustomUtilities.RollbackDEE(actionId);
            }
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            LoadPort.Load();

            if (!LoadPort.HasAttributeDefined(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse)
            || (bool)LoadPort.Attributes.GetValueOrDefault(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false) == true)
            {
                LoadPort.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false);
            }
        }

        /// <summary>
        /// Description:
        ///     - Create Lots on the Sorting Step (EPI Sorting)
        ///     - Create Wafers on the Sorting Step (EPI Sorting) inside a container docked to a LoadPort (ENA01-LP01) on the Resource (ENA01)
        ///     - Performs the MaterialIn and MaterialOut
        ///
        /// Acceptance Criteria:
        ///   - Should be used the Lot with the lower PlannedDate and higher priority
        ///   - Should create the MovementList mapping the source to be the destination (pick and store in the same position on the same container)
        ///   - Should create a LogicalWafer for each wafer and associate them to the Lot
        /// </summary>
        /// <TestCaseID>CustomLotComposing_HappyPath</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomLotComposing_HappyPath()
        {
            CustomExecutionScenario testScenario = null;

            try
            {
                testScenario = new CustomExecutionScenario();
                testScenario.FlowPath = FlowPath;
                testScenario.ScenarioQuantity = 0;
                testScenario.NumberOfMaterialsToGenerate = 3;

                testScenario.Setup();

                for (int i = 0; i < testScenario.GeneratedLots.Count; i++)
                {
                    Material generatedLot = testScenario.GeneratedLots[i];
                    generatedLot.PossibleStartDate = System.DateTime.Now.AddHours(i * -1);
                    generatedLot.Save();
                }

                for (int i = 0; i < NumberSubstratesToCreate; i++)
                {
                    testScenario.GenerateWafer(type: amsOSRAMConstants.MaterialWaferSubstrateType, productName: ProductName, flowPath: FlowPath);
                }

                for (int i = 0; i < NumberSubstratesSubstituteToCreate; i++)
                {
                    testScenario.GenerateWafer(type: amsOSRAMConstants.MaterialWaferSubstrateType, productName: ProductSubstituteName, flowPath: FlowPath);
                }

                Container container = testScenario.GenerateContainer(subMaterials: testScenario.GeneratedWafers, loadPortToDock: LoadPort).Entity;

                MaterialInOutput materialInOutput = new MaterialInInput()
                {
                    ResourceName = Resource.Name
                }.MaterialInSync();

                Material lot = ValidateMaterialIn(materialInOutput.Material, testScenario.GeneratedLots, testScenario.GeneratedWafers);

                CustomSorterJobDefinition.MovementList = GenerateMovementList(container).ToString();

                MaterialOutOutput materialOutOutput = new MaterialOutInput()
                {
                    MaterialName = materialInOutput.Material.Name,
                    ResourceName = Resource.Name,
                    CustomSorterJobDefinition = CustomSorterJobDefinition
                }.MaterialOutSync();

                ValidateMaterialOut(materialOutOutput.MaterialName, lot, testScenario.GeneratedWafers);
            }
            finally
            {
                if (testScenario != null)
                {
                    testScenario.CompleteCleanUp();
                }
            }
        }

        /// <summary>
        /// Description:
        ///     - Create Lots on the Sorting Step (EPI Sorting)
        ///     - Create Wafers on the Sorting Step (EPI Sorting) inside a container docked to a LoadPort (ENA01-LP01) on the Resource (ENA01)
        ///     - Performs the Orchestration TrackIn and TrackOut
        ///
        /// Acceptance Criteria:
        ///   - Should be used the Lot with the lower PlannedDate and higher priority
        ///   - Should create the MovementList mapping the source to be the destination (pick and store in the same position on the same container)
        ///   - Should only TrackIn and TrackOut the lot
        /// </summary>
        /// <TestCaseID>CustomLotComposing_WithoutMaterialInAndMaterialOut</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomLotComposing_WithoutMaterialInAndMaterialOut()
        {
            CustomExecutionScenario testScenario = null;

            try
            {
                testScenario = new CustomExecutionScenario();
                testScenario.FlowPath = FlowPath;
                testScenario.ScenarioQuantity = 0;
                testScenario.NumberOfMaterialsToGenerate = 3;

                testScenario.Setup();

                for (int i = 0; i < testScenario.GeneratedLots.Count; i++)
                {
                    Material generatedLot = testScenario.GeneratedLots[i];
                    generatedLot.PossibleStartDate = System.DateTime.Now.AddHours(i * -1);
                    generatedLot.Save();
                }

                for (int i = 0; i < NumberSubstratesToCreate; i++)
                {
                    testScenario.GenerateWafer(type: amsOSRAMConstants.MaterialWaferSubstrateType, productName: ProductName, flowPath: FlowPath);
                }

                for (int i = 0; i < NumberSubstratesSubstituteToCreate; i++)
                {
                    testScenario.GenerateWafer(type: amsOSRAMConstants.MaterialWaferSubstrateType, productName: ProductSubstituteName, flowPath: FlowPath);
                }

                Container container = testScenario.GenerateContainer(subMaterials: testScenario.GeneratedWafers, loadPortToDock: LoadPort).Entity;

                Material lot = testScenario.GeneratedLots[1];
                Resource.Load();
                lot = lot.ComplexDispatchAndTrackIn(Resource).Material;

                lot.ComplexTrackOutAndMoveNext();

                lot.Load();
                Assert.AreEqual(MaterialSystemState.Queued, lot.SystemState);
                Assert.AreEqual(0, lot.SubMaterialCount);
            }
            finally
            {
                if (testScenario != null)
                {
                    testScenario.CompleteCleanUp();
                }
            }
        }

        /// <summary>
        /// Description:
        ///     - Create Lots on the Sorting Step (EPI Sorting)
        ///     - Create Wafers on the Sorting Step (EPI Sorting) inside a container docked to a LoadPort (ENA01-LP01) on the Resource (ENA01)
        ///     - Performs the MaterialIn
        ///     - Change MovementList to have one invalid move
        ///     - Performs the MaterialOut
        ///
        /// Acceptance Criteria:
        ///   - Should throw an error with mismatch movement list
        /// </summary>
        /// <TestCaseID>CustomLotComposing_MismatchMovementList</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomLotComposing_MismatchMovementList()
        {
            CustomExecutionScenario testScenario = null;

            try
            {
                testScenario = new CustomExecutionScenario();
                testScenario.FlowPath = FlowPath;
                testScenario.ScenarioQuantity = 0;
                testScenario.NumberOfMaterialsToGenerate = 1;
                testScenario.Setup();

                for (int i = 0; i < NumberSubstratesToCreate; i++)
                {
                    testScenario.GenerateWafer(type: amsOSRAMConstants.MaterialWaferSubstrateType, productName: ProductName, flowPath: FlowPath);
                }

                for (int i = 0; i < NumberSubstratesSubstituteToCreate; i++)
                {
                    testScenario.GenerateWafer(type: amsOSRAMConstants.MaterialWaferSubstrateType, productName: ProductSubstituteName, flowPath: FlowPath);
                }

                Container container = testScenario.GenerateContainer(subMaterials: testScenario.GeneratedWafers, loadPortToDock: LoadPort).Entity;

                MaterialInOutput materialInOutput = new MaterialInInput()
                {
                    ResourceName = Resource.Name
                }.MaterialInSync();

                Material lot = testScenario.GeneratedLots.OrderBy(w => w.PossibleStartDate).ThenBy(w => w.Priority).FirstOrDefault();

                // Change
                JArray movementList = GenerateMovementList(container);
                movementList[0]["DestinationPosition"] = movementList[1]["DestinationPosition"];
                CustomSorterJobDefinition.MovementList = movementList.ToString();

                CmfFaultException cmfFaultException = Assert.ThrowsException<CmfFaultException>(() => new MaterialOutInput()
                {
                    MaterialName = materialInOutput.Material.Name,
                    ResourceName = Resource.Name,
                    CustomSorterJobDefinition = CustomSorterJobDefinition
                }.MaterialOutSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomMismatchMovementList);
                Assert.IsTrue(cmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            finally
            {
                if (testScenario != null)
                {
                    testScenario.CompleteCleanUp();
                }
            }
        }

        /// <summary>
        /// Description:
        ///     - Create Lots on the Sorting Step (EPI Sorting)
        ///     - Create Wafers on the Sorting Step (EPI Sorting) and substitutes on non Sorting Step (EPI Proposal) inside a container docked to a LoadPort (ENA01-LP01) on the Resource (ENA01)
        ///     - Performs the MaterialIn
        ///
        /// Acceptance Criteria:
        ///   - Should throw an error with no materials to TrackIn
        /// </summary>
        /// <TestCaseID>CustomLotComposing_WafersOutsideReceptionStep</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomLotComposing_WafersOutsideReceptionStep()
        {
            CustomExecutionScenario testScenario = null;

            try
            {
                testScenario = new CustomExecutionScenario();
                testScenario.FlowPath = FlowPath;
                testScenario.ScenarioQuantity = 0;
                testScenario.NumberOfMaterialsToGenerate = 1;
                testScenario.Setup();

                for (int i = 0; i < NumberSubstratesToCreate; i++)
                {
                    testScenario.GenerateWafer(type: amsOSRAMConstants.MaterialWaferSubstrateType, productName: ProductName, flowPath: FlowPath);
                }

                for (int i = 0; i < NumberSubstratesSubstituteToCreate; i++)
                {
                    testScenario.GenerateWafer(type: amsOSRAMConstants.MaterialWaferSubstrateType, productName: ProductSubstituteName, flowPath: FlowPathNoReceptionStep);
                }

                Container container = testScenario.GenerateContainer(subMaterials: testScenario.GeneratedWafers, loadPortToDock: LoadPort).Entity;

                CmfFaultException cmfFaultException = Assert.ThrowsException<CmfFaultException>(() => new MaterialInInput()
                {
                    ResourceName = Resource.Name
                }.MaterialInSync());

                string errorMessage = "No materials found to Track-In in the resource";
                Assert.IsTrue(cmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            finally
            {
                if (testScenario != null)
                {
                    testScenario.CompleteCleanUp();
                }
            }
        }

        /// <summary>
        /// Description:
        ///     - Create Lots on the Sorting Step (EPI Sorting)
        ///     - Create Wafers on the Sorting Step (EPI Sorting) without container docked to a LoadPort (ENA01-LP01) on the Resource (ENA01)
        ///     - Performs the MaterialIn
        ///
        /// Acceptance Criteria:
        ///   - Should throw an error with no materials to TrackIn
        /// </summary>
        /// <TestCaseID>CustomLotComposing_NoContainer</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomLotComposing_NoContainer()
        {
            CustomExecutionScenario testScenario = null;

            try
            {
                testScenario = new CustomExecutionScenario();
                testScenario.FlowPath = FlowPath;
                testScenario.ScenarioQuantity = 0;
                testScenario.NumberOfMaterialsToGenerate = 1;
                testScenario.Setup();

                for (int i = 0; i < NumberSubstratesToCreate; i++)
                {
                    testScenario.GenerateWafer(type: amsOSRAMConstants.MaterialWaferSubstrateType, productName: ProductName, flowPath: FlowPath);
                }

                for (int i = 0; i < NumberSubstratesSubstituteToCreate; i++)
                {
                    testScenario.GenerateWafer(type: amsOSRAMConstants.MaterialWaferSubstrateType, productName: ProductSubstituteName, flowPath: FlowPath);
                }

                CmfFaultException cmfFaultException = Assert.ThrowsException<CmfFaultException>(() => new MaterialInInput()
                {
                    ResourceName = Resource.Name
                }.MaterialInSync());

                string errorMessage = "No materials found to Track-In in the resource";
                Assert.IsTrue(cmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            finally
            {
                if (testScenario != null)
                {
                    testScenario.CompleteCleanUp();
                }
            }
        }

        /// <summary>
        /// Description:
        ///     - Create Lots on the Sorting Step (EPI Sorting)
        ///     - Create multiple Wafers on the Sorting Step (EPI Sorting) inside a multiple container docked to a LoadPort (ENA01-LP01) on the Resource (ENA01)
        ///     - Performs the MaterialIn and MaterialOut
        ///
        /// Acceptance Criteria:
        ///   - Should be used the Lot with the lower PlannedDate and higher priority
        ///   - Should create the MovementList mapping the source to be the destination (pick and store in the same position on the same container)
        ///   - Should create a LogicalWafer for each wafer and associate them to the Lot
        /// </summary>
        /// <TestCaseID>CustomLotComposing_MultipleContainer</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomLotComposing_MultipleContainer()
        {
            CustomExecutionScenario testScenario = null;

            try
            {
                testScenario = new CustomExecutionScenario();
                testScenario.FlowPath = FlowPath;
                testScenario.ScenarioQuantity = 0;
                testScenario.NumberOfMaterialsToGenerate = 2;
                testScenario.Setup();

                MaterialCollection waferCollectionToTrackOut = new MaterialCollection();
                for (int i = 0; i < NumberSubstratesToCreate; i++)
                {
                    System.Tuple<Material, Material> generatedWafer = testScenario.GenerateWafer(type: amsOSRAMConstants.MaterialWaferSubstrateType, productName: ProductName, flowPath: FlowPath);
                    waferCollectionToTrackOut.Add(generatedWafer.Item1);
                }

                for (int i = 0; i < NumberSubstratesSubstituteToCreate; i++)
                {
                    System.Tuple<Material, Material> generatedWafer = testScenario.GenerateWafer(type: amsOSRAMConstants.MaterialWaferSubstrateType, productName: ProductSubstituteName, flowPath: FlowPath);
                    waferCollectionToTrackOut.Add(generatedWafer.Item1);
                }

                Container containerToTrackOut = testScenario.GenerateContainer(subMaterials: waferCollectionToTrackOut, loadPortToDock: LoadPort).Entity;

                MaterialCollection waferCollection = new MaterialCollection();
                for (int i = 0; i < NumberSubstratesToCreate + NumberSubstratesSubstituteToCreate; i++)
                {
                    System.Tuple<Material, Material> generatedWafer = testScenario.GenerateWafer(type: amsOSRAMConstants.MaterialWaferSubstrateType, productName: ProductName, flowPath: FlowPath);
                    waferCollection.Add(generatedWafer.Item1);
                }

                Container container = testScenario.GenerateContainer(subMaterials: waferCollection, loadPortToDock: LoadPort).Entity;

                MaterialInOutput materialInOutput = new MaterialInInput()
                {
                    ResourceName = Resource.Name
                }.MaterialInSync();

                Material lot = ValidateMaterialIn(materialInOutput.Material, testScenario.GeneratedLots, testScenario.GeneratedWafers);

                CustomSorterJobDefinition.MovementList = GenerateMovementList(containerToTrackOut).ToString();

                MaterialOutOutput materialOutOutput = new MaterialOutInput()
                {
                    MaterialName = materialInOutput.Material.Name,
                    ResourceName = Resource.Name,
                    CustomSorterJobDefinition = CustomSorterJobDefinition
                }.MaterialOutSync();

                ValidateMaterialOut(materialOutOutput.MaterialName, lot, waferCollectionToTrackOut);
            }
            finally
            {
                if (testScenario != null)
                {
                    testScenario.CompleteCleanUp();
                }
            }
        }

        private Material ValidateMaterialIn(Material materialResponse, MaterialCollection lots, MaterialCollection wafers)
        {
            Material lot = lots.OrderBy(w => w.PossibleStartDate).ThenBy(w => w.Priority).FirstOrDefault();

            Assert.AreEqual(lot.Name, materialResponse.Name);
            Assert.AreEqual(MaterialSystemState.InProcess, materialResponse.SystemState);

            lot.LoadChildren();
            Assert.AreEqual(0, lot.SubMaterialCount, $"Material {lot.Name} should not have any SubMaterial");

            foreach (Material wafer in wafers)
            {
                Assert.IsNull(wafer.ParentMaterial, $"Material {wafer.Name} should not have any Parent Material");
            }

            return lot;
        }

        private Material ValidateMaterialOut(string materialResponse, Material lot, MaterialCollection wafers)
        {
            Assert.AreEqual(lot.Name, materialResponse);

            lot.Load();
            Assert.AreEqual(MaterialSystemState.Queued, lot.SystemState);

            lot.LoadChildren();
            Assert.AreEqual(lot.SubMaterialCount, wafers.Count);

            JArray movementList = JArray.Parse(CustomSorterJobDefinition.MovementList);
            foreach (Material logicalWafer in lot.SubMaterials)
            {
                logicalWafer.LoadChildren();
                Assert.AreEqual(1, logicalWafer.SubMaterialCount);

                Material wafer = logicalWafer.SubMaterials.FirstOrDefault();

                wafer.LoadRelation("MaterialContainer");
                Assert.IsNull(wafer.MaterialContainer);

                JToken movement = movementList.FirstOrDefault(f => f.Value<string>("MaterialName") == wafer.Name);

                logicalWafer.LoadRelation("MaterialContainer", 1);
                Assert.IsNotNull(logicalWafer.MaterialContainer);
                Assert.AreEqual(1, logicalWafer.MaterialContainer.Count);
                Assert.AreEqual(movement.Value<string>("DestinationContainer"), logicalWafer.MaterialContainer.FirstOrDefault().TargetEntity.Name);

                Assert.IsNotNull(movementList);
                Assert.AreEqual($"{lot.Name}_{movement.Value<string>("DestinationPosition")}", logicalWafer.Name);
                Assert.AreEqual(amsOSRAMConstants.DefaultMaterialLogicalWaferForm, logicalWafer.Form, $"Material {logicalWafer.Name} should have be {logicalWafer.Form}");
            }

            return lot;
        }

        private JArray GenerateMovementList(Container container)
        {
            container.Load();
            container.LoadRelation("MaterialContainer", 2);

            JArray jArray = new JArray();

            foreach (MaterialContainer containerMaterial in container.ContainerMaterials)
            {
                jArray.Add(new JObject
                {
                    ["Product"] = containerMaterial?.SourceEntity?.Product?.Name ?? "",
                    ["MaterialName"] = containerMaterial?.SourceEntity?.Name ?? "",
                    ["SourceContainer"] = containerMaterial.TargetEntity.Name,
                    ["SourcePosition"] = containerMaterial.Position,
                    ["DestinationContainer"] = containerMaterial.TargetEntity.Name,
                    ["DestinationPosition"] = containerMaterial.Position
                });
            }

            return jArray;
        }
    }
}
