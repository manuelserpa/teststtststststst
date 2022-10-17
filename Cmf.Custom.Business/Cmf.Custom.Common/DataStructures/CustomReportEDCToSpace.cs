using Cmf.Custom.amsOSRAM.Common.DataStructures.CustomReportEDCToSpaceDto;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures
{
    [XmlRoot(ElementName = "request")]
    public class CustomReportEDCToSpace
    {
        [XmlElement(ElementName = "sender")]
        public Sender Sender { get; set; }

        [XmlElement(ElementName = "lds")]
        public List<Ld> Lds { get; set; }
        
        [XmlElement(ElementName = "sample")]
        public List<Sample> Samples { get; set; }
                
        [XmlAttribute(AttributeName = "sampleDate")]
        public string SampleDate { get; set; }
    }
}
