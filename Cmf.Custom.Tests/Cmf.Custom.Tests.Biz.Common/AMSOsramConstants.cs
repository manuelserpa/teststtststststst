using System;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.Tests.Biz.Common
{
    public class AMSOsramConstants
    {
        #region MasterData

        /// <summary>
        /// Test Facility: Regensburg Production
        /// </summary>
        public static string TestFacility = "Regensburg Production";
        
        /// <summary>
        /// Test Product: "11111335"
        /// </summary>
        public const string TestProduct = "11111335";

        /// <summary>
        /// Test Container Type: "SMIFPod"
        /// </summary
        public const string ContainerSMIFPod = "SMIFPod";

        /// <summary>
        /// Form: Logistical wafer
        /// </summary>
        public static string FormWafer = "Logistical wafer";

        /// <summary>
        /// Unit: CM2
        /// </summary>
        public static string UnitWafers = "CM2";

        /// <summary>
        /// Form: Lot
        /// </summary>
        public static string FormLot = "Lot";


        /// <summary>
        /// Material Type: Production
        /// </summary>
        public static string MaterialTypeProduction = "Production";


        #region Flows and Steps

        public static string TestFlow = "FOL-UX3_EPA";

        public static string TestM3MTZnOSputterCluster6in00126F008_E = "M3-MT-ZnO-SputterCluster-6in-00126F008_E";

        public static string TestM3SSTRinseandDryinSRD02121F011_E = "M3-SST-Rinse-and-Dry-in-SRD-02121F011_E";
        
        #endregion

        #region Container
        /// <summary>
        /// Fixed container size on CREE
        /// </summary>
        public static int ContainerTotalPosition = 25;





        #endregion

        #endregion
    }
}
