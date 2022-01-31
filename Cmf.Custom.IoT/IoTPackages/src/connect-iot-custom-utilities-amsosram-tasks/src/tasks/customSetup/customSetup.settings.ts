/** Core */
import { Module, Component } from "cmf.core/src/core";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, WorkflowModel } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";
import * as CustomSetupTask from "./customSetup.task";
import { CustomSetupInputSettings, CustomSetupOutputSettings } from "./customSetup.task";
/** Angular */
import * as ng from "@angular/core";

/** Nested components */
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";
import { EnumComboBoxModule } from "cmf.core.controls/src/components/enumComboBox/enumComboBox";
import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";
import { PopOverInfoModule } from "cmf.core.controls/src/components/popOverInfo/popOverInfo";
import { ResultMessageBag, ResultMessageType } from "cmf.core.controls/src/components/resultMessage/resultMessageBag";
import { ResultMessageModule } from "cmf.core.controls/src/components/resultMessage/resultMessage";
import { Task, Dependencies, System, DI, TYPES } from "@criticalmanufacturing/connect-iot-controller-engine";

/** i18n */
import i18n from "./i18n/customSetup.settings.default";
import i18nHelp from "../../common/i18n/common.default";
import { ComplexValueType } from "@criticalmanufacturing/connect-iot-controller-engine/src/system";
import { TaskValueType } from "@criticalmanufacturing/connect-iot-controller-engine/src/task";

/** Constants */
export interface CustomSetupTaskSettings extends
    CustomSetupTask.CustomSetupSettings, WorkflowModel.TaskDefinitionSettings { }

/**
 *
 */

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-customSetup-settings",
    templateUrl: "customSetup.settings.html",
    assign: {
        i18n: i18n,
    }
})
export class CustomSetupSettings extends TaskSettingsBase implements ng.OnInit {

    /**
     * Task settings
     */
    public settings: CustomSetupTaskSettings;

    /** helper to access main task (for defaults loading) */
    private _taskInstance: CustomSetupTask.CustomSetupTask;

    /** primaryMessage as an object for property editor with valueType=Object */
    private establishCommunicationMessageObj: any = undefined
    private setOnlineCommunicationMessageObj: any = undefined
    private heartbeatBody: any = undefined

    /**
     * Constructor
     */
    constructor(private _elementRef: ng.ElementRef, viewContainerRef: ng.ViewContainerRef, service: TaskSettingsService) {
        super(viewContainerRef, service);
    }

