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
        ///       - [Original Lot Name].[2 digit counter]
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
                    string childName = $"{material.Name}.0{count}";
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
                    string childName = $"{material.Name}.0{count}";
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
    }
}
