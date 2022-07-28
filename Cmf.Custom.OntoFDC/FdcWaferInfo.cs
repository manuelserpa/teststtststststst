using System.Collections.Generic;

namespace Cmf.Custom.OntoFDC
{
    public class FdcWaferInfo
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
        /// ChamberRecipe
        /// </summary>
        public string ChamberRecipe { get; set; }

        /// <summary>
        /// Slot number of wafer in cassette
        /// </summary>
        public int? SlotPos { get; set; }

        /// <summary>
        /// LotPos
        /// </summary>
        public int? LotPos { get; set; }

        /// <summary>
        /// ReadQuality from wafer id reader (if applicable)
        /// </summary>
        public int? ReadQuality { get; set; } = -1;

        /// <summary>
        /// Processed flag
        /// </summary>
        public bool Processed { get; set; } = false;

        /// <summary>
        /// CarrierGravure identifier
        /// </summary>
        public string CarrierGravure { get; set; }

        /// <summary>
        /// Gravure identifier
        /// </summary>
        public string Gravure { get; set; }

        /// <summary>
        /// VendorName identifier
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// Frame identifier
        /// </summary>
        public string Frame { get; set; }

        /// <summary>
        /// Size of a wafer (e.g. 2, 4, 6)
        /// </summary>
        public string WaferSize { get; set; }

        /// <summary>
        /// QtyIn which is an indicator for the wafer size
        /// </summary>
        public int? QtyIn { get; set; }

        /// <summary>
        /// WaferRecipe if not the same as the batch recipe or chamber recipe
        /// </summary>
        public string WaferRecipe { get; set; }

        /// <summary>
        /// WaferState (e.g. from wafer attribute 32)
        /// </summary>
        public string WaferState { get; set; }

        /// <summary>
        /// BondingBoat identifier
        /// </summary>
        public string BondingBoat { get; set; }

        /// <summary>
        /// Is test or dummy wafer
        /// </summary>
        public bool IsDummy { get; set; } = false;

        /// <summary>
        /// Dynamic Key-/Value List for additional logistic terms
        /// </summary>
        public Dictionary<string, string> AdditionalLogisticTerms { get; set; } = new Dictionary<string, string>();


    }
}