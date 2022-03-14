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

            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, "IntegrationEntry");

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
            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, "IntegrationEntry");

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
            product.Description = productData.Description;
            product.Type = productData.Type;
            product.ProductType = Enum.TryParse(productData.ProductType, out ProductType productType) ? productType : default(ProductType);
            product.DefaultUnits = productData.DefaultUnits;

            if (product.ObjectExists())
            {
                product.Load();

                product.CreateVersion(false, productsChangeSet, string.Empty);
            }
            else
            {
                product.CreateVersion(true, productsChangeSet, string.Empty);
            }

            if (!string.IsNullOrWhiteSpace(productData.IsEnabled))
            {
                product.IsEnabled = productData.IsEnabled.ToUpper() == "Y" ? true : false;
            }

            product.Yield = Convert.ToDecimal(productData.Yield);

            ProductGroup productGroup = new ProductGroup();
            {
                productGroup.Load(productData.ProductGroup);
            };

            product.ProductGroup = productGroup;

            product.MaximumMaterialSize = Decimal.TryParse(productData.MaximumMaterialSize, out decimal maximumMaterialSize) ? maximumMaterialSize : (decimal?)null;
            product.FlowPath = productData.FlowPath;

            if (productData.UnitConversionFactors != null && productData.UnitConversionFactors.Any())
            {
                GenericTable genericTable = new GenericTable();
                {
                    genericTable.Load("ProductUnitConversionFactors");
                }

                DataTable unitConversionFactors = new DataTable();

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

            product.InitialUnitCost = Convert.ToDecimal(productData.InitialUnitCost);
            product.FinishedUnitCost = Convert.ToDecimal(productData.FinishedUnitCost);
            product.CycleTime = Convert.ToDecimal(productData.CycleTime);

            if (!string.IsNullOrWhiteSpace(productData.IncludeInSchedule))
            {
                product.IncludeInSchedule = productData.IncludeInSchedule.ToUpper() == "Y" ? true : false;
            }

            product.CapacityClass = productData.CapacityClass;
            product.MaterialTransferMode = Enum.TryParse(productData.MaterialTransferMode, out MaterialTransferMode materialTransferMode) ? materialTransferMode : default(MaterialTransferMode);
            product.MaterialTransferApprovalMode = Enum.TryParse(productData.MaterialTransferApprovalMode, out MaterialTransferApprovalMode materialTransferApprovalMode) ? materialTransferApprovalMode : default(MaterialTransferApprovalMode);
            product.MaterialTransferAllowedPickup = Enum.TryParse(productData.MaterialTransferAllowedPickup, out MaterialTransferAllowedPickup materialTransferAllowedPickup) ? materialTransferAllowedPickup : default(MaterialTransferAllowedPickup);

            if (!string.IsNullOrWhiteSpace(productData.IsEnableForMaintenanceManagement))
            {
                product.IsEnabledForMaintenanceManagement = productData.IsEnableForMaintenanceManagement.ToUpper() == "Y" ? true : false;
            }

            if (!string.IsNullOrWhiteSpace(productData.MaintenanceManagementConsumerQuantity))
            {
                product.MaintenanceManagementConsumeQuantity = productData.MaintenanceManagementConsumerQuantity.ToUpper() == "Y" ? true : false;
            }

            if (!string.IsNullOrWhiteSpace(productData.IsDiscrete))
            {
                product.IsDiscrete = productData.IsDiscrete.ToUpper() == "Y" ? true : false;
            }

            product.MoistureSensitivityLevel = productData.MoistureSensitivityLevel;

            if (!string.IsNullOrWhiteSpace(productData.FloorLife))
            {
                product.FloorLife = Int32.TryParse(productData.FloorLife, out int floorLife) ? floorLife : default(int?);
                product.FloorLifeUnitOfTime = Enum.TryParse(productData.FloorLifeUnitOfTime, out UnitOfTime floorLifeUnitOfTime) ? floorLifeUnitOfTime : default(UnitOfTime);
            }

            if (!string.IsNullOrWhiteSpace(productData.RequiresApproval))
            {
                product.RequiresApproval = productData.RequiresApproval.ToUpper() == "Y" ? true : false;
            }

            if (!string.IsNullOrWhiteSpace(productData.ApprovalRole))
            {
                product.ApprovalRole = new Role()
                {
                    Name = productData.ApprovalRole
                };
            }

            if (!string.IsNullOrWhiteSpace(productData.CanSplitForPicking))
            {
                product.CanSplitForPicking = productData.CanSplitForPicking.ToUpper() == "Y" ? true : false;
            }

            product.MaterialLogisticsDefaultRequestQuantity = Decimal.TryParse(productData.MaterialLogisticsDefaultRequestQuantity, out decimal materialLogisticsDefaultRequestQuantity) ? materialLogisticsDefaultRequestQuantity : (decimal?)null;
            product.ConsumptionScrap = Convert.ToDecimal(productData.ConsumptionScrap);
            product.AdditionalConsumptionQuantity = Convert.ToDecimal(productData.AdditionalConsumptionQuantity);

            if (!string.IsNullOrWhiteSpace(productData.IsEnableForMaintenanceManagement))
            {
                product.IsEnabledForMaterialLogistics = productData.IsEnabledForMaterialLogistics.ToUpper() == "Y" ? true : false;
            }

            if (!string.IsNullOrWhiteSpace(productData.DefaultBOM))
            {
                product.DefaultBOM = new BOM
                {
                    Name = productData.DefaultBOM
                };
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

            if (productData.ProductParametersData != null && productData.ProductParametersData.Any())
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
                            productParameter.Value = parameterData.Value;

                            productParameters.Add(productParameter);
                        }
                    }
                }

                if (productParameters != null && productParameters.Any())
                {
                    //Add relation between Product and Parameters
                    product.AddRelations(productParameters);
                }
            }

            #endregion

            #region Attributes

            if (productData.ProductAttributesData != null && productData.ProductAttributesData.Any())
            {
                EntityType entityType = new EntityType();
                {
                    entityType.Name = "Product";
                    entityType.Load();
                    entityType.LoadProperties();
                }

                // List of all attributes associated to Entity Type Product
                List<string> productAttributes = entityType.Properties.Where(w => w.PropertyType == EntityTypePropertyType.Attribute).Select(s => s.Name).ToList();

                product.LoadAttributes();

                AttributeCollection attributes = product.Attributes;

                // Associate relation between Product to Attribute
                foreach (ProductAttributeData attributeData in productData.ProductAttributesData)
                {
                    if (attributeData.Name.ToUpper().Equals("STATUS"))
                    {
                        product.IsEnabled = Convert.ToInt32(attributeData.Value) < 97;
                    }

                    if (productAttributes.Contains(attributeData.Name))
                    {
                        if (attributes.ContainsKey(attributeData.Name))
                        {
                            attributes[attributeData.Name] = attributeData.Value;
                        }
                        else
                        {
                            attributes.Add(attributeData.Name, attributeData.Value);
                        }
                    }
                }

                if (attributes != null && attributes.Any())
                {
                    //Add relation between Product and Attributes
                    product.SaveAttributes(attributes);
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
