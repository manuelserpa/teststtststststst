/** Core */
import { Module, Component } from "cmf.core/src/core";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, WorkflowModel } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";
import * as CustomDownloadRecipeToEquipmentTask from "./customDownloadRecipeToEquipment.task";

/** Angular */
import * as ng from "@angular/core";

/** Nested components */
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";
import { EnumComboBoxModule} from "cmf.core.controls/src/components/enumComboBox/enumComboBox";
import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";
import { PopOverInfoModule } from "cmf.core.controls/src/components/popOverInfo/popOverInfo";
import {ResultMessageBag, ResultMessageType} from "cmf.core.controls/src/components/resultMessage/resultMessageBag";
import {ResultMessageModule} from "cmf.core.controls/src/components/resultMessage/resultMessage";

/** i18n */
import i18n from "./i18n/customDownloadRecipeToEquipment.settings.default";
import i18nHelp from "../../common/i18n/common.default";

/** Constants */
export interface CustomDownloadRecipeToEquipmentTaskSettings extends
CustomDownloadRecipeToEquipmentTask.CustomDownloadRecipeToEquipmentSettings, WorkflowModel.TaskDefinitionSettings {}

export const HELP_INFO: ResultMessageBag = {
    message: i18nHelp.HELP_DETAILS,
    type: ResultMessageType.Info
}
/**
 *
 */

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-customDownloadRecipeToEquipment-settings",
    templateUrl: "customDownloadRecipeToEquipment.settings.html",
    assign: {
        i18n: i18n,
        i18nHelp: i18nHelp,
        RecipeBodyType: CustomDownloadRecipeToEquipmentTask.RecipeBodyType,
        help: HELP_INFO
    }
})
export class CustomDownloadRecipeToEquipmentSettings extends TaskSettingsBase implements ng.OnInit {

    //#region Public properties

    /**
     * Task settings
     */
    public settings: CustomDownloadRecipeToEquipmentTaskSettings;

    //#endregion

    /** helper to access main task (for defaults loading) */
    private _taskInstance: CustomDownloadRecipeToEquipmentTask.CustomDownloadRecipeToEquipmentTask;


    /** primaryMessage as an object for property editor with valueType=Object */
    private _primaryMessage: any = undefined

    /** inquiryMessage as an object for property editor with valueType=Object */
    private _inquiryMessage: any = undefined

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
        if ( this.settings ) {

            if ( this._primaryMessage == null ) {
                if ( this.isJson(this.settings.primaryRequestMessage)) {
                    this._primaryMessage = JSON.parse(this.settings.primaryRequestMessage);
                } else {
                    this._primaryMessage = JSON.parse(this._taskInstance.primaryRequestMessage);
                };
            }
            this.settings.primaryRequestMessage = JSON.stringify(this._primaryMessage);

            if ( this._inquiryMessage == null ) {
                if ( this.isJson(this.settings.primaryInquiryRequestMessage)) {
                    this._inquiryMessage = JSON.parse(this.settings.primaryInquiryRequestMessage);
                } else {
                    this._inquiryMessage = JSON.parse(this._taskInstance.primaryInquiryRequestMessage);
                };
            }
            this.settings.primaryInquiryRequestMessage = JSON.stringify(this._inquiryMessage);

            this.settings.streamFunctionName = this.settings.streamFunctionName != null ?
                this.settings.streamFunctionName : this._taskInstance.streamFunctionName;
            this.settings.useS7F1Message = this.settings.useS7F1Message != null ?
                this.settings.useS7F1Message : this._taskInstance.useS7F1Message;
            this.settings.successCodesS7F1 = this.settings.successCodesS7F1 != null ?
                this.settings.successCodesS7F1 : this._taskInstance.successCodesS7F1;

            this.settings.recipeNameInquiryPrimaryPath = this.settings.recipeNameInquiryPrimaryPath != null ?
                this.settings.recipeNameInquiryPrimaryPath : this._taskInstance.recipeNameInquiryPrimaryPath;
            this.settings.recipeBodyLengthInquiryPrimaryPath = this.settings.recipeBodyLengthInquiryPrimaryPath != null ?
                this.settings.recipeBodyLengthInquiryPrimaryPath : this._taskInstance.recipeBodyLengthInquiryPrimaryPath;

            this.settings.recipeNamePrimaryPath = this.settings.recipeNamePrimaryPath != null ?
                this.settings.recipeNamePrimaryPath : this._taskInstance.recipeNamePrimaryPath;
            this.settings.recipeBodyPrimaryPath = this.settings.recipeBodyPrimaryPath != null ?
                this.settings.recipeBodyPrimaryPath : this._taskInstance.recipeBodyPrimaryPath;
            this.settings.recipeBodyType = this.settings.recipeBodyType != null ?
                this.settings.recipeBodyType : this._taskInstance.recipeBodyType;
            this.settings.replyPath = this.settings.replyPath != null ?
                this.settings.replyPath : this._taskInstance.replyPath;
            this.settings.successCodes = this.settings.successCodes != null ?
                this.settings.successCodes : this._taskInstance.successCodes;
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
            if (value) {
                this._primaryMessage = value;
                this.settings.primaryRequestMessage = JSON.stringify(this._primaryMessage)
            }
        } else if (destPath === "InquiryMessageValueChange") {
            if (value) {
                this._inquiryMessage = value;
                this.settings.primaryInquiryRequestMessage = JSON.stringify(this._inquiryMessage)
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
        EnumComboBoxModule,
        BaseWidgetModule,
        PropertyContainerModule,
        PopOverInfoModule,
        ResultMessageModule
    ],
    declarations: [CustomDownloadRecipeToEquipmentSettings],
    defaultRoute: CustomDownloadRecipeToEquipmentSettings
})
export class CustomDownloadRecipeToEquipmentSettingsModule { }
