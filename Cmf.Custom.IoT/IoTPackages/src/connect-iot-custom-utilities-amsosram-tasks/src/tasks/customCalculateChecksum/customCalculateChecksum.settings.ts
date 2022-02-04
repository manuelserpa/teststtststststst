/** Core */
import { Module, Component } from "cmf.core/src/core";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, WorkflowModel } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";
import * as CustomCalculateChecksumTask from "./customCalculateChecksum.task";

/** Angular */
import * as ng from "@angular/core";

/** Nested components */
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";
import { EnumComboBoxModule} from "cmf.core.controls/src/components/enumComboBox/enumComboBox";
import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";
import { PopOverInfoModule } from "cmf.core.controls/src/components/popOverInfo/popOverInfo";

/** i18n */
import i18n from "./i18n/customCalculateChecksum.settings.default";

/** Constants */
export interface CustomCalculateChecksumTaskSettings extends CustomCalculateChecksumTask.CustomCalculateChecksumSettings,
WorkflowModel.TaskDefinitionSettings {}

/**
 *
 */

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-customCalculateChecksum-settings",
    templateUrl: "customCalculateChecksum.settings.html",
    assign: {
        i18n: i18n,
        HashFunctions: CustomCalculateChecksumTask.HashFunctions
    }
})
export class CustomCalculateChecksumSettings extends TaskSettingsBase implements ng.OnInit, ng.OnChanges {

    //#region Public properties

    /**
     * Task settings
     */
    public settings: CustomCalculateChecksumTaskSettings;

    /** helper to access main task (for defaults loading) */
    private _taskInstance: CustomCalculateChecksumTask.CustomCalculateChecksumTask;


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
            this.settings.hashFunctionSetting =  this.settings.hashFunctionSetting != null ?
            this.settings.hashFunctionSetting : this._taskInstance.hashFunctionSetting
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
    public onBeforeSave(settings: CustomCalculateChecksumTask.CustomCalculateChecksumSettings):
            CustomCalculateChecksumTask.CustomCalculateChecksumSettings {
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
        PopOverInfoModule
    ],
    declarations: [CustomCalculateChecksumSettings],
    defaultRoute: CustomCalculateChecksumSettings
})
export class CustomCalculateChecksumSettingsModule { }
