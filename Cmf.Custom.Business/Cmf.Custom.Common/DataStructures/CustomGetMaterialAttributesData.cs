using System.Collections.Generic;
using System.Xml.Serialization;
using Cmf.Custom.amsOSRAM.Common.DataStructures.CustomGetMaterialAttributesDataDto;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures
{
    [XmlRoot(ElementName = "CustomGetMaterialAttributes")]
    [XmlType(TypeName = "CustomGetMaterialAttributes")]
    public class CustomGetMaterialAttributesData
    {
        [XmlElement(ElementName = "Material", IsNullable = false)]
        public List<Material> materialList { get; set; }
    }
}
