using Cmf.Custom.Tests.Biz.Common;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.NameGeneratorManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.NameGeneratorManagement.OutputObjects;
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
            // Initial Alphanumeric value
            string alphaNumericValue = "00";

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
                    string childName = ExpectedMaterialName(splitMaterialOutput.Material.Name,alphaNumericValue);
                    string newAlphaNumericValue = AlphaNumericValueCalculator(alphaNumericValue);
                    Assert.IsTrue(childName.Equals(childMaterial.Name), $"Child name should be {childName}, instead is {childMaterial.Name}.");
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
                    string childName = ExpectedMaterialName(splitMaterialOutput.Material.Name, alphaNumericValue);
                    string newAlphaNumericValue = AlphaNumericValueCalculator(alphaNumericValue);
                    Assert.IsTrue(childName.Equals(childMaterial.Name), $"Child name should be {childName}, instead is {childMaterial.Name}.");
                    count += 1;
                }
            }
            finally
            {
                ///<Step> Clear the created materials </Step>
                if (materials.Count > 0 )
                {
                    materials.Load();
                    materials.TerminateMaterialCollection();
                }
            }
        }

        string ExpectedMaterialName(string parentLotName,string alphaNumericValue)
        {
            string expectedName = parentLotName.Substring(0, 8);
            Material parentLot = new Material
            {
                Name = parentLotName
            };
            parentLot.Load(1);

            expectedName =  expectedName + alphaNumericValue.ToString();

            return expectedName;
        }

        string AlphaNumericValueCalculator(string alphaNumericValue)
        {
            string newAlphaNumericValue = string.Empty;
            string acceptedChars = "0123456789ACFHLMNRTUX";
            //In case the counter's first char needs to be changed
            if (alphaNumericValue.Substring(1) != "X")
            {
                int idx = acceptedChars.IndexOf(alphaNumericValue.Substring(1));
                idx++;
                newAlphaNumericValue = alphaNumericValue.Substring(0,1) + acceptedChars.Substring(idx,1);
            }
            //The case where only the last character needs to change
            else
            {
                int idx = acceptedChars.IndexOf(alphaNumericValue.Substring(0,1));
                idx++;
                newAlphaNumericValue = acceptedChars.Substring(idx, 1) + "0";
            }


            return newAlphaNumericValue;
        }
    }
}
