/** Core */
import { Module, Component } from "cmf.core/src/core";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, WorkflowModel } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";
import * as CustomGetFormattedRecipeTask from "./customGetFormattedRecipe.task";

/** Angular */
import * as ng from "@angular/core";

/** Nested components */
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";
import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";
import { PopOverInfoModule } from "cmf.core.controls/src/components/popOverInfo/popOverInfo";
import {ResultMessageBag, ResultMessageType} from "cmf.core.controls/src/components/resultMessage/resultMessageBag";
import {ResultMessageModule} from "cmf.core.controls/src/components/resultMessage/resultMessage";

/** i18n */
import i18n from "./i18n/customGetFormattedRecipe.settings.default";
import i18nHelp from "../../common/i18n/common.default";

/** Constants */
export interface CustomGetFormattedRecipeTaskSettings
extends CustomGetFormattedRecipeTask.CustomGetFormattedRecipeSettings, WorkflowModel.TaskDefinitionSettings {}

export const HELP_INFO: ResultMessageBag = {
    message: i18nHelp.HELP_DETAILS,
    type: ResultMessageType.Info
}

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-customGetFormattedRecipe-settings",
    templateUrl: "customGetFormattedRecipe.settings.html",
    assign: {
        i18n: i18n,
        i18nHelp: i18nHelp,
        RecipeBodyType: CustomGetFormattedRecipeTask.RecipeBodyType,
        help: HELP_INFO
    }
})
export class CustomGetFormattedRecipeSettings extends TaskSettingsBase implements ng.OnInit {

    //#region Public properties

    /**
     * Task settings
     */
    public settings: CustomGetFormattedRecipeTaskSettings;

    //#endregion

    /**
     * Helper to access main task (for defaults loading)
     */
    private _taskInstance: CustomGetFormattedRecipeTask.CustomGetFormattedRecipeTask;

    /**
     * PrimaryMessage as an object for property editor with valueType=Object
     */
    private _primaryMessage: any = undefined;

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
            if (this._primaryMessage == null) {
                if (this.isJson(this.settings.primaryRequestMessage)) {
                    this._primaryMessage = JSON.parse(this.settings.primaryRequestMessage);
                } else {
                    this._primaryMessage = JSON.parse(this._taskInstance.primaryRequestMessage);
                };
            }
            this.settings.primaryRequestMessage = JSON.stringify(this._primaryMessage);

            this.settings.streamFunctionName = this.settings.streamFunctionName != null ?
                this.settings.streamFunctionName : this._taskInstance.streamFunctionName
            this.settings.recipeNamePrimaryPath = this.settings.recipeNamePrimaryPath != null ?
                this.settings.recipeNamePrimaryPath : this._taskInstance.recipeNamePrimaryPath
            this.settings.recipeNameSecondaryPath = this.settings.recipeNameSecondaryPath != null ?
                this.settings.recipeNameSecondaryPath :  this._taskInstance.recipeNameSecondaryPath
            this.settings.recipeBodyPath = this.settings.recipeBodyPath != null ?
                this.settings.recipeBodyPath : this._taskInstance.recipeBodyPath
            this.settings.recipeBodyType = this.settings.recipeBodyType != null ?
                this.settings.recipeBodyType : this._taskInstance.recipeBodyType
        }

    }

    /**
     * Checks if string is a parsable JSON
     * @param message string to test
     */
    private isJson(message: string): boolean {
        try {
            JSON.parse(message);
        } catch (e) {
            return false;
        }
        return true;
    }

    /**
     * Handles a change in the settings.
     * Update the settings property with given value and path
     * @param value New value
     * @param destPath Destination property
     */
    public _onSettingsValueChange(value: any, destPath: string): void {
        if (destPath === "PrimaryMessageValueChange") {
            if (value != null) {
                this._primaryMessage = value;
                this.settings.primaryRequestMessage = JSON.stringify(this._primaryMessage)
            }
        } else {
            // Set the settings with new value
            this.framework.sandbox.util.setNestedPropertyByPath(this.settings, destPath, value, true);
        }
    }

}

/** Module */
@Module({
    imports: [
        TaskSettingsModule,
        PropertyEditorModule,
        BaseWidgetModule,
        PropertyContainerModule,
        PopOverInfoModule,
        ResultMessageModule
    ],
    declarations: [CustomGetFormattedRecipeSettings],
    defaultRoute: CustomGetFormattedRecipeSettings
})
export class CustomGetFormattedRecipeSettingsModule { }
