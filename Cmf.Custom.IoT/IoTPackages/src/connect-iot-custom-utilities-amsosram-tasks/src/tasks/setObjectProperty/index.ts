import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { SetObjectPropertyTask } from "./setObjectProperty.task";
import { SetObjectPropertyDesigner } from "./setObjectProperty.designer";


@Task.TaskModule({
    task: SetObjectPropertyTask,
    designer: SetObjectPropertyDesigner
})

export default class SetObjectPropertyModule {

}
