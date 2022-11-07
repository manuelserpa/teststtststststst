using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures.CustomReportEDCToSpaceDto
{
    [XmlRoot(ElementName = "raws")]
    public class Raws
    {
        [XmlElement(ElementName = "raw")]
        public List<Raw> RawCollection { get; set; }

        [XmlAttribute(AttributeName = "storeRaws")]
        public string StoreRaws { get; set; }
    }
}
