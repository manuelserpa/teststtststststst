using Cmf.Custom.AMSOsram.Orchestration.InputObjects;
using Cmf.Custom.AMSOsram.Orchestration.OutputObjects;
using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;

namespace Cmf.Custom.Tests.Biz.ERP
{
    [TestClass]
    public class CustomGetFlowInformationForERP
    {
        private string flowVersion = "1";

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
        }

        /// <summary>
        /// Setup Flow Information Scenario
        /// </summary>
        /// <param name="productName">Input ProductName</param>
        /// <param name="flowName">Input FlowName</param>
        /// <param name="flowVersion">Input FlowVersion</param>
        /// <returns></returns>
        private XmlDocument GetFlowInformationScenario(string productName = null, string flowName = null, string flowVersion = null)
        {
            // Call service
            CustomGetFlowInformationForERPOutput output = new CustomGetFlowInformationForERPInput()
            {
                ProductName = productName,
                FlowName = flowName,
                FlowVersion = flowVersion
            }.GetFlowInformationForERPSync();

            // Load Xml
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(output.FlowInformationXml);

            // Remove XmlDeclaration
            xml.RemoveChild(xml.FirstChild);

            return xml;
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
            foreach (XmlNode flowInfoToERPNode in GetFlowInformationScenario(productName: AMSOsramConstants.DefaultTestProductName))
            {
                Assert.IsTrue(flowInfoToERPNode != null && flowInfoToERPNode.HasChildNodes,
                              $"The element {AMSOsramConstants.FlowInformationToERPNodeName} should have values in the message.");

                if (flowInfoToERPNode.Name.Equals(AMSOsramConstants.FlowInformationToERPNodeName))
                {
                    #region General Asserts

                    ValidateGeneralProperties(flowInfoToERPNode);

                    #endregion General Asserts

                    #region Product Asserts

                    XmlNode productInfoNode = flowInfoToERPNode.SelectSingleNode(AMSOsramConstants.ProductInformationNodeName);

                    Assert.IsTrue(productInfoNode != null && productInfoNode.HasChildNodes,
                                  $"The element {AMSOsramConstants.ProductInformationNodeName} should have values in the message.");

                    ValidateBasicProperties(productInfoNode, AMSOsramConstants.ProductInformationNodeName);

                    Assert.IsNotNull(productInfoNode[AMSOsramConstants.ProductInformationMaturityPropertyName],
                                     $"The Product property {AMSOsramConstants.ProductInformationMaturityPropertyName} should exist in the element.");

                    Assert.IsNotNull(productInfoNode[AMSOsramConstants.ProductInformationCycleTimePropertyName],
                                     $"The Product property {AMSOsramConstants.ProductInformationCycleTimePropertyName} should exist in the message.");

                    Assert.IsNotNull(productInfoNode[AMSOsramConstants.ProductInformationYieldPropertyName],
                                     $"The Product property {AMSOsramConstants.ProductInformationYieldPropertyName} should exist in the message.");

                    Assert.IsNotNull(productInfoNode[AMSOsramConstants.ProductInformationMaximumMaterialSizePropertyName],
                                     $"The Product property {AMSOsramConstants.ProductInformationMaximumMaterialSizePropertyName} should exist in the message.");

                    Assert.IsNotNull(productInfoNode[AMSOsramConstants.AttributesInformationNodeName],
                                     $"The Product element {AMSOsramConstants.AttributesInformationNodeName} should exist in the message.");

                    Assert.IsNotNull(productInfoNode[AMSOsramConstants.ParametersInformationNodeName],
                                     $"The Product element {AMSOsramConstants.ParametersInformationNodeName} should exist in the message.");

                    // Validate ProductName
                    Assert.AreEqual(AMSOsramConstants.DefaultTestProductName, productInfoNode[AMSOsramConstants.BasicInformationNamePropertyName].InnerText,
                                    $"The Product property {AMSOsramConstants.BasicInformationNamePropertyName} should be the value {AMSOsramConstants.DefaultTestProductName}.");

                    #endregion Product Asserts

                    #region Flow Asserts

                    XmlNode flowInfoNode = flowInfoToERPNode.SelectSingleNode(AMSOsramConstants.FlowInformationNodeName);

                    ValidateFlowProperties(flowInfoNode);

                    #endregion Flow Asserts

                    #region Steps Asserts

                    XmlNode stepsInfoNode = flowInfoNode.SelectSingleNode(AMSOsramConstants.StepsInformationNodeName);

                    Assert.IsTrue(flowInfoNode != null && flowInfoNode.HasChildNodes,
                                  $"The element {AMSOsramConstants.StepsInformationNodeName} should have values in the message.");

                    ValidateStepsProperties(stepsInfoNode);

                    #endregion Steps Asserts
                }
            }
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
            foreach (XmlNode flowInfoToERPNode in this.GetFlowInformationScenario(flowName: AMSOsramConstants.DefaultTestFlowName))
            {
                Assert.IsTrue(flowInfoToERPNode != null && flowInfoToERPNode.HasChildNodes,
                              $"The element {AMSOsramConstants.FlowInformationToERPNodeName} should have values in the message.");

                if (flowInfoToERPNode.Name.Equals(AMSOsramConstants.FlowInformationToERPNodeName))
                {
                    #region General Asserts

                    ValidateGeneralProperties(flowInfoToERPNode);

                    #endregion General Asserts

                    #region Product Asserts

                    XmlNode productInfoNode = flowInfoToERPNode.SelectSingleNode(AMSOsramConstants.ProductInformationNodeName);

                    Assert.IsTrue(productInfoNode != null && !productInfoNode.HasChildNodes,
                                  $"The element {AMSOsramConstants.ProductInformationNodeName} shouldn't have values in the message.");

                    #endregion Product Asserts

                    #region Flow Asserts

                    XmlNode flowInfoNode = flowInfoToERPNode.SelectSingleNode(AMSOsramConstants.FlowInformationNodeName);

                    ValidateFlowProperties(flowInfoNode);

                    // Validate FlowName
                    Assert.AreEqual(AMSOsramConstants.DefaultTestFlowName, flowInfoNode[AMSOsramConstants.BasicInformationNamePropertyName].InnerText,
                                    $"The Flow property {AMSOsramConstants.BasicInformationNamePropertyName} should be the value {AMSOsramConstants.DefaultTestFlowName}.");

                    #endregion Flow Asserts

                    #region Steps Asserts

                    XmlNode stepsInfoNode = flowInfoNode.SelectSingleNode(AMSOsramConstants.StepsInformationNodeName);

                    Assert.IsTrue(flowInfoNode != null && flowInfoNode.HasChildNodes,
                                  $"The element {AMSOsramConstants.StepsInformationNodeName} should have values in the message.");

                    ValidateStepsProperties(stepsInfoNode);

                    #endregion Steps Asserts
                }
            }
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
            foreach (XmlNode flowInfoToERPNode in this.GetFlowInformationScenario(flowName: AMSOsramConstants.DefaultTestFlowName, flowVersion: flowVersion))
            {
                Assert.IsTrue(flowInfoToERPNode != null && flowInfoToERPNode.HasChildNodes,
                              $"The element {AMSOsramConstants.FlowInformationToERPNodeName} should have values in the message.");

                if (flowInfoToERPNode.Name.Equals(AMSOsramConstants.FlowInformationToERPNodeName))
                {
                    #region General Asserts

                    ValidateGeneralProperties(flowInfoToERPNode);

                    #endregion General Asserts

                    #region Product Asserts

                    XmlNode productInfoNode = flowInfoToERPNode.SelectSingleNode(AMSOsramConstants.ProductInformationNodeName);

                    Assert.IsTrue(productInfoNode != null && !productInfoNode.HasChildNodes,
                                  $"The element {AMSOsramConstants.ProductInformationNodeName} shouldn't have values in the message.");

                    #endregion Product Asserts

                    #region Flow Asserts

                    XmlNode flowInfoNode = flowInfoToERPNode.SelectSingleNode(AMSOsramConstants.FlowInformationNodeName);

                    ValidateFlowProperties(flowInfoNode);

                    // Validate FlowName
                    Assert.AreEqual(AMSOsramConstants.DefaultTestFlowName, flowInfoNode[AMSOsramConstants.BasicInformationNamePropertyName].InnerText,
                                    $"The Flow property {AMSOsramConstants.BasicInformationNamePropertyName} should be the value {AMSOsramConstants.DefaultTestFlowName}.");

                    // Validate FlowVersion
                    Assert.AreEqual(flowVersion, flowInfoNode[AMSOsramConstants.FlowInformationVersionPropertyName].InnerText,
                                    $"The Flow property {AMSOsramConstants.FlowInformationVersionPropertyName} should be the value {flowVersion}.");

                    #endregion Flow Asserts

                    #region Steps Asserts

                    XmlNode stepsInfoNode = flowInfoNode.SelectSingleNode(AMSOsramConstants.StepsInformationNodeName);

                    Assert.IsTrue(flowInfoNode != null && flowInfoNode.HasChildNodes,
                                  $"The element {AMSOsramConstants.StepsInformationNodeName} should have values in the message.");

                    ValidateStepsProperties(stepsInfoNode);

                    #endregion Steps Asserts
                }
            }
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
            string localizedMessage = CustomUtilities.GetLocalizedMessageByName(AMSOsramConstants.LocalizedMessageProductNameAndFlowNameAtSameTime);

            try
            {
                this.GetFlowInformationScenario(AMSOsramConstants.DefaultTestProductName, AMSOsramConstants.DefaultTestFlowName);
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
            string localizedMessage = CustomUtilities.GetLocalizedMessageByName(AMSOsramConstants.LocalizedMessageProductNameOrFlowNameNotDefined);

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
            string localizedMessage = CustomUtilities.GetLocalizedMessageByName(AMSOsramConstants.LocalizedMessageFlowVersionWithoutFlowName);

            try
            {
                this.GetFlowInformationScenario(productName: AMSOsramConstants.DefaultTestProductName, flowVersion: flowVersion);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains(localizedMessage),
                              "The error message returned is different from the message defined in the Localized Message.");
            }
        }

