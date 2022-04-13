/** Core */
import { Module, Component } from "cmf.core/src/core";
import {
    TaskSettingsBase, TaskSettingsModule, TaskSettingsService, TaskSettings, WorkflowModel
} from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";

import * as CustomPropertyPollingTask from "./customPropertyPolling.task";
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";
import { EnumComboBoxModule } from "cmf.core.controls/src/components/enumComboBox/enumComboBox";
import { TypeSelectorModule } from "cmf.core.connect.iot/src/components/typeSelector/typeSelector";
import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";
import {
    ColumnViewModule,
    ColumnViewModel,
    ColumnViewSelectedArgs,
    ColumnViewAddArgs,
    ColumnViewRemoveArgs,
    ColumnViewActionArgs
} from "cmf.core.controls/src/components/columnView/columnView";
import { ValidatorModule, ValidatorModel, OnValidate, OnValidateArgs, ResultMessageType } from "cmf.core.controls/src/directives/validator/validator";
import { Utilities } from "@criticalmanufacturing/connect-iot-controller-engine";

/** Angular */
import * as ng from "@angular/core";

/** i18n */
import i18n from "./i18n/customPropertyPolling.settings.default";


/**
 * Get Equipment Properties Wizard Settings
 *
 * @whatItDoes
 *
 * Manages the outputs settings for the get equipment property.
 * The user can choose what type of outputs the task can have.
 *
 */

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-customPropertyPolling-settings",
    templateUrl: "customPropertyPolling.settings.html",
    assign: {
        i18n: i18n,
        OutputTypeEnum: CustomPropertyPollingTask.CustomPropertyPollingOutputType
    }
})
export class CustomPropertyPollingSettings extends TaskSettingsBase implements ng.OnChanges, OnValidate, ng.OnInit {

    //#region Private properties

    public _outputsModel: ColumnViewModel.ColumnViewModel;

    public _selectedOutput: ColumnViewModel.ColumnViewRowModel;

    public _automationProperties: WorkflowModel.System.LBOS.Cmf.Foundation.BusinessObjects.AutomationProperty[];

    public _allProperties: WorkflowModel.System.LBOS.Cmf.Foundation.BusinessObjects.AutomationProperty[];

    //#endregion

    /**
     * Settings object
     */
    public settings: CustomPropertyPollingTask.CustomPropertyPollingSettings;

    /**
     * Constructor
     */
    constructor(private _elementRef: ng.ElementRef, viewContainerRef: ng.ViewContainerRef, private service: TaskSettingsService) {
        super(viewContainerRef, service);

        service.onBeforeSave = this.onBeforeSave.bind(this);
    }

    //#region Private methods

    /**
     * Builds the column view with the model for each row
     * @param outputs Outputs array to inject model
     */
    private _buildColumnViewModel(outputs: CustomPropertyPollingTask.CustomPropertyPollingOutputSettings[]): void {
        // Reset the previous model
        this._outputsModel = {
            rootNode: {
                id: "root",
                name: i18n.NAME,
                value: i18n.TYPE,
                children: <ColumnViewModel.ColumnViewLeaf[]>outputs.map(this._buildColumnViewChildModel.bind(this))
            }
        };
    }

    /**
     * Builds the column leaf with given model
     * @param output Child output to inject model
     */
    private _buildColumnViewChildModel(output: CustomPropertyPollingTask.CustomPropertyPollingOutputSettings): ColumnViewModel.ColumnViewLeaf {
        return {
            id: this.framework.sandbox.util.uniqueId("output_"),
            name: output.property ? output.property.Name : i18n.NOT_SPECIFIED,
            value: output.outputType != null ? CustomPropertyPollingTask.CustomPropertyPollingOutputType[output.outputType] : "",
            canRemove: true,
            tag: output
        };
    }

    //#endregion Private methods

    //#region Public methods

    public ngOnInit(): void {
        // Selects the properties from the driver definition filtering by isReadable=true
        this._allProperties = this.service.workflow.getTaskDriverProperties(this.service.definition);
        this._automationProperties = this.settings?.allowNonReadable !== true ?
            this._allProperties.filter(prop => prop.IsReadable === true) : this._allProperties;

        if (this.settings && this.settings._outputs && this.settings._outputs.length > 0) {
            this.settings._outputs.forEach((prop, index) => {
                const name: string = prop.property.Name;
                this.settings._outputs[index].property = <any>this._automationProperties.find(p => p.Name === name);
                if (this.settings._outputs[index].property == null) {
                    // In this scenario, we have a property that was once readable and no longer is. Let's at least try to show it
                    this.settings._outputs[index].property = <any>this._allProperties.find(p => p.Name === name);
                }
            });
        }
    }

    /**
     * Update the columnView model when settings change
     * @param changes Changes
     */
    public ngOnChanges(changes: ng.SimpleChanges): void {
        super.ngOnChanges(changes);

        if (changes.settings) {
            this._buildColumnViewModel(this.settings._outputs || []);
        }
    }

    /**
     * Validates the model on selection a row of column view.
     * @param args ColumnView selected arguments
     */
    public _onColumnViewSelect(args: ColumnViewSelectedArgs): void {
        this._selectedOutput = args.selectedRow;
        // Take the opportunity to validate
        // this._validateOutputs();
    }

