import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { SetFormattedRecipeTask, SetFormattedRecipeSettings } from "./setFormattedRecipe.task";

@Task.Designer.TaskDesigner()
export class SetFormattedRecipeDesigner implements Task.Designer.TaskDesignerInstance, SetFormattedRecipeSettings {

    streamFunctionName: string;
    /** Should use (or not) S7F1 message prior to upload */
    useS7F1Message: boolean;
    /** Success Codes to evaluate on S7F1 message. If empty, any value is a success */
    successCodesS7F1: string;
    primaryInquiryRequestMessage: string;
    recipeNameInquiryPrimaryPath: string;
    recipeBodyLengthInquiryPrimaryPath: string;
    primaryRequestMessage: string;
    recipeNamePrimaryPath: string;
    modelNamePrimaryPath: string;
    softwareRevisionPrimaryPath: string;
    recipeParameterListPrimaryPath: string;
    isBase64Encoded: boolean;
    replyPath: string;
    successCodes: string;

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
