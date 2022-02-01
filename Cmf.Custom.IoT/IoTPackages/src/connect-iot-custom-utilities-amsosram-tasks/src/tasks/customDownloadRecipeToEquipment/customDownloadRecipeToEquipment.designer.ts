import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomDownloadRecipeToEquipmentTask, CustomDownloadRecipeToEquipmentSettings, RecipeBodyType } from "./customDownloadRecipeToEquipment.task";

@Task.Designer.TaskDesigner()
export class CustomDownloadRecipeToEquipmentDesigner implements Task.Designer.TaskDesignerInstance, CustomDownloadRecipeToEquipmentSettings {

    streamFunctionName: string;
    useS7F1Message: boolean;
    primaryInquiryRequestMessage: string;
    recipeNameInquiryPrimaryPath: string;
    recipeBodyLengthInquiryPrimaryPath: string;
    successCodesS7F1: string
    primaryRequestMessage: string;
    recipeNamePrimaryPath: string;
    recipeBodyPrimaryPath: string;
    recipeBodyType: RecipeBodyType;
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
