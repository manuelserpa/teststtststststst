using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.AMSOsram.Common.ERP
{
    /// <summary>
    /// Class representing the structure of the DM_Product element.
    /// </summary>
    [XmlRoot("DM_Product")]
    public class ProcessProductData
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Description { get; set; }

        [XmlElement]
        public string Type { get; set; }

        [XmlElement]
        public string ProductType { get; set; }

        [XmlElement]
        public string DefaultUnits { get; set; }

        [XmlElement]
        public string IsEnabled { get; set; }

        [XmlElement]
        public string Yield { get; set; }

        [XmlElement]
        public string ProductGroup { get; set; }

        [XmlElement]
        public string MaximumMaterialSize { get; set; }

        [XmlArray("ProductAttributes")]
        [XmlArrayItem("ProductAttribute")]
        public List<ProcessProductParametersData> ProcessProductParameters { get; set; }
    }

    /// <summary>
    /// Class representing the structure of the ProductAttributes element.
    /// </summary>
    public class ProcessProductParametersData
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    /// <summary>
    /// Class returns list of Products
    /// </summary>
    [XmlRoot("DM_ProductMasterData")]
    public class ProcessProductDataOutput
    {
        [XmlElement("DM_Product")]
        public List<ProcessProductData> ProcessProductsData { get; set; }
    }
}
