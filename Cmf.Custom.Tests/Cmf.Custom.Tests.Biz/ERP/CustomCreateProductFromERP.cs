using Cmf.Custom.Tests.Biz.Common.ERP.Product;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.ERP
{
    [TestClass]
    public class CustomCreateProductFromERP
    {

        private CustomExecutionScenario _scenario;
        private CustomTearDownManager customTeardownManager = null;

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            _scenario = new CustomExecutionScenario();
            customTeardownManager = new CustomTearDownManager();
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (customTeardownManager != null)
                customTeardownManager.TearDownSequentially();

            if (_scenario != null)
            {
                _scenario.CompleteCleanUp();
            }
        }

        /// <summary>
        /// Description:
        ///     - Create 3 Integration Entries
        ///     - The information for one product is incorrect
        ///     - The last product is to update
        /// 
        /// Acceptance Citeria:
        ///     - Process the first Integration Entry that has the correct information
        ///     - Report error on the second integration entry
        ///  
        /// </summary>
        /// <TestCaseID>CustomCreateProductFromERP.CustomCreateProductFromERP_SendERPMessage_CreateProducts</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomCreateProductFromERP_SendERPMessage_CreateProducts()
        {

            string messageProductDescription = "F4653F00050";
            string messageType = "Production";
            string messageProductType = "FINISHED_GOODS";
            string messageProductUnits = "CM2";
            string messageProductIsEnabled = "Y";
            string messageProductYield = "0,8";
            string messageProductGroup = "MG_PW_1500";
            string messageProductMaximumMaterialSize = "24";
            string firstProductName = Guid.NewGuid().ToString("N");
            string secondProductName = Guid.NewGuid().ToString("N");
            string[] productAttributeNames = new string[] { "SAPProductType", "Technology", "Status", "DispoLevel" };
            string[] productAttributeValues = new string[] { "F4653F00050", "PN", "95", "EOL" };

            Dictionary<string, string> parameterData = new Dictionary<string, string>() {
                { "Raster X", "1020,50" },
                { "Raster Y", "1020,60" },
                { "CM2 Average", "164.70" },
                { "Chips Fieldmask", "15816.80" },
                { "Wafer Size", "150,90" },
                { "Chips Whole Wafer", "16916.571" } };

            List<ProductParameterData> productParameterData = new List<ProductParameterData>();

            List<Conversion> Conversions = new List<Conversion>();

            Conversions.Add(new Common.ERP.Product.Conversion()
            {
                FromUnit = "CM2",
                ToUnit = "nm",
                ConversionFactor = "120"
            });

            foreach (string parameterName in parameterData.Keys)
            {
                ProductParameterData parameter = new ProductParameterData()
                {
                    Name = parameterName,
                    Value = parameterData[parameterName]
                };

                productParameterData.Add(parameter);
            }

            List<ERPProduct> productsLists = new List<ERPProduct>() {
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
                    UnitConversionFactors = Conversions,
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

            _scenario.IsToSendProducts = true;

            _scenario.ProductOutput.ProductsData = productsLists;

            _scenario.Setup();

            // Assert that Integration Entries are created and processed
            Assert.IsTrue(_scenario.IntegrationEntries.Count > 0, "Integration Entries should have been created");
            foreach (IntegrationEntry ie in _scenario.IntegrationEntries)
            {
                Assert.IsTrue(ie.IsIntegrationEntryProcessed(), "Integration Entry was not processed. Error Message: {0}", ie.ResultDescription);
            }

            ///<Step> Validate creation of Integration Entries. </Step>
            ///<ExpectedValue> First Integratio Entry should have been created. </ExpectedValue>
            IntegrationEntry firstProductIE = CustomUtilities.GetIntegrationEntry(firstProductName);
            Assert.IsTrue(firstProductIE.IsIntegrationEntryProcessed(), "Integration Entry was not processed. Error Message: {0}", firstProductIE.ResultDescription);

            ///<ExpectedValue> Second IE should fail since the product type does not exists on the system. </ExpectedValue>
            IntegrationEntry secondProductIE = CustomUtilities.GetIntegrationEntry(secondProductName);
            Assert.IsTrue(!secondProductIE.IsIntegrationEntryProcessed(), "Integration Entry was processed.");

            string errorMessage = $"New{messageType}";
            Assert.IsTrue(secondProductIE.ResultDescription.Contains(errorMessage), $"Error message should be: {errorMessage}, but instead is: {secondProductIE.ResultDescription}");

            ///<Step> Validate only one product was created. </Step>
            ///<ExpectedValue> Only the first Product should be created. </ExpectedValue>
            Product firstProduct = new Product();
            firstProduct.Load(firstProductName);
            firstProduct.LoadAttributes(new Collection<string>() { productAttributeNames[0], productAttributeNames[1], productAttributeNames[2] });
            firstProduct.LoadRelation("ProductParameter");
            firstProduct.ProductGroup.Load();
            customTeardownManager.Push(firstProduct);

            ///<Step> Validate product properties. </Step>
            ///<ExpectedValue> Product should have the information sent on the ERP message. </ExpectedValue>
            Assert.IsTrue(firstProduct.Name.Equals(firstProductName), $"Product Name should be: {firstProduct.Name}, instead is: {firstProductName}.");
            Assert.IsTrue(firstProduct.Description.Equals(messageProductDescription), $"Product Description should be: {firstProduct.Description}, instead is: {messageProductDescription}.");
            Assert.IsTrue(firstProduct.Type.Equals(messageType), $"Product Type should be: {firstProduct.Type}, instead is: {messageType}.");
            Assert.IsTrue(firstProduct.ProductType.ToString().Equals("FinishedGood"), $"Product ProductType should be: {firstProduct.ProductType}, instead is: FinishedGood.");
            Assert.IsTrue(firstProduct.DefaultUnits.Equals(messageProductUnits), $"Product DefaultUnits should be: {firstProduct.DefaultUnits}, instead is: {messageProductUnits}.");
            Assert.IsTrue(firstProduct.IsEnabled, $"Product should be enabled.");
            Assert.IsTrue(string.Format("{0:0.##}", firstProduct.Yield).Equals(CustomUtilities.GetValueAsDecimal(messageProductYield).ToString()), $"Product Yield should be: {string.Format("{0:0.##}", firstProduct.Yield)}, instead is: {CustomUtilities.GetValueAsDecimal(messageProductYield)}.");
            Assert.IsTrue(firstProduct.ProductGroup.Name.Equals(messageProductGroup), $"Product Product Group Name should be: {firstProduct.ProductGroup.Name}, instead is: {messageProductGroup}.");
            Assert.IsTrue(string.Format("{0:0.##}", firstProduct.MaximumMaterialSize).Equals(CustomUtilities.GetValueAsNullableDecimal(messageProductMaximumMaterialSize).ToString()), $"Product Maximum Material size should be: {string.Format("{0:0.##}", firstProduct.MaximumMaterialSize)}, instead is: {CustomUtilities.GetValueAsNullableDecimal(messageProductMaximumMaterialSize)}.");
            Assert.IsTrue(firstProduct.HasRelation("ProductParameter"), "Product should have relations for product parameters");

            ///<Step> Validate product paramters relation. </Step>
            ///<ExpectedValue> Product should have 2 parameters added and one updated. </ExpectedValue>
            List<ProductParameter> partParameters = firstProduct.RelationCollection["ProductParameter"].Cast<ProductParameter>().ToList();
            partParameters.Load(1);

            foreach (string parameterName in parameterData.Keys)
            {
                if (partParameters.Any(pp => pp.TargetEntity.Name.Equals(parameterName)))
                {
                    ProductParameter parameter = (ProductParameter)partParameters.FirstOrDefault(pp => pp.TargetEntity.Name.Equals(parameterName));
                    Assert.IsTrue(CustomUtilities.GetParameterValueAsDataType(parameter.TargetEntity.DataType, parameter.Value).ToString().Equals(CustomUtilities.GetParameterValueAsDataType(parameter.TargetEntity.DataType, parameterData[parameterName]).ToString()));
                }
            }

            ///<Step> Validate product attributes. </Step>
            ///<ExpectedValue> The 3 product attributes should be updated. </ExpectedValue>
            Assert.IsTrue(firstProduct.RelatedAttributes[productAttributeNames[0]].Equals(productAttributeValues[0]), $"Product attribute {productAttributeNames[0]} should be {productAttributeValues[0]}, but was {firstProduct.RelatedAttributes[productAttributeNames[0]]}");
            Assert.IsTrue(firstProduct.RelatedAttributes[productAttributeNames[1]].Equals(productAttributeValues[1]), $"Product attribute {productAttributeNames[1]} should be {productAttributeValues[1]}, but was {firstProduct.RelatedAttributes[productAttributeNames[1]]}");
            Assert.IsTrue(firstProduct.RelatedAttributes[productAttributeNames[2]].Equals(productAttributeValues[2]), $"Product attribute {productAttributeNames[2]} should be {productAttributeValues[2]}, but was {firstProduct.RelatedAttributes[productAttributeNames[2]]}");
        }
    }
}
