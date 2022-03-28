using Cmf.Custom.Tests.Biz.Common.ERP.Material;
using Cmf.Custom.Tests.Biz.Common.Extensions;
using Cmf.Custom.Tests.Biz.Common.Scenarios;
using Cmf.Custom.Tests.Biz.Common.Utilities;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
            // Set sample values for Material
            string messageMaterialName = Guid.NewGuid().ToString("N");

            string messageMaterialProduct = "11084155";

            string messageMaterialType = "Production";

            string messageMaterialStateModel = string.Empty;

            string messageMaterialState = string.Empty;

            string messageMaterialForm = "Lot";

            string messageMaterialFacility = "OSFET";

            string messageMaterialFlow = "StorageFlow";

            string messageMaterialStep = "StorageStep";

            string[] messageMaterialAttribNames = { "PurchaseOrder", "GoodsReceiptNo", "GoodsReceiptDate", "ShipmentReferenceNo", "Box", "Alias" };

            string[] messageMaterialAttribValues = { "4510859554", "5043575531", "08.11.2021", "L-108-84155-ZK21435-C009", "L-108", string.Empty };

            // Set sample values for SubMaterial
            string messageSubMaterialForm = "Wafer";

            string[] messageSubMaterialAttribNames = { "Gravure" };

            string[] messageSubMaterialAttribValues = { "NK17040736E2" };

            string[] messageSubMaterialDSNames = { "Thickness", "OrientationSpec", "Orientation", "Resistivity", "Bow" };

            string[] messageSubMaterialDSValues = { "997", "Off", "0,3", "0,085", "-5.56000000000000" };

            List<Wafer> subMaterials = new List<Wafer>();

            // Create Material objects with sample variables
            MaterialData materialData = new MaterialData();
            {
                MaterialData material = new MaterialData();

                material.Name = messageMaterialName;

                material.Product = messageMaterialProduct;

                material.Type = messageMaterialType;

                material.StateModel = messageMaterialStateModel;

                material.State = messageMaterialState;

                material.Form = messageMaterialForm;

                material.Facility = messageMaterialFacility;

                material.Flow = messageMaterialFlow;

                material.Step = messageMaterialStep;

                material.MaterialAttributes = new List<MaterialAttributes>()
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
                    },
                    new MaterialAttributes()
                    {
                        Name = messageMaterialAttribNames[2],
                        value = messageMaterialAttribValues[2]
                    },
                    new MaterialAttributes()
                    {
                        Name = messageMaterialAttribNames[3],
                        value = messageMaterialAttribValues[3]
                    },
                    new MaterialAttributes()
                    {
                        Name = messageMaterialAttribNames[4],
                        value = messageMaterialAttribValues[4]
                    },
                    new MaterialAttributes()
                    {
                        Name = messageMaterialAttribNames[5],
                        value = messageMaterialAttribValues[5]
                    }
                };

                Wafer subMaterial = new Wafer();

                subMaterial.Name = Guid.NewGuid().ToString("N");

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

                material.Wafers = subMaterials;
            }

            // Execute scenario to create Integration Entry for Incoming Material Lot
            customExecutionScenario.IsToSendIncomingMaterial = true;

            customExecutionScenario.GoodsReceiptCertificate.Material = materialData;

            customExecutionScenario.Setup();

            // Assert that Integration Entries are created and processed
            Assert.IsTrue(customExecutionScenario.IntegrationEntries.Count > 0, "Integration Entries should have been created");

            foreach (IntegrationEntry ie in customExecutionScenario.IntegrationEntries)
            {
                Assert.IsTrue(ie.IsIntegrationEntryProcessed(), $"Integration Entry was not processed. Error Message: {ie.ResultDescription}");
            }

            ///<Step> Validate creation of Integration Entries. </Step>
            ///<ExpectedValue> Integration Entry should have been created. </ExpectedValue>
            IntegrationEntry integrationEntry = CustomUtilities.GetIntegrationEntry(messageMaterialName);

            Assert.IsTrue(integrationEntry.IsIntegrationEntryProcessed(), $"Integration Entry was processed");

            string errorMessage = $"New{messageMaterialType}";
            Assert.IsTrue(integrationEntry.ResultDescription.Contains(messageMaterialType), $"Error message should be: {messageMaterialType}, but instead is: {integrationEntry.ResultDescription}");

            ///<Step> Material product properties.</Step>
            ///<ExpectedValue> Material should have the information sent on the ERP message.</ExpectedValue>
        }
    }
}
