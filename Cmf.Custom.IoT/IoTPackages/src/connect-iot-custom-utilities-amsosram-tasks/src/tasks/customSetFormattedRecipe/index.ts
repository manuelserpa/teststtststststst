import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { SetFormattedRecipeTask } from "./setFormattedRecipe.task";
import { SetFormattedRecipeDesigner } from "./setFormattedRecipe.designer";


@Task.TaskModule({
    task: SetFormattedRecipeTask,
    designer: SetFormattedRecipeDesigner
})

export default class SetFormattedRecipeModule {

}
