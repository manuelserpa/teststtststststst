export default {
    SETTINGS: "Settings",
    SETTINGS_PRIMARY: "Primary",
    SETTINGS_SECONDARY: "Secondary",
    STREAM_FUNCTION_NAME: "Stream Function Name",
    STREAM_FUNCTION_NAME_HELP: "Stream Function Name used on message. Default stream function name for Get Formatted Recipe is 'S7F25'.",
    PRIMARY_REQUEST_MESSAGE: "Request Message",
    PRIMARY_REQUEST_MESSAGE_HELP: "Custom primary/request message instead of the default one (check help tab for more details)",
    RECIPE_NAME_PRIMARY_PATH: "Recipe Name item path location",
    // tslint:disable-next-line:max-line-length
    RECIPE_NAME_PRIMARY_PATH_HELP: "Item path location, on primary message, where RecipeName input should be inserted (check help tab for more details). If empty no substitution is performed and primary message is sent as is.",
    RECIPE_NAME_SECONDARY_PATH: "Secondary Recipe name path location",
    RECIPE_NAME_SECONDARY_PATH_HELP: "Item path location, on secondary message, where RecipeName output should be retrieved (check help tab for more details)",
    RECIPE_BODY_PATH: "Recipe Body Path",
    RECIPE_BODY_PATH_HELP: "Path location, on secondary message, where RecipeBody should be retrieved (check help tab for more details)",
    RECIPE_BODY_TYPE: "Recipe Body Type",
    RECIPE_BODY_TYPE_HELP: "Recipe Body on message content can be interpreted as a Buffer or as a String.",
};
