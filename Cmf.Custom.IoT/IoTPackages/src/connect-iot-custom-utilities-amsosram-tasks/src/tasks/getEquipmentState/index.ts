import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { GetEquipmentStateTask } from "./getEquipmentState.task";
import { GetEquipmentStateDesigner } from "./getEquipmentState.designer";
import { EquipmentStateModelHandler } from "../../persistence/implementation/equipmentStateModelHandler";


@Task.TaskModule({
    task: GetEquipmentStateTask,
    designer: GetEquipmentStateDesigner,
    providers: [
        {
            class: EquipmentStateModelHandler,
            isSingleton: true,
            symbol: "GlobalEquipmentStateModelHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class GetEquipmentStateModule {

}
