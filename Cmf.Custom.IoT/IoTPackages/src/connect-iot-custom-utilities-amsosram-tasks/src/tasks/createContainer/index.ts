import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CreateContainerTask } from "./createContainer.task";
import { CreateContainerDesigner } from "./createContainer.designer";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";


@Task.TaskModule({
    task: CreateContainerTask,
    designer: CreateContainerDesigner,
    providers: [
        {
            class: ContainerProcessHandler,
            isSingleton: true,
            symbol: "GlobalContainerProcessHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class CreateContainerModule {

}
