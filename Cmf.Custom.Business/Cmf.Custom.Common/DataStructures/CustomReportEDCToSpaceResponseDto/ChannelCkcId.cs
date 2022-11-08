using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures.CustomReportEDCToSpaceResponseDto
{
    [XmlRoot(ElementName = "channelCkcId")]
    public class ChannelCkcId
    {
        [XmlAttribute(AttributeName = "channelId")]
        public int ChannelId { get; set; }

        [XmlAttribute(AttributeName = "ckcId")]
        public int CkcId { get; set; }
    }
}
