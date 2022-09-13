using Cmf.Foundation.BusinessOrchestration;
using System.Runtime.Serialization;

namespace Cmf.Custom.amsOSRAM.Orchestration.OutputObjects
{
    /// <summary>
    /// Output Object for CustomGetFlowInformationForERPOutput Service
    /// </summary>
    [DataContract(Name = "CustomGetFlowInformationForERPOutput")]
    public class CustomGetFlowInformationForERPOutput : BaseOutput
    {
        #region Properties

        /// <summary>
        /// Returns Flow Details in XML format
        /// </summary>
        [DataMember(Name = "ResultXml", Order = 0)]
        public string ResultXml { get; set; }

        #endregion
    }
}
