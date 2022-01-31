/** Core */
import { Module, Component } from "cmf.core/src/core";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, WorkflowModel } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";
import * as SetWaferToContainerTask from "./setWaferToContainer.task";

/** Angular */
import * as ng from "@angular/core";

/** Nested components */
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";
import { EnumComboBoxModule} from "cmf.core.controls/src/components/enumComboBox/enumComboBox";
import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";

/** i18n */
import i18n from "./i18n/setWaferToContainer.settings.default";

/** Constants */
export interface SetWaferToContainerTaskSettings extends SetWaferToContainerTask.SetWaferToContainerSettings, WorkflowModel.TaskDefinitionSettings {}

/**
 *
 */

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-setWaferToContainer-settings",
    templateUrl: "setWaferToContainer.settings.html",
    styleUrls: ["./setWaferToContainer.settings.css"],
    assign: {
        i18n: i18n
    }
})
export class SetWaferToContainerSettings extends TaskSettingsBase implements ng.OnInit, ng.OnChanges {

    //#region Public properties

    /**
     * Task settings
     */
    public settings: SetWaferToContainerTaskSettings;
    private _taskInstance: SetWaferToContainerTask.SetWaferToContainerTask;

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
        const currentSettings = Object.assign({}, this.settings);
        Object.assign(this.settings, SetWaferToContainerTask.SETTINGS_DEFAULTS, currentSettings);

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
    public onBeforeSave(settings: SetWaferToContainerTask.SetWaferToContainerSettings):
            SetWaferToContainerTask.SetWaferToContainerSettings {
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
        PropertyContainerModule
    ],
    declarations: [SetWaferToContainerSettings],
    defaultRoute: SetWaferToContainerSettings
})
export class SetWaferToContainerSettingsModule { }
