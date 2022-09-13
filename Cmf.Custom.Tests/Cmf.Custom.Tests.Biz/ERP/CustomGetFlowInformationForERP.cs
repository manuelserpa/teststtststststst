using Cmf.Custom.amsOSRAM.Orchestration.InputObjects;
using Cmf.Custom.amsOSRAM.Orchestration.OutputObjects;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.ERP.Flow;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.BusinessOrchestration.Utilities.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Cmf.Custom.Tests.Biz.Common.ERP.Flow.CustomFlowInformationToERPData;

namespace Cmf.Custom.Tests.Biz.ERP
{
    [TestClass]
    public class CustomGetFlowInformationForERP
    {
        /// <summary>
        /// Setup Flow Information Scenario
        /// </summary>
        /// <param name="productName">Input ProductName</param>
        /// <param name="flowName">Input FlowName</param>
        /// <param name="flowVersion">Input FlowVersion</param>
        /// <returns>Object with XML Data</returns>
        private CustomFlowInformationToERPData GetFlowInformationScenario(string productName = null, string flowName = null, string flowVersion = null)
        {
            CustomFlowInformationToERPData flowInfoData = new CustomFlowInformationToERPData();

            // Call service
            CustomGetFlowInformationForERPOutput output = new CustomGetFlowInformationForERPInput()
            {
                ProductName = productName,
                FlowName = flowName,
                FlowVersion = flowVersion
            }.GetFlowInformationForERPSync();

            Assert.IsFalse(string.IsNullOrWhiteSpace(output.ResultXml), "The ResultXml should have value.");

            // Deserliaze service output message for an object
            flowInfoData = CustomUtilities.DeserializeXmlToObject<CustomFlowInformationToERPData>(output.ResultXml);

            return flowInfoData;
        }

        /// <summary>
        /// Description:
        ///     - Get Flow information using ProductName
        /// 
        /// Acceptance Citeria:
        ///     - The Product, Flow and Steps information is returned in correct format in the XML message.
        /// </summary>
        /// <TestCaseID>CustomGetFlowInformationForERP_GetFlowInformationForERP_FlowInformationByProductName</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomGetFlowInformationForERP_GetFlowInformationForERP_FlowInformationByProductName()
        {
            CustomFlowInformationToERPData flowInfoData = this.GetFlowInformationScenario(productName: amsOSRAMConstants.DefaultTestProductName);

            Product product = new Product() { Name = flowInfoData.ProductInformationData.Name };
            product.Load();

            #region Product Asserts

            ValidateProductProperties(flowInfoData.ProductInformationData, product);

            #endregion Product Asserts

            #region Flow Asserts

            product.Flow.Load();

            ValidateFlowProperties(flowInfoData.FlowInformationData, product.Flow);

            #endregion Flow Asserts

            #region Steps Asserts

            ValidateStepsProperties(flowInfoData.FlowInformationData.Steps, product.Flow);

            #endregion Steps Asserts

            #region General Asserts

            ValidateGeneralProperties(flowInfoData, product.Flow, product);

            #endregion General Asserts
        }

        /// <summary>
        /// Description:
        ///     - Get Flow information using FlowName
        /// 
        /// Acceptance Citeria:
        ///     - The Flow and Steps information is returned in correct format in the XML message.
        /// </summary>
        /// <TestCaseID>CustomGetFlowInformationForERP_GetFlowInformationForERP_FlowInformationByFlowName</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomGetFlowInformationForERP_GetFlowInformationForERP_FlowInformationByFlowName()
        {
            CustomFlowInformationToERPData flowInfoData = this.GetFlowInformationScenario(flowName: amsOSRAMConstants.DefaultTestFlowName);

            Flow flow = new Flow() { Name = flowInfoData.FlowInformationData.Name };
            flow.Load();

            #region Product Asserts

            Assert.IsNull(flowInfoData.ProductInformationData, "The Product shouldn't have values in the message.");

            #endregion Product Asserts

            #region Flow Asserts

            ValidateFlowProperties(flowInfoData.FlowInformationData, flow);

            #endregion Flow Asserts

            #region Steps Asserts

            ValidateStepsProperties(flowInfoData.FlowInformationData.Steps, flow);

            #endregion Steps Asserts

            #region General Asserts

            ValidateGeneralProperties(flowInfoData, flow);

            #endregion General Asserts

        }

