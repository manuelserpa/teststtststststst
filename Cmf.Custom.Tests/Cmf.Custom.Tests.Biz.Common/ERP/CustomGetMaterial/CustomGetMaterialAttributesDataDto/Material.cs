using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.Tests.Biz.Common.ERP.CustomGetMaterial.CustomGetMaterialAttributesDataDto
{
    [XmlRoot(ElementName = "Material")]
    public class Material
    {
        [XmlElement(IsNullable = false)]
        public string Name { get; set; }

        [XmlElement(IsNullable = false)]
        public string Form { get; set; }

        [XmlArray(IsNullable = true, ElementName = "Attributes")]
        [XmlArrayItem(ElementName = "Attribute")]
        public List<Attribute> Attributes { get; set; }

        [XmlArray(IsNullable = false, ElementName = "SubMaterials")]
        [XmlArrayItem(ElementName = "Material")]
        public List<Material> SubMaterials { get; set; }
    }
}
