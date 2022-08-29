import { Task, Dependencies, System, DI, TYPES } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customCreateControlJob.default";

import { SecsGem } from "../../common/secsGemItem"
import { SecsItem } from "../../common/secsItem";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";
import { MovementData } from "../../persistence/model/movementData";
import { MaterialData } from "../../persistence";



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
 * See {@see CustomCreateControlJobSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-secsgem-iot-lg-setrecipe",
    inputs: {
        MaterialData: Task.TaskValueType.Object,
        MaterialMovement: Task.TaskValueType.Object,
        StartMethod: Task.TaskValueType.Boolean,
        PauseEvents: Task.TaskValueType.Object,
        MtrlOutByStatus: Task.TaskValueType.Object,
        ProcessingControlSpecification: Task.TaskValueType.Object,
        DataCollectionPlan: Task.TaskValueType.Object,
        ControlJobIdentifier: Task.TaskValueType.String,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        Material: Task.TaskValueType.Object,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.SecsGem
})
export class CustomCreateControlJobTask implements Task.TaskInstance, CustomCreateControlJobSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;

    public MaterialData: any = undefined;
    public MaterialMovement: any = undefined;
    public ControlJobIdentifier: string;
    public StartMethod: boolean = false;
    public PauseEvent: any = undefined;
    public MtrlOutByStatus: any = undefined;
    public ProcessingControlSpecification: any = undefined;
    public DataCollectionPlan: any = undefined;

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
    public objectSpec = "Equipment"
    public occupiedSlot = "1"
    public useCarrierAtLoadPortAsContainer = false;

    @DI.Inject("GlobalContainerProcessHandler")
    private _containerProcess: ContainerProcessHandler;

    /**
     * name
     */
    public name() {
    }
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

            this._logger.error("Here! " + this.ControlJobIdentifier );

            // calculate MaterialOutSpec based on input or sorter job information (if existing) and calculate Carrier Input Spec
            const carrierInputSpec = [];
            let materialMovement = this.MaterialMovement;
            if (!materialMovement) {
                materialMovement = [];
                if (material.SorterJobInformation) {
                    const sorter = material.SorterJobInformation;
                    if (material.SorterJobInformation.LogisticalProcess === "MapCarrier") {
                        // pickup the wafer, read it, set it back on the same position, use slot map obtained from equipment
                        carrierInputSpec.push({ type: "A", value: material.ContainerName }) // Carrier Name
                        // get container from persistence to get stored slot map
                        /* const container = await this._containerProcess.getContainer(material.ContainerName, Number(material.LoadPortPosition))
                        // sets slot map to known format
                        const slotMap = this.SlotMapToArray(container.SlotMap);
                        this._logger.warning("SlotMap To Be Processed: " + JSON.stringify(slotMap));
                        // slot map parsing
                        const slotValues = []
                        const transferPair = {
                            type: "L", value: [
                                {
                                    type: "L", value: [
                                        { type: "A", value: slotValues }, // source container
                                        { type: "L", value: [] }, // slot list
                                    ]
                                },
                                {
                                    type: "L", value: [
                                        { type: "A", value: slotValues }, // destination container
                                        { type: "L", value: [] }, // slot list
                                    ]
                                }]
                        };
                        for (let position = 0; position < slotMap.length; position++) {
                            if (slotMap[position].toString() === this.occupiedSlot.toString()) {
                                slotValues.push({ type: "U1", value: position + 1 })
                            }
                        }
                        if (slotValues.length > 0) {
                            materialMovement.push(transferPair);
                        } */
                    } else {
                        const sorterMovementList = JSON.parse(material.SorterJobInformation.MovementList);
                        sorterMovementList.forEach(element => {
                            const movementData: MovementData = <MovementData>element;
                            const destinationContainer = movementData.DestinationContainer;
                            const destinationSlot = movementData.DestinationPosition
                            const sourceContainer = movementData.SourceContainer;
                            const sourceSlot = movementData.SourcePosition;
                            let transferPair = materialMovement.find(c => c.value[0].value[0].value === sourceContainer
                                && c.value[1].value[0].value === destinationContainer);

                            if (!transferPair) {
                                transferPair = {
                                    type: "L", value: [
                                        {
                                            type: "L", value: [
                                                { type: "A", value: sourceContainer }, // source container
                                                { type: "L", value: [] }, // slot list
                                            ]
                                        },
                                        {
                                            type: "L", value: [
                                                { type: "A", value: destinationContainer }, // destination container
                                                { type: "L", value: [] }, // slot list
                                            ]
                                        }]
                                };
                                materialMovement.push(transferPair);
                                // check if source container is on container input
                                if (!carrierInputSpec.find(c => c.value === sourceContainer)) {
                                    carrierInputSpec.push({ type: "A", value: sourceContainer })
                                }
                                // check ig target container is on container input
                                if (!carrierInputSpec.find(c => c.value === destinationContainer)) {
                                    carrierInputSpec.push({ type: "A", value: destinationContainer })
                                }
                            }

                            transferPair.value[0].value[1].value.push({ type: "U1", value: sourceSlot })
                            transferPair.value[1].value[1].value.push({ type: "U1", value: destinationSlot })
                        });
                    }

                } else {
                    if (material.ContainerName) { // if no container used, spec allows for empty list
                        // if no sorter job exists push Container name to Carrier Input Spec
                        carrierInputSpec.push({
                            type: "A", value: (this.useCarrierAtLoadPortAsContainer ?
                                `CarrierAtLoadPort${material.LoadPortPosition}` : material.ContainerName)
                        }) // Carrier Name
                    }
                }
            }

            material.ControlJobId = this.ControlJobIdentifier ?? `CtrlJob_${material.MaterialName}`;
            try {
                const objectContent = [];
                objectContent.push({
                    type: "L", value: [
                        { type: "A", value: "ObjID" }, // Control Job Id
                        { type: "A", value: material.ControlJobId }, // Host defined identifier of the control job.
                    ]
                });

                if (this.DataCollectionPlan) { // not mandatory per E94 standard
                    objectContent.push({
                        type: "L", value: [
                            { type: "A", value: "DataCollectionPlan" }, // Data Collection Plan
                            { type: "L", value: this.DataCollectionPlan },
                            // Identifier for a data collection plan to be used during execution of the control job.
                        ]
                    });
                }

                objectContent.push({
                    type: "L", value: [
                        { type: "A", value: "CarrierInputSpec" }, // Carrier Inputs
                        {
                            // A list of carrierID for material that will be used by the ControlJob.
                            // An empty list is allowed.
                            type: "L", value: carrierInputSpec
                        }
                    ]
                });

                objectContent.push({
                    type: "L", value: [
                        { type: "A", value: "MtrlOutSpec" }, // material movement Inputs
                        { type: "L", value: materialMovement },  // Maps material from source to destination after processing.
                        // For uni-carrier operation, the list shall be empty.
                        // The list shall also be empty, if CarrierInputSpec is an empty list
                    ]
                });

                if (this.MtrlOutByStatus) { // not mandatory per E94 standard
                    objectContent.push({
                        type: "L", value: [
                            { type: "A", value: "MtrlOutByStatus" }, // Material Output by status
                            { type: "L", value: this.MtrlOutByStatus }, // List structure which maps locations or Carriers
                            // where processed material will be placed based on material status
                        ]
                    });
                }

                if (this.PauseEvent) { // not mandatory per E94 standard
                    objectContent.push({
                        type: "L", value: [
                            { type: "A", value: "PauseEvent" }, // Pause Event
                            { type: "L", value: this.PauseEvent }, // Identifier of a list of events on which the Control Job shall PAUSE.
                        ]
                    });
                }

                if (this.ProcessingControlSpecification) {
                    objectContent.push({
                        type: "L", value: [
                            { type: "A", value: "ProcessingCtrlSpec" }, // Carrier Inputs
                            {
                                // A list of structures that defines the process jobs and rules for running each that will be run within this ControlJob.
                                type: "L", value: this.ProcessingControlSpecification
                            }]
                    });
                } else {
                    objectContent.push({
                        type: "L", value: [
                            { type: "A", value: "ProcessingCtrlSpec" }, // Carrier Inputs
                            {
                                // A list of structures that defines the process jobs and rules for running each that will be run within this ControlJob.
                                type: "L", value: [{
                                    type: "L", value: [
                                        { type: "A", value: material.ProcessJobId },
                                        { type: "L", value: [] },
                                        { type: "L", value: [] },
                                    ]
                                }]
                            },
                        ]
                    })
                }
                objectContent.push({
                    type: "L", value: [
                        { type: "A", value: "ProcessOrderMgmt" }, // ProcessOrderMgmt
                        { type: "U1", value: 1 }, // Define the method for the order in which process  jobs are initiated (currently only 1)
                        // possible values of Enum: LIST, ARRIVAL, OPTIMIZE
                    ]
                });
                objectContent.push({
                    type: "L", value: [
                        { type: "A", value: "StartMethod" }, // Start Method
                        { type: "BO", value: this.StartMethod }, // A logical flag that determines if the ControlJob can start automatically
                    ]
                });


                const sendMessage: Object = {
                    type: "S14F9", item: {
                        type: "L", value: [
                            { type: "A", value: this.objectSpec }, // object Specification (E39 Structured Text)
                            { type: "A", value: "ControlJob" }, // type of object
                            { type: "L", value: objectContent }, // structure
                        ]
                    }
                }

                const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", sendMessage);
                let successFound = false;

                if (reply && reply.item && parseInt(reply.item.value[2].value[0].value) === 0) {
                    successFound = true;
                }
                if (!successFound) {
                    const error = new Error(`EI: Create Control Job failed. Error ${reply.item.value[2].value[1].value[0].value[0].value.toString()} - ${reply.item.value[2].value[1].value[0].value[1].value.toString()}`);
                    this.error.emit(error);
                    throw error;
                }
                this.Material.emit(this.MaterialData);
                this.success.emit(true);

            } catch (error) {
                this.error.emit(error);
                this._logger.error(`Error on Create Control Job Task: ${error.message}`);
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
/** customCreateControlJob Settings object */
export interface CustomCreateControlJobSettings {
    [key: string]: any;
    objectSpec: string;
    occupiedSlot: string;
    useCarrierAtLoadPortAsContainer: boolean;
}

export enum RecipeSpecificationType {
    RecipeWithoutVariableTuning = 1,
    RecipeWithVariableTuning = 2
}


