import { CustomSetupStatesEnum } from "../../utilities/setupStatesEnum";
import { DriverSetupDefinition } from "./customSetup";

export interface CustomSetupStore {
    /**
     * Retrieves the internal state
     * @returns The stored internal state
     */
    getInternalState(driverName: string): Promise<CustomSetupStatesEnum>;

    /**
     * Sets the internal state to a new value
     * @param newInternalState The new internal state to be stored
     */
    setInternalState(driverName: string, newInternalState: CustomSetupStatesEnum): Promise<void>;

    /**
     * Retrieves whether additional actions are being waited for
     * @returns A boolean specifying whether additional actions are being waited for
     */
    getWaitForAdditionalActions(driverName: string): Promise<boolean>;

    /**
     * Sets the new value of whether to wait for additional actions
     * @param value The new value of whether to wait for additional actions
     */
    setWaitForAdditionalActions(driverName: string, value: boolean): Promise<void>;

    /**
     * Stores a temporary key/value pair
     * @param key The name of the key whose value is to be set
     * @param value The value to be stored
     */
    setTempValue(key: string, prefix: string, value: any): Promise<void>;

    /**
     * Gets setup definition object for Driver
     * @param driverName Driver's Name
     */
    getSetupDefinition(driverName: string);

    /**
     * Gets setup definition object for Driver
     * @param driverName Driver's Name
     * @param setupDefinition Driver's Setup Definition
     */
    setSetupDefinition(driverName: string, setupDefinition: DriverSetupDefinition)

    /**
     * Loads all the existing Materials to memory
     */
    InitializePersistedData();
}
