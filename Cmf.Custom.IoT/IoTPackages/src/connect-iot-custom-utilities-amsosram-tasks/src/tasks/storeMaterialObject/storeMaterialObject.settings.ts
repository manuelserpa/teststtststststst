/** Core */
import { Module, Component } from "cmf.core/src/core";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, WorkflowModel } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";
import * as StoreMaterialObjectTask from "./storeMaterialObject.task";

/** Angular */
import * as ng from "@angular/core";

/** Nested components */
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";
import { EnumComboBoxModule} from "cmf.core.controls/src/components/enumComboBox/enumComboBox";
import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";

/** i18n */
import i18n from "./i18n/storeMaterialObject.settings.default";

/** Constants */
export interface StoreMaterialObjectTaskSettings extends StoreMaterialObjectTask.StoreMaterialObjectSettings, WorkflowModel.TaskDefinitionSettings {}

/**
 *
 */

@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-storeMaterialObject-settings",
    templateUrl: "storeMaterialObject.settings.html",
    styleUrls: ["./storeMaterialObject.settings.css"],
    assign: {
        i18n: i18n
    }
})
export class StoreMaterialObjectSettings extends TaskSettingsBase implements ng.OnInit, ng.OnChanges {

    //#region Public properties

    /**
     * Task settings
     */
    public settings: StoreMaterialObjectTaskSettings;
    private _taskInstance: StoreMaterialObjectTask.StoreMaterialObjectTask;

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
    }

    /** Triggered when an element from the html page is changed */
    public ngOnChanges(changes: ng.SimpleChanges): void {
        super.ngOnChanges(changes);

    }

    /**
     * Process all nodes (default and outputs)
     * @param settings Settings
     */
    public onBeforeSave(settings: StoreMaterialObjectTask.StoreMaterialObjectSettings):
            StoreMaterialObjectTask.StoreMaterialObjectSettings {
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
    declarations: [StoreMaterialObjectSettings],
    defaultRoute: StoreMaterialObjectSettings
})
export class StoreMaterialObjectSettingsModule { }
