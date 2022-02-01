import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { GetContainerTask } from "./getContainer.task";
import { GetContainerDesigner } from "./getContainer.designer";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";


@Task.TaskModule({
    task: GetContainerTask,
    designer: GetContainerDesigner,
    providers: [
        {
            class: ContainerProcessHandler,
            isSingleton: true,
            symbol: "GlobalContainerProcessHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class GetContainerModule {

}
