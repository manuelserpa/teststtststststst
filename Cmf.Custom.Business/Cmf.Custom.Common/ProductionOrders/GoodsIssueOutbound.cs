using System;
using System.Runtime.Serialization;

namespace Cmf.Custom.AMSOsram.Common.ProductionOrders
{
    [DataContract]
    public class GoodsIssueOutbound
    {
        [DataMember(Name = "TIME_STAMP")]
        public DateTime InsertedOn { get; set; }

        [DataMember(Name = "PROD_ORDER_NR")]
        public string ProductionOrderName { get; set; }

        [DataMember(Name = "LOT_NUMBER")]
        public string MaterialName { get; set; }

        [DataMember(Name = "MATERIAL_NR")]
        public string ProductName { get; set; }

        [DataMember(Name = "QUANTITY")]
        public decimal? Quantity { get; set; }

        [DataMember(Name = "QTY_UNIT")]
        public string Units { get; set; }

        [DataMember(Name = "SAP_STORE")]
        public string SAPStore { get; set; }

        [DataMember(Name = "SITE")]
        public string Site { get; set; }

        [DataMember(Name = "MOVE_TYPE")]
        public string MovementType { get; set; }

        [DataMember(Name = "WAFERS")]
        public string Wafers { get; set; }

        [DataMember(Name = "COST_CENTER")]
        public string CostCenter { get; set; }

        [DataMember(Name = "PROJECT")]
        public string Project { get; set; }

        [DataMember(Name = "ERROR_MSG")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "DATE_DELETED")]
        public DateTime DeletedOn { get; set; }

        [DataMember(Name = "SAP_TRANSFER_DATE")]
        public DateTime SAPTransferDate { get; set; }

        [DataMember(Name = "MAT_REC_NR")]
        public string SAPMaterialReceipNumber { get; set; }

        [DataMember(Name = "MAT_CAL_YEAR")]
        public int SAPMaterialReceiptYear { get; set; }

        [DataMember(Name = "SAP_TO_STORE")]
        public string SAPToStore { get; set; }

        [DataMember(Name = "BATCH")]
        public string SAPTargetBatch { get; set; }
    }
}
