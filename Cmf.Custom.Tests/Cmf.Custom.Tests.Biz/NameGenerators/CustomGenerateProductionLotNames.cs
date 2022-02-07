using Cmf.Custom.Tests.Biz.Common;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.NameGeneratorManagement.InputObjects;
using Cmf.TestScenarios.Others;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.NameGenerators
{
    [TestClass]
    public class CustomGenerateProductionLotNames
    {
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
            /// <Step>
            /// Get CustomGenerateProductionLotNames Name Generator.
            /// </Step>
            var nameGenerator = GenericGetsScenario.GetObjectByName<NameGenerator>(Name: AMSOsramConstants.CustomGenerateProductionLotNames);

            var productionLotName = string.Empty;

            for (int i = 0; i < 20; i++)
            {
                /// <Step>
                /// Generate names for Production Lot using CustomGenerateProductionLotNames Name Generator.
                /// </Step>
                productionLotName = GenerateName(nameGenerator: AMSOsramConstants.CustomGenerateProductionLotNames);

                /// <Step>
                /// Get last context of CustomGenerateProductionLotNames.
                /// </Step>
                var nameGeneratorContext = new LoadNameGeneratorContextsInput()
                {
                    NameGenerator = nameGenerator
                }.LoadNameGeneratorContextsSync().NameGenerator?.Contexts.LastOrDefault();

                /// <ExpectedResult>
                /// The generated name is equals to the last context saved.
                /// </ExpectedResult>
                Assert.IsTrue(nameGeneratorContext.Context.Equals(productionLotName.Substring(0, 5)), $"Production Lot generated name is {productionLotName.Substring(0, 5)} and last context name generator is {nameGeneratorContext.Context}.");
            };
        }

        /// <summary>
        /// Generates the name.
        /// </summary>
        /// <param name="nameGenerator">The name generator.</param>
        /// <param name="entitySourceForProperty">The entity source for property.</param>
        /// <returns></returns>
        public static string GenerateName(string nameGenerator, object entitySourceForProperty = null)
        {
            return new GenerateNewNameInput
            {
                NameGeneratorName = nameGenerator,
                EntitySourceForProperty = entitySourceForProperty
            }.GenerateNewNameSync().NewName;
        }
    }
}
