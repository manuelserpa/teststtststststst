using System.Collections.Generic;

namespace Cmf.Custom.Tests.Biz.Common
{
    public class amsOSRAMConstants
    {
        #region Attributes 
        /// <summary>
        /// Custom Resource Attribute FDCCommunication
        /// </summary>
        public const string CustomFDCCommunicationAttribute = "FDCCommunication";

        /// <summary>
        /// Product Attribute ProductionLine
        /// </summary>
        public const string ProductAttributeProductionLine = "ProductionLine";

        /// <summary>
        /// Resource Attribute IsSorter
        /// </summary>
        public const string ResourceAttributeIsSorter = "IsSorter";

        /// <summary>
        /// Resource Attribute IsLoadPortInUse
        /// </summary>
        public const string ResourceAttributeIsLoadPortInUse = "IsLoadPortInUse";

        /// <summary>
        /// Product Attribute SAPProductType
        /// </summary>
        public const string ProductAttributeSAPProductType = "SAPProductType";

        /// <summary>
        /// Facility Attribute FacilityCode
        /// </summary>
        public const string FacilityAttributeFacilityCode = "FacilityCode";

        /// <summary>
        /// Site Attribute SiteCode
        /// </summary>
        public const string SiteAttributeSiteCode = "SiteCode";

        /// <summary>
        /// Step Attribute IsWaferReception
        /// </summary>
        public const string StepAttributeIsWaferReception = "IsWaferReception";

        /// <summary>
        /// Area Attribute LdsId
        /// </summary>
        public static string AreaAttributeLdsId = "LdsId";

        /// <summary>
        /// Product Attribute BasicType
        /// </summary>
        public static string ProductAttributeBasicType = "BasicType";

        /// <summary>
        /// Parameter Attribute SendToSpace
        /// </summary>
        public static string ParameterAttributeSendToSpace = "SendToSpace";

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
        public const string DefaultMaterialFormName = FormLot;

        /// <summary>
        /// Form: Logical wafer
        /// </summary>
        public const string DefaultMaterialLogicalWaferForm = FormLogicalWafer;

        /// <summary>
        /// Default Material Wafer Form
        /// </summary>
        public const string DefaultMaterialWaferForm = FormWafer;

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
        public const string DefaultTestFlowPath = "CMFTestFlow:A:1/CMFTestStep01:1";

        /// <summary>
        /// Default Test Production Order Type
        /// </summary>
        public const string DefaultTestPOType = "CMFTestPOType";

        /// <summary>
        /// Default Test Resource Name
        /// </summary>
        public const string DefaultTestResourceName = "CMFTestResource01";

        /// <summary>
        /// Default Test Resource Name
        /// </summary>
        public const string DefaultTestProcessResourceName = "CMFTestProcessEquipment01";

        /// <summary>
        /// Default Test SubResource Name
        /// </summary>
        public const string DefaultTestProcessSubResourceName = "CMFTestSubProcessEquipment01";

        /// <summary>
        /// Default Test Resource Alternative Name
        /// </summary>
        public const string DefaultTestMeasurementResourceAlternativeName = "CMFTestMeasurementEquipment01";

        /// <summary>
        /// Default Test SubResource Alternative Name
        /// </summary>
        public const string DefaultTestMeasurementSubResourceAlternativeName = "CMFTestSubMeasurementEquipment01";

        /// <summary>
        /// Default Test Product without ProductionLine Name
        /// </summary>
        public const string DefaultTestProductWithoutProductionLineName = "CMFTestProductNoProductionLine";

        /// <summary>
        /// Default Test Product without GT ProductionLine configuration Name
        /// </summary>
        public const string DefaultTestProductGTWithoutProductionLineName = "CMFTestProductProductionLineWithoutGTConfiguration";

        /// <summary>
        /// Default Sorter Resource Name used on wafer reception
        /// </summary>
        public const string DefaultSorterResourceName = "ENA01";

        /// <summary>
        /// Default LoadPorts Resource Name of Sorter used on wafer reception
        /// </summary>
        public static readonly List<string> DefaultSorterLoadPortResourceNames = new List<string> { "ENA01-LP01", "ENA01-LP02", "ENA01-LP03", "ENA01-LP04" };

        /// <summary>
        /// Default Product Name used on wafer reception
        /// </summary>
        public const string DefaultWaferProductName = "11018814";

