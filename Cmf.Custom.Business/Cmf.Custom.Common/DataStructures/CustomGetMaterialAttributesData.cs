using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures
{
    [XmlRoot(ElementName = "CustomGetMaterialAttributes")]
    [XmlType(TypeName = "CustomGetMaterialAttributes")]
    public class CustomGetMaterialAttributesData
    {
        [XmlElement(ElementName = "Material", IsNullable = false)]
        public List<Material> materialList { get; set; }

        /// <summary>
        /// Material information class
        /// </summary>
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
}
