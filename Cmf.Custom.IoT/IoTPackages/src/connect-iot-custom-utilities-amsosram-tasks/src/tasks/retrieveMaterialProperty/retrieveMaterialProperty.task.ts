import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/retrieveMaterialProperty.default";
import { ProcessMaterialHandler } from "../../persistence/implementation/processMaterialHandler";
import { MaterialData, MaterialStateEnum } from "../../persistence/model/materialData";
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
 * See {@see RetrieveMaterialPropertySettings}
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
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        material: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class RetrieveMaterialPropertyTask implements Task.TaskInstance, RetrieveMaterialPropertySettings {

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

    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();
    public material:  Task.Output<Object> = new Task.Output<Object>();

    /** Settings */
    /** Properties Settings */
    public outputs: RetrieveMaterialPropertyOutputSettings[];

    public materialState: MaterialStateEnum;
    public retrieveAllMaterialsInCondition: boolean;

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
            this.activate = undefined;
            let material: MaterialData;
            try {
                if (!this.retrieveAllMaterialsInCondition) {
                if (this.materialName != null && this.materialName !== "") {
                    material = await this._processMaterial.getMaterialObjectFromName(this.materialName);

                    if (material == null) {
                        throw new Error (
                            `Error trying to get an material property: the material with name '${this.materialName}' does not exist on the persistence`);
                    }
                } else if (this.containerName != null && this.containerName !== "") {
                    material = await this._processMaterial.getMaterialByCarrier(this.containerName);

                    if (material == null) {
                        throw new Error (
                            `Error trying to get an material property: the material with carrier '${this.containerName}' does not exist on the persistence`);
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
                            ${this.loadPortId}' and state '${this.materialState}' does not exist on the persistence`);
                    }
                } else if (this.materialState !== null) {
                    material = await this._processMaterial.getMaterialByState(this.materialState);

                    if (material == null) {
                        throw new Error (
                            `Error trying to get an material property: the material with state '${this.materialState}' does not exist on the persistence`);
                    }
                } else {
                    throw new Error (
                        `Error: No valid information to retrieve material from persistence`);
                }
                    if (this.outputs != null) {
                        this.outputs.forEach(output => {
                            const value = this.retrieveMaterialProperty(material, output.path);
                            this[output.name].emit(value);
                            this._logger.info("Retrieved Material Property " + output.name + " with value " + value)

                        });
                    }
                    this.success.emit(true);
                    this.material.emit(material);
                } else {
                    let materials: MaterialData[] = [];

                    if (this.containerName != null && this.containerName !== "") {
                        materials = await this._processMaterial.getAllMaterialsInCarrier(this.containerName);
                        if (materials == null) {
                            throw new Error (
                                `No material was found with carrier '${this.containerName}' on the persistence`);
                        }
                    } else if (this.controlJobId != null && this.controlJobId !== "") {
                        material = await this._processMaterial.getMaterialByControlJobId(this.controlJobId);
                        if (material == null) {
                            throw new Error (
                                `Error trying to get an material property: the material with Control Job Id '${this.controlJobId}' does not exist on the persistence`);
                        }
                    } else if (this.loadPortId != null && this.materialState !== null) {
                        materials = await this._processMaterial.getAllMaterialByLoadPortIdAndMaterialState(this.loadPortId.toString(), this.materialState);
                        if (materials == null) {
                            throw new Error (
                            `No material was found with load port id '
                            ${this.loadPortId}' and state '${this.materialState}' on the persistence`);
                        }
                    } else if (this.materialState !== null) {
                        materials = await this._processMaterial.getAllMaterialsOnState(this.materialState);
                        if (materials == null) {
                            throw new Error (
                            `No material was found with state '${this.materialState}' on the persistence`);
                        }
                    } else {
                        throw new Error (
                            `Error: No valid information to retrieve material from persistence`);
                    }

                    this.success.emit(true);
                    this.material.emit(materials);
                }
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

   private retrieveMaterialProperty(material: any, path: string ): string {
        const parts = path.split( "." );
        let property = material;

        for (let i = 0; i < parts.length; i++) {
            if (property == null) {
                throw new Error(`The property '${path}' does not exist on the material object`);
            }
            property = property[parts[i]];
        }
        return property;

   }

}

// Add settings here
/** RetrieveMaterialProperty Settings object */
export interface RetrieveMaterialPropertySettings {
    materialState: MaterialStateEnum;
    retrieveAllMaterialsInCondition: boolean;
    outputs: RetrieveMaterialPropertyOutputSettings[];
}
export interface RetrieveMaterialPropertyOutputSettings {
    /** Output name */
    name: string;
    /** Path of the object property */
    path: string;
    /** Output value type */
    valueType: Task.TaskComplexValueType;
}
