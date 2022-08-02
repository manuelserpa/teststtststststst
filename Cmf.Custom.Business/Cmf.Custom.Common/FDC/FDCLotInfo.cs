using System.Collections.Generic;

namespace Cmf.Custom.AMSOsram.Common.FDC
{
    public class FDCLotInfo
    {
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
        /// Operation
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// SPS
        /// </summary>
        public string SPS { get; set; }

        /// <summary>
        /// RecipeName
        /// </summary>
        public string RecipeName { get; set; }

        /// <summary>
        /// ProductName
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// ProductRoute
        /// </summary>
        public string ProductRoute { get; set; }

        /// <summary>
        /// NumberOfWafersInBatch
        /// </summary>
        public int NumberOfWafersInBatch { get; set; }

        /// <summary>
        /// FacilityName
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// Dynamic Key-/Value List for additional logistic terms
        /// </summary>
        public List<KeyValuePair<string, string>> AdditionalLogisticTerms { get; set; } = new List<KeyValuePair<string, string>>();
    }
}
