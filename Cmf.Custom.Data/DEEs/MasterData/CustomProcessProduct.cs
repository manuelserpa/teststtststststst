using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.ERP;
using Cmf.Foundation.BusinessObjects.GenericTables;
using Cmf.Foundation.Common;
using Cmf.Foundation.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.Security.Abstractions;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.MasterData
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

            IIntegrationEntry integrationEntry = amsOSRAMUtilities.GetInputItem<IIntegrationEntry>(Input, Constants.IntegrationEntry);

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
            UseReference("", "System.Data");
            UseReference("", "System.Text");

            //Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.GenericTables");

            //Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.ERP");

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            // Load Integration Entry
            IIntegrationEntry integrationEntry = amsOSRAMUtilities.GetInputItem<IIntegrationEntry>(Input, Constants.IntegrationEntry);

            // Cast Integation Entry Message to string
            string message = Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            // Deserialize XML Message to an object
            ProductData productData = amsOSRAMUtilities.DeserializeXmlToObject<ProductData>(message);

            // Create Product context using data received from Integration Entry
            IProduct product = entityFactory.Create<IProduct>();
            product.Name = productData.Name;

            #region Properties

            if (product.ObjectExists())
            {
                product.Load();
            }

            if (!string.IsNullOrWhiteSpace(productData.Description))
            {
                product.Description = productData.Description;
            }

            if (!string.IsNullOrWhiteSpace(productData.Type))
            {
                product.Type = productData.Type;
            }

            if (!string.IsNullOrWhiteSpace(productData.ProductType))
            {
                product.ProductType = amsOSRAMUtilities.GetValueAsEnum<ProductType>(productData.ProductType);
            }

            if (!string.IsNullOrWhiteSpace(productData.DefaultUnits))
            {
                product.DefaultUnits = productData.DefaultUnits;
            }

            if (!string.IsNullOrWhiteSpace(productData.IsEnabled))
            {
                product.IsEnabled = amsOSRAMUtilities.GetValueAsNullableBoolean(productData.IsEnabled);
            }

            if (!string.IsNullOrWhiteSpace(productData.Yield))
            {
                product.Yield = amsOSRAMUtilities.GetValueAsDecimal(productData.Yield);
            }

            if (!string.IsNullOrWhiteSpace(productData.ProductGroup))
            {
                IProductGroup productGroup = entityFactory.Create<IProductGroup>();
                productGroup.Load(productData.ProductGroup);

                product.ProductGroup = productGroup;
            }

            if (!string.IsNullOrWhiteSpace(productData.MaximumMaterialSize))
            {
                product.MaximumMaterialSize = amsOSRAMUtilities.GetValueAsNullableDecimal(productData.MaximumMaterialSize);
            }

            if (!string.IsNullOrWhiteSpace(productData.InitialUnitCost))
            {
                product.InitialUnitCost = amsOSRAMUtilities.GetValueAsDecimal(productData.InitialUnitCost);
            }

            if (!string.IsNullOrWhiteSpace(productData.FinishedUnitCost))
            {
                product.FinishedUnitCost = amsOSRAMUtilities.GetValueAsDecimal(productData.FinishedUnitCost);
            }

            if (!string.IsNullOrWhiteSpace(productData.CycleTime))
            {
                product.CycleTime = amsOSRAMUtilities.GetValueAsDecimal(productData.CycleTime);
            }

            if (!string.IsNullOrWhiteSpace(productData.IncludeInSchedule))
            {
                product.IncludeInSchedule = amsOSRAMUtilities.GetValueAsNullableBoolean(productData.IncludeInSchedule);
            }

            if (!string.IsNullOrWhiteSpace(productData.CapacityClass))
            {
                product.CapacityClass = productData.CapacityClass;
            }

            if (!string.IsNullOrWhiteSpace(productData.MaterialTransferMode))
            {
                product.MaterialTransferMode = amsOSRAMUtilities.GetValueAsEnum<MaterialTransferMode>(productData.MaterialTransferMode);
            }

            if (!string.IsNullOrWhiteSpace(productData.MaterialTransferApprovalMode))
            {
                product.MaterialTransferApprovalMode = amsOSRAMUtilities.GetValueAsEnum<MaterialTransferApprovalMode>(productData.MaterialTransferApprovalMode);
            }

            if (!string.IsNullOrWhiteSpace(productData.MaterialTransferAllowedPickup))
            {
                product.MaterialTransferAllowedPickup = amsOSRAMUtilities.GetValueAsEnum<MaterialTransferAllowedPickup>(productData.MaterialTransferAllowedPickup);
            }

            if (!string.IsNullOrWhiteSpace(productData.MaintenanceManagementConsumerQuantity))
            {
                product.MaintenanceManagementConsumeQuantity = amsOSRAMUtilities.GetValueAsBoolean(productData.MaintenanceManagementConsumerQuantity);
            }

            if (!string.IsNullOrWhiteSpace(productData.IsDiscrete))
            {
                product.IsDiscrete = amsOSRAMUtilities.GetValueAsNullableBoolean(productData.IsDiscrete);
            }

            if (!string.IsNullOrWhiteSpace(productData.MoistureSensitivityLevel))
            {
                product.MoistureSensitivityLevel = productData.MoistureSensitivityLevel;
            }

            if (!string.IsNullOrWhiteSpace(productData.FloorLife))
            {
                product.FloorLife = Int32.TryParse(productData.FloorLife, out int floorLife) ? floorLife : default(int?);
            }

            if (!string.IsNullOrWhiteSpace(productData.FloorLifeUnitOfTime))
            {
                product.FloorLifeUnitOfTime = amsOSRAMUtilities.GetValueAsEnum<UnitOfTime>(productData.FloorLifeUnitOfTime);
            }

            if (!string.IsNullOrWhiteSpace(productData.RequiresApproval))
            {
                product.RequiresApproval = amsOSRAMUtilities.GetValueAsNullableBoolean(productData.RequiresApproval);
            }

            if (!string.IsNullOrWhiteSpace(productData.ApprovalRole))
            {
                IRole approvalRole = new Role();
                {
                    approvalRole.Load(productData.ApprovalRole);
                }

                product.ApprovalRole = approvalRole;
            }

            if (!string.IsNullOrWhiteSpace(productData.CanSplitForPicking))
            {
                product.CanSplitForPicking = amsOSRAMUtilities.GetValueAsBoolean(productData.CanSplitForPicking);
            }

            if (!string.IsNullOrWhiteSpace(productData.MaterialLogisticsDefaultRequestQuantity))
            {
                product.MaterialLogisticsDefaultRequestQuantity = amsOSRAMUtilities.GetValueAsNullableDecimal(productData.MaterialLogisticsDefaultRequestQuantity);
            }

            if (!string.IsNullOrWhiteSpace(productData.ConsumptionScrap))
            {
                product.ConsumptionScrap = amsOSRAMUtilities.GetValueAsDecimal(productData.ConsumptionScrap);
            }

            if (!string.IsNullOrWhiteSpace(productData.AdditionalConsumptionQuantity))
            {
                product.AdditionalConsumptionQuantity = amsOSRAMUtilities.GetValueAsDecimal(productData.AdditionalConsumptionQuantity);
            }

            if (!string.IsNullOrWhiteSpace(productData.IsEnabledForMaterialLogistics))
            {
                product.IsEnabledForMaterialLogistics = amsOSRAMUtilities.GetValueAsBoolean(productData.IsEnabledForMaterialLogistics);
            }

            if (!string.IsNullOrWhiteSpace(productData.DefaultBOM))
            {
                IBOM defaultBOM = entityFactory.Create<IBOM>();
                defaultBOM.Load(productData.DefaultBOM);

                product.DefaultBOM = defaultBOM;
            }

            if (productData.ProductManufacturer != null)
            {
                IProductManufacturer productManufacturer = entityFactory.Create<IProductManufacturer>();
                productManufacturer.Name = productData.ProductManufacturer.Name;
                productManufacturer.Note = productData.ProductManufacturer.Note;

                IProductManufacturerCollection productManufacturers = entityFactory.CreateCollection<IProductManufacturerCollection>();
                productManufacturers.Add(productManufacturer);

                product.ProductManufacturers = productManufacturers;
            }

            #endregion

            #region Create Version

            IChangeSet changeSet = entityFactory.Create<IChangeSet>();
            changeSet.Name = Guid.NewGuid().ToString();
            changeSet.Type = "General";
            changeSet.MakeEffectiveOnApproval = true;

            changeSet.Create();

            product.CreateEntity(changeSet);

            changeSet.RequestApproval();

            changeSet.MakeObjectsEffective();

            #endregion

            #region Unit Conversion Factor

            if (!productData.UnitConversionFactors.IsNullOrEmpty())
            {
                IGenericTable genericTable = new GenericTable();
                {
                    genericTable.Load("ProductUnitConversionFactors");
                }

                DataSet dataSet = new DataSet();

                foreach (Conversion conversion in productData.UnitConversionFactors)
                {
                    IFilterCollection filters = new Foundation.BusinessObjects.QueryObject.FilterCollection();

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

            #endregion

            #region Parameters

            if (!productData.ProductParametersData.IsNullOrEmpty())
            {
                IProductParameterCollection productParameters = entityFactory.CreateCollection<IProductParameterCollection>();

                // Associate relation between Product to Parameter
                foreach (ProductParameterData parameterData in productData.ProductParametersData)
                {
                    IParameter parameter = entityFactory.Create<IParameter>();
                    parameter.Name = parameterData.Name;

                    if (parameter.ObjectExists())
                    {
                        parameter.Load();

                        IProductParameter productParameter = entityFactory.Create<IProductParameter>();
    
                        productParameter.SourceEntity = product;
                        productParameter.TargetEntity = parameter;
                        productParameter.Type = ProductGroupParameterType.Characteristic;
                        productParameter.Value = amsOSRAMUtilities.GetParameterValueAsDataType(parameter.DataType, parameterData.Value).ToString();

                        productParameters.Add(productParameter);
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
                IEntityType entityType = new EntityType();
                {
                    entityType.Load(amsOSRAMConstants.EntityTypes.Product);
                    entityType.LoadProperties();
                }

                // Get attribute names and Scalar Type associated to Entity Type Product
                Dictionary<string, object> productAttributes = entityType.Properties.Where(w => w.PropertyType == EntityTypePropertyType.Attribute).Select(s => new KeyValuePair<string, object>(s.Name, s.ScalarType)).ToDictionary(d => d.Key, d => d.Value);

                product.LoadAttributes();

                IAttributeCollection attributes = product.RelatedAttributes;

                IAttributeCollection relatedAttributes = new AttributeCollection();

                // Associate relation between Product to Attribute
                foreach (ProductAttributeData attributeData in productData.ProductAttributesData)
                {
                    if (productAttributes.ContainsKey(attributeData.Name))
                    {
                        IScalarType scalarType = productAttributes[attributeData.Name] as IScalarType;

                        if (attributes.ContainsKey(attributeData.Name))
                        {
                            relatedAttributes[attributeData.Name] = amsOSRAMUtilities.GetAttributeValueAsDataType(scalarType, attributeData.Value);
                        }
                        else
                        {
                            relatedAttributes.Add(attributeData.Name, amsOSRAMUtilities.GetAttributeValueAsDataType(scalarType, attributeData.Value));
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

            //---End DEE Code---

            return Input;
        }
    }
}
