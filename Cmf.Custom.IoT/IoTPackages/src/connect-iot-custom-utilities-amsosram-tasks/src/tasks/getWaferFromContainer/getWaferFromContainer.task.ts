import { element } from "@angular/core/src/render3";
import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import { TaskDefaultSettings } from "@criticalmanufacturing/connect-iot-controller-engine/src/system";
import i18n from "./i18n/getWaferFromContainer.default";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";

/** Default values for settings */
export const SETTINGS_DEFAULTS: GetWaferFromContainerSettings = {
    inputs: [],
    outputs: [],
    message: ""
}

/**
 * @whatItDoes
 *
 * This task does something ... describe here
 *
 * @howToUse
 *
 * yada yada yada
 *
 * ### Inputs
 * * `any` : **activate** - Activate the task
 *
 * ### Outputs

 * * `bool`  : ** success ** - Triggered when the the task is executed with success
 * * `Error` : ** error ** - Triggered when the task failed for some reason
 *
 * ### Settings
 * See {@see GetWaferFromContainerSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-logmessage",
    inputs: {
        containerId: Task.TaskValueType.String,
        loadPort: Task.TaskValueType.Integer,
        slot: Task.TaskValueType.Integer,
        equipmentWaferId: Task.TaskValueType.String,
        materialWaferId: Task.TaskValueType.String,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        container: Task.TaskValueType.Object,
        wafer: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class GetWaferFromContainerTask implements Task.TaskInstance, GetWaferFromContainerSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;
    public loadPort: number;
    public containerId: string;
    public slot: number;
    public equipmentWaferId: string;
    public materialWaferId: string;

    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();
    /** Container Output */
    public container: Task.Output<Object> = new Task.Output<Object>();
    /** Wafer Output */
    public wafer: Task.Output<Object> = new Task.Output<Object>();

    /** Settings */
    inputs: Task.TaskInput[];
    outputs: Task.TaskOutput[];
    /** Properties Settings */
    message: string;

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject("GlobalContainerProcessHandler")
    private _containerProcess: ContainerProcessHandler;

    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {
        if (changes["activate"]) {
            // It is advised to reset the activate to allow being reactivated without the value being different
            this.activate = undefined;
            try {
                const container = await this._containerProcess.getContainer(this.containerId, this.loadPort);
                const wafer = await this._containerProcess.getWafer(container, this.slot, this.equipmentWaferId, this.materialWaferId);
                this.container.emit(container);
                this.wafer.emit(wafer);
                this.success.emit(true);
            } catch (e) {
                // or
                throw new Error(e);
            }
        }
    }

    /** Right after settings are loaded, create the needed dynamic outputs. */
    async onBeforeInit(): Promise<void> {
    }

    /** Initialize this task, register any event handler, etc */
    async onInit(): Promise<void> {
    }

    /** Cleanup internal data, unregister any event handler, etc */
    async onDestroy(): Promise<void> {
    }

    // On every tick (use instead of onChanges if necessary)
    // async onCheck(): Promise<void> {
    // }
}

// Add settings here
/** GetWaferFromContainer Settings object */
export interface GetWaferFromContainerSettings extends TaskDefaultSettings {
    message: string;
}
