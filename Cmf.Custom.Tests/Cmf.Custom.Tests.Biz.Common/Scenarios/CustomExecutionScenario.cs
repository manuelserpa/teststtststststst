using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.AMSOsram.Orchestration.OutputObjects;
using Cmf.Custom.Tests.Biz.Common.ERP;
using Cmf.Custom.Tests.Biz.Common.ERP.Material;
using Cmf.Custom.Tests.Biz.Common.ERP.Product;
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

        public bool IsToSetMaterialDCContext;



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

        public SmartTableManager SmartTableManager = new SmartTableManager();

        public List<Dictionary<string, string>> MaterialDCContext;

        /// <summary>
        /// CustomExecutionScenario Constructor
        /// </summary>
        public CustomExecutionScenario()
        {
            this.IntegrationEntries = new IntegrationEntryCollection();

            this.ProductOutput = new ProductDataOutput();

            this.GoodsReceiptCertificate = new GoodsReceiptCertificate();
        }

        #endregion

        public override void Setup()
        {
            string messageType = string.Empty;

            string xmlMessage = string.Empty;

            if (MaterialDCContext.Any())
            {
                foreach (Dictionary<string, string> row in MaterialDCContext)
                {
                    SmartTableManager.SetSmartTableData("MaterialDataCollectionContext", row);
                }
            }

            if (IsToSendIncomingMaterial)
            {
                messageType = "PerformIncomingMaterialMasterData";

                xmlMessage = ERPMessageSerializer<GoodsReceiptCertificate>.Serialize(this.GoodsReceiptCertificate);
            }

            if (IsToSendProducts)
            {
                messageType = "PerformProductsMasterData";

                xmlMessage = ERPMessageSerializer<ProductDataOutput>.Serialize(this.ProductOutput);
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
            if (MaterialDCContext.Any())
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
