﻿using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessOrchestration.Abstractions;
using Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.TableManagement.OutputObjects;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Foundation.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.BusinessObjects.QueryObject;
using System.Dynamic;

namespace Cmf.Custom.amsOSRAM.Actions.ProcessRules._2._2._0.Before
{
    public class CustomTibcoEMSGatewayResolverGenericTable : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            //Foundation
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.Abstractions");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.TableManagement.InputObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.TableManagement.OutputObjects");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.QueryObject");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.Abstractions");

            //Custom
            UseReference("Cmf.Common.CustomActionUtilities.dll", "Cmf.Common.CustomActionUtilities");

            //System
            UseReference("", "System.Dynamic");

            string tableName = "CustomTibcoEMSGatewayResolver";

            dynamic propertyToAdd1 = new ExpandoObject();
            propertyToAdd1.Name = "ReplyTo";
            propertyToAdd1.Position = 2;
            propertyToAdd1.Description = "Reply to Topic/Queue";
            propertyToAdd1.IsKey = false;
            propertyToAdd1.IsIndexed = false;
            propertyToAdd1.IsMandatory = false;
            propertyToAdd1.ReferenceType = ReferenceType.None;
            propertyToAdd1.ScalarTypeName = "NVarChar";
            propertyToAdd1.Size = 256;

            List<ExpandoObject> propertiesToAdd = new List<ExpandoObject>
            {
                propertyToAdd1
            };

            #region Service Provider

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            ITableOrchestration _tableOrchestration = serviceProvider.GetService<ITableOrchestration>();

            #endregion

            // Get Generic Table by name
            GetGenericTablesByFilterOutput output = _tableOrchestration.GetGenericTablesByFilter(new GetGenericTablesByFilterInput()
            {
                Filter = new FilterCollection()
                {
                    new Cmf.Foundation.BusinessObjects.QueryObject.Filter()
                    {
                        Name = "Name",
                        Operator = FieldOperator.IsEqualTo,
                        Value = tableName
                    }
                }
            });

            // Check if the generic table was found
            if (output != null && output.GenericTableCollection != null && output.GenericTableCollection.Count > 0)
            {
                IGenericTable genericTable = output.GenericTableCollection.First();

                // Load the properties of the generic table
                genericTable.LoadProperties();

                IGenericTablePropertiesCollection properties = serviceProvider.GetService<IGenericTablePropertiesCollection>();

                foreach (dynamic propertyToAdd in propertiesToAdd)
                {
                    if (genericTable.GenericTableProperties.Any(pk => pk.Name == propertyToAdd.Name))
                    {
                        continue;
                    }
     
                    // Get all the properties that are from the position forward so that we can update its order
                    foreach(IGenericTableProperty propertyToUpdate in genericTable.GenericTableProperties.Where(pk => pk.Position >= propertyToAdd.Position))
                    {
                        IGenericTableProperty property = properties.FirstOrDefault(f => f.Name == propertyToUpdate.Name);

                        if (property == null)
                        {
                            propertyToUpdate.Position = propertyToUpdate.Position + 1;
                            properties.Add(propertyToUpdate);
                        } else
                        {
                            properties.Remove(property);
                            property.Position = property.Position + 1;
                            properties.Add(property);
                        }
                    }

                    ScalarType nVarcharScalarType = new ScalarType();
                    nVarcharScalarType.Load(propertyToAdd.ScalarTypeName);

                    IGenericTableProperty add = serviceProvider.GetService<IGenericTableProperty>();
                    add.Name = propertyToAdd.Name;
                    add.Position = propertyToAdd.Position; // Property positions start on 0
                    add.Description = propertyToAdd.Description;
                    add.IsKey = propertyToAdd.IsKey;
                    add.IsIndexed = propertyToAdd.IsIndexed;
                    add.IsMandatory = propertyToAdd.IsMandatory;
                    add.ReferenceType = propertyToAdd.ReferenceType;
                    add.ScalarType = nVarcharScalarType;
                    add.Size = propertyToAdd.Size;
                    properties.Add(add);
                }

                // Update properties of the Generic Table
                FullUpdateGenericTableOutput fullUpdateGenericTableOutput = _tableOrchestration.FullUpdateGenericTable(new FullUpdateGenericTableInput()
                {
                    GenericTable = genericTable,
                    PropertiesToAddOrUpdate = properties
                });

                // Generate the generic table schema
                _tableOrchestration.GenerateGenericTableSchema(new GenerateGenericTableSchemaInput()
                {
                    GenericTable = fullUpdateGenericTableOutput.GenericTable
                });
            }

            //---End DEE Code---

            return Input;
        }
    }
}