        /// <summary>
        /// Validate General Properties
        /// </summary>
        /// <param name="flowInfoPropertiesNode">FlowInformationToERP node properties</param>
        private void ValidateGeneralProperties(XmlNode flowInfoPropertiesNode)
        {
            Assert.IsNotNull(flowInfoPropertiesNode[AMSOsramConstants.FlowInformationToERPSitePropertyName],
                             $"The property {AMSOsramConstants.FlowInformationToERPSitePropertyName} should exist in the element.");

            Assert.IsNotNull(flowInfoPropertiesNode[AMSOsramConstants.FlowInformationToERPCostCenterPropertyName],
                             $"The property {AMSOsramConstants.FlowInformationToERPCostCenterPropertyName} should exist in the element.");
        }

        /// <summary>
        /// Validate Basic Properties
        /// </summary>
        /// <param name="basicInfoPropertiesNode">Basic properties</param>
        /// <param name="parentNodeName">ParentNode name</param>
        private void ValidateBasicProperties(XmlNode basicInfoPropertiesNode, string parentNodeName)
        {
            Assert.IsNotNull(basicInfoPropertiesNode[AMSOsramConstants.BasicInformationNamePropertyName],
                             $"The {parentNodeName} property {AMSOsramConstants.BasicInformationNamePropertyName} should exist in the element.");

            Assert.IsNotNull(basicInfoPropertiesNode[AMSOsramConstants.BasicInformationDescriptionPropertyName],
                             $"The {parentNodeName} property {AMSOsramConstants.BasicInformationDescriptionPropertyName} should exist in the element.");

            Assert.IsNotNull(basicInfoPropertiesNode[AMSOsramConstants.BasicInformationTimestampPropertyName],
                             $"The {parentNodeName} property {AMSOsramConstants.BasicInformationTimestampPropertyName} should exist in the element.");

            Assert.IsNotNull(basicInfoPropertiesNode[AMSOsramConstants.BasicInformationTypePropertyName],
                             $"The {parentNodeName} property {AMSOsramConstants.BasicInformationTypePropertyName} should exist in the element.");

            Assert.IsNotNull(basicInfoPropertiesNode[AMSOsramConstants.BasicInformationStatePropertyName],
                             $"The {parentNodeName} property {AMSOsramConstants.BasicInformationStatePropertyName} should exist in the element.");
        }

