/** Core */
import { Module, Component } from "cmf.core/src/core";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, WorkflowModel } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";
import * as SetObjectPropertyTask from "./setObjectProperty.task";

/** Angular */
import * as ng from "@angular/core";

/** Nested components */
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";
import { EnumComboBoxModule} from "cmf.core.controls/src/components/enumComboBox/enumComboBox";
import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";

/** i18n */
import i18n from "./i18n/setObjectProperty.settings.default";
import { TypeSelectorModule } from "cmf.core.connect.iot/src/components/typeSelector/typeSelector";
import { ColumnViewModule } from "cmf.core.controls/src/components/columnView/columnView";
import { ValidatorModule } from "cmf.core.controls/src/directives/validator/validator";
import { Inputs } from "./steps/inputs";

/** Constants */
export interface SetObjectPropertyTaskSettings extends SetObjectPropertyTask.SetObjectPropertySettings, WorkflowModel.TaskDefinitionSettings {}

/**
 *
 */

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-setObjectProperty-settings",
    templateUrl: "setObjectProperty.settings.html",
    styleUrls: ["./setObjectProperty.settings.css"],
    assign: {
        i18n: i18n
    }
})
export class SetObjectPropertySettings extends TaskSettingsBase implements ng.OnInit, ng.OnChanges {

    //#region Private properties
    @ng.ViewChild("inputs", {read: Inputs})
    private _inputsColumnView: Inputs;
    //#endregion


    //#region Public properties

    /**
     * Task settings
     */
    public settings: SetObjectPropertyTaskSettings;
    public _taskInstance: SetObjectPropertyTask.SetObjectPropertyTask;

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
            this.settings.failIfNotExists = this.settings.failIfNotExists != null ? this.settings.failIfNotExists : this._taskInstance.failIfNotExists;
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
    public onBeforeSave(settings: SetObjectPropertyTask.SetObjectPropertySettings): SetObjectPropertyTask.SetObjectPropertySettings {
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
    declarations: [SetObjectPropertySettings, Inputs],
    defaultRoute: SetObjectPropertySettings
})
export class SetObjectPropertySettingsModule { }
