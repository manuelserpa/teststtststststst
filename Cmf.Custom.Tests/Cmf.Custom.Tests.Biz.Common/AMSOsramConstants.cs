namespace Cmf.Custom.Tests.Biz.Common
{
    public class AMSOsramConstants
    {
        #region Attributes 

        

        #endregion

        #region DEE

        /// <summary>
        /// Custom Nice Label Print DEE
        /// </summary>
        public const string CustomNiceLabelPrintDEE = "CustomNiceLabelPrint";

        #endregion

        #region Default

        /// <summary>
        /// DefaultFacilityName
        /// </summary>
        public const string DefaultFacilityName = "Regensburg Production";

        /// <summary>
        /// DefaultProductName
        /// </summary>
        public const string DefaultProductName = "11111335";

        /// <summary>
        /// Default Test Product Name
        /// </summary>
        public const string DefaultTestProductName = "CMFTestProduct";

        /// <summary>
        /// DefaultMaterialType
        /// </summary>
        public const string DefaultMaterialType = "Production";

        /// <summary>
        /// DefaultFlowName
        /// </summary>
        public const string DefaultFlowName = "FOL-UX3_EPA";

        /// <summary>
        /// Default Test Flow 
        /// </summary>
        public const string DefaultTestFlowName = "CMFTestFlow";

        /// <summary>
        /// DefaultMVPStepName
        /// </summary>
        public const string DefaultMVPStepName = "M3-LO-Wafer Sorter-Split-01518F010_E";

        /// <summary>
        /// Default Test Step
        /// </summary>
        public const string DefaultTestStepName = "CMFTestStep01";

        /// <summary>
        /// Default Test Step
        /// </summary>
        public const string DefaultTestSecondStepName = "CMFTestStep02";

        /// <summary>
        /// DefaultMaterialFormName
        /// </summary>
        public const string DefaultMaterialFormName = "Lot";

        /// <summary>
        /// Default Material Logistical Wafer Form
        /// </summary>
        public const string DefaultMaterialLogisticalWaferForm = "Logistical wafer";

        /// <summary>
        /// DefaultMaterialUnit
        /// </summary>
        public const string DefaultMaterialUnit = "CM2";

        /// <summary>
        /// Default Test Container
        /// </summary>
        public const string DefaultContainerName = "3132323039303031";

        /// <summary>
        /// Default Test Flow Path
        /// </summary>
        public const string DefaultTestFlowPath = "CMFTestFlow:1/CMFTestStep01:1";

        /// <summary>
        /// Default Test Production Order Type
        /// </summary>
        public const string DefaultTestPOType = "CMFTestPOType";

        /// <summary>
        /// Default Test Resource Name
        /// </summary>
        public const string DefaultTestResourceName = "CMFTestResource01";

        #endregion

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
        public const string ContainerSMIFPod = "SMIF Pod 8-Inch";

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


        /// <summary>
        /// Custom Material State Model
        /// </summary>
        public static string MaterialStateModel = "CustomMaterialStateModel";

        #region Flows and Steps

        /// <summary>
        /// Test Flow: FOL-UX3_EPA
        /// </summary>
        public static string TestFlow = "FOL-UX3_EPA";

        /// <summary>
        /// TestM3MTZnOSputterCluster6in00126F008_E: "M3-MT-ZnO-SputterCluster-6in-00126F008_E"
        /// </summary>
        public static string TestM3MTZnOSputterCluster6in00126F008_E = "M3-MT-ZnO-SputterCluster-6in-00126F008_E";

        /// <summary>
        /// TestM3SSTRinseandDryinSRD02121F011_E: "M3-SST-Rinse-and-Dry-in-SRD-02121F011_E"
        /// </summary>
        public static string TestM3SSTRinseandDryinSRD02121F011_E = "M3-SST-Rinse-and-Dry-in-SRD-02121F011_E";
        
        #endregion

        #region Container
        /// <summary>
        /// Fixed container size on CREE
        /// </summary>
        public static int ContainerTotalPosition = 13;





        #endregion

        #endregion

        #region Name Generators

        /// <summary>
        /// Production lot Name Generator
        /// </summary>
        public const string CustomGenerateProductionLotNames = "CustomProductionLotNameGenerator";

        #endregion

        #region SmartTables

        /// <summary>
        /// Smart Table Name CustomMaterialNiceLabelPrintContext
        /// </summary>
        public const string CustomMaterialNiceLabelPrintContextSmartTable = "CustomMaterialNiceLabelPrintContext";

        /// <summary>
        /// Smart Table Name CustomReportConsumptionToSAP
        /// </summary>
        public const string CustomReportConsumptionToSAPSmartTable = "CustomReportConsumptionToSAP";

        #endregion

        #region LookupTables
        /// <summary>
        /// Lookup table CustomSorterLogisticalProcess Name
        /// </summary>
        public static string CustomSorterLogisticalProcessLookupTable = "CustomSorterLogisticalProcess";

        /// <summary>
        /// Lookup table CustomSorterLogisticalProcess MapCarrier Value
        /// </summary>
        public static string CustomSorterLogisticalProcessMapCarrier = "MapCarrier";

        /// <summary>
        /// Lookup table CustomSorterLogisticalProcess TransferWafers Value
        /// </summary>
        public static string CustomSorterLogisticalProcessTransferWafers = "TransferWafers";

        /// <summary>
        /// Lookup table CustomSorterLogisticalProcess Compose Value
        /// </summary>
        public static string CustomSorterLogisticalProcessCompose = "Compose";
        #endregion

        #region Integration Entries

        /// <summary>
        /// Message Type CustomPostGoodsIssueToSAP
        /// </summary> 
        public static string CustomPostGoodsIssueToSAPMessageType = "CustomPerformConsumption";

        #endregion

        #region MessageBus Subjects

        public const string CustomReportEDCToSpace = "CustomReportEDCToSpace";

        #endregion
    }
}
