using Cmf.Foundation.BusinessOrchestration;
using System.Runtime.Serialization;

namespace Cmf.Custom.AMSOsram.Orchestration.InputObjects
{
    /// <summary>
    /// Input Object for CustomFlowOutboundInterfaceInput Service
    /// </summary>
    [DataContract(Name = "CustomFlowOutboundInterfaceInput")]
    public class CustomFlowOutboundInterfaceInput : BaseInput
    {
        #region Properties

        /// <summary>
        /// ProductName
        /// </summary>
        [DataMember(Name = "ProductName", Order = 0)]
        public string ProductName { get; set; }

        /// <summary>
        /// FlowName
        /// </summary>
        [DataMember(Name = "FlowName", Order = 0)]
        public string FlowName { get; set; }

        /// <summary>
        /// FlowNameVersion
        /// </summary>
        [DataMember(Name = "FlowNameVersion", Order = 0)]
        public string FlowNameVersion { get; set; }

        #endregion
    }
}
