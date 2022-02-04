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
/** i18n */
import i18n from "../i18n/retrieveMaterialProperty.settings.default";
import { RetrieveMaterialPropertyOutputSettings } from "../retrieveMaterialProperty.task";
import { OnValidate, ValidatorModel, OnValidateArgs, ResultMessageType } from "cmf.core.controls/src/directives/validator/validator";



// Used to register the mathjs custom language

/**
 * Auxiliary component used in Settings.
 */
@Component({
    moduleId: module.id,
    selector: "connect-iot-controller-engine-core-tasks-retrieveMaterialProperty-settings-output",
    templateUrl: "outputs.html",
    styleUrls: ["./outputs.css"],
    inputs: [
        "data",
    ],
    outputs: [
        "dataChange"
    ],
    assign: {
        i18n: i18n,

    }
})
export class Outputs extends CoreComponent implements ng.OnInit, ng.OnChanges, OnValidate {

    //#region Public variables

    /**
     * ColumnView model
     */
    public _model: ColumnViewModel.ColumnViewModel;

    /**
     * Current selected leaf
     */
    public _selected: ColumnViewModel.ColumnViewRowModel;

    /**
     * parameter data.
     * Used to build the columnView model.
     */
    public data: RetrieveMaterialPropertyOutputSettings[] = null;

    /**
     * On Data Change emitter.
     */
    public dataChange: ng.EventEmitter<RetrieveMaterialPropertyOutputSettings[]> = new ng.EventEmitter<RetrieveMaterialPropertyOutputSettings[]>();

    //#endregion Public variables
    /**
     * Constructor
     */
    constructor(private _elementRef: ng.ElementRef, viewContainerRef: ng.ViewContainerRef) {
        super(viewContainerRef);
        // Bind to the before save event
    }

    //#region Private methods
    private buildColumnViewModel(data: RetrieveMaterialPropertyOutputSettings[]): void {

        this._model = {
            rootNode: {
                id: "root",
                name: "links",
                children: <ColumnViewModel.ColumnViewLeaf[]>data.map(this.buildColumnViewChildModel.bind(this))
            }
        }
    }

    private buildColumnViewChildModel(data: RetrieveMaterialPropertyOutputSettings): ColumnViewModel.ColumnViewLeaf {
        return {
            id: this.framework.sandbox.util.uniqueId("link"),
            name: data.name ? data.name : "",
            value: null,
            canRemove: true,
            tag: data
        }
    }

    private emitLinks(): void {
        if (this._model && this._model.rootNode.children) {
            this.dataChange.emit(this.getLinks());
        }
    }

    private validateRow(context: OnValidateArgs, data: RetrieveMaterialPropertyOutputSettings): boolean {
        // checks if the properties values are null or empty. If so, a error message is shown
        if (data.name != null && data.name.length > 0 && data.valueType != null) {
            return true;
        } else {
            if (context) {
                context.resultMessages.push({
                    type: ResultMessageType.Error,
                    message: i18n.INVALID_OUTPUT
                })
            }
            return false;
        }
    }

    //#endregion Private methods

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
            path: null,
            valueType: null,
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
        this.emitLinks();
    }

    /**
     * Get links from columnView model.
     */
    public getLinks(): RetrieveMaterialPropertyOutputSettings[] {
        let rt: RetrieveMaterialPropertyOutputSettings[] = [];
        if (this._model != null && this._model.rootNode != null && this._model.rootNode.children != null) {
           rt = (<ColumnViewModel.ColumnViewLeaf[]>this._model.rootNode.children).map(leaf => leaf.tag);
        }
        return rt;
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
            const links = this.getLinks();
            return Promise.resolve(!links.some(link => this.validateRow(context, link) === false));
        }
    }

     //#endregion Public methods

}

