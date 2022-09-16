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
        /// MaterialIn
        /// </summary>
        /// <param name="input">MaterialIn Input Object</param>
        /// <returns>MaterialIn Output Object</returns>
        [HttpPost]
        public MaterialInOutput MaterialIn(MaterialInInput input)
        {
            MaterialInOutput output;

            Utilities.StartMethod(OBJECT_TYPE_NAME, "MaterialIn",
            new KeyValuePair<String, Object>("MaterialInInput", input));
            try
            {
                output = AMSOsramOrchestration.MaterialIn(input);

                Utilities.EndMethod(-1, -1,
                new KeyValuePair<String, Object>("MaterialInInput", input),
                new KeyValuePair<String, Object>("MaterialInOutput", output));
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
        /// MaterialOut
        /// </summary>
        /// <param name="input">MaterialOut Input Object</param>
        /// <returns>MaterialOut Output Object</returns>
        [HttpPost]
        public MaterialOutOutput MaterialOut(MaterialOutInput input)
        {
            MaterialOutOutput output;

            Utilities.StartMethod(OBJECT_TYPE_NAME, "MaterialOut",
                  new KeyValuePair<String, Object>("MaterialOutInput", input));
            try
            {
                output = AMSOsramOrchestration.MaterialOut(input);

                Utilities.EndMethod(-1, -1,
                  new KeyValuePair<String, Object>("MaterialOutInput", input),
                  new KeyValuePair<String, Object>("MaterialOutOutput", output));
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
        /// CustomReceiveStiboMessage
        /// </summary>
        /// <param name="input">CustomReceiveStiboMessage Input</param>
        /// <returns>CustomReceiveStiboMessage Output</returns>
        /// <exception cref="Cmf.Foundation.Common.CmfBaseException">If any unexpected error occurs.</exception>
        [HttpPost()]
        public CustomReceiveStiboMessageOutput CustomReceiveStiboMessage(CustomReceiveStiboMessageInput input)
        {
            Utilities.StartMethod(
                    OBJECT_TYPE_NAME,
                    "CustomReceiveStiboMessage",
                    new KeyValuePair<string, object>("CustomReceiveStiboMessageInput", input));

            CustomReceiveStiboMessageOutput output = null;

            try
            {
                output = AMSOsramOrchestration.CustomReceiveStiboMessage(input: input);

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
            catch (Exception excep)
            {
                throw new CmfBaseException(excep.Message, excep);
            }

            return output;
        }

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

        /// <summary>
        /// GetFlowInformationForERP
        /// </summary>
        /// <param name="input">CustomGetFlowInformationForERP Input Object</param>
        /// <returns>CustomGetFlowInformationForERP Output Object</returns>
        /// <exception cref="Cmf.Foundation.Common.CmfBaseException">If any unexpected error occurs.</exception>
        [HttpPost()]
        public CustomGetFlowInformationForERPOutput GetFlowInformationForERP(CustomGetFlowInformationForERPInput input)
        {
            Utilities.StartMethod(OBJECT_TYPE_NAME, "CustomGetFlowInformationForERP",
                                  new KeyValuePair<string, object>("CustomGetFlowInformationForERPInput", input));

            CustomGetFlowInformationForERPOutput output = null;

            try
            {
                output = AMSOsramOrchestration.GetFlowInformationForERP(input);

                Utilities.EndMethod(-1, -1,
                                    new KeyValuePair<string, object>("CustomGetFlowInformationForERPInput", input),
                                    new KeyValuePair<string, object>("CustomGetFlowInformationForERPOutput", output));
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