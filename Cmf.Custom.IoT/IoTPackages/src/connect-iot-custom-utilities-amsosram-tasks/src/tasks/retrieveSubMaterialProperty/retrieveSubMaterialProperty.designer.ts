import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { RetrieveSubMaterialPropertySettings, RetrieveSubMaterialPropertyOutputSettings } from "./retrieveSubMaterialProperty.task";
import { MaterialStateEnum } from "../../persistence/model/materialData";
import { SubMaterialStateEnum } from "../../persistence/model/subMaterialData";
import { SlotOrderPickingDirectionEnum } from "../../utilities/slotOrderPickingDirectionEnum"

@Task.Designer.TaskDesigner()
export class RetrieveSubMaterialPropertyDesigner implements Task.Designer.TaskDesignerInstance, RetrieveSubMaterialPropertySettings {

    // Add settings (this is just an example)
    outputs: RetrieveSubMaterialPropertyOutputSettings[];
    materialState: MaterialStateEnum
    subMaterialState: SubMaterialStateEnum;
    slotOrderPickingDirection: SlotOrderPickingDirectionEnum;

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
