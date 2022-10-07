import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import {
    CustomCreateControlJobSettings, ProcessOrderMgmt, RecipeSpecificationType,

} from "./customCreateControlJob.task";

@Task.Designer.TaskDesigner()
export class CustomCreateControlJobDesigner implements Task.Designer.TaskDesignerInstance, CustomCreateControlJobSettings {
    objectSpec: string;
    occupiedSlot: string;
    useCarrierAtLoadPortAsContainer: boolean;
    RecipeSpecificationType: RecipeSpecificationType;
    ProcessOrderMgmtValue: ProcessOrderMgmt;
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
