import { Task, Dependencies, System, DI, TYPES } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customMultiCreateProcessJob.default";
import { SecsGem } from "../../common/secsGemItem"
import { SecsItem } from "../../common/secsItem";
import { SubMaterialStateEnum } from "../../persistence/model/subMaterialData";
import { MaterialData, MovementData, ContainerProcessHandler } from "../../persistence/";

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
 * See {@see CustomMultiCreateProcessJobSettings}
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
export class CustomMultiCreateProcessJobTask implements Task.TaskInstance, CustomMultiCreateProcessJobSettings {

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
    public occupiedSlot = "1"

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

    @DI.Inject("GlobalContainerProcessHandler")
    private _containerProcess: ContainerProcessHandler;

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

            let material: MaterialData;
            if (Array.isArray(this.MaterialData)) {
                material = this.MaterialData[0];
            } else {
                material = this.MaterialData;
            }
            material.ProcessJobId = `PrJob_${material.MaterialName}`;
            try {
                const carrierContentWrapper = [];
                const recipeContent = [];

                const sendMessage: Object = {
                    type: "S16F15", item: {
                        type: "L", value: [
                            { type: "U4", value: Number(Date.now().toString()) }, // data id
                            {
                                type: "L", value: [{
                                    type: "L", value: [
                                        { type: "A", value: material.ProcessJobId }, // process job id
                                        { type: "BI", value: Number(this.MaterialFormat) }, // Material format code 0x0e
                                        { type: "L", value: carrierContentWrapper }, // carrier and content (not passed on eqp characterization)
                                        { type: "L", value: recipeContent },   // recipe specification area
                                        { type: "BO", value: this.StartProcess ? 0x01 : 0x00 }, // PRPROCESSSTART
                                        { type: "L", value: this.EventList }, // PRPAUSEEVENT
                                    ]
                                }]
                            }]
                    }
                }

                recipeContent.push({ type: "U1", value: this.RecipeSpecificationType }); // Recipe Specification type
                recipeContent.push({ type: "A", value: this.RecipeName }); // Stepper Recipe PPID
                recipeContent.push({ type: "L", value: this.RecipeParameterList }); // Empty parameter list


                if (this.SendCarrierContent) {
                    if (!material.SorterJobInformation) {
                        const slotMap = [];
                        const carrierContent = { type: "L", value: [
                            { type: "A", value: material.ContainerName }, // Carrier Content
                            { type: "L", value: slotMap } // Empty parameter list
                         ]};
                         carrierContentWrapper.push(carrierContent);
                         material.SubMaterials.forEach(s => {
                            if (s.MaterialState === SubMaterialStateEnum.Queued) {
                                slotMap.push({ type: "U1", value: s.Slot })
                            }
                         });
                   } else {
                    if (material.SorterJobInformation.LogisticalProcess === "MapCarrier") {
                        // get container from persistence to get stored slot map
                        const container = await this._containerProcess.getContainer(material.ContainerName, Number(material.LoadPortPosition))
                        // sets slot map to known format
                        const slotMap = this.SlotMapToArray(container.SlotMap);
                        // slot map parsing
                        const slotValue = [];
                        const carrierContent = { type: "L", value: [
                            { type: "A", value: material.ContainerName }, // Carrier Content
                            { type: "L", value: slotValue } // Empty parameter list
                        ]};
                        for ( let position; position < slotMap.length; position++) {
                            if (slotMap[position].toString() === this.occupiedSlot.toString()) {
                                slotValue.push({ type: "U1", value: position })
                            }
                            }
                    } else {
                    const sorterMovementList = JSON.parse(material.SorterJobInformation.MovementList);
                    sorterMovementList.forEach(element => {
                        const movementData: MovementData = <MovementData> element;
                        const sourceContainer = movementData.SourceContainer;
                        const sourceSlot = movementData.SourcePosition;
                        let carrierContent = carrierContentWrapper.find(c => c.value[0].value === sourceContainer);

                        if (!carrierContent) {
                            carrierContent = { type: "L", value: [
                                { type: "A", value: sourceContainer }, // Carrier Content
                                { type: "L", value: [] } // Empty parameter list
                            ]};
                            carrierContentWrapper.push(carrierContent);
                        }
                        carrierContent.value[1].value.push({ type: "U1", value: sourceSlot })
                        });
                    }
                   }
                }

                const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);
                let successFound = false;


                if (reply && reply.item && (Boolean(reply.item.value[1].value[0].value) ? 1 : 0) === 1) {
                    successFound = true;
                }

                if (!successFound) {
                    const error = new Error(`EI: Create Multi Process Job failed on Equipment:
                    ${JSON.stringify(reply.item.value[1].value[1])}`);

                    this.error.emit(error);
                    throw error;
                }
                this.Material.emit(material);
                this.success.emit(true);

            } catch (error) {
                this.error.emit(error);
                this._logger.error(`Error on Multi Create Process Job Task: ${error.message}`);
            }

        }
    }

    SlotMapToArray(equipmentSlotMap: any, terminator: string = "", separator: string = ""): string[] {
        if (typeof equipmentSlotMap === "string") {
            return equipmentSlotMap.replace(terminator, "").split(separator);

        } else if (Array.isArray(equipmentSlotMap)) {
            if (terminator !== "") {
                return equipmentSlotMap.slice(0, equipmentSlotMap.indexOf(terminator) - 1);
            }
            return equipmentSlotMap;
        }

        return null;
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
export interface CustomMultiCreateProcessJobSettings {
    MaterialFormat: string;
    SendCarrierContent: boolean;
    RecipeSpecificationType: RecipeSpecificationType;
    occupiedSlot: string;
}

export enum RecipeSpecificationType {
    RecipeWithoutVariableTuning = 1,
    RecipeWithVariableTuning = 2
}
