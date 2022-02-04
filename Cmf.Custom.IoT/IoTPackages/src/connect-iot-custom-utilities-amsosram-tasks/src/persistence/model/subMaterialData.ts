export interface SubMaterialData {
    /** Material MO ID */
    MaterialId: string,
    /** Material MO Name */
    MaterialName: string,
    /** State */
    MaterialState: SubMaterialStateEnum,
    /** Carrier position */
    Slot: number
}

export enum SubMaterialStateEnum {
    Queued = "Queued",
    InProcess = "InProcess",
    Processed = "Processed",
    Skipped = "Skipped"
}
