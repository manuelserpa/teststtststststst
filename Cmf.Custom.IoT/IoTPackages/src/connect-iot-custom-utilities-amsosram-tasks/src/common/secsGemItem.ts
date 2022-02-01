import { SecsGemDataTypes } from "./secsGemDataTypes";

export class SecsGem {

    public static getValue(item: any, processBuffer: boolean = true): any {
        let result: any;

        if (item && item.value !== undefined) {
            result = item.value;

            // Check for a buffer type
            if (typeof item.value === "object" && item.value.type) {
                switch (item.value.type) {
                    case "Buffer" : result = Buffer.from(result.data); break;
                }
            }
        }

        // post processing buffers
        if (result !== undefined && result instanceof Buffer && processBuffer) {
            if (result.length > 0) {
                result = `0x${result.toString("hex")}`;
            } else {
                result = undefined; // null buffer
            }
        }

        return (result);
    }

    public static getValueFromBI(item: any, format: SecsGemDataTypes): any {
        let result: any;

        if (item && item.value !== undefined) {
            result = item.value;

            // Check for a buffer type
            if (typeof item.value === "object" && item.value.type) {
                switch (item.value.type) {
                    case "Buffer" : result = Buffer.from(result.data); break;
                }
            }
        }

        // post processing buffers
        if (result !== undefined && result instanceof Buffer) {
            if (result.length > 0) {
                switch (format) {
                    case SecsGemDataTypes.U1: result = result.readUInt8(0); break;
                    case SecsGemDataTypes.U2: result = result.readUInt16BE(0); break;
                    case SecsGemDataTypes.I1: result = result.readInt8(0); break;
                    case SecsGemDataTypes.I2: result = result.readUInt16BE(0); break;
                    default: result = undefined;
                }
            } else {
                result = undefined; // null buffer
            }
        }

        return (result);
    }

    public static getItemByPath(item: any, path: string): any {
        path = path.trim();

        if (path === "/") {
            return (item);
        }

        const paths = path.split("/").filter(function(el) { return el.length !== 0; }); // RemoveEmptyEntries
        let currentRoot = item;
        for (const next of paths) {
            if (!currentRoot || currentRoot === {}) {
                return (undefined);
            }

            let subPath: string = next.trim();
            let index: number = 1;

            if (subPath.indexOf("[") !== -1 && subPath.indexOf("]")) {
                index = parseInt(subPath.substring(subPath.lastIndexOf("[") + 1, subPath.lastIndexOf("]")).trim()) || 1;
                subPath = subPath.substring(0, subPath.lastIndexOf("[")).trim();
            }

            if (subPath === "") {
                currentRoot = SecsGem.getSubItemByIndex(currentRoot, index);
            } else {
                currentRoot = SecsGem.getSubItemByType(currentRoot, subPath, index);
            }
        }

        return (currentRoot);
    }

    public static getSubItemByIndex(item: any, index: number): any {
        if (item && typeof item === "object" && item.type === "L" && item.value && item.value instanceof Array) {
            const subItems: any[] = item.value;
            if (subItems.length >= index && index > 0) {
                return(subItems[index - 1]);
            }
        }

        return({});
    }

    public static getSubItemByType(item: any, type: string, index: number): any {
        if (item && typeof item === "object" && item.type === "L" && item.value && item.value instanceof Array) {
            const subItems: any[] = item.value;
            let foundCount: number = 0;
            for (const subItem of subItems) {
                if (subItem && typeof subItem === "object" && subItem.type === type) {
                    foundCount++;
                    if (foundCount === index) {
                        return(subItem);
                    }
                }
            }
        }

        return({});
    }

}
