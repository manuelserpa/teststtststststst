using System.Runtime.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures
{
    /// <summary>
    /// Support class to represent the AdHoc call to IoT
    /// </summary>
    [DataContract]
    public class AdHocRequestAction
    {
        /// <summary>
        /// The order of the execution
        /// </summary>
        [DataMember]
        public int Order { get; set; }

        /// <summary>
        /// The driver for which the request is addressed
        /// </summary>
        [DataMember]
        public string Driver { get; set; }

        /// <summary>
        /// The request name, as a label
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The type of the request
        /// </summary>
        [DataMember]
        public AdHocActionTypes Type { get; set; }

        /// <summary>
        /// The specific content of the request, accordingly to the type of the call
        /// </summary>
        [DataMember]
        public object Content { get; set; }
    }
}
