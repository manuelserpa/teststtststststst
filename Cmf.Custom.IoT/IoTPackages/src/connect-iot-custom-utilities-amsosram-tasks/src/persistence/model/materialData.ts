import { RecipeData } from "./recipeData";
import { SubMaterialData } from "./subMaterialData";
import { SorterJobInformationData } from "./sorterJobInformation";


export interface MaterialData {
    /** Material MO ID */
    MaterialId: string,
    /** Material MO Name */
    MaterialName: string,
    /** State */
    MaterialState: MaterialStateEnum,
    /** Carrier Name */
    ContainerName: string,
    /** Sub Material List */
    SubMaterials: SubMaterialData[],
   /** Recipe parameters list */
    Recipe: RecipeData,
    /** Last Time the Material was updated */
    LastUpdate: string,
    /** Load Port Name */
    LoadPortName: string,
    /** Load Port Position */
    LoadPortPosition: string
    /** Sorter Job Information */
    SorterJobInformation: SorterJobInformationData
    /** Allow Download Recipe At TrackIn  */
    AllowDownloadRecipeAtTrackIn: Boolean,
    /** Container Only Process */
    ContainerOnlyProcess: Boolean,
    /** Control Job Id */
    ControlJobId: String,
    /** Process Job Id */
    ProcessJobId: String
}

export enum MaterialStateEnum {
    Setup = "Setup",
    InProcess = "InProcess",
    Complete = "Complete",
    Aborted = "Aborted"
}
