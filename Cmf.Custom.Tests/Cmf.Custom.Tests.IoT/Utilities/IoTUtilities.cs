using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessObjects.ConnectIoT;
using Cmf.Foundation.BusinessObjects.QueryObject;
using System.Data;

namespace Cmf.Custom.Tests.IoT.Utilities
{
	/// <summary>
	/// IoT utilities class
	/// </summary>
	public static class IoTUtilities
	{
		/// <summary>
		/// Retrieves a list of automation jobs according to its system state.
		/// By default its state is "Executing"
		/// </summary>
		/// <param name="systemState">The automation job system state</param>
		/// <returns>The automation jobs</returns>
		public static AutomationJobCollection GetActiveAutomationJobs(AutomationJobSystemState systemState = AutomationJobSystemState.Executing)
		{
			AutomationJobCollection jobs = new AutomationJobCollection();

			QueryObject query = new QueryObject
			{
				Description = "",
				EntityTypeName = "AutomationJob",
				Name = "CustomAutomationJobs",
				Query = new Query
				{
					Distinct = false,
					Filters = new FilterCollection() {
						new Filter()
						{
							Name = "SystemState",
							ObjectName = "AutomationJob",
							ObjectAlias = "AutomationJob_1",
							Operator = Foundation.Common.FieldOperator.IsEqualTo,
							Value = systemState,
							LogicalOperator = Foundation.Common.LogicalOperator.Nothing,
							FilterType = Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
						}
					},
					Fields = new FieldCollection() {
						new Field()
						{
							Alias = "Id",
							ObjectName = "AutomationJob",
							ObjectAlias = "AutomationJob_1",
							IsUserAttribute = false,
							Name = "Id",
							Position = 0,
							Sort = Foundation.Common.FieldSort.NoSort
						},
						new Field()
						{
							Alias = "Name",
							ObjectName = "AutomationJob",
							ObjectAlias = "AutomationJob_1",
							IsUserAttribute = false,
							Name = "Name",
							Position = 1,
							Sort = Foundation.Common.FieldSort.NoSort
						},
						new Field()
						{
							Alias = "SystemState",
							ObjectName = "AutomationJob",
							ObjectAlias = "AutomationJob_1",
							IsUserAttribute = false,
							Name = "SystemState",
							Position = 3,
							Sort = Foundation.Common.FieldSort.NoSort
						},
						new Field()
						{
							Alias = "__cmf_html_AutomationController_Id",
							ObjectName = "AutomationController",
							ObjectAlias = "AutomationJob_AutomationController_2",
							IsUserAttribute = false,
							Name = "Id",
							Position = 2,
							Sort = Foundation.Common.FieldSort.NoSort
						},
						new Field()
						{
							Alias = "__cmf_html_AutomationController_Name",
							ObjectName = "AutomationController",
							ObjectAlias = "AutomationJob_AutomationController_2",
							IsUserAttribute = false,
							Name = "Name",
							Position = 5,
							Sort = Foundation.Common.FieldSort.NoSort
						}
					},
					Relations = new RelationCollection() {
						new Relation()
						{
							Alias = "",
							IsRelation = false,
							Name = "",
							SourceEntity = "AutomationJob",
							SourceEntityAlias = "AutomationJob_1",
							SourceJoinType = Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
							SourceProperty = "AutomationControllerId",
							TargetEntity = "AutomationController",
							TargetEntityAlias = "AutomationJob_AutomationController_2",
							TargetJoinType = Foundation.BusinessObjects.QueryObject.Enums.JoinType.InnerJoin,
							TargetProperty = "Id"
						}
					}
				}
			};

			var queryExecuteOutput = new Cmf.Foundation.BusinessOrchestration.QueryManagement.InputObjects.ExecuteQueryInput
			{
				QueryObject = query
			}.ExecuteQuerySync();

			DataSet dataSet = TestScenarios.Others.Utilities.ToDataSet(queryExecuteOutput.NgpDataSet);

			if (Generalization.HasData(dataSet))
			{
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					AutomationJob job = new AutomationJob()
					{
						Id = (long)row["Id"]
					};

					job.Load();
					jobs.Add(job);
				}
			}

			return jobs;
		}
	}
}
