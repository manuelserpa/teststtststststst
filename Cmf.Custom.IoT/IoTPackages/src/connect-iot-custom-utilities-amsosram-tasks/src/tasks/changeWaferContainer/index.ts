import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { ChangeWaferContainerTask } from "./changeWaferContainer.task";
import { ChangeWaferContainerDesigner } from "./changeWaferContainer.designer";
import { CustomSetupStoreHandler } from "../../persistence/implementation/customSetupStoreHandler";

@Task.TaskModule({
    task: ChangeWaferContainerTask,
    designer: ChangeWaferContainerDesigner,
    providers: [
        {
            class: CustomSetupStoreHandler,
            isSingleton: true,
            symbol: "GlobalCustomSetupStoreHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
    })

export default class ChangeWaferContainerModule {

}
