import { MaterialData, MaterialStateEnum } from "../model/materialData";
import { SubMaterialData, SubMaterialStateEnum } from "../model/subMaterialData";
import { RecipeData } from "../model/recipeData";
import { Dependencies, System, TYPES, DI } from "@criticalmanufacturing/connect-iot-controller-engine";
import * as inversify from "inversify";
import * as moment from "moment";
import { ProcessMaterial } from "../model/materialProcess";

@inversify.injectable()
export class ProcessMaterialHandler implements ProcessMaterial {
    private _Material: MaterialData;
    private _Materials: MaterialData[];
    private _MaterialNames: string[];
    private _MaterialSlotToSet: string;

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
    public async trackIn(Material: MaterialData): Promise<void> {
        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        if (Material) {
            this._logger.info(`Storing data for Material ${Material.MaterialName}`);
            Material.LastUpdate = moment().utc().valueOf().toString();
            this._Materials.push(Material);
            let ContainerName = Material.ContainerName;
            if (ContainerName == null) {
                ContainerName = "default";
            }
            await this._dataStore.store(`${ContainerName}_MaterialId_${Material.MaterialId}`, Material, System.DataStoreLocation.Persistent);
            if (!this._MaterialNames.find(o => o === `${ContainerName}_MaterialId_${Material.MaterialId}`)) {
                this._MaterialNames.push(`${ContainerName}_MaterialId_${Material.MaterialId}`);
                await this._dataStore.store(`MaterialOnPersistence`, this._MaterialNames, System.DataStoreLocation.Persistent);
            }
            this._logger.info(`Stored data for Material '
            ${Material.MaterialId}' with State '${Material.MaterialState.toString()}' and content '${JSON.stringify(Material)}'`);
        }
    }
    /**
     * Retrieves the Material state for a given MaterialID
     * @param MaterialID Material Id
     */
    public async getMaterialState(MaterialName: string) {
        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        try {
            this._logger.info(`Retrieving stored data for Material "${MaterialName}"`);
            const Material = await this.getMaterialObjectFromName(MaterialName);
            const state = Material.MaterialState;
            return state;
        } catch (error) {
            this._logger.error(`Error retrieving stored data for Material "${MaterialName}": ${error.message}`);
        }
    }

    public async getMaterialObjectFromName(name: string) {
        if (name == null || name === "") {
            return;
        }

        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        try {
            const Material = this._Materials.find(o => o.MaterialName.toLowerCase() === name.toLowerCase());
            if (Material == null) {
                this._logger.warning(`The Material with the id '${name}' does not exist`);
                return null;
            }
            return Material;
        } catch (error) {
            this._logger.error(`Error: ${error.message}`);
        }
    }

    public async getMaterialByProcessJobId(processJobId: string) {
        if (processJobId == null || processJobId === "") {
            return;
        }

        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        try {
            const Material = this._Materials.find(o => o.ProcessJobId.toLowerCase() === processJobId.toLowerCase());
            if (Material == null) {
                this._logger.warning(`The Material with the process job id '${processJobId}' does not exist`);
                return null;
            }
            return Material;
        } catch (error) {
            this._logger.error(`Error: ${error.message}`);
        }
    }

    public async getMaterialByControlJobId(controlJobId: string) {
        if (controlJobId == null || controlJobId === "") {
            return;
        }

        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        try {
            const Material = this._Materials.find(o => o.ControlJobId.toLowerCase() === controlJobId.toLowerCase());
            if (Material == null) {
                this._logger.warning(`The Material with the process job id '${controlJobId}' does not exist`);
                return null;
            }
            return Material;
        } catch (error) {
            this._logger.error(`Error: ${error.message}`);
        }
    }
    /**
     * Updated the Material state for a given MaterialID
     * @param MaterialID Material Id
     * @param state State to be updated
     */
    public async setMaterialState(materialName: string, state: MaterialStateEnum) {
        const Material = await this.getMaterialObjectFromName(materialName);
        try {
            Material.MaterialState = state;
            this._logger.info(`Updating state for Material ${Material.MaterialName} to State "${state}"`);
            let ContainerName = Material.ContainerName;
            if (ContainerName == null) {
                ContainerName = "default";
            }
            await this._dataStore.store(`${ContainerName}_MaterialId_${Material.MaterialId}`, Material, System.DataStoreLocation.Persistent);
            this._logger.debug(`Updated state for Material ${Material.MaterialName} to State "${state}"`);
        } catch (error) {
            this._logger.error(`Error updating data for ${materialName} : ${error.message}`);
        }
    }

    private async storeMaterialNames(): Promise<void> {
        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        await this._dataStore.store("ProcessDataMaterialNames", this._MaterialNames, System.DataStoreLocation.Persistent);
    }
    /**
     * Retrieves the Material object for a given MaterialId
     * @param id Material Id
     */
    public async getMaterialObjectFromID(id: string) {
        if (id == null || id === "") {
            return;
        }

        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        try {
            const Material = this._Materials.find(o => o.MaterialId.toString() === id);
            if (Material == null) {
                this._logger.warning(`The Material with the id '${id}' does not exist`);
                return null;
            }
            return Material;
        } catch (error) {
            this._logger.error(`Error: ${error.message}`);
        }

    }

