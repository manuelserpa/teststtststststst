using Cmf.Custom.amsOSRAM.Common.DataStructures;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.OutputObjects;
using Cmf.LightBusinessObjects.Infrastructure.Errors;
using Cmf.Navigo.BusinessObjects;
using Cmf.TestScenarios.MaterialManagement.MaterialScenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Cmf.Custom.Tests.Biz.Materials
{
    [TestClass]
    public class CustomValidateMaterialReceptionSubstrate
    {
        private static CustomExecutionScenario classExecutionScenario = new CustomExecutionScenario();
        private static CustomMaterialScenario classMaterialScenarioDestination = null;

        private static Material GeneratedMaterial = null;
        private static string DEEActionName = "CustomValidateMaterialReceptionSubstrate";

        private static string loadPortDestinationName = amsOSRAMConstants.DefaultSorterLoadPortResourceNames[1];
        private static string flowPathName = amsOSRAMConstants.DefaultTestFlowPath;
        private static string stepName = amsOSRAMConstants.DefaultTestStepName;
        private static string productName = amsOSRAMConstants.DefaultWaferProductName;

        /// <summary>
        /// Test Initialize
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            classExecutionScenario = new CustomExecutionScenario {
                StepAttributesToSet = new Dictionary<string, Foundation.BusinessObjects.AttributeCollection>()
                {
                    {
                        stepName, new Foundation.BusinessObjects.AttributeCollection {
                            { amsOSRAMConstants.StepAttributeIsWaferReception, true }
                        }
                    }
                },
                NumberOfMaterialsToGenerate = 1,
                ProductName = productName,
                FlowPath = flowPathName
            };

            classExecutionScenario.Setup();

            GeneratedMaterial = classExecutionScenario.GeneratedLots.FirstOrDefault();

            Resource loadPortDestination = new Resource();
            loadPortDestination.Name = loadPortDestinationName;

            classMaterialScenarioDestination = new CustomMaterialScenario(setResourceOnline: false, createContainer: true, setResourceOffline: false)
            {
                AssociateSubMaterialsToContainer = false,
                Resource = loadPortDestination,
                StepName = stepName
            };

            classMaterialScenarioDestination.Setup(productName: productName, positionSorting: ContainerPositionSorting.Descending);
        }

        /// <summary>
        /// Test Initialize
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (classExecutionScenario != null)
            {
                classExecutionScenario.CompleteCleanUp();
            }

            if (classMaterialScenarioDestination != null)
            {
                classMaterialScenarioDestination.TearDown();
            }
        }

        /// <summary>
        /// Description: Validates DEE CustomValidateMaterialReceptionSubstrate that does not throw any expection when all conditions are met
        /// Acceptance Criteria:
        ///   - Wafer ID is available and registered in MES
        ///   - Wafer is at wafer reception step and proceed with substrate
        ///   - Wafer is active and not on hold
        ///   - Wafer's product is the same as existing placed wafer's product in the intended destination container
        /// </summary>
        /// <TestCaseID>CustomValidateMaterialReceptionSubstrate_HappyPath</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomValidateMaterialReceptionSubstrate_HappyPath()
        {
            Dictionary<string, object> initialInput = new Dictionary<string, object>()
            {
                { "WaferID", GeneratedMaterial.Name },
                { "TargetContainer", classMaterialScenarioDestination.ContainerScenario.Entity.Name }
            };

            ExecuteActionInput actionToExecute = new ExecuteActionInput()
            {
                Action = new Foundation.Common.DynamicExecutionEngine.Action
                {
                    Name = DEEActionName
                },
                Input = new Dictionary<string, object>(initialInput)
            };

            actionToExecute.ExecuteActionSync();
        }

        [TestMethod]
        public void CustomValidateMaterialReceptionSubstrate_ValidateMaterial_NotActive()
        {
            CustomExecutionScenario customExecutionScenario = null;

            try
            {
                // Create a new material to be not active
                customExecutionScenario = new CustomExecutionScenario()
                {
                    NumberOfMaterialsToGenerate = 1,
                    ProductName = productName,
                    FlowPath = flowPathName
                };

                customExecutionScenario.Setup();

                Material wafer = customExecutionScenario.GeneratedLots.FirstOrDefault();
                wafer.SpecialTerminate();

                wafer.Load();

                Dictionary<string, object> initialInput = new Dictionary<string, object>()
                {
                    { "WaferID", wafer.Name },
                    { "TargetContainer", classMaterialScenarioDestination.ContainerScenario.Entity.Name }
                };

                ExecuteActionInput actionToExecute = new ExecuteActionInput()
                {
                    Action = new Foundation.Common.DynamicExecutionEngine.Action
                    {
                        Name = DEEActionName
                    },
                    Input = new Dictionary<string, object>(initialInput)
                };

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = $"The object {wafer.Name} is not active";

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (customExecutionScenario != null)
                {
                    customExecutionScenario.CompleteCleanUp();
                }
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE CustomValidateMaterialReceptionSubstrate when the material is on a Step that is not marked as Wafer Reception
        /// Acceptance Criteria: Step must not be marked as Wafer Reception. The attribute IsWaferReception set to true
        /// </summary>
        /// <TestCaseID>CustomValidateMaterialReceptionSubstrate_ValidateMaterial_OnHold</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomValidateMaterialReceptionSubstrate_ValidateMaterial_OnHold()
        {
            CustomExecutionScenario customExecutionScenario = null;

            try
            {
                // Create new material to be OnHold
                customExecutionScenario = new CustomExecutionScenario()
                {
                    NumberOfMaterialsToGenerate = 1,
                    ProductName = productName,
                    FlowPath = flowPathName
                };

                customExecutionScenario.Setup();

                Material wafer = customExecutionScenario.GeneratedLots.FirstOrDefault();

                Reason reason = new Reason() { Name = "Out Of Spec" };
                reason.Load();

                wafer.HoldMaterial(reason);
                wafer.Load();

                Dictionary<string, object> initialInput = new Dictionary<string, object>()
                {
                    { "WaferID", wafer.Name },
                    { "TargetContainer", classMaterialScenarioDestination.ContainerScenario.Entity.Name }
                };

                ExecuteActionInput actionToExecute = new ExecuteActionInput()
                {
                    Action = new Foundation.Common.DynamicExecutionEngine.Action
                    {
                        Name = DEEActionName
                    },
                    Input = new Dictionary<string, object>(initialInput)
                };

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = $"Material {wafer.Name} is on Hold";

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (customExecutionScenario != null)
                {
                    customExecutionScenario.CompleteCleanUp();
                }
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE CustomValidateMaterialReceptionSubstrate when the material is on a Step that is not marked as Wafer Reception
        /// Acceptance Criteria: Step must not be marked as Wafer Reception. The attribute IsWaferReception set to true
        /// </summary>
        /// <TestCaseID>CustomValidateMaterialReceptionSubstrate_ValidateStep_NotWaferReception</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomValidateMaterialReceptionSubstrate_ValidateStep_NotWaferReception()
        {
            CustomExecutionScenario executationScenario = new CustomExecutionScenario();

            try
            {
                // Set StepAttributeIsWaferReception to false
                executationScenario.StepAttributesToSet = new Dictionary<string, Foundation.BusinessObjects.AttributeCollection>()
                {
                    {
                        stepName, new Foundation.BusinessObjects.AttributeCollection {
                            { amsOSRAMConstants.StepAttributeIsWaferReception, false }
                        }
                    }
                };
                executationScenario.Setup();

                Dictionary<string, object> initialInput = new Dictionary<string, object>()
                {
                    { "WaferID", GeneratedMaterial.Name },
                    { "TargetContainer", classMaterialScenarioDestination.ContainerScenario.Entity.Name }
                };

                ExecuteActionInput actionToExecute = new ExecuteActionInput()
                {
                    Action = new Foundation.Common.DynamicExecutionEngine.Action
                    {
                        Name = DEEActionName
                    },
                    Input = new Dictionary<string, object>(initialInput)
                };

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomStepNoWaferReception,
                                                                                GeneratedMaterial.Step.Name, amsOSRAMConstants.StepAttributeIsWaferReception);

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                executationScenario.CompleteCleanUp();
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE CustomValidateMaterialReceptionSubstrate with a docked container with different products
        /// Acceptance Criteria: The destination LoadPort must have a match between the given product and the product materials on the docked container.
        /// </summary>
        /// <TestCaseID>CustomValidateMaterialReceptionSubstrate_ValidateContainer_DifferentProduct</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomValidateMaterialReceptionSubstrate_ValidateContainer_DifferentProduct()
        {
            CustomMaterialScenario customMaterialScenario = null;

            try
            {                
                // Creates a new container with different product associated
                customMaterialScenario = new CustomMaterialScenario(setResourceOnline: false, createContainer: true, setResourceOffline: false)
                {
                    AssociateSubMaterialsToContainer = true,
                    NumberOfSubMaterials = 1
                };
                customMaterialScenario.Setup(productName: amsOSRAMConstants.DefaultProductName, positionSorting: ContainerPositionSorting.Descending);

                Dictionary<string, object> initialInput = new Dictionary<string, object>()
                {
                    { "WaferID", GeneratedMaterial.Name },
                    { "TargetContainer", customMaterialScenario.ContainerScenario.Entity.Name }
                };

                ExecuteActionInput actionToExecute = new ExecuteActionInput()
                {
                    Action = new Foundation.Common.DynamicExecutionEngine.Action
                    {
                        Name = DEEActionName
                    },
                    Input = new Dictionary<string, object>(initialInput)
                };

                CmfFaultException inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());

                string errorMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageCustomContainerDifferentProducts,
                                                                customMaterialScenario.ContainerScenario.Entity.Name, GeneratedMaterial.Product.Name);

                Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (customMaterialScenario != null)
                {
                    customMaterialScenario.Terminate();
                }
            }
        }

        /// <summary>
        /// Description: Validates error message sent by DEE CustomValidateMaterialReceptionSubstrate with invalid and missing inputs
        /// Acceptance Criteria: Inputs must be valid and entities must exist in the system
        /// </summary>
        /// <TestCaseID>CustomValidateMaterialReceptionSubstrate_ValidateInput_MissingInvalidInput</TestCaseID>
        /// <Author>Oliverio Sousa</Author>
        [TestMethod]
        public void CustomValidateMaterialReceptionSubstrate_ValidateInput_MissingInvalidInput()
        {
            #region Validate missing inputs

            CmfFaultException inputCmfFaultException;
            Dictionary<string, object> newInput;

            Dictionary<string, object> initialInput = new Dictionary<string, object>()
            {
                { "WaferID", GeneratedMaterial.Name },
                { "TargetContainer", classMaterialScenarioDestination.ContainerScenario.Entity.Name }
            };

            ExecuteActionInput actionToExecute = new ExecuteActionInput()
            {
                Action = new Foundation.Common.DynamicExecutionEngine.Action
                {
                    Name = DEEActionName
                },
                Input = new Dictionary<string, object>(initialInput)
            };

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

            string inputName = "WaferID";
            newInput = new Dictionary<string, object>(initialInput);
            newInput[inputName] = CustomUtilities.GenerateName($"Random_{inputName}");

            actionToExecute.Input = newInput;
            errorMessage = $"Material {newInput[inputName]} was not found";

            inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());
            Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");

            actionToExecute.Input = initialInput;

            inputName = "TargetContainer";
            newInput = new Dictionary<string, object>(initialInput);
            newInput[inputName] = CustomUtilities.GenerateName($"Random_{inputName}");

            actionToExecute.Input = newInput;
            errorMessage = $"Container {newInput[inputName]} was not found";

            inputCmfFaultException = Assert.ThrowsException<CmfFaultException>(() => actionToExecute.ExecuteActionSync());
            Assert.IsTrue(inputCmfFaultException.Message.Contains(errorMessage), $"Should have the following error message: {errorMessage}");

            actionToExecute.Input = initialInput;

            #endregion Validate if inputs exist on MES
        }
    }
}
