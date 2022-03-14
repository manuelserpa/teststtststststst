using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.QueryManagement.InputObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Cmf.Custom.Tests.Biz.Common.Utilities
{
    public static class CustomUtilities
    {
        /// <summary>
        /// Gets the Test Method Name
        /// </summary>
        /// <returns></returns>
        public static string GetTestMethodName()
        {
            // for when it runs via Visual Studio locally
            var stackTrace = new StackTrace();
            foreach (var stackFrame in stackTrace.GetFrames())
            {
                MethodBase methodBase = stackFrame.GetMethod();
                Object[] attributes = methodBase.GetCustomAttributes(typeof(TestMethodAttribute), false);
                if (attributes.Length >= 1)
                {
                    return methodBase.Name;
                }
            }
            return "Not called from a test method";
        }


        #region Integration Entries
        /// <summary>
        /// Dispatch Integration Entries
        /// </summary>
        /// <param name="integrationEntries"></param>
        public static void DispatchIntegrationEntries(IntegrationEntryCollection integrationEntries)
        {
            new DispatchIntegrationEntriesInput()
            {
                IntegrationEntries = integrationEntries
            }.DispatchIntegrationEntriesSync();
        } 

        public static void GetIntegrationEntry(string name)
        {
            QueryObject query = new QueryObject();
            query.Description = "";
            query.EntityTypeName = "IntegrationEntry";
            query.Name = "CustomGetIntegrationEntries";
            query.Query = new Query();
            query.Query.Distinct = true;
            query.Query.Filters = new FilterCollection() {
                new Filter()
                {
                    Name = "Name",
                    ObjectName = "IntegrationEntry",
                    ObjectAlias = "IntegrationEntry_1",
                    Operator = Cmf.Foundation.Common.FieldOperator.Contains,
                    Value = name,
                    LogicalOperator = Cmf.Foundation.Common.LogicalOperator.Nothing,
                    FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
                }
            };
            query.Query.Fields = new FieldCollection() {
                new Field()
                {
                    Alias = "Id",
                    ObjectName = "IntegrationEntry",
                    ObjectAlias = "IntegrationEntry_1",
                    IsUserAttribute = false,
                    Name = "Id",
                    Position = 0,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort
                },
                new Field()
                {
                    Alias = "Name",
                    ObjectName = "IntegrationEntry",
                    ObjectAlias = "IntegrationEntry_1",
                    IsUserAttribute = false,
                    Name = "Name",
                    Position = 1,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort
                }
            };
            query.Query.Relations = new RelationCollection();

            //DataSet dataSet = TestScenariosUtilities.ToDataSet(CustomUtilities.ExecuteQueryObject(query));
            //if (dataSet.HasData())
            //{
            //    foreach (DataRow row in dataSet.Tables[0].Rows)
            //    {
                    
            //    }
            //}

        }

        #endregion

        #region Query Object

        /// <summary>
        /// Executes the query object.
        /// </summary>
        /// <param name="queryObject">The query object.</param>
        /// <param name="queryParameters">The query parameters.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <returns></returns>
        public static NgpDataSet ExecuteQueryObject(QueryObject queryObject, QueryParameterCollection queryParameters = null, int pageSize = 10, int pageNumber = 1)
        {
            return new ExecuteQueryInput()
            {
                QueryObject = queryObject,
                PageSize = pageSize,
                PageNumber = pageNumber,
                QueryParameters = queryParameters
            }.ExecuteQuerySync().NgpDataSet;
        }

        #endregion
    }
}
