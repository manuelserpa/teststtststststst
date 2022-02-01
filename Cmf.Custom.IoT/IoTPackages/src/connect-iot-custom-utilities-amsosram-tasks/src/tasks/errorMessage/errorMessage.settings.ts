/** Core */
import { Module, Component } from "cmf.core/src/core";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, WorkflowModel } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";
import * as ErrorMessageTask from "./errorMessage.task";

/** Angular */
import * as ng from "@angular/core";

/** Nested components */
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PopOverInfoModule } from "cmf.core.controls/src/components/popOverInfo/popOverInfo";
import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";
import { TaskSettingsTabPropertiesModule } from "cmf.core.connect.iot/src/components/taskSettingsTabProperties/taskSettingsTabProperties";

/** i18n */
import i18n from "./i18n/errorMessage.settings.default";
import { Input } from "@criticalmanufacturing/connect-iot-controller-engine/src/system/systemProxy";
import { CustomErrorCodeEnum } from "../../utilities/customErrorCodeEnum";

export interface ErrorMessageTaskSettings extends ErrorMessageTask.ErrorMessageSettings, WorkflowModel.TaskDefinitionSettings {}

/**
 *
 */

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-errorMessage-settings",
    templateUrl: "errorMessage.settings.html",
    assign: {
        i18n: i18n,
        LogMode: ErrorMessageTask.LogMode,
        ErrorCodeEnum: CustomErrorCodeEnum
    }
})
export class ErrorMessageSettings extends TaskSettingsBase implements ng.OnInit {

    //#region Private properties

    private _inputsChange: Input[]

    //#endregion

    //#region Public properties

    /**
     * Task settings
     */
    public settings: ErrorMessageTaskSettings;

    //#endregion

    /**
     * Constructor
     */
    constructor(private _elementRef: ng.ElementRef, viewContainerRef: ng.ViewContainerRef, service: TaskSettingsService) {
        super(viewContainerRef, service);
        service.onBeforeSave = this.onBeforeSave.bind(this);
    }

    /** Triggered when the task is created and define the default values */
    public ngOnInit(): void {
        const currentSettings = Object.assign({}, this.settings);
        Object.assign(this.settings, ErrorMessageTask.SETTINGS_DEFAULTS, currentSettings);
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
     * Handles when the settings inputs change.
     * Dispatch event to parent component.
     */
    public onInputsChanged(inputs: Input[]): void {
        this._inputsChange = inputs;
    }

    /**
     * Triggered before saving settings
     * Copies properties data from the _propertiesChange to the task settings
     * @param settings Settings
     */
    public onBeforeSave(settings: ErrorMessageTask.ErrorMessageSettings): ErrorMessageTask.ErrorMessageSettings {
        if (this._inputsChange != null) {
            settings.inputs = this._inputsChange;
        }
        return settings;
    }
}

/** Module */
@Module({
    imports: [
        BaseWidgetModule,
        PopOverInfoModule,
        PropertyContainerModule,
        PropertyEditorModule,
        TaskSettingsModule,
        TaskSettingsTabPropertiesModule
    ],
    declarations: [ErrorMessageSettings],
    defaultRoute: ErrorMessageSettings
})
export class ErrorMessageSettingsModule { }
