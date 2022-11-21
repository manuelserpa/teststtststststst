using Cmf.Foundation.BusinessOrchestration;
using System.Runtime.Serialization;

namespace Cmf.Custom.amsOSRAM.Orchestration.InputObjects
{
    /// <summary>
    /// Input Object for GetMaterialAttributes Service
    /// </summary>
    [DataContract(Name = "CustomGetMaterialAttributesInput")]
    public class CustomGetMaterialAttributesInput : BaseInput
    {
        /// <summary>
		/// MaterialList
		/// </summary>
		[DataMember(Name = "MaterialList", Order = 0)]
        public string MaterialList
        {
            get;
            set;
        }

        /// <summary>
        /// AttributeList
        /// </summary>
        [DataMember(Name = "AttributeList", Order = 0)]
        public string AttributeList
        {
            get;
            set;
        }

        /// <summary>
        /// IncludeSubMaterials
        /// </summary>
        [DataMember(Name = "IncludeSubMaterials", Order = 0)]
        public string IncludeSubMaterials
        {
            get;
            set;
        }

        /// <summary>
        /// SubMaterialAttributeList
        /// </summary>
        [DataMember(Name = "SubMaterialAttributeList", Order = 0)]
        public string SubMaterialAttributeList
        {
            get;
            set;
        }
    }
}
