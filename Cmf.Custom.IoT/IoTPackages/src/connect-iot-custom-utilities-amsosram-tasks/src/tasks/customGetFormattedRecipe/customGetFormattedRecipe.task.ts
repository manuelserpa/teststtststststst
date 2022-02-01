import { Task, Dependencies, System, DI, TYPES } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customGetFormattedRecipe.default";
import { SecsGem } from "../../common/secsGemItem"
import { SecsItem } from "../../common/secsItem";

/**
 * @whatItDoes
 *
 * Implements Get Recipe SECS/GEM message (S7F5)
 * Allows configuration of primary and secondary messages
 * Primary message with recipe name path location is configurable
 * Secondary message with recipe name and body path are configurable.
 * Recipe body type can be string or buffer.
 * Recipe Name From Equipment is a task output (could potentially be different from Recipe Name input)
 *
 * @howToUse
 *
 * ### Inputs
 * * `any` : **activate** - Activate the task
 * * `string` : **RecipeName** - Recipe Name input
 *
 * ### Outputs
 * * `buffer` : **RecipeBody** - Recipe Body
 * * `string` : **RecipeNameFromEquipment** - Recipe Name output (from Equipment)
 * * `bool`  : ** success ** - Triggered when the the task is executed with success
 * * `Error` : ** error ** - Triggered when the task failed for some reason
 *
 * ### Settings
 * See {@see CustomGetFormattedRecipeSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-secsgem-iot-lg-getrecipe",
    inputs: {
        activate: Task.INPUT_ACTIVATE,
        recipeName: System.PropertyValueType.String
    },
    outputs: {
        recipeNameFromEquipment: System.PropertyValueType.String,
        recipeParameterList: System.PropertyValueType.String,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.SecsGem
})
export class CustomGetFormattedRecipeTask implements Task.TaskInstance, CustomGetFormattedRecipeSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;
    /** Recipe Name that will get body from */
    public recipeName: string = "";

    /** **Outputs** */
    /** RecipeName (given from Equipment) */
    public recipeNameFromEquipment:  Task.Output<String> = new Task.Output<String>();
    /** RecipeBody given from Equipment */
    public recipeParameterList: Task.Output<Object> = new Task.Output<Object>();
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();


    /** Settings */
    /** Properties Settings */
    /** Stream Function Name used on message. Default stream function name for Get Recipe is 'S7F5'. */
    streamFunctionName: string = "S7F25"
    /** Primary Request Message */
    primaryRequestMessage: string = "{\"type\": \"A\", \"name\": \"PPID\", \"value\": \"\"}";
    /** Path location, on primary message, where RecipeName should be inserted.  */
    recipeNamePrimaryPath: string = "/"
    /** Path location, on secondary message, where RecipeName should be retrieved */
    recipeNameSecondaryPath: string = "/[1]"
    /** Path location, on secondary message, where RecipeBody should be retrieved */
    recipeBodyPath: string = "/[4]"
    /** Recipe Body on message content can be interpreted as a Buffer or as a String */
    recipeBodyType: RecipeBodyType = RecipeBodyType.StringRecipeBody


    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject(TYPES.System.Driver)
    private _driverProxy: System.DriverProxy;

    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {
        if (changes["activate"]) {
            // It is advised to reset the activate to allow being reactivated without the value being different
            this.activate = undefined;

            if ( this.recipeName === "") {
                this._logger.warning(`Trying to get recipe body with an empty recipe name.`);
            }

            try {
                let sendMessage: any
                if (this.primaryRequestMessage === "{}") {
                     sendMessage = { type: this.streamFunctionName }
                } else {
                     sendMessage = { type: this.streamFunctionName, item: JSON.parse(this.primaryRequestMessage) }

                     if (this.recipeNamePrimaryPath !== "") {
                        // On empty recipeName path, no substitution is performed.
                        if (this.recipeName !== "") {
                            const itemToInsertRecipeName: SecsItem = SecsGem.getItemByPath(sendMessage.item, this.recipeNamePrimaryPath);
                            itemToInsertRecipeName.value = this.recipeName
                        }
                    }
                }

                const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);

                if ( reply && reply.item) {
                    const receivedRecipeName: string = SecsGem.getValue(SecsGem.getItemByPath(reply.item, this.recipeNameSecondaryPath));
                    let receivedRecipeBody: string;
                    receivedRecipeBody = JSON.stringify(SecsGem.getValue(SecsGem.getItemByPath(reply.item, this.recipeBodyPath), false));

                    if (this.recipeBodyType === RecipeBodyType.BufferRecipeBody) {
                        receivedRecipeBody = receivedRecipeBody = Buffer.from(receivedRecipeBody).toString('base64');
                    }

                    this.recipeParameterList.emit(receivedRecipeBody);
                    this.recipeNameFromEquipment.emit(receivedRecipeName)
                    this.success.emit(true);
                } else {
                    this.error.emit(new Error(`Unable to Get Formatted Recipe`));
                    this._logger.error(`Unable to Get Formatted Recipe`);
                }
            } catch (error) {
                this.error.emit(error);
                this._logger.error(`Error on Get Formatted Recipe Task: ${error.message}`);
            }

        }
    }

    /**
     * Right after settings are loaded, create the needed dynamic outputs.
     */
    async onBeforeInit(): Promise<void> {
    }

    /** Initialize this task, register any event handler, etc */
    async onInit(): Promise<void> {
    }

    /** Cleanup internal data, unregister any event handler, etc */
    async onDestroy(): Promise<void> {
    }

    // On every tick (use instead of onChanges if necessary)
    // async onCheck(): Promise<void> {
    // }
}

/** CustomGetFormattedRecipe Settings object */
export interface CustomGetFormattedRecipeSettings {
    /** SECS/GEM Stream Function Name */
    streamFunctionName: string;
    /** Primary Request Message */
    primaryRequestMessage: string;
    /** Path (used on primary path) to insert the Recipe Name (from task input) */
    recipeNamePrimaryPath: string;
    /** Path (used on secondary path) to retrieve Recipe Name (from Equipment) */
    recipeNameSecondaryPath: string;
    /** Path (used on secondary path) to retrieve Recipe Body */
    recipeBodyPath: string;
    /** Message content will be interpreted as a String or a Buffer */
    recipeBodyType: RecipeBodyType;
}

/**
 * Type of Recipe Body on message content
 */
export enum RecipeBodyType {
    /**
     * Message content will be interpreted as a String
     */
    StringRecipeBody = "String",
    /**
     * Message content will be interpreted as a Buffer
     */
    BufferRecipeBody = "Buffer"
}