        /// <summary>
        /// Default Recipe Name
        /// </summary>
        public const string DefaultRecipeName = "P-CLN024-TITIW";

        /// <summary>
        /// Default Service Name
        /// </summary>
        public const string DefaultServiceName = "CMFTestService";

        /// <summary>
        /// Default Space DataCollection Name
        /// </summary>
        public const string DefaultSpaceDataCollectionName = "SpaceDCTest";

        /// <summary>
        /// Default Space DataCollectionLimitSet Name
        /// </summary>
        public const string DefaultSpaceDataCollectionLimitSetName = "SpaceDCTestLimitSet";
        
        /// <summary>
        /// Default Test Shipping Facility Name
        /// </summary>
        public const string DefaultTestShippingFacilityName = "CMFTestShippingFacility";

        #endregion

        #region MasterData

        /// <summary>
        /// Test Facility: Regensburg Production
        /// </summary>
        public const string TestFacility = "Regensburg Production";

        /// <summary>
        /// Test Product: "11111335"
        /// </summary>
        public const string TestProduct = "11111335";

        /// <summary>
        /// Test Container Type: "SMIFPod"
        /// </summary
        public const string ContainerSMIFPod = "SMIF Pod 8-Inch";

        /// <summary>
        /// Test Container Type: "PEEK Cassette 8-Inch(13)"
        /// </summary
        public const string ContainerPeekCassete = "PEEK Cassette 8-Inch(13)";

        /// <summary>
        /// Test Container Type: "PEEK Cassette 8-Inch(25)"
        /// </summary
        public const string ContainerPeekCassete25 = "PEEK Cassette 8-Inch (25)";

        /// <summary>
        /// Form: Logistical wafer
        /// </summary>
        public const string FormWafer = "Logistical wafer";

        /// <summary>
        /// Unit: CM2
        /// </summary>
        public const string UnitWafers = "CM2";

        /// <summary>
        /// Form: Lot
        /// </summary>
        public const string FormLot = "Lot";

        /// <summary>
        /// Form: LogicalWafer
        /// </summary>
        public const string FormLogicalWafer = "Logical Wafer";

        /// <summary>
        /// Form: Wafer
        /// </summary>
        public const string FormWafer = "Wafer";

        /// <summary>
        /// Material Type: Production
        /// </summary>
        public const string MaterialTypeProduction = "Production";

        /// <summary>
        /// Custom Material State Model
        /// </summary>
        public const string MaterialStateModel = "CustomMaterialStateModel";

        /// <summary>
        /// Service WaferReception
        /// </summary>
        public const string ServiceWaferReception = "WaferReception";

        /// <summary>
        /// Default Material Wafer Crystal Type
        /// </summary>
        public const string MaterialWaferCrystalType = "Crystal";

        /// <summary>
        /// Default Material Wafer Carrier Type
        /// </summary>
        public const string MaterialWaferCarrierType = "Carrier";

        /// <summary>
        /// Default Material Wafer Substrate Type
        /// </summary>
        public const string MaterialWaferSubstrateType = "Substrate";

        /// <summary>
        /// Product Lot Product
        /// </summary>
        public const string ProductLotProduct = "CMFTestLotProduct";

        /// <summary>
        /// FlowPath used on the Space flow
        /// </summary>
        public const string FlowPathSpace = "CMFTestSpaceFlow:A:1/CMFTestProcessStep:1";

        /// <summary>
        /// Service CMFTestProcessService
        /// </summary>
        public const string ServiceCMFTestProcessService = "CMFTestProcessService";

        /// <summary>
        /// Service CMFTestMeasurementService
        /// </summary>
        public const string ServiceCMFTestMeasurementService  = "CMFTestMeasurementService";

        /// <summary>
        /// Business Partner SpaceSupplier
        /// </summary>
        public const string BusinessPartnerSpaceSupplier  = "Space Supplier";

        #region Flows and Steps

        /// <summary>
        /// Test Flow: FOL-UX3_EPA
        /// </summary>
        public const string TestFlow = "FOL-UX3_EPA";

        /// <summary>
        /// TestM3MTZnOSputterCluster6in00126F008_E: "M3-MT-ZnO-SputterCluster-6in-00126F008_E"
        /// </summary>
        public const string TestM3MTZnOSputterCluster6in00126F008_E = "M3-MT-ZnO-SputterCluster-6in-00126F008_E";

