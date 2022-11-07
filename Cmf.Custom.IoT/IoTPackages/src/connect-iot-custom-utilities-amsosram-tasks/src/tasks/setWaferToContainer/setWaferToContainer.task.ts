import { element } from "@angular/core/src/render3";
import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import { TaskDefaultSettings } from "@criticalmanufacturing/connect-iot-controller-engine/src/system";
import i18n from "./i18n/setWaferToContainer.default";
import { SubMaterialData } from "../../persistence";
import { MaterialData } from "../../persistence";
import { MovementData } from "../../persistence/model/movementData";
import { WaferData } from "../../persistence/model/waferData";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";
import { ContainerProcess } from "../../persistence/model/containerProcess";
import { ContainerData } from "../../persistence/model/containerData";

/** Default values for settings */
export const SETTINGS_DEFAULTS: SetWaferToContainerSettings = {
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
 * See {@see SetWaferToContainerSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-logmessage",
    inputs: {
        material: Task.TaskValueType.Object,
        containerId: Task.TaskValueType.String,
        loadPort: Task.TaskValueType.Integer,
        slotNumber: Task.TaskValueType.Integer,
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
export class SetWaferToContainerTask implements Task.TaskInstance, SetWaferToContainerSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;
    // ... more inputs
    public material: MaterialData;
    public containerId: string;
    public loadPort: number;
    public slotNumber: number;
    public equipmentWaferId: string;
    public materialWaferId: string;

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
                // create slot positions
                const containerName = this.material.ContainerName;
                let movementList;
                if (this.material.SorterJobInformation && this.material.SorterJobInformation.LogisticalProcess.toUpperCase() !== "MAPCARRIER") {
                    // this._logger.warning("Entered here: 0!!");
                    // this._logger.warning("\n\n\nJSON: " + JSON.stringify(this.material));
                    movementList = <MovementData[]>JSON.parse(this.material.SorterJobInformation.MovementList);
                }

                const slotNumber = this.slotNumber;
                const loadPort = this.loadPort;
                let slotMES;
                let movement;
                let container: ContainerData = await this._containerProcess.getContainer(this.containerId, loadPort);
                if (this.material) {

                    if (this.containerId === containerName && this.material.SubMaterials != null) {
                        // this._logger.warning("Entered here: 1");
                        slotMES = this.material.SubMaterials.find(s => s.Slot.toString() === Number(slotNumber).toString());
                        // this._logger.warning("SlotMES: " + JSON.stringify(slotMES));
                    } else if (movementList) {
                        movement = movementList.find(w => (w.SourceContainer === this.containerId && w.SourcePosition.toString() === slotNumber.toString())
                            || (w.DestinationContainer === this.containerId && w.Destination.toString() === slotNumber.toString()))
                    }
                }
                let wafer: WaferData = {} as WaferData;
                if (container) {

                    if (this.material) {
                        wafer.ParentMaterialName = this.material.MaterialName;

                        if (slotMES) {
                            wafer.MaterialWaferId = slotMES.MaterialName;

                        }
                        if (movement) {
                            wafer.MaterialWaferId = movement.MaterialName;
                        }

                    }

                    if (wafer.MaterialWaferId === undefined && this.equipmentWaferId !== undefined) {
                        wafer.MaterialWaferId = this.equipmentWaferId;
                    }

                    wafer.Slot = slotNumber;
                    if (this.equipmentWaferId) {
                        wafer.EquipmentWaferId = this.equipmentWaferId;
                    }

                    wafer = await this._containerProcess.setWaferDataToContainerData(container, wafer);

                } else {

                    let materialWaferId: string;
                    let equipmentWaferId: string;
                    let parentMaterialName: string;

                    if (slotMES) {
                        materialWaferId = slotMES.MaterialName;
                    }
                    if (movement) {
                        materialWaferId = movement.MaterialName;
                    }
                    if (this.equipmentWaferId) {
                        equipmentWaferId = this.equipmentWaferId
                    }

                    if (this.material) {
                        parentMaterialName = this.material.MaterialName;
                    }

                    wafer = await this._containerProcess.setWaferToContainer(
                        containerName,
                        loadPort,
                        slotNumber,
                        equipmentWaferId,
                        materialWaferId,
                        parentMaterialName
                    );
                }

                container = await this._containerProcess.getContainer(this.containerId, loadPort);

                this.wafer.emit(wafer);
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
/** SetWaferToContainer Settings object */
export interface SetWaferToContainerSettings extends TaskDefaultSettings {
    message: string;
}
