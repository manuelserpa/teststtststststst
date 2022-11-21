using System.Xml.Serialization;

namespace Cmf.Custom.Tests.Biz.Common.ERP.Spaces.CustomReportEDCToSpaceDto
{
    [XmlRoot(ElementName = "Sender")]
    public class Sender
    {
        [XmlText]
        public string Value { get; set; }
    }
}