        /// <summary>
        /// Validate Flow Properties
        /// </summary>
        /// <param name="flowInfoPropertiesNode">Flow properties</param>
        private void ValidateFlowProperties(XmlNode flowInfoPropertiesNode)
        {
            Assert.IsTrue(flowInfoPropertiesNode != null && flowInfoPropertiesNode.HasChildNodes,
                          $"The element {AMSOsramConstants.FlowInformationNodeName} should have values in the message.");

            ValidateBasicProperties(flowInfoPropertiesNode, AMSOsramConstants.FlowInformationNodeName);

            Assert.IsNotNull(flowInfoPropertiesNode[AMSOsramConstants.FlowInformationVersionPropertyName],
                             $"The Flow property {AMSOsramConstants.FlowInformationVersionPropertyName} should exist in the element.");

            Assert.IsNotNull(flowInfoPropertiesNode[AMSOsramConstants.FlowInformationLogicalNamePropertyName],
                             $"The Flow property {AMSOsramConstants.FlowInformationVersionPropertyName} should exist in the element.");
        }

        /// <summary>
        /// Validate Steps Properties
        /// </summary>
        /// <param name="stepsInfoNodes">Steps nodes</param>
        private void ValidateStepsProperties(XmlNode stepsInfoNodes)
        {
            foreach (XmlNode stepNode in stepsInfoNodes)
            {
                Assert.IsTrue(stepNode != null && stepNode.HasChildNodes,
                              $"The element {AMSOsramConstants.StepInformationNodeName} should have values in the message.");

                ValidateBasicProperties(stepNode, AMSOsramConstants.StepInformationNodeName);

                Assert.IsNotNull(stepNode[AMSOsramConstants.StepInformationLogicalNamePropertyName],
                                 $"The Step property {AMSOsramConstants.StepInformationLogicalNamePropertyName} should exist in the element.");

                Assert.IsNotNull(stepNode[AMSOsramConstants.StepInformationMaturityPropertyName],
                                 $"The Step property {AMSOsramConstants.StepInformationMaturityPropertyName} should exist in the element.");

                Assert.IsNotNull(stepNode[AMSOsramConstants.AttributesInformationNodeName],
                                 $"The Step element {AMSOsramConstants.AttributesInformationNodeName} should exist in the element.");
            }
        }
    }
}