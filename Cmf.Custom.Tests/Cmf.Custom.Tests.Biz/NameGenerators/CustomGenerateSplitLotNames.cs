using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.NameGeneratorManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.NameGeneratorManagement.OutputObjects;
using Cmf.Foundation.Configuration;
using Cmf.Navigo.BusinessObjects;
using Cmf.Navigo.BusinessOrchestration.FacilityManagement.FlowManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.InputObjects;
using Cmf.Navigo.BusinessOrchestration.MaterialManagement.OutputObjects;
using Cmf.TestScenarios.Others;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmf.Custom.Tests.Biz.NameGenerators
{
    [TestClass]
    public class CustomGenerateSplitLotNames
    {
        private string facilityName = "Regensburg Production";
        private string productName = "11111335";
        private string type = "Production";
        private string flowName = "FOL-UX3_EPA";
        private string stepName = "M2-SL-Wafer-Start-07301F001_E";
        private string materialFormName = "Lot";
        private string defaultUnit = "CM2";

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
            // Instance for Material Collection
            MaterialCollection materials = new MaterialCollection();


            try
            {
                ///<Step> Create a material </Step>
                Material material = new Material()
                {
                    Name = Guid.NewGuid().ToString("N"),
                    Facility = GenericGetsScenario.GetObjectByName<Facility>(facilityName),
                    Product = GenericGetsScenario.GetObjectByName<Product>(productName),
                    Type = type,
                    FlowPath = new GetCorrelationIdFlowPathInput
                    {
                        SequenceFlowPath = flowName + ":1/" + stepName + @":1"
                    }.GetCorrelationIdFlowPathSync().CorrelationIdFlowPath,
                    Form = materialFormName,
                    PrimaryQuantity = 10,
                    PrimaryUnits = defaultUnit
                };
                material.Create();
                materials.Add(material);

                // Generation of alphanumeric values
                List<string> alphanumericLotNames = GenerateLotNames(3, material.Name);
                int alphanumericLotNAmesIdx = 0;
                ///<Step> Split the material </Step>
                SplitInputParametersCollection splitInputParametersCollection = new SplitInputParametersCollection
                {
                    new SplitInputParameters
                    {
                        PrimaryQuantity = 4,
                        MaterialContainer = null
                    },
                    new SplitInputParameters
                    {
                        PrimaryQuantity = 2,
                        MaterialContainer = null
                    }
                };

                SplitMaterialInput splitMaterialInput = new SplitMaterialInput
                {
                    Material = material,
                    ChildMaterials = splitInputParametersCollection,
                    SplitMode = MaterialSplitMode.SplitNotAssembled,
                    IgnoreLastServiceId = true,
                    ServiceComments = null
                };
                SplitMaterialOutput splitMaterialOutput = splitMaterialInput.SplitMaterialSync();

                ///<ExpectedResult> The child materials follow the specified tokens </ExpectedResult>
                int count = 1;
                foreach (Material childMaterial in splitMaterialOutput.ChildMaterials)
                {
                    materials.Add(childMaterial);
                    string splittedMaterialName = childMaterial.Name.Substring(0, 8) + alphanumericLotNames[alphanumericLotNAmesIdx];
                    alphanumericLotNAmesIdx++;
                    Assert.IsTrue(splittedMaterialName.Equals(childMaterial.Name), $"Child name should be {splittedMaterialName}, instead is {childMaterial.Name}.");
                    count += 1;
                }

                ///<Step> Split a child material </Step>
                Material splitedMaterial = splitMaterialOutput.ChildMaterials.FirstOrDefault();
                splitInputParametersCollection = new SplitInputParametersCollection
                {
                    new SplitInputParameters
                    {
                        PrimaryQuantity = 2,
                        MaterialContainer = null
                    }
                };

                splitMaterialInput = new SplitMaterialInput
                {
                    Material = splitedMaterial,
                    ChildMaterials = splitInputParametersCollection,
                    SplitMode = MaterialSplitMode.SplitNotAssembled,
                    IgnoreLastServiceId = true,
                    ServiceComments = null
                };
                splitMaterialOutput = splitMaterialInput.SplitMaterialSync();


                ///<ExpectedResult> The child material follow the specified tokens </ExpectedResult>
                foreach (Material childMaterial in splitMaterialOutput.ChildMaterials)
                {
                    materials.Add(childMaterial);
                    string splittedLotName = childMaterial.Name.Substring(0, 8) + alphanumericLotNames[alphanumericLotNAmesIdx];
                    alphanumericLotNAmesIdx++;
                    Assert.IsTrue(splittedLotName.Equals(childMaterial.Name), $"Child name should be {splittedLotName}, instead is {childMaterial.Name}.");
                    count += 1;
                }
            }
            finally
            {
                ///<Step> Clear the created materials </Step>
                if (materials.Count > 0)
                {
                    materials.Load();
                    materials.TerminateMaterialCollection();
                }
            }
        }
        

        private List<string> GenerateLotNames(int numberOfNamesToGenerate, string parentMaterialName)
        {
            List<string> names = new List<string>();
            string parentMaterialNameSubstring = parentMaterialName.Substring(0, 8);
            GeneratorContext context = new LoadNameGeneratorContextsInput()
            {
                NameGenerator = GenericGetsScenario.GetObjectByName<NameGenerator>(AMSOsramConstants.CustomGenerateSplitLotNames)
            }.LoadNameGeneratorContextsSync().NameGenerator?.Contexts.LastOrDefault(c => c.Context == parentMaterialNameSubstring);

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

                for (int y = 0; y < 2; y++)
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
