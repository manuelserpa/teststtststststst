import { Task, Dependencies, System, DI, TYPES } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customReadId.default";

import { SecsGem } from "../../common/secsGemItem"
import { SecsItem } from "../../common/secsItem";
import { SubMaterialStateEnum } from "../../persistence/model/subMaterialData";


/**
 * @whatItDoes
 *
 * Implements Complex Secs Gem Commands
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
 * See {@see CustomReadIdSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-secsgem-iot-lg-setrecipe",
    inputs: {
        TargetId: Task.TaskValueType.String,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        TargetIdResult: Task.TaskValueType.String,
        AcknowledgeCode: Task.TaskValueType.String,
        MaterialId: Task.TaskValueType.String,
        StatusList: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.SecsGem
})
export class CustomReadIdTask implements Task.TaskInstance, CustomReadIdSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;

    public TargetId: string = "";


    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();
    public TargetIdResult: Task.Output<String> = new Task.Output<String>();
    public AcknowledgeCode: Task.Output<String> = new Task.Output<String>();
    public MaterialId: Task.Output<String> = new Task.Output<String>();
    /** To output Status list */
    public StatusList: Task.Output<Object> = new Task.Output<Object>();


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
                    type: "S18F9", item: {
                        type: "A", value: this.TargetId
                    }
                }
                const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);
                let successFound = false;

                if (!reply && !reply.item) {
                    const error = new Error(`EI: Read Id Failed to reply`);
                    this.error.emit(error);
                    throw error;
                }

                this._logger.warning(JSON.stringify(reply.item));
                const targetId = reply.item.value[0].value;
                const acknowledgeCode = reply.item.value[1].value;
                const materialId = reply.item.value[2].value;
                const statusList = reply.item.value[3].value;

                if (targetId === this.TargetId &&
                    acknowledgeCode === "NO" &&
                    materialId) {
                    successFound = true;
                }

                this.TargetIdResult.emit(targetId);
                this.AcknowledgeCode.emit(acknowledgeCode);
                this.MaterialId.emit(materialId);
                this.StatusList.emit(statusList);

                if (!successFound) {
                    const error = new Error(`EI: Read Id failed with result: Target id: ${targetId}; AcknowledgeCode: ${acknowledgeCode}; MaterialId: ${materialId}`);
                    this.error.emit(error);
                    throw error;
                }

                this.success.emit(true);
            } catch (error) {
                this.error.emit(error);
                this._logger.error(`EI: Error on Read Id: ${error.message}`);
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
/** customReadId Settings object */
export interface CustomReadIdSettings {
    [key: string]: any;
}



