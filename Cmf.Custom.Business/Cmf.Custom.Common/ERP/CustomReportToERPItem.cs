using System;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.ERP
{
    /// <summary>
    /// Information to be sent for movement type 261
    /// </summary>
    [XmlRoot(ElementName = "ReportToERPItem")]
    public class CustomReportToERPItem
    {
        /// <summary>
        /// The CreatedOn property
        /// </summary>
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The ProductionOrderNumber property
        /// </summary>
        [XmlElement(ElementName = "ProductionOrderNr")]
        public string ProductionOrderNumber { get; set; }

        /// <summary>
        /// The CostCenter property
        /// </summary>
        [XmlElement(ElementName = "CostCenter")]
        public string CostCenter { get; set; }

        /// <summary>
        /// The MaterialName property
        /// </summary>
        [XmlElement(ElementName = "LotNumber")]
        public string MaterialName { get; set; }

        /// <summary>
        /// The ProductName property
        /// </summary>
        [XmlElement(ElementName = "MaterialNr")]
        public string ProductName { get; set; }

        /// <summary>
        /// The Quantity property
        /// </summary>
        [XmlElement(ElementName = "Quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// The Units property
        /// </summary>
        [XmlElement(ElementName = "QuantityUnit")]
        public string Units { get; set; }

        /// <summary>
        /// The SAPStore property
        /// </summary>
        [XmlElement(ElementName = "SapStore")]
        public string SAPStore { get; set; }

        /// <summary>
        /// The Site property
        /// </summary>
        [XmlElement(ElementName = "Site")]
        public string Site { get; set; }

        /// <summary>
        /// The MovementType property
        /// </summary>
        [XmlElement(ElementName = "MovementType")]
        public string MovementType { get; set; }

        /// <summary>
        /// The SapToStore property
        /// </summary>
        [XmlElement(ElementName = "SapToStore")]
        public string SAPToStore { get; set; }

        /// <summary>
        ///  The Batch property
        /// </summary>
        [XmlElement(ElementName = "Batch")]
        public string BatchName { get; set; }

        /// <summary>
        /// The MatRecNr property
        /// </summary>
        [XmlElement(ElementName = "MatRecNr")]
        public string MatRecNr { get; set; }

        /// <summary>
        /// The MatCalYear property
        /// </summary>
        [XmlElement(ElementName = "MatCalYear")]
        public string MatCalYear { get; set; }
    }
}
