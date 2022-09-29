using System.Collections.Generic;

namespace Cmf.Custom.amsOSRAM.Common.FDC
{
    public class FDCWaferInfo
    {
        /// <summary>
        /// WaferName (<Lotname>___<LotPos>_) - mandatory
        /// </summary>
        public string WaferName { get; set; }

        /// <summary>
        /// BatchName
        /// </summary>
        public string BatchName { get; set; }

        /// <summary>
        /// LotName
        /// </summary>
        public string LotName { get; set; }

        /// <summary>
        /// Chamber
        /// </summary>
        public string Chamber { get; set; }

        /// <summary>
        /// Slot number of wafer in cassette
        /// </summary>
        public int? SlotPos { get; set; }

        /// <summary>
        /// LotPos
        /// </summary>
        public int? LotPos { get; set; }

        /// <summary>
        /// Processed flag
        /// </summary>
        public bool Processed { get; set; } = false;

        /// <summary>
        /// QtyIn which is an indicator for the wafer size
        /// </summary>
        public int? QtyIn { get; set; }

        /// <summary>
        /// WaferState (e.g. from wafer attribute 32)
        /// </summary>
        public string WaferState { get; set; }

        /// <summary>
        /// Dynamic Key-/Value List for additional logistic terms
        /// </summary>
        public List<KeyValuePair<string, string>> AdditionalLogisticTerms { get; set; } = new List<KeyValuePair<string, string>>();
    }
}
