/** Core */
import { Module, Component } from "cmf.core/src/core";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, WorkflowModel } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";
import * as SlotMapValidatorTask from "./slotMapValidator.task";

/** Angular */
import * as ng from "@angular/core";

/** Nested components */
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";
import { EnumComboBoxModule} from "cmf.core.controls/src/components/enumComboBox/enumComboBox";
import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";

/** i18n */
import i18n from "./i18n/slotMapValidator.settings.default";

import { TypeSelectorModule } from "cmf.core.connect.iot/src/components/typeSelector/typeSelector";
import { ColumnViewModule } from "cmf.core.controls/src/components/columnView/columnView";
import { ValidatorModule } from "cmf.core.controls/src/directives/validator/validator";

/** Constants */
export interface SlotMapValidatorTaskSettings extends
SlotMapValidatorTask.SlotMapValidatorSettings, WorkflowModel.TaskDefinitionSettings {}

/**
 *
 */

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-SlotMapValidator-settings",
    templateUrl: "SlotMapValidator.settings.html",
    styleUrls: ["./SlotMapValidator.settings.css"],
    assign: {
        i18n: i18n
    }
})
export class SlotMapValidatorSettings extends TaskSettingsBase implements ng.OnInit, ng.OnChanges {

    //#region Public properties


    /**
     * Task settings
     */
    public settings: SlotMapValidatorTaskSettings;
    private _taskInstance: SlotMapValidatorTask.SlotMapValidatorTask;

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
            this.settings.inputs = (this.settings.inputs != null)
            ? this.settings.inputs
            : this._taskInstance.inputs;
            this.settings.outputs = (this.settings.outputs != null)
            ? this.settings.outputs
            : this._taskInstance.outputs;

            this.settings.emptySlot = this.settings.emptySlot != null ?
            this.settings.emptySlot : this._taskInstance.emptySlot;

            this.settings.occupiedSlot = this.settings.occupiedSlot != null ?
            this.settings.occupiedSlot : this._taskInstance.occupiedSlot;

            this.settings.fixedSize = this.settings.fixedSize != null ?
            this.settings.fixedSize : this._taskInstance.fixedSize;

            this.settings.size = this.settings.size != null ?
            this.settings.size : this._taskInstance.size;

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
    public onBeforeSave(settings: SlotMapValidatorTask.SlotMapValidatorSettings):
            SlotMapValidatorTask.SlotMapValidatorSettings {

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
    declarations: [SlotMapValidatorSettings],
    defaultRoute: SlotMapValidatorSettings
})
export class SlotMapValidatorSettingsModule { }
