using Cmf.Foundation.BusinessOrchestration;
using System.Runtime.Serialization;

namespace Cmf.Custom.AMSOsram.Orchestration.InputObjects
{
    /// <summary>
    /// Input Object for CustomGetFlowInformationForERPInput Service
    /// </summary>
    [DataContract(Name = "CustomFlowOutboundInterfaceInput")]
    public class CustomGetFlowInformationForERPInput : BaseInput
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
        /// FlowVersion
        /// </summary>
        [DataMember(Name = "FlowVersion", Order = 0)]
        public string FlowVersion { get; set; }

        #endregion
    }
}
