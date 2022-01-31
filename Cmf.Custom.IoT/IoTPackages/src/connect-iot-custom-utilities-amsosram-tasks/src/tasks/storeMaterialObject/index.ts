import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { StoreMaterialObjectTask } from "./storeMaterialObject.task";
import { StoreMaterialObjectDesigner } from "./storeMaterialObject.designer";
import { ProcessMaterialHandler } from "../../persistence/index";


@Task.TaskModule({
    task: StoreMaterialObjectTask,
    designer: StoreMaterialObjectDesigner,
    providers: [
        {
            class: ProcessMaterialHandler,
            isSingleton: true,
            symbol: "GlobalProcessDataHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class StoreMaterialObjectModule {

}
