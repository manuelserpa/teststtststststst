import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomCreateProcessJobTask } from "./customCreateProcessJob.task";
import { CustomCreateProcessJobDesigner } from "./customCreateProcessJob.designer";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";


@Task.TaskModule({
    task: CustomCreateProcessJobTask,
    designer: CustomCreateProcessJobDesigner,
    providers: [
        {
            class: ContainerProcessHandler,
            isSingleton: true,
            symbol: "GlobalContainerProcessHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class CustomCreateProcessJobModule {

}