    /**
     * Validates all outputs before save
     * @param context Context
     * @param model Model
     */
    public onValidate(context: OnValidateArgs, model?: ValidatorModel): Promise<boolean> {
        return model.validate(context).then(result => {
            // flag to check if there are repeated values
            let isRepeated = false;

            // Only validate further if ok until now
            if (result === true) {
                let validationResult: boolean = true;

                // Validate selected output
                const output = <CustomPropertyPollingTask.CustomPropertyPollingOutputSettings>this._selectedOutput.rootNode.tag;

                if (output?.property != null && output?.outputType != null) {
                    // Check if readable
                    if (output.property.IsReadable !== false || this.settings?.allowNonReadable === true) {
                        // Check if there are any repeated values in the columns view
                        isRepeated = (<ColumnViewModel.ColumnViewLeaf[]>this._outputsModel.rootNode.children).some(node => {
                            const outputToCompare = (<CustomPropertyPollingTask.CustomPropertyPollingOutputSettings>node.tag);
                            if (outputToCompare != null && outputToCompare.property != null && output.property && output.outputType) {
                                if (output !== outputToCompare) {
                                    return output.property.Id === outputToCompare.property.Id;
                                }
                            }
                        });

                        if (isRepeated) {
                            context.resultMessages.push({
                                message: i18n.REPEATED_OUTPUTS,
                                type: ResultMessageType.Error
                            });
                            validationResult = false;
                        }
                    } else {
                        context.resultMessages.push({
                            message: i18n.PROPERTY_NOT_READABLE,
                            type: ResultMessageType.Error
                        });
                        validationResult = false;
                    }
                } else {
                    context.resultMessages.push({
                        message: i18n.INVALID_OUTPUTS,
                        type: ResultMessageType.Error
                    });
                    validationResult = false;
                }
                return validationResult;
            }
        });
    }

    /**
     * Handles the click on "Add" button.
     * Adds an empty model to the columnView (output).
     * Selects the last node.
     * @param args ColumnView add arguments
     */
    public _onColumnViewAdd(args: ColumnViewAddArgs): void {
        const node: ColumnViewModel.ColumnViewNode = args.columnViewNode || this._outputsModel.rootNode;

        (<ColumnViewModel.ColumnViewLeaf[]>node.children).push(this._buildColumnViewChildModel({
            property: null,
            outputType: CustomPropertyPollingTask.CustomPropertyPollingOutputType.Value
        }));

        // Notify the columnView that a node was added
        args.add(true).then((args: ColumnViewActionArgs) => this._selectedOutput = args.selectedRow);
    }

    /**
     * Remove a node from the view.
     * Also removes the selection if it is selected.
     * @param args ColumnView remove arguments
     */
    public _onColumnViewRemove(args: ColumnViewRemoveArgs): void {
        let children: ColumnViewModel.ColumnViewLeaf[];

        if (args.selectedColumn && args.selectedColumn.rootNode) {
            children = (<ColumnViewModel.ColumnViewLeaf[]>args.selectedColumn.rootNode.children);
        } else {
            children = <ColumnViewModel.ColumnViewLeaf[]>this._outputsModel.rootNode.children;
        }

        const childIndex = children.findIndex(c => c.id === args.selectedRow.rootNode.id);

        if (childIndex >= 0) {
            // Check if this node is selected
            if (this._selectedOutput === args.selectedRow) {
                this._selectedOutput = null;
            }

            // Remove from container
            args.remove(true);
        }
    }

    /**
     * Stores the value at is destiny path
     * Handles a change in the current settings for a property
     * @param value New event
     * @param destPath Destination path
     */
    public _onOutputChange(value: any, destPath: string): void {
        this.framework.sandbox.util.setNestedPropertyByPath(this._selectedOutput.rootNode.tag, destPath, value, true);

        // TODO fix this later to match the property
        if (destPath === "property" && value && value.Name) {
            this._selectedOutput.rootNode.name = value.Name;
        }
        if (destPath === "outputType") {
            (<ColumnViewModel.ColumnViewLeaf>this._selectedOutput.rootNode).value = value == null
            ? "" : CustomPropertyPollingTask.CustomPropertyPollingOutputType[value];
        }
    }

    /**
     * Handles a change in the current settings for a property
     * @param value New setting
     * @param destPath Destination path
     */
    public _onSettingChange(value: any, destPath: string): void {
        // Set the settings with new value
        this.framework.sandbox.util.setNestedPropertyByPath(this.settings, destPath, value, true);
        if (destPath === "allowNonReadable") {
            this._automationProperties = this.settings?.allowNonReadable !== true ?
                this._allProperties.filter(prop => prop.IsReadable === true) : this._allProperties;
        }
    }

    /**
     * Process all nodes (default and outputs) name to match friendlyName
     * @param settings Settings
     */
    public onBeforeSave(settings: CustomPropertyPollingTask.CustomPropertyPollingSettings): CustomPropertyPollingTask.CustomPropertyPollingSettings {
        // outputs
        settings._outputs = (<ColumnViewModel.ColumnViewLeaf[]>this._outputsModel.rootNode.children).map(child => child.tag);

        // Strip down the extra data from the automation entities
        for (let i = 0; i < settings._outputs.length; i++) {
            settings._outputs[i].property = Utilities.stripAutomationEntity(settings._outputs[i].property, ["$type", "Name", "DataType"]);
        }

        return settings;
    }

    //#endregion
}

/** Module */
@Module({
    imports: [
        TaskSettingsModule,
        BaseWidgetModule,
        PropertyEditorModule,
        ValidatorModule,
        EnumComboBoxModule,
        ColumnViewModule,
        TypeSelectorModule,
        PropertyContainerModule
    ],
    declarations: [CustomPropertyPollingSettings],
    defaultRoute: CustomPropertyPollingSettings
})
export class CustomPropertyPollingSettingsModule {
}
