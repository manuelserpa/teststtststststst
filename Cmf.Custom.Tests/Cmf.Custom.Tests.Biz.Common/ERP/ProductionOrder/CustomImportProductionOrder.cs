using System.Xml.Serialization;

namespace Cmf.Custom.Tests.Biz.Common.ERP.ProductionOrder
{
    /// <summary>
    /// Defines the Custom Import Production Order Entity.
    /// </summary>
    public class CustomImportProductionOrder
    {
        /// <summary>
        /// Gets or Sets the Name.
        /// </summary>
        [XmlElement]
        public string Name { get; set; }
        /// <summary>
        /// Gets or Sets the Type.
        /// </summary>
        [XmlElement]
        public string Type { get; set; }
        /// <summary>
        /// Gets or Sets the Order Number.
        /// </summary>
        [XmlElement]
        public string OrderNumber { get; set; }
        /// <summary>
        /// Gets or Sets the Facility Name.
        /// </summary>
        [XmlElement]
        public string Facility{ get; set; }
        /// <summary>
        /// Gets or Sets the Product Name.
        /// </summary>
        [XmlElement]
        public string Product { get; set; }
        /// <summary>
        /// Gets or Sets the Quantity.
        /// </summary>
        [XmlElement]
        public long? Quantity { get; set; }
        /// <summary>
        /// Gets or Sets the Units.
        /// </summary>
        [XmlElement]
        public string Units { get; set; }
        /// <summary>
        /// Gets or Sets the Due Date.
        /// </summary>
        [XmlElement]
        public string DueDate { get; set; }
        /// <summary>
        /// Gets or Sets the Restrict On Complete.
        /// </summary>
        [XmlElement]
        public string RestrictOnComplete{ get; set; }
        /// <summary>
        /// Gets or Sets the Under Delivery Tolerance.
        /// </summary>
        [XmlElement]
        public decimal? UnderDeliveryTolerance { get; set; }
        /// <summary>
        /// Gets or Sets Over Delivery Tolerance.
        /// </summary>
        [XmlElement]
        public decimal? OverDeliveryTolerance { get; set; }
        /// <summary>
        /// GEts or Sets the Planeed Start Date.
        /// </summary>
        [XmlElement]
        public string PlannedStartDate { get; set; }
        /// <summary>
        /// Gets or Sets the Planned End Date.
        /// </summary>
        [XmlElement]
        public string PlannedEndDate { get; set; }
        /// <summary>
        /// Gets or Sets the System State.
        /// </summary>
        [XmlElement]
        public string SystemState { get; set; }
    }
}
