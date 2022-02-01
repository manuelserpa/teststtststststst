export default {
    SETTINGS: "Settings",
    SETTINGS_PRIMARY: "Primary",
    SETTINGS_SECONDARY: "Secondary",
    STREAM_FUNCTION_NAME: "Stream Function Name",
    STREAM_FUNCTION_NAME_HELP: "Stream Function Name used on message. Default stream function name for Set Recipe is 'S7F3'.",
    USE_S7F1: "Use S7F1",
    // tslint:disable-next-line:max-line-length
    USE_S7F1_HELP: "Parameter to specify whether or not to require an inquiry for the S7F1 process program when sending the S7F3 process program from the host.",
    S7F1_SUCCESS_CODES: "S7F1 Success Codes",
    S7F1_SUCCESS_CODES_HELP: "Multiple values allowed using comma as separator ('0x01, 0x04, 0x05, 0x00'). If empty, any reply is a success.",
    PRIMARY_REQUEST_MESSAGE: "Request Message",
    // tslint:disable-next-line:max-line-length
    PRIMARY_REQUEST_MESSAGE_HELP: "Custom primary/request message instead of the default one (check help tab for more details)",
    RECIPE_NAME_PRIMARY_PATH: "Recipe Name item path location",
    // tslint:disable-next-line:max-line-length
    RECIPE_NAME_PRIMARY_PATH_HELP: "Item path location, on primary message, where RecipeName input should be inserted (check help tab for more details). If empty no substitution is performed and primary message is sent as is.",
    RECIPE_BODY_PRIMARY_PATH: "Recipe Body Path location",
    // tslint:disable-next-line:max-line-length
    RECIPE_BODY_PRIMARY_PATH_HELP: "Item path location, on primary message, where RecipeBody input should be inserted (check help tab for more details). If empty no substitution is performed and primary message is sent as is.",
    RECIPE_BODY_TYPE: "Recipe Body Type",
    RECIPE_BODY_TYPE_HELP: "Recipe Body on message content can be interpreted as a Buffer or as a String.",
    REPLY_PATH: "Reply Path",
    REPLY_PATH_HELP: "Location where the result code is located in the secondary/reply message (check help tab for more details)",
    SUCCESS_CODES: "Success Codes",
    SUCCESS_CODES_HELP: "Multiple values allowed using comma as separator ('0x01, 0x04, 0x05, 0x00'). If empty, any reply is a success.",
    PRIMARY_INQUIRY_MESSAGE: "Inquiry Message",
    RECIPE_NAME_INQUIRY_PRIMARY_PATH: "Recipe Name item path location on Recipe Inquiry",
    RECIPE_BODY__LENGTH_INQUIRY_PRIMARY_PATH: "Recipe Body Length Path location on Recipe Inquiry",
};
