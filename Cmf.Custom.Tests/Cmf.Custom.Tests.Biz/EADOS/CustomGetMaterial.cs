using Cmf.Custom.amsOSRAM.Orchestration.InputObjects;
using Cmf.Custom.amsOSRAM.Orchestration.OutputObjects;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;
using Cmf.TestScenarios.MaterialManagement.MaterialScenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cmf.Custom.Tests.Biz.EADOS
{
    [TestClass]
    public class CustomGetMaterial
    {
        #region Constants and Variables
        private Material mainMaterial = null;
        private Material firstSubMaterial = null;
        private Material secondSubMaterial = null;
        private Dictionary<string,string> originalAttributeValues = new Dictionary<string,string>();


        #endregion Constants and Variables

        [TestInitialize] 
        public void Init()
        {
            mainMaterial = new Material()
            {
                Name = "TestMaterialWithAttributes"
            };

            firstSubMaterial = new Material()
            {
                Name = "TestSubMaterialWithAttributes_1"
            };

            secondSubMaterial = new Material()
            {
                Name = "TestSubMaterialWithAttributes_2"
            };

            mainMaterial.Load();
            firstSubMaterial.Load();
            secondSubMaterial.Load();

            mainMaterial.LoadAttributes();
            firstSubMaterial.LoadAttributes();
            secondSubMaterial.LoadAttributes();

            #region SetAttributes
            mainMaterial.SaveAttribute("GoodsReceiptNo","70112202");
            mainMaterial.SaveAttribute("GoodsReceiptDate","20221107");
            mainMaterial.Save();
            mainMaterial.Load();

            firstSubMaterial.SaveAttribute("GoodsReceiptNo","41112202");
            firstSubMaterial.SaveAttribute("GoodsReceiptDate","20221114");
            firstSubMaterial.Save();
            firstSubMaterial.Load();

            secondSubMaterial.SaveAttribute("GoodsReceiptNo","20221114");
            secondSubMaterial.SaveAttribute("GoodsReceiptDate","41112202");
            secondSubMaterial.Save();
            secondSubMaterial.Load();

            #endregion SetAttributes
        }

        [TestCleanup]
        public void Cleanup()
        {
            #region
            mainMaterial.SaveAttribute("GoodsReceiptNo", "");
            mainMaterial.SaveAttribute("GoodsReceiptDate", "");
            mainMaterial.Save();
            mainMaterial.Load();

            firstSubMaterial.SaveAttribute("GoodsReceiptNo", "");
            firstSubMaterial.SaveAttribute("GoodsReceiptDate", "");
            firstSubMaterial.Save();
            firstSubMaterial.Load();

            secondSubMaterial.SaveAttribute("GoodsReceiptNo", "");
            secondSubMaterial.SaveAttribute("GoodsReceiptDate", "");
            secondSubMaterial.Save();
            secondSubMaterial.Load();
            #endregion
        }


        /// <summary>
        /// Description:
        ///     - Send an XML Message to the custom service CustomGetMaterialAttributes
        /// 
        /// Acceptance Citeria:
        ///     - Return message created with the Materials Name, Form, and all Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes.CustomGetMaterialAttributes_HappyPath_Default_Values</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_Default_Values()
        {
            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesOutput();
            string expectedReturn = "<?xml version=\"1.0\" encoding=\"utf-16\"?><CustomGetMaterialAttributes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Material><Name>TestMaterialWithAttributes</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">70112202</Attribute><Attribute Name=\"GoodsReceiptDate\">20221107</Attribute></Attributes></Material></CustomGetMaterialAttributes>";
            try
            {
                CustomGetMaterialAttributesInput customGetMaterialAttributesInput = new CustomGetMaterialAttributesInput()
                {
                    MaterialList = "TestMaterialWithAttributes",
                    AttributeList = "",
                    IncludeSubMaterials = "false",
                    SubMaterialAttributeList = ""
                };

                customGetMaterialAttributesOutput = customGetMaterialAttributesInput.CustomGetMaterialAttributesSync();

                Assert.IsTrue(customGetMaterialAttributesOutput.Result == expectedReturn);
            }
            finally
            {

            }
        }

        /// <summary>
        /// Description:
        ///     - Send an XML Message to the custom service CustomGetMaterialAttributes
        /// 
        /// Acceptance Citeria:
        ///     - Return message created with the Materials Name, Form, and requested Attributes, Submaterials Names Forms and requested Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes.CustomGetMaterialAttributes_HappyPath_AllSubMaterials_SelectedAttributes</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_AllSubMaterials_SelectedAttributes()
        {
            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesOutput();
            string expectedReturn = "<?xml version=\"1.0\" encoding=\"utf-16\"?><CustomGetMaterialAttributes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Material><Name>TestMaterialWithAttributes</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptDate\">20221107</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_1</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">41112202</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_2</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">20221114</Attribute></Attributes></Material></SubMaterials></Material></CustomGetMaterialAttributes>";
            try
            {
                CustomGetMaterialAttributesInput customGetMaterialAttributesInput = new CustomGetMaterialAttributesInput()
                {
                    MaterialList = "TestMaterialWithAttributes",
                    AttributeList = "GoodsReceiptDate",
                    IncludeSubMaterials = "",
                    SubMaterialAttributeList = "GoodsReceiptNo"
                };

                customGetMaterialAttributesOutput = customGetMaterialAttributesInput.CustomGetMaterialAttributesSync();

                Assert.IsTrue(customGetMaterialAttributesOutput.Result == expectedReturn);
            }
            finally
            {

            }
        }

        /// <summary>
        /// Description:
        ///     - Send an XML Message to the custom service CustomGetMaterialAttributes
        /// 
        /// Acceptance Citeria:
        ///     - Return message created with the Materials Name, Form, and requested Attributes, Submaterials Names Forms and requested Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes.CustomGetMaterialAttributes_HappyPath_AllSubMaterials_AllAttributes</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_AllSubMaterials_AllAttributes()
        {
            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesOutput();
            string expectedReturn = "<?xml version=\"1.0\" encoding=\"utf-16\"?><CustomGetMaterialAttributes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Material><Name>TestMaterialWithAttributes</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">70112202</Attribute><Attribute Name=\"GoodsReceiptDate\">20221107</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_1</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">41112202</Attribute><Attribute Name=\"GoodsReceiptDate\">20221114</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_2</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">20221114</Attribute><Attribute Name=\"GoodsReceiptDate\">41112202</Attribute></Attributes></Material></SubMaterials></Material></CustomGetMaterialAttributes>";
            try
            {
                CustomGetMaterialAttributesInput customGetMaterialAttributesInput = new CustomGetMaterialAttributesInput()
                {
                    MaterialList = "TestMaterialWithAttributes",
                    AttributeList = "",
                    IncludeSubMaterials = "",
                    SubMaterialAttributeList = ""
                };

                customGetMaterialAttributesOutput = customGetMaterialAttributesInput.CustomGetMaterialAttributesSync();

                Assert.IsTrue(customGetMaterialAttributesOutput.Result == expectedReturn);
            }
            finally
            {

            }
        }
    }
}
