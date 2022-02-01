import { SecsMessage } from "./secsMessage";

export interface SecsTransaction {
    id: string;
    systemBytes: Buffer;
    needsReply: boolean;
    primary: SecsMessage;
    secondary: SecsMessage;
}

export interface Message<T> {
    type: string;
    content: T;
}
