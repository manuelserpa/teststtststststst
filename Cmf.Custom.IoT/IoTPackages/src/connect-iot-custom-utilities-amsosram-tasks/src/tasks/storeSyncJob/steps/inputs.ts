/** Core */
import { Module, Component, CoreComponent } from "cmf.core/src/core";
/** Angular */
import * as ng from "@angular/core";
import {
    ColumnViewModel,
    ColumnViewSelectedArgs,
    ColumnViewAddArgs,
    ColumnViewRemoveArgs,
    ColumnViewActionArgs
} from "cmf.core.controls/src/components/columnView/columnView";
import { ComplexValueType, PropertyValueType } from "cmf.core.connect.iot/src/stores/workflowModel";
/** i18n */
import i18n from "../i18n/storeSyncJob.settings.default";
import {  StoreSyncJobInputSettings} from "../storeSyncJob.task"
import {OnValidate, ValidatorModel, OnValidateArgs, ResultMessageType } from "cmf.core.controls/src/directives/validator/validator";

/**
 * Auxiliary component used in Settings.
 */
@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-storeSyncJob-settings-input",
    templateUrl: "inputs.html",
    styleUrls: ["./inputs.css"],
    inputs: [
        "data",
    ],
    outputs: [
        "dataChange"
    ],
    assign: {
        i18n: i18n
    }
})
export class Inputs extends CoreComponent implements ng.OnInit, ng.OnChanges {

    //#region Public variables

    /* ColumnView model*/
    public _model: ColumnViewModel.ColumnViewModel;

    /* Current selected leaf */
    public _selected: ColumnViewModel.ColumnViewRowModel;

    /* Input parameter data.
     * Used to build the columnView model.*/
    public data: StoreSyncJobInputSettings[] = null;

    /* On Data Change emitter.*/
    public dataChange: ng.EventEmitter<StoreSyncJobInputSettings[]> =
    new ng.EventEmitter<StoreSyncJobInputSettings[]>();

    //#endregion Public variables

    /* Constructor */
    constructor(private _elementRef: ng.ElementRef, viewContainerRef: ng.ViewContainerRef) {
        super(viewContainerRef);
    }

    //#region Private methods
    private buildColumnViewModel(data: StoreSyncJobInputSettings[]): void {
        this._model = {
            rootNode: {
                id: "root",
                name: "links",
                children: <ColumnViewModel.ColumnViewLeaf[]>data.map(this.buildColumnViewChildModel.bind(this))
            }
        }
    }

    private buildColumnViewChildModel(data: StoreSyncJobInputSettings): ColumnViewModel.ColumnViewLeaf {
        return {
            id: this.framework.sandbox.util.uniqueId("link"),
            name: data.name ? data.name :  "",
            value: null,
            canRemove: true,
            tag: data
        }
    }

    private emitLinks(): void {
        this.validateStructure(null).then((isValid) =>  {
            if (isValid && this._model && this._model.rootNode.children) {
                this.dataChange.emit(this.getLinks());
            }
        });
    }

    private validateRow(context: OnValidateArgs, data: StoreSyncJobInputSettings): boolean {
        if (data.name != null && data.name.length > 0 && data.valueType != null) {
            return true;
        } else {
            if (context) {
                context.resultMessages.push({
                    type: ResultMessageType.Error,
                    message: i18n.INVALID_INPUT
                })
            }
            return false;
        }
    }

    private async validateStructure(context: OnValidateArgs): Promise<boolean> {
        const links = this.getLinks();
        return !links.some(link => this.validateRow(context, link) === false);
    }

    //#endregion Private methods

    //#region Public methods

    /**
     * Setup titles on initialization
     */
    public ngOnInit(): void {

    }
    /**
     * On changes hook, build columnView model
     */
    public ngOnChanges(changes: ng.SimpleChanges): void {
        if (changes.data) {
            this.buildColumnViewModel(this.data || []);
        }
    }

    /**
     * Setup new row on adding
     */
    public onColumnViewAdd(args: ColumnViewAddArgs): void {
            const node: ColumnViewModel.ColumnViewNode = args.columnViewNode || this._model.rootNode;

            (<ColumnViewModel.ColumnViewLeaf[]>node.children).push(this.buildColumnViewChildModel({
                name: null,
                defaultValue: undefined,
                valueType: null
            }));
            args.add(true).then((args: ColumnViewActionArgs) => {
                this._selected = args.selectedRow;
                this.emitLinks();
            });

    }

    /**
     * Remove row logic
     */
    public onColumnViewRemove(args: ColumnViewRemoveArgs): void {
        let children: ColumnViewModel.ColumnViewLeaf[];

        if (args.selectedColumn && args.selectedColumn.rootNode) {
            children = (<ColumnViewModel.ColumnViewLeaf[]>args.selectedColumn.rootNode.children);
        } else {
            children = <ColumnViewModel.ColumnViewLeaf[]>this._model.rootNode.children;
        }

        const childIndex = children.findIndex(c => c.id === args.selectedRow.rootNode.id);

        if (childIndex >= 0) {
            // Check if this node is selected
            if (this._selected != null && this._selected.rootNode.id === args.selectedRow.rootNode.id) {
                this._selected = null;
            }

            // Remove from container
            args.remove(true);
            this.emitLinks();

        }
    }

    /**
     * On row selected, update internal logic
     */
    public onColumnViewSelect(args: ColumnViewSelectedArgs): void {
        this._selected = args.selectedRow;
    }

    /**
     * On updating leaf, update information in mode.
     */
    public onLeafChanged(value: any, destPath: string): void {
        this.framework.sandbox.util.setNestedPropertyByPath(this._selected.rootNode.tag, destPath, value, true);

        // Update the output name in the columnView
        if (destPath === "name") {
            this._selected.rootNode.name = value || "";
        }
        if (destPath === "valueType") {
            if ((<ComplexValueType>this._selected.rootNode.tag.valueType).type === PropertyValueType.Boolean) {
                this._selected.rootNode.tag.defaultValue = false;
            } else {
                this._selected.rootNode.tag.defaultValue = null;
            }
        }
        this.emitLinks();
    }

    /**
     * Get links from columnView model.
     */
    public getLinks(): StoreSyncJobInputSettings[] {
        return (<ColumnViewModel.ColumnViewLeaf[]>this._model.rootNode.children).map(leaf => leaf.tag);
    }

    /**
     * On validate event
     * @param context Validate arguments
     * @param model Validator model
     */
    public onValidate(context: OnValidateArgs): Promise<boolean> {

        if (context && context.context && context.context === "cmf-core-controls-columnView") {
            // ColumnView is validating the current selected row.
            // Validate only the row
            return Promise.resolve(this.validateRow(context, this._selected.rootNode.tag));
        } else {
            // User is trying to change to next step.
            // Validate all structure
            return Promise.resolve(this.validateStructure(context));
        }
    }

    //#endregion Public methods

}
