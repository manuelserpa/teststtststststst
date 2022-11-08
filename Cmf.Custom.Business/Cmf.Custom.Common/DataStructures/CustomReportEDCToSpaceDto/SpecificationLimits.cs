using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures.CustomReportEDCToSpaceDto
{
    [XmlRoot(ElementName = "specificationLimits")]
    public class SpecificationLimits
    {
        [XmlAttribute(AttributeName = "upper")]
        public string Upper { get; set; }

        [XmlAttribute(AttributeName = "lower")]
        public string Lower { get; set; }
    }
}
