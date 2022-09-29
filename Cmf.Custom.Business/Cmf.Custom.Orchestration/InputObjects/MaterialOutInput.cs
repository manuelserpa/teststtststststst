using Cmf.Foundation.BusinessOrchestration;
using System.Runtime.Serialization;
using Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions;

namespace Cmf.Custom.amsOSRAM.Orchestration.InputObjects
{
    /// <summary>
    /// Input Object for MaterialOut Service
    /// </summary>
    [DataContract(Name = "MaterialOutInput")]
    public class MaterialOutInput : BaseInput
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
        /// Container Only Process
        /// </summary>
        [DataMember(Name = "ContainerOnlyProcess", Order = 0)]
        public bool ContainerOnlyProcess { get; set; }

        /// <summary>
        /// Movement List
        /// </summary>
        [DataMember(Name = "CustomSorterJobDefinition", Order = 0)]
        public ICustomSorterJobDefinition CustomSorterJobDefinition { get; set; }

        #endregion
    }
}
