import { Dependencies, System, TYPES, DI, Container } from "@criticalmanufacturing/connect-iot-controller-engine";
import * as inversify from "inversify";
import * as moment from "moment";
import { ContainerProcess } from "../model/containerProcess";
import { ContainerData } from "../model/containerData";
import { WaferData } from "../model/waferData"
import { container } from "@angular/core/src/render3";

@inversify.injectable()
export class ContainerProcessHandler implements ContainerProcess {
    @DI.Inject(TYPES.Dependencies.Logger)
    private _logger: Dependencies.Logger;

    @DI.Inject(TYPES.System.PersistedDataStore)
    private _dataStore: System.DataStore;

    private _Containers: ContainerData[];
    private _ContainerNames: string[];

    public async setWaferToContainer(containerName: string, loadPortPosition: number, slot: number,
        equipmentWaferId: string, materialWaferId: any, parentMaterialName: string): Promise<WaferData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }

        let container: ContainerData = await this.getContainer(containerName, loadPortPosition);

        if (!container) {
            container = await this.setContainer(containerName, loadPortPosition, null)
        }

        const wafer = await this.getWafer(container, slot, equipmentWaferId, materialWaferId);

        if (wafer) {
            this._logger.error("");
            return wafer;
        }

        wafer.Slot = slot;
        wafer.EquipmentWaferId = equipmentWaferId;
        wafer.MaterialWaferId = materialWaferId;

        if (parentMaterialName) {
            wafer.ParentMaterialName = parentMaterialName;
        }

        container.Slots.push(this.validateWafer(wafer));

        this.storeContainer(container);

        return wafer;
    }

    public async setWaferDataToContainerData(container: ContainerData, wafer: WaferData): Promise<WaferData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }

        const waferExistsBySlot = await this.getWafer(container, wafer.Slot, null, null);
        const waferExistsByEquipmentWaferId = await this.getWafer(container, null, wafer.EquipmentWaferId, null);

        if (waferExistsBySlot && waferExistsByEquipmentWaferId && waferExistsBySlot.Slot !== waferExistsByEquipmentWaferId.Slot) {
            this._logger.error(`Wafer Slot mismatch Wafer - on slot ${waferExistsBySlot.Slot} EquipmentWaferId is ${waferExistsBySlot.EquipmentWaferId}
             and not ${waferExistsByEquipmentWaferId.EquipmentWaferId} which is on slot ${waferExistsByEquipmentWaferId.Slot}`);
            return waferExistsBySlot;
        }

        if (waferExistsBySlot || waferExistsByEquipmentWaferId) {
            this.updateWaferDataOnContainer(container, wafer);
            return wafer;
        }

        wafer = this.validateWafer(wafer);

        container.Slots.push(wafer);

        this.storeContainer(container);

        return wafer;
    }

    public async updateWaferDataOnContainer(container: ContainerData, wafer: WaferData): Promise<WaferData> {

        const index = this.getSlotIndex(container, wafer);

        if (index === undefined) {
            throw new Error("Wafer not on container slots");
        }

        container = this.updateContainerSlots(container, wafer, index);

        this.storeContainer(container);

        return wafer;
    }

    public async updateWaferOnContainer(containerName: string, loadPortPosition: number,
        slot: number, equipmentWaferId: string, materialWaferId: string, parentMaterialName: string): Promise<WaferData> {
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

        if (wafer.Slot !== slot && slot !== undefined) {
            wafer.Slot = slot;
        }

        if (wafer.EquipmentWaferId !== equipmentWaferId && equipmentWaferId !== undefined) {
            wafer.EquipmentWaferId = equipmentWaferId;
        }

        if (wafer.MaterialWaferId !== materialWaferId && materialWaferId !== undefined) {
            wafer.MaterialWaferId = materialWaferId;
        }

        if (parentMaterialName !== undefined && wafer.ParentMaterialName !== parentMaterialName) {
            wafer.ParentMaterialName = parentMaterialName;
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
        const waferToDelete = await this.getWafer(sourceContainer, sourceWafer.Slot, sourceWafer.EquipmentWaferId, sourceWafer.MaterialWaferId);

        if (!waferToDelete) {
            this._logger.error(`Source Container ${sourceContainer.ContainerName} dos not contain source wafer ${sourceWafer.MaterialWaferId} (slot ${sourceWafer.Slot}, equipment id ${sourceWafer.EquipmentWaferId})`)
        }

        const waferToStore: WaferData = {
            Slot: targetSlot,
            MaterialWaferId: waferToDelete.MaterialWaferId,
            EquipmentWaferId: waferToDelete.EquipmentWaferId,
            CreatedOn: waferToDelete.CreatedOn,
            ModifiedOn: waferToDelete.ModifiedOn,
            ParentMaterialName: waferToDelete.ParentMaterialName
        }

        targetContainer.Slots.push(waferToStore);
        this.storeContainer(targetContainer);
        this.deleteWafer(sourceContainer, waferToDelete);

        return targetContainer;
    }

    public async getWafer(container: ContainerData, slot: number, equipmentWaferId: string, materialWaferId: string): Promise<WaferData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }

        let wafer: WaferData = null;

        if (slot !== undefined) {
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

        let container: ContainerData = null;
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

        return this._Containers.find(c =>
            c.ContainerName === containerName &&
            c.LoadPortPosition === loadPortPosition.toString());
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

        let container: ContainerData;
        container = {} as ContainerData;

        container.ContainerName = containerName;
        container.SlotMap = slotMap;
        container.LoadPortPosition = loadPortPosition.toString();
        container.Slots = [];
        container.CreatedOn = moment().utc().valueOf().toString();

        await this.storeContainer(container);

        return container;
    }

    public async updateContainer(containerName: string,
        loadPortPosition: number,
        slotMap: object,
        slots: WaferData[] = []): Promise<ContainerData> {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }

        let container: ContainerData;
        container = await this.getContainer(containerName, loadPortPosition);

        if (containerName) {
            container.ContainerName = containerName;
        }
        if (slotMap) {
            container.SlotMap = slotMap;
        }
        if (slots && Array.isArray(slots)) {
            container.Slots = slots as WaferData[];
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
        const containerNameStored = this._ContainerNames.find(c => c === containerName);

        if (!containerNameStored) {
            this._ContainerNames.push(containerName);

            await this._dataStore.store("ContainersOnPersistence",
                this._ContainerNames,
                System.DataStoreLocation.Persistent);

            this._Containers.push(container);
        }

        return await this._dataStore.store(containerName,
            container,
            System.DataStoreLocation.Persistent);
    }
    public async deleteContainer(carrier: ContainerData) {
        if (this._Containers === undefined) {
            await this.InitializePersistedData();
        }
        const containerIdentifier: string = `Carrier_${carrier.CreatedOn}_LoadPort_${carrier.LoadPortPosition}`;
        const containerNameStored = this._ContainerNames.find(c => c === containerIdentifier);

        if (containerNameStored) {
            this._ContainerNames.splice(this._ContainerNames.indexOf(containerIdentifier), 1);

            await this._dataStore.store("ContainersOnPersistence",
                this._ContainerNames,
                System.DataStoreLocation.Persistent);
        }

        this._Containers.splice(this._Containers.indexOf(carrier), 1);

        await this._dataStore.store(containerNameStored,
            undefined,
            System.DataStoreLocation.Persistent);
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

    public validateWafer(wafer: WaferData): WaferData {
        if (wafer.Slot !== undefined) {
            wafer.Slot = null;
        }
        if (!wafer.EquipmentWaferId) {
            wafer.EquipmentWaferId = null;
        }
        if (!wafer.MaterialWaferId) {
            wafer.MaterialWaferId = null;
        }
        if (!wafer.ParentMaterialName) {
            wafer.ParentMaterialName = null;
        }
        if (!wafer.CreatedOn) {
            wafer.CreatedOn = moment().utc().valueOf().toString();
        }
        if (!wafer.ModifiedOn) {
            wafer.ModifiedOn = wafer.CreatedOn;
        }
        return wafer;
    }

    public getSlotIndex(Container: ContainerData, wafer: WaferData): number {
        let index = Container.Slots.map(x => x.Slot)?.indexOf(wafer.Slot);

        if (index === undefined || index === -1) {
            index = Container.Slots.map(x => x.EquipmentWaferId)?.indexOf(wafer.EquipmentWaferId);
        }

        if (index === undefined || index === -1) {
            index = Container.Slots.map(x => x.MaterialWaferId)?.indexOf(wafer.MaterialWaferId);
        }

        if (index === undefined || index === -1) {
            return undefined;
        }

        return index;
    }

    public updateContainerSlots(Container: ContainerData, wafer: WaferData, index: number): ContainerData {
        const containerWafer = Container.Slots[index];

        if (wafer.EquipmentWaferId) {
            containerWafer.EquipmentWaferId = wafer.EquipmentWaferId;
        }
        if (wafer.MaterialWaferId) {
            containerWafer.MaterialWaferId = wafer.MaterialWaferId;
        }
        if (wafer.ParentMaterialName) {
            containerWafer.ParentMaterialName = wafer.ParentMaterialName;
        }
        if (wafer.Slot !== undefined) {
            containerWafer.Slot = wafer.Slot;
        }
        if (wafer.CreatedOn) {
            containerWafer.CreatedOn = wafer.CreatedOn;
        }

        Container.Slots[index] = containerWafer


        return Container;
    }
}
