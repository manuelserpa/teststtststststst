import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { UpdateMaterialStateTask } from "./updateMaterialState.task";
import { UpdateMaterialStateDesigner } from "./updateMaterialState.designer";
import { ProcessMaterialHandler } from "../../persistence/index";


@Task.TaskModule({
    task: UpdateMaterialStateTask,
    designer: UpdateMaterialStateDesigner,
    providers: [
        {
            class: ProcessMaterialHandler,
            isSingleton: true,
            symbol: "GlobalProcessDataHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class UpdateMaterialStateModule {

}
