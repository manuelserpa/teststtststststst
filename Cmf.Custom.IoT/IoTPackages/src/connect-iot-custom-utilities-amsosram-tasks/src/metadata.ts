import { PackageMetadata, Metadata, ControllerEngineScope } from "@criticalmanufacturing/connect-iot-controller-engine";

declare var SystemJS;

export default <PackageMetadata><unknown>{
    name: "@criticalmanufacturing/connect-iot-custom-utilities-amsosram-tasks",
    friendlyName: "AMS Osram Custom Tasks",
    version: "1.0.0",
    tasks: [
        /** Persistence Tasks */
        // Material persistence tasks
        // Material Level
        "deleteMaterialOnPersistence",
        "retrieveMaterialProperty",
        "storeMaterialObject",
        "storeMaterialProperty",
        "updateMaterialState",
        // Sub Material Level
        "retrieveSubMaterialProperty",
        "updateSubMaterialState",
        // Slot Map Validations
        "slotMapValidator",
        // Container and Wafer Persistence tasks
        // Container Level
        "createContainer",
        "getContainer",
        "updateContainer",
        "deleteContainer",
        // Wafer Level
        "setWaferToContainer",
        "updateWaferOnContainer",
        "changeWaferContainer",
        "getWaferFromContainer",
        // Equipment State Model Persistence tasks
        "updateEquipmentState",
        "getEquipmentState",
        // Fifo buffer persistence tasks
        "storeSyncJob",
        "retrieveSyncJob",
        // generic tasks
        "setObjectProperty",
        /** Recipe Tasks */
        // SecsGem recipe Tasks
        "customDownloadRecipeToEquipment",
        "customGetFormattedRecipe",
        "customSetFormattedRecipe",
        // recipe logic related tasks
        "checkIfRecipeExistsOnEquipment",
        "customCalculateChecksum",
        "recipeStructureIterator",
        /** Custom Setup and Behavior Tasks */
        // Secs Gem Custom Setup
        "customSetup",
        // Setup Persistence Task
        "customLoadSetupConfiguration",
        // Ad-Hoc Equipment Request Tasks
        "customSendAdHocEquipmentRequest",
        /** Secs Gem Extension Tasks*/
        // secs gem generic action tasks
        "customMaterialTransferStatusRequest",
        "customCarrierActionRequest",
        // Secs-Gem Control and Process Job Tasks
        "customControlJobRequest",
        "customCreateProcessJob",
        "customMultiCreateProcessJob",
        "customCreateControlJob",
        "validateEquipmentStates",
        /** Error Handling */
        "errorMessage",
     /** Data Collection */
           // obtaining property value
          "customPropertyPolling",
          "customDataTrace",
        // Data Read and Writing Tasks
         "customReadId"
    ],
    converters: [
        "customArrayPositionValue",
        "customConvertStringToArrayConverter",
        "anyToErrorCodeConstant",
    ],
    async onLoad(platform: Metadata.Platform): Promise<void> {
        // If the tasks have dependencies that are required to run in the browser, describe them here
        // according to the following examples:
        if (platform === Metadata.Platform.Browser) {
            SystemJS.config({
                paths: {
                    "buffer": "node_modules/buffer",
                    "base64-js": "node_modules/base64-js",
                    "ieee754": "node_modules/ieee754",
                    "mathjs": "node_modules/mathjs/dist",
                    "xmldom": "node_modules/xmldom",
                    "xpath": "node_modules/xpath"
                },
                packages: {
                    "buffer": {
                        main: "index.js",
                        format: "cjs"
                    },
                    "base64-js": {
                        main: "base64js.min.js",
                        format: "cjs"
                    },
                    "ieee754": {
                        main: "index.js",
                        format: "cjs"
                    },
                    "mathjs": {
                        main: "math.min.js",
                        format: "cjs"
                    },
                    "xmldom": {
                        main: "lib/dom-parser.js",
                        format: "cjs"
                    },
                    "xpath": {
                        main: "xpath.js",
                        format: "cjs"
                    }
                }
            });
        }
    },
    mandatory: {
        scopes: [ControllerEngineScope.ConnectIoT]
    }
};
