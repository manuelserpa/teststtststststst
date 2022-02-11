using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects.QueryObject;
using Cmf.Navigo.BusinessObjects;
using System.Data;

namespace Cmf.Custom.Tests.Biz.Common.Utilities
{
    /// <summary>
    /// Notification utilities
    /// </summary>
	public static class NotificationUtilities
	{
        /// <summary>
        /// Clears the given notification from the MES
        /// </summary>
        /// <param name="notifications">The notification to clear</param>
        public static void ClearNotifications(NotificationCollection notifications)
        {
            foreach (Notification notification in notifications)
            {
                notification.Load();
                notification.Terminate();
            }
        }

        /// <summary>
        /// Clears all active notifications
        /// </summary>
        public static void ClearAllNotifications()
		{
            ClearNotifications(GetActiveNotifications());
        }

        /// <summary>
        /// Gets all active notifications
        /// </summary>
        /// <returns>The notifications</returns>
        public static NotificationCollection GetActiveNotifications()
        {
            QueryObject query = new QueryObject();
            query.Description = "";
            query.EntityTypeName = "Notification";
            query.Name = "Notification";
            query.Query = new Query();
            query.Query.Distinct = false;
            query.Query.Filters = new FilterCollection() {
                new Filter()
                {
                    Name = "IsTemplate",
                    ObjectName = "Notification",
                    ObjectAlias = "Notification_1",
                    Operator = Cmf.Foundation.Common.FieldOperator.IsEqualTo,
                    Value = false,
                    LogicalOperator = Cmf.Foundation.Common.LogicalOperator.AND,
                    FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
                },
                new Filter()
                {
                    Name = "UniversalState",
                    ObjectName = "Notification",
                    ObjectAlias = "Notification_1",
                    Operator = Cmf.Foundation.Common.FieldOperator.IsEqualTo,
                    Value = 2,
                    LogicalOperator = Cmf.Foundation.Common.LogicalOperator.Nothing,
                    FilterType = Cmf.Foundation.BusinessObjects.QueryObject.Enums.FilterType.Normal,
                }
            };
            query.Query.Fields = new FieldCollection() {
                new Field()
                {
                    Alias = "Id",
                    ObjectName = "Notification",
                    ObjectAlias = "Notification_1",
                    IsUserAttribute = false,
                    Name = "Id",
                    Position = 0,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort
                },
                new Field()
                {
                    Alias = "Name",
                    ObjectName = "Notification",
                    ObjectAlias = "Notification_1",
                    IsUserAttribute = false,
                    Name = "Name",
                    Position = 1,
                    Sort = Cmf.Foundation.Common.FieldSort.NoSort
                }
            };
            query.Query.Relations = new RelationCollection();


            var queryExecuteOutput = new Cmf.Foundation.BusinessOrchestration.QueryManagement.InputObjects.ExecuteQueryInput
            {
                QueryObject = query
            }.ExecuteQuerySync();


            System.Data.DataSet dataSet = Cmf.TestScenarios.Others.Utilities.ToDataSet(queryExecuteOutput.NgpDataSet);
            NotificationCollection notifications = new NotificationCollection();

            if (Generalization.HasData(dataSet))
            {

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    Notification notification = new Notification()
                    {
                        Name = (string)row["Name"]
                    };
                    notification.Load();
                    notifications.Add(notification);
                }
            }
            return notifications;
        }
    }
}
