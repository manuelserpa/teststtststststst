/** Core */
import { Module, Component } from "cmf.core/src/core";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, WorkflowModel } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";
import * as CustomCreateProcessJobTask from "./customCreateProcessJob.task";
/** Angular */
import * as ng from "@angular/core";

/** Nested components */
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";
import { EnumComboBoxModule } from "cmf.core.controls/src/components/enumComboBox/enumComboBox";
import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";
import { PopOverInfoModule } from "cmf.core.controls/src/components/popOverInfo/popOverInfo";
import { ResultMessageBag, ResultMessageType } from "cmf.core.controls/src/components/resultMessage/resultMessageBag";
import { ResultMessageModule } from "cmf.core.controls/src/components/resultMessage/resultMessage";
import { Task, Dependencies, System, DI, TYPES } from "@criticalmanufacturing/connect-iot-controller-engine";

/** i18n */
import i18n from "./i18n/customCreateProcessJob.settings.default";
import i18nHelp from "../../common/i18n/common.default";
import { ComplexValueType } from "@criticalmanufacturing/connect-iot-controller-engine/src/system";
import { RecipeSpecificationType } from "./customCreateProcessJob.task";

/** Constants */
export interface CustomCreateProcessJobTaskSettings extends
    CustomCreateProcessJobTask.CustomCreateProcessJobSettings, WorkflowModel.TaskDefinitionSettings { }

/**
 *
 */

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-customCreateProcessJob-settings",
    templateUrl: "customCreateProcessJob.settings.html",
    assign: {
        i18n: i18n,
        RecipeSpecificationType: RecipeSpecificationType
    }
})
export class CustomCreateProcessJobSettings extends TaskSettingsBase implements ng.OnInit {

    /**
     * Task settings
     */
    public settings: CustomCreateProcessJobTaskSettings;

    /** helper to access main task (for defaults loading) */
    private _taskInstance: CustomCreateProcessJobTask.CustomCreateProcessJobTask;

    /**
     * Constructor
     */
    constructor(private _elementRef: ng.ElementRef, viewContainerRef: ng.ViewContainerRef, service: TaskSettingsService) {
        super(viewContainerRef, service);
    }

    /** Triggered when the task is created and define the default values */
    public ngOnInit(): void {

        if (this.task != null) {
            this._taskInstance = (<any>this.task)._taskInstance;
        }

        // Initialize default values for settings page
        if (this.settings) {
        }

    }


    /**
     * Handles a change in the settings.
     * Update the settings property with given value and path
     * @param value New value
     * @param destPath Destination property
     */
    public _onSettingsValueChange(value: any, destPath: string): void {
        this.framework.sandbox.util.setNestedPropertyByPath(this.settings, destPath, value, true);
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
        PopOverInfoModule,
        ResultMessageModule
    ],
    declarations: [CustomCreateProcessJobSettings],
    defaultRoute: CustomCreateProcessJobSettings
})

export class CustomCreateProcessJobSettingsModule { }
