/** Core */
import { Module, Component } from "cmf.core/src/core";
import * as CustomLoadSetupConfigurationTask from "./customLoadSetupConfiguration.task";
import { System } from "@criticalmanufacturing/connect-iot-controller-engine";
import { TaskSettingsBase, TaskSettingsModule, TaskSettingsService, TaskSettings } from "cmf.core.connect.iot/src/components/taskSettings/taskSettings";

/** Angular */
import * as ng from "@angular/core";

/** i18n */
import i18n from "./i18n/customLoadSetupConfiguration.settings.default";

import {  ValidatorModule, ValidatorModel, OnValidate, OnValidateArgs, ResultMessageType } from "cmf.core.controls/src/directives/validator/validator";
import { ListViewModule, ListViewItem, ListView } from "cmf.core.controls/src/components/listView/listView";
import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/propertyEditor/propertyEditor";

/**
 *
 */
@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-customLoadSetupConfiguration-settings",
    templateUrl: "customLoadSetupConfiguration.settings.html",
    styleUrls: ['customLoadSetupConfiguration.settings.css'],
    assign: {
        i18n: i18n,
    }
})
export class CustomLoadSetupConfigurationSettings extends TaskSettingsBase implements ng.OnChanges, ng.OnInit {

    //#region Private Variables
    /**
     * The ListView element
     */
    @ng.ViewChild(ListView)
    protected _listView: ListView;
    /**
     * EntityType used
     */
    private _entityType: System.LBOS.Cmf.Foundation.BusinessObjects.EntityType = null;
    //#endregion

    // #region Public Variables

    /**
     * Task settings
     */
    public settings: CustomLoadSetupConfigurationTask.CustomLoadSetupConfigurationSettings;

    /**
     * Properties array
     */
    public propertiesArray: ListViewItem[] = [];

    /**
     * Load all attributes
     */
    public loadAllAttributes: boolean = false;
    // #endregion

    // #region Constructors
    constructor(private _elementRef: ng.ElementRef, viewContainerRef: ng.ViewContainerRef, private service: TaskSettingsService) {
        super(viewContainerRef, service);

        // Bind to the save event
        service.onBeforeSave = this.onBeforeSave.bind(this);
    }
    // #endregion

    // #region Public Methods
    // #endregion

    // #region Private & Internal Methods

    /**
     * Get EntityType according task metadata
     */
    private async getEntityType(): Promise<System.LBOS.Cmf.Foundation.BusinessObjects.EntityType> {
        const driverName = this.service.definition.driver;
        if (driverName == null) {
            this.settings.driverName = "";
            const controller = this.service.workflow["_engine"].automationController;
            const index = Object.keys(controller).findIndex(key => key === "_ObjectType");
            if (index !== -1) {
                // This comes from AutomationController GUI (its a known workaround)
                return controller["_ObjectType"];
            } else {
                return (<any>controller.ObjectType);
            }
        } else {
            this.settings.driverName = driverName;
            const drivers = await this.service.workflow.getDrivers();
            return <any>drivers.find(driver => driver.Name === driverName).AutomationDriverDefinition.ObjectType;
        }
    }

    /**
     * Get properties to show in list view
     */
    private getPropertiesForListView(): void {
        this.propertiesArray = this._entityType.Properties.filter(property => {
            return property.PropertyType === System.LBOS.Cmf.Foundation.BusinessObjects.EntityTypePropertyType.Attribute
                && !property.IsOperationAttribute;
        }).map(attribute => {
            return <ListViewItem>{
                checked: this.settings.attributes != null && this.settings.attributes.findIndex(property => property.Name === attribute.Name) !== -1,
                disabled: false,
                data: attribute
            }
        });
    }
    // #endregion

    // #region Event handling Methods
    public ngOnInit(): void {
        // Defaults
        const currentSettings = Object.assign({}, this.settings);
        Object.assign(this.settings, CustomLoadSetupConfigurationTask.SETTINGS_DEFAULTS, currentSettings);
    }

    /**
     * Update the columnView model when settings change
     * @param changes Changes
     */
    public async ngOnChanges(changes: ng.SimpleChanges): Promise<void> {
        super.ngOnChanges(changes);

        if (this.settings.levelsToLoad == null) {
            this.settings.levelsToLoad = 0;
        }
        if (this.settings.loadAllAttributes != null) {
            this.loadAllAttributes = this.settings.loadAllAttributes;
        }
        // Cannot assume that entitytype has not changed
        this._entityType = await this.getEntityType();
        this.settings.entityTypeName = this._entityType.Name;
        this.getPropertiesForListView();
    }

    /**
     * When selecting properties, update internal state
     */

    public onPropertiesSelected(selectedItems: ListViewItem[]): void {
        this.settings.attributes = this.propertiesArray
            .filter(item => item.checked)
            .map(item => {
                return {
                    Name: item.data.Name,
                    ScalarTypeName: item.data.ScalarType.Name.toLowerCase()
                }
        });
    }

    /**
     * When selecting the LoadAllAttributes option
     * Either Select/Unselect the items
     */
    public mapAllAttributes(flag: boolean): void {
        if (this._listView != null) {
            this._listView.selectAll(flag);
        }
    }

    /**
     * Process all nodes (default and outputs)
     * @param settings Settings
     */
    // tslint:disable-next-line: max-line-length
    public onBeforeSave(settings: CustomLoadSetupConfigurationTask.CustomLoadSetupConfigurationSettings): CustomLoadSetupConfigurationTask.CustomLoadSetupConfigurationSettings {
        settings.loadAllAttributes = this.loadAllAttributes;

        if (settings.reloadEveryChange == null) {
            settings.reloadEveryChange = false;
        }
        return settings;
    }
    // #endregion
}

@Module({
    imports: [
        TaskSettingsModule,
        ListViewModule,
        BaseWidgetModule,
        PropertyEditorModule
    ],
    declarations: [CustomLoadSetupConfigurationSettings],
    defaultRoute: CustomLoadSetupConfigurationSettings
})
export class CustomLoadSetupConfigurationSettingsModule { }
