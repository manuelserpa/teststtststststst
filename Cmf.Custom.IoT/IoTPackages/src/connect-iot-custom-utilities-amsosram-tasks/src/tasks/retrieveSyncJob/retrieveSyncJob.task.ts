import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/retrieveSyncJob.default";
import { SyncInformationJobHandler } from "../../persistence/implementation/syncInformationJobHandler";
import { SyncInformationJobData } from "../../persistence/model/syncInformationData";
import { SyncInformationJobProcess } from "../../persistence";

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
 * See {@see RetrieveSyncJobSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-retrieve",
    inputs: {
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        job: Task.TaskValueType.Object,
        emptyQueue: Task.TaskValueType.Boolean,
        jobCreatedOn: Task.TaskValueType.String,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class RetrieveSyncJobTask implements Task.TaskInstance, RetrieveSyncJobSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;

    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();
    public job:  Task.Output<Object> = new Task.Output<Object>();
    public emptyQueue:  Task.Output<boolean> = new Task.Output<boolean>();
    public jobCreatedOn:  Task.Output<String> = new Task.Output<String>();

    /** Settings */
    /** Properties Settings */
    public  storageName: string;

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject("GlobalSyncInformationJobHandler")
    private _syncJob: SyncInformationJobProcess;

    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {
        if (changes["activate"]) {
            // It is advised to reset the activate to allow being reactivated without the value being different
            this.activate = undefined;
            let job: SyncInformationJobData;
            try {
                job = await this._syncJob.getFirstSyncJob(this.storageName);
                if (job == null) {
                    this.emptyQueue.emit(true);
                } else {
                    let queueSize: number;
                    this.job.emit(job.Data);
                    this.jobCreatedOn.emit(job.CreatedOn);
                    queueSize = await this._syncJob.deleteJobFromPersistence(this.storageName);
                    this.emptyQueue.emit(queueSize === 0);
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
/** RetrieveSyncJob Settings object */
export interface RetrieveSyncJobSettings {
    storageName: string;
}
