import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { RecipeStructureIteratorTask } from "./recipeStructureIterator.task";
import { RecipeStructureIteratorDesigner } from "./recipeStructureIterator.designer";
import { RecipeQueueHandler } from "../../persistence/index";



@Task.TaskModule({
    task: RecipeStructureIteratorTask,
    designer: RecipeStructureIteratorDesigner,
    providers: [
        {
            class: RecipeQueueHandler,
            isSingleton: true,
            symbol: "GlobalRecipeQueueHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class RecipeStructureIteratorModule {

}
