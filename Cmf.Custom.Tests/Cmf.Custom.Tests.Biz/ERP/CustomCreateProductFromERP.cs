using Cmf.Custom.Tests.Biz.Common.ERP;
using Cmf.Custom.Tests.Biz.Common.ERP.Product;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Foundation.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.Tests.Biz.ERP
{
    [TestClass]
    public class CustomCreateProductFromERP
    {

        private ExecutionScenario _scenario;

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            _scenario = new ExecutionScenario();
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (_scenario != null)
            {
                _scenario.CompleteCleanUp();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <TestCaseID>CustomCreateProductFromERP.CustomCreateProductFromERP_SendERPMessage_CreateProducts</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomCreateProductFromERP_SendERPMessage_CreateProducts()
        {

            string MessageProductDescription = "F4653F00050";
            string MessageType = "CHIP";
            string MessageProductType = "FINISHED_GOODS";
            string MessageProductUnits = "CM2";
            string MessageProductIsEnabled = "Y";
            string MessageProductYield = "88.0";
            string MessageProductGroup = "MG_PW_1000";
            string MessageProductMaximumMaterialSize = "24";

            List < ERPProduct > productsLists = new List<ERPProduct>() {
                new ERPProduct{
                    Name = Guid.NewGuid().ToString("N"),
                    Description = MessageProductDescription,
                    Type = MessageType,
                    ProductType = MessageProductType,
                    DefaultUnits = MessageProductUnits,
                    IsEnabled = MessageProductIsEnabled,
                    Yield = MessageProductYield,
                    ProductGroup = MessageProductGroup,
                    MaximumMaterialSize = MessageProductMaximumMaterialSize,
                    ProductParametersData = new List<ProductParameterData>(){
                        new ProductParameterData()
                        {
                            Name = "Raster X",
                            Value = "1020"
                        },
                        new ProductParameterData()
                        {
                            Name = "Raster Y",
                            Value = "1020"
                        },
                        new ProductParameterData()
                        {
                            Name = "CM2 Average",
                            Value = "1020"
                        },
                        new ProductParameterData()
                        {
                            Name = "Chips Fieldmask",
                            Value = "1020"
                        },
                        new ProductParameterData()
                        {
                            Name = "Wafer Size",
                            Value = "1020"
                        },
                        new ProductParameterData()
                        {
                            Name = "Chips Whole Wafer",
                            Value = "1020"
                        }
                    },
                    ProductAttributesData = new List<ProductAttributeData>()
                    {
                        new ProductAttributeData()
                        {
                            Name = "SAP Product Type",
                            Value = "F4653F00050"
                        },
                        new ProductAttributeData()
                        {
                            Name = "Technogoly",
                            Value = "PN"
                        },
                        new ProductAttributeData()
                        {
                            Name = "Status",
                            Value = "97"
                        },
                        new ProductAttributeData()
                        {
                            Name = "Dispo Level",
                            Value = "EOL"
                        }
                    }
                },
                new ERPProduct{
                    Name = Guid.NewGuid().ToString("N"),
                    Description = MessageProductDescription,
                    Type = MessageType,
                    ProductType = MessageProductType,
                    DefaultUnits = MessageProductUnits,
                    IsEnabled = MessageProductIsEnabled,
                    Yield = MessageProductYield,
                    ProductGroup = MessageProductGroup,
                    MaximumMaterialSize = MessageProductMaximumMaterialSize,
                    ProductParametersData = new List<ProductParameterData>(){
                        new ProductParameterData()
                        {
                            Name = "Raster X",
                            Value = "1020"
                        },
                        new ProductParameterData()
                        {
                            Name = "Raster Y",
                            Value = "1020"
                        },
                        new ProductParameterData()
                        {
                            Name = "CM2 Average",
                            Value = "1020"
                        },
                        new ProductParameterData()
                        {
                            Name = "Chips Fieldmask",
                            Value = "1020"
                        },
                        new ProductParameterData()
                        {
                            Name = "Wafer Size",
                            Value = "1020"
                        },
                        new ProductParameterData()
                        {
                            Name = "Chips Whole Wafer",
                            Value = "1020"
                        }
                    },
                    ProductAttributesData = new List<ProductAttributeData>()
                    {
                        new ProductAttributeData()
                        {
                            Name = "SAP Product Type",
                            Value = "F4653F00050"
                        },
                        new ProductAttributeData()
                        {
                            Name = "Technogoly",
                            Value = "PN"
                        },
                        new ProductAttributeData()
                        {
                            Name = "Status",
                            Value = "97"
                        },
                        new ProductAttributeData()
                        {
                            Name = "Dispo Level",
                            Value = "EOL"
                        }
                    }
                }
            };


            _scenario.ERPProductList = productsLists;

            _scenario.Setup();

            // Assert that Integration Entries are created and processed
            Assert.IsTrue(_scenario.IntegrationEntries.Count > 0, "Integration Entries should have been created");
            foreach (IntegrationEntry ie in _scenario.IntegrationEntries)
            {
                Assert.IsTrue(ie.IsIntegrationEntryProcessed(), "Integration Entry was not processed. Error Message: {0}", ie.ResultDescription);
            }
        }
    }
}
