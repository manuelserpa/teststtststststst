import { Dependencies, System, TYPES, DI } from "@criticalmanufacturing/connect-iot-controller-engine";
import * as inversify from "inversify";
import * as moment from "moment";
import { EquipmentStateModelProcess } from "../model/equipmentStateModelProcess";
import { CustomEquipmentStateEnum } from "../../utilities/customEquipmentStateEnum";
import { EquipmentStateModelData } from "../model/equipmentStateModelData";

@inversify.injectable()
export class  EquipmentStateModelHandler implements EquipmentStateModelProcess {
    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject(TYPES.System.PersistedDataStore)
    private _dataStore: System.DataStore;

    private _EquipmentStateModels: EquipmentStateModelData[];
    private _EquipmentStateModelNames: string[];

    async updateEquipmentState(resourceName: string, loadPort: number, processSubResourceNumber: number, equipmentState: CustomEquipmentStateEnum) {
        if ( this._EquipmentStateModels === undefined ) {
            await this.InitializePersistedData();
        }

        if (!resourceName) {
            resourceName = await  this._dataStore.retrieve("Generic_ResourceName", null);
        }

        let equipmentStateModel: EquipmentStateModelData;
        let stateModel: EquipmentStateModelData;
        equipmentStateModel = await this.getCurrentEquipmentState(resourceName, null, null);
        if (!equipmentStateModel) {
            equipmentStateModel = {
                Name: resourceName,
                LoadPortNumber: null,
                ProcessSubResourceNumber: null,
                State: equipmentState,
                ModifiedOn: moment().utc().valueOf().toString(),
                PreviousModification: null,
                PreviousState: null,
                SubResourcesStates: []
            }
        } else if (!loadPort && !processSubResourceNumber) {
            equipmentStateModel.PreviousModification = equipmentStateModel.ModifiedOn;
            equipmentStateModel.PreviousState = equipmentStateModel.State;
            equipmentStateModel.State = equipmentState;
            equipmentStateModel.ModifiedOn = moment().utc().valueOf().toString();
        }

        if (loadPort) {
            stateModel = equipmentStateModel.SubResourcesStates.find(s => s.LoadPortNumber.toString() === loadPort.toString());
            if (!stateModel) {
                stateModel = {
                    Name: null,
                    LoadPortNumber: loadPort,
                    ProcessSubResourceNumber: null,
                    State: equipmentState,
                    ModifiedOn: moment().utc().valueOf().toString(),
                    PreviousModification: null,
                    PreviousState: null,
                    SubResourcesStates: []
                }
                equipmentStateModel.SubResourcesStates.push(stateModel);
            } else {
                stateModel.PreviousModification = equipmentStateModel.ModifiedOn;
                stateModel.PreviousState = equipmentStateModel.State;
                stateModel.State = equipmentState;
                stateModel.ModifiedOn = moment().utc().valueOf().toString();
            }
        } else if (processSubResourceNumber) {
            stateModel = equipmentStateModel.SubResourcesStates.find(s => s.ProcessSubResourceNumber.toString() === processSubResourceNumber.toString());
            if (!stateModel) {
                stateModel = {
                    Name: null,
                    LoadPortNumber: null,
                    ProcessSubResourceNumber: processSubResourceNumber,
                    State: equipmentState,
                    ModifiedOn: moment().utc().valueOf().toString(),
                    PreviousModification: null,
                    PreviousState: null,
                    SubResourcesStates: []
                }
                equipmentStateModel.SubResourcesStates.push(stateModel);
            } else {
                stateModel.PreviousModification = equipmentStateModel.ModifiedOn;
                stateModel.PreviousState = equipmentStateModel.State;
                stateModel.State = equipmentState;
                stateModel.ModifiedOn = moment().utc().valueOf().toString();
            }
        }
        await this.storeState(equipmentStateModel);
        if (stateModel) {
            return stateModel;
        }
        return equipmentStateModel;
    }
    async getCurrentEquipmentState(resourceName: string, loadPort: number, processSubResourceNumber: number) {
        if ( this._EquipmentStateModels === undefined ) {
            await this.InitializePersistedData();
        }

        if (!resourceName) {
            resourceName = await  this._dataStore.retrieve("Generic_ResourceName", null);
        }

        let equipmentStateModel: EquipmentStateModelData;
        equipmentStateModel = await this._EquipmentStateModels.find(s => s.Name === resourceName);

        if (equipmentStateModel && loadPort) {
            equipmentStateModel = equipmentStateModel.SubResourcesStates.find(s => s.LoadPortNumber.toString() === loadPort.toString());
        }
        if (equipmentStateModel && processSubResourceNumber) {
            equipmentStateModel = equipmentStateModel.SubResourcesStates.
            find(s => s.ProcessSubResourceNumber.toString() === processSubResourceNumber.toString());
        }
        return equipmentStateModel;
    }
    async storeState(equipmentStateModel: EquipmentStateModelData) {
        if ( this._EquipmentStateModels === undefined ) {
            await this.InitializePersistedData();
        }
        const stateStorageName = `EquipmentState_${equipmentStateModel.Name}`;
        const stateStorageNameStored = this._EquipmentStateModelNames.find( c => c === stateStorageName)
        if (!stateStorageNameStored) {
           this._EquipmentStateModelNames.push(stateStorageName);
           await this._dataStore.store("EquipmentStateModelOnPersistence", this._EquipmentStateModelNames, System.DataStoreLocation.Persistent);
           this._EquipmentStateModels.push(equipmentStateModel);
        }
        return await this._dataStore.store(stateStorageName,
            equipmentStateModel, System.DataStoreLocation.Persistent);
    }
    async InitializePersistedData() {
        this._EquipmentStateModelNames = await  this._dataStore.retrieve("EquipmentStateModelOnPersistence", []);
        this._EquipmentStateModels = [] as EquipmentStateModelData[];
        for (const name of this._EquipmentStateModelNames) {
            const wafer = await this._dataStore.retrieve(name, undefined);
            if (wafer) {
            this._EquipmentStateModels.push(wafer);
            }
        }
    }
}
