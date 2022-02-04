import { Task, Dependencies, System, DI, TYPES } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customDownloadRecipeToEquipment.default";
import { SecsGem } from "./../../common/secsGemItem"
import { SecsItem } from "../../common/secsItem";


/**
 * @whatItDoes
 *
 * Implements Set Recipe SECS/GEM message (S7F3)
 * Allows configuration of primary and secondary messages.
 * Can use S7F1 (Set Recipe Inquire) previous to S7F3.
 * Primary message with recipe name and body path location is configurable
 * Recipe body type can be string or buffer.
 *
 * @howToUse
 *
 * ### Inputs
 * *
 * * `buffer` : **RecipeBody** - Recipe Body
 * * `string` : **RecipeName** - Recipe Name
 * * `any` : **activate** - Activate the task
 *
 * ### Outputs

 * * `bool`  : ** success ** - Triggered when the the task is executed with success
 * * `Error` : ** error ** - Triggered when the task failed for some reason
 *
 * ### Settings
 * See {@see CustomDownloadRecipeToEquipmentSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-secsgem-iot-lg-setrecipe",
    inputs: {
        activate: Task.INPUT_ACTIVATE,
        recipeName: System.PropertyValueType.String,
        recipeBody: undefined
    },
    outputs: {
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.SecsGem
})
export class CustomDownloadRecipeToEquipmentTask implements Task.TaskInstance, CustomDownloadRecipeToEquipmentSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;
    /** Recipe name that will have the body set */
    public recipeName: string = "";
    /** Recipe Body that will be set */
    public recipeBody: any;


    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();


    /** Settings */
    /** Properties Settings */
    /** Stream Function Name used on message. Default stream function name for Set Recipe is 'S7F3'. */
    streamFunctionName: string = "S7F3";
    /** Should use (or not) S7F1 message prior to upload */
    useS7F1Message: boolean = true;
    /** Success Codes to evaluate on S7F1 message. If empty, any value is a success */
    successCodesS7F1: string = "0x00"
    /** Extra message content to be included on primary inquiry message.*/
    // tslint:disable-next-line:max-line-length
    primaryInquiryRequestMessage: string =  "{\"type\": \"L\", \"value\": [ { \"type\": \"A\", \"name\": \"PPID\", \"value\": \"\"}, { \"type\": \"U4\", \"name\": \"LENGTH\", \"value\": \"\"}]}";
    /** Path location, on primary message, where RecipeName should be inserted.  */
    recipeNameInquiryPrimaryPath: string = "/[1]"
    /** Path location, on primary message, where RecipeBody should be inserted.  */
    recipeBodyLengthInquiryPrimaryPath: string = "/[2]"
    /** Extra message content to be included on primary request message.*/
    // tslint:disable-next-line:max-line-length
    primaryRequestMessage: string =  "{\"type\": \"L\", \"value\": [ { \"type\": \"A\", \"name\": \"PPID\", \"value\": \"\"}, { \"type\": \"BI\", \"name\": \"PPBODY\", \"value\": \"\"}]}";
    /** Path location, on primary message, where RecipeName should be inserted.  */
    recipeNamePrimaryPath: string = "/[1]"
    /** Path location, on primary message, where RecipeBody should be inserted.  */
    recipeBodyPrimaryPath: string = "/[2]"
    /** Recipe Body on message content can be interpreted as a Buffer or as a String */
    recipeBodyType: RecipeBodyType = RecipeBodyType.BufferRecipeBody;
    /** SecsGem Path on S7F3 reply */
    replyPath: string = "/";
    /**
     * Success Codes to evaluate on S7F3 reply
     * If empty, any value is a success
     */
    successCodes: string = "0x00";

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
            this.recipeBody = this.recipeBody || "";


            if ( this.recipeName === "") {
                this._logger.warning(`Trying to set recipe body with an empty recipe name.`);
            }
            if ( this.recipeBody == null) {
                this._logger.warning(`Trying to set recipe body with an empty recipe body.`);
            }

            let recipeSentSuccessfully: boolean = false;
            // TODO: Entender ou adaptar esse trecho
            const bodyArray = [];
            const buffer = Buffer.from(this.recipeBody, 'base64');

            for (let i = 0; i < buffer.length; i++) {
                bodyArray.push(buffer[i]);
            }

            // this._logger.warning(`\nBody Array Length: ${bodyArray.length}`);

            // If used, check S7F1 reply
            let validS7F1Reply: boolean = false;
            if ( this.useS7F1Message ) {
                try {
                    /**
                     * Message format
                        S7F1 W
                        <L[2]
                        <A PPID>
                        <U4 LENGTH>
                        >
                        */
                       let sendMessage: any
                       if (this.primaryInquiryRequestMessage === "{}") {
                           sendMessage = { type: "S7F1"}
                       } else {
                           sendMessage = { type: "S7F1", item: JSON.parse(this.primaryInquiryRequestMessage) }
                       }

                       if (this.recipeNameInquiryPrimaryPath !== "") {
                        // On empty recipeName path, no substitution is performed.
                        const itemToInsertRecipeName: SecsItem = SecsGem.getItemByPath(sendMessage.item, this.recipeNameInquiryPrimaryPath);
                        itemToInsertRecipeName.value = this.recipeName;
                    }

                    if (this.recipeBodyLengthInquiryPrimaryPath !== "") {
                        // On empty recipeBodyLength path, no substitution is performed.
                        const itemToInsertRecipeBodyLength: SecsItem = SecsGem.getItemByPath(sendMessage.item, this.recipeBodyLengthInquiryPrimaryPath);
                        itemToInsertRecipeBodyLength.value = bodyArray.length;
                    }

                    const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);
                    /**
                     * S7F2
                        <B[1] PPGNT>
                        *  0 = OK
                        1 = Load already
                        2 = No space
                        3 = Invalid PPID
                        4 = Busy, try again
                        5 = Denied
                        >5 = Other error
                        6-64 Reserved
                        */
                    let successFound: boolean = false
                    if (this.successCodesS7F1.trim() === "" ) {
                        // if empty, any reply is a success.
                        successFound = true;
                    } else {
                        for (const successCode of this.successCodesS7F1.split(",")) {
                            if (reply &&
                                reply.item &&
                                parseInt(SecsGem.getValue(reply.item, true)) === parseInt(successCode.trim())) {
                                successFound = true;
                                break;
                            }
                        }
                    }

                    if ( successFound ) {
                        validS7F1Reply = true;
                    }
                    if ( !validS7F1Reply ) {

                        let errorResult = "error";
                        if (parseInt(SecsGem.getValue(SecsGem.getItemByPath(reply.item, this.replyPath))) < 6 ) {
                            errorResult = InquiryReturnCode[parseInt(SecsGem.getValue(SecsGem.getItemByPath(reply.item, this.replyPath)))].toString();
                        } else {
                            errorResult = InquiryReturnCode[6].toString();
                        }
                        this.error.emit(new Error(`Error on S7F1 reply: ${errorResult} Equipment error: ${JSON.stringify(reply)}`));
                        this._logger.error(`Error on S7F1 reply: ${errorResult} Equipment error: ${JSON.stringify(reply)}`);
                    }
                } catch (error) {
                    this.error.emit(error);
                }
            }

            if (validS7F1Reply || !this.useS7F1Message ) {
                // Ok, now we can send S7F3 command (Process Program Send)
                try {
                    /**
                     * S7F3 W
                        <L[2]
                        <A PPID>
                        <B PPBODY>
                        >
                        */

                    let sendMessage: any
                    if (this.primaryRequestMessage === "{}") {
                        sendMessage = { type: this.streamFunctionName }
                    } else {
                        sendMessage = { type: this.streamFunctionName, item: JSON.parse(this.primaryRequestMessage) }

                        if (this.recipeNamePrimaryPath !== "") {
                            // On empty recipeName path, no substitution is performed.
                            const itemToInsertRecipeName: SecsItem = SecsGem.getItemByPath(sendMessage.item, this.recipeNamePrimaryPath);
                            itemToInsertRecipeName.value = this.recipeName
                        }

                        if (this.recipeBodyPrimaryPath !== "") {
                            let body: Buffer;
                            switch (this.recipeBodyType) {
                                case RecipeBodyType.StringRecipeBody: body = Buffer.from(this.recipeBody, 'base64'); break;
                                case RecipeBodyType.BufferRecipeBody: body = this.recipeBody; break;
                            }


                            // On empty recipeBody path, no substitution is performed.
                            const itemToInsertRecipeBody: SecsItem = SecsGem.getItemByPath(sendMessage.item, this.recipeBodyPrimaryPath);
                            // recipeBody is treated according to item.type on primary message.
                            switch (itemToInsertRecipeBody.type) {
                                case "A":
                                    // // On "A" (ascii) type, input string which is a  ascii decimal char space separated values, are converted to string.
                                    // // .e.g., "48 49 50 51 32 65 66 67 68" on input is converted to:  <A PPBODY '0123 ABCD'> *
                                    // const a: string[] = this.recipeBody.toString().split(" ");
                                    // let outString: string = "";
                                    // for (const stringValue of a) {
                                    //     outString = outString.concat(String.fromCharCode(parseInt(stringValue)));
                                    // }
                                    itemToInsertRecipeBody.value = body.toString();
                                break;
                                case "BI":
                                default:
                                    itemToInsertRecipeBody.value = bodyArray;
                                break;
                            }
                        }
                    }

                    const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);

                    /**
                     * ACKC7: 0 = Accepted
                        1 = Denied
                        2 = Length error
                        3 = Reserved
                        4 = PPID not found
                        5 = Mode unsupported
                        >5 = Other error
                        6-64 Reserved
                        Message format
                        S7F4
                        <B[1] ACKC7>
                    */

                    let successFound: boolean = false
                    if (this.successCodes.trim() === "" ) {
                        // if empty, any reply is a success.
                        successFound = true;
                    } else {
                        for (const successCode of this.successCodes.split(",")) {
                            if (reply &&
                                reply.item &&
                                parseInt(SecsGem.getValue(SecsGem.getItemByPath(reply.item, this.replyPath))) === parseInt(successCode.trim())) {
                                successFound = true;
                                break;
                            }
                        }
                    }
                    if (successFound) {
                        recipeSentSuccessfully = true;
                        this.success.emit(true);
                    }

                    if ( !recipeSentSuccessfully ) {

                        let errorResult = "error";
                        if (parseInt(SecsGem.getValue(SecsGem.getItemByPath(reply.item, this.replyPath))) < 6 ) {
                            errorResult = DownloadReturnCode[parseInt(SecsGem.getValue(SecsGem.getItemByPath(reply.item, this.replyPath)))].toString();
                        } else {
                            errorResult = DownloadReturnCode[6].toString();
                        }

                        this.error.emit(new Error(`Error on S7F3 reply: ${errorResult} Equipment error: ${JSON.stringify(reply)}`));
                        this._logger.error(`Error on S7F3 reply: ${errorResult} Equipment error: ${JSON.stringify(reply)}`);
                    }
                } catch (error) {
                    this.error.emit(error);
                    this._logger.error(`Error on S7F3 message on Set Recipe Body Task: ${error.message}`);
                }
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

