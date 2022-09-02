using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Cmf.Custom.AMSOsram.Common.ERP
{
    /// <summary>
    /// Class representing the structure of the Flow element.
    /// </summary>
    public class FlowData
    {
        /// <summary>
        /// The Site property
        /// </summary>
        [XmlElement(ElementName = "SITE")]
        public string Site { get; set; }

        /// <summary>
        /// The Facility property
        /// </summary>
        [XmlElement(ElementName = "PRODUCTION_LINE")]
        public string ProductionLine { get; set; }

        /// <summary>
        /// The Production Level property
        /// </summary>
        [XmlElement(ElementName = "PRODUCTION_LEVEL")]
        public string ProductionLevel { get; set; }

        /// <summary>
        /// The Flow ID property
        /// </summary>
        [XmlElement(ElementName = "FLOW_ID")]
        public string FlowId { get; set; }

        /// <summary>
        /// The Flow Name property
        /// </summary>
        [XmlElement(ElementName = "FLOW_NAME")]
        public string FlowName { get; set; }

        /// <summary>
        /// The Flow Version property
        /// </summary>
        [XmlElement(ElementName = "FLOW_VERSION")]
        public string FlowVersion { get; set; }

        /// <summary>
        /// The Flow State property
        /// </summary>
        [XmlElement(ElementName = "FLOW_STATE")]
        public string FlowState { get; set; }

        /// <summary>
        /// The Flow Type property
        /// </summary>
        [XmlElement(ElementName = "FLOW_TYPE")]
        public string FlowType { get; set; }

        /// <summary>
        /// The Flow Effective Date property
        /// </summary>
        [XmlElement(ElementName = "FLOW_EFFECTIVE_DATE")]
        public DateTime FlowEffectiveDate { get; set; }

        /// <summary>
        /// The Sequence property
        /// </summary>
        [XmlElement(ElementName = "SEQUENCE")]
        public string Sequence { get; set; }

        /// <summary>
        /// The Step ID property
        /// </summary>
        [XmlElement(ElementName = "STEP_ID")]
        public string StepId { get; set; }

        /// <summary>
        /// The Step Logical Name property
        /// </summary>
        [XmlElement(ElementName = "STEP_LOGICAL_NAME")]
        public string StepLogicalName { get; set; }

        /// <summary>
        /// The Step Effective Timestamp property
        /// </summary>
        [XmlElement(ElementName = "STEP_EFFECTIVE_TIMESTAMP")]
        public DateTime StepEffectiveTimestamp { get; set; }

        /// <summary>
        /// The Step Attributes property
        /// </summary>
        [XmlArray("STEP_ATTRIBUTES")]
        [XmlArrayItem("STEP_ATTRIBUTE")]
        public List<StepAttributeData> StepAttributesData { get; set; }

        /// <summary>
        /// The Step Maturity State property
        /// </summary>
        [XmlElement(ElementName = "STEP_MATURITY_STATE")]
        public string StepMaturityState { get; set; }

        /// <summary>
        /// The Service Name property
        /// </summary>
        [XmlElement(ElementName = "SERVICE_NAME")]
        public string ServiceName { get; set; }

        /// <summary>
        /// The Resource Name property
        /// </summary>
        [XmlElement(ElementName = "RESOURCE_NAME")]
        public string ResourceName { get; set; }

        /// <summary>
        /// The Resource Description property
        /// </summary>
        [XmlElement(ElementName = "RESOURCE_DESCRIPTION")]
        public string ResourceDescription { get; set; }

        /// <summary>
        /// The Resource Costcenter property
        /// </summary>
        [XmlElement(ElementName = "RESOURCE_COSTCENTER")]
        public string ResourceCostcenter { get; set; }

        /// <summary>
        /// The Resource Attributes property
        /// </summary>
        [XmlArray("RESOURCE_ATTRIBUTES")]
        [XmlArrayItem("RESOURCE_ATTRIBUTE")]
        public List<ResourceAttributeData> ResourceAttributesData { get; set; }

        /// <summary>
        /// The Recipe Name property
        /// </summary>
        [XmlElement(ElementName = "RECIPE_NAME")]
        public string RecipeName { get; set; }

        /// <summary>
        /// The Recipe Parameters property
        /// </summary>
        [XmlArray("RECIPE_PARAMETERS")]
        [XmlArrayItem("RECIPE_PARAMETER")]
        public List<RecipeParameterData> RecipeParameterData { get; set; }

        /// <summary>
        /// The Product Name property
        /// </summary>
        [XmlElement(ElementName = "PRODUCT_NAME")]
        public string ProductName { get; set; }

        /// <summary>
        /// The Product Type property
        /// </summary>
        [XmlElement(ElementName = "PRODUCT_TYPE")]
        public string ProductType { get; set; }

        /// <summary>
        /// The Product Basic Type property
        /// </summary>
        [XmlElement(ElementName = "PRODUCT_BASIC_TYPE")]
        public string ProductBasicType { get; set; }

        /// <summary>
        /// The Product Attributes property
        /// </summary>
        [XmlArray("PRODUCT_ATTRIBUTES")]
        [XmlArrayItem("PRODUCT_ATTRIBUTE")]
        public List<ProductAttributeData> ProductAttributeData { get; set; }

        /// <summary>
        /// The Product Maturity property
        /// </summary>
        [XmlElement(ElementName = "PRODUCT_MATURITY")]
        public string ProductMaturity { get; set; }

        /// <summary>
        /// The Product CT property
        /// </summary>
        [XmlElement(ElementName = "PRODUCT_CT")]
        public string ProductCT { get; set; }

        /// <summary>
        /// The Product Yield property
        /// </summary>
        [XmlElement(ElementName = "PRODUCT_YIELD")]
        public string ProductYield { get; set; }

        /// <summary>
        /// The Product Parameters property
        /// </summary>
        [XmlArray("PRODUCT_PARAMETERS")]
        [XmlArrayItem("PRODUCT_PARAMETER")]
        public List<ProductParameterData> ProductParameterData { get; set; }


        /// <summary>
        /// The Default Lot Size property
        /// </summary>
        [XmlElement(ElementName = "DEFAULT_LOT_SIZE")]
        public string DefaultLotSize { get; set; }

        /// <summary>
        /// The Labour Costcenter property
        /// </summary>
        [XmlElement(ElementName = "LABOUR_COSTCENTER")]
        public string LabourCostcenter { get; set; }

        /// <summary>
        /// The MUT property
        /// </summary>
        [XmlElement(ElementName = "MUT")]
        public string MUT { get; set; }

        /// <summary>
        /// The PCT property
        /// </summary>
        [XmlElement(ElementName = "PCT")]
        public string PCT { get; set; }

        /// <summary>
        /// The F1 property
        /// </summary>
        [XmlElement(ElementName = "F1")]
        public string F1 { get; set; }

    }



    /// <summary>
    /// Class representing the structure of the StepAttribute element.
    /// </summary>
    public class StepAttributeData
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlText]
        public string Value { get; set; }
    }


    /// <summary>
    /// Class representing the structure of the ResourceAttribute element.
    /// </summary>
    public class ResourceAttributeData
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    /// <summary>
    /// Class representing the structure of the RecipeParameter element.
    /// </summary>
    public class RecipeParameterData
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("data_Type")]
        public string DataType { get; set; }

        [XmlAttribute("units")]
        public string Units { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
