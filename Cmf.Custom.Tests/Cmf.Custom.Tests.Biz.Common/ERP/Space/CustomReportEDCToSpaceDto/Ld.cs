using System.Xml.Serialization;

namespace Cmf.Custom.Tests.Biz.Common.ERP.Spaces.CustomReportEDCToSpaceDto
{
    [XmlRoot(ElementName = "lds")]
    public class Ld
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
    }
}
