using Cmf.Custom.Tests.Biz.Common.ERP.Material;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.TestUtilities;
using Cmf.Navigo.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cmf.Custom.Tests.Biz.ERP
{
    [TestClass]
    public class CustomIncomingMaterialLotCreation
    {
        private CustomExecutionScenario customExecutionScenario;

        private CustomTearDownManager customTearDownManager;

        [TestInitialize]
        public void TestInitialization()
        {
            this.customExecutionScenario = new CustomExecutionScenario();

            this.customTearDownManager = new CustomTearDownManager();
        }

        [TestMethod]
        public void CustomIncomingMaterialLotCreation_SendERPMessage_CreateMaterial()
        {
            MaterialCollection materials = new MaterialCollection();

            try
            {
                // Set sample values for Material
                string messageMaterialName = Guid.NewGuid().ToString("N");

                string messageMaterialProduct = "11018814";

                string messageMaterialType = "Production";

                string messageMaterialStateModel = string.Empty;

                string messageMaterialState = string.Empty;

                string messageMaterialForm = "Logistical wafer";

                string messageMaterialFacility = "Regensburg Production";

                string messageMaterialFlow = "FOL-UX3_EPA";

                string messageMaterialStep = "M2-SL-Wafer-Start-07301F001_E";

                string[] messageMaterialAttribNames = { "GoodsReceiptNo", "GoodsReceiptDate" };

                string[] messageMaterialAttribValues = { "5043575531", "08.11.2021" };

                string messageSubMaterialName = Guid.NewGuid().ToString("N");

                string messageSubMaterialForm = "Wafer";

                string[] messageSubMaterialAttribNames = { "Gravure" };

                string[] messageSubMaterialAttribValues = { messageSubMaterialName };

                string[] messageSubMaterialDSNames = { "Thickness", "OrientationSpec", "Orientation", "Resistivity", "Bow" };

                string[] messageSubMaterialDSValues = { "997", "Off", "0,3", "0,085", "-7.56000000000000" };

                List<Wafer> subMaterials = new List<Wafer>();

                // Create Material objects with sample variables
                MaterialData firstScenarioMaterialData = new MaterialData();
                {
                    firstScenarioMaterialData.Name = messageMaterialName;

                    firstScenarioMaterialData.Product = messageMaterialProduct;

                    firstScenarioMaterialData.Type = messageMaterialType;

                    firstScenarioMaterialData.StateModel = messageMaterialStateModel;

                    firstScenarioMaterialData.State = messageMaterialState;

                    firstScenarioMaterialData.Form = messageMaterialForm;

                    firstScenarioMaterialData.Facility = messageMaterialFacility;

                    firstScenarioMaterialData.Flow = messageMaterialFlow;

                    firstScenarioMaterialData.Step = messageMaterialStep;

                    firstScenarioMaterialData.MaterialAttributes = new List<MaterialAttributes>()
                    {
                        new MaterialAttributes()
                        {
                            Name = messageMaterialAttribNames[0],
                            value = messageMaterialAttribValues[0]
                        },
                        new MaterialAttributes()
                        {
                            Name = messageMaterialAttribNames[1],
                            value = messageMaterialAttribValues[1]
                        }
                    };

                    Wafer subMaterial = new Wafer();

                    subMaterial.Name = messageSubMaterialName;

                    subMaterial.Form = messageSubMaterialForm;

                    subMaterial.MaterialAttributes = new List<MaterialAttributes>()
                    {
                        new MaterialAttributes()
                        {
                            Name = messageSubMaterialAttribNames[0],
                            value = messageSubMaterialAttribValues[0]
                        }
                    };

                    subMaterial.MaterialEDCData = new List<MaterialEDCData>()
                    {
                        new MaterialEDCData
                        {
                            Name = messageSubMaterialDSNames[0],
                            value = messageSubMaterialDSValues[0]
                        },
                        new MaterialEDCData
                        {
                            Name = messageSubMaterialDSNames[1],
                            value = messageSubMaterialDSValues[1]
                        },
                        new MaterialEDCData
                        {
                            Name = messageSubMaterialDSNames[2],
                            value = messageSubMaterialDSValues[2]
                        },
                        new MaterialEDCData
                        {
                            Name = messageSubMaterialDSNames[3],
                            value = messageSubMaterialDSValues[3]
                        },
                        new MaterialEDCData
                        {
                            Name = messageSubMaterialDSNames[4],
                            value = messageSubMaterialDSValues[4]
                        }
                    };

                    subMaterials.Add(subMaterial);

                    firstScenarioMaterialData.Wafers = subMaterials;
                }

                Dictionary<string, string> materialDCContext = new Dictionary<string, string>()
                {
                    { "Step", "M2-SL-Wafer-Start-07301F001_E" },
                    { "Operation", "Certificate" },
                    { "DataCollection", "ProductCertificateDataUnitTest" },
                    { "DataCollectionLimitSet", "ProductCertificateDataLimitSetUnitTest" },
                    { "DataCollectionType", "Immediate" }
                };

                // Execute scenario to create Integration Entry for Incoming Material Lot
                customExecutionScenario.IsToSetMaterialDCContext = true;

                customExecutionScenario.MaterialDCContext = new List<Dictionary<string, string>>()
                {
                    materialDCContext
                };

                customExecutionScenario.IsToSendIncomingMaterial = true;

                customExecutionScenario.GoodsReceiptCertificate.Material = firstScenarioMaterialData;

                customExecutionScenario.Setup();

                ///<Step> Material product properties.</Step>
                ///<ExpectedValue> Material should have the information sent on the ERP message.</ExpectedValue>
                Material firstMaterial = new Material();
                {
                    firstMaterial.Load(firstScenarioMaterialData.Name);

                    firstMaterial.Product.Load();

                    firstMaterial.Facility.Load();

                    firstMaterial.Flow.Load();

                    firstMaterial.Step.Load();

                    firstMaterial.LoadAttributes(new Collection<string> { messageMaterialAttribNames[0], messageMaterialAttribNames[1] });

                    firstMaterial.LoadChildren();

                    customTearDownManager.Push(firstMaterial);

                    materials.Add(firstMaterial);
                }

                ///<Step> Validate material properties. </Step>
                ///<ExpectedValue> Material should have the information sent on the ERP message. </ExpectedValue>
                Assert.IsTrue(firstMaterial.Name.Equals(messageMaterialName), $"Material Name should be: {firstMaterial.Name}, instead is: {messageMaterialName}");

                Assert.IsTrue(firstMaterial.Product.Name.Equals(messageMaterialProduct), $"Product Name should be: {firstMaterial.Product.Name}, instead is: {messageMaterialProduct}");

                Assert.IsTrue(firstMaterial.Type.Equals(messageMaterialType), $"Material Type should be: {firstMaterial.Type}, instead is: {messageMaterialType}");

                Assert.IsTrue(firstMaterial.Form.Equals(messageMaterialForm), $"Material Form should be: {firstMaterial.Form}, instead is: {messageMaterialForm}");

                Assert.IsTrue(firstMaterial.Facility.Name.Equals(messageMaterialFacility), $"Facility should be: {firstMaterial.Facility.Name}, instead is: {messageMaterialFacility}");

                Assert.IsTrue(firstMaterial.Flow.Name.Equals(messageMaterialFlow), $"Flow should be: {firstMaterial.Flow.Name}, instead is: {messageMaterialFlow}");

                Assert.IsTrue(firstMaterial.Step.Name.Equals(messageMaterialStep), $"Step should be: {firstMaterial.Step.Name}, instead is: {messageMaterialStep}");

                ///<Step> Validate Material attributes. </Step>
                ///<ExpectedValue> The 2 Material attributes should be updated. </ExpectedValue>
                Assert.IsTrue(firstMaterial.Attributes[messageMaterialAttribNames[0]].Equals(messageMaterialAttribValues[0]), $"Material attribute {messageMaterialAttribNames[0]} should be {messageMaterialAttribValues[0]}, but was {firstMaterial.Attributes[messageMaterialAttribNames[0]]}");

                Assert.IsTrue(firstMaterial.Attributes[messageMaterialAttribNames[1]].Equals(messageMaterialAttribValues[1]), $"Material attribute {messageMaterialAttribNames[1]} should be {messageMaterialAttribValues[1]}, but was {firstMaterial.Attributes[messageMaterialAttribNames[1]]}");

                foreach (Material subMaterial in firstMaterial.SubMaterials)
                {
                    subMaterial.Load();

                    Assert.IsTrue(subMaterial.Name.Equals(messageSubMaterialName), $"Sub-Material Name should be: {subMaterial.Name} instead is: {messageSubMaterialName}");

                    Assert.IsTrue(subMaterial.Form.Equals(messageSubMaterialForm), $"Sub-Material Form should be: {subMaterial.Form} instead is: {messageSubMaterialForm}");

                    subMaterial.LoadAttributes(new Collection<string> { messageSubMaterialAttribNames[0] });

                    ///<Step> Validate Sub-Material attributes. </Step>
                    ///<ExpectedValue> The 4 Material attributes should be updated. </ExpectedValue>
                    Assert.IsTrue(subMaterial.Attributes[messageSubMaterialAttribNames[0]].Equals(messageSubMaterialAttribValues[0]), $"Sub-Material attribute {messageSubMaterialAttribNames[0]} should be {messageSubMaterialAttribValues[0]}, but was {subMaterial.Attributes[messageSubMaterialAttribNames[0]]}");
                }

                firstMaterial.LoadRelations(new Collection<string> { "MaterialHoldReason" });

                MaterialHoldReasonCollection firstMaterialHoldReasons = firstMaterial.MaterialHoldReasons;

                Assert.IsTrue(firstMaterial.HoldCount == 1, $"Material should have one reason instead has {firstMaterial.HoldCount}");

                MaterialHoldReason firstMaterialHoldReason = firstMaterial.MaterialHoldReasons.FirstOrDefault();

                Assert.IsTrue(firstMaterialHoldReason.TargetEntity.Name.Equals("Out Of Spec"), $"Material Hold Reason Name should be: Out of Spec instead is: {firstMaterialHoldReason.TargetEntity.Name}");

                //Set one of the Test Reasons

                MaterialData secondScenarioMaterialData = firstScenarioMaterialData;

                secondScenarioMaterialData.Wafers[0].MaterialEDCData[0].value = "650";
                secondScenarioMaterialData.Wafers[0].MaterialEDCData[1].value = "Off";
                secondScenarioMaterialData.Wafers[0].MaterialEDCData[2].value = "0.3";
                secondScenarioMaterialData.Wafers[0].MaterialEDCData[3].value = "0.085";
                secondScenarioMaterialData.Wafers[0].MaterialEDCData[4].value = "-5";

                customExecutionScenario.GoodsReceiptCertificate.Material = secondScenarioMaterialData;

                customExecutionScenario.Setup();

                Material secondMaterial = new Material();
                {
                    secondMaterial.Load(messageMaterialName);
                }

                secondMaterial.LoadRelations(new Collection<string> { "MaterialHoldReason" });

                MaterialHoldReasonCollection secondMaterialHoldReasons = secondMaterial.MaterialHoldReasons;

                Assert.IsTrue(secondMaterial.HoldCount == 1, $"Material should have one reason instead has {secondMaterial.HoldCount}");

                MaterialHoldReason secondMaterialHoldReason = secondMaterial.MaterialHoldReasons.FirstOrDefault();

                Assert.IsTrue(!secondMaterialHoldReason.TargetEntity.Name.Equals("Out Of Spec"), $"Material Hold Reason Name should be: {secondMaterialHoldReason.TargetEntity.Name} instead is: Out Of Spec");
            }
            finally
            {
                ///<Step> Clear the created materials </Step>
                if (materials.Count > 0)
                {
                    materials.Load();

                    materials.TerminateMaterialCollection();

                    customExecutionScenario.CompleteCleanUp();
                }
            }
        }
    }
}
