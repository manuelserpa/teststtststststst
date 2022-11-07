using Cmf.Custom.amsOSRAM.Common.DataStructures.CustomReportEDCToSpaceResponseDto;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures
{
    [XmlRoot(ElementName = "response")]
    public class CustomReportEDCToSpaceResponse
    {
        [XmlElement(ElementName = "samples")]
        public List<Sample> Samples { get; set; }        

        [XmlAttribute(AttributeName = "uploadSuccess")]
        public bool UploadSuccess { get; set; }

        [XmlAttribute(AttributeName = "validationSuccess")]
        public bool ValidationSuccess { get; set; }
    }
}
