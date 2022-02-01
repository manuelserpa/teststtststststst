import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { RetrieveSyncJobTask } from "./retrieveSyncJob.task";
import { RetrieveSyncJobDesigner } from "./retrieveSyncJob.designer";
import { SyncInformationJobHandler } from "../../persistence/index";


@Task.TaskModule({
    task: RetrieveSyncJobTask,
    designer: RetrieveSyncJobDesigner,
    providers: [
        {
            class: SyncInformationJobHandler,
            isSingleton: true,
            symbol: "GlobalSyncInformationJobHandler",
            scope: Task.ProviderScope.Controller
        }
    ]
})

export default class RetrieveSyncJobModule {

}
