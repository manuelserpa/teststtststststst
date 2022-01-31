import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomLoadSetupConfigurationTask } from "./customLoadSetupConfiguration.task";
import { CustomLoadSetupConfigurationDesigner } from "./customLoadSetupConfiguration.designer";
import { CustomSetupStoreHandler } from "../../persistence/implementation/customSetupStoreHandler";

@Task.TaskModule({
    task: CustomLoadSetupConfigurationTask,
    designer: CustomLoadSetupConfigurationDesigner,
    providers: [
        {
            class: CustomSetupStoreHandler,
            isSingleton: true,
            symbol: "GlobalCustomSetupStoreHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
    })
export default class CustomLoadSetupConfigurationTaskModule {

}
