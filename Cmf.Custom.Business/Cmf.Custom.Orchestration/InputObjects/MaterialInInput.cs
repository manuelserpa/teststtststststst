using Cmf.Foundation.BusinessOrchestration;
using System.Runtime.Serialization;

namespace Cmf.Custom.amsOSRAM.Orchestration.InputObjects
{
    /// <summary>
    /// Input Object for MaterialIn Service
    /// </summary>
    [DataContract(Name = "MaterialInInput")]
    public class MaterialInInput : BaseInput
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

        /// <summary>
        /// Carrier Id
        /// </summary>
        [DataMember(Name = "CarrierId", Order = 0)]
        public string CarrierId { get; set; }

        /// <summary>
        /// Sub Resource Order
        /// </summary>
        [DataMember(Name = "SubResourceOrder", Order = 0)]
        public int? SubResourceOrder { get; set; }

        #endregion
    }
}
