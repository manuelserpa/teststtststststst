import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/updateMaterialState.default";
import { ProcessMaterialHandler } from "../../persistence/implementation/processMaterialHandler";
import { MaterialData, MaterialStateEnum } from "../../persistence/model/materialData";
import { SubMaterialData, SubMaterialStateEnum } from "../../persistence/model/subMaterialData";
import { SlotOrderPickingDirectionEnum } from "../../utilities/slotOrderPickingDirectionEnum"
import { LotProcessDefinitionEnum } from "../../utilities/lotProcessDefinitionEnum";

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
 * See {@see UpdateMaterialStateSettings}
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
        firstRequestOfBatch: Task.TaskValueType.Buffer,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        material: Task.TaskValueType.Object,
        batchProcessComplete: Task.TaskValueType.Boolean,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class UpdateMaterialStateTask implements Task.TaskInstance, UpdateMaterialStateSettings {

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
    public firstRequestOfBatch: boolean;

    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();
    public material: Task.Output<MaterialData> = new Task.Output<MaterialData>();
    public batchProcessComplete: Task.Output<boolean> = new Task.Output<boolean>();

    /** Settings */
    /** Properties Settings */

    public materialState: MaterialStateEnum;
    public materialStateToSet: MaterialStateEnum;
    public lotProcessDefinition: LotProcessDefinitionEnum;

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
            this._logger.warning(`Starting the process`);
            try {

                if (this.materialName != null && this.materialName !== "") {
                    material = await this._processMaterial.getMaterialObjectFromName(this.materialName);

                    if (material == null) {
                        throw new Error(
                            `Error trying to get a material property: the material with name '${this.materialName}' does not exist on the persistence`)
                    }
                } else if (this.containerName != null && this.containerName !== "") {
                    material = await this._processMaterial.getMaterialByCarrier(this.containerName);

                    if (material == null &&
                        (this.lotProcessDefinition === null ||
                            (this.lotProcessDefinition !== null && this.lotProcessDefinition.toString() !== LotProcessDefinitionEnum.Batch.toString()))) {
                        throw new Error(
                            `Error trying to get a material property: the material with carrier '${this.containerName}' does not exist on the persistence`)
                    } else if (material == null && this.lotProcessDefinition !== null
                        && this.lotProcessDefinition.toString() !== LotProcessDefinitionEnum.Batch.toString()
                        && this.firstRequestOfBatch) {
                        throw new Error(
                            `Error trying to get a material property: there is no Material matching the batch process request with carrier '
                            ${this.containerName}' on the persistence`)
                    }
                } else if (this.controlJobId != null && this.controlJobId !== "") {
                    material = await this._processMaterial.getMaterialByControlJobId(this.controlJobId);
                    if (material == null) {
                        throw new Error(
                            `Error trying to get an material property: the material with Control Job Id '${this.controlJobId}' does not exist on the persistence`);
                    }
                } else if (this.processJobId != null && this.processJobId !== "") {
                    material = await this._processMaterial.getMaterialByProcessJobId(this.processJobId);
                    if (material == null) {
                        throw new Error(
                            `Error trying to get an material property: the material with Process Job Id '${this.processJobId}' does not exist on the persistence`);
                    }
                } else if (this.loadPortId != null && this.materialState !== null) {
                    material = await this._processMaterial.getMaterialByLoadPortIdAndMaterialState(this.loadPortId.toString(), this.materialState);

                    if (material == null && (this.lotProcessDefinition === null ||
                        (this.lotProcessDefinition !== null && this.lotProcessDefinition.toString() !== LotProcessDefinitionEnum.Batch.toString()))) {
                        throw new Error(
                            `Error trying to get an material property: the material in load port id '
                            ${this.loadPortId}' and state '${this.materialState}' does not exist on the persistence`)
                    } else if (material == null
                        && this.lotProcessDefinition !== null && this.lotProcessDefinition.toString() !== LotProcessDefinitionEnum.Batch.toString()
                        && this.firstRequestOfBatch) {
                        throw new Error(
                            `Error trying to get a material property: there is no Material matching the batch process request with load port id '
                            ${this.loadPortId}' and state '${this.materialState}' on the persistence`)
                    }
                } else if (this.materialState !== null) {
                    material = await this._processMaterial.getMaterialByState(this.materialState);

                    if (material == null && (this.lotProcessDefinition === null ||
                        (this.lotProcessDefinition !== null && this.lotProcessDefinition.toString() !== LotProcessDefinitionEnum.Batch.toString()))) {
                        throw new Error(
                            `Error trying to get a material property: the material with state '${this.materialState}' does not exist on the persistence`)
                    } else if (material == null
                        && this.lotProcessDefinition !== null && this.lotProcessDefinition.toString() !== LotProcessDefinitionEnum.Batch.toString()
                        && this.firstRequestOfBatch) {
                        throw new Error(
                            `Error trying to get a material property: there is no Material matching the batch process request with state '
                            ${this.materialState}' on the persistence`)
                    }
                } else {
                    throw new Error(
                        `Error: No valid information to retrieve material from persistence`)
                }

                if (material != null) {
                    material.MaterialState = this.materialStateToSet;
                    this._processMaterial.updateMaterial(material);
                    this.material.emit(material);
                } else if (this.lotProcessDefinition !== null
                    && this.lotProcessDefinition.toString() !== LotProcessDefinitionEnum.Batch.toString()) {
                    this.batchProcessComplete.emit(true);
                }

                this.success.emit(true);

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
/** UpdateMaterialState Settings object */
export interface UpdateMaterialStateSettings {
    materialState: MaterialStateEnum;
    materialStateToSet: MaterialStateEnum;
    lotProcessDefinition: LotProcessDefinitionEnum;
}
