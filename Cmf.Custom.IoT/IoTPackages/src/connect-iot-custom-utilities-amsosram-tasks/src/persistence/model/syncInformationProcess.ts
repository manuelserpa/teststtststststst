import { SyncInformationJobData } from "./syncInformationData";

export interface SyncInformationJobProcess {

    /**
     * Creates the Job on last position of the queue
     */
    create(job: SyncInformationJobData, queue: string): Promise<void>;

    /**
     * Retrieves the First Job
     * @param carrierId Material Name
     */
    getFirstSyncJob(queue: string);

    /**
     * Deletes the first job
     */
    deleteJobFromPersistence (queue: string);

    /**
     * Loads all the existing Materials to memory
     */
    InitializePersistedData(queue: string);

}
