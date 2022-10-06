import { element } from "@angular/core/src/render3";
import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import { TaskDefaultSettings } from "@criticalmanufacturing/connect-iot-controller-engine/src/system";
import i18n from "./i18n/updateContainer.default";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";
import { WaferData } from "../../persistence";

/** Default values for settings */
export const SETTINGS_DEFAULTS: UpdateContainerSettings = {
    inputs: [],
    outputs: [],
    message: ""
}

interface Movement {
    MaterialName: string
    SourceContainer: string
    SourcePosition: number
    DestinationContainer: string
    DestinationPosition: number
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
 * See {@see UpdateContainerSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-logmessage",
    inputs: {
        containerId: Task.TaskValueType.String,
        slotMap: Task.TaskValueType.Object,
        loadPort: Task.TaskValueType.Integer,
        slots: Task.TaskValueType.String,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        container: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class UpdateContainerTask implements Task.TaskInstance, UpdateContainerSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;
    public loadPort: number;
    public containerId: string;
    public slotMap: object;
    public slots: string;

    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();
    /** Container Output */
    public container: Task.Output<Object> = new Task.Output<Object>();


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
                let slotsParsed: WaferData[] = null;

                if (this.slots &&
                    this.slots.length > 0) {
                    slotsParsed = JSON.parse(this.slots) as WaferData[];
                }

                const container = await this._containerProcess.updateContainer(this.containerId, this.loadPort, this.slotMap, slotsParsed);
                this.container.emit(container);
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
/** UpdateContainer Settings object */
export interface UpdateContainerSettings extends TaskDefaultSettings {
    message: string;
}
