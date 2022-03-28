import { Task, Dependencies, System, DI, TYPES, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customDataTrace.default";
import * as moment from "moment";
import updateFactoryJobStateDefault from "../updateFactoryJobState/i18n/updateFactoryJobState.default";

import { SecsGem } from "./../../common/secsGemItem"
import { SecsItem } from "../../common/secsItem";
import { SecsTransaction, Message} from "../../common/secsTransaction"
import { PostDataColectionToMESDesigner } from "../../../__TEMP__/src/tasks/postDataCollectionToMES/postDataCollectionToMES.designer";
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
        activate: Task.INPUT_ACTIVATE,
        /** Trace  id*/
        traceId: Task.TaskValueType.Integer,
        /** group size (number of samples included in a trace report) */
        groupSize: Task.TaskValueType.Integer,
        /** Number of samples expected after trigger occurring */
        numberOfSample: Task.TaskValueType.Integer,
        /** Number of ms to define the timer */
        sampleInterval: Task.TaskValueType.String
    },
    outputs: {
        timestamp: Task.TaskValueType.DateTime,
        rawData: Task.TaskValueType.Object,
        id: Task.TaskValueType.String,
        sampleNumber: Task.TaskValueType.Integer,
        sampleTime: Task.TaskValueType.String,
        success: Task.OUTPUT_SUCCESS,
        error: Task.OUTPUT_ERROR
    },
    protocol: Task.TaskProtocol.All
})
export class CustomDataTraceTask implements Task.TaskInstance, CustomDataTraceSettings {
    _traceId: number;
    _groupSize: number;
    _numberOfSamples: number;
    _autoActivate: boolean;
    _sampleInterval: string;

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
    _outputs: CustomDataTraceOutputSettings[];

    /**
     * Activate task input
     */
    public activate: any = undefined;

    /** Trace  id*/
    public traceId: number
    /** group size (number of samples included in a trace report) */
    public groupSize: number
    /** Number of samples expected after trigger occurring */
    public numberOfSample: number
    /** Number of ms to define the timer */
    public sampleInterval: string

    private _activeTraceId: string;
    // Outputs:
    /** Timestamp of the occurrence */
    public timestamp: Task.Output<Date> = new Task.Output<Date>();
    public id: Task.Output<String> = new Task.Output<String>();
    public sampleNumber: Task.Output<Number> = new Task.Output<Number>();
    public sampleTime: Task.Output<String> = new Task.Output<String>();
    public rawData: Task.Output<Object> = new Task.Output<Object>();
    /**
     * Triggered when the task finish executing
     */
    public success: Task.Output<boolean> = new Task.Output<boolean>();
    public keys: System.LBOS.Cmf.Foundation.BusinessObjects.AutomationProperty[] = [];

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
            // register listener on driver
            await this.registerDataTraceListener()
            // register data trace on tool
            await this.activateDataTrace(this.traceId, this.sampleInterval, this.numberOfSample, this.groupSize);
        } else {
            if (this._activated) {
                this._logger.info(`${this._timerType} deactivated at '${moment().toDate()}'`);
            }
            await this.deactivateDataTrace(this.traceId);
        }
    }
}

    private async registerDataTraceListener() {
        await this._driverProxy.unsubscribeRaw(this.onMessageReceived);
        await this._driverProxy.subscribeRaw(`connect.iot.driver.secsgem.receivedMessage.S6F1`, this.onMessageReceived);
        await this._driverProxy.notifyRaw("connect.iot.driver.secsgem.registerHandler", { type: "S6F1", mode: "NotifyOnly" });
    }

    /** Destroy the timer */
    private async deactivateDataTrace(traceIdValue: number, ): Promise<void> {
        this._activated = false;

        let traceId: number = traceIdValue;
        if (!traceId) {
            traceId = this._traceId
        }
        const propertiesId = [];
        const secsMessage = {
            type: "S17F7",
            item: {
                type: "L", value: [
                    { type: "A", name: "TRID", value: this._activeTraceId },
                ]
            }
        }

        const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", secsMessage);

        let successFound = false;
        if (reply && reply.item && parseInt(reply.item.value[0].value) === 1) {
            successFound = true;
            this._activeTraceId = "";
        }

        if (!successFound) {
            this.error.emit(new Error ("Failed to delete Data Trace " + traceId));
        }
    }

    /** Create the timer */
    private async activateDataTrace( traceIdValue: number, samplePeriodValue: string, sampleNumberValue: number, groupSizeValue: number): Promise<void> {
        if (!this._activated) {
            this._activated = true;
           }
            let traceId: number = traceIdValue;
            if (!traceId) {
                traceId = this._traceId
                if (!traceId) {
                    traceId = Date.now()
                }
            }
            this._activeTraceId = traceId.toString();

            let samplePeriod: string = samplePeriodValue;
            if (!samplePeriod) {
                samplePeriod = this._sampleInterval
            }

            let sampleNumber: number = sampleNumberValue;
            if (!sampleNumber) {
                sampleNumber = this._numberOfSamples
            }

            let groupSize: number = groupSizeValue;
            if (!groupSize) {
                groupSize = this._groupSize
            }

            const propertiesId = [];
            const secsMessage = {
                type: "S2F23",
                item: {
                    type: "L", value: [
                        { type: "A", name: "TRID", value: this._activeTraceId },
                        { type: "A", name: "DSPER", value: samplePeriod },
                        { type: "A", name: "TOTSMP", value: sampleNumber.toString() },
                        { type: "A", name: "REPGSZ", value: groupSize.toString() },
                        { type: "L", value: propertiesId },
                    ]
                }
            }

            const variableIdType = (<any>this._driverProxy)._driver._currentIntegrationConfiguration.DeviceConfiguration.communication.variableIdType;
            const deviceProperties = (<any>this._driverProxy)._driver._currentIntegrationConfiguration.DeviceConfiguration.properties;
            for (const output of this._outputs) {
                this.keys.push(output.property);
                const property = deviceProperties.find(p => p.name === output.property.Name)
                propertiesId.push({type: variableIdType, value: property.deviceId });
            }


            const reply = await this._driverProxy.sendRaw("connect.iot.driver.secsgem.sendMessage", secsMessage);
                const COMMACK = SecsGem.getValue(SecsGem.getItemByPath(reply.item, "/"));
            // failed to create trace
               if (COMMACK === "0x00" || COMMACK === 0) {
                     this._logger.info(`Trace '${traceId}' started successfully.`)
                     this.success.emit(true);
                } else {
                     this._logger.error(`Unable to establish communications`);
                     this.error.emit(new Error("Unable to establish communications"));
            }
    }


