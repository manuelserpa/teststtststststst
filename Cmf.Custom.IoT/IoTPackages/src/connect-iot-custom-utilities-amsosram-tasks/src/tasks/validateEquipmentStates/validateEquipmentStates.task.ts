import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import { TaskDefaultSettings } from "@criticalmanufacturing/connect-iot-controller-engine/src/system";
import i18n from "./i18n/validateEquipmentStates.default";
import { EquipmentStateModelHandler } from "../../persistence/implementation/equipmentStateModelHandler";
import { EquipmentStateModelProcess } from "../../persistence/model/equipmentStateModelProcess";
import { CustomEquipmentStateEnum } from "../../utilities/customEquipmentStateEnum";
import { EquipmentStateModelData } from "../../persistence";


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
 * See {@see ValidateEquipmentStatesSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-connect-iot-lg-logmessage",
    inputs: {
        resourceName: Task.TaskValueType.String,
        loadPort: Task.TaskValueType.Integer,
        processSubResourceNumber: Task.TaskValueType.Integer,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        match: Task.TaskValueType.Boolean,
        doesNotMatch: Task.TaskValueType.Boolean,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    }
})
export class ValidateEquipmentStatesTask implements Task.TaskInstance, ValidateEquipmentStatesSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;
    public loadPort: number;
    public processSubResourceNumber: number;
    public resourceName: string;

    /** **Outputs** */
    /** To output a success notification */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public error: Task.Output<Error> = new Task.Output<Error>();
    public match: Task.Output<boolean> = new Task.Output<boolean>();
    public doesNotMatch: Task.Output<boolean> = new Task.Output<boolean>();

    /** Settings */
    inputs: Task.TaskInput[];
    outputs: Task.TaskOutput[];
    /** Properties Settings */
    public currentState: CustomEquipmentStateEnum;
    public previousState: CustomEquipmentStateEnum;
    public comparePreviousState: boolean;

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject("GlobalEquipmentStateModelHandler")
    private _equipmentStateModels: EquipmentStateModelHandler;

    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {
        if (changes["activate"]) {
            // It is advised to reset the activate to allow being reactivated without the value being different
            this.activate = undefined;
            const equipmentState = await this._equipmentStateModels.getCurrentEquipmentState(this.resourceName,
                this.loadPort, this.processSubResourceNumber);
          if (this.comparePreviousState) {
               if (equipmentState.State === this.currentEquipmentState && equipmentState.PreviousState === this.previousEquipmentState) {
                   this.match.emit(true);
               }
               this.doesNotMatch.emit(true);
            } else {
                if (equipmentState.State === this.currentEquipmentState) {
                   this.match.emit(true);
                }
                this.doesNotMatch.emit(true);
            }
            this.success.emit(true);
        }
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
/** ValidateEquipmentStates Settings object */
export interface ValidateEquipmentStatesSettings extends TaskDefaultSettings {
    currentState: CustomEquipmentStateEnum,
    previousState: CustomEquipmentStateEnum,
    comparePreviousState: boolean
}