        /// <summary>
        /// TestM3SSTRinseandDryinSRD02121F011_E: "M3-SST-Rinse-and-Dry-in-SRD-02121F011_E"
        /// </summary>
        public const string TestM3SSTRinseandDryinSRD02121F011_E = "M3-SST-Rinse-and-Dry-in-SRD-02121F011_E";

        #endregion

        #region Container
        /// <summary>
        /// Fixed container size on CREE
        /// </summary>
        public const int ContainerTotalPosition = 13;

        #endregion

        #endregion

        #region Name Generators

        /// <summary>
        /// Production lot Name Generator
        /// </summary>
        public const string CustomGenerateProductionLotNames = "CustomProductionLotNameGenerator";

        /// <summary>
        /// Split Lot Name Generator
        /// </summary>
        public const string CustomGenerateSplitLotNames = "CustomGenerateSplitLotNames";


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

        /// <summary>
        /// Smart Table Name CustomProductContainerCapacities
        /// </summary>
        public const string CustomProductContainerCapacitiesSmartTable = "CustomProductContainerCapacities";

        #endregion

        #region LookupTables
        /// <summary>
        /// Lookup table CustomSorterLogisticalProcess Name
        /// </summary>
        public const string CustomSorterLogisticalProcessLookupTable = "CustomSorterLogisticalProcess";

        /// <summary>
        /// Lookup table CustomTransactions Name
        /// </summary>
        public const string CustomTransactionsLookupTable = "CustomTransactions";

        /// <summary>
        /// Lookup table CustomSorterLogisticalProcess MapCarrier Value
        /// </summary>
        public const string CustomSorterLogisticalProcessMapCarrier = "MapCarrier";

        /// <summary>
        /// Lookup table CustomSorterLogisticalProcess TransferWafers Value
        /// </summary>
        public const string CustomSorterLogisticalProcessTransferWafers = "TransferWafers";

        /// <summary>
        /// Lookup table CustomSorterLogisticalProcess Compose Value
        /// </summary>
        public const string CustomSorterLogisticalProcessCompose = "Compose";

        /// <summary>
        /// Lookup table CustomSorterProcessLookupTable Name
        /// </summary>
        public const string CustomSorterProcessLookupTable = "CustomSorterProcess";

        /// <summary>
        /// Lookup table CustomSorterProcess WaferReception Value
        /// </summary>
        public const string CustomSorterProcessWaferReception = "WaferReception";

        #endregion

        #region Integration Entries

        /// <summary>
        /// Message Type CustomPostGoodsIssueToSAP
        /// </summary> 
        public const string CustomPostGoodsIssueToSAPMessageType = "CustomPerformConsumption";

        #endregion

        #region MessageBus Subjects

        /// <summary>
        /// Tibco subject for report DataCollection to Space
        /// </summary>
        public const string CustomReportEDCToSpace = "CustomReportEDCToSpace";

        #endregion

        #region ONTO FDC

        /// <summary>
        /// Integration Entry: SourceSystem column
        /// </summary>
        public const string SourceSystem = "SourceSystem";

        /// <summary>
        /// Integration Entry: TargetSystem column
        /// </summary>
        public const string TargetSystem = "TargetSystem";

        /// <summary>
        /// Integration Entry: MessageType column
        /// </summary>
        public const string MessageType = "MessageType";

        //// <summary>
        /// LOTIN Integration Entry Message Type
        /// </summary>
        public const string MessageType_LOTIN = "LOTIN";

        /// <summary>
        /// LOTOUT Integration Entry Message Type
        /// </summary>
        public const string MessageType_LOTOUT = "LOTOUT";

        /// <summary>
        /// WAFERIN Integration Entry Message Type
        /// </summary>
        public const string MessageType_WAFERIN = "WAFERIN";

        /// <summary>
        /// WAFEROUT Integration Entry Message Type
        /// </summary>
        public const string MessageType_WAFEROUT = "WAFEROUT";

        /// <summary>
        /// OntoFDC TargetSystem
        /// </summary>
        public const string TargetSystem_OntoFDC = "OntoFDC";

        /// <summary>
        /// OntoFDC SourceSystem
        /// </summary>
        public const string SourceSystem_OntoFDC = "MES";

        /// <summary>
        /// OsramEventName
        /// </summary>
        public const string OsramEventName = "OsramEventName";

        #endregion

        #region Configurations

        /// <summary>
        /// PollingInterval config value
        /// </summary>
        public const string PollingIntervalConfigValue = "/Cmf/System/Configuration/Integration/PollingInterval/";

