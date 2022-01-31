import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomSetupTask } from "./customSetup.task";
import { CustomSetupDesigner } from "./customSetup.designer";
import { CustomSetupStoreHandler } from "../../persistence/implementation/customSetupStoreHandler";


@Task.TaskModule({
    task: CustomSetupTask,
    designer: CustomSetupDesigner,
    providers: [
        {
            class: CustomSetupStoreHandler,
            isSingleton: true,
            symbol: "GlobalCustomSetupStoreHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class CustomSetupModule {

}
