import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/errorMessage.default";
import * as moment from "moment";
import { TaskDefaultSettings } from "@criticalmanufacturing/connect-iot-controller-engine/src/system/systemProxy";
import { CustomErrorCodeEnum } from "../../utilities/customErrorCodeEnum";
import { CustomSystemOfOriginEnum } from "../../utilities/customSystemOfOriginEnum";

/**
 * Verbosity types for the log message
 */
export enum ErrorMessageVerbosityType {
    Debug = "Debug",
    Information = "Information",
    Error = "Error",
    Warning = "Warning",
}

/**
 * Log modes
 */
export enum LogMode {
    RawText = "RawText",
    MultipleInputs = "MultipleInputs",
}

/**
 * Default values for settings
 */
export const SETTINGS_DEFAULTS: ErrorMessageSettings = {
    message: "",
    errorCodeToEmit: CustomErrorCodeEnum.OtherError,
    systemOfOrigin: CustomSystemOfOriginEnum.EI,
    errorNumber: 0,
    clearInputs: true,
    mode: LogMode.RawText,
    isCustomFormat: false,
    messageFormat: "",
    inputs: []
}

/**
 * Auto input port key
 */
export const AUTO_IN: string = "autoIn";

/**
 * Message port key
 */
export const MESSAGE: string = "message";

/**
 * Separator for multiple inputs default format
 */
export const DEFAULT_FORMAT_SEPARATOR: string = " = ";


/**
 * @whatItDoes
 * This task prints the received inputs to the log.
 *
 * @howToUse
 * Choose whether to use a single input or with multiple dynamic inputs.
 * In case of single input, both 'activate' and 'message' trigger the logging action.
 * In case of multiple inputs it is necessary to add the needed inputs. If a custom format
 * is used, a single message with the received values is logged, otherwise each input triggers
 * a different message. This mode requires 'activate' to trigger the logging action.
 *
 * ### Inputs
 * * `any` : **activate** - Activate the task
 * * `any` : **message** - Message to log
 *
 * ### Outputs
 * * `bool`  : ** success ** - Triggered when the the task is executed with success
 * * `Error` : ** error ** - Triggered when the task failed for some reason
 *
 * ### Settings
 * See {@see ErrorMessageSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-tasks-connect-iot-lg-logmessage",
    inputs: {
        activate: Task.INPUT_ACTIVATE,
        autoIn: <Task.TaskType>{
            friendlyName: i18n.AUTO_INPUT_PORT_TEXT,
            type: Task.AUTO
        },
        message: undefined
    },
    outputs: {
        errorCode: Task.TaskValueType.String,
        errorText: Task.TaskValueType.String
    }
})
export class ErrorMessageTask implements Task.TaskInstance, ErrorMessageSettings {

    private _defaultMessage: string;

    /**
     * Activate task input
     */
    public activate: any = undefined;

    /**
     * Properties Settings
     */
    message: any;
    errorCodeToEmit: CustomErrorCodeEnum;
    systemOfOrigin: CustomSystemOfOriginEnum;
    errorNumber: number = 0;
    clearInputs: boolean;
    mode: LogMode;
    isCustomFormat: boolean;
    messageFormat: string;
    escapeNewLine: boolean;
    inputs: Task.TaskInput[]

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    public errorCode: Task.Output<String> = new Task.Output<String>();
    public errorText: Task.Output<String> = new Task.Output<String>();
    /**
     * When input value changes, check if there is any defined output and, if so, if we have a match for the given input.
     * If a match is found, emit the new value out from the output. Otherwise, check if there is a default value that should be emitted.
     *
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {


        this.activate = undefined;
        let text;
        switch (this.mode || LogMode.RawText) {
            case LogMode.RawText:
                text = this.normalizeText(this.message ?? this._defaultMessage);
                if (this.clearInputs) {
                    this.message = undefined;
                }
                break;
            case LogMode.MultipleInputs:
                if (changes["activate"] != null) {
                    if (this.isCustomFormat) {
                        let messageAsString = this.messageFormat;
                        if (this.inputs != null && this.inputs.length > 0) {
                            this.inputs.forEach((input: Task.TaskInput) => {
                                const inputValue = this.normalizeText(this[Utilities.propertyToInput(input.name)]);
                                // Replace tokens
                                const token = `\${${input.name}}`;
                                messageAsString = messageAsString.split(token).join(inputValue);
                                // Reset value
                                if (this.clearInputs) {
                                    this[Utilities.propertyToInput(input.name)] = undefined;
                                }
                            });
                        }
                        // Log message
                        text = messageAsString;
                    } else {
                        if (this.inputs != null && this.inputs.length > 0) {
                            this.inputs.forEach((input: Task.TaskInput) => {
                                const inputValue = this.normalizeText(this[Utilities.propertyToInput(input.name)]);
                                // Log each input
                                const inputName = input.valueType.friendlyName ? input.valueType.friendlyName : input.name;
                                text.concat(text, DEFAULT_FORMAT_SEPARATOR, inputName.concat(DEFAULT_FORMAT_SEPARATOR, inputValue));
                                // Reset value
                                if (this.clearInputs) {
                                    this[Utilities.propertyToInput(input.name)] = undefined;
                                }
                            });
                        }
                    }
                }
                break;
        }

        if (this.mode === LogMode.RawText || changes["activate"] != null) {
            // emit error code with structure [ErrorCode]_[System]_[ErrorNumber]
            this.errorCode.emit(`${this.errorCodeToEmit}_${this.systemOfOrigin}_${this.errorNumber}`);
            this.errorText.emit(text);
        }
    }

    public async onInit(): Promise<void> {
        this.clearInputs = Utilities.convertValueToType(this.clearInputs, Task.TaskValueType.Boolean, true);
        this._defaultMessage = this.message;
    }

    /**
     * Normalize a string into something printable (extracted from AnyToString converter)
     * @param value Original value
     */
    private normalizeText(value: any): string {
        if (typeof value === "object") {
            if (value instanceof Date) {
                return (value.toISOString());
            } else if (typeof (value) === "object" && moment.isMoment(value)) {
                return (value.toISOString());
            } else if (value instanceof Error) {
                return (JSON.stringify(value, Object.getOwnPropertyNames(value)));
            } else if (value instanceof Map) {
                return JSON.stringify(Utilities.convertMapToObject(value));
            } else {
                return (JSON.stringify(value));
            }
        } else {
            if (value == null) {
                return ("<null>");
            } else {
                return typeof value["toString"] === "function" ? value.toString() : "<null>";
            }
        }
    }
}

/**
 * Settings definition
 */
export interface ErrorMessageSettings extends TaskDefaultSettings {
    /**
     * Default message value
     */
    message: string;
    /**
     * Error Code
     */
    errorCodeToEmit: CustomErrorCodeEnum;
    /**
     * System of Origin
     */
    systemOfOrigin: CustomSystemOfOriginEnum;
    /**
     * Error Number
     */
    errorNumber: number;
    /**
     * Reset inputs values on activate
     */
    clearInputs: boolean;
    /**
     * Log mode
     */
    mode: LogMode;
    /**
     * Message custom format in multiple inputs mode
     */
    messageFormat: string;
    /**
     * Use a custom format message
     */
    isCustomFormat: boolean;
}
