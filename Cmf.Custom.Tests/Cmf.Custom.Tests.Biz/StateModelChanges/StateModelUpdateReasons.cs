using Cmf.Foundation.BusinessObjects;
using Cmf.TestScenarios.Others;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.StateModelChanges
{
    [TestClass]
    public class StateModelUpdateReasons
    {

        /// <summary>
        /// Description:
        ///     - Go to the SEMI-E10 state model
        ///     - Open the transitions for the same state 
        ///     
        /// Acceptance Criteria:
        ///     - UnscheduleToUnschedule - Default Reason = UDM - Waiting for Maintenance
        ///     - NonscheduleToNonschedule - Default Reason = NSU - Unworked Time
        ///     - ScheduledToScheduled - Default Reason = SDP - Preventive Maintenance
        ///     - EngineeringToEngineering - Default Reason = ENE - Engineering
        ///     - StandByToStandBy - Default Reason = SBI - Idle
        ///     - ProductiveToProductive - Default Reason = PRP - Production
        ///     
        /// </summary>
        /// <TestCaseID>StateModelUpdateReasons.StateModelUpdateReasons_CheckDefaultReasons_Happypath</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void StateModelUpdateReasons_CheckDefaultReasons_Happypath()
        {
            Dictionary<string, string[]> expectedReasons = new Dictionary<string, string[]>()
            {
                { "Unscheduled Down", new string[] { "ReasonsUnscheduledDownToUnscheduledDown", "UDM" }},
                { "Nonscheduled", new string[] { "ReasonsNonscheduledToNonscheduled", "NSU" }},
                { "Scheduled Down", new string[] { "ReasonsScheduledDownToScheduledDown", "SDP" }},
                { "Engineering", new string[] { "ReasonsEngineeringToEngineering", "ENE" }},
                { "Standby", new string[] { "ReasonsStandbyToStandby", "SBI" }},
                { "Productive", new string[] { "ReasonsProductiveToProductive","PRP" }}
            };

            ///<Step> Go to the State model SEMI E10 and its transitions </Step>
            StateModel stateModel = GenericGetsScenario.GetObjectByName<StateModel>("SEMI E10");
            StateModelTransitionCollection transitionsToSameState = new StateModelTransitionCollection();
            transitionsToSameState.AddRange(stateModel.StateTransitions.Where(st => st.FromState.Name == st.ToState.Name));

            ///<ExpedtedResult> Validate the State Transitions for the same state according to the Acceptance Criteria </ExpedtedResult>
            foreach (StateModelTransition transition in transitionsToSameState)
            {
                string defaultReasonLT = transition.ReasonLookupTable;
                string defaultReasonValue = transition.ReasonDefaultValue;
                
                Assert.IsTrue(expectedReasons[transition.ToState.Name][0].Equals(defaultReasonLT), $"The transition from state {transition.FromState.Name} to {transition.ToState.Name} should have the default Lookup Table {expectedReasons[transition.ToState.Name][0]}, instead has {defaultReasonLT}.");
                Assert.IsTrue(expectedReasons[transition.ToState.Name][1].Equals(defaultReasonValue), $"The transition from state {transition.FromState.Name} to {transition.ToState.Name} should have the default Lookup Table {expectedReasons[transition.ToState.Name][1]}, instead has {defaultReasonValue}.");
            }
        }
    }
}