    /**
     * Retrieves the Material object for a given Material slot
     * @param slot Material slot
     */
    public async getMaterialByState(state: MaterialStateEnum) {
        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        try {
            this._logger.warning(`Trying to get the Material with the State '${state.toString()}'`);
            const Material = this._Materials.find(o => o.MaterialState === state.toString());
            if (Material === undefined) {
                return null;
            }
            return Material;
        } catch (error) {
            this._logger.error(`Error: ${error.message}`);
        }
    }

    /**
    * Retrieves the Material object for a given Material slot
    * @param slot Material slot
    */
    public async getMaterialByCarrier(carrier: string) {
        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        try {
            const Material = this._Materials.find(o => o.ContainerName === carrier);
            if (Material === undefined) {
                return null;
            }
            return Material;
        } catch (error) {
            this._logger.error(`Error: ${error.message}`);
        }
    }

    public async getMaterialByLoadPortIdAndMaterialState(loadPort: string, state: MaterialStateEnum) {
        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        try {
            this._logger.warning(`Trying to get the Material in Load port number '${loadPort}' with the State '${state.toString()}'`);
            const Material = this._Materials.find(o => o.LoadPortPosition === loadPort && o.MaterialState === state.toString());
            if (Material === undefined) {
                return null;
            }
            return Material;
        } catch (error) {
            this._logger.error(`Error: ${error.message}`);
        }
    }

    /**
     * Retrieves the Material object for a given Material slot
     * @param slot Material slot
     */
    public async getAllMaterialsOnState(state: MaterialStateEnum) {
        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        try {
            this._logger.warning(`Trying to get the Material with the State '${state.toString()}'`);
            const Materials = this._Materials.filter(o => o.MaterialState === state.toString());
            if (Materials === undefined) {
                return null;
            }
            const MaterialsToReturn: MaterialData[] = [];
            Materials.forEach(async material => { MaterialsToReturn.push(material); })
            if (MaterialsToReturn) {
                return MaterialsToReturn;
            }
        } catch (error) {
            this._logger.error(`Error: ${error.message}`);
        }

    }

    /**
    * Retrieves the Material object for a given Material slot
    * @param slot Material slot
    */
    public async getAllMaterialsInCarrier(carrier: string) {
        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        try {
            const Materials = this._Materials.filter(o => o.ContainerName === carrier);
            if (Materials === undefined) {
                return null;
            }
            const MaterialsToReturn: MaterialData[] = [];
            Materials.forEach(async material => { MaterialsToReturn.push(material); });
            if (MaterialsToReturn) {
                return MaterialsToReturn;
            }
        } catch (error) {
            this._logger.error(`Error: ${error.message}`);
        }

    }

    public async getAllMaterialByLoadPortIdAndMaterialState(loadPort: string, state: MaterialStateEnum) {
        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        try {
            this._logger.warning(`Trying to get the Material in Load port number '${loadPort}' with the State '${state.toString()}'`);
            const Materials = this._Materials.filter(o => o.LoadPortPosition === loadPort && o.MaterialState === state.toString());
            if (Materials === undefined) {
                return null;
            }
            const MaterialsToReturn: MaterialData[] = [];
            Materials.forEach(async material => { MaterialsToReturn.push(material); });
            if (MaterialsToReturn) {
                return MaterialsToReturn;
            }
        } catch (error) {
            this._logger.error(`Error: ${error.message}`);
        }

    }


    /**
     * Used to save the Material object
     * @param Material Material Object
     */
    public async updateMaterial(Material: MaterialData) {
        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        Material.LastUpdate = moment().utc().valueOf().toString();
        let ContainerName = Material.ContainerName;
        if (ContainerName == null) {
            ContainerName = "default";
        }
        await this._dataStore.store(`${ContainerName}_MaterialId_${Material.MaterialId}`, Material, System.DataStoreLocation.Persistent);
    }

    /**
     * Removes an Material object from the persistence
     * @param id Material Id
     */
    public async deleteMaterialFromPersistence(id: string) {
        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        const Material = this._Materials.find(o => o.MaterialId.toString() === id);
        if (Material != null) {
            this._Materials.splice(this._Materials.indexOf(Material), 1);
            let ContainerName = Material.ContainerName;
            if (ContainerName == null) {
                ContainerName = "default";
            }
            await this._dataStore.store(`${ContainerName}_MaterialId_${Material.MaterialId}`, undefined, System.DataStoreLocation.Persistent);
            this._MaterialNames.splice(this._MaterialNames.findIndex(o => o === `${ContainerName}_MaterialId_${Material.MaterialId}`), 1);
            await this._dataStore.store(`MaterialOnPersistence`, this._MaterialNames, System.DataStoreLocation.Persistent);
        }
    }


    /**
     * Validates and deletes the Materials on persistency that are no longer used
     * @param MaterialIds Array of MaterialIds existing on the equipment side.
     */
    public async validatePersistenceMaterials(MaterialIds: string[]): Promise<string[]> {
        if (this._Materials === undefined) {
            await this.InitializePersistedData();
        }
        return;
    }

    /**
     * Loads all the existing Materials to memory
     */
    public async InitializePersistedData() {
        this._Materials = [];
        this._MaterialNames = await this._dataStore.retrieve('MaterialOnPersistence', []);
        this._MaterialNames.forEach(async identifier => {
            this._Materials.push(await this._dataStore.retrieve(identifier, undefined));
        });

    }


}
