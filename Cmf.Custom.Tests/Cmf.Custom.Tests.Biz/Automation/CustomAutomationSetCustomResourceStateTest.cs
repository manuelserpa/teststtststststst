using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.TestScenarios.ResourceManagement.ResourceScenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Cmf.Custom.Tests.Biz.Automation
{
    [TestClass]
    public class CustomAutomationSetCustomResourceState
    {
        private const string customAutomationSetCustomResourceStateDEE = "CustomAutomationSetCustomResourceState";
        private const string constResourceName = "ResourceName";
        private const string constStateName = "StateName";
        private const string constStateModelName = "StateModelName";
        private const string constChamberResourceNumber = "ChamberResourceNumber";
        private const string resourceName = "5FASH1";
        private const string resourceChamberName = "5FASH1-PM01";
        private const string stateNameIdle = "Idle";
        private const string stateNameBusy = "Busy";
        private const string stateModelName = "CustomEquipmentPerformanceTrackingStateModel";

        /// <summary>
        /// Description: This test validates the adjustment of the secondary resource state after a successful call of CustomAutomationSetCustomResourceState
        /// </summary>
        /// <TestCaseID>CustomAutomationSetCustomResourceState_Success</TestCaseID>
        /// <Author>Thomas Galler</Author>
        [TestMethod]
        public void CustomAutomationSetCustomResourceState_Success()
        {
            string beforeState = String.Empty;
            string afterState = String.Empty;

            // Check if test resource exists in the system and proceed
            Resource resource = new Resource();
            resource.Name = resourceName;
            resource.Load();
            if (resource.ObjectExists())
            {
                // Set secondary state to busy
                AdjustAutomationState(resourceName, stateNameBusy, stateModelName);
                beforeState = GetSecondaryResourceState(resourceChamberName);

                // Set secondary state to idle
                AdjustAutomationState(resourceName, stateNameIdle, stateModelName);
                afterState = GetSecondaryResourceState(resourceChamberName);

                Assert.AreNotEqual(beforeState, afterState, "The secondary state of the chamber was not adjusted correctly.");
            }
            else
            {
                Assert.Fail("Resource '{0}' is missing in the system! Check Test MasterData!", resource.Name);
            }
        }

        #region Help Methods

        /// <summary>
        /// Returns the resources states
        /// </summary>
        /// <returns>string states</returns>
        private string GetSecondaryResourceState(string resourceName)
        {

            // Creates the scenario for Resource
            ResourceBOScenario resourceBOScenario = new ResourceBOScenario();
            resourceBOScenario.Entity.Name = resourceName;
            resourceBOScenario.Entity.Load();
            resourceBOScenario.LoadStateModels();

            String states = String.Format($"All current states for Resource {resourceName}:");
            foreach (var s in resourceBOScenario.Entity.CurrentStates)
            {
                states = String.Format($"{states}\n State: {s.CurrentState.Name} - State Model: {s.StateModel.Name}");
            }

            return states;
        }

        /// <summary>
        /// Adjusts the resource state by executing CustomAutomationSetCustomResourceState
        /// </summary>
        private void AdjustAutomationState(string resourceName, string stateName, string stateModelName)
        {
            // Create Input for DEE call
            Dictionary<string, object> inputDee = new Dictionary<string, object>
                {
                    { constResourceName, resourceName },
                    { constStateName, stateName },
                    { constStateModelName, stateModelName},
                    { constChamberResourceNumber, "1" }
                };
            // Get CustomAutomationSetCustomResourceState DEE
            Foundation.Common.DynamicExecutionEngine.Action deeAction = new GetActionByNameInput()
            {
                Name = customAutomationSetCustomResourceStateDEE
            }.GetActionByNameSync().Action;

            // Call CustomAutomationSetCustomResourceState DEE
            new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput()
            {
                Action = deeAction,
                Input = inputDee
            }.ExecuteActionSync();
        }

        #endregion
    }
}
