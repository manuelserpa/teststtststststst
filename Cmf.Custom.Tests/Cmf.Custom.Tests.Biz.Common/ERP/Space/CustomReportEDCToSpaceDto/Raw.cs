using System.Xml.Serialization;

namespace Cmf.Custom.Tests.Biz.Common.ERP.Spaces.CustomReportEDCToSpaceDto
{
    [XmlRoot(ElementName = "raw")]
    public class Raw
    {
        [XmlElement(ElementName = "rawValue")]
        public decimal RawValue { get; set; }
    }
}
