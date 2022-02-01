import { SyncInformationJobData } from "../model/syncInformationData";
import { Dependencies, System, TYPES, DI } from "@criticalmanufacturing/connect-iot-controller-engine";
import * as inversify from "inversify";
import * as moment from "moment";
import { SyncInformationJobProcess } from "../model/syncInformationProcess";

@inversify.injectable()
export class SyncInformationJobHandler implements SyncInformationJobProcess {
    private _SyncInformationJobs: SyncInformationJobData[];
    private _SyncInformationJobNames: string[];
    private queue: string;

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject(TYPES.System.PersistedDataStore)
    private _dataStore: System.DataStore;


    /**
     * Method to create the Material persistence file
     * when a MO material is tracked in
     * State is saved as SETUP
     * @param Material Material data
     */
    public async create(Job: SyncInformationJobData, queue: string ): Promise<void> {
        if ( this._SyncInformationJobs === undefined || this.queue === undefined  || this.queue !== queue) {
            await this.InitializePersistedData(queue);
         }
        if (Job) {
            this._logger.info(`Storing data for Sync Job`);
            Job.CreatedOn = moment().utc().valueOf().toString();
            this._SyncInformationJobs.push(Job);
            await this._dataStore.store(`${Job.CreatedOn}_SyncJob_${queue}`, Job, System.DataStoreLocation.Temporary);
            this._SyncInformationJobNames.push(`${Job.CreatedOn}_SyncJob_${queue}`);
            await this._dataStore.store(`SyncJobQueue_${queue}`, this._SyncInformationJobNames, System.DataStoreLocation.Temporary);
            this._logger.info(`Stored data for Sync Job`);
        }

    }

    public async getFirstSyncJob(queue: string ) {
        if ( this._SyncInformationJobs === undefined || this.queue === undefined  || this.queue !== queue) {
            await this.InitializePersistedData(queue);
         }
        try {
            if (this._SyncInformationJobs.length > 0) {
            const Job = this._SyncInformationJobs[0]; // .sort((a,b) => parseInt (a.CreatedOn.toString()) - parseInt (b.CreatedOn.toString()))[0]
            return Job;
        } else {
            return null;
        }
        } catch (error) {
            this._logger.error(`Error: ${error.message}`);
        }
    }

    /**
     * Removes an Material object from the persistence
     * @param id Material Id
     */
    public async deleteJobFromPersistence (queue: string) {
        if ( this._SyncInformationJobs === undefined || this.queue === undefined  || this.queue !== queue) {
            await this.InitializePersistedData(queue);
         }
         if (this._SyncInformationJobs.length > 0) {
             const Job = this._SyncInformationJobs[0]; // .sort((a,b) => parseInt (a.CreatedOn.toString()) - parseInt (b.CreatedOn.toString()))[0];
            this._SyncInformationJobs.splice(0, 1);
            await this._dataStore.store(`${Job.CreatedOn}_SyncJob_${queue}`, undefined, System.DataStoreLocation.Temporary);
            this._SyncInformationJobNames.splice(this._SyncInformationJobNames.findIndex( o => o === `${Job.CreatedOn}_SyncJob_${queue}`), 1);
            await this._dataStore.store(`SyncJobQueue_${queue}`, this._SyncInformationJobNames, System.DataStoreLocation.Temporary);
        }
        return this._SyncInformationJobs.length;
    }



    /**
     * Loads all the existing Materials to memory
     */
    public async InitializePersistedData(queue: string) {
        this._SyncInformationJobs = [];
        this._SyncInformationJobNames = await this._dataStore.retrieve(`SyncJobQueue_${queue}`, []);
        if (this._SyncInformationJobNames !== null) {
        this._SyncInformationJobNames.forEach(async identifier => {
            this._SyncInformationJobs.push(await this._dataStore.retrieve(identifier, undefined));
        });
    }
    }

}
