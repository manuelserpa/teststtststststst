using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.ERP;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Base;
using Cmf.Foundation.Security;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Cmf.Custom.AMSOsram.Actions.MasterData
{
    public class CustomProcessProduct : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Summary text
            ///     
            /// Action Groups:
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>
            #endregion

            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, AMSOsramConstants.EntityTypes.IntegrationEntry);

            if (integrationEntry is null || integrationEntry.IntegrationMessage is null || integrationEntry.IntegrationMessage.Message is null || integrationEntry.IntegrationMessage.Message.Length <= 0)
            {
                return false;
            }

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            //System
            UseReference("", "System");
            UseReference("", "System.Collections.Generic");
            UseReference("", "System.Data");
            UseReference("", "System.Linq");
            UseReference("", "System.Text");

            //Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.GenericTables");
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common");
            UseReference("Cmf.Foundation.Common.dll", "Cmf.Foundation.Common.Base");
            UseReference("Cmf.Foundation.Security.dll", "Cmf.Foundation.Security");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.ERP");

            // Load Integration Entry
            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, AMSOsramConstants.EntityTypes.IntegrationEntry);

            // Cast Integation Entry Message to string
            string message = Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            // Deserialize XML Message to an object
            ProductData productData = AMSOsramUtilities.DeserializeXmlToObject<ProductData>(message);

            // ChangeSet to create/update Products
            ChangeSet productsChangeSet = new ChangeSet();
            {
                productsChangeSet.Name = Guid.NewGuid().ToString("N");

                productsChangeSet.Description = $"ChangeSet to create/update Product {productData.Name}.";

                productsChangeSet.Type = $"General";

                productsChangeSet.MakeEffectiveOnApproval = true;
            }

            productsChangeSet.Create();

            // Create Product context using data received from Integration Entry
            Product product = new Product();

            product.Name = productData.Name;

            product.Type = productData.Type;

            if (product.ObjectExists())
            {
                product.Load();

                product.CreateVersion(false, productsChangeSet, string.Empty);
            }
            else
            {
                product.CreateVersion(true, productsChangeSet, string.Empty);
            }

            product.Description = productData.Description;

            product.ProductType = AMSOsramUtilities.GetValueAsEnum<ProductType>(productData.ProductType);

            product.DefaultUnits = productData.DefaultUnits;

            product.IsEnabled = AMSOsramUtilities.GetValueAsNullableBoolean(productData.IsEnabled);

            product.Yield = AMSOsramUtilities.GetValueAsDecimal(productData.Yield);

            ProductGroup productGroup = new ProductGroup();
            {
                productGroup.Load(productData.ProductGroup);
            };

            product.ProductGroup = productGroup;

            product.MaximumMaterialSize = AMSOsramUtilities.GetValueAsNullableDecimal(productData.MaximumMaterialSize);

            product.FlowPath = productData.FlowPath;

            if (!productData.UnitConversionFactors.IsNullOrEmpty())
            {
                GenericTable genericTable = new GenericTable();
                {
                    genericTable.Load("ProductUnitConversionFactors");
                }

                DataSet dataSet = new DataSet();

                foreach (Conversion conversion in productData.UnitConversionFactors)
                {
                    Foundation.BusinessObjects.QueryObject.FilterCollection filters = new Foundation.BusinessObjects.QueryObject.FilterCollection();

                    filters.Add(new Foundation.BusinessObjects.QueryObject.Filter() { Name = "ProductName", Operator = FieldOperator.IsEqualTo, Value = productData.Name });
                    filters.Add(new Foundation.BusinessObjects.QueryObject.Filter() { Name = "FromUnit", Operator = FieldOperator.IsEqualTo, Value = conversion.FromUnit });
                    filters.Add(new Foundation.BusinessObjects.QueryObject.Filter() { Name = "ToUnit", Operator = FieldOperator.IsEqualTo, Value = conversion.ToUnit });

                    genericTable.LoadData(filters);

                    dataSet = NgpDataSet.ToDataSet(genericTable.Data);

                    if (dataSet.Tables[0].Rows.Count == 0)
                    {
                        DataRow dataRow = dataSet.Tables[0].NewRow();

                        dataRow["ProductUnitConversionFactorsId"] = -1;
                        dataRow["LastServiceHistoryId"] = -1;
                        dataRow["LastOperationHistorySeq"] = -1;
                        dataRow["ProductName"] = productData.Name;
                        dataRow["FromUnit"] = conversion.FromUnit;
                        dataRow["ToUnit"] = conversion.ToUnit;
                        dataRow["ConversionFactor"] = conversion.ConversionFactor;

                        dataSet.Tables[0].Rows.Add(dataRow);
                    }
                    else
                    {
                        dataSet.Tables[0].Rows[0]["ConversionFactor"] = conversion.ConversionFactor;
                    }
                }

                if (dataSet != null & dataSet.Tables.Count > 0)
                {
                    genericTable.InsertOrUpdateRows(NgpDataSet.FromDataSet(dataSet));
                }
            }

            // TO DO: SubProducts

            product.InitialUnitCost = AMSOsramUtilities.GetValueAsDecimal(productData.InitialUnitCost);

            product.FinishedUnitCost = AMSOsramUtilities.GetValueAsDecimal(productData.FinishedUnitCost);

            product.CycleTime = AMSOsramUtilities.GetValueAsDecimal(productData.CycleTime);

            product.IncludeInSchedule = AMSOsramUtilities.GetValueAsNullableBoolean(productData.IncludeInSchedule);

            product.CapacityClass = productData.CapacityClass;

            product.MaterialTransferMode = AMSOsramUtilities.GetValueAsEnum<MaterialTransferMode>(productData.MaterialTransferMode);

            product.MaterialTransferApprovalMode = AMSOsramUtilities.GetValueAsEnum<MaterialTransferApprovalMode>(productData.MaterialTransferApprovalMode);

            product.MaterialTransferAllowedPickup = AMSOsramUtilities.GetValueAsEnum<MaterialTransferAllowedPickup>(productData.MaterialTransferAllowedPickup);

            product.IsEnabledForMaintenanceManagement = AMSOsramUtilities.GetValueAsBoolean(productData.IsEnableForMaintenanceManagement);

            product.MaintenanceManagementConsumeQuantity = AMSOsramUtilities.GetValueAsBoolean(productData.MaintenanceManagementConsumerQuantity);

            product.IsDiscrete = AMSOsramUtilities.GetValueAsNullableBoolean(productData.IsDiscrete);

            product.MoistureSensitivityLevel = productData.MoistureSensitivityLevel;

            if (!string.IsNullOrWhiteSpace(productData.FloorLife))
            {
                product.FloorLife = Int32.TryParse(productData.FloorLife, out int floorLife) ? floorLife : default(int?);

                product.FloorLifeUnitOfTime = AMSOsramUtilities.GetValueAsEnum<UnitOfTime>(productData.FloorLifeUnitOfTime);
            }

            product.RequiresApproval = AMSOsramUtilities.GetValueAsNullableBoolean(productData.RequiresApproval);

            if (!string.IsNullOrWhiteSpace(productData.ApprovalRole))
            {
                Role approvalRole = new Role();
                {
                    approvalRole.Load(productData.ApprovalRole);
                }

                product.ApprovalRole = approvalRole;
            }

            product.CanSplitForPicking = AMSOsramUtilities.GetValueAsBoolean(productData.CanSplitForPicking);

            product.MaterialLogisticsDefaultRequestQuantity = AMSOsramUtilities.GetValueAsNullableDecimal(productData.MaterialLogisticsDefaultRequestQuantity);

            product.ConsumptionScrap = AMSOsramUtilities.GetValueAsDecimal(productData.ConsumptionScrap);

            product.AdditionalConsumptionQuantity = AMSOsramUtilities.GetValueAsDecimal(productData.AdditionalConsumptionQuantity);

            product.IsEnabledForMaterialLogistics = AMSOsramUtilities.GetValueAsBoolean(productData.IsEnabledForMaterialLogistics);

            if (!string.IsNullOrWhiteSpace(productData.DefaultBOM))
            {
                BOM defaultBOM = new BOM();
                {
                    defaultBOM.Load(productData.DefaultBOM);
                }

                product.DefaultBOM = defaultBOM;
            }

            if (productData.ProductManufacturer != null)
            {
                product.ProductManufacturers = new ProductManufacturerCollection()
                {
                        new ProductManufacturer()
                        {
                            Name = productData.ProductManufacturer.Name,
                            Note = productData.ProductManufacturer.Note
                        }
                 };
            }

            product.Save();

            #region Parameters

            if (!productData.ProductParametersData.IsNullOrEmpty())
            {
                ProductParameterCollection productParameters = new ProductParameterCollection();

                // Associate relation between Product to Parameter
                foreach (ProductParameterData parameterData in productData.ProductParametersData)
                {
                    Parameter parameter = new Parameter();
                    {
                        parameter.Name = parameterData.Name;
                    }

                    if (parameter.ObjectExists())
                    {
                        parameter.Load();

                        ProductParameter productParameter = new ProductParameter();
                        {
                            productParameter.SourceEntity = product;
                            productParameter.TargetEntity = parameter;
                            productParameter.Type = ProductGroupParameterType.Characteristic;
                            productParameter.Value = AMSOsramUtilities.GetParameterValueAsDataType(parameter.DataType, parameterData.Value).ToString();

                            productParameters.Add(productParameter);
                        }
                    }
                }

                if (!productParameters.IsNullOrEmpty())
                {
                    //Add relation between Product and Parameters
                    product.AddRelations(productParameters);
                }
            }

            #endregion

            #region Attributes

            if (!productData.ProductAttributesData.IsNullOrEmpty())
            {
                EntityType entityType = new EntityType();
                {
                    entityType.Load(AMSOsramConstants.EntityTypes.Product);
                    entityType.LoadProperties();
                }

                // Get attribute names and Scalar Type associated to Entity Type Product
                Dictionary<string, object> productAttributes = entityType.Properties.Where(w => w.PropertyType == EntityTypePropertyType.Attribute).Select(s => new KeyValuePair<string, object>(s.Name, s.ScalarType)).ToDictionary(d => d.Key, d => d.Value);

                product.LoadAttributes();

                AttributeCollection attributes = product.RelatedAttributes;

                AttributeCollection relatedAttributes = new AttributeCollection();

                // Associate relation between Product to Attribute
                foreach (ProductAttributeData attributeData in productData.ProductAttributesData)
                {
                    if (attributeData.Name.ToUpper().Equals("STATUS"))
                    {
                        product.IsEnabled = !string.IsNullOrWhiteSpace(attributeData.Value) && Convert.ToInt32(attributeData.Value) < 97;

                        product.Save();
                    }

                    if (productAttributes.ContainsKey(attributeData.Name))
                    {
                        ScalarType scalarType = productAttributes[attributeData.Name] as ScalarType;

                        if (attributes.ContainsKey(attributeData.Name))
                        {
                            relatedAttributes[attributeData.Name] = AMSOsramUtilities.GetAttributeValueAsDataType(scalarType, attributeData.Value);
                        }
                        else
                        {
                            relatedAttributes.Add(attributeData.Name, AMSOsramUtilities.GetAttributeValueAsDataType(scalarType, attributeData.Value));
                        }
                    }
                }

                if (!relatedAttributes.IsNullOrEmpty())
                {
                    //Add relation between Product and Attributes
                    product.SaveRelatedAttributes(relatedAttributes);
                }
            }

            #endregion

            productsChangeSet.Load();

            productsChangeSet.RequestApproval();

            if (productsChangeSet.SystemState == ChangeSetSystemState.InApproval)
            {
                productsChangeSet.Approve();
            }

            if (productsChangeSet.UniversalState == UniversalState.Active)
            {
                productsChangeSet.MakeObjectsEffective();
            }

            if (productsChangeSet.UniversalState != UniversalState.Terminated)
            {
                productsChangeSet.Terminate();
            }

            //---End DEE Code---

            return Input;
        }
    }
}
