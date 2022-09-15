# DEE Actions

## Custom DEE Actions

The following DEE Actions were created to support customer requirements.

| DEE Action                     | Description       |
| ------                    | ------            |
| [CustomAssociateSorterJobDefinitionToContextTable](/AMSOsram/techspec>artifacts>deeactions>CustomAssociateSorterJobDefinitionToContextTable) | DEE action to create CustomSorterJobDefinition and associate to the context on CustomSorterJobDefinitionContext smart table. |
| [CustomAutomationAdjustLoadPortState](/AMSOsram/techspec>artifacts>deeactions>CustomAutomationAdjustLoadPortState) | This DEE Action is triggered by IoT Automation in order to adjust the state of a Load Port. |
| [CustomAutomationGetRecipeBody](/AMSOsram/techspec>artifacts>deeactions>CustomAutomationGetRecipeBody) | This DEE is responsible for validating the Recipe and return the RecipeBody, the RecipeNameOnEquipment and RecipeName of a given Recipe. |
| [Custom Create Goods Issue Message](/AMSOsram/techspec>artifacts>deeactions>CustomCreateGoodsIssueMessage) | DEE Action to create an Integration Entry with Goods Issue information. |
| [Custom Generate Production Lot Names](/AMSOsram/techspec>artifacts>deeactions>CustomGenerateProductionLotNames) | DEE Action used to generate new Lot names. |
| [CustomGenerateSorterJobDefinitionFromFutureAction](/AMSOsram/techspec>artifacts>deeactions>CustomGenerateSorterJobDefinitionFromFutureAction) | Dee action to Generate a Custom Sorter Job Definition if exists a Required Future Action for a given material. |
| [Custom Generate Split Lot Names](/AMSOsram/techspec>artifacts>deeactions>CustomGenerateSplitLotNames) | Dee Action used to generate splited Materials name. |
| [CustomGetTibcoConfigurations](/AMSOsram/techspec>artifacts>deeactions>CustomGetTibcoConfigurations) | DEE action that retrieves the required configurations from MES to allow connecting to TibcoEMS. |
| [Custom Import Production Orders From ERP](/AMSOsram/techspec>artifacts>deeactions>CustomImportProductionOrdersFromERP) | DEE action to receive a list of **Production Orders** and create a Integration Entry per **Production Order**. |
| [CustomIncomingMaterialLotCreation](/AMSOsram/techspec>artifacts>deeactions>CustomIncomingMaterialLotCreation) | DEE action to create or update (on hold) lot incoming from ERP. |
| [CustomInvalidateCache](/AMSOsram/techspec>artifacts>deeactions>CustomInvalidateCache) | DEE action to publish an **Invalidate Cache** message to Message Bus when Generic Table CustomTibcoEMSGatewayResolver data is changed.. |
| [Custom Nice Label Print](/AMSOsram/techspec>artifacts>deeactions>CustomNiceLabelPrint) | DEE Action to be triggered on material track out to send retrive and send information for the nice label printing. |
| [Custom Process Product](/AMSOsram/techspec>artifacts>deeactions>CustomProcessProduct) | Action used to create or update Product using body message of an Integration Entry. |
| [CustomProcessProductionOrder](/AMSOsram/techspec>artifacts>deeactions>CustomProcessProductionOrder) | DEE action to receive a xml message with the needed information to create or update a **Production Order**. |
| [Custom Process Products From ERP](/AMSOsram/techspec>artifacts>deeactions>CustomProcessProductsFromERP) | Action used to create an Integration Entry per Product using ERP received message. |
| [CustomReportDataToFDC](/AMSOsram/techspec>artifacts>deeactions>CustomReportDataToFDC) | Dee action is triggered to create an integration entry with the material data to send to FDC. |
| [CustomReportEDCToSpaceHandler](/AMSOsram/techspec>artifacts>deeactions>CustomReportEDCToSpaceHandler) | DEE action to validate DataCollection and create a XML message to be sent to Space system. |
| [CustomSendAbortInformationToIoT](/AMSOsram/techspec>artifacts>deeactions>CustomSendAbortInformationToIoT) | DEE action to Trigger IoT call to send information about Aborted Materials. |
| [CustomSendFDCLotInfo](/AMSOsram/techspec>artifacts>deeactions>CustomSendFDCLotInfo) | DEE action to be triggered by the Integration Entry Handler to process Integration Entries and send Lot data to Onto FDC. |
| [CustomSendFDCWaferInfo](/AMSOsram/techspec>artifacts>deeactions>CustomSendFDCWaferInfo) | DEE action to be triggered by the Integration Entry Handler to process Integration Entries and send Lot data to Onto FDC. |
| [Custom Send Process Message](/AMSOsram/techspec>artifacts>deeactions>CustomSendProcessMessage) | Brief DEE Action description |
| [CustomSendTrackInInformationToIoT](/AMSOsram/techspec>artifacts>deeactions>CustomSendTrackInInformationToIoT) | DEE action to Trigger IoT call to send the Materials TrackIn related information. |
| [CustomSendTrackOutInformationToIoT](/AMSOsram/techspec>artifacts>deeactions>CustomSendTrackOutInformationToIoT) | DEE action to Trigger IoT call to send the Materials Track Out related information. |
| [CustomTerminateVendorContainer](/AMSOsram/techspec>artifacts>deeactions>CustomTerminateVendorContainer) | DEE Action used to terminate a Container from a specific type configured as a VendorContainerType. |
| [CustomUndockContainerValidation](/AMSOsram/techspec>artifacts>deeactions>CustomUndockContainerValidation) | DEE Action used to validate if a Container can be undocked based on its Type being or not configured as a VendorContainerType. |


