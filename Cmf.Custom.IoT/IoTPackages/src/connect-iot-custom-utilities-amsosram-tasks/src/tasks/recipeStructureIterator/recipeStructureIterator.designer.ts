import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { RecipeStructureIteratorSettings } from "./recipeStructureIterator.task";


@Task.Designer.TaskDesigner()
export class RecipeStructureIteratorDesigner implements Task.Designer.TaskDesignerInstance, RecipeStructureIteratorSettings {
    storageName: string;
    paddingSize: number;
    paddingCharacter: string;
    isBottomUp: boolean;
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