// Add settings here
/** CustomDownloadRecipeToEquipment Settings object */
export interface CustomDownloadRecipeToEquipmentSettings {
    /** SECS/GEM Stream Function Name */
    streamFunctionName: string;
    /** If True will send a Set Recipe Inquire previous to the S7F3 message*/
    useS7F1Message: boolean;
    /** Success Codes to consider on S7F1 message */
    successCodesS7F1: string;
    primaryInquiryRequestMessage: string;
    recipeNameInquiryPrimaryPath: string;
    recipeBodyLengthInquiryPrimaryPath: string;
    /** Primary Request Message */
    primaryRequestMessage: string;
    /** Path (used on primary path) to insert the Recipe Name (from task input) */
    recipeNamePrimaryPath: string;
    /** Path (used on primary path) to insert the Recipe Body (from task input) */
    recipeBodyPrimaryPath: string;
    /** Recipe Body input will be interpreted as a String or a Buffer */
    recipeBodyType: RecipeBodyType;
    /** Location (on secondary message) where Success Codes will be extracted */
    replyPath: string;
    /**
     * Success Codes to consider on secondary message reply
     * If empty string, any reply is a success.
     * (comma separated list, eg: '0x00, 0x02')
    */
    successCodes: string;
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


export enum InquiryReturnCode {
   "OK",
   "Load already",
   "No space",
   "Invalid PPID",
   "Busy, try again",
   "Denied",
   "Other error (Reserved)",
  }

export enum DownloadReturnCode {
    "Accepted",
    "Denied",
    "Length error",
    "Reserved",
    "PPID not found",
    "Mode unsupported",
    "Other error (Reserved)",
}
