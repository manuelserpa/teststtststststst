import { Dependencies, System, TYPES, DI, Container } from "@criticalmanufacturing/connect-iot-controller-engine";
import * as inversify from "inversify";
import * as moment from "moment";
import { ContainerProcess } from "../model/containerProcess";
import { ContainerData } from "../model/containerData";
import { WaferData } from "../model/waferData"

@inversify.injectable()
export class ContainerProcessHandler implements ContainerProcess {
    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject(TYPES.System.PersistedDataStore)
    private _dataStore: System.DataStore;

    private _Containers: ContainerData[];
    private _ContainerNames: string[];

    public async setWaferToContainer(containerName: string, loadPortPosition: number, slot: number,
        equipmentWaferId: string, materialWaferId: any): Promise<WaferData> {
        this._logger.error("7 ===========>");
        this._logger.error("containerName ===========>:" + containerName);
        this._logger.error("loadPortPosition ===========>:" + loadPortPosition);
        this._logger.error("slot ===========>:" + slot);
        this._logger.error("equipmentWaferId ===========>:" + equipmentWaferId);

        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        let container = await this.getContainer(containerName, loadPortPosition);
        if (!container) {
            container = await this.setContainer(containerName, loadPortPosition, null)
        }
        let wafer = await this.getWafer(container, slot, equipmentWaferId, null);
        // this._logger.error("Wafer ===========>:" + JSON.stringify(wafer));

        if (wafer) {
            this._logger.error("");
            return wafer;
        } else {
            wafer = {} as WaferData;
        }

        wafer.Slot = slot;
        wafer.EquipmentWaferId = equipmentWaferId;
        wafer.MaterialWaferId = materialWaferId;

        container.Slots.push(wafer);

        this.storeContainer(container);
        return wafer;
    }
    public async setWaferDataToContainerData(container: ContainerData, wafer: WaferData): Promise<WaferData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        const waferExists = await this.getWafer(container, wafer.Slot, wafer.EquipmentWaferId, null);
        this._logger.error("6 ===========>");
        if (waferExists) {
            this._logger.error("");
            return waferExists;
        }

