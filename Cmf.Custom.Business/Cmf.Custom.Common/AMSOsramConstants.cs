namespace Cmf.Custom.AMSOsram.Common
{
    /// <summary>
    /// Support class that represents the constants to be used on the business layer
    /// </summary>
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
        /// Automation Generic Nice Label Print Resource Path
        /// </summary>
        public static string AutomationGenericNiceLabelPrintResourcePath = "/Cmf/Custom/Automation/NiceLabelPrintResource";

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

        /// <summary>
        /// Automation RequestType SendNiceLabelPrintInformation
        /// </summary>
        public static string AutomationRequestSendNiceLabelPrintInformation = "NiceLabelPrintInformation";

        #endregion

        #region Defaults

        /// <summary>
        /// Default Column name Printer from ST CustomMaterialNiceLabelPrintContext
        /// </summary>
        public const string CustomMaterialNiceLabelPrintContextPrinter = "Printer";

        /// <summary>
        /// Default Column name Label from ST CustomMaterialNiceLabelPrintContext
        /// </summary>
        public const string CustomMaterialNiceLabelPrintContextLabel = "Label";

        /// <summary>
        /// Default Column name Quantity from ST CustomMaterialNiceLabelPrintContext
        /// </summary>
        public const string CustomMaterialNiceLabelPrintContextQuantity = "Quantity";

        /// <summary>
        /// Default operation for incoming lot creation
        /// </summary>
        public const string CustomIncomingLotCreationOperation = "Certificate";

        #endregion

        #region GenericTables

        /// <summary>
        /// Custom Reclaim Container Type table name
        /// </summary>
        public static string GenericTableCustomReclaimContainerType = "CustomReclaimContainerType";

        /// <summary>
        /// Custom Reclaim Container Type table SourceContainerType Property 
        /// </summary>
        public static string GenericTableCustomReclaimContainerTypeSourceContainerTypeProperty = "SourceContainerType";

        /// <summary>
        /// CustomReclaimContainerType table ReclaimContainerType Property 
        /// </summary>
        public static string GenericTableCustomReclaimContainerTypeReclaimContainerTypeProperty = "ReclaimContainerType";

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
        /// Resource Is Sorter
        /// </summary>
        public static string ResourceAttributeIsSorter = "IsSorter";

        /// <summary>
        /// Resource Is Load Port In Use
        /// </summary>
        public static string ResourceAttributeIsLoadPortInUse = "IsLoadPortInUse";

        /// <summary>
        /// Resource Allow Download Recipe At TrackIn
        /// </summary>
        public static string ResourceAttributeAllowDownloadRecipeAtTrackIn = "AllowDownloadRecipeAtTrackIn";


        /// <summary>
        /// Container Attribute Map Container Needed for sorter
        /// </summary>
        public static string ContainerAttributeMapContainerNeeded = "MapContainerNeeded";

        /// <summary>
        /// Container Attribute Product
        /// </summary>
        public static string ContainerAttributeProduct = "Product";

        /// <summary>
        /// Product Attribute IsTestWaferMeasurementStep
        /// </summary>
        public static string ProductAttributeCanCreateInventory = "CanCreateInventory";


        #endregion

        #region SmartTables

        /// <summary>
        /// Smart Table Name CustomMaterialNiceLabelPrintContext
        /// </summary>
        public const string CustomMaterialNiceLabelPrintContextSmartTable = "CustomMaterialNiceLabelPrintContext";

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

        #region JSON Schemas
        /// <summary>
        /// JSON Schema for Custom Sorter Job Definition for Map Carrier Logistical Process
        /// </summary>
        public static string CustomSorterJobDefinitionMapCarrierSchema =
            @"{
                'description': 'JSON Schema for Custom Sorter Job Definition for Map Carrier Logistical Process.',
                'properties': {
                    'FutureActionType': {
                        'type': 'string',
                        'required': true
                    },
                    'Moves': {
                        'type': 'array',
                        'items': {
				            'type': 'object',
				            'properties': {
					            'MaterialName': {
						            'type': 'string'
					            },
					            'SourceContainer': {
						            'type': 'string'
					            },
					            'SourcePosition': {
						            'type': 'integer'
					            },
					            'DestinationContainer': {
						            'type': 'string'
					            },
					            'DestinationPosition': {
						            'type': 'integer'
					            }
				            }              
                        },
                        'required': true
                    },
                    'DeleteOnCompletion': {
                        'type': 'boolean'
                    }
                }
            }";

        /// <summary>
        /// JSON Schema for Custom Sorter Job Definition for Map Carrier Logistical Process
        /// </summary>
        public static string CustomSorterJobDefinitionTransferWafersSchema =
            @"{
                'description': 'JSON Schema for Custom Sorter Job Definition for Transfer Wafers Logistical Process.',
                'properties': {
                    'FutureActionType': {
                        'type': 'string',
                        'required': true
                    },
                    'Moves': {
                        'type': 'array',
                        'items': {
				            'type': 'object',
				            'properties': {
					            'MaterialName': {
						            'type': 'string',
                                    'required': true
					            },
					            'SourceContainer': {
						            'type': 'string',
                                    'required': true
					            },
					            'SourcePosition': {
						            'type': 'integer',
                                    'required': true
					            },
					            'DestinationContainer': {
						            'type': 'string',
                                    'required': true
					            },
					            'DestinationPosition': {
						            'type': 'integer',
                                    'required': true
					            }
				            }              
                        },
                        'required': true,
                        'minItems': 0
                    },
                    'DeleteOnCompletion': {
                        'type': 'boolean'
                    }
                }
            }";

        /// <summary>
        /// JSON Schema for Custom Sorter Job Definition for Map Carrier Logistical Process
        /// </summary>
        public static string CustomSorterJobDefinitionComposeSchema =
            @"{
                'description': 'JSON Schema for Custom Sorter Job Definition for Compose Logistical Process.',
                'properties': {
                    'FutureActionType': {
                        'type': 'string',
                        'required': true
                    },
                    'Moves': {
                        'type': 'array',
                        'items': {
				            'type': 'object',
				            'properties': {
                                'Product': {
                                    'type': 'string',
                                    'required': true
                                },
                                'Substitutes': {
                                    'type': 'array',
                                    'items': {
				                        'type': 'object',
				                        'properties': {
                                            'Substitute': {
                                                'type': 'string',
                                                'required': true
                                            },
                                            'Priority': {
                                                'type': 'integer',
                                                'required': true
                                            },
                                        }
                                    },
                                    'required': true
                                },
					            'MaterialName': {
						            'type': 'string'
					            },
					            'SourceContainer': {
						            'type': 'string'
					            },
					            'SourcePosition': {
						            'type': 'integer'
					            },
					            'DestinationContainer': {
						            'type': 'string'
					            },
					            'DestinationPosition': {
						            'type': 'integer'
					            }
				            }              
                        },
                        'required': true,
                        'minItems': 1,
                    },
                    'DeleteOnCompletion': {
                        'type': 'boolean'
                    }
                }
            }";
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
        /// Localized Message: CustomLocalizedMessageCustomSorterJobDefinitionInvalidMovementList
        /// </summary>
        public static string LocalizedMessageCustomSorterJobDefinitionInvalidMovementList = "CustomLocalizedMessageCustomSorterJobDefinitionInvalidMovementList";

        /// <summary>
        /// Localized Message: IoT Connection Timeout
        /// </summary>
        public static string LocalizedMessageIoTConnectionTimeout = "CustomLocalizedMessageIoTConnectionTimeout";

        /// <summary>
        /// Localized Message: CustomLocalizedMessageNoHoldReasonsAvailableErrorMessage
        /// </summary>
        public static string LocalizedMessageNoHoldReasonAvailableErrorMessage = "CustomLocalizedMessageNoHoldReasonAvailableErrorMessage";

        /// <summary> 
        /// Localized Message: Custom Localized Message Recipe Body Empty
        /// </summary>
        public static string LocalizedMessageRecipeBodyEmpty = "CustomLocalizedMessageRecipeBodyEmpty";

        /// <summary> 
        /// Localized Message: Custom Localized Message Recipe Without Body
        /// </summary>
        public static string LocalizedMessageRecipeWithoutBody = "CustomLocalizedMessageRecipeWithoutBody";

        /// <summary>
        /// Localized Message: StateModelStateDoesNotExistException
        /// </summary>
        public static string LocalizedMessageStateModelStateDoesNotExistException = "StateModelStateDoesNotExistException";

        /// <summary>
        /// Localized Message: StateModelDoesNotExistException
        /// </summary>
        public static string LocalizedMessageStateModelDoesNotExistException = "StateModelDoesNotExistException";

        /// <summary>
        /// Localized Message: CustomUpdateMaterialOnDifferentFlowStep
        /// </summary>
        public static string LocalizedMessageCustomUpdateMaterialOnDifferentFlowStep = "CustomUpdateMaterialOnDifferentFlowStep";

        /// <summary>
        /// Localized Message: CustomUpdateMaterialDifferentWaferData
        /// </summary>
        public static string LocalizedMessageCustomUpdateMaterialDifferentWaferData = "CustomUpdateMaterialDifferentWaferData";

        /// <summary>
        /// Localized Message: CustomWrongCertificateConfiguration
        /// </summary>
        public static string LocalizedMessageCustomWrongCertificateConfiguration = "CustomWrongCertificateConfiguration";

        /// <summary>
        /// Localized Message: CustomUpdateMaterialDifferentProduct
        /// </summary>
        public static string LocalizedMessageCustomUpdateMaterialDifferentProduct = "CustomUpdateMaterialDifferentProduct";

        /// <summary>
        /// Localized Message: CustomUpdateMaterialDifferentFlow
        /// </summary>
        public static string LocalizedMessageCustomUpdateMaterialDifferentFlow = "CustomUpdateMaterialDifferentFlow";

        /// <summary>
        /// Localized Message: CustomUpdateMaterialDifferentStep
        /// </summary>
        public static string LocalizedMessageCustomUpdateMaterialDifferentStep = "CustomUpdateMaterialDifferentStep";

        /// <summary>
        /// Localized Message: CustomUpdateMaterialDifferentType
        /// </summary>
        public static string LocalizedMessageCustomUpdateMaterialDifferentType = "CustomUpdateMaterialDifferentType";

        /// <summary>
        /// Localized Message: CustomUpdateMaterialDifferentWafers
        /// </summary>
        public static string LocalizedMessageCustomUpdateMaterialDifferentWafers = "CustomUpdateMaterialDifferentWafers";

        /// <summary>
        /// Localized Message: CustomUpdateMaterialProductWaferSizeMissing
        /// </summary>
        public static string LocalizedMessageCustomUpdateMaterialProductWaferSizeMissing = "CustomUpdateMaterialProductWaferSizeMissing";

        /// <summary>
        /// Localized Message: CustomStorageLocationMissing
        /// </summary>
        public const string LocalizedMessageCustomStorageLocationMissing = "CustomStorageLocationMissing";

        #endregion

        #region State Model
        /// <summary>
        /// Custom Material State Model
        /// </summary>
        public static string MaterialStateModel = "CustomMaterialStateModel";

        /// <summary>
        /// Custom Load Port State Model
        /// </summary>
        public static string CustomLoadPortStateModel = "CustomLoadPortStateModel";

        /// <summary>
        /// CustomLoadPortStateModelState Created State Model State
        /// </summary>
        public static string CustomLoadPortStateModelStateReadyToLoadStateModelState = "ReadyToLoad";

        /// <summary>
        /// CustomLoadPortStateModelState TransportReserved State Model State
        /// </summary>
        public static string CustomLoadPortStateModelStateReservedStateModelState = "Reserved";

        /// <summary>
        /// CustomLoadPortStateModelState In Progress State Model State
        /// </summary>
        public static string CustomLoadPortStateModelStateTransferBlockedStateModelState = "TransferBlocked";

        /// <summary>
        /// CustomLoadPortStateModelState Delivered State Model State
        /// </summary>
        public static string CustomLoadPortStateModelStateReadyToUnloadStateModelState = "ReadyToUnload";

        /// <summary>
        /// Custom Material State Model State - Setup
        /// </summary>
        public static string MaterialStateModelStateSetup = "Setup";


        #endregion

        #region Configuration

        /// <summary>
        /// Hold Step reason when an abort process is performed at the lot
        /// </summary>
        public static string DefaultAbortProcessHoldReasonConfig = "/AMSOsram/AbortProcess/HoldReason/";

        /// <summary>
        /// Hold Step reason for lot incoming 
        /// </summary>
        public static string DefaultLotIncomingHoldReasonConfig = "/Cmf/Guis/Configuration/Material/IncomingLotAutoHoldReason";
        #endregion

        #region Parameters

        /// <summary>
        /// Product Parameter with wafer quantity
        /// </summary>
        public const string CustomParameterWaferQuantity = "Wafer Size";

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

        /// <summary>
        /// SAP Info received event name
        /// </summary>
        public const string ERPInfoReceivedEventName = "ERPInfoReceived";

        /// <summary>
        /// SAP Info sent event name
        /// </summary>
        public const string ERPInfoSentEventName = "ERPInfoSent";

        #endregion

        #region Name Generators

        /// <summary>
        /// Split Lot Name Generator
        /// </summary>
        public const string CustomGenerateSplitLotNames = "CustomGenerateSplitLotNames";

        /// <summary>
        /// Production lot Name Generator
        /// </summary>
        public const string CustomGenerateProductionLotNames = "CustomProductionLotNameGenerator";

        #endregion

        #region EntityTypes

        public class EntityTypes
        {
            /// <summary>
            /// Product
            /// </summary>
            public const string Product = "Product";
        }

        #endregion

        #region MessageTypes

        public class MessageTypes
        {
            /// <summary>
            /// Message Type: CustomStorageLocationMissing
            /// </summary>
            public const string CustomPerformConsumptionToSAP = "PerformConsumptionToSAP";
        }

        #endregion
    }
}
