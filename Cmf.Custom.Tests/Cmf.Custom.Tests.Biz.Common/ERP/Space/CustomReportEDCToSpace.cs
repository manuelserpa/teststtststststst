using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cmf.Custom.Tests.Biz.Common.ERP.Space
{
    [XmlRoot(ElementName = "Sender")]
    public class Sender
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "lds")]
    public class Ld
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "key")]
    public class Key
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "raw")]
    public class Raw
    {
        [XmlArray("Keys")]
        [XmlArrayItem("Key")]
        public List<Key> Keys { get; set; }

        [XmlElement(ElementName = "rawValue")]
        public decimal RawValue { get; set; }
    }

    [XmlRoot(ElementName = "raws")]
    public class Raws
    {
        [XmlElement(ElementName = "raw")]
        public List<Raw> raws { get; set; }

        [XmlAttribute(AttributeName = "storeRaws")]
        public string StoreRaws { get; set; }
    }

    [XmlRoot(ElementName = "sample")]
    public class Sample
    {
        [XmlArray("keys")]
        [XmlArrayItem("key")]
        public List<Key> Keys { get; set; }

        [XmlElement(ElementName = "upper")]
        public string Upper { get; set; }

        [XmlElement(ElementName = "lower")]
        public string Lower { get; set; }

        [XmlElement(ElementName = "raws")]
        public Raws Raws { get; set; }

        [XmlAttribute(AttributeName = "parameterName")]
        public string ParameterName { get; set; }

        [XmlAttribute(AttributeName = "parameterUnit")]
        public string ParameterUnit { get; set; }
    }

    [XmlRoot(ElementName = "Request")]
    public class CustomReportEDCToSpace
    {
        [XmlElement(ElementName = "Sender")]
        public Sender Sender { get; set; }

        [XmlElement(ElementName = "lds")]
        public List<Ld> Lds { get; set; }

        [XmlElement(ElementName = "sample")]
        public List<Sample> Samples { get; set; }

        [XmlAttribute(AttributeName = "sampleDate")]
        public string SampleDate { get; set; }
    }
}