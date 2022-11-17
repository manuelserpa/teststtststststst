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
        private static Material mainMaterial = null;
        private static Material firstSubMaterial = null;
        private static Material secondSubMaterial = null;
        private static Material thirdMaterial = null;
        private static Material fourthMaterial = null;
        private static Material fifthMaterial = null;
        private Dictionary<string,string> originalAttributeValues = new Dictionary<string,string>();


        #endregion Constants and Variables

        [ClassInitialize] 
        public static void Init(TestContext testContext)
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

            thirdMaterial = new Material()
            {
                Name = "TestMaterialWithAttributes_3"
            };

            fourthMaterial = new Material()
            {
                Name = "TestSubMaterialWithAttributes_4"
            };

            fifthMaterial = new Material()
            {
                Name = "TestSubMaterialWithAttributes_5"
            };

            mainMaterial.Load();
            firstSubMaterial.Load();
            secondSubMaterial.Load();
            thirdMaterial.Load();
            fourthMaterial.Load();
            fifthMaterial.Load();

            mainMaterial.LoadAttributes();
            firstSubMaterial.LoadAttributes();
            secondSubMaterial.LoadAttributes();
            thirdMaterial.LoadAttributes();
            fourthMaterial.LoadAttributes();
            fifthMaterial.LoadAttributes();

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

            thirdMaterial.SaveAttribute("GoodsReceiptNo", "70112202");
            thirdMaterial.SaveAttribute("GoodsReceiptDate", "20221107");
            thirdMaterial.Save();
            thirdMaterial.Load();

            fourthMaterial.SaveAttribute("GoodsReceiptNo", "70112202");
            fourthMaterial.SaveAttribute("GoodsReceiptDate", "20221107");
            fourthMaterial.Save();
            fourthMaterial.Load();

            fifthMaterial.SaveAttribute("GoodsReceiptNo", "70112202");
            fifthMaterial.SaveAttribute("GoodsReceiptDate", "20221107");
            fifthMaterial.Save();
            fifthMaterial.Load();

            #endregion SetAttributes
        }

        [ClassCleanup]
        public static void Cleanup()
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

            thirdMaterial.SaveAttribute("GoodsReceiptNo", "");
            thirdMaterial.SaveAttribute("GoodsReceiptDate", "");
            thirdMaterial.Save();
            thirdMaterial.Load();

            fourthMaterial.SaveAttribute("GoodsReceiptNo", "");
            fourthMaterial.SaveAttribute("GoodsReceiptDate", "");
            fourthMaterial.Save();
            fourthMaterial.Load();

            fifthMaterial.SaveAttribute("GoodsReceiptNo", "");
            fifthMaterial.SaveAttribute("GoodsReceiptDate", "");
            fifthMaterial.Save();
            fifthMaterial.Load();
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
            CustomGetMaterialAttributesInput customGetMaterialAttributesInput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = "TestMaterialWithAttributes",
                AttributeList = "",
                IncludeSubMaterials = "false",
                SubMaterialAttributeList = ""
            };

            customGetMaterialAttributesOutput = customGetMaterialAttributesInput.CustomGetMaterialAttributesSync();

            Assert.IsTrue(customGetMaterialAttributesOutput.ResultXML == expectedReturn);
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
            CustomGetMaterialAttributesInput customGetMaterialAttributesInput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = "TestMaterialWithAttributes",
                AttributeList = "GoodsReceiptDate",
                IncludeSubMaterials = "",
                SubMaterialAttributeList = "GoodsReceiptNo"
            };

            customGetMaterialAttributesOutput = customGetMaterialAttributesInput.CustomGetMaterialAttributesSync();

            Assert.IsTrue(customGetMaterialAttributesOutput.ResultXML == expectedReturn);
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
            
            CustomGetMaterialAttributesInput customGetMaterialAttributesInput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = "TestMaterialWithAttributes",
                AttributeList = "",
                IncludeSubMaterials = "",
                SubMaterialAttributeList = ""
            };

            customGetMaterialAttributesOutput = customGetMaterialAttributesInput.CustomGetMaterialAttributesSync();

            Assert.IsTrue(customGetMaterialAttributesOutput.ResultXML == expectedReturn);
        }

        /// <summary>
        /// Description:
        ///     - Send an XML Message to the custom service CustomGetMaterialAttributes
        /// 
        /// Acceptance Citeria:
        ///     - Return message created with the Materials Name, Form, and requested Attributes, Submaterials Names Forms and requested Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes_HappyPath_Two_Main_Materials_AllSubMaterials_AllAttributes</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_Two_Main_Materials_AllSubMaterials_AllAttributes()
        {
            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesOutput();
            string expectedReturn = "<?xml version=\"1.0\" encoding=\"utf-16\"?><CustomGetMaterialAttributes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Material><Name>TestMaterialWithAttributes</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">70112202</Attribute><Attribute Name=\"GoodsReceiptDate\">20221107</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_1</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">41112202</Attribute><Attribute Name=\"GoodsReceiptDate\">20221114</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_2</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">20221114</Attribute><Attribute Name=\"GoodsReceiptDate\">41112202</Attribute></Attributes></Material></SubMaterials></Material><Material><Name>TestMaterialWithAttributes_3</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">70112202</Attribute><Attribute Name=\"GoodsReceiptDate\">20221107</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_4</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">70112202</Attribute><Attribute Name=\"GoodsReceiptDate\">20221107</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_5</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">70112202</Attribute><Attribute Name=\"GoodsReceiptDate\">20221107</Attribute></Attributes></Material></SubMaterials></Material></CustomGetMaterialAttributes>";
            CustomGetMaterialAttributesInput customGetMaterialAttributesInput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = "TestMaterialWithAttributes,TestMaterialWithAttributes_3",
                AttributeList = "",
                IncludeSubMaterials = "",
                SubMaterialAttributeList = ""
            };

            customGetMaterialAttributesOutput = customGetMaterialAttributesInput.CustomGetMaterialAttributesSync();

            Assert.IsTrue(customGetMaterialAttributesOutput.ResultXML == expectedReturn);
        }

        /// <summary>
        /// Description:
        ///     - Send an XML Message to the custom service CustomGetMaterialAttributes
        /// 
        /// Acceptance Citeria:
        ///     - Return message created with the Materials Name, Form, and requested Attributes, Submaterials Names Forms and requested Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes_HappyPath_Two_Main_Materials_Limited_SubMaterials_AllAttributes</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_Two_Main_Materials_Limited_SubMaterials_AllAttributes()
        {
            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesOutput();
            string expectedReturn = "<?xml version=\"1.0\" encoding=\"utf-16\"?><CustomGetMaterialAttributes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Material><Name>TestMaterialWithAttributes</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">70112202</Attribute><Attribute Name=\"GoodsReceiptDate\">20221107</Attribute></Attributes></Material><Material><Name>TestMaterialWithAttributes_3</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">70112202</Attribute><Attribute Name=\"GoodsReceiptDate\">20221107</Attribute></Attributes></Material></CustomGetMaterialAttributes>";
            CustomGetMaterialAttributesInput customGetMaterialAttributesInput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = "TestMaterialWithAttributes,TestMaterialWithAttributes_3",
                AttributeList = "",
                IncludeSubMaterials = "false",
                SubMaterialAttributeList = ""
            };

            customGetMaterialAttributesOutput = customGetMaterialAttributesInput.CustomGetMaterialAttributesSync();

            Assert.IsTrue(customGetMaterialAttributesOutput.ResultXML == expectedReturn);
        }

        /// <summary>
        /// Description:
        ///     - Send an XML Message to the custom service CustomGetMaterialAttributes
        /// 
        /// Acceptance Citeria:
        ///     - Return message created with the Materials Name, Form, and requested Attributes, Submaterials Names Forms and requested Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes_HappyPath_Two_Main_Materials_Limited_SubMaterials_Limited_Attributes</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_Two_Main_Materials_Limited_SubMaterials_Limited_Attributes()
        {
            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesOutput();
            string expectedReturn = "<?xml version=\"1.0\" encoding=\"utf-16\"?><CustomGetMaterialAttributes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Material><Name>TestMaterialWithAttributes</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">70112202</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_1</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptDate\">20221114</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_2</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptDate\">41112202</Attribute></Attributes></Material></SubMaterials></Material><Material><Name>TestMaterialWithAttributes_3</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">70112202</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_4</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptDate\">20221107</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_5</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptDate\">20221107</Attribute></Attributes></Material></SubMaterials></Material></CustomGetMaterialAttributes>";
            CustomGetMaterialAttributesInput customGetMaterialAttributesInput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = "TestMaterialWithAttributes,TestMaterialWithAttributes_3",
                AttributeList = "GoodsReceiptNo",
                IncludeSubMaterials = "True",
                SubMaterialAttributeList = "GoodsReceiptDate"
            };

            customGetMaterialAttributesOutput = customGetMaterialAttributesInput.CustomGetMaterialAttributesSync();

            Assert.IsTrue(customGetMaterialAttributesOutput.ResultXML == expectedReturn);
        }
    }
}
