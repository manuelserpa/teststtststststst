using Cmf.Custom.amsOSRAM.Orchestration.InputObjects;
using Cmf.Custom.amsOSRAM.Orchestration.OutputObjects;
using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;
using Cmf.TestScenarios;
using Cmf.TestScenarios.MaterialManagement.MaterialScenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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
        private static BaseScenario<Material> baseScenario = null;
        private static List<MaterialScenario> materialScenarioList= null;
        private static Foundation.BusinessObjects.AttributeCollection attributeCollection = new Foundation.BusinessObjects.AttributeCollection();
        private static List<string> materialNameList = new List<string> {   "TestMaterialWithAttributes",
                                                                            "TestSubMaterialWithAttributes_1",
                                                                            "TestSubMaterialWithAttributes_2",
                                                                            "TestMaterialWithAttributes_3",
                                                                            "TestSubMaterialWithAttributes_4",
                                                                            "TestSubMaterialWithAttributes_5"
        };

        #endregion Constants and Variables

        [ClassInitialize] 
        public static void Init(TestContext testContext)
        {
            attributeCollection.Add("GoodsReceiptNo", "81112022");
            attributeCollection.Add("GoodsReceiptDate", "20221118");
            attributeCollection.Add("GoodsReceiptComponentNo", "71112022");
            attributeCollection.Add("Gravure", "81112022");

            foreach(string name in materialNameList)
            {
                MaterialScenario materialScenario = new MaterialScenario();
                materialScenario.Entity.Name = name;
                materialScenario.Entity.Load();
                materialScenario.Entity.SaveAttributes(attributeCollection);
            }
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            System.Collections.ObjectModel.Collection<string> keysToDelete = new System.Collections.ObjectModel.Collection<string> {    "GoodsReceiptNo",
                                                                                                                                        "GoodsReceiptDate",
                                                                                                                                        "GoodsReceiptComponentNo",
                                                                                                                                        "Gravure"
            };
            foreach (string name in materialNameList)
            {
                MaterialScenario materialScenario = new MaterialScenario();
                materialScenario.Entity.Name = name;
                materialScenario.Entity.Load();
                materialScenario.Entity.RemoveAttributes(keysToDelete);
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
            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesOutput();
            string expectedReturn = "<?xml version=\"1.0\" encoding=\"utf-16\"?><CustomGetMaterialAttributes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Material><Name>TestMaterialWithAttributes</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material><Material><Name>TestMaterialWithAttributes_3</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material></CustomGetMaterialAttributes>";
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
        ///     - Return message created with the Materials Name, Form, and all Attributes, Submaterials Names Forms and all Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes_HappyPath_MainMaterial_Not_Restricted_AllSubMaterials_Not_Restricted</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_MainMaterial_Not_Restricted_AllSubMaterials_Not_Restricted()
        {
            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesOutput();
            string expectedReturn = "<?xml version=\"1.0\" encoding=\"utf-16\"?><CustomGetMaterialAttributes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Material><Name>TestMaterialWithAttributes</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_1</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_2</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material></SubMaterials></Material><Material><Name>TestMaterialWithAttributes_3</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_4</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_5</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material></SubMaterials></Material></CustomGetMaterialAttributes>";
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
        ///     - Return message created with the Materials Name, Form, and all Attributes, Submaterials Names Forms and requested Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes_HappyPath_MainMaterial_Not_Restricted_AllSubMaterials_Restricted</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_MainMaterial_Not_Restricted_AllSubMaterials_Restricted()
        {
            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesOutput();
            string expectedReturn = "<?xml version=\"1.0\" encoding=\"utf-16\"?><CustomGetMaterialAttributes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Material><Name>TestMaterialWithAttributes</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_1</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_2</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material></SubMaterials></Material><Material><Name>TestMaterialWithAttributes_3</Name><Form>Lot</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_4</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_5</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material></SubMaterials></Material></CustomGetMaterialAttributes>";
            CustomGetMaterialAttributesInput customGetMaterialAttributesInput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = "TestMaterialWithAttributes,TestMaterialWithAttributes_3",
                AttributeList = "",
                IncludeSubMaterials = "",
                SubMaterialAttributeList = "Gravure,GoodsReceiptComponentNo"
            };

            customGetMaterialAttributesOutput = customGetMaterialAttributesInput.CustomGetMaterialAttributesSync();

            Assert.IsTrue(customGetMaterialAttributesOutput.ResultXML == expectedReturn);
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
            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesOutput();
            string expectedReturn = "<?xml version=\"1.0\" encoding=\"utf-16\"?><CustomGetMaterialAttributes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Material><Name>TestMaterialWithAttributes</Name><Form>Lot</Form><Attributes><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material><Material><Name>TestMaterialWithAttributes_3</Name><Form>Lot</Form><Attributes><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material></CustomGetMaterialAttributes>";
            CustomGetMaterialAttributesInput customGetMaterialAttributesInput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = "TestMaterialWithAttributes,TestMaterialWithAttributes_3",
                AttributeList = "Gravure,GoodsReceiptComponentNo",
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
        ///     - Return message created with the Materials Name, Form, and requested Attributes, Submaterials Names Forms and all Attributes
        /// 
        /// </summary>
        /// <TestCaseID>CustomGetMaterialAttributes_HappyPath_MainMaterial_Restricted_AllSubMaterials</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_MainMaterial_Restricted_AllSubMaterials()
        {
            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesOutput();
            string expectedReturn = "<?xml version=\"1.0\" encoding=\"utf-16\"?><CustomGetMaterialAttributes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Material><Name>TestMaterialWithAttributes</Name><Form>Lot</Form><Attributes><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_1</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_2</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material></SubMaterials></Material><Material><Name>TestMaterialWithAttributes_3</Name><Form>Lot</Form><Attributes><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_4</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_5</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes></Material></SubMaterials></Material></CustomGetMaterialAttributes>";
            CustomGetMaterialAttributesInput customGetMaterialAttributesInput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = "TestMaterialWithAttributes,TestMaterialWithAttributes_3",
                AttributeList = "Gravure,GoodsReceiptComponentNo",
                IncludeSubMaterials = "true",
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
        /// <TestCaseID>CustomGetMaterialAttributes_HappyPath_MainMaterial_Restricted_Submaterial_Restricted</TestCaseID>
        /// <Author>Gergoe Pajor</Author>
        [TestMethod]
        public void CustomGetMaterialAttributes_HappyPath_MainMaterial_Restricted_Submaterial_Restricted()
        {
            CustomGetMaterialAttributesOutput customGetMaterialAttributesOutput = new CustomGetMaterialAttributesOutput();
            string expectedReturn = "<?xml version=\"1.0\" encoding=\"utf-16\"?><CustomGetMaterialAttributes xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Material><Name>TestMaterialWithAttributes</Name><Form>Lot</Form><Attributes><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_1</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_2</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute></Attributes></Material></SubMaterials></Material><Material><Name>TestMaterialWithAttributes_3</Name><Form>Lot</Form><Attributes><Attribute Name=\"Gravure\">81112022</Attribute><Attribute Name=\"GoodsReceiptComponentNo\">71112022</Attribute></Attributes><SubMaterials><Material><Name>TestSubMaterialWithAttributes_4</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute></Attributes></Material><Material><Name>TestSubMaterialWithAttributes_5</Name><Form>Logical Wafer</Form><Attributes><Attribute Name=\"GoodsReceiptNo\">81112022</Attribute><Attribute Name=\"GoodsReceiptDate\">20221118</Attribute></Attributes></Material></SubMaterials></Material></CustomGetMaterialAttributes>";
            CustomGetMaterialAttributesInput customGetMaterialAttributesInput = new CustomGetMaterialAttributesInput()
            {
                MaterialList = "TestMaterialWithAttributes,TestMaterialWithAttributes_3",
                AttributeList = "Gravure,GoodsReceiptComponentNo",
                IncludeSubMaterials = "True",
                SubMaterialAttributeList = "GoodsReceiptDate,GoodsReceiptNo"
            };

            customGetMaterialAttributesOutput = customGetMaterialAttributesInput.CustomGetMaterialAttributesSync();

            Assert.IsTrue(customGetMaterialAttributesOutput.ResultXML == expectedReturn);
        }
    }
}
