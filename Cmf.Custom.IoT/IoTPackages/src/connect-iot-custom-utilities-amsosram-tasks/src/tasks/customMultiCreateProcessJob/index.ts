import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomMultiCreateProcessJobTask } from "./customMultiCreateProcessJob.task";
import { CustomMultiCreateProcessJobDesigner } from "./customMultiCreateProcessJob.designer";


@Task.TaskModule({
    task: CustomMultiCreateProcessJobTask,
    designer: CustomMultiCreateProcessJobDesigner
})

export default class CustomMultiCreateProcessJobModule {

}