// private onMessageReceived: any = (message: Message<SecsTransaction>): void => {
    private onMessageReceived: any = async (message: Message<SecsTransaction>): Promise<void> => {
        try {
            const primary = message.content.primary;



            // S6F1
            // L[] [0]
            // TRID [1]
            // Check message trace id. Ignore if doesn't match the trace id configured by this task
            const msgTraceId = Utilities.convertValueToType(
                SecsGem.getValue(SecsGem.getSubItemByIndex(primary.item, 1)).trim(),
                Task.TaskValueType.String, "");
            if (msgTraceId == null || msgTraceId !== this._activeTraceId) {
                return;
            }

            this._logger.info(`Trace received for trace id ${msgTraceId}`)

            this.id.emit(this._activeTraceId);
            // trigger event and timestamp
            this.timestamp.emit(moment().toDate());
            this.rawData.emit(primary);

            // SMPLN [2]
            const _sampleNumber = SecsGem.getValue(SecsGem.getSubItemByIndex(primary.item, 2))
            this.sampleNumber.emit(_sampleNumber);

            // STIME [3]
            const _sampleTime = SecsGem.getValue(SecsGem.getSubItemByIndex(primary.item, 3))
            this.sampleTime.emit(_sampleTime);

            // L[] - 4 List of Variables
            const values = SecsGem.getValue(SecsGem.getSubItemByIndex(primary.item, 4));

            values.forEach((value, index) => {
                const requestedProperty = this.keys[index];

                const outputName = `\$${requestedProperty.Name}`;
                const outputInstance = (<Task.Output<any>>this[outputName]);
                let outputType = CustomDataTraceOutputType.Value;

                if (this._outputs) {
                    const propertySettings = this._outputs.find(o => o.property.Id === requestedProperty.Id);

                    if (propertySettings) {
                        outputType = propertySettings.outputType;
                    }
                }

                switch (outputType) {
                    case CustomDataTraceOutputType.RawValue:
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
            this.success.emit(true);

        } catch (error) {
            this._logger.error(`Error processing data trace message: ${error.message}`);
            this.error.emit(error);
        }
    };

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
        await this.activateDataTrace(null, null, null, null);
    }
}

async onDestroy(): Promise<void> {
}
}

export interface CustomDataTraceSettings {
    _outputs: CustomDataTraceOutputSettings[];
    allowNonReadable: boolean;
    /** Auto activate the event listeners */
    _autoActivate: boolean;
    /** Trace  id*/
    _traceId: number;
    /** group size (number of samples included in a trace report) */
    _groupSize: number;
    /** Number of samples expected after trigger occurring */
    _numberOfSamples: number;
    /** Number of ms to define the timer */
    _sampleInterval: string;

    /** Create a new execution context when emitting an output */
    emitInNewContext: boolean;
}

export interface CustomDataTraceOutputSettings {
    property: System.LBOS.Cmf.Foundation.BusinessObjects.AutomationProperty;
    outputType: CustomDataTraceOutputType;
}

/** Get Property Value task output value data to emit */
export enum CustomDataTraceOutputType {
    /** Equipment data value converted to the defined datatype */
    Value = "Value",
    /** Raw value received from the equipment */
    RawValue = "RawValue"
}
