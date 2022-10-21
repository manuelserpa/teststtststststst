/** Core */
import { Component, Module, CoreComponent } from "cmf.core/src/core";
import { Cmf } from "cmf.lbos";

/** Nested modules */
import {
    TransactionWizardModule,
    TransactionWizardInterface,
    TransactionWizardArgs,
} from "cmf.core.business.controls/src/directives/transactionWizard/transactionWizard";
import { PageBag } from "cmf.core.controls/src/components/page/pageBag";
import { Wizard } from "cmf.core.controls/src/components/wizard/wizardBase";

/** i18n */
import i18n from "./i18n/customWizardAdhocTransfer.default";

/** Angular */
import * as ng from "@angular/core";

/** Custom */
import { CustomStepSorterProcessModule } from "./steps/customStepSorterProcess/customStepSorterProcess";

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
 * <customization-amsosram-resource-customWizardAdhocTransfer></customization-amsosram-resource-customWizardAdhocTransfer>
 * ```
 *
 * ### _NOTES_
 * (optional, Provide additional notes here)
 *
 * @description
 *
 * ## CustomWizardAdhocTransfer Component
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
    selector: "customization-amsosram-resource-customWizardAdhocTransfer",
    inputs: [],
    outputs: [],
    templateUrl: "./customWizardAdhocTransfer.html",
    styleUrls: ["./customWizardAdhocTransfer.css"],
    assign: { i18n: i18n },
})
export class CustomWizardAdhocTransfer extends CoreComponent implements TransactionWizardInterface {
    /**
     * The wizard element
     */
    @ng.ViewChild(Wizard)
    protected _nestedWizard: Wizard;

    //#region Private properties

    //#endregion

    public loadPort: Cmf.Navigo.BusinessObjects.SubResource;

    public product: Cmf.Navigo.BusinessObjects.Product;

    public sorterProcess: string;

    public quantity: number;

    //#region Public properties

    /**
     * The instance of the wizard
     */
    public instance: Cmf.Navigo.BusinessObjects.Resource;

    //#endregion

    constructor(
        viewContainerRef: ng.ViewContainerRef,
        private _pageBag: PageBag
    ) {
        super(viewContainerRef);
    }

    //#region Private methods

    //#endregion

    //#region Public methods

    /**
     * Method that prepares the data for the wizard
     */
    public prepareDataInput(): Promise<Cmf.Foundation.BusinessOrchestration.BaseInput[]> {
        // Please provide all the inputs objects that need to be fetched when the wizard loads (if required)

        const instanceInput = new Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects.GetObjectByIdInput();
        instanceInput.IgnoreLastServiceId = true;
        instanceInput.Id = this._pageBag.context.resource.Id;
        instanceInput.Type = "Resource";

        const inputs: Cmf.Foundation.BusinessOrchestration.BaseInput[] = [];
        inputs.push(instanceInput);

        return Promise.resolve(inputs);
    }

    /**
     * Method that receive the data from prepareDataInput
     */
    public handleDataOutput(outputs: Cmf.Foundation.BusinessOrchestration.BaseOutput[]): Promise<void> {
        // Please handle all the outputs from prepareDataInput (if required)
        if (outputs != null && outputs.length > 0) {
            const output = outputs[0] as
                Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.OutputObjects.GetObjectByIdOutput;

            if (output != null && output.Instance != null) {
                this.instance = output.Instance;

                if (this._nestedWizard != null) {
                    return this._nestedWizard
                        .reEvaluateContextPreConditions({ instance: this.instance })
                        .then(() => {
                            return Promise.resolve();
                        });
                }
            }
        }

        if (this.instance == null) {
            throw new Error(i18n.errors.NO_INSTANCE_FOUND);
        }

        return Promise.resolve(null);
    }

    /**
     * The wizard prepareTransactionInput method where we can append the input for the final wizard
     * @param args Current inputs where the user can append or simply resolve its own input.
     */
    public prepareTransactionInput(
        args: TransactionWizardArgs
    ): Promise<Cmf.Foundation.BusinessOrchestration.BaseInput> {
        // Set the DEE Action to call
        const executeActionInput = new Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects.ExecuteActionInput();
        executeActionInput.Action = new Cmf.Foundation.Common.DynamicExecutionEngine.Action();
        executeActionInput.Action.Name = "CustomSendAdHocTransferInformationToIoT";

        // Set the inputs
        executeActionInput.Input = new Map<string, any>();
        executeActionInput.Input.set("Resource", this.instance.Name);
        executeActionInput.Input.set("SorterProcess", this.sorterProcess);
        executeActionInput.Input.set("Quantity", this.quantity);
        executeActionInput.Input.set("Product", this.product.Name);
        executeActionInput.Input.set("LoadPort", this.loadPort.Name);

        return Promise.resolve(executeActionInput);
    }

    /**
     * The wizard hook for handling the above service call.
     * @param output output object, result of the input created in the prepareTransactionInput
     */
    public handleTransactionOutput(
        output: Cmf.Foundation.BusinessOrchestration.BaseOutput
    ): Promise<void> {
        // Please handle all the output of the last service call (if required)
        return null;
    }

    //#endregion
}

@Module({
    imports: [TransactionWizardModule, CustomStepSorterProcessModule],
    declarations: [CustomWizardAdhocTransfer],
    defaultRoute: CustomWizardAdhocTransfer,
    exports: [CustomWizardAdhocTransfer],
})
export class CustomWizardAdhocTransferModule { }
