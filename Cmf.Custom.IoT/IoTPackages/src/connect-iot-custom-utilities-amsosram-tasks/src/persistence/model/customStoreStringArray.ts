export interface CustomStoreStringArray {

    /**
     * Sets the internal state to a new value
     * @param newInternalState The new internal state to be stoered
     */
    setValue(array: String[]): Promise<void>;


    /**
     * Fetches the value of a stored key/value pair
     * @param key The name of key whose value is to be retrieved
     * @param defaultValue The default value to return if a key/value isn't found
     * @returns The value assigned to the specified key
     */
    getValue(): Promise<any>;
}
