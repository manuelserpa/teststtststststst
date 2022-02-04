import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { UpdateContainerTask } from "./updateContainer.task";
import { UpdateContainerDesigner } from "./updateContainer.designer";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";


@Task.TaskModule({
    task: UpdateContainerTask,
    designer: UpdateContainerDesigner,
    providers: [
        {
            class: ContainerProcessHandler,
            isSingleton: true,
            symbol: "GlobalContainerProcessHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class UpdateContainerModule {

}
