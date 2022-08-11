//#region Import statements


/** Core */
import { PackageMetadata, Framework } from "cmf.core/src/core";
import { MenuItem } from "cmf.core/src/domain/metadata/menu";

/** i18n */
import i18n from "./i18n/main.default";

declare var SystemJS: { import: any }

//#endregion

function applyConfig(packageName: string) {

    const config: PackageMetadata = {
        version: "1.11.0",
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
            menuGroups: [
                {
                    position: 2000,
                    id: "Shell.AMSOsram",
                    iconClass: "icon-docs-st-lg-userguide",
                    route: "AMSOsram",
                    itemsGenerator: class MenuGen {
                        public items(framework: Framework): Promise<MenuItem[]> {
                            return SystemJS.import("./node_modules/cmf.docs.area.AMSOsram/assets/__generatedMenuItems.json").then((jsonContent) => {
                                return jsonContent;
                            });
                        }
                    }.prototype,
                    title: `AMSOsram`,
                }],
            menuItems: [
                {
                    "id": "index",
                    "menuGroupId": "Shell.AMSOsram",
                    "title": "Index",
                    "actionId": ""
                },
                {
                    "id": "techspec",
                    "menuGroupId": "Shell.AMSOsram",
                    "title": "Technical Specification",
                    "actionId": ""
                },
                {
                    "id": "userguide",
                    "menuGroupId": "Shell.AMSOsram",
                    "title": "User Guide",
                    "actionId": ""
                },
                {
                    "id": "releasenotes",
                    "menuGroupId": "Shell.AMSOsram",
                    "title": "Release Notes",
                    "actionId": ""
                },
                {
                    "id": "faq",
                    "menuGroupId": "Shell.AMSOsram",
                    "title": "FAQ",
                    "actionId": ""
                }
            ],
            entityTypes: [],
            routes: [{
                routeConfig: []
                }]
        }
    };
    return config;
}

export default applyConfig;
