import { WaferData } from "./waferData";


export interface ContainerData {
    /** Material MO ID */
    ContainerName: string,
    /** Sub Material List */
    Slots: WaferData[],
    /** Created on */
    CreatedOn: string,
    /** Last Modified time */
    ModifiedOn: string
    /** Load Port Position */
    LoadPortPosition: string
    /** Slot Map */
    SlotMap: object;
}
