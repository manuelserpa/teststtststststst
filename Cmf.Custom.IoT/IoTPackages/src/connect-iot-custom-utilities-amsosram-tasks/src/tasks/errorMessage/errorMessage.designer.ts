import { Task, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import {
    ErrorMessageTask,
    ErrorMessageSettings,
    ErrorMessageVerbosityType,
    LogMode,
    AUTO_IN,
    MESSAGE
} from "./errorMessage.task";
import { LinkingPort, AutoLinkResult } from "@criticalmanufacturing/connect-iot-controller-engine/src/task/designer/taskDesignerInstance";
import { Input } from "@criticalmanufacturing/connect-iot-controller-engine/src/system/systemProxy";
import { CustomErrorCodeEnum } from "../../utilities/customErrorCodeEnum";

@Task.Designer.TaskDesigner()
export class ErrorMessageDesigner implements Task.Designer.TaskDesignerInstance, ErrorMessageSettings {


    /**
     * Used to validate the auto port linking
     */
    @DI.Inject(TYPES.Task.Designer.Container)
    private _container: Task.Designer.TaskDesignerContainer;

    /**
     * Task related messages - to be filled with i18n messages before appearing in the GUI
     */
    @DI.Inject(TYPES.Task.Messages)
    private _messages: Task.TaskMessages;

    message: string;
    errorCodeToEmit: CustomErrorCodeEnum;
    errorNumber: number;
    clearInputs: boolean;
    mode: LogMode;
    isCustomFormat: boolean;
    messageFormat: string;
    inputs: Input[];
    outputs?: Task.TaskProperty[];

    private async processAutoPortLink(autoPort: string, linkingPort: LinkingPort, inputs: Task.TaskInputs, outputs: Task.TaskOutputs):
        Promise<AutoLinkResult> {
        const rt: AutoLinkResult = { messages: [] };

        // Validate port name
        rt.messages = rt.messages.concat(this._container.autoPortLinkValidation(Utilities.propertyToInput(linkingPort.name), inputs, outputs));

        // If no error, add input
        if (rt.messages.length === 0) {
            this.inputs.push({
                name: linkingPort.name,
                valueType: Object.assign({ friendlyName: linkingPort.name }, linkingPort.type)
            });

            rt.port = {
                name: Utilities.propertyToInput(linkingPort.name)
            }
        }

        return rt;
    }

    /**
     * Resolve the inputs to be displayed in the task during design time
     * @param inputs List of inputs automatically resolved.
     * Return the updated list of inputs to design
     */
    public async onGetInputs(inputs: Task.TaskInputs): Promise<Task.TaskInputs> {
        if (this.mode === LogMode.MultipleInputs) {
            if (this.inputs != null && this.inputs.length > 0) {
                for (const input of this.inputs) {
                    const inputName = Utilities.propertyToInput(input.name);
                    inputs[inputName] = this._container.getPortName(input);
                    this[inputName] = input.defaultValue;
                }
            }
            // hide the message port
            delete inputs[MESSAGE];
        } else {
            // hide the auto port
            delete inputs[AUTO_IN];
        }

        return inputs;
    }

    /**
     * Resolve the outputs to be displayed in the task during design time
     * @param outputs List of outputs automatically resolved.
     * Return the updated list of outputs to design
     */
    public async onGetOutputs(outputs: Task.TaskOutputs): Promise<Task.TaskOutputs> {
        return outputs;
    }

    /**
     * Add Inputs handler. Signals the task that a new link was connected to an 'Auto' input.
     * @param outputPort The output port of the task connecting to this task.
     */
    public async onAutoInputLink(autoPort: string, outputPort: LinkingPort, inputs: Task.TaskInputs, outputs: Task.TaskOutputs): Promise<AutoLinkResult> {
        return this.processAutoPortLink(autoPort, outputPort, inputs, outputs);
    }
}
