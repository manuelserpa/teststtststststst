using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.DataStructures;
using Cmf.Foundation.BusinessObjects;
using Cmf.Navigo.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cmf.Custom.AMSOsram.Actions.Integrations
{
    public class CustomProcessProductionOrder : DeeDevBase
    {
        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---     

            //System
            UseReference("", "System.Text");

            //Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");

            //Custom
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.DataStructures");

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

            // Load Integration Entry
            IntegrationEntry integrationEntry = AMSOsramUtilities.GetInputItem<IntegrationEntry>(Input, "IntegrationEntry");

            // Cast Integation Entry Message to string
            string message = Encoding.UTF8.GetString(integrationEntry.IntegrationMessage.Message);

            CustomImportProductionOrder customImportProductionOrder = AMSOsramUtilities.DeserializeXmlToObject<CustomImportProductionOrder>(message);

            string name = !string.IsNullOrEmpty(customImportProductionOrder.Name) ? customImportProductionOrder.Name : customImportProductionOrder.OrderNumber;

            ProductionOrder productionOrder = new ProductionOrder();

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
                Facility facility = new Facility() { Name = customImportProductionOrder.Facility };
                facility.Load();

                productionOrder.Facility = facility;
            }

            if (!string.IsNullOrEmpty(customImportProductionOrder.Product))
            {
                Product product = new Product() { Name = customImportProductionOrder.Product };
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
                productionOrder.DueDate = AMSOsramUtilities.GetValueAsDateTime(customImportProductionOrder.DueDate);
            }

            if (!string.IsNullOrEmpty(customImportProductionOrder.RestrictOnComplete))
            {
                productionOrder.RestrictOnComplete = AMSOsramUtilities.GetValueAsNullableBoolean(customImportProductionOrder.RestrictOnComplete);
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
                productionOrder.PlannedStartDate = AMSOsramUtilities.GetValueAsDateTime(customImportProductionOrder.PlannedStartDate);
            }

            if (!string.IsNullOrEmpty(customImportProductionOrder.PlannedEndDate))
            {
                productionOrder.PlannedEndDate = AMSOsramUtilities.GetValueAsDateTime(customImportProductionOrder.PlannedEndDate);
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
