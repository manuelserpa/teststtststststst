import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { RetrieveSubMaterialPropertyTask } from "./retrieveSubMaterialProperty.task";
import { RetrieveSubMaterialPropertyDesigner } from "./retrieveSubMaterialProperty.designer";
import { ProcessMaterialHandler } from "../../persistence/index";


@Task.TaskModule({
    task: RetrieveSubMaterialPropertyTask,
    designer: RetrieveSubMaterialPropertyDesigner,
    providers: [
        {
            class: ProcessMaterialHandler,
            isSingleton: true,
            symbol: "GlobalProcessDataHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class RetrieveSubMaterialPropertyModule {

}
