using Cmf.Foundation.BusinessOrchestration;
using System.Runtime.Serialization;

namespace Cmf.Custom.amsOSRAM.Orchestration.OutputObjects
{
    /// <summary>
    /// Output Object for GetMaterialAttributes Service
    /// </summary>
    [DataContract(Name = "CustomGetMaterialAttributesOutput")]
    public class CustomGetMaterialAttributesOutput : BaseOutput
    {
        #region Properties

        /// <summary>
		/// ResultXML
		/// </summary>
		[DataMember(Name = "ResultXML", Order = 100)]
        public string ResultXML
        {
            get;
            set;
        }

        #endregion
    }
}
