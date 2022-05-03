import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomReadIdTask } from "./customReadId.task";
import { CustomReadIdDesigner } from "./customReadId.designer";


@Task.TaskModule({
    task: CustomReadIdTask,
    designer: CustomReadIdDesigner
})

export default class CustomReadIdModule {

}
