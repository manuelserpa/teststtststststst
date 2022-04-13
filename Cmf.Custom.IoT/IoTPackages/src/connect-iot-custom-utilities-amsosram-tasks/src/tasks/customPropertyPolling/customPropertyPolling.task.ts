import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customPropertyPolling.default";
import * as moment from "moment";

// /**
//  * @whatItDoes
//  *
//  * This task
//  *
//  * @howToUse
//  *
//  * Select a KPI either by using the task settings or by providing it as an input.
//  * Set a value and the task will automatically calculate the current KPI time frame and update it with the given value.
//  *
//  *
//  * ### Inputs
//  * * `Cmf.Navigo.BusinessObjects.KPI` : **kpi** - KPI to update
//  * * `number` : **value** - New KPI Time Frame actual value
//  *
//  * ### Outputs
//  * * `boolean` : **done** - Signals the successful execution of KPI Time frame update
//  *
//  * ### Settings
//  * See {@see EquipmentEventSettings}
//  */
@Task.Task({
    name: i18n.TITLE,
    iconClass: "icon-core-tasks-connect-iot-lg-getproperties",
    inputs: {
        activate: Task.INPUT_ACTIVATE
    },
    outputs: {
        timestamp: Task.TaskValueType.DateTime,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.All
})
export class CustomPropertyPollingTask implements Task.TaskInstance, CustomPropertyPollingSettings {
    _autoActivate: boolean;
    interval: number;
    _numberOfOccurrencesAllowed: number;
    emitInNewContext: boolean;
    public error: Task.Output<Error> = new Task.Output<Error>();
    /**
     * Accessor helper for untyped properties and output emitters.
     */
    [key: string]: any;

    allowNonReadable: boolean;

    /**
     * Outputs (values/RawValues) of the properties
     */
    _outputs: CustomPropertyPollingOutputSettings[];

    /**
     * Activate task input
     */
    public activate: any = undefined;

    // Outputs:
    /** Timestamp of the occurrence */
    public timestamp: Task.Output<Date> = new Task.Output<Date>();

    /**
     * Triggered when the task finish executing
     */
    public success: Task.Output<boolean> = new Task.Output<boolean>();


    @DI.Inject(TYPES.System.Driver)
    private _driverProxy: System.DriverProxy;

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    /**
    * When input value changes, check if there is any defined output and, if so, if we have a match for the given input.
    * If a match is found, emit the new value out from the output. Otherwise, check if there is a default value that should be emitted.
    *
    * @param changes Task changes
    */
   async onChanges(changes: Task.Changes): Promise<void> {
    if (changes["activate"]) {
        // Allow notification for the same value
        this.activate = undefined;
        const activateValue: boolean = Utilities.convertValueToType(changes["activate"].currentValue, Task.TaskValueType.Boolean, false);
        if (activateValue === true) {
            await this.activateTimer();
        } else {
            if (this._activated) {
                this._logger.info(`${this._timerType} deactivated at '${moment().toDate()}'`);
            }
            await this.deActivateTimer();
        }
    }
}

    /** Destroy the timer */
    private async deActivateTimer(): Promise<void > {
        if (this._activated) {
            this._activated = false
           clearInterval(this._waitTimer);
           this._waitTimer = undefined;
           if (this._numberOfOccurrencesAllowed !== 0) {
               this._numberOfOccurrences = 0;
               }
           }
    }

    /** Create the timer */
    private async activateTimer(): Promise<void> {
        if (!this._activated) {
            this._activated = true;
        this._logger.info(`Timer activated with the interval='${this.interval} ms' at '${moment().toDate()}'`);
        this._waitTimer = setInterval(() => {
            this.manageExecutionContext(() => {
                this.getProperties();
                this.success.emit(true);
                // Check if more runs are to be performed
                if (this._numberOfOccurrencesAllowed !== 0) {
                    if (++this._numberOfOccurrences >= this._numberOfOccurrencesAllowed) {
                        this.deActivateTimer();
                    }
                }
            });
        }, this.interval);
    }
}

private async getProperties() {
    try {
        if (this._outputs) {
            const keys: System.LBOS.Cmf.Foundation.BusinessObjects.AutomationProperty[] = [];

            for (const output of this._outputs) {
                keys.push(output.property);
            }

            const values = await this._driverProxy.getProperties(keys);

            // if at least one failed, fail everything
            const anyError = values.find(v => v instanceof Error);
            if (anyError != null) {
                this.error.emit(<any>anyError);
                return;
            }

            values.forEach((value, index) => {
                const requestedProperty = keys[index];

                const outputName = `\$${requestedProperty.Name}`;
                const outputInstance = (<Task.Output<any>>this[outputName]);
                let outputType = CustomPropertyPollingOutputType.Value;

                if (this._outputs) {
                    const propertySettings = this._outputs.find(o => o.property.Id === requestedProperty.Id);

                    if (propertySettings) {
                        outputType = propertySettings.outputType;
                    }
                }

                switch (outputType) {
                    case CustomPropertyPollingOutputType.RawValue:
                        this._logger.debug(
                            `Emitting raw value for property '${requestedProperty.Name}': ${Utilities.objectToString(value.originalValue)}`);

                        if (value.originalValue && (typeof value.originalValue === "object") && value.originalValue.type) {
                            switch (value.originalValue.type) {
                                case "Buffer": outputInstance.emit(Buffer.from(value.originalValue.data)); break;
                                default: outputInstance.emit(value.originalValue); break;
                            }
                        } else {
                            outputInstance.emit(value.originalValue);
                        }
                        break;
                    default:
                        this._logger.debug(`Emitting property value '${requestedProperty.Name}'='${value.value}'`);
                        outputInstance.emit(value.value);
                        break;
                }
            });
            this.timestamp.emit(moment().toDate());
            this.success.emit(true);
        }
    } catch (error) {
        this._logger.error(`Error occurred: ${error.message}`);
        this.error.emit(error);
    }
}
async onBeforeInit(): Promise<void> {
    if (this._outputs) {
        for (const output of this._outputs) {
            this[`\$${output.property.Name}`] = new Task.Output<any>();
        }
    }
}

/** Manage whether to run the code in a new execution or not */
private manageExecutionContext(codeToExecute: () => void) {
    if (this.emitInNewContext === true) {
        const newExecutionContext = this._executionContext.fork({ properties: {} });
        newExecutionContext.run(codeToExecute);
    } else {
        codeToExecute();
    }
}

async onInit(): Promise<void> {
    this.interval = Utilities.convertValueToType(this.interval, Task.TaskValueType.Integer, 10000);
    this._autoActivate = Utilities.convertValueToType(this._autoActivate, Task.TaskValueType.Boolean, true);
    this._numberOfOccurrencesAllowed = Utilities.convertValueToType(this._numberOfOccurrencesAllowed, Task.TaskValueType.Integer, 30);

    if (this._autoActivate) {
        await this.activateTimer();
    }
}

async onDestroy(): Promise<void> {
}
}

export interface CustomPropertyPollingSettings {
    _outputs: CustomPropertyPollingOutputSettings[];
    allowNonReadable: boolean;
        /** Auto activate the event listeners */
        _autoActivate: boolean;
        /** Number of ms to define the timer */
        interval: number;
        /** Number of occurrences allowed after activation, when TimerType:Timer and TimerWorkingMode:NumberOfOccurrences */
        _numberOfOccurrencesAllowed: number;
        /** Create a new execution context when emitting an output */
        emitInNewContext: boolean;
}

export interface CustomPropertyPollingOutputSettings {
    property: System.LBOS.Cmf.Foundation.BusinessObjects.AutomationProperty;
    outputType: CustomPropertyPollingOutputType;
}

/** Get Property Value task output value data to emit */
export enum CustomPropertyPollingOutputType {
    /** Equipment data value converted to the defined datatype */
    Value = "Value",
    /** Raw value received from the equipment */
    RawValue = "RawValue"
}
