import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/recipeStructureIterator.default";
import { MaterialData } from "../../persistence/model/materialData";
import { RecipeData } from "../../persistence/model/recipeData";
import { RecipeQueueHandler } from "../../persistence/implementation/recipeQueueHandler";
/**
 * @whatItDoes
 *
 * This task does something ... describe here
 *
 * @howToUse
 *
 * yada yada yada
 *
 * ### Inputs
 * * `any` : **activate** - Activate the task
 *
 * ### Outputs

 * * `bool`  : ** success ** - Triggered when the the task is executed with success
 * * `Error` : ** error ** - Triggered when the task failed for some reason
 *
 * ### Settings
 * See {@see RecipeStructureIteratorSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-retrieve",
    inputs: {
        materialData: Task.TaskValueType.Object,
        recipeData: Task.TaskValueType.Object,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        recipeName: Task.TaskValueType.String,
        ppid: Task.TaskValueType.String,
        checksum: Task.TaskValueType.String,
        recipeLevel: Task.TaskValueType.Integer,
        recipeBody: Task.TaskValueType.String,
        recipeParameters: Task.TaskValueType.Object,
        recipeTreeEnd: Task.TaskValueType.Boolean,
        Success: Task.OUTPUT_SUCCESS,
        Error: Task.OUTPUT_ERROR
    }
})
export class RecipeStructureIteratorTask implements Task.TaskInstance, RecipeStructureIteratorSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;

    public storageName: string;
    public paddingSize: number;
    public paddingCharacter: string;
    public isBottomUp: boolean;
    public materialData: MaterialData[];
    public recipeData: any;
    public recipeDataStored: RecipeData;

    private materialDataStored: MaterialData[];
    private recipeQueue: RecipeData[] = [];
    private paddingSizeLoaded: number = 0;
    private paddingCharacterLoaded: string = '';


    /** **Outputs** */
    public recipeName: Task.Output<string> = new Task.Output<string>();
    public ppid: Task.Output<string> = new Task.Output<string>();
    public checksum: Task.Output<string> = new Task.Output<string>();
    public recipeLevel: Task.Output<number> = new Task.Output<number>();
    public recipeTreeEnd: Task.Output<boolean> = new Task.Output<boolean>();
    public recipeBody: Task.Output<string> = new Task.Output<string>();
    public recipeParameters: Task.Output<Object> = new Task.Output<Object>();
    /** To output a success notification */
    public Success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public Error: Task.Output<Error> = new Task.Output<Error>();
    // public material:  Task.Output<MaterialData> = new Task.Output<MaterialData>();
    // public batchProcessComplete:  Task.Output<boolean> = new Task.Output<boolean>();

    /** Settings */
    /** Properties Settings */


    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject("GlobalRecipeQueueHandler")
    private _recipeQueueHandler: RecipeQueueHandler;

    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {


        if (changes["activate"]) {
            // It is advised to reset the activate to allow being reactivated without the value being different

            this.activate = undefined;

            try {
                if (this.materialData != null) {

                    this.materialDataStored = this.materialData;
                    this.materialData = undefined;
                    this.recipeData = undefined;
                    this.recipeQueue = [];
                    this.RunThroughHierarchy(this.materialDataStored[0].Recipe, 0);

                    if (!this.isBottomUp) {
                        this.recipeQueue = this.recipeQueue.reverse();
                    }

                    this._recipeQueueHandler.storeRecipeQueue(this.recipeQueue, this.storageName);
                } else if (this.recipeData != null) {
                    // corrigir estrutura da DEE
                    this.recipeDataStored = this.BuildRecipeData(this.recipeData);

                    // passar estrutura para o Run Through Hierarchy
                    this.materialData = undefined;
                    this.recipeData = undefined;
                    this.recipeQueue = [];
                    this.RunThroughHierarchy(this.recipeDataStored, 0);

                    if (!this.isBottomUp) {
                        this.recipeQueue = this.recipeQueue.reverse();
                    }

                    this._recipeQueueHandler.storeRecipeQueue(this.recipeQueue, this.storageName);

                    // dentro do RTH, analisar se tem recipe body, se sim ["Body"] = body
                }

                if (this.materialData == null && this.recipeData == null) {

                    if (this.paddingCharacter != null) {
                        this.paddingCharacterLoaded = this.paddingCharacter;
                    }

                    if (this.paddingSize != null) {
                        this.paddingSizeLoaded = this.paddingSize;
                    }

                    this.recipeQueue = await this._recipeQueueHandler.retrieveRecipeQueue(this.storageName);

                    if (this.recipeQueue.length > 0) {
                        const recipeDataToEmit = this.recipeQueue.shift();
                        const levelToEmit = recipeDataToEmit["Level"];
                        const bodyToEmit = recipeDataToEmit["Body"];

                        this._recipeQueueHandler.storeRecipeQueue(this.recipeQueue, this.storageName);

                        this.recipeName.emit(recipeDataToEmit.RecipeName);
                        this.ppid.emit(recipeDataToEmit.NameOnEquipment.padEnd(this.paddingSizeLoaded, this.paddingCharacterLoaded));
                        this.checksum.emit(recipeDataToEmit.Checksum);
                        if (recipeDataToEmit.RecipeParameters) {
                            this.recipeParameters.emit(recipeDataToEmit.RecipeParameters);
                        }
                        this.recipeLevel.emit(levelToEmit);
                        if (bodyToEmit) {
                            this.recipeBody.emit(bodyToEmit);
                        }
                    } else {
                        this.recipeTreeEnd.emit(true);
                    }
                }
                this.Success.emit(true);

            } catch (error) {
                this._logger.error(`Error occurred: ${error.message}`);
                this.Error.emit(error);
            }
        }
    }

    RunThroughHierarchy(recipeData: RecipeData, level: number): void {
        const subRecipes = recipeData.SubRecipes;
        if (subRecipes != null) {
            for (let i = 0; i < subRecipes.length; i++) {
                this.RunThroughHierarchy(subRecipes[i], level + 1);
            }
        }

        recipeData.SubRecipes = null;
        recipeData["Level"] = level;
        this.recipeQueue.push(recipeData);
    }

    BuildRecipeData(recipe: any): RecipeData {

        const subRecipes = [];
        for (const subRecipe of recipe['SubRecipes']) {
            subRecipes.push(this.BuildRecipeData(subRecipe));
        }
        const recipeDataObject: RecipeData = {
            RecipeName: recipe['RecipeName'],
            NameOnEquipment: recipe['RecipeNameOnEquipment'],
            Checksum: recipe['Checksum'],
            SubRecipes: subRecipes,
            RecipeParameters: recipe['RecipeParameters'],
            RecipeId: null,
            Order: null
        };

        if (recipe["Body"] != null) {
            recipeDataObject['Body'] = recipe['RecipeBody'];
        }

        return recipeDataObject;
    }

    /** Right after settings are loaded, create the needed dynamic outputs. */
    async onBeforeInit(): Promise<void> {
        if (this.outputs) {
            for (const output of this.outputs) {
                this[output.name] = new Task.Output<any>();
            }
        }
    }

    /** Initialize this task, register any event handler, etc */
    async onInit(): Promise<void> {
    }

    /** Cleanup internal data, unregister any event handler, etc */
    async onDestroy(): Promise<void> {
    }

}

// Add settings here
/** RecipeStructureIterator Settings object */
export interface RecipeStructureIteratorSettings {
    storageName: string;
    paddingSize: number;
    paddingCharacter: string;
    isBottomUp: boolean;
}
