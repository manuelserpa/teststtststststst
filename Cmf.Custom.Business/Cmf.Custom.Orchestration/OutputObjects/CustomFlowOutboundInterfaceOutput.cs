using Cmf.Foundation.BusinessOrchestration;
using Cmf.Navigo.BusinessObjects;
using System.Runtime.Serialization;

namespace Cmf.Custom.AMSOsram.Orchestration.OutputObjects
{
    /// <summary>
    /// Output Object for CustomFlowOutboundInterface Service
    /// </summary>
    [DataContract(Name = "CustomFlowOutboundInterfaceOutput")]
    public class CustomFlowOutboundInterfaceOutput : BaseOutput
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
