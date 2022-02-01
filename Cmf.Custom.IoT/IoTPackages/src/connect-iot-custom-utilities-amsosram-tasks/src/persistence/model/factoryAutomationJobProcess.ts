import { FactoryAutomationJobData } from "./factoryAutomationJobData";

export interface FactoryAutomationJobProcess {

    /**
     * Method to create the Material persistence file
     * when a MO material is tracked in
     * @param job Material data
     */
    create(job: FactoryAutomationJobData): Promise<void>;

    /**
     * Retrieves the Material state for a given MaterialID
     * @param carrierId Material Name
     */
    getJobByCarrierId(carrierId: string);

    /**
     * Removes an Material object from the persistence
     * @param id Material Id
     */
    deleteJobFromPersistence (id: string);

    /**
     * Loads all the existing Materials to memory
     */
    InitializePersistedData();

}
