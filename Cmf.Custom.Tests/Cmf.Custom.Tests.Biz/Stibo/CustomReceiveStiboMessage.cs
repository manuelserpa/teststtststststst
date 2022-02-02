using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.AMSOsram.Orchestration.OutputObjects;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Foundation.Common.Integration;
using Cmf.LightBusinessObjects.Infrastructure.Errors;
using Cmf.TestScenarios.Others;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.Tests.Biz.Stibo
{
    [TestClass]
    public class CustomReceiveStiboMessage
    {
        /// <summary>
        /// Description:
        ///     - Send an Stibo Message to the custom service CustomReceiveStiboMessage
        /// 
        /// Acceptance Citeria:
        ///     - Integration Entry created containing the Stibo Message
        /// 
        /// </summary>
        /// <TestCaseID>CustomReceiveStiboMessage.CustomReceiveStiboMessage_SendStiboMessage_IntegrationEntryCreated</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomReceiveStiboMessage_SendStiboMessage_IntegrationEntryCreated()
        {
            CustomReceiveStiboMessageOutput output = new CustomReceiveStiboMessageOutput();

            try
            {
                /// <Step>
                /// Send mock XML message
                /// </Step>
                string message = @$"<root><Facility>Facility</Facility><Area>Area</Area><Material>Material</Material></root>";
                string messageType = "MessageType";

                CustomReceiveStiboMessageInput input = new CustomReceiveStiboMessageInput
                {
                    Message = message,
                    MessageType = messageType
                };

                output = input.CustomReceiveStiboMessageSync();

                output.Result.IntegrationMessage = GenericGetsScenario.GetObjectByName<IntegrationMessage>(output.Result.Name + "Message");

                string ieMessage = Encoding.UTF8.GetString(output.Result.IntegrationMessage.Message);

                /// <ExpectedResult>
                /// Send message from Stibo to MES
                /// </ExpectedResult>
                Assert.IsTrue(message.Equals(ieMessage));
                Assert.IsTrue(output.Result.SourceSystem.Equals("Stibo"), $"Integration Entry Source System should be Stibo instead is {output.Result.SourceSystem}.");
                Assert.IsTrue(output.Result.TargetSystem.Equals("MES"), $"Integration Entry Source System should be MES instead is {output.Result.TargetSystem}.");
                Assert.IsTrue(output.Result.SystemState.Equals(IntegrationEntrySystemState.Received), $"Integration Entry System State should be {IntegrationEntrySystemState.Received} instead is {output.Result.SystemState}.");
            }
            finally
            {
                /// <Step>
                /// Clear created Integration Entry
                /// </Step>
                if (output.Result != null)
                {
                    /// <ExpectedResult>
                    /// Integration Entry is terminated
                    /// </ExpectedResult>
                    new MarkIntegrationEntriesAsFailedInput
                    {
                        IntegrationEntryIds = new List<long>()
                        {
                            output.Result.Id
                        }
                    }.MarkIntegrationAsFailedAsync();
                }
            }
        }

        /// <summary>
        /// Description:
        ///     - Send an Stibo Message to the custom service CustomReceiveStiboMessage with an empty message
        /// 
        /// Acceptance Citeria:
        ///     - Integration Entry not Created
        /// </summary>
        /// <TestCaseID>CustomReceiveStiboMessage.CustomReceiveStiboMessage_SendStiboMessage_ThrowErrorMessage</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomReceiveStiboMessage_SendStiboMessage_ThrowErrorMessage()
        {
            try
            {
                /// <Step>
                /// Send empty message
                /// </Step>
                string message = null;
                string messageType = "MessageType";

                CustomReceiveStiboMessageInput input = new CustomReceiveStiboMessageInput
                {
                    Message = message,
                    MessageType = messageType
                };

                string expectedErrorMessage = "Received message from Stibo is empty.";

                CmfFaultException emptyMessageException = Assert.ThrowsException<CmfFaultException>(() => input.CustomReceiveStiboMessageSync());

                /// <ExpectedResult>
                /// Message should be presented
                /// </ExpectedResult>
                Assert.IsTrue(emptyMessageException.Message.Contains(expectedErrorMessage), $"Error message should be {expectedErrorMessage}, instead is {emptyMessageException.Message}");
            }
            finally
            {

            }
        }
    }
}
