using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.NameGeneratorManagement.InputObjects;
using Cmf.Foundation.Configuration;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.FacilityManagement.FlowManagement.InputObjects;
using Cmf.TestScenarios.Others;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.NameGenerators
{
    [TestClass]
    public class CustomGenerateProductionLotNames
    {
        private const string siteFacilityPrefix = "RP";
        private const int numberOfCharacters = 6;
        private const string productionLineName = "ProdLine2";

        private MaterialCollection materials;

        /// <summary>
        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            this.materials = new MaterialCollection();
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (materials.Any())
            {
                materials.Load();
                materials.TerminateMaterialCollection();
            }
        }

        /// <summary>
        /// Description:
        ///     - Create Production Lot name to the custom Name Generator CustomProductionLotNameGenerator
        /// 
        /// Acceptance Citeria:
        ///     - Production Lot Name following the specifed tokens: 
        ///       - [Site & Facility prefix associated to the ProductionLine attribute][Alphanumeric 6 digits counter][00]
        /// 
        /// </summary>
        /// <TestCaseID>CustomGenerateProductionLotNames_CreateProductionLot_HappyPath</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomGenerateProductionLotNames_CreateProductionLot_HappyPath()
        {
            List<string> generatedNames = GenerateLotNames(25);

            for (int i = 0; i < generatedNames.Count; i++)
            {
                string lotName = LotNameGeneratorScenario(amsOSRAMConstants.FormLot, amsOSRAMConstants.DefaultTestProductName);
                string expectedLotName = string.Format("{0}{1}00", siteFacilityPrefix, generatedNames[i]);
                Assert.AreEqual(expectedLotName, lotName, $"Lot name doesn't not match with the expected name: {expectedLotName}.");
            };
        }

        /// <summary>
        /// Description:
        ///     - Try to create Material without AllowedDigits Configuration
        /// 
        /// Acceptance Citeria:
        ///     - Thow a message associated to the CustomConfigMissingValue LocalizedMessage
        /// 
        /// </summary>
        /// <TestCaseID>CustomGenerateProductionLotNames_CreateProductionLot_ThrowAnErrorWhenAllowedDigitsConfigurationDoesNotExist</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomGenerateProductionLotNames_CreateProductionLot_ThrowAnErrorWhenAllowedDigitsConfigurationDoesNotExist()
        {
            string localizedMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageConfigMissingValue,
                                                                                amsOSRAMConstants.DefaultLotNameAllowedCharacters);


            string configValue = ConfigUtilities.GetConfigValue(amsOSRAMConstants.DefaultLotNameAllowedCharacters) as string;

            try
            {
                ConfigUtilities.RemoveConfigValue(amsOSRAMConstants.DefaultLotNameAllowedCharacters);

                Assert.IsTrue(string.IsNullOrWhiteSpace(ConfigUtilities.GetConfigValue(amsOSRAMConstants.DefaultLotNameAllowedCharacters) as string),
                              "The Config should be an empty value.");

                this.LotNameGeneratorScenario(amsOSRAMConstants.FormLot, amsOSRAMConstants.DefaultTestProductWithoutProductionLineName);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains(localizedMessage),
                              $"The error message returned is different from the message defined in the Localized Message.");
            }
            finally
            {
                // Rollback to the original Config value
                ConfigUtilities.SetConfigValue(amsOSRAMConstants.DefaultLotNameAllowedCharacters, configValue);
            }
        }

        /// <summary>
        /// Description:
        ///     - Create Material Lot with the following characteristics:
        ///       - Name is null
        ///       - Product without configured ProductionLine attribute
        /// 
        /// Acceptance Citeria:
        ///     - Thow a message associated to the CustomProductionLineAttributeWithoutValue LocalizedMessage
        /// 
        /// </summary>
        /// <TestCaseID>CustomGenerateProductionLotNames_CreateProductionLot_ThrowAnErrorWhenProductionLineAttributeWithoutValue</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomGenerateProductionLotNames_CreateProductionLot_ThrowAnErrorWhenProductionLineAttributeWithoutValue()
        {
            string localizedMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageProductionLineAttributeWithoutValue,
                                                                                amsOSRAMConstants.DefaultTestProductWithoutProductionLineName);

            try
            {
                this.LotNameGeneratorScenario(amsOSRAMConstants.FormLot, amsOSRAMConstants.DefaultTestProductWithoutProductionLineName);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains(localizedMessage),
                              $"The error message returned is different from the message defined in the Localized Message.");
            }
        }

        /// <summary>
        /// Description:
        ///     - Create Material Lot with the following characteristics:
        ///       - Name is null
        ///       - ProductionLine associated with Product without configuration in GenericTableCustomProductionLineConversion
        /// 
        /// Acceptance Citeria:
        ///     - Thow a message associated to the CustomGTWihtoutDataForSpecificProductionLine LocalizedMessage
        /// 
        /// </summary>
        /// <TestCaseID>CustomGenerateProductionLotNames_CreateProductionLot_ThrowAnErrorWhenProductionLineIsNotConfigutedOnGT</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomGenerateProductionLotNames_CreateProductionLot_ThrowAnErrorWhenProductionLineIsNotConfigutedOnGT()
        {
            string localizedMessage = CustomUtilities.GetLocalizedMessageByName(amsOSRAMConstants.LocalizedMessageGTWihtoutDataForSpecificProductionLine,
                                                                                amsOSRAMConstants.GenericTableCustomProductionLineConversion,
                                                                                productionLineName);

            try
            {
                this.LotNameGeneratorScenario(amsOSRAMConstants.FormLot, amsOSRAMConstants.DefaultTestProductGTWithoutProductionLineName);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains(localizedMessage),
                              $"The error message returned is different from the message defined in the Localized Message.");
            }
        }

        /// <summary>
        /// Generate Lot Names
        /// </summary>
        /// <param name="numberOfNamesToGenerate">Number of name to generate</param>
        /// <returns>List of Lot Names</returns>
        private List<string> GenerateLotNames(int numberOfNamesToGenerate)
        {
            List<string> names = new List<string>();

            GeneratorContext context = new LoadNameGeneratorContextsInput()
            {
                NameGenerator = GenericGetsScenario.GetObjectByName<NameGenerator>(amsOSRAMConstants.CustomGenerateProductionLotNames)
            }.LoadNameGeneratorContextsSync().NameGenerator?.Contexts.LastOrDefault(c => c.Context == siteFacilityPrefix);

            int lastCounterValue = 0;

            if (context != null)
            {
                lastCounterValue = context.LastCounterValue;
            }

            string lotNameAllowedCharacters = ConfigUtilities.GetConfigValue(amsOSRAMConstants.DefaultLotNameAllowedCharacters) as string;
            int currentCounter = lastCounterValue;
            int allowedCharactersSize = lotNameAllowedCharacters.Length;

            for (int i = 0; i < numberOfNamesToGenerate; i++)
            {
                currentCounter++;
                lastCounterValue = currentCounter;
                string alphanumericCounter = string.Empty;

                for (int y = 0; y < numberOfCharacters; y++)
                {
                    int currLetterInt = lastCounterValue % allowedCharactersSize;
                    alphanumericCounter += (char)lotNameAllowedCharacters[0 + currLetterInt];
                    lastCounterValue /= allowedCharactersSize;
                }

                // Throw an exception case counter has insufficient number of digits
                if (lastCounterValue > 0)
                {
                    throw new Exception("Insufficient number of digits");
                }

                // Revert the counter Chars order
                char[] counterChars = alphanumericCounter.ToCharArray();
                Array.Reverse(counterChars);
                alphanumericCounter = new string(counterChars);

                names.Add(alphanumericCounter);
            }

            return names;
        }

        /// <summary>
        /// Lot Name Generator Scenario
        /// </summary>
        /// <param name="materialForm">Material Form</param>
        /// <param name="productName">ProductName to associate</param>
        /// <returns>Material name</returns>
        private string LotNameGeneratorScenario(string materialForm, string productName)
        {
            Material material = new Material()
            {
                Name = null,
                Facility = GenericGetsScenario.GetObjectByName<Facility>(amsOSRAMConstants.DefaultFacilityName),
                Product = GenericGetsScenario.GetObjectByName<Product>(productName),
                Type = amsOSRAMConstants.MaterialTypeProduction,
                FlowPath = new GetCorrelationIdFlowPathInput
                {
                    SequenceFlowPath = amsOSRAMConstants.DefaultTestFlowPath
                }.GetCorrelationIdFlowPathSync().CorrelationIdFlowPath,
                Form = materialForm,
                PrimaryQuantity = 10,
                PrimaryUnits = "CM2"
            };
            material.Create();
            materials.Add(material);

            return material.Name;
        }
    }
}
