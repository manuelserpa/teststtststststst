import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/storeMaterialProperty.default";
import { ProcessMaterialHandler } from "../../persistence/implementation/processMaterialHandler";
import { MaterialData } from "../../persistence/model/materialData";


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
 * See {@see StoreMaterialPropertySettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-store",
    inputs: {
        materialName: Task.TaskValueType.String,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        material: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class StoreMaterialPropertyTask implements Task.TaskInstance, StoreMaterialPropertySettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    private materialObject: MaterialData;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;
    public materialId: string;


    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();
    public material: Task.Output<MaterialData> = new Task.Output<MaterialData>();

    /** Settings */
    /** Properties Settings */
    public inputs: StoreMaterialPropertyInputSettings[];
    public clearActivate: boolean = true;


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

            if (this.clearActivate) {
                this.activate = undefined;
            }

            if (this.materialName != null &&  this.materialName !== "" && this.inputs != null) {
                try {
                    this.materialObject = await this._processMaterial.getMaterialObjectFromName(this.materialName);
                    this.inputs.forEach(input => {
                        this._logger.info(`Trying to set property '${input.path}' in material '${this.materialName}'`);
                        this.updatedMaterialProperty(input.path, this[input.name] || input.defaultValue);
                    });
                    await this._processMaterial.updateMaterial(this.materialObject);

                    this.success.emit(true);
                    this.material.emit(this.materialObject);
                } catch (error) {

                    this._logger.error(`Error trying to update the material property: ${error.message}`);
                    throw new Error (error);

                }

            }
        }
    }

    /** Right after settings are loaded, create the needed dynamic outputs. */
    async onBeforeInit(): Promise<void> {
    }

    /** Initialize this task, register any event handler, etc */
    async onInit(): Promise<void> {
        this.clearActivate = Utilities.convertValueToType(this.clearActivate, Task.TaskValueType.Boolean , true);
    }

    /** Cleanup internal data, unregister any event handler, etc */
    async onDestroy(): Promise<void> {
    }


    /**
     * Navigates the object to get the property value based on the path
     * @param object object from the input
     * @param path path to use to browse the value
     */
    private updatedMaterialProperty(path: string, value: any) {
        const parts = path.split(".");
        let level = 0;
        parts.reduce((a, b) => {
            level++;
            if (level === parts.length) {
                if (typeof a[b] === "undefined") {
                    throw new Error(`The property '${path}' does not exist on the material object`);
                }
                a[b] = value;
                return value;
            } else {
                if (typeof a[b] === "undefined" || typeof a[b] !== "object") {
                    throw new Error(`The property '${path}' does not exist on the material object`);
                }
                return a[b];
            }
        }, this.materialObject);
    }

}

// Add settings here
/** StoreMaterialProperty Settings object */
export interface StoreMaterialPropertySettings {
    inputs: StoreMaterialPropertyInputSettings[];
    clearActivate: boolean;
}
export interface StoreMaterialPropertyInputSettings {
    /** Input name */
    name: string;
    /** Object property path*/
    path: string;
    /** Input value type */
    valueType: Task.TaskComplexValueType;
    /** Default value if any */
    defaultValue?: any;
}
