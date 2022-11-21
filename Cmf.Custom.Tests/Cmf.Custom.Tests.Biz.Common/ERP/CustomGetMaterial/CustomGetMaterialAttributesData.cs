using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.Tests.Biz.Common.ERP.CustomGetMaterial
{
    [XmlRoot(ElementName = "CustomGetMaterialAttributes")]
    [XmlType(TypeName = "CustomGetMaterialAttributes")]
    public class CustomGetMaterialAttributesData
    {
        [XmlElement(ElementName = "Material", IsNullable = false)]
        public List<CustomGetMaterialAttributesDataDto.Material> MaterialList { get; set; }
    }
}