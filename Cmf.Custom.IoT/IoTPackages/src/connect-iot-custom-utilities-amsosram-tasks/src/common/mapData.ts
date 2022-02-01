
/** Representation of a map from/to equipment */
export interface MapData {
    /** Identifier of the map */
    materialId: string;
    /* Material Id Type */
    idType?: number;
    /* Map Format */
    mapFormat?: number;
    /** Angle where the notch is located */
    notchLocation?: number;
    /** Angle of frame rotation */
    frameRotation?: number;
    /** Origin Location */
    originLocation?: number;
    /** Process Axis */
    processAxis?: number;
    /** Bin code equivalents */
    binCodes?: string;
    /** Null Bin code value */
    nullBinCode?: string;
    /** List of reference points */
    referencePoints?: number[][];
    /** Die units of measurement */
    unitsOfMeasurement?: string;
    /** X-axis die size */
    xDies?: number;
    /** Y-axis die size */
    yDies?: number;
    /** Row count in die increments */
    rowCount?: number;
    /** Col count in die increments */
    colCount?: number;
    /** Process die count */
    processDieCount?: number;
    /** Starting positions when requested */
    startingPosition?: number[];
    /** Map as a string */
    mapString?: string;
    /** Map as a string */
    mapArray?: string[];
}