        /// <summary>
        /// PollingInterval config value
        /// </summary>
        public const string FDCActiveConfigPath = "/amsOSRAM/FDC/Active/";

        /// <summary>
        /// Default ContainerType that is not allowed to be undocked
        /// </summary>
        public const string DefaultVendorContainerTypesConfig = "/amsOSRAM/Container/VendorContainerTypes/";

        /// <summary>
        /// Lot name allowed characters
        /// </summary>
        public const string DefaultLotNameAllowedCharacters = "/amsOSRAM/Material/LotNameAllowedCharacters";

        /// <summary>
        /// Movement type to send goods issue
        /// </summary>
        public const string DefaultGoodsIssueMovementTypeConfig = "/Cmf/Custom/ERP/MovementType/GoodsIssue";

        /// <summary>
        /// Protocol used by Space
        /// </summary>
        public const string DefaultProtocolSpaceConfig = "/amsOSRAM/Protocol/Space";

        #endregion Configurations

        #region Localized Messages

        /// <summary>
        /// Localized Message: CustomProductionOrderDoesNotExists
        /// </summary>
        public const string LocalizedMessageCustomProductionOrderDoesNotExists = "CustomProductionOrderDoesNotExists";

        /// <summary>
        /// Localized Message: CustomInvalidPrimaryQuantity
        /// </summary>
        public const string LocalizedMessageCustomInvalidPrimaryQuantity = "CustomInvalidPrimaryQuantity";

        /// <summary>
        /// Localized Message: CustomPrimaryUnitObjectNull
        /// </summary>
        public const string LocalizedMessageCustomPrimaryUnitObjectNull = "CustomPrimaryUnitObjectNull";

        /// <summary>
        /// Localized Message: Custom Localized Message CustomLocalizedMessageProductNameAndFlowNameAtSameTime
        /// </summary>
        public const string LocalizedMessageProductNameAndFlowNameAtSameTime = "CustomLocalizedMessageProductNameAndFlowNameAtSameTime";

        /// <summary>
        /// Localized Message: Custom Localized Message CustomLocalizedMessageProductNameOrFlowNameNotDefined
        /// </summary>
        public const string LocalizedMessageProductNameOrFlowNameNotDefined = "CustomLocalizedMessageProductNameOrFlowNameNotDefined";

        /// <summary>
        /// Localized Message: Custom Localized Message CustomLocalizedMessageFlowVersionWithoutFlowName
        /// </summary>
        public const string LocalizedMessageFlowVersionWithoutFlowName = "CustomLocalizedMessageFlowVersionWithoutFlowName";

        /// <summary>
        /// Localized Message: Custom Localized Message CustomLocalizedMessageProductHasNoFlowPath
        /// </summary>
        public const string LocalizedMessageProductHasNoFlowPath = "CustomLocalizedMessageProductHasNoFlowPath";

        /// <summary>
        /// Localized Message: Custom Localized Message CustomLocalizedMessageNonExistentProduct
        /// </summary>
        public const string LocalizedMessageNonExistentProduct = "CustomLocalizedMessageNonExistentProduct";

        /// <summary>
        /// Localized Message: Custom Localized Message CustomLocalizedMessageNonExistentFlow
        /// </summary>
        public const string LocalizedMessageNonExistentFlow = "CustomLocalizedMessageNonExistentFlow";

        /// <summary>
        /// Localized Message: Custom Localized Message CustomLocalizedMessageCustomFlowInformationToERPDataObjectNull
        /// </summary>
        public const string LocalizedMessageCustomFlowInformationToERPDataObjectNull = "CustomLocalizedMessageCustomFlowInformationToERPDataObjectNull";

        /// <summary>
        /// Localized Message: CustomProductionLineAttributeWithoutValue
        /// </summary>
        public const string LocalizedMessageProductionLineAttributeWithoutValue = "CustomProductionLineAttributeWithoutValue";

        /// <summary>
        /// Localized Message: CustomGTWihtoutDataForSpecificProductionLine
        /// </summary>
        public const string LocalizedMessageGTWihtoutDataForSpecificProductionLine = "CustomGTWihtoutDataForSpecificProductionLine";

        /// <summary>
        /// Localized Message: CustomInsufficientDigitsForNameGenerator
        /// </summary>
        public const string LocalizedMessageInsufficientDigitsForNameGenerator = "CustomInsufficientDigitsForNameGenerator";

