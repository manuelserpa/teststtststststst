import { RecipeParameterData } from "./recipeParameterData";

export interface RecipeData {
    /** Recipe parameter Name */
    RecipeName: string,
    /** Recipe parameter Id */
    RecipeId: string,
    /** Recipe name on equipment */
    NameOnEquipment: string,
    /** Recipe Checksum */
    Checksum: string,
    /** Recipe order */
    Order: number,
    /** Recipe parameter units */
    SubRecipes: RecipeData[]
    /** Recipe parameter units */
    RecipeParameters: RecipeParameterData[]
}

