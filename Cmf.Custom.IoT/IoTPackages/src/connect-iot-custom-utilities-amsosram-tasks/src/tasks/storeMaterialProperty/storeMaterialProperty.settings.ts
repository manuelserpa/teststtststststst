/** Core */
import { Module, Component } from "cmf.core/src/core";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, WorkflowModel } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";
import * as StoreMaterialPropertyTask from "./storeMaterialProperty.task";

/** Angular */
import * as ng from "@angular/core";

/** Nested components */
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";
import { EnumComboBoxModule} from "cmf.core.controls/src/components/enumComboBox/enumComboBox";
import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";

/** i18n */
import i18n from "./i18n/storeMaterialProperty.settings.default";

import { Inputs } from "./steps/inputs";
import { TypeSelectorModule } from "cmf.core.connect.iot/src/components/typeSelector/typeSelector";
import { ColumnViewModule } from "cmf.core.controls/src/components/columnView/columnView";
import { ValidatorModule } from "cmf.core.controls/src/directives/validator/validator";

/** Constants */
export interface StoreMaterialPropertyTaskSettings extends StoreMaterialPropertyTask.StoreMaterialPropertySettings, WorkflowModel.TaskDefinitionSettings {}

/**
 *
 */

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-storeMaterialProperty-settings",
    templateUrl: "storeMaterialProperty.settings.html",
    styleUrls: ["./storeMaterialProperty.settings.css"],
    assign: {
        i18n: i18n
    }
})
export class StoreMaterialPropertySettings extends TaskSettingsBase implements ng.OnInit, ng.OnChanges {

    //#region Public properties

    @ng.ViewChild("inputs", {read: Inputs})
    private _inputsColumnView: Inputs;

    /**
     * Task settings
     */
    public settings: StoreMaterialPropertyTaskSettings;
    private _taskInstance: StoreMaterialPropertyTask.StoreMaterialPropertyTask;

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

        if (this.settings != null) {
            this.settings.clearActivate = this.settings.clearActivate != null ? this.settings.clearActivate : this._taskInstance.clearActivate;
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
    }

    /**
     * Process all nodes (default and outputs)
     * @param settings Settings
     */
    public onBeforeSave(settings: StoreMaterialPropertyTask.StoreMaterialPropertySettings):
            StoreMaterialPropertyTask.StoreMaterialPropertySettings {

        if (this._inputsColumnView != null) {
            this.settings.inputs = this._inputsColumnView.getLinks();
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
    declarations: [StoreMaterialPropertySettings, Inputs],
    defaultRoute: StoreMaterialPropertySettings
})
export class StoreMaterialPropertySettingsModule { }
