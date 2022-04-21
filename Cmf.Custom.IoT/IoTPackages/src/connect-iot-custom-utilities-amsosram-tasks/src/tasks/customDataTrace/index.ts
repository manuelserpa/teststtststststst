import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomDataTraceTask } from "./customDataTrace.task";
import { CustomDataTraceDesigner } from "./customDataTrace.designer";


@Task.TaskModule({
    task: CustomDataTraceTask,
    designer: CustomDataTraceDesigner
})
export default class CustomDataTraceTaskModule {

}
