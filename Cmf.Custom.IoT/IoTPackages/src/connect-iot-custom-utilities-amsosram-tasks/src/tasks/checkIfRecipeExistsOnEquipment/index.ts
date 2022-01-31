import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CheckIfRecipeExistsOnEquipmentTask } from "./checkIfRecipeExistsOnEquipment.task";
import { CheckIfRecipeExistsOnEquipmentDesigner } from "./checkIfRecipeExistsOnEquipment.designer";



@Task.TaskModule({
    task: CheckIfRecipeExistsOnEquipmentTask,
    designer: CheckIfRecipeExistsOnEquipmentDesigner,
    providers: [ ]
})

export default class CheckIfRecipeExistsOnEquipmentModule {

}
