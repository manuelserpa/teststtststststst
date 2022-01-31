import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomMaterialTransferStatusRequestTask } from "./customMaterialTransferStatusRequest.task";
import { CustomMaterialTransferStatusRequestDesigner } from "./customMaterialTransferStatusRequest.designer";


@Task.TaskModule({
    task: CustomMaterialTransferStatusRequestTask,
    designer: CustomMaterialTransferStatusRequestDesigner
})

export default class CustomMaterialTransferStatusRequestModule {

}
