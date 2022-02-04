import { Task, Dependencies, System, DI, TYPES } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customMaterialTransferStatusRequest.default";
import { SecsGem } from "./../../common/secsGemItem"
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
 * See {@see CustomMaterialTransferStatusRequestSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-tasks-connect-iot-lg-getproperties",
    inputs: {
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        tsip: Task.TaskValueType.String,
        tsop: Task.TaskValueType.String,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.SecsGem
})
export class CustomMaterialTransferStatusRequestTask implements Task.TaskInstance, CustomMaterialTransferStatusRequestSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;
    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();

    public tsip: Task.Output<String> = new Task.Output<String>();

    public tsop: Task.Output<String> = new Task.Output<String>();


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

            try {
                let sendMessage: any;
                const parametersListMessage: SecsItem[] = [];
                sendMessage = { type: "S1F9", item: parametersListMessage};

                const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);

                 this.tsip.emit(SecsGem.getValue(SecsGem.getItemByPath(reply.item, "\[1]")).toString());
                this.tsop.emit(SecsGem.getValue(SecsGem.getItemByPath(reply.item, "\[2]")).toString());
                this.success.emit(true);
            } catch (error) {
                this.error.emit(error);
                this._logger.error(`Error on Complex Command Task: ${error.message}`);
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
/** customMaterialTransferStatusRequest Settings object */
export interface CustomMaterialTransferStatusRequestSettings {
    [key: string]: any;
}