        /// <summary>
        /// Localized Message: CustomConfigMissingValue
        /// </summary>
        public const string LocalizedMessageConfigMissingValue = "CustomConfigMissingValue";

        /// <summary>
        /// Localized Message: CustomValueDoesNotExistLookupTable
        /// </summary>
        public const string LocalizedMessageCustomValueDoesNotExistLookupTable = "CustomValueDoesNotExistLookupTable";

        /// <summary>
        /// Localized Message: CustomResourceIsNotSorter
        /// </summary>
        public const string LocalizedMessageCustomResourceIsNotSorter = "CustomResourceIsNotSorter";

        /// <summary>
        /// Localized Message: CustomResourceNotOnline
        /// </summary>
        public const string LocalizedMessageCustomResourceNotOnline = "CustomResourceNotOnline";

        /// <summary>
        /// Localized Message: CustomResourceNotDescendant
        /// </summary>
        public const string LocalizedMessageCustomResourceNotDescendant = "CustomResourceNotDescendant";

        /// <summary>
        /// Localized Message: CustomResourceInUse
        /// </summary>
        public const string LocalizedMessageCustomResourceInUse = "CustomResourceInUse";

        /// <summary>
        /// Localized Message: CustomSmartTableNoResolution
        /// </summary>
        public const string LocalizedMessageCustomSmartTableNoResolution = "CustomSmartTableNoResolution";

        /// <summary>
        /// Localized Message: CustomConvertToType
        /// </summary>
        public const string LocalizedMessageCustomConvertToType = "CustomConvertToType";

        /// <summary>
        /// Localized Message: CustomResourceNoDockerContainer
        /// </summary>
        public const string LocalizedMessageCustomResourceNoDockerContainer = "CustomResourceNoDockerContainer";

        /// <summary>
        /// Localized Message: CustomResourceNoEnoughPositionsOrInUse
        /// </summary>
        public const string LocalizedMessageCustomResourceNoEnoughPositionsOrInUse = "CustomResourceNoEnoughPositionsOrInUse";

        /// <summary>
        /// Localized Message: CustomResourceContainerDockedDifferentProducts
        /// </summary>
        public const string LocalizedMessageCustomResourceContainerDockedDifferentProducts = "CustomResourceContainerDockedDifferentProducts";

        /// <summary>
        /// Localized Message: CustomResourceContainersNoEnoughPositions
        /// </summary>
        public const string LocalizedMessageCustomResourceContainersNoEnoughPositions = "CustomResourceContainersNoEnoughPositions";

        /// <summary>
        /// Localized Message: CustomResourceContainersWrongPositions
        /// </summary>
        public const string LocalizedMessageCustomResourceContainersWrongPositions = "CustomResourceContainersWrongPositions";

        /// <summary>
        /// Localized Message: CustomStepNoWaferReception
        /// </summary>
        public const string LocalizedMessageCustomStepNoWaferReception = "CustomStepNoWaferReception";

        /// <summary>
        /// Localized Message: CustomContainerDifferentProducts
        /// </summary>
        public const string LocalizedMessageCustomContainerDifferentProducts = "CustomContainerDifferentProducts";

        #endregion Localized Messages

        #region GenericTables

        /// <summary>
        /// Custom Production Line Conversion table name 
        /// </summary>
        public const string GenericTableCustomProductionLineConversion = "CustomProductionLineConversion";

        /// <summary>
        /// Custom Production Line Conversion table ProductionLine property 
        /// </summary>
        public const string GenericTableCustomProductionLineConversionProductionLineProperty = "ProductionLine";

        /// <summary>
        /// Custom Production Line Conversion table Site property 
        /// </summary>
        public const string GenericTableCustomProductionLineConversionSiteProperty = "Site";

        /// <summary>
        /// Custom Transactions to Tibco table name
        /// </summary>
        public const string GenericTableCustomTransactionsToTibco = "CustomTransactionsToTibco";

        /// <summary>
        /// Custom Transactions to Tibco table Transaction Property
        /// </summary>
        public const string GenericTableCustomTransactionsToTibcoTransactionProperty = "Transaction";

        /// <summary>
        /// Custom Transactions to Tibco table IsEnabled Property
        /// </summary>
        public const string GenericTableCustomTransactionsToTibcoIsEnabledProperty = "IsEnabled";

        #endregion GenericTables
    }
}
