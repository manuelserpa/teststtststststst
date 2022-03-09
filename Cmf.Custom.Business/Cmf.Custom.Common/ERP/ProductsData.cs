using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.AMSOsram.Common.ERP
{
    /// <summary>
    /// Class representing the structure of the DM_Product element.
    /// </summary>
    public class ProductData
    {
        [XmlAttribute("name")]
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

        [XmlElement]
        public string FlowPath { get; set; }

        [XmlArray("UnitConversionFactors")]
        [XmlArrayItem("Conversion")]
        public List<Conversion> UnitConversionFactors { get; set; }

        [XmlElement]
        public string SubProducts { get; set; }

        [XmlElement]
        public string BinningTree { get; set; }

        [XmlElement]
        public string InitialUnitCost { get; set; }

        [XmlElement]
        public string FinishedUnitCost { get; set; }

        [XmlElement]
        public string CycleTime { get; set; }

        [XmlElement]
        public string IncludeInSchedule { get; set; }

        [XmlElement]
        public string CapacityClass { get; set; }

        [XmlElement]
        public string MaterialTransferMode { get; set; }

        [XmlElement]
        public string MaterialTransferApprovalMode { get; set; }

        [XmlElement]
        public string MaterialTransferAllowedPickup { get; set; }

        [XmlElement]
        public string IsEnableForMaintenanceManagement { get; set; }

        [XmlElement]
        public string MaintenanceManagementConsumerQuantity { get; set; }

        [XmlElement]
        public string IsDiscrete { get; set; }

        [XmlElement]
        public string MoistureSensitivityLevel { get; set; }

        [XmlElement]
        public string FloorLife { get; set; }

        [XmlElement]
        public string FloorLifeUnitOfTime { get; set; }

        [XmlElement]
        public string RequiresApproval { get; set; }

        [XmlElement]
        public string ApprovalRole { get; set; }

        [XmlElement]
        public string CanSplitForPicking { get; set; }

        [XmlElement]
        public string MaterialLogisticsDefaultRequestQuantity { get; set; }

        [XmlElement]
        public string ConsumptionScrap { get; set; }

        [XmlElement]
        public string AdditionalConsumptionQuantity { get; set; }

        [XmlElement]
        public string IsEnabledForMaterialLogistics { get; set; }

        [XmlElement]
        public string DefaultBOM { get; set; }

        [XmlElement]
        public ProductManufacturerData ProductManufacturer { get; set; }

        [XmlArray("Attributes")]
        [XmlArrayItem("Attribute")]
        public List<ProductAttributeData> ProductAttributesData { get; set; }

        [XmlArray("Parameters")]
        [XmlArrayItem("Parameter")]
        public List<ProductParameterData> ProductParametersData { get; set; }
    }

    /// <summary>
    /// Class representing the structure of the UnitConversionFactors element.
    /// </summary>
    public class Conversion
    {
        [XmlElement]
        public string FromUnit { get; set; }

        [XmlElement]
        public string ToUnit { get; set; }

        [XmlElement]
        public string ConversionFactor { get; set; }
    }

    /// <summary>
    /// Class representing the structure of the ProductManufacturer element.
    /// </summary>
    [XmlRoot("ProductManufacturer")]
    public class ProductManufacturerData
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement]
        public string Note { get; set; }
    }

    /// <summary>
    /// Class representing the structure of the ProductParameter element.
    /// </summary>
    public class ProductParameterData
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    /// <summary>
    /// Class representing the structure of the ProductAttribute element.
    /// </summary>
    public class ProductAttributeData
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    /// <summary>
    /// Class returns list of Products
    /// </summary>
    [XmlRoot("DM_ProductMasterData")]
    public class ProductDataOutput
    {
        [XmlArray("Products")]
        [XmlArrayItem("DM_Product")]
        public List<ProductData> ProductsData { get; set; }
    }
}
