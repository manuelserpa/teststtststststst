import { Dependencies, System, TYPES, DI } from "@criticalmanufacturing/connect-iot-controller-engine";
import * as inversify from "inversify";

import { CustomStoreStringArray } from "../model/customStoreStringArray";

@inversify.injectable()
export class CustomStoreStringArrayHandler implements CustomStoreStringArray {

    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject(TYPES.System.PersistedDataStore)
    private _dataStore: System.DataStore;

    public carriersBeingRequestedStorage = "_TransportRequestAwaitReply";
    async setValue(array: String[]): Promise<void> {
       return await this._dataStore.store(this.carriersBeingRequestedStorage, array, System.DataStoreLocation.Temporary);
    }
    async getValue(): Promise<any> {
        return await this._dataStore.retrieve(this.carriersBeingRequestedStorage, []);
    }

}
