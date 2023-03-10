using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.TestUtilities;
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
    public class CustomGenerateSplitLotNames
    {
        private MaterialCollection splittedMaterials;
        private int numberOfCharacters = 2;
        private string materialSubString;


        /// Test Initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialization()
        {
            this.splittedMaterials = new MaterialCollection();
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (splittedMaterials.Any())
            {
                splittedMaterials.Load();
                splittedMaterials.TerminateMaterialCollection();
            }
        }

        /// <summary>
        /// Description:
        ///     - Create Production Lot 
        ///     - Split the lot
        ///     - Validate the name of the child materials
        /// 
        /// Acceptance Citeria:
        ///     - Child Material Name is following the specifed tokens: 
        ///       - [First 8 chars of parent lot name][2 digits of alphanumeric counter]
        /// 
        /// </summary>
        /// <TestCaseID>CustomGenerateSplitLotNames.CustomGenerateSplitLotNames_SplitMaterials_HappyPath</TestCaseID>
        /// <Author>David Guilherme</Author>
        [TestMethod]
        public void CustomGenerateSplitLotNames_SplitMaterials_HappyPath()
        {
            List<string> generatedSplitNames = GenerateLotNames(25);
            splittedMaterials = this.SplitLotNameGeneratorScenario(amsOSRAMConstants.DefaultMaterialFormName);
            for (int i = 0; i < generatedSplitNames.Count; i++)
            {
                string expectedSplitLotName = string.Format("{0}{1}", materialSubString, generatedSplitNames[i]);
                Assert.AreEqual(expectedSplitLotName, splittedMaterials[i].Name, $"Lot name doesn't not match with the expected name: {expectedSplitLotName}.");
            }
        }

        /// <summary>
        /// Split Lot Name Scenario
        /// </summary>
        /// <param name="materialForm">Material Form</param>
        /// <returns>Lot Split Materials</returns>
        private MaterialCollection SplitLotNameGeneratorScenario(string materialFormName)
        {
            Material material = new Material()
            {
                Name = null,
                Facility = GenericGetsScenario.GetObjectByName<Facility>(amsOSRAMConstants.DefaultFacilityName),
                Product = GenericGetsScenario.GetObjectByName<Product>(amsOSRAMConstants.DefaultTestProductName),
                Type = amsOSRAMConstants.MaterialTypeProduction,
                FlowPath = new GetCorrelationIdFlowPathInput
                {
                    SequenceFlowPath = amsOSRAMConstants.DefaultTestFlowPath
                }.GetCorrelationIdFlowPathSync().CorrelationIdFlowPath,
                Form = materialFormName,
                PrimaryQuantity = 25,
                PrimaryUnits = amsOSRAMConstants.DefaultMaterialUnit
            };
            material.Create();
            materialSubString = material.Name.Substring(0,8);
            SplitInputParametersCollection splitInputParametersCollection = new SplitInputParametersCollection();

            for (int i = 0; i < material.PrimaryQuantity; i++)
            {
                SplitInputParameters splitInputParameters = new SplitInputParameters
                {
                    PrimaryQuantity = 1,
                    MaterialContainer = null
                };
                splitInputParametersCollection.Add(splitInputParameters);
            }
            return material.Split(splitInputParametersCollection);
        }

        private List<string> GenerateLotNames(int numberOfNamesToGenerate)
        {
            List<string> names = new List<string>();
            int lastCounterValue = 0;
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
    } 
}
