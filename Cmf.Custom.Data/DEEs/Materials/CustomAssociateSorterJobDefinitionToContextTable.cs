using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Cmf.Custom.AMSOsram.BusinessObjects;
using Cmf.Custom.AMSOsram.Common;
using Cmf.Custom.AMSOsram.Common.Extensions;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.SmartTables;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement;
using Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects;
using Cmf.Foundation.Common;

namespace Cmf.Custom.AMSOsram.Actions.Materials
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
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects");
            UseReference("Cmf.Foundation.BusinessOrchestration.dll", "");
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.SmartTables");
            UseReference("", "Cmf.Foundation.Common");
            UseReference("", "Cmf.Foundation.BusinessObjects.Cultures");
            UseReference("", "Cmf.Foundation.BusinessOrchestration.GenericServiceManagement");
            UseReference("", "Cmf.Foundation.BusinessOrchestration.GenericServiceManagement.InputObjects");
            // Navigo
            UseReference("Cmf.Navigo.BusinessObjects.dll", "Cmf.Navigo.BusinessObjects");
            // Custom
            UseReference("Cmf.Custom.AMSOsram.BusinessObjects.CustomSorterJobDefinition.dll", "Cmf.Custom.AMSOsram.BusinessObjects");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common");
            UseReference("Cmf.Custom.AMSOsram.Common.dll", "Cmf.Custom.AMSOsram.Common.Extensions");
            // System
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json");
            UseReference("Newtonsoft.Json.dll", "Newtonsoft.Json.Linq");
            UseReference("%MicrosoftNetPath%System.Data.Common.dll", "System.Data");

            // Create Custom Sorter Job Definition
            CustomSorterJobDefinition customSorterJobDefinition = new CustomSorterJobDefinition
            {
                SourceCarrierType = Input["SourceCarrierType"].ToString(),
                TargetCarrierType = Input["TargetCarrierType"].ToString(),
                LogisticalProcess = Input["LogisticalProcess"].ToString(),
                FlipWafer = (bool)Input["FlipWafer"],
                ReadWaferId = (bool)Input["ReadWaferId"],
                WaferIdOnBottom = (bool)Input["WaferIdOnBottom"],
                MovementList = Input["MovementList"].ToString()
            };

            CreateObjectInput createObjectInput = new CreateObjectInput();
            createObjectInput.Object = customSorterJobDefinition;

            CustomSorterJobDefinition createdSortedJobDefinition = GenericServiceManagementOrchestration.CreateObject(createObjectInput).Object as CustomSorterJobDefinition;

            // Add Custom Sorter Job Definition to context table
            string smartTableName = AMSOsramConstants.CustomSorterJobDefinitionContextName;
            SmartTable smartTable = new SmartTable() { Name = smartTableName };
            smartTable.Load();

            DataSet dataSet = smartTable.GetEmptyTableDataSet();

            DataRow customSorterJobDefinitionContextRow = dataSet.Tables[0].NewRow();
            customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnStep] = Input["Step"].ToString();
            customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnProduct] = null;
            customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnProductGroup] = null;
            customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnFlow] = null;
            customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnMaterial] = Input["Lot"].ToString();
            customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnMaterialType] = null;
            customSorterJobDefinitionContextRow[AMSOsramConstants.CustomSorterJobDefinitionContextColumnCustomSorterJobDefinition] = createdSortedJobDefinition.Name;

            dataSet.Tables[0].Rows.Add(customSorterJobDefinitionContextRow);
            dataSet.AcceptChanges();

            smartTable.InsertOrUpdateRows(NgpDataSet.FromDataSet(dataSet));

            //---End DEE Code---

            return Input;
        }
    }
}
