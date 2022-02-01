import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { StoreMaterialPropertySettings, StoreMaterialPropertyInputSettings } from "./storeMaterialProperty.task";

@Task.Designer.TaskDesigner()
export class StoreMaterialPropertyDesigner implements Task.Designer.TaskDesignerInstance, StoreMaterialPropertySettings {

    inputs: StoreMaterialPropertyInputSettings[];
    clearActivate: boolean;
    /**
     * Resolve the inputs to be displayed in the task during design time
     * @param inputs List of inputs automatically resolved.
     * Return the updated list of inputs to design
     */
    public async onGetInputs(inputs: Task.TaskInputs): Promise<Task.TaskInputs> {
        if (this.inputs) {
            for (const input of this.inputs) {
                inputs[input.name] = input.valueType;
                this[input.name] = input.defaultValue;
            }
        }
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
