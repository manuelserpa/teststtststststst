using Cmf.Custom.Tests.Biz.Common.ERP;
using Cmf.Custom.Tests.Biz.Common.ERP.Product;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

            string messageProductDescription = "F4653F00050";
            string messageType = "CHIP";
            string messageProductType = "FINISHED_GOODS";
            string messageProductUnits = "CM2";
            string messageProductIsEnabled = "Y";
            string messageProductYield = "88.0";
            string messageProductGroup = "MG_PW_1000";
            string messageProductMaximumMaterialSize = "24";
            string firstProductName = Guid.NewGuid().ToString("N");
            string secondProductName = Guid.NewGuid().ToString("N");
            string[] productAttributeNames = new string[] { "SAP Product Type", "Technology", "Status", "Dispo Level" };
            string[] productAttributeValues = new string[] { "F4653F00050", "PN", "97", "EOL" };

            Dictionary<string, string> parameterData = new Dictionary<string, string>() { 
                { "Raster X", "1020" }, 
                { "Raster Y", "1020" }, 
                { "CM2 Average", "164.55" }, 
                { "Chips Fieldmask", "15816" }, 
                { "Wafer Size", "150" }, 
                { "Chips Whole Wafer", "16916.571" } };

            List<ProductParameterData> productParameterData = new List<ProductParameterData>();

            foreach (string parameterName in parameterData.Keys)
            {
                ProductParameterData parameter = new ProductParameterData()
                {
                    Name = parameterName,
                    Value = parameterData[parameterName]
                };

                productParameterData.Add(parameter);
            }

            List < ERPProduct > productsLists = new List<ERPProduct>() {
                new ERPProduct{
                    Name = firstProductName,
                    Description = messageProductDescription,
                    Type = messageType,
                    ProductType = messageProductType,
                    DefaultUnits = messageProductUnits,
                    IsEnabled = messageProductIsEnabled,
                    Yield = messageProductYield,
                    ProductGroup = messageProductGroup,
                    MaximumMaterialSize = messageProductMaximumMaterialSize,
                    ProductParametersData = productParameterData,
                    ProductAttributesData = new List<ProductAttributeData>()
                    {
                        new ProductAttributeData()
                        {
                            Name = productAttributeNames[0],
                            Value = productAttributeValues[0]
                        },
                        new ProductAttributeData()
                        {
                            Name = productAttributeNames[1],
                            Value = productAttributeValues[1]
                        },
                        new ProductAttributeData()
                        {
                            Name = productAttributeNames[2],
                            Value = productAttributeValues[2]
                        },
                        new ProductAttributeData()
                        {
                            Name = productAttributeNames[3],
                            Value = productAttributeValues[3]
                        }
                    }
                },
                new ERPProduct{
                    Name = secondProductName,
                    Description = messageProductDescription,
                    Type = $"New{messageType}",
                    ProductType = messageProductType,
                    DefaultUnits = messageProductUnits,
                    IsEnabled = messageProductIsEnabled,
                    Yield = messageProductYield,
                    ProductGroup = messageProductGroup,
                    MaximumMaterialSize = messageProductMaximumMaterialSize,
                    ProductParametersData = productParameterData,
                    ProductAttributesData = new List<ProductAttributeData>()
                    {
                        new ProductAttributeData()
                        {
                            Name = productAttributeNames[0],
                            Value = productAttributeValues[0]
                        },
                        new ProductAttributeData()
                        {
                            Name = productAttributeNames[1],
                            Value = productAttributeValues[1]
                        },
                        new ProductAttributeData()
                        {
                            Name = productAttributeNames[2],
                            Value = productAttributeValues[2]
                        },
                        new ProductAttributeData()
                        {
                            Name = productAttributeNames[3],
                            Value = productAttributeValues[3]
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

            ///<Step> Validate creation of Integration Entries </Step>

            ///<Step> Validate only one product was created </Step>
            Product firstProduct = new Product();
            firstProduct.Load(firstProductName);
            firstProduct.LoadAttributes(new Collection<string>() { productAttributeNames[0], productAttributeNames[1], productAttributeNames[2] });
            firstProduct.LoadRelation("ProductParameter");
            firstProduct.ProductGroup.Load();

            Product secondProduct = new Product();
            secondProduct.Load(secondProductName);

            ///<Step> Validate product properties </Step>
            Assert.IsTrue(firstProduct.Name.Equals(firstProductName), $"");
            Assert.IsTrue(firstProduct.Description.Equals(messageProductDescription), $"");
            Assert.IsTrue(firstProduct.Type.Equals(messageType), $"");
            Assert.IsTrue(firstProduct.ProductType.ToString().Equals(messageType), $"");
            Assert.IsTrue(firstProduct.DefaultUnits.Equals(messageProductUnits), $"");
            Assert.IsTrue(firstProduct.IsEnabled, $"");
            Assert.IsTrue(firstProduct.Yield.ToString().Equals(messageProductYield), $"");
            Assert.IsTrue(firstProduct.ProductGroup.Name.Equals(messageProductGroup), $"");
            Assert.IsTrue(firstProduct.MaximumMaterialSize.ToString().Equals(messageProductMaximumMaterialSize), $"");
            Assert.IsTrue(firstProduct.HasRelation("ProductParameter"), "Product should have relations for product parameters");
            Assert.IsTrue(firstProduct.RelationCollection["ProductParameter"].Count == 3, $"Product Parameter relation count should be 3, but was {firstProduct.RelationCollection["ProductParameter"].Count}");
            Assert.IsTrue(firstProduct.RelatedAttributes.Count == 3 , $"Product attribute count should be 3, but was {firstProduct.RelatedAttributes.Count}");

            ///<Step> Validate product paramters relation </Step>
            List<ProductParameter> partParameters = firstProduct.RelationCollection["ProductParameter"].Cast<ProductParameter>().ToList();

            foreach (string parameterName in parameterData.Keys)
            {
                ProductParameter parameter = (ProductParameter)partParameters.FirstOrDefault(pp => pp.TargetEntity.Name.Equals(parameterName));
                Assert.IsTrue(parameter.Value.Equals(parameterData[parameterName]));
            }

            ///<Step> Validate product attributes </Step>
            Assert.IsTrue(firstProduct.RelatedAttributes[productAttributeNames[0]].Equals(productAttributeValues[0]), $"Product attribute {productAttributeNames[0]} should be {productAttributeValues[0]}, but was {firstProduct.RelatedAttributes[productAttributeNames[0]]}");
            Assert.IsTrue(firstProduct.RelatedAttributes[productAttributeNames[1]].Equals(productAttributeValues[1]), $"Product attribute {productAttributeNames[1]} should be {productAttributeValues[1]}, but was {firstProduct.RelatedAttributes[productAttributeNames[1]]}");
            Assert.IsTrue(firstProduct.RelatedAttributes[productAttributeNames[2]].Equals(productAttributeValues[2]), $"Product attribute {productAttributeNames[2]} should be {productAttributeValues[2]}, but was {firstProduct.RelatedAttributes[productAttributeNames[2]]}");

        }
    }
}
