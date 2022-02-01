import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { StoreMaterialPropertyTask } from "./storeMaterialProperty.task";
import { StoreMaterialPropertyDesigner } from "./storeMaterialProperty.designer";
import { ProcessMaterialHandler } from "../../persistence/index";


@Task.TaskModule({
    task: StoreMaterialPropertyTask,
    designer: StoreMaterialPropertyDesigner,
    providers: [
        {
            class: ProcessMaterialHandler,
            isSingleton: true,
            symbol: "GlobalProcessDataHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class StoreMaterialPropertyModule {

}
