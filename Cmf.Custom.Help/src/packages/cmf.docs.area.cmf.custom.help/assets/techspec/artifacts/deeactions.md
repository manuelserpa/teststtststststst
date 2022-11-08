# DEE Actions

## Custom DEE Actions

The following DEE Actions were created to support customer requirements.

| DEE Action                     | Description       |
| ------                    | ------            |
| [CustomMaterialAttributesOnTrackOut](/cmf.custom.help/techspec>artifacts>deeactions>custom_material_attributes_on_trackout) | DEE action responsible to set attributes on Material on TrackOut |
| [CustomReportEDCToSpaceHandler](/cmf.custom.help/techspec>artifacts>deeactions>custom_report_edc_to_space_handler) | DEE action to validate DataCollection and create a XML message to be sent to Space system. |
| [CustomSendAdHocTransferInformationToIoT](/cmf.custom.help/techspec>artifacts>deeactions>custom_send_adhoc_transfer_information_to_iot) | DEE Action responsible for sending AdHoc Transfer Information to IoT in order to process containers operations. |
| [CustomSendEventMessage](/cmf.custom.help/techspec>artifacts>deeactions>custom_send_event_message) | DEE Action used to publish Lot event messages to MessageBus based on Material action. E.g.: Material.TrackIn, Material.TrackOut, Material.MoveNext. |
| [CustomTibcoEMSReplyHandler](/cmf.custom.help/techspec>artifacts>deeactions>custom_tibco_ems_reply_handler) | DEE Action to handle the reply send from Tibco EMS. |
| [CustomValidateMaterialReceptionSubstrate](/cmf.custom.help/techspec>artifacts>deeactions>custom_validate_material_reception_substrate) | DEE action responsible for validate if the wafer is valid to proceed with the process. |
| [CustomAssociateSorterJobDefinitionToContextTable](/cmf.custom.help/techspec>artifacts>deeactions>customassociatesorterjobdefinitiontocontexttable) | DEE action to create CustomSorterJobDefinition and associate to the context on CustomSorterJobDefinitionContext smart table. |
| [CustomAutomationAdjustLoadPortState](/cmf.custom.help/techspec>artifacts>deeactions>customautomationadjustloadportstate) | This DEE Action is triggered by IoT Automation in order to adjust the state of a Load Port. |
| [CustomAutomationGetRecipeBody](/cmf.custom.help/techspec>artifacts>deeactions>customautomationgetrecipebody) | This DEE is responsible for validating the Recipe and return the RecipeBody, the RecipeNameOnEquipment and RecipeName of a given Recipe. |
| [Custom Create Goods Issue Message](/cmf.custom.help/techspec>artifacts>deeactions>customcreategoodsissuemessage) | DEE Action to create an Integration Entry with Goods Issue information. |
| [Custom Generate Production Lot Names](/cmf.custom.help/techspec>artifacts>deeactions>customgenerateproductionlotnames) | DEE Action used to generate new Lot names. |
| [CustomGenerateSorterJobDefinitionFromFutureAction](/cmf.custom.help/techspec>artifacts>deeactions>customgeneratesorterjobdefinitionfromfutureaction) | Dee action to Generate a Custom Sorter Job Definition if exists a Required Future Action for a given material. |
| [CustomGenerateSplitLotNames](/cmf.custom.help/techspec>artifacts>deeactions>customgeneratesplitlotnames) | Dee Action used to generate Materials name for split lots. |
| [CustomGetTibcoConfigurations](/cmf.custom.help/techspec>artifacts>deeactions>customgettibcoconfigurations) | DEE action that retrieves the required configurations from MES to allow connecting to TibcoEMS. |
| [Custom Import Production Orders From ERP](/cmf.custom.help/techspec>artifacts>deeactions>customimportproductionordersfromerp) | DEE action to receive a list of **Production Orders** and create a Integration Entry per **Production Order**. |
| [CustomIncomingMaterialLotCreation](/cmf.custom.help/techspec>artifacts>deeactions>customincomingmateriallotcreation) | DEE action to create or update (on hold) lot incoming from ERP. |
| [CustomInvalidateCache](/cmf.custom.help/techspec>artifacts>deeactions>custominvalidatecache) | DEE action to publish an **Invalidate Cache** message to Message Bus when Generic Table CustomTibcoEMSGatewayResolver data is changed.. |
| [Custom Nice Label Print](/cmf.custom.help/techspec>artifacts>deeactions>customnicelabelprint) | DEE Action to be triggered on material track out to send retrieve and send information for the nice label printing. |
| [Custom Process Product](/cmf.custom.help/techspec>artifacts>deeactions>customprocessproduct) | Action used to create or update Product using body message of an Integration Entry. |
| [CustomProcessProductionOrder](/cmf.custom.help/techspec>artifacts>deeactions>customprocessproductionorder) | DEE action to receive a xml message with the needed information to create or update a **Production Order**. |
| [Custom Process Products From ERP](/cmf.custom.help/techspec>artifacts>deeactions>customprocessproductsfromerp) | Action used to create an Integration Entry per Product using ERP received message. |
| [CustomReportDataToFDC](/cmf.custom.help/techspec>artifacts>deeactions>customreportdatatofdc) | Dee action is triggered to create an integration entry with the material data to send to FDC. |
| [CustomSendAbortInformationToIoT](/cmf.custom.help/techspec>artifacts>deeactions>customsendabortinformationtoiot) | DEE action to Trigger IoT call to send information about Aborted Materials. |
| [CustomSendFDCLotInfo](/cmf.custom.help/techspec>artifacts>deeactions>customsendfdclotinfo) | DEE action to be triggered by the Integration Entry Handler to process Integration Entries and send Lot data to Onto FDC. |
| [CustomSendFDCWaferInfo](/cmf.custom.help/techspec>artifacts>deeactions>customsendfdcwaferinfo) | DEE action to be triggered by the Integration Entry Handler to process Integration Entries and send Lot data to Onto FDC. |
| [CustomSendProcessMessage](/cmf.custom.help/techspec>artifacts>deeactions>customsendprocessmessage) | DEE action to be triggered by the Integration Entry Handler to process Integration Entries and send Inform Goods Issue message to ERP. |
| [CustomSendTrackInInformationToIoT](/cmf.custom.help/techspec>artifacts>deeactions>customsendtrackininformationtoiot) | DEE action to Trigger IoT call to send the Materials TrackIn related information. |
| [CustomSendTrackOutInformationToIoT](/cmf.custom.help/techspec>artifacts>deeactions>customsendtrackoutinformationtoiot) | DEE action to Trigger IoT call to send the Materials Track Out related information. |
| [CustomTerminateVendorContainer](/cmf.custom.help/techspec>artifacts>deeactions>customterminatevendorcontainer) | DEE Action used to terminate a Container from a specific type configured as a VendorContainerType. |
| [CustomUndockContainerValidation](/cmf.custom.help/techspec>artifacts>deeactions>customundockcontainervalidation) | DEE Action used to validate if a Container can be undocked based on its Type being or not configured as a VendorContainerType. |


