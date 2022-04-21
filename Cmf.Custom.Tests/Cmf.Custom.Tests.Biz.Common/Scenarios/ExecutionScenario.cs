using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.AMSOsram.Orchestration.OutputObjects;
using Cmf.Custom.Tests.Biz.Common.ERP;
using Cmf.Custom.Tests.Biz.Common.ERP.Product;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Foundation.Common.Base;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmf.Custom.Tests.Biz.Common.Scenarios
{
    public class ExecutionScenario : BaseCustomScenario
    {

        #region Properties

        /// <summary>
        /// Integration Entries
        /// </summary>
        public IntegrationEntryCollection IntegrationEntries
        {
            get;
            private set;
        } = new IntegrationEntryCollection();

        /// <summary>
        /// Should the scenario send an Production Order message
        /// </summary>
        public bool IsToSendProducts { get; set; } = true;

        /// <summary>
        /// Should the scenario associate each material to a Production Order
        /// </summary>
        public bool IsToAssingPOsToMaterials { get; set; } = false;

        /// <summary>
        /// ERP Product Maping data
        /// </summary>
        public ERPProduct ERPProduct
        {
            get;
            set;
        } = null;

        public int ProductsToGenerate { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        public string ProductsMessageType { get; set; } = "PerformProductsMasterData";

        /// <summary>
        /// 
        /// </summary>
        public List<ERPProduct> ERPProductList { get; set; } = new List<ERPProduct>();

        /// <summary>
        /// 
        /// </summary>
        public ProductDataOutput products { get; set; } = new ProductDataOutput();

        /// <summary>
        /// Scenario Quantity to be used by ProductionOrders and Materials creation
        /// </summary>
        public virtual decimal ScenarioQuantity { get; set; } = 1;

        /// <summary>
        /// Product Name to be used by the scenario
        /// </summary>
        public virtual string ProductName { get; set; } = AMSOsramConstants.DefaultTestProductName;

        /// <summary>
        /// Facility Name to be used by the scenario
        /// </summary>
        public string FacilityName { get; set; } = AMSOsramConstants.DefaultFacilityName;

        /// <summary>
        /// Lot Name to be used by the scenario
        /// </summary>
        public string LotName { get; set; } = null;

        /// <summary>
        /// FlowPath to be used by the scenario
        /// </summary>
        public virtual string FlowPath { get; set; } = AMSOsramConstants.DefaultTestFlowPath;

        /// <summary>
        /// Collection of ProductionOrders generated
        /// </summary>
        public ProductionOrderCollection GeneratedProductionOrders { get; set; } = new ProductionOrderCollection();

        /// <summary>
        /// Collection of Lots generated
        /// </summary>
        public MaterialCollection GeneratedLots { get; set; } = new MaterialCollection();

        /// <summary>
        /// Number of ProductionOrders to Generate by the Scenario
        /// </summary>
        public int NumberOfProductionOrdersToGenerate { get; set; } = 0;

        /// <summary>
        /// Number of Lots to Generate by the Scenario
        /// </summary>
        public int NumberOfMaterialsToGenerate { get; set; } = 0;

        /// <summary>
        /// Number of Wafers to Generate by the Scenario
        /// </summary>
        public int NumberOfChildMaterialsToGenerate { get; set; } = 0;

        /// <summary>
        /// Material to be generated form
        /// </summary>
        public string MaterialToGenerateForm = AMSOsramConstants.DefaultMaterialFormName;

        #endregion

        public override void Setup()
        {
            if (IsToSendProducts)
            {
                products.ProductsData = ERPProductList;

                string xmlMessage = ERPMessageSerializer<ProductDataOutput>.Serialize(products);

                CustomReceiveERPMessageInput input = new CustomReceiveERPMessageInput()
                {
                    MessageType = ProductsMessageType,
                    Message = xmlMessage
                };

                CustomReceiveERPMessageOutput outputProducts = input.CustomReceiveERPMessageSync();

                if (outputProducts != null)
                {
                    IntegrationEntries.Add(outputProducts.Result);
                }

                CustomUtilities.DispatchIntegrationEntries(new IntegrationEntryCollection() { outputProducts.Result });
            }

            // Create Production orders to be used
            for (int i = 0; i < NumberOfProductionOrdersToGenerate; i++)
            {
                ProductionOrder generatedProductionOrder =
                    CustomUtilities.CreateProductionOrder(
                        name: LotName,
                        tearDownManager: TearDownManager,
                        productName: ProductName,
                        quantity: ScenarioQuantity);

                GeneratedProductionOrders.Add(generatedProductionOrder);
            }

            // Create Lots to be used
            for (int i = 0; i < NumberOfMaterialsToGenerate; i++)
            {
                ProductionOrder productionOrder = null;
                string materialName = LotName;

                // In case the lots need to be associated to a Production Order 
                if (IsToAssingPOsToMaterials)
                {
                    productionOrder = GeneratedProductionOrders[i];
                    materialName = productionOrder.Name;
                }

                Material generatedMaterial =
                    CustomUtilities.CreateMaterial(
                        tearDownManager: TearDownManager,
                        name: materialName,
                        prodOrder: productionOrder,
                        productName: ProductName,
                        flowPath: FlowPath,
                        primaryQuantity: ScenarioQuantity * NumberOfChildMaterialsToGenerate,
                        facilityName: FacilityName,
                        form: MaterialToGenerateForm);

                // Create Wafers to be used
                if (NumberOfChildMaterialsToGenerate > 0)
                {
                    MaterialCollection subMaterialsCollection = new MaterialCollection();

                    for (int count = 0; count < NumberOfChildMaterialsToGenerate; count++)
                    {
                        Material subMaterial = new Material()
                        {
                            PrimaryQuantity = ScenarioQuantity
                        };

                        subMaterialsCollection.Add(subMaterial);
                    }

                    ExpandMaterialOutput expandMaterialOutput = new ExpandMaterialInput()
                    {
                        Material = generatedMaterial,
                        SubMaterials = subMaterialsCollection,
                        Form = AMSOsramConstants.DefaultMaterialLogisticalWaferForm
                    }.ExpandMaterialSync();

                    generatedMaterial = expandMaterialOutput.Material;
                    generatedMaterial.SubMaterials = expandMaterialOutput.ExpandedSubMaterials;
                }

                GeneratedLots.Add(generatedMaterial);
            }
        }

        public override void CompleteCleanUp()
        {
            // Remove created Integration Entries
            TerminateIntegrationEntries();

            TearDownManager.TearDownSequentially();
        }

        #region Private Methods
        /// <summary>
        /// Terminate created Integration Entries
        /// </summary>
        private void TerminateIntegrationEntries()
        {
            if (IntegrationEntries.Any())
            {
                bool isTerminated = false;
                int terminationAttempts = 0;
                while (!isTerminated && terminationAttempts < 3)
                {
                    try
                    {
                        IntegrationEntries.Load<IntegrationEntry>();

                        IntegrationEntryCollection integrationEntries = new IntegrationEntryCollection();
                        integrationEntries.AddRange(IntegrationEntries.Where(ie => ie.UniversalState != UniversalState.Terminated));

                        foreach (var item in integrationEntries)
                        {
                            new TerminateIntegrationEntryInput()
                            {
                                IntegrationEntry = item
                            }.TerminateIntegrationEntrySync();
                        }

                        isTerminated = true;
                    }
                    catch (Exception)
                    {
                        terminationAttempts++;
                    }
                }
            }
        } 
        #endregion
    }
}
