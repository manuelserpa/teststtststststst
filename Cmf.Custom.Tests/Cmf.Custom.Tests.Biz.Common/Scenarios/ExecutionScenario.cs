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

        public string ProductsMessageType { get; set; } = "SAPMessage";

        #endregion

        public override void Setup()
        {
            if (IsToSendProducts)
            {
                ERPProduct = new ERPProduct()
                {
                    Name = "",
                    Description = "",
                    Type = "",
                    ProductType = "",
                    DefaultUnits = "",
                    IsEnabled = "",
                    Yield = "",
                    ProductGroup = "",
                    MaximumMaterialSize = "",
                    ProductParametersData = new List<ProductParameterData>()
                    {
                        new ProductParameterData()
                        {
                            Name = "",
                            Value = "",
                        }
                    }

                };

                string xmlMessage = ERPMessageSerializer<ERPProduct>.Serialize(ERPProduct);

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
