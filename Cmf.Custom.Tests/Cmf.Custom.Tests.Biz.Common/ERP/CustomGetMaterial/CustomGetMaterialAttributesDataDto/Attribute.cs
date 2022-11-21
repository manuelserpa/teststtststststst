using System.Xml.Serialization;

namespace Cmf.Custom.Tests.Biz.Common.ERP.CustomGetMaterial.CustomGetMaterialAttributesDataDto
{
    /// <summary>
    /// Attribute information class
    /// </summary>
    public class Attribute
    {
        [XmlAttribute()]
        public string Name { get; set; }

        [XmlText()]
        public string Value { get; set; }
    }
}
