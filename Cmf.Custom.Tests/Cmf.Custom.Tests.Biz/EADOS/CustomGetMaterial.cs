using Cmf.Custom.amsOSRAM.Orchestration.InputObjects;
using Cmf.Custom.amsOSRAM.Orchestration.OutputObjects;
using Cmf.Custom.Tests.Biz.Common.ERP.CustomGetMaterial;
using Cmf.Custom.Tests.Biz.Common.ERP.Space;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessOrchestration.DynamicExecutionEngineManagement.InputObjects;
using Cmf.Navigo.BusinessObjects;
using Cmf.TestScenarios;
using Cmf.TestScenarios.MaterialManagement.MaterialScenarios;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.EADOS
{
    [TestClass]
    public class CustomGetMaterial
    {
        #region Constants and Variables

        private static Material firstLot = null;
        private static Material firstChildFirstLot = null;
        private static Material secondChildFirstLot = null;
        private static Material secondLot = null;
        private static Material firstChildSecondLot = null;
        private static Material secondChildSecondLot = null;
        private static Collection<string> allAttributes = new Collection<string> { "GoodsReceiptNo", "GoodsReceiptDate", "Gravure", "GoodsReceiptComponentNo" };

        private static CustomExecutionScenario customExecutionScenario = null;

        #endregion Constants and Variables

        [ClassInitialize] 
        public static void Init(TestContext testContext)
        {
            Foundation.BusinessObjects.AttributeCollection attributeCollection = new Foundation.BusinessObjects.AttributeCollection();
            attributeCollection.Add("GoodsReceiptNo", "81112022");
            attributeCollection.Add("GoodsReceiptDate", "20221118");
            attributeCollection.Add("GoodsReceiptComponentNo", "71112022");
            attributeCollection.Add("Gravure", "81112022");

            customExecutionScenario = new CustomExecutionScenario();
            customExecutionScenario.NumberOfMaterialsToGenerate = 2;
            customExecutionScenario.NumberOfChildMaterialsToGenerate= 2;
            customExecutionScenario.Setup();

            MaterialCollection materials = new MaterialCollection();
            MaterialCollection lots = customExecutionScenario.GeneratedLots;
            
            foreach(Material lot in lots)
            {
                lot.LoadChildren();
                materials.AddRange(lot.SubMaterials);
                materials.Add(lot);
            }

            materials.SaveCollectionAttributes(attributeCollection);

            firstLot = lots.First();
            secondLot = lots.Last();

            firstLot.LoadAttributes(allAttributes);
            secondLot.LoadAttributes(allAttributes);

            firstLot.LoadChildren();
            firstLot.SubMaterials.LoadCollectionAttributes(allAttributes);
            firstChildFirstLot = firstLot.SubMaterials.First();
            secondChildFirstLot = firstLot.SubMaterials.Last();

            secondLot.LoadChildren();
            secondLot.SubMaterials.LoadCollectionAttributes(allAttributes);
            firstChildSecondLot = secondLot.SubMaterials.First();
            secondChildSecondLot = secondLot.SubMaterials.Last();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
           if (customExecutionScenario != null)
            {
                customExecutionScenario.CompleteCleanUp();
            }
        }

        /// <summary>
        /// Description:
        ///     - Send an XML Message to the custom service CustomGetMaterialAttributes
        /// 
        /// Acceptance Citeria:
        ///     - Return message created with the Materials Name, Form, and all Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes_HappyPath_MainMaterial_Not_Restricted_No_Submaterials</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_MainMaterial_Not_Restricted_No_Submaterials()
        {
            MaterialCollection lots = customExecutionScenario.GeneratedLots;

            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = String.Join(",", lots.Select(s => s.Name)),
                AttributeList = "",
                IncludeSubMaterials = "false",
                SubMaterialAttributeList = ""
            }.CustomGetMaterialAttributesSync();

            CustomGetMaterialAttributesData customGetMaterialAttributesData = CustomUtilities.DeserializeXmlToObject<CustomGetMaterialAttributesData>(customGetMaterialAttributesOutput.ResultXML);

            ValidateMessage(customGetMaterialAttributesData.MaterialList, lots, allAttributes);

            foreach (Common.ERP.CustomGetMaterial.CustomGetMaterialAttributesDataDto.Material material in customGetMaterialAttributesData.MaterialList)
            {
                Assert.AreEqual(0, material.SubMaterials.Count);
            }
        }

        /// <summary>
        /// Description:
        ///     - Send an XML Message to the custom service CustomGetMaterialAttributes
        /// 
        /// Acceptance Citeria:
        ///     - Return message created with the Materials Name, Form, and all Attributes, Submaterials Names Forms and all Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes_HappyPath_MainMaterial_Not_Restricted_AllSubMaterials_Not_Restricted</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_MainMaterial_Not_Restricted_AllSubMaterials_Not_Restricted()
        {
            MaterialCollection lots = customExecutionScenario.GeneratedLots;

            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = String.Join(",", lots.Select(s => s.Name)),
                AttributeList = "",
                IncludeSubMaterials = "",
                SubMaterialAttributeList = ""
            }.CustomGetMaterialAttributesSync();

            CustomGetMaterialAttributesData customGetMaterialAttributesData = CustomUtilities.DeserializeXmlToObject<CustomGetMaterialAttributesData>(customGetMaterialAttributesOutput.ResultXML);

            ValidateMessage(customGetMaterialAttributesData.MaterialList, lots, allAttributes);
            ValidateSubMaterialMessage(customGetMaterialAttributesData.MaterialList, lots, allAttributes);
        }

        /// <summary>
        /// Description:
        ///     - Send an XML Message to the custom service CustomGetMaterialAttributes
        /// 
        /// Acceptance Citeria:
        ///     - Return message created with the Materials Name, Form, and all Attributes, Submaterials Names Forms and requested Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes_HappyPath_MainMaterial_Not_Restricted_AllSubMaterials_Restricted</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_MainMaterial_Not_Restricted_AllSubMaterials_Restricted()
        {
            Collection<string> subAttributes = new Collection<string> { "Gravure", "GoodsReceiptComponentNo" };
            MaterialCollection lots = customExecutionScenario.GeneratedLots;

            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = String.Join(",", lots.Select(s => s.Name)),
                AttributeList = "",
                IncludeSubMaterials = "",
                SubMaterialAttributeList = String.Join(",", subAttributes)
            }.CustomGetMaterialAttributesSync();

            CustomGetMaterialAttributesData customGetMaterialAttributesData = CustomUtilities.DeserializeXmlToObject<CustomGetMaterialAttributesData>(customGetMaterialAttributesOutput.ResultXML);

            ValidateMessage(customGetMaterialAttributesData.MaterialList, lots, allAttributes);
            ValidateSubMaterialMessage(customGetMaterialAttributesData.MaterialList, lots, subAttributes);
        }

        /// <summary>
        /// Description:
        ///     - Send an XML Message to the custom service CustomGetMaterialAttributes
        /// 
        /// Acceptance Citeria:
        ///     - Return message created with the Materials Name, Form, and requested Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes_HappyPath_MainMaterial__Restricted_No_SubMaterials</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_MainMaterial__Restricted_No_SubMaterials()
        {
            Collection<string> attributes = new Collection<string> { "Gravure", "GoodsReceiptComponentNo" };
            MaterialCollection lots = customExecutionScenario.GeneratedLots;

            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = String.Join(",", lots.Select(s => s.Name)),
                AttributeList = String.Join(",", attributes),
                IncludeSubMaterials = "false",
                SubMaterialAttributeList = ""
            }.CustomGetMaterialAttributesSync();

            CustomGetMaterialAttributesData customGetMaterialAttributesData = CustomUtilities.DeserializeXmlToObject<CustomGetMaterialAttributesData>(customGetMaterialAttributesOutput.ResultXML);

            ValidateMessage(customGetMaterialAttributesData.MaterialList, lots, attributes);

            foreach(Common.ERP.CustomGetMaterial.CustomGetMaterialAttributesDataDto.Material material in customGetMaterialAttributesData.MaterialList)
            {
                Assert.AreEqual(0, material.SubMaterials.Count);
            }
        }

        /// <summary>
        /// Description:
        ///     - Send an XML Message to the custom service CustomGetMaterialAttributes
        /// 
        /// Acceptance Citeria:
        ///     - Return message created with the Materials Name, Form, and requested Attributes, Submaterials Names Forms and all Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes_HappyPath_MainMaterial_Restricted_AllSubMaterials</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_MainMaterial_Restricted_AllSubMaterials()
        {
            Collection<string> attributes = new Collection<string> { "Gravure", "GoodsReceiptComponentNo" };
            MaterialCollection lots = customExecutionScenario.GeneratedLots;
            
            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = String.Join(",", customExecutionScenario.GeneratedLots.Select(s => s.Name)),
                AttributeList = String.Join(",", attributes),
                IncludeSubMaterials = "true",
                SubMaterialAttributeList = ""
            }.CustomGetMaterialAttributesSync();

            CustomGetMaterialAttributesData customGetMaterialAttributesData = CustomUtilities.DeserializeXmlToObject<CustomGetMaterialAttributesData>(customGetMaterialAttributesOutput.ResultXML);

            ValidateMessage(customGetMaterialAttributesData.MaterialList, lots, attributes);
            ValidateSubMaterialMessage(customGetMaterialAttributesData.MaterialList, lots, allAttributes);
        }

        /// <summary>
        /// Description:
        ///     - Send an XML Message to the custom service CustomGetMaterialAttributes
        /// 
        /// Acceptance Citeria:
        ///     - Return message created with the Materials Name, Form, and requested Attributes, Submaterials Names Forms and requested Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes_HappyPath_MainMaterial_Restricted_Submaterial_Restricted</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_MainMaterial_Restricted_Submaterial_Restricted()
        {
            Collection<string> attributes = new Collection<string> { "Gravure", "GoodsReceiptComponentNo" };
            Collection<string> subAttributes = new Collection<string> { "GoodsReceiptNo", "GoodsReceiptDate" };
            MaterialCollection lots = customExecutionScenario.GeneratedLots;

            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = String.Join(",", lots.Select(s => s.Name)),
                AttributeList = String.Join(",", attributes),
                IncludeSubMaterials = "true",
                SubMaterialAttributeList = String.Join(",", subAttributes)
            }.CustomGetMaterialAttributesSync();

            CustomGetMaterialAttributesData customGetMaterialAttributesData = CustomUtilities.DeserializeXmlToObject<CustomGetMaterialAttributesData>(customGetMaterialAttributesOutput.ResultXML);

            ValidateMessage(customGetMaterialAttributesData.MaterialList, lots, attributes);
            ValidateSubMaterialMessage(customGetMaterialAttributesData.MaterialList, lots, subAttributes);
        }

        /// <summary>
        /// Validates the material message.
        /// </summary>
        /// <param name="materialList">The material list.</param>
        /// <param name="materials">The materials.</param>
        /// <param name="attributes">The attributes.</param>
        private void ValidateMessage(List<Common.ERP.CustomGetMaterial.CustomGetMaterialAttributesDataDto.Material> materialList, MaterialCollection materials, Collection<string> attributes)
        {
            int counter = 0;

            foreach (Material material in materials)
            {
                Common.ERP.CustomGetMaterial.CustomGetMaterialAttributesDataDto.Material customGetMaterial = materialList[counter];

                Assert.AreEqual(material.Name, customGetMaterial.Name);
                Assert.AreEqual(material.Form, customGetMaterial.Form);

                Assert.AreEqual(attributes.Count, customGetMaterial.Attributes.Count);

                foreach (string attribute in attributes)
                {
                    Assert.AreEqual(material.Attributes[attribute], customGetMaterial.Attributes.First(f => f.Name == attribute).Value);
                }

                counter++;
            }
        }

        /// <summary>
        /// Validates the sub material message.
        /// </summary>
        /// <param name="materialList">The material list.</param>
        /// <param name="materials">The materials.</param>
        /// <param name="attributes">The attributes.</param>
        private void ValidateSubMaterialMessage(List<Common.ERP.CustomGetMaterial.CustomGetMaterialAttributesDataDto.Material> materialList, MaterialCollection materials, Collection<string> attributes)
        {
            int counter = 0;

            foreach (Material material in materials)
            {
                ValidateMessage(materialList[counter].SubMaterials, material.SubMaterials, attributes);
                counter++;
            }
        }
    }
}
