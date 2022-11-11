using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration;
using Cmf.Navigo.BusinessObjects;
using System.Runtime.Serialization;

namespace Cmf.Custom.amsOSRAM.Orchestration.InputObjects
{
    /// <summary>
    /// Input Object for MaterialAttribute Service
    /// </summary>
    [DataContract(Name = "CustomGetMaterialAttributesInput")]
    public class CustomGetMaterialAttributesInput : BaseInput
    {
        /// <summary>
		/// Message
		/// </summary>
		[DataMember(Name = "MaterialList", Order = 0)]
        public string MaterialList
        {
            get;
            set;
        }

        /// <summary>
        /// MessageType
        /// </summary>
        [DataMember(Name = "AttributeList", Order = 0)]
        public string AttributeList
        {
            get;
            set;
        }

        /// <summary>
        /// MessageType
        /// </summary>
        [DataMember(Name = "IncludeSubMaterials", Order = 0)]
        public string IncludeSubMaterials
        {
            get;
            set;
        }

        /// <summary>
        /// MessageType
        /// </summary>
        [DataMember(Name = "SubMaterialAttributeList", Order = 0)]
        public string SubMaterialAttributeList
        {
            get;
            set;
        }
    }
}
