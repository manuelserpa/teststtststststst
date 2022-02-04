import { Dependencies, System, TYPES, DI } from "@criticalmanufacturing/connect-iot-controller-engine";
import * as inversify from "inversify";
import { CustomSetupStatesEnum } from "../../utilities/setupStatesEnum";
import { CustomSetupStore } from "../model/customSetupStore";
import { DriverSetupDefinition} from "../model/customSetup";

@inversify.injectable()
export class CustomSetupStoreHandler implements CustomSetupStore {
    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject(TYPES.System.PersistedDataStore)
    private _dataStore: System.DataStore;

    private _driverSetupDefinition: DriverSetupDefinition[];
    private _driverSetupDefinitionName: string[];

    public async getInternalState(prefix: string): Promise<CustomSetupStatesEnum> {
        const internalState: number = await this._dataStore.retrieve(prefix + "_InternalState", -1);
        if (internalState === -1) {
            await this.setInternalState(prefix, CustomSetupStatesEnum.EstablishCommunication);
            return CustomSetupStatesEnum.EstablishCommunication;
        }
        return <CustomSetupStatesEnum>internalState;
    }
    public async setInternalState(prefix: string, newInternalState: CustomSetupStatesEnum): Promise<void> {
        await this._dataStore.store(prefix + "_InternalState", <number>newInternalState, System.DataStoreLocation.Temporary);
    }

    public async getWaitForAdditionalActions(prefix: string): Promise<boolean> {
        const value: boolean = await this._dataStore.retrieve(prefix + "_WaitForAdditionalActions", undefined);
        if (value === undefined) {
            await this.setWaitForAdditionalActions(prefix, false);
            return false;
        }
        return value;
    }
    public async setWaitForAdditionalActions(prefix: string, value: boolean): Promise<void> {
        await this._dataStore.store(prefix + "_WaitForAdditionalActions", value, System.DataStoreLocation.Temporary);
    }

    public async setTempValue(key: string, prefix: string, value: any): Promise<void> {
        await this._dataStore.store(prefix + "_" + key, value, System.DataStoreLocation.Temporary);
    }

    public async getValue(key: string, prefix: string, defaultValue: any): Promise<any> {
        // this._logger.info("Persistency Access: Prefix + Key: " + (prefix !== "" ? prefix + "_" : "") + key);

        return await this._dataStore.retrieve(prefix + "_" + key, await this._dataStore.retrieve(key, defaultValue));
    }

    public async getSetupDefinition(driverName: string) {
        if (!this._driverSetupDefinition) {
            await this.InitializePersistedData();
        }
        return this._driverSetupDefinition.find(s => s.DriverName === driverName);
    }
    public async setSetupDefinition(driverName: string, setupDefinition: DriverSetupDefinition) {
        if (!this._driverSetupDefinition) {
            await this.InitializePersistedData();
        }
        if (!this._driverSetupDefinitionName.find(s => s === `DriverSetupDefinition_${driverName}`)) {
            this._driverSetupDefinitionName.push(`DriverSetupDefinition_${driverName}`);
            await this._dataStore.store("_DriverSetupDefinitionNames", this._driverSetupDefinitionName, System.DataStoreLocation.Persistent);
        }
        let definition = this._driverSetupDefinition.find(s => s.DriverName === driverName);
        if (!definition) {
            this._driverSetupDefinition.push(setupDefinition);
        } else {
            definition = setupDefinition;
        }
        await this._dataStore.store(`DriverSetupDefinition_${driverName}`, setupDefinition, System.DataStoreLocation.Persistent);
    }

    public async InitializePersistedData() {
        this._driverSetupDefinitionName = await  this._dataStore.retrieve("_DriverSetupDefinitionNames", []);
        this._driverSetupDefinition = [] as DriverSetupDefinition[];
        for (const name of this._driverSetupDefinitionName) {
            const setupDefinition = await this._dataStore.retrieve(name, undefined);
            if (setupDefinition) {
            this._driverSetupDefinition.push(setupDefinition);
            }
        }
    }

}
