import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomCreateProcessJobTask } from "./customCreateProcessJob.task";
import { CustomCreateProcessJobDesigner } from "./customCreateProcessJob.designer";


@Task.TaskModule({
    task: CustomCreateProcessJobTask,
    designer: CustomCreateProcessJobDesigner
})

export default class CustomCreateProcessJobModule {

}
