import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomControlJobRequestTask } from "./customControlJobRequest.task";
import { CustomControlJobRequestDesigner } from "./customControlJobRequest.designer";


@Task.TaskModule({
    task: CustomControlJobRequestTask,
    designer: CustomControlJobRequestDesigner
})

export default class CustomControlJobRequestModule {

}
