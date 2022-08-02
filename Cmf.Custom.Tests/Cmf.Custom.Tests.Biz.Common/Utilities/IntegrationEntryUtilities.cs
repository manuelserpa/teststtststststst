using System;
using System.Data;
using System.IO;
using System.Xml;
using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.ErpManagement.OutputObjects;

namespace Cmf.Custom.Tests.Biz.Common.Utilities
{
    public static class IntegrationEntryUtilities
    {
        /// <summary>
		/// Returns the first Integration Entry created after a given DateTime
		/// </summary>
        /// <param name="fromDate">From date</param>
        /// <param name="messageType">Message type</param>
        /// <param name="includeTerminated">Consider Terminated integration entries</param>
        /// <param name="sourceSystem">The Source System. Default: MES </param>
        /// <param name="targetSystem">The Target System. Default: OntoFDC </param>
		public static IntegrationEntry GetIntegrationEntry(string messageType, DateTime fromDate, bool includeTerminated, string sourceSystem = "", string targetSystem = "")
        {
            IntegrationEntry integrationEntry = null;
            // Remove miliseconds due to comparison between datetime from .NET and datetime returned from database
            DateTime fromDateWithoutMiliseconds = new DateTime(fromDate.Ticks - (fromDate.Ticks % TimeSpan.TicksPerSecond), fromDate.Kind);

            FilterCollection filters = new FilterCollection()
            {
                new Filter()
                {
                    Name = AMSOsramConstants.SourceSystem,
                    Value = string.IsNullOrWhiteSpace(sourceSystem)?AMSOsramConstants.SourceSystem_OntoFDC:sourceSystem,
                    LogicalOperator = Foundation.Common.LogicalOperator.AND
                },
                new Filter()
                {
                    Name = AMSOsramConstants.TargetSystem,
                    Value = string.IsNullOrWhiteSpace(targetSystem)?AMSOsramConstants.TargetSystem_OntoFDC: targetSystem,
                    LogicalOperator = Foundation.Common.LogicalOperator.AND
                },
                new Filter()
                {
                    Name = AMSOsramConstants.MessageType,
                    Value = messageType,
                    LogicalOperator = Foundation.Common.LogicalOperator.AND
                },
                new Filter()
                {
                    Name =  "CreatedOn",
                    Value = fromDateWithoutMiliseconds,
                    Operator = Foundation.Common.FieldOperator.GreaterThanOrEqualTo,
                    LogicalOperator = Foundation.Common.LogicalOperator.Nothing
                }
            };

            // Get the Integration Entry
            GetIntegrationEntriesOutput integrationEntryCreated = new GetIntegrationEntriesInput()
            {
                Filters = filters,
                IncludeTerminated = includeTerminated                
            }.GetIntegrationEntriesSync();

            DataSet dataset = new DataSet();
            dataset.ReadXml(new XmlTextReader(new StringReader(integrationEntryCreated.Entries.DataXML)));

            if (dataset.Tables != null && dataset.Tables.Count > 0 && dataset.Tables[0].Rows != null && dataset.Tables[0].Rows.Count > 0)
            {
                integrationEntry = new IntegrationEntry()
                {
                    Name = dataset.Tables[0].Rows[0]["Name"].ToString()
                };
            }

            return integrationEntry;
        }
    }
}
