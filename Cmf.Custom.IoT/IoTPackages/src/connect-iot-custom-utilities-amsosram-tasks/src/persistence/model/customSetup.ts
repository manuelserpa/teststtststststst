import { CustomSetupStatesEnum } from "../../utilities/setupStatesEnum";

export interface DriverSetupDefinition {
    /** Driver Name */
    DriverName: String,
    /** Establish Communication Definition */
    AutomationEquipmentSkipEstablishCommunication: Boolean,
    /** Set Online Definition */
    AutomationEquipmentSkipSetOnline: Boolean,
    /** Report and Report Linking Definition */
    AutomationEquipmentSkipDefineReportMode: Boolean,
    AutomationEquipmentDeleteReportMode: String,
    AutomationEquipmentSkipLinkEvents: Boolean,
    /** Events Enabling definition*/
    AutomationEquipmentEnableDisableEventsMode: String,
    /** Alarms Enabling definition */
    AutomationEquipmentEnableDisableAlarmsMode: String,
    /** Custom Setup Definitions */
    AutomationEquipmentWaitOnSetupTimeoutAndRetryIfErrorOccurs: Boolean,
    /** Persisted flags for setup*/
    InternalState: CustomSetupStatesEnum,
    WaitForAdditionalActions: Boolean,
    /** Communication configurations */
    AutomationEquipmentAddress: String;
    AutomationEquipmentDeviceId: Number;
    AutomationEquipmentIPPort: Number;
    AutomationEquipmentCustomLibrary: String;
    AutomationEquipmentSerialPortName: String;
    AutomationEquipmentSerialBaudRate: Number;
}
