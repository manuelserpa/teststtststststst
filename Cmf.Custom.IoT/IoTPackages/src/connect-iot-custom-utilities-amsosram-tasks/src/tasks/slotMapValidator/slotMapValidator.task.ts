import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/slotMapValidator.default";
import { MaterialData } from "../../persistence/model/materialData";
import { calculateSlotMapMismatch } from "../../utilities/slotMapValidationUtilities";

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

 * * `bool`  : ** Success ** - Triggered when the the task is executed with Success
 * * `Error` : ** error ** - Triggered when the task failed for some reason
 *
 * ### Settings
 * See {@see SlotMapValidatorSettings}
 */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-tasks-connect-iot-lg-checksum",
    inputs: {
        materialData: Task.TaskValueType.Object,
        equipmentSlotMap: Task.TaskValueType.Object,
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        Success: Task.OUTPUT_SUCCESS,
        Error: Task.OUTPUT_ERROR
    }
})
export class SlotMapValidatorTask implements Task.TaskInstance, SlotMapValidatorSettings {

    /** Accessor helper for untyped properties and output emitters. */
    [key: string]: any;

    /** **Inputs** */
    /** Activate task execution */
    public activate: any = undefined;

    public occupiedSlot: string = "1";
    public emptySlot: string = "0";
    public separator: string;
    public fixedSize: boolean = true;
    public size: number = 25;
    public terminator: string;

    public materialData: MaterialData[];
    public equipmentSlotMap: any;


    /** **Outputs** */
    /** To output a Success notification */
    public Success: Task.Output<boolean> = new Task.Output<boolean>();
    /** To output an error notification */
    public Error: Task.Output<Error> = new Task.Output<Error>();
    // public material:  Task.Output<MaterialData> = new Task.Output<MaterialData>();
    // public batchProcessComplete:  Task.Output<boolean> = new Task.Output<boolean>();

    /** Settings */
    /** Properties Settings */


    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;


    /**
     * When one or more input values is changed this will be triggered,
     * @param changes Task changes
     */
    async onChanges(changes: Task.Changes): Promise<void> {


        if (changes["activate"]) {
            // It is advised to reset the activate to allow being reactivated without the value being different

            this.activate = undefined;
            try {

                if (this.materialData == null || this.materialData[0] == null) {
                    throw new Error("No MaterialData received");
                }

                // either null, no content or one character (default retrieve from an empty slot map will be considered empty)
                if (this.equipmentSlotMap == null || this.equipmentSlotMap.length < 2) {
                    throw new Error("Slot Map was not read by the equipment for current container");
                }

                if (this.separator == null) {
                    this.separator = "";
                }

                if (this.terminator == null) {
                    this.terminator = "";
                }


                const slotMapArray = this.SlotMapToArray(this.equipmentSlotMap);

                if (slotMapArray === null) {
                    throw new Error("Unsupported type");
                }

                const materialDataSubMaterials = this.materialData[0].SubMaterials.map(x => x.Slot);

                // if fixed size calculate slot map incoming from equipment has defined size
                if (this.fixedSize) {
                    if (this.size !== slotMapArray.length) {
                        throw new Error("Slot Map is not the correct size");
                    }

                } else {
                    const biggestMaterialDataPosition = materialDataSubMaterials.reduce(function (a, b) {
                        return Math.max(a, b);
                    });

                    if (biggestMaterialDataPosition => slotMapArray.length) {
                        throw new Error("Lengths do not match");
                    }
                }

                for (let i = 0; i < slotMapArray.length; i++) {

                    const slotValue = slotMapArray[i];

                    const physicalPosition = i + 1;
                    const found = materialDataSubMaterials.some(element => element.toString() === physicalPosition.toString());

                    // calculate if slot does not match
                    if ((found && slotValue.toString() === this.emptySlot) || (!found && slotValue.toString() === this.occupiedSlot)) {
                        // slot mismatch found
                        const containerName = this.materialData[0].ContainerName;
                        const slotMapLog = calculateSlotMapMismatch(
                            containerName,
                            slotMapArray,
                            this.materialData[0].SubMaterials,
                            this.occupiedSlot,
                            this.emptySlot
                        );
                        throw new Error(slotMapLog);
                    }

                }

                this.Success.emit(true);

            } catch (error) {
                this._logger.error(`Error occurred: ${error.message}`);
                this.Error.emit(error);
            }
        }
    }

    SlotMapToArray(equipmentSlotMap: any): string[] {
        if (typeof equipmentSlotMap === "string") {
            return equipmentSlotMap.replace(this.terminator, "").split(this.separator);

        } else if (Array.isArray(equipmentSlotMap)) {
            if (this.terminator !== "") {
                return equipmentSlotMap.slice(0, equipmentSlotMap.indexOf(this.terminator) - 1);
            }
            return equipmentSlotMap;
        }

        return null;
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

}

// Add settings here
/** SlotMapValidator Settings object */
export interface SlotMapValidatorSettings {
    occupiedSlot: string;
    emptySlot: string;
    separator: string;
    fixedSize: boolean;
    size: number;
    terminator: string;
}
