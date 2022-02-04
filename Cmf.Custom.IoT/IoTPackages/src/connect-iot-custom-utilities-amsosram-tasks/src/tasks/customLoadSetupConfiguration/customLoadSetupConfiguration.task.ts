import { Task, Dependencies, System, DI, TYPES, Container, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customLoadSetupConfiguration.default";
import { CustomSetupStoreHandler } from "../../persistence/implementation/customSetupStoreHandler";
import { DriverSetupDefinition } from "../../persistence/model/customSetup";
/** Default settings */
export const SETTINGS_DEFAULTS: CustomLoadSetupConfigurationSettings = {
    entityTypeName: "",
    attributes: [],
    loadAllAttributes: false,
    levelsToLoad: 0,
    driverName: "",
    reloadEveryChange: false,
    retries: 30,
    sleepBetweenRetries: 1000,
    _autoActivate: false,
}

/**
 * @whatItDoes
 *
 * This task waits the correct instance, load it according the defined settings and makes it available for the remaining workflow.
 *
 * @howToUse
 *
 * According this task settings, it waits for the instance to be available and, when it is necessary (thought "Activate" input),
 * loads the instance if necessary and makes it available to be used in the workflow.
 *
 *
 * ### Inputs
 * * `any` : **activate** - Triggers the load of the instance and make it available.
 *
 * ### Outputs
 * * `Cmf.Foundation.BusinessObjects.Entity` : **instance** - Loaded instance
 * * `Error*  : **error** - Error event (for logging some error that my occur)
 *
 * Others outputs may be available according to this task settings
 *
 * ### Settings
 * See {@see EntityAdjustStateSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-tasks-connect-iot-lg-intance",
    inputs: {
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        instance: <Task.TaskComplexValueType>{
            type: System.PropertyValueType.ReferenceType,
            referenceType: System.LBOS.Cmf.Foundation.Common.ReferenceType.EntityType,
            collectionType: System.CollectionType.None
        },
        customLibrary: Task.TaskValueType.String,
        equipmentIPAddress: Task.TaskValueType.String,
        equipmentIPPort: Task.TaskValueType.Integer,
        equipmentSerialPortName: Task.TaskValueType.String,
        equipmentBaudRate: Task.TaskValueType.Integer,
        equipmentDeviceId: Task.TaskValueType.Integer,
        deleteReportMode: Task.TaskValueType.String,
        enableDisableEventMode: Task.TaskValueType.String,
        enableDisableAlarmMode: Task.TaskValueType.String,
        error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.All,
    driverOptional: true
})
export class CustomLoadSetupConfigurationTask implements Task.TaskInstance, CustomLoadSetupConfigurationSettings {
    //#region Private Variables

    /** Instance Proxy. Responsible for receiving the correct instance. */
    @DI.Inject(TYPES.Task.InstanceProxy)
    private _instanceProxy: System.InstanceProxy;

    /** Logger */
    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    /** System API. Used to perform service calls. */
    @DI.Inject(TYPES.System.Proxy)
    private _systemProxy: System.SystemProxy;

    @DI.Inject(TYPES.Dependencies.Injector)
    private _taskContainer: Container;

    //    @DI.Inject(TYPES.System.PersistedDataStore)
    // private _dataStore: System.DataStore;
    @DI.Inject("GlobalCustomSetupStoreHandler")
    private _setupStore: CustomSetupStoreHandler;

    /** Callback used when instance is available. */
    private _instanceCallBack: System.InstanceProxyCallback;

    /** Instance */
    private _instance: System.LBOS.Cmf.Foundation.BusinessObjects.Entity;

    /** Flag that informs that instance was loaded once. */
    private _instanceWasLoaded: boolean = false;
    //#endregion

    // #region Public Variables
    /** EntityType name */
    public entityTypeName: string = SETTINGS_DEFAULTS.entityTypeName;
    /** Array of chosen attributes to be loaded and their scalar type name */
    public attributes: {Name: string, ScalarTypeName: string}[] = SETTINGS_DEFAULTS.attributes;
    /** Load all attributes flag. If true, load all attributes. (ignore attributes property) */
    public loadAllAttributes: boolean = SETTINGS_DEFAULTS.loadAllAttributes;
    /** Levels to load to be used */
    public levelsToLoad: number = SETTINGS_DEFAULTS.levelsToLoad;
    /** Task driver name */
    public driverName: string = SETTINGS_DEFAULTS.driverName;
    /** Flag that indicates that instance must be loaded every time the task is "Activated" or not. */
    public reloadEveryChange: boolean = SETTINGS_DEFAULTS.reloadEveryChange;
    /** Number of retries until a good answer is received from System */
    public retries: number = SETTINGS_DEFAULTS.retries;
    /** Number of milliseconds to wait between retries */
    public sleepBetweenRetries: number = SETTINGS_DEFAULTS.sleepBetweenRetries;
    /** Flag that determines if the activation is to be done when the entity is received */
    public _autoActivate: boolean = SETTINGS_DEFAULTS._autoActivate;

    /** Activate the task using the defined values */
    public activate: any = undefined;

    /**
     * Error event (for logging some error that my occur)
     */
    public error: Task.Output<Error> = new Task.Output<Error>();

    /**
     * instance output
     */
    public instance: Task.Output<System.LBOS.Cmf.Foundation.BusinessObjects.Entity> = new Task.Output<System.LBOS.Cmf.Foundation.BusinessObjects.Entity>();

    // outputs
    public customLibrary:  Task.Output<String> = new Task.Output<String>();
    public equipmentIPAddress: Task.Output<String> = new Task.Output<String>();
    public equipmentIPPort: Task.Output<number> = new Task.Output<number>();
    public equipmentSerialPortName: Task.Output<String> = new Task.Output<String>();
    public equipmentBaudRate: Task.Output<number> = new Task.Output<number>();
    public equipmentDeviceId: Task.Output<number> = new Task.Output<number>();
    public deleteReportMode: Task.Output<String> = new Task.Output<String>();
    public enableDisableEventMode: Task.Output<String> = new Task.Output<String>();
    public enableDisableAlarmMode: Task.Output<String> = new Task.Output<String>();

    // #endregion

    // #region Constructors
    // #endregion

    // #region Public Methods

    // #endregion

    // #region Private & Internal Methods
    private instanceCallBack: System.InstanceProxyCallback = async (availableInstance: System.InstanceProxyModel) => {
        // Keep the instance received from controller
        this._instance = availableInstance.instance;
        this._instanceWasLoaded = false;
        if (this._instance != null && this._instance["$type"] != null) {
            this.entityTypeName = this.entityTypeName || this._instance["$type"];
        }

        if (this._autoActivate === true) {
            const changes: Task.Changes = {};
            changes["activate"] = {
                currentValue: true,
                previousValue: undefined
            };

            this.onChanges(changes);
        }
    }
    // #endregion

    // #region Event handling Methods


    /**
     * OnChanges hook.
     * If activate is received, handles the loading logic
     */
    public async onChanges(changes: Task.Changes): Promise<void> {
        const self = this;
        if (changes["activate"]) {
            // Reset flag
            self.activate = undefined;

            const prefix = this.driverName;

            const driverSetupDefinition: DriverSetupDefinition = {} as DriverSetupDefinition;
            driverSetupDefinition.DriverName = prefix;

            // Check if instance was received
            if (self._instance == null) {
                const errorText: string = "CustomLoadSetupConfiguration was not yet notified about the entity it will represent.";
                this._logger.error(errorText);
                this.error.emit(new Error (errorText));
                return;
            }

           // If instance was not loaded or is to reload every time, load it
           const attributes: string[] = [
            "AutomationEquipmentSkipDefineReportMode",
            "AutomationEquipmentSkipEstablishCommunication",
            "AutomationEquipmentSkipLinkEvents",
            "AutomationEquipmentSkipSetOnline",
            "AutomationEquipmentWaitOnSetupTimeOutAndRetryIfErrorOccurs",
            "AutomationEquipmentEnableDisableAlarmsMode",
            "AutomationEquipmentEnableDisableEventsMode",
            "AutomationEquipmentDeleteReportMode",
            "AutomationEquipmentAddress",
            "AutomationEquipmentDeviceId",
            "AutomationEquipmentIPPort",
            "AutomationEquipmentCustomLibrary",
            "AutomationEquipmentSerialPortName",
            "AutomationEquipmentSerialBaudRate"
           ];
            const systemSettings: Utilities.SystemApiUtilsSettings = {
                    maxRetries: this.retries,
                    sleepBetweenRetries: this.sleepBetweenRetries,
                    logger: this._logger,
                }

            self._instance = await this._systemProxy.getObjectById(self._instance.Id, this.entityTypeName, this.levelsToLoad, null, systemSettings);
            self._instance = await this._systemProxy.loadAttributes(self._instance, attributes, systemSettings);

            // attributes store to use on setup
            let skipDefineReport = self._instance.Attributes.get("AutomationEquipmentSkipDefineReportMode");
            if (skipDefineReport == null) {
                skipDefineReport = false;
                this._logger.info(`AutomationEquipmentSkipDefineReportMode: ${skipDefineReport} (default value)`);
            } else {
                this._logger.info(`AutomationEquipmentSkipDefineReportMode: ${skipDefineReport}`);
            }
            // await this._dataStore.store(prefix + "_" + "AutomationEquipmentSkipDefineReportMode", skipDefineReport, System.DataStoreLocation.Persistent);
            driverSetupDefinition.AutomationEquipmentSkipDefineReportMode = skipDefineReport;

            let skipEstablishCommunication = self._instance.Attributes.get("AutomationEquipmentSkipEstablishCommunication");
            if (skipEstablishCommunication == null) {
                skipEstablishCommunication = false;
                this._logger.info(`AutomationEquipmentSkipEstablishCommunication: ${skipEstablishCommunication} (default value)`);
            } else {
                this._logger.info(`AutomationEquipmentSkipEstablishCommunication: ${skipEstablishCommunication}`);
            }
            // await this._dataStore.store(prefix + "_" + "AutomationEquipmentSkipEstablishCommunication",
            // skipEstablishCommunication, System.DataStoreLocation.Persistent);
            driverSetupDefinition.AutomationEquipmentSkipEstablishCommunication = skipEstablishCommunication;

            let skipLinkEvents = self._instance.Attributes.get("AutomationEquipmentSkipLinkEvents");
            if (skipLinkEvents == null) {
                skipLinkEvents = false;
                this._logger.info(`AutomationEquipmentSkipLinkEvents: ${skipLinkEvents} (default value)`);
            } else {
                this._logger.info(`AutomationEquipmentSkipLinkEvents: ${skipLinkEvents}`);
            }
            // await this._dataStore.store(prefix + "_" + "AutomationEquipmentSkipLinkEvents", skipLinkEvents, System.DataStoreLocation.Persistent)
            driverSetupDefinition.AutomationEquipmentSkipLinkEvents = skipLinkEvents;

            let skipSetOnline = self._instance.Attributes.get("AutomationEquipmentSkipSetOnline");
            if (skipSetOnline == null) {
                skipSetOnline = false;
                this._logger.info(`AutomationEquipmentSkipSetOnline: ${skipSetOnline} (default value)`);
            } else {
                this._logger.info(`AutomationEquipmentSkipSetOnline: ${skipSetOnline}`);
            }
            // await this._dataStore.store(prefix + "_" + "AutomationEquipmentSkipSetOnline", skipSetOnline, System.DataStoreLocation.Persistent)
            driverSetupDefinition.AutomationEquipmentSkipSetOnline = skipSetOnline;

            let waitOnSetupTimeoutAndRetry = self._instance.Attributes.get("AutomationEquipmentWaitOnSetupTimeoutAndRetryIfErrorOccurs");
            if (waitOnSetupTimeoutAndRetry == null) {
                waitOnSetupTimeoutAndRetry = false;
                this._logger.info(`AutomationEquipmentWaitOnSetupTimeoutAndRetryIfErrorOccurs: ${waitOnSetupTimeoutAndRetry} (default value)`);
            } else {
                this._logger.info(`AutomationEquipmentWaitOnSetupTimeoutAndRetryIfErrorOccurs: ${waitOnSetupTimeoutAndRetry}`);
            }
            // await this._dataStore.store(prefix + "_" + "AutomationEquipmentWaitOnSetupTimeoutAndRetryIfErrorOccurs",
            // waitOnSetupTimeoutAndRetry, System.DataStoreLocation.Persistent);
            driverSetupDefinition.AutomationEquipmentWaitOnSetupTimeoutAndRetryIfErrorOccurs = waitOnSetupTimeoutAndRetry;

            let enableDisableAlarmMode = self._instance.Attributes.get("AutomationEquipmentEnableDisableAlarmsMode");
            if (enableDisableAlarmMode == null) {
                enableDisableAlarmMode = "DisableAllEnableSelection";
                this._logger.info(`AutomationEquipmentEnableDisableAlarmsMode: ${enableDisableAlarmMode} (default value)`);
            } else {
                this._logger.info(`AutomationEquipmentEnableDisableAlarmsMode: ${enableDisableAlarmMode}`);
            }
            // await this._dataStore.store(prefix + "_" + "AutomationEquipmentEnableDisableAlarmsMode", enableDisableAlarmMode,
            // System.DataStoreLocation.Persistent);
            driverSetupDefinition.AutomationEquipmentEnableDisableAlarmsMode = enableDisableAlarmMode;

            let enableDisableEventMode = self._instance.Attributes.get("AutomationEquipmentEnableDisableEventsMode");
            if (enableDisableEventMode == null) {
                enableDisableEventMode = "DisableAllEnableSelection";
                this._logger.info(`AutomationEquipmentEnableDisableEventsMode: ${enableDisableEventMode} (default value)`);
            } else {
                this._logger.info(`AutomationEquipmentEnableDisableEventsMode: ${enableDisableEventMode}`);
            }
            // await this._dataStore.store(prefix + "_" + "AutomationEquipmentEnableDisableEventsMode", enableDisableEventMode,
            // System.DataStoreLocation.Persistent);
            driverSetupDefinition.AutomationEquipmentEnableDisableEventsMode = enableDisableEventMode;

            let deleteReportMode = self._instance.Attributes.get("AutomationEquipmentDeleteReportMode");
            if (deleteReportMode == null) {
                deleteReportMode = "DeleteAllCreatedReports";
                this._logger.info(`AutomationEquipmentEnableDisableEventsMode: ${deleteReportMode} (default value)`);
            } else {
                this._logger.info(`AutomationEquipmentEnableDisableEventsMode: ${deleteReportMode}`);
            }
            // await this._dataStore.store(prefix + "_" + "AutomationEquipmentDeleteReportMode", deleteReportMode, System.DataStoreLocation.Persistent)
            driverSetupDefinition.AutomationEquipmentDeleteReportMode = deleteReportMode;

            // attribute to emit to setup secs/gem
            const equipmentAddress = self._instance.Attributes.get("AutomationEquipmentAddress");
            driverSetupDefinition.AutomationEquipmentAddress = equipmentAddress;
            this._logger.info(`AutomationEquipmentAddress: ${equipmentAddress}`);

            const deviceId = self._instance.Attributes.get("AutomationEquipmentDeviceId");
            driverSetupDefinition.AutomationEquipmentDeviceId = deviceId;
            this._logger.info(`AutomationEquipmentDeviceId: ${deviceId}`);

            const ipPort = self._instance.Attributes.get("AutomationEquipmentIPPort");
            driverSetupDefinition.AutomationEquipmentIPPort = ipPort;
            this._logger.info(`AutomationEquipmentIPPort: ${ipPort}`);

            const customLibrary = self._instance.Attributes.get("AutomationEquipmentCustomLibrary");
            driverSetupDefinition.AutomationEquipmentCustomLibrary = customLibrary;
            this._logger.info(`AutomationEquipmentCustomLibrary: ${customLibrary}`);

            const serialPortName = self._instance.Attributes.get("AutomationEquipmentSerialPortName");
            driverSetupDefinition.AutomationEquipmentSerialPortName = serialPortName;
            this._logger.info(`AutomationEquipmentSerialPortName: ${serialPortName}`);

            const serialBaudRate = self._instance.Attributes.get("AutomationEquipmentSerialBaudRate");
            driverSetupDefinition.AutomationEquipmentSerialBaudRate = serialBaudRate;
            this._logger.info(`AutomationEquipmentSerialBaudRate: ${serialBaudRate}`);

            await this._setupStore.setSetupDefinition(prefix, driverSetupDefinition);
            // flags to reset
            // ConnectionEstablished
            await this._setupStore.setTempValue(prefix, "_ConnectionEstablished", false);
            // SetupFirstRun
            await this._setupStore.setTempValue(prefix, "_SetupFirstRun", true);
            // ConnectionEstablish
            await this._setupStore.setTempValue(prefix, "_ConnectionEstablish", false);

            this.customLibrary.emit(customLibrary)
            this.equipmentIPAddress.emit(equipmentAddress)
            this.equipmentIPPort.emit(ipPort)
            this.equipmentSerialPortName.emit(serialPortName)
            this.equipmentBaudRate.emit(serialBaudRate)
            this.equipmentDeviceId.emit(deviceId)
            this.deleteReportMode.emit(deleteReportMode.replace("UnlinkDefinedReportsOnly", "").replace("BulkLink", ""));
            this.enableDisableEventMode.emit(enableDisableEventMode === "EnableAll" ? "DisableAllEnableSelection" : enableDisableEventMode);
            this.enableDisableAlarmMode.emit(enableDisableAlarmMode === "EnableAll" ? "DisableAllEnableSelection" : enableDisableAlarmMode);

            // emit output
            self.instance.emit(self._instance);

        }
    }

    /**
     * On BeforeInit logic: Makes dynamic outputs available.
     */
    public async onBeforeInit(): Promise<void> {
        if (this.attributes) {
            for (const output of this.attributes) {
                this[`${output.Name}`] = new Task.Output<any>();
            }
        }
    }

    /**
     * On Init, registers the callback for receiving the instance
     */
    public async onInit(): Promise<void> {
        if (this.driverName == null) {
            // Default for controller
            this.driverName = "";

            // Calculate the value based on the existence (or not) of a driver proxyin the task container
            if (this._taskContainer.isBound(TYPES.System.Driver)) {
                const driverProxy = this._taskContainer.get<System.DriverProxy>(TYPES.System.Driver);
                this.driverName = (<any>driverProxy.automationControllerDriverDefinition).DisplayName;
            }
        }

        if (this.driverName != null && this._instanceProxy != null) {
            this._instanceCallBack = this._instanceProxy.subscribe(this.driverName, this.instanceCallBack);
        }
    }

    /**
     * On destroy, unsubscribe the instanceProxy.
     */
    public async onDestroy(): Promise<void> {
        if (this._instanceProxy != null && this._instanceCallBack != null) {
            this._instanceProxy.unsubscribe(this.driverName, this._instanceCallBack);
            this._instanceCallBack = <any>null;
        }
    }
    // #endregion
}

/** Task Settings */
export interface CustomLoadSetupConfigurationSettings {
   [key: string]: any;
}
