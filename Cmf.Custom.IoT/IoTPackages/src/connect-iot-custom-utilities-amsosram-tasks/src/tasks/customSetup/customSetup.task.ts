import { Task, Dependencies, System, DI, TYPES } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customSetup.default";
import { CustomSetupStoreHandler } from "../../persistence/implementation/customSetupStoreHandler";
import { CustomSetupStatesEnum } from "../../utilities/setupStatesEnum";
import { SecsTransaction, Message } from "../../common/secsTransaction";
import { Console } from "console";
import { threadId } from "worker_threads";
import { rejects } from "assert";
import { DriverSetupDefinition } from "../../persistence/model/customSetup";

/**
 * @whatItDoes
 *
 * Implements Complx Secs Gem Commands
 *
 * @howToUse
 *
 * ### Inputs
 * *
 * * `any` : **activate** - Activate the task
 *
 * ### Outputs

 * * `bool`  : ** success ** - Triggered when the the task is executed with success
 * * `Error` : ** error ** - Triggered when the task failed for some reason
 *
 * ### Settings
 * See {@see CustomSetupSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-secsgem-iot-lg-setrecipe",
    inputs: {
        activate: Task.INPUT_ACTIVATE,
    },
    outputs: {
        SetupSuccessfully: Task.TaskValueType.Boolean,
        SetupFirstRun: Task.TaskValueType.Boolean,
        SetupHeartbeatFailed: Task.TaskValueType.Boolean,
        SetupErrorMessage: Task.TaskValueType.String,
        Success: Task.OUTPUT_SUCCESS,
        Error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.SecsGem
})
export class CustomSetupTask implements Task.TaskInstance, CustomSetupSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    public establishCommunicationMessageObj: object;
    public setOnlineCommunicationMessageObj: object;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;

    public inputs: CustomSetupInputSettings[] = [];
    public outputs: CustomSetupOutputSettings[] = [];

    /** **Outputs** */
    /** To output a success notification */
    public Success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public Error: Task.Output<Error> = new Task.Output<Error>();

    public SetupSuccessfully: Task.Output<boolean> = new Task.Output<boolean>();
    public SetupFirstRun: Task.Output<boolean> = new Task.Output<boolean>();
    public SetupHeartbeatFailed: Task.Output<boolean> = new Task.Output<boolean>();
    public SetupErrorMessage: Task.Output<string> = new Task.Output<string>();

    /** Settings */
    /** Properties Settings */
    /** Establish Communication body of message */
    establishCommunicationMessageStr: string = "{ \"type\": \"S1F13\", \"item\": { \"type\": \"L\", \"value\": [] } }";
    /** Success Codes to evaluate on reply message. If empty, any value is a success */
    establishCommunicationSuccessCodes: string = "0x00";
    /** Establish Communication Additional Actions */
    establishCommunicationAdditionalActions: boolean = false;
    /** Establish Communication Additional Actions Timeout */
    establishCommunicationAdditionalActionsTimeout: number = 30;

    /** Set Online Communication body of message */
    setOnlineCommunicationMessageStr: string = "{ \"type\": \"S1F17\", \"item\": {} }";
    /** Success Codes to evaluate on reply message. If empty, any value is a success */
    setOnlineSuccessCodes: string = "0x00,0x02";
    /** Set Online Additional Actions */
    setOnlineAdditionalActions: boolean = false;
    /** Set Online Additional Actions Timeout */
    setOnlineAdditionalActionsTimeout: number = 30;
    /** Delete Reports Additional Actions */

    deleteReportsAdditionalActions: boolean = false;
    /** Delete Reports Timeout */
    deleteReportsTimeout: number = 60;
    /** Delete Reports Additional Actions Timeout */
    deleteReportsAdditionalActionsTimeout: number = 30;

    /** Internal Define Reports Additional Actions */
    internalDefineReportsAdditionalActions: boolean = false;
    /** Internal Define Reports Timeout */
    internalDefineReportsTimeout: number = 60;
    /** Internal Define Reports Additional Actions Timeout */
    internalDefineReportsAdditionalActionsTimeout: number = 30;

    /** Link Events Additional Actions */
    linkEventsAdditionalActions: boolean = false;
    /** Link Events Timeout */
    linkEventsTimeout: number = 60;
    /** Link Events Additional Actions Timeout */
    linkEventsAdditionalActionsTimeout: number = 30;
    /** Success Codes to evaluate on reply message. If empty, any value is a success */
    linkEventsSuccessCodes: string = "0x00";

    /** Enable Disable Events Additional Actions */
    enableDisableEventsAdditionalActions: boolean = false;
    /** Enable Disable Events Additional Actions Timeout */
    enableDisableEventsTimeout: number = 60;
    /** Enable Disable Events Additional Actions Timeout */
    enableDisableEventsAdditionalActionsTimeout: number = 30;

    /** Enable Disable Alarms Additional Actions */
    enableDisableAlarmsAdditionalActions: boolean = false;
    /** Enable Disable Alarms Timeout */
    enableDisableAlarmsTimeout: number = 60;
    /** Enable Disable Alarms Additional Actions Timeout */
    enableDisableAlarmsAdditionalActionsTimeout: number = 30;

    /** Wait Time Between Retries */
    waitTimeBetweenRetries: number = 5;
    /** Reset Setup on Establish Communication received from Equipment */
    resetSetupOnEstablishCommunicationReceived: boolean = true;

    /** Heartbeat */
    heartbeatSxFy: string = "S1F1";
    heartbeatBody: string = "{}";
    heartbeatTimeout: number = 0;

    driverName: string = null;
    driverSetupDefinition: DriverSetupDefinition = null;

    private toAdditionalActions: NodeJS.Timeout = null;

    private toRetry: NodeJS.Timeout = null;

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject(TYPES.System.Driver)
    private _driverProxy: System.DriverProxy;

    @DI.Inject("GlobalCustomSetupStoreHandler")
    private _setupDataStore: CustomSetupStoreHandler;

    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {

        this.driverName = this._driverProxy.automationControllerDriverDefinition.DisplayName;

        let internalState = await this._setupDataStore.getInternalState(this.driverName);
        const isWaitingForAdditionalActions =
            await this._setupDataStore.getWaitForAdditionalActions(this.driverName);

        try {
            if (changes["activate"]) {
                if (!this.driverSetupDefinition) {
                    this.driverSetupDefinition = await this._setupDataStore.getSetupDefinition(this.driverName);
                    // creates object based on old persistency configuration to support older workflows
                    if (!this.driverSetupDefinition) {
                        this._logger.warning("Using old configuration, consider using task customLoadSetupConfiguration");
                        this.driverSetupDefinition = {} as DriverSetupDefinition;
                        this.driverSetupDefinition.AutomationEquipmentSkipEstablishCommunication =
                            await this._setupDataStore.getValue(
                                "AutomationEquipmentWaitOnSetupTimeoutAndRetryIfErrorOccurs", this.driverName, false);
                        this.driverSetupDefinition.AutomationEquipmentSkipEstablishCommunication =
                            await this._setupDataStore.getValue(
                                "AutomationEquipmentSkipEstablishCommunication", this.driverName, false);
                        this.driverSetupDefinition.AutomationEquipmentSkipDefineReportMode =
                            await this._setupDataStore.getValue("AutomationEquipmentSkipSetOnline", this.driverName, false);
                        this.driverSetupDefinition.AutomationEquipmentSkipDefineReportMode =
                            await this._setupDataStore.getValue(
                                "AutomationEquipmentSkipDefineReportMode", this.driverName, false);
                        this.driverSetupDefinition.AutomationEquipmentSkipDefineReportMode =
                            await this._setupDataStore.getValue(
                                "AutomationEquipmentSkipDefineReportMode", this.driverName, false);
                        this.driverSetupDefinition.AutomationEquipmentDeleteReportMode =
                            await this._setupDataStore.getValue("AutomationEquipmentDeleteReportMode", this.driverName, null);
                        this.driverSetupDefinition.AutomationEquipmentDeleteReportMode =
                            await this._setupDataStore.getValue("AutomationEquipmentDeleteReportMode", this.driverName, null);
                        this.driverSetupDefinition.AutomationEquipmentEnableDisableAlarmsMode =
                            await this._setupDataStore.getValue(
                                "AutomationEquipmentEnableDisableAlarmsMode", this.driverName, "DisableAllEnableSelection");
                    }
                }
            }
            if (changes["activate"] && internalState !== CustomSetupStatesEnum.EstablishCommunication) {
                await this.ResetTask();
            }

            if (!isWaitingForAdditionalActions && internalState === CustomSetupStatesEnum.EstablishCommunication && changes["activate"]) {
                await this.ExecuteCurrentStep(internalState);
            }

            if (isWaitingForAdditionalActions) {
                if (this.CheckIfChangesIsValid(internalState, changes)) {
                    clearTimeout(this.toAdditionalActions);
                    this.toAdditionalActions = null;
                    if (this.GetChangesValue(internalState, changes)) {
                        await this.PassToNextStep(internalState);
                        internalState = await this._setupDataStore.getInternalState(this.driverName);
                        await this.ExecuteCurrentStep(internalState);
                    } else {
                        await this.ResetTask()
                    }
                }
            }
        } catch (error) {
            this._logger.error("\n Error:" + JSON.stringify(error));
            // throw new Error(error);
        }
        finally {
            this.activate = undefined;
            const arrayProperties = Object.getOwnPropertyNames(this).filter(c => c.endsWith("_in"));
            for (let i = 0; i < arrayProperties.length; i++) {
                this[arrayProperties[i]] = undefined;
            }
        }
    }

    private async PassToNextStep(currentStep: CustomSetupStatesEnum) {
        await this._setupDataStore.setInternalState(this.driverName, currentStep + 1);
    }

    private async ExecuteCurrentStep(currentStep: CustomSetupStatesEnum) {

        try {
            let executionDone = false;
            switch (currentStep) {
                case CustomSetupStatesEnum.EstablishCommunication:
                    executionDone = await this.ExecuteEstablishCommunication();
                    break;
                case CustomSetupStatesEnum.SetOnline:
                    executionDone = await this.ExecuteSetOnline();
                    break;
                case CustomSetupStatesEnum.DeleteReports:
                    executionDone = await this.ExecuteDeleteReports();
                    break;
                case CustomSetupStatesEnum.InternalDefineReports:
                    executionDone = await this.ExecuteInternalDefineReports();
                    break;
                case CustomSetupStatesEnum.LinkEvents:
                    executionDone = await this.ExecuteLinkEvents();
                    break;
                case CustomSetupStatesEnum.EnableDisableEvents:
                    executionDone = await this.ExecuteEnableDisableEvents();
                    break;
                case CustomSetupStatesEnum.EnableDisableAlarms:
                    executionDone = await this.ExecuteEnableDisableAlarms();
                    break;
                case CustomSetupStatesEnum.END:
                    executionDone = await this.FinishConnection();
                    break;
                default:
                    break;
            }


            if (executionDone && currentStep !== CustomSetupStatesEnum.END) {

                await this.VerifyIfAdditionalActionIsActive(currentStep);
                const waitForAdditionalAction =
                    await this._setupDataStore.getWaitForAdditionalActions(this.driverName);

                if (!waitForAdditionalAction) {
                    await this.PassToNextStep(currentStep);
                    const internalState = await this._setupDataStore.getInternalState(this.driverName);
                    await this.ExecuteCurrentStep(internalState);
                }
            }

            if (executionDone && currentStep === CustomSetupStatesEnum.END) {
                await this._setupDataStore.setInternalState(this.driverName, 0);
                await this.ResetTask(false);
            }
        } catch (error) {
            await this.TreatError(error, currentStep)
        }
    }

    private async TreatError(error: Error, currentStep: CustomSetupStatesEnum) {
        const hasRetry = this.driverSetupDefinition.AutomationEquipmentWaitOnSetupTimeoutAndRetryIfErrorOccurs;
        if (hasRetry) {
            if (await this._setupDataStore.getValue("SetupFirstRun", this.driverName, true)) {
                this.SetupFirstRun.emit(true);
            }
            this.SetupErrorMessage.emit(error.stack);
            await this._setupDataStore.setTempValue("SetupFirstRun", this.driverName, false);

            if (error["isAdditionalActionTimeout"]) {
                await this.ResetTask();
            } else {
                await this.ResetTask(false);
                setTimeout(async () => {
                    await this.ExecuteCurrentStep(currentStep);
                }, this.waitTimeBetweenRetries * 1000);
            }

        } else {
            await this._setupDataStore.setInternalState(this.driverName, 0);
            await this.ResetTask(false);
            this.Error.emit(error);
        }

    }

    private async VerifyIfAdditionalActionIsActive(currentStep: CustomSetupStatesEnum) {
        const stringState = CustomSetupStatesEnum[currentStep];
        const additionalAction = this[((stringState).charAt(0).toLowerCase() + stringState.slice(1) + "AdditionalActions")];

        const timeoutProperty = (stringState).charAt(0).toLowerCase() + stringState.slice(1) + "AdditionalActionsTimeout";

        if (additionalAction === true) {
            const uuid = this.GenerateUniqSerial();
            await this._setupDataStore.setTempValue(
                CustomSetupStatesEnum[currentStep] + "AdditionalActions", this.driverName, uuid);
            this[CustomSetupStatesEnum[currentStep] + "AdditionalActions"].emit(uuid);
            await this._setupDataStore.setWaitForAdditionalActions(this.driverName, true);
            this.toAdditionalActions = setTimeout(async () => {
                this._logger.error(`Timeout: ${stringState}`);
                const error = new Error(`Timeout: ${stringState}`);
                error["isAdditionalActionTimeout"] = true;
                await this.TreatError(error, currentStep);
            }, this[timeoutProperty] * 1000);
        } else {
            await this._setupDataStore.setWaitForAdditionalActions(this.driverName, false);
        }
    }


    private CheckIfChangesIsValid(currentStep: CustomSetupStatesEnum, changes: Task.Changes): boolean {
        const additionalActionToVerify = CustomSetupStatesEnum[currentStep];
        if (changes[(additionalActionToVerify) + "AdditionalActions_in"]) {
            this[(additionalActionToVerify) + "AdditionalActions_in"] = undefined;
            return true;
        }

        return false;
    }

    private GetChangesValue(currentStep: CustomSetupStatesEnum, changes: Task.Changes): boolean {
        const additionalActionToVerify = CustomSetupStatesEnum[currentStep];
        if (changes[(additionalActionToVerify) + "AdditionalActions_in"]) {
            return (<boolean>changes[(additionalActionToVerify) + "AdditionalActions_in"].currentValue);
        }
    }

    private async ResetTask(executeCurrentStep: boolean = true) {
        this._logger.warning("CustomSetupTask was Reset.");
        let internalState = await this._setupDataStore.getInternalState(this.driverName);
        if (internalState !== CustomSetupStatesEnum.EstablishCommunication && internalState !== CustomSetupStatesEnum.SetOnline) {
            internalState = 1;
        }
        await this._setupDataStore.setInternalState(this.driverName, internalState);
        await this._setupDataStore.setWaitForAdditionalActions(this.driverName, false);
        clearTimeout(this.toAdditionalActions);
        this.toAdditionalActions = null;
        const currentStep = await this._setupDataStore.getInternalState(this.driverName);
        if (executeCurrentStep) {
            await this.ExecuteCurrentStep(currentStep);
        }

    }

    private async ExecuteEstablishCommunication(): Promise<boolean> {


        const AutomationEquipmentSkipEstablishCommunication = this.driverSetupDefinition.AutomationEquipmentSkipEstablishCommunication;

        const ConnectionEstablish =
            await this._setupDataStore.getValue("ConnectionEstablish", this.driverName, false);

        if (!AutomationEquipmentSkipEstablishCommunication && !ConnectionEstablish) {
            // unsubscribe from establish communication request to avoid issues
            if (this.resetSetupOnEstablishCommunicationReceived) {
                this.unsubscribeEstablishCommunication();
            }
            // prepare message
            const sendMessage: Object = JSON.parse(this.establishCommunicationMessageStr);

            const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);

            let successFound = false;
            for (const successCode of this.establishCommunicationSuccessCodes.split(",")) {
                if (reply && reply.item && parseInt(reply.item.value[0].value.data) === parseInt(successCode.trim())) {
                    successFound = true;
                    await this._setupDataStore.setTempValue("ConnectionEstablish", this.driverName, true);
                    break;
                }
            }

            if (!successFound) {
                const error = new Error("Failed to Establish Communication, equipment reported reply code: "
                    + reply.item.value[0].value.data.toString());
                this._logger.error(error.message);
                throw error;
            } else {
                if (this.resetSetupOnEstablishCommunicationReceived) {
                    await this.subscribeEstablishCommunication();
                }
                return true;
            }

        }

        if (ConnectionEstablish) {
            this._logger.warning("\n\n Connection already established S1F13, not resending message...");
            return true;
        }

        return false;

    }

    private async ExecuteSetOnline(): Promise<boolean> {

        // Execute Set Online (S1F17)
        const AutomationEquipmentSkipSetOnline = this.driverSetupDefinition.AutomationEquipmentSkipSetOnline;
        if (!AutomationEquipmentSkipSetOnline) {

            const sendMessage: Object = JSON.parse(this.setOnlineCommunicationMessageStr);

            const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);

            // validate result codes
            let successFound = false;
            for (const successCode of this.setOnlineSuccessCodes.split(",")) {
                if (reply && reply.item && parseInt(reply.item.value.data) === parseInt(successCode.trim())) {
                    successFound = true;
                    break;
                }
            }

            if (!successFound) {
                const error = new Error("Failed to Set Online, equipment reported reply code: "
                    + reply.item.value.data);
                this._logger.error(error.message);
                throw error;
            } else {
                return true;
            }
        } else {
            this._logger.warning("AutomationEquipmentSkipSetOnline: true");
            return true;
        }
    }

    private async ExecuteDeleteReports(): Promise<boolean> {

        // execute product internal delete reports
        await this._driverProxy.sendRaw("connect.iot.driver.secsgem.internalDeleteReports", undefined, this.deleteReportsTimeout * 1000);
        return true;
    }

    private async ExecuteInternalDefineReports(): Promise<boolean> {

        // verify if report definition is intended
        const AutomationEquipmentSkipDefineReportMode = this.driverSetupDefinition.AutomationEquipmentSkipDefineReportMode;
        if (!AutomationEquipmentSkipDefineReportMode) {
            // Execute product internal define reports
            await this._driverProxy.sendRaw("connect.iot.driver.secsgem.internalDefineReports", undefined, this.internalDefineReportsTimeout * 1000);
        }

        return true;
    }

    private async ExecuteLinkEvents(): Promise<boolean> {
        // verify if event link is intended
        const AutomationEquipmentSkipLinkEvents = this.driverSetupDefinition.AutomationEquipmentSkipLinkEvents;
        if (!AutomationEquipmentSkipLinkEvents) {
            const AutomationEquipmentDeleteReportMode = this.driverSetupDefinition.AutomationEquipmentDeleteReportMode;
            if (!AutomationEquipmentDeleteReportMode ||
                (!AutomationEquipmentDeleteReportMode.toString().includes("UnlinkDefinedReportsOnly") &&
                    !AutomationEquipmentDeleteReportMode.toString().includes("BulkLink"))) {
                // execute product internal link
                await this._driverProxy.sendRaw("connect.iot.driver.secsgem.internalLinkEvents", undefined, this.linkEventsTimeout * 1000);
            } else {
                const eventIdType = (<any>this._driverProxy)._driver._currentIntegrationConfiguration.DeviceConfiguration.communication.eventIdType;
                const reportIdType = (<any>this._driverProxy)._driver._currentIntegrationConfiguration.DeviceConfiguration.communication.reportIdType;
                const dataIdType = (<any>this._driverProxy)._driver._currentIntegrationConfiguration.DeviceConfiguration.communication.dataIdType;
                // execute custom link of events code
                // get flags
                const bulkLink = AutomationEquipmentDeleteReportMode.toString().includes("BulkLink");
                const unlinkDefinedReportsOnly = AutomationEquipmentDeleteReportMode.toString().includes("UnlinkDefinedReportsOnly");

                const _eventReportsLink: Map<string, string[]> = new Map<string, string[]>();
                let nextDataItemId = 1;
                let nextSequentialReportId = 1;

                // get report and event information
                try {

                    for (const event of (<any>this._driverProxy)._driver._currentIntegrationConfiguration.DeviceConfiguration.events) {

                        // Define a sequential reportId based on the already defined
                        if (event.extendedData && event.extendedData.isAlarm === false) {
                            continue;
                        }
                        nextSequentialReportId++;

                        const reportsDefined: Map<string, string[]> = new Map<string, string[]>();
                        for (const variable of event.properties) {
                            const reportId = (variable.extendedData).reportId || nextSequentialReportId.toString();
                            if (!reportsDefined.has(reportId)) {
                                reportsDefined.set(reportId, []);
                            }
                        }
                        _eventReportsLink.set(event.deviceId, Array.from(reportsDefined.keys()));
                    }

                    if (bulkLink) {
                        // execute bulk link

                        // build Unlink Events bulk message
                        // Unlink
                        const unlinkList = [];
                        // let reply = await this.sendAndWaitReply(
                        const unlinkRequest = {
                            type: "S2F35", item:
                            {
                                type: "L", value: [
                                    { type: dataIdType, name: "DATAID", value: nextDataItemId },
                                    { type: "L", value: unlinkList }
                                ]
                            }
                        };
                        nextDataItemId++;
                        // link
                        const linkList = [];
                        const linkRequest = {
                            type: "S2F35", item:
                            {
                                type: "L", value: [
                                    { type: dataIdType, name: "DATAID", value: nextDataItemId },
                                    { type: "L", value: linkList }
                                ]
                            }
                        };

                        for (const eventId of Array.from(_eventReportsLink.keys())) {
                            if (!unlinkDefinedReportsOnly) {
                                // if all events are to be unlinked
                                // bulk Unlink message creation
                                unlinkList.push({
                                    type: "L", value: [
                                        { type: eventIdType, name: "CEID", value: eventId },
                                        { type: "L", value: [] },
                                    ]
                                });
                            }

                            const reports = _eventReportsLink.get(eventId);
                            const reportList = [];

                            if (reports) {
                                for (const report of reports) {
                                    reportList.push({ type: reportIdType, name: "RPTID", value: report });
                                }
                            }

                            if (reportList.length > 0) {
                                if (unlinkDefinedReportsOnly) {
                                    // if only defined events are to be unlinked
                                    // bulk Unlink message creation
                                    unlinkList.push({
                                        type: "L", value: [
                                            { type: eventIdType, name: "CEID", value: eventId },
                                            { type: "L", value: [] },
                                        ]
                                    });
                                }
                                // bulk link message creation
                                // adding list
                                linkList.push({
                                    type: "L", value: [
                                        { type: eventIdType, name: "CEID", value: eventId },
                                        { type: "L", value: reportList },
                                    ]
                                });
                            }
                        }
                        // unlink request
                        const unlinkReply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", unlinkRequest);

                        const successCodes = "0x00"
                        let unlinkSuccessFound = false;
                        for (const successCode of this.linkEventsSuccessCodes.split(",")) {
                            if (unlinkReply && unlinkReply.item && parseInt(unlinkReply.item.value.data) === parseInt(successCode.trim())) {
                                unlinkSuccessFound = true;
                                break;
                            }
                        }

                        if (!unlinkSuccessFound) {
                            const error = new Error(`UnlinkEvent (S2F35) failed`);
                            throw error;
                        }

                        // link request
                        const linkReply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", linkRequest);
                        let linkSuccessFound = false;
                        for (const successCode of this.linkEventsSuccessCodes.split(",")) {
                            if (linkReply && linkReply.item && parseInt(linkReply.item.value.data) === parseInt(successCode.trim())) {
                                linkSuccessFound = true;
                                break;
                            }
                        }
                        if (!linkSuccessFound) {
                            const error = new Error(`LinkEvent (S2F35) failed`);
                            throw error;
                        }
                    } else {
                        // link event and reports one by one
                        for (const eventId of Array.from(_eventReportsLink.keys())) {
                            //#region Link
                            const reports = _eventReportsLink.get(eventId);
                            const reportList = [];

                            if (reports) {
                                for (const report of reports) {
                                    reportList.push({ type: reportIdType, name: "RPTID", value: report });
                                }
                            }

                            if (reportList.length > 0 || !unlinkDefinedReportsOnly) {
                                //#region Unlink
                                const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", {
                                    type: "S2F35", item:
                                    {
                                        type: "L", value: [
                                            { type: dataIdType, name: "DATAID", value: nextDataItemId },
                                            {
                                                type: "L", value: [
                                                    {
                                                        type: "L", value: [
                                                            { type: eventIdType, name: "CEID", value: eventId },
                                                            { type: "L", value: [] },
                                                        ]
                                                    }, // L
                                                ]
                                            } // L
                                        ]
                                    } // L
                                });

                                const successCodes = "0x00"
                                let unlinkSuccessFound = false;
                                for (const successCode of this.linkEventsSuccessCodes.split(",")) {
                                    if (reply && reply.item && parseInt(reply.item.value.data) === parseInt(successCode.trim())) {
                                        unlinkSuccessFound = true;
                                        break;
                                    }

                                    nextDataItemId++;
                                }

                                if (!unlinkSuccessFound) {
                                    const error = new Error(`UnlinkEvent (S2F35) failed`);
                                    throw error;
                                }
                            }
                            //#endregion

                            if (reportList.length > 0) {
                                // Send the message
                                const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", {
                                    type: "S2F35", item:
                                    {
                                        type: "L", value: [
                                            { type: dataIdType, name: "DATAID", value: nextDataItemId },
                                            {
                                                type: "L", value: [
                                                    {
                                                        type: "L", value: [
                                                            { type: eventIdType, name: "CEID", value: eventId },
                                                            { type: "L", value: reportList },
                                                        ]
                                                    },
                                                ]
                                            }
                                        ]
                                    }
                                });

                                let linkSuccessFound = false;
                                for (const successCode of this.linkEventsSuccessCodes.split(",")) {
                                    if (reply && reply.item && parseInt(reply.item.value.data) === parseInt(successCode.trim())) {
                                        linkSuccessFound = true;
                                        break;
                                    }
                                }
                                if (!linkSuccessFound) {
                                    const error = new Error(`LinkEvent (S2F35) failed`);
                                    throw error;
                                }
                                nextDataItemId++;
                            }
                            //#endregion
                        }
                    }

                } catch (error) {
                    const e = new Error(`Link/Unlink Events (S2F35) failed: ${error.message}`);
                    throw e;
                }
            }
        }
        return true;
    }

    private async ExecuteEnableDisableEvents(): Promise<boolean> {
        // Get event enable disable mode
        const AutomationEquipmentEnableDisableEventsMode = this.driverSetupDefinition.AutomationEquipmentEnableDisableEventsMode;
        // if enable all then execute custom code
        if (AutomationEquipmentEnableDisableEventsMode === "EnableAll") {
            const message = {
                type: "S2F37", item:
                {
                    type: "L", value: [
                        { type: "BO", name: "CEED", value: true },
                        { type: "L", value: [] }
                    ]
                }
            };
            await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", message);

        } else {
            // else use internal enable disable event
            await this._driverProxy.sendRaw("connect.iot.driver.secsgem.internalEnableDisableEvents", undefined, this.enableDisableEventsTimeout * 1000);

        }


        return true;
    }

    private async ExecuteEnableDisableAlarms(): Promise<boolean> {

        // Get alarm mode
        const AutomationEquipmentEnableDisableAlarmsMode = this.driverSetupDefinition.AutomationEquipmentEnableDisableAlarmsMode;
        // Get alarm Id type from driver
        const AutomationEquipmentAlarmTypeId = (<any>this._driverProxy)._driver._currentIntegrationConfiguration.DeviceConfiguration.communication.alarmIdType;

        this._logger.warning("AutomationEquipmentEnableDisableAlarmsMode:" + AutomationEquipmentEnableDisableAlarmsMode)

        // If enable all alarms is intended execute custom logic
        if (AutomationEquipmentEnableDisableAlarmsMode === "EnableAll") {
            const message = {
                type: "S5F3", item:
                {
                    type: "L", value: [
                        { type: "BI", name: "ALED", value: 0x80 }, // From SEMI E5, bit 8 = 1 means enable alarm
                        { type: AutomationEquipmentAlarmTypeId, value: undefined }
                    ]
                }
            };
            await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", message);
        } else if (AutomationEquipmentEnableDisableAlarmsMode === "EnableDisableNothing") {
            this._logger.info('EnableDisableAlarms disabled in settings');
            return true;
        } else {
            // else execute product logic
            await this._driverProxy.sendRaw("connect.iot.driver.secsgem.internalEnableDisableAlarms", undefined, this.enableDisableAlarmsTimeout * 1000);

        }
        return true;
    }

    private async subscribeEstablishCommunication() {
        try {

            await this._driverProxy.unsubscribeRaw(this.onEstablishCommunicationReceived);
            await this._driverProxy.subscribeRaw("connect.iot.driver.secsgem.receivedMessage.S1F13", this.onEstablishCommunicationReceived);
            // register handler
            await this._driverProxy.unsubscribeRaw(this.onEstablishCommunicationReceived);
            await this._driverProxy.subscribeRaw(`connect.iot.driver.secsgem.receivedMessage.S1F13`, this.onEstablishCommunicationReceived);
            await this._driverProxy.notifyRaw("connect.iot.driver.secsgem.registerHandler", { type: "S1F13", mode: "NotifyOnly" });
            this._logger.info(`Subscribed topic S1F13 in driver communication`);
        } catch (error) {
            this._logger.error(`Failed to subscribe topic S1F13 in driver communication: ${error.message}`);
            if (!this.driverSetupDefinition.AutomationEquipmentWaitOnSetupTimeoutAndRetryIfErrorOccurs) {
                this.error.emit(error);
            }
        }
    }

    private async unsubscribeEstablishCommunication() {
        try {
            await this._driverProxy.unsubscribeRaw(this.onEstablishCommunicationReceived);
            this._logger.info(`Unsubscribed topic S1F13 in driver communication`);
        } catch (error) {
            this._logger.error(`Failed to subscribe topic S1F13 in driver communication: ${error.message}`);
            if (!this.driverSetupDefinition.AutomationEquipmentWaitOnSetupTimeoutAndRetryIfErrorOccurs) {
                this.error.emit(error);
            }
        }
    }

    private onEstablishCommunicationReceived: any = (message: Message<SecsTransaction>): void => {
        if (this.resetSetupOnEstablishCommunicationReceived) {
            this.resetToReconnect()
        } else {
            this.unsubscribeEstablishCommunication();
        }
    }

    private async resetToReconnect() {
        this._logger.warning("Establish Communication was received. Resetting setup.");
        // reset Setup First Run to true in order to send notification if Setup retry fails
        await this._setupDataStore.setTempValue("SetupFirstRun", this.driverName, true);
        await this._setupDataStore.setInternalState(this.driverName, CustomSetupStatesEnum.EstablishCommunication);
        await this.ResetTask(true);
    }

    private async FinishConnection(): Promise<boolean> {
        await this._setupDataStore.setTempValue("SetupFirstRun", this.driverName, false);
        this.SetupSuccessfully.emit(true);

        if (this.heartbeatTimeout > 0) {
            this.Heartbeat();
        }

        return true;
    }

    private async Heartbeat(): Promise<boolean> {
        const sendMessage = { type: "S1F1", item: {} };

        try {
            const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage, this.heartbeatTimeout);
            this.Heartbeat();
        } catch (e) {
            this.ExecuteCurrentStep(CustomSetupStatesEnum.EstablishCommunication);
        };

        return true;
    }

    GenerateUniqSerial(): string {
        return 'xxxx-xxxx-xxx-xxxx'.replace(/[x]/g, (c) => {
            const r = Math.floor(Math.random() * 16);
            return r.toString(16);
        });
    }

    /**
     * Right after settings are loaded, create the needed dynamic outputs.
     */
    async onBeforeInit(): Promise<void> {
        if (this.inputs) {
            for (const input of this.inputs) {
                // TODO: check why this doesn't work
                // this[input.name] = new Task.Input();
            }
        }
        if (this.inputs && this.inputs.length > 0) {
            for (const input of this.inputs) {
                input.value = false;
            }
        }
        if (this.outputs) {
            for (const output of this.outputs) {
                this[output.name] = new Task.Output();
            }
        }
        this.to = null;
    }

    /** Initialize this task, register any event handler, etc */
    async onInit(): Promise<void> {
    }

    /** Cleanup internal data, unregister any event handler, etc */
    async onDestroy(): Promise<void> {
        if (this.inputs) {
            for (const input of this.inputs) {
                // TODO: check why this doesn't work
                // (<Task.Input<any>>this[input.name]).destroy();
                delete this[input.name + "_in"];
            }
        }
        if (this.outputs) {
            for (const output of this.outputs) {
                (<Task.Output<any>>this[output.name]).destroy();
                delete this[output.name];
            }
        }
    }
}

