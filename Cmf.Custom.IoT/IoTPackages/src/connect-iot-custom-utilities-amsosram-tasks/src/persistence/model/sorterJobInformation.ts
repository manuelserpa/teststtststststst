import { MovementData } from "./movementData";

export interface SorterJobInformationData {
    AlignWafer: boolean,
    FlipWafer: boolean
    LogisticalProcess: string,
    MovementList: string,
    ReadWaferId: boolean,
    SourceCarrierType: string,
    TargetCarrierType: string,
    WaferIdOnBottom: boolean,
}
