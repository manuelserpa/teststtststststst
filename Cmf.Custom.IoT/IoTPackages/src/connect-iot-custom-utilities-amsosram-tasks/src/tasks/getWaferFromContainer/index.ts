import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { GetWaferFromContainerTask } from "./getWaferFromContainer.task";
import { GetWaferFromContainerDesigner } from "./getWaferFromContainer.designer";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";


@Task.TaskModule({
    task: GetWaferFromContainerTask,
    designer: GetWaferFromContainerDesigner,
    providers: [
        {
            class: ContainerProcessHandler,
            isSingleton: true,
            symbol: "GlobalContainerProcessHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class GetWaferFromContainerModule {

}
