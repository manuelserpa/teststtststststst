using System;
using System.Xml.Serialization;

namespace Cmf.Custom.amsOSRAM.Common.ERP
{
    /// <summary>
    /// Information to be sent for movement type 261
    /// </summary>
    public class CustomReportToERPItem
    {
        /// <summary>
        /// The CreatedOn property
        /// </summary>
        [XmlElement(ElementName = "TIME_STAMP")]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// The ProductionOrderNumber property
        /// </summary>
        [XmlElement(ElementName = "PROD_ORDER_NR")]
        public string ProductionOrderNumber { get; set; }

        /// <summary>
        /// The MaterialName property
        /// </summary>
        [XmlElement(ElementName = "LOT_NUMBER")]
        public string MaterialName { get; set; }

        /// <summary>
        /// The ProductName property
        /// </summary>
        [XmlElement(ElementName = "MATERIAL_NR")]
        public string ProductName { get; set; }

        /// <summary>
        /// The Quantity property
        /// </summary>
        [XmlElement(ElementName = "QUANTITY")]
        public decimal? Quantity { get; set; }

        /// <summary>
        /// The Units property
        /// </summary>
        [XmlElement(ElementName = "QTY_UNIT")]
        public string Units { get; set; }

        /// <summary>
        /// The SAPStore property
        /// </summary>
        [XmlElement(ElementName = "SAP_STORE")]
        public string SAPStore { get; set; }

        /// <summary>
        /// The Site property
        /// </summary>
        [XmlElement(ElementName = "SITE")]
        public string Site { get; set; }

        /// <summary>
        /// The MovementType property
        /// </summary>
        [XmlElement(ElementName = "MOVE_TYPE")]
        public string MovementType { get; set; }

        /// <summary>
        /// The SubMaterialCount property
        /// </summary>
        [XmlElement(ElementName = "WAFERS")]
        public int SubMaterialCount { get; set; }
    }
}
