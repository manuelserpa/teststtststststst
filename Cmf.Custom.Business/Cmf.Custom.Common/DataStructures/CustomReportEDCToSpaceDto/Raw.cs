using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures.CustomReportEDCToSpaceDto
{
    [XmlRoot(ElementName = "raw")]
    public class Raw
    {
        [XmlElement(ElementName = "rawValue")]
        public decimal RawValue { get; set; }
    }
}
