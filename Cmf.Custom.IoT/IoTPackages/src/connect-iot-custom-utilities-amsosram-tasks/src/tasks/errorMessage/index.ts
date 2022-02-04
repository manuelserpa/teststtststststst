import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { ErrorMessageTask } from "./errorMessage.task";
import { ErrorMessageDesigner } from "./errorMessage.designer";


@Task.TaskModule({
    task: ErrorMessageTask,
    designer: ErrorMessageDesigner
})

export default class ErrorMessageModule {

}
