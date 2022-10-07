//#region Import statements


/** MES */
import { PackageMetadata } from "cmf.mes/src/mes";


//#endregion

function applyConfig(packageName: string) {

    const config: PackageMetadata = {
        version: "",
        name: `${packageName}`,
        components: [
            // Below this line all components are attached automatically during build
            // inject:components
            // endinject:components
        ],
        directives: [
            // Below this line all directives are attached automatically during build
            // inject:directives
            // endinject:directives
        ],
        pipes: [
            // Below this line all pipes are attached automatically during build
            // inject:pipes
            // endinject:pipes
        ],
        i18n: [
            // Below this line all i18n are attached automatically during build
            // inject:i18n
            // endinject:i18n
        ],
        widgets: [
            // Below this line all widgets are attached automatically during build
            // inject:widgets
            // endinject:widgets
        ],
        dataSources: [
            // Below this line all dataSources are attached automatically during build
            // inject:dataSources
            // endinject:dataSources
        ],
        converters: [
            // Below this line all converters are attached automatically during build
            // inject:converters
            // endinject:converters
        ],
        metadataLoadedHandler: () => {
            // Place here the specific module loader configs to load the dependencies of this package
        },
        flex: {
        actionButtonGroups: [],
            actionButtons: [],
            actions: [],
            menuGroups: [],
            menuItems: [],
            entityTypes: [],
            routes: [{
                    routeConfig: [
                ]
                }]
        }
    };
    return config;
}

export default applyConfig;
