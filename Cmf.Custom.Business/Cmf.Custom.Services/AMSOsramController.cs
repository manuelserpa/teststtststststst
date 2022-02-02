using Cmf.Custom.AMSOsram.Orchestration;
using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.AMSOsram.Orchestration.OutputObjects;
using Cmf.Foundation.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Cmf.Custom.AMSOsram.Services
{
    /// <summary>
    /// AMSOsram Services
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AMSOsramController : ControllerBase
    {
        private const string OBJECT_TYPE_NAME = "Cmf.Custom.AMSOsram.Services.AMSOsramManagement";

        /// <summary>
        /// CustomReceiveERPMessage
        /// </summary>
        /// <param name="input">CustomReceiveERPMessage Input</param>
        /// <returns>CustomReceiveERPMessage Output</returns>
        /// <exception cref="Cmf.Foundation.Common.CmfBaseException">If any unexpected error occurs.</exception>
        [HttpPost()]
        public CustomReceiveERPMessageOutput CustomReceiveERPMessage(CustomReceiveERPMessageInput input)
        {
            Utilities.StartMethod(
                    OBJECT_TYPE_NAME,
                    "CustomReceiveERPMessage",
                    new KeyValuePair<string, object>("CustomReceiveERPMessageInput", input));

            CustomReceiveERPMessageOutput output = null;
            try
            {
                output = AMSOsramOrchestration.CustomReceiveERPMessage(input);

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
            catch (Exception excep)
            {
                throw new CmfBaseException(excep.Message, excep);
            }

            return output;
        }
    }
}