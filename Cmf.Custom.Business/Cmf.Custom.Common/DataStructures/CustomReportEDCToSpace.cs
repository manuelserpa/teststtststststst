using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.DataStructures
{
    [XmlRoot("Request")]
    public class CustomReportEDCToSpace
    {
        [XmlAttribute("sampleDate")]
        public string SampleDate { get; set; }  

        [XmlElement]
        public Sender Sender { get; set; }

        [XmlArray("Ids")]
        [XmlArrayItem("id")]
        public List<SiteCode> Ids { get; set; }


        [XmlArray("keys")]
        [XmlArrayItem("key")]
        public List<Key> Keys { get; set; }

        [XmlElement("sample")]
        public List<Sample> Samples { get; set; }
    }

    public class Sender 
    { 
        [XmlElement("value")]
        public string Value { get; set; }
    }

    public class SiteCode
    {
        [XmlText]
        public string Value { get; set; }
    }

    public class Key
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class Sample
    {
        [XmlAttribute("parameterName")]
        public string ParameterName { get; set; }

        [XmlAttribute("parameterUnit")]
        public string ParameterUnit { get; set; }

        [XmlArray("Keys")]
        [XmlArrayItem("Key")]
        public List<Key> Keys { get; set; }

        [XmlElement("upper")]
        public string Upper { get; set; }

        [XmlElement("lower")]
        public string Lower { get; set; }

        [XmlElement("raws")]
        public Raws Raws { get; set; }
    }

    public class Raws
    {
        [XmlAttribute("storeRaws")]
        public string StoreRaws { get; set; }

        [XmlArrayItem("raw")]
        public List<Raw> raws { get; set; }
    }

    public class Raw 
    {
        [XmlArray("Keys")]
        [XmlArrayItem("Key")]
        public List<Key> Keys { get; set; }

        [XmlElement("rawValue")]
        public decimal RawValue { get; set; }
    }
}
