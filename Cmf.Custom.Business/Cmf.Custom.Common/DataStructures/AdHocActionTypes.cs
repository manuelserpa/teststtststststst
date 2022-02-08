using System;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.AMSOsram.Common.DataStructures
{
    /// <summary>
    /// An enum that represents the types of possible actions for AdHoc IoT Requests
    /// </summary>
    public enum AdHocActionTypes
    {
        /// <summary>
        /// Type that represents a free send request for the equipment
        /// </summary>
        SendRequest,
        /// <summary>
        /// Type that represents an IoT request for getting a status variable in the equipment
        /// </summary>
        GetVariables,
        /// <summary>
        /// Type that represents an IoT request for setting a status variable in the equipment
        /// </summary>
        SetVariables,
        /// <summary>
        /// Type that represents an IoT request for executing a remote command
        /// </summary>
        ExecuteCommand
    }
}
