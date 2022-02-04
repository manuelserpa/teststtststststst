﻿using System;
using System.Collections.Generic;
using Cmf.Custom.AMSOsram.Orchestration;
using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.AMSOsram.Orchestration.OutputObjects;
using Cmf.Foundation.Common;
using Microsoft.AspNetCore.Mvc;

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
    }
}