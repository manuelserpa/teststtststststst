import { ComponentFramework } from "cmf.core/src/core";
import Cmf from "cmf.lbos";

export interface CustomProductContainerCapacitiesProperties {
    productName?: string
    productGroupName?: string
}

export interface CustomProductContainerCapacitiesRow {
    TargetCapacity: number
    Product?: string
    ProductGroup?: string
    SourceCapacity: number
}

export const ResolveCustomProductContainerCapacities = async (framework: ComponentFramework, {
    productName, productGroupName
}: CustomProductContainerCapacitiesProperties) => {
    const smartTable = new Cmf.Foundation.BusinessObjects.SmartTables.SmartTable();
    smartTable.Name = "CustomProductContainerCapacities";

    const input = new Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects.ResolveSmartTableInput();
    input.SmartTable = smartTable;
    input.Values = new Map();

    if (productName != null) {
        input.Values.set("Product", productName);
    }

    if (productGroupName != null) {
        input.Values.set("ProductGroup", productGroupName);
    }

    const output = await framework.sandbox.lbo.call(input) as
        Cmf.Foundation.BusinessOrchestration.TableManagement.OutputObjects.ResolveSmartTableOutput;

    if (output?.Result?.["T_ST_CustomProductContainerCapacities"]?.length === 0) {
        return null;
    }

    return output.Result["T_ST_CustomProductContainerCapacities"][0] as CustomProductContainerCapacitiesRow;
}
