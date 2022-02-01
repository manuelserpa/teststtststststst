import { SecsItem } from "./secsItem";

export interface SecsMessage {
    name?: string;
    description?: string;
    type: string;
    stream?: number;
    function?: number;
    item?: SecsItem;
}
