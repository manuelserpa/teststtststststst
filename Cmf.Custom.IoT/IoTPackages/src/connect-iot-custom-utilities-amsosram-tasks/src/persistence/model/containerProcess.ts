import { ContainerData } from "./containerData";
import { WaferData } from "./waferData";

export interface ContainerProcess {

    //#region  Container Actions

    /**
     * Retrieves the Material object for a given MaterialId
     * @param id Material Id
     */
    getContainer(containerName: string, loadPortPosition: number);


    /**
     * Retrieves the Material object for a given MaterialId
     * @param id Material Id
     */
    getContainerByName(containerName: string);


    /**
     * Retrieves the Material object for a given MaterialId
     * @param id Material Id
     */
    getContainerByLoadPort(loadPortPosition: number);

    /**
     * Retrieves the Material object for a given MaterialId
     * @param id Material Id
     */
    setContainer(containerName: string, loadPortPosition: number, slotMap: object);

    /**
     * update container
     * @param carrier  Container
     */
    updateContainer(containerName: string, loadPortPosition: number, slotMap: object, slots: object, materialData: object)

    /**
     * Retrieves the Material object for a given Material slot
     * @param carrier Material slot
     */
    deleteContainer(carrier: ContainerData);

    //#endregion Container Actions

    /**
     * Method to create the Material persistence file
     * when a MO material is tracked in
     * @param material Material data
     */
    setWaferToContainer(containerName: string, loadPortPosition: number, slot: number, equipmentWaferId: string, materialWaferId);

    /**
    * Method to create the Material persistence file
    * when a MO material is tracked in
    * @param material Material data
    */
    setWaferDataToContainerData(container: ContainerData, wafer: WaferData);

    /**
     * Retrieves the Material state for a given MaterialID
     * @param MaterialName Material Name
     */
    updateWaferOnContainer(containerName: string, loadPortPosition: number,
        slot: number, equipmentWaferId: string, materialWaferId: string);

    /**
     * Updates the state for a given MaterialId
     * @param MaterialID Material Id
     * @param state state to set
     */
    changeWaferFromContainer(sourceContainer: ContainerData, sourceWafer: WaferData, targetContainer: ContainerData, targetSlot: number);

    /**
     * Retrieves the Material object for a given MaterialId
     * @param id Material Id
     */
    getWafer(container: ContainerData, slot: number, equipmentWaferId: string, materialWaferId: string);

    /**
     * Retrieves the Material object for a given MaterialId
     * @param id Material Id
     */
    getWaferBySlot(container: ContainerData, slot: number);

    /**
 * Retrieves the Material object for a given MaterialId
 * @param id Material Id
 */
    getWaferByEquipmentName(container: ContainerData, equipmentWaferId: string);

    /**
     * Retrieves the Material object for a given MaterialId
     * @param id Material Id
     */
    getWaferByMaterialName(container: ContainerData, materialWaferId: string);

    /**
     * Retrieves the Material object for a given Material slot
     * @param state Material slot
     */
    deleteWafer(container: ContainerData, wafer: WaferData);



    /**
     * Loads all the existing Materials to memory
     */
    InitializePersistedData();

}
