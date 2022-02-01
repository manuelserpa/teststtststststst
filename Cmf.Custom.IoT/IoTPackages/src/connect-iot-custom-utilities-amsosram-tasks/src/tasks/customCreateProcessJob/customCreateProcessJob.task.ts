import { Task, Dependencies, System, DI, TYPES } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customCreateProcessJob.default";

import { SecsGem } from "../../common/secsGemItem"
import { SecsItem } from "../../common/secsItem";


/**
 * @whatItDoes
 *
 * Implements Complx Secs Gem Commands
 *
 * @howToUse
 *
 * ### Inputs
 * *
 * * `any` : **activate** - Activate the task
 *
 * ### Outputs

 * * `bool`  : ** success ** - Triggered when the the task is executed with success
 * * `Error` : ** error ** - Triggered when the task failed for some reason
 *
 * ### Settings
 * See {@see CustomCreateProcessJobSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-secsgem-iot-lg-setrecipe",
    inputs: {
        RecipeName: Task.TaskValueType.String,
        MaterialData: Task.TaskValueType.Object,
        RecipeParameterList: Task.TaskValueType.Object,
        EventList: Task.TaskValueType.Object,
        StartProcess: Task.TaskValueType.Boolean,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        Material: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.SecsGem
})
export class CustomCreateProcessJobTask implements Task.TaskInstance, CustomCreateProcessJobSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;

    public MaterialData: any = undefined;
    public EventList: any = undefined;
    public RecipeName: string = "";
    public RecipeParameterList: any = undefined;
    public StartProcess: boolean = false;

    public MaterialFormat: string = "0x0e";
    public SendCarrierContent: boolean = false;
    public RecipeSpecificationType: RecipeSpecificationType = RecipeSpecificationType.RecipeWithoutVariableTuning;

    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();
    /** To output Material Data */
    public Material: Task.Output<Object> = new Task.Output<Object>();


    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject(TYPES.System.Driver)
    private _driverProxy: System.DriverProxy;

    public successCodes = "0x00";
    public replyPath = "/[1]";
    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {
        if (changes["activate"]) {
            // It is advised to reset the activate to allow being reactivated without the value being different
            this.activate = undefined;

            let material;
            if (Array.isArray(this.MaterialData)) {
                material = this.MaterialData[0];
            } else {
                material = this.MaterialData;
            }
            material.ProcessJobId = `PrJob_${material.MaterialName}`;
            try {

                const carrierContent = [];
                const recipeContent = [];
                const sendMessage: Object = {
                    type: "S16F11", item: {
                        type: "L", value: [
                            { type: "U4", value: Number(Date.now().toString()) }, // dataid
                            { type: "A", value: material.ProcessJobId },
                            { type: "BI", value: this.MaterialFormat }, // Material format code 0x0e
                            { type: "L", value: carrierContent }, // carrier and content (not passed on eqp characterization)
                            { type: "L", value: recipeContent },   // recipe specification area
                            { type: "BO", value: this.StartProcess ? 0x01 : 0x00 }, // PRPROCESSSTART
                            { type: "L", value: this.EventList }, // PRPAUSEEVENT
                        ]
                    }
                }

                recipeContent.push({ type: "U1", value: this.RecipeSpecificationType }); // Recipe Specification type
                recipeContent.push({ type: "A", value: this.RecipeName }); // Recipe PPID
                recipeContent.push({ type: "L", value: this.RecipeParameterList }); // Empty parameter list

                if (this.SendCarrierContent) {
                    const slotMap = [];
                    carrierContent.push({ type: "A", value: material.ContainerName }); // Carrier Content
                    carrierContent.push({ type: "L", value: slotMap }); // Empty parameter list

                    material.SubMaterials.forEach(s => slotMap.push({ type: "U1", value: s.Slot }));
                }

                const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);
                let successFound = false;



                if (reply && reply.item && parseInt(reply.item.value[1].value[0].data) === 1) {
                    successFound = true;
                }


                if (!successFound) {
                    const error = new Error(`EI: Create Process Job failed on Equipment: ${reply.item.value[1].value[1].value[0].value[0].data.toString()} - ${reply.item.value[1].value[1].value[0].value[1].data.toString()}`);
                    this.error.emit(error);
                    throw error;
                }
                this.Material.emit(material);
                this.success.emit(true);
            } catch (error) {
                this.error.emit(error);
                this._logger.error(`Error on Create Process Job Task: ${error.message}`);
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
}

// Add settings here
/** customCreateProcessJob Settings object */
export interface CustomCreateProcessJobSettings {
    MaterialFormat: string;
    SendCarrierContent: boolean;
    RecipeSpecificationType: RecipeSpecificationType;
}

export enum RecipeSpecificationType {
    RecipeWithoutVariableTuning = 1,
    RecipeWithVariableTuning = 2
}


