import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/setObjectProperty.default";

/**
 * @whatItDoes
 *
 * This task when activated updates a property value of a given object (the path for the property is provided in the task settings)
 *
 * @howToUse
 *
 *
 * ### Inputs
 * * `object` : **inputObject** - Object to update
 * * `any` : **activate** - Activate the task
 *
 * ### Outputs
 * * `object`: **outputObject** - Updated Object
 * * `bool`  : **success** - Triggered when the the task is executed with success
 * * `Error` : **error** - Triggered when the task failed for some reason
 *
 * ### Settings
 * See {@see SetObjectPropertySettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-logmessage",
    inputs: {
        inputObject: Task.TaskValueType.Object,
        activate: Task.INPUT_ACTIVATE,
    },
    outputs: {
        outputObject: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class SetObjectPropertyTask implements Task.TaskInstance, SetObjectPropertySettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Object to update*/
    public inputObject: Object;
    /** Activate task execution */
    public activate: any = undefined;

    /** **Outputs** */
    /** To output the updated object */
    public outputObject: Task.Output<Object> = new Task.Output<Object>();
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();


    /** Settings */
    /** Properties Settings */
    public inputs: SetObjectPropertyInputSettings[];
    public failIfNotExists: boolean = true;

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;


    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {
        if (changes["activate"]) {
            this.activate = undefined;
            if (this.inputObject != null && this.inputs != null) {
                this.inputs.forEach(input => {
                    this.updatedObjectProperty(input.path, this[input.name] || input.defaultValue);
                });
                this.outputObject.emit(this.inputObject);
                this.success.emit(true);
            } else {
                this._logger.error(`Missing mandatory inputs to perform the SetObjectProperty action!`);
            }

        }
    }

    /** Right after settings are loaded, create the needed dynamic outputs. */
    async onBeforeInit(): Promise<void> {
    }

    /** Initialize this task, register any event handler, etc */
    async onInit(): Promise<void> {
        this.failIfNotExists = Utilities.convertValueToType(this.failIfNotExists, Task.TaskValueType.Boolean , true);
    }

    /** Cleanup internal data, unregister any event handler, etc */
    async onDestroy(): Promise<void> {
    }

   /**
     * Navigates the object to get the property value based on the path
     * @param object object from the input
     * @param path path to use to browse the value
     */
    private updatedObjectProperty(path: string, value: any) {
        const parts = path.split(".");
        let level = 0;
        parts.reduce((a, b) => {
            level++;
            if (level === parts.length) {
                if (this.failIfNotExists && typeof a[b] === "undefined") {
                    throw new Error(`The property '${path}' does not exist in the given object`);
                }
                a[b] = value;
                return value;
            } else {
                if (this.failIfNotExists && typeof a[b] === "undefined") {
                    throw new Error(`The property '${path}' does not exist in the given object`);
                }
                if (typeof a[b] === "undefined" || typeof a[b] !== "object") {
                    a[b] = {};
                }
                return a[b];
            }
        }, this.inputObject);
    }

}

// Add settings here
/** SetObjectProperty Settings object */
export interface SetObjectPropertySettings {
    failIfNotExists: boolean;
    /** task inputs */
    inputs: SetObjectPropertyInputSettings[];
}

export interface SetObjectPropertyInputSettings {
    /** Input name */
    name: string;
    /** Object property path*/
    path: string;
    /** Input value type */
    valueType: Task.TaskComplexValueType;
    /** Default value if any */
    defaultValue?: any;
}
