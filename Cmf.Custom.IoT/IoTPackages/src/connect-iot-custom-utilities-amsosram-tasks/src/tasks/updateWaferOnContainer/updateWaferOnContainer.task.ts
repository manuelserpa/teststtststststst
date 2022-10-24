import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import { TaskDefaultSettings } from "@criticalmanufacturing/connect-iot-controller-engine/src/system";
import i18n from "./i18n/updateWaferOnContainer.default";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";
import { MovementData } from "../../persistence/model/movementData";
import { ContainerData } from "../../persistence/model/containerData";
import { WaferData } from "../../persistence/model/waferData";
import { MaterialData } from "../../persistence";

/** Default values for settings */
export const SETTINGS_DEFAULTS: UpdateWaferOnContainerSettings = {
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
 * See {@see UpdateWaferOnContainerSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-logmessage",
    inputs: {
        material: Task.TaskValueType.Object,
        loadPort: Task.TaskValueType.Integer,
        slotNumber: Task.TaskValueType.Integer,
        equipmentWaferId: Task.TaskValueType.String,
        parentMaterialId: Task.TaskValueType.String,
        activate: Task.INPUT_ACTIVATE,
    },
    outputs: {
        container: Task.TaskValueType.Object,
        wafer: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR,
    }
})
export class UpdateWaferOnContainerTask implements Task.TaskInstance, UpdateWaferOnContainerSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;
    // ... more inputs
    public material: MaterialData;
    public loadPort: number;
    public slotNumber: number;
    public equipmentWaferId: string;
    public parentMaterialId: string;

    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();
    public container: Task.Output<Object> = new Task.Output<Object>();
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
                // ... code here
                // create slot positions
                const containerName = this.material.ContainerName;
                let movementList;
                if (this.material.SorterJobInformation) {
                    movementList = <MovementData[]>JSON.parse(this.material.SorterJobInformation.MovementList);
                }

                const slotNumber = this.slotNumber;
                const loadPort = this.loadPort;
                let slotMES;
                let movement;
                let container: ContainerData = await this._containerProcess.getContainer(this.containerId, loadPort);
                if (this.containerId === containerName) {
                    slotMES = this.material.SubMaterials.find(s => s.Slot === slotNumber);
                } else if (movementList) {
                    movement = movementList.find(w => w.SourceContainer === this.containerId && w.SourcePosition === slotNumber)
                }

                let wafer: WaferData;

                if (slotMES) {
                    wafer = await this._containerProcess.updateWaferOnContainer(containerName, loadPort,
                        slotNumber, slotMES.MaterialName, this.equipmentWaferId, this.parentMaterialId);
                } else if (movement) {
                    wafer = await this._containerProcess.updateWaferOnContainer(containerName, loadPort,
                        slotNumber, movement.MaterialName, this.equipmentWaferId, this.parentMaterialId);
                } else {
                    wafer = await this._containerProcess.updateWaferOnContainer(containerName, loadPort,
                        slotNumber, null, this.equipmentWaferId, this.parentMaterialId);
                }

                container = await this._containerProcess.getContainer(this.containerId, loadPort);

                this.wafer.emit(wafer);
                this.container.emit(container);
                this.success.emit(true);
            } catch (e) {
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
/** UpdateWaferOnContainer Settings object */
export interface UpdateWaferOnContainerSettings extends TaskDefaultSettings {
    message: string;
}
