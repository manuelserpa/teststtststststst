namespace amsOSRAMEIAutomaticTests.Objects.Utilities
{
    public static class Constants
    {
        #region MasterData
        public static string DefaultFacility = "SUNY PROD";

        public static string DefaultProduct = "SUNYMOSFETG3A50";

        public static string DefaultFlow = "SUNYMOSFETG3A50";

        public static string DefaultStep = "MT L1-Wafer ID Read";

        public static string DefaultFlowPath = "SUNYMOSFETG3A50:1/MT L1:1/MT L1-Wafer ID Read:1";

        public static string WaferForm = "Wafer";

        public static string LotForm = "Lot";

        public static string ProductionType = "Production";
        #region Consumables


        #endregion

        #region Flows

        #endregion


        #endregion

        #region Attributes
        #endregion

        public static string ResourceDefaultStateModelStandby = "Standby";

        public static string UserName = "amsOSRAMIoTUser";

        /// <summary>
        /// Relation name between a material and resource
        /// </summary>
        public const string RelationMaterialResource = "MaterialResource";

        /// <summary>
        /// Relation name between a material and PO
        /// </summary>
        public const string RelationMaterialProductionOrder = "MaterialProductionOrder";




    }
}
