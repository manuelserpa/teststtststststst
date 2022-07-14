import { Task, Dependencies, System, DI, TYPES } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customCarrierActionRequest.default";
import { SecsGem } from "../../common/secsGemItem"
import { SecsItem } from "../../common/secsItem";


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
 * See {@see CustomCarrierActionRequestSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-secsgem-iot-lg-setrecipe",
    inputs: {
        CarrierId: Task.TaskValueType.String,
        PortNumber: Task.TaskValueType.Integer,
        CarrierAttributes: Task.TaskValueType.Object,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.SecsGem
})
export class CustomCarrierActionRequestTask implements Task.TaskInstance, CustomCarrierActionRequestSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    public CarrierActionRequest: CarrierActionRequest;

    public CarrierId: String;
    public PortNumber: Number;
    public CarrierAttributes: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;
    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();


    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject(TYPES.System.Driver)
    private _driverProxy: System.DriverProxy;

    public successCodes = "0x00";
    public replyPath = "/[1]";
    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {
        if (changes["activate"]) {
            // It is advised to reset the activate to allow being reactivated without the value being different
            this.activate = undefined;

            // clear the inputs before proceeding
            // const inputValues: any = {};
            // for (const input of this.inputs) {
            //     const currentInputValue = this[input.name];
            //     inputValues[input.name] = currentInputValue;
            //     this[input.name] = undefined;
            // };
            let portNumberConverted = '';
            if (this.PortNumber) {
                portNumberConverted = this.PortNumber.toString();
            }

            this._logger.info("\nPortNumber S3F17: " + this.PortNumber);
            this._logger.info("\nCarrierAction S3F17: " + this.CarrierActionRequest);

            const dataIdType = (<any>this._driverProxy)._driver._currentIntegrationConfiguration.DeviceConfiguration.communication.dataIdType;

            try {
                const sendMessage: Object = {
                    type: "S3F17", item: {
                        type: "L", value: [
                            { type: dataIdType, value: Number(Date.now().toString()) }, // dataId
                            { type: "A", value: CarrierActionRequest[this.CarrierActionRequest] }, // carrierAction
                            { type: "A", value: this.CarrierId }, // carrier id
                            { type: "A", value: portNumberConverted }, // portNumber
                            { type: "L", value: this.CarrierAttributes }​​​​​​​ // carrier attributes list
                        ]
                    }
                }

                const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);
                let successFound = false;

                if (reply && reply.item && parseInt(reply.item.value[0].value) === 0) {
                    successFound = true;
                }

                if (!successFound) {


                    const error = new Error(`EI: Error on Proceed with carrier request: ${reply.item.value[1].value[0].value[0].value}​​​​​​​ - ${reply.item.value[1].value[0].value[1].value}​​​​​​​`);
                    this.error.emit(error);
                    throw error;
                }

                this.success.emit(true);

            } catch (error) {
                this.error.emit(error);
                this._logger.error(`Error on Custom Carrier Action Request Task: ${error.message}`);
            }

        }
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
/** customCarrierActionRequest Settings object */
export interface CustomCarrierActionRequestSettings {
    /** SECS/GEM Stream Function Name */
    CarrierActionRequest: CarrierActionRequest;
}

export enum CarrierActionRequest {
    CancelCarrier,
    CancelCarrierAtPort,
    ProceedWithCarrier,
    CarrierOut
}


