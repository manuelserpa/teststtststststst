/** Core */
import { Module, Component } from "cmf.core/src/core";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, WorkflowModel } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";
import * as RetrieveSubMaterialPropertyTask from "./retrieveSubMaterialProperty.task";

/** Angular */
import * as ng from "@angular/core";

/** Nested components */
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";
import { EnumComboBoxModule} from "cmf.core.controls/src/components/enumComboBox/enumComboBox";
import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";

/** i18n */
import i18n from "./i18n/retrieveSubMaterialProperty.settings.default";

import { Outputs } from "./steps/outputs";
import { TypeSelectorModule } from "cmf.core.connect.iot/src/components/typeSelector/typeSelector";
import { ColumnViewModule } from "cmf.core.controls/src/components/columnView/columnView";
import { ValidatorModule } from "cmf.core.controls/src/directives/validator/validator";

import { MaterialStateEnum } from "../../persistence/model/materialData"
import { SubMaterialStateEnum } from "../../persistence/model/subMaterialData"
import { SlotOrderPickingDirectionEnum } from "../../utilities/slotOrderPickingDirectionEnum"

/** Constants */
export interface RetrieveSubMaterialPropertyTaskSettings extends
RetrieveSubMaterialPropertyTask.RetrieveSubMaterialPropertySettings, WorkflowModel.TaskDefinitionSettings {}

/**
 *
 */

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-retrieveSubMaterialProperty-settings",
    templateUrl: "retrieveSubMaterialProperty.settings.html",
    styleUrls: ["./retrieveSubMaterialProperty.settings.css"],
    assign: {
        i18n: i18n,
        MaterialStateEnum: MaterialStateEnum,
        SubMaterialStateEnum: SubMaterialStateEnum,
        SlotOrderPickingDirectionEnum: SlotOrderPickingDirectionEnum,
    }
})
export class RetrieveSubMaterialPropertySettings extends TaskSettingsBase implements ng.OnInit, ng.OnChanges {

    //#region Public properties

    @ng.ViewChild("outputs", {read: Outputs})
    private _outputsColumnView: Outputs;

    /**
     * Task settings
     */
    public settings: RetrieveSubMaterialPropertyTaskSettings;
    private _taskInstance: RetrieveSubMaterialPropertyTask.RetrieveSubMaterialPropertyTask;

    //#endregion

    /**
     * Constructor
     */
    constructor(private _elementRef: ng.ElementRef, viewContainerRef: ng.ViewContainerRef, private service: TaskSettingsService) {
        super(viewContainerRef, service);

        service.onBeforeSave = this.onBeforeSave.bind(this);
    }

    /** Triggered when the task is created and define the default values */
    public ngOnInit(): void {

        if (this.task != null) {
            this._taskInstance = (<any>this.task)._taskInstance;
        }

        // Initialize default values for settings page
        if (this.settings != null) {
            // ...
            // Example with defaults in the .task itself
            // this.settings.message = this.settings.message != null ? this.settings.message : this._taskInstance.message;
        }
    }

    /** Triggered when an element from the html page is changed */
    public ngOnChanges(changes: ng.SimpleChanges): void {
        super.ngOnChanges(changes);

    }

    /**
     * Handles a change in the settings.
     * Update the settings property with given value and path
     * @param value New value
     * @param destPath Destination property
     */
    public _onSettingsValueChange(value: any, destPath: string): void {
        // Set the settings with new value
        this.framework.sandbox.util.setNestedPropertyByPath(this.settings, destPath, value, true);
    }

    /**
     * Process all nodes (default and outputs)
     * @param settings Settings
     */
    public onBeforeSave(settings: RetrieveSubMaterialPropertyTask.RetrieveSubMaterialPropertySettings):
            RetrieveSubMaterialPropertyTask.RetrieveSubMaterialPropertySettings {

        if (this._outputsColumnView != null) {
            this.settings.outputs = this._outputsColumnView.getLinks();
        }
        return settings;
    }
}

/** Module */
@Module({
    imports: [
        TaskSettingsModule,
        PropertyEditorModule,
        EnumComboBoxModule,
        BaseWidgetModule,
        PropertyContainerModule,
        TypeSelectorModule,
        ColumnViewModule,
        ValidatorModule
    ],
    declarations: [RetrieveSubMaterialPropertySettings, Outputs],
    defaultRoute: RetrieveSubMaterialPropertySettings
})
export class RetrieveSubMaterialPropertySettingsModule { }
