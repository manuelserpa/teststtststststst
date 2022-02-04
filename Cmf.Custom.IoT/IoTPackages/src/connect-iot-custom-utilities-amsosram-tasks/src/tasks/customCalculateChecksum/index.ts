import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomCalculateChecksumTask } from "./customCalculateChecksum.task";
import { CustomCalculateChecksumDesigner } from "./customCalculateChecksum.designer";


@Task.TaskModule({
    task: CustomCalculateChecksumTask,
    designer: CustomCalculateChecksumDesigner
})

export default class CustomCalculateChecksumModule {

}
