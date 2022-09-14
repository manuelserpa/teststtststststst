using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.NameGeneratorManagement.InputObjects;
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
        ///       - [Site][2 digits for the fiscal year][2 digits for the fiscal week][Alphanumeric running number] 
        /// 
        /// </summary>
        /// <TestCaseID>CustomGenerateProductionLotNames.CustomGenerateProductionLotNames_CreateProductionLot_HappyPath</TestCaseID>
        /// <Author>André Cruz</Author>
        [TestMethod]
        public void CustomGenerateProductionLotNames_CreateProductionLot_HappyPath()
        {
            List<string> generatedNames = GenerateLotNames(25);

            for (int i = 0; i < generatedNames.Count; i++)
            {
                string lotName = LotNameGeneratorScenario(AMSOsramConstants.FormLot, AMSOsramConstants.DefaultTestProductName);
                string expectedLotName = string.Format("{0}{1}00", siteFacilityPrefix, generatedNames[i]);
                Assert.AreEqual(expectedLotName, lotName, $"Lot name doesn't not match with the expected name: {expectedLotName}.");
            };
        }

        private List<string> GenerateLotNames(int numberOfNamesToGenerate)
        {
            List<string> names = new List<string>();

            GeneratorContext context = new LoadNameGeneratorContextsInput()
            {
                NameGenerator = GenericGetsScenario.GetObjectByName<NameGenerator>(AMSOsramConstants.CustomGenerateProductionLotNames)
            }.LoadNameGeneratorContextsSync().NameGenerator?.Contexts.LastOrDefault(c => c.Context == siteFacilityPrefix);

            int lastCounterValue = 0;

            if (context != null)
            {
                lastCounterValue = context.LastCounterValue;
            }

            string lotNameAllowedCharacters = ConfigUtilities.GetConfigValue(AMSOsramConstants.DefaultLotNameAllowedCharacters) as string;
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

        private string LotNameGeneratorScenario(string materialForm, string productName)
        {
            Material material = new Material()
            {
                Name = null,
                Facility = GenericGetsScenario.GetObjectByName<Facility>(AMSOsramConstants.DefaultFacilityName),
                Product = GenericGetsScenario.GetObjectByName<Product>(productName),
                Type = AMSOsramConstants.MaterialTypeProduction,
                FlowPath = new GetCorrelationIdFlowPathInput
                {
                    SequenceFlowPath = AMSOsramConstants.DefaultTestFlowPath
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
