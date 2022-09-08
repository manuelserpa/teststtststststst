using Cmf.Foundation.BusinessOrchestration;
using System.Runtime.Serialization;

namespace Cmf.Custom.AMSOsram.Orchestration.OutputObjects
{
    /// <summary>
    /// Output Object for CustomGetFlowInformationForERPOutput Service
    /// </summary>
    [DataContract(Name = "CustomGetFlowInformationForERPOutput")]
    public class CustomGetFlowInformationForERPOutput : BaseOutput
    {
        #region Properties

        /// <summary>
        /// FlowDetails
        /// </summary>
        [DataMember(Name = "FlowInformationXml", Order = 0)]
        public string FlowInformationXml { get; set; }

        #endregion
    }
}
