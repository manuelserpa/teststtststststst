import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { UpdateSubMaterialStateTask } from "./updateSubMaterialState.task";
import { UpdateSubMaterialStateDesigner } from "./updateSubMaterialState.designer";
import { ProcessMaterialHandler } from "../../persistence/index";


@Task.TaskModule({
    task: UpdateSubMaterialStateTask,
    designer: UpdateSubMaterialStateDesigner,
    providers: [
        {
            class: ProcessMaterialHandler,
            isSingleton: true,
            symbol: "GlobalProcessDataHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class UpdateSubMaterialStateModule {

}