        /// <summary>
        /// Description:
        ///     - Get information of a specific Flow Version
        /// 
        /// Acceptance Citeria:
        ///     - The Flow and Steps information is returned in correct format in the XML message.
        /// </summary>
        /// <TestCaseID>CustomGetFlowInformationForERP_GetFlowInformationForERP_FlowInformationOfSpecificVersion</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomGetFlowInformationForERP_GetFlowInformationForERP_FlowInformationOfSpecificVersion()
        {
            CustomFlowInformationToERPData flowInfoData = this.GetFlowInformationScenario(flowName: amsOSRAMConstants.DefaultTestFlowName, flowVersion: "1");

            Flow flow = new Flow() { Name = flowInfoData.FlowInformationData.Name, Version = Convert.ToInt32(flowInfoData.FlowInformationData.Version) };
            flow.Load();

            #region Product Asserts

            Assert.IsNull(flowInfoData.ProductInformationData, "The Product shouldn't have values in the message.");

            #endregion Product Asserts

            #region Flow Asserts

            ValidateFlowProperties(flowInfoData.FlowInformationData, flow);

            #endregion Flow Asserts

            #region Steps Asserts

            ValidateStepsProperties(flowInfoData.FlowInformationData.Steps, flow);

            #endregion Steps Asserts

            #region General Asserts

            ValidateGeneralProperties(flowInfoData, flow);

            #endregion General Asserts
        }

        /// <summary>
        /// Description:
        ///     - Get Flow information sending ProductName and FlowName filled in the Input.
        /// 
        /// Acceptance Citeria:
        ///     - Exception message is returned.
        /// </summary>
        /// <TestCaseID>CustomGetFlowInformationForERP_GetFlowInformationForERP_ThrowAnErrorWhenProductNameAndFlowNameAreBothFilled</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomGetFlowInformationForERP_GetFlowInformationForERP_ThrowAnErrorWhenProductNameAndFlowNameAreBothFilled()
        {
            string localizedMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageProductNameAndFlowNameAtSameTime);

