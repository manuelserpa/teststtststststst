using System.Runtime.Serialization;

namespace Cmf.Custom.AMSOsram.Common.DataStructures
{
    /// <summary>
    /// BrokerMessageDirection enum
    /// </summary>
    [DataContract]
    public enum BrokerMessageDirection
    {
        /// <summary>
        /// Inbound message
        /// </summary>
        [EnumMember]
        Inbound = 0,

        /// <summary>
        /// Outbound message
        /// </summary>
        [EnumMember]
        Outbound = 1
    }
}
