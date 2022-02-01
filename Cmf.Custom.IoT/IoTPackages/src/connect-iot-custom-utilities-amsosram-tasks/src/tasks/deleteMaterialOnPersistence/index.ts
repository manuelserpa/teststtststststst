import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { DeleteMaterialOnPersistenceTask } from "./deleteMaterialOnPersistence.task";
import { DeleteMaterialOnPersistenceDesigner } from "./deleteMaterialOnPersistence.designer";
import { ProcessMaterialHandler } from "../../persistence/index";


@Task.TaskModule({
    task: DeleteMaterialOnPersistenceTask,
    designer: DeleteMaterialOnPersistenceDesigner,
    providers: [
        {
            class: ProcessMaterialHandler,
            isSingleton: true,
            symbol: "GlobalProcessDataHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class DeleteMaterialOnPersistenceModule {

}
