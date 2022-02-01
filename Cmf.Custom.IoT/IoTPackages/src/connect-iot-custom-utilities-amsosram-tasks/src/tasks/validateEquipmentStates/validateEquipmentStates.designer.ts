import { Task, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import { ValidateEquipmentStatesSettings } from "./validateEquipmentStates.task";
import { CustomEquipmentStateEnum } from "../../utilities/customEquipmentStateEnum"

@Task.Designer.TaskDesigner()
export class ValidateEquipmentStatesDesigner implements Task.Designer.TaskDesignerInstance, ValidateEquipmentStatesSettings {
    currentState: CustomEquipmentStateEnum;
    previousState: CustomEquipmentStateEnum;
    comparePreviousState: boolean;

    // Add settings (this is just an example)
    inputs: Task.TaskInput[];
    outputs: Task.TaskOutput[];
    message: string;

    /**
     * Resolve the inputs to be displayed in the task during design time
     * @param inputs List of inputs automatically resolved.
     * Return the updated list of inputs to design
     */
    public async onGetInputs(inputs: Task.TaskInputs): Promise<Task.TaskInputs> {

        return inputs;
    }

    /**
     * Resolve the outputs to be displayed in the task during design time
     * @param outputs List of outputs automatically resolved.
     * Return the updated list of outputs to design
     */
    public async onGetOutputs(outputs: Task.TaskOutputs): Promise<Task.TaskOutputs> {

        return outputs;
    }
}
