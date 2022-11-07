using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures.CustomReportEDCToSpaceResponseDto
{
    [XmlRoot(ElementName = "sample")]
    public class Sample
    {
        [XmlElement(ElementName = "samples")]
        public List<Sample> Samples { get; set; }

        [XmlArray("channelCkcIds")]
        [XmlArrayItem("channelCkcId")]
        public List<ChannelCkcId> ChannelCkcIds { get; set; }

        [XmlArray("violations")]
        [XmlArrayItem("violation")]
        public List<Violation> Violations { get; set; }

        [XmlAttribute(AttributeName = "accepted")]
        public bool Accepted { get; set; }

        [XmlAttribute(AttributeName = "excluded")]
        public int Excluded { get; set; }

        [XmlAttribute(AttributeName = "parameterName")]
        public string ParameterName { get; set; }

        [XmlAttribute(AttributeName = "parameterUnit")]
        public string ParameterUnit { get; set; }
    }
}
