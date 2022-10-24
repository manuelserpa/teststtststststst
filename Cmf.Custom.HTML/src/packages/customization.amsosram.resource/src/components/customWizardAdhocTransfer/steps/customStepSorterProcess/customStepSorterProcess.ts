//#region Imports

/** Core */
import { Component, Module, CoreComponent } from "cmf.core/src/core";

/** Nested modules */

import { BaseWidgetModule } from "cmf.core.controls/src/components/baseWidget/baseWidget";

import { PropertyContainerModule } from "cmf.core.business.controls/src/components/propertyContainer/propertyContainer";
import { PropertyEditorModule } from "cmf.core.business.controls/src/components/PropertyEditor/PropertyEditor";
import { PropertyViewerModule } from "cmf.core.business.controls/src/components/propertyViewer/propertyViewer";

/** i18n */
import i18n from "./i18n/customStepSorterProcess.default";

/** Angular */
import * as ng from "@angular/core";
import Cmf from "cmf.lbos";
import {
    CustomGetProductsWithProductGroupQuery,
    CustomGetResourceLoadPortsData,
    CustomGetResourceLoadPortsDataQueryInputs
} from "../../../../utils/queries";
import { ResolveCustomProductContainerCapacities } from "../../../../utils/smartTables";

//#endregion

//#region Exports

//#endregion

//#region Constants

//#endregion

/**
 * @whatItDoes
 *
 * Please provide a meaningful description of this component
 * Try to answer these questions:
 * * What is it?
 * * What it does?
 * * How does it behave with different sizes?
 * * Does it retrieve data from any external source (server, local database, text file, etc...)?
 *
 * @howToUse
 *
 * This component is used with the inputs and outputs mentioned below.
 *
 * Besides the description above, please complement it with a meaningful description of this component that answer these questions:
 * * How to use it?
 * * Where and When to use it?
 *
 * ### Inputs
 * `string` : **name** - The name of this component
 * `number` : **value** - The value of this component
 *
 * ### Outputs
 * `string` : **onNameChange** - When the name of the component change, this output emits the new name
 * `number` : **onValueChange** - When the value of the component change, this output emits the new value
 *
 * ### Example
 * To use the component, assume this HTML Template as an example:
 *
 * ```HTML
 * <customization-amsosram-resource-customStepSorterProcess></customization-amsosram-resource-customStepSorterProcess>
 * ```
 *
 * ### _NOTES_
 * (optional, Provide additional notes here)
 *
 * @description
 *
 * ## CustomStepSorterProcess Component
 *
 * ### Dependencies
 *
 * #### Components
 * * ComponentA : `package`
 * * ComponentB : `package`
 *
 * #### Services
 * * ServiceA : `package`
 * * ServiceB : `package`
 *
 * #### Directives
 * * DirectiveA : `package`
 * * DirectiveB : `package`
 *
 */
@Component({
    moduleId: __moduleName,
    selector: 'customization-amsosram-resource-customStepSorterProcess',
    inputs: ['resource', 'selectedLoadPort', 'selectedProduct', 'selectedSorterProcess', 'selectedQuantity'],
    outputs: ['selectedLoadPortChange', 'selectedProductChange', 'selectedSorterProcessChange', 'selectedQuantityChange'],
    templateUrl: './customStepSorterProcess.html',
    styleUrls: ["./customStepSorterProcess.css"],
    assign: { i18n: i18n }
})
export class CustomStepSorterProcess extends CoreComponent implements ng.OnChanges {

    //#region Private properties

    private _selectedLoadPort: Cmf.Navigo.BusinessObjects.SubResource;

    private _selectedProduct: Cmf.Navigo.BusinessObjects.Product;

    private _selectedSorterProcess: string;

    private _selectedQuantity: number;

    //#endregion

    //#region Public properties

    /*
    * Product query to get product group
    */
    public productQuery = CustomGetProductsWithProductGroupQuery();

    /**
    * Resource
    */
    public resource: Cmf.Navigo.BusinessObjects.Resource = null;

    /**
    * Load Ports (Resource)
    */
    public loadPorts = new Cmf.Navigo.BusinessObjects.SubResourceCollection();

    /**
    * Load Port (Resource)
    */
    public get selectedLoadPort() {
        return this._selectedLoadPort;
    };

    public selectedLoadPortChange = new ng.EventEmitter<Cmf.Navigo.BusinessObjects.SubResource>();

    public set selectedLoadPort(loadPort) {
        this._selectedLoadPort = loadPort;
        this.selectedLoadPortChange.emit(loadPort);
    };

    /**
    * Product
    */
    public get selectedProduct() {
        return this._selectedProduct
    };

    public selectedProductChange = new ng.EventEmitter<Cmf.Navigo.BusinessObjects.Product>();

    public set selectedProduct(product) {
        this._selectedProduct = product;
        this.selectedProductChange.emit(product);
    };

    /**
    * Quantity
    */
    public get selectedQuantity() {
        return this._selectedQuantity
    };

    public selectedQuantityChange = new ng.EventEmitter<number>();

    public set selectedQuantity(Quantity) {
        this._selectedQuantity = Quantity;
        this.selectedQuantityChange.emit(Quantity);
    };

    /**
    * Sorter Process
    */
    public get selectedSorterProcess() {
        return this._selectedSorterProcess
    };

    public selectedSorterProcessChange = new ng.EventEmitter<string>();

    public set selectedSorterProcess(sorterProcess) {
        this._selectedSorterProcess = sorterProcess;
        this.selectedSorterProcessChange.emit(sorterProcess);
    };

    //#endregion

    /**
     * Constructor
     *
     * @param viewContainerRef the reference to the component view container
     */
    constructor(viewContainerRef: ng.ViewContainerRef, private _elementRef: ng.ElementRef) {
        super(viewContainerRef);
    }

    //#region Private methods

    private async getResourceLoadPortsData(parameters: CustomGetResourceLoadPortsDataQueryInputs) {
        let loadPorts = new Cmf.Navigo.BusinessObjects.SubResourceCollection()

        try {
            this.framework.sandbox.feedback.startProgressIndicator(this._elementRef);

            loadPorts = await CustomGetResourceLoadPortsData(this.framework, parameters);
        } catch (error) {
            this.framework.sandbox.feedback.error(error);
        } finally {
            this.framework.sandbox.feedback.stopProgressIndicator(this._elementRef);

            return loadPorts;
        }
    }

    //#endregion

    //#region Public methods

    /**
     * On Changes
     * @param changes The detected changes
     */
    public async ngOnChanges(changes: any) {
        if (changes["resource"]?.currentValue != null) {
            this.resource = changes["resource"].currentValue;

            const loadPorts = await this.getResourceLoadPortsData({ sourceEntityId: this.resource.Id });

            this.loadPorts = loadPorts.filter(loadPort => loadPort.Attributes.get("IsLoadPortInUse") === false) as
                Cmf.Navigo.BusinessObjects.SubResourceCollection;
        }
    }

    public async onSelectedProductChange(product) {
        if (product?.Name == null) {
            return;
        }

        const result = await ResolveCustomProductContainerCapacities(this.framework, {
            productName: product.Name,
            productGroupName: product.ProductGroup_Name
        });

        if (result && this.selectedQuantity == null) {
            this.selectedQuantity = result.SourceCapacity
        }
    };

    //#endregion
}

@Module({
    imports: [
        BaseWidgetModule,
        PropertyContainerModule,
        PropertyEditorModule,
        PropertyViewerModule
    ],
    declarations: [CustomStepSorterProcess],
    exports: [CustomStepSorterProcess]
})
export class CustomStepSorterProcessModule { }
