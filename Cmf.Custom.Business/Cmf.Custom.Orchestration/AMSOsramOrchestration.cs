using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.AMSOsram.Orchestration.OutputObjects;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Integration;
using System;
using System.Collections.Generic;
using System.Text;

using Cmf.Common.CustomActionUtilities;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.AMSOsram.Orchestration.OutputObjects;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Cultures;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmf.Custom.AMSOsram.Orchestration
{
    public static class AMSOsramOrchestration
    {
        private const string OBJECT_TYPE_NAME = "Cmf.Custom.AMSOsram.Orchestration.AMSOsramManagementOrchestration";

        /// <summary>
        /// Service to generate an Integration Entry based on the received Stibo message
        /// </summary>
        /// <param name="input">Input Object</param>
        /// <returns>Output Object</returns>
        /// <exception cref="Cmf.Foundation.Common.CmfBaseException">If any unexpected error occurs.</exception>
        public static CustomReceiveStiboMessageOutput CustomReceiveStiboMessage(CustomReceiveStiboMessageInput input)
        {
            Utilities.StartMethod(
                OBJECT_TYPE_NAME,
                "CustomReceiveStiboMessage",
                new KeyValuePair<string, object>("CustomReceiveStiboMessageInput", input));

            CustomReceiveStiboMessageOutput output = new CustomReceiveStiboMessageOutput();

            try
            {
                Utilities.ValidateNullInput(input);

                if (!string.IsNullOrEmpty(input.Message))
                {
                    // Set Integration Entry properties
                    IntegrationEntry integrationEntry = new IntegrationEntry
                    {
                        Name = Guid.NewGuid().ToString("N"),
                        EventName = AMSOsramConstants.CustomIntegrationInboundEventName,
                        SourceSystem = AMSOsramConstants.CustomStiboSystem,
                        TargetSystem = Constants.MesSystemDesignation,
                        MessageType = input.MessageType,
                        IntegrationMessage = new IntegrationMessage
                        {
                            Message = Encoding.Default.GetBytes(input.Message)
                        },
                        SystemState = IntegrationEntrySystemState.Received,
                        MessageDate = DateTime.Now
                    };

                    // Create Integration Entry and Set to Result
                    output.Result = GenericServiceManagementOrchestration.CreateObject(
                        createObjectInput: new CreateObjectInput
                        {
                            Object = integrationEntry
                        }).Object as IntegrationEntry;
                }
                else
                {
                    throw new Exception(LocalizedMessage.GetLocalizedMessage(messageName: AMSOsramConstants.CustomReceiveEmptyMessage).MessageText);
                }

                Utilities.EndMethod(
                    -1,
                    -1,
                    new KeyValuePair<string, object>("CustomReceiveStiboMessageInput", input),
                    new KeyValuePair<string, object>("CustomReceiveStiboMessageOutput", output));
            }
            catch (CmfBaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CmfBaseException(ex.Message, ex);
            }

            return output;
        }

        /// <summary>
        /// Service to generate an Integration Entry based on the received message
        /// </summary>
        /// <param name="input">CustomReceiveERPMessage Input</param>
        /// <returns>CustomReceiveERPMessage Output</returns>
        /// <exception cref="Cmf.Foundation.Common.CmfBaseException">If any unexpected error occurs.</exception>
        public static CustomReceiveERPMessageOutput CustomReceiveERPMessage(CustomReceiveERPMessageInput input)
        {

            Utilities.StartMethod(
                OBJECT_TYPE_NAME,
                "CustomReceiveERPMessage",
                new KeyValuePair<string, object>("CustomReceiveERPMessageInput", input));

            CustomReceiveERPMessageOutput output = new CustomReceiveERPMessageOutput();

            try
            {
                Utilities.ValidateNullInput(input);

                if (!string.IsNullOrEmpty(input.Message))
                {
                    // create Integration Entry
                    IntegrationEntry integrationEntry = new IntegrationEntry
                    {
                        Name = Guid.NewGuid().ToString("N"),
                        EventName = AMSOsramConstants.CustomIntegrationInboundEventName,
                        SourceSystem = AMSOsramConstants.CustomERPSystem,
                        TargetSystem = Constants.MesSystemDesignation,
                        MessageType = input.MessageType,
                        IntegrationMessage = new IntegrationMessage
                        {
                            Message = Encoding.Default.GetBytes(input.Message)
                        },
                        SystemState = IntegrationEntrySystemState.Received,
                        MessageDate = DateTime.Now
                    };

                    // update output
                    output.Result = GenericServiceManagementOrchestration.CreateObject(new CreateObjectInput
                    {
                        Object = integrationEntry
                    }).Object as IntegrationEntry;

                }
                else
                {
                    throw new Exception(LocalizedMessage.GetLocalizedMessage(AMSOsramConstants.CustomReceiveEmptyMessage).MessageText); 
                }

                Utilities.EndMethod(
                    -1,
                    -1,
                    new KeyValuePair<string, object>("CustomReceiveERPMessageInput", input),
                    new KeyValuePair<string, object>("CustomReceiveERPMessageOutput", output));
            }
            catch (CmfBaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CmfBaseException(ex.Message, ex);
            }

            return output;
        }
    }
}
