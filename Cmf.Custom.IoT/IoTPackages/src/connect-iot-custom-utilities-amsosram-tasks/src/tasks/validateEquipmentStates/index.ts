import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { ValidateEquipmentStatesTask } from "./validateEquipmentStates.task";
import { ValidateEquipmentStatesDesigner } from "./validateEquipmentStates.designer";
import { EquipmentStateModelHandler } from "../../persistence/implementation/equipmentStateModelHandler";

@Task.TaskModule({
    task: ValidateEquipmentStatesTask,
    designer: ValidateEquipmentStatesDesigner,
    providers: [
        {
            class: EquipmentStateModelHandler,
            isSingleton: true,
            symbol: "GlobalEquipmentStateModelHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class ValidateEquipmentStatesModule {

}
