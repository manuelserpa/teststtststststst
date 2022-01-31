import { SubMaterialData } from "../persistence";

export { calculateSlotMapMismatch as calculateSlotMapMismatch }

function calculateSlotMapMismatch(containerName: string, slotMapEquipment: string[], slotMapMES: SubMaterialData[], occupiedValue = "1", emptyValue = "0") {

    let slotNumber: number = 0;
    let log: string = `Container: ${containerName} MES slot map does not match equipment \n\nSlot | Equipment| Container\n`;
    for (const value of slotMapEquipment) {
        slotNumber = slotNumber + 1;
        const material = slotMapMES.find(s => s.Slot.toString() === slotNumber.toString());
        if ((value.toString() === occupiedValue && material == null) || (value.toString() === emptyValue && material != null)) {
            log += ` ${slotNumber.toString().padStart(2, "0")} | ${(value.toString() === occupiedValue ? "Occupied" : "Empty").padEnd(9, "")}| ${(material == null ? "Empty" : material.MaterialName)}\n`;
        }
    }

    return log += `\n\n`;
}
