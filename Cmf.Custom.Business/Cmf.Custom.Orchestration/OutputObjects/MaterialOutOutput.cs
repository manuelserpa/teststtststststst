using System.Runtime.Serialization;
using Cmf.Foundation.BusinessOrchestration;

namespace Cmf.Custom.AMSOsram.Orchestration.OutputObjects
{
    /// <summary>
    /// Output Object for MaterialOut Service
    /// </summary>
    [DataContract(Name = "MaterialOutOutput")]
    public class MaterialOutOutput : BaseOutput
    {
        #region Properties

        /// <summary>
        /// Material Name
        /// </summary>
        [DataMember(Name = "MaterialName", Order = 0)]
        public string MaterialName { get; set; }

        /// <summary>
        /// Resource Name
        /// </summary>
        [DataMember(Name = "ResourceName", Order = 0)]
        public string ResourceName { get; set; }

        #endregion
    }
}
