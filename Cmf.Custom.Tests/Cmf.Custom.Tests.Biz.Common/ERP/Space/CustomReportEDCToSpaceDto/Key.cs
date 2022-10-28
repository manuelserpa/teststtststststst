using System.Xml.Serialization;

namespace Cmf.Custom.Tests.Biz.Common.ERP.Spaces.CustomReportEDCToSpaceDto
{
    [XmlRoot(ElementName = "key")]
    public class Key
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Value { get; set; }
    }
}
