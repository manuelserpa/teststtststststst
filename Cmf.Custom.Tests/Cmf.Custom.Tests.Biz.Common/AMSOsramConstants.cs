using System;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.Tests.Biz.Common
{
    public class AMSOsramConstants
    {
        #region MasterData

        /// <summary>
        /// Test Facility: SUNY
        /// </summary>
        public static string TestFacility = "SUNY";

        /// <summary>
        /// Test Facility: MVCC Utica
        /// </summary>
        public static string TestFacilityMVL = "MVCC Utica";

        /// <summary>
        /// Test Product: "SUNYMOSFETG3A50"
        /// </summary>
        public const string TestProduct = "SUNYMOSFETG3A50";

        /// <summary>
        /// Test Flow "SUNYMOSFETG3A50"
        /// </summary>
        public static string TestFlow = "SUNYMOSFETG3A50";

        /// <summary>
        /// Test Flow "TesteFlowSPC"
        /// </summary>
        public static string TestSPCFlow = "TesteFlowSPC";

        /// <summary>
        /// Test Flow "TesteFlowReworkChange"
        /// </summary>
        public static string TestReworkFlow = "TesteFlowReworkChange";

        /// <summary>
        /// Test Step "MT L1 Wafer ID Read";
        /// </summary>
        public static string TestStepMTL1WaferIDRead = "MT L1 Wafer ID Read";

        /// <summary>
        /// Test Step "MT L1 Scrub 62";
        /// </summary>
        public static string TestStepMTL1Scrub62 = "MT L1 Scrub 62";

        /// <summary>
        /// Test Step "MT L1 Scrub 62";
        /// </summary>
        public static string TestStepMTL1IncomingWafersInspection = "MT L1 Incoming wafers inspection";

        /// <summary>
        /// Test Step  "MT L1 Expose";
        /// </summary>
        public static string TestStepMTL1Expose = "MT L1 Expose";

        /// <summary>
        /// Test Step "MT L1 Scrub 62";
        /// </summary>
        public static string TestStepMTL1CDMeasurement = "MT L1 CD Measurement";

        /// <summary>
        /// Test Step "MT L10 Intermetal Deposition";
        /// </summary>
        public static string TestStepMTL10IntermetalDeposition = "MT L10 Intermetal Deposition";

        /// <summary>
        /// Test Step "MT L10 Transfer Metal to Teflon Cassette";
        /// </summary>
        public static string TestStepMTL10TransferMetalToTeflonCassette = "MT L10 Transfer Metal to Teflon Cassette";

        /// <summary>
        /// Test Step "MT L10 Transfer Metal to Teflon Cassette";
        /// </summary>
        public static string TestStepMTL10NitrideDeposition = "MT L10 Nitride Deposition";

        /// <summary>
        /// Test Step "MT L10 Coat";
        /// </summary>
        public static string TestStepMTL10Coat = "MT L10 Coat";

        /// <summary>
        /// Test Step "MT L10 Scrub 50"
        /// </summary>
        public static string TestMTL10Scrub50 = "MT L10 Scrub 50";

        /// <summary>
        /// Test Step "MT L2 Transfer Teflon RCA to FEOL Cassette";
        /// </summary>
        public static string TestStepMTL2TranferRCAoFEOLCassete = "MT L2 Transfer Teflon RCA to FEOL Cassette";

        /// <summary>
        /// Test Step "MT L2 Transfer Teflon RCA to FEOL Cassette";
        /// </summary>
        public static string TestStepMTL5FinalCDMeasurement = "MT L5 Final CD Measurement";

        /// <summary>
        /// Test FlowPath "SUNYMOSFETG3A50:1/MT L1:1/MT L1 Wafer ID Read:1"
        /// </summary>
        public static string TestFlowPath = "SUNYMOSFETG3A50:1/MT L1:1/MT L1 Wafer ID Read:1";

        /// <summary>
        /// Form: Wafer
        /// </summary>
        public static string FormWafer = "Wafer";

        /// <summary>
        /// Form: Lot
        /// </summary>
        public static string FormLot = "Lot";

        /// <summary>
        /// Product Type: Production
        /// </summary>
        public static string ProductTypeProduction = "Production";

        /// <summary>
        /// Product Type: Engineering
        /// </summary>
        public static string ProductTypeEngineering = "Engineering";

        /// <summary>
        /// Unit: Wafers
        /// </summary>
        public static string UnitWafers = "Wafers";

        /// <summary>
        /// Container Type: BEOL
        /// </summary>
        public const string ContainerTypeBEOL = "BEOL";

        /// <summary>
        /// Container Type: FEOL
        /// </summary>
        public const string ContainerTypeFEOL = "FEOL";

        /// <summary>
        /// Container Type: Teflon
        /// </summary>
        public const string ContainerTypeTeflon = "Teflon";

        /// <summary>
        /// Container Type: Metal
        /// </summary>
        public const string ContainerTypeMetal = "Metal";

        /// <summary>
        /// Custom Material State Model
        /// </summary>
        public static string MaterialStateModel = "CustomMaterialStateModel";

        /// <summary>
        /// Custom Material State Model State - Setup
        /// </summary>
        public static string MaterialStateModelStateSetup = "Setup";

        /// <summary>
        /// Material Attribute RequiresRework
        /// </summary>
        public static string MaterialAttributeRequiresRework = "RequiresRework";

        /// <summary>
        /// Material Attribute MonitorLot
        /// </summary>
        public static string MaterialAttributeMonitorLot = "MonitorLot";

        /// <summary>
        /// Step Attribute CollectInformationForMonitorGroup
        /// </summary>
        public static string StepAttributeCollectInformationForMonitorGroup = "CollectInformationForMonitorGroup";

        /// <summary>
        /// Step Attribute GroupProductionAndMonitorLots
        /// </summary>
        public static string StepAttributeGroupProductionAndMonitorLots = "GroupProductionAndMonitorLots";

        /// <summary>
        /// Step Attribute DetachWafersOnLotArrival
        /// </summary>
        public static string StepAttributeDetachWafersOnLotArrival = "DetachWafersOnLotArrival";

        /// <summary>
        /// Material Type Monitor
        /// </summary>
        public static string MaterialTypeMonitor = "Monitor";

        /// <summary>
        /// Material Type: Production
        /// </summary>
        public static string MaterialTypeProduction = "Production";

        /// <summary>
        /// Material Type: Engineering
        /// </summary>
        public static string MaterialTypeEngineering = "Engineering";

        /// <summary>
        /// SmartTable Name CustomReworkPathsCounters
        /// </summary>
        public static string customReworkPathsCountersSMName = "CustomReworkPathsCounters";

        /// <summary>
        /// SmartTable Name CustomReworkPathsLimits
        /// </summary>
        public static string customReworkPathsLimitsSMName = "CustomReworkPathsLimits";

        /// <summary>
        /// Data Collection DC_AmatVerity2WaferStatisticsTest
        /// </summary>
        public static string TestDCAmatVerity2WaferStatistics = "DC_AmatVerity2WaferStatisticsTest";

        /// <summary>
        /// Data Collection DC_AmatVerity2WaferStatisticsFreeTextTest
        /// </summary>
        public static string TestDCAmatVerity2WaferStatisticsFreeTextTest = "DC_AmatVerity2WaferStatisticsFreeTextTest";

        /// <summary>
        /// Data Collection DC Intermetal Deposition
        /// </summary>
        public static string TestDCIntermetalDeposition = "DC Intermetal Deposition";

        /// <summary>
        /// Data Collection DC Intermetal Deposition
        /// </summary>
        public static string TestDCIntermetalDepositionV2 = "DC Intermetal Deposition V2";

        /// <summary>
        /// Test SamplingPatternTest
        /// </summary>
        public static string TestSamplingPattern = "SamplingPatternTest";

        /// <summary>
        /// Default Dee Rule to use when calling the AssociateRuleToSPCchart method
        /// </summary>
        public static string DeeRuleCustomSPCViolationRequiresRework = "CustomSPCViolationRequiresRework";

        /// <summary>
        /// Chart Name SPC Chart Intermetal Deposition Thickness
        /// </summary>
        public static string TestSPCChartIntermetalDepositionThickness = "SPC Chart Intermetal Deposition Thickness";

        /// <summary>
        /// DataCollectionLimitSet DCLS Intermetal Deposition V2
        /// </summary>
        public static string TestDCLSIntermetalDepositionV2LimitSet = "DCLS Intermetal Deposition V2";

        /// <summary>
        /// CustomMaterialResponsibleStep Attribute TrackOutDate
        /// </summary>
        public static string CustomMaterialResponsibleStepAttributeTrackOutDate = "TrackOutDate";

        #endregion

        #region State Model

        /// <summary>
        /// SUNY_E10 State Model
        /// </summary>
        public static string StateModel_SUNY_E10 = "SUNY_E10";

        /// <summary>
        /// NY E10 State Model
        /// </summary>
        public static string StateModel_NY_E10 = "NY E10";

        /// <summary>
        /// StateModelState: SBY.WAIT-PRODUCT
        /// </summary>
        public const string StateModelState_SBY_WAIT_PRODUCT = "SBY.WAIT-PRODUCT";

        /// <summary>
        /// StateModelState: PRD.PRODUCTION
        /// </summary>
        public const string StateModelState_PRD_PRODUCTION = "PRD.PRODUCTION";

        /// <summary>
        /// StateModelState: ENG.WAIT STATE
        /// </summary>
        public const string StateModelState_ENG_WAIT_STATE = "ENG.WAIT STATE";

        /// <summary>
        /// StateModelState: ENG.QUAL STATE
        /// </summary>
        public const string StateModelState_ENG_QUAL_STATE = "ENG.QUAL STATE";

        /// <summary>
        /// StateModelState: SDT.WAIT STATE
        /// </summary>
        public const string StateModelState_SDT_WAIT_STATE = "SDT.WAIT STATE";

        /// <summary>
        /// StateModelState: SDT.QUAL STATE
        /// </summary>
        public const string StateModelState_SDT_QUAL_STATE = "SDT.QUAL STATE";

        /// <summary>
        /// StateModelState: UDT.WAIT STATE
        /// </summary>
        public const string StateModelState_UDT_WAIT_STATE = "UDT.WAIT STATE";

        /// <summary>
        /// StateModelState: UDT.QUAL STATE
        /// </summary>
        public const string StateModelState_UDT_QUAL_STATE = "UDT.QUAL STATE";

        /// <summary>
        /// StateModelState: NST.QUAL STATE
        /// </summary>
        public const string StateModelState_NST_QUAL_STATE = "NST.QUAL STATE";

        /// <summary>
        /// StateModelState: Standby
        /// </summary>
        public static string StateModelStateStandby = "Standby";

        /// <summary>
        /// StateModelState: Scheduled Maintenance
        /// </summary>
        public static string StateModelStateScheduledMaintenance = "Scheduled Maintenance";

        /// <summary>
        /// StateModelState: Unscheduled Maintenance
        /// </summary>
        public static string StateModelStateUnscheduledMaintenance = "Unscheduled Maintenance";

        /// <summary>
        /// StateModelState: Engineering
        /// </summary>
        public static string StateModelStateEngineering = "Engineering";

        /// <summary>
        /// StateModelState: Waiting Engineer
        /// </summary>
        public static string StateModelStateWaitingEngineer = "Waiting Engineer";

        /// <summary>
        /// StateModelState: Productive
        /// </summary>
        public static string StateModelStateProductive = "Productive";

        /// <summary>
        /// StateModelState: Operator Qualification
        /// </summary>
        public static string StateModelStateOperatorQualification = "Operator Qualification";

        /// <summary>
        /// StateModelState: Nonscheduled
        /// </summary>
        public static string StateModelStateNonscheduled = "Nonscheduled";

        /// <summary>
        /// StateModelState: Equipment Install
        /// </summary>
        public static string StateModelStateEquipmentInstall = "Equipment Install";

        /// <summary>
        /// CustomTransportMovementSM State Model
        /// </summary>
        public static string StateModelCustomCarrierTransportMovement = "CustomTransportMovementSM";

        /// <summary>
        /// StateModelState: Created
        /// </summary>
        public static string StateModelStateCreated = "Created";

        /// <summary>
        /// StateModelState: Requested
        /// </summary>
        public static string StateModelStateRequested = "Requested";

        /// <summary>
        /// StateModelState: In Progress
        /// </summary>
        public static string StateModelStateInProgress = "In Progress";

        /// <summary>
        /// StateModelState: Delivered
        /// </summary>
        public static string StateModelStateDelivered = "Delivered";

        /// <summary>
        /// StateModelState: Removed
        /// </summary>
        public static string StateModelStateRemoved = "Removed";

        /// <summary>
        /// CustomLoadPortStateModelState State Model Name
        /// </summary>
        public static string CustomLoadPortStateModelStateName = "CustomLoadPortStateModelState";

        /// <summary>
        /// CustomLoadPortStateModelState Available State Model State
        /// </summary>
        public static string CustomLoadPortStateModelStateAvailableStateModelState = "Available";

        /// <summary>
        /// CustomLoadPortStateModelState Occupied State Model State
        /// </summary>
        public static string CustomLoadPortStateModelStateOccupiedStateModelState = "Occupied";

        /// <summary>
        /// CustomLoadPortStateModelState TransportReserved State Model State
        /// </summary>
        public static string CustomLoadPortStateModelStateTransportReservedStateModelState = "TransportReserved";

        #endregion

        #region Import Wafers

        /// <summary>
        /// ImportWafer Default Flow Configuration Path
        /// </summary>
        public static string ImportWaferDefaultFlowConfigPath = "/Cree/ImportWafer/DefaultFlow/";

        /// <summary>
        /// ImportWafer Default Distribution List
        /// </summary>
        public static string ImportWaferDefaultDistribuitionList = "/Cree/ImportWafer/DistributionList/";

        #endregion

        #region Transport / Inficon

        /// <summary>
        /// Config Path that enable/disable the Transport System
        /// </summary>
        public static string EnableTransportSystemConfigPath = "/Cree/FactoryAutomation/EnableTransportSystem/";

        /// <summary>
        /// Name of DEE responsible for importing the carrier data from Inficon
        /// </summary>
        public static string TriggerImportDeeName = "CustomImportTransportInfoFromInficon";

        /// <summary>
        /// Name of DEE responsible for starting the transport job
        /// </summary>
        public static string TriggerTransportJobDeeName = "CustomTriggerTransportJob";

        /// <summary>
        /// Config Path that holds the table name from Inficon
        /// </summary>
        public static string TransportInfoTableNameConfigPath = "/Cree/Inficon/TransportInfoTableName/";

        /// <summary>
        /// Table name on Oracle database at CMF
        /// </summary>
        public static string TransportInforOracleInficonTableName = "SITE_CARRIER_DISPATCH";

        /// <summary>
        /// Inficon CustomImportTransportInfoFromInficonTimer
        /// </summary>
        public static string CustomImportTransportInfoFromInficonTimer = "CustomImportTransportInfoFromInficonTimer";

        #endregion

        #region Configurations

        /// <summary>
        /// Abort Process hold reason
        /// </summary>
        public static string AbortProcessHoldReasonConfig = "/Cree/AbortProcess/HoldReason/";

        /// <summary>
        /// TrackOut Process hold reason
        /// </summary>
        public static string TrackoutProcessHoldReasonConfig = "/Cree/TrackOutProcess/HoldReason/";

        /// <summary>
        /// SPC OutOfSpec hold reason
        /// </summary>
        public static string SPCOutOfSpecHoldReasonConfig = "/Cree/SPC/OutOfSpec/HoldReason/";

        /// <summary>
        /// Product Attribute MaximumNumberOfRecycles
        /// </summary>
        public static string ProductMaximumNumberOfRecyclesConfig = "/Cree/Product/MaximumNumberOfRecycles/";

        /// <summary>
        /// Scrap Temporary OffFlow
        /// </summary>
        public static string ScrapTemporaryOffFlowConfig = "/Cree/SorterJob/ScrapTemporaryOffFlow/";

        /// <summary>
        /// Scrap Temporary OffFlow Reason
        /// </summary>
        public static string ScrapTemporaryOffFlowReasonConfig = "/Cree/SorterJob/ScrapTemporaryOffFlowReason/";

        /// <summary>
        /// Hold after Scrap Reason
        /// </summary>
        public static string HoldAfterScrapReasonConfig = "/Cree/SorterJob/HoldAfterScrapReason/";

        #endregion

        #region DataCollection/Sampling Pattern Context

        /// <summary>
        /// SmartTable Step Property
        /// </summary>
        public static string ColumnStep = "Step";

        /// <summary>
        /// SmartTable Material Property
        /// </summary>
        public static string ColumnMaterial = "Material";

        /// <summary>
        /// SmartTable Operation Property
        /// </summary>
        public static string ColumnOperation = "Operation";

        /// <summary>
        /// SmartTable DataCollection Property
        /// </summary>
        public static string ColumnDataCollection = "DataCollection";

        /// <summary>
        /// SmartTable DataCollectionType Property
        /// </summary>
        public static string ColumnDataCollectionType = "DataCollectionType";

        /// <summary>
        /// SmartTable Operation TrackIn
        /// </summary>
        public static string Trackin = "TrackIn";

        /// <summary>
        /// SmartTable Operation TrackOut
        /// </summary>
        public static string Trackout = "TrackOut";

        /// <summary>
        /// SmartTable DataCollectionType LongRunningAfterTrackIn
        /// </summary>
        public static string DataCollectionTypeLongRunningAfterTrackIn = "LongRunningAfterTrackIn";

        /// <summary>
        /// SmartTable DataCollectionType Immediate
        /// </summary>
        public static string DataCollectionTypeImmediate = "Immediate";

        /// <summary>
        /// SmartTable SamplingPattern Property
        /// </summary>
        public static string ColumnSamplingPattern = "SamplingPattern";

        /// <summary>
        /// SmartTable DataCollectionLimitSet Property
        /// </summary>
        public static string ColumnDataCollectionLimitSet = "DataCollectionLimitSet";

        /// <summary>
        /// SmartTable LastServiceHistoryId Property
        /// </summary>
        public static string ColumnLastServiceHistoryId = "LastServiceHistoryId";

        /// <summary>
        /// SmartTable LastOperationHistorySeq Property
        /// </summary>
        public static string ColumnLastOperationHistorySeq = "LastOperationHistorySeq";

        #endregion

        #region Container
        /// <summary>
        /// Fixed container size on CREE
        /// </summary>
        public static int ContainerTotalPosition = 25;
        #endregion

        #region Smart Tables

        #region Custom Resource Notification Configuration

        /// <summary>
        /// SmartTable Custom Resource Notification Configuration Name
        /// </summary>
        public static string STCustomResourceActionNotificationsName = "CustomResourceActionNotifications";

        /// <summary>
        /// SmartTable Custom Resource Notification Configuration Name
        /// </summary>
        public static string STCustomMaterialActionNotificationsName = "CustomMaterialActionNotification";

        /// <summary>
        /// SmartTable Custom Resource Action Notification Resource Property
        /// </summary>
        public static string STCustomResourceActionNotificationResource = "Resource";

        /// <summary>
        /// SmartTable Custom Resource Action Notification Resource Type Property
        /// </summary>
        public static string STCustomResourceActionNotificationResourceType = "ResourceType";

        /// <summary>
        /// SmartTable Custom Action Notification Severity Property
        /// </summary>
        public static string STCustomActionNotificationSeverity = "Severity";

        /// <summary>
        /// SmartTable Custom Resource Action Notification State Model Property
        /// </summary>
        public static string STCustomResourceActionNotificationStateModel = "StateModel";

        /// <summary>
        /// SmartTable Custom Resource Action Notification From State Property
        /// </summary>
        public static string STCustomResourceActionNotificationFromState = "FromState";

        /// <summary>
        /// SmartTable Custom Resource Notification Configuration To State Property
        /// </summary>
        public static string STCustomResourceActionNotificationToState = "ToState";

        /// <summary>
        /// SmartTable Custom Notification Trigger Property
        /// </summary>
        public static string STCustomNotificationTriggerProperty = "NotificationTrigger";

        /// <summary>
        /// SmartTable Custom Notification Action Property
        /// </summary>
        public static string STCustomNotificationActionProperty = "NotificationAction";

        /// <summary>
        /// SmartTable Custom Notification Target Role Property
        /// </summary>
        public static string STCustomNotificationTargetRoleProperty = "TargetRole";

        /// <summary>
        /// SmartTable Custom Notification Target Role Warning Property
        /// </summary>
        public static string STCustomNotificationTargetRoleWarningProperty = "TargetRoleWarning";

        /// <summary>
        /// SmartTable Custom Notification Target Role Critical Property
        /// </summary>
        public static string STCustomNotificationTargetRoleCriticalProperty = "TargetRoleCritical";

        /// <summary>
        /// SmartTable Custom Notification Is Enable Property
        /// </summary>
        public static string STCustomNotificationIsEnableProperty = "IsEnable";

        #endregion

        #region Process lots on Qualification tool states

        /// <summary>
        /// Smart table to hold the lot types that can be processed when the tool is in qualification state
        /// </summary>
        public static string CustomLotTypesQualificationToolStatesSmartTable = "CustomLotTypesQualificationToolStates";

        /// <summary>
        /// SmartTable Id Property
        /// </summary>
        public static string CustomLotsQualificationToolStatesColumnId = "CustomLotTypesQualificationToolStatesId";

        /// <summary>
        /// SmartTable StateModel Property
        /// </summary>
        public static string CustomLotsQualificationToolStatesColumnStateModel = "StateModel";

        /// <summary>
        /// SmartTable State Property
        /// </summary>
        public static string CustomLotsQualificationToolStatesColumnState = "State";

        /// <summary>
        /// SmartTable Resource Property
        /// </summary>
        public static string CustomLotsQualificationToolStatesColumnResource = "Resource";

        /// <summary>
        /// SmartTable ResourceType Property
        /// </summary>
        public static string CustomLotsQualificationToolStatesColumnResourceType = "ResourceType";

        /// <summary>
        /// SmartTable ResourceModel Property
        /// </summary>
        public static string CustomLotsQualificationToolStatesColumnResourceModel = "ResourceModel";

        /// <summary>
        /// SmartTable LotType Property
        /// </summary>
        public static string CustomLotsQualificationToolStatesColumnLotType = "LotType";

        #endregion

        #region Custom Sorter Job Definition
        /// <summary>
        /// Custom Sorter Job Definition Context Smart Table Name
        /// </summary>
        public static string STCustomSorterJobDefinitionContext = "CustomSorterJobDefinitionContext";
        #endregion

        #region Trigger Notifications on Test Wafer Bank Thresholds

        /// <summary>
        /// SmartTable Custom Test Wafer Bank Treshold Notification Configuration Name
        /// </summary>
        public static string STCustomTestWaferBankThresholdNotification = "CustomTestWaferBankThresholdsNotification";

        /// <summary>
        /// SmartTable CustomTestWaferBankThresholdsNotification Id Property
        /// </summary>
        public static string STCustomTestWaferBankThresholdNotificationId = "CustomTestWaferBankThresholdsNotificationId";

        /// <summary>
        /// SmartTable CustomTestWaferBankThresholdsNotification WarningThreshold Property
        /// </summary>
        public static string STCustomTestWaferBankTresholdNotificationWarningThresholdProperty = "WarningThreshold";

        /// <summary>
        /// SmartTable CustomTestWaferBankThresholdsNotification CriticalThreshold Property
        /// </summary>
        public static string STCustomTestWaferBankTresholdNotificationCriticalThresholdProperty = "CriticalThreshold";

        #endregion

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

        /// <summary>
        /// CustomTestWaferTag LookupTable Value 'High Grade' Tag
        /// </summary>
        public static string LookupTableValueCustomTestWaferTagHighGrade = "High Grade";

        /// <summary>
        /// CustomTestWaferTag LookupTable Value 'Lower Grade' Tag
        /// </summary>
        public static string LookupTableValueCustomTestWaferTagLowerGrade = "Lower Grade";

        /// <summary>
        /// CustomTestWaferTag LookupTable Value 'Reclaim Grade' Tag
        /// </summary>
        public static string LookupTableValueCustomTestWaferTagReclaim = "Reclaim";

        #endregion

        #region Generic Tables
        /// <summary>
        /// Generic Table Custom Product Container Type Configuration Name
        /// </summary>
        public static string GTCustomProductContainerTypeName = "CustomProductContainerType";

        /// <summary>
        /// Generic Table Custom Product Container Type Configuration Name
        /// </summary>
        public static string GTCustomReclaimContainerTypeName = "CustomReclaimContainerType";

        /// <summary>
        /// Generic Table Custom Product Container Type Configuration Name
        /// </summary>
        public static string GTCustomProductContainerTypeProductProperty = "Product";

        /// <summary>
        /// Generic Table Custom Product Container Type Configuration Name
        /// </summary>
        public static string GTCustomProductContainerTypeContainerTypeProperty = "ContainerType";

        /// <summary>
        /// Generic Table Custom Product Container Type Configuration Name
        /// </summary>
        public static string GTCustomReclaimContainerTypeSourceContainerTypeProperty = "SourceContainerType";

        /// <summary>
        /// Generic Table Custom Product Container Type Configuration Name
        /// </summary>
        public static string GTCustomReclaimContainerTypeReclaimContainerTypeProperty = "ReclaimContainerType";
        #endregion

        #region Attributes

        /// <summary>
        /// Material Attribute TWGradeTag
        /// </summary>
        public static string MaterialAttributeTestWaferGradeTag = "TestWaferGradeTag";

        /// <summary>
        /// Step Attribute IsTestWaferMeasurementStep
        /// </summary>
        public static string StepAttributeIsTestWaferMeasurementStep = "IsTestWaferMeasurementStep";

        /// <summary>
        /// Material Attribute NumberOfRecycles
        /// </summary>
        public static string MaterialAttributeNumberOfRecycles = "NumberOfRecycles";

        /// <summary>
        /// Step Attribute IsDispositionStep
        /// </summary>
        public static string StepAttributeIsDispositionStep = "IsDispositionStep";

        /// <summary>
        /// Product Attribute MaximumNumberOfRecycles
        /// </summary>
        public static string ProductAttributeMaximumNumberOfRecycles = "MaximumNumberOfRecycles";

        /// <summary>
        /// Resource Is Sorter
        /// </summary>
        public static string ResourceAttributeIsSorter = "IsSorter";

        /// <summary>
        /// Step Attribute IsSapFirstStep
        /// </summary>
        public static string IsSapFirstStep = "IsSapFirstStep";

        #endregion

        #region Queries

        /// <summary>
        /// CustomCarrierTransportMovementQuery query object name
        /// </summary>
        public static string CustomGetActiveCarrierTransportMovementsQueryObjectName = "CustomGetActiveCarrierTransportMovements";

        #endregion
    }
}
