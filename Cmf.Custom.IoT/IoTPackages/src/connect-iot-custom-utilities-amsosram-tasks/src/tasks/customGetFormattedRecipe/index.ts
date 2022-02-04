import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomGetFormattedRecipeTask } from "./customGetFormattedRecipe.task";
import { CustomGetFormattedRecipeDesigner } from "./customGetFormattedRecipe.designer";


@Task.TaskModule({
    task: CustomGetFormattedRecipeTask,
    designer: CustomGetFormattedRecipeDesigner
})

export default class CustomGetFormattedRecipeModule {

}
