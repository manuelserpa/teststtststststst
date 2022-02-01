import { EquipmentStateModelData } from "./equipmentStateModelData";
import { CustomEquipmentStateEnum } from "../../utilities/customEquipmentStateEnum";

export interface EquipmentStateModelProcess {

    /**
     * Stores state for main resource/load port
     * @param loadPort Load Port number
     * @param equipmentState Equipment State
     */
    updateEquipmentState(resourceName: string, loadPort: number, processSubResourceNumber: number, equipmentState: CustomEquipmentStateEnum);

      /**
     * Retrieves the Material state for a given MaterialID
     * @param loadPort Load Port number
     */
    getCurrentEquipmentState(resourceName: string, loadPort: number, processSubResourceNumber: number);

    /**
     * Loads all the existing States to memory
     */
    InitializePersistedData();

}
