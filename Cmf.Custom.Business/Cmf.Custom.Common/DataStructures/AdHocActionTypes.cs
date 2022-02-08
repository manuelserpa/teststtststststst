using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cmf.Custom.AMSOsram.Common.DataStructures
{
    /// <summary>
    /// An enum that represents the types of possible actions for AdHoc IoT Requests
    /// </summary>
    [DataContract]
    public enum AdHocActionTypes
    {
        /// <summary>
        /// Type that represents a free send request for the equipment
        /// </summary>
        [EnumMember]
        SendRequest,
        /// <summary>
        /// Type that represents an IoT request for getting a status variable in the equipment
        /// </summary>
        [EnumMember]
        GetVariables,
        /// <summary>
        /// Type that represents an IoT request for setting a status variable in the equipment
        /// </summary>
        [EnumMember]
        SetVariables,
        /// <summary>
        /// Type that represents an IoT request for executing a remote command
        /// </summary>
        [EnumMember]
        ExecuteCommand
    }
}
