import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomCreateControlJobTask } from "./customCreateControlJob.task";
import { CustomCreateControlJobDesigner } from "./customCreateControlJob.designer";


@Task.TaskModule({
    task: CustomCreateControlJobTask,
    designer: CustomCreateControlJobDesigner
})

export default class CustomCreateControlJobModule {

}
