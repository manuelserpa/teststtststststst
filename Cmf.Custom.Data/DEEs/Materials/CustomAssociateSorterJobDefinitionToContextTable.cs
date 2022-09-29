using System.Collections.Generic;
using System.Data;
using Cmf.Custom.amsOSRAM.Common;
using Cmf.Custom.amsOSRAM.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.Common;
using Cmf.Foundation.BusinessObjects.Abstractions;
using System;
using Cmf.Foundation.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessOrchestration.Abstractions;

namespace Cmf.Custom.amsOSRAM.Actions.Materials
{
    class CustomAssociateSorterJobDefinitionToContextTable : DeeDevBase
    {
        public override bool DeeTestCondition(Dictionary<string, object> Input)
        {
            //---Start DEE Condition Code---

            #region Info

            /* Description:
             *     Dee action to associate the Custom Sorter Job Definition with the context table.
             *
             * Action Groups:
             *
            */

            #endregion

            #region Validate input

            if (!Input.ContainsKey("SourceCarrierType"))
            {
                throw new ArgumentNullCmfException("SourceCarrierType");
            }
            else if (!Input.ContainsKey("TargetCarrierType"))
            {
                throw new ArgumentNullCmfException("TargetCarrierType");
            }
            else if (!Input.ContainsKey("LogisticalProcess"))
            {
                throw new ArgumentNullCmfException("LogisticalProcess");
            }
            else if (!Input.ContainsKey("FlipWafer"))
            {
                throw new ArgumentNullCmfException("FlipWafer");
            }
            else if (!Input.ContainsKey("ReadWaferId"))
            {
                throw new ArgumentNullCmfException("ReadWaferId");
            }
            else if (!Input.ContainsKey("WaferIdOnBottom"))
            {
                throw new ArgumentNullCmfException("WaferIdOnBottom");
            }
            else if (!Input.ContainsKey("MovementList"))
            {
                throw new ArgumentNullCmfException("MovementList");
            }

            #endregion

            return true;
            //---End DEE Condition Code---
        }

        public override Dictionary<string, object> DeeActionCode(Dictionary<string, object> Input)
        {
            //---Start DEE Code---

            // Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.SmartTables");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "Cmf.Foundation.BusinessOrchestration.Abstractions");
            
            // Custom
            UseReference("Cmf.Custom.amsOSRAM.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.amsOSRAM.BusinessObjects.Abstractions");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common");
            UseReference("Cmf.Custom.amsOSRAM.Common.dll", "Cmf.Custom.amsOSRAM.Common.Extensions");
            
            // System
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
            UseReference("%MicrosoftNetPath%System.Data.Common.dll", "System.Data");

            // Get services provider information
            IServiceProvider serviceProvider = (IServiceProvider)Input["ServiceProvider"];
            IEntityFactory entityFactory = serviceProvider.GetService<IEntityFactory>();
            IGenericServiceOrchestration genericServiceOrchestration = serviceProvider.GetService<IGenericServiceOrchestration>();

            // Create Custom Sorter Job Definition
            ICustomSorterJobDefinition customSorterJobDefinition = entityFactory.Create<ICustomSorterJobDefinition>();
            customSorterJobDefinition.SourceCarrierType = Input["SourceCarrierType"].ToString();
            customSorterJobDefinition.TargetCarrierType = Input["TargetCarrierType"].ToString();
            customSorterJobDefinition.LogisticalProcess = Input["LogisticalProcess"].ToString();
            customSorterJobDefinition.FlipWafer = (bool)Input["FlipWafer"];
            customSorterJobDefinition.ReadWaferId = (bool)Input["ReadWaferId"];
            customSorterJobDefinition.WaferIdOnBottom = (bool)Input["WaferIdOnBottom"];
            customSorterJobDefinition.MovementList = Input["MovementList"].ToString();

            CreateObjectInput createObjectInput = new CreateObjectInput();
            createObjectInput.Object = customSorterJobDefinition;

            ICustomSorterJobDefinition createdSortedJobDefinition = genericServiceOrchestration.CreateObject(createObjectInput).Object as ICustomSorterJobDefinition;

            // Add Custom Sorter Job Definition to context table
            string smartTableName = amsOSRAMConstants.CustomSorterJobDefinitionContextName;
            ISmartTable smartTable = new SmartTable() { Name = smartTableName };
            smartTable.Load();

            DataSet dataSet = smartTable.GetEmptyTableDataSet();

            DataRow customSorterJobDefinitionContextRow = dataSet.Tables[0].NewRow();
            customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnStep] = Input["Step"].ToString();
            customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnProduct] = null;
            customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnProductGroup] = null;
            customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnFlow] = null;
            customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnMaterial] = Input["Lot"].ToString();
            customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnMaterialType] = null;
            customSorterJobDefinitionContextRow[amsOSRAMConstants.CustomSorterJobDefinitionContextColumnCustomSorterJobDefinition] = createdSortedJobDefinition.Name;

            dataSet.Tables[0].Rows.Add(customSorterJobDefinitionContextRow);
            dataSet.AcceptChanges();

            smartTable.InsertOrUpdateRows(NgpDataSet.FromDataSet(dataSet));

            //---End DEE Code---

            return Input;
        }
    }
}
