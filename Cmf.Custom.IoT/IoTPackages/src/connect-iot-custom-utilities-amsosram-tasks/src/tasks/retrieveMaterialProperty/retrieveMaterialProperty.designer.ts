import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { RetrieveMaterialPropertySettings, RetrieveMaterialPropertyOutputSettings } from "./retrieveMaterialProperty.task";
import { MaterialStateEnum } from "../../persistence/model/materialData";

@Task.Designer.TaskDesigner()
export class RetrieveMaterialPropertyDesigner implements Task.Designer.TaskDesignerInstance, RetrieveMaterialPropertySettings {

    outputs: RetrieveMaterialPropertyOutputSettings[];
    materialState: MaterialStateEnum;
    retrieveAllMaterialsInCondition: boolean;

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
        if (this.outputs) {
            for (const output of this.outputs) {
                outputs[output.name] = output.valueType;
            }
        }
        return outputs;
    }
}
