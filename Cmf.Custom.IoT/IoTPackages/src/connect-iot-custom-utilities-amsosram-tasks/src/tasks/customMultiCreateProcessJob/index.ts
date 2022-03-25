import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomMultiCreateProcessJobTask } from "./customMultiCreateProcessJob.task";
import { CustomMultiCreateProcessJobDesigner } from "./customMultiCreateProcessJob.designer";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";


@Task.TaskModule({
    task: CustomMultiCreateProcessJobTask,
    designer: CustomMultiCreateProcessJobDesigner,
    providers: [
        {
            class: ContainerProcessHandler,
            isSingleton: true,
            symbol: "GlobalContainerProcessHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class CustomMultiCreateProcessJobModule {

}
