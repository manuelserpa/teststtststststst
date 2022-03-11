using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.AMSOsram.Orchestration.OutputObjects;
using Cmf.Custom.Tests.Biz.Common.ERP;
using Cmf.Custom.Tests.Biz.Common.ERP.Product;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Foundation.Common.Base;
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
        public string ProductsMessageType { get; set; } = "SAPMessage";

        /// <summary>
        /// 
        /// </summary>
        public List<ERPProduct> ERPProductList { get; set; } = new List<ERPProduct>();

        /// <summary>
        /// 
        /// </summary>
        public ProductDataOutput products { get; set; } = new ProductDataOutput();

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

                CustomUtilities.DispatchIntegrationEntries(new IntegrationEntryCollection() { outputProducts.Result });
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
