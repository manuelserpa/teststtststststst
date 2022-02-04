import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { SlotMapValidatorTask } from "./slotMapValidator.task";
import { SlotMapValidatorDesigner } from "./slotMapValidator.designer";



@Task.TaskModule({
    task: SlotMapValidatorTask,
    designer: SlotMapValidatorDesigner,
    providers: [ ]
})

export default class SlotMapValidatorModule {

}
