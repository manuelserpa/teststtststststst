import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/checkIfRecipeExistsOnEquipment.default";


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
 * See {@see CheckIfRecipeExistsOnEquipmentSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-retrieve",
    inputs: {
        recipeNameOnEquipment: Task.TaskValueType.String,
        recipeList: Task.TaskValueType.Object,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        recipeExists: Task.TaskValueType.Boolean,
        recipeDoesNotExist: Task.TaskValueType.Boolean,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class CheckIfRecipeExistsOnEquipmentTask implements Task.TaskInstance, CheckIfRecipeExistsOnEquipmentSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;
    public recipeNameOnEquipment: string;
    public recipeList: String[];


    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();
    public recipeExists:  Task.Output<boolean> = new Task.Output<boolean>();
    public recipeDoesNotExist:  Task.Output<boolean> = new Task.Output<boolean>();


    /** Settings */
    /** Properties Settings */


    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {
        if (changes["activate"]) {
            // It is advised to reset the activate to allow being reactivated without the value being different
            this.activate = undefined;
            const name: string = this.recipeNameOnEquipment;
        if (this.recipeList == null || this.recipeList.length === 0) {
            this._logger.debug("No recipes on equipment");
            this.recipeDoesNotExist.emit(true);
        }
        const recipe = this.recipeList.find(r => r.toString().trim() === name.trim());
        if ( recipe != null) {
            this._logger.debug("Recipe " + name + " exists on equipment");
            this.recipeExists.emit(true);
        } else {
            this._logger.debug("Recipe " + name + " does not exist on equipment");
            this.recipeDoesNotExist.emit(true);
        }
        this.success.emit(true);
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
export interface CheckIfRecipeExistsOnEquipmentSettings {
    [key: string]: any;
}
