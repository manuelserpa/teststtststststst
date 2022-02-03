using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cmf.Custom.AMSOsram.Common.DataStructures
{
    /// <summary>
    /// Support class that represents a group of AdHoc Actions to be sent to IoT
    /// </summary>
    [DataContract]
    public class AdHocRequest
    {
        /// <summary>
        /// The driver for which the request is addressed
        /// </summary>
        [DataMember]
        public string Driver { get; set; }

        /// <summary>
        /// Flag that indicates that this group must stop if anything goes wrong in the processing of the calls
        /// </summary>
        [DataMember]
        public bool StopOnError { get; set; }

        /// <summary>
        /// Group of AdHoc Actions to be sent to IoT
        /// </summary>
        [DataMember]
        public List<AdHocRequestAction> Actions { get; set; }
    }
}
