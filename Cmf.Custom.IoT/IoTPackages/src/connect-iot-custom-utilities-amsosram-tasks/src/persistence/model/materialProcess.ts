import { MaterialData, MaterialStateEnum } from "./materialData";

export interface ProcessMaterial {

    /**
     * Method to create the Material persistence file
     * when a MO material is tracked in
     * @param material Material data
     */
    trackIn(material: MaterialData): Promise<void>;

    /**
     * Retrieves the Material state for a given MaterialID
     * @param MaterialName Material Name
     */
    getMaterialState(materialName: string);

    /**
     * Updates the state for a given MaterialId
     * @param MaterialID Material Id
     * @param state state to set
     */
    setMaterialState(materialName: string, state: MaterialStateEnum);

    /**
     * Retrieves the Material object for a given MaterialId
     * @param id Material Id
     */
    getMaterialObjectFromID(id: string);

    /**
     * Retrieves the Material object for a given MaterialId
     * @param id Material Id
     */
    getMaterialObjectFromName(name: string);

    /**
     * Retrieves the Material object for a given Material slot
     * @param state Material slot
     */
    getMaterialByState(state: MaterialStateEnum);


     /**
     * Retrieves the Material object for a given Material slot
     * @param carrier Container
     */
    getMaterialByCarrier(carrier: string);

    /**
     * Retrieves the Material object for a given Process Job Id
     * @param processJobId Process Job Id
     */
    getMaterialByProcessJobId(processJobId: string);

      /**
     * Retrieves the Material object for a given Control Job Id
     * @param controlJobId Control Job Id
     */
    getMaterialByControlJobId(controlJobId: string);

    /**
     * Retrieves the Material object for a given Load Port and State
     * @param carrier Material slot
     */
    getMaterialByLoadPortIdAndMaterialState(loadPort: string, state: MaterialStateEnum);

    /**
     * Used to save the Material object
     * @param Material Material Object
     */
    updateMaterial(material: MaterialData);

    /**
     * Removes an Material object from the persistence
     * @param id Material Id
     */
    deleteMaterialFromPersistence (id: string);

    /**
     * Validates and deletes the Materials on persistency that are no longer used
     * @param MaterialIds Array of MaterialIds existing on the equipment side.
     */
    validatePersistenceMaterials(MaterialIds: string []): Promise<string []>;

    /**
     * Loads all the existing Materials to memory
     */
    InitializePersistedData();

}
