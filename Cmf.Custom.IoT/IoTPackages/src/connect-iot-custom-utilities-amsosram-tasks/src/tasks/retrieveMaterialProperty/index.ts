import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { RetrieveMaterialPropertyTask } from "./retrieveMaterialProperty.task";
import { RetrieveMaterialPropertyDesigner } from "./retrieveMaterialProperty.designer";
import { ProcessMaterialHandler } from "../../persistence/index";


@Task.TaskModule({
    task: RetrieveMaterialPropertyTask,
    designer: RetrieveMaterialPropertyDesigner,
    providers: [
        {
            class: ProcessMaterialHandler,
            isSingleton: true,
            symbol: "GlobalProcessDataHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class RetrieveMaterialPropertyModule {

}
