import { CustomEquipmentStateEnum } from "../../utilities/customEquipmentStateEnum";

export interface EquipmentStateModelData {
   Name: string,
   LoadPortNumber: number,
   ProcessSubResourceNumber: number,
   State: CustomEquipmentStateEnum,
   PreviousState: CustomEquipmentStateEnum,
   ModifiedOn: String,
   PreviousModification: String,
   SubResourcesStates: EquipmentStateModelData[]
}


