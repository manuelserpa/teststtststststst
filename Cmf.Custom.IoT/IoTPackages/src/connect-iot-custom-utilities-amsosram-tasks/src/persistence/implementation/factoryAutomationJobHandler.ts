import { FactoryAutomationJobData } from "../model/factoryAutomationJobData";
import { Dependencies, System, TYPES, DI } from "@criticalmanufacturing/connect-iot-controller-engine";
import * as inversify from "inversify";
import * as moment from "moment";
import { FactoryAutomationJobProcess } from "../model/factoryAutomationJobProcess";

@inversify.injectable()
export class FactoryAutomationJobHandler implements FactoryAutomationJobProcess {
    private _FactoryAutomationJob: FactoryAutomationJobData;
    private _FactoryAutomationJobs: FactoryAutomationJobData[];
    private _FactoryAutomationJobNames: string[];

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
    public async create(Job: FactoryAutomationJobData): Promise<void> {
        if ( this._FactoryAutomationJobs === undefined ) {
            await this.InitializePersistedData();
         }
        if (Job) {
            this._logger.info(`Storing data for Job for Transport Job for Carrier ${Job.CarrierId}`);
            if (this._FactoryAutomationJobs.find(o => o.CarrierId === Job.CarrierId)) {
                this._logger.warning(`Deleting old job for carrier id ${Job.CarrierId}`);
                await this.deleteJobFromPersistenceByCarrierId(Job.CarrierId);
            }
            Job.CreatedOn = moment().utc().valueOf().toString();
            Job.ModifiedOn = moment().utc().valueOf().toString();
            this._FactoryAutomationJobs.push(Job);
            await this._dataStore.store(`${Job.CarrierId}_CarrierId_${Job.Trigger}`, Job, System.DataStoreLocation.Persistent);
            this._FactoryAutomationJobNames.push(`${Job.CarrierId}_CarrierId_${Job.Trigger}`);
            await this._dataStore.store(`FactoryAutomationJobsOnPersistence`, this._FactoryAutomationJobNames, System.DataStoreLocation.Persistent);

            this._logger.info(`Stored data for Carrier '
            ${Job.CarrierId}' with trigger '${Job.Trigger}' and content '${JSON.stringify(Job)}'`);
        }
    }

    public async getJobByCarrierId(carrierId: string) {
        if (carrierId == null || carrierId === "") {
            return;
        }

        if ( this._FactoryAutomationJobs === undefined ) {
            await this.InitializePersistedData();
         }
        try {
            const Job = this._FactoryAutomationJobs.find(o => o.CarrierId === carrierId);
            if (Job == null) {
                this._logger.warning(`The Job with the carrier id '${carrierId}' does not exist`);
                return null;
             } else {
                const JobPersistence = await this._dataStore.retrieve(`${Job.CarrierId}_CarrierId_${Job.Trigger}`, undefined);
                if (JobPersistence && Job.CarrierId === JobPersistence.CarrierId) {
                    return Job;
                }
            }
        } catch (error) {
            this._logger.error(`Error: ${error.message}`);
        }
    }

    /**
     * Used to save the Material object
     * @param Material Material Object
     */
    public async updateMaterial(Job: FactoryAutomationJobData) {
        if ( this._FactoryAutomationJobs === undefined ) {
            await this.InitializePersistedData();
         }
        Job.ModifiedOn = moment().utc().valueOf().toString();
        await this._dataStore.store(`${Job.CarrierId}_CarrierId_${Job.Trigger}`, Job, System.DataStoreLocation.Persistent);
    }

    /**
     * Removes an Material object from the persistence
     * @param id Material Id
     */
    public async deleteJobFromPersistence (id: string) {
        if ( this._FactoryAutomationJobs === undefined ) {
            await this.InitializePersistedData();
         }
        const Job = this._FactoryAutomationJobs.find(o => o.JobId.toString() === id);
        if (Job != null) {
            this._FactoryAutomationJobs.splice(this._FactoryAutomationJobs.indexOf(Job), 1);
            await this._dataStore.store(`${Job.CarrierId}_CarrierId_${Job.Trigger}`, undefined, System.DataStoreLocation.Persistent);
            this._FactoryAutomationJobNames.splice(this._FactoryAutomationJobNames.findIndex( o => o === `${Job.CarrierId}_CarrierId_${Job.Trigger}`), 1);
            await this._dataStore.store(`FactoryAutomationJobsOnPersistence`, this._FactoryAutomationJobNames, System.DataStoreLocation.Persistent);
        }
    }

     /**
     * Removes an Material object from the persistence
     * @param id Material Id
     */
    public async deleteJobFromPersistenceByCarrierId (carrierId: string) {
        if ( this._FactoryAutomationJobs === undefined ) {
            await this.InitializePersistedData();
         }
        const Job = this._FactoryAutomationJobs.find(o => o.CarrierId.toString() === carrierId);
        if (Job != null) {
            this._FactoryAutomationJobs.splice(this._FactoryAutomationJobs.indexOf(Job), 1);
            await this._dataStore.store(`${Job.CarrierId}_CarrierId_${Job.Trigger}`, undefined, System.DataStoreLocation.Persistent);
            this._FactoryAutomationJobNames.splice(this._FactoryAutomationJobNames.findIndex( o => o === `${Job.CarrierId}_CarrierId_${Job.Trigger}`), 1);
            await this._dataStore.store(`FactoryAutomationJobsOnPersistence`, this._FactoryAutomationJobNames, System.DataStoreLocation.Persistent);
        }
    }



    /**
     * Loads all the existing Materials to memory
     */
    public async InitializePersistedData() {
        this._FactoryAutomationJobs = [];
        this._FactoryAutomationJobNames = await this._dataStore.retrieve('FactoryAutomationJobsOnPersistence', []);
        this._FactoryAutomationJobNames.forEach(async identifier => {
            this._FactoryAutomationJobs.push(await this._dataStore.retrieve(identifier, undefined));
        });

    }


}
