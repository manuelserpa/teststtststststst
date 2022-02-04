using Cmf.Foundation.BusinessOrchestration;
using Cmf.Navigo.BusinessObjects;
using System.Runtime.Serialization;

namespace Cmf.Custom.AMSOsram.Orchestration.OutputObjects
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
        public Material Material { get; set; }

        #endregion
    }
}