        wafer.CreatedOn = moment().utc().valueOf().toString();
        wafer.ModifiedOn = wafer.CreatedOn;
        container.Slots.push(wafer);
        this.storeContainer(container);
        return wafer;
    }
    public async updateWaferOnContainer(containerName: string, loadPortPosition: number,
        slot: number, equipmentWaferId: string, materialWaferId: string): Promise<WaferData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        const container = await this.getContainer(containerName, loadPortPosition);
        if (!container) {
            this._logger.error("");
            throw new Error("");
        }
        const wafer = await this.getWafer(container, slot, equipmentWaferId, materialWaferId);
        if (!wafer) {
            this._logger.error("");
            return wafer;
        }
        if (wafer.Slot !== slot) {
            wafer.Slot = slot;
        }
        if (wafer.EquipmentWaferId !== equipmentWaferId) {
            wafer.EquipmentWaferId = equipmentWaferId;
        }
        if (wafer.MaterialWaferId !== materialWaferId) {
            wafer.MaterialWaferId = materialWaferId;
        }
        this.storeContainer(container);
        return wafer;
    }
    public async changeWaferFromContainer(sourceContainer: ContainerData, sourceWafer: WaferData, targetContainer: ContainerData,
        targetSlot: number): Promise<ContainerData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        const slot = sourceWafer.Slot;
        if (targetSlot) {
            sourceWafer.Slot = targetSlot;
        }
        targetContainer.Slots.push(sourceWafer);
        this.storeContainer(targetContainer);
        const waferToDelete = await this.getWaferBySlot(sourceContainer, slot);
        this.deleteWafer(sourceContainer, waferToDelete);
        return targetContainer;
    }

    public async getWafer(container: ContainerData, slot: number, equipmentWaferId: string, materialWaferId: string): Promise<WaferData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        let wafer = null;
        if (slot !== 0) {
            wafer = await this.getWaferBySlot(container, slot);
        }
        if (!wafer && equipmentWaferId) {
            wafer = await this.getWaferByEquipmentName(container, equipmentWaferId);
        }
        if (!wafer && materialWaferId) {
            wafer = await this.getWaferByMaterialName(container, materialWaferId);
        }
        return wafer;
    };

    public async getWaferBySlot(container: ContainerData, slot: number): Promise<WaferData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        return container.Slots.find(w => w.Slot === slot);
    }
    public async getWaferByEquipmentName(container: ContainerData, equipmentWaferId: string): Promise<WaferData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        return container.Slots.find(w => w.EquipmentWaferId === equipmentWaferId);
    }
    public async getWaferByMaterialName(container: ContainerData, materialWaferId: string): Promise<WaferData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        return container.Slots.find(w => w.MaterialWaferId === materialWaferId);
    }
    public async deleteWafer(container: ContainerData, wafer: WaferData) {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        container.Slots.splice(container.Slots.indexOf(wafer), 1);
        await this.storeContainer(container);
        return container;
    }

    public async getContainer(containerName: string, loadPortPosition: number): Promise<ContainerData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        let container = null;
        if (containerName && loadPortPosition && loadPortPosition !== 0) {
            container = await this.getContainerByNameAndLoadPort(containerName, loadPortPosition);
        }
        if (!container && containerName) {
            container = await this.getContainerByName(containerName);
        }
        if (!container && loadPortPosition && loadPortPosition !== 0) {
            container = await this.getContainerByLoadPort(loadPortPosition);
        }
        return container;
    }
    public async getContainerByNameAndLoadPort(containerName: string, loadPortPosition: number): Promise<ContainerData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        return this._Containers.find(c => c.ContainerName === containerName && c.LoadPortPosition === loadPortPosition.toString());
    }
    public async getContainerByName(containerName: string): Promise<ContainerData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        return this._Containers.find(c => c.ContainerName === containerName);
    }
    public async getContainerByLoadPort(loadPortPosition: number): Promise<ContainerData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        return this._Containers.find(c => c.LoadPortPosition === loadPortPosition.toString());
    }
    public async setContainer(containerName: string, loadPortPosition: number, slotMap: object): Promise<ContainerData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        const container = {} as ContainerData;

        container.ContainerName = containerName;
        container.SlotMap = slotMap;
        container.LoadPortPosition = loadPortPosition.toString();
        container.Slots = [];
        container.CreatedOn = moment().utc().valueOf().toString();

        await this.storeContainer(container);

        return container;
    }
    public async updateContainer(containerName: string, loadPortPosition: number, slotMap: object): Promise<ContainerData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        const container = await this.getContainer(containerName, loadPortPosition);

        if (containerName) {
            container.ContainerName = containerName;
        }
        if (slotMap) {
            container.SlotMap = slotMap;
        }
        await this.storeContainer(container);

        return container;
    }
    public async storeContainer(container: ContainerData) {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        container.ModifiedOn = moment().utc().valueOf().toString();
        const containerName = `Carrier_${container.CreatedOn}_LoadPort_${container.LoadPortPosition}`;
        const containerNameStored = this._ContainerNames.find(c => c === `Carrier_${container.CreatedOn}_LoadPort_${container.LoadPortPosition}`)
        if (!containerNameStored) {
            this._ContainerNames.push(containerName);
            await this._dataStore.store("ContainersOnPersistence", this._ContainerNames, System.DataStoreLocation.Persistent);
            this._Containers.push(container);
        }
        return await this._dataStore.store(`Carrier_${container.CreatedOn}_LoadPort_${container.LoadPortPosition}`,
            container, System.DataStoreLocation.Persistent);
    }
    public async deleteContainer(carrier: ContainerData) {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        const containerNameStored = this._ContainerNames.find(c => c === `Carrier_${carrier.CreatedOn}_LoadPort_${carrier.LoadPortPosition}`)
        if (containerNameStored) {
            this._ContainerNames.splice(this._ContainerNames.indexOf(`Carrier_${carrier.CreatedOn}_LoadPort_${carrier.LoadPortPosition}`), 1);
            await this._dataStore.store("ContainersOnPersistence", this._ContainerNames, System.DataStoreLocation.Persistent);
        }
        this._Containers.splice(this._Containers.indexOf(carrier), 1)
        await this._dataStore.store(containerNameStored, undefined, System.DataStoreLocation.Persistent);
    }

    public async InitializePersistedData() {
        this._ContainerNames = await this._dataStore.retrieve("ContainersOnPersistence", []);
        this._Containers = [] as ContainerData[];
        for (const name of this._ContainerNames) {
            const wafer = await this._dataStore.retrieve(name, undefined);
            if (wafer) {
                this._Containers.push(wafer);
            }
        }
    }

}
