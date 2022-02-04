import { RecipeData } from "./recipeData";


export interface RecipeQueue {

    /**
     * Method to create the Material persistence file
     * @param recipeQueue Recipe Queue
     */
    storeRecipeQueue(recipeQueue: RecipeData[], storageName: string);

    /**
     * Retrieves the RecipeQueue given a storage name
     * @param storageName Storage Name
     */
    retrieveRecipeQueue(storageName: string);

}
