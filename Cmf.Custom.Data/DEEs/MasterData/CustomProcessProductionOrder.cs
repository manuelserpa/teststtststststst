using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using Cmf.Navigo.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Foundation.Common.Abstractions;
using Cmf.Navigo.BusinessObjects;

namespace Cmf.Custom.amsOSRAM.Actions.Integrations
{
    public class CustomProcessProductionOrder : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            #region Info

            /// <summary>
            /// Summary text
            ///     - DEE Action to Create or Update a Production Order.
            /// Action Groups:
            /// Depends On:
            /// Is Dependency For:
            /// Exceptions:
            /// </summary>

            #endregion

            //System
            UseReference("", "System.Text");

            //Custom
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.DataStructures");

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();

            // Load Integration Entry
            IIntegrationEntry integrationEntry = amsOSRAMUtilities.GetInputItem<IIntegrationEntry>(Input, "IntegrationEntry");

            // Cast Integation Entry Message to string
            string message = Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            CustomImportProductionOrder customImportProductionOrder = amsOSRAMUtilities.DeserializeXmlToObject<CustomImportProductionOrder>(message);

            string name = !string.IsNullOrEmpty(customImportProductionOrder.Name) ? customImportProductionOrder.Name : customImportProductionOrder.OrderNumber;

            IProductionOrder productionOrder = entityFactory.Create<IProductionOrder>();

            productionOrder.Name = name;

            if (productionOrder.ObjectExists())
            {
                productionOrder.Load();
            }

            if (!string.IsNullOrEmpty(customImportProductionOrder.Type))
            {
                productionOrder.Type = customImportProductionOrder.Type;
            }

            if (!string.IsNullOrEmpty(customImportProductionOrder.OrderNumber))
            {
                productionOrder.OrderNumber = customImportProductionOrder.OrderNumber;
            }

            if (!string.IsNullOrEmpty(customImportProductionOrder.Facility))
            {
                IFacility facility = entityFactory.Create<IFacility>();
                facility.Name = customImportProductionOrder.Facility;
                facility.Load();

                productionOrder.Facility = facility;
            }

            if (!string.IsNullOrEmpty(customImportProductionOrder.Product))
            {
                IProduct product = entityFactory.Create<IProduct>();
                product.Name = customImportProductionOrder.Product;
                product.Load();

                productionOrder.Product = product;
            }

            if (customImportProductionOrder.Quantity.HasValue)
            {
                productionOrder.Quantity = customImportProductionOrder.Quantity.Value;
            }

            if (!string.IsNullOrEmpty(customImportProductionOrder.Units))
            {
                productionOrder.Units = customImportProductionOrder.Units;
            }

            if (!string.IsNullOrEmpty(customImportProductionOrder.DueDate))
            {
                productionOrder.DueDate = amsOSRAMUtilities.GetValueAsDateTime(customImportProductionOrder.DueDate);
            }

            if (!string.IsNullOrEmpty(customImportProductionOrder.RestrictOnComplete))
            {
                productionOrder.RestrictOnComplete = amsOSRAMUtilities.GetValueAsNullableBoolean(customImportProductionOrder.RestrictOnComplete);
            }

            if (customImportProductionOrder.UnderDeliveryTolerance.HasValue)
            {
                productionOrder.UnderDeliveryTolerance = customImportProductionOrder.UnderDeliveryTolerance.Value;
            }

            if (customImportProductionOrder.OverDeliveryTolerance.HasValue)
            {
                productionOrder.OverDeliveryTolerance = customImportProductionOrder.OverDeliveryTolerance.Value;
            }

            if (!string.IsNullOrEmpty(customImportProductionOrder.PlannedStartDate))
            {
                productionOrder.PlannedStartDate = amsOSRAMUtilities.GetValueAsDateTime(customImportProductionOrder.PlannedStartDate);
            }

            if (!string.IsNullOrEmpty(customImportProductionOrder.PlannedEndDate))
            {
                productionOrder.PlannedEndDate = amsOSRAMUtilities.GetValueAsDateTime(customImportProductionOrder.PlannedEndDate);
            }

            productionOrder.SystemState = ProductionOrderSystemState.Released;

            if (!string.IsNullOrEmpty(customImportProductionOrder.SystemState))
            {
                productionOrder.SystemState = (ProductionOrderSystemState)Enum.Parse(typeof(ProductionOrderSystemState), customImportProductionOrder.SystemState);
            }

            if (productionOrder.ObjectExists())
            {
                productionOrder.Save();
            }
            else
            {
                productionOrder.Create();
            }

            //---End DEE Code---

            return Input;
        }

        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---   

            #region Info
            /// <summary>
            /// Creates or Updates a Production Order From ERP.
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
    }
}
