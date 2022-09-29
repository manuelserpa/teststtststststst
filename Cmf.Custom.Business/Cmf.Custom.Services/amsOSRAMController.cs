using Cmf.Custom.amsOSRAM.Orchestration.Abstractions;
using Cmf.Custom.amsOSRAM.Orchestration.InputObjects;
using Cmf.Custom.amsOSRAM.Orchestration.OutputObjects;
using Cmf.Foundation.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
namespace Cmf.Custom.amsOSRAM.Services
{
    /// <summary>
    /// amsOSRAM Services
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class amsOSRAMController : ControllerBase
    {
        // Orchestrations
        private readonly IamsOSRAMManagementOrchestration _amsOSRAMManagementOrchestration;

        // Constants
        private const string OBJECT_TYPE_NAME = "Cmf.Custom.amsOSRAM.Services.amsOSRAMManagement";

        /// <summary>
        /// Initializes a new instance of the <see cref="amsOSRAMController"/> class.
        /// </summary>
        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public amsOSRAMController(IamsOSRAMManagementOrchestration amsOSRAMManagementOrchestration) : base()
        {
            this._amsOSRAMManagementOrchestration = amsOSRAMManagementOrchestration;
        }

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
                output = _amsOSRAMManagementOrchestration.MaterialIn(input);
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
        [HttpPost()]
        public MaterialOutOutput MaterialOut(MaterialOutInput input)
        {
            MaterialOutOutput output;
            Utilities.StartMethod(OBJECT_TYPE_NAME, "MaterialOut",
                  new KeyValuePair<String, Object>("MaterialOutInput", input));
            try
            {
                output = _amsOSRAMManagementOrchestration.MaterialOut(input);
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
                output = _amsOSRAMManagementOrchestration.CustomReceiveStiboMessage(input: input);
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
                output = _amsOSRAMManagementOrchestration.CustomReceiveERPMessage(input);
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
                output = _amsOSRAMManagementOrchestration.GetFlowInformationForERP(input);

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
