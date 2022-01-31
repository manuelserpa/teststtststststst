import { Task, Dependencies, System, DI, TYPES } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customSendAdHocEquipmentRequest.default";
import { SecsGem } from "./../../common/secsGemItem"
import { SecsItem } from "../../common/secsItem";
import { convertValueToType } from "../../utilities/convertUtilities";

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
 * See {@see CustomSendAdHocEquipmentRequestSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-secsgem-iot-lg-setrecipe",
    inputs: {
        adHocRequest: Task.TaskValueType.Object,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        returnObject: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.SecsGem
})
export class CustomSendAdHocEquipmentRequestTask implements Task.TaskInstance, CustomSendAdHocEquipmentRequestSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;

    public adHocRequest: any = undefined;

    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();

    public returnObject: Task.Output<Object> = new Task.Output<Object>();


    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject(TYPES.System.Driver)
    private _driverProxy: System.DriverProxy;


    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {
        if (changes["activate"]) {
            // It is advised to reset the activate to allow being reactivated without the value being different
            this.activate = undefined;

            const request = this.adHocRequest;
            // clear input
            this.adHocRequest = undefined;
            const driverName = this._driverProxy.automationControllerDriverDefinition.DisplayName;
            // to allow multiple driver on the controller, each driver must care for it's own request and ignore the others
            if (request.Driver !== driverName) {
                this._logger.warning(this.log(`Request received for driver ${request.Driver}, ignoring it`));
                return;
            }
            this._logger.warning(this.log(`*************************************************************************`));
            this._logger.warning(this.log(`Executing Ad-Hoc Requests Summary:`));
            this._logger.warning(this.log(`Number of request to execute: ${request.Actions.length}`));
            this._logger.warning(this.log(`Stop on error detected: ${request.StopOnError}`));
            this._logger.warning(this.log(`*************************************************************************`));

            const actionResult: any[] = [];
            for (const action of request.Actions) {
                let successFound: boolean = false;
                try {
                    this._logger.warning(this.log(`Executing Request ${action.Order}: ${action.Name} (${action.Type})`));
                    if (action.Type.toString() === AdHocActionTypes.SendRequest.toString()) {
                        const replyGroup = await this.sendRequest(action);
                        successFound = replyGroup.SuccessFound;
                        actionResult.push({ Name: action.Name, Order: action.Order, Result: replyGroup.SuccessFound, Reply: replyGroup.Reply });

                    } else if (action.Type.toString() === AdHocActionTypes.GetVariables.toString()) {
                        const reply = await this.getVariablesExistentInDriver(action.Content.List);
                        successFound = true;
                        actionResult.push({ Name: action.Name, Order: action.Order, Result: true, Reply: reply });
                    } else if (action.Type.toString() === AdHocActionTypes.SetVariables.toString()) {
                        const reply = await this.setVariables(action.Content.List);
                        successFound = reply;
                        actionResult.push({ Name: action.Name, Order: action.Order, Result: successFound, Reply: reply });
                    } else {
                        this._logger.error(this.log(`AdHoc Action Type ${action.Type} not supported yet`));
                    }
                } catch (error) {
                    actionResult.push({ Name: action.Name, Order: action.Order, Result: false, Reply: undefined });
                    this._logger.error(this.log(`Executing Request ${action.Order} FAILED: ${action.Name} (${action.Type}) with error ${error.message}`));
                }

                if (successFound) {
                    this._logger.warning(this.log(`Request ${action.Order}: ${action.Name} (${action.Type}) was successful`))
                }
                if (!successFound) {
                    this._logger.error(this.log(`Request ${action.Order}: ${action.Name} (${action.Type}) was not successful`))
                    if (request.StopOnError) {
                        this._logger.warning(this.log(`Stop on Error defined, stopping AdHoc Request list`))
                        break;
                    }
                }

                this._logger.warning(this.log(`*************************************************************************`));
                this._logger.warning(this.log(`Executing Ad-Hoc Requests Complete:`));
                this._logger.warning(this.log(`*************************************************************************`));
            };

            this.success.emit(true);
            this.returnObject.emit(actionResult);

        }
    }

    private async sendRequest(action: any): Promise<any> {
        let successFound = false;
        const sendMessage = { type: action.Content.SxFy, item: JSON.parse(action.Content.Body) };
        const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);

        const successCodes = action.Content.SuccessCodes;
        const replyPath = action.Content.ReplyPath;

        const replyStringify = JSON.stringify(reply);
        this._logger.warning(this.log(`Executing Request ${action.Order} returned: ${action.Name} (${action.Type}) \n${replyStringify}`));

        if (!successCodes || successCodes.trim() === "" || !replyPath || replyPath.trim() === "") {
            // if empty, any reply is a success.
            successFound = true;
        } else {
            for (const successCode of successCodes.split(",")) {
                if (reply &&
                    reply.item &&
                    parseInt(SecsGem.getValue(SecsGem.getItemByPath(reply.item, replyPath))) === parseInt(successCode.trim())) {
                    successFound = true;
                    break;
                }
            }
        }
        return { Reply: reply, SuccessFound: successFound };

    }

    private async getVariablesExistentInDriver(properties: any[]): Promise<any[]> {
        const driverProperties = this._driverProxy.automationControllerDriverDefinition.AutomationDriverDefinition.Properties;
        const variableIdType = (<any>this._driverProxy)._driver._currentIntegrationConfiguration.DeviceConfiguration.communication.variableIdType;

        const constants = driverProperties.filter(p => p.ExtendedData && (<any>p.ExtendedData).variableType === "Constant"
            && properties.some(p2 => p.Name === p2.Name));
        const otherTypes = driverProperties.filter(p => p.ExtendedData && (<any>p.ExtendedData).variableType !== "Constant"
            && properties.some(p2 => p.Name === p2.Name));

        const results: any[] = [];


        if (constants && constants.length > 0) {
            const messageContent = [];
            for (const constant of constants) {
                messageContent.push(<SecsItem>{ type: variableIdType, name: "ECID", value: constant.DevicePropertyId });
            }

            const sendMessage = { type: "S2F13", item: { type: "L", value: messageContent } };
            const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);

            if (reply && reply.item) {
                for (let i = 0; i < constants.length; i++) {
                    const receivedValueItem = SecsGem.getItemByPath(reply.item, `/[${i + 1}]`);
                    const value: any = {
                        propertyName: constants[i].Name,
                        originalValue: receivedValueItem,
                        value: this.convertValueFromDevice(receivedValueItem, constants[i].DataType.toString()),
                    };
                    results.push(value);
                }
            } else {
                this.logger.error(`Failed to get values for constants`);
                throw new Error("Failed to values for the properties");
            }
        }
        if (otherTypes && otherTypes.length > 0) {
            const messageContent = [];
            for (const variable of otherTypes) {
                messageContent.push(<SecsItem>{ type: variableIdType, name: "ECID", value: variable.DevicePropertyId });
            }

            const sendMessage = { type: "S1F3", item: { type: "L", value: messageContent } };
            const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);

            if (reply && reply.item) {
                for (let i = 0; i < otherTypes.length; i++) {
                    const receivedValueItem = SecsGem.getItemByPath(reply.item, `/[${i + 1}]`);

                    const value: any = {
                        propertyName: otherTypes[i].Name,
                        originalValue: receivedValueItem,
                        value: this.convertValueFromDevice(receivedValueItem, otherTypes[i].DataType.toString()),
                    };
                    results.push(value);
                }
            } else {
                this.logger.error(`Failed to get values for variables`);
                throw new Error("Failed to values for the properties");
            }
        }
        return (results);

    }

    private async setVariables(values: any[]): Promise<boolean> {

        const driverProperties = this._driverProxy.automationControllerDriverDefinition.AutomationDriverDefinition.Properties;
        const valuesToSet: Map<any, any> =
            new Map<any, any>();

        for (const value of values) {
            const currentInputValue = value.Value;
            const automationProperty = driverProperties.filter(c => c.Name === value.Name);

            if (currentInputValue == null) {
                this._logger.debug(
                    `Input '${value.Name}' is mandatory but has no value at the moment.
                        No output value will be calculated as a result of this detected change.`
                );
                return;
            }

            if (!automationProperty.some(c => c)) {
                this._logger.debug(
                    `Input '${value.Name}' was not found in the driver. No output value will be calculated as a result of this detected change.`
                );
                return;
            }

            // Store the value to trigger
            if (currentInputValue != null) {
                valuesToSet.set(automationProperty[0], currentInputValue);
            }
        }

        const result = await this._driverProxy.setProperties(valuesToSet);

        return result;
    }

    private async getVariablesInexistentInDriver(properties: any[]): Promise<any[]> {
        const constants = properties.filter(p => p.ExtendedData && (<any>p.ExtendedData).variableType === "Constant");
        const otherTypes = properties.filter(p => p.ExtendedData && (<any>p.ExtendedData).variableType !== "Constant");

        const results: any[] = [];
        if (constants && constants.length > 0) {
            const messageContent = [];
            for (const constant of constants) {
                messageContent.push(<SecsItem>{ type: constant.AutomationProtocolDataType, name: "ECID", value: constant.DevicePropertyId });
            }

            const sendMessage = { type: "S2F13", item: { type: "L", value: messageContent } };
            const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);

            if (reply && reply.item) {
                for (let i = 0; i < constants.length; i++) {
                    const receivedValueItem = SecsGem.getItemByPath(reply.item, `/[${i + 1}]`);
                    const value: any = {
                        propertyName: constants[i].name,
                        originalValue: receivedValueItem,
                        value: this.convertValueFromDevice(receivedValueItem, constants[i].dataType),
                    };
                    results.push(value);
                }
            } else {
                this.logger.error(`Failed to get values for constants`);
                throw new Error("Failed to values for the properties");
            }
        }
        if (otherTypes && otherTypes.length > 0) {
            const messageContent = [];
            for (const variable of otherTypes) {
                messageContent.push(<SecsItem>{ type: variable.AutomationProtocolDataType, name: "ECID", value: variable.DevicePropertyId });
            }

            const sendMessage = { type: "S1F3", item: { type: "L", value: messageContent } };
            const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);

            if (reply && reply.item) {
                for (let i = 0; i < otherTypes.length; i++) {
                    const receivedValueItem = SecsGem.getItemByPath(reply.item, `/[${i + 1}]`);
                    const value: any = {
                        propertyName: otherTypes[i].name,
                        originalValue: receivedValueItem,
                        value: this.convertValueFromDevice(receivedValueItem, otherTypes[i].dataType),
                    };
                    results.push(value);
                }
            } else {
                this.logger.error(`Failed to get values for variables`);
                throw new Error("Failed to values for the properties");
            }
        }
        return (results);
    }

    private convertValueFromDevice(secsItem: SecsItem, type: string): any {
        if (secsItem.type === "L") {
            return undefined; // Lists cannot be converted
        } else {
            return (convertValueToType(SecsGem.getValue(secsItem), type));
        }
    }

    private log(log: string) {
        return `[Driver ${this._driverProxy.automationControllerDriverDefinition.DisplayName}]: ${log}`
    }

    /**
     * Right after settings are loaded, create the needed dynamic outputs.
     */
    async onBeforeInit(): Promise<void> {
    }

    /** Initialize this task, register any event handler, etc */
    async onInit(): Promise<void> {
    }

    /** Cleanup internal data, unregister any event handler, etc */
    async onDestroy(): Promise<void> {
    }
}

// Add settings here
/** customSendAdHocEquipmentRequest Settings object */
export interface CustomSendAdHocEquipmentRequestSettings {
    [key: string]: any;
}

export enum AdHocActionTypes {
    SendRequest,
    GetVariables,
    SetVariables,
    ExecuteCommand
}
