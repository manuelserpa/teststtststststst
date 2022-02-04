import { Task, System } from "@criticalmanufacturing/connect-iot-controller-engine";
import { CustomLoadSetupConfigurationTask, CustomLoadSetupConfigurationSettings } from "./customLoadSetupConfiguration.task";


@Task.Designer.TaskDesigner()
export class CustomLoadSetupConfigurationDesigner implements Task.Designer.TaskDesignerInstance, CustomLoadSetupConfigurationSettings {
    /** EntityType Name */
    entityTypeName: string;
    /** Attributes to be loaded */
    attributes: {Name: string, ScalarTypeName: string}[];
    /** Load all attributes flag */
    loadAllAttributes: boolean;
    /** LevelsToLoad to be used in loading */
    levelsToLoad: number;
    /** Driver name */
    driverName: string;
    /** Flag that tells if its to load entity in every change */
    reloadEveryChange: boolean;
    /** Flag that determines if the activation is to be done when the entity is received */
    _autoActivate: boolean;
    /** Number of retries until a good answer is received from System */
    public retries: number;
    /** Number of milliseconds to wait between retries */
    sleepBetweenRetries: number;


    /**
     * Deal with dynamic outputs and set the current typing for instance output
     */
    public async onGetOutputs(outputs: Task.TaskOutputs): Promise<Task.TaskOutputs> {
        // // For now , do a simple parse
        // if (this.attributes != null && this.attributes.length > 0) {
        //     this.attributes.forEach(property => {
        //         const scalarType = property.ScalarTypeName;
        //         if (scalarType === "bit") {
        //             outputs[property.Name] = Task.TaskValueType.Boolean;
        //         } else if (scalarType === "datetime") {
        //             outputs[property.Name] = Task.TaskValueType.DateTime;
        //         } else if (scalarType === "decimal") {
        //             outputs[property.Name] = Task.TaskValueType.Decimal;
        //         } else if (scalarType === "int") {
        //             outputs[property.Name] = Task.TaskValueType.Integer;
        //         } else {
        //             outputs[property.Name] = Task.TaskValueType.String;
        //         }
        //     });
        // }
        // if (this.entityTypeName != null) {
        //     outputs["instance"] = <Task.TaskComplexValueType>{
        //         type: System.PropertyValueType.ReferenceType,
        //         referenceType: System.LBOS.Cmf.Foundation.Common.ReferenceType.EntityType,
        //         referenceTypeName: this.entityTypeName,
        //         collectionType: System.CollectionType.None
        //     }
        // }
        return outputs;
    }
}
