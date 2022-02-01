import { Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomSetupTask, CustomSetupSettings, CustomSetupInputSettings, CustomSetupOutputSettings } from "./customSetup.task";

@Task.Designer.TaskDesigner()
export class CustomSetupDesigner implements Task.Designer.TaskDesignerInstance, CustomSetupSettings {
    deleteReportsTimeout: number;
    internalDefineReportsTimeout: number;
    linkEventsTimeout: number;
    enableDisableEventsTimeout: number;
    enableDisableAlarmsTimeout: number;
    establishCommunicationMessageStr: string;
    establishCommunicationSuccessCodes: string;
    establishCommunicationAdditionalActions: boolean;
    establishCommunicationAdditionalActionsTimeout: number;
    setOnlineCommunicationMessageStr: string;
    setOnlineSuccessCodes: string;
    setOnlineAdditionalActions: boolean;
    setOnlineAdditionalActionsTimeout: number;
    deleteReportsAdditionalActions: boolean;
    deleteReportsAdditionalActionsTimeout: number;
    internalDefineReportsAdditionalActions: boolean;
    internalDefineReportsAdditionalActionsTimeout: number;
    linkEventsAdditionalActions: boolean;
    linkEventsAdditionalActionsTimeout: number;
    enableDisableEventsAdditionalActions: boolean;
    enableDisableEventsAdditionalActionsTimeout: number;
    enableDisableAlarmsAdditionalActions: boolean;
    enableDisableAlarmsAdditionalActionsTimeout: number;
    waitTimeBetweenRetries: number;
    inputs: CustomSetupInputSettings[];
    outputs: CustomSetupOutputSettings[];
    resetSetupOnEstablishCommunicationReceived: boolean;
    heartbeatSxFy: string;
    heartbeatBody: string;
    heartbeatTimeout: number;

    /**
     * Resolve the inputs to be displayed in the task during design time
     * @param inputs List of inputs automatically resolved.
     * Return the updated list of inputs to design
     */
    public async onGetInputs(inputs: Task.TaskInputs): Promise<Task.TaskInputs> {
        if (this.inputs) {
            for (const input of this.inputs) {
                inputs[input.name] = input.valueType;
                this[input.name] = null;
            }
        }
        return inputs;
    }

    /**
     * Resolve the outputs to be displayed in the task during design time
     * @param outputs List of outputs automatically resolved.
     * Return the updated list of outputs to design
     */
    public async onGetOutputs(outputs: Task.TaskOutputs): Promise<Task.TaskOutputs> {
        if (this.outputs) {
            for (const output of this.outputs) {
                outputs[output.name] = output.valueType;
                this[output.name] = null;
            }
        }
        return outputs;
    }
}
