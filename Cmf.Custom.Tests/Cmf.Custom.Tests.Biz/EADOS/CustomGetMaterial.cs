using Cmf.Custom.amsOSRAM.Orchestration.InputObjects;
using Cmf.Custom.amsOSRAM.Orchestration.OutputObjects;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
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
        /// <TestCaseID>CustomGetMaterialAttributes.CustomGetMaterialAttributes_HappyPath_Default_Values</TestCaseID>
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
    }
}
