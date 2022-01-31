import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomDownloadRecipeToEquipmentTask } from "./customDownloadRecipeToEquipment.task";
import { CustomDownloadRecipeToEquipmentDesigner } from "./customDownloadRecipeToEquipment.designer";


@Task.TaskModule({
    task: CustomDownloadRecipeToEquipmentTask,
    designer: CustomDownloadRecipeToEquipmentDesigner
})

export default class CustomDownloadRecipeToEquipmentModule {

}
