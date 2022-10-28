﻿using Cmf.Custom.amsOSRAM.Orchestration.InputObjects;
using Cmf.Custom.amsOSRAM.Orchestration.OutputObjects;
using Cmf.Custom.Tests.Biz.Common.ERP.Material;
using Cmf.Custom.Tests.Biz.Common.ERP.Product;
using Cmf.Custom.Tests.Biz.Common.ERP.ProductionOrder;
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
using System.Collections.ObjectModel;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.Common.Scenarios
{
    public class CustomExecutionScenario : CustomBaseScenario
    {

        #region Properties

        /// <summary>
        /// Should the scenario send an Product message
        /// </summary>
        public bool IsToSendProducts;

        /// <summary>
        /// Should the scenario send an Incoming Material message
        /// </summary>
        public bool IsToSendIncomingMaterial;

        /// <summary>
        /// Should the scenario associate each material to a Production Order
        /// </summary>
        public bool IsToAssingPOsToMaterials { get; set; } = false;

        /// <summary>
        /// Integration Entries
        /// </summary>
        public IntegrationEntryCollection IntegrationEntries;

        /// <summary>
        /// ProductDataOutput
        /// </summary>
        public ProductDataOutput ProductOutput;

        /// <summary>
        /// GoodsReceiptCertificate
        /// </summary>
        public GoodsReceiptCertificate GoodsReceiptCertificate;

        /// <summary>
        /// ERP Message to create Production Orders by Integration Entries
        /// </summary>
        public CustomImportProductionOrderCollection CustomImportProductionOrderCollection = null;

        /// <summary>
        /// SmartTableManager
        /// </summary>
        public SmartTableManager SmartTableManager { get; set; } = new SmartTableManager();

        /// <summary>
        /// List of smart tables to be cleared in Setup
        /// </summary>
        public List<string> SmartTablesToClearInSetup { get; set; } = new List<string>();

        /// <summary>
        /// SmartTable MaterialDataCollectionContext 
        /// </summary>
        public List<Dictionary<string, string>> MaterialDCContext = new List<Dictionary<string, string>>();

        /// <summary>
        /// SmartTable CustomProductContainerCapacities 
        /// </summary>
        public List<Dictionary<string, string>> CustomProductContainerCapacities = new List<Dictionary<string, string>>();

        /// <summary>
        /// SmartTable RecipeContext 
        /// </summary>
        public List<Dictionary<string, string>> RecipeContext = new List<Dictionary<string, string>>();

        /// <summary>
        /// SmartTable CustomReportConsumptionToSAP 
        /// </summary>
        public List<Dictionary<string, string>> CustomReportConsumptionToSAP = new List<Dictionary<string, string>>();

        /// <summary>
        /// GenericTableManager
        /// </summary>
        public GenericTableManager GenericTableManager { get; set; } = new GenericTableManager();

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
        public virtual string ProductName { get; set; } = amsOSRAMConstants.DefaultTestProductName;

        /// <summary>
        /// Facility Name to be used by the scenario
        /// </summary>
        public string FacilityName { get; set; } = amsOSRAMConstants.DefaultFacilityName;

        /// <summary>
        /// Lot Name to be used by the scenario
        /// </summary>
        public string LotName { get; set; } = null;

        /// <summary>
        /// FlowPath to be used by the scenario
        /// </summary>
        public virtual string FlowPath { get; set; } = amsOSRAMConstants.DefaultTestFlowPath;

        /// <summary>
        /// Collection of ProductionOrders generated
        /// </summary>
        public ProductionOrderCollection GeneratedProductionOrders { get; set; } = new ProductionOrderCollection();

        /// <summary>
        /// Collection of Lots generated
        /// </summary>
        public MaterialCollection GeneratedLots { get; set; } = new MaterialCollection();

        /// <summary>
        /// Collection of Wafers generated
        /// </summary>
        public MaterialCollection GeneratedWafers { get; set; } = new MaterialCollection();

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
        public string MaterialToGenerateForm = amsOSRAMConstants.DefaultMaterialFormName;

        /// <summary>
        /// Resource Attributes to Set
        /// </summary>
        public Dictionary<string, AttributeCollection> ResourceAttributesToSet = new Dictionary<string, AttributeCollection>();

        /// <summary>
        /// Step Attributes to Set
        /// </summary>
        public Dictionary<string, AttributeCollection> StepAttributesToSet = new Dictionary<string, AttributeCollection>();

        /// <summary>
        /// Resource attributs to Rollback in TearDown
        /// </summary>
        private readonly Dictionary<string, AttributeCollection> ResourceAttributesToRollback = new Dictionary<string, AttributeCollection>();

        /// <summary>
        /// Step attributs to Rollback in TearDown
        /// </summary>
        private readonly Dictionary<string, AttributeCollection> StepAttributesToRollback = new Dictionary<string, AttributeCollection>();

        #endregion

        /// <summary>
        /// CustomExecutionScenario Constructor
        /// </summary>
        public CustomExecutionScenario()
        {
            this.IntegrationEntries = new IntegrationEntryCollection();

            this.ProductOutput = new ProductDataOutput();

            this.GoodsReceiptCertificate = new GoodsReceiptCertificate();
        }


        /// <summary>
        /// CustomExecutionScenario Setup
        /// </summary>
        public override void Setup()
        {
            #region Smart Table Configuration

            foreach (string smartTableName in SmartTablesToClearInSetup)
            {
                SmartTableManager.ClearSmartTable(smartTableName);
            }

            if (MaterialDCContext.Any())
            {
                foreach (Dictionary<string, string> row in MaterialDCContext)
                {
                    SmartTableManager.SetSmartTableData("MaterialDataCollectionContext", row);
                }
            }

            if (RecipeContext.Any())
            {
                foreach (Dictionary<string, string> row in RecipeContext)
                {
                    SmartTableManager.SetSmartTableData("RecipeContext", row);
                }
            }

            if (CustomProductContainerCapacities.Any())
            {
                foreach (Dictionary<string, string> row in CustomProductContainerCapacities)
                {
                    SmartTableManager.SetSmartTableData(amsOSRAMConstants.CustomProductContainerCapacitiesSmartTable, row);
                }
            }

            if (CustomReportConsumptionToSAP.Any())
            {
                foreach (Dictionary<string, string> row in CustomReportConsumptionToSAP)
                {
                    SmartTableManager.SetSmartTableData(amsOSRAMConstants.CustomReportConsumptionToSAPSmartTable, row);
                }
            }

            #endregion

            #region Set attributes

            if (ResourceAttributesToSet.Any())
            {
                foreach (string name in ResourceAttributesToSet.Keys)
                {
                    Resource resource = new Resource()
                    {
                        Name = name
                    };

                    Collection<string> attributesNames = new Collection<string>();
                    Collection<string> attributesNamesToRemove = new Collection<string>();
                    AttributeCollection attributesToSave = new AttributeCollection();

                    foreach (KeyValuePair<string, object> attributes in ResourceAttributesToSet[name])
                    {
                        attributesNames.Add(attributes.Key);

                        if (attributes.Value == null)
                        {
                            attributesNamesToRemove.Add(attributes.Key);
                        } else
                        {
                            attributesToSave.Add(attributes.Key, attributes.Value);
                        }
                    }

                    resource.Load();
                    resource.LoadAttributes(attributesNames);

                    ResourceAttributesToRollback.Add(name, resource.Attributes);

                    resource.RemoveAttributes(attributesNamesToRemove);
                    resource.SaveAttributes(attributesToSave);
                }
            }

            if (StepAttributesToSet.Any())
            {
                foreach (string name in StepAttributesToSet.Keys)
                {
                    Step step = new Step()
                    {
                        Name = name
                    };

                    Collection<string> attributesNames = new Collection<string>();
                    Collection<string> attributesNamesToRemove = new Collection<string>();
                    AttributeCollection attributesToSave = new AttributeCollection();

                    foreach (KeyValuePair<string, object> attributes in StepAttributesToSet[name])
                    {
                        attributesNames.Add(attributes.Key);

                        if (attributes.Value == null)
                        {
                            attributesNamesToRemove.Add(attributes.Key);
                        }
                        else
                        {
                            attributesToSave.Add(attributes.Key, attributes.Value);
                        }
                    }

                    step.Load();
                    step.LoadAttributes(attributesNames);

                    StepAttributesToRollback.Add(name, step.Attributes);

                    step.RemoveAttributes(attributesNamesToRemove);
                    step.SaveAttributes(attributesToSave);
                }
            }

            #endregion

            string messageType = string.Empty;

            string xmlMessage = string.Empty;

            if (IsToSendIncomingMaterial)
            {
                messageType = "PerformIncomingMaterialMasterData";

                xmlMessage = CustomUtilities.SerializeToXML<GoodsReceiptCertificate>(this.GoodsReceiptCertificate);
            }

            if (IsToSendProducts)
            {
                messageType = "PerformProductsMasterData";

                xmlMessage = CustomUtilities.SerializeToXML<ProductDataOutput>(this.ProductOutput);
            }

            if (CustomImportProductionOrderCollection != null)
            {
                messageType = "PerformProductionOrdersMasterData";

                xmlMessage = CustomUtilities.SerializeToXML<CustomImportProductionOrderCollection>(this.CustomImportProductionOrderCollection);
            }

            if (!string.IsNullOrEmpty(messageType) && !string.IsNullOrEmpty(xmlMessage))
            {
                CustomReceiveERPMessageInput input = new CustomReceiveERPMessageInput()
                {
                    MessageType = messageType,
                    Message = xmlMessage
                };

                CustomReceiveERPMessageOutput output = input.CustomReceiveERPMessageSync();

                if (output?.Result != null)
                {
                    IntegrationEntries.Add(output.Result);
                }

                CustomUtilities.DispatchIntegrationEntries(new IntegrationEntryCollection() { output.Result });
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
                        primaryQuantity: NumberOfChildMaterialsToGenerate != 0 ? ScenarioQuantity * NumberOfChildMaterialsToGenerate : ScenarioQuantity,
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
                        Form = amsOSRAMConstants.DefaultMaterialLogicalWaferForm
                    }.ExpandMaterialSync();

                    generatedMaterial = expandMaterialOutput.Material;
                    generatedMaterial.SubMaterials = expandMaterialOutput.ExpandedSubMaterials;
                }

                GeneratedLots.Add(generatedMaterial);
            }
        }

        public override void CompleteCleanUp()
        {
            if (ResourceAttributesToSet.Any())
            {
                foreach (string name in ResourceAttributesToSet.Keys)
                {
                    Resource resource = new Resource()
                    {
                        Name = name
                    };

                    resource.Load();

                    if (ResourceAttributesToRollback.ContainsKey(name))
                    {
                        resource.SaveAttributes(ResourceAttributesToRollback[name]);
                    }

                    Collection<string> attributesToRemove = new Collection<string>();

                    foreach (string attributeName in ResourceAttributesToSet[name].Keys)
                    {
                        if (!(ResourceAttributesToRollback.ContainsKey(name) && ResourceAttributesToRollback[name].ContainsKey(attributeName)))
                        {
                            attributesToRemove.Add(attributeName);
                        }
                    }

                    if (attributesToRemove.Count > 0)
                    {
                        resource.RemoveAttributes(attributesToRemove);
                    }
                }
            }

            if (StepAttributesToSet.Any())
            {
                foreach (string name in StepAttributesToSet.Keys)
                {
                    Step step = new Step()
                    {
                        Name = name
                    };

                    step.Load();

                    if (StepAttributesToRollback.ContainsKey(name))
                    {
                        step.SaveAttributes(StepAttributesToRollback[name]);
                    }

                    Collection<string> attributesToRemove = new Collection<string>();

                    foreach (string attributeName in StepAttributesToSet[name].Keys)
                    {
                        if (!(StepAttributesToRollback.ContainsKey(name) && StepAttributesToRollback[name].ContainsKey(attributeName)))
                        {
                            attributesToRemove.Add(attributeName);
                        }
                    }

                    if (attributesToRemove.Count > 0)
                    {
                        step.RemoveAttributes(attributesToRemove);
                    }
                }
            }

            if (MaterialDCContext.Any() || SmartTablesToClearInSetup.Any() || RecipeContext.Any() || CustomProductContainerCapacities.Any())
            {
                SmartTableManager.TearDown();
            }

            // Remove created Integration Entries
            TerminateIntegrationEntries();

            GeneratedLots.Load();

            foreach (Material material in GeneratedLots)
            {
                if (material.HoldCount > 0)
                {
                    material.LoadRelation("MaterialHoldReason");

                    EntityRelationCollection materialHoldReasons = material.RelationCollection["MaterialHoldReason"];

                    foreach (MaterialHoldReason materialHoldReason in materialHoldReasons)
                    {
                        material.ReleaseByReason(materialHoldReason);
                    }
                }
            }

            TearDownManager.TearDownSequentially();
        }

        #region Private Methods

        public Tuple<Material, Material> GenerateWafer(string type = null, Material parentMaterial = null, string productName = null, string flowPath = null, decimal? primaryQuantity = null, string faciltyName = null)
        {
            Material generatedWafer =
                    CustomUtilities.CreateMaterial(
                        type: type ?? amsOSRAMConstants.MaterialWaferSubstrateType,
                        tearDownManager: TearDownManager,
                        productName: productName ?? amsOSRAMConstants.DefaultTestProductName,
                        flowPath: flowPath ?? FlowPath,
                        primaryQuantity: primaryQuantity ?? 1,
                        facilityName: faciltyName ?? FacilityName,
                        form: amsOSRAMConstants.DefaultMaterialWaferForm);

            Material parentMaterialLoaded = parentMaterial;

            if (parentMaterialLoaded != null)
            {
                parentMaterialLoaded = new AttachMaterialsInput()
                {
                    Material = parentMaterial,
                    SubMaterials = new MaterialCollection { generatedWafer }
                }.AttachMaterialsSync().Material;
            }

            GeneratedWafers.Add(generatedWafer);

            return Tuple.Create(generatedWafer, parentMaterialLoaded);
        }

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
