import { Task, System } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomDataTraceSettings, CustomDataTraceOutputSettings, CustomDataTraceOutputType } from "./customDataTrace.task";

@Task.Designer.TaskDesigner()
export class CustomDataTraceDesigner implements Task.Designer.TaskDesignerInstance, CustomDataTraceSettings {
    _traceId: number;
    _groupSize: number;
    _numberOfSamples: number;
    _sampleInterval: string;
    emitInNewContext: boolean;
    _autoActivate: boolean;

    _outputs: CustomDataTraceOutputSettings[];

    allowNonReadable: boolean;

    public async onGetInputs(inputs: Task.TaskInputs): Promise<Task.TaskInputs> {
        return inputs;
    }

    public async onGetOutputs(outputs: Task.TaskOutputs): Promise<Task.TaskOutputs> {
        if (this._outputs) {
            for (const output of this._outputs) {
                const outputName: string = `\$${output.property.Name}`;

                if (output.outputType === CustomDataTraceOutputType.RawValue) {
                    outputs[outputName] = null;
                } else {
                    switch (output.property.DataType) {
                        case System.LBOS.Cmf.Foundation.BusinessObjects.AutomationDataType.Boolean : outputs[outputName] = Task.TaskValueType.Boolean; break;
                        case System.LBOS.Cmf.Foundation.BusinessObjects.AutomationDataType.Date : outputs[outputName] = Task.TaskValueType.Date; break;
                        case System.LBOS.Cmf.Foundation.BusinessObjects.AutomationDataType.DateTime : outputs[outputName] = Task.TaskValueType.DateTime; break;
                        case System.LBOS.Cmf.Foundation.BusinessObjects.AutomationDataType.Decimal : outputs[outputName] = Task.TaskValueType.Decimal; break;
                        case System.LBOS.Cmf.Foundation.BusinessObjects.AutomationDataType.Integer : outputs[outputName] = Task.TaskValueType.Integer; break;
                        case System.LBOS.Cmf.Foundation.BusinessObjects.AutomationDataType.Long : outputs[outputName] = Task.TaskValueType.Long; break;
                        case System.LBOS.Cmf.Foundation.BusinessObjects.AutomationDataType.String : outputs[outputName] = Task.TaskValueType.String; break;
                        case System.LBOS.Cmf.Foundation.BusinessObjects.AutomationDataType.Time : outputs[outputName] = Task.TaskValueType.Time; break;
                        default: outputs[outputName] = Task.TaskValueType.String; break;
                    }
                }
            }
        }

        return outputs;
    }
}
