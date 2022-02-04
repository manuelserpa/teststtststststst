import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { DeleteContainerTask } from "./deleteContainer.task";
import { DeleteContainerDesigner } from "./deleteContainer.designer";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";


@Task.TaskModule({
    task: DeleteContainerTask,
    designer: DeleteContainerDesigner,
    providers: [
        {
            class: ContainerProcessHandler,
            isSingleton: true,
            symbol: "GlobalContainerProcessHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class DeleteContainerModule {

}
