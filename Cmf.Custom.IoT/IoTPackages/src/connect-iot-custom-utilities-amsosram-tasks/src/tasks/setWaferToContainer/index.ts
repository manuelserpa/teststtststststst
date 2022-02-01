import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { SetWaferToContainerTask } from "./setWaferToContainer.task";
import { SetWaferToContainerDesigner } from "./setWaferToContainer.designer";
import { CustomSetupStoreHandler } from "../../persistence/implementation/customSetupStoreHandler";


@Task.TaskModule({
    task: SetWaferToContainerTask,
    designer: SetWaferToContainerDesigner,
    providers: [
        {
            class: CustomSetupStoreHandler,
            isSingleton: true,
            symbol: "GlobalCustomSetupStoreHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
    })

export default class SetWaferToContainerModule {

}
