import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/storeSyncJob.default";
import { SyncInformationJobHandler } from "../../persistence/implementation/syncInformationJobHandler";
import { SyncInformationJobData } from "../../persistence/model/syncInformationData";


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
 * See {@see StoreSyncJobSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-store",
    inputs: {
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        storedObject: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class StoreSyncJobTask implements Task.TaskInstance, StoreSyncJobSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;
    public object: any;
    public inputs: StoreSyncJobInputSettings[];

    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();

    public storedObject: any;


    /** Settings */
    /** Properties Settings */
    public storageName: string;

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject("GlobalSyncInformationJobHandler")
    private _syncJobHandler: SyncInformationJobHandler;
    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {
        if (changes["activate"]) {
            if (this.clearActivate) {
                this.activate = undefined;
            }

            const scope: any = {};
            this._logger.warning("Storage Name " + this.storageName);
            for (const input of this.inputs) {
                const currentInputValue = this[input.name];
                if (currentInputValue != null) {
                    scope[input.name] = currentInputValue;
                } else if (input.defaultValue != null) {
                    scope[input.name] = input.defaultValue;
                }
            }
            for (const input of this.inputs) {
                        this[input.name] = undefined;
            }
                try {
                    const syncJobToStore: SyncInformationJobData = {
                        CreatedOn: "",
                        Data: scope,
                    };
                    const syncJob = await this._syncJobHandler.create( syncJobToStore, this.storageName );
                    this.success.emit(true);
 //                   this.storedObject.emit(syncJobToStore);
                } catch (error) {
                    this._logger.error(`Error trying to store sync job: ${error.message}`);
                    throw new Error (error);
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

}

// Add settings here
/** StoreSyncJob Settings object */
export interface StoreSyncJobSettings {
    inputs: StoreSyncJobInputSettings[];
    storageName: string;
}
export interface StoreSyncJobInputSettings {
    /** Input name */
    name: string;
    /** Input value type */
    valueType: Task.TaskComplexValueType;
    /** Default value if any */
    defaultValue?: any;
}
