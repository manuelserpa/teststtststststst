/** Core */
import { Module, Component } from "cmf.core/src/core";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, WorkflowModel } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";
import * as CustomCreateControlJobTask from "./customCreateControlJob.task";
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
import i18n from "./i18n/customCreateControlJob.settings.default";
import i18nHelp from "../../common/i18n/common.default";
import { ComplexValueType } from "@criticalmanufacturing/connect-iot-controller-engine/src/system";
import { RecipeSpecificationType } from "./customCreateControlJob.task";

/** Constants */
export interface CustomCreateControlJobTaskSettings extends
    CustomCreateControlJobTask.CustomCreateControlJobSettings, WorkflowModel.TaskDefinitionSettings { }

/**
 *
 */

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-customCreateControlJob-settings",
    templateUrl: "customCreateControlJob.settings.html",
    assign: {
        i18n: i18n,
        RecipeSpecificationType: RecipeSpecificationType
    }
})
export class CustomCreateControlJobSettings extends TaskSettingsBase implements ng.OnInit {

    /**
     * Task settings
     */
    public settings: CustomCreateControlJobTaskSettings;

    /** helper to access main task (for defaults loading) */
    private _taskInstance: CustomCreateControlJobTask.CustomCreateControlJobTask;

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
            this.settings.objectSpec = this.settings.objectSpec != null ?
            this.settings.objectSpec : this._taskInstance.objectSpec;
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
    declarations: [CustomCreateControlJobSettings],
    defaultRoute: CustomCreateControlJobSettings
})

export class CustomCreateControlJobSettingsModule { }
