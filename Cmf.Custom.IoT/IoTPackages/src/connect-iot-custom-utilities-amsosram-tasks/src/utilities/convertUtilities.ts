
export { convertValueToType as convertValueToType}

function convertValueToType(value: any, toType: ValueType | string, defaultValue?: any, throwOnError?: boolean): any {
    if (value == null) {
        return (defaultValue);
    }

    // Used for different types conversions
    const valueAsString = String(value).trim().toLocaleLowerCase();

    if (typeof(toType) === "string") {
        const stringType: string = toType == null ? "string" : toType.trim().toLocaleLowerCase();
        let valueType: ValueType = ValueType.String;
        switch (stringType) {
            case "string": valueType = ValueType.String; break;
            case "integer": valueType = ValueType.Integer; break;
            case "long": valueType = ValueType.Long; break;
            case "decimal": valueType = ValueType.Decimal; break;
            case "boolean": valueType = ValueType.Boolean; break;
            case "object": valueType = ValueType.Object; break;
            case "password": valueType = ValueType.Password; break;
            case "datetime": valueType = ValueType.DateTime; break;
            case "culture": valueType = ValueType.Culture; break;
            default: valueType = ValueType.String; break;
        }

        // Continue with type converted
        toType = valueType;
    }

    try {
        switch (toType) {
            case ValueType.Boolean: {
                if (typeof(value) === "boolean") {
                    return (value);
                } else {
                    return( valueAsString === "true" ||
                            valueAsString === "t" ||
                            valueAsString === "yes" ||
                            valueAsString === "y" ||
                            valueAsString === "1" );
                }
            }
            case ValueType.String:
            case ValueType.Password:
            case ValueType.Culture:
                return ((typeof(value) === "string") ? value : String(value));
            case ValueType.Decimal: {
                if (typeof(value) === "number") {
                    return (value);
                } else {
                    const converted = parseFloat(valueAsString);
                    if (isNaN(converted)) {
                        throw new Error(`Unable to convert '${valueAsString}' to decimal`);
                    }
                    return(converted);
                }
            }
            case ValueType.Integer:
            case ValueType.Long: {
                if (typeof(value) === "number") {
                    return (value);
                } else {
                    const converted = parseInt(valueAsString);
                    if (isNaN(converted)) {
                        throw new Error(`Unable to convert '${valueAsString}' to integer`);
                    }
                    return(converted);
                }
            }
            case ValueType.DateTime: {
                if (value instanceof Date) {
                    return (value);
                } else {
                    const converted = new Date(valueAsString);
                    if (isNaN(converted.getTime())) {
                        throw new Error(`Unable to convert '${valueAsString}' to date`);
                    }
                    return(converted);
                }
            }
            case ValueType.Object: return ((typeof(value) === "object") ? value : JSON.parse(String(value)));
        }
    } catch (error) {
        const mustFail = throwOnError == null ? true : throwOnError;
        if (defaultValue != null || !mustFail) {
            return(defaultValue);
        } else {
            throw error;
        }
    }

    // Got here, simply return the original value
    return (value);
}


/** Value types */
enum ValueType {
    /** Boolean value */
    Boolean = "Boolean",
    /** String value*/
    String = "String",
    /** Decimal value */
    Decimal = "Decimal",
    /** Integer value */
    Integer = "Integer",
    /** Long value (right now is the same as integer) */
    Long = "Long",
    /** Object value (json object) */
    Object = "Object",
    /** Password value */
    Password = "Password",
    /** DateTime value */
    DateTime = "DateTime",
    /** Culture value */
    Culture = "Culture",
}
