import { Converter, DI, Dependencies, TYPES, Task, Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customArrayPositionValue.default";

/**
 * @whatItDoes
 *
 * Extracts a value from a position of an array and type casts it
 *
 */
@Converter.Converter({
    name: i18n.TITLE,
    input: Task.TaskValueType.Object,
    output: undefined,
    parameters: {
        type: [{
            friendlyName: "Boolean",
            value: Task.TaskValueType.Boolean
        }, {
            friendlyName: "String",
            value: Task.TaskValueType.String
        }, {
            friendlyName: "Integer",
            value: Task.TaskValueType.Integer
        }, {
            friendlyName: "Long",
            value: Task.TaskValueType.Long
        }, {
            friendlyName: "Decimal",
            value: Task.TaskValueType.Decimal
        }, {
            friendlyName: "Object",
            value: Task.TaskValueType.Object
        }],
        position: Task.TaskValueType.String,
    },
})
export class CustomArrayPositionValueConverter implements Converter.ConverterInstance<object, any> {

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    /**
     * Extracts a value from a position of an array and type casts it
     * @param position position that we want to extract from array
     * @param type type of the value to be extracted from the position
     */
    transform(value: object, parameters: { [key: string]: any; }): any {

        if (!(value instanceof Array)) {
            throw new Error("Given input value is not an instance of an Array");
        }  else {
            const result = Utilities.convertValueToType(value[parameters["position"]], parameters["type"], undefined);
            return result;
        }
    }

}
