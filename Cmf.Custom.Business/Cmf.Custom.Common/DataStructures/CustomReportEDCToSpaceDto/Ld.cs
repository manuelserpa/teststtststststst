using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures.CustomReportEDCToSpaceDto
{
    [XmlRoot(ElementName = "lds")]
    public class Ld
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
    }
}
