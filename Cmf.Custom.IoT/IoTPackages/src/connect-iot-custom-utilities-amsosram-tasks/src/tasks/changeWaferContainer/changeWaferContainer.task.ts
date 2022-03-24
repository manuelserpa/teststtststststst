import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import { TaskDefaultSettings } from "@criticalmanufacturing/connect-iot-controller-engine/src/system";
import i18n from "./i18n/changeWaferContainer.default";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";
import { MovementData } from "../../persistence/model/movementData";
import { ContainerData } from "../../persistence/model/containerData";
import { WaferData } from "../../persistence/model/waferData";
import { MaterialData } from "../../persistence";


/** Default values for settings */
export const SETTINGS_DEFAULTS: ChangeWaferContainerSettings = {
    inputs: [],
    outputs: [],
    message: "",
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
 * See {@see ChangeWaferContainerSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-logmessage",
    inputs: {
        sourceContainerId: Task.TaskValueType.String,
        sourceLoadPort: Task.TaskValueType.Integer,
        sourceSlotNumber: Task.TaskValueType.Integer,

        equipmentWaferId: Task.TaskValueType.String,
        materialWaferId: Task.TaskValueType.String,

        targetContainerId: Task.TaskValueType.String,
        targetLoadPort: Task.TaskValueType.Integer,
        targetSlotNumber: Task.TaskValueType.Integer,

        activate: Task.INPUT_ACTIVATE,

        // Add more inputs here
        // example: containerId: System.PropertyValueType.String,
    },
    outputs: {
        // Add more outputs here:
        // Example:  notifyMessage: System.PropertyValueType.String,
        sourceContainer: Task.TaskValueType.Object,
        targetContainer: Task.TaskValueType.Object,
        wafer: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class ChangeWaferContainerTask implements Task.TaskInstance, ChangeWaferContainerSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;
    // ... more inputs
    public sourceContainerId: string;
    public sourceLoadPort: number;
    public sourceSlotNumber: number;

    public equipmentWaferId: string;
    public materialWaferId: string;

    public targetContainerId: string;
    public targetLoadPort: number;
    public targetSlotNumber: number;


    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();
    public sourceContainer: Task.Output<Object> = new Task.Output<Object>();
    public targetContainer: Task.Output<Object> = new Task.Output<Object>();
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
                let sourceContainer = await this._containerProcess.getContainer(this.sourceContainerId, this.sourceLoadPort);
                this._logger.warning("\n\nSourceContainer Gathered: " + JSON.stringify(sourceContainer));
                if (!sourceContainer) {
                    throw new Error(
                        `EI: On Container Change: Source Container ${this.sourceContainerId} (Load Port Id ${this.sourceLoadPort}) does not exist`);
                }
                let wafer = await this._containerProcess.getWafer(sourceContainer, this.sourceSlotNumber, this.equipmentWaferId, this.materialWaferId);
                this._logger.warning("\n\nWafer Gathered: " + JSON.stringify(wafer));
                if (!wafer) {
                    throw new Error(
                        `EI: On Container Change: Wafer on Slot ${this.sourceSlotNumber} - Equipment ID ${this.equipmentWaferId}// MES Id ${this.materialWaferId}) does not exist on Source Carrier ${sourceContainer.ContainerName}`);
                }
                let targetContainer = await this._containerProcess.getContainer(this.targetContainerId, this.targetLoadPort);
                this._logger.warning("\n\nTargetContainer Gathered: " + JSON.stringify(targetContainer));
                if (!targetContainer) {
                    targetContainer = await this._containerProcess.setContainer(this.targetContainerId, this.targetLoadPort, null);
                }
                let slotNumber = this.sourceSlotNumber;
                if (this.targetSlotNumber) {
                    slotNumber = this.targetSlotNumber;
                }

                targetContainer = await this._containerProcess.changeWaferFromContainer(sourceContainer, wafer, targetContainer, slotNumber);


                sourceContainer = await this._containerProcess.getContainer(this.sourceContainerId, this.sourceLoadPort);
                wafer = await this._containerProcess.getWafer(targetContainer, slotNumber, this.equipmentWaferId, this.materialWaferId);

                this.targetContainer.emit(targetContainer);
                this.sourceContainer.emit(sourceContainer);
                this.wafer.emit(wafer);

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
/** ChangeWaferContainer Settings object */
export interface ChangeWaferContainerSettings extends TaskDefaultSettings {
    message: string;
}
