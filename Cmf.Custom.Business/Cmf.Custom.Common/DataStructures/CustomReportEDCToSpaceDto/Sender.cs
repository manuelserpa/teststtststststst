using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures.CustomReportEDCToSpaceDto
{
    [XmlRoot(ElementName = "Sender")]
    public class Sender
    {
        [XmlText]
        public string Value { get; set; }
    }
}
