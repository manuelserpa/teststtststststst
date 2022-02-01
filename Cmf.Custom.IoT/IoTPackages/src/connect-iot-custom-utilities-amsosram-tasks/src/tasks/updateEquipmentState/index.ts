import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { UpdateEquipmentStateTask } from "./updateEquipmentState.task";
import { UpdateEquipmentStateDesigner } from "./updateEquipmentState.designer";
import { EquipmentStateModelHandler } from "../../persistence/implementation/equipmentStateModelHandler";


@Task.TaskModule({
    task: UpdateEquipmentStateTask,
    designer: UpdateEquipmentStateDesigner,
    providers: [
        {
            class: EquipmentStateModelHandler,
            isSingleton: true,
            symbol: "GlobalEquipmentStateModelHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class UpdateEquipmentStateModule {

}