    /** Triggered when the task is created and define the default values */
    public ngOnInit(): void {

        if (this.task != null) {
            this._taskInstance = (<any>this.task)._taskInstance;
        }

        // Initialize default values for settings page
        if (this.settings) {
            this.settings.inputs = (this.settings.inputs != null)
                ? this.settings.inputs
                : this._taskInstance.inputs;
            this.settings.outputs = (this.settings.outputs != null)
                ? this.settings.outputs
                : this._taskInstance.outputs;

            // Establish Communication Message
            if (this.establishCommunicationMessageObj == null) {
                if (this.isJson(this.settings.establishCommunicationMessageStr)) {
                    this.establishCommunicationMessageObj = JSON.parse(this.settings.establishCommunicationMessageStr);
                } else {
                    this.establishCommunicationMessageObj = JSON.parse(this._taskInstance.establishCommunicationMessageStr);
                };
            }
            this.settings.establishCommunicationMessageStr = JSON.stringify(this.establishCommunicationMessageObj);

            // Establish Communication Success Codes
            this.settings.establishCommunicationSuccessCodes = this.settings.establishCommunicationSuccessCodes != null ?
                this.settings.establishCommunicationSuccessCodes : this._taskInstance.establishCommunicationSuccessCodes;

            // Establish Communication Additional Actions
            this.settings.establishCommunicationAdditionalActions = this.settings.establishCommunicationAdditionalActions != null ?
                this.settings.establishCommunicationAdditionalActions : this._taskInstance.establishCommunicationAdditionalActions;

            // Establish Communication Additional Actions Timeout
            this.settings.establishCommunicationAdditionalActionsTimeout = this.settings.establishCommunicationAdditionalActionsTimeout != null ?
                this.settings.establishCommunicationAdditionalActionsTimeout : this._taskInstance.establishCommunicationAdditionalActionsTimeout;

            // Set Online Communication Message
            if (this.setOnlineCommunicationMessageObj == null) {
                if (this.isJson(this.settings.setOnlineCommunicationMessageStr)) {
                    this.setOnlineCommunicationMessageObj = JSON.parse(this.settings.setOnlineCommunicationMessageStr);
                } else {
                    this.setOnlineCommunicationMessageObj = JSON.parse(this._taskInstance.setOnlineCommunicationMessageStr);
                };
            }
            this.settings.setOnlineCommunicationMessageStr = JSON.stringify(this.setOnlineCommunicationMessageObj);

            // Set Online Success Codes
            this.settings.setOnlineSuccessCodes = this.settings.setOnlineSuccessCodes != null ?
                this.settings.setOnlineSuccessCodes : this._taskInstance.setOnlineSuccessCodes;

            // Set Online Additional Actions
            this.settings.setOnlineAdditionalActions = this.settings.setOnlineAdditionalActions != null ?
                this.settings.setOnlineAdditionalActions : this._taskInstance.setOnlineAdditionalActions;

            // Set Online Additional Actions Timeout
            this.settings.setOnlineAdditionalActionsTimeout = this.settings.setOnlineAdditionalActionsTimeout != null ?
                this.settings.setOnlineAdditionalActionsTimeout : this._taskInstance.setOnlineAdditionalActionsTimeout;

            // Delete Reports Additional Actions
            this.settings.deleteReportsAdditionalActions = this.settings.deleteReportsAdditionalActions != null ?
                this.settings.deleteReportsAdditionalActions : this._taskInstance.deleteReportsAdditionalActions;

            // Delete Reports Timeout
            this.settings.deleteReportsTimeout = this.settings.deleteReportsTimeout != null ?
                this.settings.deleteReportsTimeout : this._taskInstance.deleteReportsTimeout;

            // Delete Reports Additional Actions Timeout
            this.settings.deleteReportsAdditionalActionsTimeout = this.settings.deleteReportsAdditionalActionsTimeout != null ?
                this.settings.deleteReportsAdditionalActionsTimeout : this._taskInstance.deleteReportsAdditionalActionsTimeout;

            // Internal Define Reports Additional Actions
            this.settings.internalDefineReportsAdditionalActions = this.settings.internalDefineReportsAdditionalActions != null ?
                this.settings.internalDefineReportsAdditionalActions : this._taskInstance.internalDefineReportsAdditionalActions;

            // Internal Define Reports Timeout
            this.settings.internalDefineReportsTimeout = this.settings.internalDefineReportsTimeout != null ?
                this.settings.internalDefineReportsTimeout : this._taskInstance.internalDefineReportsTimeout;

            // Internal Define Reports Additional Actions Timeout
            this.settings.internalDefineReportsAdditionalActionsTimeout = this.settings.internalDefineReportsAdditionalActionsTimeout != null ?
                this.settings.internalDefineReportsAdditionalActionsTimeout : this._taskInstance.internalDefineReportsAdditionalActionsTimeout;

            // Link Events Additional Actions
            this.settings.linkEventsAdditionalActions = this.settings.linkEventsAdditionalActions != null ?
                this.settings.linkEventsAdditionalActions : this._taskInstance.linkEventsAdditionalActions;

            // Link Events Timeout
            this.settings.linkEventsTimeout = this.settings.linkEventsTimeout != null ?
                this.settings.linkEventsTimeout : this._taskInstance.linkEventsTimeout;

            // Link Events Additional Actions Timeout
            this.settings.linkEventsAdditionalActionsTimeout = this.settings.linkEventsAdditionalActionsTimeout != null ?
                this.settings.linkEventsAdditionalActionsTimeout : this._taskInstance.linkEventsAdditionalActionsTimeout;

            // Enable Disable Events Additional Actions
            this.settings.enableDisableEventsAdditionalActions = this.settings.enableDisableEventsAdditionalActions != null ?
                this.settings.enableDisableEventsAdditionalActions : this._taskInstance.enableDisableEventsAdditionalActions;

            // Enable Disable Events Timeout
            this.settings.enableDisableEventsTimeout = this.settings.enableDisableEventsTimeout != null ?
                this.settings.enableDisableEventsTimeout : this._taskInstance.enableDisableEventsTimeout;

            // Enable Disable Events Additional Actions Timeout
            this.settings.enableDisableEventsAdditionalActionsTimeout = this.settings.enableDisableEventsAdditionalActionsTimeout != null ?
                this.settings.enableDisableEventsAdditionalActionsTimeout : this._taskInstance.enableDisableEventsAdditionalActionsTimeout;

            // Enable Disable Alarms Additional Actions
            this.settings.enableDisableAlarmsAdditionalActions = this.settings.enableDisableAlarmsAdditionalActions != null ?
                this.settings.enableDisableAlarmsAdditionalActions : this._taskInstance.enableDisableAlarmsAdditionalActions;

            // Enable Disable Alarms Timeout
            this.settings.enableDisableAlarmsTimeout = this.settings.enableDisableAlarmsTimeout != null ?
                this.settings.enableDisableAlarmsTimeout : this._taskInstance.enableDisableAlarmsTimeout;

            // Enable Disable Alarms Additional Actions Timeout
            this.settings.enableDisableAlarmsAdditionalActionsTimeout = this.settings.enableDisableAlarmsAdditionalActionsTimeout != null ?
                this.settings.enableDisableAlarmsAdditionalActionsTimeout : this._taskInstance.enableDisableAlarmsAdditionalActionsTimeout;

            // Wait Time Between Retries
            this.settings.waitTimeBetweenRetries = this.settings.waitTimeBetweenRetries != null ?
                this.settings.waitTimeBetweenRetries : this._taskInstance.waitTimeBetweenRetries;

            // Reset Setup on Establish Communication received from Equipment
            this.settings.resetSetupOnEstablishCommunicationReceived = this.settings.resetSetupOnEstablishCommunicationReceived != null ?
                this.settings.resetSetupOnEstablishCommunicationReceived : this._taskInstance.resetSetupOnEstablishCommunicationReceived;

            // Heartbeat SxFy
            this.settings.heartbeatSxFy = this.settings.heartbeatSxFy != null ?
                this.settings.heartbeatSxFy : this._taskInstance.heartbeatSxFy;

            // Heartbeat Body
            if (this.heartbeatBody == null) {
                if (this.isJson(this.settings.heartbeatBody)) {
                    this.heartbeatBody = JSON.parse(this.settings.heartbeatBody);
                } else {
                    this.heartbeatBody = JSON.parse(this._taskInstance.heartbeatBody);
                };
            }
            this.settings.heartbeatBody = JSON.stringify(this.heartbeatBody);

            // Heartbeat Timeout
            this.settings.heartbeatTimeout = this.settings.heartbeatTimeout != null ?
                this.settings.heartbeatTimeout : this._taskInstance.heartbeatTimeout;
        }

    }

