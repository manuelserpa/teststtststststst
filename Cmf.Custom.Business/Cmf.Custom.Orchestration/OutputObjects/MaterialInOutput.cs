using Cmf.Foundation.BusinessOrchestration;
using System.Runtime.Serialization;
using Cmf.Navigo.BusinessObjects.Abstractions;

namespace Cmf.Custom.amsOSRAM.Orchestration.OutputObjects
{
    /// <summary>
    /// Output Object for MaterialIn Service
    /// </summary>
    [DataContract(Name = "MaterialInOutput")]
    public class MaterialInOutput : BaseOutput
    {
        #region Properties

        /// <summary>
        /// Material
        /// </summary>
        [DataMember(Name = "Material", Order = 0)]
        public IMaterial Material { get; set; }

        #endregion
    }
}
