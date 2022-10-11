using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessOrchestration.Abstractions;
using Cmf.Foundation.BusinessOrchestration.EntityTypeManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cmf.Custom.amsOSRAM.Actions.ProcessRules.EntityTypes
{
    public class CustomFacilityAttributes : DeeDevBase
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

            return true;

            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //Foundation
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.EntityTypeManagement.InputObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.Abstractions");

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            //SourceEntity Facility
            IEntityType facilityEntity = entityFactory.Create<IEntityType>();
            facilityEntity.Load("Facility");

            if (facilityEntity.ObjectExists())
            {
                facilityEntity.Load();

                facilityEntity.LoadAllProperties();

                if (!facilityEntity.Properties.Any(p => p.Name.Equals("FacilityCode", StringComparison.InvariantCultureIgnoreCase)))
                {
                    IScalarType nVarCharScalarType = entityFactory.Create<IScalarType>();
                    nVarCharScalarType.Load("NVarChar");

                    IEntityTypeProperty entityTypeProperty = entityFactory.Create<IEntityTypeProperty>();
                    entityTypeProperty.Name = "FacilityCode";
                    entityTypeProperty.Description = "Facility Code";
                    entityTypeProperty.PropertyType = EntityTypePropertyType.Attribute;
                    entityTypeProperty.ReferenceType = ReferenceType.None;
                    entityTypeProperty.ScalarType = nVarCharScalarType;
                    entityTypeProperty.IsEnabled = true;
                    entityTypeProperty.IsIndexed = false;
                    entityTypeProperty.IsMandatory = false;
                    entityTypeProperty.IsHistoryEnable = true;
                    entityTypeProperty.LoadToDWH = true;

                    IEntityTypePropertyCollection attributesToAdd = entityFactory.CreateCollection<IEntityTypePropertyCollection>();
                    attributesToAdd.Add(entityTypeProperty);

                    IEntityTypeOrchestration entityTypeOrchestration = serviceProvider.GetService<IEntityTypeOrchestration>();

                    facilityEntity = entityTypeOrchestration.AddEntityTypeProperties(new AddEntityTypePropertiesInput
                    {
                        EntityType = facilityEntity,
                        EntityTypeProperties = attributesToAdd
                    }).EntityType;

                    entityTypeOrchestration.GenerateEntityTypeDBSchema(new GenerateEntityTypeDBSchemaInput
                    {
                        EntityType = facilityEntity
                    });
                }
            }

            //---End DEE Code---

            return Input;
        }
    }
}
