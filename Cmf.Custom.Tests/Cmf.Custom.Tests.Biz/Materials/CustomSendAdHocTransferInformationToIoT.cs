using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.LightBusinessObjects.Infrastructure.Errors;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Cmf.Custom.Tests.Biz.Materials
{
    [TestClass]
    public class CustomSendAdHocTransferInformationToIoT
    {
        private static Dictionary<string, object> rollbackResourceProperties = new Dictionary<string, object>();
        private static Cmf.Foundation.Common.DynamicExecutionEngine.Action rollbackDEEAction;
        private static CustomExecutionScenario classExecutionScenario = new CustomExecutionScenario();
        private static CustomMaterialScenario classMaterialScenario = null;
        private static string resourceName = amsOSRAMConstants.DefaultSorterResourceName;
        private static string sorterProcess = amsOSRAMConstants.CustomSorterProcessWaferReception;
        private static string loadPortName = amsOSRAMConstants.DefaultSorterLoadPort1Name;
        private static string productName = amsOSRAMConstants.DefaultWaferProductName;
        private static string loadPortToDockName = amsOSRAMConstants.DefaultSorterLoadPort2Name;
        private static string loadPortToDock2Name = amsOSRAMConstants.DefaultSorterLoadPort3Name;
        private static string recipeName = amsOSRAMConstants.DefaultRecipeName;
        private static string serviceName = amsOSRAMConstants.ServiceWaferReception;
        private static int sourceCapacity = 10;
        private static int targetCapacity = amsOSRAMConstants.ContainerTotalPosition - 2;

        private ExecuteActionInput actionToExecute;
        private ResourceCollection resourcesToCleanup;

        /// <summary>
        /// Test Initialize
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Set IsRecipeManagementEnabled on the resource
            Resource resource = new Resource();
            resource.Name = amsOSRAMConstants.DefaultSorterResourceName;

            resource.Load();

            rollbackResourceProperties.Add("IsRecipeManagementEnabled", resource.IsRecipeManagementEnabled);
            resource.IsRecipeManagementEnabled = true;

            resource.Save();

            classExecutionScenario = new CustomExecutionScenario
            {
                ResourceAttributesToSet = new Dictionary<string, Foundation.BusinessObjects.AttributeCollection>()
                {
                    {
                        resourceName, new Foundation.BusinessObjects.AttributeCollection {
                            { amsOSRAMConstants.ResourceAttributeIsSorter, true }
                        }
                    },
                    {
                        loadPortName, new Foundation.BusinessObjects.AttributeCollection {
                            { amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false }
                        }
                    },
                    {
                        loadPortToDockName, new Foundation.BusinessObjects.AttributeCollection {
                            { amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false }
                        }
                    },
                    {
                        loadPortToDock2Name, new Foundation.BusinessObjects.AttributeCollection {
                            { amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false }
                        }
                    }
                },
                CustomProductContainerCapacities = new List<Dictionary<string, string>>
                {
                    {
                        new Dictionary<string, string> {
                            {"Product", productName },
                            { "SourceCapacity", sourceCapacity.ToString() },
                            { "TargetCapacity", targetCapacity.ToString() }
                        }
                    }
                },
                RecipeContext = new List<Dictionary<string, string>>
                {
                    {
                        new Dictionary<string, string> {
                            { "Service", serviceName },
                            { "Product", productName },
                            { "Resource", resourceName },
                            { "Recipe", recipeName }
                        }
                    }
                }
            };

            classExecutionScenario.Setup();

            // Set Resource Online
            classMaterialScenario = new CustomMaterialScenario(setResourceOnline: true, createContainer: false, setResourceOffline: false);
            classMaterialScenario.Resource = resource;

            classMaterialScenario.Setup();

            // Run DEE with our custom hook to capture the message sent to IoT
            string deeName = "CustomSendAdHocTransferInformationToIoT";

            GetActionByNameOutput getActionByNameOutput = new GetActionByNameInput()
            {
                Name = deeName
            }.GetActionByNameSync();

            Assert.IsNotNull(getActionByNameOutput.Action, $"The DEE {deeName} is missing");

            rollbackDEEAction = getActionByNameOutput.Action;
            CustomUtilities.UpdateOrCreateDEE(getActionByNameOutput.Action.Name, getActionByNameOutput.Action.ActionCode, "Input[\"Result\"] = materialDataToIot;return Input;", validationCode: getActionByNameOutput.Action.ValidationCode);
        }

        /// <summary>
        /// Test Initialize
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // Rollback Scenario
            if (classExecutionScenario != null)
            {
                classExecutionScenario.CompleteCleanUp();
            }

            // Rollback IsRecipeManagementEnabled on the resource
            Resource resource = new Resource();
            resource.Name = amsOSRAMConstants.DefaultSorterResourceName;
            resource.Load();

            resource.IsRecipeManagementEnabled = (bool?)rollbackResourceProperties["IsRecipeManagementEnabled"];

            resource.Save();

            // Rollback Resource state
            if (classMaterialScenario != null)
            {
                classMaterialScenario.TearDown();
            }

            // Rollback DEE
            CustomUtilities.UpdateOrCreateDEE(rollbackDEEAction.Name, rollbackDEEAction.ActionCode, validationCode: rollbackDEEAction.ValidationCode);
        }

        /// <summary>
        /// Test Initialize
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            actionToExecute = new ExecuteActionInput()
            {
                Action = new Foundation.Common.DynamicExecutionEngine.Action
                {
                    Name = "CustomSendAdHocTransferInformationToIoT"
                },
                Input = new Dictionary<string, object> {
                    { "Resource", resourceName },
                    { "SorterProcess", sorterProcess },
                    { "Quantity", targetCapacity },
                    { "Product", productName },
                    { "LoadPort", loadPortName }
                }
            };

            resourcesToCleanup = new ResourceCollection();
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            resourcesToCleanup.Load();

            foreach (Resource resource in resourcesToCleanup)
            {
                resource.SaveAttribute(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false);
            }
        }

        /// <summary>
        /// Description: Validates Message sent to IoT triggered by DEE SendAdHocTransferInformationToIoT with a single container docked
        /// Acceptance Criteria:
        ///   - Inputs must be valid and entities must exist in the system
        ///   - Resource (Parent resource of LoadPorts) must have IsRecipeManagementEnabled set to true
        ///   - Resource (Parent resource of LoadPorts) must be a Sorter. The attribute isSorter set to true
        ///   - Resource (Parent resource of LoadPorts) must be online.
        ///   - Source LoadPort must be a SubResource of the given Resource
        ///   - Source LoadPort must not be in use. The attribute IsLoadPortInUse set to false
        ///   - SmartTable CustomProductContainerCapacities must have at least one resolved row.
        ///   - SmartTable RecipeContext must have at least one resolved row with the `WaferReception` as a Service.
        ///   - To be a possible destination port:
        ///       - Must have a container docked
        ///       - Must not be in use. The attribute IsLoadPortInUse set to false
        ///       - Must have free positions
        ///       - Must have a match between the product provided and the product materials on the docked container, if exists.
        ///   - The sum of possible destination port positions must fulfill the given quantity
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_SingleContainer_HappyPath</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_SingleContainer_HappyPath()
        {
            CustomMaterialScenario materialScenario = null;

            try
            {
                materialScenario = new CustomMaterialScenario(setResourceOnline: false, createContainer: true, setResourceOffline: false)
                {
                    NumberOfSubMaterials = 0
                };
                materialScenario.Setup(productName: productName, positionSorting: ContainerPositionSorting.Descending);

                Resource loadPortToDock1 = new Resource();
                loadPortToDock1.Name = loadPortToDockName;
                loadPortToDock1.Load();

                materialScenario.DockContainer(loadPortToDock1);

                MaterialData materialData = RunExecuteAction(actionToExecute);

                Resource sourceLoadPort = new Resource();
                sourceLoadPort.Name = loadPortName;
                sourceLoadPort.Load();

                resourcesToCleanup.Add(loadPortToDock1);
                resourcesToCleanup.Add(sourceLoadPort);

                Recipe recipe = new Recipe();
                recipe.Name = recipeName;
                recipe.Load();

                string loadPortPosition = sourceLoadPort.DisplayOrder.Value.ToString();
                string carrierAtLoadPort = $"CarrierAtLoadPort{loadPortPosition}";

                ValidateMaterialData(materialData, carrierAtLoadPort, loadPortPosition);
                ValidateRecipe(recipe, materialData.Recipe);
                ValidateCustomSorterJobDefinition(materialData);

                dynamic movementList = JsonArray.Parse(materialData.SorterJobInformation.MovementList.ToString());
                int quantityToFullfil = (int)actionToExecute.Input["Quantity"];

                Container container = materialScenario.ContainerScenario.Entity;

                ContainerCollection containers = new ContainerCollection();
                containers.Add(container);

                ValidateMovementList(movementList, containers, quantityToFullfil, targetCapacity, carrierAtLoadPort);

                Assert.IsTrue(sourceLoadPort.GetAttributeValue(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false), $"LoadPort {sourceLoadPort.Name} should have the attribute {amsOSRAMConstants.ResourceAttributeIsLoadPortInUse} set to true");
                Assert.IsTrue(loadPortToDock1.GetAttributeValue(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false), $"LoadPort {loadPortToDock1.Name} should have the attribute {amsOSRAMConstants.ResourceAttributeIsLoadPortInUse} set to true");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (materialScenario != null)
                {
                    materialScenario.TearDown();
                }
            }
        }

        /// <summary>
        /// Description: Validates Message sent to IoT triggered by DEE SendAdHocTransferInformationToIoT with two containers docked
        /// Acceptance Criteria:
        ///   - Inputs must be valid and entities must exist in the system
        ///   - Resource (Parent resource of LoadPorts) must have IsRecipeManagementEnabled set to true
        ///   - Resource (Parent resource of LoadPorts) must be a Sorter. The attribute isSorter set to true
        ///   - Resource (Parent resource of LoadPorts) must be online.
        ///   - Source LoadPort must be a SubResource of the given Resource
        ///   - Source LoadPort must not be in use. The attribute IsLoadPortInUse set to false
        ///   - SmartTable CustomProductContainerCapacities must have at least one resolved row.
        ///   - SmartTable RecipeContext must have at least one resolved row.
        ///   - To be a possible destination port:
        ///       - Must have a container docked
        ///       - Must not be in use. The attribute IsLoadPortInUse set to false
        ///       - Must have free positions
        ///       - Must have a match between the product provided and the materials on the docked container, if exists.
        ///   - The sum of possible destination port positions must fulfill the given quantity
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_MultipleContainers_HappyPath</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_MultipleContainers_HappyPath()
        {
            CustomMaterialScenario materialScenarioLP1 = null;
            CustomMaterialScenario materialScenarioLP2 = null;

            try
            {
                materialScenarioLP1 = new CustomMaterialScenario(setResourceOnline: false, createContainer: true, setResourceOffline: false)
                {
                    NumberOfSubMaterials = 0
                };
                materialScenarioLP1.Setup(productName: productName);

                int numberOfSubMaterialsLP2 = 1;

                materialScenarioLP2 = new CustomMaterialScenario(setResourceOnline: false, createContainer: true, setResourceOffline: false)
                {
                    AssociateSubMaterialsToContainer = false,
                    NumberOfSubMaterials = numberOfSubMaterialsLP2
                };
                materialScenarioLP2.Setup(productName: productName, positionSorting: ContainerPositionSorting.Descending, automaticContainerPositions: false);

                // Add Material to a given position
                Material submaterial = materialScenarioLP2.SubMaterials.FirstOrDefault();

                MaterialContainerCollection materialContainerCollection = new MaterialContainerCollection();
                materialContainerCollection.Add(new MaterialContainer
                {
                    SourceEntity = submaterial,
                    Position = amsOSRAMConstants.ContainerTotalPosition
                });

                materialScenarioLP2.ContainerScenario.AssociateMaterials(materialContainerCollection);

                Resource loadPortToDock1 = new Resource();
                loadPortToDock1.Name = loadPortToDockName;
                loadPortToDock1.Load();

                materialScenarioLP1.DockContainer(loadPortToDock1);

                Resource loadPortToDock2 = new Resource();
                loadPortToDock2.Name = loadPortToDock2Name;
                loadPortToDock2.Load();

                materialScenarioLP2.DockContainer(loadPortToDock2);

                // We need to remove the number of submaterial to not overflow the container
                actionToExecute.Input["Quantity"] = (int)actionToExecute.Input["Quantity"] * 2 - numberOfSubMaterialsLP2;

                MaterialData materialData = RunExecuteAction(actionToExecute);

                Resource sourceLoadPort = new Resource();
                sourceLoadPort.Name = loadPortName;
                sourceLoadPort.Load();

                resourcesToCleanup.Add(loadPortToDock1);
                resourcesToCleanup.Add(loadPortToDock2);
                resourcesToCleanup.Add(sourceLoadPort);

                Recipe recipe = new Recipe();
                recipe.Name = recipeName;
                recipe.Load();

                string loadPortPosition = sourceLoadPort.DisplayOrder.Value.ToString();
                string carrierAtLoadPort = $"CarrierAtLoadPort{loadPortPosition}";

                ValidateMaterialData(materialData, carrierAtLoadPort, loadPortPosition);
                ValidateRecipe(recipe, materialData.Recipe);
                ValidateCustomSorterJobDefinition(materialData);

                dynamic movementList = JsonArray.Parse(materialData.SorterJobInformation.MovementList.ToString());
                int quantityToFullfil = (int)actionToExecute.Input["Quantity"];

                Container container1 = materialScenarioLP1.ContainerScenario.Entity;
                Container container2 = materialScenarioLP2.ContainerScenario.Entity;

                ContainerCollection containers = new ContainerCollection();
                containers.Add(container1);
                containers.Add(container2);

                ValidateMovementList(movementList, containers, quantityToFullfil, targetCapacity, carrierAtLoadPort);

                loadPortToDock1.LoadAttribute(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse);
                loadPortToDock2.LoadAttribute(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse);

                Assert.IsTrue(sourceLoadPort.GetAttributeValue(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false), $"LoadPort {sourceLoadPort.Name} should have the attribute {amsOSRAMConstants.ResourceAttributeIsLoadPortInUse} set to true");
                Assert.IsTrue(loadPortToDock1.GetAttributeValue(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false), $"LoadPort {loadPortToDock1.Name} should have the attribute {amsOSRAMConstants.ResourceAttributeIsLoadPortInUse} set to true");
                Assert.IsTrue(loadPortToDock2.GetAttributeValue(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false), $"LoadPort {loadPortToDock2.Name} should have the attribute {amsOSRAMConstants.ResourceAttributeIsLoadPortInUse} set to true");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (materialScenarioLP1 != null)
                {
                    materialScenarioLP1.TearDown();
                }

                if (materialScenarioLP2 != null)
                {
                    materialScenarioLP2.TearDown();
                }
            }
        }

        /// <summary>
        /// Description: Validates Message sent to IoT triggered by DEE SendAdHocTransferInformationToIoT with a single container docked
        /// Acceptance Criteria:
        ///   - Inputs must be valid and entities must exist in the system
        ///   - Resource (Parent resource of LoadPorts) must have IsRecipeManagementEnabled set to true
        ///   - Resource (Parent resource of LoadPorts) must be a Sorter. The attribute isSorter set to true
        ///   - Resource (Parent resource of LoadPorts) must be online.
        ///   - Source LoadPort must be a SubResource of the given Resource
        ///   - Source LoadPort must not be in use. The attribute IsLoadPortInUse set to false
        ///   - SmartTable CustomProductContainerCapacities must have at least one resolved row.
        ///   - SmartTable RecipeContext must have at least one resolved row with the `WaferReception` as a Service.
        ///   - To be a possible destination port:
        ///       - Must have a container docked
        ///       - Must not be in use. The attribute IsLoadPortInUse set to false
        ///       - Must have free positions
        ///       - Must have a match between the product provided and the product materials on the docked container, if exists.
        ///   - The sum of possible destination port positions must fulfill the given quantity
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_SingleContainerSourceLoadPort_HappyPath</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_SingleContainerSourceLoadPort_HappyPath()
        {
            CustomMaterialScenario materialScenario = null;

            try
            {
                materialScenario = new CustomMaterialScenario(setResourceOnline: false, createContainer: true, setResourceOffline: false)
                {
                    NumberOfSubMaterials = 0
                };
                materialScenario.Setup(productName: productName, positionSorting: ContainerPositionSorting.Descending);

                Resource sourceLoadPort = new Resource();
                sourceLoadPort.Name = loadPortName;
                sourceLoadPort.Load();

                materialScenario.DockContainer(sourceLoadPort);

                MaterialData materialData = RunExecuteAction(actionToExecute);

                resourcesToCleanup.Add(sourceLoadPort);

                Recipe recipe = new Recipe();
                recipe.Name = recipeName;
                recipe.Load();

                string loadPortPosition = sourceLoadPort.DisplayOrder.Value.ToString();
                string carrierAtLoadPort = $"CarrierAtLoadPort{loadPortPosition}";

                ValidateMaterialData(materialData, carrierAtLoadPort, loadPortPosition);
                ValidateRecipe(recipe, materialData.Recipe);
                ValidateCustomSorterJobDefinition(materialData);

                dynamic movementList = JsonArray.Parse(materialData.SorterJobInformation.MovementList.ToString());
                int quantityToFullfil = (int)actionToExecute.Input["Quantity"];

                Container container = materialScenario.ContainerScenario.Entity;

                ContainerCollection containers = new ContainerCollection();
                containers.Add(container);

                ValidateMovementList(movementList, containers, quantityToFullfil, targetCapacity, carrierAtLoadPort);

                Assert.IsTrue(sourceLoadPort.GetAttributeValue(amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false), $"LoadPort {sourceLoadPort.Name} should have the attribute {amsOSRAMConstants.ResourceAttributeIsLoadPortInUse} set to true");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (materialScenario != null)
                {
                    materialScenario.TearDown();
                }
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE SendAdHocTransferInformationToIoT with invalid and missing inputs
        /// Acceptance Criteria: Inputs must be valid and entities must exist in the system
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_ValidateInput_MissingInvalidInput</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_ValidateInput_MissingInvalidInput()
        {
            #region Validate missing inputs

            Dictionary<string, object> newInput;
            CmfFaultException inputCmfFaultException;

            Dictionary<string, object> initialInput = new Dictionary<string, object>(actionToExecute.Input);
            string errorMessage = "Missing value for mandatory property {0}";

            foreach (string input in initialInput.Keys)
            {
                newInput = new Dictionary<string, object>(initialInput);
                newInput.Remove(input);

                actionToExecute.Input = newInput;

                inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());
                Assert.IsTrue(inputCmfFaultException.Message.Contains(String.Format(errorMessage, input)), $"Should have the following error message: {String.Format(errorMessage, input)}");
            }

            actionToExecute.Input = initialInput;

            #endregion Validate missing inputs

            #region Validate if inputs exist on MES

            string inputName = "Resource";
            newInput = new Dictionary<string, object>(initialInput);
            newInput[inputName] = CustomUtilities.GenerateName($"Random_{inputName}");

            actionToExecute.Input = newInput;
            errorMessage = $"Resource {newInput[inputName]} was not found";

            inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());
            Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");

            actionToExecute.Input = initialInput;

            inputName = "Product";
            newInput = new Dictionary<string, object>(initialInput);
            newInput[inputName] = CustomUtilities.GenerateName($"Random_{inputName}");

            actionToExecute.Input = newInput;
            errorMessage = $"Product {newInput[inputName]} was not found";

            inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());
            Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");

            actionToExecute.Input = initialInput;

            inputName = "LoadPort";
            newInput = new Dictionary<string, object>(initialInput);
            newInput[inputName] = CustomUtilities.GenerateName($"Random_{inputName}");

            actionToExecute.Input = newInput;
            errorMessage = $"Resource {newInput[inputName]} was not found";

            inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());
            Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");

            actionToExecute.Input = initialInput;

            #endregion Validate if inputs exist on MES

            #region Check SorterProcess Lookup Table

            inputName = "SorterProcess";
            newInput = new Dictionary<string, object>(initialInput);
            newInput[inputName] = CustomUtilities.GenerateName($"Random_{inputName}");

            actionToExecute.Input = newInput;

            CmfFaultException cmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

            errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomValueDoesNotExistLookupTable, newInput[inputName], amsOSRAMConstants.CustomSorterProcessLookupTable);

            Assert.IsTrue(cmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");

            #endregion Check SorterProcess Lookup Table
        }

        /// <summary>
        /// Description: Validates error message sent by DEE SendAdHocTransferInformationToIoT with a resource that is not a sorter
        /// Acceptance Criteria: Resource (Parent resource of LoadPorts) must be a Sorter. The attribute isSorter set to true
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_ValidateResource_NotSorter</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_ValidateResource_NotSorter()
        {
            CustomExecutionScenario executionScenario = new CustomExecutionScenario();

            try
            {
                executionScenario.ResourceAttributesToSet = new Dictionary<string, Foundation.BusinessObjects.AttributeCollection>()
                {
                    {
                        resourceName, new Foundation.BusinessObjects.AttributeCollection {
                            { amsOSRAMConstants.ResourceAttributeIsSorter, null }
                        }
                    }
                };
                executionScenario.Setup();

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomResourceIsNotSorter, resourceName);

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                executionScenario.CompleteCleanUp();
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE SendAdHocTransferInformationToIoT with a resource offline
        /// Acceptance Criteria: Resource (Parent resource of LoadPorts) must be online.
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_ValidateResource_Offline</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_ValidateResource_Offline()
        {
            string resourceName = actionToExecute.Input.GetValueOrDefault("Resource", amsOSRAMConstants.DefaultSorterResourceName) as string;
            CustomMaterialScenario materialScenario = null;

            try
            {
                materialScenario = new CustomMaterialScenario(setResourceOnline: false, createContainer: false, setResourceOffline: true)
                {
                    NumberOfSubMaterials = 0
                };

                Resource resource = new Resource();
                resource.Name = resourceName;

                materialScenario.Resource = resource;
                materialScenario.Setup();

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());
                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomResourceNotOnline, resourceName);

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (materialScenario != null)
                {
                    materialScenario.TearDown();
                }
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE SendAdHocTransferInformationToIoT with a LoadPort that is not a SubResource of the given Resource
        /// Acceptance Criteria: Source LoadPort must be a SubResource of the given Resource
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_ValidateLoadPort_NotLoadPortOfResource</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_ValidateLoadPort_NotLoadPortOfResource()
        {
            string loadPortName = amsOSRAMConstants.DefaultTestResourceName;

            CustomExecutionScenario executionScenario = new CustomExecutionScenario();

            try
            {
                executionScenario.ResourceAttributesToSet = new Dictionary<string, Foundation.BusinessObjects.AttributeCollection>()
                {
                    {
                        loadPortName, new Foundation.BusinessObjects.AttributeCollection {
                            { amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, false }
                        }
                    }
                };
                executionScenario.Setup();

                // Change LoadPort to another Resource
                actionToExecute.Input["LoadPort"] = loadPortName;

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomResourceNotDescendant, loadPortName, resourceName);

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                executionScenario.CompleteCleanUp();
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE SendAdHocTransferInformationToIoT with a LoadPort in use
        /// Acceptance Criteria: Source LoadPort must not be in use. The attribute IsLoadPortInUse set to false
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_ValidateLoadPort_InUse</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_ValidateLoadPort_InUse()
        {
            CustomExecutionScenario executionScenario = new CustomExecutionScenario();

            try
            {
                executionScenario.ResourceAttributesToSet = new Dictionary<string, Foundation.BusinessObjects.AttributeCollection>()
                {
                    {
                        loadPortName, new Foundation.BusinessObjects.AttributeCollection {
                            { amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, true }
                        }
                    }
                };
                executionScenario.Setup();

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomResourceInUse, loadPortName);

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                executionScenario.CompleteCleanUp();
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE SendAdHocTransferInformationToIoT without any resolution for the ST CustomProductContainerCapacities
        /// Acceptance Criteria: SmartTable CustomProductContainerCapacities must have at least one resolved row.
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_ValidateSmartTableQuantities_NoResolution</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_ValidateSmartTableQuantities_NoResolution()
        {
            CustomExecutionScenario executionScenario = new CustomExecutionScenario();

            try
            {
                executionScenario.SmartTablesToClearInSetup.Add(amsOSRAMConstants.CustomProductContainerCapacitiesSmartTable);
                executionScenario.Setup();

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomSmartTableNoResolution, "Capacity", amsOSRAMConstants.CustomProductContainerCapacitiesSmartTable);

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                executionScenario.CompleteCleanUp();
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE SendAdHocTransferInformationToIoT without any resolution for the ST RecipeContext
        /// Acceptance Criteria: SmartTable RecipeContext must have at least one resolved row.
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_ValidateRecipeContext_NoResolution</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_ValidateRecipeContext_NoResolution()
        {
            CustomExecutionScenario executionScenario = new CustomExecutionScenario();

            try
            {
                executionScenario.SmartTablesToClearInSetup.Add("RecipeContext");
                executionScenario.Setup();

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomSmartTableNoResolution, "Recipe", "RecipeContext");

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                executionScenario.CompleteCleanUp();
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE SendAdHocTransferInformationToIoT without any docked container on the destination LoadPorts
        /// Acceptance Criteria: The destination LoadPort must have a container docked
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_ValidateContainer_NoContainerDocked</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_ValidateContainer_NoContainerDocked()
        {
            CustomExecutionScenario executionScenario = new CustomExecutionScenario();

            try
            {
                executionScenario.Setup();

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomResourceNoDockerContainer, resourceName);

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                executionScenario.CompleteCleanUp();
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE SendAdHocTransferInformationToIoT with docked container in use
        /// Acceptance Criteria: The destination LoadPort must not be in use. The attribute IsLoadPortInUse set to false
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_ValidateContainer_DestinationLoadPortToDockInUse</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_ValidateContainer_DestinationLoadPortToDockInUse()
        {
            CustomExecutionScenario executionScenario = new CustomExecutionScenario();
            CustomMaterialScenario materialScenario = null;

            try
            {
                materialScenario = new CustomMaterialScenario(setResourceOnline: false, createContainer: true, setResourceOffline: false)
                {
                    NumberOfSubMaterials = 0
                };

                executionScenario.ResourceAttributesToSet = new Dictionary<string, Foundation.BusinessObjects.AttributeCollection>()
                {
                    {
                        loadPortToDockName, new Foundation.BusinessObjects.AttributeCollection {
                            { amsOSRAMConstants.ResourceAttributeIsLoadPortInUse, true }
                        }
                    }
                };

                executionScenario.Setup();
                materialScenario.Setup();

                Resource loadPortToDock = new Resource();
                loadPortToDock.Name = loadPortToDockName;
                loadPortToDock.Load();

                resourcesToCleanup.Add(loadPortToDock);

                materialScenario.DockContainer(loadPortToDock);

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomResourceNoEnoughPositionsOrInUse, resourceName);

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                executionScenario.CompleteCleanUp();

                if (materialScenario != null)
                {
                    materialScenario.TearDown();
                }
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE SendAdHocTransferInformationToIoT with the docked container full to fullfil the desired quantity.
        /// Acceptance Criteria: The destination LoadPort must have free positions to fullfil the desired quantity
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_ValidateContainer_DestinationLoadPortFull</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_ValidateContainer_DestinationLoadPortFull()
        {
            CustomMaterialScenario materialScenario = null;

            try
            {
                materialScenario = new CustomMaterialScenario(setResourceOnline: false, createContainer: true, setResourceOffline: false)
                {
                    AssociateSubMaterialsToContainer = true,
                    NumberOfSubMaterials = targetCapacity
                };

                materialScenario.Setup(productName: productName);

                Resource loadPortToDock = new Resource();
                loadPortToDock.Name = loadPortToDockName;
                loadPortToDock.Load();

                materialScenario.DockContainer(loadPortToDock);

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomResourceNoEnoughPositionsOrInUse, resourceName);

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (materialScenario != null)
                {
                    materialScenario.TearDown();
                }
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE SendAdHocTransferInformationToIoT with docked container with capacity but with other products.
        /// Acceptance Criteria: The destination LoadPort must have a match between the given product and the product materials on the docked container.
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_ValidateContainer_DestinationLoadPortDifferentProduct</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_ValidateContainer_DestinationLoadPortDifferentProduct()
        {
            CustomMaterialScenario materialScenario = null;

            try
            {
                materialScenario = new CustomMaterialScenario(setResourceOnline: false, createContainer: true, setResourceOffline: false)
                {
                    AssociateSubMaterialsToContainer = true,
                    NumberOfSubMaterials = 1
                };

                materialScenario.Setup(productName: amsOSRAMConstants.TestProduct);

                Resource loadPortToDock = new Resource();
                loadPortToDock.Name = loadPortToDockName;
                loadPortToDock.Load();

                materialScenario.DockContainer(loadPortToDock);

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomResourceContainerDockedDifferentProducts, resourceName, productName);

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (materialScenario != null)
                {
                    materialScenario.TearDown();
                }
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE SendAdHocTransferInformationToIoT with the docked container without capacity to fullfil the desired quantity.
        /// Acceptance Criteria: The destination LoadPort must have enough free positions to fullfil the desired quantity
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_ValidateContainer_DestinationLoadPortNotEnoughQuantity</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_ValidateContainer_DestinationLoadPortNotEnoughQuantity()
        {
            CustomMaterialScenario materialScenario = null;

            try
            {
                materialScenario = new CustomMaterialScenario(setResourceOnline: false, createContainer: true, setResourceOffline: false)
                {
                    AssociateSubMaterialsToContainer = true,
                    NumberOfSubMaterials = 0
                };
                materialScenario.Setup(productName: productName);

                Resource loadPortToDock1 = new Resource();
                loadPortToDock1.Name = loadPortToDockName;
                loadPortToDock1.Load();

                materialScenario.DockContainer(loadPortToDock1);

                // Set quantity to overflow the container
                int expectedQuantity = (int)actionToExecute.Input["Quantity"] * 2;
                actionToExecute.Input["Quantity"] = expectedQuantity;

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomResourceContainersNoEnoughPositions, expectedQuantity, targetCapacity);

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (materialScenario != null)
                {
                    materialScenario.TearDown();
                }
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE SendAdHocTransferInformationToIoT with the two docked container without capacity to fullfil the desired quantity.
        /// Acceptance Criteria: The destination LoadPorts must have enough free positions to fullfil the desired quantity
        /// </summary>
        /// <TestCaseID>CustomSendAdHocTransferInformationToIoT_ValidateContainer_DestinationLoadPortsNotEnoughQuantity</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomSendAdHocTransferInformationToIoT_ValidateContainer_DestinationLoadPortsNotEnoughQuantity()
        {
            CustomMaterialScenario materialScenarioLP1 = null;
            CustomMaterialScenario materialScenarioLP2 = null;

            try
            {
                materialScenarioLP1 = new CustomMaterialScenario(setResourceOnline: false, createContainer: true, setResourceOffline: false)
                {
                    AssociateSubMaterialsToContainer = true,
                    NumberOfSubMaterials = 0
                };
                materialScenarioLP1.Setup(productName: productName);

                materialScenarioLP2 = new CustomMaterialScenario(setResourceOnline: false, createContainer: true, setResourceOffline: false)
                {
                    AssociateSubMaterialsToContainer = true,
                    NumberOfSubMaterials = 0
                };
                materialScenarioLP2.Setup(productName: productName);

                Resource loadPortToDock1 = new Resource();
                loadPortToDock1.Name = loadPortToDockName;
                loadPortToDock1.Load();

                Resource loadPortToDock2 = new Resource();
                loadPortToDock2.Name = loadPortToDock2Name;
                loadPortToDock2.Load();

                materialScenarioLP1.DockContainer(loadPortToDock1);
                materialScenarioLP2.DockContainer(loadPortToDock2);

                // Set quantity to overflow the containers
                int expectedQuantity = (int)actionToExecute.Input["Quantity"] * 4;
                actionToExecute.Input["Quantity"] = expectedQuantity;

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomResourceContainersNoEnoughPositions, expectedQuantity, targetCapacity * 2);

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (materialScenarioLP1 != null)
                {
                    materialScenarioLP1.TearDown();
                }

                if (materialScenarioLP2 != null)
                {
                    materialScenarioLP2.TearDown();
                }
            }
        }

        private MaterialData RunExecuteAction(ExecuteActionInput actionInput)
        {
            ExecuteActionOutput executeActionOutput = actionInput.ExecuteActionSync();

            Assert.IsTrue(executeActionOutput.Output.ContainsKey("Result"), "Something went wrong with the DEE");

            object result = executeActionOutput.Output.GetValueOrDefault("Result");

            List<MaterialData> materialDataCollection = JsonConvert.DeserializeObject<List<MaterialData>>(result.ToString());
            Assert.AreEqual(1, materialDataCollection.Count, "Must have one MaterialData");

            return materialDataCollection.FirstOrDefault();
        }

        private void ValidateMaterialData(MaterialData materialData, string carrierAtLoadPort, string loadPortPosition)
        {
            Assert.AreEqual(carrierAtLoadPort, materialData.MaterialId, $"The MaterialID should be {carrierAtLoadPort} but got {materialData.MaterialId} instead.");
            Assert.AreEqual(carrierAtLoadPort, materialData.MaterialName, $"The MaterialName should be {carrierAtLoadPort} but got {materialData.MaterialName} instead.");
            Assert.AreEqual(carrierAtLoadPort, materialData.ContainerId, $"The ContainerId should be {carrierAtLoadPort} but got {materialData.ContainerId} instead.");
            Assert.AreEqual(carrierAtLoadPort, materialData.ContainerName, $"The ContainerName should be {carrierAtLoadPort} but got {materialData.ContainerName} instead.");
            Assert.AreEqual(loadPortPosition, materialData.LoadPortPosition, $"The LoadPortPosition should be {loadPortPosition} but got {materialData.LoadPortPosition} instead.");
            Assert.AreEqual("Setup", materialData.MaterialState, $"The MaterialState should be Setup but got {materialData.MaterialState} instead.");
            Assert.IsTrue(materialData.ContainerOnlyProcess, "The ContainerOnlyProcess should be true");
            Assert.IsNotNull(materialData.SorterJobInformation, $"The SorterJobInformation should not be null");
        }

        private void ValidateRecipe(Recipe recipe, RecipeData recipeData)
        {
            Assert.AreEqual(recipe.Id.ToString(), recipeData.RecipeId, $"The RecipeId should be {recipe.Id} but got {recipeData.RecipeId} instead.");
            Assert.AreEqual(recipe.Name, recipeData.RecipeName, $"The RecipeName should be {recipe.Name} but got {recipeData.RecipeName} instead.");
            Assert.AreEqual(recipe.ResourceRecipeName, recipeData.NameOnEquipment, $"The NameOnEquipment should be {recipe.ResourceRecipeName} but got {recipeData.NameOnEquipment} instead.");
            Assert.AreEqual(recipe.BodyChecksum, recipeData.Checksum, $"The Checksum should be {recipe.BodyChecksum} but got {recipeData.Checksum} instead.");

            recipe.LoadRelation("RecipeParameter");
            Assert.AreEqual(recipe.RelationCollection["RecipeParameter"].Count, recipeData.RecipeParameters.Count, $"The Recipe {recipe.Name} has {recipe.RelationCollection["RecipeParameter"].Count} but RecipeDataParameter has {recipeData.RecipeParameters.Count} instead.");

            foreach (RecipeParameter recipeParameter in recipe.RelationCollection["RecipeParameter"])
            {
                RecipeParameterData recipeParameterData = recipeData.RecipeParameters.FirstOrDefault(f => f.Name == recipeParameter.TargetEntity.Name);

                Assert.IsNotNull(recipeParameterData.Name, $"Should exist the parameter {recipeParameter.TargetEntity.Name} on the RecipeDataParameter");
                Assert.AreEqual(Decimal.Truncate((decimal)recipeParameter.Value), Decimal.Truncate(Decimal.Parse(recipeParameterData.Value)), $"The {recipeParameterData.Name} should be {recipeParameter.Value} but is {recipeParameterData.Value} instead");
            }
        }

        private void ValidateCustomSorterJobDefinition(MaterialData materialData)
        {
            string expectedLogicalProcess = "AdHocTransferWafers-WaferReception";

            Assert.AreEqual(expectedLogicalProcess, materialData.SorterJobInformation.LogisticalProcess, $"The SorterJobInformation LogisticalProcess should be {expectedLogicalProcess} but got {materialData.SorterJobInformation.LogisticalProcess} instead.");
            Assert.IsFalse(materialData.SorterJobInformation.FlipWafer, "The SorterJobInformation FlipWafer should be false");
            Assert.IsFalse(materialData.SorterJobInformation.AlignWafer, "The SorterJobInformation AlignWafer should be false");
            Assert.IsTrue(materialData.SorterJobInformation.WaferIdOnBottom, "The SorterJobInformation WaferIdOnBottom should be true");
            Assert.IsTrue(materialData.SorterJobInformation.ReadWaferId, "The SorterJobInformation ReadWaferId should be true");
        }

        private void ValidateMovementList(dynamic movementList, ContainerCollection containers, int quantityToFullfil, int targetCapacity, string carrierAtLoadPort)
        {
            Assert.AreEqual(quantityToFullfil, movementList.Count, $"The MovementList should have {quantityToFullfil}, but got {movementList.Count} instead");

            Dictionary<string, List<int>> mapContainer = new Dictionary<string, List<int>>();

            foreach (Container currentContainer in containers.OrderByDescending(o => o.UsedPositions))
            {
                currentContainer.LoadRelation("MaterialContainer");

                List<int> positionsAvailable = new List<int>();

                for (int i = 1; i <= currentContainer.TotalPositions; i++)
                {
                    if (!currentContainer.ContainerMaterials.Any(c => c.Position == i))
                    {
                        positionsAvailable.Add(i);
                    }
                }

                int maxDestinationPosition = targetCapacity - currentContainer.UsedPositions.Value;

                if (maxDestinationPosition > 0)
                {
                    mapContainer.Add(currentContainer.Name, positionsAvailable.OrderByDescending(o => o).ToList().GetRange(0, maxDestinationPosition));
                }
            }

            int counter = 1;

            foreach (KeyValuePair<string, List<int>> row in mapContainer)
            {
                string expectedContainer = row.Key;

                foreach (int expectedPosition in row.Value)
                {
                    dynamic movement = movementList[counter - 1];

                    string materialName = (string)movement["MaterialName"];
                    string sourceContainer = (string)movement["SourceContainer"];
                    int sourcePosition = (int)movement["SourcePosition"];
                    string destinationContainer = (string)movement["DestinationContainer"];
                    int destinationPosition = (int)movement["DestinationPosition"];

                    Assert.AreEqual(carrierAtLoadPort, sourceContainer, $"The SourceContainer on Movement {counter} should be {carrierAtLoadPort} but got {sourceContainer} instead");
                    Assert.AreEqual(counter, sourcePosition, $"The SourcePosition on Movement {counter} should be {counter} but got {sourcePosition} instead");
                    Assert.AreEqual(expectedContainer, destinationContainer, $"The DestinationContainer on Movement {counter} should be {expectedContainer} but got {destinationContainer} instead");
                    Assert.AreEqual(expectedPosition, destinationPosition, $"The DestinationPosition on Movement {counter} should be {expectedPosition} but got {destinationPosition} instead");

                    counter++;
                }
            }
        }
    }
}
