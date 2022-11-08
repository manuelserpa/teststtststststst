import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomLoadSetupConfigurationTask } from "./customLoadSetupConfiguration.task";
import { CustomLoadSetupConfigurationDesigner } from "./customLoadSetupConfiguration.designer";
import { CustomSetupStoreHandler } from "../../persistence/implementation/customSetupStoreHandler";
import { ContainerProcessHandler, EquipmentStateModelHandler, ProcessMaterialHandler, RecipeQueueHandler } from "../../persistence";

@Task.TaskModule({
    task: CustomLoadSetupConfigurationTask,
    designer: CustomLoadSetupConfigurationDesigner,
    providers: [
        {
            class: CustomSetupStoreHandler,
            isSingleton: true,
            symbol: "GlobalCustomSetupStoreHandler",
            scope: Task.ProviderScope.Controller
        },
        {
            class: ContainerProcessHandler,
            isSingleton: true,
            symbol: "GlobalContainerProcessHandler",
            scope: Task.ProviderScope.Controller
        },
        {
            class: ProcessMaterialHandler,
            isSingleton: true,
            symbol: "GlobalProcessDataHandler",
            scope: Task.ProviderScope.Controller
        },
        {
            class: EquipmentStateModelHandler,
            isSingleton: true,
            symbol: "GlobalEquipmentStateModelHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
    })
export default class CustomLoadSetupConfigurationTaskModule {

}
