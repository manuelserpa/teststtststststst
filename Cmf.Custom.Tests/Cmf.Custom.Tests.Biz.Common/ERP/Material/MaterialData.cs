using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.Tests.Biz.Common.ERP.Material
{
    /// <summary>
    /// Class representing the structure of the Material element.
    /// </summary>
    [XmlRoot]
    public class MaterialData
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Product { get; set; }

        [XmlElement]
        public string Type { get; set; }

        [XmlElement]
        public string StateModel { get; set; }

        [XmlElement]
        public string State { get; set; }

        [XmlElement]
        public string Form { get; set; }

        [XmlElement]
        public string Facility { get; set; }

        [XmlElement]
        public string Flow { get; set; }

        [XmlElement]
        public string Step { get; set; }

        [XmlArray("Attributes")]
        [XmlArrayItem("key")]
        public List<MaterialAttributes> MaterialAttributes { get; set; }

        [XmlArray("SubMaterial")]
        [XmlArrayItem("Material")]
        public List<MaterialData> Wafers { get; set; }

        [XmlArray("EDCData")]
        [XmlArrayItem("key")]
        public List<MaterialEDCData> MaterialEDCData { get; set; }
    }

    /// <summary>
    /// Class representing the structure of the MaterialAttribute element.
    /// </summary>
    public class MaterialAttributes
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string value { get; set; }
    }

    /// <summary>
    /// Class representing the structure of the EDC Data element.
    /// </summary>
    public class MaterialEDCData
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }

    /// <summary>
    /// Class returns list of Materials
    /// </summary>
    [XmlRoot("GoodsReceiptCertificate")]
    public class GoodsReceiptCertificate
    {
        public MaterialData Material { get; set; }
    }
}
