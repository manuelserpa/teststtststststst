import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomCarrierActionRequestTask } from "./customCarrierActionRequest.task";
import { CustomCarrierActionRequestDesigner } from "./customCarrierActionRequest.designer";


@Task.TaskModule({
    task: CustomCarrierActionRequestTask,
    designer: CustomCarrierActionRequestDesigner
})

export default class CustomCarrierActionRequestModule {

}
