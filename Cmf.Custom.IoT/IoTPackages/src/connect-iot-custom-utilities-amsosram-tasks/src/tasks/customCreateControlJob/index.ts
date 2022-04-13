import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomCreateControlJobTask } from "./customCreateControlJob.task";
import { CustomCreateControlJobDesigner } from "./customCreateControlJob.designer";
import { ContainerProcessHandler } from "../../persistence/implementation/containerDataHandler";


@Task.TaskModule({
    task: CustomCreateControlJobTask,
    designer: CustomCreateControlJobDesigner,
    providers: [
        {
            class: ContainerProcessHandler,
            isSingleton: true,
            symbol: "GlobalContainerProcessHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class CustomCreateControlJobModule {

}