            try
            {
                this.GetFlowInformationScenario(amsOSRAMConstants.DefaultTestProductName, amsOSRAMConstants.DefaultTestFlowName);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains(localizedMessage),
                              "The error message returned is different from the message defined in the Localized Message.");
            }
        }

        /// <summary>
        /// Description:
        ///     - Get Flow information using null values in the Input.
        /// 
        /// Acceptance Citeria:
        ///     - Exception message is returned.
        /// </summary>
        /// <TestCaseID>CustomGetFlowInformationForERP_GetFlowInformationForERP_ThrowAnErrorWhenProductNameAndFlowNameIsNotFilled</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomGetFlowInformationForERP_GetFlowInformationForERP_ThrowAnErrorWhenProductNameAndFlowNameIsNotFilled()
        {
            string localizedMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageProductNameOrFlowNameNotDefined);

            try
            {
                this.GetFlowInformationScenario();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains(localizedMessage),
                              "The error message returned is different from the message defined in the Localized Message.");
            }
        }

        /// <summary>
        /// Description:
        ///     - Get Flow information sending only the FlowVersion filled in the Input.
        /// 
        /// Acceptance Citeria:
        ///     - Exception message is returned.
        /// </summary>
        /// <TestCaseID>CustomGetFlowInformationForERP_GetFlowInformationForERP_ThrowAnErrorWhenFlowVersionIsFilledButFlowNameIsNullOrEmpty</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomGetFlowInformationForERP_GetFlowInformationForERP_ThrowAnErrorWhenFlowVersionIsFilledButFlowNameIsNullOrEmpty()
        {
            string localizedMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageFlowVersionWithoutFlowName);

            try
            {
                this.GetFlowInformationScenario(productName: amsOSRAMConstants.DefaultTestProductName, flowVersion: "1");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains(localizedMessage),
                              "The error message returned is different from the message defined in the Localized Message.");
            }
        }

        /// <summary>
        /// Validate General properties
        /// </summary>
        /// <param name="flowInfoData">Properties from deserialized object</param>
        /// <param name="flow">Flow entity</param>
        /// <param name="product">Product entity</param>
        private void ValidateGeneralProperties(CustomFlowInformationToERPData flowInfoData, Flow flow, Product product = null)
        {
            // Validate Site name associated to the ProductionLine attribute
            if (product != null && product.HasRelatedAttributeDefined(amsOSRAMConstants.ProductAttributeProductionLine))
            {
                string productionLine = product.GetRelatedAttributeValue<Product, string>(amsOSRAMConstants.ProductAttributeProductionLine);

                if (!string.IsNullOrWhiteSpace(productionLine))
                {
                    // Load Generic Table CustomProductionLineConversion
                    GenericTable customProdLineConversionGT = new GenericTable() { Name = amsOSRAMConstants.GenericTableCustomProductionLineConversion };
                    customProdLineConversionGT.Load();

                    // Based on ProductLine Product attribute get Site and Facility name from Generic Table
                    customProdLineConversionGT.LoadData(new Foundation.BusinessObjects.QueryObject.FilterCollection()
                    {
                        new Foundation.BusinessObjects.QueryObject.Filter()
                        {
                            Name = amsOSRAMConstants.GenericTableCustomProductionLineConversionProductionLineProperty,
                            Operator = FieldOperator.IsEqualTo,
                            LogicalOperator = LogicalOperator.Nothing,
                            Value = productionLine
                        }
                    });

                    if (customProdLineConversionGT.HasData)
                    {
                        DataSet prodLineConversionDataSet = new ToDataSetInput()
                        {
                            NgpDataSet = customProdLineConversionGT.Data
                        }.ToDataSetSync().DataSet;

                        string flowInfoDataSite = prodLineConversionDataSet.Tables[0].Rows[0][amsOSRAMConstants.GenericTableCustomProductionLineConversionSiteProperty].ToString();

                        Assert.AreEqual(flowInfoData.Site, flowInfoDataSite, $"The Site should be the value {flowInfoData.Site}.");
                    }
                }
            }

            flow.LoadRelation(nameof(FlowStep));

            FlowStepCollection flowSteps = flow.FlowSteps;

            if (flowSteps != null && flowSteps.Any())
            {
                flowSteps.FirstOrDefault().TargetEntity?.LoadRelation(nameof(StepArea));

                string costCenterName = flowSteps.FirstOrDefault().TargetEntity?.StepAreas?.FirstOrDefault()?.TargetEntity?.CostCenter;

                Assert.AreEqual(flowInfoData.CostCenter, costCenterName, $"The CostCenter should be the value {flowInfoData.CostCenter}.");
            }
        }

        /// <summary>
        /// Validate BasicInformation properties
        /// </summary>
        /// <param name="basicInfoPropertiesNode">BasicInformation from deserialized object</param>
        /// <param name="entity">Entiy base</param>
        private void ValidateBasicProperties(BasicInformation basicInfoPropertiesNode, EntityBase entity)
        {
            Assert.AreEqual(basicInfoPropertiesNode.Name, entity.Name,
                            $"The Name should be the value {basicInfoPropertiesNode.Name}.");

            Assert.AreEqual(basicInfoPropertiesNode.Description, entity.Description,
                            $"The Description should be the value {basicInfoPropertiesNode.Description}.");

            Assert.AreEqual(basicInfoPropertiesNode.Timestamp, entity.CreatedOn.ToString(),
                            $"The Timestamp should be the value {basicInfoPropertiesNode.Timestamp}.");

            Assert.AreEqual(basicInfoPropertiesNode.State, entity.UniversalState.ToString(),
                            $"The State should be the value {basicInfoPropertiesNode.State}.");
        }

        /// <summary>
        /// Validate Product properties
        /// </summary>
        /// <param name="productInfoData">ProductInformation from deserialized object</param>
        /// <param name="product">Product entity</param>
        private void ValidateProductProperties(ProductInformation productInfoData, Product product)
        {
            // Validate Basic properties
            ValidateBasicProperties(productInfoData, product);

            // Validate Product properties
            Assert.AreEqual(productInfoData.Type, product.Type,
                            $"The Product Type should be the value {productInfoData.Type}.");

            Assert.AreEqual(productInfoData.Maturity, product.Maturity,
                            $"The Product Maturity should be the value {productInfoData.Maturity}.");

            Assert.AreEqual(productInfoData.CycleTime, product.CycleTime.ToString(),
                            $"The Product CycleTime should be the value {productInfoData.CycleTime}.");

            Assert.AreEqual(productInfoData.Yield, product.Yield.ToString(),
                            $"The Product Yield should be the value {productInfoData.Yield}.");

            Assert.AreEqual(productInfoData.MaximumMaterialSize, product.MaximumMaterialSize.ToString(),
                            $"The Product MaximumMaterialSize should be the value {productInfoData.MaximumMaterialSize}.");

            product.LoadAttributes();

            // Validate Product attributes
            if (product.Attributes != null && product.Attributes.Any())
            {
                foreach (AttributeInformation attributeInformation in productInfoData.ProductAttributes)
                {
                    Assert.AreEqual(attributeInformation.Value, product.Attributes[attributeInformation.Name].ToString(),
                                    $"The Product Attribute {attributeInformation.Name} should be the value {attributeInformation.Value}.");
                }
            }

            // Validate Product related attributes
            if (product.RelatedAttributes != null && product.RelatedAttributes.Any())
            {
                foreach (AttributeInformation attributeInformation in productInfoData.ProductAttributes)
                {
                    Assert.AreEqual(attributeInformation.Value, product.RelatedAttributes[attributeInformation.Name].ToString(),
                                    $"The Product Related Attribute {attributeInformation.Name} should be the value {attributeInformation.Value}.");
                }
            }

            product.LoadRelation(nameof(ProductParameter));

            if (product.HasRelation(nameof(ProductParameter)))
            {
                List<ProductParameter> productParameters = product.RelationCollection[nameof(ProductParameter)].Cast<ProductParameter>().ToList();

                //Validate Product parameters
                List<ParameterInformation> productParametersInfoData = productInfoData.ProductParameters;

                for (int i = 0; i < productParametersInfoData.Count; i++)
                {
                    productParameters[i].TargetEntity.Load();

                    string parameterName = productParameters[i].TargetEntity.Name;
                    string parameterValue = productParameters.FirstOrDefault(p => p.TargetEntity.Name == parameterName).Value.ToString();
                    string parameterType = productParameters[i].Type.ToString();

                    Assert.AreEqual(productInfoData.ProductParameters[i].Name, parameterName,
                                    $"The name of Product Parameter should be {productInfoData.ProductParameters[i].Name}.");

                    Assert.AreEqual(productInfoData.ProductParameters[i].Value, parameterValue,
                                    $"The Product Parameter {productInfoData.ProductParameters[i].Name} should be the Value {productInfoData.ProductParameters[i].Value}.");

                    Assert.AreEqual(productInfoData.ProductParameters[i].Type, parameterType,
                                    $"The Product Parameter {productInfoData.ProductParameters[i].Name} should be the Type {productInfoData.ProductParameters[i].Type}.");
                }
            }
        }

        /// <summary>
        /// Validate Flow properties
        /// </summary>
        /// <param name="flowInfoData">FlowInformation from deserialized objec</param>
        /// <param name="flow">Flow entity</param>
        private void ValidateFlowProperties(FlowInformation flowInfoData, Flow flow)
        {
            // Validate Basic properties
            ValidateBasicProperties(flowInfoData, flow);

            // Validate Flow properties
            Assert.AreEqual(flowInfoData.Type, flow.Type,
                            $"The Flow Type should be the value {flowInfoData.Type}.");

            Assert.AreEqual(flowInfoData.LogicalName, flow.LogicalNames?.FirstOrDefault()?.LogicalName,
                            $"The Flow LogicalName should be the value {flowInfoData.LogicalName}.");

            Assert.AreEqual(flowInfoData.Version, flow.Version.ToString(),
                            $"The Flow Version should be the value {flowInfoData.Version}.");
        }

        /// <summary>
        /// Validate FlowSteps properties
        /// </summary>
        /// <param name="flowStepsInfoData">FlowSteps from deserialized object</param>
        /// <param name="flow">Flow entity</param>
        private void ValidateStepsProperties(List<StepInformation> flowStepsInfoData, Flow flow)
        {
            flow.LoadRelation(nameof(FlowStep));

            FlowStepCollection flowSteps = flow.FlowSteps;

            if (flowSteps != null && flowSteps.Any())
            {
                for (int i = 0; i < flowStepsInfoData.Count; i++)
                {
                    flowSteps[i].TargetEntity?.Load();

                    // Validate Basic properties
                    ValidateBasicProperties(flowStepsInfoData[i], flowSteps[i].TargetEntity);

                    // Validate Step properties
                    Assert.AreEqual(flowStepsInfoData[i].Type, flowSteps[i].TargetEntity?.Type,
                                    $"The FlowStep Type should be the value {flowStepsInfoData[i].Type}.");

                    Assert.AreEqual(flowStepsInfoData[i].LogicalName, flowSteps[i].LogicalName,
                                    $"The FlowStep LogicalName should be the value {flowStepsInfoData[i].LogicalName}.");

                    Assert.AreEqual(flowStepsInfoData[i].Maturity, flowSteps[i].TargetEntity?.Maturity,
                                    $"The FlowStep Maturity should be the value {flowStepsInfoData[i].Maturity}.");

                    // Validate Step attributes
                    flowSteps[i].TargetEntity?.LoadAttributes();

                    if (flowSteps[i].TargetEntity.Attributes != null && flowSteps[i].TargetEntity.Attributes.Any())
                    {
                        foreach (AttributeInformation attributeInformation in flowStepsInfoData[i].StepAttributes)
                        {
                            Assert.AreEqual(attributeInformation.Value, flowSteps[i].TargetEntity.Attributes[attributeInformation.Name].ToString(),
                                            $"The Step Attribute {attributeInformation.Name} should be the value {attributeInformation.Value}.");
                        }
                    }
                }
            }
        }
    }
}
