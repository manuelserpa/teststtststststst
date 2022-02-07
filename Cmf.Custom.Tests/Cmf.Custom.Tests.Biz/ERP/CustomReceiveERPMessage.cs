using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.AMSOsram.Orchestration.OutputObjects;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Foundation.Common.Integration;
using Cmf.LightBusinessObjects.Infrastructure.Errors;
using Cmf.TestScenarios.Others;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.Tests.Biz.ERP
{
    [TestClass]
    public class CustomReceiveERPMessage
    {

        /// <summary>
        /// Description:
        ///     - Send an ERP Message to the custom service CustomReceiveERPMessage
        ///     
        /// Acceptance Criteria:
        ///     - Integration Entry created containing the ERP Message
        ///     
        /// </summary>
        /// <TestCaseID>CustomReceiveERPMessage.CustomReceiveERPMessage_SendERPMessage_IntegrationEntryCreated</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomReceiveERPMessage_SendERPMessage_IntegrationEntryCreated()
        {
            CustomReceiveERPMessageOutput output = new CustomReceiveERPMessageOutput();
            try
            {
                ///<Step> Send message to MES </Step>
                string message = @$"<root>
                                    <facility>Test Facility</facility>
                                    <area>Test Area</area>
                                    <material>Test Material</material>
                                </root>";
                string messageType = "TestMessageType";

                CustomReceiveERPMessageInput input = new CustomReceiveERPMessageInput()
                {
                    MessageType = messageType,
                    Message = message
                };

                output = input.CustomReceiveERPMessageSync();

                output.Result.IntegrationMessage = GenericGetsScenario.GetObjectByName<IntegrationMessage>(output.Result.Name + "Message");

                string ieMessage = Encoding.UTF8.GetString(output.Result.IntegrationMessage.Message);

                output.Result.Load();

                ///<ExpectedResult> Error message should be presented </ExpectedResult>
                Assert.IsTrue(message.Equals(ieMessage));
                Assert.IsTrue(output.Result.SourceSystem.Equals("ERP"), $"Integration Entry Source System should be ERP instead is {output.Result.SourceSystem}.");
                Assert.IsTrue(output.Result.TargetSystem.Equals("MES"), $"Integration Entry Source System should be MES instead is {output.Result.TargetSystem}.");
                Assert.IsTrue(output.Result.SystemState.Equals(IntegrationEntrySystemState.Received), $"Integration Entry System State should be Received instead is {output.Result.SystemState}.");
            }
            finally
            {
                ///<Step> Clear created Integration Entry </Step>
                if (output.Result != null)
                {
                    ///<ExpectedResult> Integration Entry is terminated </ExpectedResult>
                    new MarkIntegrationEntriesAsFailedInput
                    {
                        IntegrationEntryIds = new List<long>() { output.Result.Id }
                    }.MarkIntegrationAsFailedSync();
                }
            }
        }

        /// <summary>
        /// Description:
        ///     - Send an ERP Message to the custom service CustomReceiveERPMessage with an empty message
        ///     
        /// Acceptance Criteria:
        ///     - Integration Entry not created 
        ///     
        /// </summary>
        /// <TestCaseID>CustomReceiveERPMessage.CustomReceiveERPMessage_SendERPEmptyMessage_ThrowErrorMessage</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomReceiveERPMessage_SendERPEmptyMessage_ThrowErrorMessage()
        {
            ///<Step> Send empty message </Step>
            string message = null;
            string messageType = "TestMessageType";

            CustomReceiveERPMessageInput input = new CustomReceiveERPMessageInput()
            {
                MessageType = messageType,
                Message = message
            };
            string expectedErrorMessage = "Message Received is empty.";
            CmfFaultException emptyMessageException = Assert.ThrowsException<CmfFaultException>(() => input.CustomReceiveERPMessageSync());

            ///<ExpectedResult> Error message should be presented </ExpectedResult>
            Assert.IsTrue(emptyMessageException.Message.Contains(expectedErrorMessage), $"Error message should be {expectedErrorMessage}, instead is {emptyMessageException.Message}");
        }

    }
}
