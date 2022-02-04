import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/storeMaterialObject.default";
import { ProcessMaterialHandler } from "../../persistence/implementation/processMaterialHandler";
import { MaterialData } from "../../persistence/model/materialData";
import * as inversify from "inversify";

/**
 * @whatItDoes
 *
 * This task sends the material parameters to the line controller.
 * Handles all responses and request properties changes, regarding the material download.
 *
 * @howToUse
 *
 * ### Inputs
 * * `any` : **activate** - Activate the task
 *
 * ### Outputs
 * * `bool`  : ** success ** - Triggered when the the task is executed with success
 * * `Error` : ** error ** - Triggered when the task failed for some reason
 *
 * ### Settings
 * See {@see StoreMaterialObjectSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-store",
    inputs: {
        activate: Task.INPUT_ACTIVATE,
        materialObject: System.PropertyValueType.Object,
    },
    outputs: {
        materialDownloaded: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.All
})
export class StoreMaterialObjectTask implements Task.TaskInstance, StoreMaterialObjectSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;

    /** Material data to download */
    public materialObject: MaterialData[] = null;

    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();

    public materialDownloaded: Task.Output<MaterialData[]> = new Task.Output<MaterialData[]>();


    /** Properties Settings */

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
            this.materialObject.forEach(async material => {
            await this._processMaterial.trackIn(<MaterialData>material);
            });

            this.materialDownloaded.emit(this.materialObject);
            this.success.emit(true);
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

    /**
     * creates the material properties structure that will be set by the driver.
     * @param material number of he material slot
     */

}

// Add settings here
/** StoreMaterialObject Settings object */
export interface StoreMaterialObjectSettings {
    [key: string]: any;

}
