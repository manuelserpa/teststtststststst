import { Task, Dependencies, System, DI, TYPES } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customControlJobRequest.default";


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
 * See {@see CustomControlJobRequestSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-secsgem-iot-lg-setrecipe",
    inputs: {
        ControlJobName: Task.TaskValueType.String,
        CommandParameterList: Task.TaskValueType.Object,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.SecsGem
})
export class CustomControlJobRequestTask implements Task.TaskInstance, CustomControlJobRequestSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    ControlJobCommand: ControlJobCommand;
    ControlJobName: string;
    CommandParameterList: any[];
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

            try {
                const sendMessage: Object = {
                    type: "S16F27", item: {
                        type: "L", value: [
                            { type: "A", value: this.ControlJobName }, // control job
                            { type: "U1", value: this.ControlJobCommand }, // control job command setting
                            { type: "L", value: this.CommandParameterList } // command parameter list
                        ]
                    }
                }

                const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);
                let successFound = false;


                if (reply && reply.item && (Boolean(reply.item.value[0].value[0].value))) {
                    successFound = true;
                }

                if (!successFound) {
                    const error =
                        new Error(`EI: Error on Custom Control Job Request:  ${JSON.stringify(reply.item.value[0].value[1].value[0].value[0].value)} - ${JSON.stringify(reply.item.value[0].value[1].value[0].value[1].value)}`);
                    this.error.emit(error);
                    throw error;
                }

                this.success.emit(true);

            } catch (error) {
                this.error.emit(error);
                this._logger.error(`Error on Custom Control Job Request Task: ${error.message}`);
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
/** customControlJobRequest Settings object */
export interface CustomControlJobRequestSettings {
    ControlJobCommand: ControlJobCommand;
}

export enum ControlJobCommand {
    Start = 1,
    Pause = 2,
    Resume = 3,
    Cancel = 4,
    Deselect = 5,
    Stop = 6,
    Abort = 7,
    HOQ = 8
}
