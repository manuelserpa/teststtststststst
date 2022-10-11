using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.Abstractions;
using Cmf.Foundation.BusinessObjects.SmartTables;
using System.Collections.Generic;
using System.Data;

namespace Cmf.Custom.amsOSRAM.Actions.ProcessRules._2._0._0.After
{
    internal class CustomUpdateIntegrationHandlerResolutionSmartTable : DeeDevBase
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

            //System
            UseReference("%MicrosoftNetPath%System.Data.Common.dll", "System.Data");

            //Foundation
            UseReference("Cmf.Foundation.BusinessObjects.dll", "Cmf.Foundation.BusinessObjects.SmartTables");


            // Load Smart Table
            ISmartTable integrationHandlerResolution = new SmartTable() { Name = "IntegrationHandlerResolution"};

            integrationHandlerResolution.Load();
            integrationHandlerResolution.LoadData();

            // Get the rwo we want to modifí from the Smart Table
            INgpDataRow resolveRow = new NgpDataRow
            {
                { "FromSystem", "MES" },
                { "ToSystem", "ERP" },
                { "MessageType", "CustomPerformConsumption" }
            };

            INgpDataSet resolveDataSet = integrationHandlerResolution.Resolve(resolveRow, false);
            DataSet dataSet = NgpDataSet.ToDataSet(resolveDataSet);

            // Modify row
            dataSet.Tables[0].Rows[0]["ActionName"] = "CustomSendProcessMessage";
            dataSet.AcceptChanges();

            // Update the Smart Table
            integrationHandlerResolution.InsertOrUpdateRows(NgpDataSet.FromDataSet(dataSet));

            //---End DEE Code---

            return Input;
        }
    }
}
