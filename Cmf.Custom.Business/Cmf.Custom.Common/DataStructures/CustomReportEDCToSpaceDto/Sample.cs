using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures.CustomReportEDCToSpaceDto
{
    [XmlRoot(ElementName = "sample")]
    public class Sample
    {
        [XmlArray("keys")]
        [XmlArrayItem("key")]
        public List<Key> Keys { get; set; }

        [XmlElement(ElementName = "specificationLimits")]
        public SpecificationLimits SpecificationLimits { get; set; }

        [XmlElement(ElementName = "raws")]
        public Raws Raws { get; set; }

        [XmlElement(ElementName = "parameterName")]
        public string ParameterName { get; set; }

        [XmlAttribute(AttributeName = "parameterUnit")]
        public string ParameterUnit { get; set; }
    }
}