// Add settings here
/** customSetup Settings object */
export interface CustomSetupSettings {
    /** Establish Communication Message */
    establishCommunicationMessageStr: string;
    /** Establish Communication Success Codes */
    establishCommunicationSuccessCodes: string;
    /** Establish Communication Additional Actions */
    establishCommunicationAdditionalActions: boolean;
    establishCommunicationAdditionalActionsTimeout: number;

    /** Set Online Communication Message */
    setOnlineCommunicationMessageStr: string;
    /** Set Online Success Codes */
    setOnlineSuccessCodes: string;
    /** Set Online Additional Actions */
    setOnlineAdditionalActions: boolean;
    setOnlineAdditionalActionsTimeout: number;

    /** Delete Reports Additional Actions */
    deleteReportsAdditionalActions: boolean;
    deleteReportsAdditionalActionsTimeout: number;
    deleteReportsTimeout: number;

    /** Internal Define Reports Additional Actions */
    internalDefineReportsAdditionalActions: boolean;
    internalDefineReportsAdditionalActionsTimeout: number;
    internalDefineReportsTimeout: number;

    /** Link Events Additional Actions */
    linkEventsAdditionalActions: boolean;
    linkEventsAdditionalActionsTimeout: number;
    linkEventsTimeout: number;

    /** Enable Disable Events Additional Actions */
    enableDisableEventsAdditionalActions: boolean;
    enableDisableEventsAdditionalActionsTimeout: number;
    enableDisableEventsTimeout: number;

    /** Enable Disable Alarms Additional Actions */
    enableDisableAlarmsAdditionalActions: boolean;
    enableDisableAlarmsAdditionalActionsTimeout: number;
    enableDisableAlarmsTimeout: number;

    /** Wait Time Between Retries */
    waitTimeBetweenRetries: number;
    /** Reset Setup on Establish Communication received from Equipment */
    resetSetupOnEstablishCommunicationReceived: boolean;

    /** Heartbeat */
    heartbeatSxFy: string;
    heartbeatBody: string;
    heartbeatTimeout: number;

    /** Command Inputs*/
    inputs: CustomSetupInputSettings[];
    /** Command Output */
    outputs: CustomSetupOutputSettings[];
}

export interface CustomSetupInputSettings extends Task.TaskInput {
    /** Input name */
    name: string;
    /** Input value type */
    valueType: Task.TaskComplexValueType;
    value: boolean;
}

export interface CustomSetupOutputSettings extends Task.TaskOutput {
    /** Output name */
    name: string;
    /** Output value type */
    valueType: Task.TaskComplexValueType;
    value: string;
}
