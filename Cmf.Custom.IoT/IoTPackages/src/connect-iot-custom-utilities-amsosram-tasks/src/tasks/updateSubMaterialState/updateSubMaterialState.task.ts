import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/updateSubMaterialState.default";
import { ProcessMaterialHandler } from "../../persistence/implementation/processMaterialHandler";
import { MaterialData, MaterialStateEnum } from "../../persistence/model/materialData";
import { SubMaterialData, SubMaterialStateEnum } from "../../persistence/model/subMaterialData";
import { SlotOrderPickingDirectionEnum } from "../../utilities/slotOrderPickingDirectionEnum"

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
 * See {@see UpdateSubMaterialStateSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-retrieve",
    inputs: {
        materialName: Task.TaskValueType.String,
        containerName: Task.TaskValueType.String,
        controlJobId: Task.TaskValueType.String,
        processJobId: Task.TaskValueType.String,
        loadPortId: Task.TaskValueType.Integer,
        subMaterialName: Task.TaskValueType.String,
        subMaterialSlot: Task.TaskValueType.Integer,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        material: Task.TaskValueType.Object,
        subMaterial: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class UpdateSubMaterialStateTask implements Task.TaskInstance, UpdateSubMaterialStateSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;

    public materialName: string;
    public containerName: string;
    public processJobId: string;
    public controlJobId: string;
    public loadPortId: number;

    public subMaterialName: string;
    public subMaterialSlot: number;

    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();
    public material:  Task.Output<MaterialData> = new Task.Output<MaterialData>();
    public subMaterial:  Task.Output<SubMaterialData> = new Task.Output<SubMaterialData>();


    /** Settings */
    /** Properties Settings */

    public materialState: MaterialStateEnum;
    public subMaterialState: SubMaterialStateEnum;
    public slotOrderPickingDirection: SlotOrderPickingDirectionEnum
    public subMaterialStateToSet: SubMaterialStateEnum;

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject("GlobalProcessDataHandler")
    private _processMaterial: ProcessMaterialHandler;

    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {
        if (changes["activate"]) {
            // It is advised to reset the activate to allow being reactivated without the value being different
            this.activate = undefined;
            let material: MaterialData;
            try {
                if (this.materialName != null && this.materialName !== "") {
                    material = await this._processMaterial.getMaterialObjectFromName(this.materialName);

                    if (material == null) {
                        throw new Error (
                            `Error trying to get an material property: the material with name '${this.materialName}' does not exist on the persistence`)
                    }
                } else if (this.containerName != null && this.containerName !== "") {
                    material = await this._processMaterial.getMaterialByCarrier(this.containerName);

                    if (material == null) {
                        throw new Error (
                            `Error trying to get an material property: the material with carrier '${this.containerName}' does not exist on the persistence`)
                    }
                } else if (this.controlJobId != null && this.controlJobId !== "") {
                    material = await this._processMaterial.getMaterialByControlJobId(this.controlJobId);
                    if (material == null) {
                        throw new Error (
                            `Error trying to get an material property: the material with Control Job Id '${this.controlJobId}' does not exist on the persistence`);
                    }
                } else if (this.processJobId != null && this.processJobId !== "") {
                    material = await this._processMaterial.getMaterialByProcessJobId(this.processJobId);
                    if (material == null) {
                        throw new Error (
                            `Error trying to get an material property: the material with Process Job Id '${this.processJobId}' does not exist on the persistence`);
                    }
                } else if (this.loadPortId != null && this.materialState !== null) {
                    material = await this._processMaterial.getMaterialByLoadPortIdAndMaterialState(this.loadPortId.toString(), this.materialState);

                    if (material == null) {
                        throw new Error (
                            `Error trying to get an material property: the material in load port id '
                            ${this.loadPortId}' and state '${this.materialState}' does not exist on the persistence`)
                    }
                } else if (this.materialState !== null) {
                    material = await this._processMaterial.getMaterialByState(this.materialState);

                    if (material == null) {
                        throw new Error (
                            `Error trying to get an material property: the material with state '${this.materialState}' does not exist on the persistence`)
                    }
                } else {
                    throw new Error (
                        `Error: No valid information to retrieve material from persistence`)
                }

                let subMaterial: SubMaterialData;

                if (this.subMaterialName != null && this.subMaterialName !== "") {
                    subMaterial = material.SubMaterials.find(o => o.MaterialName.toLowerCase() === this.subMaterialName.toLowerCase())

                    if (subMaterial == null) {
                        throw new Error (
                            `Error trying to get a submaterial property: the submaterial with name '
                            ${this.subMaterialName}' does not exist on the persistence`)
                    }
                } else if (this.subMaterialSlot != null) {
                    subMaterial = material.SubMaterials.find(o => o.Slot.toString() === this.subMaterialSlot.toString())

                    if (subMaterial == null) {
                        throw new Error (
                            `Error trying to get a submaterial property: the submaterial with the slot '
                            ${this.subMaterialSlot}' does not exist on the persistence`)
                    }
                } else if (this.subMaterialState !== null && this.slotOrderPickingDirection !== null) {
                        subMaterial = material.SubMaterials.sort((n1, n2) => {
                            if (this.slotOrderPickingDirection === SlotOrderPickingDirectionEnum.Descending) {
                                return n2.Slot - n1.Slot;
                               } else {
                                return n1.Slot - n2.Slot;
                               }
                        }).find(o => o.MaterialState === this.subMaterialState)
                    if (subMaterial == null) {
                        throw new Error (
                            `Error trying to get an sub material: the sub material with state '${this.subMaterialState}' does not exist on the persistence`)
                    }
                } else {
                    throw new Error (
                        `Error: No valid information to retrieve sub material from persistence`)
                }

                   subMaterial.MaterialState = this.subMaterialStateToSet;
                   this._processMaterial.updateMaterial(material);

                   this.success.emit(true);
                   this.material.emit(material);
                   this.subMaterial.emit(subMaterial);

            } catch (error) {
                this._logger.error(`Error occurred: ${error.message}`);
                this.error.emit(error);
            }
        }
    }

    /** Right after settings are loaded, create the needed dynamic outputs. */
    async onBeforeInit(): Promise<void> {
        if (this.outputs) {
            for (const output of this.outputs) {
                this[output.name] = new Task.Output<any>();
            }
        }
    }

    /** Initialize this task, register any event handler, etc */
    async onInit(): Promise<void> {
    }

    /** Cleanup internal data, unregister any event handler, etc */
    async onDestroy(): Promise<void> {
    }

}

// Add settings here
/** UpdateSubMaterialState Settings object */
export interface UpdateSubMaterialStateSettings {
    materialState: MaterialStateEnum;
    subMaterialState: SubMaterialStateEnum;
    slotOrderPickingDirection: SlotOrderPickingDirectionEnum;
    subMaterialStateToSet: SubMaterialStateEnum;
}