    /**
     * Checks if string is a parsable JSON
     * @param message string to test
     */
    private isJson(message: string): boolean {
        try {
            JSON.parse(message);
        } catch (e) {
            return false;
        }
        return true;
    }

    /**
     * Handles a change in the settings.
     * Update the settings property with given value and path
     * @param value New value
     * @param destPath Destination property
     */
    public _onSettingsValueChange(value: any, destPath: string): void {
        // Set the settings with new value
        this.framework.sandbox.util.setNestedPropertyByPath(this.settings, destPath, value, true);

        switch (destPath) {
            case "EstablishCommunicationMessageValueChange":
                if (value) {
                    this.establishCommunicationMessageObj = value;
                    this.settings.establishCommunicationMessageStr = JSON.stringify(this.establishCommunicationMessageObj)
                }
                break;
            case "EstablishCommunicationAdditionalActions":
            case "SetOnlineAdditionalActions":
            case "DeleteReportsAdditionalActions":
            case "InternalDefineReportsAdditionalActions":
            case "LinkEventsAdditionalActions":
            case "EnableDisableEventsAdditionalActions":
            case "EnableDisableAlarmsAdditionalActions":
                this.SwitchInputOnOff(value, destPath);
                this.SwitchOutputOnOff(value, destPath);
                break;
            case "SetOnlineCommunicationMessageValueChange":
                if (value) {
                    this.setOnlineCommunicationMessageObj = value;
                    this.settings.setOnlineCommunicationMessageStr = JSON.stringify(this.setOnlineCommunicationMessageObj)
                }
                break;
        }
    }

    private SwitchInputOnOff(isActive: boolean, context: string) {
        const doesExist: boolean = this.settings.inputs.some(o => o.name === context + "_in");
        if (!doesExist && isActive) {
            const inputItem: CustomSetupInputSettings = { name: context + "_in", valueType: {} as ComplexValueType, value: false }
            inputItem.valueType.type = Task.TaskValueType.Boolean;
            this.settings.inputs.push(inputItem);
        } else if (doesExist && !isActive) {
            let index: number = -1;
            for (let i = 0; i < this.settings.inputs.length; ++i) {
                if (this.settings.inputs[i].name === context + "_in") {
                    index = i;
                    break;
                }
            }
            if (index > -1) {
                this.settings.inputs.splice(index, 1);
            }
        }
    }

    private SwitchOutputOnOff(isActive: boolean, context: string) {
        const doesExist: boolean = this.settings.outputs.some(o => o.name === context);
        if (!doesExist && isActive) {
            console.log(JSON.stringify(this.settings.outputs));
            console.log("this.settings.establishCommunicationAdditionalActionsTimeout = " + this.settings.establishCommunicationAdditionalActionsTimeout);
            const outputItem: CustomSetupOutputSettings = { name: context, valueType: {} as ComplexValueType, value: "" }
            outputItem.valueType.type = Task.TaskValueType.String;
            this.settings.outputs.push(outputItem);
        } else if (doesExist && !isActive) {
            let index: number = -1;
            for (let i = 0; i < this.settings.outputs.length; ++i) {
                if (this.settings.outputs[i].name === context) {
                    index = i;
                    break;
                }
            }
            if (index > -1) {
                this.settings.outputs.splice(index, 1);
            }
        }
    }
}

/** Module */
@Module({
    imports: [
        TaskSettingsModule,
        PropertyEditorModule,
        EnumComboBoxModule,
        BaseWidgetModule,
        PropertyContainerModule,
        PopOverInfoModule,
        ResultMessageModule
    ],
    declarations: [CustomSetupSettings],
    defaultRoute: CustomSetupSettings
})

export class CustomSetupSettingsModule { }
