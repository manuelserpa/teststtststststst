using Cmf.Foundation.BusinessOrchestration;
using System.Runtime.Serialization;

namespace Cmf.Custom.AMSOsram.Orchestration.OutputObjects
{
    /// <summary>
    /// Output Object for CustomGetFlowInformationForERPOutput Service
    /// </summary>
    [DataContract(Name = "CustomFlowOutboundInterfaceOutput")]
    public class CustomGetFlowInformationForERPOutput : BaseOutput
    {
        #region Properties

        /// <summary>
        /// FlowDetails
        /// </summary>
        [DataMember(Name = "FlowDetails", Order = 0)]
        public string FlowDetails { get; set; }

        #endregion
    }
}
