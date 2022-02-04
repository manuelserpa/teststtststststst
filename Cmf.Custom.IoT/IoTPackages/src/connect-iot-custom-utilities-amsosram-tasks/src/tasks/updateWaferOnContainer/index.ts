import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { UpdateWaferOnContainerTask } from "./updateWaferOnContainer.task";
import { UpdateWaferOnContainerDesigner } from "./updateWaferOnContainer.designer";
import { CustomSetupStoreHandler } from "../../persistence/implementation/customSetupStoreHandler";

@Task.TaskModule({
    task: UpdateWaferOnContainerTask,
    designer: UpdateWaferOnContainerDesigner,
    providers: [
        {
            class: CustomSetupStoreHandler,
            isSingleton: true,
            symbol: "GlobalCustomSetupStoreHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
    })
export default class UpdateWaferOnContainerModule {

}
