import { Converter, DI, Dependencies, TYPES, Task } from "@criticalmanufacturing/connect-iot-controller-engine";
import i18n from "./i18n/customConvertStringToArrayConverter.default";

/**
 * @whatItDoes
 *
 * Transforms String To Array
 *
 */
@Converter.Converter({
    name: i18n.TITLE,
    input: Task.TaskValueType.String,
    output: Task.TaskValueType.Object,
    parameters: {
        separators: Task.TaskValueType.String,
    },
})
export class CustomConvertStringToArrayConverterConverter implements Converter.ConverterInstance<string, object> {

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    /**
     * Transforms String To Array
     * @param value string value
     * @param parameters Transformation parameters
     */
    transform(value: string, parameters: { [key: string]: any; }): object {
        return value.split(parameters["separators"]);
    }
}
