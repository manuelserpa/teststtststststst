import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomSendAdHocEquipmentRequestTask } from "./customSendAdHocEquipmentRequest.task";
import { CustomSendAdHocEquipmentRequestDesigner } from "./customSendAdHocEquipmentRequest.designer";


@Task.TaskModule({
    task: CustomSendAdHocEquipmentRequestTask,
    designer: CustomSendAdHocEquipmentRequestDesigner
})

export default class CustomSendAdHocEquipmentRequestModule {

}
