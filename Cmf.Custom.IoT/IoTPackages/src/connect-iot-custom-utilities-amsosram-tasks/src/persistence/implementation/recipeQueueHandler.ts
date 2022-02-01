import { RecipeData } from "../model/recipeData";
import { Dependencies, System, TYPES, DI } from "@criticalmanufacturing/connect-iot-controller-engine";
import * as inversify from "inversify";
import { RecipeQueue } from "../model/recipeQueue";

@inversify.injectable()
export class RecipeQueueHandler implements RecipeQueue {

    private _recipeQueue: RecipeData[];
    private _levelQueue: number[];

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject(TYPES.System.PersistedDataStore)
    private _dataStore: System.DataStore;


    public async storeRecipeQueue(recipeQueue: RecipeData[], storageName: string) {
        await this._dataStore.store(`_TempRecipeDownload_${storageName}_RecipeQueue`, recipeQueue, System.DataStoreLocation.Temporary);
        this._recipeQueue = recipeQueue;
    }

    public async retrieveRecipeQueue(storageName: string) {
        try {
            const recipeQueue = await this._dataStore.retrieve(`_TempRecipeDownload_${storageName}_RecipeQueue`, undefined);
            if (recipeQueue) {
                this._recipeQueue = recipeQueue;
                return this._recipeQueue;
            }
        } catch (error) {
            this._logger.error(`Error: ${error.message}`);
        }
    }
}
