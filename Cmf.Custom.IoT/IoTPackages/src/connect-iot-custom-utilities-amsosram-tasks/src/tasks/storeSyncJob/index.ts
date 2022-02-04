import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { StoreSyncJobTask } from "./storeSyncJob.task"
import { StoreSyncJobDesigner } from "./storeSyncJob.designer"
import { SyncInformationJobHandler } from "../../persistence/index";



@Task.TaskModule({
    task: StoreSyncJobTask,
    designer: StoreSyncJobDesigner,
    providers: [
        {
            class: SyncInformationJobHandler,
            isSingleton: true,
            symbol: "GlobalSyncInformationJobHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class StoreSyncJobModule {

}
