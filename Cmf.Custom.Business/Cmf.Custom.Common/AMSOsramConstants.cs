namespace Cmf.Custom.AMSOsram.Common
{
    public class AMSOsramConstants
    {
        #region Automation

        /// <summary>
        /// Automation TrackIn Timeout Configuration Path
        /// </summary>
        public static string AutomationTrackInTimeoutConfigurationPath = "/Cmf/Custom/Automation/TrackInTimeout";

        /// <summary>
        /// Integration Inbound Event Name
        /// </summary>
        public static string AutomationGenericRequestTimeoutConfigurationPath = "/Cmf/Custom/Automation/GenericRequestTimeout";

        /// <summary>
        /// Automation RequestType TrackIn
        /// </summary>
        public static string AutomationRequestTypeTrackIn = "TrackIn";

        /// <summary>
        /// Automation RequestType TrackIn
        /// </summary>
        public static string AutomationRequestTypeTrackOut = "TrackOut";

        /// <summary>
        /// Automation RequestType Abort
        /// </summary>
        public static string AutomationRequestTypeAbort = "Abort";

        /// <summary>
        /// Automation RequestType SendAdHocRequest
        /// </summary>
        public static string AutomationRequestSendAdHocRequest = "SendAdHocRequest";

        #endregion

        #region LookupTables

        /// <summary>
        /// Custom Sorter Logistical Process
        /// </summary>
        public static string LookupTableCustomSorterLogisticalProcess = "CustomSorterLogisticalProcess";

        /// <summary>
        /// Custom Sorter Logistical Process for MapCarrier
        /// </summary>
        public static string LookupTableCustomSorterLogisticalProcessMapCarrier = "MapCarrier";

        /// <summary>
        /// Custom Sorter Logistical Process for TransferWafers
        /// </summary>
        public static string LookupTableCustomSorterLogisticalProcessTransferWafers = "TransferWafers";

        /// <summary>
        /// Custom Sorter Logistical Process for Compose
        /// </summary>
        public static string LookupTableCustomSorterLogisticalProcessCompose = "Compose";

        /// <summary>
        /// Possible types for Container
        /// </summary>
        public static string LookupTableContainerType = "ContainerType";

        #endregion

        #region Attributes

        /// <summary>
        /// Container Attribute Lot
        /// </summary>
        public static string ContainerAttributeLot = "Lot";

        /// <summary>
        /// Resource Is Load Port In Use
        /// </summary>
        public static string ResourceAttributeIsLoadPortInUse = "IsLoadPortInUse";

        #endregion

        #region SmartTables

        #region CustomSorterJobDefinitionContext

        /// <summary>
        /// SmartTable CustomSorterJobDefinitionContext Name
        /// </summary>
        public static string CustomSorterJobDefinitionContextName = "CustomSorterJobDefinitionContext";

        /// <summary>
        /// SmartTable Step Property
        /// </summary>
        public static string CustomSorterJobDefinitionContextColumnStep = "Step";

        /// <summary>
        /// SmartTable Product Property
        /// </summary>
        public static string CustomSorterJobDefinitionContextColumnProduct = "Product";

        /// <summary>
        /// SmartTable ProductGroup Property
        /// </summary>
        public static string CustomSorterJobDefinitionContextColumnProductGroup = "ProductGroup";

        /// <summary>
        /// SmartTable Flow Property
        /// </summary>
        public static string CustomSorterJobDefinitionContextColumnFlow = "Flow";

        /// <summary>
        /// SmartTable Material Property
        /// </summary>
        public static string CustomSorterJobDefinitionContextColumnMaterial = "Material";

        /// <summary>
        /// SmartTable MaterialType Property
        /// </summary>
        public static string CustomSorterJobDefinitionContextColumnMaterialType = "MaterialType";

        /// <summary>
        /// SmartTable MaterialType Property
        /// </summary>
        public static string CustomSorterJobDefinitionContextColumnCustomSorterJobDefinition = "CustomSorterJobDefinition";

        #endregion
        #endregion

        #region Localized Messages

        /// <summary>
        /// Received message is empty
        /// </summary>
        public const string CustomReceiveEmptyMessage = "CustomReceiveEmptyMessage";

        /// <summary>
        /// Localized Message: ConfigNotFound (config name)
        /// </summary>
        public static string LocalizedMessageConfigNotFound = "CustomLocalizedMessageConfigNotFound";

        /// <summary>
        /// Localized Message: IoT Connection Timeout
        /// </summary>
        public static string LocalizedMessageIoTConnectionTimeout = "CustomLocalizedMessageIoTConnectionTimeout";


        #endregion

        #region Queries
        ///// <summary>
        ///// CustomCarrierTransportMovementQuery query object name
        ///// </summary>
        //public static string CustomGetActiveCarrierTransportMovementsQueryObjectName = "CustomGetActiveCarrierTransportMovements";

        ///// <summary>
        ///// CustomGetActiveCarrierTransportMovements query TriggerTime parameter name
        ///// </summary>
        //public static string CustomGetActiveCarrierTransportMovementsTriggerTimeParameterName = "TriggerTime_IsLessThanOrEqualTo_Now";

        ///// <summary>
        ///// CustomGetActiveCarrierTransportMovements query Destination parameter name
        ///// </summary>
        //public static string CustomGetActiveCarrierTransportMovementsDestinationParameterName = "Destination_IsEqualTo";

        ///// <summary>
        ///// CustomGetActiveCarrierTransportMovements query Container parameter name
        ///// </summary>
        //public static string CustomGetActiveCarrierTransportMovementsContainerParameterName = "Container_IsEqualTo";

        ///// <summary>
        ///// CustomGetActiveCarrierTransportMovements query MainStateModelStateId parameter name
        ///// </summary>
        //public static string CustomGetActiveCarrierTransportMovementsMainStateModelStateIdParameterName = "MainStateModelStateId_IsEqualTo";

        ///// <summary>
        ///// CustomGetActiveCarrierTransportMovements query ModifiedOn parameter name
        ///// </summary>
        //public static string CustomGetActiveCarrierTransportMovementsModifiedOnParameterName = "ModifiedOn_LessThanOrEqualTo";

        ///// <summary>
        ///// CustomGetActiveCarrierTransportMovements query FlowPath parameter name
        ///// </summary>
        //public static string CustomGetActiveCarrierTransportMovementsFlowPathParameterName = "FlowPath_IsEqualTo";

        ///// <summary>
        ///// CustomGetActiveCarrierTransportMovements query MainLot parameter name
        ///// </summary>
        //public static string CustomGetActiveCarrierTransportMovementsMainLotParameterName = "MainLot_IsEqualTo";

        ///// <summary>
        ///// CustomGetActiveCarrierTransportMovements query AutomationJobId parameter name
        ///// </summary>
        //public static string CustomGetActiveCarrierTransportMovementsAutomationJobIdParameterName = "AutomationJobId";

        ///// <summary>
        ///// CustomGetActiveCarrierTransportMovements query ExpectedDestination parameter name
        ///// </summary>
        //public static string CustomGetActiveCarrierTransportMovementsExpectedDestinationParameterName = "ExpectedDestination";

        ///// <summary>
        ///// CustomCarrierTransportMovement Id FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementIdFieldOutputName = "Id";

        ///// <summary>
        ///// CustomCarrierTransportMovement Name FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementNameFieldOutputName = "Name";

        ///// <summary>
        ///// CustomCarrierTransportMovement ContainerId FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementContainerIdFieldOutputName = "ContainerId";

        ///// <summary>
        ///// CustomCarrierTransportMovement ContainerName FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementContainerNameFieldOutputName = "ContainerName";

        ///// <summary>
        ///// CustomCarrierTransportMovement DestinationId FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementDestinationIdFieldOutputName = "DestinationId";

        ///// <summary>
        ///// CustomCarrierTransportMovement DestinationName FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementDestinationNameFieldOutputName = "DestinationName";

        ///// <summary>
        ///// CustomCarrierTransportMovement FlowPath FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementFlowPathFieldOutputName = Navigo.Common.Constants.FlowPath;

        ///// <summary>
        ///// CustomCarrierTransportMovement MainStateModelStateId FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementMainStateModelStateIdFieldOutputName = "MainStateModelStateId";

        ///// <summary>
        ///// CustomCarrierTransportMovement TriggerTime FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementTriggerTimeFieldOutputName = "TriggerTime";

        ///// <summary>
        ///// CustomCarrierTransportMovement DeliveredTo FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementDeliveredToFieldOutputName = "DeliveredTo";

        ///// <summary>
        ///// CustomCarrierTransportMovement MainContainer FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementMainContainerFieldOutputName = "MainContainer";

        ///// <summary>
        ///// CustomCarrierTransportMovement MainLot FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementMainLotFieldOutputName = "MainLot";

        ///// <summary>
        ///// CustomCarrierTransportMovement OriginalTransportRequestId FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementOriginalTransportRequestIdFieldOutputName = "OriginalTransportRequestId";

        ///// <summary>
        ///// CustomCarrierTransportMovement Priority FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementPriorityFieldOutputName = "Priority";

        ///// <summary>
        ///// CustomCarrierTransportMovement ReadTimestamp FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementReadTimestampFieldOutputName = "ReadTimestamp";

        ///// <summary>
        ///// CustomCarrierTransportMovement SequentialOrder FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementSequentialOrderFieldOutputName = "SequentialOrder";

        ///// <summary>
        ///// CustomCarrierTransportMovement AutomationJobId FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementAutomationJobIdFieldOutputName = "AutomationJobId";

        ///// <summary>
        ///// CustomCarrierTransportMovement ExpectedDestination FieldOutput Name
        ///// </summary>
        //public static string CustomCarrierTransportMovementExpectedDestinationFieldOutputName = "ExpectedDestination";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Name
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPorts = "CustomGetResourceLoadPortsData";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Parent Resource parameter
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsParentResourceParameter = "ParentResource";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Resource Association Type parameter
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsContainerResourceAssociationTypeParameter = "ContainerResourceAssociationType";

        ///// <summary>
        ///// Query CustomGetResourceLoadPortsData LoadPortStateModelStateId parameter
        ///// </summary>
        //public static string QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortStateModelStateIdParameter = "LoadPortStateModelStateId";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Parent Resource Id Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsParentResourceIdColumn = "SourceEntityId";

        ///// <summary>
        ///// Query CustomGetContainersDockedOnResourceLoadPorts Parent Resource Name Column
        ///// </summary>
        //public static string QueryCustomGetContainersDockedOnResourceLoadPortsParentResourceNameColumn = "SourceEntityName";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Load Port Id Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortIdColumn = "TargetEntityId";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Load Port Name Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortNameColumn = "TargetEntityName";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Is Load Port In Use Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsIsLoadPortInUseColumn = "TargetEntityIsLoadPortInUse";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Load Port Type Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortTypeColumn = "TargetEntityLoadPortType";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Load Port Modified On Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortModifiedOnColumn = "TargetEntityModifiedOn";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Container Id Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsContainerIdColumn = "TargetEntityContainerResourceSourceEntityId";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Container Name Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsContainerNameColumn = "TargetEntityContainerResourceSourceEntityName";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Container Type Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsContainerTypeColumn = "TargetEntityContainerResourceSourceEntityType";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Container Total Positions Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsContainerTotalPositionsColumn = "TargetEntityContainerResourceSourceEntityTotalPositions";

        /// <summary>
        /// Query CustomGetResourceLoadPortsData Load Port State Model State Id Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsLoadPortStateModelStateIdColumn = "TargetEntityMainStateModelStateId";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Container Used Positions Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsContainerUsedPositionsColumn = "TargetEntityContainerResourceSourceEntityUsedPositions";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Container Lot Attribute Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsContainerLotAttributeColumn = "TargetEntityContainerResourceSourceEntityLot";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Container Product Attribute Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsContainerProductAttributeColumn = "TargetEntityContainerResourceSourceEntityProduct";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Container Parent Material Id Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsContainerParentMaterialIdColumn = "TargetEntityContainerResourceSourceEntityMaterialContainerSourceEntityParentMaterialId";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Container Parent Material Name Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsContainerParentMaterialNameColumn = "TargetEntityContainerResourceSourceEntityMaterialContainerSourceEntityParentMaterialName";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Container Resource Association Type Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsContainerResourceAssociationTypeColumn = "TargetEntityContainerResourceSourceEntityResourceAssociationType";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Container MapContainerNeeded Attribute Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsContainerMapContainerNeededAttributeColumn = "TargetEntityContainerResourceSourceEntityMapContainerNeeded";

        /// <summary>
        /// Query CustomGetContainersDockedOnResourceLoadPorts Container Transport Requested Attribute Column
        /// </summary>
        public static string QueryCustomGetContainersDockedOnResourceLoadPortsContainerTransportRequestedAttributeColumn = "TargetEntityContainerResourceSourceEntityTransportRequested";

        #endregion

        #region Custom Sorter Job Definition Movement List Properties
        /// <summary>
        /// Custom Sorter Job Json Moves Material Name Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonMovesPropertyMaterialName = "MaterialName";

        /// <summary>
        /// Custom Sorter Job Json Moves Source Container Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonMovesPropertySourceContainer = "SourceContainer";

        /// <summary>
        /// Custom Sorter Job Json Moves Source Position Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonMovesPropertySourcePosition = "SourcePosition";

        /// <summary>
        /// Custom Sorter Job Json Moves Source Load Port Number Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonMovesPropertySourceLoadPortNumber = "SourceLoadPortNumber";

        /// <summary>
        /// Custom Sorter Job Json Moves Destination Container Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonMovesPropertyDestinationContainer = "DestinationContainer";

        /// <summary>
        /// Custom Sorter Job Json Moves Destination Position Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonMovesPropertyDestinationPosition = "DestinationPosition";

        /// <summary>
        /// Custom Sorter Job Json Moves Destination Load Port Number Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonMovesPropertyDestinationLoadPortNumber = "DestinationLoadPortNumber";

        /// <summary>
        /// Custom Sorter Job Json Future Action Type Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonPropertyFutureActionType = "FutureActionType";

        /// <summary>
        /// Custom Sorter Job Json Moves Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonPropertyMoves = "Moves";

        /// <summary>
        /// Custom Sorter Job Json Delete On Completion Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonPropertyDeleteOnCompletion = "DeleteOnCompletion";

        /// <summary>
        /// Custom Sorter Job Json Moves Product Name Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonMovesPropertyProductName = "Product";

        /// <summary>
        /// Custom Sorter Job Json Moves Substitutes Array Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonMovesPropertySubstitutes = "Substitutes";

        /// <summary>
        /// Custom Sorter Job Json Moves Substitute Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonMovesPropertySubstitute = "Substitute";

        /// <summary>
        /// Custom Sorter Job Json Reclaim Product Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonPropertyReclaimProduct = "ReclaimProduct";

        /// <summary>
        /// Custom Sorter Job Json Grading Future Action Type Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonPropertyGradingFutureActionType = "Grading";

        /// <summary>
        /// Custom Sorter Job Json Split Future Action Type Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonPropertySplitFutureActionType = "Split";

        /// <summary>
        /// Custom Sorter Job Json Merge Future Action Type Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonPropertyMergeFutureActionType = "Merge";

        /// <summary>
        /// Custom Sorter Job Json Scrap Future Action Type Property
        /// </summary>
        public static string CustomSorterJobDefinitionJsonPropertyScrapFutureActionType = "Scrap";

        #endregion

        #region Integration Entries


        /// <summary>
        /// System ERP
        /// </summary>
        public const string CustomERPSystem = "ERP";

        /// <summary>
        /// Stibo System
        /// </summary>
        public const string CustomStiboSystem = "Stibo";

        /// <summary>
        /// Integration Inbound Event Name
        /// </summary>
        public const string CustomIntegrationInboundEventName = "Inbound";

        #endregion
        
        #region Name Generators

        public const string CustomGenerateSplitLotNames = "CustomGenerateSplitLotNames";

        #endregion
    }
}
