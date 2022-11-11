using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures
{
    [XmlRoot(ElementName="CustomGetMaterialAttributes")]
    public class CustomGetMaterialAttributesDS
    {
        [XmlElement(ElementName = "Material", IsNullable = false)]
        public List<MaterialForXML> materialList { get; set; }
    }
    public class MaterialForXML
    {
        [XmlElement(IsNullable = false)]
        public string Name { get; set; }

        [XmlElement(IsNullable = false)]
        public string Form { get; set; }

        [XmlArray(IsNullable = true, ElementName = "Attributes")]
        [XmlArrayItem(ElementName = "Attribute")]
        public List<AttributeForXML> Attributes { get; set; }

        [XmlArray(IsNullable = false, ElementName = "SubMaterials")]
        [XmlArrayItem(ElementName = "Material")]
        public List<SubMaterialForXML> SubMaterials { get; set; }
    }

    public class SubMaterialForXML
    {
        [XmlElement(IsNullable = false)]
        public string Name { get; set; }

        [XmlElement(IsNullable = false)]
        public string Form { get; set; }

        [XmlArray(IsNullable = true, ElementName = "Attributes")]
        [XmlArrayItem(ElementName = "Attribute")]
        public List<AttributeForXML> Attributes { get; set; }
    }

    public class AttributeForXML
    {
        [XmlAttribute()]
        public string Name { get; set; }

        [XmlText()]
        public string Value { get; set; }
    }
}
