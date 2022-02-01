import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/deleteMaterialOnPersistence.default";
import { ProcessMaterialHandler } from "../../persistence/implementation/processMaterialHandler";
import { MaterialData } from "../../persistence/model/materialData";


/**
 * @whatItDoes
 *
 * This task upon activation will delete the material object, for a given materialId, from the persistence
 *
 * @howToUse
 *
 * The task will be activated by receiving a value in the input port.
 * To perform this action an material id must be provided in the input port 'materialId'.
 *
 * ### Inputs
 * * `string` : **materialId** - Material Id
 * * `any` : **activate** - Activate the task
 *
 * ### Outputs
 * * `bool`  : **success** - Triggered when the the task is executed with success
 * * `Error` : **error** - Triggered when the task failed for some reason
 *
 * ### Settings
 * See {@see DeleteMaterialOnPersistenceSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-logmessage",
    inputs: {
        materials: Task.TaskValueType.Object,
        activate: Task.INPUT_ACTIVATE,
    },
    outputs: {
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class DeleteMaterialOnPersistenceTask implements Task.TaskInstance, DeleteMaterialOnPersistenceSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Id of the material to be deleted from the persistence */
    public materials: MaterialData[];
    /** Activate task execution */
    public activate: any = undefined;

    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();


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
            if (this.materials != null) {
                try {
                    this._logger.info(`Starting the material deleting process: MaterialId ${this.materialId}`);
                    this.materials.forEach(async material => {
                    await this._processMaterial.deleteMaterialFromPersistence(material.MaterialId);
                    });
                    this.success.emit(true);
                } catch (error) {
                    this._logger.error(`Error while deleting material: ${error.message}`);
                    this.error.emit(error);
                }

            } else {
                this._logger.info(`Trying to delete the material from persistence but no 'materialId' was provided!`)
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

}

// Add settings here
/** DeleteMaterialOnPersistence Settings object */
export interface DeleteMaterialOnPersistenceSettings {
    [key: string]: any;
}
