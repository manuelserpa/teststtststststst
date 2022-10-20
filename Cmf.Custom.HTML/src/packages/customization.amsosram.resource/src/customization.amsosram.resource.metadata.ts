//#region Import statements

/** Core */
import { Framework, PackageMetadata } from "cmf.core/src/core";
import {
  ActionBarElementType,
  ActionButtonBuildContextHandler,
  ActionCanExecuteHandler,
  ActionComplexValueType,
  ActionMode,
  ActionValueType,
  SOURCE_COMPONENT,
  UI_PAGE_COMPONENT,
} from "cmf.core/src/domain/metadata/action";
import Cmf from "cmf.lbos";

//#endregion

//#region BuildContext

const onSendAdHocTransferInformationBuildContext: ActionButtonBuildContextHandler =
  (framework: Framework, context?: any): Promise<any> => {
    const sourceComponent = context[SOURCE_COMPONENT];

    if (sourceComponent) {
      let resource = new Cmf.Navigo.BusinessObjects.Resource();

      if (sourceComponent.source.component._entityType === "Resource") {
        resource = sourceComponent.source.component._parentComponent.epEntity;
      }

      if (sourceComponent.source.component._entityType === "UIPage") {
        resource = context[UI_PAGE_COMPONENT].source.ClusterProperties.Resource;
      }

      if (resource.Name) {
        context = {
          ...context,
          resource: resource,
          resourceId: resource.Id,
          resourceName: resource.Name,
        };
      }
    }

    return Promise.resolve(context);
  };

//#endregion

//#region CanExecute

const CustomSendAdHocTransferInformationCanExecute: ActionCanExecuteHandler =
  async (framework: Framework, context: any, messages: string[]): Promise<boolean> => {
    if (context.resource == null) {
      return false;
    }

    const input = new Cmf.Navigo.BusinessOrchestration.ResourceManagement.InputObjects.LoadResourceAttributesInput();
    input.Resource = context.resource;
    input.AttributeNames = ["IsSorter"];

    const output = await framework.sandbox.lbo.call(input) as
      Cmf.Navigo.BusinessOrchestration.ResourceManagement.OutputObjects.LoadResourceAttributesOutput;

    return (
      output.Resource.Attributes.has("IsSorter") &&
      output.Resource.Attributes.get("IsSorter") === true
    );
  };

//#endregion

function applyConfig(packageName: string) {
  const config: PackageMetadata = {
    version: "2.2.0",
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
      actionBars: [
        {
          id: "cmf.core.business.controls.entityPage.Resource",
          elementsToAdd: [
            {
              type: ActionBarElementType.ACTION_BUTTON,
              id: "Custom.Resource.SendAdHocTransferInformationButton",
            },
          ],
        },
        {
          id: "UIPage_16652929767335483",
          elementsToAdd: [
            {
              type: ActionBarElementType.ACTION_BUTTON,
              id: "Custom.Resource.SendAdHocTransferInformationButton"
            }
          ]
        }
      ],
      actionButtonGroups: [],
      actionButtons: [
        {
          id: "Custom.Resource.SendAdHocTransferInformationButton",
          actionId: "Custom.Resource.SendAdHocTransferInformation",
          title: "AdHoc Transfer Wafers",
          iconClass: "icon-core-st-lg-transfer",
          onBuildContext: onSendAdHocTransferInformationBuildContext,
        },
      ],
      actions: [
        {
          id: "Custom.Resource.SendAdHocTransferInformation",
          canExecute: CustomSendAdHocTransferInformationCanExecute,
          mode: ActionMode.ModalPage,
          route: "Entity/Resource/:id/CustomAdhocTransfWizard",
          inputs: {
            resourceId: <ActionComplexValueType>{
              type: ActionValueType.String,
            },
            resourceName: <ActionComplexValueType>{
              type: ActionValueType.String,
            },
            resource: <ActionComplexValueType>{
              type: ActionValueType.ReferenceType,
              referenceType: Cmf.Foundation.Common.ReferenceType.EntityType,
              referenceTypeName: "Resource",
            },
          },
        },
      ],
      menuGroups: [],
      menuItems: [],
      entityTypes: [],
      routes: [
        {
          routeConfig: [
            {
              path: "Entity/Resource/:id/CustomAdhocTransfWizard",
              loadChildren: `${packageName}/src/components/customWizardAdhocTransfer/customWizardAdhocTransfer#CustomWizardAdhocTransferModule`,
            },
          ],
        },
      ],
    },
  };
  return config;
}

export default applyConfig;
