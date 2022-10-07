using System;
using System.Collections.Generic;
using System.Data;
using Cmf.Foundation.BusinessObjects.Abstractions;

namespace Cmf.Custom.amsOSRAM.Common.Extensions
{
    public static class TableExtensions
    {
        /// <summary>
        /// Auxiliar Method that Creates an empty DataSet from a dictionary with table properties
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableProperties"></param>
        /// <returns></returns>
        private static DataSet CreateEmptyDataSet(string tableName, Dictionary<string, Type> tableProperties)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            if (tableProperties == null || tableProperties.Count <= 0)
            {
                throw new ArgumentException("Argument must have at least one value", "tableProperties");
            }

            DataTable emptyDataTable = new DataTable
            {
                TableName = tableName,
                CaseSensitive = false
            };

            foreach (KeyValuePair<string, Type> tableProperty in tableProperties)
            {
                emptyDataTable.Columns.Add(new DataColumn
                {
                    ColumnName = tableProperty.Key,
                    DataType = tableProperty.Value
                });
            }

            DataSet emptyDataSet = new DataSet();
            emptyDataSet.Tables.Add(emptyDataTable);

            return emptyDataSet;
        }

        /// <summary>
        /// Extension Method that returns an empty DataSet for a SmartTable
        /// </summary>
        /// <param name="smartTable"></param>
        /// <param name="includeTableIdColumns"></param>
        /// <returns></returns>
        public static DataSet GetEmptyTableDataSet(this ISmartTable smartTable, bool includeTableIdColumns = true)
        {
            if (smartTable == null)
            {
                throw new ArgumentNullException("smartTable");
            }

            Dictionary<string, Type> tableProperties = new Dictionary<string, Type>();

            if (includeTableIdColumns)
            {
                tableProperties.Add(string.Format("{0}Id", smartTable.Name), typeof(Int64));
                tableProperties.Add(Cmf.Foundation.Common.Constants.LastServiceHistoryId, typeof(Int64));
                tableProperties.Add(Cmf.Foundation.Common.Constants.LastOperationHistorySeq, typeof(Int64));
            }

            foreach (ISmartTableProperty smartTableProperty in smartTable.SmartTableProperties)
            {
                if (!tableProperties.ContainsKey(smartTableProperty.Name))
                {
                    tableProperties.Add(smartTableProperty.Name, Type.GetType(smartTableProperty.ScalarType.NativeType));
                }
            }

            return CreateEmptyDataSet(smartTable.Name, tableProperties);
        }
    }
}
