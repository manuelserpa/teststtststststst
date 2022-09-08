using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.AMSOsram.Common.DataStructures
{
    /// <summary>
    /// Flow information to ERP data structure
    /// </summary>
    [XmlRoot(ElementName = "FlowInformationToERP")]
    public class CustomFlowInformationToERPData
    {
        /// <summary>
        /// Site name property
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string Site { get; set; }

        /// <summary>
        /// Cost Center property
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string CostCenter { get; set; }

        /// <summary>
        /// Product information data property
        /// </summary>
        [XmlElement(ElementName = "Product", IsNullable = true)]
        public ProductInformation ProductInformationData { get; set; }

        /// <summary>
        /// Flow information data property
        /// </summary>
        [XmlElement(ElementName = "Flow", IsNullable = true)]
        public FlowInformation FlowInformationData { get; set; }

        /// <summary>
        /// Product information data structure
        /// </summary>
        public class ProductInformation : BasicInformation
        {
            /// <summary>
            /// Maturity element property
            /// </summary>
            [XmlElement(IsNullable = true)]
            public string Maturity { get; set; }

            /// <summary>
            /// CycleTime element property
            /// </summary>
            [XmlElement(IsNullable = true)]
            public string CycleTime { get; set; }

            /// <summary>
            /// Yield element property
            /// </summary>
            [XmlElement(IsNullable = true)]
            public string Yield { get; set; }

            /// <summary>
            /// MaximumMaterialSize element property
            /// </summary>
            [XmlElement(IsNullable = true)]
            public string MaximumMaterialSize { get; set; }

            /// <summary>
            /// ProductAttributes list property
            /// </summary>
            [XmlArray(ElementName = "Attributes", IsNullable = true)]
            [XmlArrayItem(ElementName = "Attribute")]
            public List<AttributeInformation> ProductAttributes { get; set; }

            /// <summary>
            /// ProductParameters list property
            /// </summary>
            [XmlArray(ElementName = "Parameters", IsNullable = true)]
            [XmlArrayItem(ElementName = "Parameter")]
            public List<ParameterInformation> ProductParameters { get; set; }
        }

        /// <summary>
        /// Flow information data structure
        /// </summary>
        public class FlowInformation : BasicInformation
        {
            /// <summary>
            /// Version element property
            /// </summary>
            [XmlElement(IsNullable = true)]
            public string Version { get; set; }

            /// <summary>
            /// LogicalName element property
            /// </summary>
            [XmlElement(IsNullable = true)]
            public string LogicalName { get; set; }

            /// <summary>
            /// Steps list property
            /// </summary>
            [XmlArray(ElementName = "Steps", IsNullable = true)]
            [XmlArrayItem(ElementName = "Step")]
            public List<StepInformation> Steps { get; set; }
        }

        /// <summary>
        /// Step information data structure
        /// </summary>
        public class StepInformation : BasicInformation
        {
            /// <summary>
            /// LogicalName element property
            /// </summary>
            [XmlElement(IsNullable = true)]
            public string LogicalName { get; set; }

            /// <summary>
            /// Maturity element property
            /// </summary>
            [XmlElement(IsNullable = true)]
            public string Maturity { get; set; }

            /// <summary>
            /// StepAttributes list property
            /// </summary>
            [XmlArray(ElementName = "Attributes", IsNullable = true)]
            [XmlArrayItem(ElementName = "Attribute")]
            public List<AttributeInformation> StepAttributes { get; set; }
        }

        /// <summary>
        /// Basic information data structure
        /// </summary>
        public class BasicInformation
        {
            /// <summary>
            /// Name element property
            /// </summary>
            [XmlElement(IsNullable = true)]
            public string Name { get; set; }

            /// <summary>
            /// Description element property
            /// </summary>
            [XmlElement(IsNullable = true)]
            public string Description { get; set; }

            /// <summary>
            /// Timestamp element property
            /// </summary>
            [XmlElement(IsNullable = true)]
            public string Timestamp { get; set; }

            /// <summary>
            /// Type element property
            /// </summary>
            [XmlElement(IsNullable = true)]
            public string Type { get; set; }

            /// <summary>
            /// State element property
            /// </summary>
            [XmlElement(IsNullable = true)]
            public string State { get; set; }
        }

        /// <summary>
        /// Parameter information data structure
        /// </summary>
        public class ParameterInformation
        {
            /// <summary>
            /// Name attribute property
            /// </summary>
            [XmlAttribute]
            public string Name { get; set; }

            /// <summary>
            /// Type attribute property
            /// </summary>
            [XmlAttribute]
            public string Type { get; set; }

            /// <summary>
            /// Value text property
            /// </summary>
            [XmlText]
            public string Value { get; set; }
        }

        /// <summary>
        /// Attribute information data structure
        /// </summary>
        public class AttributeInformation
        {
            /// <summary>
            /// Name attribute property
            /// </summary>
            [XmlAttribute]
            public string Name { get; set; }

            /// <summary>
            /// Value text property
            /// </summary>
            [XmlText]
            public string Value { get; set; }
        }
    }
}
