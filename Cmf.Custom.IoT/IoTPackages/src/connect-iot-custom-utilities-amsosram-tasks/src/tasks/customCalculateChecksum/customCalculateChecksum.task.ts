import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customCalculateChecksum.default";
import sha256, { Hash } from "./hashFunctions/sha256";

/**
 * @whatItDoes
 *
 * This task performs Checksum Calculation of a data input with a configured hash function and outputs the string hash
 *
 * @howToUse
 *
 * ### Inputs
 * * `string || buffer`: **data** : data to calculate the hash
 * * `string`: **hashFunction** : hash Function to perform
 * * `any` : **activate** - Activate the task
 *
 * ### Outputs

 * * `string`: **hash** - result of hashFunction on data
 * * `bool`  : ** success ** - Triggered when the the task is executed with success
 * * `Error` : ** error ** - Triggered when the task failed for some reason
 *
 * ### Settings
 * See {@see CustomCalculateChecksumSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-tasks-connect-iot-lg-checksum",
    inputs: {
        data: undefined,
        hashFunction: Task.TaskValueType.String,
        activate: Task.INPUT_ACTIVATE,
    },
    outputs: {
        checksum: Task.TaskValueType.String,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class CustomCalculateChecksumTask implements Task.TaskInstance, CustomCalculateChecksumSettings {
    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public data: any;
    public hashFunction: Task.TaskValueType.String;
    public activate: any = undefined;


    /** **Outputs** */
    /** result of hashFunction on data input */
    public checksum: Task.Output<string> = new Task.Output<string>();
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();


    /** Settings */
    /** Properties Settings */
    hashFunctionSetting: HashFunctions = HashFunctions.Sha256

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {

        if (changes["activate"] ) {
            // It is advised to reset the activate to allow being reactivated without the value being different
            this.activate = undefined;

            let validHash: boolean = false;
            if ( this.hashFunction != null ) {
                if (Object.values(HashFunctions).includes(<any>this.hashFunction)) {
                    this.hashFunctionSetting = HashFunctions[this.hashFunction.toString()];
                    validHash = true;
                }
                if (! validHash) {
                    this._logger.error(`Invalid Hash specified on input: '${this.hashFunction}'`);
                    throw new Error (`Invalid Hash specified on input: '${this.hashFunction}'`);
                }
            }

            let bufferData: Buffer;
            if (this.data instanceof Buffer) {
                bufferData = this.data;
            } else if (typeof this.data === "string") {
                bufferData = Buffer.from(this.data);
            } else {
                this._logger.error("Input data type not a string or a buffer.")
                throw new Error ("Input data type not a string or a buffer.");
            }

            const dataToHash: string = this.data.toString();
            this._logger.info(`Calculating '${this.hashFunctionSetting}' of '${dataToHash.length}' chars...`)
            let hashResult: string = "";
            switch (this.hashFunctionSetting) {
                case HashFunctions.Sha256:
                    const hashInput: Uint8Array = new Uint8Array(bufferData); // new encoding.TextEncoder().encode(dataToHash);
                    const hashOutput: Uint8Array = sha256(hashInput);
                    hashResult = this.bytesConvertToHexString(hashOutput).toUpperCase();
                    break;
                default:
                    this._logger.error(`Unimplemented hash: '${this.hashFunctionSetting}'`)
                    throw new Error (`Unimplemented hash: '${this.hashFunctionSetting}'`);
                    break;
            }
            this._logger.info(`'${this.hashFunctionSetting}' hash calculated: '${hashResult}'.`);
            this.checksum.emit(hashResult);
            this.success.emit(true);
        }
    }

    private bytesConvertToHexString(byte: Uint8Array): string {
        let string = ""
        for (const val of byte) {
            string = string + ( val.toString(16) ).padStart( 2, "0" );
        }
        return string
    }

    /** Right after settings are loaded, create the needed dynamic outputs. */
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
/** CustomCalculateChecksum Settings object */
export interface CustomCalculateChecksumSettings {
    hashFunctionSetting: HashFunctions;
}

/**
 * Usable Hash Functions
 */
export enum HashFunctions {
    Sha256 = "Sha256"
}


