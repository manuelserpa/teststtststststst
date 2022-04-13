import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomPropertyPollingTask } from "./customPropertyPolling.task";
import { CustomPropertyPollingDesigner } from "./customPropertyPolling.designer";


@Task.TaskModule({
    task: CustomPropertyPollingTask,
    designer: CustomPropertyPollingDesigner
})
export default class CustomPropertyPollingTaskModule {

}
