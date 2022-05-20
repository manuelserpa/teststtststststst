using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.AMSOsram.Orchestration.OutputObjects;
using Cmf.Custom.Tests.Biz.Common.ERP.Material;
using Cmf.Custom.Tests.Biz.Common.ERP.Product;
using Cmf.Custom.Tests.Biz.Common.ERP.ProductionOrder;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Foundation.Common.Base;
using System;
using System.Collections.Generic;
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
        /// Integration Entries
        /// </summary>
        public IntegrationEntryCollection IntegrationEntries;

        /// <summary>
        /// 
        /// </summary>
        public ProductDataOutput ProductOutput;

        /// <summary>
        /// 
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
        }

        public override void CompleteCleanUp()
        {
            if (MaterialDCContext.Any() || SmartTablesToClearInSetup.Any())
            {
                SmartTableManager.TearDown();
            }

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
